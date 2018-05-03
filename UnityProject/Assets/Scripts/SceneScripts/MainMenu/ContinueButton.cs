using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Umbra.Managers;
using Umbra.Data;
using System.IO;
using Umbra.Models;
using System.Linq;


namespace Umbra.Scenes.MainMenu {
	public class ContinueButton : MonoBehaviour {

	    bool active;
	    bool set = false;

	    float alpha = 0.4f;
	    float sizeBoost = 25f;

		private PlayerModel _playerModel;

		void Start() {

            if (GameStateManager.Instance.gameState != null)
            {
                active = true;

                SettingsMenu.SettingsMenuScript.ApplySettings(GameStateManager.Instance.settings);
                SettingsMenu.SettingsMenuScript.updatedSettings = GameStateManager.Instance.settings;
                //flag this to update any game that may be loaded instead of loading its settings (this overwrites)
                SettingsMenu.SettingsMenuScript.hasBeenUpdated = true;
                _playerModel = new PlayerModel();
            }

            
		}

		public void Pressed() {
			GameStateManager.Instance.PushScene (GameScene.StarMap);  //Hardcoded to starmap for now
	    }
		// Update is called once per frame
		void Update () {
	        GetComponent<Button>().interactable = active;
	        if (active && set == false)
	        {
				Player player = _playerModel.data;
	            Rect r = GetComponentInChildren<RectTransform>().rect;
	            RectTransform rt = GetComponentInChildren<RectTransform>();
	            Text t = GetComponentInChildren<Text>();
	            Color c = GetComponentInChildren<Image>().color;
	            rt.sizeDelta = new Vector2(r.width, r.height + sizeBoost);
	            rt.position = new Vector3(rt.position.x, rt.position.y + sizeBoost/2, rt.position.z);
	            GetComponentInChildren<Image>().color = new Color(c.r, c.g, c.b, alpha);
				t.text = "Continue\n<size=10>" + player.name + 
					"\n(Level " + player.characterLevel + " " + player.className + ")" + "</size>";
	            set = true;
	        }
		}
	}
}