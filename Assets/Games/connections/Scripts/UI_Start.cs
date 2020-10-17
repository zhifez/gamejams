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
      content.text = "<size=25><b>How to play:</b></size>";
      content.text += "\n\n1. WASD or Arrow Keys to move around; J or Z Keys to interact with kiosk.";
      content.text += "\n\n2. At the start of the game, only one service is available: <b>Unaffiliated</b>, and to activate it, you'll need to tweak the signal strength and speed to match the frequency of the service. Go to main kiosk, under the <b>Services</b> section to know the exact value of its frequency.";
      content.text += "\n\n3. To tweak the signal strength and speed, you can either rotate the satellite dish (via its kiosk), or tweak signal offset values of the transmission machine (via its kiosk) to match the service's frequencies.";
      content.text += "\n\n4. As you purchase higher tier services, you'll need to connect your transmission machine to more satellite dishes (you may purchase up to 4, including the default dish) in order to match the frequencies needed to activate the service. To do this, you'll need to go to the <b>Connections</b> section of the main kiosk UI, and link these machines up with the sat-dishes.";
      content.text += "\n\n5. You lose when your funds hit $0 or below after a day ended.";
      content.text += "\n\n6. However, if your funds hit $0 or below after purchasing new items, the game will not end.";
      content.text += "\n\n7. You win when you manage to earn $" + PLAYER_STATS.targetFunds + " in funds, and when you've activated all 4 services.";
      // content.text = "Rules:";
      // content.text += "\n  1. Earn as much funds as you can to stay afloat.";
      // content.text += "\n  2. Unlock as many service licenses as you can.";
      // content.text += "\n  3. You lose when your funds hit $0 or below after a day ended.";
      // content.text += "\n  4. However, if your funds hit $0 or below after purchasing new items, the game will not end.";
      // content.text += "\n  5. You win when you manage to earn $" + PLAYER_STATS.targetFunds + " in funds, and when you've activated all 4 services.";
      // content.text += "\n\nControls:";
      // content.text += "\n  1. W/A/S/D or Arrow Keys to move around.";
      // content.text += "\n  2. J or Z Key to interact with kiosks.";
    }
  }
}