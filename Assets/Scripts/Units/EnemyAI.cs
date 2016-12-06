using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public static class EnemyAI : object {

    public static bool running = false;

    private static Unit FindTarget(BoardManager board, Unit cur) {
        List<Unit> viableTargets = cur.EnemiesInMovementRange();
        foreach (Unit candidate in viableTargets)
            if (candidate.curHealth <= Combat.DamageAgainst(cur, candidate, board)) // Could kill candidate in 1 turn
                return candidate;

        return board.playerUnits[0];
    }

    public static IEnumerator UpdateEnemy(BoardManager board, Cursor cursor) {
        running = true;
        Debug.Log(1);

        /* MoveUnits & attack */
        foreach (Unit u in board.enemyUnits) {
            Unit target = FindTarget(board, u);
            u.MoveTowards(target.pos);
            Debug.Log(2);
            while (Unit.animationHappening)
                yield return new WaitForEndOfFrame();
            Debug.Log(3);

            if (u.CalculateAttackArea().Contains(target.pos)) {
                Debug.Log(4);
                Combat.Battle(u, target, board);
                Debug.Log(5);
                while (Unit.animationHappening)
                    yield return new WaitForEndOfFrame();
                Debug.Log(6);
            }
        }

        // Set all player units to move again
        foreach (Unit u in board.playerUnits) {
            u.hasMoved = false;
            u.UpdateColor();
        }
        running = false;
        BoardManager.turn = BoardManager.Turn.Player;
        PhaseAnimator.animationIsPlaying = true;
    }

}

