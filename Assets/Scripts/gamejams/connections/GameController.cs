using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
		public Kiosk[] tmKiosks;
		public SateliteDish[] satDishes;
		public Kiosk[] satKiosks;

		private SateliteDish activeSatDish;
		private TransmissionMachine activeTm;
		public List<ServiceStatus> serviceStatuses;

		//--------------------------------------------------
    // state machine
    //--------------------------------------------------
		public enum State {
			none,
			start,
			idle,
			manage_overall,
			manage_satelite,
			manage_tm
		}
		private State _currentState = State.idle;
		public State currentState {
			get { return _currentState; }
			set {
				if ( _currentState == value ) {
					return;
				}
				
				_currentState = value;

				// next state
				switch ( _currentState ) {
				case State.start:
					SCIENTIST.transform.position = playerStartPos.position;
					SCIENTIST.transform.rotation = playerStartPos.rotation;
					currentState = State.idle;
					DATA_PACKAGE.enabled = true;
					break;

				case State.idle:
					break;
				}
			}
		}

		private void State_manage_overall () {
			if ( Input.GetKeyDown ( KeyCode.Escape ) ) {
				StopManageOverall ();
				return;
			}

			if ( Input.GetKeyDown ( KeyCode.Q ) ) {
				UI_MAIN.GoToPrevSection ();
			}
			if ( Input.GetKeyDown ( KeyCode.E ) ) {
				UI_MAIN.GoToNextSection ();
			}
		}

		private void State_manage_satelite () {
			if ( Input.GetKeyDown ( KeyCode.Escape ) ) {
				StopManageSatelite ();
				return;
			}

			float _offset = 0f;
			if ( INPUT_HOR < -_offset || INPUT_HOR > _offset
				|| INPUT_VER < -_offset || INPUT_VER > _offset ) {
				if ( INPUT_HOR < -_offset || INPUT_HOR > _offset ) {
					activeSatDish.RotateDish ( INPUT_HOR, "Horizontal" );
				}

				if ( INPUT_VER < -_offset || INPUT_VER > _offset ) {
					activeSatDish.RotateDish ( INPUT_VER, "Vertical" );
				}
			}
			else {	
				activeSatDish.RotateDish ( 0 );
			}

			UI_SAT.UpdateValues (
				activeSatDish.valueX,
				activeSatDish.valueY
			);
		}

		private void State_manage_tm () {
			if ( Input.GetKeyDown ( KeyCode.Escape ) ) {
				StopManageTM ();
				return;
			}

			if ( Input.GetKeyDown ( KeyCode.W ) 
				|| Input.GetKeyDown ( KeyCode.UpArrow ) ) {
				activeTm.SelectPrevLinkedSatDish ();
			}
			if ( Input.GetKeyDown ( KeyCode.S ) 
				|| Input.GetKeyDown ( KeyCode.DownArrow ) ) {
				activeTm.SelectNextLinkedSatDish ();
			}
			if ( Input.GetKeyDown ( KeyCode.A ) 
				|| Input.GetKeyDown ( KeyCode.LeftArrow ) ) {
				activeTm.DecrementSignalOffset ();
			}
			if ( Input.GetKeyDown ( KeyCode.D ) 
				|| Input.GetKeyDown ( KeyCode.RightArrow ) ) {
				activeTm.IncrementSignalOffset ();
			}

			UI_TM.UpdateValues ( activeTm );
		}

		private void RunState () {
			switch ( currentState ) {
			case State.manage_overall:
				State_manage_overall ();
				break;

			case State.manage_satelite:
				State_manage_satelite ();
				break;

			case State.manage_tm:
				State_manage_tm ();
				break;
			}
		}

		//--------------------------------------------------
    // private
    //--------------------------------------------------

    //--------------------------------------------------
    // public
    //--------------------------------------------------
		public bool isIdle {
			get { return currentState == State.idle; }
		}

		public void ManageOverall () {
			currentState = State.manage_overall;
			UI_MAIN.enabled = true;
			SCIENTIST.enabled = false;
			CAMERA.SetLookAtTarget ( transform );
		}

		private void StopManageOverall () {
			currentState = State.idle;
			UI_MAIN.enabled = false;
			SCIENTIST.enabled = true;
			CAMERA.SetLookAtTarget ( null );
		}

		public void ManageSatelite ( string sateliteId ) {
			currentState = State.manage_satelite;
			SCIENTIST.enabled = false;

			foreach ( SateliteDish satDish in satDishes ) {
				if ( satDish.name.Equals ( sateliteId ) ) {
					activeSatDish = satDish;
					break;
				}
			}
			
			UI_SAT.Setup ( activeSatDish );
			CAMERA.SetLookAtTarget ( 
				activeSatDish.transform,
				playerStartPos
			);
		}

		private void StopManageSatelite () {
			currentState = State.idle;
			UI_SAT.enabled = false;
			SCIENTIST.enabled = true;
			activeSatDish = null;
			CAMERA.SetLookAtTarget ( null );
		}

		public void ManageTM ( string tmId ) {
			currentState = State.manage_tm;
			SCIENTIST.enabled = false;

			foreach ( TransmissionMachine tm in tmMachines ) {
				if ( tm.name.Equals ( tmId ) ) {
					activeTm = tm;
					break;
				}
			}

			UI_TM.Setup ( activeTm );
			CAMERA.SetLookAtTarget ( activeTm.transform );
		}

		private void StopManageTM () {
			currentState = State.idle;
			UI_TM.enabled = false;
			SCIENTIST.enabled = true;
			activeTm = null;
			CAMERA.SetLookAtTarget ( null );
		}

		public void AddServiceStatus ( ServiceStatus _status ) {
			foreach ( ServiceStatus ss in serviceStatuses ) {
				if ( ss.service == _status.service
					&& ss.tmMachineId.Equals ( _status.tmMachineId ) ) {
					ss.isStable = _status.isStable;
					return;
				}
			}

			serviceStatuses.Add ( _status );
		}

		public void RemoveServiceStatus ( ServiceStatus _status ) {
			for ( int a=0; a<serviceStatuses.Count; ++a ) {
				ServiceStatus ss = serviceStatuses[a];
				if ( ss.service == _status.service
					&& ss.tmMachineId.Equals ( _status.tmMachineId ) ) {
					serviceStatuses.RemoveAt ( a );
					break;
				}
			}
		}

		public bool HasService ( DataPackage.Service _service ) {
			foreach ( ServiceStatus ss in serviceStatuses ) {
				if ( ss.service == _service ) {
					return true;
				}
			}
			return false;
		}

		public float GetServiceMultipler ( DataPackage.Service _service ) {
			float _multiplier = 0f;
			foreach ( ServiceStatus ss in serviceStatuses ) {
				if ( ss.service == _service ) {
					_multiplier += ss.isStable ? 1f : 0.5f;
				}
			}
			return _multiplier;
		}

    //--------------------------------------------------
    // protected
    //--------------------------------------------------
    protected void Awake () {
			instance = this;
		}

		protected void Start () {
			for ( int a=0; a<tmMachines.Length; ++a ) {
				if ( a > 0 ) {
					tmMachines[a].gameObject.SetActive ( false );
					tmKiosks[a].gameObject.SetActive ( false );
				}
				tmKiosks[a].linkId = tmMachines[a].name;
			}
			for ( int a=0; a<satDishes.Length; ++a ) {
				if ( a > 0 ) {
					satDishes[a].enabled = false;
					satKiosks[a].gameObject.SetActive ( false );
				}
				satKiosks[a].linkId = satDishes[a].name;
			}

			tmMachines[0].LinkSateliteDish ( satDishes[0], 0f, 0f );
			// tmMachines[0].LinkSateliteDish ( satDishes[1], 0f, 0f );
			// tmMachines[1].LinkSateliteDish ( satDishes[0], 0f, 0f );

			serviceStatuses = new List<ServiceStatus> ();
			
			currentState = State.start;
		}

    protected void Update () {
			RunState ();
    }
	}
}
