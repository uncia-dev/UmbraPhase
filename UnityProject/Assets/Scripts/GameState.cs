using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using Umbra.Managers;
using Umbra.Models;
using System;
using System.IO;
using Umbra.Xml;

namespace Umbra.Data
{
    [System.Serializable]
    public class GameState
    {
        public Player player;
        public Spaceship starbase;

		// List of factions - keeps track of reputations and rosters
		public List<Faction> factions;
		// Dictionary of item instances
		public Dictionary<string, List<List<Item>>> items;
		// Dictionary of unit instances
		public Dictionary<string, List<List<Unit>>> units;
		// Dictionary of character instances
		public Dictionary<string, List<List<Character>>> characters;

		/*
		Items, characters and units are stored in dictionaries of the form (string, List of List of Object). Why?
		Simpler way to reach the objects!

		To access character with id x from faction y at index z just access characters[x][y][z]
		The idea here is to separate the objects by their IDs first, then by the faction that holds an instance.
		This can reduce the amount of linear searches that our game is doing
		Make sure to also use the DatabaseCoordinates object.
		*/

		public StarMapLevel currentStarmap;
		public string currentPlanetName;
        public Map currentMap; // Switching over to using the map instead of the name for loading maps
        public Outpost currentOutpost; //set upon loading a map by retrieving the outpost reference from map

		public Dictionary<string, Outpost> outpostDictionary;
        public Dictionary<int, SpaceshipSection> spaceshipSectionDictionary;
		public Dictionary<int, MapObject> mapObjectDictionary;
		public Dictionary<int, StarClusterPlanetState> starClusterPlanetDictionary;
		public Dictionary<int, StarClusterState> starClusterDictionary;
		public Dictionary<int, WormholeState> wormholeDictionary;

		// No need to serialize the base objects; load them from the XMLs when needed
		[NonSerialized]
		public Dictionary<string, Ability> abilityDictionary;
		[NonSerialized]
		public Dictionary<string, Buff> buffDictionary;
		[NonSerialized]
		public Dictionary<string, Character> characterDictionary;
		[NonSerialized]
		public Dictionary<string, Faction> factionDictionary;
		[NonSerialized]
		public Dictionary<string, Item> itemDictionary;
		[NonSerialized]
		public Dictionary<string, Map> mapDictionary;
		[NonSerialized]
		public Dictionary<string, Rank> rankDictionary;
		[NonSerialized]
		public Dictionary<string, Perk> perkDictionary;
		[NonSerialized]
		public Dictionary<string, Unit> unitDictionary;

		// List of the two warring characters; used when moving to a battle map
		public DBCoordinates[] battlingCharacters;

		public List<string> Objectives = new List<string>(new string[] {"Destroy all enemies.", "Make friends with netural factions."});
        
		/*
		 * Read the XML files into the game state object 
		 */
		public void databaseLoad() {

			abilityDictionary = LoadFromXml.loadFromXml<Ability> ("UmbraAbilities");
			buffDictionary = LoadFromXml.loadFromXml<Buff>("UmbraBuffs");
			characterDictionary = LoadFromXml.loadFromXml<Character>("UmbraCharacters");
			factionDictionary = LoadFromXml.loadFromXml<Faction>("UmbraFactions");
			itemDictionary = LoadFromXml.loadFromXml<Item>("UmbraItems");
			mapDictionary = LoadFromXml.loadFromXml<Map>("UmbraMaps");
			perkDictionary = LoadFromXml.loadFromXml<Perk>("UmbraPerks");
			rankDictionary = LoadFromXml.loadFromXml<Rank>("UmbraRanks");
			unitDictionary = LoadFromXml.loadFromXml<Unit>("UmbraUnits");

		}

		/*
		 * Return true if all databases are loaded (ie populated)
		 */
		public bool databaseIsLoaded() {

			try{

				if (
					abilityDictionary.Count > 0 &&
					buffDictionary.Count > 0 &&
					characterDictionary.Count > 0 &&
					factionDictionary.Count > 0 &&
					itemDictionary.Count > 0 &&
					mapDictionary.Count > 0 &&
					perkDictionary.Count > 0 &&
					rankDictionary.Count > 0 &&
					unitDictionary.Count > 0) return true;

			} catch (NullReferenceException) {
				// do nothing, this is expected when init() is run
			}
			return false;

		}

        // called when starting the game
        public void init() {

			if (databaseIsLoaded () == false) databaseLoad ();

			// init all vars
            player = new Player();
            starbase = new Spaceship();
			outpostDictionary = new Dictionary<string, Outpost>();
            spaceshipSectionDictionary = new Dictionary<int, SpaceshipSection>();
			mapObjectDictionary = new Dictionary<int, MapObject> ();
			starClusterPlanetDictionary = new Dictionary<int, StarClusterPlanetState> ();
			starClusterDictionary = new Dictionary<int, StarClusterState> ();
			wormholeDictionary = new Dictionary<int, WormholeState> ();
			wormholeDictionary[0] = (new WormholeState { isCurrentLocation = true } );

			factions = new List<Faction> ();
			units = new Dictionary<string, List<List<Unit>>> ();
			characters = new Dictionary<string, List<List<Character>>> ();
			items = new Dictionary<string, List<List<Item>>> ();

            //setting everyone to these values for now
            // minerals, gasses, fuel, water, food, meds, f1c, f2c, f3c, f4c, f5c
            player.resourcesFaction1 = player.resourcesFaction2 = player.resourcesFaction3 = player.resourcesFaction4 = player.resourcesFaction5 = player.resourcesFaction6 = 10000;
            player.resourcesMinerals = player.resourcesGas = player.resourcesFuel = player.resourcesFood = player.resourcesWater = player.resourcesMeds = 10000;

            starbase.name = "SSC Radiance";

            for (int i = 0; i < 18; i++)
            {
                SpaceshipSection module = new SpaceshipSection();
                module.name = ShipManager.moduleName[i];
                module.desc = ShipManager.moduleDesc[i];
                module.nextRecReq = new int[6];
                for (int j = 0; j < 6; j++)
                {
                    module.nextRecReq[j] = 500;
                }
                module.levelCurrent = 1;
                spaceshipSectionDictionary.Add(i + 1, module);
                starbase.spaceshipSectionID.Add(module);

            }

			// Add blank perk to dictionary
			perkDictionary["noperk"] = new Perk();
			// Add blank rank to dictionary
			rankDictionary["norank"] = new Rank ();
			// Add generic item to dictionary
			itemDictionary ["defaultjunk"] = new Item ();

			factions.Add (new Faction ()); // use this for unassigned units and characters
			// Clone factions to a faction list
			foreach (Faction f in factionDictionary.Values) {
				// Clone a new faction from XML to game state variant
				factions.Add(f.clone ());
				factions [0].reputations.Add (-1); // set all reputations to -1 for nofaction faction
			}

			// Build blank dictionary of units, and assign an empty list for each unit of each faction
			foreach (string u in unitDictionary.Keys) {
				units [u] = new List<List<Unit>> ();
				for (int i = 0; i < factions.Count; i++) units [u].Add (new List<Unit> ());
			}

			// Build blank dictionary of items, and assign an empty list for each item of each faction
			foreach (string i in itemDictionary.Keys) {
				items [i] = new List<List<Item>> ();
				for (int c = 0; c < factions.Count; c++) items [i].Add (new List<Item> ());
			}

			/*
			Build dictionary of character clones, assigning a character clone to each faction, based on the ID, just
			like units and items. This won't use the Model, since it's only for initialization. 

			New characters will be created/accessed via the CharacterModel. Also note that each character entry will
			have only one character clone at this stage.
			*/
			foreach (Character c in characterDictionary.Values) {
				// Make a clone of each character template
				Character tc = c.clone ();
				// ensure faction index is within bounds; if not, add unit to nofaction (ie 0)
				tc.factionIdx = (c.factionIdx >= 0 && c.factionIdx < factions.Count) ? c.factionIdx : 0;
				tc.dbloc.setValues(tc.id, c.factionIdx, 0);

				characters [tc.id] = new List<List<Character>> ();
				for (int i = 0; i < factions.Count; i++) {
					characters [tc.id].Add (new List<Character>());
				}
				characters [tc.id] [tc.factionIdx].Add (tc);

			}

			battlingCharacters = new DBCoordinates[2] {
				new DBCoordinates(), new DBCoordinates()
			};

        }
    }
}