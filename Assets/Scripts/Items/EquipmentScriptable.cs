using UnityEngine;
using System;

[CreateAssetMenu(fileName="FE Equipment", menuName="Scriptable/FE Equipment", order=1)]
public class Equipment : ScriptableObject {

    public string Name;
    public string equipType;
    public char rank;
    public Sprite sprite;

    public int range;
    public int weight;
    public int might;
    public int hit;
    public int crit;
    public int curUses;
    public int maxUses;
}
