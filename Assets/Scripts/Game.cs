using UnityEngine;
using System.Collections;

public class Game : MonoBehaviour {

	public string mainMenuLevel;
	public string skipToLevel;
	public GameObject skipToLevelPrefab;

	public void Start(){
		Invoke("OpenFirstBoard", .25f);
	}

	void OpenFirstBoard(){
		if(skipToLevelPrefab != null)
			Board.currBoard.LoadBoardFromPrefab(skipToLevelPrefab);
		else if(skipToLevel.Length > 0)
			Board.currBoard.LoadBoard(skipToLevel);
		else
			Board.currBoard.LoadBoard(mainMenuLevel);
	}

}
