using UnityEngine;
using System.Collections;
using Umbra.Models;
using Umbra.Utilities;
using Umbra.Data;
using Umbra.UI;
using Umbra.Scenes.BattleMap;
using System.Collections.Generic;
using Umbra.Xml;
using System;
using UnityEngine.UI;

namespace Umbra.Managers
{

	// Manager/controller class for a battle scenario
    public class BattleManager : SceneSingleton<BattleManager>
    {

		private TileManager _tileManager;
		private ActorManager _actorManager;

		private AI _ai;

		private AbilityModel _abilityModel;
		private CharacterModel _characterModel;
		private BuffModel _buffModel;
		private FactionModel _factionModel;
		private ItemModel _itemModel;
		private MapModel _mapModel;
		private PerkModel _perkModel;
		private UnitModel _unitModel;
		private PlayerModel _playerModel;
	
		private float _morale; // -1 to 1, 0 means equal morale for both sides
		private int _currentPlayer; // 0 is the player, 1 is the computer
		private int _turnCounter;
		private bool _autoplay;
		private System.Random random;

		private List<List<BattleActor>> _actors;
		private List<List<BattleActor>> _actorsDead;

		// UI Elements
		private Slider _moraleSlider;
		private CanvasGroup _moraleBar;

		// Diamond UI Menu
		private DiamondUIController _diamondUIController;
		private CanvasGroup _contextMenu;
		private List<Button> _abilityButtons; // includes item buttons as well
		private Text _turnCounterText;

		private Button _btnFlee;
		private Button _btnReturn;
		private Button _btnAutoplay;

		// Bottom Bar Feedback
		private CanvasGroup _bottomBar;
		private Text _feedbackText;

		// Unit Plate
		private Text _unitName;
		private Image _unitNameBackground;
		private Image _unitIcon;
		private Slider _unitHP;
		private Text _unitHPCurr;
		private Slider _unitAP;
		private Text _unitAPCurr;
		private Slider _unitSP;
		private Text _unitSPCurr;

		// Game Over Panel
		private Text _battleResult;
		private GameObject[] _gameOverSides;

		// Inspect Panel
		private Image _inspectIcon;
		private Text _inspectName;
		private Image _inspectNameBackground;
		private Text _inspectText;

		// Flags
		private bool _contextMenuSet;

        void Awake()
        {

			_tileManager = TileManager.Instance;
			_actorManager = ActorManager.Instance;

			_actors = new List<List<BattleActor>> { 
				new List<BattleActor>(), 
				new List<BattleActor>()
			};
			_actorsDead = new List<List<BattleActor>> {
				new List<BattleActor>(),
				new List<BattleActor>()
			};

			_ai = new AI ();
			random = new System.Random ();

			_abilityModel = new AbilityModel ();
			_buffModel = new BuffModel ();
			_characterModel = new CharacterModel ();
			_factionModel = new FactionModel ();
			_itemModel = new ItemModel ();
			_mapModel = new MapModel ();
			_perkModel = new PerkModel ();
			_unitModel = new UnitModel();
			_playerModel = new PlayerModel ();

			// UI Elements
			_moraleSlider = GameObject.Find ("MoraleSlider").GetComponent<Slider> ();
			_moraleBar = GameObject.Find ("MoraleBar").GetComponent<CanvasGroup> ();

			// Diamond UI Menu
			_diamondUIController = GameObject.Find ("DiamondUI").GetComponent<DiamondUIController> ();
			_abilityButtons = new List<Button> ();
			_abilityButtons.Add(GameObject.Find("ContextMenu/Abilities/UnitButtons/Ability1Button").GetComponent<Button>());
			_abilityButtons.Add(GameObject.Find("ContextMenu/Abilities/UnitButtons/Ability2Button").GetComponent<Button>());
			_abilityButtons.Add(GameObject.Find("ContextMenu/Abilities/UnitButtons/Ability3Button").GetComponent<Button>());
			_abilityButtons.Add(GameObject.Find("ContextMenu/Abilities/UnitButtons/Ability4Button").GetComponent<Button>());
			_abilityButtons.Add(GameObject.Find("ContextMenu/Abilities/CharacterUnitButtons/Ability5Button").GetComponent<Button>());
			_abilityButtons.Add(GameObject.Find("ContextMenu/Abilities/PlayerUnitButtons/Ability6Button").GetComponent<Button>());
			_abilityButtons.Add(GameObject.Find("ContextMenu/Abilities/CharacterUnitButtons/Item1Button").GetComponent<Button>());
			_abilityButtons.Add(GameObject.Find("ContextMenu/Abilities/PlayerUnitButtons/Item2Button").GetComponent<Button>());

			_btnFlee = GameObject.Find ("ButtonAnchor/DefaultBattle/FleeButton").GetComponent<Button> ();
			_btnReturn = GameObject.Find ("ButtonAnchor/DefaultBattle/ReturnButton").GetComponent<Button> ();
			_btnAutoplay = GameObject.Find ("ButtonAnchor/DefaultBattle/AutoplayButton").GetComponent<Button> ();
			_turnCounterText = GameObject.Find ("TurnCounter").GetComponent<Text> ();

			_btnReturn.interactable = false;

			// Bottom Bar Feedback
			_feedbackText = GameObject.Find ("ActionFeedbackBoxText").GetComponent<Text>();
			_bottomBar = GameObject.Find ("BottomBar").GetComponent<CanvasGroup> ();

			// Unit Plate
			_unitName = GameObject.Find ("BottomBar/SelectedUnit/SelectedName/SelectedNameText").GetComponent<Text> ();
			_unitNameBackground = GameObject.Find ("BottomBar/SelectedUnit/SelectedName/Background").GetComponent<Image> ();
			_unitIcon = GameObject.Find ("BottomBar/SelectedUnit/SelectedIcon").GetComponent<Image> ();
			_unitHP = GameObject.Find ("BottomBar/SelectedUnit/SelectedHealthBar").GetComponent<Slider> ();
			_unitHPCurr = GameObject.Find ("BottomBar/SelectedUnit/SelectedHealthBar/SelectedHealthBarText").GetComponent<Text> ();
			_unitAP = GameObject.Find ("BottomBar/SelectedUnit/SelectedArmorBar").GetComponent<Slider> ();
			_unitAPCurr = GameObject.Find ("BottomBar/SelectedUnit/SelectedArmorBar/SelectedArmorBarText").GetComponent<Text> ();
			_unitSP = GameObject.Find ("BottomBar/SelectedUnit/SelectedShieldBar").GetComponent<Slider> ();
			_unitSPCurr = GameObject.Find ("BottomBar/SelectedUnit/SelectedShieldBar/SelectedShieldBarText").GetComponent<Text> ();

			// Game Over Panel
			_battleResult = GameObject.Find ("GameOver/BattleResult").GetComponent<Text> ();
			_gameOverSides = GameObject.FindGameObjectsWithTag ("BattleMap_Side");

			// Inspect Panel
			_inspectIcon = GameObject.Find ("InspectWindow/Icon").GetComponent<Image> ();
			_inspectName = GameObject.Find ("InspectWindow/Name").GetComponent<Text> ();
			_inspectNameBackground = GameObject.Find ("InspectWindow/NameBackground").GetComponent<Image> ();
			_inspectText = GameObject.Find ("InspectWindow/Description").GetComponent<Text> ();

        }
			
        void Start()
        {

			// force exploration map if the current map is not a battle map
			if (!_mapModel.getCurrentMap().isBattleMap) GameStateManager.Instance.PushScene (GameScene.ExplorationMap);
			else {

				_morale = 0f;
				_currentPlayer = 0;
				_turnCounter = 0;
				_autoplay = false;
				_contextMenuSet = false;

				_feedbackText.text = "Battle mode engaged.\n";

				int x = 0;
				int n = 0;
				// This method here should connect the units to the map's spawn points!
				foreach (Character c in _characterModel.getBattlingCharacters()) {
					
					int y = 0;
					_actors [n].Add (
						new BattleActor (
							_actorManager.CreateActorWithCharacter (c, x, y, n, y, n != 0, n), n, y
						)
					);

					foreach (Unit u in _unitModel.getUnits(_characterModel.getBattlegroup(c))) {
						y++;
						_actors [n].Add(
							new BattleActor(
								_actorManager.CreateActorWithUnit (u, x, y, n, y, n != 0, n), n, y
							)
						);
					}

					x += 10;
					n++;

				}

				if (_actors [0].Count > _actors [1].Count)
					_morale = -0.1f;
				if (_actors [0].Count < _actors [1].Count)
					_morale = 0.1f;

				IconLoader.Load ();

				_diamondUIController.setContextMenuBattle (false);
				_diamondUIController.setUnitPlate (false);
				_diamondUIController.setInspectWindow (false);
				_diamondUIController.setGameOverWindow (false);

			}

        }

        // Update is called once per frame
        void Update()
        {

			_moraleSlider.value = _morale;
			_turnCounterText.text = _turnCounter.ToString ();

			// Check for moving actors
			if (_actorManager.moveActors.Count > 0) {
				_diamondUIController.setContextMenuBattle (false);

			// if there are no moving actors, a turn can happen
			} else {

				// Handle context menu only if it's the player's turn
				if (_currentPlayer == 0) {

					// if autoplay is not active let player do something
					if (!_autoplay) {

						// Clear targeted and/or selected unit when right clicking
						if (Input.GetMouseButtonUp (1)) clearSelection ();

						playerTurn ();

					// Autoplay is active, let AI take over
					} else {
						AITurn (_actors [0], _actors [1]);
					}

				} else if (_currentPlayer == 1) {
					// AI's turn
					AITurn(_actors[1], _actors[0]);
				} // else disable controls for both sides - likely game over

			}

        }

		/*
		 * Code that handles displaying/hiding the context menu, populating the unit plate and inspect window
		 */
		void playerTurn() {
			
			// Check if unit plate is active and populate it if so
			if (_diamondUIController.isUnitPlateActive ())
				populateUnitPlate (
					_actorManager.getPlayerTargetedUnit () != null ? 
					_actorManager.getPlayerTargetedUnit () : _actorManager.getPlayerSelectedUnit ()
				);
			else
				_diamondUIController.setUnitPlate (_actorManager.getPlayerSelectedUnit () != null);

			// Is context menu set and active?
			if (!_contextMenuSet) {

				// Is there an active target?
				if (_actorManager.getPlayerTargetedUnit() != null) {

					_diamondUIController.setContextMenuBattle(
						true,
						_actorManager.getPlayerSelectedUnit () != null ? 
						_actorManager.getPlayerSelectedUnit ().isCharacter : false,
						_actorManager.getPlayerSelectedUnit () != null ? 
						_actorManager.getPlayerSelectedUnit ().isPlayerCharacter : false,
						!_actorManager.getPlayerSelectedActor ().isHostile &&
						!_actorManager.getPlayerSelectedUnit().isDead
					);
				}

				// Flag context menu as set; prevent it from being refreshed every update
				if (_diamondUIController.isContextMenuActive ()) {
					_contextMenuSet = true;
					setAbilityButtonStates (
						_actorManager.getPlayerSelectedActor (), 
						_actorManager.getPlayerTargetedActor ()
					);
				}

			}

			// If a unit is selected, but there is no target, you can move the selected unit
			if (
				_actorManager.getPlayerSelectedUnit () != null && 
				_actorManager.getPlayerTargetedUnit () == null
			) {
				// Attempt to move selected player unit
				if (_actorManager.getPlayerSelectedActorSide () == 0 && // a null checker and side checker in one
					_tileManager.playerTargetedTile != null &&
					_actorManager.getPlayerTargetedActor () == null) {
					moveActor (_actorManager.getPlayerSelectedActor (), _tileManager.playerTargetedTile);
				}
			}

		}

		/*
		 * Let the AI do something
		 */
		void AITurn (List<BattleActor> side1, List<BattleActor> side2) {

			AIAction doSomething = _ai.doSomething (side1, side2);

			_diamondUIController.setContextMenuBattle (false);
			_diamondUIController.setInspectWindow (false);
			_diamondUIController.setUnitPlate (false);

			if (doSomething.ability != null)
				castAbility (
					doSomething.caster, 
					doSomething.target, 
					-1, 
					doSomething.ability,
					true);
			else if (doSomething.destination != null)
				moveActor (
					doSomething.caster.actor, 
					doSomething.destination,
					true
				);
			// else just concede and skip turn - should only happen if all AI unit are too far and immobile
			nextTurn ();

		}

		/*
		 * Displays an error message and clears the target. Helper function for moveActor
		 */
		void moveBad(string text) {
			_feedbackText.text = text;
			_actorManager.clearPlayerTargetedActor ();
		}

		/*
		 * Move Actor a to Tile t
		 */
		void moveActor(Actor a, Node t, bool isComputer=false) {
			// is movement done by a computer actor? if so, assume all calculations were done properly
			if (isComputer) {
				_actorManager.MoveActor (a, t.x, t.y);
			// move player actor
			} else {
				if (!a.unit.isDead) {
					if (a.unit.isMovable && !a.unit.isDisabled) {
						if (t.isPassable) {
							if (
								_tileManager.getDistanceBetweenNodes (a.parent, t) <
								a.unit.movementRange * a.unit.movementRangeMult) {
								_actorManager.MoveActor (a, t.x, t.y);
								_feedbackText.text = "Moving unit...\n";
								nextTurn ();
							} else moveBad("Destination is too far away...\n");
						} else moveBad("Cannot move unit to targeted tile...\n");
					} else moveBad("Cannot move this unit...\n");
				} else moveBad("Cannot move destoyed units...\n");
			}
		}

		/*
		 * Roll a die with specified sides
		 */
		int diceRoll(int sides) {
			if (sides > 0)
				return random.Next (0, sides);
			return -1;
		}

		/*
		 * Set the next player for the next turn
		 */
		void nextPlayer() {
			_currentPlayer = _currentPlayer == 0 ? 1 : 0;
			// If morale favors one side, give that side an up to 5% chance at an extra turn
			if (_morale < 0) {
				if ((diceRoll ((int)(20 / Math.Abs (_morale))) == 0)) {
					_currentPlayer = 0;
					_feedbackText.text += "\nPlayer gained an extra turn.";
				}
			} else if (_morale > 0) {
				if ((diceRoll ((int)(20 / Math.Abs (_morale))) == 0)) {
					_currentPlayer = 1;
					_feedbackText.text += "\nComputer gained an extra turn.";
				}
			}
		}

		/*
		 * Start new turn
		 */
		void nextTurn() {

			if (_currentPlayer != 2) {

				for (int i = 0; i < 2; i++) {
					if (_actorsDead [i].Count == _actors [i].Count) {
						gameOver (i);
						break;
					}
				}

				if (_currentPlayer != 2) { // ensure these don't get called if gameOver was called
					decrementDurations ();
					_turnCounter += 1;
					nextPlayer ();
				}

			}

			clearSelection ();

		}

		/*
		 * Clears data from each ability button
		 */
		void clearAbilityButtonStates() {
			string text; 
			for (int i = 0; i < 8; i++) {
				_abilityButtons [i].interactable = false;
				if (i < 6) text = "Ability " + (i + 1).ToString ();
				else text = "Item " + (i - 5).ToString ();
				setAbilityButtonText (_abilityButtons [i], text);
			}
			_contextMenuSet = false;
		}

		/*
		 * Set button b's text to text
		 */
		void setAbilityButtonText(Button b, string text) {
			b.transform.Find ("Text").GetComponent<Text> ().text = text;
		}

		/*
		 * Enable/disable context menu buttons depending to the boolean values from the Ability class
		 */
		void setAbilityButtonStates(Actor caster, Actor target) {

			if (caster.side == 0) {

				int c = 0;
				// Scan caster's abilities and determine if any ability can be cast
				foreach (string k in caster.unit.abilities) {
					Ability t = _abilityModel.getAbilityById (k);
					if (t != null && !caster.unit.isDead) {
						_abilityButtons [c].interactable = _abilityModel.getAbilityState (
							t, 
							target.unit.isDead,
							_actors [caster.side] [caster.sideIndex].abilityLockouts [c],
							_actors [caster.side] [caster.sideIndex].abilityCooldowns [c],
							caster.unit.altitude == target.unit.altitude,
							_actorManager.getDistanceBetweenActors (caster, target),
							target.unit.isVisible,
							caster == target, 
							target.isHostile
						);
						// if there is a cooldown, show the number of turns left
						if (_actors [caster.side] [caster.sideIndex].abilityCooldowns [c] > 0) {
							setAbilityButtonText(
								_abilityButtons [c], 
								_actors [caster.side] [caster.sideIndex].abilityCooldowns [c].ToString ()
							);
						// if there is no cooldown, show the ability name instead
						} else {
							setAbilityButtonText (
								_abilityButtons [c],
								t.name
							);
						}
						c++;
					} else {
						_abilityButtons [c].interactable = false;
					}
				}

				// Scan caster's items and determine if any can be used
				for (c = 6; c < 8; c++) {
					// Item i = _characterModel.inventoryGetEquippedItem (caster.character, c - 6); - bypass this due to time constraints
					Item i = _itemModel.getItem(_characterModel.getInventoryItem(caster.character, c - 6));
					_abilityButtons [c].interactable = false;
					if (i != null && !caster.unit.isDead) {
						Ability t = _abilityModel.getAbilityById (i.abilityID);
						Debug.Log (t.name);
						if (t != null) {
							_abilityButtons [c].interactable = _abilityModel.getAbilityState (
									t, 
									target.unit.isDead,
									false, // items have no lockout
									_actors [caster.side] [caster.sideIndex].abilityCooldowns [c],
									caster.unit.altitude == target.unit.altitude,
									_actorManager.getDistanceBetweenActors (caster, target),
									target.unit.isVisible,
									caster == target,
								target.isHostile
								);
							// if there is a cooldown, show the number of turns left
							if (_actors [caster.side] [caster.sideIndex].abilityCooldowns [c] > 0) {
								setAbilityButtonText (
									_abilityButtons [c],
									_actors [caster.side] [caster.sideIndex].abilityCooldowns [c].ToString ()
								);
								// if there is no cooldown, show the item name instead
							} else {
								setAbilityButtonText(
									_abilityButtons [c],
									i.name
								);
							}

						} 
					}
				}

			}

		}

		/*
		 * Display and populate Inspect Window for unit u
		 */
		void populateInspectWindow(Unit u) {

			if (u != null) {

				Unit src = _unitModel.getSourceUnit (u.id);

				clearSelection ();

				_diamondUIController.setInspectWindow (true);

				_inspectIcon.sprite = IconLoader.GetSpriteByName (u.icon, "Units");
				_inspectName.text = u.name;
				_inspectNameBackground.color = _factionModel.getFactionColor (u.dbloc.faction);

				_inspectText.text = "Description\n\n";
				_inspectText.text += u.description;

				_inspectText.text += "\n\nClassification: ";

				if (u.unitClass == "Infantry") _inspectText.text += "Infantry";
				if (u.unitClass == "GroundVehicle")	_inspectText.text += "Ground Vehicle";
				if (u.unitClass == "FlyingVehicle") _inspectText.text += "Flying Vehicle";

				_inspectText.text += "\nTech Level: " + u.techLevel;
				_inspectText.text += "\nHealth: " + src.health;
				_inspectText.text += "\nArmor: " + src.armor;
				_inspectText.text += "\nShield: " + src.shield;
				_inspectText.text += "\n";

				int i = 1;
				foreach (string ak in src.abilities) {
					Ability ab = _abilityModel.getAbilityById (ak);
					if (ab != null) {
						_inspectText.text += 
							"\nAbility " + i.ToString () + ": " + ab.name + " (Area of Effect: " +
							ab.AOERadius.ToString () + "; Cooldown: " + ab.cooldown.ToString () + ")";
					}
					i++;
				}

			}

		}

		/*
		 * Populate the bottom bar unit plate with the values grabbed from u
		 */
		void populateUnitPlate(Unit u) {
			
			if (u != null) {
				
				List<int> stats = _unitModel.getUnitStats (u);

				// Populate unit plate with the correct values
				_unitName.text = u.name;
				_unitNameBackground.color = _factionModel.getFactionColor (u.dbloc.faction);
				_unitIcon.sprite = IconLoader.GetSpriteByName (u.icon, "Units");

				_unitHPCurr.text = stats [0].ToString () + " / " + stats [1].ToString ();
				_unitHP.maxValue = Math.Max(stats[0], stats[1]);
				_unitHP.value = stats [0];

				_unitAPCurr.text = stats [2].ToString () + " / " + stats [3].ToString ();
				_unitAP.maxValue = Math.Max(stats[2], stats [3]);
				_unitAP.value = stats [2];

				_unitSPCurr.text = stats [4].ToString () + " / " + stats [5].ToString ();
				_unitSP.maxValue = Math.Max(stats[4], stats [5]);
				_unitSP.value = stats [4];

			}

		}

		/*
		 * Decrement ability and buff cooldowns of ALL actors in the battlemap
		 */
		void decrementDurations() {

			foreach (List<BattleActor> atl in _actors) {
				foreach (BattleActor at in atl) {

					if (!at.actor.unit.isDead && at.actor.unit.health == 0)	at.actor.unit.isDead = true;

					if (!at.actor.unit.isDead) { // don't waste CPU cycles with dead targets
						// Decrement all ability cooldowns by 1
						for (int i = 0; i < 8; i++) {
							if (at.abilityCooldowns [i] > 0)
								at.abilityCooldowns [i]--;
						}
						// Decrement ALL buff durations by 1, and remove buffs that reached 0
						for (int i = at.buffDurations.Count - 1; i >= 0; i--) {
							if (at.buffDurations [i] > 0) {
								// reapply buff effect, in case another one was removed
								at.buffDurations [i]--;
								//_buffModel.applyBuffEffects (at.actor.unit, at.buffs [i]);
								buffTick (at, at.buffs [i]);
							} else {
								_feedbackText.text += 
								"\"" + _buffModel.getBuffById (at.buffs [i]).name + "\" faded from " +
								at.actor.unit.name + ".\n";
								_buffModel.resetBuffEffects (
									at.actor.unit, 
									_unitModel.getSourceUnit (at.actor.unit.id), 
									at.buffs [i]
								);
								at.removeBuff (i);
							}
						}
						// modify actor's color based on the current buffs
						at.applyColor(_buffModel.getResultantBuffColor (at.buffs));
					}
				}
			}
				
		}

		/*
		 * Add val to _morale, while ensure _morale is within its boundaries
		 */
		void adjustMorale(float val) {
			_morale += val;
			_morale = Mathf.Min(1, Mathf.Max(-1, _morale));
		}

		/*
		 * Runs a few extra functions based on signal returned from hard coded ability
		 */
		public void executeSignal(Dictionary<string, string> signal, Actor caster, Actor target, 
			int abilityIndex, Ability ability
		) {

			if (signal != null) {

				if (signal.ContainsKey ("RemoveBuff")) 
					_actors [target.side] [target.sideIndex].removeBuff (signal ["RemoveBuff"]);

				if (signal.ContainsKey ("Message"))
					_feedbackText.text += signal ["Message"] + "\n";

				if (signal.ContainsKey ("CooldownReset"))
					if (signal ["CooldownReset"] == "caster")
						_actors [caster.side] [caster.sideIndex].buffDurations [abilityIndex] = ability.cooldown;

			}

		}

		/*
		 * Apply buff damage over time ability, if any, to damage actor's unit
		 */
		void buffTick(BattleActor actor, string buffID) {
			Ability buffAbility = _abilityModel.getAbilityById (_buffModel.getBuffById (buffID).ability);
			if (buffAbility != null) castAbility (actor, actor, -1, buffAbility, true);
		}

		/*
		 * Cast ability with name ability from unit caster to target unit
		 */
		void castAbility(
			BattleActor caster, BattleActor target, int abilityIndex, Ability abilityOverride=null, bool bypassTurns=false
		) {

			// run ability function (if any) on target unit
			// apply attached buff (if any) on target unit

			Ability ability;

			if (abilityOverride == null) {

				// Is it an ability?
				if (abilityIndex >= 0 && abilityIndex < 6)
					ability = _abilityModel.getAbilityById (
						caster.actor.unit.abilities [abilityIndex]
					);
				// Is it an item?
				else
					ability = _abilityModel.getAbilityById (
						//_characterModel.inventoryGetEquippedItem (caster.actor.character, abilityIndex - 6).abilityID
						_characterModel.inventoryGetItem(caster.actor.character, abilityIndex - 6).abilityID
					);

			// grab the ability override instead
			} else {
				ability = abilityOverride;
			}

			// Prevent casting ability if the target is dead, and ability has no effect on the dead
			if (target.actor.unit.health == 0 && !ability.isCastableOnDisabled) {

				if (target.actor.unit.unitClass == "infantry") _feedbackText.text += "Target is already dead.\n";
				else _feedbackText.text += "Target was already destroyed.\n";

			// Target is alive, or dead and can be "attacked". Might be illegal in some countries.
			} else {

				// Fire projectile at target
				if (ability.projectileSprite != "") {
					Actor _projectile = _actorManager.CreateActorWithProjectile (
						ability.projectileSprite, caster.actor.x, caster.actor.y
					);
					_actorManager.MoveProjectile (_projectile, target.actor.x, target.actor.y);
				}

				// Display feedback and incur cooldown only on cast abilities, not DOTs
				if (abilityIndex > -1) {
					_feedbackText.text += caster.actor.unit.name + " used the ability " + ability.name;
					if (caster != target) _feedbackText.text += " on " + target.actor.unit.name;
					_feedbackText.text +=  ".\n";					
					// add + 1 to ensure timer does not lose a second for this turn
					caster.abilityCooldowns [abilityIndex] = ability.cooldown + 1;
				}

				// Grab nearest actors to target, determined by the ability's AOE Radius parameter.
				// 0 means just the target
				List<Actor> affectedTargets = _actorManager.getNearestActors (target.actor, ability.AOERadius);
				// Get distance between the main target and the caster as well, for proximity bonus damage
				double distance = _actorManager.getDistanceBetweenActors (caster.actor, target.actor);

				// Start damaging all affected targets
				foreach (Actor u in affectedTargets) {

					// First, call the ability's assigned function, if any, and execute an action based on the signal
					// returned by the ability's function
					if (ability.functionName != "") {
						executeSignal (
							_abilityModel.callAbilityFunction (caster.actor, u, ability, distance), 
							caster.actor, 
							target.actor,
							abilityIndex,
							ability
						);
					}

					// Only do damage calculations if AoE affected unit is alive; redundant check for main target
					if (!u.unit.isDead) {

						// Cast ability on actor only if caster and actor are enemies, or friendly fire is allowed or
						// ability is helpful and caster and target(s) are friendly
						if (
							(u.side != caster.side && !ability.isHelpful) ||
							(u.side == caster.side && ability.isHelpful) ||
							ability.isHarmfulFriendly
						) {

							// Get damage values done to the target's health, armor and shield
							List<int> damage = _abilityModel.applyAbility (
								caster.actor.unit, 
								u.unit, 
								ability, 
								distance,
								_actors [target.side] [0].actor.character.perkIDs.Contains ("Armorer"),
								_actors [caster.side] [0].actor.character.perkIDs.Contains ("Marksmanship"),
								_actors [caster.side] [0].actor.character.perkIDs.Contains ("FirstAid"),
								_morale,
								caster.actor.side,
								target.actor.side
					        );

							if (damage [0] < 0)
								_feedbackText.text += 
									u.unit.name + " was healed for " + (damage [0] * -1).ToString () + " points.\n";
							if (damage [0] > 0) {
								_feedbackText.text += 
									u.unit.name + " suffered " + damage [0].ToString () + " points of damage.\n";
							}

							// Display information if unit was killed, and adjust morale
							if (u.unit.health <= 0) {
								
								if (u.unit.unitClass == "infantry")
									_feedbackText.text += u.unit.name + " was slain.\n";
								else
									_feedbackText.text += u.unit.name + " was destroyed.\n";

								u.unit.isDead = true;

								_actors [u.side] [u.sideIndex].removeAllBuffs ();
								_actorsDead [u.side].Add (_actors[u.side][u.sideIndex]);

								if (u.side == 0) adjustMorale (1.0f / _actors [1].Count);									
								else adjustMorale (-1.0f / _actors [1].Count);
									
							}

							if (damage [1] < 0)
								_feedbackText.text += 
									u.unit.name + "'s armor was repaired for " + (damage [1] * -1).ToString () + " points.\n";
							if (damage [1] > 0) {
								_feedbackText.text += 
									u.unit.name + "'s armor was damaged for " + damage [1].ToString () + " points.\n";
							}

							if (damage [2] < 0)
								_feedbackText.text += 
									u.unit.name + " gained " + (damage [2] * -1).ToString () + " shield points.\n";
							if (damage [2] > 0) {
								_feedbackText.text += 
									u.unit.name + " lost " + damage [2].ToString () + " shield points.\n";
							}
							
							// Apply buffs caused by this ability only if the target is still alive
							if (u.unit.health > 0) {
								if (ability.targetBuffs.Count > 0) {
									foreach (string buffID in ability.targetBuffs) {
										Buff b = _buffModel.getBuffById (buffID);
										if (b != null) {
											if (_actors [u.side] [u.sideIndex].addBuff (buffID, b.duration)) {
												_buffModel.applyBuffEffects (
													_actors [u.side] [u.sideIndex].actor.unit, 
													buffID
												);
											}
											_feedbackText.text += "\"" + b.name + "\" was applied to " + u.unit.name + ".\n";
										}
									}
								}
							}

						}

					} // else do nothing; first if statement takes care of target, so AoE units can be skipped if dead

				}

				// bypassTurns is used by DOT abilities; would be silly to skip a turn for every DOT tick
				if (!bypassTurns) nextTurn ();

			}

		}

		/*
		 * End this battle
		 */
		void gameOver(int loser, bool fled=false) {

			// Set current player to 2, preventing further interactions with the battle field
			_currentPlayer = 2;

			// Toggle UI elements to disable further interaction with the battle map
			_btnFlee.interactable = false;
			_btnAutoplay.interactable = false;
			_bottomBar.alpha = 0;
			_bottomBar.interactable = false;
			_moraleBar.alpha = 0;
			_moraleBar.interactable = false;
			_diamondUIController.setContextMenuBattle (false);
			_diamondUIController.setInspectWindow (false);
			_diamondUIController.setGameOverWindow (true);

			// If the player character died in this battle, it is game over
			if (_actors [loser] [0].actor.character.isPlayerCharacter &&
				_actors [loser] [0].actor.unit.isDead) {
				_battleResult.text = "Game over.";
			} else {
				_battleResult.text = fled ? "You fled!" : (loser == 1) ? "You Are Victorious!" : "You Lost!";
				_btnReturn.interactable = true;
			}

			if (!fled) { // this prevents the slaying of a fleeing character
				// Mark loser's character as dead
				_actors [loser] [0].actor.character.isDead = true;
			}

			// Now loop through all units to determine losses, increment battlecounts and perform other calculations
			for (int i = 0; i < 2; i++) {

				// Set icons and names of the winner/loser here
				_gameOverSides [i].transform.Find("Icon").GetComponent<Image>().sprite = 
					IconLoader.GetSpriteByName(_actors[i][0].actor.character.icon, "Avatars");
				_gameOverSides [i].transform.Find ("Name").GetComponent<Text> ().text = 
					_actors [i] [0].actor.unit.name;

				// tmp contains the text that is displayed in each panel
				Text tmp = _gameOverSides [i].transform.Find ("Report").GetComponent<Text> ();

				// List lost units
				tmp.text = "Units Lost\n\n";
				foreach (BattleActor at in _actors[i]) {
					if (at.actor.unit.isDead)
						tmp.text += at.actor.unit.name + "\n";
					else {

						at.actor.unit.battleCount++;

						// Check for units that can swap abilities and reset the abilities
						if (
							at.actor.unit.id == "GVF5Medium" ||
							at.actor.unit.id == "HF5LeaderGround")
							at.actor.unit.abilities [3] = "F5_ScavengeGV";

						if (
							at.actor.unit.id == "AVF6Light" ||
							at.actor.unit.id == "HF6LeaderFlying")
							at.actor.unit.abilities [3] = "F6_AdaptAV";

					}
				}

				// Increment character's battle count as well
				_actors [i] [0].actor.character.battleCount++;

				// resurrect characters if they died in battle
				if (_actors [i] [0].actor.unit.isDead && ( (fled) || (loser != i) )) {
					_actors [i] [0].actor.unit.health = 125;
					_actors [i] [0].actor.unit.armor = 25;
					_actors [i] [0].actor.unit.shield = 75;
					_actors [i] [0].actor.unit.isDead = false;
				}

				// purge dead units from character's battlegroup
				_characterModel.battlegroupClear (_actors [i] [0].actor.character, true);

				// Special conditions for player's side
				if (i == 0) { 

					// add/remove resources to/from the player
					if (loser == 0)
						tmp.text += "\n\nResources Lost\n\n";
					else {
						// also add XP to character that won
						_characterModel.addExperiencePoints(_actors[i][0].actor.character, 15 * _actorsDead[1].Count);
						tmp.text += "\nXP gained: " + (15 * _actorsDead [1].Count).ToString ();
						tmp.text += "\n\nResources Gained\n";
					}

					// Output resource gains/losses
					List<int> res = 
						new RandomResources ().generateRandomResources (_actors [1] [0].actor.character.dbloc.faction);

					if (res [0] > 0) {
						tmp.text += "Minerals: " + res [0].ToString () + "; ";
						_playerModel.data.resourcesMinerals += res [0] * ((loser == 0) ? -1 : 1);
						_playerModel.data.resourcesMinerals = Mathf.Min (_playerModel.data.resourcesMinerals, 0);
					}

					if (res [1] > 0) {
						tmp.text += "Gas: " + res [1].ToString () + "; ";
						_playerModel.data.resourcesGas += res [1] * ((loser == 0) ? -1 : 1);
						_playerModel.data.resourcesGas = Mathf.Min (_playerModel.data.resourcesGas, 0);
					}

					if (res [2] > 0) {
						tmp.text += "Fuel: " + res [2].ToString () + "; ";
						_playerModel.data.resourcesFuel += res [2] * ((loser == 0) ? -1 : 1);
						_playerModel.data.resourcesFuel = Mathf.Min (_playerModel.data.resourcesFuel, 0);
					}

					if (res [3] > 0) {
						tmp.text += "Water: " + res [3].ToString () + "; ";
						_playerModel.data.resourcesWater += res [3] * ((loser == 0) ? -1 : 1);
						_playerModel.data.resourcesWater = Mathf.Min (_playerModel.data.resourcesWater, 0);
					}

					if (res [4] > 0) {
						tmp.text += "Food: " + res [4].ToString () + "; ";
						_playerModel.data.resourcesFood += res [4] * ((loser == 0) ? -1 : 1);
						_playerModel.data.resourcesFood = Mathf.Min (_playerModel.data.resourcesFood, 0);
					}

					if (res [5] > 0) {
						tmp.text += "Medical Supplies: " + res [5].ToString () + "; ";
						_playerModel.data.resourcesMeds += res [5] * ((loser == 0) ? -1 : 1);
						_playerModel.data.resourcesMeds = Mathf.Min (_playerModel.data.resourcesMeds, 0);
					}

					if (res [7] > 0) {
						tmp.text += _factionModel.getFactionCurrency (1) + ": " + res [7].ToString () + "; ";
						_playerModel.data.resourcesFaction1 += res [7];
						_playerModel.data.resourcesFaction1 = Mathf.Min (_playerModel.data.resourcesFaction1, 0);
					}

					if (res [8] > 0) {
						tmp.text += _factionModel.getFactionCurrency (2) + ": " + res [8].ToString () + "; ";
						_playerModel.data.resourcesFaction2 += res [8];
						_playerModel.data.resourcesFaction2 = Mathf.Min (_playerModel.data.resourcesFaction2, 0);
					}

					if (res [9] > 0) {
						tmp.text += _factionModel.getFactionCurrency (3) + ": " + res [9].ToString () + "; ";
						_playerModel.data.resourcesFaction3 += res [9];
						_playerModel.data.resourcesFaction3 = Mathf.Min (_playerModel.data.resourcesFaction3, 0);
					}

					if (res [10] > 0) {
						tmp.text += _factionModel.getFactionCurrency (4) + ": " + res [10].ToString () + "; ";
						_playerModel.data.resourcesFaction4 += res [10];
						_playerModel.data.resourcesFaction4 = Mathf.Min (_playerModel.data.resourcesFaction4, 0);
					}

					if (res [11] > 0) {
						tmp.text += _factionModel.getFactionCurrency (5) + ": " + res [11].ToString () + "; ";
						_playerModel.data.resourcesFaction5 += res [11];
						_playerModel.data.resourcesFaction5 = Mathf.Min (_playerModel.data.resourcesFaction5, 0);
					}

					if (res [12] > 0) {
						tmp.text += _factionModel.getFactionCurrency (6) + ": " + res [12].ToString ();
						_playerModel.data.resourcesFaction6 += res [12];
						_playerModel.data.resourcesFaction6 = Mathf.Min (_playerModel.data.resourcesFaction6, 0);
					}

					// Modify reputations for the player faction
					_factionModel.modifyFactionReputation(1, 2, -5);
					_factionModel.modifyFactionReputation(1, 3, 5);
					_factionModel.modifyFactionReputation(1, 4, -3);
					_factionModel.modifyFactionReputation(1, 5, 5);
					_factionModel.modifyFactionReputation(1, 6, 5);

				}

			}

		}

		/* 
		 * Clear selected and targeted tiles, close context menu and inspect window, clear unit plate
		 */
		void clearSelection() {
			_actorManager.clearPlayerSelectedActor ();
			_contextMenuSet = false;
			_diamondUIController.setInspectWindow (false);
			_diamondUIController.setContextMenuBattle (false);
			_diamondUIController.setUnitPlate (false);
			clearAbilityButtonStates ();
		}

		/*** Commands and actions triggered by the player ***/

		// Call appropriate function, based on action
		public void click(BattleButtons action) {

			switch (action) {

			case BattleButtons.cmdAutoplay:
				commandsAutoplay ();
				break;
			case BattleButtons.cmdFlee:
				commandsFlee ();
				break;
			case BattleButtons.cmdReturn:
				commandsReturn ();
				break;
			case BattleButtons.ctxInspect:
				actionInspect ();
				break;
			case BattleButtons.ctxAbility1:
				actionAbility1 ();
				break;
			case BattleButtons.ctxAbility2:
				actionAbility2 ();
				break;
			case BattleButtons.ctxAbility3:
				actionAbility3 ();
				break;
			case BattleButtons.ctxAbility4:
				actionAbility4 ();
				break;
			case BattleButtons.ctxAbility5:
				actionAbility5 ();
				break;
			case BattleButtons.ctxAbility6:
				actionAbility6 ();
				break;
			case BattleButtons.ctxItem1:
				actionUseItem1 ();
				break;
			case BattleButtons.ctxItem2:
				actionUseItem2 ();
				break;

			}

		}

		// Functions assigned to the buttons on the screen
		void actionInspect() {
			populateInspectWindow (_actorManager.getPlayerTargetedUnit ());
		}

		void actionAbility(int idx) {
			Actor caster = _actorManager.getPlayerSelectedActor ();
			Actor target = _actorManager.getPlayerTargetedActor ();
			castAbility (_actors[caster.side][caster.sideIndex], _actors[target.side][target.sideIndex], idx);
		}

		void actionAbility1() {
			actionAbility (0);
		}

		void actionAbility2() {
			actionAbility (1);
		}

		void actionAbility3() {
			actionAbility (2);
		}

		void actionAbility4() {
			actionAbility (3);
		}

		void actionAbility5() {
			actionAbility (4);
		}

		void actionAbility6() {
			actionAbility (5);
		}

		void actionUseItem1() {
			actionAbility (6);
		}

		void actionUseItem2() {
			actionAbility (7);
		}

		// Commands called by the right-bottom menu buttons
		void commandsFlee() {
			gameOver(0, true); // force a game loss, making the player lose resources and dead units
			//GameStateManager.Instance.SaveGame (); // save game now, to remember all lost units
			//_mapModel.setCurrentMapByID(_mapModel.getCurrentMap().parentMapID);
			//GameStateManager.Instance.PushScene (GameScene.ExplorationMap);
		}

		void commandsAutoplay() {
			_feedbackText.text = "Clicked Autoplay. The AI will take over the player units as well.";
			_autoplay = !_autoplay;
		}

		void commandsReturn() {
			// nothing here really, maybe add stuff in the future
			GameStateManager.Instance.SaveGame (); // save game now, to remember all lost units
			_mapModel.setCurrentMapByID(_mapModel.getCurrentMap().parentMapID);
			GameStateManager.Instance.PushScene (GameScene.ExplorationMap);
		}

    }

}