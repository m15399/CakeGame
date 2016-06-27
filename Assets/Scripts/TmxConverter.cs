using UnityEngine;
using UnityEditor;
using System.IO;

public class TmxConverter : AssetPostprocessor {

	static string tmxDir = "Assets/TmxLevels/";
	static string destDir = "Assets/Resources/Levels/";

	static string GetRelativeDir(string tmxAsset){
		string dirName = Path.GetDirectoryName(tmxAsset);
		if(dirName.IndexOf(tmxDir) != 0){
			Debug.LogError("Expected tmx file to be in " + tmxDir);
			dirName = "";
		} else {
			dirName = dirName.Substring(tmxDir.Length);
		}
		return dirName;
	}

	static string GetDestDir(string tmxAsset){
		return destDir + GetRelativeDir(tmxAsset) + "/";
	}

	static string GetDestPath(string tmxAsset){
		return GetDestDir(tmxAsset) + Path.GetFileNameWithoutExtension(tmxAsset) + ".txt";
	}

	static void AddTmxAsset(string asset){
		if(asset.EndsWith(".tmx")){
			if(!File.Exists(asset))
				return;

			string destFileDir = GetDestDir(asset);
			string destFile = GetDestPath(asset);

			Debug.Log("Creating .txt file from .tmx: " + destFile);
			Directory.CreateDirectory(destFileDir);
			File.Copy(asset, destFile, true);
			AssetDatabase.ImportAsset(destFile);
		}
	}

	static void DeleteTmxAsset(string asset){
		if(asset.EndsWith(".tmx")){
			string destFile = GetDestPath(asset);
			Debug.Log("Deleting .txt file: " + destFile);
			AssetDatabase.DeleteAsset(destFile);
		}
	}

	static void DeleteEmptyDirs(string startLocation){
		foreach (var directory in Directory.GetDirectories(startLocation)){
			DeleteEmptyDirs(directory);

			string[] files = Directory.GetFiles(directory);
			bool hasFiles = false;
			foreach(string fn in files){
				if(Path.GetFileName(fn)[0] != '.'){
					hasFiles = true;
					break;
				}
			}

			if (!hasFiles && Directory.GetDirectories(directory).Length == 0){
				Directory.Delete(directory, true);
				Debug.Log("Deleting empty directory: " + directory);
			} 
		}
	}

	/// Takes all .tmx files in the TmxLevels directory and copies them to 
	/// Assets/Resources/Levels, changing their extension to .txt.
	/// This allows them to be opened with Resources.Load.
	static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths){

		foreach(string asset in importedAssets){
			AddTmxAsset(asset);
		}

		foreach(string asset in deletedAssets){
			DeleteTmxAsset(asset);
		}

		for (int i=0; i<movedAssets.Length; i++){
			DeleteTmxAsset(movedFromAssetPaths[i]);
			AddTmxAsset(movedAssets[i]);
		}

		DeleteEmptyDirs(destDir);
	}

}