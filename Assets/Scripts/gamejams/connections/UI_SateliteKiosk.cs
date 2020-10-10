using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace com.zhifez.seagj {
  public class UI_SateliteKiosk : Base {
    public static UI_SateliteKiosk instance;

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
    public void Setup ( SateliteDish _satDish ) {
      title.text = _satDish.name;
      instructions.text = "Press W/S or Arrow Up/Down keys to change " + _satDish.name + "'s facing direction vertically.";
      instructions.text += "\nPress A/D or Arrow Left/Right keys to change " + _satDish.name + "'s facing direction horizontally.";
      instructions.text += "\nPress Escape to close this window.";
      enabled = true;
    }

    public void UpdateValues ( float _strength, float _speed ) {
      values.text = "signal strength: " + _strength +
        "\nsignal speed: " + _speed;
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