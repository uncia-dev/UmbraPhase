using UnityEngine;
using System.Collections;
using Umbra;
using Umbra.Data;
using System.Collections.Generic;
using System.Linq;


namespace Umbra.Models
{
	public class BuffModel : Model<Dictionary<string, Buff>>
    {

        public BuffModel()
        {
			data = GameStateManager.Instance.gameState.buffDictionary;
        }

		/*
		 * Return buff with specified ID, or null if none is found
		 */
		public Buff getBuffById(string id) {
			if (data.ContainsKey(id)) return data[id];
			return null;
		}

		/*
		 * Apply effects of buff with specified buffID on unit u
		 */
		public void applyBuffEffects(Unit u, string buffID) {

			Buff b = getBuffById (buffID);

			// alter booleans in u
			u.isMovable = b.isMovable;
			u.isInvincible = b.isInvincible;
			u.isVisible = b.isVisible;
			u.isDisabled = b.isDisabled;

			// alter multipliers in u
			u.movementRange += b.movementRange;
			u.movementRangeMult += b.movementRangeMult;
			u.movementSpeed += b.movementSpeed;
			u.movementSpeedMult += b.movementSpeedMult;
			//u.health += b.health;
			//u.healthMult += b.healthMult;
			//u.armor += b.armor;
			//u.armorMult += b.armorMult;
			//u.shield += b.shield;
			//u.shieldMult += b.shieldMult;
			u.damageBonus += b.damageBonus;
			u.damageMult += b.damageMult;
			u.healBonus += b.healBonus;
			u.healMult += b.healMult;

		}

		/*
		 * Remove buff multiplier effects from unit u, with source unit src
		 */
		public void resetBuffEffects(Unit u, Unit src, string buffID) {

			// src overrides the need to call the unit model

			Buff b = getBuffById (buffID);

			// alter booleans in u
			if (b.isMovable != src.isMovable) u.isMovable = src.isMovable;
			if (b.isInvincible != src.isInvincible) u.isInvincible = src.isInvincible;
			if (b.isVisible != src.isVisible) u.isVisible = src.isVisible;
			if (b.isDisabled != src.isDisabled) u.isDisabled = src.isDisabled;

			// alter multipliers in u
			u.movementRange -= b.movementRange;
			u.movementRangeMult -= b.movementRangeMult;
			u.movementSpeed -= b.movementSpeed;
			u.movementSpeedMult -= b.movementSpeedMult;
			//u.healthMult -= b.healthMult;
			//u.armorMult -= b.armorMult;
			//u.shieldMult -= b.shieldMult;
			u.damageBonus -= b.damageBonus;
			u.damageMult -= b.damageMult;
			u.healBonus -= b.healBonus;
			u.healMult -= b.healMult;

			/*
			if (u.health > 0) u.health -= b.health;
			u.health = Mathf.Max (0, u.health);

			if (u.armor > 0) u.armor -= b.armor;
			u.armor = Mathf.Max (0, u.armor);

			if (u.shield > 0) u.shield -= b.shield;
			u.shield = Mathf.Max (0, u.shield);
			*/

		}

		/*
		 * Return average of of all colors applied from each buff from the list buffIDs
		 */
		public Color getResultantBuffColor(List<string> buffIDs) {

			Color result = new Color (1, 1, 1, 1);

			if (buffIDs.Count > 0) {

				float[] colorSums = new float[] { 0.0f, 0.0f, 0.0f, 0.0f };

				foreach (string buffID in buffIDs) {
					for (int i = 0; i < 4; i++) {
						colorSums [i] += data [buffID].color [i];
						colorSums [i] += 0.1f;
					}
				}

				result.r = colorSums [0] / buffIDs.Count;
				result.g = colorSums [1] / buffIDs.Count;
				result.b = colorSums [2] / buffIDs.Count;
				result.a = colorSums [3] / buffIDs.Count;

			}

			return result;

		}

    }
}