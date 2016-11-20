using UnityEngine;
using System.Collections;

public class Cursor : MonoBehaviour {

	public int poX;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
		float xAxis = Input.GetAxis("Horizontal");
		float yAxis = Input.GetAxis("Vertical");

		// Move cursor right
		if(xAxis > 0){
			print("right");

		// Mover cursor left
		} else if(xAxis < 0){
			print("left");

		}

		// Move cursor up
		if(yAxis > 0){
			print("up");

		
		} else if(yAxis < 0){
			print("down");

		}

		// Action button
		if(Input.GetButtonDown("Action")){
			print("A");

		}

		// Cancel button
		if(Input.GetButtonDown("Cancel")){
			print("B");

		}

		// Left trigger
		if(Input.GetButtonDown("LeftTrigger")){
			print("L");

		}

		// Right trigger
		if(Input.GetButtonDown("RightTrigger")){
			print("R");

		}

		// Start button
		if(Input.GetButtonDown("Start")){
			print("Start");

		}

		// Select button
		if(Input.GetButtonDown("Select")){
			print("select");

		}
	}
}
