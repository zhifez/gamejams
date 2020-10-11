using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace com.zhifez.seagj {
  public class UI_Game : Base {
    public static UI_Game instance;

    public Text fundsLabel;
    public Text timeLabel;

    //--------------------------------------------------
    // private
    //--------------------------------------------------

    //--------------------------------------------------
    // public
    //--------------------------------------------------
    public void UpdateFundsLabel ( int amount ) {
      fundsLabel.text = "Funds\n$" + amount;
    }

    public void UpdateTimeLabel ( float amount ) {
      int _hours = Mathf.FloorToInt ( amount );
      int _minutes = Mathf.FloorToInt ( ( amount % 1.0f ) * 60 );
      timeLabel.text = "Begin Work\n" + _hours + "h " + _minutes + "m";
    }

    public void UpdateTimeLabel ( string label ) {
      timeLabel.text = label;
    }
    
    public void SetLabelsAlpha ( float value = 1.0f ) {
      Color _alpha = fundsLabel.color;
      _alpha.a = value;
      fundsLabel.color = _alpha;
      timeLabel.color = _alpha;
    }

    //--------------------------------------------------
    // protected
    //--------------------------------------------------
    protected void Awake () {
      instance = this;
    }
  }
}