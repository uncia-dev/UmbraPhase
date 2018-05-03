using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Umbra;
using Umbra.Data;
using Umbra.Scenes.UpgradeShip;


namespace Umbra.Models
{
    public class StarbaseModel : Model<Spaceship>
    {
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

        public Dictionary<int, SpaceshipSection> spaceShipSections;

        public StarbaseModel()
        {
            data = GameStateManager.Instance.gameState.starbase;
            spaceShipSections = GameStateManager.Instance.gameState.spaceshipSectionDictionary;
        }

        public UpgradeShip.moduleData findDataByName(string name)
        {
            foreach (var section in spaceShipSections.Values)
            {
                if (section.name.Equals(name))
                {
                    return new UpgradeShip.moduleData(section.desc, section.levelCurrent, section.nextRecReq);
                }
            }
            return new UpgradeShip.moduleData("A module.", -1, new int[6]);
        }

        public SpaceshipSection getModuleByName(string name)
        {
            foreach (var section in spaceShipSections.Values)
            {
                if (section.name.Equals(name))
                {
                    return section;
                }
            }
            return null;
        }
    }
}