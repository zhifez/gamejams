using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

namespace com.zhifez.seagj {
  public class MainMenu : Base {
    public Transform camTarget;
    public float initPanAngle = 45;
    public float panAngle = 15f;
    public float panDuration = 3f;

    //--------------------------------------------------
    // private
    //--------------------------------------------------
    private void RotateCamTarget () {
      Vector3 _nextAngle = new Vector3 ( 0f, initPanAngle + panAngle, 0f );
      camTarget.DORotate ( 
        _nextAngle, panDuration
      )
      .SetEase ( Ease.Linear )
      .OnComplete ( () => {
        _nextAngle.y = initPanAngle - panAngle;
        camTarget.DORotate ( 
          _nextAngle, 
          panDuration 
        )
        .SetEase ( Ease.Linear )
        .OnComplete ( () => {
          RotateCamTarget ();
        } );
      } );
    }

    //--------------------------------------------------
    // public
    //--------------------------------------------------

    //--------------------------------------------------
    // protected
    //--------------------------------------------------
    protected void Start () {
			// TODO: play audio
      Vector3 _camAngle = new Vector3 ( 0f, initPanAngle, 0f );
      _camAngle.y -= panAngle;
      camTarget.eulerAngles = _camAngle;
      RotateCamTarget ();
    }

    protected void Update () {
      if ( Input.GetKeyDown ( KeyCode.Space ) ) {
        DOTween.KillAll ();
				SceneManager.LoadScene ( "gameplay" );
			}
    }
  }
}