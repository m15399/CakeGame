using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class Tile : MonoBehaviour {

	static float snapSpeed = .3f; 

	public enum MoveType {
		FIXED,
		MOVES, 
		PUSHABLE
	}

	public enum OverlapType {
		SOLID,
		OVERLAPPABLE,
		CUSTOM
	}

	public enum OverlapResolution {
		PUT_OVERLAPPED,
		PUT_OVERLAPPER,
		DO_NOTHING
	}

	public enum DestructableType {
		INDESTRUCTABLE,
		DESTRUCTABLE
	}

	public enum KillReason {
		STABBED,
		SQUISHED,
		EATEN
	}

	public MoveType moveType = MoveType.FIXED;
	public OverlapType overlapType = OverlapType.SOLID;
	public DestructableType destructableType = DestructableType.INDESTRUCTABLE;

	public TileType type = TileType.NONE;

	public Vector2 tilePos;
	public int tx { get { return (int)tilePos.x; }}
	public int ty { get { return (int)tilePos.y; }}

	TileModifier _tileModifier = null;
	public TileModifier tileModifier {
		get {
			if(_tileModifier == null){
				_tileModifier = GetComponent<TileModifier>();
				if(_tileModifier == null)
					_tileModifier = gameObject.AddComponent<TileModifier>() as TileModifier;
			}
			return _tileModifier;
		}
	}

	void Update () {
		Vector3 desiredPos = new Vector3(
			tilePos.x, 
			Board.currBoard.playerRow - tilePos.y, 
			transform.localPosition.z
		);

		float fac = snapSpeed * 60 * Time.deltaTime;
		fac = Mathf.Clamp(fac, 0, 1);
		transform.localPosition = Vector3.Lerp(transform.localPosition, desiredPos, fac);
	}

	public void Kill(KillReason reason){
		GameObject.Destroy(gameObject);
	}

	public bool IsSolid(Tile entrant){
		switch(overlapType){
		case OverlapType.CUSTOM:
			return tileModifier.IsSolid(entrant);
		case OverlapType.OVERLAPPABLE:
			return false;
		case OverlapType.SOLID:
		default:
			return true;
		}
	}

	public bool WillMove(Vector2 dir, Tile pusher){
		return tileModifier.WillMove(dir, pusher);
	}

	public bool AttemptMove(Vector2 dir, Tile pusher){
		if(WillMove(dir, pusher)){

			tileModifier.PreMove(dir, pusher);

			// Try to push
			if(Board.currBoard.IsSolid(tilePos + dir, this)){
				Board.currBoard.MoveTile(tilePos + dir, dir, this);
			}

			tilePos += dir;
			return true;
		} else {
			return false;
		}
	}
		
	public OverlapResolution WasOverlapped(Tile overlapper){ 
		return tileModifier.OnOverlap(overlapper); 
	}
	public void PassArgs(string[] args){ tileModifier.PassArgs(args); }
}
