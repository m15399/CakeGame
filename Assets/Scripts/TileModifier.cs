using UnityEngine;
using System.Collections;

public class TileModifier : MonoBehaviour {

	Tile _tile = null;
	public Tile tile {
		get {
			if(_tile == null)
				_tile = GetComponent<Tile>();
			return _tile;
		}
	}

	public virtual void PassArgs(string[] args){
		Debug.LogError("Unhandled tile arg: " + args[0]);
	}

	public virtual Tile.OverlapResolution OnOverlap(Tile overlapper){
		Debug.LogError("Unhandled overlap");
		return Tile.OverlapResolution.DO_NOTHING;
	}

	public virtual bool IsSolid(Tile entrant){
		Debug.LogError("Unhandled IsSolid check");
		return true;
	}

	public virtual bool WillMove(Vector2 dir, Tile pusher){
		switch(tile.moveType){
		case Tile.MoveType.FIXED:
			return false;
		case Tile.MoveType.MOVES:
			if(pusher != null)
			if(pusher.ty != tile.ty)
				return false; // cannot push a dude vertically

			return !Board.currBoard.IsSolid(tile.tilePos + dir, tile) || 
				Board.currBoard.WillMove(tile.tilePos + dir, dir, tile); 
		case Tile.MoveType.PUSHABLE:
			if(pusher == null)
				return false;
			return !Board.currBoard.IsSolid(tile.tilePos + dir, tile) || 
				Board.currBoard.WillMove(tile.tilePos + dir, dir, tile);
		default:
			return false;
		}
	}

	public virtual void PreMove(Vector2 dir, Tile pusher){
		
	}

}
