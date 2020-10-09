using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.zhifez.gamejams;

namespace com.zhifez.seagj {
  public class Scientist : MonoBehaviour {
    private Player player;
    private Animator animator;

    private bool _isProud = false;
    private bool isProud {
      get { return _isProud; }
      set {
        _isProud = value;
        player.enabled = !_isProud;
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
      player = GetComponentInChildren<Player> ();
      animator = GetComponentInChildren<Animator> ();
    }

    protected void Update () {
      if ( Input.GetKeyDown ( KeyCode.Space ) ) {
        isProud = true;
      }
      if ( Input.GetKeyUp ( KeyCode.Space ) ) {
        isProud = false;
      }

      if ( isProud ) {
        animator.SetInteger ( "anim", 2 );
      }
      else {
        if ( player.isMoving ) {
          animator.SetInteger ( "anim", 1 );
        }
        else {
          animator.SetInteger ( "anim", 0 );
        }
      }
    }
  }
}