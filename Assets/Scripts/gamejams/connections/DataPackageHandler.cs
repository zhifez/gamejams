using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.zhifez.seagj {
  [ System.Serializable ]
  public class DataPackage {
    public enum Service {
      unaffiliated, maxen, digify, celcular
    }
    public Service service;
    public enum Type {
      e_mail, phone_message, encrypted_data, malware
    }
    public Type type;
    public int size;
    public float transmitTimer;
    public float multiplier;

    public DataPackage ( Service service, Type type, int size ) {
      this.service = service;
      this.type = type;
      this.size = size;
    }
  }

  [ System.Serializable ]
  public class SignalPattern {
    public float strength;
		public float speed;

    public SignalPattern ( float strength, float speed ) {
      this.strength = strength;
      this.speed = speed;
    }
  }

	[ System.Serializable ]
	public class ServiceSignalPattern {
		public DataPackage.Service service;
    public int price;
    public int dailyRates;
    public int commission;
    public SignalPattern[] signalPatterns;
	}

  [ System.Serializable ]
  public class ServiceStatus {
		public DataPackage.Service service;
    public bool isStable;
    public string tmMachineId;

    public ServiceStatus ( DataPackage.Service service, bool isStable ) {
      this.service = service;
      this.isStable = isStable;
    }

    public bool Equals ( ServiceStatus compare ) {
      if ( compare == null ) {
        return false;
      }
      return ( this.service == compare.service 
        && this.isStable == compare.isStable
        && this.tmMachineId.Equals ( compare.tmMachineId ) );
    }
  }

  [ System.Serializable ]
  public class Result {
    public string result0;
    public string result1;
    public int totalEarnings;
  }

  [ System.Serializable ]
  public class PriceAndRates {
    public int price;
    public int dailyRates;
  }

  public class DataPackageHandler : Base {
    public static DataPackageHandler instance;

    public float signalAccuracy = 0.05f;
    public float minOverallSignalAccuracy = 0.4f;
    public float minGenerateDataInterval = 2f;
    public float maxGenerateDataInterval = 4f;
    public float minResolvePendingInterval = 1f;
    public float maxResolvePendingInterval = 2f;
    public float dataTransmitDuration = 2f;
    public float commissionMultiplier = 0.75f;
    public ServiceSignalPattern[] serviceSignalPatterns;
    public PriceAndRates tmMachineRates;
    public PriceAndRates satDishRates;

    private float timer;
    private float resolvePendingTimer = 0f;
    private List<DataPackage> pendingData;
    private List<DataPackage> activeData;
    private List<DataPackage> transmittedData;
    private List<DataPackage.Service> enabledServices;

    //--------------------------------------------------
    // private
    //--------------------------------------------------
    private void RunPendingData () {
      resolvePendingTimer -= Time.deltaTime;
      if ( resolvePendingTimer <= 0 ) {
        resolvePendingTimer = Random.Range ( minResolvePendingInterval, maxResolvePendingInterval );

        for ( int a=0; a<pendingData.Count; ++a ) {
          if ( GAME.HasService ( pendingData[a].service ) ) { // TODO: Detect whether service is available
            activeData.Add ( pendingData[a] );
            pendingData.RemoveAt ( a );
            break;
          }
        }
      }
    }

    private void RunActiveData () {
      for ( int a=0; a<activeData.Count; ++a ) {
        bool _hasService = GAME.HasService ( activeData[a].service );
        if ( !_hasService ) {
          activeData[a].transmitTimer = 0f;
          pendingData.Add ( activeData[a] );
          activeData.RemoveAt ( a );
          break;
        }

        float _serviceMultiplier = GAME.GetServiceMultipler ( activeData[a].service );
        activeData[a].transmitTimer += Time.deltaTime * _serviceMultiplier;
        float _duration = ( activeData[a].size + 1 ) * dataTransmitDuration;
        if( activeData[a].transmitTimer > _duration ) {
          activeData[a].multiplier = Mathf.Max ( 1f, _serviceMultipler * commissionMultiplier );
          transmittedData.Insert ( 0, activeData[a] );
          activeData.RemoveAt ( a );
          break;
        }
      }
    }

    //--------------------------------------------------
    // public
    //--------------------------------------------------
    public ServiceStatus GetServiceStatus ( SignalPattern[] _signalPatterns ) {
      foreach ( ServiceSignalPattern ssp in serviceSignalPatterns ) {
        List<SignalPattern> _tempSP = new List<SignalPattern> ();
        _tempSP.AddRange ( _signalPatterns );
        if ( ssp.signalPatterns.Length != _signalPatterns.Length
          || !enabledServices.Contains ( ssp.service ) ) {
          continue;
        }

        for ( int a=0; a<ssp.signalPatterns.Length; ++a ) {
          SignalPattern sp = ssp.signalPatterns[a];
          
          for ( int b=0; b<_tempSP.Count; ++b ) {
            SignalPattern spCompare = _tempSP[b];
            float _strengthDiff = Mathf.Abs ( sp.strength - spCompare.strength );
            float _speedDiff = Mathf.Abs ( sp.speed - spCompare.speed );
            if ( _strengthDiff <= signalAccuracy
              && _speedDiff < signalAccuracy ) { // stable enough
              _tempSP.RemoveAt ( b );
              break;
            }
          }
        }

        if ( _tempSP.Count <= 0 ) {
          return new ServiceStatus ( ssp.service, true );
        }
        else if ( ( float ) _tempSP.Count / ( float ) ssp.signalPatterns.Length <= minOverallSignalAccuracy ) {
          return new ServiceStatus ( ssp.service, false );
        }
      }
      return null;
    }

    public Result GetFinalResult () {
      Result _result = new Result ();
      _result.result0 = "<size=25><b>total data transmitted:</b> " + transmittedData.Count + "</size>";
      
      int[] _dataCount = new int[ serviceSignalPatterns.Length ];
      int[] _commissions = new int[ serviceSignalPatterns.Length ];
      _result.totalEarnings = 0;
      for ( int a=0; a<serviceSignalPatterns.Length; ++a ) {
        ServiceSignalPattern ssp = serviceSignalPatterns[a];
        int _totalDataSize = 0;
        foreach ( DataPackage data in transmittedData ) {
          if ( data.service == ssp.service ) {
            ++_dataCount[a];
            _totalDataSize += Mathf.RoundToInt ( data.size * data.multiplier );
          }
        }
        if ( _dataCount[a] > 0 ) {
          _result.result0 += "\n  " + ssp.service + ": " + _dataCount[a];
        }
        _commissions[a] = _dataCount[a] * ssp.commission * _totalDataSize;
        _result.totalEarnings += _commissions[a];
      }

      _result.result0 += "\n\n<size=25><b>total commissions:</b> $" + _result.totalEarnings + "</size>";

      for ( int a=0; a<serviceSignalPatterns.Length; ++a ) {
        if ( _commissions[a] > 0 ) {
          _result.result0 += "\n  " + serviceSignalPatterns[a].service + ": $" + _commissions[a];
        }
      }
      
      int _totalBills = 0;
      _totalBills -= GAME.enabledTmMachineCount * tmMachineRates.dailyRates;
      _totalBills -= GAME.enabledSatDishCount * satDishRates.dailyRates;
      string _billsResult = "\n  electric_bills: $" + _totalBills;
      if ( enabledServices != null ) {
        foreach ( DataPackage.Service es in enabledServices ) {
          if ( es == DataPackage.Service.unaffiliated ) {
            continue;
          }

          foreach ( ServiceSignalPattern ssp in serviceSignalPatterns ) {
            if ( ssp.service == es ) {
              _totalBills -= ssp.dailyRates;
              _billsResult += "\n  " + ssp.service + "_licensing_daily: $-" + ssp.dailyRates;
              break;
            }
          }
        }
      }

      _result.result0 += "\n\n<size=25><b>total bills:</b> $" + _totalBills + "</size>";
      _result.result0 += _billsResult;

      _result.totalEarnings += _totalBills;
      _result.result0 += "\n\n<size=25><b>total earnings:</b> $" + _result.totalEarnings + "</size>";

      _result.result1 = "<size=25><b>current funds:</b> $" + PLAYER_STATS.funds + "</size>";
      _result.result1 += "\n  + earnings: $" + _result.totalEarnings;

      PLAYER_STATS.funds += _result.totalEarnings;
      _result.result1 += "\n\n<size=25><b>total funds:</b> $" + ( PLAYER_STATS.funds ) + "</size>";
      return _result;
    }

    public bool ServiceIsEnabled ( DataPackage.Service _service ) {
      return ( enabledServices != null
        && enabledServices.Contains ( _service ) );
    }

    public void EnableService ( DataPackage.Service _service ) {
      if ( enabledServices == null ) {
        enabledServices = new List<DataPackage.Service> ();
      }

      if ( !enabledServices.Contains ( _service ) ) {
        enabledServices.Add ( _service );
      }
    }

    public void DisableService ( DataPackage.Service _service ) {
      if ( enabledServices != null ) {
        enabledServices.Remove ( _service );
      }
    }

    //--------------------------------------------------
    // protected
    //--------------------------------------------------
    protected void Awake () {
      instance = this;
      
      pendingData = new List<DataPackage> ();
      activeData = new List<DataPackage> ();
      transmittedData = new List<DataPackage> ();

      EnableService ( DataPackage.Service.unaffiliated );

      enabled = false;
    }

    protected void OnEnable () {
      timer = Random.Range ( minGenerateDataInterval, maxGenerateDataInterval );
      resolvePendingTimer = Random.Range ( minResolvePendingInterval, maxResolvePendingInterval );
      pendingData.Clear ();
      activeData.Clear ();
      transmittedData.Clear ();
    }

    protected void Update () {
      timer -= Time.deltaTime;
      if ( timer <= 0 ) {
        timer = Random.Range ( minGenerateDataInterval, maxGenerateDataInterval );

        string[] _serviceNames = System.Enum.GetNames ( typeof ( DataPackage.Service ) );
        string[] _typeNames = System.Enum.GetNames ( typeof ( DataPackage.Type ) );
        int _maxServiceTypes = _serviceNames.Length;
        if ( enabledServices.Count + 1 < _serviceNames.Length ) {
          _maxServiceTypes -= enabledServices.Count + 1;
        }
        pendingData.Add ( new DataPackage (
          ( DataPackage.Service ) Random.Range ( 0, _maxServiceTypes ),
          ( DataPackage.Type ) Random.Range ( 0, _typeNames.Length ),
          Random.Range ( 0, 3 )
        ) );
      }

      RunPendingData ();
      RunActiveData ();

      if ( UI_MAIN_TRANSMISSION != null ) {
        UI_MAIN_TRANSMISSION.UpdatePendingDataLabel ( pendingData );
        UI_MAIN_TRANSMISSION.UpdateActiveDataLabel ( activeData );
        UI_MAIN_TRANSMISSION.UpdateTransmittedDataLabel ( transmittedData );
      }
    }
  }
}