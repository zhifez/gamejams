using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace com.zhifez.seagj {
  public class MainMenu : Base {
    

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
			// play audio
    }

    protected void Update () {
      if ( Input.GetKeyDown ( KeyCode.Space ) ) {
				SceneManager.LoadScene ( "gameplay" );
			}
    }
  }
}