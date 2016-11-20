using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class BoardManager : MonoBehaviour {

    
	public TerrainType[] terrainsTypes;
	public int[,] titles;

    public int rows = 8;
    public int cols = 8;

    void Start() {

        // Allocate grid
		titles = new int[rows, cols];

        // Initialize grid
		for (int i = 0; i < rows; i++) {
			for (int j = 0; j < cols; j++) {
				titles [i, j] = 0;
			}
		}
    }
    
	
	void Update() {
	
	}
}
