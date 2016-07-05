using UnityEngine;
using System.Collections;

public class CakeTileModifier : TileModifier {

	public override void OnOverlap(Tile tile, Tile other){
		Board.currBoard.SwapOutTile(other, TileFactory.CreateTile(TileType.EATENCAKE));
		GameObject.Destroy(tile.gameObject);
	}

}
