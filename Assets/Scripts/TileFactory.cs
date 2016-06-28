using UnityEngine;
using System.Collections.Generic;

public class TileFactory : MonoBehaviour {

	[System.Serializable]
	public class PrefabIdPair : System.Object {
		public GameObject prefab;
		public TileType type;
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

	static Dictionary<TileType, GameObject> _prefabMap = null; 
	static Dictionary<TileType, GameObject> prefabMap {
		get {
			if(_prefabMap == null){
				// Build a map of TileType -> prefab
				_prefabMap = new Dictionary<TileType, GameObject>();
				foreach (PrefabIdPair pair in tileFactory.prefabs){
					_prefabMap[pair.type] = pair.prefab;
				}
			}
			return _prefabMap;
		}
	}

	public static Tile CreateTile(TileType type){
		try {
			GameObject prefab = prefabMap[type];
			Tile tile = GameObject.Instantiate(prefab).GetComponent<Tile>();

			tile.transform.parent = Board.board.transform;
			tile.type = type;
			return tile;
		} catch (KeyNotFoundException) {
			return null;
		}
	}

	public static bool CreateAndAddTile(TileType type, int tx, int ty){
		Tile tile = CreateTile(type);
		if(tile != null){
			tile.tilePos = new Vector2(tx, ty); 
			Board.board.AddTile(tx, ty, tile);
			return true;
		} else 
			return false;
	}

}
