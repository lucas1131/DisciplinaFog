using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Cursor : MonoBehaviour {

    public enum ArrowType {
        STUMP_UP,
        STUMP_DOWN,
        STUMP_LEFT,
        STUMP_RIGHT,

        UP_DOWN,
        UP_LEFT,
        UP_RIGHT,

        DOWN_UP,
        DOWN_LEFT,
        DOWN_RIGHT,

        LEFT_RIGHT,

        RIGHT_LEFT,

        ARROW_UP,
        ARROW_DOWN,
        ARROW_LEFT,
        ARROW_RIGHT
    }

    public Sprite[] arrowSprites;

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
	public GameObject moveTilePrefab;
    public GameObject arrowPrefab;
	public List<GameObject> moveTiles;
    public List<GameObject> arrows;
	private List<Position> path;			// Calculated path from A*
	private List<Position> possibleMoves;	// For CalculateMovementArea

	// Use this for initialization
	void Start(){

		// Get objects references
		unitWindow = GameObject.Find("Canvas/UnitWindow");
		mainCamera = GameObject.Find("Camera");
		board = GameObject.Find("Map").GetComponent<BoardManager>();
		
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

			ChangeAnimationTo(focusedUnit, "victory");
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

		if(board.turn == BoardManager.Turn.Player)
			UpdatePlayer();
		else if(board.turn == BoardManager.Turn.Enemy)
			EnemyAI.UpdateEnemy(board, this, board.playerUnits, 
				board.enemyUnits, board.allyUnits);
		// else if(board.turn == BoardManager.Turn.Ally)
		// 	AllyAI.UpdateAlly();
	}

	void UpdatePlayer(){

		// Only update cursor if battle menu is off
		if(!battleMenu.isActiveAndEnabled){
		
			// Move camera if it has a new target position
			if(mainCamera.transform.position != tgtCamPos)
				CameraMove();

			// Move cursor if it has a new target position
			if(pos.position != tgtPos)
				MoveCursor();
			else
				ProcessAxis();

			// Update terrain info only if cursor has moved
			if(cursorMoved) {
				board.DisplayTerrainInfo(posX, posY);

	            if (selectedUnit != null) {
	                if (arrows != null)
	                    foreach (GameObject go in arrows)
	                        GameObject.Destroy(go);
	                arrows = new List<GameObject>();
	                if ((posX != selectedUnit.posX || posY != selectedUnit.posY)
	                        && possibleMoves != null 
	                        && possibleMoves.Contains(position)) {
	                    List<Position> path = selectedUnit.PathTo(position);
	                    CreateArrows(arrows, path);
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
					ChangeAnimationTo(focusedUnit, "idle");

				// Check new tile for a unit
				focusedUnit = board.GetUnit(posX, posY);
				if(	focusedUnit != null && 
					!focusedUnit.hasMoved && 
					focusedUnit.faction == Faction.PLAYER){

					ChangeAnimationTo(focusedUnit, "victory");
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
        if (!Global.animationHappening)
        {
            // Action button
            if (Input.GetButtonDown("Action"))
            {

                // Battle menu is open
                if (battleMenu.isActiveAndEnabled)
                {

                    ProcessMenu();

                    // Battle menus is NOT open
                }
                else
                {

                    // If no unit has been selected, try to select a unit
                    if (selectedUnit == null)
                    {
                        SelectUnit();

                        if (selectedUnit)
                        {

                            // Disable terrain and unit windows
                            unitWindow.SetActive(false);
                            board.tInfo.SetActive(false);

                            // Instantiate blue squares
                            possibleMoves = selectedUnit.CalculateMovementArea();
                            moveTiles = new List<GameObject>();

                            foreach (Position p in possibleMoves)
                            {
                                moveTiles.Add(
                                    Instantiate(moveTilePrefab,
                                                new Vector3(p.x, p.y, 0f),
                                                Quaternion.identity)
                                    as GameObject
                                );
                            }
                        }
                    }

                    // We already have a selected unit, try to act
                    else if (selectedUnit.faction == Faction.PLAYER)
                    {

                        Position p = new Position(posX, posY);
                        // Check path if position is inside calculated movement area
                        if (possibleMoves != null && possibleMoves.Contains(p))
                        {

                            // this.gameObject.SetActive(false);	

                            DestroyMovementDisplay();
                            selectedUnit.prevPosX = selectedUnit.posX;
                            selectedUnit.prevPosY = selectedUnit.posY;
                            selectedUnit.MoveTowards(p);

                            Unit[] adjacent = new Unit[4];

                            adjacent[0] = board.GetUnit(posX + 1, posY);
                            adjacent[1] = board.GetUnit(posX - 1, posY);
                            adjacent[2] = board.GetUnit(posX, posY + 1);
                            adjacent[3] = board.GetUnit(posX, posY - 1);

                            battleMenu.gameObject.SetActive(true);
                            battleMenu.OpenMenu(new bool[]{
                            ((selectedUnit.equipedItem >= 0) && CanAttack(adjacent)),	// attack
							CanRescue(adjacent),	// rescue
							true,					// item
							CanTrade(adjacent),		// trade
							true,					// wait
							false,					// unit
							false,					// status
							false					// end
						});
                        }
                    }
                }
            }

            // Cancel button
            if (Input.GetButtonDown("Cancel"))
            {

                // Deactivate menu
                if (battleMenu.isActiveAndEnabled)
                {

                    battleMenu.gameObject.SetActive(false);

                    if (selectedUnit != null)
                    {
                        selectedUnit.posX = selectedUnit.prevPosX;
                        selectedUnit.posY = selectedUnit.prevPosY;
                    }
                }

                // Dont have a selected unit
                if (selectedUnit == null)
                {
                    ChangeCursorSpeed(cursorSpdAlt, delayAlt);

                    // Deselect a unit
                    // TODO: check for menu nesting first (stack of "selections"?)
                }
                else
                {

                    // Dont have a selected unit
                    if (selectedUnit == null)
                    {
                        ChangeCursorSpeed(cursorSpdAlt, delayAlt);

                    }
                    else
                    {
                        // Deselect a unit
                        // TODO: check for menu nesting first (stack of "selections"?)

                        // Delete blue tiles if exists
                        DestroyMovementDisplay();

                        // Move cursor back to top of unit
                        tgtPos = new Vector3(selectedUnit.posX, selectedUnit.posY, 0f);

                        // Revert unit animation to victory only if unit is player's,
                        // else revert to idle
                        if (selectedUnit.faction == Faction.PLAYER)
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
            }

            if (Input.GetButtonUp("Cancel"))
                ChangeCursorSpeed(cursorSpdDefault, delayDefault);

            // Left trigger
            if (Input.GetButtonDown("LeftTrigger"))
            {

                // If no unit is focused, go to first player's unit
                if (focusedUnit == null)
                    focusedUnit = board.GetNextUnit(Faction.PLAYER, 0);

                // Go to next unit in focused unit's faction
                else
                    focusedUnit = board.GetNextUnit(focusedUnit.faction,
                                                    focusedUnit.index);

                if (focusedUnit != null)
                {
                    posX = focusedUnit.posX;
                    posY = focusedUnit.posY;
                    tgtPos = new Vector3(posX, posY, 0f);
                }

                UpdateUnitWindow(focusedUnit);
            }

            // Right trigger
            if (Input.GetButtonDown("RightTrigger"))
            {
                // Open unit menu
            }

            // Start button
            if (Input.GetButtonDown("Start"))
            {
                // Show minimap
            }
        }
	}

	void ProcessMenu(){

		Unit[] adjacent = new Unit[4];
		Unit aux;

		adjacent[0] = board.GetUnit(posX+1, posY);
		adjacent[1] = board.GetUnit(posX-1, posY);
		adjacent[2] = board.GetUnit(posX, posY+1);
		adjacent[3] = board.GetUnit(posX, posY-1);

		switch(battleMenu.getCurrentEntry().name){
		case "Attack":
			
			foreach(Unit u in adjacent){
				if( (aux = board.GetUnit(u.pos + adjacent[0].pos)).faction == Faction.ENEMY ){
					posX =  aux.posX;
					posY =  aux.posY;
				}
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
			ChangeAnimationTo(selectedUnit, "idle");
			
			// Stop spawning arrows
			possibleMoves = null;

			// RIP battle menu
			battleMenu.gameObject.SetActive(false);
				
			// Deselect unit
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
			board.turn = BoardManager.Turn.Enemy;				
			break;
		}
	}

    void DestroyMovementDisplay() {
        foreach(GameObject go in moveTiles)
            GameObject.Destroy(go);

        if (arrows != null)
            foreach(GameObject go in arrows)
                GameObject.Destroy(go);
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

			battleMenu.gameObject.SetActive(true);
			battleMenu.OpenMenu( new bool[]{ 
				false,					// attack
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
		} else if(!focusedUnit.hasMoved){
			
			selectedUnit = focusedUnit;
			ChangeAnimationTo(selectedUnit, "walkDown");
		}
	}

	bool CanTrade(Unit[] adjacent){

		foreach(Unit u in adjacent){
			if(u != null && u.faction == Faction.PLAYER)
				return true;
		}
		return false;
	}

	bool CanRescue(Unit[] adjacent){

		foreach(Unit u in adjacent){
			if(u != null && selectedUnit != null &&
				(u.faction == Faction.PLAYER || u.faction == Faction.ALLY) &&
				selectedUnit.stats.aid >= u.stats.aid)
				return true;
		}
		return false;
	}

	bool CanAttack(Unit[] adjacent){

		// Search for equipments in unit's inventory
		foreach(Item i in selectedUnit.inventory){
			
		}

		// Search for enemies 
		foreach(Unit u in adjacent){
			if(u != null && u.faction == Faction.ENEMY)
				return true;
		}
		return false;
	}

	public static void ChangeAnimationTo(Unit unit, string name){

		if(unit == null) return;
		
		Animator anim = unit.unitSprite.GetComponent<Animator>();

		anim.SetBool("idle", false);
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

	void AddArrow(Position p, List<GameObject> arr, ArrowType type) {
        GameObject go = Instantiate(
            arrowPrefab,
            new Vector3(p.x, p.y, 0f),
            Quaternion.identity
        ) as GameObject;
        go.GetComponent<SpriteRenderer>().sprite = arrowSprites[(int) type];
        arr.Add(go);
    }

    void AddArrow(Position p, List<GameObject> arr, Position p1, Position p2) {
        AddArrow(p, arr, GetArrowType(p1, p, p2));
    }

    ArrowType GetArrowType(Position p1, Position p, Position p2) {
        p1 = p - p1;
        p2 = p2 - p;

        if (p1.x < 0) {
            if (p2.x < 0)
                return ArrowType.RIGHT_LEFT;
            else if (p2.y > 0)
                return ArrowType.UP_RIGHT;
            return ArrowType.DOWN_RIGHT;
        }

        if (p1.x > 0) {
            if (p2.x > 0)
                return ArrowType.LEFT_RIGHT;
            else if (p2.y > 0)
                return ArrowType.UP_LEFT;
            return ArrowType.DOWN_LEFT;
        }

        if (p1.y > 0) {
            if (p2.y > 0)
                return ArrowType.DOWN_UP;
            else if (p2.x < 0)
                return ArrowType.DOWN_LEFT;
            return ArrowType.DOWN_RIGHT;
        }

        // p1.y < 0
        if (p2.y < 0)
            return ArrowType.UP_DOWN;
        if (p2.x > 0)
            return ArrowType.UP_RIGHT;
        return ArrowType.UP_LEFT;
    }

    void CreateArrows(List<GameObject> arr, List<Position> path) {
        Position d;
        int i = 1;

        d = path[0] - selectedUnit.pos;
        if (d.x > 0)
            AddArrow(selectedUnit.pos, arr, ArrowType.STUMP_RIGHT);
        else if (d.x < 0)
            AddArrow(selectedUnit.pos, arr, ArrowType.STUMP_LEFT);
        else if (d.y > 0)
            AddArrow(selectedUnit.pos, arr, ArrowType.STUMP_UP);
        else
            AddArrow(selectedUnit.pos, arr, ArrowType.STUMP_DOWN);

        if (path.Count > 1) {
            AddArrow(path[0], arr, selectedUnit.pos, path[1]);

            while (i < path.Count-1) {
                AddArrow(path[i], arr, path[i-1], path[i+1]);
                i++;
            }

            d = path[i] - path[i-1];
            if (d.x > 0)
                AddArrow(path[i], arr, ArrowType.ARROW_RIGHT);
            else if (d.x < 0)
                AddArrow(path[i], arr, ArrowType.ARROW_LEFT);
            else if (d.y > 0)
                AddArrow(path[i], arr, ArrowType.ARROW_UP);
            else
                AddArrow(path[i], arr, ArrowType.ARROW_DOWN);
        } else {
            if (d.x > 0)
                AddArrow(path[0], arr, ArrowType.ARROW_RIGHT);
            else if (d.x < 0)
                AddArrow(path[0], arr, ArrowType.ARROW_LEFT);
            else if (d.y > 0)
                AddArrow(path[0], arr, ArrowType.ARROW_UP);
            else
                AddArrow(path[0], arr, ArrowType.ARROW_DOWN);
        }
    }
}
