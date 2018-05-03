using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Umbra.Data;
using Umbra.Scenes.OutpostMenu;
using System;
using Umbra.Managers;
using Umbra.Models;
using Umbra.Utilities;
using System.Collections.Generic;

namespace Umbra.Scenes.CharMenu
{
    public class CharacterList
    {
        public CharNode head;
        public CharNode tail;
        public CharNode curr;
        public CharNode magnified;
        public CharNode magnified2;
        public int numChar;

        public CharacterList()
        {
            head = null;
            tail = null;
            curr = null;
            numChar = 0;
        }

        public void addChar(Character occupant)
        {
            numChar++;

			/*
            CharNode cN = null;

            if (head == null)
            {
                // insert a node whose next and prev values are the node itself
                cN = new CharNode(occupant, cN, cN);
                head = cN;
                tail = cN;
            }
            else
            {
                // insert node at the tail, and link the head and tail via this node
                cN = new CharNode(occupant, tail, head);
                tail.next = cN;
                head.prev = cN;
            }
			*/
            
            CharNode cN = null;
            if (tail == null)
            {
                cN = new CharNode(occupant, head, null);
                head = cN;
                tail = cN;
                if (numChar == 1)
                {
                    head.next = tail;
                    head.prev = tail;
                    tail.next = head;
                    tail.prev = head;
                }
            }
            else
            {
                cN = new CharNode(occupant, head, null);
                head.prev = cN;
                head = cN;
                head.prev = tail;
                tail.next = head;
            }
            
        }
    }

    public class CharNode
    {
        public Character character;
        public CharNode prev;
        public CharNode next;

        public CharNode()
        {
            character = null;
            prev = null;
            next = null;
        }

        public CharNode(Character occupant, CharNode n, CharNode p)
        {
            character = occupant;
            next = n;
            prev = p;
        }
    }

    public class CharMenuScript : MonoBehaviour
    {
        public CharacterList charList;
        public Sprite[] avatarSprites;
        public const int NUM_AVATAR = 5;
        public GameObject currCharNameLbl;
        public GameObject[] avatarImgs;
        public GameObject levelLbl;
        public GameObject rankLbl;
        public GameObject experienceLbl;
        public GameObject classLbl;
        public GameObject battlesFoughtLbl;
        public GameObject backgroundInfoLbl;
        public GameObject[] unitImgs;
        public Sprite noAvatarImg;
        public GameObject infantryImg;
        public GameObject groundImg;
        public GameObject flyingImg;
        public GameObject[] perkImgs;
        public GameObject[] itemImgs;
        public GameObject leftBtn;
        public GameObject rightBtn;
        public GameObject noProfileLbl;

        public GameObject[] itemHightlights;

        private CharacterModel _characterModel;
        private UnitModel _unitModel;
        private RankModel _rankModel;
        private PerkModel _perkModel;
        private ItemModel _itemModel;

        void Start()
        {
            //GameStateManager.Instance.LoadGame();
            charList = new CharacterList();
            avatarSprites = new Sprite[5];
            avatarImgs = new GameObject[7];
            unitImgs = new GameObject[8];
            perkImgs = new GameObject[3];
            itemImgs = new GameObject[6];

            currCharNameLbl = GameObject.Find("CurrCharNameLbl");
            avatarImgs[0] = GameObject.Find("CharOneImg");
            avatarImgs[1] = GameObject.Find("CharTwoImg");
            avatarImgs[2] = GameObject.Find("CharThreeImg");
            avatarImgs[3] = GameObject.Find("CurrCharImg");
            avatarImgs[4] = GameObject.Find("CharFiveImg");
            avatarImgs[5] = GameObject.Find("CharSixImg");
            avatarImgs[6] = GameObject.Find("CharSevenImg");

            leftBtn = GameObject.Find("LeftBtn");
            rightBtn = GameObject.Find("RightBtn");
            noProfileLbl = GameObject.Find("NoProfileLbl");

            unitImgs[0] = GameObject.Find("UnitOneImg");
            unitImgs[1] = GameObject.Find("UnitTwoImg");
            unitImgs[2] = GameObject.Find("UnitThreeImg");
            unitImgs[3] = GameObject.Find("UnitFourImg");
            unitImgs[4] = GameObject.Find("UnitFiveImg");
            unitImgs[5] = GameObject.Find("UnitSixImg");
            unitImgs[6] = GameObject.Find("UnitSevenImg");
            unitImgs[7] = GameObject.Find("UnitEightImg");

            levelLbl = GameObject.Find("LevelLbl");
            rankLbl = GameObject.Find("RankLbl");
            experienceLbl = GameObject.Find("ExperienceLbl");
            classLbl = GameObject.Find("ClassLbl");
            battlesFoughtLbl = GameObject.Find("BattlesFoughtLbl");
            backgroundInfoLbl = GameObject.Find("BackgroundLbl");

            flyingImg = GameObject.Find("FlyingVehicleButton");
            groundImg = GameObject.Find("GroundVehicleButton");
            infantryImg = GameObject.Find("InfantryButton");

            IconLoader.Load(true);

            perkImgs[0] = GameObject.Find("PerkOneImg");
            perkImgs[1] = GameObject.Find("PerkTwoImg");
            perkImgs[2] = GameObject.Find("PerkThreeImg");
            itemImgs[0] = GameObject.Find("Item1Button");
            itemImgs[1] = GameObject.Find("Item2Button");
            itemImgs[2] = GameObject.Find("Item3Button");
            itemImgs[3] = GameObject.Find("Item4Button");
            itemImgs[4] = GameObject.Find("Item5Button");
            itemImgs[5] = GameObject.Find("Item6Button");

            itemHightlights = new GameObject[6];

            for (int i = 0; i < 6; i++)
            {
                itemHightlights[i] = GameObject.Find("Item" + (i + 1).ToString() + "Highlight");
            }

            _characterModel = new CharacterModel();
            _itemModel = new ItemModel();
            _perkModel = new PerkModel();
            _unitModel = new UnitModel();
            _rankModel = new RankModel();

            //foreach(var character in _characterModel.data)
            foreach (Character character in _characterModel.getAllCharacters(1))
            {
				charList.addChar(character);
				Debug.Log (character.name);
            }

            charList.curr = charList.head;
			//charList.curr = charList.tail;
            updateGUI();
        }

        public void updateGUI()
        {
            noProfileLbl.SetActive(false);
            leftBtn.SetActive(true);
            rightBtn.SetActive(true);

            for (int i = 0; i < 7; i++)
            {
                avatarImgs[i].SetActive(true);
            }

            if (charList.numChar < 7)
            {
                double diff = 7 - charList.numChar;
                int numToHide = (int)Math.Ceiling(diff / 2);

                int i = 0;
                int j = 6;
                while (numToHide != 0)
                {
                    leftBtn.transform.position = new Vector2(avatarImgs[i].transform.position.x, leftBtn.transform.position.y);
                    rightBtn.transform.position = new Vector2(avatarImgs[j].transform.position.x, rightBtn.transform.position.y);
                    avatarImgs[i].SetActive(false);
                    avatarImgs[j].SetActive(false);
                    numToHide--;
                    i++;
                    j--;
                }
            }

            if (charList.numChar == 0)
            {
                noProfileLbl.SetActive(true);
                leftBtn.SetActive(false);
                rightBtn.SetActive(false);
            }
            else if (charList.numChar >= 1)
            {
                if (charList.numChar == 1)
                {
                    leftBtn.SetActive(false);
                    rightBtn.SetActive(false);
                }

                CharNode c = charList.curr;

                for (int i = 0; i < 7; i++)
                {
                    if (i == 3)
                    {
                        charList.magnified = c;
                    }
                    avatarImgs[i].GetComponent<Image>().sprite = IconLoader.GetSpriteByName(c.character.icon, "Avatars");
                    c = c.next; // this was disconnecting the current node from the character currently displayed
                }

                currCharNameLbl.GetComponent<Text>().text = charList.magnified.character.name;
                levelLbl.GetComponent<Text>().text = charList.magnified.character.level.ToString();
                experienceLbl.GetComponent<Text>().text = charList.magnified.character.experiencePoints.ToString();
                battlesFoughtLbl.GetComponent<Text>().text = charList.magnified.character.battleCount.ToString();
                classLbl.GetComponent<Text>().text = charList.magnified.character.className;
                rankLbl.GetComponent<Text>().text = _rankModel.getRankById(charList.magnified.character.rankID).name;
                backgroundInfoLbl.GetComponent<Text>().text = charList.magnified.character.description;
                for (int i = 0; i < 8; i++)
                {
                    unitImgs[i].GetComponent<Image>().sprite = noAvatarImg;
                    unitImgs[i].GetComponent<Umbra.Scenes.TradeMenu.DraggableObject>().defaultValues();
                }

                List<Unit> bg = _unitModel.getUnits(_characterModel.getBattlegroup(charList.magnified.character, true), true);

                //if (bg != null) { // bg will never be null; just an empty list at worst
                int unitCount = 0;
                foreach (Unit unit in bg)
                {
                    if (unit != null)
                    {
                        unitImgs[unitCount].GetComponent<Image>().sprite = IconLoader.GetSpriteByName(unit.icon, "Units");
                        unitImgs[unitCount++].GetComponent<Umbra.Scenes.TradeMenu.DraggableObject>().setUnitId(unit);
                    }
                }
                //}

                Unit personalUnit = _unitModel.getUnit(charList.magnified.character.units[0]);
                Unit groundUnit = _unitModel.getUnit(charList.magnified.character.units[1]);
                Unit flyingUnit = _unitModel.getUnit(charList.magnified.character.units[2]);
                int unitChoice = charList.magnified.character.unitCurrentIdx;

                //default values if the references are null
                groundImg.GetComponent<Image>().sprite = noAvatarImg;
                groundImg.GetComponent<Umbra.Scenes.TradeMenu.DraggableObject>().defaultValues();
                flyingImg.GetComponent<Image>().sprite = noAvatarImg;
                flyingImg.GetComponent<Umbra.Scenes.TradeMenu.DraggableObject>().defaultValues();
                infantryImg.GetComponent<Image>().sprite = noAvatarImg; // personal unit
                infantryImg.GetComponent<Umbra.Scenes.TradeMenu.DraggableObject>().defaultValues();

                SetEquippedItems();
                SetSelectedUnit(unitChoice);

                if (groundUnit != null)
                {
                    groundImg.GetComponent<Image>().sprite = IconLoader.GetSpriteByName(groundUnit.icon, "Units");
                    groundImg.GetComponent<Umbra.Scenes.TradeMenu.DraggableObject>().setUnitId(groundUnit);
                }
                if (flyingUnit != null)
                {
                    flyingImg.GetComponent<Image>().sprite = IconLoader.GetSpriteByName(flyingUnit.icon, "Units");
                    flyingImg.GetComponent<Umbra.Scenes.TradeMenu.DraggableObject>().setUnitId(flyingUnit);
                }
                if (personalUnit != null)
                {
                    infantryImg.GetComponent<Image>().sprite = IconLoader.GetSpriteByName(personalUnit.icon, "Units");
                    infantryImg.GetComponent<Umbra.Scenes.TradeMenu.DraggableObject>().setUnitId(personalUnit);
                }

                for (int i = 0; i < 3; i++)
                {
                    perkImgs[i].GetComponent<Image>().sprite = noAvatarImg;
                    perkImgs[i].GetComponent<Umbra.Scenes.TradeMenu.DraggableObject>().defaultValues();
                }

                int perkCount = 0;
                foreach (string perkID in charList.magnified.character.perkIDs)
                {
                    Perk perk = _perkModel.getPerkById(perkID);
                    perkImgs[perkCount].GetComponent<Image>().sprite = IconLoader.GetSpriteByName(perk.icon, "Perks");
                    perkImgs[perkCount++].GetComponent<Umbra.Scenes.TradeMenu.DraggableObject>().setPerkId(perk);
                }

                for (int i = 0; i < 6; i++)
                {
                    itemImgs[i].GetComponent<Image>().sprite = noAvatarImg;
                    itemImgs[i].GetComponent<Umbra.Scenes.TradeMenu.DraggableObject>().defaultValues();
                    itemImgs[i].GetComponent<Button>().interactable = false;
                }

                int itemCount = 0;
                List<Item> _items = _itemModel.getItems(_characterModel.getInventory(charList.magnified.character, true), true);
                foreach (Item item in _items)
                {
                    if (item != null)
                    {
                        itemImgs[itemCount].GetComponent<Image>().sprite = IconLoader.GetSpriteByName(item.icon, "Items");
                        itemImgs[itemCount].GetComponent<Button>().interactable = true;
                        itemImgs[itemCount++].GetComponent<Umbra.Scenes.TradeMenu.DraggableObject>().setItemId(item);
                    }
                }
            }
        }

        public void EquipItem(int idx)
        {
            if (idx >= 0 && idx < 6)
            {
                int i1 = charList.curr.character.inventoryEquipped[0];
                int i2 = charList.curr.character.inventoryEquipped[1];
                int slot = 0;
                if (itemImgs[idx].GetComponent<Image>().color == Color.blue) { slot = 0; }
                else
                {
                    //if (i1 == -1) { slot = 0; }
                    if (i2 == -1) { slot = 1; }
                }
                _characterModel.inventoryEquipItem(charList.curr.character, idx, slot);
            }
            SetEquippedItems();

        }

        public void SetEquippedItems()
        {
            int i1 = charList.curr.character.inventoryEquipped [0];
            int i2 = charList.curr.character.inventoryEquipped [1];

            for (int i = 0; i < 6; i++) {
                itemImgs[i].GetComponent<Image>().color = Color.white;
            }

            if (i1 >= 0 && i1 <= 5) itemImgs[i1].GetComponent<Image>().color = Color.blue;
            if (i2 >= 0 && i2 <= 5) itemImgs[i2].GetComponent<Image>().color = Color.blue;
            

        }

        public void SetSelectedUnit(int idx)
        {
            infantryImg.GetComponent<Image>().color = Color.white;
            groundImg.GetComponent<Image>().color = Color.white;
            flyingImg.GetComponent<Image>().color = Color.white;

            if (idx == 0)
            {
                infantryImg.GetComponent<Image>().color = Color.blue;
            }
            if (idx == 1)
            {
                groundImg.GetComponent<Image>().color = Color.blue;
            }
            if (idx == 2)
            {
                flyingImg.GetComponent<Image>().color = Color.blue;
            }
            _characterModel.setCharacterSelectedUnit(charList.magnified.character, idx);
        }

        public void clickedInfantry()
        {
            if (!charList.curr.character.units[0].isEmpty()) SetSelectedUnit(0);
        }

        public void clickedGroundVehicle()
        {
            if (!charList.curr.character.units[1].isEmpty()) SetSelectedUnit(1);
        }

        public void clickedFlyingVehicle()
        {
            if (!charList.curr.character.units[2].isEmpty()) SetSelectedUnit(2);
        }

        public void clickedItem1()
        {
            EquipItem(0);
        }

        public void clickedItem2()
        {
            EquipItem(1);
        }

        public void clickedItem3()
        {
            EquipItem(2);
        }

        public void clickedItem4()
        {
            EquipItem(3);
        }

        public void clickedItem5()
        {
            EquipItem(4);
        }

        public void clickedItem6()
        {
            EquipItem(5);
        }

        public void leftArrowClick()
        {
            charList.curr = charList.curr.prev;
            GameStateManager.Instance.SaveGame();
            updateGUI();
        }

        public void rightArrowClick()
        {
            charList.curr = charList.curr.next;
            GameStateManager.Instance.SaveGame();
            updateGUI();
        }

        public void loadRepMenu()
        {
            GameStateManager.Instance.PushScene(GameScene.ReputationMenu);
        }

        public void tradeBtn()
        {
            GameStateManager.Instance.PushScene(GameScene.TradeMenu);
        }

        public void rosterBtn()
        {
            GameStateManager.Instance.PushScene(GameScene.RosterMenu);
        }

        public void returnBtn()
        {
            GameStateManager.Instance.PopScene();
        }
    }
}