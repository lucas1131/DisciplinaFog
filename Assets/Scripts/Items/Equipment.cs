using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Equipment : Item {

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

	// Use this for initialization
	public Equipment(EquipmentScriptable equipScript) {
		this.Name = equipScript.Name;
	    this.sprite = equipScript.sprite;
	    this.curUses = equipScript.curUses;
	    this.maxUses = equipScript.maxUses;

	    this.equipType = equipScript.equipType;
	    this.rank = equipScript.rank;

	    this.range = equipScript.range;
	    this.weight = equipScript.weight;
	    this.might = equipScript.might;
	    this.hit = equipScript.hit;
	    this.crit = equipScript.crit;
	    this.isWeapon = true;
	}
}
