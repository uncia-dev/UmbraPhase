using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using Umbra;
using Umbra.Data;
using Umbra.Managers;

namespace Umbra.Models
{
	public class AbilityModel : Model<Dictionary<string, Ability>>
    {

		private Abilities _hardcodedAbilities;

        public AbilityModel()
        {
			data = GameStateManager.Instance.gameState.abilityDictionary;
			_hardcodedAbilities = new Abilities ();
        }

		public Ability getAbilityById(string id) {
			if (data.ContainsKey(id)) return data[id];
			return null;
		}

		/*
		 * Return a list of damage values for ability with specified ID.
		 * Used for damage calculations.
		 */
		public List<int> getAbilityDamageArray(string id) {

			List<int> result = new List<int> ();

			if (data.ContainsKey (id)) {
				result.Add (data [id].damageHealth);
				result.Add (data [id].damageArmor);
				result.Add (data [id].damageShield);
				result.Add (data [id].damageAcid);
				result.Add (data [id].damageElectric);
				result.Add (data [id].damageFire);
				result.Add (data [id].damageFrost);
				result.Add (data [id].damageRadiation);
			} else {
				for (int i = 0; i < 9; i++)
					result.Add (0);
			}

			return result;

		}

		/*
		 * Return state of ability a, depending on whether or not there is a hostile relationship
		 * between selected and targeted actor.
		 */
		public bool getAbilityState(
			Ability a, bool deadCheck, bool lockedCheck, int cooldown, bool layerCheck, double range, bool cloakCheck, 
			bool selfCheck, bool hostileCheck
		) {
			bool result = false;

			/*
			Debug.Log (a.name);
			Debug.Log(
				deadCheck + " " + lockedCheck + " " + cooldown + " " + layerCheck + " " + range + " " + cloakCheck +
				" " + selfCheck + " " + hostileCheck
			);
			*/	

			if (a != null) {
				// check if the target is dead or if the ability is castable on dead units
				if (!deadCheck || (deadCheck && a.isCastableOnDisabled)) {
					// check if ability is available
					if (!lockedCheck && cooldown == 0) {
						// check if the units are on the same layer
						if ((layerCheck && a.isZAxisLocked) || !a.isZAxisLocked) {
							// check if ability can reach the target
							if (range <= a.range || range == -1) {
								// check if unit is cloaked
								if (cloakCheck) {
									if (a.isSelfCast) {
										result = !hostileCheck && selfCheck;
									} else {
										result = (a.isHarmful && a.isHelpful) || // rare, for some CC abilities
										(hostileCheck && a.isHarmful) || // is target hostile and ability harmful?
										(!hostileCheck && a.isHelpful); // is target friendly and ability helpful?
									}
								}
							}
						}
					}
				}
			}

			//Debug.Log (result);

			return result;
		}

		/*
		 * Calculate amount of damage done to target's health, from ability a with specified calculated raw damage and 
		 * multipliers from bonuses/debuffs
		 */
		int calculateHealthDamage(Ability a, Unit target, int rawDamage, double damageMultipliers) {

			return Convert.ToInt32(
				(
					rawDamage +
					a.damageRadiation + // extra damage from radiation 
					a.damageAcid * 0.25 + // extra damage from acid
					a.damageFire * 0.25 // extra damage from fire
				) * damageMultipliers *
				((target.shield == 0 && target.armor == 0) ? (1.0 + a.noDefensesBonus) : 1.0) * // add bonus multiplier for no defenses
				(target.shield > 0 ? (a.ignoresShield) : 1) * // is there a shield? then mitigate the damage completely - the percentage from a.ignoresShield
				(target.armor > 0 ? (Math.Min(1.0, (Math.Max (0.0, (300.0 - target.armor * target.armorMult)) / 300.0) + a.ignoresArmor)) : 1) // apply armor mitigation; higher armor = more mitigation
			);

		}

		/*
		 * Calculate amount of damage done to target's armor, from ability a with specified calculated raw damage and 
		 * multipliers from bonuses/debuffs
		 */
		int calculateArmorDamage(Ability a, Unit target, int rawDamage, double damageMultipliers) {

			return Convert.ToInt32 (
				(
					rawDamage + 
					a.damageArmor +
					a.damageAcid * 0.5 + // extra damage from acid
					a.damageFire * 0.5 + // extra damage from fire
					a.damageFrost * 0.25 + // extra damage from frost
					a.damageRadiation * -1 + // radiation cannot exactly kill metal now, can it?
					a.damageElectric * -0.5 // electric damage reduction
				) * damageMultipliers *
				(target.shield > 0 ? (a.ignoresShield) : 1) // is there a shield? then mitigate the damage completely - the percentage from a.ignoresShield
			);

		}

		/*
		 * Calculate amount of damage done to target's shield, from ability a with specified calculated raw damage and 
		 * multipliers from bonuses/debuffs
		 */
		int calculateShieldDamage(Ability a, Unit target, int rawDamage, double damageMultipliers) {

			return Convert.ToInt32(
				(
					rawDamage + 
					a.damageShield + 
					a.damageElectric * 0.75 + // electric damage heavily affects shields
					a.damageFire * 0.5 // fire damage has a stronger effect on shields
				) * damageMultipliers
			);

		}

		/*
		 * Calculate and apply the damage/healing caused by caster using ability a on target, within the specified
		 * distance. Returns an array cotaining the damage dealt to the target's health, armor and shield.
		 */
		public List<int> applyAbility(
			Unit caster, Unit target, Ability a, double distance, 
			bool perkArmorer=false, bool perkMarksmanship=false, bool perkFirstAid=false,
			float morale=0, int casterSide=0, int targetSide=1
		) {

			List<int> result = new List<int> { 0, 0, 0 };

			if (!target.isInvincible) { // only bother causing damage if the target is not invincible

				if (a.isDamaging) { // only do the calculations below if the the ability causes damage

					UnitModel _unitModel = new UnitModel ();
					List<int> targetStats = _unitModel.getUnitStats (target);

					double classBonus = 1;
					double moraleResist = 0;
					double moraleDamage = 0;

					// Bonus damage based on unit classification
					if (target.unitClass == "Infantry")
						classBonus += a.infantryBonusDamage;
					if (target.unitClass == "GroundVehicle")
						classBonus += a.groundVehicleBonusDamage;
					if (target.unitClass == "FlyingVehicle")
						classBonus += a.flyingVehicleBonusDamage;

					// Calculate morale based damage bonus
					if ((morale < 0 && casterSide == 0) || (morale > 0 && casterSide == 1))
						moraleDamage = 0.25 * Mathf.Abs (morale);

					// Calculate morale based resist bonus
					if ((morale > 0 && casterSide == 0) || (morale < 0 && casterSide == 1))
						moraleResist = 0.25 * Mathf.Abs (morale);

					// Raw damage
					int rawDamage = 
						a.damageHealth +
						a.damageFire +
						a.damageFrost +
						a.damageElectric +
						a.damageAcid +
						a.damageRadiation;

					// Multipliers for various conditions; apply to each subformula
					double damageMultipliers =
						(1.0 + a.proximityBonus * (a.range - distance) / a.range) * // proximity bonus
						classBonus * // class bonus
						caster.damageMult * // caster's damage multiplier
						(perkMarksmanship ? 1.1 : 1.0) * // marksmanship perk bonus damage
						(perkArmorer ? 0.9 : 1.0) * // armor perk damage reduction
						(1.0 + moraleDamage) * // high morale damage bonus
						(1.0 - moraleResist); // low morale damage reduction

					// HEALTH DAMAGE
					if (a.damageHealth < 0) { // If damage is negative, assume this ability heals
						if (targetStats [0] < targetStats [1]) {
							result [0] = Convert.ToInt32 (a.damageHealth * caster.healMult * (perkFirstAid ? 1.1 : 1) * (1 + moraleDamage));
							target.health = Math.Min (target.health - result [0], targetStats [1]);
						}

					} else { // else, calculate damage done to the unit
						// Try to damage the unit's health
						// Armor and shields will protect a unit's health, unless the ability can pierce the defenses
						result [0] = calculateHealthDamage (a, target, rawDamage, damageMultipliers);
						target.health = Math.Max (0, target.health - result [0]);
						if (target.health == 0)	target.isDead = true; // kill the unit if it hits 0 hp
					}

					// ARMOR DAMAGE
					if (a.damageArmor < 0) { // if damage armor is negative, assume armor repair
						if (targetStats [2] < targetStats [3]) {
							result [1] = Convert.ToInt32 (a.damageArmor * caster.healMult * (perkFirstAid ? 1.1 : 1) * (1 + moraleDamage));
							target.armor = Math.Min(target.armor - result [1], targetStats[3]);
						}
					} else { // else, calculate damage done to the unit's armor, but only if there is armor left
						// Try to damage the unit's armor if shields are up, otherwise destroy the armor
						// Armor soaks up damage as the armor points increase, up to 400, after which the unit is nearly
						// invincible, as long as the armor holds
						if (target.armor > 0) {
							result [1] = calculateArmorDamage (a, target, rawDamage, damageMultipliers);
							target.armor = Math.Max(0, (target.armor - result [1]));
						}
					}

					// SHIELD DAMAGE
					if (a.damageShield < 0) { // if damage shield is negative, assume shield boost
						result [2] = Convert.ToInt32 (a.damageShield * caster.healMult * (1 + moraleDamage));
						target.shield += result [2] * -1; // no limits on shields; just keep adding more and more
					} else { // else, calculate damage done to unit's shield, but only if there is a shield left
						// Damage the unit's shield - no trying here, as long as a shield is up, it'll soak all damage
						// unless the ability bypasses it
						if (target.shield > 0) {
							result [2] = calculateShieldDamage (a, target, rawDamage, damageMultipliers);
							target.shield = Math.Max (0, (target.shield - result [2]));
						}
					}

				}

			}

			return result;

		}

		/*
		 * Call function assigned to functionID
		 */
		public Dictionary<string, string> callAbilityFunction(Actor caster, Actor target, Ability a, double distance=-1) {

			Dictionary<string, string> result = new Dictionary<string, string> ();

			if (a.functionName == "F5_ScavengeGV")
				result = _hardcodedAbilities.StealAbility (caster, target);
			else if (a.functionName == "Sabotage")
				result = _hardcodedAbilities.Sabotage (target);
			else if (a.functionName == "Resurrect")
				result = _hardcodedAbilities.Resurrect (target);
			else if (a.functionName == "Decloak")
				result = _hardcodedAbilities.Decloak (target);
			else if (a.functionName == "Selfdestruct")
				result = _hardcodedAbilities.SelfDestruct (caster);

			return result;

		}

    }
}