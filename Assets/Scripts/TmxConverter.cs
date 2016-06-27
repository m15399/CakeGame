using UnityEngine;
using UnityEditor;
using System.IO;

public class TmxConverter : AssetPostprocessor {

	/// Takes all .tmx files in the TmxLevels directory and copies them to 
	/// Assets/Resources/Levels, changing their extension to .txt.
	/// This allows them to be opened with Resources.Load.
	static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromPath){

		string expectedDir = "Assets/TmxLevels/";
		string destDir = "Assets/Resources/Levels/";

		foreach(string asset in importedAssets){
			if(asset.EndsWith(".tmx")){
				if(!File.Exists(asset))
					continue;
				Debug.Log("Converting " + asset + " to .txt");

				string dirName = Path.GetDirectoryName(asset);
				if(dirName.IndexOf(expectedDir) != 0){
					Debug.LogError("Expected tmx file to be in " + expectedDir);
					continue;
				}
				dirName = dirName.Substring(expectedDir.Length);

				string destFileDir = destDir + dirName + "/";
				string destFile = destFileDir + Path.GetFileNameWithoutExtension(asset) + ".txt";

				Debug.Log("Copying .tmx file to " + destFile);
				Directory.CreateDirectory(destFileDir);
				File.Copy(asset, destFile, true);
				AssetDatabase.ImportAsset(destFile);
			}
		}
	}

}