using UnityEngine;
using System;

/* IMPORTANT: DONT CHANGE ENUM ORDER!! */
public enum Terrains {
	
	Plains,
	Forest,
	Floor,
	Pillar,
	Mountain,
	Peak,
	Gate,
	Throne,
	Fort,
	River,
	Sea,
	Desert,
	Thicket,
	Wall,
	CrackedWall,
	Door,
	ClosedChest,
	OpenedChest,
	Switch
}

[System.Serializable]
public class TerrainType {

	// Terrain name
	public string name;
	public static readonly int 
		TOTAL_TERRAINS = Enum.GetNames(typeof(Terrains)).Length;

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
	public int life;
}