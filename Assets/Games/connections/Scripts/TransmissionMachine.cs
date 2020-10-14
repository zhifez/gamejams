using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.zhifez.seagj {
	[ System.Serializable ]
	public class LinkedSatDish {
		public SateliteDish sateliteDish;
		public float signalStrengthOffset;
		public float signalSpeedOffset;

		public LinkedSatDish ( SateliteDish sateliteDish ) {
			this.sateliteDish = sateliteDish;
			this.signalStrengthOffset = 0f;
			this.signalSpeedOffset = 0f;
		}
	}

  public class TransmissionMachine : Base {
    public Transform emitter;
    public Transform receiver;
		public Color[] graphColors;
		public GameObject statusActive;
		public GameObject statusInactive;

		private const int graphFrequency = 40;
		private const float offsetMin = -2f;
		private const float offsetMax = 2f;
		private const float offsetStep = 0.05f;

    private List<LineRenderer> lineRenderers;
		private int _linkedSatDishIndex = 0;
		public int linkedSatDishIndex {
			get { return _linkedSatDishIndex; }
		}
		private List<LinkedSatDish> _linkedSatDishes;
		public List<LinkedSatDish> linkedSatDishes {
			get { return _linkedSatDishes; }
		}

		private ServiceStatus _currentServiceStatus;
		private ServiceStatus currentServiceStatus {
			get { return _currentServiceStatus; }
			set {
				if ( _currentServiceStatus != null ) {
					if ( _currentServiceStatus.Equals ( value ) ) {
						return;
					}

					GAME.RemoveServiceStatus ( _currentServiceStatus );

					PlayAudio ( "tm_machine_status_inactive" );
					statusActive.SetActive ( false );
					statusInactive.SetActive ( true );
				}

				_currentServiceStatus = value;

				if ( _currentServiceStatus != null ) {
					GAME.AddServiceStatus ( _currentServiceStatus );

					PlayAudio ( "tm_machine_status_active" );
					statusActive.SetActive ( true );
					statusInactive.SetActive ( false );
				}
			}
		}

    //--------------------------------------------------
    // private
    //--------------------------------------------------
		private void EnableWave ( string _name ) {
			if ( lineRenderers == null ) {
				lineRenderers = new List<LineRenderer> ();
			}

			LineRenderer _lineRender = null;
			foreach ( LineRenderer lr in lineRenderers ) {
				if ( !lr.gameObject.activeSelf ) {
					_lineRender = lr;
					break;
				}
			}
			if ( _lineRender == null ) {
				GameObject _waveGO = new GameObject ();
				_waveGO.transform.SetParent ( transform );
				_waveGO.transform.localPosition = Vector3.zero;

				_lineRender = _waveGO.AddComponent<LineRenderer> ();
				_lineRender.material = new Material (
					Shader.Find ( "Sprites/Default" )
				);
				_lineRender.widthMultiplier = 0.2f;
				_lineRender.positionCount = 10;
				_lineRender.alignment = LineAlignment.View;
				_lineRender.numCornerVertices = 5;
				_lineRender.numCapVertices = 5;
				lineRenderers.Add ( _lineRender );
			}

			float _alpha = 1.0f;
			Gradient _gradient = new Gradient ();
			GradientColorKey[] _colorKeys = new GradientColorKey[graphColors.Length];
			GradientAlphaKey[] _alphaKeys = new GradientAlphaKey[graphColors.Length];
			float _value = 1f / ( _colorKeys.Length - 1 );
			for ( int a=0; a<graphColors.Length; ++a ) {
				_colorKeys[a] = new GradientColorKey ( graphColors[a], _value * a );
				_alphaKeys[a] = new GradientAlphaKey ( _alpha, _value * a );
			} 
			_gradient.SetKeys ( _colorKeys, _alphaKeys );
			_lineRender.colorGradient = _gradient;
			_lineRender.name = _name;
		}

		private void DisableWave ( string name ) {
			foreach ( LineRenderer lr in lineRenderers ) {
				if ( lr.gameObject.name.Equals ( name ) ) {
					lr.gameObject.SetActive ( false );
					break;
				}
			}
		}

		private LineRenderer GetWave ( string name ) {
			foreach ( LineRenderer lr in lineRenderers ) {
				if ( lr.name.Equals ( name ) ) {
					return lr;
				}
			}
			return null;
		}

		private void RunWaves () {
			float _dist = Vector3.Distance ( emitter.position, receiver.position );

			float _min = 0f;
			float _max = 1.5f;
			List<SignalPattern> _signalPatterns = new List<SignalPattern> ();
			foreach ( LinkedSatDish lsd in linkedSatDishes ) {
				LineRenderer _wave = GetWave ( lsd.sateliteDish.name );
				_wave.gameObject.SetActive ( true );
				float _strength = lsd.signalStrengthOffset + lsd.sateliteDish.valueX;
				_strength = Mathf.Clamp ( _strength, _min, _max );
				float _speed = lsd.signalSpeedOffset + lsd.sateliteDish.valueY;
				_speed = Mathf.Clamp ( _speed, _min, _max );
				int _frequency = Mathf.RoundToInt ( _speed * ( float ) graphFrequency );
				_frequency = Mathf.Max ( 10, _frequency );
				if ( _wave.positionCount != _frequency ) {
					_wave.positionCount = _frequency;
				}
				
				_signalPatterns.Add ( new SignalPattern (
					_strength,
					_speed
				) );

				float _width = _dist / ( float ) _frequency;

				float t = Time.time * 2f;
				Vector3[] _points = new Vector3[ _frequency + 1 ];
				for ( int a=0; a<_frequency + 1; ++a ) {
					_points[a] = receiver.position;
					_points[a].x -= a * _width;
					_points[a].y += Mathf.Sin ( a + t ) * _strength;
				}
				_wave.SetPositions ( _points );
			}

			ServiceStatus _serviceStatus = DATA_PACKAGE.GetServiceStatus ( _signalPatterns.ToArray () );
			if ( _serviceStatus != null ) {
				_serviceStatus.tmMachineId = name;
			}
			currentServiceStatus = _serviceStatus;
		}

    //--------------------------------------------------
    // public
    //--------------------------------------------------
		public bool SateliteDishIsLinked ( SateliteDish _satDish ) {
			foreach ( LinkedSatDish lsd in _linkedSatDishes ) {
				if ( lsd.sateliteDish == _satDish ) {
					return true;
				}
			}
			return false;
		}

		public void LinkSateliteDish ( SateliteDish _satDish, float _strength, float _speed ) {
			EnableWave ( _satDish.name );
			LinkedSatDish _lsd = new LinkedSatDish ( _satDish );
			_lsd.signalStrengthOffset = _strength;
			_lsd.signalSpeedOffset = _speed;
			_linkedSatDishes.Add ( _lsd );
		}

		public void UnlinkSateliteDish ( SateliteDish _satDish ) {
			for ( int a=0; a<_linkedSatDishes.Count; ++a ) {
				if ( _linkedSatDishes[a].sateliteDish == _satDish ) {
					_linkedSatDishes.RemoveAt ( a );
					DisableWave ( _satDish.name );
					break;
				}
			}
		}

		public void SelectPrevLinkedSatDish () {
			--_linkedSatDishIndex;
			if ( _linkedSatDishIndex <= 0 ) {
				_linkedSatDishIndex = 0;
			}
		}

		public void SelectNextLinkedSatDish () {
			++_linkedSatDishIndex;
			if ( _linkedSatDishIndex >= _linkedSatDishes.Count * 2 - 1 ) {
				_linkedSatDishIndex = _linkedSatDishes.Count * 2 - 1;
			}
		}

		public void IncrementSignalOffset () {
			if ( _linkedSatDishes.Count <= 0 ) {
				return;
			}

			int _index = Mathf.FloorToInt ( ( float ) _linkedSatDishIndex / 2f );
			if ( _linkedSatDishIndex % 2 == 0 ) {
				_linkedSatDishes[ _index ].signalStrengthOffset += offsetStep;
				if ( _linkedSatDishes[ _index ].signalStrengthOffset >= offsetMax ) {
					_linkedSatDishes[ _index ].signalStrengthOffset = offsetMax;
				}
			}
			else {
				_linkedSatDishes[ _index ].signalSpeedOffset += offsetStep;
				if ( _linkedSatDishes[ _index ].signalSpeedOffset >= offsetMax ) {
					_linkedSatDishes[ _index ].signalSpeedOffset = offsetMax;
				}
			}
		}

		public void DecrementSignalOffset () {
			if ( _linkedSatDishes.Count <= 0 ) {
				return;
			}
			
			int _index = Mathf.FloorToInt ( ( float ) _linkedSatDishIndex / 2f );
			if ( _linkedSatDishIndex % 2 == 0 ) {
				_linkedSatDishes[ _index ].signalStrengthOffset -= offsetStep;
				if ( _linkedSatDishes[ _index ].signalStrengthOffset <= offsetMin ) {
					_linkedSatDishes[ _index ].signalStrengthOffset = offsetMin;
				}
			}
			else {
				_linkedSatDishes[ _index ].signalSpeedOffset -= offsetStep;
				if ( _linkedSatDishes[ _index ].signalSpeedOffset <= offsetMin ) {
					_linkedSatDishes[ _index ].signalSpeedOffset = offsetMin;
				}
			}
		}

    //--------------------------------------------------
    // protected
    //--------------------------------------------------
    protected void Awake () {
			_linkedSatDishes = new List<LinkedSatDish> ();
			statusActive.SetActive ( false );

			enabled = false;
    }

		protected void OnDisable () {
			_linkedSatDishes.Clear ();
			_linkedSatDishIndex = 0;
			if ( lineRenderers != null ) {
				foreach ( LineRenderer lr in lineRenderers ) {
					lr.gameObject.SetActive ( false );
				}
			}
			_currentServiceStatus = null;

			StopAllAudios ();
		}

    protected void Update () {
			RunWaves ();
    }
  }
}