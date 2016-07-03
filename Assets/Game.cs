using UnityEngine;
using System.Collections;

public class Game : MonoBehaviour {

	public string mainMenuLevel;
	public string skipToLevel;

	public void Start(){
		Invoke("OpenFirstBoard", .25f);
	}

	void OpenFirstBoard(){
		if(skipToLevel.Length > 0)
			Board.currBoard.LoadBoard(skipToLevel);
		else
			Board.currBoard.LoadBoard(mainMenuLevel);
	}

}
