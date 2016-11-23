using UnityEngine;
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

	private Transform pos;	// Actual position
	private Vector3 tgtPos;	// Target position

	private Unit focusedUnit;	// Cursor is hovering over this unit
	private Unit selectedUnit;	// Player selected this unit to move/act

	// Cursor position in tile grid
	public int posX = 0;
	public int posY = 0;

	public BoardManager tiles;
	public GameObject unitWindow;

	// Use this for initialization
	void Start () {
		
		// Prepare position variables to smoothly move the cursor
		pos = GetComponent<Transform>();
		tgtPos = new Vector3(pos.position.x, pos.position.y, 0f);

		// Cursor animation speed
		cursorSpd = cursorSpdDefault;
		delay = delayDefault;

		// Get first unit without moving the cursor
		focusedUnit = tiles.GetUnit(posX, posY);
		if(focusedUnit != null)
			ChangeAnimationTo(focusedUnit, "victory");


		// Get terrain info without moving the cursor
		tiles.DisplayTerrainInfo(posX, posY);
	}
	
	// Update is called once per frame
	void Update () {

		if(pos.position != tgtPos) MoveCursor();
		else ProcessAxis();

		// Update terrain info only if cursor has moved
		if(cursorMoved) 
			tiles.DisplayTerrainInfo(posX, posY);
		ProcessInput();
		counter++;

		focusedUnit = tiles.GetUnit(posX, posY);
	}

	void ProcessAxis(){

		cursorMoved = false;
		float xAxis = Input.GetAxis("Horizontal");
		float yAxis = Input.GetAxis("Vertical");

		// Move cursor right
		if(xAxis > 0 && posX < tiles.cols-1){
			posX++;
			cursorMoved = true;
		}

		// Mover cursor left
		else if(xAxis < 0 && posX > 0){
			posX--;
			cursorMoved = true;
		}

		// Move cursor up
		if(yAxis > 0 && posY < tiles.rows-1){
			posY++;
			cursorMoved = true;
		}

		// Move cursor down 
		else if(yAxis < 0 && posY > 0){
			posY--;
			cursorMoved = true;
		}
		
		tgtPos = new Vector3(posX, posY, 0f);
		
		// Get whatever it is in this position in the tiles, but ony if cursor
		// cursorMoved to avoid getting it every update
		if(cursorMoved){

			// Reset animation to idle if cursor was previously on another unit
			// but only if player hasn't select a unit
			if(selectedUnit == null){
				if(focusedUnit != null)
					ChangeAnimationTo(focusedUnit, "idle");

				// Check new tile for a unit
				focusedUnit = tiles.GetUnit(posX, posY);
				if(focusedUnit != null){
					
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
					float health = focusedUnit.curHealth/focusedUnit.maxHealth;
					child.localScale = new Vector2(health, 1f);

					// Second child is portraitas
					child = unitWindow.transform.GetChild(1);
					//child.GameObject.GetComponent<Image>().sprite = ;

					if(focusedUnit.faction == Unit.Faction.PLAYER)
						ChangeAnimationTo(focusedUnit, "victory");
				
				// If there is no focused unit, disable unit window
				} else unitWindow.SetActive(false);
			}
		}
	}
	
	void ProcessInput(){

		// Action button
		if(Input.GetButtonDown("Action")){
			
			// If no unit has been selected, try to select a unit
			if(selectedUnit == null)
				SelectUnit();

			// We already have a selected unit, try to act
			else {

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
				if(selectedUnit.faction == Unit.Faction.PLAYER)
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
			}
		}

		if(Input.GetButtonUp("Cancel"))
			ChangeCursorSpeed(cursorSpdDefault, delayDefault);

		// Left trigger
		if(Input.GetButtonDown("LeftTrigger")){
			// Go to next unit
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
}
