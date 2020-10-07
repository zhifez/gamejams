using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.zhifez.gamejams {
  public class CameraFollow : MonoBehaviour {
    public Vector3 targetOffset = Vector3.zero;
    public float distance = 10f;
    public float height = 10f;
    public float speed = 10f;
    public float rotateSpeed = 50f;

    private Transform target;

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
      target = GameObject.FindGameObjectWithTag ( "Player" ).transform;
    }

    protected void LateUpdate () {
      Vector3 _followPos = target.position;
      _followPos.z -= distance;
      _followPos.y += height;

      transform.position = Vector3.Slerp (
        transform.position,
        _followPos,
        Time.deltaTime * speed
      );

      Vector3 _targetPos = target.position + targetOffset;
      Vector3 _dir = Vector3.Normalize ( _targetPos - transform.position );
      Quaternion _rot = Quaternion.LookRotation ( _dir );
      transform.rotation = Quaternion.Slerp (
        transform.rotation,
        _rot,
        Time.deltaTime * rotateSpeed
      );
    }
  }
}