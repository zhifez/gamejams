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

	public class GameController : Base {
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
		private SateliteDish activeSatDish;

		//--------------------------------------------------
    // state machine
    //--------------------------------------------------
		public enum State {
			start,
			idle,
			manage_satelite
		}
		private State _currentState = State.idle;
		public State currentState {
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
				case State.start:
					SCIENTIST.transform.position = playerStartPos.position;
					SCIENTIST.transform.rotation = playerStartPos.rotation;
					currentState = State.idle;
					break;

				case State.idle:
					break;
				}
			}
		}

		private void State_manage_satelite () {
			if ( Input.GetKeyDown ( KeyCode.Escape ) ) {
				StopManageSatelite ();
				return;
			}

			float _offset = 0f;
			if ( INPUT_HOR < -_offset || INPUT_HOR > _offset ) {
				activeSatDish.RotateDish ( INPUT_HOR, "Horizontal" );
			}

			if ( INPUT_VER < -_offset || INPUT_VER > _offset ) {
				activeSatDish.RotateDish ( INPUT_VER, "Vertical" );
			}

			if ( INPUT_HOR >= -_offset && INPUT_HOR <= _offset
				&& INPUT_VER >= -_offset && INPUT_VER <= _offset ) {
				activeSatDish.RotateDish ( 0 );
			}
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
		public void ManageSatelite ( string sateliteId ) {
			currentState = State.manage_satelite;
			UI_SateliteKiosk.instance.enabled = true;
			SCIENTIST.enabled = false;

			foreach ( SateliteDish satDish in satDishes ) {
				if ( satDish.name.Equals ( sateliteId ) ) {
					activeSatDish = satDish;
					break;
				}
			}

			CAMERA.SetLookAtTarget ( activeSatDish.transform );
		}

		public void StopManageSatelite () {
			currentState = State.idle;
			UI_SateliteKiosk.instance.enabled = false;
			SCIENTIST.enabled = true;
			activeSatDish = null;

			CAMERA.SetLookAtTarget ( null );
		}

    //--------------------------------------------------
    // protected
    //--------------------------------------------------
    protected void Awake () {
			instance = this;

			for ( int a=1; a<tmMachines.Length; ++a ) {
				tmMachines[a].gameObject.SetActive ( false );
			}
			for ( int a=0; a<satDishes.Length; ++a ) {
				if ( a > 0 ) {
					satDishes[a].enabled = false;
					satKiosks[a].gameObject.SetActive ( false );
				}
				satKiosks[a].linkId = satDishes[a].name;
			}

			machineLinks = new List<MachineLink> ();
			machineLinks.Add ( new MachineLink ( tmMachines[0], satDishes[0] ) );
    }

		protected void Start () {
			currentState = State.start;
		}

    protected void Update () {
			RunState ();
    }
	}
}
