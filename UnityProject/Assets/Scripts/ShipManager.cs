using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Umbra.Data;
using Umbra.Scenes.UpgradeShip;
using Umbra.Models;

namespace Umbra.Managers {
	public class ShipManager : Singleton<ShipManager>
	{
		private PlayerModel _playerModel;

	    protected ShipManager() { }

		void Start() {
			_playerModel = new PlayerModel ();
		}

	    //stores module names in array for now
	    public static string[] moduleName = new string[18]
	    {
	        "Armory","Barracks","Academy","Garage","Factory Bay","Engineering Bay","Docking Bays","Shipyard Bay","Aerospace Bay","Research Bay","Construction Bay","Living Quarters","Tourism Center","Botanical Pipe","Sick Bay","Water Processing","Processing Plant","Waste Processing"
	    };

	    //stores module descriptions in array for now
	    public static string[] moduleDesc = new string[18]
	    {
	        "Allows weapon and armor upgrades for infantry. Upgrade permits the leveling of units.","Required to generate infantry. Upgrade increases recruitment speed.","Required to unlock more infantry. Upgrade unlocks soldiers.", "Required to generate ground vehicles. Upgrade increases recruitment speed.", "Required to build ground vehicles. Upgrade unlocks more vehicles.","Allows weapon and armor upgrades for ground vehicles. Upgrade permits leveling up of vehicles.","Required to generate air/space vehicles. Upgrade increases recruitment speed.","Required to build air/space vehicles. Upgrade unlocks more air/space vehicles.","Allows weapon and armor upgrades for air/space vehicles. Upgrade increases leveling of air/space vehicles.","Unlocks the most powerful units of all types in the fleet.","Used to create outposts. Upgrade results in more complete deployment and more construction options.","Randomly selects a companion for recruitment before a mission. Upgrade grants a higher recruit level, but cannot exceed yours.","Slowly generates faction-specific currency. Upgrade increases generation rate.","Generates food. Upgrade increases generation rate.","Generates medical supplements. Upgrade increases generation rate.","Generates water. Upgrade increases generation rate.","Generates fuel, minerals, and gas. Upgrade increases generation rate.","Generates fuel and water. Upgrade increases generation rate."
	    };

	    public static UpgradeShip.moduleData findDataByName(string name)
	    {
	        for (int i = 1; i < 19; i++)
	        {
				if (GameStateManager.Instance.gameState.spaceshipSectionDictionary[i].name.Equals(name))
	            {
					return new UpgradeShip.moduleData(GameStateManager.Instance.gameState.spaceshipSectionDictionary[i].desc, GameStateManager.Instance.gameState.spaceshipSectionDictionary[i].levelCurrent, GameStateManager.Instance.gameState.spaceshipSectionDictionary[i].nextRecReq);
	            }
	        }
	        return new UpgradeShip.moduleData("A module.", -1, new int[6]);
	    }

	    public static SpaceshipSection getModuleByName(string name)
	    {
	        for (int i = 1; i < 19; i++)
	        {
				if (GameStateManager.Instance.gameState.spaceshipSectionDictionary[i].name.Equals(name))
	            {
					return GameStateManager.Instance.gameState.spaceshipSectionDictionary[i];
	            }
	        }
	        return null;
	    }
	    public void upgrade(SpaceshipSection module)
	    {
			Player player = _playerModel.data;
	        // minerals, gasses, fuel, water, food, meds, f1c, f2c, f3c, f4c, f5c index
			player.resourcesMinerals = (player.resourcesMinerals- module.nextRecReq[0]);
			player.resourcesGas = (player.resourcesGas - module.nextRecReq[1]);
			player.resourcesFuel = (player.resourcesFuel - module.nextRecReq[2]);
			player.resourcesWater = (player.resourcesWater - module.nextRecReq[3]);
			player.resourcesFood = (player.resourcesFood - module.nextRecReq[4]);
			player.resourcesMeds = (player.resourcesMeds - module.nextRecReq[5]);

	        module.levelCurrent++;
	        //double requirements
	        for (int i = 0; i < 6; i++)
	        {
	            module.nextRecReq[i] *= 2;
	        }
	        GameStateManager.Instance.SaveGame();
	    }

	}
}