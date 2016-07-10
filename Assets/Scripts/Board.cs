using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Board : MonoBehaviour {

	const float defaultWinLoseTime = .6f, defaultJumpTime = .35f;

	static Board _currBoard = null;
	public static Board currBoard {
		get {
			if(_currBoard == null)
				_currBoard = GameObject.Find("Board").GetComponent<Board>();
			return _currBoard;
		}
	}



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

	string levelName, nextLevelName;
	bool loadingBoard = true;

	public GameObject viewBlockerPrefab;
	GameObject vbl, vbr;

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
			Game.instance.ReturnToMenu();
		}
	}

	void WinBoard(){
		WinBoard(defaultWinLoseTime);
	}

	void WinBoard(float delay){
		if(!loadingBoard){
			EndBoard();
			Invoke("LoadNextBoard", delay);
		}
	}

	public void LoseBoard(){
		EndBoard();
		Invoke("LoadBoard", defaultWinLoseTime);
	}

	void EndBoard(){
		allowInput = false;
		loadingBoard = true;
	}

	public void JumpToBoard(string level){
		if(!loadingBoard){
			EndBoard();
			nextLevelName = level;
			Invoke("LoadNextBoard", defaultJumpTime);
		}
	}

	void Resize(int w, int h){
		width = w; height = h;
		Resize();
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

	bool HasMoveables(int row){
		for(int i = 0; i < width; i++){
			Tile tile = tiles[i, row];
			if(tile != null && tile.moveType == Tile.MoveType.MOVES)
				return true;
		}
		return false;
	}

	public bool IsSolid(Vector2 pos, Tile entrant){
		if(!InsideBoard(pos))
			return true;
		Tile tile = GetTile(pos);
		if(tile != null)
			return tile.IsSolid(entrant);
		return false;
	}

	public bool WillMove(Vector2 pos, Vector2 dir, Tile pusher){
		if(!InsideBoard(pos))
			return false;
		Tile tile = GetTile(pos);
		if(tile != null)
			return tile.WillMove(dir, pusher);
		return false;
	}

	bool InsideBoard(Vector2 pos){
		int tx = (int)pos.x, ty = (int)pos.y;
		if(tx < 0 || tx >= width || ty < 0 || ty >= height)
			return false;
		return true;
	}

	public Tile GetTile(Vector2 pos){
		int tx = (int)pos.x, ty = (int)pos.y;
		return tiles[tx, ty];
	}

	public Tile GetTile(int tx, int ty){
		return tiles[tx, ty];
	}

	public void AddTile(int tx, int ty, Tile tile){
		if(tiles[tx, ty] != null)
			Debug.LogError("Trying to overwrite existing tile at " + tx + ", " + ty);
		tiles[tx, ty] = tile;
	}

	public void ReplaceTile(int tx, int ty, Tile newTile){
		tiles[tx, ty] = newTile;
	}

	public void RemoveTile(Tile tile){
		tiles[tile.tx, tile.ty] = null;
	}

	// Destroy a tile and replace it with another one
	public void SwapOutTile(Tile tile, Tile newTile){		
		newTile.transform.localPosition = tile.transform.localPosition;
		newTile.tilePos = tile.tilePos;

		ReplaceTile(tile.tx, tile.ty, newTile);

		GameObject.Destroy(tile.gameObject);
	}

	public void MoveTile(Vector2 pos, Vector2 dir, Tile pusher){
		MoveTile(GetTile(pos), dir, pusher);
	}

	public void MoveTile(Tile tile, Vector2 dir, Tile pusher){
		if(tile == null)
			return; 
		int tx = tile.tx, ty = tile.ty;
		
		if(tile.AttemptMove(dir, pusher)){
			
			int tx2 = tx + (int)dir.x, ty2 = ty + (int)dir.y;
			Tile overlappedTile = tiles[tx2, ty2];

			tiles[tx, ty] = null;

			if(overlappedTile != null){
				Tile.OverlapResolution result = overlappedTile.WasOverlapped(tile);
				switch(result){
				case Tile.OverlapResolution.PUT_OVERLAPPER:
					ReplaceTile(tx2, ty2, tile);
					break;
				case Tile.OverlapResolution.PUT_OVERLAPPED:
					ReplaceTile(tx2, ty2, overlappedTile);
					break;
				case Tile.OverlapResolution.DO_NOTHING:
				default:
					break;
				}
			} else {
				ReplaceTile(tx2, ty2, tile);
			}

		}
	}

	void MoveRowHorizontal(int row, int dirX){
		Vector2 dir = new Vector2(dirX, 0);

		for(int i = 0; i < width; i++){
			// start from right or left side, based on dirX
			int tx = (dirX == 1 ? width - i - 1 : i);
			Tile tile = GetTile(tx, row);
			if(tile != null && tile.moveType == Tile.MoveType.MOVES)
				MoveTile(tile, dir, null);
		}
		CheckTileCoords();
	}

	void MoveRowVertical(int row, int dirY){
		Vector2 forwards = new Vector2(0, dirY);

		// Check if can move every tile
		bool canMove = true;
		for(int i = 0; i < width; i++){
			Tile tile = tiles[i, row];
			if(tile != null && tile.moveType == Tile.MoveType.MOVES){
				if(!tile.WillMove(forwards, null)){
					canMove = false;
					break;
				}
			}
		}

		// Move row 
		if(canMove){
			for(int i = 0; i < width; i++){
				Tile tile = GetTile(i, row);
				if(tile != null && tile.moveType == Tile.MoveType.MOVES)
					MoveTile(tile, forwards, null);
			}
			_playerRow += dirY;
		}
		CheckTileCoords();
	}

	void PrintIds(){
		string s = "";
		Debug.Log("Printing Ids");
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
							" (found tile coord of " + tx + ", " + ty + ")");
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
