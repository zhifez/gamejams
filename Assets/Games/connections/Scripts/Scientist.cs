using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.zhifez.gamejams;

namespace com.zhifez.seagj {
  public class Scientist : Base {
    public static Scientist instance;

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
    public void Respawn ( Transform _point ) {
			transform.position = _point.position;
			transform.rotation = _point.rotation;
      animator.SetInteger ( "anim", 0 );
    }

    //--------------------------------------------------
    // protected
    //--------------------------------------------------
    protected void Awake () {
      instance = this;

      player = GetComponentInChildren<Player> ();
      animator = GetComponentInChildren<Animator> ();

      enabled = false;
    }

    protected void OnDisable () {
      animator.SetInteger ( "anim", 0 );
      player.enabled = false;
    }

    protected void OnEnable () {
      player.enabled = true;
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