using UnityEngine;
using System;

public abstract class Consumable : Item {

    public string Name;
    public string description;
    public Sprite sprite;
    public int curUses;
    public int maxUses;

    public abstract void OnUse(Unit self);
}
