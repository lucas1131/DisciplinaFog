using UnityEngine;

public class Class : ScriptableObject {
    
    public int[] moveCost;

    public int MovementCost(Terrains t) {
        return moveCost[(int)t];
    }
}
