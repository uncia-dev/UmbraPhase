using UnityEngine;
using System.Collections;
using Umbra;
using Umbra.Data;
using System.Collections.Generic;
using System.Linq;


namespace Umbra.Models
{
    public class OutpostModel : Model<Dictionary<string, Outpost>>
    {
        public OutpostModel()
        {
            data = GameStateManager.Instance.gameState.outpostDictionary;
        }

        public Outpost createOutpost(string id)
        {
            Outpost o = new Outpost();
            o.id = id;
            o.name = "Outpost Umbra";
            GameStateManager.Instance.gameState.outpostDictionary.Add(id, o);
            GameStateManager.Instance.SaveGame();
            return o;
        }

        public Outpost getCurrentOutpost()
        {
            return GameStateManager.Instance.gameState.currentOutpost;
        }

        public bool doesOutpostExist(string mapID)
        {
            return data.ContainsKey(mapID);
        }

        public Outpost getOutpostByID(string id)
        {
            return data[id];
        }
    }
}