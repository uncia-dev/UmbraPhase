using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Umbra.Data;
using Umbra.Managers;
using Umbra.Models;

namespace Umbra.Scenes.UpgradeShip {
	public class UpgradeShip : MonoBehaviour {

	    //saves two function calls to retrieve these from DataManager
	    public struct moduleData
	    {
	        public string desc; public int level; public int[] recReq;
	        public moduleData(string d, int l, int[] r) { desc = d; level = l; recReq = r; }
	    };
	    public GameObject hoveringObj;
	    public GameObject resourceLbl;
	    public static GameObject lastClickedModule;
	    public GameObject moduleTitleLbl;
	    public GameObject levelLbl;
	    public GameObject informationLbl;
	    public GameObject[] resourceLabels;
	    public SpaceshipSection selectedModule;

		private PlayerModel _playerModel;
        private StarbaseModel _shipModel;

        public void upgrade(SpaceshipSection module)
        {
            Player player = _playerModel.data;

            player.resourcesMinerals = (player.resourcesMinerals - module.nextRecReq[0]);
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

	    public void loadShipMenu()
	    {
            GameStateManager.Instance.PopScene();
	    }

	    public void onClickUpgrade()
	    {
	        if (canUpgrade())
	        {
				upgrade(selectedModule);
	            updateLabels(selectedModule.name);
	        }
	    }

	    public void onClickModule(GameObject module)
	    {
	            string moduleName = module.GetComponentInChildren<Text>().text;
	            informationLbl.GetComponent<Text>().color = new Color(255, 255, 255);

	            selectedModule = _shipModel.getModuleByName(moduleName);
	            updateLabelColors();
	            UpgradeShip.lastClickedModule = module;
	            moduleTitleLbl.GetComponent<Text>().text = moduleName;
	            moduleData md = _shipModel.findDataByName(moduleName);
	            informationLbl.GetComponent<Text>().text = md.desc;
	            levelLbl.GetComponent<Text>().text = md.level.ToString();
	            for (int i = 0; i < 6; i++)
	            {
	                resourceLabels[i].GetComponent<Text>().text = md.recReq[i].ToString();
	            }
	    }

	    public void onHover(GameObject obj)
	    {
	        hoveringObj = obj;

            resourceLbl.SetActive(true);
            resourceLbl.GetComponent<Text>().text = obj.name.Substring(0, obj.name.Length - 3);
            resourceLbl.transform.position = new Vector3(hoveringObj.transform.position.x - 45, hoveringObj.transform.position.y, 0);
	    }

	    public void onExit()
	    {
	        resourceLbl.SetActive(false);
	    }

	    public bool canUpgrade()
	    {
			Player player = _playerModel.data;

            bool upgrade = true;

	        if (selectedModule.levelCurrent >= 3)
	        {
	            informationLbl.GetComponent<Text>().text = "Maximum level already reached.";
	            informationLbl.GetComponent<Text>().color = new Color(255, 0, 0);
	            return false;
	        }

			if (!(player.resourcesMinerals >= selectedModule.nextRecReq[0]))
	        {
                upgrade = false;
	        }

			if (!(player.resourcesGas >= selectedModule.nextRecReq[1]))
            {
                upgrade = false;
            }
			if (!(player.resourcesFuel >= selectedModule.nextRecReq[2]))
            {
                upgrade = false;
            }
			if (!(player.resourcesWater >= selectedModule.nextRecReq[3]))
            {
                upgrade = false;
            }
			if (!(player.resourcesFood >= selectedModule.nextRecReq[4]))
            {
                upgrade = false;
            }
			if (!(player.resourcesMeds >= selectedModule.nextRecReq[5]))
            {
                upgrade = false;
            }

            if (!upgrade)
            {
                informationLbl.GetComponent<Text>().text = "Not enough resources.";
                informationLbl.GetComponent<Text>().color = new Color(255, 0, 0);
            }

            return upgrade;
	    }

        public void updateLabelColors()
        {
			Player player = _playerModel.data;

			if (!(player.resourcesMinerals >= selectedModule.nextRecReq[0]))
            {
                resourceLabels[0].GetComponent<Text>().color = new Color(255, 0, 0);
            }
            else
            {
                resourceLabels[0].GetComponent<Text>().color = new Color(255, 255, 255);

            }

			if (!(player.resourcesGas >= selectedModule.nextRecReq[1]))
            {
                resourceLabels[1].GetComponent<Text>().color = new Color(255, 0, 0);
            }
            else
            {
                resourceLabels[1].GetComponent<Text>().color = new Color(255, 255, 255);
            }

			if (!(player.resourcesFuel >= selectedModule.nextRecReq[2]))
            {
                resourceLabels[2].GetComponent<Text>().color = new Color(255, 0, 0);
            }
            else
            {
                resourceLabels[2].GetComponent<Text>().color = new Color(255, 255, 255);
            }

			if (!(player.resourcesWater >= selectedModule.nextRecReq[3]))
            {
                resourceLabels[3].GetComponent<Text>().color = new Color(255, 0, 0);
            }
            else
            {
                resourceLabels[3].GetComponent<Text>().color = new Color(255, 255, 255);
            }

			if (!(player.resourcesFood >= selectedModule.nextRecReq[4]))
            {
                resourceLabels[4].GetComponent<Text>().color = new Color(255, 0, 0);
            }
            else
            {
                resourceLabels[4].GetComponent<Text>().color = new Color(255, 255, 255);
            }

			if (!(player.resourcesMeds >= selectedModule.nextRecReq[5]))
            {
                resourceLabels[5].GetComponent<Text>().color = new Color(255, 0, 0);
            }
            else
            {
                resourceLabels[5].GetComponent<Text>().color = new Color(255, 255, 255);
            }
        }

		void Start () {
	        //GameStateManager.Instance.LoadGame();

			_playerModel = new PlayerModel ();
            _shipModel = new StarbaseModel();

	        resourceLabels = GameObject.FindGameObjectsWithTag("ResourceCounts");
	        moduleTitleLbl = GameObject.Find("SelectedModuleLbl");
	        levelLbl = GameObject.Find("LevelLbl");
	        informationLbl = GameObject.Find("InformationLbl");
	        resourceLbl = GameObject.Find("ResourceCountLbl");
	        resourceLbl.SetActive(false);

	        //load Armory data to start
	        moduleTitleLbl.GetComponent<Text>().text = "Armory";
	        moduleData md = _shipModel.findDataByName("Armory");
	        informationLbl.GetComponent<Text>().text = md.desc;
	        levelLbl.GetComponent<Text>().text = md.level.ToString();
	        for (int i = 0; i < 6; i++)
	        {
	            resourceLabels[i].GetComponent<Text>().text = md.recReq[i].ToString();
	        }
	        selectedModule = _shipModel.getModuleByName("Armory");
	        updateLabelColors();
	        UpgradeShip.lastClickedModule = GameObject.Find("ArmoryBtn");
		}

	    public void updateLabels(string name)
	    {
	        moduleTitleLbl.GetComponent<Text>().text = name;
	        updateLabelColors();
	        moduleData md = ShipManager.findDataByName(name);
	        levelLbl.GetComponent<Text>().text = md.level.ToString();
	        for (int i = 0; i < 6; i++)
	        {
	            resourceLabels[i].GetComponent<Text>().text = md.recReq[i].ToString();
	        }
	    }
	}
}
