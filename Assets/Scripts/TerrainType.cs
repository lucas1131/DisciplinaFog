using UnityEngine;
using System;
using System.Collections;

/* NOTE: DONT CHANGE ENUM ORDER!! */
public enum Terrains {
	
	Plains = 0,
	Forest = 1,
	Floor = 2,
	Pillar = 3,
	Mountain = 4,
	Peak = 5,
	Gate = 6,
	Throne = 7,
	Fort = 8,
	River = 9,
	Sea = 10,
	Desert = 11,
	Thicket = 12,
	Wall = 13,
	CrackedWall = 14,
	Door = 15,
	ClosedChest = 16,
	OpenedChest = 17,
	Switch = 18
}

[System.Serializable]
public class TerrainType {

	// Number of terrains
	public static readonly int 
		TOTAL_TERRAINS = Enum.GetNames(typeof(Terrains)).Length;

	// Terrain name
	public string name;

	// Terrain sprite
	// public Sprite sprite;

	// Default to 1
	public int baseWeight = 1;

	// Default to true
	public bool isWalkable = true;

	// Terrain status bonuses
	public int avoid;
	public int defense;
	// Recover percentage per turn
	public int recover;
	
	// Used only for cracked walls
	// TODO: change this to something instatiable
	public int life;

	public TerrainType(string name, int weight, bool walkable, int avoid,
						int defense, int recover, int life){
		this.name = name;
		this.baseWeight = weight;
		this.isWalkable = walkable;
		this.avoid = avoid;
		this.defense = defense;
		this.recover = recover;
		this.life = life;
	}
}