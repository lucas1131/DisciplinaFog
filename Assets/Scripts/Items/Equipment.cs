using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Equipment : Item {

    public EquipmentScriptable es;

	public string Name;
    public Sprite sprite;
    public int curUses;
    public int maxUses;

    public string equipType;
    public char rank;

    public int rangeMin;
    public int rangeMax;
    public int weight;
    public int might;
    public int hit;
    public int crit;

    public bool beastBonus = false;
    public bool flierBonus = false;
    public bool armoredBonus = false;
    public bool dragonBonus = false;

	// Use this for initialization
    void Start(){

        this.Name = es.Name;
        this.sprite = es.sprite;
        this.curUses = es.curUses;
        this.maxUses = es.maxUses;

        this.equipType = es.equipType;
        this.rank = es.rank;

        this.rangeMin = es.rangeMin;
        this.rangeMax = es.rangeMax;
        this.weight = es.weight;
        this.might = es.might;
        this.hit = es.hit;
        this.crit = es.crit;
        this.isWeapon = es;

        this.beastBonus = es.beastBonus;
        this.flierBonus = es.flierBonus;
        this.armoredBonus = es.armoredBonus;
        this.dragonBonus = es.dragonBonus;
        this.gameObject.SetActive(es);
    }

	public Equipment(EquipmentScriptable equipScript) {
		this.Name = equipScript.Name;
	    this.sprite = equipScript.sprite;
	    this.curUses = equipScript.curUses;
	    this.maxUses = equipScript.maxUses;

	    this.equipType = equipScript.equipType;
	    this.rank = equipScript.rank;

	    this.rangeMin = equipScript.rangeMin;
	    this.rangeMax = equipScript.rangeMax;
	    this.weight = equipScript.weight;
	    this.might = equipScript.might;
	    this.hit = equipScript.hit;
	    this.crit = equipScript.crit;
	    this.isWeapon = true;

    	this.beastBonus = equipScript.beastBonus;
    	this.flierBonus = equipScript.flierBonus;
    	this.armoredBonus = equipScript.armoredBonus;
    	this.dragonBonus = equipScript.dragonBonus;
	}
}
