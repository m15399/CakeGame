using UnityEngine;
using System.Collections;
using System.Xml;

public class TiledImporter {

	static string cachedLevelName = "";
	static TextAsset cachedLevel = null;
	static int bottomWallOffset = 12, topWallOffset = 10;

	static string GetLevelPath(string levelName){
		return "Levels/" + levelName;
	}

	static TextAsset OpenLevel(string levelName){
		if(levelName == cachedLevelName)
			return cachedLevel;

		if(cachedLevel != null)
			Resources.UnloadAsset(cachedLevel);
		cachedLevel = (TextAsset)Resources.Load(GetLevelPath(levelName));
		return cachedLevel;
	}

	public static bool LevelExists(string levelName){
		return OpenLevel(levelName) != null;
	}

	// Expects the name of a csv-mode .tmx file, renamed to .txt
	public static bool Import(string levelName){

		string actualPath = "Resources/" + GetLevelPath(levelName) + ".txt";

		TextAsset ta = OpenLevel(levelName);
		if(ta == null){
			Debug.LogError("Level not found: " + actualPath);
			return false;
		}

		XmlDocument xml = new XmlDocument();
		xml.LoadXml(ta.text);

		XmlNode mapNode = xml.SelectSingleNode("map");
		int width = int.Parse(mapNode.Attributes["width"].Value);
		int height = int.Parse(mapNode.Attributes["height"].Value);
		Debug.Log("Found map of size: " + width + "x" + height + " at " + actualPath);
		Board.currBoard.width = width; 
		Board.currBoard.height = height + bottomWallOffset + topWallOffset;
		Board.currBoard.startRow = topWallOffset;

		string[] jumpLevels = null;
		int currJumpLevel = 0;

		XmlNodeList properties = xml.SelectNodes("map/properties/property");
		foreach(XmlNode property in properties){
			string name = property.Attributes["name"].Value;
			string value = property.Attributes["value"].Value;
//			Debug.Log("Prop - " + name + ", " + value);

			switch(name){
			case "Start Row":
				Board.currBoard.startRow = topWallOffset + int.Parse(value);
				break;
			case "Jump Levels":
				jumpLevels = value.Split(',');
				break;
			default:
				break;
			}
		}

		XmlNode dataNode = xml.SelectSingleNode("map/layer/data");
		string dataString = dataNode.InnerText;
		if(dataString.Length == 0){
			Debug.LogError("TMX file has no data, or it is not using CSV encoding: " + actualPath);
			return false;
		}
		string[] data = dataString.Split(',');

		// Read data and add tiles
		int maxY = 0;
		int i = 0;
		for(int y = 0; y < height; y++){
			for(int x = 0; x < width; x++){
				string id = data[i];
				TileType tileType = (TileType) int.Parse(id);

				Tile tile = TileFactory.CreateAndAddTile(tileType, x, y + topWallOffset);

				switch(tileType){
				case TileType.JUMP_LEVEL:
					if(jumpLevels == null || currJumpLevel >= jumpLevels.Length)
						Debug.LogError("Not enough 'Jump Levels' given in Tiled level.");
					else {
						string jumpLevel = jumpLevels[currJumpLevel++];
						JumpLevelOverlapHandler overlapHandler = tile.GetComponent<JumpLevelOverlapHandler>();
						overlapHandler.jumpLevel = jumpLevel;
					}
					break;
				default:
					break;
				}

				if(tile != null)
					maxY = y + topWallOffset;
				i++;
			}
		}

		// Add wall at top and bottom
		TileFactory.AddFullWidthWall(0);
		TileFactory.AddFullWidthWall(maxY + bottomWallOffset);

		return true;
	}

	class NameAndNumber {
		public string name;
		public string numberString;
		public int number;

		public void AddToNumber(int num){
			SetNumber(num + number);
		}

		public void SetNumber(int num){
			number = num;
			numberString = ("" + number).PadLeft(numberString.Length, '0');
		}

		public string GetLevelName(){
			return name + numberString;
		}
	}

	static NameAndNumber GetLevelNameAndNumber(string levelName){
		NameAndNumber ret = new NameAndNumber();
		string levelNumS = "";
		for(int i = levelName.Length-1; i >= 0; i--){
			char ch = levelName[i];
			if(char.IsDigit(ch))
				levelNumS = ch + levelNumS;
			else
				break;
		}
		ret.number = int.Parse(levelNumS);
		ret.numberString = ("" + ret.number).PadLeft(levelNumS.Length, '0');
		ret.name = levelName.Substring(0, levelName.Length - ret.numberString.Length);
		return ret;
	}

	public static string GetLevelByAdding(string levelName, int add){
		NameAndNumber levelInfo = GetLevelNameAndNumber(levelName);
		levelInfo.AddToNumber(add);
		return levelInfo.GetLevelName();
	}

	public static string GetLevelByNumber(string levelName, int num){
		NameAndNumber levelInfo = GetLevelNameAndNumber(levelName);
		levelInfo.SetNumber(num);
		return levelInfo.GetLevelName();
	}

}
