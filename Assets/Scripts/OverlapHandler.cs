using UnityEngine;
using System.Collections;

public abstract class OverlapHandler : MonoBehaviour {

	public virtual void WasOverlapped(Tile tile, Tile other){
		GameObject.Destroy(tile.gameObject);
	}

}
