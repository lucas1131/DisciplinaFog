using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class BoardManager : MonoBehaviour {

	public Text terrainInfo;

	public Unit[] units;
	public int size = 1;
	
	public TerrainType[] terrainsTypes;
	public int[,] titles;

	public int rows = 8;
	public int cols = 8;

	void Start() {

		// Allocate vectors
		titles = new int[cols, rows];
		units = new Unit[size];

		// Initialize grid
		for (int i = 0; i < cols; i++) {
			for (int j = 0; j < rows; j++) {
				titles[i, j] = (int) Terrains.Plains;
			}
		}

		// Set text font
		terrainInfo.font = Resources.Load<Font>("Font/FE-Text");

		//CreateTestScene();
	}
	
	void Update() {
	
	}


	void OnGUI(){

		//GUI.Box(new Rect(10, 10, 95, 85), "");
		//GUI.TextArea(new Rect(10, 10, 95, 85), terrainInfo);
	}

	public void DisplayTerrainInfo(int x, int y){

		string tmp;
		tmp = terrainsTypes[titles[x, y]].name;
		tmp += "\nAvo: " + terrainsTypes[titles[x, y]].avoid;
		tmp += "\nDef: " + terrainsTypes[titles[x, y]].defense;
		tmp += "\nRec: " + terrainsTypes[titles[x, y]].recover;
		if(terrainsTypes[titles[x, y]].name.Equals("Cracked Wall"))
			tmp += "\nLife: " + terrainsTypes[titles[x, y]].life;

		terrainInfo.text = tmp;
	}

	public Unit GetUnit(int x, int y){
		return null;
	}

	void CreateTestScene(){

		int counter = 0;
		for (int i = 0; i < cols; i++) {
			for (int j = 0; j < rows; j++) {
				titles[i, j] = counter++%TerrainType.TOTAL_TERRAINS;
			}
		}
	}
}
