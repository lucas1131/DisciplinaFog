using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Combat : MonoBehaviour {

    private static void Kill(ref Unit[] units, Unit ded)
    {
        List<Unit> list = new List<Unit>(units);
        list.Remove(ded);
        units = list.ToArray();
    }

    private static void Kill(BoardManager board, Unit u)
    {
        Unit[] v;
        if (u.faction == Faction.PLAYER)
            Kill(ref board.playerUnits, u);
        else if (u.faction == Faction.ALLY)
            Kill(ref board.allyUnits, u);
        else
            Kill(ref board.enemyUnits, u);

        GameObject.Destroy(u.gameObject);
    }

	public static IEnumerator Battle(Unit attacker, Unit defender, BoardManager board){

		float atkDmg;
		float atkHitRate;
		float atkCritRate;

		float defDmg;
		float defHitRate;
		float defCritRate;

		string atkAnimName = null;
		string defAnimName = null;

		// Discover their relative position to select apropriate animation
		Position relative = attacker.pos - defender.pos;

		if(relative.x == 1){
			atkAnimName = "attackLeft";
			defAnimName = "attackRight";
		} else if(relative.x == -1){
			atkAnimName = "attackRight";
			defAnimName = "attackLeft";
		} else if(relative.y == 1){
			atkAnimName = "attackDown";
			defAnimName = "attackUp";
		} else if(relative.y == -1){
			atkAnimName = "attackUp";
			defAnimName = "attackDown";
		}

		/* Placeholders */
		Animator atkAnim = attacker.transform.GetChild(0).GetComponent<Animator>();
		Animator defAnim = defender.transform.GetChild(0).GetComponent<Animator>();
		// attacker.ChangeAnimationTo(atkAnimName);
		// defender.ChangeAnimationTo(defAnimName);
		// atkAnim.Stop();
		// defAnim.Stop();
		/* Placeholders */

		DoubleHit(attacker, defender);
		CalculateEffectiveness(attacker, defender);
		CalculateTriangleBonus(attacker, defender);

		atkDmg = Damage(attacker, defender, board);
		atkHitRate = Accuracy(attacker, defender, board);
		atkCritRate = CriticalChance(attacker, defender);

		defDmg = Damage(defender, attacker, board);
		defHitRate = Accuracy(defender, attacker, board);
		defCritRate = CriticalChance(defender, attacker);

		// Atacker attack!
		attacker.ChangeAnimationTo(atkAnimName);
		if(Random.Range(0f, 100f) <= atkHitRate){
            print("oi1");
            // atkAnim.Play("attackLeft", -1, 0f);
			defender.curHealth -= (int) atkDmg;
            print("h: " + defender.curHealth);
			if(defender.curHealth <= 0)
            {
                print("oi caralho");
                Kill(board, defender);
				attacker.ChangeAnimationTo("idle");
				yield return null;
			}
		}

		// yield return new WaitForSeconds(1f);
		attacker.ChangeAnimationTo("idle");
		// atkAnim.Stop();
		// attacker.GetComponent<Animator>().playbackTime;


		// Defender counterattack!
		defender.ChangeAnimationTo(defAnimName);
		if(Random.Range(0f, 100f) <= defHitRate){

			// atkAnim.Play("attackLeft", -1, 0f);
			attacker.curHealth -= (int) defDmg;
			if(attacker.curHealth <= 0){
				Kill(board, attacker);
				defender.ChangeAnimationTo("idle");
				yield return null;
			}
		}

		// yield return new WaitForSeconds(1f);
		defender.ChangeAnimationTo("idle");
		// defAnim.Stop();
		// defAnim.playbackTime = 0f;
		// attacker.GetComponent<Animator>().playbackTime;

		// Atacker attack double!
		attacker.ChangeAnimationTo(atkAnimName);
		if(attacker.doubleHit && Random.Range(0f, 100f) <= atkHitRate){

			// atkAnim.Play("attackLeft", -1, 0f);
			defender.curHealth -= (int) atkDmg;
			if(defender.curHealth <= 0)
            {
                print("oi caralho2");
                Kill(board, defender);
				attacker.ChangeAnimationTo("idle");
				yield return null;
			}
		}

		// yield return new WaitForSeconds(1f);
		attacker.ChangeAnimationTo("idle");
		// atkAnim.Stop();
		// atkAnim.playbackTime = 0f;
		// attacker.GetComponent<Animator>().playbackTime;

		// Defender attack double!
		defender.ChangeAnimationTo(defAnimName);
		if(defender.doubleHit && Random.Range(0f, 100f) <= defHitRate){

			// atkAnim.Play("attackLeft", -1, 0f);
			attacker.curHealth -= (int) defDmg;
			if(attacker.curHealth <= 0){
				Kill(board, attacker);
				defender.ChangeAnimationTo("idle");
				yield return null;
			}
		}

		// yield return new WaitForSeconds(1f);
		defender.ChangeAnimationTo("idle");
		// defAnim.Stop();
		// defAnim.playbackTime = 0f;
		// attacker.GetComponent<Animator>().playbackTime;


		// Restart idle animations
		atkAnim.Play("idle", -1, 0f);
		defAnim.Play("idle", -1, 0f);


		// Print statistics
		// BUG: it appears that the bonuses are not being applied
		// maybe some reference type problem? passing parameter to a function
		// creates a local variable?
		// print("atkDmg: " + atkDmg);
		// print("atkHitRate: " + atkHitRate);
		// print("atkCritRate: " + atkCritRate);

		// print("defDmg: " + defDmg);
		// print("defHitRate: " + defHitRate);
		// print("defCritRate: " + defCritRate);
	}

    public static int DamageAgainst(Unit attacker, Unit defender, BoardManager board) {
        float atkDmg;

        DoubleHit(attacker, defender);
        CalculateEffectiveness(attacker, defender);
        CalculateTriangleBonus(attacker, defender);

        atkDmg = Damage(attacker, defender, board);
        
        return (int)atkDmg;
    }

	private static float AtkSpeed(Unit u){

		if((u.equipedItem >= 0) && (u.inventory[u.equipedItem] as Equipment).weight <= u.stats.con)
			return u.stats.spd;
		else if((u.equipedItem >= 0) && (u.inventory[u.equipedItem] as Equipment).weight > u.stats.con)
			return ((u.inventory[u.equipedItem] as Equipment).weight - u.stats.con);
		else return 0f;
	}

	
	private static float HitRate(Unit u){
		if(u.equipedItem < 0) return 0;
		return (u.inventory[u.equipedItem] as Equipment).hit + u.stats.skill*2 + u.stats.luck/2;
	}

	private static float Evade(Unit u, BoardManager board){
		return AtkSpeed(u)*2 + u.stats.luck + board.types[board.board[u.posX, u.posY]].avoid;
	}

	public static float Accuracy(Unit attacker, Unit defender, BoardManager board){
		return HitRate(attacker) - Evade(defender, board) + attacker.hitBonus;
	}

	private static float AttackPower(Unit u, bool isMagical){
		if(u.equipedItem < 0) return 0;
		if(isMagical)
			return u.stats.mag + ((u.inventory[u.equipedItem] as Equipment).might + u.mtBonus)*u.effectiveness;
		else
			return u.stats.str + ((u.inventory[u.equipedItem] as Equipment).might + u.mtBonus)*u.effectiveness;
	}

	private static float DefensePower(Unit u, BoardManager board, bool isMagical){
		if(isMagical)
			return board.types[board.board[u.posX, u.posY]].defense + u.stats.res;
		else
			return board.types[board.board[u.posX, u.posY]].defense + u.stats.def;
	}
	
	public static float Damage(Unit attacker, Unit defender, BoardManager board){

		if(attacker.equipedItem < 0) return 0;
		
		bool isMagical;
		float dmg;
		Equipment atkE;
		// print("[DEBUG]: attacker: " + attacker);
		print("attacker.inventory["+attacker.equipedItem+"]: " + (attacker.inventory[attacker.equipedItem] as Equipment));
		atkE = (attacker.inventory[attacker.equipedItem] as Equipment);
		// print("equiptype: " + atkE.equipType);

		isMagical = atkE.equipType == "Anima" || 
					atkE.equipType == "Light" ||
					atkE.equipType == "Dark" || 
					atkE.Name == "Runesword" ||
					atkE.Name == "Lightbrand";

		dmg = AttackPower(attacker, isMagical) - 
			DefensePower(defender, board, isMagical);

		return ( (dmg < 0) ? 0 : dmg);
	}

	private static float CriticalRate(Unit u){

		if(u.equipedItem < 0) return 0;

		int bonus = 0;
		Equipment e = (u.inventory[u.equipedItem] as Equipment);

		if(u.cls.Name == "Swordmaster" || u.cls.Name == "Berserker")
			bonus += 15;

		return e.crit + u.stats.skill/2 + bonus;
	}

	private static float CriticalEvade(Unit u){
		return u.stats.luck;
	}

	public static float CriticalChance(Unit attacker, Unit defender){
		if(attacker.equipedItem < 0) return 0;
		float chance = CriticalRate(attacker) - CriticalEvade(defender);
		return ( (chance < 0) ? 0 : chance);
	}
	
	public static void DoubleHit(Unit u1, Unit u2){

		// Reset bonuses
		u1.doubleHit = false;
		u2.doubleHit = false;

		if(u1.stats.spd >= u2.stats.spd+4)
			u1.doubleHit = true;
		else if(u1.stats.spd+4 <= u2.stats.spd)
			u2.doubleHit = true;
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

				u1.hitBonus = -15;
				u1.mtBonus = -1;

			} else if(e2.equipType == "Axe"){
				
				u1.hitBonus = 15;
				u1.mtBonus = 1;

				u2.hitBonus = -15;
				u2.mtBonus = -1;
			}
		} else if(e1.equipType == "Lance"){
			if(e2.equipType == "Axe"){
				
				
				u2.hitBonus = 15;
				u2.mtBonus = 1;

				u1.hitBonus = -15;
				u1.mtBonus = -1;

			} else if(e2.equipType == "Sword"){
				
				u1.hitBonus = 15;
				u1.mtBonus = 1;

				u2.hitBonus = -15;
				u2.mtBonus = -1;
			}
		} else if(e1.equipType == "Axe"){
			if(e2.equipType == "Sword"){
				
				
				u2.hitBonus = 15;
				u2.mtBonus = 1;

				u1.hitBonus = -15;
				u1.mtBonus = -1;

			} else if(e2.equipType == "Lance"){
				
				u1.hitBonus = 15;
				u1.mtBonus = 1;

				u2.hitBonus = -15;
				u2.mtBonus = -1;
			}
		} else if(e1.equipType == "Anima"){
			if(e2.equipType == "Dark"){
				
				
				u2.hitBonus = 15;
				u2.mtBonus = 1;

				u1.hitBonus = -15;
				u1.mtBonus = -1;

			} else if(e2.equipType == "Light"){
				
				u1.hitBonus = 15;
				u1.mtBonus = 1;

				u2.hitBonus = -15;
				u2.mtBonus = -1;
			}
		} else if(e1.equipType == "Dark"){
			if(e2.equipType == "Light"){
				
				
				u2.hitBonus = 15;
				u2.mtBonus = 1;

				u1.hitBonus = -15;
				u1.mtBonus = -1;

			} else if(e2.equipType == "Anima"){
				
				u1.hitBonus = 15;
				u1.mtBonus = 1;

				u2.hitBonus = -15;
				u2.mtBonus = -1;
			}
		} else if(e1.equipType == "Light"){
			if(e2.equipType == "Anima"){
				
				
				u2.hitBonus = 15;
				u2.mtBonus = 1;

				u1.hitBonus = -15;
				u1.mtBonus = -1;

			} else if(e2.equipType == "Dark"){
				
				u1.hitBonus = 15;
				u1.mtBonus = 1;

				u2.hitBonus = -15;
				u2.mtBonus = -1;
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
