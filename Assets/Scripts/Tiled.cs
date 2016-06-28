using UnityEngine;
using System.Collections;
using System.Xml;

public class Tiled {

	static string currLevelName = "";
	static TextAsset currLevel = null;
	static int bottomWallOffset = 12;

	static string GetLevelPath(string levelName){
		return "Levels/" + levelName;
	}

	static TextAsset OpenLevel(string levelName){
		if(levelName == currLevelName)
			return currLevel;

		currLevel = (TextAsset)Resources.Load(GetLevelPath(levelName));
		return currLevel;
	}

	public static bool LevelExists(string levelName){
		return OpenLevel(levelName) != null;
	}

	// Expects the name of a csv-mode .tmx file, renamed to .txt
	public static bool Import(string levelName){

		Debug.Log("Importing level: Resources/" + GetLevelPath(levelName) + ".txt");

		TextAsset ta = OpenLevel(levelName);
		if(ta == null){
			Debug.Log("Level not found.");
			return false;
		}

		XmlDocument xml = new XmlDocument();
		xml.LoadXml(ta.text);

		XmlNode mapNode = xml.SelectSingleNode("map");
		int width = int.Parse(mapNode.Attributes["width"].Value);
		int height = int.Parse(mapNode.Attributes["height"].Value);
		Debug.Log("Found map of size: " + width + " x " + height);
		Board.board.width = width; 
		Board.board.height = height + bottomWallOffset;

		XmlNode dataNode = xml.SelectSingleNode("map/layer/data");
		string dataString = dataNode.InnerText;
		if(dataString.Length == 0){
			Debug.LogError("TMX file has no data, or it is not using CSV encoding");
			return false;
		}
		string[] data = dataString.Split(',');

		// Read data and add tiles
		int maxY = 0;
		int i = 0;
		for(int y = 0; y < height; y++){
			for(int x = 0; x < width; x++){
				string id = data[i];
				if(TileFactory.CreateAndAddTile((TileType)int.Parse(id), x, y))
					maxY = y;
				i++;
			}
		}

		// Add wall at bottom
		for(int x = 0; x < width; x++){
			TileFactory.CreateAndAddTile(TileType.WALL, x, maxY + bottomWallOffset);
		}


		return true;
	}

}
