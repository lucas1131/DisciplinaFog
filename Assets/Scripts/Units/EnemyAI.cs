using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public static class EnemyAI {

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

        /* MoveUnits & attack */
        foreach (Unit u in board.enemyUnits) {
            if (u == null)
                continue;
            Unit target = FindTarget(board, u);
            u.MoveTowards(target.pos);
            while (Unit.animationHappening)
                yield return new WaitForEndOfFrame();

            if (u.CalculateAttackArea().Contains(target.pos)) {
                Combat.Battle(u, target, board);
                while (Unit.animationHappening)
                    yield return new WaitForEndOfFrame();
            }
        }

        // Set all player units to move again
        foreach (Unit u in board.playerUnits) {
            if (u == null)
                continue;
            u.hasMoved = false;
            u.UpdateColor();
        }
        running = false;
        BoardManager.turn = BoardManager.Turn.Player;
        PhaseAnimator.animationIsPlaying = true;
    }

}

