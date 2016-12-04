using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class BoardManager : MonoBehaviour {

	public enum Turn {
		Player,
		Enemy,
		Ally
	}

    [HideInInspector]
	public GameObject tInfo;
	[HideInInspector]
	public Turn turn = Turn.Player;

    [HideInInspector]
	public Unit[] playerUnits, enemyUnits, allyUnits;
	
	public TerrainType[] types = 
		Array.ConvertAll<Terrains, TerrainType>(
        	Enum.GetValues(typeof(Terrains)) as Terrains[],
            t => new TerrainType(t.ToString(), true, 0, 0, 0, 0)
        );

	public int[,] board;

	public int rows = 10;
	public int cols = 15;

	void Start() {

		// Get objects references
		tInfo = GameObject.Find("Canvas/TerrainWindow");

		// Find all units
		playerUnits = GameObject.Find("PUnits").GetComponentsInChildren<Unit>();
		enemyUnits = GameObject.Find("EUnits").GetComponentsInChildren<Unit>();
		allyUnits = GameObject.Find("AUnits").GetComponentsInChildren<Unit>();

		// Initialize units lists
		InitUnits(playerUnits);
		InitUnits(enemyUnits);
		InitUnits(allyUnits);

		// Allocate grid
		board = new int[cols, rows];

		// Initialize grid
		for (int i = 0; i < cols; i++) {
			for (int j = 0; j < rows; j++) {
				board[i, j] = (int) Terrains.Plains;
			}
		}

		// Initialize test scene
		CreateTestScene();
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
		tName.text = types[board[x, y]].tName;

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
	}

    public Unit GetUnit(Position p) {
        return GetUnit(p.x, p.y);
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

		for (int i = 0; i < cols; i++) 
			for (int j = 0; j < rows; j++) 
				board[i, j] = (int) Terrains.Plains;

		board[0, 0] = (int) Terrains.River;
		board[0, 1] = (int) Terrains.River;
		board[0, 2] = (int) Terrains.River;
		
		board[2, 0] = (int) Terrains.Forest;
		
		board[9, 0] = (int) Terrains.River;
		board[10, 0] = (int) Terrains.River;
		board[11, 0] = (int) Terrains.River;
		board[12, 0] = (int) Terrains.River;
		board[12, 1] = (int) Terrains.River;
		
		board[0, 9] = (int) Terrains.Mountain;
		board[1, 9] = (int) Terrains.Mountain;

		board[2, 9] = (int) Terrains.Wall;
		board[3, 9] = (int) Terrains.Wall;
		board[4, 9] = (int) Terrains.Wall;
		board[2, 8] = (int) Terrains.Wall;
		board[3, 8] = (int) Terrains.Wall;
		board[4, 8] = (int) Terrains.Wall;
		board[2, 7] = (int) Terrains.Wall;
		board[4, 7] = (int) Terrains.Wall;

		board[7, 9] = (int) Terrains.Forest;
		
		board[13, 2] = (int) Terrains.Forest;
		board[13, 1] = (int) Terrains.Forest;
		
		board[9, 9] = (int) Terrains.Mountain;
		board[10, 9] = (int) Terrains.Mountain;
		board[11, 8] = (int) Terrains.Mountain;
		board[12, 8] = (int) Terrains.Mountain;
		board[13, 7] = (int) Terrains.Mountain;
		board[14, 7] = (int) Terrains.Mountain;
		
		board[14, 9] = (int) Terrains.Peak;
		board[13, 9] = (int) Terrains.Peak;
		board[12, 9] = (int) Terrains.Peak;
		board[11, 9] = (int) Terrains.Peak;
		board[14, 8] = (int) Terrains.Peak;
		board[13, 8] = (int) Terrains.Peak;
		
		// DEBUG
		board[12, 2] = (int) Terrains.Peak;

	}

	public void InitUnits(Unit[] units){

		int counter = 0;
		foreach (Unit u in units)
			u.index = counter++;
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
