using UnityEngine;
using System.Collections;

public class Game : MonoBehaviour {

	public string mainMenuLevel;
	public bool skipToLevel;
	public string skipLevel;

	static Game _instance = null;
	public static  Game instance{
		get {
			if(_instance == null)
				_instance = GameObject.Find("Game").GetComponent<Game>();
			return _instance;
		}
	}

	public void Start(){
		Invoke("OpenFirstBoard", .25f);
	}

	void OpenFirstBoard(){
		if(skipToLevel && skipLevel.Length > 0)
			Board.currBoard.LoadBoard(skipLevel);
		else
			ReturnToMenu();
	}

	public void ReturnToMenu(){
		Board.currBoard.LoadBoard(mainMenuLevel);
	}

}
