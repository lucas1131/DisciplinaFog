﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum Faction {
	PLAYER,
	ENEMY,
	ALLY
}

public class Unit : MonoBehaviour {


	[System.Serializable]
	public struct Status {
		public int str;
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
    public int index = 0;

	// Counters
	public int maxHealth;
	public int curHealth;
	public int level;
	public int exp;
	
	// Status
	public Status stats;

    public BoardManager board;

    // Children
    [HideInInspector]
    public Transform unitSprite;
    [HideInInspector]
	public Transform effectSprite;
    [HideInInspector]
	public Transform portraitSprite;

	// Position and movement variables
    public Position pos = new Position(0, 0);
    [HideInInspector]
	public bool hasMoved = false;
	public int startX;
	public int startY;

	public int posX {
        set { this.pos.x = value; }
        get { return this.pos.x; }
    }

	public int posY {
        set { this.pos.y = value; }
        get { return this.pos.y; }
    }

	// Use this for initialization
	void Start () {

		unitSprite = this.gameObject.transform.GetChild(0);
		effectSprite = this.gameObject.transform.GetChild(1);
		portraitSprite = this.gameObject.transform.GetChild(2);

		this.posX = startX;
		this.posY = startY;
		this.transform.position = new Vector2(this.posX, this.posY);	

		// TODO: read statistics from savefile
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public List<Position> CalculateMovementArea() {
        List<Position> visited = new List<Position>();  // Não tem Set em c# e
                                                        // não vou implementar
        List<Position> moveArea = new List<Position>();
        Queue<Pair<Position, int>> q = new Queue<Pair<Position, int>>();

        Position[] deltas = new Position[] {
            new Position(0, 1),
            new Position(1, 0),
            new Position(0, -1),
            new Position(-1, 0),
        };

        q.Enqueue(
                    new Pair<Position, int>(
                        new Position(this.pos),
                        this.stats.move
                    )
        );
        while (q.Count > 0) {
            Pair<Position, int> p = q.Dequeue();
            Position cur = p.first;
            int curMov = p.second;

            if (board.GetUnit(cur.x, cur.y) == null)
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

        moveArea.Remove(this.pos);

        return moveArea;
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

    // Manhattan distance
    private int AStarHeuristic(Position p1, Position p2) {
        Position d = p1 - p2;
        if (d.x < 0)
            d.x *= -1;
        if (d.y < 0)
            d.y *= -1;
        return d.x + d.y;
    }

    private int TileCost(Position p) {
        Terrains t = board.GetTerrain(p.x, p.y);
        return cls.GetMovementCost(t);
    }

    private List<Position> ReconstructPath(
            Position curr,
            Dictionary<Position, Position> cameFrom) {
        List<Position> path = new List<Position>();

        while (cameFrom.ContainsKey(curr)) {
            path.Add(curr);
            curr = cameFrom[curr];
        }

        path.Reverse();
        return path;
    }

    private List<Position> PathTo(Position target) {
        HashSet<Position> closedSet = new HashSet<Position>();
        HashSet<Position> openSet = new HashSet<Position>();
        PriorityQueue<Position> nextPositions = new PriorityQueue<Position>();
        Dictionary<Position, Position> cameFrom =
                new Dictionary<Position, Position>();
        Dictionary<Position, int> gScore = new Dictionary<Position, int>();
        Dictionary<Position, int> fScore = new Dictionary<Position, int>();

        gScore[pos] = 0;
        fScore[pos] = AStarHeuristic(pos, target);

        openSet.Add(pos);
        nextPositions.Add(pos, fScore[pos]);

        while (openSet.Count > 0) {
            Position current = nextPositions.Pop();
            openSet.Remove(current);
            closedSet.Add(pos);

            if (current == target)
                return ReconstructPath(target, cameFrom);

            foreach (Position p in current.ValidNeighbors(board)) {
                if (!closedSet.Contains(p)) {
                    int g = gScore[current] + TileCost(p);

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

        return null;
    }

    public void MoveTowards(Position target) {
        List<Position> path = PathTo(target);
        int curMove = stats.move;
        int cost;
        int i = 0;

        // TODO PRINT AQUI PRA TESTAR CAMINHOAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA

        while (curMove >= (cost = TileCost(path[i]))) {
            curMove -= cost;
            i++;
        }

        if (i > 0)
            pos = path[i-1];
    }
}
