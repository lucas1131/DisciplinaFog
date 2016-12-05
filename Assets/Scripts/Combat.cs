using UnityEngine;
using System.Collections;

public class Combat : MonoBehaviour {

	public static void Battle(Unit atacker, Unit defender, BoardManager board){

	}

	public static float AtkSpeed(Unit u){

		if((u.equipedItem >= 0) && (u.inventory[u.equipedItem] as Equipment).weight <= u.stats.con)
			return u.stats.spd;
		else if((u.equipedItem >= 0) && (u.inventory[u.equipedItem] as Equipment).weight > u.stats.con)
			return ((u.inventory[u.equipedItem] as Equipment).weight - u.stats.con);
		else return 0f;
	}

	public static void DoubleHit(Unit atacker, Unit defender){

		if(atacker.stats.spd >= defender.stats.spd+4)
			atacker.doubleHit = true;
		if(atacker.stats.spd+4 <= defender.stats.spd)
			defender.doubleHit = true;
	}

	public static float HitRate(Unit u){
		return (u.inventory[u.equipedItem] as Equipment).hit + u.stats.skill*2 + u.stats.luck/2;
	}

	public static float Evade(Unit u, BoardManager board){
		return AtkSpeed(u)*2 + u.stats.luck + board.types[board.board[u.posX, u.posY]].avoid;
	}

	public static float Accuracy(Unit atacker, Unit defender, BoardManager board){
		return HitRate(atacker) - Evade(defender, board) + atacker.hitBonus;
	}

	public static float AttackPower(Unit u, bool isMagical){
		if(isMagical)
			return u.stats.mag + ((u.inventory[u.equipedItem] as Equipment).might + u.mtBonus)*u.effectiveness;
		else
			return u.stats.str + ((u.inventory[u.equipedItem] as Equipment).might + u.mtBonus)*u.effectiveness;
	}

	public static float DefensePower(Unit u, BoardManager board, bool isMagical){
		if(isMagical)
			return board.types[board.board[u.posX, u.posY]].defense + u.stats.res;
		else
			return board.types[board.board[u.posX, u.posY]].defense + u.stats.def;
	}
	
	public static float Damage(Unit atacker, Unit defender, BoardManager board){
		
		bool isMagical;
		float dmg;
		Equipment atkE;
		atkE = (atacker.inventory[atacker.equipedItem] as Equipment);

		isMagical = atkE.equipType == "Anima" || 
					atkE.equipType == "Light" ||
					atkE.equipType == "Dark" || 
					atkE.Name == "Runesword" ||
					atkE.Name == "Lightbrand";

		dmg = AttackPower(atacker, isMagical) - 
			DefensePower(defender, board, isMagical);

		return ( (dmg < 0) ? 0 : dmg);
	}

	public static float CriticalRate(Unit u){

		int bonus = 0;
		Equipment e = (u.inventory[u.equipedItem] as Equipment);

		if(u.cls.Name == "Swordmaster" || u.cls.Name == "Berserker")
			bonus += 15;

		return e.crit + u.stats.skill/2 + bonus;
	}

	public static float CriticalEvade(Unit u){
		return u.stats.luck;
	}

	public static float CriticalChance(Unit atacker, Unit defender){
		float chance = CriticalRate(atacker) - CriticalEvade(defender);
		return ( (chance < 0) ? 0 : chance);
	}
	
	public static void CalculateTriangleBonus(Unit u1, Unit u2){ 

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

	public static void CalculateEffectiveness(Unit u1, Unit u2){

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

		// Check against armored
		if(u1.cls.isArmored && e2.armoredBonus)
			u2.effectiveness = 2;
		if(u2.cls.isArmored && e1.armoredBonus)
			u1.effectiveness = 2;

		// Check against dragon
		if(u1.cls.isDragon && e2.dragonBonus)
			u2.effectiveness = 2;
		if(u2.cls.isDragon && e1.dragonBonus)
			u1.effectiveness = 2;
	}
}
