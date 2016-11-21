using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class BoardManager : MonoBehaviour {

	public Text terrainInfo;

	public Unit[] units;
	public int size = 1;
	
	public TerrainType[] terrainsTypes;
	public int[,] tiles;

	public int rows = 8;
	public int cols = 8;

	void Start() {

		// Allocate vectors
		tiles = new int[cols, rows];
		units = new Unit[size];

		// Initialize grid
		for (int i = 0; i < cols; i++) {
			for (int j = 0; j < rows; j++) {
				tiles[i, j] = (int) Terrains.Plains;
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
		tmp = terrainsTypes[tiles[x, y]].name;
		tmp += "\nAvo: " + terrainsTypes[tiles[x, y]].avoid;
		tmp += "\nDef: " + terrainsTypes[tiles[x, y]].defense;
		tmp += "\nRec: " + terrainsTypes[tiles[x, y]].recover;
		if(terrainsTypes[tiles[x, y]].name.Equals("Cracked Wall"))
			tmp += "\nLife: " + terrainsTypes[tiles[x, y]].life;

		terrainInfo.text = tmp;
	}

	public Unit GetUnit(int x, int y){
		return null;
	}

    public Terrains GetTerrain(int x, int y) {
        return terrainsTypes[tiles[x, y]];
    }

	void CreateTestScene(){

		int counter = 0;
		for (int i = 0; i < cols; i++) {
			for (int j = 0; j < rows; j++) {
				tiles[i, j] = counter++%TerrainType.TOTAL_TERRAINS;
			}
		}
	}
}
