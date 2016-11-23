using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class BoardManager : MonoBehaviour {

	public GameObject tInfo;

	public Unit[] playerUnits;
	public Unit[] enemyUnits;
	public Unit[] allyUnits;
	
	public TerrainType[] types;
	public int[,] board;

	public int rows = 10;
	public int cols = 15;

	void Start() {

		// Allocate grid
		board = new int[cols, rows];

		// Initialize grid
		for (int i = 0; i < cols; i++) {
			for (int j = 0; j < rows; j++) {
				board[i, j] = (int) Terrains.Plains;
			}
		}

		// Initialize types
		InitializeTerrainTypes();
		CreateTestScene();
	}
	
	void Update() {
	
	}

	public void DisplayTerrainInfo(int x, int y){

		/* Hierarchy order		Index

			Terrain Box
				|
				|-Name 			0
				|-Stats 		1
				\-Wall 			2
					|
					|-Sprite 	0
					\-Life 		1
		*/

		// Get name Text (always used)
		Text tName = 
			tInfo.transform.GetChild(0).gameObject.GetComponent<Text>();
		// Get statistics Text (disabled only for unwalkable terrain)
		Text tStats = 
			tInfo.transform.GetChild(1).gameObject.GetComponent<Text>();
		// Get wall handler object (enabled only for Cracked Wall)
		GameObject wallHandler = tInfo.transform.GetChild(2).gameObject;
		
		// Always set terrain name
		tName.text = types[board[x, y]].name;

		// Normal terrains, display name and statistics and disable wall
		if(types[board[x, y]].isWalkable){

			wallHandler.SetActive(false);
			tStats.gameObject.SetActive(true);

			tStats.text = "Def\t\t" + types[board[x, y]].defense + 
						  "\nAvo\t\t" + types[board[x, y]].avoid;

		} else {

			// If its a cracked wall, display image and life
			if(board[x, y] == (int) Terrains.CrackedWall){

				wallHandler.SetActive(true);
				tStats.gameObject.SetActive(false);

				GameObject wallLife = 
					wallHandler.transform.GetChild(1).gameObject;

				// NOTE: This life is wrong, it must be instanciated
				wallLife.GetComponent<Text>().text = ""+types[board[x, y]].life;

			// Else its any other unwalkable terrain, just display its name
			} else {

				wallHandler.SetActive(false);
				tStats.gameObject.SetActive(false);
			}
		}
		
		// tName = types[board[x, y]].name;

		// string tmp;

		// tmp += "\nAvo: " + types[board[x, y]].avoid;
		// tmp += "\nDef: " + types[board[x, y]].defense;
		// tmp += "\nRec: " + types[board[x, y]].recover;
		// if(types[board[x, y]].name.Equals("Cracked Wall"))
		// 	tmp += "\nLife: " + types[board[x, y]].life;
	}

	public Unit GetUnit(int x, int y){

		// Iterate through Player's units list
		foreach(Unit u in playerUnits){

			// Unit found
			if(u.posX == x && u.posY == y)
				return u;
		}

		// Iterate through Enemy's units list
		foreach(Unit u in enemyUnits){

			// Unit found
			if(u.posX == x && u.posY == y)
				return u;
		}

		// Iterate through Ally's units list
		foreach(Unit u in allyUnits){

			// Unit found
			if(u.posX == x && u.posY == y)
				return u;
		}

		// Not found
		return null;
	}

	public Terrains GetTerrain(int x, int y) {
		return (Terrains) board[x, y];
	}

	// Test only
	void CreateTestScene(){

		int counter = 0;
		for (int i = 0; i < cols; i++) 
			for (int j = 0; j < rows; j++) 
				board[i, j] = counter++%TerrainType.TOTAL_TERRAINS;
	}

	// Initialization
	void InitializeTerrainTypes(){

		int i = 0;
		
		types = new TerrainType[19];

		types[i++] = new TerrainType("Forest", 2, true, 10, 1, 0, 0);
		types[i++] = new TerrainType("Plains", 1, true, 0, 0, 0, 0);
		types[i++] = new TerrainType("Floor", 1, true, 0, 0, 0, 0);
		types[i++] = new TerrainType("Pillar", 2, true, 10, 1, 0, 0);
		types[i++] = new TerrainType("Mountain", 4, true, 30, 3, 0, 0);
		types[i++] = new TerrainType("Peak", 5, false, 40, 4, 0, 0);
		types[i++] = new TerrainType("Gate", 2, true, 30, 3, 20, 0);
		types[i++] = new TerrainType("Throne", 2, true, 30, 3, 20, 0);
		types[i++] = new TerrainType("Fort", 2, true, 20, 2, 10, 0);
		types[i++] = new TerrainType("River", 4, true, 0, 0, 0, 0);
		types[i++] = new TerrainType("Sea", 5, false, 0, 0, 0, 0);
		types[i++] = new TerrainType("Desert", 2, true, 0, 0, 0, 0);
		types[i++] = new TerrainType("Thicket", 10, false, 0, 0, 0, 0);
		types[i++] = new TerrainType("Wall", 10, false, 0, 0, 0, 0);
		types[i++] = new TerrainType("Wall", 10, false, 0, 0, 0, 20);
		types[i++] = new TerrainType("Door", 10, false, 0, 0, 0, 0);
		types[i++] = new TerrainType("Chest", 1, true, 0, 0, 0, 0);
		types[i++] = new TerrainType("Chest", 1, true, 0, 0, 0, 0);
		types[i++] = new TerrainType("Switch", 1, true, 0, 0, 0, 0);
	}

	public Unit GetNextUnit(Faction f, int index){
		
		// Get faction units list
		Unit[] list = (f == Faction.PLAYER) ? playerUnits :
						( (f == Faction.ENEMY) ? enemyUnits :
													allyUnits );

		// If list is empty
		if(list.Length == 0) return null;
		return list[(index+1)%list.Length];
	}
}
