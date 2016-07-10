using UnityEngine;
using System.Collections;

public class CakeTileModifier : TileModifier {

	public override Tile.OverlapResolution OnOverlap(Tile overlapper){
		Board.currBoard.SwapOutTile(overlapper, TileFactory.CreateTile(TileType.EATEN_CAKE));
		tile.Kill(Tile.KillReason.EATEN);
		return Tile.OverlapResolution.DO_NOTHING;
	}

	public override bool IsSolid(Tile entrant){
		return !(entrant.moveType == Tile.MoveType.MOVES); // solid except to dudes
	}

}
