using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Umbra.Data;
using Umbra.Managers;
using Umbra.Models;
using UnityEngine.EventSystems;

namespace Umbra.Scenes.OutpostMenu
{
    public class OutpostMenuScript : MonoBehaviour
    {
        public GameObject hoveringObj;
        public GameObject resourceLbl;
        public static GameObject lastClickedModule;
        public GameObject moduleTitleLbl;
        public GameObject informationLbl;
        public GameObject[] resourceLabels;
        public bool editingName;
        public GameObject outpostNameField;
        public GameObject placeHolder;
        public GameObject editButton;
        public GameObject fieldText;
        public string selectedStructure;
        public int[] recReq;
        public bool canUpgrade;
        public GameObject selected;
        public GameObject menuBtn;
        public static bool outpostFound;
        public bool isHoveringTextField;

        private PlayerModel _playerModel;

        public static Outpost id;


        void changeOutpostName()
        {
            string newName = outpostNameField.GetComponent<InputField>().text.Trim();

            if (newName == null || newName == "")
            {
            }
            else
            {
                editingName = false;
                outpostNameField.GetComponent<InputField>().interactable = false;
                outpostNameField.GetComponent<InputField>().text = "";
                placeHolder.SetActive(true);
                editButton.SetActive(false);
                id.name = newName;
                placeHolder.GetComponent<Text>().text = newName;

                if (newName.Length > 8)
                {
                    placeHolder.GetComponent<Text>().fontSize = 20;
                    fieldText.GetComponent<Text>().fontSize = 20;
                }
                else
                {
                    placeHolder.GetComponent<Text>().fontSize = 30;
                    fieldText.GetComponent<Text>().fontSize = 30;
                }

                GameStateManager.Instance.SaveGame();
            }
        }

        public void onClickUpgrade()
        {
            if (canUpgrade)
            {
                Player player = _playerModel.data;
                player.resourcesMinerals = (player.resourcesMinerals - recReq[0]);
                player.resourcesGas = (player.resourcesGas - recReq[1]);
                player.resourcesFuel = (player.resourcesFuel - recReq[2]);
                player.resourcesWater = (player.resourcesWater - recReq[3]);
                player.resourcesFood = (player.resourcesFood - recReq[4]);
                player.resourcesMeds = (player.resourcesMeds - recReq[5]);

                if (selectedStructure.Equals("Outpost"))
                {
                    id.hasOutpost = true;
                }
                if (selectedStructure.Equals("Garrison"))
                {
                    id.hasGarrison = true;
                }
                if (selectedStructure.Equals("Force Field"))
                {
                    id.hasForceField = true;
                }
                if (selectedStructure.Equals("Turrets"))
                {
                    id.hasTurrets = true;
                }
                if (selectedStructure.Equals("Proc. Plant"))
                {
                    id.hasProcessingPlant = true;
                }
                if (selectedStructure.Equals("Med. Lab"))
                {
                    id.hasMedicalLab = true;
                }
                if (selectedStructure.Equals("Biodome"))
                {
                    id.hasBiodome = true;
                }
                if (selectedStructure.Equals("Trade Post"))
                {
                    id.hasTradingPost = true;
                }

                GameStateManager.Instance.SaveGame();

                informationLbl.GetComponent<Text>().text = selectedStructure + " has been constructed.";
                informationLbl.GetComponent<Text>().color = new Color(0, 255, 0);

                selected.GetComponent<Button>().interactable = false;
                menuBtn.GetComponent<Button>().interactable = false;
            }
            else
            {
                informationLbl.GetComponent<Text>().text = "Not enough resources to build!";
                informationLbl.GetComponent<Text>().color = new Color(255, 0, 0);
            }
        }

        public void onClickModule(GameObject module)
        {
            menuBtn.GetComponent<Button>().interactable = true;
            selected = module;
            canUpgrade = true;
            selectedStructure = module.GetComponentInChildren<Text>().text;
            informationLbl.GetComponent<Text>().color = new Color(255, 255, 255);
            moduleTitleLbl.GetComponent<Text>().text = selectedStructure;
            if (selectedStructure.Equals("Proc. Plant"))
            {
                moduleTitleLbl.GetComponent<Text>().text = "Processing Plant";
            }
            if (selectedStructure.Equals("Med. Lab"))
            {
                moduleTitleLbl.GetComponent<Text>().text = "Medical Lab";
            }

            updateLabelData();
        }

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
            _playerModel = new PlayerModel();

            //testing purposes
            id = GameStateManager.Instance.gameState.currentOutpost;
            outpostFound = true;

            menuBtn = GameObject.Find("MenuButton");
            canUpgrade = false;
            selectedStructure = "Outpost";
            editingName = false;
            outpostNameField = GameObject.Find("OutpostNameField");
            placeHolder = GameObject.Find("Placeholder");
            editButton = GameObject.Find("EditOutpostNameBtn");
            editButton.SetActive(false);
            fieldText = GameObject.Find("FieldText");
            selected = GameObject.Find("OutpostBtn");

            resourceLabels = GameObject.FindGameObjectsWithTag("ResourceCounts");
            moduleTitleLbl = GameObject.Find("SelectedModuleLbl");
            informationLbl = GameObject.Find("InformationLbl");
            resourceLbl = GameObject.Find("ResourceCountLbl");
            resourceLbl.SetActive(false);

            placeHolder.GetComponent<Text>().text = id.name;

            if (placeHolder.GetComponent<Text>().text.Length > 8)
            {
                placeHolder.GetComponent<Text>().fontSize = 20;
                fieldText.GetComponent<Text>().fontSize = 20;
            }
            else
            {
                placeHolder.GetComponent<Text>().fontSize = 30;
                fieldText.GetComponent<Text>().fontSize = 30;
            }

            if (id.hasOutpost)
            {
                selected.GetComponent<Button>().interactable = false;
                menuBtn.GetComponent<Button>().interactable = false;
            }
            if (id.hasGarrison)
            {
                GameObject.Find("GarrisonBtn").GetComponent<Button>().interactable = false;
            }
            if (id.hasForceField)
            {
                GameObject.Find("ForceFieldBtn").GetComponent<Button>().interactable = false;
            }
            if (id.hasTurrets)
            {
                GameObject.Find("TurretsBtn").GetComponent<Button>().interactable = false;
            }
            if (id.hasProcessingPlant)
            {
                GameObject.Find("ProcBtn").GetComponent<Button>().interactable = false;
            }
            if (id.hasMedicalLab)
            {
                GameObject.Find("MedBtn").GetComponent<Button>().interactable = false;
            }
            if (id.hasBiodome)
            {
                GameObject.Find("BioBtn").GetComponent<Button>().interactable = false;
            }
            if (id.hasTradingPost)
            {
                GameObject.Find("TradeBtn").GetComponent<Button>().interactable = false;
            }

            updateLabelData();
        }

        public void onHoverTextField()
        {
            isHoveringTextField = true;
        }

        public void onExitTextField()
        {
            isHoveringTextField = false;
        }

        public void editOutpostNameField()
        {
            if (!editingName)
            {
                editingName = true;
                outpostNameField.GetComponent<InputField>().interactable = true;
                outpostNameField.GetComponent<InputField>().text = placeHolder.GetComponent<Text>().text;
                placeHolder.SetActive(false);
                editButton.SetActive(true);
            }
        }

        public void updateLabelData()
        {
            Outpost o = id;

            if (selectedStructure.Equals("Outpost"))
            {
                recReq = o.outpostReq;
                informationLbl.GetComponent<Text>().text = o.outpostDesc;
            }
            if (selectedStructure.Equals("Garrison"))
            {
                recReq = o.garrisonReq;
                informationLbl.GetComponent<Text>().text = o.garrisonDesc;
            }
            if (selectedStructure.Equals("Force Field"))
            {
                recReq = o.forceFieldReq;
                informationLbl.GetComponent<Text>().text = o.forceFieldDesc;
            }
            if (selectedStructure.Equals("Turrets"))
            {
                recReq = o.turretsReq;
                informationLbl.GetComponent<Text>().text = o.turretsDesc;
            }
            if (selectedStructure.Equals("Proc. Plant"))
            {
                recReq = o.processingReq;
                informationLbl.GetComponent<Text>().text = o.processingDesc;
            }
            if (selectedStructure.Equals("Med. Lab"))
            {
                recReq = o.medicalReq;
                informationLbl.GetComponent<Text>().text = o.medicalDesc;
            }
            if (selectedStructure.Equals("Biodome"))
            {
                recReq = o.biodomeReq;
                informationLbl.GetComponent<Text>().text = o.biodomeDesc;
            }
            if (selectedStructure.Equals("Trade Post"))
            {
                recReq = o.tradingReq;
                informationLbl.GetComponent<Text>().text = o.tradingDesc;
            }
            for (int i = 0; i < 6; i++)
            {
                resourceLabels[i].GetComponent<Text>().text = recReq[i].ToString();
            }
            updateLabelColors();
        }

        public void updateLabelColors()
        {
            Player player = _playerModel.data;
            if (!(player.resourcesMinerals >= recReq[0]))
            {
                canUpgrade = false;
                resourceLabels[0].GetComponent<Text>().color = new Color(255, 0, 0);
            }
            else
            {
                resourceLabels[0].GetComponent<Text>().color = new Color(255, 255, 255);
            }

            if (!(player.resourcesGas >= recReq[1]))
            {
                canUpgrade = false;
                resourceLabels[1].GetComponent<Text>().color = new Color(255, 0, 0);
            }
            else
            {
                resourceLabels[1].GetComponent<Text>().color = new Color(255, 255, 255);
            }

            if (!(player.resourcesFuel >= recReq[2]))
            {
                canUpgrade = false;
                resourceLabels[2].GetComponent<Text>().color = new Color(255, 0, 0);
            }
            else
            {
                resourceLabels[2].GetComponent<Text>().color = new Color(255, 255, 255);
            }

            if (!(player.resourcesWater >= recReq[3]))
            {
                canUpgrade = false;
                resourceLabels[3].GetComponent<Text>().color = new Color(255, 0, 0);
            }
            else
            {
                resourceLabels[3].GetComponent<Text>().color = new Color(255, 255, 255);
            }

            if (!(player.resourcesFood >= recReq[4]))
            {
                canUpgrade = false;
                resourceLabels[4].GetComponent<Text>().color = new Color(255, 0, 0);
            }
            else
            {
                resourceLabels[4].GetComponent<Text>().color = new Color(255, 255, 255);
            }

            if (!(player.resourcesMeds >= recReq[5]))
            {
                canUpgrade = false;
                resourceLabels[5].GetComponent<Text>().color = new Color(255, 0, 0);
            }
            else
            {
                resourceLabels[5].GetComponent<Text>().color = new Color(255, 255, 255);
            }
        }

        public void returnBtn()
        {
            GameStateManager.Instance.PopScene();
        }

        // Update is called once per frame
        void Update()
        {
            if (OutpostMenuScript.outpostFound)
            {
                if (editButton.activeSelf == false && isHoveringTextField && Input.GetMouseButtonUp(1))
                {
                    editOutpostNameField();
                    EventSystem.current.SetSelectedGameObject(outpostNameField);
                }
            }
        }
    }
}
