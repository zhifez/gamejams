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

		public int graphFrequency = 20;
		private int pseudoGraphFrequency {
			get {
				return graphFrequency + 1;
			}
		}
		private const float graphStrength = 1f;
		private const float graphSpeed = 5f;

    private List<LineRenderer> lineRenderers;
		private int _linkedSatDishIndex = 0;
		public int linkedSatDishIndex {
			get { return _linkedSatDishIndex; }
		}
		private List<LinkedSatDish> _linkedSatDishes;
		public List<LinkedSatDish> linkedSatDishes {
			get { return _linkedSatDishes; }
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
				_lineRender.positionCount = pseudoGraphFrequency;
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
			float _width = _dist / ( float ) graphFrequency;

			foreach ( LinkedSatDish lsd in linkedSatDishes ) {
				LineRenderer _wave = GetWave ( lsd.sateliteDish.name );
				float _strength = lsd.sateliteDish.strength;
				_strength += lsd.signalStrengthOffset;
				_strength *= graphStrength;
				float _speed = 1 + lsd.sateliteDish.speed;
				_speed += lsd.signalSpeedOffset;
				_speed *= graphSpeed;

				float t = Time.time * _speed;
				Vector3[] _points = new Vector3[ pseudoGraphFrequency ];
				for ( int a=0; a<pseudoGraphFrequency; ++a ) {
					_points[a] = receiver.position;
					_points[a].x -= a * _width;
					_points[a].y += Mathf.Sin ( a + t ) * _strength;
				}
				_wave.SetPositions ( _points );
			}
		}

    //--------------------------------------------------
    // public
    //--------------------------------------------------
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
			int _index = Mathf.FloorToInt ( ( float ) _linkedSatDishIndex / 2f );
			if ( _linkedSatDishIndex % 2 == 0 ) {
				_linkedSatDishes[ _index ].signalStrengthOffset += 0.1f;
				if ( _linkedSatDishes[ _index ].signalStrengthOffset >= 1f ) {
					_linkedSatDishes[ _index ].signalStrengthOffset = 1f;
				}
			}
			else {
				_linkedSatDishes[ _index ].signalSpeedOffset += 0.1f;
				if ( _linkedSatDishes[ _index ].signalSpeedOffset >= 1f ) {
					_linkedSatDishes[ _index ].signalSpeedOffset = 1f;
				}
			}
		}

		public void DecrementSignalOffset () {
			int _index = Mathf.FloorToInt ( ( float ) _linkedSatDishIndex / 2f );
			if ( _linkedSatDishIndex % 2 == 0 ) {
				_linkedSatDishes[ _index ].signalStrengthOffset -= 0.1f;
				if ( _linkedSatDishes[ _index ].signalStrengthOffset <= 0 ) {
					_linkedSatDishes[ _index ].signalStrengthOffset = 0;
				}
			}
			else {
				_linkedSatDishes[ _index ].signalSpeedOffset -= 0.1f;
				if ( _linkedSatDishes[ _index ].signalSpeedOffset <= 0 ) {
					_linkedSatDishes[ _index ].signalSpeedOffset = 0;
				}
			}
		}

    //--------------------------------------------------
    // protected
    //--------------------------------------------------
    protected void Awake () {
			_linkedSatDishes = new List<LinkedSatDish> ();
    }

    protected void Update () {
			RunWaves ();
    }
  }
}