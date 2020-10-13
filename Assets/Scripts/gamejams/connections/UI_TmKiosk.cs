using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace com.zhifez.seagj {
  public class UI_TmKiosk : Base {
    public static UI_TmKiosk instance;

    public Transform container;
    public Text title;
    public Text values;
    public Text instructions;

    //--------------------------------------------------
    // private
    //--------------------------------------------------

    //--------------------------------------------------
    // public
    //--------------------------------------------------
    public void Setup ( TransmissionMachine _tmMachine ) {
      title.text = _tmMachine.name;
      instructions.text = "press W/S or Arrow Up/Down keys to switch between an offset value to alter (arranged by linked satelite dishes)";
      instructions.text += "\npress A/D or Arrow Left/Right keys to change the offset value";
      instructions.text += "\npress ESCAPE to close this window";
      enabled = true;
    }

    public void UpdateValues ( TransmissionMachine tm ) {
      values.text = "";
      if ( tm.linkedSatDishes.Count <= 0 ) {
        values.text = "this machine is not linked to any satelite dish";
        values.text += "\ngo to the central kiosk to link them up";
        return;
      }

      for ( int a=0; a<tm.linkedSatDishes.Count; ++a ) {
        if ( a > 0 ) {
          values.text += "\n";
        }
        values.text += tm.linkedSatDishes[a].sateliteDish.name;

        float _valueX = tm.linkedSatDishes[a].sateliteDish.valueX;
        float _valueY = tm.linkedSatDishes[a].sateliteDish.valueY;
        values.text += " (str: " + _valueX.ToString ( "N2" );
        values.text += ", spd: " + _valueY.ToString ( "N2" ) + ")";
				int _index = Mathf.FloorToInt ( ( float ) tm.linkedSatDishIndex / 2f );
        string _selected = "";
				if ( _index == a && tm.linkedSatDishIndex % 2 == 0 ) {
					_selected += "> ";
				}
        float _valueDeci = tm.linkedSatDishes[a].signalStrengthOffset;
        values.text += "\n  " + _selected + "signal strength offset: " + _valueDeci.ToString ( "N2" );
        
        _selected = "";
				if ( _index == a && tm.linkedSatDishIndex % 2 == 1 ) {
					_selected += "> ";
				}
        _valueDeci = tm.linkedSatDishes[a].signalSpeedOffset;
        values.text += "\n  " + _selected + "signal speed offset: " + _valueDeci.ToString ( "N2" );
      }
    }

    //--------------------------------------------------
    // protected
    //--------------------------------------------------
    protected void Awake () {
      instance = this;

      container.localScale = Vector3.zero;
      container.gameObject.SetActive ( false );
      enabled = false;
    }

    protected void OnDisable () {
      container.DOScale ( Vector3.zero, 0.2f )
      .SetEase ( Ease.InBack )
      .OnComplete ( () => {
        container.gameObject.SetActive ( false );
      } );
    }

    protected void OnEnable () {
      container.localScale = Vector3.zero;
      container.gameObject.SetActive ( true );
      container.DOScale ( Vector3.one, 0.2f )
      .SetEase ( Ease.OutBack );
    }

    protected void Update () {

    }
  }
}