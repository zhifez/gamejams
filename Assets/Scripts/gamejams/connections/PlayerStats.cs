using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.zhifez.seagj {
  public class PlayerStats : Base {
    public static PlayerStats instance;

    public int startingFund = 1000;
    public int secondsPerHour = 30;
    public int workHour = 8;

    private float _dayTimer = 0f;
    private float dayTimer {
      get { return _dayTimer; }
      set {
        _dayTimer = value;
        if ( _dayTimer <= 0f ) {
          _dayTimer = 0f;
          UI_GAME.UpdateTimeLabel ( "End of Work" );
        }
        else {
          UI_GAME.UpdateTimeLabel ( _dayTimer / ( float ) secondsPerHour );
        }
      }
    }
    private bool dayTimerIsStarted = false;
    private int _funds;
    public int funds {
      get { return _funds; }
      set {
        if ( _funds == value ) {
          return;
        }

        _funds = value;

        UI_GAME.UpdateFundsLabel ( funds );
      }
    }

    //--------------------------------------------------
    // private
    //--------------------------------------------------

    //--------------------------------------------------
    // public
    //--------------------------------------------------
    public void InitFund () {
      funds = startingFund;
    }

    public void BeginTimer () {
      dayTimer = secondsPerHour * workHour;
      dayTimerIsStarted = true;
    }

    //--------------------------------------------------
    // protected
    //--------------------------------------------------
    protected void Awake () {
      instance = this;
    }

    protected void Update () {
      if ( dayTimerIsStarted ) {
        dayTimer -= Time.deltaTime;
        if ( dayTimer <= 0 ) {
          dayTimerIsStarted = false;
        }
      }
    }
  }
}