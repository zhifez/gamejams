using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.zhifez.seagj {
  public class Base : MonoBehaviour {
		protected GameController GAME {
			get { return GameController.instance; }
		}

		protected Scientist SCIENTIST {
			get { return Scientist.instance; }
		}
		
		protected UI_SateliteKiosk UI_SAT_DISH {
			get { return UI_SateliteKiosk.instance; }
		}

		protected float INPUT_HOR {
			get { return Input.GetAxis ( "Horizontal" ); }
		}

		protected float INPUT_VER {
			get { return Input.GetAxis ( "Vertical" ); }
		}
  }
}