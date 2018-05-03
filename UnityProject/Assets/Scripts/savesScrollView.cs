using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Umbra.Managers;
using Umbra.Data;


namespace Umbra.Scenes.LoadMenu {
	public class savesScrollView : MonoBehaviour {

//	    List<Data.Game> savedGames;
	    float yPos = 100;

		// Use this for initialization
		void Start () {
//	        savedGames = GameState.savedGames;
//	        savedGames.Reverse();
//
//	        foreach (Game i in savedGames)
//	        {
//	            Vector3 pos = new Vector3(-300, yPos, 0);
//	            pos = Camera.main.WorldToScreenPoint(pos);
//	            GameObject saveSlot = (GameObject)Instantiate(Resources.Load("saveSlot"), pos, Quaternion.identity);
//	            saveSlot.GetComponent<RectTransform>().position = pos;
//	            saveSlot.GetComponentInChildren<Text>().text = i.database.player.name + " lvl:" + i.database.player.level;
//	            //saveSlot.transform.parent = transform;
//	            saveSlot.transform.SetParent(transform);
//	            yPos -= saveSlot.GetComponent<RectTransform>().rect.height + 5;
//	        }

		}
	}
}
