using UnityEngine;
using System.Collections;

public class CakeOverlapHandler : OverlapHandler {

	public override void WasOverlapped(Tile tile, Tile other){
		Board.board.SwapOutTile(other, TileFactory.CreateTile(TileType.EATENCAKE));
		GameObject.Destroy(tile.gameObject);
	}

}
