using UnityEngine;
using System.Collections;

public class JumpLevelOverlapHandler : OverlapHandler {

	public string jumpLevel;

	public override void WasOverlapped(Tile tile, Tile other){
		Board.currBoard.EndBoardAndLoad(jumpLevel);
		GameObject.Destroy(tile.gameObject);
	}

}
