using UnityEngine;
using System.Collections;
using Umbra.Models;
using Umbra.Utilities;
using Umbra.Data;
using Umbra.UI;
using System.Collections.Generic;
using Umbra.Xml;
using Umbra.Scenes.ExplorationMap;

using UnityEngine.UI;

namespace Umbra.Managers
{

	public class ExplorationManager : SceneSingleton<ExplorationManager>
	{

		private TileManager _tileManager;
		private ActorManager _actorManager;

		private CharacterModel _characterModel;
		private FactionModel _factionModel;
		private ItemModel _itemModel;
		private List<int> _playerReputations;
		private MapModel _mapModel;
		private PlayerModel _playerModel;

		private List<List<Actor>> _actors;
		private List<Actor> _itemActors;

		// UI elements
		private DiamondUIController _diamondUIController;
		private List<Button> _contextButtons;

		// Bottom Bar Feedback
		private Text _feedbackText;

		// Character Plate
		private Text _unitName;
		private Image _unitNameBackground;
		private Image _unitIcon;

		// Inspect Panel
		private Image _inspectIcon;
		private Text _inspectName;
		private Text _inspectText;

		// Flags
		private bool _contextMenuSet;
		// This script must be called only by exploration maps, so there is no point for the bool below
		//public bool onExplorationMap = true; // is on the exploration map and not a battle map - if it is render things like outpost
		public bool waitingOnOutpost;
		public bool containsOutpost; //temporary

		void Awake()
		{
		
			_tileManager = TileManager.Instance;
			_actorManager = ActorManager.Instance;

			_actors = new List<List<Actor>>();
			for (int i = 0; i < 7; i++) {
				_actors.Add (new List<Actor> ());
			}
			_itemActors = new List<Actor> ();

			_characterModel = new CharacterModel ();
			_factionModel = new FactionModel ();
			_itemModel = new ItemModel ();
			_mapModel = new MapModel ();
			_playerReputations = _factionModel.data[1].reputations;
			_playerModel = new PlayerModel ();

			// UI Elements
			_diamondUIController = GameObject.Find ("DiamondUI").GetComponent<DiamondUIController> ();
			_contextButtons = new List<Button> ();
			_contextButtons.Add (GameObject.Find ("ContextMenu/Basics/InspectButton").GetComponent<Button> ());
			_contextButtons.Add (GameObject.Find ("ContextMenu/Exploration/RecruitAttackButton").GetComponent<Button> ());
			_contextButtons.Add (GameObject.Find ("ContextMenu/Exploration/TalkUseButton").GetComponent<Button> ());

			// Character Plate
			_unitName = GameObject.Find ("BottomBar/SelectedUnit/SelectedName/SelectedNameText").GetComponent<Text> ();
			_unitNameBackground = GameObject.Find ("BottomBar/SelectedUnit/SelectedName/Background").GetComponent<Image> ();
			_unitIcon = GameObject.Find ("BottomBar/SelectedUnit/SelectedIcon").GetComponent<Image> ();

			// Bottom Bar Feedback
			_feedbackText = GameObject.Find ("ActionFeedbackBoxText").GetComponent<Text>();
			_feedbackText.text = "Welcome to " + _mapModel.getCurrentMap().name;

			// Inspect Panel
			_inspectIcon = GameObject.Find ("InspectWindow/Icon").GetComponent<Image> ();
			_inspectName = GameObject.Find ("InspectWindow/Name").GetComponent<Text> ();
			_inspectText = GameObject.Find ("InspectWindow/Description").GetComponent<Text> ();

		}

		// Use this for initialization
		void Start()
		{

			// force battle map if the current map is not a exploration map
			if (_mapModel.getCurrentMap ().isBattleMap)
				GameStateManager.Instance.PushScene (GameScene.BattleMap);
			else {

				Debug.Log (_mapModel.getCurrentMap ().id);

				foreach (SpawnPoint sp in _tileManager.spawnpoints) {
			
					// Spawn a character
					if (sp.type == SpawnType.Character) {

						Character c = _characterModel.getCharacterByID (sp.toSpawn);

						if (c != null) {

							if (!c.isDead) {

								if (_characterModel.getLocation (c).map != _mapModel.getCurrentMap ().id)
									_characterModel.setLocation (c, sp.x, sp.y, 0, _mapModel.getCurrentMap ().id);

								_actors [c.factionIdx].Add (
									_actorManager.CreateActorWithCharacter (
										c, 
										c.worldloc.x, 
										c.worldloc.y,
										c.factionIdx, 
										_actors [c.factionIdx].Count, 
										!(_playerReputations [c.dbloc.faction] >= 75 || c.dbloc.faction == 1),
										c.worldloc.direction
									)
								);

							}

						}

					}

					// Spawn an item
					if (sp.type == SpawnType.Item) {

						if (_itemModel.getSourceItem (sp.toSpawn) != null) {

							// need condition here that checks if items were already picked up

							Item i = _itemModel.createItem (sp.toSpawn);

							if (_itemModel.getLocation (i).isEmpty ())
								_itemModel.setLocation (i, sp.x, sp.y, 0, _mapModel.getCurrentMap ().id);

							_itemActors.Add (
								_actorManager.CreateActorWithItem (
									i,
									i.worldloc.x,
									i.worldloc.y
								)
							);

						}

					}

				}
				_characterModel.clearBattlingCharacters ();
				_contextMenuSet = false;

				/* Old spawn system
				int x = 0;
				int y = 0;
				int n = 0;
				// Spawn characters on the map
				foreach (Character c in _characterModel.getAllCharacters()) {

					if (!c.isDead) {

						if (_characterModel.getLocation (c).isEmpty ())
							_characterModel.setLocation (c, x, y);
						
						_actors [n].Add (
							_actorManager.CreateActorWithCharacter (
								c, 
								c.worldloc.x, 
								c.worldloc.y,
								n, 
								_actors[n].Count, 
								!(_playerReputations [c.dbloc.faction] >= 75 || c.dbloc.faction == 1)
							)
						);

						n++;

					}

					x += 5;
					y += 1;

				}
				*/

				IconLoader.Load ();

				_diamondUIController.setContextMenuExploration (false);
				_diamondUIController.setUnitPlate (false);
				_diamondUIController.setInspectWindow (false);

			}

		}

		// Update is called once per frame
		void Update()
		{

			if (_actorManager.moveActors.Count == 0) {

				// Clear targeted and/or selected unit when right clicking
				if (Input.GetMouseButtonUp (1))
					clearSelection ();

				playerTurn ();

			}

		}

		/*
		 * Code that handles displaying/hiding the context menu, populating the unit plate and inspect window
		 */
		void playerTurn() {

			// Check if unit plate is active and populate it if so
			if (_diamondUIController.isUnitPlateActive ())
				populateCharacterPlate (_actorManager.getPlayerSelectedCharacter ());
			else
				_diamondUIController.setUnitPlate (_actorManager.getPlayerSelectedCharacter () != null);

			// Is context menu set and active?
			if (!_contextMenuSet) {

				// Is there an active target?
				if (_actorManager.getPlayerSelectedActor() != null &&
					(
						_actorManager.getPlayerTargetedCharacter() != null || 
						_actorManager.getPlayerTargetedItem() != null
					)) {
					_diamondUIController.setContextMenuExploration(
						true,
						!_actorManager.getPlayerSelectedActor().isHostile
					);
				}

				// Flag context menu as set; prevent it from being refreshed every update
				if (_diamondUIController.isContextMenuActive ()) {
					_contextMenuSet = true;
					setExplorationButtonStates (
						_actorManager.getPlayerSelectedActor(),
						_actorManager.getPlayerTargetedActor()
					);
				}

			}
				
			// If a unit is selected, but there is no target, you can move the selected unit
			if (
				_actorManager.getPlayerSelectedCharacter () != null && 
				_actorManager.getPlayerTargetedCharacter () == null
			) {
				// Attempt to move selected player unit
				if ((!_actorManager.getPlayerSelectedActor ().isHostile) &&
					_tileManager.playerTargetedTile != null &&
					_actorManager.getPlayerTargetedActor () == null) {
					moveActor (_actorManager.getPlayerSelectedActor (), _tileManager.playerTargetedTile);
				}
			}

		}

		/*
		 * Displays an error message and clears the target. Helper function for moveActor
		 */
		void moveBad(string text) {
			_feedbackText.text = text;
		}

		/*
		 * Move Actor a to Tile t
		 */
		void moveActor(Actor a, Node t) {
			if (t.isPassable) {
				_actorManager.MoveActor (a, t.x, t.y);
				clearSelection ();
			} else moveBad("Cannot move unit to targeted tile...");
		}

		/*
		 * Set interactable state of exploration buttons based on proximity between selected and targeted actors
		 */
		void setExplorationButtonStates(Actor a, Actor b) {

			if (a == b) {
			
				_diamondUIController.setContextMenuExploration (true, false);
			
			} else {

				if (_actorManager.getDistanceBetweenActors (a, b) != 1) {
					
					_contextButtons [1].interactable = false;
					_contextButtons [2].interactable = false;

				} else {

					_contextButtons [1].interactable = true;
					_contextButtons [2].interactable = true;

				}

			}

		}

		/*
		 * Populate character plate with values grabbed from c
		 */
		void populateCharacterPlate(Character c) {

			if (c != null) {
				// Populate unit plate with the correct values
				_unitName.text = c.name;
				_unitNameBackground.color = _factionModel.getFactionColor (c.dbloc.faction);
				_unitIcon.sprite = IconLoader.GetSpriteByName (c.icon, "Avatars");
			}

		}

		/*
		 * Display and populate Inspect Window for unit u
		 */
		void populateInspectWindow(Actor a) {

			if (a.character != null) {

				Character c = a.character;
				Character src = _characterModel.getSourceCharacter (c.id);

				clearSelection ();
				_diamondUIController.setInspectWindow (true);

				_inspectIcon.sprite = IconLoader.GetSpriteByName (c.icon, "Avatars");
				_inspectName.text = c.name;

				_inspectText.text = "Level " + c.level + " " + c.className;
				_inspectText.text +=  " of the " + _factionModel.getFaction (c.dbloc.faction).name;
				_inspectText.text += "\nRank: " + new RankModel ().getRankById (c.rankID).name;
				_inspectText.text += "\n\nDescription\n\n" + c.description;

				_inspectText.text += "\n\nBattlegroup\n";

				UnitModel _unitModel = new UnitModel ();

				foreach (DBCoordinates dbc in c.battlegroup) {
					Unit u = _unitModel.getUnit (dbc);
					if (u != null) _inspectText.text += "\n" + u.name;
				}

			}

			if (a.item != null) {

				Item i = a.item;
				Item src = _itemModel.getSourceItem (i.id);

				clearSelection ();
				_diamondUIController.setInspectWindow (true);

				_inspectIcon.sprite = IconLoader.GetSpriteByName (i.icon, "Items");
				_inspectName.text = i.name;
				_inspectText.text = i.description;

			}

		}

		/*
		 * Clear selected and targeted tiles
		 */
		void clearSelection() {
			_actorManager.clearPlayerSelectedActor ();
			_contextMenuSet = false;
			_diamondUIController.setInspectWindow (false);
			_diamondUIController.setContextMenuExploration (false);
			_diamondUIController.setUnitPlate (false);
		}

		/*** Commands and actions triggered by the player ***/

		// Call appropriate function, based on action
		public void click(ExplorationButtons action) {

			switch (action) {

			case ExplorationButtons.cmdObjectives:
				commandsObjectives ();
				break;
			case ExplorationButtons.ctxInspect:
				actionInspect ();
				break;
			case ExplorationButtons.ctxRecruitAttack:
				actionRecruitAttack();
				break;
			case ExplorationButtons.ctxTalkUse:
				actionTalkUse();
				break;
			}

		}

		// Functions assigned to the buttons on the screen
		void actionInspect() {
			populateInspectWindow (_actorManager.getPlayerTargetedActor());
		}

		void actionRecruitAttack () {

			if (_actorManager.getPlayerTargetedItem () != null) {
				_feedbackText.text = "You cannot attack an item.";
			} else {
				if (_actorManager.getPlayerTargetedActor () != _actorManager.getPlayerSelectedActor ()) {
					_feedbackText.text =
					"Attacking " + _actorManager.getPlayerTargetedActor ().character.name;
					if (_actorManager.getPlayerTargetedActor ().isHostile) {
						if (_characterModel.setBattlingCharacters (
							   _actorManager.getPlayerSelectedCharacter (),
							   _actorManager.getPlayerTargetedCharacter ())) {
							GameStateManager.Instance.SaveGame ();
							_mapModel.setBattleMap (_mapModel.getCurrentMap (), 0);
							GameStateManager.Instance.PushScene (GameScene.BattleMap);
						} else
							_feedbackText.text = "Cannot attack that target.";
					} else
						_feedbackText.text = "Cannot attack friendly targets.";
				} else
					_feedbackText.text = "A character cannot attack itself.";
			}

		}

		void actionTalkUse() {

			// Talk to character
			if (_actorManager.getPlayerTargetedCharacter () != null) {
				_feedbackText.text = 
					"Talking to " + _actorManager.getPlayerTargetedActor ().character.name;
			}

			// Pick up item
			if (_actorManager.getPlayerTargetedItem () != null) {

				if (_characterModel.inventoryGetSize (_actorManager.getPlayerSelectedCharacter ()) < 6) {
					_feedbackText.text = "Picked up " + _actorManager.getPlayerTargetedItem ().name + ".";
					_characterModel.inventoryAddItem (
						_actorManager.getPlayerSelectedCharacter (),
						_actorManager.getPlayerTargetedItem ()
					);

					foreach (DBCoordinates dbc in _characterModel.getInventory(_actorManager.getPlayerSelectedCharacter())) {
						Debug.Log (_itemModel.getItem (dbc).name);
					}

					Destroy (_actorManager.getPlayerTargetedActor ().GameObject); // remove item from game world
				} else {
					_feedbackText.text = "Inventory is full. Cannot pick up item.";
				}

			}
				
		}

		void commandsObjectives() {
            Umbra.UI.Obj_GUI_Window.Open();
		}

	}

}