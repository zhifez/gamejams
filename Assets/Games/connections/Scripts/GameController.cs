using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

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
		public Transform endLookPos;
		public TransmissionMachine[] tmMachines;
		public Kiosk[] tmKiosks;
		public SateliteDish[] satDishes;
		public Kiosk[] satKiosks;

		private TransmissionMachine activeTmMachine;
		private SateliteDish activeSatDish;
		public List<ServiceStatus> serviceStatuses;
		private int _enabledTmMachineCount = 0;
		public int enabledTmMachineCount {
			get { return _enabledTmMachineCount; }
			set {
				_enabledTmMachineCount = value;

				for ( int a=0; a<tmMachines.Length; ++a ) {
					tmMachines[a].gameObject.SetActive ( a < _enabledTmMachineCount );
					tmMachines[a].enabled = ( a < _enabledTmMachineCount );
					tmKiosks[a].gameObject.SetActive ( a < _enabledTmMachineCount );
				}
			}
		}
		private int _enabledSatDishCount;
		public int enabledSatDishCount {
			get { return _enabledSatDishCount; }
			set {
				_enabledSatDishCount = value;

				for ( int a=0; a<satDishes.Length; ++a ) {
					satDishes[a].enabled = ( a < _enabledSatDishCount );
					satKiosks[a].gameObject.SetActive ( a < _enabledSatDishCount );
				}
			}
		}

		//--------------------------------------------------
    // state machine
    //--------------------------------------------------
		public enum State {
			none,
			start,
			idle,
			manage_overall,
			manage_satelite,
			manage_tm,
			results
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
					SCIENTIST.enabled = false;
					SCIENTIST.Respawn ( playerStartPos );
					CAMERA.SetLookAtTarget ( endLookPos );
					UI_GAME.SetLabelsAlpha ( 0f );
					UI_START.enabled = true;
					UI_END.enabled = false;
					break;

				case State.idle:
					UI_GAME.SetLabelsAlpha ( 1.0f );
					UI_END.enabled = false;
					break;

				case State.manage_overall:
				case State.manage_satelite:
				case State.manage_tm:
				case State.results:
					UI_GAME.SetLabelsAlpha ( 0.2f );
					break;
				}
			}
		}

		private void State_start () {
			if ( Input.GetKeyDown ( KeyCode.Space ) ) {
				AudioController.Play ( "ui_btn_direction_section" );
				UI_START.enabled = false;
				PLAYER_STATS.BeginTimer ();
				CAMERA.SetLookAtTarget ( null );
				UI_GAME.SetLabelsAlpha ( 1.0f );
				SCIENTIST.enabled = true;
				DATA_PACKAGE.enabled = true;
				currentState = State.idle;
			}
		}

		private void State_manage_overall () {
			if ( Input.GetKeyDown ( KeyCode.Escape ) ) {
				AudioController.Play ( "ui_btn_direction" );
				StopManageOverall ();
				return;
			}

			if ( Input.GetKeyDown ( KeyCode.Q ) ) {
				AudioController.Play ( "ui_btn_direction_section" );
				UI_MAIN.GoToPrevSection ();
			}
			if ( Input.GetKeyDown ( KeyCode.E ) ) {
				AudioController.Play ( "ui_btn_direction_section" );
				UI_MAIN.GoToNextSection ();
			}
		}

		private void State_manage_satelite () {
			if ( Input.GetKeyDown ( KeyCode.Escape ) ) {
				AudioController.Play ( "ui_btn_direction" );
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
				AudioController.Play ( "ui_btn_direction" );
				StopManageTM ();
				return;
			}

			if ( Input.GetKeyDown ( KeyCode.W ) 
				|| Input.GetKeyDown ( KeyCode.UpArrow ) ) {
				AudioController.Play ( "ui_btn_direction" );
				activeTmMachine.SelectPrevLinkedSatDish ();
			}
			if ( Input.GetKeyDown ( KeyCode.S ) 
				|| Input.GetKeyDown ( KeyCode.DownArrow ) ) {
				AudioController.Play ( "ui_btn_direction" );
				activeTmMachine.SelectNextLinkedSatDish ();
			}
			if ( Input.GetKeyDown ( KeyCode.A ) 
				|| Input.GetKeyDown ( KeyCode.LeftArrow ) ) {
				AudioController.Play ( "ui_btn_direction" );
				activeTmMachine.DecrementSignalOffset ();
			}
			if ( Input.GetKeyDown ( KeyCode.D ) 
				|| Input.GetKeyDown ( KeyCode.RightArrow ) ) {
				AudioController.Play ( "ui_btn_direction" );
				activeTmMachine.IncrementSignalOffset ();
			}

			UI_TM.UpdateValues ( activeTmMachine );
		}

		private void RunState () {
			switch ( currentState ) {
			case State.start:
				State_start ();
				break;

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
					activeTmMachine = tm;
					break;
				}
			}

			UI_TM.Setup ( activeTmMachine );
			CAMERA.SetLookAtTarget ( activeTmMachine.transform );
		}

		private void StopManageTM () {
			currentState = State.idle;
			UI_TM.enabled = false;
			SCIENTIST.enabled = true;
			activeTmMachine = null;
			CAMERA.SetLookAtTarget ( null );
		}

		public int TotalEnabledAllServices () {
			List<DataPackage.Service> _services = new List<DataPackage.Service> ();
			foreach ( ServiceStatus ss in serviceStatuses ) {
				if ( !_services.Contains ( ss.service ) ) {
					_services.Add ( ss.service );
				}
			}
			return _services.Count;
		}

		public bool HasEnabledAllServices () {
			List<DataPackage.Service> _services = new List<DataPackage.Service> ();
			foreach ( ServiceStatus ss in serviceStatuses ) {
				if ( !_services.Contains ( ss.service ) ) {
					_services.Add ( ss.service );
				}
			}
			return ( _services.Count >= DATA_PACKAGE.serviceSignalPatterns.Length );
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

		public int GetSameServiceCount ( DataPackage.Service _service ) {
			int _total = 0;
			foreach ( ServiceStatus ss in serviceStatuses ) {
				if ( ss.service == _service ) {
					++_total;
				}
			}
			return _total;
		}

		public float GetServiceMultiplier ( DataPackage.Service _service ) {
			float _multiplier = 0f;
			foreach ( ServiceStatus ss in serviceStatuses ) {
				if ( ss.service == _service ) {
					_multiplier += ss.isStable ? 1f : 0.5f;
				}
			}
			return _multiplier;
		}

		public void StartGame () {
			currentState = State.start;
		}

		public void EndGame () {
			currentState = State.results;
			SCIENTIST.Respawn ( playerStartPos );
			SCIENTIST.enabled = false;
			DATA_PACKAGE.enabled = false;
			UI_MAIN.enabled = false;
			UI_TM.enabled = false;
			UI_SAT.enabled = false;
			UI_END.enabled = true;
			CAMERA.SetLookAtTarget ( endLookPos );
		}

		public void RestartGame () {
			UI_END.enabled = false;
			foreach ( TransmissionMachine tm in tmMachines ) {
				tm.enabled = false;
			}
			enabledTmMachineCount = 1;
			
			foreach ( SateliteDish sd in satDishes ) {
				sd.enabled = false;
			}
			enabledSatDishCount = 1;

			tmMachines[0].LinkSateliteDish ( satDishes[0], 0f, 0f );
			serviceStatuses.Clear ();
			DATA_PACKAGE.Reboot ();
			PLAYER_STATS.Init ();

			currentState = State.start;
		}

    //--------------------------------------------------
    // protected
    //--------------------------------------------------
    protected void Awake () {
			instance = this;
		}

		protected void Start () {
			for ( int a=0; a<tmMachines.Length; ++a ) {
				tmKiosks[a].linkId = tmMachines[a].name;
			}
			enabledTmMachineCount = 1;

			for ( int a=0; a<satDishes.Length; ++a ) {
				satKiosks[a].linkId = satDishes[a].name;
			}
			enabledSatDishCount = 1;

			serviceStatuses = new List<ServiceStatus> ();

			DOVirtual.DelayedCall ( 1f, () => {
				RestartGame ();
			} );

			AudioController.PlayAmbienceSound ( "factory_ambience" );
		}

    protected void Update () {
			RunState ();
    }
	}
}
