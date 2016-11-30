using UnityEngine;
using System;

public abstract class Consumable : MonoBehaviour {

    public string Name;
    public string description;
    public int curUses;
    public int maxUses;
    public Sprite sprite;

    public abstract void OnUse(Unit u);
}
