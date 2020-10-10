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

    public void UpdateServicesLabel ( List<DataPackage> dataPackages ) {
      // servicesLabel.text = content;
    }

    public void UpdatePendingDataLabel ( List<DataPackage> dataPackages ) {
      string _dataList = "";
      int _count = 0;
      foreach ( DataPackage data in dataPackages ) {
        if ( _count > 0 ) {
          _dataList += "\n";
        }
        ++_count;
        _dataList += "<color=#CBEF43>[" + data.service + "]</color> " + data.type + ", size: ";
        switch ( data.size ) {
        case 0:
          _dataList += "100kb";
          break;

        case 1:
          _dataList += "500kb";
          break;

        case 2:
          _dataList += "1mb";
          break;
        }
      }
      pendingDataTitle.text = "pending data (" + dataPackages.Count + ")";
      pendingDataLabel.text = _dataList;
    }

    public void UpdateActiveDataLabel ( List<DataPackage> dataPackages ) {
      string _dataList = "";
      int _count = 0;
      foreach ( DataPackage data in dataPackages ) {
        if ( _count > 0 ) {
          _dataList += "\n";
        }
        ++_count;
        _dataList += "<color=#CBEF43>[" + data.service + "]</color> " + data.type + ", size: ";
        switch ( data.size ) {
        case 0:
          _dataList += "100kb";
          break;

        case 1:
          _dataList += "500kb";
          break;

        case 2:
          _dataList += "1mb";
          break;
        }

        float _perc = data.transmitTimer / ( data.size * DATA_PACKAGE.dataTransmitDuration );
        _dataList += " (" + Mathf.RoundToInt ( _perc * 100f ) + "%)";
      }
      activeDataTitle.text = "active data (" + dataPackages.Count + ")";
      activeDataLabel.text = _dataList;
    }

    public void UpdateTransmittedDataLabel ( List<DataPackage> dataPackages ) {
      string _dataList = "";
      int _count = 0;
      foreach ( DataPackage data in dataPackages ) {
        if ( _count > 0 ) {
          _dataList += "\n";
        }
        ++_count;
        _dataList += "<color=#CBEF43>[" + data.service + "]</color> " + data.type + ", size: ";
        switch ( data.size ) {
        case 0:
          _dataList += "100kb";
          break;

        case 1:
          _dataList += "500kb";
          break;

        case 2:
          _dataList += "1mb";
          break;
        }
      }
      transmittedDataTitle.text = "transmitted data (" + dataPackages.Count + ")";
      transmittedDataLabel.text = _dataList;
    }

    //--------------------------------------------------
    // protected
    //--------------------------------------------------
    protected void Awake () {
      instance = this;
    }

    protected void Update () {
      
    }
  }
}