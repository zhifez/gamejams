using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace com.zhifez.seagj {
  public class UI_Main_Connections : Base {
    public Text[] labels;
    public GameObject instructSelectTm;
    public GameObject instructSelectLink;

    private int _selectionIndex = 0;
    private int selectionIndex {
      get { return _selectionIndex; }
      set {
        _selectionIndex = value;
        
        int _maxLength = 0;
        if ( activeTm == null ) {
          foreach ( TransmissionMachine tm in GAME.tmMachines ) {
            if ( tm.gameObject.activeSelf ) {
              ++_maxLength;
            }
          }
        }
        else {
          foreach ( SateliteDish sd in GAME.satDishes ) {
            if ( sd.enabled ) {
              ++_maxLength;
            }
          }
        }
        
        if ( _selectionIndex < 0 ) {
          if ( activeTm == null ) {
            _selectionIndex = _maxLength - 1;
          }
          else {
            _selectionIndex = System.Array.IndexOf ( GAME.tmMachines, activeTm );
            activeTm = null;
          }
        }
        else if ( _selectionIndex >= _maxLength ) {
          _selectionIndex = 0;
        }
      }
    }

    private TransmissionMachine _activeTm;
    private TransmissionMachine activeTm {
      get { return _activeTm; }
      set {
        _activeTm = value;

        instructSelectTm.SetActive ( _activeTm == null );
        instructSelectLink.SetActive ( _activeTm != null );
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
    protected void OnEnable () {
      if ( GAME != null ) {
        activeTm = null;
        selectionIndex = 0;
      }
    }

    protected void Update () {
      if ( activeTm == null ) {
        if ( Input.GetKeyDown ( KeyCode.A )
          || Input.GetKeyDown ( KeyCode.LeftArrow ) ) {
          --selectionIndex;
        }

        if ( Input.GetKeyDown ( KeyCode.D )
          || Input.GetKeyDown ( KeyCode.RightArrow ) ) {
          ++selectionIndex;
        }

        if ( Input.GetKeyDown ( KeyCode.S )
          || Input.GetKeyDown ( KeyCode.DownArrow ) ) {
          activeTm = GAME.tmMachines[ selectionIndex ];
          selectionIndex = 0;
        }
      }
      else {
        if ( Input.GetKeyDown ( KeyCode.W )
          || Input.GetKeyDown ( KeyCode.UpArrow ) ) {
          --selectionIndex;
        }

        if ( Input.GetKeyDown ( KeyCode.S )
          || Input.GetKeyDown ( KeyCode.DownArrow ) ) {
          ++selectionIndex;
        }

        if ( Input.GetKeyDown ( KeyCode.J )
          || Input.GetKeyDown ( KeyCode.Z ) ) {
          // link or unlink
          SateliteDish sd = GAME.satDishes[ selectionIndex ];
          bool _isLinked = activeTm.SateliteDishIsLinked ( sd );
          if ( _isLinked ) {
            activeTm.UnlinkSateliteDish ( sd );
          }
          else {
            activeTm.LinkSateliteDish ( sd, 0f, 0f );
          }
        }
      }

      for ( int a=0; a<GAME.tmMachines.Length; ++a ) {
        labels[a].text = "";
        TransmissionMachine tm = GAME.tmMachines[a];
        if ( !tm.gameObject.activeSelf ) {
          continue;
        }

        if ( activeTm == tm ) {
          labels[a].text = "<b>" + tm.name + "</b>";
        }
        else if ( selectionIndex == a ) {
          labels[a].text = "> " + tm.name;
        }
        else {
          labels[a].text += tm.name;
        }

        for ( int b=0; b<GAME.satDishes.Length; ++b ) {
          SateliteDish sd = GAME.satDishes[b];
          if ( !sd.enabled ) {
            continue;
          }

          labels[a].text += "\n    ";
          if ( activeTm != null 
            && selectionIndex == b ) {
            labels[a].text += "> ";
          }

          bool _isLinked = tm.SateliteDishIsLinked ( sd );
          labels[a].text += ( _isLinked ? "[X] " : "[] " ) + sd.name;
        }
      }
    }
  }
}