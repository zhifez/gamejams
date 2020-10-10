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

  public class DataPackageHandler : Base {
    public static DataPackageHandler instance;

    public float minGenerateDataInterval = 2f;
    public float maxGenerateDataInterval = 4f;
    public float minResolvePendingInterval = 1f;
    public float maxResolvePendingInterval = 2f;
    public float dataTransmitDuration = 2f;

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
          if ( true ) { // TODO: Detect whether service is available
            activeData.Add ( pendingData[a] );
            pendingData.RemoveAt ( a );
            break;
          }
        }
      }
    }

    private void RunActiveData () {
      for ( int a=0; a<activeData.Count; ++a ) {
        activeData[a].transmitTimer += Time.deltaTime;
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

    //--------------------------------------------------
    // protected
    //--------------------------------------------------
    protected void Awake () {
      instance = this;
      
      pendingData = new List<DataPackage> ();
      activeData = new List<DataPackage> ();
      transmittedData = new List<DataPackage> ();

      // enabled = false;
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

      UI_MAIN_TRANSMISSION.UpdatePendingDataLabel ( pendingData );
      UI_MAIN_TRANSMISSION.UpdateActiveDataLabel ( activeData );
      UI_MAIN_TRANSMISSION.UpdateTransmittedDataLabel ( transmittedData );
    }
  }
}