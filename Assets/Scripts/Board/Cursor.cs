﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Cursor : MonoBehaviour {

	// Cursor movement speed
	public float cursorSpdAlt = 0.5f;
	public float cursorSpdDefault = 0.25f;
	private float cursorSpd;
	
	// Cursor animation speed
	public int delayAlt = 0;
	public int delayDefault = 1;
	private int delay;

	private bool cursorMoved = false;
	private int counter = 0;

	private Transform pos;		// Actual position
	private Vector3 tgtPos;		// Cursor target position
	private Vector3 tgtCamPos;	// Camera target position

	private Unit focusedUnit;	// Cursor is hovering over this unit
	private Unit selectedUnit;	// Player selected this unit to move/act

	// Cursor position in tile grid
	public int posX = 0;
	public int posY = 0;

	// Camera variables
	public int camX = 0;
	public int camY = 0;
	public int camWidth = 14;
	public int camHeight = 10;

	[HideInInspector]
	public GameObject unitWindow;
	[HideInInspector]
	public GameObject mainCamera;
	[HideInInspector]
	public BoardManager board;

	// Use this for initialization
	void Start(){

		// Get objects references
		unitWindow = GameObject.Find("Canvas/UnitWindow");
		mainCamera = GameObject.Find("Camera");
		board = GameObject.Find("Map").GetComponent<BoardManager>();
		
		// Prepare position variables to smoothly move the cursor
		pos = GetComponent<Transform>();
		tgtPos = new Vector3(pos.position.x, pos.position.y, 0f);

		// Cursor animation speed
		cursorSpd = cursorSpdDefault;
		delay = delayDefault;

		tgtCamPos = mainCamera.transform.position;

		// Get first unit without moving the cursor
		focusedUnit = board.GetUnit(posX, posY);
		if(focusedUnit != null)
			ChangeAnimationTo(focusedUnit, "victory");

		// Get terrain info without moving the cursor
		board.DisplayTerrainInfo(posX, posY);

		// First unit window update
		UpdateUnitWindow(focusedUnit);
		mainCamera.transform.position = new Vector3(camX, camY, -10f);
	}
	
	// Update is called once per frame
	void Update(){

		// Move camera if it has a new target position
		if(mainCamera.transform.position != tgtCamPos)
			CameraMove();

		// Move cursor if it has a new target position
		if(pos.position != tgtPos) MoveCursor();
		else ProcessAxis();

		// Update terrain info only if cursor has moved
		if(cursorMoved) 
			board.DisplayTerrainInfo(posX, posY);
		ProcessInput();
		counter++;

		focusedUnit = board.GetUnit(posX, posY);
	}

	void ProcessAxis(){

		cursorMoved = false;
		float xAxis = Input.GetAxis("Horizontal");
		float yAxis = Input.GetAxis("Vertical");

		// Move cursor right
		if(xAxis > 0 && posX < board.cols-1){
			
			// Camera Logic
			if(posX - camX >= camWidth/2 && camX + camWidth/2 <= board.cols) 
				camX++;
			
			posX++;
			cursorMoved = true;
		}

		// Mover cursor left
		else if(xAxis < 0 && posX > 0){
			
			// Camera Logic
			if(posX - camX <= -camWidth/2 && camX - camWidth/2 >= 0) 
				camX--;
			
			posX--;
			cursorMoved = true;
		}

		// Move cursor up
		if(yAxis > 0 && posY < board.rows-1){
			
			// Camera Logic
			if(posY - camY >= camHeight/2-1 && camY + camHeight/2 <= board.rows) 
				camY++;
			
			posY++;
			cursorMoved = true;
		}

		// Move cursor down 
		else if(yAxis < 0 && posY > 0){
			
			// Camera Logic
			if(posY - camY <= -camHeight/2 && camY - camHeight/2 >= 0) 
				camY--;
			
			posY--;
			cursorMoved = true;
		}
		
		tgtPos = new Vector3(posX, posY, 0f);
		tgtCamPos = new Vector3(camX, camY, -10f);
		
		// Get whatever it is in this position in the board, but ony if cursor
		// cursorMoved to avoid getting it every update
		if(cursorMoved){

			// Reset animation to idle if cursor was previously on another unit
			// but only if player hasn't select a unit
			if(selectedUnit == null){
				if(focusedUnit != null)
					ChangeAnimationTo(focusedUnit, "idle");

				// Check new tile for a unit
				focusedUnit = board.GetUnit(posX, posY);
				if(focusedUnit != null && focusedUnit.faction == Faction.PLAYER)
					ChangeAnimationTo(focusedUnit, "victory");
			}

			if(selectedUnit == null)
				UpdateUnitWindow(focusedUnit);
		}

		// Check if cursor is too close to UI menus to change their position
		if(posX - camX <= -3){

			Vector3 pos = new Vector2(340f, -189f);
			board.tInfo.GetComponent<RectTransform>().localPosition = pos;
		}

		else if(posX - camX >= 3){

			Vector3 pos = new Vector2(-340f, -189f);
			board.tInfo.GetComponent<RectTransform>().localPosition = pos;
		}
	}
	
	void ProcessInput(){

		// Action button
		if(Input.GetButtonDown("Action")){
			
			// If no unit has been selected, try to select a unit
			if(selectedUnit == null){

				SelectUnit();

				// Disable terrain and unit windows
				if(selectedUnit){
					unitWindow.SetActive(false);
					board.tInfo.SetActive(false);
				}

				if(selectedUnit){
					print("[DEBUG]: movement list:");
					print(selectedUnit.CalculateMovementArea());
                    //selectedUnit.CalculateMovementArea();
				}
			}

			// We already have a selected unit, try to act
			else if(selectedUnit.faction == Faction.PLAYER){
				selectedUnit.MoveTowards(new Position(posX, posY));
			}
		}

		// Cancel button
		if(Input.GetButtonDown("Cancel")){

			// Dont have a selected unit
			if(selectedUnit == null)
				ChangeCursorSpeed(cursorSpdAlt, delayAlt);

			// Deselect a unit
			// TODO: check for menu nesting first (stack of "selections"?)
			else {
				
				// Revert unit animation to victory only if unit is player's,
				// else revert to idle
				if(selectedUnit.faction == Faction.PLAYER)
					ChangeAnimationTo(selectedUnit, "victory");
				else 
					ChangeAnimationTo(selectedUnit, "idle");

				// Move cursor to top of unit
				posX = selectedUnit.posX;
				posY = selectedUnit.posY;
				tgtPos = new Vector3(posX, posY, 0f);

				// Change unit status to focused only and deselect unit
				focusedUnit = selectedUnit;
				selectedUnit = null;

				UpdateUnitWindow(focusedUnit);
				board.tInfo.SetActive(true);
			}
		}

		if(Input.GetButtonUp("Cancel"))
			ChangeCursorSpeed(cursorSpdDefault, delayDefault);

		// Left trigger
		if(Input.GetButtonDown("LeftTrigger")){
			
			// If no unit is focused, go to first player's unit
			if(focusedUnit == null)
				focusedUnit = board.GetNextUnit(Faction.PLAYER, 0);

			// Go to next unit in focused unit's faction
			else
				focusedUnit = board.GetNextUnit(focusedUnit.faction,
												focusedUnit.index);
			
			if(focusedUnit != null){
				posX = focusedUnit.posX;
				posY = focusedUnit.posY;
				tgtPos = new Vector3(posX, posY, 0f);
			}

			UpdateUnitWindow(focusedUnit);
		}

		// Right trigger
		if(Input.GetButtonDown("RightTrigger")){
			// Open unit menu
		}

		// Start button
		if(Input.GetButtonDown("Start")){
			// Show minimap
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

	void CameraMove(){

		if(delay == 0 || counter%delay == 0){
			
			// Local placeholders to modify pos.position
			float X = mainCamera.transform.position.x;
			float Y = mainCamera.transform.position.y;

			// Reset delay counter
			counter = 0;

			// Interpolate position
			X += (tgtCamPos.x - mainCamera.transform.position.x)*cursorSpd;
			Y += (tgtCamPos.y - mainCamera.transform.position.y)*cursorSpd;

			// Round position interpolation
			if(Mathf.Abs(tgtCamPos.x - mainCamera.transform.position.x) < 0.1f)
				X = tgtCamPos.x;
			if(Mathf.Abs(tgtCamPos.y - mainCamera.transform.position.y) < 0.1f)
				Y = tgtCamPos.y;

			// Update position
			mainCamera.transform.position = new Vector3(X, Y, -10f);
		}
	}

	void ChangeCursorSpeed(float speed, int delay){
		this.delay = delay;
		cursorSpd = speed;
	}

	void SelectUnit(){

		// No unit in square, open menu
		if(focusedUnit == null){

			UI.OpenMenu();


		/* All units should show their movement range when selected. If it
		 *	is a Player unit, check if unit have already cursorMoved first. If it
		 *	has, do nothing, else, select it.
		 */
		// Player unit
		} else {
			
			selectedUnit = focusedUnit;
			ChangeAnimationTo(selectedUnit, "walkDown");

			if(focusedUnit.tag.Equals("Player") && 
					!focusedUnit.hasMoved){
			// Select unit and show its movement

			// Enemy unit
			} else if(focusedUnit.tag.Equals("Enemy")){
			// Select unit and show its movement

			// Ally unit
			} else if(focusedUnit.tag.Equals("Ally")){
			// Select unit and show its movement

			}
		}
	}

	void ChangeAnimationTo(Unit unit, string name){
		
		print("Changin " + unit.unitName + " animation to " + name);
		Animator anim = unit.unitSprite.GetComponent<Animator>();

		anim.SetBool("victory", false);
		anim.SetBool("healed", false);
		anim.SetBool("walkUp", false);
		anim.SetBool("walkDown", false);
		anim.SetBool("walkRight", false);
		anim.SetBool("walkLeft", false);
		anim.SetBool("attackUp", false);
		anim.SetBool("attackDown", false);
		anim.SetBool("attackRight", false);
		anim.SetBool("attackLeft", false);
		anim.SetBool("idle", false);

		anim.SetBool(name, true);
	}

	void UpdateUnitWindow(Unit unit){

		if(unit == null) {
			unitWindow.SetActive(false); 
			return;
		}

		// Enable and update unit window
		/* Unit window hierarchy		Index
		 *	UnitWindow
		 *		|
		 *		|-UnitLifeBar			0
		 *		|-Portrait				1
		 *		|-Name 					2
		 *		\-Life 					3
		 */

		unitWindow.SetActive(true);
		Transform child = unitWindow.transform.GetChild(0);

		// First child is unit life bar
		float health = ((float) unit.curHealth)/((float) unit.maxHealth);

		child.localScale = new Vector3(health, 1f, 1f);

		// Second child is portraits
		child = unitWindow.transform.GetChild(1);
		
		// This child can be set active or not, so we need more work
		// to get a reference to it, however we just want a copy of
		// the unit's portrait, so there is no need to activate it

		// Using temporary variable to add code legibility
		GameObject go =
			utils.FindObject(unit.gameObject, "PortraitSprite");
		
		child.GetComponent<Image>().sprite = 
			go.GetComponent<SpriteRenderer>().sprite;

		// Third child is name
		child = unitWindow.transform.GetChild(2);
		child.GetComponent<Text>().text = unit.unitName;

		// Fourth child is life text
		child = unitWindow.transform.GetChild(3);
		child.GetComponent<Text>().text = 
			unit.curHealth + "/" + unit.maxHealth;
	}
}
