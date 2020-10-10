using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.zhifez.gamejams;

namespace com.zhifez.seagj {
	[ System.Serializable ]
	public class MachineLink {
		public TransmissionMachine tm;
		public SateliteDish sat;

		public MachineLink ( TransmissionMachine tm, SateliteDish sat ) {
			this.tm = tm;
			this.sat = sat;
		}
	}

	public class GameController : MonoBehaviour {
		public static GameController instance;

		public Transform playerStartPos;
		public TransmissionMachine[] tmMachines;
		public SateliteDish[] satDishes;
		public Kiosk[] satKiosks;

		private CameraFollow CAMERA {
			get { return CameraFollow.instance; }
		}
		private Scientist SCIENTIST {
			get { return Scientist.instance; }
		}
		private List<MachineLink> machineLinks;

		//--------------------------------------------------
    // state machine
    //--------------------------------------------------
		public enum State {
			idle,
			manage_satelite
		}
		private State _currentState = State.idle;
		private State currentState {
			get { return _currentState; }
			set {
				if ( _currentState == value ) {
					return;
				}
				
				// prev state
				switch ( _currentState ) {
				case State.manage_satelite:
					CAMERA.SetOverrideTarget ( null );
					break;
				}

				_currentState = value;

				// next state
				switch ( _currentState ) {
				case State.idle:
					SCIENTIST.transform.position = playerStartPos.position;
					SCIENTIST.transform.rotation = playerStartPos.rotation;
					break;
				}
			}
		}

		private void State_manage_satelite () {

		}

		private void RunState () {
			switch ( currentState ) {
			case State.manage_satelite:
				State_manage_satelite ();
				break;
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
    protected void Awake () {
			instance = this;

			for ( int a=1; a<tmMachines.Length; ++a ) {
				tmMachines[a].gameObject.SetActive ( false );
			}
			for ( int a=1; a<satDishes.Length; ++a ) {
				satDishes[a].gameObject.SetActive ( false );
				satKiosks[a].gameObject.SetActive ( false );
				satKiosks[a].linkID = satDishes[a].name;
			}

			machineLinks = new List<MachineLink> ();
			machineLinks.Add ( new MachineLink ( tmMachines[0], satDishes[0] ) );
    }

		protected void Start () {
			currentState = State.idle;
		}

    protected void Update () {
			RunState ();
    }
	}
}
