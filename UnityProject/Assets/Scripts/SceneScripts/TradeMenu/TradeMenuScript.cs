using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Umbra.Data;
using Umbra.Scenes.CharMenu;
using Umbra.Models;
using Umbra.Utilities;

namespace Umbra.Scenes.TradeMenu
{
    public class TradeMenuScript : MonoBehaviour
    {
        public CharacterList charList;
        public GameObject charOneLbl;
        public GameObject charTwoLbl;
        public GameObject charOneImg;
        public GameObject charTwoImg;
        public List<Item> charOneItems;
        public List<Item> charTwoItems;
        public List<Unit> charOneUnits;
        public List<Unit> charTwoUnits;
        public GameObject[] uSlot1;
        public GameObject[] uSlot2;
        public GameObject[] iSlot1;
        public GameObject[] iSlot2;
        public GameObject draggableObjectPrefab;
        public List<GameObject> instantiatedObjects;
        public Sprite noAvatarImg;
        public GameObject noCharactersLbl;
        public static GameObject unitInfoContent;
        public static GameObject tradeMenuObj;

        private CharacterModel _characterModel;
        private UnitModel _unitModel;
        private ItemModel _itemModel;
        private GameState _gameState;

        public static Character char1;
        public static Character char2;
        public static bool specialTrade = false;

        /// <summary>
        /// Call this function when wanting initiate trade between two specific character
        /// After calling, TradeMenu is loaded
        /// </summary>
        public static void InitiateTrade(Character c1, Character c2)
        {
            specialTrade = true;
            char1 = c1;
            char2 = c2;
            GameStateManager.Instance.PushScene(GameScene.TradeMenu);
        }

        public void Trade()
        {
            if (charList.numChar >= 2 || specialTrade)
            {
                foreach (var slot in uSlot1)
                {
                    if (slot.transform.childCount > 0)
                    {
                        Unit unitId = slot.transform.GetChild(0).gameObject.GetComponent<DraggableObject>().unitId;
                        _characterModel.battlegroupAddUnit(charList.magnified.character, unitId);
                    }
                }
                foreach (var slot in uSlot2)
                {
                    if (slot.transform.childCount > 0)
                    {
                        Unit unitId = slot.transform.GetChild(0).gameObject.GetComponent<DraggableObject>().unitId;
                        _characterModel.battlegroupAddUnit(charList.magnified2.character, unitId);
                    }
                }
                foreach (var slot in iSlot1)
                {
                    if (slot.transform.childCount > 0)
                    {
                        Item itemId = slot.transform.GetChild(0).gameObject.GetComponent<DraggableObject>().itemId;
                        _characterModel.inventoryAddItem(charList.magnified.character, itemId);
                    }
                }
                foreach (var slot in iSlot2)
                {
                    if (slot.transform.childCount > 0)
                    {
                        Item itemId = slot.transform.GetChild(0).gameObject.GetComponent<DraggableObject>().itemId;
                        Debug.Log(_characterModel.inventoryAddItem(charList.magnified2.character, itemId));
                    }
                }
                GameStateManager.Instance.SaveGame();
            }
        }

        public void UpdateGUI()
        {
            foreach (var obj in instantiatedObjects)
            {
                Destroy(obj);
            }
            instantiatedObjects.Clear();

            charOneLbl.GetComponent<Text>().text = charList.magnified.character.name;
            charTwoLbl.GetComponent<Text>().text = charList.magnified2.character.name;
            charOneImg.GetComponent<Image>().sprite = IconLoader.GetSpriteByName(charList.magnified.character.icon, "Avatars");
            charTwoImg.GetComponent<Image>().sprite = IconLoader.GetSpriteByName(charList.magnified2.character.icon, "Avatars");

            charOneUnits = _unitModel.getUnits(_characterModel.getBattlegroup(charList.magnified.character, false), false);
            charOneItems = _itemModel.getItems(_characterModel.getInventory(charList.magnified.character, false), false);
            charTwoUnits = _unitModel.getUnits(_characterModel.getBattlegroup(charList.magnified2.character, false), false);
            charTwoItems = _itemModel.getItems(_characterModel.getInventory(charList.magnified2.character, false), false);

            int count = 0;
            foreach (var id in charOneUnits)
            {
                GameObject newObj = Instantiate(draggableObjectPrefab, Vector3.zero, Quaternion.identity) as GameObject;
                newObj.transform.localScale = new Vector3(1f, 1f, 1f);
                newObj.GetComponent<Image>().sprite = IconLoader.GetSpriteByName(id.icon, "Units");
                newObj.GetComponent<DraggableObject>().setUnitId(id);
                newObj.transform.parent = uSlot1[count++].transform;
                instantiatedObjects.Add(newObj);
            }

            count = 0;
            foreach (var id in charTwoUnits)
            {
                GameObject newObj = Instantiate(draggableObjectPrefab, Vector3.zero, Quaternion.identity) as GameObject;
                newObj.transform.localScale = new Vector3(1f, 1f, 1f);
                newObj.GetComponent<Image>().sprite = IconLoader.GetSpriteByName(id.icon, "Units");
                newObj.GetComponent<DraggableObject>().setUnitId(id);
                newObj.transform.parent = uSlot2[count++].transform;
                instantiatedObjects.Add(newObj);
            }

            count = 0;
            foreach (var id in charOneItems)
            {
                GameObject newObj = Instantiate(draggableObjectPrefab, Vector3.zero, Quaternion.identity) as GameObject;
                newObj.transform.localScale = new Vector3(1f, 1f, 1f);
                newObj.GetComponent<Image>().sprite = IconLoader.GetSpriteByName(id.icon, "Items");
                newObj.GetComponent<DraggableObject>().setItemId(id);
                newObj.transform.parent = iSlot1[count++].transform;
                instantiatedObjects.Add(newObj);
            }

            count = 0;
            foreach (var id in charTwoItems)
            {
                GameObject newObj = Instantiate(draggableObjectPrefab, Vector3.zero, Quaternion.identity) as GameObject;
                newObj.transform.localScale = new Vector3(1f, 1f, 1f);
                newObj.GetComponent<Image>().sprite = IconLoader.GetSpriteByName(id.icon, "Items");
                newObj.GetComponent<DraggableObject>().setItemId(id);
                newObj.transform.parent = iSlot2[count++].transform;
                instantiatedObjects.Add(newObj);
            }
        }

        public void returnBtn()
        {
            specialTrade = false;
            GameStateManager.Instance.PopScene();
        }

        public void LeftOne()
        {
            if (charList.numChar > 2)
            {
                charList.magnified = charList.magnified.prev;
                //while character on left side (magnified) is equal to the right side (magnified2), keep looping
                while (charList.magnified == charList.magnified2)
                {
                    charList.magnified = charList.magnified.prev;
                }
                UpdateGUI();
            }
        }

        public void RightOne()
        {
            if (charList.numChar > 2)
            {
                charList.magnified = charList.magnified.next;
                while (charList.magnified == charList.magnified2)
                {
                    charList.magnified = charList.magnified.next;
                }
                UpdateGUI();
            }
        }

        public void LeftTwo()
        {
            if (charList.numChar > 2)
            {
                charList.magnified2 = charList.magnified2.prev;
                while (charList.magnified == charList.magnified2)
                {
                    charList.magnified2 = charList.magnified2.prev;
                }
                UpdateGUI();
            }
        }
        public void RightTwo()
        {
            if (charList.numChar > 2)
            {
                charList.magnified2 = charList.magnified2.next;
                while (charList.magnified == charList.magnified2)
                {
                    charList.magnified2 = charList.magnified2.next;
                }
                UpdateGUI();
            }
        }


        void Start()
        {
            _gameState = GameStateManager.Instance.gameState;

            DraggableObject.onTradeMenu = true;
            tradeMenuObj = GameObject.Find("TradeMenuObj");
            unitInfoContent = GameObject.Find("UnitInfoContent");
            noCharactersLbl.SetActive(false);
            DragScript.isRoster = false;
            charList = new CharacterList();
            instantiatedObjects = new List<GameObject>();

            IconLoader.Load();
            noAvatarImg = IconLoader.GetSpriteByName("DefaultAvatar", "Avatars");

            charOneImg.GetComponent<Image>().sprite = noAvatarImg;
            charTwoImg.GetComponent<Image>().sprite = noAvatarImg;

            uSlot1 = new GameObject[8];
            uSlot2 = new GameObject[8];
            iSlot1 = new GameObject[6];
            iSlot2 = new GameObject[6];

            for (int i = 0; i < 8; i++)
            {
                uSlot1[i] = GameObject.Find("USlot1_" + (i + 1));
                uSlot2[i] = GameObject.Find("USlot2_" + (i + 1));
                if (i <= 5)
                {
                    iSlot1[i] = GameObject.Find("ISlot1_" + (i + 1));
                    iSlot2[i] = GameObject.Find("ISlot2_" + (i + 1));
                }
            }

            _characterModel = new CharacterModel();
            _unitModel = new UnitModel();
            _itemModel = new ItemModel();

            foreach (Character character in _characterModel.getAllCharacters(1))
            {
                charList.addChar(character);
            }

            if (charList.numChar > 0)
            {
                noCharactersLbl.SetActive(false);
                charList.magnified = charList.head;
                charList.magnified2 = charList.head.next;

                if (specialTrade)
                {
                    charList.magnified.character = char1;
                    charList.magnified2.character = char2;
                    charList.numChar = 0;
                    
                }
                UpdateGUI();
            }
            else
            {
                noCharactersLbl.SetActive(true);
                unitInfoContent.SetActive(false);
            }
        }
    }
}