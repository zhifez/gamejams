using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.zhifez.gamejams {
  public class CameraFollow : MonoBehaviour {
    public static CameraFollow instance;

    public Vector3 targetOffset = Vector3.zero;
    public float distance = 10f;
    public float height = 10f;
    public float speed = 10f;
    public float rotateSpeed = 50f;

    private Transform _target;
    private Transform target {
      get { 
        if ( overrideTarget != null ) {
          return overrideTarget;
        }
        return _target; 
      }
    }
    private Transform overrideTarget;
    private float maxHeight;

    //--------------------------------------------------
    // private
    //--------------------------------------------------

    //--------------------------------------------------
    // public
    //--------------------------------------------------
    public void SetOverrideTarget ( Transform _target ) {
      overrideTarget = _target;
    }

    //--------------------------------------------------
    // protected
    //--------------------------------------------------
    protected void Awake () {
      instance = this;

      _target = GameObject.FindGameObjectWithTag ( "Player" ).transform;
    }

    protected void Update () {
      RaycastHit _hit;
      maxHeight = height;
      for ( int a=0; a<4; ++a ) {
        Vector3 _rayOrigin = target.position;
        _rayOrigin.y += 2.5f;
        switch ( a ) {
        case 1:
          _rayOrigin.x -= 5f;
          break;

        case 2:
          _rayOrigin.x -= 5f;
          break;

        case 3:
          _rayOrigin.z -= distance;
          break;
        }
        if ( Physics.Raycast ( _rayOrigin, Vector3.up, out _hit ) ) {
          if ( _hit.collider.CompareTag ( "Room" ) ) {
            maxHeight = ( _hit.point.y - target.position.y - 2f );
          }
        }
      }
    }

    protected void LateUpdate () {
      Vector3 _followPos = target.position;
      _followPos.z -= distance;
      _followPos.y += Mathf.Min ( height, maxHeight );

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