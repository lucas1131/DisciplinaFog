﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public enum Faction {
	PLAYER,
	ENEMY,
	ALLY
}

public class Unit : MonoBehaviour {

    public static bool animationHappening = false;

    public GameObject UnitSprite;
    public GameObject EffectSprite;

    [System.Serializable]
    public struct Status {
        public int str;
        public int mag;
        public int skill;
        public int spd;
        public int luck;
        public int def;
        public int res;
        public int move;
        public int con;
        public int aid;
    }

    // Information
    public string unitName;
    public Faction faction;
    public ClassStats cls;
    [HideInInspector]
    public int index = 0;

    // Counters
    public int maxHealth;
    public int curHealth;
    public int level;
    public int exp;
    public Item[] inventory = new Item[5];
    public int equipedItem;

    // Status
    public Status stats;

    // Combat status
    public float hitBonus = 0;
    public float mtBonus = 0;
    public int effectiveness = 1;
    public bool doubleHit = false;

    [HideInInspector]
    public BoardManager board;

    // Children
    [HideInInspector]
    public Transform unitSprite;
    private Transform effectSprite;
    private Transform portraitSprite;

    // Color indicators for unit action
    private Color gray = new Color(0.55f, 0.55f, 0.55f);
    private Color normal = new Color(1f, 1f, 1f);

    // Position and movement variables
    public Position pos = new Position(0, 0);
    public Position prevPos;
    [HideInInspector]
    public bool hasMoved = false;
    public int startX;
    public int startY;

    // Gambiarras do VVillam
    public Queue<Position> pathToTarget;
    private int step;
    public static readonly int nSteps = 8;
    private float stepOffset = 1 / Mathf.Pow(2, 1.0f / nSteps);

    public int prevPosX;
    public int prevPosY;


    public int posX {
        set {
            this.pos.x = value;
            x = value;
        }
        get { return this.pos.x; }
    }

    public int posY {
        set {
            this.pos.y = value;
            y = value;
        }
        get { return this.pos.y; }
    }

    public float x {
        set {
            transform.position = new Vector2(value, transform.position.y);
        }
        get { return transform.position.x; }
    }

    public float y {
        set {
            transform.position = new Vector2(transform.position.x, value);
        }
        get { return transform.position.y; }
    }

    // Use this for initialization
    void Start() {

        // Get objects references from hierarchy
        board = GameObject.Find("Map").GetComponent<BoardManager>();
        unitSprite = this.gameObject.transform.GetChild(0);
        effectSprite = this.gameObject.transform.GetChild(1);
        portraitSprite = this.gameObject.transform.GetChild(2);
        
        this.posX = startX;
        this.posY = startY;
        this.prevPosX = startX;
        this.prevPosY = startY;
        this.transform.position = new Vector2(this.posX, this.posY);

        // Find first equipment in inventory to equip it
        int counter = 0;
        equipedItem = -1;   // no item equiped
        foreach (Item i in inventory) {
            if (i != null && i.isWeapon)
                equipedItem = counter;
            counter++;
        }

        UpdateInventory();

        // TODO: read statistics from savefile
    }

    public List<Position> CalculateMovementArea() {
        HashSet<Position> visited = new HashSet<Position>();
        List<Position> moveArea = new List<Position>();
        Queue<Pair<Position, int>> q = new Queue<Pair<Position, int>>();

        Position[] deltas = new Position[] {
            new Position(0, 1),
            new Position(1, 0),
            new Position(0, -1),
            new Position(-1, 0),
        };

        q.Enqueue(new Pair<Position, int>(pos, stats.move));
        visited.Add(pos);

        while (q.Count > 0) {

            Pair<Position, int> p = q.Dequeue();
            Position cur = p.first;
            int curMov = p.second;

            if (CanStandAt(cur))
                moveArea.Add(cur);

            foreach (Position del in deltas) {

                Position next = cur + del;
                if (next.IsValid(board) && !visited.Contains(next)) {

                    Unit u = board.GetUnit(next.x, next.y);
                    Terrains t = board.GetTerrain(next.x, next.y);
                    int cost = cls.GetMovementCost(t);

                    if (cost <= curMov && this.CanMoveThrough(u))
                        q.Enqueue(new Pair<Position, int>(next, curMov - cost));
                    visited.Add(next);
                }
            }
        }

        return moveArea;
    }

    public List<Unit> EnemiesInMovementRange() {
        List<Unit> units = new List<Unit>();
        HashSet<Position> visited = new HashSet<Position>();
        Queue<Pair<Position, int>> q = new Queue<Pair<Position, int>>();

        Position[] deltas = new Position[] {
            new Position(0, 1),
            new Position(1, 0),
            new Position(0, -1),
            new Position(-1, 0),
        };

        q.Enqueue(new Pair<Position, int>(pos, stats.move));
        visited.Add(pos);

        while (q.Count > 0) {
            Pair<Position, int> p = q.Dequeue();
            Position cur = p.first;
            int curMov = p.second;
            
            Unit u = board.GetUnit(cur);
            if (CanMoveThrough(u)) {
                foreach (Position del in deltas) {

                    Position next = cur + del;
                    if (next.IsValid(board) && !visited.Contains(next)) {

                        u = board.GetUnit(next.x, next.y);
                        Terrains t = board.GetTerrain(next.x, next.y);
                        int cost = cls.GetMovementCost(t);

                        if (cost <= curMov)
                            q.Enqueue(new Pair<Position, int>(next, curMov - cost));
                        visited.Add(next);
                    }
                }
            } else {
                units.Add(u);
            }
        }

        return units;
    }

    public List<Position> CalculateAttackArea() {
        
        if(equipedItem < 0) return new List<Position>();

        List<Position> area = new List<Position>();
        HashSet<Position> visited = new HashSet<Position>();
        Queue<Pair<Position, int>> q = new Queue<Pair<Position, int>>();
        
        Position[] deltas = new Position[] {
            new Position(0, 1),
            new Position(1, 0),
            new Position(0, -1),
            new Position(-1, 0),
        };

        Equipment e = (inventory[equipedItem] as Equipment);
        int[] dists = new int[] { e.rangeMin, e.rangeMax };
        int minDist, maxDist;
        if (dists.Length > 1) {
            minDist = dists[0];
            maxDist = dists[1];
        } else {
            minDist = 1;
            maxDist = dists[0];
        }

        q.Enqueue(new Pair<Position, int>(pos, 0));

        while (q.Count > 0) {
            Pair<Position, int> p = q.Dequeue();
            Position cur = p.first;
            int curDist = p.second;

            if (curDist >= minDist)
                area.Add(cur);

            visited.Add(cur);

            if (curDist < maxDist) {
                foreach (Position del in deltas) {
                    Position next = cur + del;
                    if (next.IsValid(board) && !visited.Contains(next)) {
                        q.Enqueue(new Pair<Position, int>(next, curDist + 1));
                    }
                }
            }
        }

        return area;
    }

	public bool CanMoveThrough(Unit other) {
		if (other == null)
			return true;

		if (this.faction == other.faction)
			return true;
		if (this.faction == Faction.PLAYER)
			return other.faction == Faction.ALLY;
		if (this.faction == Faction.ALLY)
			return other.faction == Faction.PLAYER;

		return false;
	}
    
	private int AStarHeuristic(Position p1, Position p2) {
        return p1.ManhattanDistance(p2);
	}

	private int TileCost(Position p) {
		Terrains t = board.GetTerrain(p.x, p.y);
		return cls.GetMovementCost(t);
	}

	private List<Position> ReconstructPath(Position curr, 
		Dictionary<Position, Position> cameFrom) {

		List<Position> path = new List<Position>();

		while (cameFrom.ContainsKey(curr)) {
			path.Add(curr);
			curr = cameFrom[curr];
		}

		path.Reverse();
		return path;
	}

	public List<Position> PathTo(Position target){

		HashSet<Position> closedSet = new HashSet<Position>();
		HashSet<Position> openSet = new HashSet<Position>();
		
		PriorityQueue<Position> nextPositions = new PriorityQueue<Position>();
		
		Dictionary<Position, Position> cameFrom =
				new Dictionary<Position, Position>();
		
		Dictionary<Position, int> gScore = new Dictionary<Position, int>();
		Dictionary<Position, int> fScore = new Dictionary<Position, int>();
        print(pos + " to " + target);

		// Initialize and start
		gScore[pos] = 0;
		fScore[pos] = AStarHeuristic(pos, target);

		openSet.Add(pos);
		nextPositions.Add(pos, fScore[pos]);

		while(openSet.Count > 0){

			Position current = nextPositions.Pop();
			openSet.Remove(current);
			closedSet.Add(current);

			if (current == target)
				return ReconstructPath(target, cameFrom);

			bool gDefined = gScore.ContainsKey(current);

			foreach (Position p in current.ValidNeighbors(board)) {

				if (!closedSet.Contains(p) && CanMoveThrough(board.GetUnit(p))) {
					int g;

					if (gDefined)
						g = gScore[current] + TileCost(p);
					else
						g = int.MaxValue;

					if (openSet.Add(p)) {
			
						cameFrom[p] = current;
						gScore[p] = g;
						int f = g + AStarHeuristic(p, target);
						fScore[p] = f;
						nextPositions.Add(p, f);
			
					} else if (g < gScore[p]) {
			
						cameFrom[p] = current;
						gScore[p] = g;
						int f = g + AStarHeuristic(p, target);
						fScore[p] = f;
			
						nextPositions.Update(
							x => (x == p),
							x => f
						);
					}
				} 
			}
		}
		return new List<Position>();
	}

	public void MoveTowards(Position target) {
		List<Position> path = PathTo(target);
		int curMove = stats.move;
		int cost;
		int i = 0;

        pathToTarget = new Queue<Position>();
        step = 0;
        if (path != null)
        {
            while (i < path.Count && curMove >= (cost = TileCost(path[i])))
            {
                pathToTarget.Enqueue(path[i]);
                curMove -= cost;
                i++;
            }

            StartCoroutine(MoveFromTo(path, Time.fixedDeltaTime * 10));
        }
    }

    IEnumerator MoveFromTo(List<Position> path, float step){
        
        int i = 0;
        
        animationHappening = true;

        Position prev = pos;//new Position(posX,posY);

        foreach(Position p in path){
            
            setMoveAnimation(prev, p);
            i++;
            Vector2 target = new Vector2(p.x, p.y);
            
            while (Vector2.Distance(target, transform.position) >= 0.05f){
                transform.position = Vector2.MoveTowards(transform.position, target, step);
                yield return null; // Leave the routine and return here in the next frame
            }
            prev = p;
        }

        animationHappening = false;

        if(path.Count > 0){
            posX = path[i - 1].x;
            posY = path[i - 1].y;
        }
    }

    private void setMoveAnimation(Position prev, Position dest){
  
        Position up = new Position(0, 1);
        Position down = new Position(0, -1);
        Position left = new Position(-1, 0);
        Position right = new Position(1, 0);

        //dest-prev
        int i = 0;

        //move vertical 
        if(prev.x == dest.x){
            //vertical pra cima
            if (dest.y - prev.y > 0)
                ChangeAnimationTo("walkUp");
            //vertical pra baixo
            else
                ChangeAnimationTo("walkDown");
        
        //movimento horizontal 
        } else {
            //horizontal pra direita
            if (dest.x - prev.x > 0)
                ChangeAnimationTo("walkRight");
            //horizontal pra esquerda
            else{
                ChangeAnimationTo("walkLeft");
            }
        }
    }

    public bool CanStandAt(Position p) {
        Unit u = board.GetUnit(p);
        return u == null || u == this;
    }

    public void ChangeAnimationTo(string name){

        Animator anim = this.unitSprite.GetComponent<Animator>();

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

    public void UpdateInventory(){
        
        int counter = 0;

        foreach(Item i in inventory){
            if(i != null && i.isWeapon) break;
            counter++;
        }

        equipedItem = ( (counter >= 5) ? -1 : counter);
    }

    public void UpdateColor(){

        if(hasMoved)
            unitSprite.GetComponent<SpriteRenderer>().color = gray;
        else 
            unitSprite.GetComponent<SpriteRenderer>().color = normal;
    }
}
