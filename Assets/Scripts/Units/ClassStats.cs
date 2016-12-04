using UnityEngine;
using System;
using System.Collections.Generic;

[CreateAssetMenu(fileName="FE Class", menuName="Scriptable/FE Class", order=1)]
public class ClassStats : ScriptableObject {

    public string Name;

    public bool isBeast = false;
    public bool isFlier = false;
    public bool isArmored = false;
    public bool isDragon = false;
    
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

    public Cost[] MoveCost = Array.ConvertAll<Terrains, Cost>(
                                 Enum.GetValues(typeof(Terrains)) as Terrains[],
                                 t => new Cost(t.ToString(), 999)
                             );

    public int GetMovementCost(Terrains t) {
        int cost = MoveCost[(int)t].Value;
        if (cost <= 0)
            cost = 999;
        return cost;
    }
}
