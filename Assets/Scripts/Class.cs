using UnityEngine;

public class Class : ScriptableObject {
    
    public ClassStats stats;

    public int GetMovementCost(Terrains t) {
        return stats.MoveCost[(int)t];
    }
}
