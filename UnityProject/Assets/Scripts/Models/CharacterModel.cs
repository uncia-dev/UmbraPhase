using UnityEngine;
using System.Collections;
using Umbra;
using Umbra.Data;
using System.Collections.Generic;
using System.Linq;

namespace Umbra.Models
{
    public class CharacterModel : Model<Dictionary<string, List<List<Character>>>>
    {
        
		public Dictionary<string, Character> source;
		public int numFactions;
		public DBCoordinates[] battlingCharacters;

		public CharacterModel()
        {
            data = GameStateManager.Instance.gameState.characters;
			source = GameStateManager.Instance.gameState.characterDictionary;
			numFactions = GameStateManager.Instance.gameState.factions.Count;
			battlingCharacters = GameStateManager.Instance.gameState.battlingCharacters;
        }

		/*
		 * Get coordinates of this object
		 */
		public WorldCoordinates getLocation(Character c) {
			if (c != null) return c.worldloc;
			return new WorldCoordinates();
		}

		/*
		 * Set coordinates of this object
		 */
		public void setLocation(Character c, int x, int y, int d=0, string m="") {
			if (c != null && x > -1 && y > -1) c.worldloc.setValues (x, y, d, m);
		}

		/*
		 * Set coordinates of this object
		 */
		public void setLocation(Character c, WorldCoordinates wl) {
			if (c != null && wl != null) c.worldloc.setValues (wl);
		}

		/*
		 * Creates a clone of the character with specified id and faction.
		 * If no faction is specified, the character will be added to faction 0.
		 * Note that this method is really only applied to generic characters.
		 * Unique character creation will be ignored.
		 */
		public Character createCharacter(string id, int factionIdx=0)
		{
			Character c = getSourceCharacter (id); // at first just a reference
			if (c != null) {

				if (!data.ContainsKey (id)) {
					data [id] = new List<List<Character>> ();
					for (int i = 0; i < numFactions; i++) data [id].Add(new List<Character> ());
				}

				// omit creating a copy of a unique character
				// during the init() of GameState, unique characters area already created
				if (!c.isUnique) {
					// ensure faction index is within bounds; if not, add character to nofaction
					int f = (factionIdx >= 0 && factionIdx < numFactions) ? factionIdx : 0;
					// create clone of the source character, give it a db location and add it to the model
					c = getSourceCharacter (id, true);
					c.dbloc.setValues (c.id, f, numFactions);
					data [id] [f].Add (c);
					// Initialize this character (see method below)
					initializeCharacter (c);
				} else {
					c = null;
				}

			}

			return c;
		}

		/*
		 * Get character at specified database coordinates or null if not found
		 */
		public Character getCharacter(DBCoordinates coords)
        {
			try {
				return data[coords.id][coords.faction][coords.index];
			} catch {
				Debug.Log (
					"Unit at coordinates (" + coords.id + ", " + coords.faction + ", " + coords.index + ") not found."
				);
			}
			return null;
        }

		/*
		 * Get character with specified ID
		 */
		public Character getCharacterByID(string id, int faction=-1, int index=-1) {

			if (data.ContainsKey(id)) {

				int f = (faction >= 0 && faction <= 6) ? faction : getSourceCharacter (id).factionIdx;
				int i = (index >= 0 && index < data [id][f].Count) ? index : 0;

				try {
					return data [id] [f] [i];
				} catch {
					return null;
				}

			}

			return null;

		}

		/*
		 * Return list of the two battling characters (for battlegroups)
		 */
		public List<Character> getBattlingCharacters() {
			List<Character> result = new List<Character> ();
			if (!battlingCharacters [0].isEmpty ()) result.Add (getCharacter (battlingCharacters [0]));
			if (!battlingCharacters [1].isEmpty ()) result.Add (getCharacter (battlingCharacters [1]));
			if (result.Count == 2) return result;
			return null;
		}

		/*
		 * Set the two battling characters that will face off each other in a battle. Return true if the characters are
		 * not of the same faction and the characters are valid.
		 */
		public bool setBattlingCharacters(Character c1, Character c2) {

			bool result = false;

			if (c1 != null && c2 != null) {

				// Ensure c1 and c2 are not of the same faction
				if (c1.dbloc.faction != c2.dbloc.faction) {
					battlingCharacters [0] = c1.dbloc.clone();
					battlingCharacters [1] = c2.dbloc.clone ();
					return true;
				}
			}

			return result;

		}

		/*
		 * Clear list of battling characters
		 */
		public void clearBattlingCharacters() {
			battlingCharacters [0].clear ();
			battlingCharacters [1].clear ();
		}

		/*
		 * Return source Character (from the XML) with the specified id.
		 * If clone is set to true, this returns a clone of the Character, rather than a direct reference
		 */
		public Character getSourceCharacter(string id, bool clone=false) {
			if (source.ContainsKey (id)) return (clone == false) ? source [id] : source[id].clone();
			return null;
		}

		/*
		 * Return a list of all characters with specified ID. Additionally, you can specify which faction as well.
		 */
		public List<Character> getAllCharacters(string id, int factionIdx=-1) {
			
			List<Character> result = new List<Character>();
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
					foreach (Character c in data[id][f]) {
						result.Add (c);
					}
				}
			}

			return result;

		}

		/*
		 * Return a list of all characters assigned to faction factionIdx. 
		 * If factionIdx is -1, get ALL characters in the model.
		 */
		public List<Character> getAllCharacters(int factionIdx=-1) {

			List<Character> result = new List<Character>();
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
				// This can probably be optimized a bit better. Too many loops!
				foreach (string key in data.Keys) {
					for (int i = m; i < n; i++) {
						foreach (Character c in data[key][i]) {
							result.Add (c);
						}
					}
				}
			}

			return result;

		}

		/*
		 * Returns the player character
		 */
        public Character getPlayerCharacter()
        {
			return data ["PlayerCharacter"] [1] [0];
        }

		/*
		 * Set database coordinates of character c
		 */
		public void setDBCoordinates(Character c, DBCoordinates coords) {
			if (c != null && coords != null) {
				c.dbloc = coords.clone ();
			}
		}

		/*
		 * Return currently selected unit for character c
		 */
		public Unit getCharacterSelectedUnit(Character c) {
			return (new UnitModel().getUnit (c.units [c.unitCurrentIdx]));
		}

		/*
		 * Set character c's active unit to index idx. 
		 */
		public void setCharacterSelectedUnit(Character c, int idx) {
			if (c != null && idx >= 0 && idx < 3) {
				if (new UnitModel().getUnit(c.units[idx]) != null) c.unitCurrentIdx = idx;
			}
		}

		// BATTLEGROUP CONTROLS

		/*
		 * Get unit DBCoordinates at index
		 */
		public DBCoordinates getBattlegroupUnit (Character c, int index) {
			DBCoordinates result = new DBCoordinates ();
			if (index > -1 && index < 8) return c.battlegroup [index];
			return result;
		}

		/*
		 * Get Character's battlegroup DNCoordinates as a list.
		 */
		public List<DBCoordinates> getBattlegroup (Character c, bool includeEmpty=false) {
			List<DBCoordinates> result = new List<DBCoordinates> ();
			foreach (DBCoordinates udbc in c.battlegroup) {
				if ((udbc.isEmpty() && includeEmpty) == true || (!udbc.isEmpty())) result.Add (udbc);
			}
			return result;
		}

		/*
		 * Get Character c's battlegroup size
		 */
		public int battlegroupGetSize(Character c) {
			int s = 0;
			if (c != null) {
				foreach (DBCoordinates u in c.battlegroup) {
					if (!u.isEmpty ())	s++;
				}
			}
			return s;
		}

		/*
		 * Get Character c's maximum allowed battlegroup size
		 */
		public int battlegroupGetMaxSize(Character c) {
			int result = 1;
			if (c != null) {
				Dictionary<string, Rank> ranks = GameStateManager.Instance.gameState.rankDictionary;
				if (ranks.ContainsKey (c.rankID)) result = ranks [c.rankID].unitCap;
			}
			// Ensure unit cap is within the range [1,8]
			return (result > 0 && result < 9) ? result : 1;
		}

		/*
		 * Add unit u to character c's battlegroup, while also removing u from its current battlegroup
		 */
		public bool battlegroupAddUnit(Character c, Unit u, int slot=-1)
		{

			bool result = false;

			if (c != null && u != null) {

				// Make sure u's parent is not already c
				if (!u.parent.Equals (c.dbloc)) {

					// only add u to c's battlegroup if there is room left, according to the rank's unit cap
					if (battlegroupGetSize (c) < battlegroupGetMaxSize (c)) {

						// check if the unit has a parent assigned (ie a battlegroup) and remove the unit from the
						// parent's	battlegroup
						if (!u.parent.isEmpty ()) battlegroupRemoveUnit(getCharacter (u.parent), u); 

						u.parent = c.dbloc.clone ();

						int s;
						if (slot >= 0 && slot < battlegroupGetMaxSize (c)) s = slot;
						else {
							// look for the first empty slot in c's battlegroup
							for (s = 0; s < battlegroupGetMaxSize (c); s++) {
								if (c.battlegroup [s].isEmpty ()) break;
							}
						}

						if (s < battlegroupGetMaxSize (c) && c.battlegroup[s].isEmpty()) {
							c.battlegroup [s] = u.dbloc.clone ();
							result = true; // if we got here, then the method was successful
						}

					}

				}

			}

			return result;

		}

		/*
		 * Remove unit u from character's battlegroup
		 */
		public bool battlegroupRemoveUnit(Character c, Unit u)
		{

			bool result = false;

			if (c != null && u != null) {
				// Clear u's parent if it equals to c's dbloc
				if (u.parent.Equals(c.dbloc)) u.parent.clear();
				// Ensure c does not have u in its battlegroup - whether or not the above statement yielded false
				// linear search for existing unit - using foreach to avoid surprises
				foreach (DBCoordinates dbc in c.battlegroup) {
					// if unit is indeed found, clear the battlegroup slot (ie blank it)
					// look for all references to u; we want to make sure there are no surprises
					if (dbc.Equals (u.dbloc)) dbc.clear ();
				}
			}

			return result;

		}

		/*
		 * Clear character c's battlegroup
		 */
		public void battlegroupClear(Character c, bool onlyDead=false) {
			if (c != null) {
				UnitModel _unitModel = new UnitModel();
				foreach (DBCoordinates udbc in c.battlegroup) {
					Unit u = _unitModel.getUnit (udbc);
					if (u != null && ((onlyDead && u.isDead) || (!onlyDead))) battlegroupRemoveUnit(c, u);
				}
			}
		}

		// INVENTORY CONTROLS

		/*
		 * Get item DBCoordinates at index
		 */
		public DBCoordinates getInventoryItem (Character c, int index) {
			DBCoordinates result = new DBCoordinates ();
			if (index > -1 && index < 8) return c.inventory [index];
			return result;
		}

		/*
		 * Get Character's inventory as a list.
		 */
		public List<DBCoordinates> getInventory (Character c, bool includeEmpty=false) {
			List<DBCoordinates> result = new List<DBCoordinates> ();
			foreach (DBCoordinates idbc in c.inventory) {
				if ((idbc.isEmpty() && includeEmpty) == true || (!idbc.isEmpty())) result.Add (idbc);
			}
			return result;
		}

		/*
		 * Return item at itemIdx from inventory
		 */
		public Item inventoryGetItem(Character c, int itemIdx) {
			Item result = null;
			if (c != null && itemIdx >= 0 && itemIdx <= 5) {
				result = new ItemModel ().getItem (c.inventory [itemIdx]);
			}
			return result;
		}

		/*
		 * Return equipped item at itemIdx (0 or 1) from inventory
		 */
		public Item inventoryGetEquippedItem(Character c, int itemIdx) {
			Item result = null;
			if (c != null && (itemIdx == 0 || itemIdx == 1)) {
                ItemModel itemModel = new ItemModel();
				if (c.inventoryEquipped [itemIdx] >= 0 && c.inventoryEquipped[itemIdx] < 6) {
					result = itemModel.getItem (c.inventory [c.inventoryEquipped [itemIdx]]);
				}
			}
			return result;
		}

		/*
		 * Equip item at itemIdx, to specified slot. If item is already equipped in either slot, unequip it.
		 * Only player characters may equip two items; regular characters can equip just one.
		 */
		public void inventoryEquipItem(Character c, int itemIdx, int slot=0) {
			if (c != null && itemIdx >= 0 && itemIdx <= 6 && (slot == 0 || slot == 1)) {

				// Unequip item, if it is equipped in either slot
				if (c.inventoryEquipped [0] == itemIdx)
					c.inventoryEquipped [0] = -1;
				else if (c.inventoryEquipped [1] == itemIdx)
					c.inventoryEquipped [1] = -1;
				// Else equip the item
				else {
					// Only if there is an actual item in that slot
					if (!c.inventory [itemIdx].isEmpty ())
						// Prevent regular characters from equipping a second item
						c.inventoryEquipped [c.isPlayerCharacter == true ? slot : 0] = itemIdx;
				}
			}
		}

		/*
		 * Get the current size of the inventory (ie the number of items actually stored in it.
		 */
		public int inventoryGetSize(Character c) {
			int s = 0;
			if (c != null) {
				foreach (DBCoordinates i in c.inventory) {
					if (!i.isEmpty()) s++;
				}
			}
			return s;
		}

		/*
		 * Add item i to character c's inventory, while also removing i from its current inventory
		 */
		public bool inventoryAddItem(Character c, Item i, int slot=-1) {
			
			bool result = false;

			if (c != null && i != null) {

				// Make sure i's parent is not already c
				if (!i.parent.Equals (c.dbloc)) {

					// only add i to c's inventory if there is room left
					if (inventoryGetSize (c) <= 6) {

						// check if the item has a parent assigned (ie an inventory) and remove it from parent's
						// inventory

						if (!i.parent.isEmpty ()) inventoryRemoveItem(getCharacter (i.parent), i);

						i.parent = c.dbloc.clone ();

						int s;
						if (slot >= 0 && slot < 6) s = slot;
						else {
							// look for the first empty slot in c's inventory
							for (s = 0; s < 6; s++) {
								if (c.inventory [s].isEmpty ()) break;
							}
						}

						if (s < 6 && c.inventory [s].isEmpty ()) {
							c.inventory [s] = i.dbloc.clone ();
							result = true; // if we got here, then the method was successful
						}

					}

				}

			}

			return result;
		}

		public bool inventoryRemoveItem(Character c, Item i) {

			bool result = false;

			if (c != null && i != null) {
				// Clear u's parent if it equals to c's dbloc
				if (i.parent.Equals(c.dbloc)) i.parent.clear();

				// Unequip item if it is equipped in either slot
                ItemModel it = new ItemModel();
                int ind = 0;
                for (ind = 0; ind < c.inventory.Count; ind++)
                {
                    if (c.inventory[ind].Equals(i.dbloc))
                    {
						DBCoordinates dbloc = c.inventory[ind];
                        break;
                    }
                }
                if (c.inventoryEquipped[0] == ind) { c.inventoryEquipped[0] = -1; }
                if (c.inventoryEquipped[1] == ind) { c.inventoryEquipped[1] = -1; }
				//if (inventoryGetEquippedItem (c, 0).dbloc.Equals (i.dbloc)) c.inventoryEquipped [0] = -1;
				//if (inventoryGetEquippedItem (c, 1).dbloc.Equals (i.dbloc)) c.inventoryEquipped [1] = -1;

				// If c is in a game world, drop the item (ie change its worldloc)
				if (!c.worldloc.isEmpty()) i.worldloc.setValues(c.worldloc);

				// Ensure c does not have i in its inventory - whether or not the above statement yielded false
				// linear search for existing item - using foreach to avoid surprises
				foreach (DBCoordinates dbc in c.inventory) {
					// if item is indeed found, clear the inventory slot (ie blank it)
					// look for all references to i; we want to make sure there are no surprises
					if (dbc.Equals (i.dbloc)) dbc.clear ();
				}

			}

			return result;

		}

		public void inventoryClear(Character c) {
			if (c != null) {
				ItemModel _itemModel = new ItemModel();
				foreach (DBCoordinates idbc in c.inventory) {
					inventoryRemoveItem(c, _itemModel.getItem(idbc));
				}
			}
		}

		/*
		 * Add xp amount of experience points to Character c. If xp cap for level is reached, level up.
		 */
		public void addExperiencePoints(Character c, int xp) {

			if (c.experiencePoints < 1300) c.experiencePoints += xp;

			if (c.experiencePoints < 250) {
				c.level = 1;
			} else if (c.experiencePoints >= 250 && c.experiencePoints < 550) {
				c.level = 2;
			} else if (c.experiencePoints >= 550 && c.experiencePoints < 900) {
				c.level = 3;
			} else if (c.experiencePoints >= 900 && c.experiencePoints < 1300) {
				c.level = 4;
			} else {
				c.level = 5;
			}

		}

		/*
		 * Fully initialize this character, to ensure its fields are valid, and its units and items are added to the 
		 * Unit Model.
		 */
		public bool initializeCharacter(Character c) {

			// Only do this if the Character was not yet initialized
			if (c.isInitialized == false) {

				// Using a few models in here; I see no other option aside for moving this outside of 
				UnitModel _unitModel = new UnitModel ();
				PerkModel _perkModel = new PerkModel ();
				ItemModel _itemModel = new ItemModel ();
				Unit tmp;

				// Ensure unit has a valid rank
				if (new PerkModel().getPerkById(c.rankID) == null) c.rankID = "norank";

				// Ensure perk IDs are valid, otherwise remove them
				for (int i = c.perkIDs.Count - 1; i >= 0; i--)
					if (_perkModel.getPerkById (c.perkIDs [i]) == null)	c.perkIDs.RemoveAt (i);

				// Create and assign personal units, if any
				tmp = _unitModel.createUnit (c.unitPersonalID, c.factionIdx);
				if (tmp != null) c.units[0] = tmp.dbloc.clone();

				tmp = _unitModel.createUnit (c.unitGroundVehicleID, c.factionIdx);
				if (tmp != null) c.units[1] = tmp.dbloc.clone();

				tmp = _unitModel.createUnit (c.unitFlyingVehicleID, c.factionIdx);
				if (tmp != null) c.units[2] = tmp.dbloc.clone();

				for (int i = 0; i < 3; i++) {
					if (!c.units [i].isEmpty ()) {
						tmp = _unitModel.getUnit (c.units [i]);
						tmp.name = c.name;
						tmp.description = c.description;
						tmp.icon = c.icon;
					}
				}

				// Clear these - units were already created
				c.unitPersonalID = "";
				c.unitGroundVehicleID = "";
				c.unitFlyingVehicleID = "";

				// Populate c's battlegroup and add units to the UnitModel
				for (int u=0; u < (c.battlegroupUnitIDs.Count <= 8 ? c.battlegroupUnitIDs.Count : 8); u++) {
					battlegroupAddUnit (c, _unitModel.createUnit (c.battlegroupUnitIDs[u], c.factionIdx));
				}
				c.battlegroupUnitIDs.Clear ();

				// Populate c's inventory and add units to the ItemModel
				for (int i = 0; i < (c.itemIDs.Count <= 6 ? c.itemIDs.Count : 6); i++) {
					inventoryAddItem (c, _itemModel.createItem (c.itemIDs [i], c.factionIdx));
				}
				c.itemIDs.Clear ();

				c.isInitialized = true;

			}

			return c.isInitialized;
		}

    }
}
