using UnityEngine;
using System.Collections;
using Umbra.Managers;

using UnityEngine.UI;
using Umbra.Models;
using Umbra.Data;
using Umbra.Scenes.ExplorationMap;

namespace Umbra.Utilities {

	public class TileScript : MonoBehaviour
	{
        
		public Node tile; // tile containing this script
		public TileManager tm;
		public MapModel mapModel;

		//private Text _textArea; // remove this later on

		void Awake() {
			tm = TileManager.Instance;
			mapModel = new MapModel ();
		}

		// Use this for initialization
	    void Start()
	    {
			// Remove this later
			//_textArea = GameObject.Find ("FeedbackBoxText").GetComponent<Text>();
	    }

		void Update() {

			/*
			_textArea.text = "Selected: " +
				((tm.playerSelectedTile != null && tm.playerSelectedTile.actor != null) ? tm.playerSelectedTile.actor.name + " (Side: " + tm.playerSelectedTile.actor.side +  ")" : "empty") +
				"; targeted: " +
				((tm.playerTargetedTile != null && tm.playerTargetedTile.actor != null) ? tm.playerTargetedTile.actor.name + " (Side: " + tm.playerTargetedTile.actor.side + ")": "empty");
			*/

		}

	    void OnMouseUp()
	    {
            
			// Outpost handling for exploration map
			if (!mapModel.getCurrentMap ().isBattleMap) {

				// if player has clicked Outpost button, assign the outpost to the clicked tile position if it doesn't contain a object
				if (mapModel.getCurrentMap ().isOutpostFriendly) {

					if (ExplorationManager.Instance.waitingOnOutpost) {
						if (tile.isPassable && tile.actor == null) {
							OutpostModel outpostM = new OutpostModel ();
							Outpost outpost = outpostM.getOutpostByID (mapModel.getCurrentMap ().id);
							outpost.x = tile.x;
							outpost.y = tile.y;
							tile.isPassable = false;
							Quaternion rot = new Quaternion (-0.4331436f, 0.07942633f, -0.2500829f, 0.8622857f);
							Vector3 pos = tm.GridToWorldspace (tile.x, tile.y);
							pos.z = -1f;
							pos.y -= 0.6f;
							pos.x -= 0.2f;
							GameObject obj = (GameObject)Instantiate (Resources.Load ("OutpostTemplate"), pos, rot);
							tile.content = obj;
							tm.setNodeObject (obj, tile.x, tile.y);
							obj.GetComponent<SpriteRenderer> ().sprite = TileManager.Instance.objects ["outpost"];
							obj.transform.parent = TileManager.Instance.transform;
							TileManager.Instance.liveObjects.Add (obj);
							outpost.hasBeenPlaced = true;
							GameStateManager.Instance.SaveGame ();
							ExplorationManager.Instance.waitingOnOutpost = false;
							Debug.Log ("-" + mapModel.getCurrentMap ().outpostRef.id + "-");
						} else {
							Exploration explorationScript = GameObject.Find ("Exploration").GetComponent<Exploration> ();
							explorationScript.outpostPlacementLbl.GetComponent<Text> ().color = Color.red;
							explorationScript.outpostPlacementLbl.GetComponent<Text> ().text = "Already occupied, try another...";
						}
					}

				}

				//if the object on the tile is an outpost
				if (tile != null) {
					if (tile.actor == null && tile.content != null && tile.content.GetComponent<SpriteRenderer> ().sprite == TileManager.Instance.objects ["outpost"]) {
						GameStateManager.Instance.PushScene (GameScene.OutpostMenu);
					}
				}

			}

			// Select a tile
			if (tm.playerSelectedTile == null) {

				// select tile only if there's an actor in it
				if (tile != null && tile.actor != null) {
					tm.playerSelectedTile = tile;
					// additionally, if the selected actor is an enemy, target it as well
					if (tile.actor.isHostile)
						tm.playerTargetedTile = tile;
				}
			
			// If a tile is selected, set a target instead, but only if there is no target
			} else {

				if (tm.playerTargetedTile == null) {
					tm.playerTargetedTile = tile;
				}

			}

		}

	}
}