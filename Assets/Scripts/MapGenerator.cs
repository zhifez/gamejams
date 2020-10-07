﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.zhifez.gamejams {
	[ System.Serializable ]
	public class MapPoint {
		public int r;
		public int c;
		public int height; // for room generation use

		public MapPoint ( int r, int c ) {
			this.r = r;
			this.c = c;
		}

		public string Log () {
			return r + ", " + c;
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

		public Vector3 GetPosition ( float unitSize, int rowUnit, int columnUnit, int heightUnit ) {
			return new Vector3 (
				c * columnUnit,
				height * heightUnit,
				r * rowUnit
			) * unitSize;
		}
 	}

	[ System.Serializable ]
	public class RoomAssets {
		// public string name;
		public GameObject[] topLeftGO;
		public GameObject[] topRightGO;
		public GameObject[] bottomLeftGO;
		public GameObject[] bottomRightGO;
		public GameObject[] topGO;
		public GameObject[] bottomGO;
		public GameObject[] leftGO;
		public GameObject[] rightGO;
		public GameObject[] centerGO;
	}

	public class MapGenerator : MonoBehaviour {
		public int row = 10;
		public int column = 10;
		public float unitSize = 5f;
		public int rowUnit = 3;
		public int columnUnit = 5;
		public int heightUnit = 1;
		public int minPivot = 7;
		public int maxPivot = 10;
		public int minObstacle = 3;
		public int maxObstacle = 5;

		public RoomAssets defaultRoomAssets;

		private string[][] _generatedMap;
		public string[][] generatedMap {
			get { return _generatedMap; }
		}
		private List<MapPoint> _mapPaths;
		public List<MapPoint> mapPaths {
			get { return _mapPaths; }
		}
		private MapPoint _startPoint;
		public MapPoint startPoint {
			get { return _startPoint; }
		}
		private MapPoint _endPoint;
		public MapPoint endPoint {
			get { return _endPoint; }
		}

		//--------------------------------------------------
		// private
		//--------------------------------------------------
		private int[][] buggedTempMap;
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
					if ( canCrossover ) {
						buggedTempMap = _tempMap;
						Debug.Log ( "from: " + startPoint.Log () + "; to: " + endPoint.Log () + " (" + _pathScore + ")" );
					}
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

		private void GenerateMap () {
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
			int _padding = 1;
			_startPoint = new MapPoint ( 
				Random.Range ( _padding, row - _padding ), 
				Random.Range ( _padding, column - _padding ) 
			);
			while ( _startPoint.ExistsIn ( _obsPoints.ToArray () ) ) {
				_startPoint = new MapPoint ( 
					Random.Range ( _padding, row - _padding ), 
					Random.Range ( _padding, column - _padding ) 
				);
			}
			_endPoint = new MapPoint ( 
				Random.Range ( _padding, row - _padding ), 
				Random.Range ( _padding, column - _padding ) 
			);
			while ( _endPoint.Equals ( _startPoint )
				|| _endPoint.ExistsIn ( _obsPoints.ToArray () ) ) {
				_endPoint = new MapPoint ( 
					Random.Range ( _padding, row - _padding ), 
					Random.Range ( _padding, column - _padding ) 
				);
			}
			
			_generatedMap[ _startPoint.r ][ _startPoint.c ] = "start";
			_generatedMap[ _endPoint.r ][ _endPoint.c ] = "end";

			int _pivot = Random.Range ( minPivot, maxPivot );

			MapPoint _startPath = _startPoint.Clone ();
			_mapPaths = new List<MapPoint> ();
			_mapPaths.Add ( _startPath );
			for ( int a=0; a<_pivot; ++a ) {
				if ( _mapPaths.Count > row * column ) {
					break;
				}

				MapPoint _mp = new MapPoint (
					Random.Range ( 0, row ),
					Random.Range ( 0, column )
				);
				while ( _mp.Equals ( _startPath )
					|| _mp.Equals ( _endPoint )
					|| _mp.ExistsIn ( _mapPaths.ToArray () )
					|| _mp.ExistsIn ( _obsPoints.ToArray () ) ) {
					_mp = new MapPoint (
						Random.Range ( 0, row ),
						Random.Range ( 0, column )
					);
				}

				FindAndUpdatePaths ( _startPath, _mp );
				
				// _pivotRows.Remove ( _randRow );
				_startPath = _mp;
			}
			// Debug.Log ( "startpath: " + _startPath.r + ", " + _startPath.c );
			FindAndUpdatePaths ( _startPath, _endPoint );
		}

		private void FindAndUpdatePaths ( MapPoint start, MapPoint end ) {
			List<MapPoint> _pathsFound = FindPaths ( _generatedMap, start, end );
			if ( _pathsFound == null ) {
				_pathsFound = FindPaths ( _generatedMap, start, end, true );
			}

			if ( _pathsFound != null ) {
				for ( int p=1; p<_pathsFound.Count; ++p ) {
					// if ( !_pathsFound[p].ExistsIn ( _mapPaths.ToArray () ) ) {
						_mapPaths.Add ( _pathsFound[p] );
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
					// }
				}
			}
			else {
				Debug.LogError ( "paths not found even after crossover" );
			}
		}

		private void GenerateRooms () {
			// 1. Go from one path from _mapPaths to another;
			// 		if there's an intersection, increase the following path in _mapPaths by 1
			for ( int a=0; a<_mapPaths.Count; ++a ) {
				MapPoint mp = _mapPaths[a];
				if ( a > 0 ) {
					for ( int b=0; b<a; ++b ) { // compare with existing paths
						if ( _mapPaths[b].Equals ( mp ) ) {
							++mp.height;
						}
					}
				}
			}

			// 2. Generate a room layout, 3 x 5
		}

		//--------------------------------------------------
		// public
		//--------------------------------------------------
		public void Generate () {
			GenerateMap ();
			GenerateRooms ();
		}

		public Vector3 GetStartPosition () {
			Vector3 _startPos = startPoint.GetPosition (
				unitSize, rowUnit, columnUnit, heightUnit
			);
			_startPos.z -= unitSize;
			return _startPos;
		}

		//--------------------------------------------------
		// protected
		//--------------------------------------------------
		// TEST: Generate a new map every 0.2 seconds
		// private float timer = 0f;
		// protected void Update () {
		// 	timer -= Time.deltaTime;
		// 	if ( timer <= 0f ) {
		// 		timer = 0.2f;
		// 		Generate ();
		// 	}
		// }

		protected void OnDrawGizmos () {
			if ( _generatedMap != null ) {
				for ( int r=0; r<_generatedMap.Length; ++r ) {
					for ( int c=0; c<_generatedMap[r].Length; ++c ) {
						// _generatedMap[r][c] = 0;
						Vector3 _pos = new Vector3 ( c * columnUnit, ( float ) heightUnit / 2f, r * rowUnit ) * unitSize;
						bool _drawWire = true;
						switch ( _generatedMap[r][c] ) {
						case "start":
						case "end":
							Gizmos.color = Color.red;
							break;

						case "pivot":
							Gizmos.color = Color.black;
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
							Gizmos.DrawWireCube ( _pos, new Vector3 ( columnUnit, heightUnit, rowUnit ) * unitSize );	
						}
						else {
							Gizmos.DrawCube ( _pos, new Vector3 ( columnUnit, heightUnit, rowUnit ) * unitSize );	
						}			
					}
				}
			}

			if ( _mapPaths != null ) {
				for ( int a=0; a<_mapPaths.Count; ++a ) {
					MapPoint mp = _mapPaths[a];
					Vector3 _pos = new Vector3 ( 
						mp.c * columnUnit, 
						mp.height * heightUnit, 
						mp.r * rowUnit
					) * unitSize;
					if ( _generatedMap[ mp.r ][ mp.c ] == "pivot" ) {
						Gizmos.color = Color.black;
						Gizmos.DrawWireSphere ( _pos, ( unitSize / 2f ) );
					}
					else {
						Gizmos.color = Color.white;
						Gizmos.DrawWireSphere ( _pos, ( unitSize / 2f ) + ( a + 1 ) * ( unitSize / 100f ) );
					}

					if ( a > 0 ) {
						MapPoint mpPrev = _mapPaths[ a - 1 ];
						Vector3 _prevPos = new Vector3 ( 
							mpPrev.c * columnUnit, 
							mpPrev.height * heightUnit, 
							mpPrev.r * rowUnit
						) * unitSize ;
						Gizmos.color = Color.blue;
						Gizmos.DrawLine ( _pos, _prevPos );
					}
				}
			}

			if ( buggedTempMap != null ) {
				Gizmos.color = Color.red;
				for ( int r=0; r<buggedTempMap.Length; ++r ) {
					for ( int c=0; c<buggedTempMap[r].Length; ++c ) {
						int _mapValue = buggedTempMap[r][c];
						Vector3 _pos = new Vector3 ( 
							c * columnUnit, 
							5, 
							r * rowUnit 
						) * unitSize;
						if ( _mapValue > 0 ) {
							Gizmos.DrawWireSphere ( _pos, ( unitSize / 2f ) + _mapValue * ( unitSize / 100f ) );
						}
					}
				}
			}
		}
	}
}