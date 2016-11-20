using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class BoardManager : MonoBehaviour {

	public Unit[] units;
    public int size = 1;
    
    public TerrainType[] terrainsTypes;
    public int[,] titles;

    public int rows = 8;
    public int cols = 8;

    void Start() {

        // Allocate vectors
        titles = new int[rows, cols];
		units = new Unit[size];

        // Initialize vectors
		for (int i = 0; i < rows; i++) {
			for (int j = 0; j < cols; j++) {
				titles [i, j] = 0;
                units = null;
			}
		}
    }
    
	void Update() {
	
	}

    public void DisplayTerrainInfo(int x, int y){
        print("Name: " + terrainsTypes[titles[x, y]].name);
        print("Avoid: " + terrainsTypes[titles[x, y]].avoid);
        print("Defense: " + terrainsTypes[titles[x, y]].defense);
        print("Recover: " + terrainsTypes[titles[x, y]].recover);
    }

    public Unit GetUnit(int x, int y){
        return null;
    }
}
