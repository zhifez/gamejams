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

    //--------------------------------------------------
    // private
    //--------------------------------------------------

    //--------------------------------------------------
    // public
    //--------------------------------------------------
    public void InitLayout ( RoomAssets roomAssets ) {
      roomWidth = mapGen.columnUnit * mapGen.unitSize;
      roomLength = mapGen.rowUnit * mapGen.unitSize;;

      layout = new string[ mapGen.rowUnit ][];
      for ( int r=0; r<mapGen.rowUnit; ++r ) {
        layout[r] = new string[ mapGen.columnUnit ];
        for ( int c=0; c<mapGen.columnUnit; ++c ) {
          layout[r][c] = "Rooms/room0/";
          if ( r == 0 ) {
            if ( c == 0 ) {
              layout[r][c] += roomAssets.bottomLeftGO[0].name;
            }
            else if ( c == mapGen.columnUnit - 1 ) {
              layout[r][c] += roomAssets.bottomRightGO[0].name;
            }
            else {
              layout[r][c] += roomAssets.bottomGO[0].name;
            }
          }
          else if ( r == mapGen.rowUnit - 1 ) {
            if ( c == 0 ) {
              layout[r][c] += roomAssets.topLeftGO[0].name;
            }
            else if ( c == mapGen.columnUnit - 1 ) {
              layout[r][c] += roomAssets.topRightGO[0].name;
            }
            else {
              layout[r][c] += roomAssets.topGO[0].name;
            }
          }
          else {
            if ( c == 0 ) {
              layout[r][c] += roomAssets.leftGO[0].name;
            }
            else if ( c == mapGen.columnUnit - 1 ) {
              layout[r][c] += roomAssets.rightGO[0].name;
            }
            else {
              layout[r][c] += roomAssets.centerGO[0].name;
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
      
    }

    protected void Update () {

    }
  }
}