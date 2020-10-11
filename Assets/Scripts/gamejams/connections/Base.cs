using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.zhifez.gamejams;

namespace com.zhifez.seagj {
  public class Base : MonoBehaviour {
		protected GameController GAME {
			get { return GameController.instance; }
		}

		protected PlayerStats PLAYER_STATS {
			get { return PlayerStats.instance; }
		}

		protected DataPackageHandler DATA_PACKAGE {
			get { return DataPackageHandler.instance; }
		}

		protected CameraFollow CAMERA {
			get { return CameraFollow.instance; }
		}
		
		protected Scientist SCIENTIST {
			get { return Scientist.instance; }
		}
		
		protected UI_Game UI_GAME {
			get { return UI_Game.instance; }
		}

		protected UI_Main UI_MAIN {
			get { return UI_Main.instance; }
		}

		protected UI_End UI_END {
			get { return UI_End.instance; }
		}

		protected UI_Main_Transmission UI_MAIN_TRANSMISSION {
			get { return UI_Main_Transmission.instance; }
		}

		protected UI_SateliteKiosk UI_SAT {
			get { return UI_SateliteKiosk.instance; }
		}

		protected UI_TmKiosk UI_TM {
			get { return UI_TmKiosk.instance; }
		}

		protected float INPUT_HOR {
			get { return Input.GetAxis ( "Horizontal" ); }
		}

		protected float INPUT_VER {
			get { return Input.GetAxis ( "Vertical" ); }
		}
  }
}