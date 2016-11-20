using UnityEngine;
using System.Collections;

public class Unit : MonoBehaviour {

	// Visuals
	public Sprite portrait;
	public Sprite unit;

	// Counters
	public int health;
	public int level;
	public int exp;
	
	// Status
	public int str;
	public int skill;
	public int spd;
	public int luck;
	public int def;
	public int res;
	public int move;
	public int con;
	public int aid;

	// Position and movement variables
	public int posX = 0;
	public int posY = 0;
	public bool hasMoved = false;

	// Use this for initialization
	void Start () {
		// TODO: read statistics from savefile
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void CalculateMovementArea(){
		
	}
}
