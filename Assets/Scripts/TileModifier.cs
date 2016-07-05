using UnityEngine;
using System.Collections;

public class TileModifier : MonoBehaviour {

	public virtual void PassArgs(string[] args){
		Debug.LogError("Unhandled tile arg: " + args[0]);
	}

	public virtual void OnOverlap(Tile tile, Tile other){
		GameObject.Destroy(tile.gameObject);
	}

}
