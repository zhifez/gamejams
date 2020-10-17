using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

namespace com.zhifez.seagj {
  [ System.Serializable ]
  public class PurchaseLog {
    public int tmMachineCount;
    public int satDishCount;
    public List<DataPackage.Service> newServices; 
    public int selectionIndex;

    public PurchaseLog () {
      tmMachineCount = 0;
      satDishCount = 0;
      newServices = new List<DataPackage.Service> ();
      selectionIndex = 0;
    }
  }

  public class UI_End : Base {
    public static UI_End instance;

    public Transform container;
    public Text title;
    public Text[] resultsLabels;
    public Text[] purchasesLabels;
    public Text gameOverLabel;
    public Text instructionsLabel;

    private PurchaseLog purchaseLog;

    //--------------------------------------------------
    // state machine
    //--------------------------------------------------
    public enum State {
      none,
      results,
      purchases,
      game_over,
      game_ends
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
        gameOverLabel.gameObject.SetActive ( 
          _currentState == State.game_over 
          || _currentState == State.game_ends 
        );

        switch ( _currentState ) {
        case State.results:
          Setup_results ();
          break;

        case State.purchases:
          title.text = "purchases";
          instructionsLabel.text = "press W/S or Arrow Up/Down keys to choose an item";
          instructionsLabel.text += "\npress A/D or Arrow Left/Right keys to change quantity of selected item";
          instructionsLabel.text += "\npress SPACE to purchase and continue";
          purchaseLog = new PurchaseLog ();
          break;

        case State.game_over:
          title.text = "game over";
          instructionsLabel.text = "press SPACE to start a new game";
          instructionsLabel.text += "\npress ESCAPE key to go to the main menu";

          gameOverLabel.text = "Oops. You ran out of funds ($" + PLAYER_STATS.funds + ").";
          gameOverLabel.text += "\n\nYou survived " + PLAYER_STATS.daysSurvived;
          if ( PLAYER_STATS.daysSurvived > 1 ) {
            gameOverLabel.text += " days.";
          }
          else {
            gameOverLabel.text += " day.";
          }
          gameOverLabel.text += "\n\nBetter luck next time.";
          gameOverLabel.text += "\n\nThank you for playing!";
          gameOverLabel.text += "\n\nThe End";
          break;

        case State.game_ends:
          title.text = "endgame";
          instructionsLabel.text = "press SPACE to start a new game";
          instructionsLabel.text += "\npress ESCAPE key to go to the main menu";

          gameOverLabel.text = "You raised $" + PLAYER_STATS.funds + " in funds, in " + PLAYER_STATS.daysSurvived + " days!";
          gameOverLabel.text += "\n\nThat's $" + ( PLAYER_STATS.funds - PLAYER_STATS.targetFunds ) + " more than the targeted value!";
          gameOverLabel.text += "\n\nGood job for making this far.";
          gameOverLabel.text += "\n\nSend an email to zhifez.studio@gmail.com if you feel like telling us about what you like/dislike about the game.";
          gameOverLabel.text += "\n\nHave a nice day!";
          break;
        }
      }
    }

    private void Setup_results () {
      title.text = "results";
      instructionsLabel.text = "press SPACE to continue";
      
      Result _result = DATA_PACKAGE.GetFinalResult ();
      resultsLabels[0].text = _result.result0;
      resultsLabels[1].text = _result.result1;
    }

    private void State_results () {
      if ( Input.GetKeyDown ( KeyCode.Space ) ) {
				AudioController.Play ( "ui_btn_direction_section" );
        if ( PLAYER_STATS.funds >= PLAYER_STATS.targetFunds
          && GAME.HasEnabledAllServices () ) {
          currentState = State.game_ends;
        }
        else if ( PLAYER_STATS.funds > 0 ) {
          currentState = State.purchases;
        }
        else {
          currentState = State.game_over;
        }
      }
    }

    private void State_purchases () {
      if ( Input.GetKeyDown ( KeyCode.Space ) ) {
				AudioController.Play ( "ui_btn_direction_section" );
        PLAYER_STATS.funds -= GetTotalPurchase ();

        // if ( PLAYER_STATS.funds > 0 ) {
          GAME.enabledTmMachineCount += purchaseLog.tmMachineCount;
          GAME.enabledSatDishCount += purchaseLog.satDishCount;
          foreach ( DataPackage.Service ns in purchaseLog.newServices ) {
            DATA_PACKAGE.EnableService ( ns );
          }
          GAME.StartGame ();
        // }
        // else {
        //   currentState = State.game_over;
        // }
      }

      int _maxSelection = DATA_PACKAGE.serviceSignalPatterns.Length + 2;
      if ( Input.GetKeyDown ( KeyCode.W )
        || Input.GetKeyDown ( KeyCode.UpArrow ) ) {
        AudioController.Play ( "ui_btn_direction" );
        --purchaseLog.selectionIndex;
        if ( purchaseLog.selectionIndex <= 0 ) {
          purchaseLog.selectionIndex = 0;
        }
      }

      if ( Input.GetKeyDown ( KeyCode.S )
        || Input.GetKeyDown ( KeyCode.DownArrow ) ) {
        AudioController.Play ( "ui_btn_direction" );
        ++purchaseLog.selectionIndex;
        if ( purchaseLog.selectionIndex >= _maxSelection ) {
          purchaseLog.selectionIndex = _maxSelection - 1;
        }
      }

      int _maxTmMachinesCount = GAME.tmMachines.Length - GAME.enabledTmMachineCount;
      int _maxSatDishCount = GAME.satDishes.Length - GAME.enabledSatDishCount;
      if ( Input.GetKeyDown ( KeyCode.A )
        || Input.GetKeyDown ( KeyCode.LeftArrow ) ) {
        switch ( purchaseLog.selectionIndex ) {
        case 0:
          if ( _maxTmMachinesCount > 0 ) {
            --purchaseLog.tmMachineCount;
            if ( purchaseLog.tmMachineCount < 0 ) {
              purchaseLog.tmMachineCount = 0;
              AudioController.Play ( "ui_btn_reject" );
            }
            else {
              AudioController.Play ( "ui_btn_toggle" );
            }
          }
          break;

        case 1:
          if ( _maxSatDishCount > 0 ) {
            --purchaseLog.satDishCount;
            if ( purchaseLog.satDishCount < 0 ) {
              purchaseLog.satDishCount = 0;
            }
            else {
              AudioController.Play ( "ui_btn_toggle" );
            }
          }
          break;

        default:
          ServiceSignalPattern _ssp = DATA_PACKAGE.serviceSignalPatterns[ purchaseLog.selectionIndex - 2 ];
          if ( purchaseLog.newServices.Contains ( _ssp.service ) ) {
            purchaseLog.newServices.Remove ( _ssp.service );
            AudioController.Play ( "ui_btn_toggle" );
          }
          break;
        }
      }

      if ( Input.GetKeyDown ( KeyCode.D )
        || Input.GetKeyDown ( KeyCode.RightArrow ) ) {
        switch ( purchaseLog.selectionIndex ) {
        case 0:
          if ( _maxTmMachinesCount > 0 ) {
            ++purchaseLog.tmMachineCount;
            if ( purchaseLog.tmMachineCount > _maxTmMachinesCount ) {
              purchaseLog.tmMachineCount = _maxTmMachinesCount;
              AudioController.Play ( "ui_btn_reject" );
            }
            else {
              AudioController.Play ( "ui_btn_toggle" );
            }
          }
          break;

        case 1:
          if ( _maxSatDishCount > 0 ) {
            ++purchaseLog.satDishCount;
            if ( purchaseLog.satDishCount > _maxSatDishCount ) {
              purchaseLog.satDishCount = _maxSatDishCount;
            }
            else {
              AudioController.Play ( "ui_btn_toggle" );
            }
          }
          break;

        default:
          ServiceSignalPattern _ssp = DATA_PACKAGE.serviceSignalPatterns[ purchaseLog.selectionIndex - 2 ];
          if ( !purchaseLog.newServices.Contains ( _ssp.service )
            && !DATA_PACKAGE.ServiceIsEnabled ( _ssp.service ) ) {
            purchaseLog.newServices.Add ( _ssp.service );
            AudioController.Play ( "ui_btn_toggle" );
          }
          break;
        }
      }

      purchasesLabels[0].text = "";
      purchasesLabels[1].text = "";
      for ( int a=0; a<_maxSelection; ++a ) {
        string _prefix = ( a > 0 ) ? "\n" : "";
        if ( purchaseLog.selectionIndex == a ) {
          _prefix += "> ";
        }

        switch ( a ) {
        case 0:
          purchasesLabels[0].text += _prefix + "transmission_machine - ";
          if ( _maxTmMachinesCount <= 0 ) {
            purchasesLabels[0].text += "[full]";
            purchasesLabels[1].text += "-";
          }
          else {
            purchasesLabels[0].text += "$" + DATA_PACKAGE.tmMachineRates.price;
            purchasesLabels[1].text += "x " + purchaseLog.tmMachineCount;
          }
          break;

        case 1:
          // purchasesLabels[0].text += _prefix + "satelite_dish - $" + DATA_PACKAGE.satDishRates.price;
          // purchasesLabels[1].text += "\nx " + purchaseLog.satDishCount;
          purchasesLabels[0].text += _prefix + "satelite_dish - ";
          if ( _maxSatDishCount <= 0 ) {
            purchasesLabels[0].text += "[full]";
            purchasesLabels[1].text += "\n-";
          }
          else {
            purchasesLabels[0].text += "$" + DATA_PACKAGE.satDishRates.price;
            purchasesLabels[1].text += "\nx " + purchaseLog.satDishCount;
          }
          purchasesLabels[0].text += "\n\n";
          purchasesLabels[1].text += "\n\n";
          break;

        default:
          ServiceSignalPattern _ssp = DATA_PACKAGE.serviceSignalPatterns[ a - 2 ];
          purchasesLabels[0].text += _prefix + _ssp.service + "_licensing - ";
          if ( DATA_PACKAGE.ServiceIsEnabled ( _ssp.service ) ) {
            purchasesLabels[0].text += "[purchased]";
            purchasesLabels[1].text += "\n";
          }
          else {
            purchasesLabels[0].text += "$" + _ssp.price;
            if ( purchaseLog.newServices.Contains ( _ssp.service ) ) {
              purchasesLabels[1].text += "\nselected";
            }
            else {
              purchasesLabels[1].text += "\n-";
            }
          }
          break;
        }
      }

      purchasesLabels[0].text += "\n\n-------------------------\n";
      purchasesLabels[0].text += "\nTotal Price";
      purchasesLabels[0].text += "\nFunds Left After Purchase";
      purchasesLabels[0].text = purchasesLabels[0].text.ToLower ();

      int _totalPrice = GetTotalPurchase ();
      purchasesLabels[1].text += "\n\n-------------------------\n";
      purchasesLabels[1].text += "\n$" + _totalPrice;
      purchasesLabels[1].text += "\n$" + ( PLAYER_STATS.funds - _totalPrice );
    }

    private void State_game_over () {
      if ( Input.GetKeyDown ( KeyCode.Escape ) ) {
				AudioController.Play ( "ui_btn_direction_section" );
        enabled = false;

        DOVirtual.DelayedCall ( 0.5f, () => {
          DOTween.KillAll ();
          SceneManager.LoadScene ( "menu" );
        } );
      }

      if ( Input.GetKeyDown ( KeyCode.Space ) ) {
        GAME.RestartGame ();
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
      case State.game_ends:
        State_game_over ();
        break;
      }
    }

    //--------------------------------------------------
    // private
    //--------------------------------------------------
    private int GetTotalPurchase () {
      int _total = 0;
      for ( int a=0; a<DATA_PACKAGE.serviceSignalPatterns.Length + 2; ++a ) {
        switch ( a ) {
        case 0:
          _total += purchaseLog.tmMachineCount * DATA_PACKAGE.tmMachineRates.price;
          break;

        case 1:
          _total += purchaseLog.satDishCount * DATA_PACKAGE.satDishRates.price;
          break;

        default:
          ServiceSignalPattern _ssp = DATA_PACKAGE.serviceSignalPatterns[ a - 2 ];
          if ( purchaseLog.newServices.Contains ( _ssp.service ) ) {
            _total += _ssp.price;
          }
          break;
        }
      }
      return _total;
    }

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