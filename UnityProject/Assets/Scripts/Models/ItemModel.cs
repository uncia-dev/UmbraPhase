using UnityEngine;
using System.Collections;
using Umbra;
using Umbra.Data;
using System.Collections.Generic;
using System.Linq;


namespace Umbra.Models
{
	public class ItemModel : Model<Dictionary<string, List<List<Item>>>>
    {

		public Dictionary<string, Item> source;
		public int numFactions;

        public ItemModel()
        {
			data = GameStateManager.Instance.gameState.items;
			source = GameStateManager.Instance.gameState.itemDictionary;
			numFactions = GameStateManager.Instance.gameState.factions.Count;
        }

		/*
		 * Get coordinates of this object
		 */
		public WorldCoordinates getLocation(Item i) {
			if (i != null) return i.worldloc;
			return new WorldCoordinates();
		}

		/*
		 * Set coordinates of this object
		 */
		public void setLocation(Item i, int x, int y, int d=0, string m="") {
			if (i != null && x > -1 && y > -1) i.worldloc.setValues (x, y, d, m);
		}

		/*
		 * Set coordinates of this object
		 */
		public void setLocation(Item i, WorldCoordinates wl) {
			if (i != null && wl != null) i.worldloc.setValues (wl);
		}

		/*
		 * Creates a clone of the item with specified it and faction.
		 * If no faction is specified, item will be added to faction 0.
		 */
		public Item createItem(string id, int factionIdx=0) {
			Item i = getSourceItem (id, true);
			if (i != null) {
				// ensure faction index is within bounds; if not, add item to nofaction
				int f = (factionIdx >= 0 && factionIdx < numFactions) ? factionIdx : 0;
				i.dbloc.setValues (i.id, f, data [id] [f].Count);
				data[id][f].Add (i);
			}
			return i;
		}

		/*
		 * Get item at specified database coordinates or null if not found
		 */
		public Item getItem(DBCoordinates coords) {
			try {
				return data[coords.id][coords.faction][coords.index];
			} catch {
				return null;
			}
		}

		/*
		 * Get list of units at database coordinates from the specified list
		 */
		public List<Item> getItems(List<DBCoordinates> coords, bool includeNulls=false) {
			List<Item> result = new List<Item> ();
			foreach (DBCoordinates idbc in coords) {
				Item i = getItem (idbc);
				if ((i == null && includeNulls) || (i != null)) result.Add (i);
			}
			return result;
		}

		/*
		 * Return source Item (from the XML) with specified id.
		 * If clone is true, this returns a clone of the Item, rather than a direct reference
		 */
		public Item getSourceItem(string id, bool clone=false) {
			if (source.ContainsKey (id)) return (clone == false) ? source [id] : source [id].clone ();
			return null;
		}

    }
}