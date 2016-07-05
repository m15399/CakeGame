using UnityEngine;
using System.Collections;

public class JumpLevelTileModifier : TileModifier {

	public string jumpLevel;

	public override void PassArgs(string[] args){
		jumpLevel = args[0];
	}

	public override void OnOverlap(Tile tile, Tile other){
		Board.currBoard.EndBoardAndLoad(jumpLevel);
		GameObject.Destroy(tile.gameObject);
	}

}
