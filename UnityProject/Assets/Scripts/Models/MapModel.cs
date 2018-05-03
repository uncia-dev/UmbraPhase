using UnityEngine;
using System.Collections;
using Umbra;
using Umbra.Data;
using System.Collections.Generic;
using System.Linq;


namespace Umbra.Models
{
	public class MapModel : Model<Dictionary<string, Map>>
    {
        public MapModel()
        {
			data = GameStateManager.Instance.gameState.mapDictionary;
        }

		/* We don't really want to create maps if none are found in the database
        public Map createMap(string mapid)
        {
            Map m = new Map();
            data.Add(m);
            //    m.id = data.Count.ToString(); //get rid of this after testing
            m.id = mapid;
            m.filename = "Map" + mapid; 
            GameStateManager.Instance.SaveGame();
            return m;
        }
		*/

        public Map getCurrentMap()
        {
            return GameStateManager.Instance.gameState.currentMap;
        }

		public Map getMapByID(string mapid) {
			if (data.ContainsKey(mapid)) return data [mapid];
			return null;
		}

        public void setCurrentMapByMap(Map map)
        {
			if (map != null) GameStateManager.Instance.gameState.currentMap = map;
        }

		public void setCurrentMapByID(string mapid)
		{
			if (data.ContainsKey(mapid)) GameStateManager.Instance.gameState.currentMap = data [mapid];
		}

		public void setBattleMap(Map map, int index) {
			if (index >= 0 && index < map.battleMapIDs.Count) {
				setCurrentMapByID (map.battleMapIDs [index]);
			}
		}

    }
}