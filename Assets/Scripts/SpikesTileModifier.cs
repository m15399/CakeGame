using UnityEngine;
using System.Collections;

public class SpikesTileModifier : TileModifier {

	public override Tile.OverlapResolution OnOverlap(Tile overlapper){
		if(overlapper.moveType == Tile.MoveType.MOVES)
			Board.currBoard.LoseBoard();
		GameObject.Destroy(overlapper.gameObject);
		return Tile.OverlapResolution.PUT_OVERLAPPED;
	}

}
