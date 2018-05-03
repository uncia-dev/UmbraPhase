using UnityEngine;
using System.Collections;
using Umbra;
using Umbra.Data;
using System.Collections.Generic;
using System.Linq;


namespace Umbra.Models
{
	public class RankModel : Model<Dictionary<string, Rank>>
    {

        public RankModel()
        {
			data = GameStateManager.Instance.gameState.rankDictionary;
        }

		public Rank getRankById(string id) {
			if (data.ContainsKey(id)) return data[id];
			return data ["norank"];
		}

    }
}