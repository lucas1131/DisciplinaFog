using UnityEngine;
using System;

public abstract class Misc : MonoBehaviour {

    public string Name;
    public string description;
    public Sprite sprite;
    public int curUses;
    public int maxUses;

    public abstract void OnUse(GameObject go);
}
