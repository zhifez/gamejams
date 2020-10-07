using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.zhifez.gamejams {
  public class Test : MonoBehaviour {
    public Transform playerTransform;

    private MapGenerator mapGen;

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
      mapGen = GetComponentInChildren<MapGenerator> ();
      mapGen.Generate ();

      playerTransform.position = mapGen.GetStartPosition ();
    }

    protected void Update () {

    }
  }
}