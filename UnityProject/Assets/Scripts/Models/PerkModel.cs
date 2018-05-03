using UnityEngine;
using System.Collections;
using Umbra;
using Umbra.Data;
using System.Collections.Generic;
using System.Linq;


namespace Umbra.Models
{
	public class PerkModel : Model<Dictionary<string, Perk>>
    {

        public PerkModel()
        {
			data = GameStateManager.Instance.gameState.perkDictionary;
        }

		/*
		 * Return perk object with specified id, or a blank perk if not found
		 */
		public Perk getPerkById(string id) {
			if (data.ContainsKey(id)) return data[id];
			return data ["noperk"];
		}

    }
}