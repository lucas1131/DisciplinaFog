using UnityEngine;
using System;
using System.Collections.Generic;

[CreateAssetMenu(fileName="FE Equipment", menuName="Scriptable/FE Equipment", order=1)]
public class Equipment : ScriptableObject {

    public string Name;
    public string equipType;
    public char rank;
    public int range;
    public int weight;
    public int might;
    public int hit;
    public int crit;
    
}
