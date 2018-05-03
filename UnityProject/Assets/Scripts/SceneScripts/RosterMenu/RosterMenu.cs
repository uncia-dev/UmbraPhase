using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Umbra.Data;
using Umbra.Scenes.CharMenu;
using Umbra.Scenes.TradeMenu;
using System.Linq;
using Umbra.Models;
using Umbra.Utilities;

namespace Umbra.Scenes.RosterMenu
{
    public class RosterMenu : MonoBehaviour
    {
        public CharacterList charList;
        public GameObject charOneLbl;
        public GameObject charOneImg;
        public Sprite[] avatarSprites;
        public List<Unit> charUnits;
        public static List<Unit> filteredUnits;
        public List<GameObject> charSlot;
        public List<GameObject> filteredSlot;
        public GameObject draggableObjectPrefab;
        public static List<GameObject> instantiatedObjects;
        public Sprite noAvatarImg;
        public GameObject rosterSlotPrefab;
        public int filteredID;
        public GameObject scrollContent;
        public GameObject emptyRosterLbl;
        public GameObject noUnitsLbl;
        public static List<Unit> distinctUnits;
        public GameObject filterContent;
        public GameObject filterButtonPrefab;
        public static GameObject rosterMenuObj;
        public List<GameObject> instantiatedBtns;
        public static GameObject unitInfoContent;
        public GameObject personalSlot;
        public GameObject groundSlot;
        public GameObject flyingSlot;

        private GameState _gameState;

        CharacterModel _characterModel;
        UnitModel _unitModel;

        void Start()
        {
            _gameState = GameStateManager.Instance.gameState;

            personalSlot.SetActive(false);
            groundSlot.SetActive(false);
            flyingSlot.SetActive(false);

            DraggableObject.onTradeMenu = false;
            unitInfoContent = GameObject.Find("UnitInfoContent");
            instantiatedBtns = new List<GameObject>();
            rosterMenuObj = GameObject.Find("RosterObject");
            emptyRosterLbl.SetActive(true);
            noUnitsLbl.SetActive(true);
            DragScript.isRoster = true;
            charList = new CharacterList();
            instantiatedObjects = new List<GameObject>();

            IconLoader.Load(true);
            noAvatarImg = IconLoader.GetSpriteByName("DefaultAvatar", "Avatars");

            charOneImg.GetComponent<Image>().sprite = noAvatarImg;

            charSlot = new List<GameObject>();
            filteredSlot = new List<GameObject>();

            for (int i = 0; i < 8; i++)
            {
                charSlot.Add(GameObject.Find("USlot1_" + (i + 1)));
            }

            getDistinctUnits();
            filteredUnits = new List<Unit>();

            _characterModel = new CharacterModel();
            _unitModel = new UnitModel();

            //foreach (var character in _charModel.data)
            foreach (Character character in _characterModel.getAllCharacters(1))
            {
                charList.addChar(character);
            }

            if (charList.numChar > 0)
            {
                charList.magnified = charList.head;
                UpdateGUI();
            }
        }

        public void TransferBtn()
        {
            if (charList.numChar >= 1)
            {
                List<Unit> charOneUnits = new List<Unit>();

                //NOTE: By re-assigning a Battlegroup's list to the new list create by retrieving Units from slots, 
                //The images of the Units appear in the exact same position as Drag-and-Dropped location.
                foreach (var slot in charSlot)
                {
                    if (slot.transform.childCount > 0)
                    {
                        Unit unit = slot.transform.GetChild(0).gameObject.GetComponent<DraggableObject>().unitId;
                        //unit.parent = charList.magnified.character.dbloc.clone ();
                        _characterModel.battlegroupAddUnit(charList.magnified.character, unit);
                        charOneUnits.Add(unit);
                    }
                }
                foreach (var slot in filteredSlot)
                {
                    if (slot.transform.childCount > 0)
                    {
                        if (!slot.transform.GetChild(0).gameObject.GetComponent<DragScript>().isDisabled)
                        {
                            Unit unit = slot.transform.GetChild(0).gameObject.GetComponent<DraggableObject>().unitId;
                            _characterModel.battlegroupRemoveUnit(charList.magnified.character, unit);
                            //charList.magnified.character.battlegroupRef.RemoveUnit(unit);

                        }
                    }
                }
                if (charList.magnified.character.isPlayerCharacter)
                {

                    //PlayerModel pm = new PlayerModel();

                    // Added personal unit here as well so the player can see what units are assigned to the character
                    // The code is disabled for now, since it requires some work

                    if (personalSlot.transform.childCount > 0)
                    {
                        if (!personalSlot.transform.GetChild(0).gameObject.GetComponent<DragScript>().isDisabled)
                        {
                            Unit unitId = personalSlot.transform.GetChild(0).gameObject.GetComponent<DraggableObject>().unitId;
                            /*
                            if (charList.magnified.character.units[0] != null)
                            {
                                charList.magnified.character.units[0].battlegroupRef = null;
                            }
                            */
                            charList.magnified.character.units[0] = unitId.dbloc.clone();
                            //unitId.battlegroupRef = charList.magnified.character.battlegroupRef; -- character units do NOT go into the battlegroups
                            //pm.data.chosenPersonalUnitRef = unitId;
                        }
                    }
                    if (groundSlot.transform.childCount > 0)
                    {
                        if (!groundSlot.transform.GetChild(0).gameObject.GetComponent<DragScript>().isDisabled)
                        {
                            Unit unitId = groundSlot.transform.GetChild(0).gameObject.GetComponent<DraggableObject>().unitId;
                            /*
                            if (charList.magnified.character.units[1] != null)
                            {
                                charList.magnified.character.units[1].battlegroupRef = null;
                            }
                            */
                            charList.magnified.character.units[1] = unitId.dbloc.clone();
                            //unitId.battlegroupRef = charList.magnified.character.battlegroupRef; -- character units do NOT go into the battlegroups
                            //pm.data.chosenGroundVehicleUnitRef = unitId;
                        }
                    }
                    if (flyingSlot.transform.childCount > 0)
                    {
                        if (!flyingSlot.transform.GetChild(0).gameObject.GetComponent<DragScript>().isDisabled)
                        {
                            Unit unitId = flyingSlot.transform.GetChild(0).gameObject.GetComponent<DraggableObject>().unitId;
                            /*
                            if (charList.magnified.character.units[2] != null)
                            {
                                charList.magnified.character.units[2].battlegroupRef = null;
                            }
                            */
                            charList.magnified.character.units[2] = unitId.dbloc.clone();
                            //unitId.battlegroupRef = charList.magnified.character.battlegroupRef; -- character units do NOT go into the battlegroups
                            //pm.data.chosenFlyingVehicleUnitRef = unitId;
                        }
                    }
                }
                //charList.magnified.character.battlegroupRef.unitRef = charOneUnits; -- character units do NOT go into the battlegroups
                GameStateManager.Instance.SaveGame();
            }
            getDistinctUnits();
            UpdateGUI();
        }

        public void getDistinctUnits()
        {
            UnitModel _unitModel = new UnitModel();

            foreach (var obj in instantiatedBtns)
            {
                Destroy(obj);
            }
            instantiatedBtns.Clear();
            //distinctUnits = _unitModel.data.FindAll(unit => unit.isRecruitable == true).GroupBy(unit => unit.id).Select(grp => grp.First()).ToList();
            distinctUnits = _unitModel.getAllUnits(1).FindAll(unit => unit.isCharacter == false).GroupBy(unit => unit.id).Select(grp => grp.First()).ToList();

            if (distinctUnits.Count == 0)
            {
                noUnitsLbl.SetActive(true);
            }
            else
            {
                noUnitsLbl.SetActive(false);

                foreach (Unit u in distinctUnits)
                {
                    GameObject newObj = Instantiate(filterButtonPrefab, Vector3.zero, Quaternion.identity) as GameObject;
                    newObj.transform.localScale = new Vector3(1.335f, 1.335f, 1.335f);
                    newObj.transform.parent = filterContent.transform;
                    newObj.GetComponent<Image>().sprite = IconLoader.GetSpriteByName(u.icon, "Units");
                    newObj.GetComponent<FilterUnitScript>().unitID = u.id;
                    newObj.GetComponent<FilterUnitScript>().unitSample = u;
                    instantiatedBtns.Add(newObj);
                }
            }
        }

        public void UpdateGUI()
        {
            if (charList.magnified.character.isPlayerCharacter)
            {
                // Disable this feature due to time constraints and lack of alternative units
                //personalSlot.SetActive (true);
                //groundSlot.SetActive(true);
                //flyingSlot.SetActive(true);
            }
            else
            {
                personalSlot.SetActive(false);
                groundSlot.SetActive(false);
                flyingSlot.SetActive(false);
            }

            foreach (var obj in instantiatedObjects)
            {
                Destroy(obj);
            }
            instantiatedObjects.Clear();
            filteredSlot.Clear();

            charOneLbl.GetComponent<Text>().text = charList.magnified.character.name + "'s Battlegroup";
            charOneImg.GetComponent<Image>().sprite = IconLoader.GetSpriteByName(charList.magnified.character.icon, "Avatars");
            charUnits = _unitModel.getUnits(_characterModel.getBattlegroup(charList.magnified.character, false), false);

            if (charList.magnified.character.isPlayerCharacter)
            {
                Unit pUnit = _unitModel.getUnit(charList.magnified.character.units[0]);
                Unit gUnit = _unitModel.getUnit(charList.magnified.character.units[1]);
                Unit fUnit = _unitModel.getUnit(charList.magnified.character.units[2]);

                if (pUnit != null)
                {
                    GameObject newObj = Instantiate(draggableObjectPrefab, Vector3.zero, Quaternion.identity) as GameObject;
                    newObj.transform.localScale = new Vector3(0.97f, 0.97f, 0.97f);
                    newObj.GetComponent<Image>().sprite = IconLoader.GetSpriteByName(pUnit.icon, "Units");
                    newObj.GetComponent<DraggableObject>().setUnitId(gUnit);
                    newObj.transform.parent = personalSlot.transform;
                    instantiatedObjects.Add(newObj);
                }

                if (gUnit != null)
                {
                    GameObject newObj = Instantiate(draggableObjectPrefab, Vector3.zero, Quaternion.identity) as GameObject;
                    newObj.transform.localScale = new Vector3(0.97f, 0.97f, 0.97f);
                    newObj.GetComponent<Image>().sprite = IconLoader.GetSpriteByName(gUnit.icon, "Units");
                    newObj.GetComponent<DraggableObject>().setUnitId(gUnit);
                    newObj.transform.parent = groundSlot.transform;
                    instantiatedObjects.Add(newObj);
                }

                if (fUnit != null)
                {
                    GameObject newObj = Instantiate(draggableObjectPrefab, Vector3.zero, Quaternion.identity) as GameObject;
                    newObj.transform.localScale = new Vector3(0.97f, 0.97f, 0.97f);
                    newObj.GetComponent<Image>().sprite = IconLoader.GetSpriteByName(fUnit.icon, "Units");
                    newObj.GetComponent<DraggableObject>().setUnitId(fUnit);
                    newObj.transform.parent = flyingSlot.transform;
                    instantiatedObjects.Add(newObj);
                }
            }

            int count = 0;
            foreach (var id in charUnits)
            {
                GameObject newObj = Instantiate(draggableObjectPrefab, Vector3.zero, Quaternion.identity) as GameObject;
                newObj.transform.localScale = new Vector3(0.97f, 0.97f, 0.97f);
                newObj.GetComponent<Image>().sprite = IconLoader.GetSpriteByName(id.icon, "Units");
                newObj.GetComponent<DraggableObject>().setUnitId(id);
                newObj.transform.parent = charSlot[count++].transform;
                instantiatedObjects.Add(newObj);
            }

            if (filteredUnits.Count == 0)
            {
                emptyRosterLbl.SetActive(true);
            }
            else
            {
                emptyRosterLbl.SetActive(false);

                foreach (Unit u in filteredUnits)
                {
                    GameObject newObj = Instantiate(rosterSlotPrefab, Vector3.zero, Quaternion.identity) as GameObject;
                    newObj.transform.localScale = new Vector3(1.335f, 1.335f, 1.335f);
                    newObj.transform.parent = scrollContent.transform;
                    instantiatedObjects.Add(newObj);
                    filteredSlot.Add(newObj);

                    GameObject newObj2 = Instantiate(draggableObjectPrefab, Vector3.zero, Quaternion.identity) as GameObject;
                    newObj2.transform.localScale = new Vector3(0.97f, 0.97f, 0.97f);
                    newObj2.GetComponent<Image>().sprite = IconLoader.GetSpriteByName(u.icon, "Units");
                    newObj2.GetComponent<DraggableObject>().setUnitId(u);
                    newObj2.transform.parent = newObj.transform;
                    instantiatedObjects.Add(newObj2);

                    //if (id.battlegroupRef != null) {
                    if (!u.parent.isEmpty())
                    {
                        newObj.GetComponent<SlotObject>().isDisabled = true;
                        newObj2.GetComponent<DragScript>().isDisabled = true;
                        Color color = newObj2.GetComponent<Image>().color;
                        newObj2.GetComponent<Image>().color = Color.grey;
                    }
                    else
                    {
                        newObj.GetComponent<SlotObject>().isDisabled = false;
                        newObj2.GetComponent<DragScript>().isDisabled = false;
                    }
                }
            }
        }

        public void CreateUnitMenu()
        {
            GameStateManager.Instance.PushScene(GameScene.CreateUnitMenu);
        }

        public void LeftOne()
        {
            if (charList.numChar > 1)
            {
                charList.magnified = charList.magnified.prev;
                UpdateGUI();
            }
        }

        public void RightOne()
        {
            if (charList.numChar > 1)
            {
                charList.magnified = charList.magnified.next;
                UpdateGUI();
            }
        }

        public void returnBtn()
        {
            GameStateManager.Instance.PopScene();
        }
    }
}