using UnityEngine;
using System.Collections;

public class Tiled {

	// Expects .txt Flare map file exported from Tiled
	public static void Import(string levelName){
		string levelPath = "Levels/" + levelName;
		Debug.Log("Loading level at: Resources/" + levelPath + ".txt");
		TextAsset ta = (TextAsset)Resources.Load(levelPath);
		string levelString = ta.text;

		int widthIndex = levelString.IndexOf("width=") + 6;
		int widthLength = levelString.IndexOf('\n', widthIndex) - widthIndex;
		string widthString = levelString.Substring(widthIndex, widthLength);
		int width = int.Parse(widthString);

		int heightIndex = levelString.IndexOf("height=") + 7;
		int heightLength = levelString.IndexOf('\n', heightIndex) - heightIndex;
		string heightString = levelString.Substring(heightIndex, heightLength);
		int height = int.Parse(heightString);

		Debug.Log("Found map of size: " + width + " x " + height);
		Board.board.width = width; Board.board.height = height;

		string dataString = levelString.Substring(levelString.IndexOf("data=") + 5);
		string[] data = dataString.Split(',');

		int i = 0;
		for(int y = 0; y < height; y++){
			for(int x = 0; x < width; x++){
				string id = data[i];
				TileFactory.CreateTile(int.Parse(id), x, y);
				i++;
			}
		}
	}

}
