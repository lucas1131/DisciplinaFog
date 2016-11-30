using UnityEngine;
using System;

[CreateAssetMenu(fileName="FE Equip", menuName="Scriptable/FE Equip", order=1)]
public class EquipmentScriptable : ScriptableObject {

    public string Name;
    public Sprite sprite;
    public int curUses;
    public int maxUses;

    public string equipType;
    public char rank;

    public string range;
    public int weight;
    public int might;
    public int hit;
    public int crit;
}
