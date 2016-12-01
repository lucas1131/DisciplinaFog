using UnityEngine;
using System.Collections;

public static class EnemyAI : object {

	public static void UpdateEnemy(BoardManager board, Cursor cursor,
		Unit[] playerUnits, Unit[] enemyUnits, Unit[] allyUnits){

		cursor.gameObject.SetActive(false);

		// MoveUnits & attack

		if(allyUnits.Length > 0){
			board.turn = BoardManager.Turn.Ally;
			// AllyAI.UpdateAlly();
		} else {
			
			// Set all player units to move again
			foreach(Unit u in playerUnits)
				u.hasMoved = false;
			cursor.gameObject.SetActive(true);
			board.tInfo.SetActive(true);
			board.turn = BoardManager.Turn.Player;			
		}
	}
}
