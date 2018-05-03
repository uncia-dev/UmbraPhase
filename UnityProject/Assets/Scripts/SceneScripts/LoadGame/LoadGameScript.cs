using UnityEngine;
using System.Collections;
using Umbra.Data;
using System.IO;
using UnityEngine.UI;
using Umbra.Models;

namespace Umbra.Scenes.LoadMenu
{
    public class LoadGameScript : MonoBehaviour
    {

        public GameObject saveSlotPrefab;
        public GameObject saveContent;
        public GameObject helperLabel;

        void Start()
        {
            DirectoryInfo directory = new DirectoryInfo(Application.persistentDataPath);
            FileInfo[] saveFiles = directory.GetFiles();
            GameObject newSlot = null;

            foreach (var file in saveFiles)
            {
                GameStateManager.Instance.currentSavePath = "/" + file.Name;
                GameStateManager.Instance.LoadGame();
                PlayerModel p = new PlayerModel();

                newSlot = Instantiate(saveSlotPrefab, Vector3.zero, Quaternion.identity) as GameObject;
				newSlot.GetComponentInChildren<Text>().text = 
					p.data.name + " (Level " + p.data.characterLevel + " " + p.data.className + 
					") - " + file.Name + " - " + file.LastWriteTime.ToShortDateString() + " " + 
					file.LastWriteTime.ToShortTimeString();
                newSlot.GetComponent<SaveSlotInfo>().fileName = file.Name;
				newSlot.GetComponent<SaveSlotInfo>().playerName = p.data.name;
                newSlot.GetComponent<SaveSlotInfo>().lastWrittenTo = file.LastWriteTime;
                newSlot.transform.parent = saveContent.transform;
            }
        }

        public void ReturnButton()
        {
            GameStateManager.Instance.PushScene(GameScene.MainMenu);
        }
    }
}