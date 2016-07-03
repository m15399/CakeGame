using UnityEngine;
using System.Collections;

public class Tile : MonoBehaviour {

	static float snapSpeed = .3f; 

	public enum MoveType {
		FIXED,
		MOVES, 
//		PUSHABLE
	}

	public enum OverlapType {
		SOLID,
		OVERLAPPABLE
	}

	public MoveType moveType;
	public OverlapType overlapType;

	public TileType type = TileType.NONE;

	public Vector2 tilePos;
	public int tx { get { return (int)tilePos.x; }}
	public int ty { get { return (int)tilePos.y; }}

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

	public bool IsSolid(){
		switch(overlapType){
		case OverlapType.OVERLAPPABLE:
			return false;
		case OverlapType.SOLID:
		default:
			return true;
		}
	}

	public bool MoveOk(Vector2 dir){
		switch(moveType){
		case MoveType.FIXED:
			return true;
		case MoveType.MOVES:
		default:
			return !Board.currBoard.IsSolid(tilePos + dir);
		}
	}

	public bool WillMove(Vector2 dir){
		switch(moveType){
		case MoveType.FIXED:
			return false;
		case MoveType.MOVES:
			return MoveOk(dir);
		default:
			return false;
		}
	}

	public bool AttemptMove(Vector2 dir){
		if(WillMove(dir)){
			tilePos += dir;
			return true;
		} else {
			return false;
		}
	}

	public void WasOverlapped(Tile other){
		OverlapHandler oh = GetComponent<OverlapHandler>();
		if(oh != null)
			oh.WasOverlapped(this, other);
		else
			GameObject.Destroy(gameObject);
	}
}
