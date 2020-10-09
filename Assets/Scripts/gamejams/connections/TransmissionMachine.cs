using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.zhifez.seagj {
  public class TransmissionMachine : MonoBehaviour {
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