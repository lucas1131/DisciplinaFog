using UnityEngine;
using System.Collections;

public class Combat : MonoBehaviour {

	private static float atkHitBonus = 0;
	private static float atkMtBonus = 0;

	private static float defHitBonus = 0;
	private static float defMtBonus = 0;

	private static bool attackerDouble = false;
	private static bool defenderDouble = false;

	public static void Battle(Unit attacker, Unit defender, BoardManager board){

		CalculateTriangleBonus((attacker.inventory[attacker.equipedItem] as Equipment),
					(defender.inventory[defender.equipedItem] as Equipment));



		// Reset vars
		atkHitBonus = 0;
		atkMtBonus = 0;
		defHitBonus = 0;
		defMtBonus = 0;
		attackerDouble = false;
		defenderDouble = false;
	}

	private static float AtkSpeed(Unit u){

		if((u.equipedItem >= 0) && (u.inventory[u.equipedItem] as Equipment).weight <= u.stats.con)
			return u.stats.spd;
		else if((u.equipedItem >= 0) && (u.inventory[u.equipedItem] as Equipment).weight > u.stats.con)
			return ((u.inventory[u.equipedItem] as Equipment).weight - u.stats.con);
		else return 0f;
	}

	private static void DoubleHit(Unit attacker, Unit defender){

		if(attacker.stats.spd >= defender.stats.spd+4)
			attackerDouble = true;
		if(attacker.stats.spd+4 <= defender.stats.spd)
			defenderDouble = true;
	}

	private static float HitRate(Unit u){
		return (u.inventory[u.equipedItem] as Equipment).hit + u.stats.skill*2 + u.stats.luck/2;
	}

	private static float Evade(Unit u, BoardManager board){
		return AtkSpeed(u)*2 + u.stats.luck + board.types[board.board[u.posX, u.posY]].avoid;
	}

	private static float Accuracy(Unit attacker, Unit defender, BoardManager board){
		
		return HitRate(attacker) - Evade(defender, board) + atkHitBonus;
	}

	private static void CalculateTriangleBonus(Equipment e1, Equipment e2){ 

		if(e1.equipType == "Sword"){
			if(e2.equipType == "Lance"){
				defHitBonus = 15;
				defMtBonus = 1;
			} else if(e2.equipType == "Axe"){
				atkHitBonus = 15;
				atkMtBonus = 1;
			}
		} else if(e1.equipType == "Lance"){
			if(e2.equipType == "Axe"){
				defHitBonus = 15;
				defMtBonus = 1;
			} else if(e2.equipType == "Sword"){
				atkHitBonus = 15;
				atkMtBonus = 1;
			}
		} else if(e1.equipType == "Axe"){
			if(e2.equipType == "Sword"){
				defHitBonus = 15;
				defMtBonus = 1;
			} else if(e2.equipType == "Lance"){
				atkHitBonus = 15;
				atkMtBonus = 1;
			}
		} else if(e1.equipType == "Anima"){
			if(e2.equipType == "Dark"){
				defHitBonus = 15;
				defMtBonus = 1;
			} else if(e2.equipType == "Light"){
				atkHitBonus = 15;
				atkMtBonus = 1;
			}
		} else if(e1.equipType == "Dark"){
			if(e2.equipType == "Light"){
				defHitBonus = 15;
				defMtBonus = 1;
			} else if(e2.equipType == "Anima"){
				atkHitBonus = 15;
				atkMtBonus = 1;
			}
		} else if(e1.equipType == "Light"){
			if(e2.equipType == "Anima"){
				defHitBonus = 15;
				defMtBonus = 1;
			} else if(e2.equipType == "Dark"){
				atkHitBonus = 15;
				atkMtBonus = 1;
			}
		}
	}
}
