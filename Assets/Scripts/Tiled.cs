using UnityEngine;
using System.Collections;
using System.Xml;

public class Tiled {

	// Expects the name of a csv-mode .tmx file, renamed to .txt
	public static void Import(string levelName){
		
		string levelPath = "Levels/" + levelName;
		Debug.Log("Loading level at: Resources/" + levelPath + ".txt");
		TextAsset ta = (TextAsset)Resources.Load(levelPath);

		XmlDocument xml = new XmlDocument();
		xml.LoadXml(ta.text);

		XmlNode mapNode = xml.SelectSingleNode("map");
		int width = int.Parse(mapNode.Attributes["width"].Value);
		int height = int.Parse(mapNode.Attributes["height"].Value);
		Debug.Log("Found map of size: " + width + " x " + height);
		Board.board.width = width; Board.board.height = height;

		XmlNode dataNode = xml.SelectSingleNode("map/layer/data");
		string dataString = dataNode.InnerText;
		if(dataString.Length == 0){
			Debug.LogError("TMX file has no data, or it is not using CSV encoding");
			return;
		}
		string[] data = dataString.Split(',');

		int i = 0;
		for(int y = 0; y < height; y++){
			for(int x = 0; x < width; x++){
				string id = data[i];
				TileFactory.CreateAndAddTile((TileType)int.Parse(id), x, y);
				i++;
			}
		}
	}

}
