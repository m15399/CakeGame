using UnityEngine;
using System.Collections;

public class SwordDudeTileModifier : TileModifier {

	public override bool WillMove(Vector2 dir, Tile pusher){
		Tile nextTile = Board.currBoard.GetTile(tile.tilePos + dir);
		if(nextTile != null && nextTile.destructableType == Tile.DestructableType.DESTRUCTABLE){
			return true;
		}
		return base.WillMove(dir, pusher);
	}

	public override void PreMove(Vector2 dir, Tile pusher){
		Debug.Log("swording!");
		// Try to destroy tile
		Tile nextTile = Board.currBoard.GetTile(tile.tilePos + dir);
		Debug.Log(nextTile);
		if(nextTile != null && nextTile.destructableType == Tile.DestructableType.DESTRUCTABLE){
			Board.currBoard.RemoveTile(nextTile);
			nextTile.Kill(Tile.KillReason.STABBED);
		}
	}

}
