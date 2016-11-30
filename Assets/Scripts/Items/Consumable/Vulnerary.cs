using UnityEngine;
using System.Collections;

public class Vulnerary : Consumable {

	public override void OnUse(Unit self){

		if((self.curHealth += 10) > self.maxHealth)
			self.curHealth = self.maxHealth;
	}
}
