using UnityEngine;
using System;
using System.Collections;

/* NOTE: DONT CHANGE ENUM ORDER!! */
public enum Terrains {
	
	/* Default to 1 movement point */
	Plains = 0,
	Floor,
	ClosedChest,
	OpenedChest,
	Switch,
	Armory,
	Vendor,
	House,
	Village,
	
	/* Default to 2 movement points */
	Forest,
	Pillar,
	Desert,
	Gate,
	Throne,
	Fort,

	/* Default to 4 movement points */
	River,
	Mountain,
	
	/* Default to 999 movement points */
	Sea,
	Peak,
	Thicket,
	Wall,
	CrackedWall,
	Fence,
	Door
}

[System.Serializable]
public class TerrainType {

	// Number of terrains
	public static readonly int 
		TOTAL_TERRAINS = Enum.GetNames(typeof(Terrains)).Length;

	// Terrain tName
	public string tName;

	public bool isWalkable;

	// Terrain status bonuses
	public int avoid;
	public int defense;
	// Recover percentage per turn
	public int recover;
	
	// Used only for cracked walls
	// TODO: change this to something instatiable
	public int life;

	public TerrainType(string name, bool isWalkable, int avoid, 
						int defense, int recover, int life){

		this.avoid = avoid;
		this.defense = defense;
		this.recover = recover;
		this.life = life;

		if(name == "ClosedChest" || name == "OpenedChest") 
			this.tName = "Chest";
		else if(name == "CrackedWall")
			this.tName = "Wall";
		else this.tName = name;
	}
}