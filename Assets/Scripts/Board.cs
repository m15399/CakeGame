using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Board : MonoBehaviour {

	const float defaultEndWaitTime = .6f;

	static Board _currBoard = null;
	public static Board currBoard {
		get {
			if(_currBoard == null)
				_currBoard = GameObject.Find("Board").GetComponent<Board>();
			return _currBoard;
		}
	}

	string levelName, nextLevelName;
	bool loadingBoard = true;

	public GameObject viewBlockerPrefab;
	GameObject vbl, vbr;

	Tile[,] tiles;
	int _width, _height;
	public int width { get { return _width; } set { _width = value; Resize(); } }
	public int height { get { return _height; } set { _height = value; Resize(); } }

	int _playerRow = 0;
	public int playerRow { get { return _playerRow; } }
	public int startRow { get; set; }

	bool allowInput = false;
	bool inputCenteredH = true;
	bool inputCenteredV = true;

	void Start(){
		vbl = GameObject.Instantiate(viewBlockerPrefab);
		vbr = GameObject.Instantiate(viewBlockerPrefab);
		vbr.transform.localScale = new Vector3(-1, 1, 1);
	}

	/// <summary>
	/// Loads the board named by 'levelName'
	/// </summary>
	bool LoadBoard(){
		loadingBoard = true;
		allowInput = false;
		bool success = false;

		if(TiledImporter.LevelExists(levelName)){
			ClearBoard();
			success = TiledImporter.Import(levelName);
			if(success){
				_playerRow = startRow;
				allowInput = true;

				if(!HasMoveables(playerRow)){
					Debug.LogError("Board was won with no moves. " +
						"Did you forget to set the 'Start Row' property? Currently " + startRow);
				}
			}
		}
		loadingBoard = false;
		return success;
	}

	void LoadNextBoard(){
		if(nextLevelName.Length > 0){
			LoadBoard(nextLevelName);
		} else 
			LoadBoardByAdding(1);
	}

	void LoadBoardByAdding(int add){
		LoadBoard(TiledImporter.GetLevelByAdding(levelName, add));
	}

	void LoadBoardByNumber(int num){
		LoadBoard(TiledImporter.GetLevelByNumber(levelName, num));
	}

	public void LoadBoard(string newLevelName){
		string prevLevelName = levelName;
		levelName = newLevelName;

		bool success = LoadBoard();
		if(!success){
			Debug.Log("Couldn't find level: " + levelName);
			levelName = prevLevelName;
		}
	}

	void WinBoard(){
		WinBoard(defaultEndWaitTime);
	}

	void WinBoard(float delay){
		if(!loadingBoard){
			EndBoard();
			Invoke("LoadNextBoard", delay);
		}
	}

	void EndBoard(){
		allowInput = false;
		loadingBoard = true;
	}

	public void EndBoardAndLoad(string level){
		if(!loadingBoard){
			EndBoard();
			nextLevelName = level;
			Invoke("LoadNextBoard", defaultEndWaitTime);
		}
	}

	void Resize(){
		ClearBoard();
		tiles = new Tile[width, height];

		transform.localPosition = new Vector3(-width/2.0f + .5f, 1, 0);
		vbl.transform.position = new Vector3(-width/2.0f, 0);
		vbr.transform.position = new Vector3(width/2.0f, 0);
	}

	void ClearBoard(){
		if(tiles == null)
			return;
		
		for(int i = 0; i < tiles.GetLength(0); i++){
			for(int j = 0; j < tiles.GetLength(1); j++){
				if(tiles[i,j] != null){
					GameObject.Destroy(tiles[i,j].gameObject);
					tiles[i,j] = null;
				}
			}
		}
		tiles = null;
		_playerRow = startRow = 0;
		allowInput = false;
		nextLevelName = "";
	}

	void Update(){

		int moveDirH = 0;
		int moveDirV = 0;

		// Translate inputs to moveDirH and moveDirV
		{
			float horizInput = Input.GetAxis("Horizontal");
			float threshhold = .5f;
			if(horizInput < -threshhold)
				moveDirH = -1;
			else if (horizInput > threshhold)
				moveDirH = 1;

			float vertInput = Input.GetAxis("Vertical");
			if(vertInput < -threshhold)
				moveDirV = 1;
			else if (vertInput > threshhold)
				moveDirV = -1;


			if(moveDirH == 0)
				inputCenteredH = true;
			else if(inputCenteredH)
				inputCenteredH = false;
			else
				moveDirH = 0;

			if(moveDirV == 0)
				inputCenteredV = true;
			else if(inputCenteredV)
				inputCenteredV = false;
			else
				moveDirV = 0;
		}
			

		if(!HasMoveables(playerRow) && allowInput){ // Level completed
			WinBoard();
		}

		if(allowInput){
			if(moveDirH != 0)
				MoveRowHorizontal(playerRow, moveDirH);
			if(moveDirV != 0)
				MoveRowVertical(playerRow, moveDirV);

			// Restart button
			if(Input.GetKeyDown("r")){
				LoadBoard();
			}

			// Cheats
			if(Application.isEditor){
				if(Input.GetKeyDown("n"))
					LoadBoardByAdding(1);
				if(Input.GetKeyDown("p"))
					LoadBoardByAdding(-1);
				if(Input.GetKeyDown("1")) LoadBoardByNumber(1); 
				if(Input.GetKeyDown("2")) LoadBoardByNumber(2); 
				if(Input.GetKeyDown("3")) LoadBoardByNumber(3); 
				if(Input.GetKeyDown("4")) LoadBoardByNumber(4); 
				if(Input.GetKeyDown("5")) LoadBoardByNumber(5); 
				if(Input.GetKeyDown("6")) LoadBoardByNumber(6); 
				if(Input.GetKeyDown("7")) LoadBoardByNumber(7); 
				if(Input.GetKeyDown("8")) LoadBoardByNumber(8); 
				if(Input.GetKeyDown("9")) LoadBoardByNumber(9); 
				if(Input.GetKeyDown("0")) LoadBoardByNumber(10); 
			}
		}

	}

	bool HasMoveables(int row){
		for(int i = 0; i < width; i++){
			Tile tile = tiles[i, row];
			if(tile != null && tile.moveType == Tile.MoveType.MOVES)
				return true;
		}
		return false;
	}

	public bool IsSolid(Vector2 pos){
		int tx = (int)pos.x, ty = (int)pos.y;

		if(tx < 0 || tx >= width || ty < 0 || ty >= height) // outside board?
			return true;
		if(tiles[tx, ty] != null)
			return tiles[tx, ty].IsSolid();

		return false;
	}

	public void AddTile(int tx, int ty, Tile tile){
		if(tiles[tx, ty] != null)
			Debug.LogError("Trying to overwrite existing tile at " + tx + ", " + ty);
		tiles[tx, ty] = tile;
	}

	public void ReplaceTile(int tx, int ty, Tile newTile){
		tiles[tx, ty] = newTile;
	}

	// Destroy a tile and replace it with another one
	public void SwapOutTile(Tile tile, Tile newTile){
		newTile.transform.localPosition = tile.transform.localPosition;
		newTile.tilePos = tile.tilePos;

		ReplaceTile(tile.tx, tile.ty, newTile);

		GameObject.Destroy(tile.gameObject);
	}

	public void MoveTile(int tx, int ty, Vector2 dir){
		Tile tile = tiles[tx, ty];
		if(tile == null)
			return; 

		if(tile.AttemptMove(dir)){
			int tx2 = tx + (int)dir.x, ty2 = ty + (int)dir.y;
			Tile overlappedTile = tiles[tx2, ty2];

			tiles[tx, ty] = null;
			ReplaceTile(tx2, ty2, tile);

			if(overlappedTile != null){
				overlappedTile.WasOverlapped(tile);
			}
		}
	}

	void MoveRowHorizontal(int row, int dirX){
		Vector2 dir = new Vector2(dirX, 0);

		for(int i = 0; i < width; i++){
			// start from right or left side, based on dirX
			int tx = (dirX == 1 ? width - i - 1 : i);
			MoveTile(tx, row, dir);
		}
		CheckTileCoords();
	}

	void MoveRowVertical(int row, int dirY){
		Vector2 forwards = new Vector2(0, dirY);

		// Check if can move every tile
		bool canMove = true;
		for(int i = 0; i < width; i++){
			Tile tile = tiles[i, row];
			if(tile != null && !tiles[i, row].MoveOk(forwards)){
				canMove = false;
				break;
			}
		}

		// Move row 
		if(canMove){
			for(int i = 0; i < width; i++){
				MoveTile(i, row, forwards);
			}
			_playerRow += dirY;
		}
		CheckTileCoords();
	}

	void PrintIds(){
		string s = "";
		Debug.Log("Printing Solids");
		for(int j = 0; j < height; j++){
			for(int i = 0; i < width; i++ ){
				Tile tile = tiles[i, j];
				if(tile != null)
					s += (int)tile.type;
				else
					s += "0";
			}
			s += "\n";
		}
		Debug.Log(s);
	}

	void CheckTileCoords(){
		if(!Application.isEditor)
			return;

		bool success = true;
		for(int i = 0; i < width; i++){
			for(int j = 0; j < height; j++){
				Tile tile = tiles[i,j];
				if(tile != null){
					int tx = tile.tx, ty = tile.ty;
					if(tx != i || ty != j){
						Debug.Log("Tile coords test failed: " + i + ", " + j + 
							"(found " + tx + ", " + ty + ")");
						success = false;
					}
				}
			}
		}
		if(!success){
			Debug.LogError("Tile coords test failed! Printing board ids: ");
			PrintIds();
		}
	}
}
