using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class LevelProperties : MonoBehaviour {

	public int width = 4, height = 80, startRow = 0;

	GameObject levelBounds = null;

	void Start(){
		if(Application.isPlaying){
			levelBounds = GameObject.Find(gameObject.name + "/LevelBounds(Clone)");
			if(levelBounds != null){
				levelBounds.SetActive(false);
			}
		}
	}

	void Update(){
		if(!Application.isPlaying){
			transform.localPosition = new Vector3(
				-width/2.0f - .5f, 4, 0
			);

			levelBounds = GameObject.Find(gameObject.name + "/LevelBounds(Clone)");
			if(levelBounds == null){
				levelBounds = GameObject.Instantiate(Resources.Load("Prefabs/LevelBounds") as GameObject);
				levelBounds.transform.parent = transform;
			}

			Debug.Log(levelBounds.name);
			levelBounds.transform.localPosition = new Vector3(-.5f, .5f, 0);
			levelBounds.transform.localScale = new Vector3(width, height, 1);

//			// reload test
//			GameObject current = levelBounds;
//			string pName = current.name;
//			if(pName.EndsWith("(Clone)"))
//				pName = pName.Substring(0, -6);
//			Debug.Log("panem = " + pName);
//			GameObject reloaded = GameObject.Instantiate(Resources.Load("Prefabs/" + pName) as GameObject);
//			reloaded.transform.localPosition = current.transform.localPosition;
//			reloaded.transform.localRotation = current.transform.localRotation;
//			reloaded.transform.localScale = current.transform.localScale;
//			reloaded.transform.parent = current.transform.parent;
		} 
	}
		

}
