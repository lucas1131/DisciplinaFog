using UnityEngine;
using System.Collections;

public class Vulnerary : Consumable {

	public override void OnUse(Unit u){
		
		if((u.curHealth += 10) > u.maxHealth)
			u.curHealth = u.maxHealth;
	}
}
