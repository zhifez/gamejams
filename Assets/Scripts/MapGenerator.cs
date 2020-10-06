using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.zhifez.gamejams {
	[ System.Serializable ]
	public class MapPoint {
		public int r;
		public int c;

		public MapPoint ( int r, int c ) {
			this.r = r;
			this.c = c;
		}

		public MapPoint Clone () {
			return JsonUtility.FromJson<MapPoint> (
				JsonUtility.ToJson ( this )
			);
		}

		public bool Equals ( MapPoint mp ) {
			return ( this.r == mp.r && this.c == mp.c );
		}

		public bool ExistsIn ( MapPoint[] mps ) {
			foreach ( MapPoint mp in mps ) {
				if ( mp.Equals ( this ) ) {
					return true;
				}
			}
			return false;
		}
 	}

	public class MapGenerator : MonoBehaviour {
		public static MapGenerator instance;

		public int row = 10;
		public int column = 10;
		public float rowUnit = 1.0f;
		public float columnUnit = 1.5f;
		public int minPivot = 3;
		public int maxPivot = 5;
		public int minObstacle = 3;
		public int maxObstacle = 5;

		private string[][] _generatedMap;
		public string[][] generatedMap {
			get { return _generatedMap; }
		}

		//--------------------------------------------------
		// private
		//--------------------------------------------------
		private List<MapPoint> FindPaths ( 
			string[][] map, 
			MapPoint startPoint, 
			MapPoint endPoint,
			bool canCrossover = false
		) {
			// 1. Setup _tempMap
			int[][] _tempMap = new int[ map.Length ][];
			for ( int r=0; r<map.Length; ++r ) {
				_tempMap[r] = new int[ map[r].Length ];
				for ( int c=0; c<map[r].Length; ++c ) {
					string _mapState = map[r][c];
					switch ( _mapState ) {
					case null:
					case "start":
					case "end":
						_tempMap[r][c] = 0;
						break;

					default:
						_tempMap[r][c] = canCrossover ? 0 : -1;
						break;
					}
				}
			}

			// 2. Populate _tempMap with path find numbers from startPoint to endPoint
			int _iteration = 0;
			int _maxIteration = _tempMap.Length * _tempMap[0].Length;
			int _pathScore = 1;
			_tempMap[ startPoint.r ][ startPoint.c ] = _pathScore;
			bool _reachedEnd = false;
			while ( !_reachedEnd
				&& _iteration < _maxIteration ) {
				// a. Get all MapPoints in _tempMap that has the same _pathScore value
				List<MapPoint> _curPoints = new List<MapPoint> ();
				for ( int r=0; r<_tempMap.Length; ++r ) {
					for ( int c=0; c<_tempMap[r].Length; ++c ) {
						if ( _tempMap[r][c] == _pathScore ) {
							_curPoints.Add ( new MapPoint ( r, c ) );
						}
					}
				}
				if ( _curPoints.Count <= 0 ) {
					break;
				}

				++_pathScore;
				
				// b. Plot _curPath to points in _tempMap
				for ( int a=0; a<_curPoints.Count; ++a ) {
					for ( int b=0; b<4; ++b ) {
						MapPoint _mp = _curPoints[a].Clone ();
						switch ( b ) {
						case 0: // up
							--_mp.c;
							break;

						case 1: // down
							++_mp.c;
							break;

						case 2: // left
							--_mp.r;
							break;

						case 3: // right
							++_mp.r;
							break;
						}

						if ( _mp.r >= 0 && _mp.r < _tempMap.Length 
							&& _mp.c >= 0 && _mp.c < _tempMap[0].Length ) {
							if ( _tempMap[ _mp.r ][ _mp.c ] == 0
								&& map[ _mp.r ][ _mp.c ] != "start" ) {
								_tempMap[ _mp.r ][ _mp.c ] = _pathScore;

								if ( _mp.Equals ( endPoint ) ) {
									_reachedEnd = true;
								}
							}
						}
					}
				}

				++_iteration;
			}
			
			if ( !_reachedEnd ) {
				return null;
			}

			// 3. Reverse and track points until startPoint is reached
			List<MapPoint> _paths = new List<MapPoint> ();
			_paths.Add ( endPoint.Clone () );
			--_pathScore; // ignore end point
			while ( _pathScore > 0 ) {
				for ( int a=0; a<4; ++a ) {
					MapPoint _trackPoint = _paths[0].Clone ();
					switch ( a ) {
					case 0: // up
						--_trackPoint.c;
						break;

					case 1: // down
						++_trackPoint.c;
						break;

					case 2: // left
						--_trackPoint.r;
						break;

					case 3: // right
						++_trackPoint.r;
						break;
					}

					if ( _trackPoint.r >= 0 && _trackPoint.r < _tempMap.Length 
						&& _trackPoint.c >= 0 && _trackPoint.c < _tempMap[0].Length ) {
						if ( _tempMap[ _trackPoint.r ][ _trackPoint.c ] == _pathScore ) {
							_paths.Insert ( 0, _trackPoint );
							break;
						}
					}
				}

				--_pathScore;
			}

			return _paths;
		}

		//--------------------------------------------------
		// public
		//--------------------------------------------------
		private List<MapPoint> paths;
		public void Generate () {
			_generatedMap = new string[ row ][];
			for ( int r=0; r<row; ++r ) {
				_generatedMap[r] = new string[ column ];
				for ( int c=0; c<column; ++c ) {
					_generatedMap[r][c] = null;
				}
			}

			int _obstacle = Random.Range ( minObstacle, maxObstacle );
			List<MapPoint> _obsPoints = new List<MapPoint> ();
			for ( int a=0; a<_obstacle; ++a ) {
				MapPoint _obs = new MapPoint (
					Random.Range ( 0, row ),
					Random.Range ( 0, column )
				);
				while ( _obs.ExistsIn ( _obsPoints.ToArray () ) ) {
					_obs = new MapPoint (
						Random.Range ( 0, row - 0 ),
						Random.Range ( 0, column - 0 )
					);
				}
				_obsPoints.Add ( _obs );

				_generatedMap[ _obs.r ][ _obs.c ] = "obstacle";
			}
			
			// generate rooms
			MapPoint _s = new MapPoint ( 
				Random.Range ( 0, row ), 
				Random.Range ( 0, column ) );
			MapPoint _e = new MapPoint ( 
				Random.Range ( 0, row ), 
				Random.Range ( 0, column ) 
			);
			while ( _e.Equals ( _s ) ) {
				_e = new MapPoint ( 
					Random.Range ( 0, row ), 
					Random.Range ( 0, column ) 
				);
			}
			
			_generatedMap[ _s.r ][ _s.c ] = "start";
			_generatedMap[ _e.r ][ _e.c ] = "end";

			int _pivot = Random.Range ( minPivot, maxPivot );
			List<int> _pivotRows = new List<int> ();
			for ( int a=1; a<column - 1; ++a ) {
				_pivotRows.Add ( a );
			}
			MapPoint _startPath = _s.Clone ();
			paths = new List<MapPoint> ();
			for ( int a=0; a<_pivot; ++a ) {
				int _padding = 0;// Mathf.FloorToInt ( ( float ) column / 2 ) - ( a + 2 );
				if ( _padding <= 0 ) {
					_padding = 0;
				}
				
				int _randRow = _pivotRows[ Random.Range ( 0, _pivotRows.Count ) ];
				MapPoint _mp = new MapPoint (
					_randRow,
					Random.Range ( _padding, column - _padding )
				);
				while ( _mp.ExistsIn ( paths.ToArray () ) ) {
					_mp = new MapPoint (
						_randRow,
						Random.Range ( _padding, column - _padding )
					);
				}
				Debug.Log ( a + "/" + _pivot + ": " + _mp.r + ", " + _mp.c );
				FindAndUpdatePaths ( _startPath, _mp );

				_pivotRows.Remove ( _randRow );
				_startPath = _mp;
			}
			Debug.Log ( "startpath: " + _startPath.r + ", " + _startPath.c );
			FindAndUpdatePaths ( _startPath, _e );
		}

		private void FindAndUpdatePaths ( MapPoint start, MapPoint end ) {
			List<MapPoint> _pathsFound = FindPaths ( _generatedMap, start, end );
			if ( _pathsFound == null ) {
				_pathsFound = FindPaths ( _generatedMap, start, end, true );
			}

			if ( _pathsFound != null ) {
				for ( int p=0; p<_pathsFound.Count; ++p ) {
					if ( !_pathsFound[p].ExistsIn ( paths.ToArray () ) ) {
						paths.Add ( _pathsFound[p] );
						if ( _generatedMap[ _pathsFound[p].r ][ _pathsFound[p].c ] == null
							|| _generatedMap[ _pathsFound[p].r ][ _pathsFound[p].c ] == "obstacle" ) {
							if ( _pathsFound[p].Equals ( start )
								|| _pathsFound[p].Equals ( end ) ) {
								_generatedMap[ _pathsFound[p].r ][ _pathsFound[p].c ] = "pivot";
							}
							else {
								_generatedMap[ _pathsFound[p].r ][ _pathsFound[p].c ] = "room";
							}
						}
					}
				}
			}
		}

		//--------------------------------------------------
		// protected
		//--------------------------------------------------
		protected void Update () {
			if ( Input.GetKeyDown ( KeyCode.Space ) ) {
				Generate ();
			}
		}

		protected void OnDrawGizmos() {
			if ( _generatedMap != null ) {
				for ( int r=0; r<_generatedMap.Length; ++r ) {
					for ( int c=0; c<_generatedMap[r].Length; ++c ) {
						// _generatedMap[r][c] = 0;
						Vector3 _pos = new Vector3 ( c * columnUnit, 0, r * rowUnit );
						bool _drawWire = true;
						switch ( _generatedMap[r][c] ) {
						case "start":
						case "pivot":
							Gizmos.color = Color.black;
							break;

						case "end":
							Gizmos.color = Color.red;
							break;

						case "room":
							Gizmos.color = Color.green;
							break;

						case "obstacle":
							_drawWire = false;
							Gizmos.color = Color.black;
							break;

						default:
							_drawWire = false;
							Gizmos.color = Color.white;
							break;
						}
						
						if ( _drawWire ) {
							Gizmos.DrawWireCube ( _pos, new Vector3 ( columnUnit, 1, rowUnit ) );	
						}
						else {
							Gizmos.DrawCube ( _pos, new Vector3 ( columnUnit, 1, rowUnit ) );	
						}			
					}
				}
			}

			if ( paths != null ) {
				Gizmos.color = Color.white;
				for ( int a=0; a<paths.Count; ++a ) {
					MapPoint mp = paths[a];
					Vector3 _pos = new Vector3 ( mp.c * columnUnit, 0, mp.r * rowUnit );
					Gizmos.DrawWireSphere ( _pos, 0.05f + ( a + 1 ) * 0.005f );		
				}
			}
		}
	}
}