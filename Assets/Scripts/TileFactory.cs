using UnityEngine;
using System.Collections.Generic;

public class TileFactory : MonoBehaviour {

	[System.Serializable]
	public class PrefabIdPair : System.Object {
		public GameObject prefab;
		public int id;
	}

	public PrefabIdPair[] prefabs;



	static TileFactory _tileFactory = null;
	static TileFactory tileFactory {
		get {
			if(_tileFactory == null)
				_tileFactory = GameObject.Find("TileFactory").GetComponent<TileFactory>();
			return _tileFactory;
		}
	}

	static Dictionary<int, GameObject> _prefabMap = null; 
	static Dictionary<int, GameObject> prefabMap {
		get {
			if(_prefabMap == null){
				// Build a map of int -> GameObject
				_prefabMap = new Dictionary<int, GameObject>();
				foreach (PrefabIdPair pair in tileFactory.prefabs){
					_prefabMap[pair.id] = pair.prefab;
				}
			}
			return _prefabMap;
		}
	}

	public static void CreateTile(GameObject prefab, int tx, int ty){
		Tile tile = GameObject.Instantiate(prefab).GetComponent<Tile>();
		tile.transform.parent = Board.board.transform;
		tile.tilePos = new Vector2(tx, ty); 
		Board.board.AddTile(tx, ty, tile);
	}

	public static void CreateTile(int id, int tx, int ty){
		try {
			CreateTile(prefabMap[id], tx, ty);
		} catch (KeyNotFoundException) {}
	}

}
