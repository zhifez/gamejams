using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace com.zhifez.seagj {
  public class UI_Main_Transmission : Base {
    public static UI_Main_Transmission instance;
    
    public Text servicesTitle;
    public Text servicesLabel;
    public Text pendingDataTitle;
    public Text pendingDataLabel;
    public Text activeDataTitle;
    public Text activeDataLabel;
    public Text transmittedDataTitle;
    public Text transmittedDataLabel;

    //--------------------------------------------------
    // private
    //--------------------------------------------------

    //--------------------------------------------------
    // public
    //--------------------------------------------------
    public void ClearLabels () {
      servicesLabel.text = "";
      pendingDataLabel.text = "";
      activeDataLabel.text = "";
      transmittedDataLabel.text = "";
    }

    public void UpdateServicesLabel ( List<ServiceStatus> serviceStatuses ) {
      string _content = "";
      int _count = 0;
      foreach ( ServiceStatus ss in serviceStatuses ) {
        if ( _count > 0 ) {
          _content += "\n";
        }
        ++_count;
        if ( ss.isStable ) {
          _content += ss.service.ToString ();
        }
        else {
          _content += "<color=#E83F6F>" + ss.service + " (unstable)</color>";
        }
      }
      servicesLabel.text = _content;
    }

    public void UpdatePendingDataLabel ( List<DataPackage> dataPackages ) {
      string _content = "";
      int _count = 0;
      foreach ( DataPackage data in dataPackages ) {
        if ( _count > 0 ) {
          _content += "\n";
        }
        ++_count;
        _content += "<color=#CBEF43>[" + data.service + "]</color> " + data.type + ", size: ";
        switch ( data.size ) {
        case 0:
          _content += "100kb";
          break;

        case 1:
          _content += "500kb";
          break;

        case 2:
          _content += "1mb";
          break;
        }
      }
      pendingDataTitle.text = "pending data (" + dataPackages.Count + ")";
      pendingDataLabel.text = _content;
    }

    public void UpdateActiveDataLabel ( List<DataPackage> dataPackages ) {
      string _content = "";
      int _count = 0;
      foreach ( DataPackage data in dataPackages ) {
        if ( _count > 0 ) {
          _content += "\n";
        }
        ++_count;
        _content += "<color=#CBEF43>[" + data.service + "]</color> " + data.type + ", size: ";
        switch ( data.size ) {
        case 0:
          _content += "100kb";
          break;

        case 1:
          _content += "500kb";
          break;

        case 2:
          _content += "1mb";
          break;
        }

        float _perc = data.transmitTimer / ( ( data.size + 1 ) * DATA_PACKAGE.dataTransmitDuration );
        _content += " (" + Mathf.RoundToInt ( _perc * 100f ) + "%)";
      }
      activeDataTitle.text = "active (" + dataPackages.Count + ")";
      activeDataLabel.text = _content;
    }

    public void UpdateTransmittedDataLabel ( List<DataPackage> dataPackages ) {
      string _content = "";
      int _count = 0;
      foreach ( DataPackage data in dataPackages ) {
        if ( _count > 0 ) {
          _content += "\n";
        }
        ++_count;
        _content += "<color=#CBEF43>[" + data.service + "]</color> " + data.type + ", size: ";
        switch ( data.size ) {
        case 0:
          _content += "100kb";
          break;

        case 1:
          _content += "500kb";
          break;

        case 2:
          _content += "1mb";
          break;
        }
      }
      transmittedDataTitle.text = "transmitted (" + dataPackages.Count + ")";
      transmittedDataLabel.text = _content;
    }

    //--------------------------------------------------
    // protected
    //--------------------------------------------------
    protected void Awake () {
      instance = this;
    }

    protected void Update () {
      if ( GAME == null ) {
        return;
      }

			UpdateServicesLabel ( GAME.serviceStatuses );
    }
  }
}