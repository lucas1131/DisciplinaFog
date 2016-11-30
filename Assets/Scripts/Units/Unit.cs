using UnityEngine;
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
	[HideInInspector]
	public int index = 0;

	// Counters
	public int maxHealth;
	public int curHealth;
	public int level;
	public int exp;
	
	// Status
	public Status stats;

	[HideInInspector]
	public BoardManager board;

	// Children
	[HideInInspector]
	public Transform unitSprite;
	private Transform effectSprite;
	private Transform portraitSprite;

	// Position and movement variables
	public Position pos = new Position(0, 0);
	[HideInInspector]
	public bool hasMoved = false;
	public int startX;
	public int startY;

    public Queue<Position> pathToTarget;
    private int step;
    public static readonly int nSteps = 8;
    private float stepOffset = 1/Mathf.Pow(2, 1.0f/nSteps);

	public int posX {
		set { this.pos.x = value; }
		get { return this.pos.x; }
	}

	public int posY {
		set { this.pos.y = value; }
		get { return this.pos.y; }
	}

    public float x {
        set {
            transform.position = new Vector2(value+0.5f, transform.position.y);
        }
        get { return transform.position.x-0.5f; }
    }

    public float y {
        set {
            transform.position = new Vector2(transform.position.x, value);
        }
        get { return transform.position.y; }
    }

	// Use this for initialization
	void Start () {

		// Get objects references from hierarchy
		board = GameObject.Find("Map").GetComponent<BoardManager>();
		unitSprite = this.gameObject.transform.GetChild(0);
		effectSprite = this.gameObject.transform.GetChild(1);
		portraitSprite = this.gameObject.transform.GetChild(2);

		this.posX = startX;
		this.posY = startY;
		this.transform.position = new Vector2(this.posX, this.posY);    

		// TODO: read statistics from savefile
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

				if (!closedSet.Contains(p)) {
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

		return null;
	}

    void Update() {
        if (pathToTarget != null) {
            //TODO coisas
        }
    }

	public void MoveTowards(Position target) {
		List<Position> path = PathTo(target);
		int curMove = stats.move;
		int cost;
		int i = 0;

        pathToTarget = new Queue<Position>();
        step = 0;
		while(i < path.Count && curMove >= (cost = TileCost(path[i]))){
			print("path["+i+"]: " + path[i]);
            pathToTarget.Enqueue(path[i]);
			curMove -= cost;
			i++;
		}
	}

    public bool CanStandAt(Position p) {
        Unit u = board.GetUnit(p);
        return u == null || u == this;
    }
}
