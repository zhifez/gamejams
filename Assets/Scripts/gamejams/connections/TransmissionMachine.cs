using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.zhifez.seagj {
	[ System.Serializable ]
	public class LinkedSatDish {
		public SateliteDish sateliteDish;
		public int signalStrengthOffset;
		public int signalSpeedOffset;

		public LinkedSatDish ( SateliteDish sateliteDish ) {
			this.sateliteDish = sateliteDish;
			this.signalStrengthOffset = 0;
			this.signalSpeedOffset = 0;
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
		public float graphStrength = 1f;
		public float graphSpeed = 1f;

    private LineRenderer lineRenderer;
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

    //--------------------------------------------------
    // public
    //--------------------------------------------------
		public void LinkSateliteDish ( SateliteDish _satDish ) {
			_linkedSatDishes.Add (
				new LinkedSatDish ( _satDish )
			);
		}

		public void UnlinkSateliteDish ( SateliteDish _satDish ) {
			for ( int a=0; a<_linkedSatDishes.Count; ++a ) {
				if ( _linkedSatDishes[a].sateliteDish == _satDish ) {
					_linkedSatDishes.RemoveAt ( a );
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
				++_linkedSatDishes[ _index ].signalStrengthOffset;
				if ( _linkedSatDishes[ _index ].signalStrengthOffset >= 10 ) {
					_linkedSatDishes[ _index ].signalStrengthOffset = 10;
				}
			}
			else {
				++_linkedSatDishes[ _index ].signalSpeedOffset;
				if ( _linkedSatDishes[ _index ].signalSpeedOffset >= 10 ) {
					_linkedSatDishes[ _index ].signalSpeedOffset = 10;
				}
			}
		}

		public void DecrementSignalOffset () {
			int _index = Mathf.FloorToInt ( ( float ) _linkedSatDishIndex / 2f );
			if ( _linkedSatDishIndex % 2 == 0 ) {
				--_linkedSatDishes[ _index ].signalStrengthOffset;
				if ( _linkedSatDishes[ _index ].signalStrengthOffset <= 0 ) {
					_linkedSatDishes[ _index ].signalStrengthOffset = 0;
				}
			}
			else {
				--_linkedSatDishes[ _index ].signalSpeedOffset;
				if ( _linkedSatDishes[ _index ].signalSpeedOffset <= 0 ) {
					_linkedSatDishes[ _index ].signalSpeedOffset = 0;
				}
			}
		}

    //--------------------------------------------------
    // protected
    //--------------------------------------------------
    protected void Awake () {
			lineRenderer = GetComponentInChildren<LineRenderer> ();
			lineRenderer.material = new Material (
				Shader.Find ( "Sprites/Default" )
			);
			lineRenderer.widthMultiplier = 0.2f;
			lineRenderer.positionCount = pseudoGraphFrequency;
			lineRenderer.alignment = LineAlignment.View;
			lineRenderer.numCornerVertices = 5;
			lineRenderer.numCapVertices = 5;

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
			lineRenderer.colorGradient = _gradient;

			_linkedSatDishes = new List<LinkedSatDish> ();
    }

    protected void Update () {
			float _dist = Vector3.Distance ( emitter.position, receiver.position );
			float _width = _dist / ( float ) graphFrequency;
			Vector3[] _points = new Vector3[ pseudoGraphFrequency ];
			float t = Time.time * graphSpeed;
			for ( int a=0; a<pseudoGraphFrequency; ++a ) {
				_points[a] = receiver.position;
				_points[a].x -= a * _width;
				_points[a].y += Mathf.Sin ( a + t ) * graphStrength;
			}
			lineRenderer.SetPositions ( _points );
    }
  }
}