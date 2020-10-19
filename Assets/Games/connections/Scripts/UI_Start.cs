using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace com.zhifez.seagj {
  public class UI_Start : Base {
    public static UI_Start instance;

    public Transform container;
    public Text title;
    public Text content;

    //--------------------------------------------------
    // private
    //--------------------------------------------------

    //--------------------------------------------------
    // public
    //--------------------------------------------------
    public string GenerateGuide () {
      string _guide = "<size=25><b>How to play:</b></size>";
      _guide += "\n\n1. WASD or Arrow Keys to move around; J or Z Keys to interact with kiosk.";
      _guide += "\n\n2. At the start of the game, only one service is available: <b>Unaffiliated</b>, and to activate it, you'll need to tweak the signal strength and speed to match the frequency of the service. Go to main kiosk, under the <b>Services</b> section to know the exact value of its frequency.";
      _guide += "\n\n3. To tweak the signal strength and speed, you can either rotate the satellite dish (via its kiosk), or tweak signal offset values of the transmission machine (via its kiosk) to match the service's frequencies.";
      _guide += "\n\n4. As you purchase higher tier services, you'll need to connect your transmission machine to more satellite dishes (you may purchase up to 4, including the default dish) in order to match the frequencies needed to activate the service. To do this, you'll need to go to the <b>Connections</b> section of the main kiosk UI, and link these machines up with the sat-dishes.";
      _guide += "\n\n5. You lose when your funds hit $0 or below after a day ended.";
      _guide += "\n\n6. However, if your funds hit $0 or below after purchasing new items, the game will not end.";
      _guide += "\n\n7. You win when you manage to earn $" + PLAYER_STATS.targetFunds + " in funds and activated all 4 services.";
      return _guide;
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

      if ( GAME == null ) {
        return;
      }

      AudioController.Play ( "kiosk_interact" );
      container.gameObject.SetActive ( true );
      container.DOScale ( Vector3.one, 0.2f )
      .SetEase ( Ease.OutBack );

      title.text = "Welcome to day " + ( PLAYER_STATS.daysSurvived + 1 );
      if ( GAME.isHardMode ) {
        if ( PLAYER_STATS.daysSurvived <= 0 ) {
          content.text = GenerateGuide ();
        }
      }
      else {
        content.text = GenerateGuide ();
      }
    }
  }
}