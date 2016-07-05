using UnityEngine;
using System.Collections;

public class JumpLevelTileModifier : TileModifier {

	public string jumpLevel;

	public override void PassArgs(string[] args){
		jumpLevel = args[0];
	}

	public override Tile.OverlapResolution OnOverlap(Tile tile, Tile overlapper){
		Board.currBoard.EndBoardAndLoad(jumpLevel);
		GameObject.Destroy(tile.gameObject);
		return Tile.OverlapResolution.PUT_OVERLAPPER;
	}

}
