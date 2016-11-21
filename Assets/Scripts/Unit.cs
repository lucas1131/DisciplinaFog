using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Unit : MonoBehaviour {


	// Visuals
	public Sprite portrait;
	public Sprite unit;

	// Counters
    public Class cls;
	public int health;
	public int level;
	public int exp;
	
	// Status
	public int str;
	public int skill;
	public int spd;
	public int luck;
	public int def;
	public int res;
	public int move;
	public int con;
	public int aid;

    public BoardManager board;

	// Position and movement variables
    public Position pos = new Position(0, 0);
	public int posX {
        get { return this.pos.x; }
    }
	public int posY {
        get { return this.pos.y; }
    }
	public bool hasMoved = false;

	// Use this for initialization
	void Start () {
		// TODO: read statistics from savefile
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	List<Position> CalculateMovementArea() {
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

        q.Enqueue(new Pair<Position, int>(new Position(this.pos), curMov));
        while (q.Count() > 0) {
            Pair<Position, int> p = q.Dequeue();
            Position cur = p.first;
            int curMov = p.second;
            moveArea.Add(cur);
            foreach (Position del in deltas) {
                Position next = cur + del;
                if (next.IsValid(board) && !visited.Contains(next)) {
                    Terrains t = board.titles[next.x][next.y];
                    int cost = cls.GetMovementCost(t);
                    if (cost <= curMov)
                        q.Enqueue(new Pair<Position, int>(next, curMov - cost));
                    visited.Add(next);
                }
            }
        }

        moveArea.Remove(this.pos);

        return moveArea;
	}
}
