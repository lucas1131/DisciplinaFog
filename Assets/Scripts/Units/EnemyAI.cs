using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyAI : MonoBehaviour {
	private List<Position> candidates;
	private List<Position> area;
	private int i;
	
	public BoardManager board;
	public Unit[] playerUnits, enemyUnits;
	public Cursor cursor;
	public bool coroutine;

	private void Awake() {
		i = 0;
		coroutine = false;
		candidates = new List<Position>();
	}

	private void Update() {
		if(!coroutine && (BoardManager.turn == BoardManager.Turn.Enemy)) StartCoroutine("AICoroutine");
	}

	public IEnumerator AICoroutine() {
	   coroutine = true;
       Unit u = board.enemyUnits[i++];
       
       if(i >= board.enemyUnits.Length) {
       		if(board.allyUnits.Length > 0){
			BoardManager.turn = BoardManager.Turn.Ally;
			PhaseAnimator.PlayAnimation = true;
			} else {
				BoardManager.turn = BoardManager.Turn.Player;
				PhaseAnimator.PlayAnimation = true;
				cursor.gameObject.SetActive(true);
				board.tInfo.SetActive(true);
			}
       }
       
       else ProcessAI(u);
       coroutine = false;

       yield return null;
	}

	public void ProcessAI(Unit u) {
		if(!u) print("U EH NULO");
		area = u.CalculateAttackArea();
		if(area == null) print("FODEU");
		//if(!area) print("AREA EH NULA");

		foreach(Position p in area) {
			Unit v = board.GetUnit(p);

			if((v != null) && (v.faction != u.faction))
				candidates.Add(p);
		}

		area = u.CalculateMovementArea();

		foreach(Position p in candidates) {
			if(area.Contains(p)) {
				u.MoveTowards(p);
				Combat.Battle(u, board.GetUnit(p), board);
				u.hasMoved = true;
				u.UpdateColor();
				break;
			}
		}
	}
}

