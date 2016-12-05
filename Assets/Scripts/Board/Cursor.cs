using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

/* The Cursor game object acts as player controller.
	As of now, it also acts as a game manager toghether with the BoardManager
	(this should be changed!).
*/
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

	[HideInInspector]
	public static Unit focusedUnit;		// Cursor is hovering over this unit
	[HideInInspector]
	public static Unit selectedUnit;	// Player selected this unit to move/act

	// Cursor position in tile grid
	public Position position = new Position(0, 0);
	public int posX {
		get { return position.x; }
		set { position.x = value; }
	}
	public int posY {
		get { return position.y; }
		set { position.y = value; }
	}

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
	public BattleMenuController battleMenu;

	// Movement and related variables
	public MovementDisplay md;
	private List<Position> path;			// Calculated path from A*
	private List<Position> possibleMoves;	// For CalculateMovementArea

	// ns mais oq eu to faseno
	private List<Unit> possibleAtks;
	private bool atkCase = false;
	private uint atkindex = 0;

	// Use this for initialization
	void Start(){

		// Get objects references
		unitWindow = GameObject.Find("Canvas/UnitWindow");
		mainCamera = GameObject.Find("Camera");
		board = GameObject.Find("Map").GetComponent<BoardManager>();
		md = board.GetComponent<MovementDisplay>();
		
		// Prepare position variables to smoothly move the cursor
		// Also set cursor initial position to the Lord
		pos = GetComponent<Transform>();
		tgtPos = new Vector3(board.playerUnits[0].x, board.playerUnits[0].y, 0f);
		pos.position = tgtPos;
		posX = (int) pos.position.x;
		posY = (int) pos.position.y;

		// Cursor animation speed
		cursorSpd = cursorSpdDefault;
		delay = delayDefault;

		tgtCamPos = mainCamera.transform.position;

		// Get first unit without moving the cursor
		focusedUnit = board.GetUnit(posX, posY);
		if(	focusedUnit != null && 
			!focusedUnit.hasMoved && 
			focusedUnit.faction == Faction.PLAYER){

			focusedUnit.ChangeAnimationTo("victory");
		}

		// Get terrain info without moving the cursor
		board.DisplayTerrainInfo(posX, posY);

		// First unit window update
		UpdateUnitWindow(focusedUnit);
		mainCamera.transform.position = new Vector3(camX, camY, -10f);
		ProcessAxis();
	}

	// Update is called once per frame
	void Update(){

		if(PhaseAnimator.animationIsPlaying) return;

		if(BoardManager.turn == BoardManager.Turn.Player)
			UpdatePlayer();
		
		else if(BoardManager.turn == BoardManager.Turn.Enemy)
			EnemyAI.UpdateEnemy(board, this, board.playerUnits, 
				board.enemyUnits, board.allyUnits);

		else if(BoardManager.turn == BoardManager.Turn.Ally)
			AllyAI.UpdateAlly();
	}

	void UpdatePlayer(){

		// Move camera if it has a new target position
		if(mainCamera.transform.position != tgtCamPos)
			CameraMove();

		// Move cursor if it has a new target position
		if(pos.position != tgtPos)
			MoveCursor();

		// If we are in atack case
		if(atkCase){
			if(pos.position == tgtPos &&
				(Input.GetAxis("Horizontal") > 0 ||
				Input.GetAxis("Vertical") > 0) ){
				
				atkindex = ((uint) atkindex+1u)%( (uint) possibleAtks.Count);
				position = possibleAtks[(int)atkindex].pos;
				tgtPos = possibleAtks[(int)atkindex].transform.position;
				cursorMoved = true;
			}
			if(pos.position == tgtPos &&
				(Input.GetAxis("Horizontal") < 0 ||
				Input.GetAxis("Vertical") < 0) ){
				
				atkindex = ((uint) atkindex-1u)%( (uint) possibleAtks.Count);
				position = possibleAtks[(int)atkindex].pos;
				tgtPos = possibleAtks[(int)atkindex].transform.position;
				cursorMoved = true;
			}
		}

		// Only update cursor if battle menu is off
		if(!battleMenu.isActiveAndEnabled){
			if(pos.position == tgtPos)
				ProcessAxis();

			// Update terrain info only if cursor has moved
			if(cursorMoved) {
				board.DisplayTerrainInfo(posX, posY);

				if (selectedUnit != null && selectedUnit.faction == Faction.PLAYER) {
					
					if(md.arrows != null)
						foreach(GameObject go in md.arrows)
							GameObject.Destroy(go);
					md.arrows = new List<GameObject>();
					
					if((posX != selectedUnit.posX || posY != selectedUnit.posY)
							&& possibleMoves != null 
							&& possibleMoves.Contains(position)) {
						List<Position> path = selectedUnit.PathTo(position);
						
						md.u = selectedUnit;
						if(selectedUnit.faction == Faction.PLAYER && path != null)
							md.CreateArrows(md.arrows, path);
					}
				}
				cursorMoved = false;
			}
		}

		// We still need to process input here
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
					focusedUnit.ChangeAnimationTo("idle");

				// Check new tile for a unit
				focusedUnit = board.GetUnit(posX, posY);
				if(	focusedUnit != null && 
					!focusedUnit.hasMoved && 
					focusedUnit.faction == Faction.PLAYER){

					focusedUnit.ChangeAnimationTo("victory");
				}
			}

			if(selectedUnit == null)
				UpdateUnitWindow(focusedUnit);
		}

		// Check if cursor is too close to UI menus to change their position
		if(posX - camX <= -3){

			Vector3 pos = new Vector2(340f, -189f);
			board.tInfo.GetComponent<RectTransform>().localPosition = pos;

			unitWindow.transform.localPosition = new Vector3(316f, 227f, 0f);
		}

		else if(posX - camX >= 3){

			Vector3 pos = new Vector2(-340f, -189f);
			board.tInfo.GetComponent<RectTransform>().localPosition = pos;
			
			unitWindow.transform.localPosition = new Vector3(-301f, 227f, 0f);
		}
	}

	void ProcessInput(){

		if (!Unit.animationHappening){

			// Action button
			if (Input.GetButtonDown("Action")){

				// If we are atacking
				if(atkCase){

					// Combat.Battle(selectedUnit, board.GetUnit(position), board);

					// Set unit action as done, so it cannot move again
					// selectedUnit.hasMoved = true;

					// Revert animation back to idle
					// selectedUnit.ChangeAnimationTo("idle");

					// Move cursor back to player's unit
					// position = selectedUnit.pos;
					// tgtPos = selectedUnit.pos.ToVector2();
					
					// Stop spawning arrows
					// possibleMoves = null;
						
					// Deselect unit
					// focusedUnit = selectedUnit;
					// selectedUnit.UpdateColor();
					// selectedUnit = null;
					
					// Desativate atack
					// atkCase = false;

					// Update Unit and Terrain windows
					// UpdateUnitWindow(focusedUnit);
					// board.tInfo.SetActive(true);

				// Battle menu is open
				} else if(battleMenu.isActiveAndEnabled){
					ProcessMenu();

				// Battle menu is NOT open
				} else {

					// If no unit has been selected, try to select a unit
					if (selectedUnit == null){
						SelectUnit();

						if (selectedUnit){

							// Disable terrain and unit windows
							unitWindow.SetActive(false);
							board.tInfo.SetActive(false);

							// Instantiate blue squares
							possibleMoves = selectedUnit.CalculateMovementArea();
							md.moveTiles = new List<GameObject>();

							foreach (Position p in possibleMoves){
								md.moveTiles.Add(
									Instantiate(md.moveTilePrefab,
												new Vector3(p.x, p.y, 0f),
												Quaternion.identity)
									as GameObject
								);
							}
						}
					/* We already have a selected unit, try to act */
					} else if (selectedUnit.faction == Faction.PLAYER){

						Position p = new Position(posX, posY);
						// Check path if position is inside calculated movement area
						if (possibleMoves != null && possibleMoves.Contains(p)){

							// this.gameObject.SetActive(false);	

							md.DestroyMovementDisplay();
							selectedUnit.prevPosX = selectedUnit.posX;
							selectedUnit.prevPosY = selectedUnit.posY;
							selectedUnit.MoveTowards(p);

							// Used to check for trade/rescue/atack options
							// This ignores weapons with atack range different
							// than 1
							Unit[] adjacent = new Unit[4];

							adjacent[0] = board.GetUnit(posX + 1, posY);
							adjacent[1] = board.GetUnit(posX - 1, posY);
							adjacent[2] = board.GetUnit(posX, posY + 1);
							adjacent[3] = board.GetUnit(posX, posY - 1);

							StartCoroutine(WaitToOpenMenu(new bool[] {
								((selectedUnit.equipedItem >= 0) &&
								CanAttack(adjacent)),	// atack

								CanRescue(adjacent),	// rescue
								true,					// item
								CanTrade(adjacent),		// trade
								true,					// wait
								false,					// unit
								false,					// status
								false					// end
							}));
						}
					}
				}
			}

			// Cancel button
			if (Input.GetButtonDown("Cancel")){

				// If we selected a unit, return it to original position
				if (selectedUnit != null){
					selectedUnit.posX = selectedUnit.prevPosX;
					selectedUnit.posY = selectedUnit.prevPosY;
				}

				// Deactivate menu
				if (battleMenu.isActiveAndEnabled)
					battleMenu.gameObject.SetActive(false);

				// Dont have a selected unit
				if (selectedUnit == null){
					ChangeCursorSpeed(cursorSpdAlt, delayAlt);

				atkCase = false;

				// Deselect a unit
				// TODO: check for menu nesting first (stack of "selections"?)
				} else {

					// Dont have a selected unit
					if (selectedUnit == null){
						ChangeCursorSpeed(cursorSpdAlt, delayAlt);

					} else {
						// Deselect a unit
						// TODO: check for menu nesting first (stack of "selections"?)

						// Delete blue tiles if exists
						md.DestroyMovementDisplay();

						// Move cursor back to top of unit
						tgtPos = new Vector3(selectedUnit.posX, selectedUnit.posY, 0f);

						// Revert unit animation to victory only if unit is player's,
						// else revert to idle
						if (selectedUnit.faction == Faction.PLAYER)
							selectedUnit.ChangeAnimationTo("victory");
						else
							selectedUnit.ChangeAnimationTo("idle");

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
			}

			if (Input.GetButtonUp("Cancel"))
				ChangeCursorSpeed(cursorSpdDefault, delayDefault);

			// Left trigger
			if (Input.GetButtonDown("LeftTrigger")){

				// If no unit is focused, go to first player's unit
				if (focusedUnit == null)
					focusedUnit = board.GetNextUnit(Faction.PLAYER, 0);

				// Go to next unit in focused unit's faction
				else focusedUnit = board.GetNextUnit(focusedUnit.faction,
													focusedUnit.index);

				if (focusedUnit != null){
					posX = focusedUnit.posX;
					posY = focusedUnit.posY;
					tgtPos = new Vector3(posX, posY, 0f);
				}
				UpdateUnitWindow(focusedUnit);
			}

			// Right trigger
			if (Input.GetButtonDown("RightTrigger")){
				// Open unit menu
			}

			// Start button
			if (Input.GetButtonDown("Start")){
				// Show minimap
			}
		}
	}

	void ProcessMenu(){

		possibleAtks = new List<Unit>();
		Unit[] adjacent = new Unit[4];
		int i = 0;

		Position[] deltas = new Position[] {
			new Position(posX+1, posY),
			new Position(posX-1, posY),
			new Position(posX, posY+1),
			new Position(posX, posY-1)
		};

		adjacent[i] = board.GetUnit(deltas[i++]);
		adjacent[i] = board.GetUnit(deltas[i++]);
		adjacent[i] = board.GetUnit(deltas[i++]);
		adjacent[i] = board.GetUnit(deltas[i++]);

		i = 0;

		switch(battleMenu.getCurrentEntry().name){
		case "Attack":
			
			foreach(Unit u in adjacent){
				
				if(u == null) continue;
				if(u.faction == Faction.ENEMY)
					possibleAtks.Add(u);
			}

			atkCase = true;
			battleMenu.gameObject.SetActive(false);
			
			// Move cursor between possible atks
			if(possibleAtks[0] != null){
				position = possibleAtks[0].pos;
				tgtPos = possibleAtks[0].transform.position;
			}

			break;

		case "Rescue":

			break;
		case "Item":

			break;
		case "Trade":

			break;
		case "Wait":
			
			// Update windows info
			UpdateUnitWindow(selectedUnit);
			board.tInfo.SetActive(true);

			// Set unit action as done, so it cannot move again
			selectedUnit.hasMoved = true;

			// Revert animation back to idle
			selectedUnit.ChangeAnimationTo("idle");
			
			// Stop spawning arrows
			possibleMoves = null;

			// RIP battle menu
			battleMenu.gameObject.SetActive(false);
				
			// Deselect unit
			focusedUnit = selectedUnit;
			selectedUnit.UpdateColor();
			selectedUnit = null;
			break;

		case "Unit":

			break;
		case "Status":

			break;
		case "End":
			
			// Deactivate info windows
			unitWindow.SetActive(false);
			board.tInfo.SetActive(false);

			// RIP battle menu
			battleMenu.gameObject.SetActive(false);
			BoardManager.turn = BoardManager.Turn.Enemy;	
			PhaseAnimator.PlayAnimation = true;
			break;
		}
	}

	IEnumerator WaitToOpenMenu(bool[] entries){

		while(Unit.animationHappening)
			yield return new WaitForSeconds(0.1f);

		battleMenu.gameObject.SetActive(true);
		battleMenu.OpenMenu(entries);
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

		// No unit in square or unit has already moved, open menu
		if(focusedUnit == null || focusedUnit.hasMoved){

			battleMenu.gameObject.SetActive(true);
			battleMenu.OpenMenu( new bool[]{ 
				false,					// atack
				false,					// rescue
				false,					// item
				false,					// trade
				false,					// wait
				true,					// unit
				true,					// status
				true					// end
			});


		/* All units should show their movement range when selected. If it
		 *	is a Player unit, check if unit have already cursorMoved first. If it
		 *	has, do nothing, else, select it.
		 */
		// Player unit
		} else {
			
			selectedUnit = focusedUnit;
			selectedUnit.ChangeAnimationTo("walkDown");
		}
	}

	bool CanTrade(Unit[] adjacent){

		foreach(Unit u in adjacent){
			if(u != null && u.faction == Faction.PLAYER && u != selectedUnit)
				return true;
		}
		return false;
	}

	bool CanRescue(Unit[] adjacent){

		foreach(Unit u in adjacent){
			if(u != null && selectedUnit != null &&
				(u.faction == Faction.PLAYER || u.faction == Faction.ALLY) &&
				selectedUnit.stats.aid >= u.stats.aid &&
				u != selectedUnit)
				return true;
		}
		return false;
	}

	bool CanAttack(Unit[] adjacent){

		// Search for enemies 
		foreach(Unit u in adjacent){
			if(u != null && u.faction == Faction.ENEMY)
				return true;
		}
		return false;
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
