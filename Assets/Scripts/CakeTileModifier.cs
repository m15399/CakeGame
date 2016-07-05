using UnityEngine;
using System.Collections;

public class CakeTileModifier : TileModifier {

	public override Tile.OverlapResolution OnOverlap(Tile tile, Tile overlapper){
		Board.currBoard.SwapOutTile(overlapper, TileFactory.CreateTile(TileType.EATENCAKE));
		GameObject.Destroy(tile.gameObject);
		return Tile.OverlapResolution.DO_NOTHING;
	}

}
