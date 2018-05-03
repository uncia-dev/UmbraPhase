using UnityEngine;
using System.Collections;
using Umbra;
using Umbra.Data;
using System.Collections.Generic;
using System.Linq;


namespace Umbra.Models
{
	public class FactionModel : Model<List<Faction>>
    {

		public Dictionary<string, Faction> src;
		public int numFactions;

        public FactionModel()
        {
			data = GameStateManager.Instance.gameState.factions;
			src = GameStateManager.Instance.gameState.factionDictionary;
			numFactions = data.Count;
        }

		/*
		 * Return faction at index idx
		 */
		public Faction getFaction(int idx) {
			if (idx >= 0 && idx < numFactions) return data [idx];
			return null;

		}

		/*
		 * Return index of faction with specified id
		 */
		public int getFactionIndex(string id) {
			int result = 0;
			if (src.ContainsKey (id)) return src [id].factionIdx;
			return result;
		}

		/*
		 * Return color object of this faction
		 */
		public Color getFactionColor(int idx) {

			Color result = new Color (0.25f, 0.25f, 0.25f, 1.0f);

			if (getFaction (idx) != null) {
				result.r = data [idx].color [0];
				result.g = data [idx].color [1];
				result.b = data [idx].color [2];
				result.a = 1f;
			}

			return result;

		}

		/*
		 * Return name of faction's currency
		 */
		public string getFactionCurrency(int idx) {
			if (getFaction (idx) != null) {
				return data [idx].currency;
			}
			return "";
		}

		/*
		 * Modify faction reputation between Factions with indices f1 and f2
		 */
		public void modifyFactionReputation(int f1, int f2, int amt) {
			if (f1 >= 1 && f1 <= 6 && f2 >= 1 && f2 <= 6) {
				data [f1].reputations [f2] = Mathf.Min (Mathf.Max (0, data [f1].reputations [f2] + amt), 100);
			}
		}

	}
}