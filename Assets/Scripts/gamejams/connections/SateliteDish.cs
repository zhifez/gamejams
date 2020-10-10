using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.zhifez.seagj {
  public class SateliteDish : MonoBehaviour {
    public Transform baseTransform;
    public Transform bowlTransform;

    public float valueX {
      get {
        float _angleX = Mathf.Abs ( bowlTransform.eulerAngles.x );
        _angleX -= minRotateUp;
        _angleX /= ( maxRotateUp - minRotateUp );
        return _angleX;
      }
    }

    public float valueY {
      get {
        float _angleY = Mathf.Abs ( bowlTransform.eulerAngles.y );
        _angleY /= 180f;
        if ( _angleY > 180f ) {
          _angleY *= -1f;
        }
        return _angleY;
      }
    }
    
    private const float rotateSpeed = 30f;
    private const float minRotateUp = 10f;
    private const float maxRotateUp = 80f;
    private float rotateDirection = 0f;

    //--------------------------------------------------
    // state machine
    //--------------------------------------------------
    public enum State {
      idle,
      move_hor,
      move_ver
    }
    private State _currentState = State.idle;
    private State currentState {
      get { return _currentState; }
      set {
        _currentState = value;

        switch ( _currentState ) {
        case State.idle:
          rotateDirection = 0f;
          break;
        }
      }
    }

    //--------------------------------------------------
    // private
    //--------------------------------------------------

    //--------------------------------------------------
    // public
    //--------------------------------------------------
    public void RotateDish ( float _value, string _direction = null ) {
      switch ( _direction ) {
      case "horizontal":
        currentState = State.move_hor;
        break;

      case "vertical":
        currentState = State.move_ver;
        break;

      default:
        currentState = State.idle;
        break;
      }
      rotateDirection = _value;
    }

    //--------------------------------------------------
    // protected
    //--------------------------------------------------
    protected void Awake () {
      currentState = State.idle;
    }

    protected void Update () {
      Vector3 _rotation = Vector3.zero;

      switch ( currentState ) {
      case State.move_hor:
        _rotation += Vector3.up * rotateDirection * 1f;
        baseTransform.Rotate (
          _rotation * Time.deltaTime * rotateSpeed,
          Space.World
        );
        break;

      case State.move_ver:
        if ( ( rotateDirection < 0 && bowlTransform.eulerAngles.x < 80f )
          || ( rotateDirection > 0 && bowlTransform.eulerAngles.x > 10f ) ) {
          _rotation += bowlTransform.right * rotateDirection * -1f;
        }
        bowlTransform.Rotate (
          _rotation * Time.deltaTime * rotateSpeed,
          Space.World
        );
        break;
      }
    }
  }
}