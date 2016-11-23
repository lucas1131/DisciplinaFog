using UnityEngine;
using System.Collections;

public class utils : MonoBehaviour {

	public static GameObject FindObject(GameObject parent, string name){

		// This is the only way to get inactive game objects
		// The boolean parameter is wether or not to include inactive objects
		// in search
		Transform[] trs = parent.GetComponentsInChildren<Transform>(true);
		
		// Check each object name to search for desired object
		foreach(Transform t in trs)
			if(t.name == name)
				return t.gameObject;

		// Not found
		return null;
	}
}
