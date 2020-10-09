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

			float alpha = 1.0f;
			Gradient gradient = new Gradient ();
			gradient.SetKeys (
				new GradientColorKey[] { 
					new GradientColorKey ( graphColors[0], 0.0f ), 
					new GradientColorKey ( graphColors[1], 1.0f ), 
					// new GradientColorKey ( c3, 1.0f ) 
				},
				new GradientAlphaKey[] { 
					new GradientAlphaKey ( alpha, 0.0f ), 
					new GradientAlphaKey ( alpha, 1.0f ), 
					// new GradientAlphaKey ( alpha, 1.0f ) 
				}
			);
			lineRenderer.colorGradient = gradient;
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