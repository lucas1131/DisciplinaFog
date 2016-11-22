using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Unit : MonoBehaviour {

	public enum Faction {
		PLAYER,
		ENEMY,
		ALLY
	}

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
    public Class cls;

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

        q.Enqueue(new Pair<Position, int>(new Position(this.pos), this.stats.move));
        while (q.Count > 0) {
            
            Pair<Position, int> p = q.Dequeue();
            Position cur = p.first;
            int curMov = p.second;
            moveArea.Add(cur);
            
            foreach (Position del in deltas) {
                
                Position next = cur + del;
                if (next.IsValid(board) && !visited.Contains(next)) {
                
                    Terrains t = board.GetTerrain(next.x, next.y);
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
