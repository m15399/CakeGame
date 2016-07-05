using UnityEngine;
using System.Collections;

public class Game : MonoBehaviour {

	public string mainMenuLevel;
	public bool skipToLevel;
	public string skipLevel;

	public void Start(){
		Invoke("OpenFirstBoard", .25f);
	}

	void OpenFirstBoard(){
		if(skipToLevel && skipLevel.Length > 0)
			Board.currBoard.LoadBoard(skipLevel);
		else
			Board.currBoard.LoadBoard(mainMenuLevel);
	}

}
