using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName="FE Class", menuName="Scriptable/FE Class", order=1)]
public class ClassStats : ScriptableObject {

    [System.Serializable]
    public class Cost {
        [HideInInspector]
        public string Name;
        public int Value;
        public Cost(string s, int i) {
            Name = s;
            Value = i;
        }
    }

    public Cost[] MoveCost = new Cost[] {
        new Cost ("Plains", 999),
        new Cost ("Forest", 999),
        new Cost ("Floor", 999),
        new Cost ("Pillar", 999),
        new Cost ("Mountain", 999),
        new Cost ("Peak", 999),
        new Cost ("Gate", 999),
        new Cost ("Throne", 999),
        new Cost ("Fort", 999),
        new Cost ("River", 999),
        new Cost ("Sea", 999),
        new Cost ("Desert", 999),
        new Cost ("Thicket", 999),
        new Cost ("Wall", 999),
        new Cost ("CrackedWall", 999),
        new Cost ("Door", 999),
        new Cost ("ClosedChest", 999),
        new Cost ("OpenedChest", 999),
        new Cost ("Switch", 999)
    };

    public int GetMovementCost(Terrains t) {
        int cost = MoveCost[(int)t].Value;
        if (cost <= 0)
            cost = 999;
        return cost;
    }
}
