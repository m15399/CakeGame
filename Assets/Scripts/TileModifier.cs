using UnityEngine;
using System.Collections;

public class TileModifier : MonoBehaviour {

	Tile _tile = null;
	public Tile tile {
		get {
			if(_tile == null)
				_tile = GetComponent<Tile>();
			return _tile;
		}
	}

	public virtual void PassArgs(string[] args){
		Debug.LogError("Unhandled tile arg: " + args[0]);
	}

	public virtual Tile.OverlapResolution OnOverlap(Tile overlapper){
		Debug.LogError("Unhandled overlap");
		return Tile.OverlapResolution.DO_NOTHING;
	}

	public virtual bool IsSolid(Tile entrant){
		Debug.LogError("Unhandled IsSolid check");
		return true;
	}

}
