using UnityEngine;
using System.Collections;

public class SpikesTileModifier : TileModifier {

	public override Tile.OverlapResolution OnOverlap(Tile overlapper){
		if(overlapper.moveType == Tile.MoveType.MOVES)
			Board.currBoard.LoseBoard();
		overlapper.Kill(Tile.KillReason.STABBED);
		return Tile.OverlapResolution.PUT_OVERLAPPED;
	}

}
