using UnityEngine;
using System.Collections;

public class Combat : MonoBehaviour {

	public static void Battle(Unit attacker, Unit defender, BoardManager board){

		CalculateTriangleBonus(attacker, defender);

	}

	public static float AtkSpeed(Unit u){

		if((u.equipedItem >= 0) && (u.inventory[u.equipedItem] as Equipment).weight <= u.stats.con)
			return u.stats.spd;
		else if((u.equipedItem >= 0) && (u.inventory[u.equipedItem] as Equipment).weight > u.stats.con)
			return ((u.inventory[u.equipedItem] as Equipment).weight - u.stats.con);
		else return 0f;
	}

	public static void DoubleHit(Unit attacker, Unit defender){

		if(attacker.stats.spd >= defender.stats.spd+4)
			attacker.doubleHit = true;
		if(attacker.stats.spd+4 <= defender.stats.spd)
			defender.doubleHit = true;
	}

	public static float HitRate(Unit u){
		return (u.inventory[u.equipedItem] as Equipment).hit + u.stats.skill*2 + u.stats.luck/2;
	}

	public static float Evade(Unit u, BoardManager board){
		return AtkSpeed(u)*2 + u.stats.luck + board.types[board.board[u.posX, u.posY]].avoid;
	}

	public static float Accuracy(Unit attacker, Unit defender, BoardManager board){
		
		return HitRate(attacker) - Evade(defender, board) + attacker.hitBonus;
	}

	public static float AttackPower(Unit u){

		return u.stats.str + ((u.inventory[u.equipedItem] as Equipment).might + u.mtBonus)*u.effectiveness;
	}
	
	
	private static void CalculateTriangleBonus(Unit u1, Unit u2){ 

		Equipment e1, e2;

		// Reset bonuses
		u1.hitBonus = 0;
		u1.mtBonus = 0;
		u2.hitBonus = 0;
		u2.mtBonus = 0;

		// Get units equipment
		if(u1.equipedItem > 0)
			e1 = u1.inventory[u1.equipedItem] as Equipment;
		else return;

		if(u2.equipedItem > 0)
			e2 = u2.inventory[u2.equipedItem] as Equipment;
		else return;

		// Calculate bonuses
		if(e1.equipType == "Sword"){
			if(e2.equipType == "Lance"){
				u2.hitBonus = 15;
				u2.mtBonus = 1;
			} else if(e2.equipType == "Axe"){
				u1.hitBonus = 15;
				u1.mtBonus = 1;
			}
		} else if(e1.equipType == "Lance"){
			if(e2.equipType == "Axe"){
				u2.hitBonus = 15;
				u2.mtBonus = 1;
			} else if(e2.equipType == "Sword"){
				u1.hitBonus = 15;
				u1.mtBonus = 1;
			}
		} else if(e1.equipType == "Axe"){
			if(e2.equipType == "Sword"){
				u2.hitBonus = 15;
				u2.mtBonus = 1;
			} else if(e2.equipType == "Lance"){
				u1.hitBonus = 15;
				u1.mtBonus = 1;
			}
		} else if(e1.equipType == "Anima"){
			if(e2.equipType == "Dark"){
				u2.hitBonus = 15;
				u2.mtBonus = 1;
			} else if(e2.equipType == "Light"){
				u1.hitBonus = 15;
				u1.mtBonus = 1;
			}
		} else if(e1.equipType == "Dark"){
			if(e2.equipType == "Light"){
				u2.hitBonus = 15;
				u2.mtBonus = 1;
			} else if(e2.equipType == "Anima"){
				u1.hitBonus = 15;
				u1.mtBonus = 1;
			}
		} else if(e1.equipType == "Light"){
			if(e2.equipType == "Anima"){
				u2.hitBonus = 15;
				u2.mtBonus = 1;
			} else if(e2.equipType == "Dark"){
				u1.hitBonus = 15;
				u1.mtBonus = 1;
			}
		}
	}

	private static void CalculateEffectiveness(Unit u1, Unit u2){

		Equipment e1, e2;

		u1.effectiveness = 1;
		u2.effectiveness = 1;

		// Get units equipment
		if(u1.equipedItem > 0)
			e1 = u1.inventory[u1.equipedItem] as Equipment;
		else return;

		if(u2.equipedItem > 0)
			e2 = u2.inventory[u2.equipedItem] as Equipment;
		else return;

		// Check against fliers
		if(u1.cls.isFlier && e2.flierBonus)
			u2.effectiveness = 2;
		if(u2.cls.isFlier && e1.flierBonus)
			u1.effectiveness = 2;

		// Check against beasts
		if(u1.cls.isBeast && e2.beastBonus)
			u2.effectiveness = 2;
		if(u2.cls.isBeast && e1.beastBonus)
			u1.effectiveness = 2;
	}
}
