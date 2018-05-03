using UnityEngine;
using System.Collections;
using Umbra.Data;
using UnityEngine.UI;
using System.IO;
using System.Linq;

namespace Umbra.Scenes.LoadMenu
{
    public class SaveSlotInfo : MonoBehaviour
    {
        public string fileName;
        public string playerName;
        public System.DateTime lastWrittenTo;

        public void onClick()
        {
            GameObject.Find("HelperInfoLbl").GetComponent<Text>().text = "Loading, please wait...";
            GameStateManager.Instance.currentSavePath = "/" + fileName;
            //GameStateManager.Instance.LoadGame();
            GameStateManager.Instance.SaveGame(); //to make it the latest save file

            //set the settings to a possible updated settings
            if (SettingsMenu.SettingsMenuScript.hasBeenUpdated)
            {
                GameStateManager.Instance.settings = SettingsMenu.SettingsMenuScript.updatedSettings;
                GameStateManager.Instance.SaveGame();
            }
            else
            {
                SettingsMenu.SettingsMenuScript.ApplySettings(GameStateManager.Instance.settings);
            }

            //last known location?
            GameStateManager.Instance.PushScene(GameScene.StarMap);
        }
    }
}