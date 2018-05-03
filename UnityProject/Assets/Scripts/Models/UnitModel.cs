using UnityEngine;
using System.Collections;
using Umbra;
using Umbra.Data;
using System.Collections.Generic;
using System.Linq;
using System;


namespace Umbra.Models
{
    public class UnitModel : Model<Dictionary<string, List<List<Unit>>>>
    {

		public Dictionary<string, Unit> source;
		public int numFactions;

        public UnitModel()
        {
            data = GameStateManager.Instance.gameState.units;
			source = GameStateManager.Instance.gameState.unitDictionary;
			numFactions = GameStateManager.Instance.gameState.factions.Count;
        }

		/*
		 * Get coordinates of this object
		 */
		public WorldCoordinates getLocation(Unit u) {
			if (u != null) return u.worldloc;
			return new WorldCoordinates();
		}

		/*
		 * Set coordinates of this object
		 */
		public void setLocation(Unit u, int x, int y, int d=0, string m="") {
			if (u != null && x > -1 && y > -1) u.worldloc.setValues (x, y, d, m);
		}

		/*
		 * Set coordinates of this object
		 */
		public void setLocation(Unit u, WorldCoordinates wl) {
			if (u != null && wl != null) u.worldloc.setValues (wl);
		}

		/*
		 * Creates a clone of the unit with specified id and faction.
		 * If no faction is specified, unit will be added to faction 0.
		 */
		public Unit createUnit(string id, int factionIdx=0) {
			Unit u = getSourceUnit(id, true);
			if (u != null) {
				// ensure faction index is within bounds; if not, add unit to nofaction
				int f = (factionIdx >= 0 && factionIdx < numFactions) ? factionIdx : 0;
				u.dbloc.setValues (u.id, f, data[id][f].Count);
				data[id][f].Add (u);
			}
			return u;
        }

		/*
		 * Get unit at specified database coordinates or null if not found
		 */
		public Unit getUnit(DBCoordinates coords) {
			try {
				return data[coords.id][coords.faction][coords.index];
			} catch {
				return null;
			}

		}

		/*
		 * Get list of units at database coordinates from the specified list
		 */
		public List<Unit> getUnits(List<DBCoordinates> coords, bool includeNulls=false) {
			List<Unit> result = new List<Unit> ();
			foreach (DBCoordinates udbc in coords) {
				Unit u = getUnit (udbc);
				if ((u == null && includeNulls) || (u != null)) result.Add (u);
			}
			return result;
		}

		/*
		 * Return source Unit (from the XML) with specified id.
		 * If clone is true, this returns a clone of the Unit, rather a direct reference.
		 */
		public Unit getSourceUnit(string id, bool clone=false) {
			if (source.ContainsKey(id)) return (clone == false) ? source[id] : source[id].clone();
			return null;
		}

		/*
		 * Return a list of all units with specified ID. Additionally, you can specify which faction as well.
		 */
		public List<Unit> getAllUnits(string id, int factionIdx=-1) {

			List<Unit> result = new List<Unit> ();
			int m = -1;
			int n = -1;

			if (factionIdx == -1) {
				m = 0;
				n = numFactions;
			} else if (factionIdx >= 0 && factionIdx < numFactions) {
				m = factionIdx;
				n = m + 1;
			}

			if (data.ContainsKey (id) && m > -1 && n > -1) {
				for (int f=m; f < n; f++) {
					foreach (Unit u in data[id][f]) {
						result.Add (u);
					}
				}
			}

			return result;

		}

		/*
		 * Return a list of all units assigned to faction factionIdx. 
		 * If factionIdx is -1, get ALL units in the model.
		 */
		public List<Unit> getAllUnits(int factionIdx=-1) {

			List<Unit> result = new List<Unit>();
			int m = -1;
			int n = -1;

			if (factionIdx == -1) {
				m = 0;
				n = numFactions;
			} else if (factionIdx >= 0 && factionIdx < numFactions) {
				m = factionIdx;
				n = m + 1;
			}

			if (m > -1 && n > -1) {
				// This can probably be optimized a bit better.
				foreach (string key in data.Keys) {
					for (int i = m; i < n; i++) {
						foreach (Unit u in data[key][i]) {
							result.Add (u);
						}
					}
				}
			}

			return result;
		}

		/*
		 * Set database coordinates of unit u
		 */
		public void setDBCoordinates(Unit u, DBCoordinates coords) {
			if (u != null) {
				u.dbloc = coords.clone ();
			}
		}

		/*
		 * Assign coordinates of unit u's parent (ie character) to u
		 */
		public void setParent(Unit u, Character c) {
			if (u != null && c != null) {
				u.parent = c.dbloc.clone ();
			}
		}

		/*
		 * Returns the main 3 stats of unit u, while affected by multipliers
		 */
		public List<int> getUnitStats(Unit u) {
			List<int> result = new List<int> ();
			if (u != null) {
				Unit s = getSourceUnit (u.id);
				if (s != null) {
					result.Add (Convert.ToInt32(u.health * u.healthMult));
					result.Add (Convert.ToInt32(s.health * u.healthMult)); // multiply max hp too; this is not a typo!
					result.Add (Convert.ToInt32(u.armor * u.armorMult));
					result.Add (Convert.ToInt32(s.armor * u.armorMult)); // multiply max ap too; this is not a typo!
					result.Add (Convert.ToInt32(u.shield * u.shieldMult));
					result.Add (Convert.ToInt32(s.shield * u.shieldMult)); // multiply max sp too; this is not a typo!
				}
			}
			return result;
		}

    }
}