﻿using System.Collections;
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

		private List<MachineLink> machineLinks;
		private SateliteDish activeSatDish;
		private TransmissionMachine activeTm;

		//--------------------------------------------------
    // state machine
    //--------------------------------------------------
		public enum State {
			start,
			idle,
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

			UI_SateliteKiosk.instance.UpdateValues (
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

			UI_TmKiosk.instance.UpdateValues ( activeTm );
		}

		private void RunState () {
			switch ( currentState ) {
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

		public void ManageSatelite ( string sateliteId ) {
			currentState = State.manage_satelite;
			SCIENTIST.enabled = false;

			foreach ( SateliteDish satDish in satDishes ) {
				if ( satDish.name.Equals ( sateliteId ) ) {
					activeSatDish = satDish;
					break;
				}
			}
			
			UI_SAT_DISH.Setup ( activeSatDish );
			CAMERA.SetLookAtTarget ( 
				activeSatDish.transform,
				playerStartPos
			);
		}

		private void StopManageSatelite () {
			currentState = State.idle;
			UI_SAT_DISH.enabled = false;
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

    //--------------------------------------------------
    // protected
    //--------------------------------------------------
    protected void Awake () {
			instance = this;

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
