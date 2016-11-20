using UnityEngine;

public class Class : ScriptableObject {
    
    public int[] moveCost;

    public int GetMovementCost(Terrains t) {
        return moveCost[(int)t];
    }
}
