using UnityEngine;
using System.Collections;

public class SpikesTileModifier : TileModifier {

	public override Tile.OverlapResolution OnOverlap(Tile tile, Tile overlapper){
		GameObject.Destroy(overlapper.gameObject);
		Board.currBoard.LoseBoard();
		return Tile.OverlapResolution.PUT_OVERLAPPED;
	}

}
