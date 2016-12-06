using UnityEngine;
using System.Collections;

using System.Collections.Generic;

public static class EnemyAI : object {

    private static Unit FindTarget(BoardManager board, Unit cur) {
        List<Unit> viableTargets = cur.EnemiesInMovementRange();
        foreach (Unit candidate in viableTargets)
            if (candidate.curHealth <= Combat.DamageAgainst(cur, candidate, board)) // Could kill candidate in 1 turn
                return candidate;

        return board.playerUnits[0];
    }

	public static IEnumerator UpdateEnemy(BoardManager board, Cursor cursor,
		Unit[] playerUnits, Unit[] enemyUnits, Unit[] allyUnits) {
		cursor.gameObject.SetActive(false);

        /* MoveUnits & attack */
        foreach (Unit u in enemyUnits) {
            Unit target = FindTarget(board, u);
            u.MoveTowards(target.pos);
            while (Unit.animationHappening)
                yield return new WaitForEndOfFrame();

            if (u.CalculateAttackArea().Contains(target.pos))
                Combat.Battle(u, target, board);
        }

        // Set all player units to move again
        foreach (Unit u in playerUnits){
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
