using UnityEngine;
using System.Collections;

public static class EnemyAI : object {

	public static IEnumerator UpdateEnemy(BoardManager board, Cursor cursor,
		Unit[] playerUnits, Unit[] enemyUnits, Unit[] allyUnits){

		cursor.gameObject.SetActive(false);

		/* MoveUnits & attack */
			
		// Set all player units to move again
		foreach(Unit u in playerUnits){
			u.hasMoved = false;
			u.UpdateColor();
		}

		if(board.allyUnits.Length > 0){
			BoardManager.turn = BoardManager.Turn.Ally;
			PhaseAnimator.PlayAnimation = true;
		} else {

			BoardManager.turn = BoardManager.Turn.Player;
			PhaseAnimator.PlayAnimation = true;

			// Activate Cursor object
			cursor.gameObject.SetActive(true);

			yield return new WaitForSeconds(1f);
			PhaseAnimator.PlayAnimation = true;
		}
	}
}
