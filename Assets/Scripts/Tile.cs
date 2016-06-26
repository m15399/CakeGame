using UnityEngine;
using System.Collections;

public class Tile : MonoBehaviour {

	static float snapSpeed = .3f; 

	public enum MoveType {
		FIXED,
		MOVES, 
//		PUSHABLE
	}

	public MoveType moveType;

	public Vector2 tilePos;
	public int tx { get { return (int)tilePos.x; }}
	public int ty { get { return (int)tilePos.y; }}

	void Update () {
		Vector3 desiredPos = new Vector3(
			tilePos.x, 
			Board.board.playerRow - tilePos.y, 
			transform.localPosition.z
		);

		float fac = snapSpeed * 60 * Time.deltaTime;
		fac = Mathf.Clamp(fac, 0, 1);
		transform.localPosition = Vector3.Lerp(transform.localPosition, desiredPos, fac);
	}

	public bool MoveOk(Vector2 dir){
		return !Board.board.IsSolid(tilePos + dir);
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
}
