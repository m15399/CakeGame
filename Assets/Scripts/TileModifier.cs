using UnityEngine;
using System.Collections;

public class TileModifier : MonoBehaviour {

	public virtual void PassArgs(string[] args){
		Debug.LogError("Unhandled tile arg: " + args[0]);
	}

	public virtual Tile.OverlapResolution OnOverlap(Tile tile, Tile overlapper){
		Debug.LogError("Unhandled overlap");
		return Tile.OverlapResolution.DO_NOTHING;
	}

}
