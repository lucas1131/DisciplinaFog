using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public abstract class Consumable : Item {

    public string Name;
    public string description;
    public Sprite sprite;
    public int curUses;
    public int maxUses;

    private Text usesText;

    public abstract void OnUse(Unit self);

    void Start(){
    	usesText = transform.GetChild(2).gameObject.GetComponent<Text>();
    }

    void Update(){
    	usesText.text = curUses + "/" + maxUses;
    }
}
