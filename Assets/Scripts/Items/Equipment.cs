using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Equipment : MonoBehaviour {

	EquipmentScriptable item;

	// Use this for initialization
	void Start () {
		transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = item.sprite;
		transform.GetChild(1).GetComponent<Text>().text = item.Name;
	}
}
