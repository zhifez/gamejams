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

  public class DataPackageHandler : Base {
    public static DataPackageHandler instance;

    public float minGenerateDataInterval = 2f;
    public float maxGenerateDataInterval = 4f;
    public float minResolvePendingInterval = 1f;
    public float maxResolvePendingInterval = 2f;
    public float dataTransmitDuration = 2f;
    public ServiceSignalPattern[] serviceSignalPatterns;

    private float timer;
    private float resolvePendingTimer = 0f;
    private List<DataPackage> pendingData;
    private List<DataPackage> activeData;
    private List<DataPackage> transmittedData;

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
          transmittedData.Add ( activeData[a] );
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
        if ( ssp.signalPatterns.Length != _signalPatterns.Length ) {
          continue;
        }

        float _stableDiff = 0.1f;
        for ( int a=0; a<ssp.signalPatterns.Length; ++a ) {
          SignalPattern sp = ssp.signalPatterns[a];
          
          for ( int b=0; b<_tempSP.Count; ++b ) {
            SignalPattern spCompare = _tempSP[b];
            float _strengthDiff = Mathf.Abs ( sp.strength - spCompare.strength );
            float _speedDiff = Mathf.Abs ( sp.speed - spCompare.speed );
            if ( _strengthDiff <=_stableDiff
              && _speedDiff < _stableDiff ) { // stable enough
              _tempSP.RemoveAt ( b );
              break;
            }
          }
        }

        if ( _tempSP.Count <= 0 ) {
          return new ServiceStatus ( ssp.service, true );
        }
        else if ( ( float ) _tempSP.Count / ( float ) ssp.signalPatterns.Length <= 0.4f ) {
          return new ServiceStatus ( ssp.service, false );
        }
      }
      return null;
    }

    //--------------------------------------------------
    // protected
    //--------------------------------------------------
    protected void Awake () {
      instance = this;
      
      pendingData = new List<DataPackage> ();
      activeData = new List<DataPackage> ();
      transmittedData = new List<DataPackage> ();

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
        pendingData.Add ( new DataPackage (
          ( DataPackage.Service ) Random.Range ( 0, _serviceNames.Length ),
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