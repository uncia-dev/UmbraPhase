using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Umbra.Data;
using Umbra.Managers;
using Umbra.Models;

namespace Umbra.Scenes.ShipMenu {

	public class ShipMenu : MonoBehaviour {
        
	    public GameObject resourceLbl;
	    public GameObject hoveringObj;
	    public bool disableFollow;
	    public GameObject[] resourceLabels;
        public GameObject popCountLbl;
        public GameObject shipNameLabel;

		private GameState _gameState;
		private PlayerModel _playerModel;
		private FactionModel _factionModel;

        public void loadRoster()
        {
            GameStateManager.Instance.PushScene(GameScene.RosterMenu);
        }

	    public void loadUpgradeShipMenu()
	    {
			GameStateManager.Instance.PushScene(GameScene.ShipUpgradeMenu);
	    }

        public void loadCreateUnit()
        {
			GameStateManager.Instance.PushScene(GameScene.CreateUnitMenu);
        }

        public void closeBtn()
        {
            GameStateManager.Instance.PopScene();
        }

	    public void onHover(GameObject obj)
	    {
	        disableFollow = false;
	        hoveringObj = obj;

	        resourceLbl.SetActive(true);
            resourceLbl.GetComponent<Text>().text = obj.name.Substring(0, obj.name.Length - 3);
            resourceLbl.transform.position = new Vector3(hoveringObj.transform.position.x - 45, hoveringObj.transform.position.y, 0);
	    }

	    public void onExit()
	    {
	         resourceLbl.SetActive(false);
	    }

	    public void updateLabelData()
	    {
			Player player = _playerModel.data;
			popCountLbl.GetComponent<Text>().text = player.resourcesPeople.ToString();
			resourceLabels[0].GetComponent<Text>().text = player.resourcesMinerals.ToString();
			resourceLabels[1].GetComponent<Text>().text = player.resourcesGas.ToString();
			resourceLabels[2].GetComponent<Text>().text = player.resourcesFuel.ToString();
			resourceLabels[3].GetComponent<Text>().text = player.resourcesWater.ToString();
			resourceLabels[4].GetComponent<Text>().text = player.resourcesFood.ToString();
			resourceLabels[5].GetComponent<Text>().text = player.resourcesMeds.ToString();
			resourceLabels[6].GetComponent<Text>().text = player.resourcesFaction1.ToString();
			resourceLabels[7].GetComponent<Text>().text = player.resourcesFaction2.ToString();
			resourceLabels[8].GetComponent<Text>().text = player.resourcesFaction3.ToString();
			resourceLabels[9].GetComponent<Text>().text = player.resourcesFaction4.ToString();
			resourceLabels[10].GetComponent<Text>().text = player.resourcesFaction5.ToString();

			for (int i = 2; i < 7; i++) {
				Color c = new Color (_factionModel.data[i].color [0], _factionModel.data[i].color [1], _factionModel.data[i].color [2]);
				GameObject.Find ("lblCurrencyFaction" + i.ToString()).GetComponent<Text> ().text = _factionModel.data[i].currency;
				GameObject.Find ("lblCurrencyFaction" + i.ToString ()).GetComponent<Text> ().color = c;
			}

		}

		void Start () {
	        //GameStateManager.Instance.LoadGame();
            _gameState = GameStateManager.Instance.gameState;
            _playerModel = new PlayerModel();
			_factionModel = new FactionModel ();

	        resourceLabels = GameObject.FindGameObjectsWithTag("ResourceCounts");
	        disableFollow = false;
            popCountLbl = GameObject.Find("PopCountLbl");
	        resourceLbl = GameObject.Find("ResourceCountLbl");
	        resourceLbl.SetActive(false);

            StarbaseModel shipModel = new StarbaseModel();
            shipNameLabel.GetComponent<Text>().text = shipModel.data.name;

	        updateLabelData();
		}
	}
}
