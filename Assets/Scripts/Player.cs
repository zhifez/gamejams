using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.zhifez.gamejams {
  public class Player : MonoBehaviour {
    public float speed = 10f;
    public float rotateSpeed = 20f;
    public float gravity = 30f;

    private CharacterController charControl;

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
      charControl = GetComponentInChildren<CharacterController> ();
    }

    protected void Update () {
      float _horAxis = Input.GetAxis ( "Horizontal" );
      float _verAxis = Input.GetAxis ( "Vertical" );

      float _rotateOffset = 0.05f;
      if ( _horAxis < -_rotateOffset || _horAxis > _rotateOffset
        || _verAxis < -_rotateOffset || _verAxis > _rotateOffset ) {
        Vector3 _dir = new Vector3 ( _horAxis, 0f, _verAxis );
        Quaternion _rot = Quaternion.LookRotation ( _dir );
        transform.rotation = Quaternion.Slerp (
          transform.rotation,
          _rot,
          Time.deltaTime * rotateSpeed
        );
      }

      float _moveOffset = 0.15f;
      Vector3 _move = Vector3.up * -gravity;
      if ( _horAxis < -_moveOffset || _horAxis > _moveOffset
        || _verAxis < -_moveOffset || _verAxis > _moveOffset ) {
        _move += transform.forward * speed;
      }
      charControl.Move ( _move * Time.deltaTime );
    }
  }
}