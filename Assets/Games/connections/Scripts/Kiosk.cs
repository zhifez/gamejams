using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace com.zhifez.seagj {
  public class Kiosk : Base {
    public enum LinkType {
      main, satelite, t_machine
    }
    public LinkType linkType;
    public string linkId;
    public Transform pointerIcon;

    private const float interactDistance = 1.2f;

    private Tween pointerTween;
    private bool _showPointer = false;
    private bool showPointer {
      get { return _showPointer; }
      set {
        if ( _showPointer == value ) {
          return;
        }

        _showPointer = value;
        if ( _showPointer ) {
          pointerIcon.localScale = Vector3.zero;
          pointerIcon.gameObject.SetActive ( true );
          pointerIcon.localPosition = new Vector3 ( 0f, 3f, 0f );
          pointerIcon.DOScale ( Vector3.one, 0.2f )
          .SetEase ( Ease.OutBack );
          TweenPointer ();
        }
        else {
          if ( pointerTween != null ) {
            pointerTween.Kill ();
            pointerTween = null;
          }
          pointerIcon.DOScale ( Vector3.zero, 0.2f )
          .SetEase ( Ease.InBack )
          .OnComplete ( () => {
            pointerIcon.gameObject.SetActive ( false );
          } );
        }
      }
    }

    //--------------------------------------------------
    // private
    //--------------------------------------------------
    private void TweenPointer () {
      pointerTween = pointerIcon.DOLocalMoveY ( 4f, 0.5f )
      .SetEase ( Ease.Linear )
      .OnComplete ( () => {
        pointerTween = pointerIcon.DOLocalMoveY ( 3f, 0.5f )
        .SetEase ( Ease.Linear )
        .OnComplete ( () => {
          TweenPointer ();
        } );
      } );
    }

    //--------------------------------------------------
    // public
    //--------------------------------------------------

    //--------------------------------------------------
    // protected
    //--------------------------------------------------
    protected void Awake () {
      pointerIcon.gameObject.SetActive ( false );
    }

    protected void Update () {
      if ( GAME.isIdle ) {
        Vector3 _interactPoint = transform.position + transform.forward * interactDistance * 0.75f;
        float _dist = Vector3.Distance ( _interactPoint, SCIENTIST.transform.position );
        showPointer = ( _dist < interactDistance );
        if ( showPointer ) {
          if ( Input.GetKeyDown ( KeyCode.J )
            || Input.GetKeyDown ( KeyCode.Z ) ) {
            switch ( linkType ) {
            case LinkType.main:
              GAME.ManageOverall ();
              break;

            case LinkType.satelite:
              GAME.ManageSatelite ( linkId );
              break;

            case LinkType.t_machine:
              GAME.ManageTM ( linkId );
              break;
            }
          }
        }
      }
      else {
        showPointer = false;
      }
    }

    protected void OnDrawGizmos () {
      Vector3 _interactPoint = transform.position + transform.forward * interactDistance * 0.75f;
      Gizmos.DrawWireSphere ( _interactPoint, interactDistance );
    }
  }
}