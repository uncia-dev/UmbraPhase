using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using Umbra.Data;
using Umbra.Managers;
using Umbra.Models;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Umbra.Scenes.RepMenu
{
    public class RepMenuScript : MonoBehaviour
    {
        
		//public GameObject[] progressBars;
		//private PlayerModel _playerModel;
		private FactionModel _factionModel;
		private List<int> _playerReputations;

        void Start()
        {
			//GameStateManager.Instance.LoadGame();
			//_playerModel = new PlayerModel ();
			_factionModel = new FactionModel ();
			_playerReputations = _factionModel.data[1].reputations;
			//progressBars = GameObject.FindGameObjectsWithTag("RepProgressBars");
            updateBars();
        }

        public void loadCharMenu()
        {
            GameStateManager.Instance.PopScene();
        }

		public void setReputationlabel(string lbl, string txt) {
			
			GameObject.Find (lbl).GetComponent<Text> ().text = txt;
		}

		public void setReputationColor(string bar, int rep) {

			Color c;

			if (rep == -1)
				c = new Color (0.26f, 0.44f, 0.76f);
			else if (rep >= 0 && rep < 25)
				c = new Color (1.00f, 0.0f, 0.0f);
			else if (rep >= 25 && rep < 45)
				c = new Color (1.00f, 0.3f, 0.0f);
			else if (rep >= 45 && rep <= 55)
				c = new Color (0.5f, 0.5f, 0.5f);
			else if (rep > 55 && rep < 75)
				c = new Color (0.1f, 0.5f, 0.1f);			
			else if (rep >= 75)
				c = new Color (0.1f, 0.5f, 0.1f);
			else
				c = new Color (1.0f, 1.0f, 1.0f);

			GameObject.Find (bar).GetComponent<Image> ().color = c;

		}

        public void updateBars()
        {
            //Player player = _playerModel.data;
			List<Faction> factions = new FactionModel().data;

			int i = 0;
			for (int j=1; j <= 6; j++) {
				string lbl =  factions[j].name + ((_playerReputations[j] == -1) ? "" : " (" + _playerReputations[j].ToString() + "/100)");
				setReputationlabel ("lblFaction" + (i+1).ToString (), lbl);
				setReputationColor ("progressBar" + (j).ToString (), _playerReputations[j]);
				i++;
			}

        }
    }
}