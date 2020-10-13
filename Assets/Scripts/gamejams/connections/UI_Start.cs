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

      container.gameObject.SetActive ( true );
      container.DOScale ( Vector3.one, 0.2f )
      .SetEase ( Ease.OutBack );

      title.text = "Welcome to day " + ( PLAYER_STATS.daysSurvived + 1 );
      content.text = "Rules:";
      content.text += "\n  1. Earn as much money as you can to stay afloat.";
      content.text += "\n  2. Unlock as many service licenses as you can.";
      content.text += "\n  3. You lose when your funds hit $0 or below after a day ended.";
      content.text += "\n  4. But if your funds hit $0 or below after purchasing new items, the game will not end due to rule No. 3.";
      content.text += "\n  5. You win when you manage to earn $" + PLAYER_STATS.targetFunds + " in funds.";
      content.text += "\n\nControls:";
      content.text += "\n  1. W/A/S/D or Arrow Keys to move around.";
      content.text += "\n  2. J or Z Key to interact with kiosks.";
    }
  }
}