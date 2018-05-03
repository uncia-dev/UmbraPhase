using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Umbra.Data;
using Umbra.Scenes.TradeMenu;
using System.Linq;
using Umbra.Models;
using Umbra.Utilities;
using Umbra;
using Umbra.Scenes.RosterMenu;
using System.Reflection;
using System;

namespace Umbra.CreateUnitMenu
{
    public class CreateUnitScript : MonoBehaviour
    {

        public Canvas Canvas;
        public GameObject warningLabelPrefab;
        public List<GameObject> warningLabels;
        public GameObject avatarImage;
        public UnitModel _unitModel;
        public GameObject nameField;
        public GameObject noUnitsLbl;
        public List<Unit> distinctUnits;
        public GameObject filterContent;
        public GameObject filterButtonPrefab;
        public static string unitID;
        public static GameObject lastClicked;
        public GameObject unitCreatedLbl;
        public float timer;
        public static GameObject[] resourceLabels;
        public GameObject resourceLbl;
        public GameObject hoveringObj;

        public void onHover(GameObject obj)
        {
            hoveringObj = obj;

            resourceLbl.SetActive(true);
            resourceLbl.GetComponent<Text>().text = obj.name.Substring(0, obj.name.Length - 3);
            resourceLbl.transform.position = new Vector3(hoveringObj.transform.position.x - 45, hoveringObj.transform.position.y, 0);
        }

        public void onExit()
        {
            resourceLbl.SetActive(false);
        }


        void Start()
        {
            //GameStateManager.Instance.LoadGame();
            warningLabels = new List<GameObject>();

            _unitModel = new UnitModel();

            resourceLabels = GameObject.FindGameObjectsWithTag("ResourceCounts");
            resourceLbl = GameObject.Find("ResourceCountLbl");
            resourceLbl.SetActive(false);

            //avatarImage = GameObject.Find("CharOne");
            //avatarImage.GetComponent<Image>().sprite = IconLoader.GetCurrentAvatarSprite();

            unitCreatedLbl = GameObject.Find("UnitCreatedLbl");
            noUnitsLbl = GameObject.Find("NoUnitsLbl");
            nameField = GameObject.Find("UnitName");
            filterContent = GameObject.Find("Content");
            unitCreatedLbl.SetActive(false);

            IconLoader.Load();

            timer = 0f;
            getDistinctUnits();


        }

        public void getDistinctUnits()
        {
            //distinctUnits = _unitModel.data.FindAll(unit => unit.isRecruitable == true).GroupBy(unit => unit.id).Select(grp => grp.First()).ToList();


            // TODO modify this to also only display units from friendly factions.
            // In other words, if faction1's (player) reputation with another faction is at 75 or above, you can 
            // recruit their units
            distinctUnits = _unitModel.getAllUnits().FindAll(unit => unit.isRecruitable == true).GroupBy(unit => unit.id).Select(grp => grp.First()).ToList();

            List<Unit> friendly = new List<Unit>();
            PlayerModel pModel = new PlayerModel();
            FactionModel fModel = new FactionModel();
            List<int> playerRep = fModel.data[1].reputations;
            StarbaseModel shipModel = new StarbaseModel();

            foreach (var unit in distinctUnits)
            {
                bool unitIsGood = false;

                if (unit.factionIdx == 1)
                {
                    unitIsGood = true;
                }
                if (unit.factionIdx != 1)
                {
                    if (playerRep[unit.factionIdx-1] >= 75 || playerRep[unit.factionIdx-1] == -1) //remove unit.factionIdx-1
                    {
                        unitIsGood = true;
                    }
                }
                if (unitIsGood)
                {
                    bool reqGoodSoFar = true;

                    foreach (var reqSection in unit.requiredSections)
                    {
                        foreach (var section in shipModel.spaceShipSections)
                        {
                            if (section.Value.name.Replace(" ", "").Equals(reqSection))
                            {
                                if (!(section.Value.levelCurrent > 1)) //has been upgraded at least once
                                {
                                    reqGoodSoFar = false;
                                }
                            }
                        }
                    }
                    if (reqGoodSoFar)
                    {
                        friendly.Add(unit);
                    }
                }
            }

            distinctUnits = friendly;

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
                    newObj.GetComponent<FilterUnitScript>().onCreateUnitMenu = true;
                    newObj.GetComponent<FilterUnitScript>().unitID = u.id;
                    newObj.GetComponent<FilterUnitScript>().unitSample = u;
                }
            }
        }

        public void returnBtn()
        {
            GameStateManager.Instance.PopScene();
        }

        public void newUnitSubmit()
        {
            foreach (var label in warningLabels)
            {
                Destroy(label);
            }
            warningLabels.Clear();

            if (fieldsAreGood())
            {
                PlayerModel _playerModel = new PlayerModel();
                Unit trainer = lastClicked.GetComponent<FilterUnitScript>().unitSample;
                Unit newUnit = _unitModel.createUnit(unitID, 1);
                if (_playerModel.data.resourcesPeople >= trainer.costPeople
                    && _playerModel.data.resourcesMinerals >= trainer.costMinerals
                    && _playerModel.data.resourcesGas >= trainer.costGas
                    && _playerModel.data.resourcesGas >= trainer.costGas
                    && _playerModel.data.resourcesFood >= trainer.costFood
                    && _playerModel.data.resourcesWater >= trainer.costWater
                    && _playerModel.data.resourcesMeds >= trainer.costMeds
                    && _playerModel.data.resourcesFuel >= trainer.costFuel
                    )
                {
                    unitCreatedLbl.GetComponent<Text>().text = "Unit Created!";
                    unitCreatedLbl.GetComponent<Text>().color = Color.green;
                    unitCreatedLbl.SetActive(true);
                    timer = 2.0f;

                    //subtract the resources
                    _playerModel.data.resourcesMinerals -= trainer.costMinerals;
                    _playerModel.data.resourcesGas -= trainer.costGas;
                    _playerModel.data.resourcesFood -= trainer.costFood;
                    _playerModel.data.resourcesWater -= trainer.costWater;
                    _playerModel.data.resourcesMeds -= trainer.costMeds;
                    _playerModel.data.resourcesFuel -= trainer.costFuel;
                    _playerModel.data.resourcesPeople -= trainer.costPeople;
                    GameStateManager.Instance.SaveGame();
                }
                else
                {
                    unitCreatedLbl.GetComponent<Text>().text = "Not enough resources!";
                    unitCreatedLbl.GetComponent<Text>().color = Color.red;
                    unitCreatedLbl.SetActive(true);
                    timer = 2.0f;
                }
            }
        }

        public void LeftArrowBtn()
        {
            //avatarImage.GetComponent<Image>().sprite = IconLoader.NavigateLeft();
        }

        public void RightArrowBtn()
        {
            //avatarImage.GetComponent<Image>().sprite = IconLoader.NavigateRight();
        }

        //most validation already handled by game object components
        public bool fieldsAreGood()
        {
            bool isGood = true;

            /*
            string unitName = nameField.GetComponent<Text>().text;
            if (unitName == null || unitName == "")
            {
                isGood = false;
                createWarning(nameField.transform.parent.gameObject, "Enter a valid name.");
            }
            */
            if (unitID == null || unitID == "" || lastClicked == null)
            {
                isGood = false;
                createWarning(GameObject.Find("PopCountTitleLbl"), "Select a unit type.");
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

        void Update()
        {
            if (timer > 0)
            {
                timer -= Time.deltaTime;
            }
            if (unitCreatedLbl.activeSelf && timer <= 0)
            {
                unitCreatedLbl.SetActive(false);
            }
        }
    }
}