using UnityEngine;
using System.Collections;

public class Cursor : MonoBehaviour {

	private float cursorSpd = 0.25f;
	public float cursorSpdAlt = 0.5f;
	public float cursorSpdDefault = 0.25f;
	
	private int delay = 2;
	public int delayAlt = 0;
	public int delayDefault = 2;

	private int counter = 0;

	Transform pos;	// Actual position
	Vector3 tgtPos;	// Target position

	Unit focusedUnit;

	public int posX = 0;
	public int posY = 0;

	public BoardManager titles;

	// Use this for initialization
	void Start () {
		pos = GetComponent<Transform>();
		tgtPos = new Vector3(pos.position.x, pos.position.y, 0f);
	}
	
	// Update is called once per frame
	void Update () {

		if(pos.position - tgtPos != Vector3.zero) MoveCursor();
		else ProcessAxis();

		ProcessInput();
		counter++;

		focusedUnit = titles.GetUnit(posX, posY);
	}

	void ProcessAxis(){

		float xAxis = Input.GetAxis("Horizontal");
		float yAxis = Input.GetAxis("Vertical");

		// Move cursor right
		if(xAxis > 0 && posX < titles.cols) posX++;
		// Mover cursor left
		else if(xAxis < 0 && posX > 0) posX--;
		// Move cursor up
		if(yAxis > 0 && posY < titles.rows) posY++;
		// Move cursor down 
		else if(yAxis < 0 && posY > 0) posY--;
		
		tgtPos = new Vector3(posX, posY, 0f);
	}
	
	void ProcessInput(){

		// Action button
		if(Input.GetButtonDown("Action")){
			
			// Get whatever it is in this position in the titles
			titles.DisplayTerrainInfo(posX, posY);
			focusedUnit = titles.GetUnit(posX, posY);
			print("unit: " + focusedUnit);
		}

		// Cancel button
		if(Input.GetButtonDown("Cancel")){
			delay = delayAlt;
			cursorSpd = cursorSpdAlt;
		}

		if(Input.GetButtonUp ("Cancel")){
			delay = delayDefault;
			cursorSpd = cursorSpdDefault;
		}

		// Left trigger
		if(Input.GetButtonDown("LeftTrigger")){
			
		}

		// Right trigger
		if(Input.GetButtonDown("RightTrigger")){
			
		}

		// Start button
		if(Input.GetButtonDown("Start")){
			
		}

		// Select button
		if(Input.GetButtonDown("Select")){
			
		}
	}

	void MoveCursor(){

		if(delay == 0 || counter%delay == 0){
			
			// Local placeholders to modify pos.position
			float X = pos.position.x;
			float Y = pos.position.y;

			// Reset delay counter
			counter = 0;

			// Interpolate position
			X += (tgtPos.x - pos.position.x)*cursorSpd;
			Y += (tgtPos.y - pos.position.y)*cursorSpd;
			// Round position interpolation
			if(Mathf.Abs(tgtPos.x - pos.position.x) < 0.1f)
				X = tgtPos.x;
			if(Mathf.Abs(tgtPos.y - pos.position.y) < 0.1f)
				Y = tgtPos.y;

			// Update position
			pos.position = new Vector3(X, Y, 0f);
		}

	}
}
