using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Xml.Serialization;
using System;
using Umbra.Utilities;

namespace Umbra.Data
{
    //This file will hold the classes that will be used in the "database" file. 
    //The database file will have a bunch of dictionaries (C# hash map) of these classes to 
    //hold all of the assets or versions loaded from xml 
    //The ID values from the schema became the map key for the database, so not be stored in class

    [System.Serializable]
    public class Faction
    {
		public string id { get; set; }
		public int factionIdx { get; set; }
		public string name { get; set; }
		public string description { get; set; }
        [XmlArrayItem("reputation")]
		public List<int> reputations { get; set; }
		public string currency { get; set; }
		[XmlArrayItem("channel")]
		public float[] color { get; set; }
		[XmlIgnore]
		public int battleCount { get; set; }
		public string icon { get; set; }

        public Faction()
        {
			id = "nofaction";
			factionIdx = 0;
			name = "No affiliation.";
			description = "No affiliation.";
			reputations = new List<int> ();
            currency = "";
			color = new float[] {0.0f, 0.0f, 0.0f};
            icon = "";
        }

		public Faction clone()	{

			MemoryStream m = new MemoryStream();
			BinaryFormatter b = new BinaryFormatter();
			b.Serialize(m, this);
			m.Position = 0;
			return (Faction)b.Deserialize(m);

		}

    }

    //This was originally in the game data file
    [System.Serializable]
    public class Movable
    {
        public float[] loc;                             // because we cant serialize Vectors D:
        Vector3 getLoc()
        {
            return new Vector3(loc[0], loc[1], loc[2]);
        }
        void setLoc(Vector3 l)
        {
            loc[0] = l.x;
            loc[1] = l.y;
            loc[2] = l.z;
        }
    }

    [System.Serializable]
    public class QuestScript
    {   
        public int id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string scriptCode { get; set; }

        public QuestScript()
        {
            id = 0;
            name = "";
            description = "";
            scriptCode = "";
        }

    }

    [System.Serializable]
    public class QuestStage
    {
        public int id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string icon { get; set; }
        public List<QuestScript> questsScriptID { get; set; } //list of keys which you can find the corresponding questscripts in the hash map
        public bool isCompleted { get; set; }
        public List<Dialogue> dialogueID { get; set; }

        public QuestStage()
        {
            id = 0;
            //questID = new Quest();
            name = "";
            description = "";
            icon = "";
            questsScriptID = new List<QuestScript>();
            isCompleted = false;
            dialogueID = new List<Dialogue>();
        }
    }

    [System.Serializable]
    public class Dialogue
    {

        public string dialogueID { get; set; }
        public string dialogueTextScript { get; set; }
        public string speaker { get; set; } //I am unsure what the speaker will be (just a name? or a class?)

        // deprecated
        public int id { get; set; }
        // deprecated

        public Dialogue()
        {
            id = 0;
            dialogueTextScript = "";
            speaker = "";
        }
    }
    [System.Serializable]
    public class Quest
    {

        public string questID { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string icon { get; set; }
        public List<QuestStage> questsStageID { get; set; }
        public bool isCompleted { get; set; }

        // deprecated
        public int id { get; set; }
        // deprecated

        public Quest()
        {
            id = 0;
            name = "";
            description = "";
            icon = "";
            questsStageID = new List<QuestStage>();
            //            mapID = new Map();
            isCompleted = false;
        }
    }
    [System.Serializable]
    public class Tileset
    {

        public string tileSetID { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string icon { get; set; }
        public List<Tile> TileID { get; set; }

        // deprecated
        public int id { get; set; }
        // deprecated

        public Tileset()
        {
            id = 0;
            name = "";
            description = "";
            icon = "";
            TileID = new List<Tile>();
        }
    }
    [System.Serializable]
    public class Tile
    {
        public int id { get; set; }
        public string filePath { get; set; }
        public bool isDeadly { get; set; }
        public bool isPassable { get; set; }
        public int damagePerSecond { get; set; }

        public Tile()
        {
            id = 0;
            filePath = "";
            isDeadly = false;
            isPassable = false;
            damagePerSecond = 0;
        }
    }
    [System.Serializable]
    public class Map
    {

        public string id { get; set; }
		public string name { get; set; }
		public string description { get; set; }
		public string filename { get; set; } //The filename is used to load the map not the name
		public string outpostID { get; set; }
		public Outpost outpostRef { get; set; } // TODO: Serialization issue?
		public string mapQuestID { get; set; }
		public string parentMapID { get; set; }
		[XmlArrayItem("battleMapID")]
		public List<string> battleMapIDs { get; set; }
        //public Tileset tileSetID { get; set; }
        //public MapObject mapObjectID { get; set; }
		public bool isBattleMap { get; set; }
		public bool isSpaceMap { get; set; }
		public bool isOutpostFriendly { get; set; }
		public string icon { get; set; }

        public Map()
        {
            id = "";
            name = "";
            description = "";
			filename = "";
			outpostID = "";
			outpostRef = new Outpost (); // TODO: Serialization issue?
			mapQuestID = "";
			battleMapIDs = new List<string>();
			isBattleMap = false;
			isSpaceMap = false;
			isOutpostFriendly = false;
			icon = "";
        }
    }
    [System.Serializable]
    public class MapObject
    {
        public int id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string icon { get; set; }
        public string sprite { get; set; }
        public string model { get; set; }
        public List<string> soundSelect { get; set; } //unsure of this one as well
        public List<string> soundAmbient { get; set; } //unsure of this one as well
        public MapObjectScript mapObjectScriptID { get; set; }
        public bool isVisited { get; set; }
        public int ownerID { get; set; } //?
        public int sizeW { get; set; }
        public int sizeL { get; set; }
        public int sizeH { get; set; }
        public bool isSolid { get; set; }
        public int LocationX { get; set; }
        public int LocationY { get; set; }
        public int LocationZ { get; set; }

        public MapObject()
        {
            id = 0;
            name = "";
            description = "";
            icon = "";
            sprite = "";
            model = "";
            soundSelect = new List<string>();
            soundAmbient = new List<string>();
            //            mapObjectScriptID = new MapObjectScript();
            isVisited = false;
            ownerID = 0;
            sizeW = sizeL = sizeH = 0;
            isSolid = false;
            LocationX = LocationY = LocationZ = 0;
        }
    }
    [System.Serializable]
    public class MapObjectScript
    {
        public int id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string scriptCode { get; set; }

        public MapObjectScript()
        {
            id = 0;
            name = "";
            description = "";
            scriptCode = "";
        }
    }
    [System.Serializable]
    public class GeneralScript
    {
        public int id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string scriptCode { get; set; }
        public int parentID { get; set; } //?

        public GeneralScript()
        {
            id = 0;
            name = "";
            description = "";
            scriptCode = "";
            parentID = 0;
        }
    }

    [Serializable]
    public class StarClusterPlanetState {
        public bool isVisited = false;
        public bool isCompleted = false;
    }

    [Serializable]
    public class StarClusterState {
        public bool isVisited = false;
    }

    [Serializable]
    public class WormholeState {
        public bool isVisited = false;
        public bool isCurrentLocation = false;
    }

    [System.Serializable]
    public class Player : Movable
    {

		// These are grabbed during the new character creation process
		public string name { get; set; }
		public string className { get; set; }
		public string gender { get; set; }
		public string shipName { get; set; }
		public int characterLevel { get; set; }

		// Disabling this functionality; lacking the units anyway
		//public Unit chosenPersonalUnitRef { get; set; }
		//public Unit chosenGroundVehicleUnitRef { get; set; }
		//public Unit chosenFlyingVehicleUnitRef { get; set; }

		public List<Quest> questID { get; set; }
        public string icon { get; set; }
        public int karma { get; set; }
        public int luck { get; set; }
        public int resourcesMinerals { get; set; }
        public int resourcesGas { get; set; }
        public int resourcesFuel { get; set; }
        public int resourcesWater { get; set; }
        public int resourcesFood { get; set; }
        public int resourcesMeds { get; set; }
        public int resourcesPeople { get; set; }
        public int resourcesFaction1 { get; set; }
        public int resourcesFaction2 { get; set; }
        public int resourcesFaction3 { get; set; }
        public int resourcesFaction4 { get; set; }
        public int resourcesFaction5 { get; set; }
        public int resourcesFaction6 { get; set; }
		[XmlIgnore]
		public int battleCount { get; set; }


        public Player()
        {
			name = "John Walker";
			className = "Soldier";
			gender = "Male";
			characterLevel = 1;
			//chosenPersonalUnitRef = null;
			//chosenGroundVehicleUnitRef = null;
			//chosenFlyingVehicleUnitRef = null;
			shipName = "SSC Radiance";
            questID = new List<Quest>();
            icon = "";
            karma = 0;
            luck = 0;
            resourcesMinerals = resourcesGas = resourcesFuel = resourcesWater = resourcesFood = resourcesMeds = resourcesPeople = 0;
            resourcesFaction1 = resourcesFaction2 = resourcesFaction3 = resourcesFaction4 = resourcesFaction5 = resourcesFaction6 = 0;
            battleCount = 0;
        }
    }

    [System.Serializable]
    public class Outpost
    {
        public string id { get; set; }
        public int x { get; set; }
        public int y { get; set; }
        public bool hasBeenPlaced { get; set; }
        public string outpostDesc;
        public string garrisonDesc;
        public string forceFieldDesc;
        public string turretsDesc;
        public string processingDesc;
        public string medicalDesc;
        public string biodomeDesc;
        public string tradingDesc;
        public int[] outpostReq;
        public int[] garrisonReq;
        public int[] forceFieldReq;
        public int[] turretsReq;
        public int[] processingReq;
        public int[] medicalReq;
        public int[] biodomeReq;
        public int[] tradingReq;
        public string name { get; set; }
        public string description { get; set; }
        public string icon { get; set; }
        public bool hasOutpost { get; set; }
        public bool hasGarrison { get; set; }
        public bool hasForceField { get; set; }
        public bool hasTurrets { get; set; }
        public bool hasProcessingPlant { get; set; }
        public bool hasMedicalLab { get; set; }
        public bool hasBiodome { get; set; }
        public bool hasTradingPost { get; set; }
        public Character defendingCharacterID { get; set; }
        public MapObject mapObjectID { get; set; }

        public Outpost()
        {
            outpostReq = new int[6];
            garrisonReq = new int[6];
            forceFieldReq = new int[6];
            turretsReq = new int[6];
            processingReq = new int[6];
            medicalReq = new int[6];
            biodomeReq = new int[6];
            tradingReq = new int[6];

            for (int i = 0; i < 6; i++)
            {
                outpostReq[i] = 500;
                garrisonReq[i] = 500;
                forceFieldReq[i] = 500;
                turretsReq[i] = 500;
                processingReq[i] = 500;
                medicalReq[i] = 500;
                biodomeReq[i] = 500;
                tradingReq[i] = 500;
            }

            outpostDesc = "Features a mining facility that slowly gathers minerals, water, and gasses. Allows a battlegroup.";
            garrisonDesc = "Boosts defenses and morale in case a battle occurs. Adds walls for protection against enemy units.";
            forceFieldDesc = "Further boosts defenses and morale in case a battle occurs.";
            turretsDesc = "Four turrets that randomly attack an enemy unit in battle. Boosts player damage multipliers.";
            processingDesc = "Speeds up the resource generation process and increases the quantity of gained resources. Generates fuel.";
            medicalDesc = "Generates medical supplies.";
            biodomeDesc = "Generates food.";
            tradingDesc = "Allows exchanging resources for other resources. Randomly posts a companion for recruitment.";

            id = "";
            name = "";
            description = "";
            icon = "";
            hasOutpost = false;
            hasGarrison = false;
            hasForceField = false;
            hasTurrets = false;
            hasProcessingPlant = false;
            hasMedicalLab = false;
            hasBiodome = false;
            hasTradingPost = false;
            //defendingCharacterID = new Character();
            //mapObjectID = new MapObject();
        }
    }
    [System.Serializable]
    public class Spaceship
    {
        public int id { get; set; }
        public string name { get; set; }
        public List<SpaceshipSection> spaceshipSectionID { get; set; }

        public Spaceship()
        {
            id = 0;
            name = "";
            spaceshipSectionID = new List<SpaceshipSection>();
        }
    }

    [System.Serializable]
    public class SpaceshipSection
    {
        public int id { get; set; }
        public string name { get; set; }
        public int levelCurrent { get; set; }
        public int levelMax { get; set; }
        public bool isMax { get; set; }
        public bool isUnlocked { get; set; }
        public string desc { get; set; }
        public int[] nextRecReq { get; set; }

        public SpaceshipSection()
        {
            id = 0;
            name = "";
            levelCurrent = 0;
            levelMax = 0;
            isMax = false;
            isUnlocked = false;
            desc = "";

        }
    }

	[System.Serializable]
	public class WorldCoordinates
	{

		public int x { get; set; }
		public int y { get ; set; }
		public int direction { get; set; }
		public string map { get; set; }

		// Default, empty constructor
		public WorldCoordinates() {
			setValues (-1, -1, -1, "");
		}

		// Constructors with the data members for arguments; comes in handy
		public WorldCoordinates(int cx, int cy, int d=0, string m="") {
			setValues (cx, cy, d, m);
		}

		// Set values of this instance
		public void setValues(int cx, int cy, int d=0, string m="") {
			x = cx;
			y = cy;
			direction = d;
			if (!m.Equals("")) map = String.Copy (m);
		}

		// Set values of this instance
		public void setValues(WorldCoordinates wc) {
			x = wc.x;
			y = wc.y;
			direction = wc.direction;
			map = String.Copy (wc.map);
		}

		// Check for equality versus other
		public bool Equals(WorldCoordinates other) {
			if (this != other) {
				if (map.Equals (other.map) && x == other.x && y == other.y && direction == other.direction)
					return true;
			}
			return false;
		}

		// Check if all values have been initialized
		public bool isEmpty() {
			//return (map.Equals("") || x == -1 || y == -1);
			return x == -1 || y == -1;
		}

		// Return a clone of this object
		public WorldCoordinates clone() {
			return new WorldCoordinates (x, y, direction, String.Copy(map));
		}

		// Return a string of this DBCoordiantes' content
		override
		public string ToString() {
			return "( " + x.ToString() + ", " + y.ToString() + ", " + direction.ToString() + ", " + map + " )";
		}

	}

	[System.Serializable]
	public class DBCoordinates // would have used Tuples instead, but Unity does not support them :\
	{

		public string id { get; set; }
		public int faction { get; set; }
		public int index { get; set; }

		// Default, empty constructor
		public DBCoordinates() {
			setValues ("", -1, -1);
		}

		// Constructors with the data members for arguments; comes in handy
		public DBCoordinates(string objID, int objFactionIdx, int objListIdx) {
			setValues (objID, objFactionIdx, objListIdx);
		}

		// Set values of this instance
		public void setValues(string objID, int objFactionIdx, int objListIdx) {
			id = String.Copy (objID);
			faction = objFactionIdx;
			index = objListIdx;
		}

		// Check for equality versus other
		public bool Equals(DBCoordinates other) {
			if (this != other) {
				if (id.Equals (other.id) && faction == other.faction && index == other.index)
					return true;
			}
			return false;
		}

		// Check if all values have been initialized
		public bool isEmpty() {
			return (id.Equals("") || faction == -1 || index == -1);
		}

		// Return to original "empty" state
		public void clear() {
			setValues ("", -1, -1);
		}

		// Return a clone of this object
		public DBCoordinates clone() {
			return new DBCoordinates (String.Copy(id), faction, index);
		}

		// Return a string of this DBCoordiantes' content
		override
		public string ToString() {
			return "( " + id + ", " + faction.ToString() + ", " + index.ToString() + " )";
		}

	}

    [System.Serializable]
    public class Character
    {   
		
		[XmlIgnore]
		public DBCoordinates dbloc;
		[XmlIgnore]
		public WorldCoordinates worldloc { get; set; }

		// Basics
        public string id { get; set; }
        public string name { get; set; }
        public string description { get; set; }

        // Attributes
        public string gender { get; set; }
        public string className { get; set; }
		[XmlArrayItem("unitID")]
		public List<string> battlegroupUnitIDs { get; set; } // initial values only
		[XmlIgnore]
		public List<DBCoordinates> battlegroup { get; set; } // this now stores the "coordinates" to the units in the model
		public int factionIdx { get; set; }
        public int level { get; set; }
		public string rankID { get; set; }
        public int experiencePoints { get; set; }
		public int unitCurrentIdx { get; set; }
		[XmlIgnore]
		public List<DBCoordinates> units { get; set; }
		public string unitPersonalID { get; set; } // initial values only
		public string unitGroundVehicleID { get; set; } // initial values only
		public string unitFlyingVehicleID { get; set; } // initial values only
		[XmlIgnore]
        public int battleCount { get; set; }
		[XmlArrayItem("perkID")]
        public List<string> perkIDs { get; set; }
		[XmlArrayItem("itemID")]
		public List<string> itemIDs { get; set; } // initial values only
		[XmlIgnore]
		public List<DBCoordinates> inventory { get; set; }
		[XmlIgnore]
		public int[] inventoryEquipped { get; set; }

        // Flags
        public bool isEnabled { get; set; }
        public bool isPlayerCharacter { get; set; }
        public bool isImportant { get; set; }
        public bool isDead { get; set; }
		public bool isUnique { get; set; }
		[XmlIgnore]
		public bool isInitialized { get; set; }

        // Resources
        public string icon { get; set; }

        public Character()
        {
			
			dbloc = new DBCoordinates ();
			worldloc = new WorldCoordinates ();

			id = "";
            name = "";
            gender = "";
            description = "";
            className = "";
			battlegroupUnitIDs = new List<string>();
			battlegroup = new List<DBCoordinates> ();
			for (int i = 0; i < 8; i++) battlegroup.Add (new DBCoordinates ()); // populate the battlegroup with blanks
			factionIdx = -1;
			level = 0;
			rankID = "";
			experiencePoints = 0;
			unitCurrentIdx = 0;
			units = new List<DBCoordinates> ();
			for (int i = 0; i < 3; i++)	units.Add (new DBCoordinates ()); // populate the unit choices with blanks
			unitPersonalID = "";
			unitGroundVehicleID = "";
			unitFlyingVehicleID = "";
			battleCount = 0;
			perkIDs = new List<string> ();
			itemIDs = new List<string> ();
			inventory = new List<DBCoordinates> ();
			for (int i = 0; i < 6; i++) inventory.Add (new DBCoordinates ()); // populate the inventory with blanks
			inventoryEquipped = new int[2]{-1, -1};
			isEnabled = false;
			isPlayerCharacter = false;
            isImportant = false;
            isDead = false;
			isUnique = false;
			isInitialized = false;
			icon = "DefaultAvatar";
        }

		public Character clone() {

			MemoryStream m = new MemoryStream();
			BinaryFormatter b = new BinaryFormatter();
			b.Serialize(m, this);
			m.Position = 0;
			return (Character)b.Deserialize(m);

		}

    }

    [System.Serializable]
    public class Rank
    {
        // Basics
        public string id { get; set; }
        public string name { get; set; }
        public string description { get; set; }

        // Attributes
        public int requiredLevel { get; set; }
        public int unitCap { get; set; }
        public string nextRankID { get; set; }

        // Resources
        public string icon { get; set; }

        public Rank()
        {
			id = "norank";
			name = "No Rank";
            description = "This character has no rank.";
            icon = "";
            requiredLevel = 0;
			unitCap = 0;
            nextRankID = "";
        }
    }

    [System.Serializable]
    public class Perk
    {
        // Basics
        public string id { get; set; }
        public string name { get; set; }
        public string description { get; set; }

        // Flags
        public bool isEnabled { get; set; }

        // Resources
        public string icon { get; set; }

        // perk function
        public int perkMultiplierArray { get; set; } //unsure of what this should be

        public Perk()
        {
			id = "noperk";
            name = "Special Snowflake";
            description = "You are a special snowflake. No bonuses though.";
            icon = "DefaultPerk";
            perkMultiplierArray = 0;
            isEnabled = false;
        }
    }

    [System.Serializable]
    public class Item
    {

		[XmlIgnore]
		public DBCoordinates dbloc;
		[XmlIgnore]
		public WorldCoordinates worldloc { get; set; }
		[XmlIgnore]
		public DBCoordinates parent { get; set; }

		// Basics
        public string id { get; set; }
        public string name { get; set; }
        public string description { get; set; }

        // Attributes
        public string abilityID { get; set; }
		public int abilityCooldown { get; set; }
        public string buffID { get; set; }
		public int buffCooldown { get; set; }
        public int charges { get; set; }

        // Flags
        public bool isSellable { get; set; }
        public bool isPurchasable { get; set; }
        public bool isDroppable { get; set; }

        // Costs
        public int costMinerals { get; set; }
        public int costGas { get; set; }
        public int costFuel { get; set; }
        public int costWater { get; set; }
        public int costFood { get; set; }
        public int costMeds { get; set; }
        public int costPeople { get; set; }
        public int costFaction1 { get; set; }
        public int costFaction2 { get; set; }
        public int costFaction3 { get; set; }
        public int costFaction4 { get; set; }
        public int costFaction5 { get; set; }
        public int costFaction6 { get; set; }

        // Resources
        public string icon { get; set; }
		public string sprite { get; set; }

        public Item()
        {
			
			dbloc = new DBCoordinates ();
			worldloc = new WorldCoordinates ();
			parent = new DBCoordinates ();

			id = "DefaultJunk";
			name = "Level 1 Vendor Trash";
            description = "It's a gray item.";
            icon = "DefaultItem";
			sprite = "";
			abilityID = "";
			abilityCooldown = -1;
			buffID = "";
			buffCooldown = -1;
            charges = 0;
            isSellable = false;
            isPurchasable = false;
            isDroppable = false;
            costMinerals = costGas = costFuel = costWater = costFood = costMeds = costPeople = 0;
            costFaction1 = costFaction2 = costFaction3 = costFaction4 = costFaction5 = costFaction6 = 0;

        }

		public Item clone()	{

			MemoryStream m = new MemoryStream();
			BinaryFormatter b = new BinaryFormatter();
			b.Serialize(m, this);
			m.Position = 0;
			return (Item)b.Deserialize(m);

		}

    }

    [System.Serializable]
    public class Unit
    {

		[XmlIgnore]
		public DBCoordinates dbloc;
		[XmlIgnore]
		public WorldCoordinates worldloc { get; set; }
		[XmlIgnore]
		public DBCoordinates parent { get; set; }
        
		// Basics
		public string id { get; set; } // this is not a unique ID - it's the type of unit, ex. HF1_Soldier
        public string name { get; set; }
        public string description { get; set; }
		public int factionIdx { get; set; }

        // Attributes
        public string unitClass { get; set; }
        public int movementRange { get; set; }
        public double movementRangeMult { get; set; }
        public int movementSpeed { get; set; }
        public double movementSpeedMult { get; set; }
        public int altitude { get; set; }
        public int density { get; set; }
        public int subunitCount { get; set; }        
        public int health { get; set; }
        public double healthMult { get; set; }
        public int armor { get; set; }
        public double armorMult { get; set; }
        public int shield { get; set; }
        public double shieldMult { get; set; }
        public int damageBonus { get; set; }
        public double damageMult { get; set; }
        public int healBonus { get; set; }
        public double healMult { get; set;}
        public int techLevel { get; set; }
		[XmlIgnore]
        public int battleCount { get; set; }

        // AI
        public int threatLevel { get; set; }
        public bool isDefender { get; set; }

        // Flags
        public bool isMovable { get; set; }
        public bool isSpaceFriendly { get; set; }
        public bool isIndoorsFriendly { get; set; }
        public bool isOrganic { get; set; }
        public bool isSquishy { get; set; }
        public bool isCharacter { get; set; }
        public bool isPlayerCharacter { get; set; }
        public bool isDead { get; set; }
        public bool isInvincible { get; set; }
        public bool isVisible { get; set; }
        public bool isDisabled { get; set; }

        // Recruitment
        public bool isRecruitable { get; set; }
        public string requiredRankID { get; set; }
        [XmlArrayItem("sectionID")]
        public List<string> requiredSections { get; set; }
        public int recruitmentTimer { get; set; }
        public double recruitmentTimerMult { get; set; }
        public int costMinerals { get; set; }
        public int costGas { get; set; }
        public int costFuel { get; set; }
        public int costWater { get; set; }
        public int costFood { get; set; }
        public int costMeds { get; set; }
        public int costPeople { get; set; }
        public int costFaction1 { get; set; }
        public int costFaction2 { get; set; }
        public int costFaction3 { get; set; }
        public int costFaction4 { get; set; }
        public int costFaction5 { get; set; }
        public int costFaction6 { get; set; }

        // Abilities 
        [XmlArrayItem("ability")]
        public List<string> abilities { get; set; }

        // Resources
        public string icon { get; set; }
        public string baseModel { get; set; }
        public string baseSprite { get; set; }
        public string eliteModel { get; set; }
        public string eliteSprite { get; set; }
        [XmlArrayItem("sprite")]
        public List<string> particlesDamage { get; set; }
        [XmlArrayItem("sprite")]
        public List<string> particlesExplosion { get; set; }
        [XmlArrayItem("sound")]
        public List<string> soundSelect { get; set; }
        [XmlArrayItem("sound")]
        public List<string> soundMove { get; set; }
        [XmlArrayItem("sound")]
        public List<string> soundMoveError { get; set; }
        [XmlArrayItem("sound")]
        public List<string> soundMoving { get; set; }
        [XmlArrayItem("sound")]
        public List<string> soundAttack { get; set; }
        [XmlArrayItem("sound")]
        public List<string> soundAttackError { get; set; }
        [XmlArrayItem("sound")]
        public List<string> soundDeath { get; set; }

        public Unit()
        {

			dbloc = new DBCoordinates ();
			worldloc = new WorldCoordinates ();
			parent = new DBCoordinates ();

			id = "cannonfodder";
            name = "Cannon Fodder";
            description = "Cannon fodder unit that should never exist in the game world.";
			factionIdx = -1;

            // Attributes
            unitClass = "";
            movementRange = 1;
            movementRangeMult = 1;
            movementSpeed = 1;
            movementSpeedMult = 1;
            altitude = 0;
            density = 0;
            subunitCount = 1;
            health = 1;
            healthMult = 1;
            armor = 1;
            armorMult = 1;
            shield = 1;
            shieldMult = 1;
            damageBonus = 0;
            damageMult = 1;
            healBonus = 0;
            healMult = 1;
            techLevel = 1;
            battleCount = 0;

            // AI
            threatLevel = 0;
            isDefender = false;

            // Flags
            isMovable = true;
            isSpaceFriendly = false;
            isIndoorsFriendly = false;
            isOrganic = false;
            isSquishy = false;
            isCharacter = false;
            isPlayerCharacter = false;
            isDead = false;
            isInvincible = false;
            isVisible = false;
            isDisabled = false;

            // Recruitment
            isRecruitable = true;
            requiredRankID = "";
            requiredSections = new List<string>();;
            recruitmentTimer = 0;
            recruitmentTimerMult = 1;
            costMinerals = 0;
            costGas = 0;
            costFuel = 0;
            costWater = 0;
            costFood = 0;
            costMeds = 0;
            costPeople = 0;
            costFaction1 = 0;
            costFaction2 = 0;
            costFaction3 = 0;
            costFaction4 = 0;
            costFaction5 = 0;
            costFaction6 = 0;

            // Abilities 
			abilities = new List<string>();

            // Resources
			icon = "DefaultUnit";
            baseModel = "";
            baseSprite = "";
            eliteModel = "";
            eliteSprite = "";
            particlesDamage = new List<string>();
            particlesExplosion = new List<string>();
            soundSelect = new List<string>();
            soundMove = new List<string>();
            soundMoveError = new List<string>();
            soundMoving = new List<string>();
            soundAttack = new List<string>();
            soundAttackError = new List<string>();
            soundDeath = new List<string>();

        }

		// Get the sprite or model of this unit
        public string getVisuals() {
            // trigger eliteModel/Sprite for techlevel 3 - expand the logic later
            return baseModel != "" ? baseModel : baseSprite;
        }

		public Unit clone()	{

			MemoryStream m = new MemoryStream();
			BinaryFormatter b = new BinaryFormatter();
			b.Serialize(m, this);
			m.Position = 0;
			return (Unit)b.Deserialize(m);

		}

    }

    [System.Serializable]
    public class Ability
    {

        // Basics
        public string id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public int cooldown { get; set; }
		public int damageHealth { get; set; }
        public int damageArmor { get; set; }
        public int damageShield { get; set; }
		public int damageFire { get; set; }
		public int damageFrost { get; set; }
		public int damageElectric { get; set; }
		public int damageAcid { get; set; }
		public int damageRadiation { get; set; }
		public double ignoresHealth { get; set; }
		public double ignoresArmor { get; set; }
		public double ignoresShield { get; set; }
		[XmlArrayItem("buffID")]
		public List<string> casterBuffs { get; set; }
		[XmlArrayItem("buffID")]
		public List<string> targetBuffs { get; set; }
        public string functionName { get; set; }
		public int range { get; set; }
		public int AOERadius { get; set; }
		public int AIPriority { get; set; }
		public double proximityBonus { get; set; }
		public double infantryBonusDamage { get; set; }
		public double groundVehicleBonusDamage { get; set; }
		public double flyingVehicleBonusDamage { get; set; }
		public double noDefensesBonus { get; set; }

        // Flags
        public bool isHarmful { get; set; }
		public bool isHelpful { get; set; }
        public bool isHarmfulFriendly { get; set; }
		public bool isSelfCast { get; set; }
		public bool isLockable { get; set; }
        public bool isResistible { get; set; }
		public bool isCrowdControlling { get; set; }
		public bool isZAxisLocked { get; set; }
		public bool isDamaging { get; set; }
		public bool isCastableOnDisabled { get; set; }

        // Resources
        public string icon { get; set; }
        public string projectileSprite { get; set; }
        public string projectibleModel { get; set; }
        public string animationCasterSprite { get; set; }
        public string animationCasterModel { get; set; }
        public string animationCastingSprite { get; set; }
        public string animationCastingModel { get; set; }
        public string animationTargetSprite { get; set; }
        public string animationTargetModel { get; set; }
        public string soundStart { get; set; }
        public string soundCasting { get; set; }
        public string soundEnd { get; set; }        

        public Ability()
        {
            // Basics
            id = "generic";
            name = "generic ability";
            description = "";
            cooldown = 0;
			damageHealth = 0;
            damageArmor = 0;
            damageShield = 0;
			damageFire = 0;
			damageFrost = 0;
			damageElectric = 0;
			damageAcid = 0;
			damageRadiation = 0;
			ignoresHealth = 0;
			ignoresArmor = 0;
			ignoresShield = 0;
			casterBuffs = new List<string> ();
			targetBuffs = new List<string> ();
            functionName = "";
			range = 0;
			AOERadius = 0;
			AIPriority = 0;
			proximityBonus = 0;
			infantryBonusDamage = 0;
			groundVehicleBonusDamage = 0;
			flyingVehicleBonusDamage = 0;
			noDefensesBonus = 0;

            // Flags
            isHarmful = false;
			isHelpful = false;
            isHarmfulFriendly = false;
			isSelfCast = true;
			isLockable = false;
            isResistible = false;
			isCrowdControlling = false;
			isZAxisLocked = false;
			isDamaging = false;
			isCastableOnDisabled = false;

            // Resources
            icon = "";
            projectileSprite = "";
            projectibleModel = "";
            animationCasterSprite = "";
            animationCasterModel = "";
            animationCastingSprite = "";
            animationCastingModel = "";
            animationTargetSprite = "";
            animationTargetModel = "";
            soundStart = "";
            soundCasting = "";
            soundEnd = "";
        }
    }

    [System.Serializable]
    public class Buff
    {

        // Basics
        public string id { get; set; }
        public string name { get; set; }
        public string description { get; set; }

        // Attributes
        public int duration { get; set; }
		public string type { get; set; }
		public string ability { get; set; }
		[XmlArrayItem("channel")]
		public float[] color { get; set; }

        // Flags
        public bool isHelpful { get; set; }
        public bool isHarmful { get; set; }
        public bool isRemovable { get; set; }
        public bool isPersistent { get; set; }

        // Effects
        public int movementRange { get; set; }
        public double movementRangeMult { get; set; }
        public int movementSpeed { get; set; }
        public double movementSpeedMult { get; set; }
        public int health { get; set; }
        public double healthMult { get; set; }
        public int armor { get; set; }
        public double armorMult { get; set; }
        public int shield { get; set; }
        public double shieldMult { get; set; }
        public int damageBonus { get; set; }
        public double damageMult { get; set; }
        public int healBonus { get; set; }
        public double healMult { get; set; }
        public bool isMovable { get; set; }
        public bool isInvincible { get; set; }
        public bool isVisible { get; set; }
        public bool isDisabled { get; set; }

        // Resources
        public string icon { get; set; }

        public Buff()
        {
            id = "generic";
            name = "generic";
            description = "";

            // Attributes
            duration = 0;
			type = "Buff";
			ability = "";
			color = new float[] {0.0f, 0.0f, 0.0f, 1.0f};

            // Flags
            isHelpful = false;
            isHarmful = false;
            isRemovable = false;
            isPersistent = false;

            // Effects
            movementRange = 0;
            movementRangeMult = 0;
            movementSpeed = 0;
            movementSpeedMult = 0;
            health = 0;
            healthMult = 0;
            armor = 0;
            armorMult = 0;
            shield = 0;
            shieldMult = 0;
            damageBonus = 0;
            damageMult = 0;
            healBonus = 0;
            healMult = 0;
            isMovable = true;
            isInvincible = false;
            isVisible = false;
            isDisabled = false;

            // Resources
            icon = "";
        }
    }
}
