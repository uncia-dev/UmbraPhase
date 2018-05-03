using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using sceneParam = System.Collections.Generic.KeyValuePair<string, string>;
using Umbra.Managers;
using Umbra.Data;
using Umbra;
using Umbra.Models;
using System.Collections.Generic;
using Umbra.Utilities;
using System.IO;
using System.Linq;

namespace Umbra.Scenes.NewGame
{
    public class NewGame : MonoBehaviour
    {
        public Canvas Canvas;

        public GameObject warningLabelPrefab;
        public List<GameObject> warningLabels;
        public GameObject avatarImage;
        public GameObject helperInfo;
        public GameObject savePanel;
        public string saveGameName;
        public int currentClass = 0;
        public string[] classes;
        public GameObject classLabel;
        public GameObject leftClass;
        public GameObject rightClass;
        public GameObject returnBtn;
        public static Unit personalUnit = null;
        public GameObject storyIntroPanel;
        public bool dataGood;
        public GameObject introStoryContent;
        public float timer;
        public int frames;
        public bool isScrolling;
        public GameObject gameLogoLbl;
        public GameObject titleLbl;

        public void LeftArrowBtn()
        {
            avatarImage.GetComponent<Image>().sprite = IconLoader.NavigateLeft();
        }

        public void RightArrowBtn()
        {
            avatarImage.GetComponent<Image>().sprite = IconLoader.NavigateRight();
        }

        public void LeftClass()
        {
            currentClass--;
            if (currentClass == -1)
            {
                currentClass = 2;
            }
            classLabel.GetComponent<Text>().text = classes[currentClass];
        }

        public void RightClass()
        {
            currentClass++;
            if (currentClass == 3)
            {
                currentClass = 0;
            }
            classLabel.GetComponent<Text>().text = classes[currentClass];
        }

        // Use this for initialization
        void Start()
        {
            frames = 0;
            timer = 2.0f;
            classes = new string[3] { "Soldier", "Saboteur", "Battle Technician" };
            warningLabels = new List<GameObject>();
			IconLoader.Load(true, new List<string>(){
				"Female_01Avatar", "Female_02Avatar", 
				"Male_01Avatar", "Male_02Avatar"
			});
            avatarImage = GameObject.Find("CharOne");
            avatarImage.GetComponent<Image>().sprite = IconLoader.GetCurrentAvatarSprite();
            classLabel.GetComponent<Text>().text = classes[0];
            storyIntroPanel.SetActive(false);
        }

        public void CreateButtonDelegator()
        {

            if (savePanel.activeSelf)
            {
                InputField[] inf = Canvas.GetComponentsInChildren<InputField>();
                string saveName = inf[2].text;
                if (saveName == null || saveName == "")
                {
                    foreach (var label in warningLabels)
                    {
                        Destroy(label);
                    }
                    createWarning(inf[2].gameObject, "Enter a valid save name.");
                }
                else
                {
                    returnBtn.SetActive(false);
                    foreach (var label in warningLabels)
                    {
                        Destroy(label);
                    }
                    saveGameName = saveName;
                    GameStateManager.Instance.CreateNewGame(saveGameName);
                    GameStateManager.Instance.SaveGame();
                    savePanel.SetActive(false);
                    helperInfo.GetComponent<Text>().text = "Create your personal character";
                }

            }
            else if (!savePanel.activeSelf && !dataGood)
            {
                newCharSubmit();
            }
            else if (storyIntroPanel.activeSelf && dataGood)
            {
                GameObject.Find("HelperInfoLbl").GetComponent<Text>().text = "Loading, please wait...";
                GameStateManager.Instance.PushScene(GameScene.StarMap);
            }
        }

        //NOTE: A new character should be created alongside player.
        public void newCharSubmit()
        {
            //destroy existing warning labels
            foreach (var label in warningLabels)
            {
                Destroy(label);
            }

            InputField[] inf = Canvas.GetComponentsInChildren<InputField>();

            if (fieldsAreGood(inf))
            {
                PlayerModel _playerModel = new PlayerModel();
                CharacterModel _characterModel = new CharacterModel();
				Player player = _playerModel.data;
                StarbaseModel _starbaseModel = new StarbaseModel();
                Spaceship ship = _starbaseModel.data;
				Character playerCharacter = _characterModel.getPlayerCharacter();

                ship.name = inf[1].text.Trim();

                //Player associated assignments
                //player.name = inf[0].text.Trim();
				player.name = inf[0].text.Trim();
                //player.level = 1;
				player.characterLevel = 1;
                player.icon = IconLoader.GetCurrentAvatarName();
				player.gender = determineGender();
				player.className = GameObject.Find ("ClassLabel").GetComponent<Text> ().text;
                //player.characterRef = character;

				/* No longer need this code since the XML Loader takes over
	            try
                {
                    //retrieve a unit of unitID HF1_... so long as the battlegroup reference is empty indicating no other character is using it
                    character.unitPersonalRef = uM.data.First(u => u.id == "HF1_" + determinePersonalUnit() && u.battlegroupRef == null);
                    player.chosenPersonalUnitRef = character.unitPersonalRef;
                    character.unitPersonalRef.battlegroupRef = bg;
                }
                catch (System.Exception ex) { Debug.Log("HF1_" + classes[currentClass] + " not found!"); }

                Rank rank = null;
                try
                {
                    rank = GameStateManager.Instance.gameState.rankDictionary["F1Rank6"];
                }
                catch (System.Exception ex) { }
                if (rank != null)
                {
                    character.rankRef = rank;
                }
                character.isPlayerCharacter = true;
                character.battlegroupRef = bg;
				character.name = player.name;
                character.level = 1;
                character.className = classes[currentClass];
                character.icon = IconLoader.GetCurrentAvatarName();
                character.gender = determineGender();
                */

				if (playerCharacter != null) {

					playerCharacter.name = player.name;
					playerCharacter.gender = player.gender;
					playerCharacter.className = player.className;
					playerCharacter.icon = IconLoader.GetCurrentAvatarName();

					if (player.className == "Saboteur") {
						playerCharacter.unitGroundVehicleID = "HF1Saboteur_GroundVehicle";
						playerCharacter.unitFlyingVehicleID = "HF1Saboteur_FlyingVehicle";
					} else if (player.className == "Battle Technician") {
						playerCharacter.unitGroundVehicleID = "HF1BattleTechnician_GroundVehicle";
						playerCharacter.unitFlyingVehicleID = "HF1BattleTechnician_FlyingVehicle";
					} else {
						playerCharacter.unitGroundVehicleID = "HF1Soldier_GroundVehicle";
						playerCharacter.unitFlyingVehicleID = "HF1Soldier_FlyingVehicle";
					}

					//User-friendly notification in case loading times are slow - let users know the game didn't freeze
					GameObject.Find ("HelperInfoLbl").GetComponent<Text> ().text = "";
					GameObject.Find ("ProfileTitleLbl").SetActive (false);

					if (SettingsMenu.SettingsMenuScript.hasBeenUpdated) {
						GameStateManager.Instance.settings = SettingsMenu.SettingsMenuScript.updatedSettings;
					}

					initializeRosters ();
					GameStateManager.Instance.SaveGame ();
					dataGood = true;
					storyIntroPanel.SetActive (true);

					//printRosters ();

				} // else the game will break anyway; failed to load the XMLs it seems

            }
        }

		/*
		 * Fully populate each roster by converting all ID fields to objects, and assigning these objects to their
		 * corresponding references. This method only needs to be run when starting a new game.
		 */
		public void initializeRosters() {
			CharacterModel _characterModel = new CharacterModel ();
			foreach (string cid in _characterModel.data.Keys) {
				for (int i = 0; i < _characterModel.numFactions; i++) {
					for (int j = 0; j < _characterModel.data [cid] [i].Count; j++) {
						_characterModel.initializeCharacter (_characterModel.data[cid][i][j]);
					}
				}
			}
		}

		/*
		 * Print every faction, its characters and each character's units, rank and perks
		 */
		public void printRosters() {

			FactionModel _factionModel = new FactionModel ();
			CharacterModel _characterModel = new CharacterModel ();
			UnitModel _unitModel = new UnitModel ();
			ItemModel _itemModel = new ItemModel ();
			PerkModel _perkModel = new PerkModel ();
			RankModel _rankModel = new RankModel ();

			foreach (Faction f in _factionModel.data) {

				Debug.Log ("Faction " + f.factionIdx + ": " + f.name);

				foreach (Character c in _characterModel.getAllCharacters(f.factionIdx)) {
			
					string strout = 
						"+--" + c.name +
						" (Level " + c.level + " " + c.className + " - " + 
						_rankModel.getRankById(c.rankID).name + ") UNITS: ";

					foreach (Unit u in _unitModel.getUnits(_characterModel.getBattlegroup(c))) {
						strout += "[" + u.id + "] ";
					}
					strout += "PERKS: ";

					foreach (string p in c.perkIDs) {
						strout += "[" + _perkModel.getPerkById(p) + "] ";
					}

					strout += "ITEMS: ";
					foreach (Item i in _itemModel.getItems(_characterModel.getInventory(c))) {
						strout += "[" + i.id + "] ";
					}
					Debug.Log (strout);

				}

			}

		}

		/*
        public string determinePersonalUnit()
        {
            if (classes[currentClass].Equals("Battle Technician"))
            {
                return "BattleTechnician";
            }
            return classes[currentClass];
        }
        */

        //Based on sprite name - therefore, these sprites must be properly named according to gender
        public string determineGender()
        {
            if (IconLoader.GetCurrentAvatarSprite().name.Contains("Male"))
            {
                return "Male";
            }
            else if (IconLoader.GetCurrentAvatarSprite().name.Contains("Female"))
            {
                return "Female";
            }
            else
            {
                return "Unspecified";
            }
        }

        //most validation already handled by game object components
        public bool fieldsAreGood(InputField[] inf)
        {
            bool isGood = true;
            string name = inf[0].text;
            string shipName = inf[1].text;

            if (name == null || name == "")
            {
                isGood = false;
                createWarning(inf[0].gameObject, "Enter a valid name.");
            }
            if (shipName == null || shipName == "")
            {
                isGood = false;
                createWarning(inf[1].gameObject, "Enter a valid ship name.");
            }
            return isGood;
        }

        //instantiates warning label prefab and sets its location relative to the corresponding field
        //adds it to a list to be later destroyed
        public void createWarning(GameObject field, string message)
        {
            GameObject newLabel = Instantiate(warningLabelPrefab, Vector3.zero, Quaternion.identity) as GameObject;
            newLabel.transform.parent = Canvas.transform;
            newLabel.transform.position = new Vector3(field.transform.position.x + field.GetComponent<RectTransform>().rect.width, field.transform.position.y, 0f);
            newLabel.GetComponent<Text>().text = message;
            warningLabels.Add(newLabel);
        }

        public void returnToMenu()
        {
            GameStateManager.Instance.PushScene(GameScene.MainMenu);
        }

        void Update()
        {
            if ((storyIntroPanel && introStoryContent) && storyIntroPanel.activeSelf && !(introStoryContent.transform.position.y <= 0) && !(frames >= 360))
            {
                if (frames > 0)
                {
                    gameLogoLbl.SetActive(false);
                    titleLbl.GetComponent<Text>().text = "Umbra Phase";
                    titleLbl.SetActive(true);
                    isScrolling = true;
                }
                if (timer > 0)
                {
                    timer -= Time.deltaTime;
                }
                if (timer <= 0)
                {
                    introStoryContent.transform.position = new Vector3(introStoryContent.transform.position.x, introStoryContent.transform.position.y + 1, introStoryContent.transform.position.z);
                    timer = 0.05f;
                    frames++;
                    isScrolling = true;
                }
            }
        }
    }
}