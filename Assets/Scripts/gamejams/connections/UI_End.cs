using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

namespace com.zhifez.seagj {
  public class UI_End : Base {
    public static UI_End instance;

    public Transform container;
    public Text title;
    public Text[] resultsLabels;
    public Text[] purchasesLabels;
    public Text gameOverLabel;
    public Text instructionsLabel;

    //--------------------------------------------------
    // state machine
    //--------------------------------------------------
    public enum State {
      none,
      results,
      purchases,
      game_over
    }
    private State _currentState = State.none;
    private State currentState {
      get { return _currentState; }
      set {
        if ( _currentState == value ) {
          return;
        }

        _currentState = value;

        resultsLabels[0].gameObject.SetActive ( _currentState == State.results );
        resultsLabels[1].gameObject.SetActive ( _currentState == State.results );
        purchasesLabels[0].gameObject.SetActive ( _currentState == State.purchases );
        purchasesLabels[1].gameObject.SetActive ( _currentState == State.purchases );
        gameOverLabel.gameObject.SetActive ( _currentState == State.game_over );

        switch ( _currentState ) {
        case State.results:
          Setup_results ();
          break;

        case State.purchases:
          title.text = "purchases";
          instructionsLabel.text = "Press W/S or Arrow Up/Down keys to choose an item";
          instructionsLabel.text += "\nPress A/D or Arrow Left/Right keys to change quantity of selected item";
          instructionsLabel.text += "\nPress SPACE to purchase and continue";
          break;

        case State.game_over:
          title.text = "game over";
          instructionsLabel.text = "Press R key to restart game";
          instructionsLabel.text += "\nPress SPACE to continue";
          break;
        }
      }
    }

    private void Setup_results () {
      title.text = "results";
      instructionsLabel.text = "Press SPACE to continue";
      
      Result _result = DATA_PACKAGE.GetFinalResult ();
      resultsLabels[0].text = _result.result0;
      resultsLabels[1].text = _result.result1;
    }

    private void State_results () {
      if ( Input.GetKeyDown ( KeyCode.Space ) ) {
        currentState = State.purchases;
      }
    }

    private void State_purchases () {
      if ( Input.GetKeyDown ( KeyCode.Space ) ) {
        if ( PLAYER_STATS.funds > 0 ) {
          GAME.StartGame ();
        }
        else {
          currentState = State.game_over;
        }
      }
    }

    private void State_game_over () {
      if ( Input.GetKeyDown ( KeyCode.Space ) ) {
        SceneManager.LoadScene ( "menu" );
      }

      if ( Input.GetKeyDown ( KeyCode.R ) ) {
        Scene _scene = SceneManager.GetActiveScene (); 
        SceneManager.LoadScene ( _scene.name );
      }
    }

    private void RunState () {
      switch ( currentState ) {
      case State.results:
        State_results ();
        break;

      case State.purchases:
        State_purchases ();
        break;

      case State.game_over:
        State_game_over ();
        break;
      }
    }

    //--------------------------------------------------
    // private
    //--------------------------------------------------

    //--------------------------------------------------
    // public
    //--------------------------------------------------

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

      currentState = State.none;
    }

    protected void OnEnable () {
      container.localScale = Vector3.zero;
      container.gameObject.SetActive ( true );
      container.DOScale ( Vector3.one, 0.2f )
      .SetEase ( Ease.OutBack );

      currentState = State.results;
    }

    protected void Update () {
      RunState ();
    }
  }
}