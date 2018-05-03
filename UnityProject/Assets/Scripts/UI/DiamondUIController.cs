using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Umbra.Data;
using Umbra.Controller;
using Umbra.Managers;
using Umbra.Models;

namespace Umbra.UI {
	public class DiamondUIController : MonoBehaviour {

		public MapButtons currentMap;

		private GameObject _activeSubmenu;
		private Color _defaultColor;
		private GameObject _toggleMenu;
		private Color _highlightedColor;
		private GameStateManager _manager;

//		private float _starMapZoom = 60f;
//		private float _wormholeMapZoom = 90f;
		private Vector3 _starMapPos;
		private Vector3 _wormholeMapPos;

		private BattleManager _battleManager;
		private ExplorationManager _explorationManager;

		void Awake() {

			_manager = GameStateManager.Instance;

			Button b = null;

			if (currentMap.Equals (MapButtons.Starmap)) {
				_toggleMenu = transform.Find ("ButtonAnchor/ToggleableMenus").gameObject;
				b = transform.Find ("ButtonAnchor/MapSelection/StarmapButton").GetComponent<Button> ();
			} else if (currentMap.Equals (MapButtons.ExplorationMap)) {
				b = transform.Find ("ButtonAnchor/MapSelection/StarmapButton").GetComponent<Button> ();
				_explorationManager = ExplorationManager.Instance;
			} else if (currentMap.Equals (MapButtons.BattleMap)) {
				b = transform.Find ("ButtonAnchor/DefaultBattle/FleeButton").GetComponent<Button> ();
				_battleManager = BattleManager.Instance;
			} else {
				// crash the game because there's something smelly here
			}

			_defaultColor = b.colors.normalColor;
			_highlightedColor = b.colors.highlightedColor;

		}

		public GameObject SetActiveSubmenu(string MenuName) {
			_activeSubmenu = null;
			_toggleMenu.SetActive (true);
			foreach (Transform menu in _toggleMenu.transform) {
				if (menu.name.Equals(MenuName)) {
					menu.gameObject.SetActive(true);
					_activeSubmenu = menu.gameObject;
				} else {
					menu.gameObject.SetActive(false);
				}
			}

			return _activeSubmenu;
		}

		public GameObject GetActiveMenu() {
			return _activeSubmenu;
		}

		public void OpenWormholeMap() {

			if (currentMap.Equals (MapButtons.ExplorationMap)) {

				// code here will need to 'remember' the last opened star map,
				// and whether or not the wormhole map is visible
				_manager.PushScene (GameScene.StarMap);

			} else {

				Camera.main.cullingMask &= ~(1 << LayerMask.NameToLayer("Starmap") | LayerMask.NameToLayer("LocalMap"));
				Camera.main.cullingMask |= (1 << LayerMask.NameToLayer ("WormholeMap"));
				SetActiveButton ("WormholemapButton");

				if (currentMap.Equals (MapButtons.Starmap)) {
					_starMapPos = Camera.main.transform.position;
					Camera.main.transform.position = _wormholeMapPos;
					//				_starMapZoom = Camera.main.fieldOfView;
					//				Camera.main.fieldOfView = _wormholeMapZoom;
				}

			currentMap = MapButtons.WormholeMap;

			GameObject.Find ("WormholeMapOverlay").GetComponent<WormholeController> ().OpenMap();

			}

		}

		public void OpenStarmap() {


			if (currentMap.Equals (MapButtons.ExplorationMap)) {
					_manager.PushScene (GameScene.StarMap);
			} else {

				Camera.main.cullingMask &= ~(1 << LayerMask.NameToLayer ("WormholeMap") | LayerMask.NameToLayer ("LocalMap"));
				Camera.main.cullingMask |= (1 << LayerMask.NameToLayer ("Starmap"));
				SetActiveButton ("StarmapButton");

				if (currentMap.Equals (MapButtons.WormholeMap)) {
					_wormholeMapPos = Camera.main.transform.position;
					Camera.main.transform.position = _starMapPos;
					// _wormholeMapZoom = Camera.main.fieldOfView;
					// Camera.main.fieldOfView = _starMapZoom;
				}

				currentMap = MapButtons.Starmap;

			}
		}

        public void OpenShipMenu() {
			_manager.PushScene (GameScene.ShipMenu);
		}

		public void OpenMainMenu() {
			_manager.PushScene (GameScene.MainMenu);
		}

		public void OpenOutpostMenu() {
            MapModel mapModel = new MapModel();
            Map map = mapModel.getCurrentMap();
            if (map != null && !map.outpostRef.hasBeenPlaced) // if outpost hasn't been assigned a place on a tile
            {
                _explorationManager.waitingOnOutpost = !_explorationManager.waitingOnOutpost;
            }
            else 
            {
                _manager.PushScene(GameScene.OutpostMenu);
            }
		}

		public void OpenCharacterMenu() {
			_manager.PushScene (GameScene.CharacterMenu);
		}

		public void OpenUnitsMenu() {
			_manager.PushScene (GameScene.CreateUnitMenu);
		}

		public void OpenObjectivesMenu() {
			_explorationManager.click (ExplorationButtons.cmdObjectives);
		}

		public void OpenLocalMap() {
			
			// this should load the map the player was already on
			// or if the player did not land yet, gray out the button

			MapModel mapModel = new MapModel();
			if (mapModel.getCurrentMap () != null) {
				_manager.PushScene (GameScene.ExplorationMap);
				SetActiveButton ("LocalmapButton");
			} else {
				Debug.Log ("No local map to load!");
			}

        }

		public void EnableLandButton(bool visitable) {
			var landbtn = transform.Find ("ButtonAnchor/ToggleableMenus/PlanetSelected/Land").GetComponent<Button> ();
			landbtn.interactable = visitable;
		}

		/* Exploration Map Context Menu Commands */
		public void ExplorationClickInspect() {
			_explorationManager.click (ExplorationButtons.ctxInspect);
		}

		public void ExplorationClickRecruitAttack() {
			_explorationManager.click (ExplorationButtons.ctxRecruitAttack);
		}

		public void ExplorationClickTalkUse() {
			_explorationManager.click (ExplorationButtons.ctxTalkUse);
		}

		/* Battle Map Commands */
		public void BattleCommandFlee() {
			// This breaks the Battle Map for a moment; do not call ExplorationManager.Instance in Battle Maps, as it'll run the script
			//ExplorationManager.Instance.onExplorationMap = true; //flag that it shouldn't render things like outposts or red actors (these are test actors, right?)
			_battleManager.click (BattleButtons.cmdFlee);
		}

		public void BattleCommandAutoplay() {
			_battleManager.click (BattleButtons.cmdAutoplay);
		}

		public void BattleCommandReturn() {
			_battleManager.click (BattleButtons.cmdReturn);
			// An exploration map will implictly call the ExplorationManager anyway
			//GameStateManager.Instance.PushScene(GameScene.ExplorationMap, true);
		}

		/* Battle Map Context Menu Commands */
		public void BattleClickInspect() {
			_battleManager.click (BattleButtons.ctxInspect);
		}

		public void BattleClickAbility1() {
			_battleManager.click (BattleButtons.ctxAbility1);
		}

		public void BattleClickAbility2() {
			_battleManager.click (BattleButtons.ctxAbility2);
		}

		public void BattleClickAbility3() {
			_battleManager.click (BattleButtons.ctxAbility3);
		}

		public void BattleClickAbility4() {
			_battleManager.click (BattleButtons.ctxAbility4);
		}

		public void BattleClickAbility5() {
			_battleManager.click (BattleButtons.ctxAbility5);
		}

		public void BattleClickAbility6() {
			_battleManager.click (BattleButtons.ctxAbility6);
		}

		public void BattleClickItem1() {
			_battleManager.click (BattleButtons.ctxItem1);
		}

		public void BattleClickItem2() {
			_battleManager.click (BattleButtons.ctxItem2);
		}

		void toggleCanvasGroup(CanvasGroup group, bool state) {
			if (group != null) {
				group.alpha = state == true ? 1 : 0;
				group.interactable = state;
			}
		}

		/*
		 * Set visibility of the Inspect Window in the battle map
		 */
		public void setInspectWindow(bool state) {
			toggleCanvasGroup (
				GameObject.Find("InspectWindow").GetComponent<CanvasGroup>(),
				state
			);
		}

		/*
		 * Set visibility of the Game Over window
		 */
		public bool setGameOverWindow(bool state) {
			toggleCanvasGroup (
				GameObject.Find("GameOver").GetComponent<CanvasGroup>(),
				state
			);
			return true;
		}

		/*
		 * Set visibility of the Unit plate in the battle map
		 */
		public bool setUnitPlate(bool state) {
			toggleCanvasGroup (
				GameObject.Find("BottomBar/SelectedUnit").GetComponent<CanvasGroup>(),
				state
			);
			return true;
		}

		/*
		 * Set visibility of context menu to state
		 */
		public bool setContextMenuExploration(
			bool state, bool showExplorationComponents=false
		) {

			toggleCanvasGroup (
				GameObject.Find ("ContextMenu").GetComponent<CanvasGroup> (), 
				state
			);

			toggleCanvasGroup (
				GameObject.Find("ContextMenu/Exploration").GetComponent<CanvasGroup> (),
				showExplorationComponents
			);

			return true;

		}

		/*
		 * Set visibility of context menu to state
		 */
		public bool setContextMenuBattle(
			bool state, bool isCharacter=false, bool isPlayerCharacter=false, bool showAbilities=true
		) {

			toggleCanvasGroup (
				GameObject.Find ("ContextMenu").GetComponent<CanvasGroup> (), 
				state
			);

			toggleCanvasGroup (
				GameObject.Find ("ContextMenu/Abilities").GetComponent<CanvasGroup> (),
				showAbilities
			);

			toggleCanvasGroup (
				GameObject.Find ("ContextMenu/Abilities/CharacterUnitButtons").GetComponent<CanvasGroup> (), 
				isCharacter
			);
			toggleCanvasGroup (
				GameObject.Find ("ContextMenu/Abilities/PlayerUnitButtons").GetComponent<CanvasGroup> (), 
				isPlayerCharacter
			);

			return true;

		}

		/* 
		 * Return true if context menu is active (Only for Exploration and Battle Maps)
		 */
		public bool isContextMenuActive() {
			if (
				currentMap.Equals (MapButtons.ExplorationMap) ||
				currentMap.Equals (MapButtons.BattleMap))
				return (
					GameObject.Find ("ContextMenu").GetComponent<CanvasGroup> ().alpha == 1 &&
					GameObject.Find ("ContextMenu").GetComponent<CanvasGroup> ().interactable == true
				);
			return false;
		}

		/*
		 * Return true is unit plate is active (Only in Battle Maps)
		 */
		public bool isUnitPlateActive() {
			return (
					GameObject.Find ("BottomBar/SelectedUnit").GetComponent<CanvasGroup> ().alpha == 1 &&
					GameObject.Find ("BottomBar/SelectedUnit").GetComponent<CanvasGroup> ().interactable == true
				);
		}

		public void SetActiveButton(string buttonName) {

			Button[] buttons = transform.Find("ButtonAnchor/MapSelection").GetComponentsInChildren<Button> ();
			ColorBlock colors;
			foreach (var button in buttons) {
				if (button.name.Equals (buttonName)) {
					colors = button.colors;
					colors.normalColor = _highlightedColor;
					button.colors = colors;
				} else {
					colors = button.colors;
					colors.normalColor = _defaultColor;
					button.colors = colors;
				}
			}
		}
	}
}