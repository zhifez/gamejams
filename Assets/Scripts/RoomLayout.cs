using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.zhifez.gamejams {
  public class RoomLayout : MonoBehaviour {
		private string[][] layout;

    private float roomWidth;
    private float roomLength;

    private bool tryGetMapGen = false;
    private MapGenerator _mapGen;
    private MapGenerator mapGen {
      get {
        if ( !tryGetMapGen ) {
          _mapGen = GetComponentInParent<MapGenerator> ();
          tryGetMapGen = true;
        }
        return _mapGen;
      }
    }

    private bool tryGetBoxCollider = false;
    private BoxCollider _boxCollider;
    private BoxCollider boxCollider {
      get {
        if ( !tryGetBoxCollider ) {
          _boxCollider = gameObject.AddComponent<BoxCollider> ();
          tryGetBoxCollider = true;
        }
        return _boxCollider;
      }
    }

    //--------------------------------------------------
    // private
    //--------------------------------------------------

    //--------------------------------------------------
    // public
    //--------------------------------------------------
    public void InitLayout ( 
      string roomTheme,
      bool doorTop,
      bool doorBottom,
      bool doorLeft,
      bool doorRight
    ) {
      roomWidth = mapGen.columnUnit * mapGen.unitSize;
      roomLength = mapGen.rowUnit * mapGen.unitSize;

      boxCollider.size = new Vector3 (
        roomWidth, 0.5f, roomLength
      );
      boxCollider.center = new Vector3 ( 0f, -0.25f, 0f );

      layout = new string[ mapGen.rowUnit ][];
      for ( int r=0; r<mapGen.rowUnit; ++r ) {
        layout[r] = new string[ mapGen.columnUnit ];
        for ( int c=0; c<mapGen.columnUnit; ++c ) {
          layout[r][c] = "Rooms/" + roomTheme + "/";
          if ( r == 0 ) {
            if ( c == 0 ) {
              layout[r][c] += "bottom_left";
            }
            else if ( c == mapGen.columnUnit - 1 ) {
              layout[r][c] += "bottom_right";
            }
            else {
              if ( doorBottom ) {
                layout[r][c] += "center";
              }
              else {
                layout[r][c] += "bottom";
              }
            }
          }
          else if ( r == mapGen.rowUnit - 1 ) {
            if ( c == 0 ) {
              layout[r][c] += "top_left";
            }
            else if ( c == mapGen.columnUnit - 1 ) {
              layout[r][c] += "top_right";
            }
            else {
              if ( doorTop ) {
                layout[r][c] += "center";
              }
              else {
                layout[r][c] += "top";
              }
            }
          }
          else {
            if ( c == 0 ) {
              if ( doorLeft ) {
                layout[r][c] += "center";
              }
              else {
                layout[r][c] += "left";
              }
            }
            else if ( c == mapGen.columnUnit - 1 ) {
              if ( doorRight ) {
                layout[r][c] += "center";
              }
              else {
                layout[r][c] += "right";
              }
            }
            else {
              layout[r][c] += "center";
            }
          }
        }
      }
    }

    public void Generate () {
      for ( int r=0; r<layout.Length; ++r ) {
        for ( int c=0; c<layout[r].Length; ++c ) {
          Vector3 _roomPartPos = transform.position;
          _roomPartPos.x -= roomWidth * 0.5f - mapGen.unitSize * 0.5f;// + c * mapGen.unitSize;
          _roomPartPos.x += c * mapGen.unitSize;
          _roomPartPos.z -= roomLength * 0.5f - mapGen.unitSize * 0.5f;// + r * mapGen.unitSize;
          _roomPartPos.z += r * mapGen.unitSize;
          GameObject _roomPartGO = Resources.Load<GameObject> ( layout[r][c] );
          _roomPartGO = Instantiate<GameObject> ( 
            _roomPartGO, _roomPartPos, Quaternion.identity
          );
          _roomPartGO.transform.SetParent ( transform );
        }
      }
    }

    //--------------------------------------------------
    // protected
    //--------------------------------------------------
    protected void Awake () {
      gameObject.tag = "Room";
    }

    protected void Update () {

    }
  }
}