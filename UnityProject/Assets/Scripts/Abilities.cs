using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using Umbra.Data;
using Umbra.Models;
using Umbra.Managers;

namespace Umbra.Data
{

	// This class contains a list of hard-coded ability methods

	public class Abilities
	{

		/*
		 * Steal a random ability from targeted unit, or specified ability
		 */
		public Dictionary<string, string> StealAbility (Actor caster, Actor target, int idx=-1) {

			int ab = 0;

			if (idx > -1 && idx < 6) {
				ab = idx;
			} else {
				System.Random random = new System.Random ();
				if (target.unit.isPlayerCharacter) {
					ab = random.Next (0, 5);
				} else if (target.unit.isCharacter) {
					ab = random.Next (0, 4);
				} else {
					ab = random.Next (0, 3);
				}
			}

			caster.unit.abilities [3] = target.unit.abilities [ab];			

			return null;

		}

		/*
		 * Kill target unit if it is at 50% armor or less and 25% shield or less
		 */
		public Dictionary<string, string> Sabotage (Actor target) {
			Dictionary<string, string> result = new Dictionary<string, string> ();
			List<int> stats = new UnitModel ().getUnitStats (target.unit);
			if (
				((double)stats [2] / (double)stats [3]) <= 0.5 &&
				((double)stats [4] / (double)stats [5]) <= 0.25) {
				target.unit.isDead = false;
				target.unit.health = 0;
				target.unit.armor = 0;
				target.unit.shield = 0;
			} else {
				result ["Message"] = "Sabotage failed. Cooldown reset.";
				result ["CooldownReset"] = "caster";
			}
			return result;
		}

		/*
		 * Bring back target unit to life
		 */
		public Dictionary<string, string> Resurrect (Actor target) {
			target.unit.isDead = false;
			target.unit.health = 1;
			target.unit.armor = 0;
			target.unit.shield = 0;
			return null;
		}

		/*
		 * Decloak enemy unit, if it has a cloak applied
		 */
		public Dictionary<string, string> Decloak(Actor target) {
			Dictionary<string, string> result = new Dictionary<string, string> ();
			result ["RemoveBuff"] = "Cloak";
			if (!target.unit.isVisible)
				result ["Message"] = target.unit.name + " was decloaked.";
			target.unit.isVisible = true;
			target.sprite.GetComponent<SpriteRenderer> ().color = new Color (1.0f, 1.0f, 1.0f, 1.0f);
			return result;
		}

		/*
		 * Self destruct unit that called this ability
		 */
		public Dictionary<string, string> SelfDestruct(Actor caster) {
			caster.unit.isDead = true;
			caster.unit.health = 0;
			caster.unit.armor = 0;
			caster.unit.shield = 0;
			return null;
		}

	}
}

