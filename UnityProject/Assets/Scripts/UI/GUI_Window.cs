using UnityEngine;
using System.Collections;
using Umbra.Data;
using Umbra.Models;
using System.Collections.Generic;

namespace Umbra.UI
{
    public class GUI_Window : MonoBehaviour
    {
        private static Rect windowSize;
        private static string title;
        private static bool isItemInspector;
        private static bool isUnitInspector;
        private static bool isPerkInspector;
        private static bool isAbilityInspector;
        private static bool render;
        private Vector2 scrollPosition;
        private static string content = "";
        private static Unit unitId;
        private static Item itemId;
        private static Perk perkId;
        private static Ability abilityId;
        private static Vector2 windowLoc;
        private static float lastX;
        private static float lastY;
        public static bool permanentClose;
        private static string minimizeButtonText;
        private static bool insideTrigger;
        private static bool pinned;
        private static string pinnedText;

        public void Start()
        {
            windowSize = new Rect(0, 0, 172, 150);
            render = true;
            windowSize.x = 0;
            windowSize.y = Screen.height - 20 + 2;
            permanentClose = false;
            content = "Hover over an image to inspect";
            title = "Inspector Active";
            minimizeButtonText = "-";
            pinnedText = "^";
        }

        public static void isNothing()
        {
            isItemInspector = false;
            isUnitInspector = false;
            isPerkInspector = false;
            isAbilityInspector = false;
        }

        public static void isAbilityGUI(Ability id)
        {
            isAbilityInspector = true;
            isPerkInspector = false;
            isItemInspector = false;
            isUnitInspector = false;
            abilityId = id;
        }

        public static void isPerkGUI(Perk id)
        {
            isPerkInspector = true;
            isItemInspector = false;
            isUnitInspector = false;
            isAbilityInspector = false;
            perkId = id;
        }

        public static void isItemGUI(Item id)
        {
            isItemInspector = true;
            isUnitInspector = false;
            isPerkInspector = false;
            isAbilityInspector = false;
            itemId = id;
        }

        public static void isUnitGUI(Unit id)
        {
            isUnitInspector = true;
            isItemInspector = false;
            isPerkInspector = false;
            isAbilityInspector = false;
            unitId = id;
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.I))
            {
                if (!permanentClose)
                {
                    isItemInspector = false;
                    isUnitInspector = false;
                    isPerkInspector = false;
                    isAbilityInspector = false;
                    windowSize.x = 0;
                    windowSize.y = Screen.height - 20 + 2;
                    permanentClose = true;
                    title = "Inspector Idle";
                    minimizeButtonText = "+";
                }
                else
                {
                    permanentClose = false;
                    content = "Hover over an image to inspect";
                    title = "Inspector Active";
                    minimizeButtonText = "-";
                }
            }
        }

        public static void ShowWindow()
        {
            if (!permanentClose)
            {
                render = true;
            }
        }

        public static void HideWindow()
        {
            render = false;
        }

        public void OnGUI()
        {
            if (!permanentClose && render && !isUnitInspector && !isItemInspector && !isPerkInspector && !isAbilityInspector)
            {
                windowSize = GUI.Window(0, windowSize, MyWindow, title);
            }
            else if (!permanentClose && render && (isUnitInspector || isItemInspector || isPerkInspector || isAbilityInspector))
            {
                if (!pinned)
                {
                    lastX = Event.current.mousePosition.x - 13;
                    lastY = Event.current.mousePosition.y - 139;
                    windowSize.x = lastX;
                    windowSize.y = lastY;
                    //insideTrigger = true;
                }
                windowSize = GUI.Window(0, windowSize, MyWindow, title);
            }
            else if (permanentClose)
            {
                windowSize = GUI.Window(0, windowSize, MyWindow, title);
            }
        }

        //will pop up again when hovering over image
        public static void temporaryHide()
        {
            if (!pinned)
            {
                isItemInspector = false;
                isUnitInspector = false;
                isPerkInspector = false;
                isAbilityInspector = false;
                windowSize.x = 0;
                windowSize.y = Screen.height - 20 + 2;
                permanentClose = false;
                title = "Inspector Active";
                minimizeButtonText = "-";
            }
        }

        public static void onDraggingHide()
        {
            if (!pinned)
            {
                isItemInspector = false;
                isUnitInspector = false;
                isPerkInspector = false;
                isAbilityInspector = false;
                windowSize.x = 0;
                windowSize.y = Screen.height - 20 + 2;
                permanentClose = true;
                title = "Dragging...";
                minimizeButtonText = "+";
            }
        }

        public static void onDropShow()
        {
            if (!pinned)
            {
                permanentClose = false;
                content = "Hover over an image to inspect";
                title = "Inspector Active";
                minimizeButtonText = "-";
            }
        }

        private void MyWindow(int id)
        {
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(159), GUILayout.Height(120));
            GUILayout.Label(content);
            GUILayout.EndScrollView();

            if (isItemInspector)
            {
                content = "";
                title = "Item.name";
                if (itemId.name != "") { title = itemId.name; }
                content += "<b>Name</b>: " + itemId.name + "\n";
                content += "<b>Charges</b>: " + itemId.charges.ToString() + "\n";
                content += "<b>Can Sell</b>: " + itemId.isSellable.ToString() + "\n";
                content += "<b>Description</b>: " + itemId.description;
            }
            else if (isUnitInspector)
            {
                UnitModel um = new UnitModel();
                Unit src = um.getSourceUnit(GUI_Window.unitId.id);
                content = "";
                title = "Unit.name";
                if (unitId.name != "") { title = unitId.name; }
                content += "<b>Name</b>: " + unitId.name + "\n";
                content += "<b>Description</b>: " + unitId.description + "\n";
                content += "<b>Health</b>: " + unitId.health * unitId.healthMult + "/" + src.health * unitId.healthMult + "\n";
                content += "<b>Shield</b>: " + unitId.shield * unitId.shieldMult + "/" + src.shield * unitId.shieldMult + "\n";
                content += "<b>Armor</b>: " + unitId.armor * unitId.armorMult + "/" + src.armor * unitId.armorMult + "\n\n";
                content += "<b>Abilities</b>:\n";
                int count = 0;
                foreach (var ability in unitId.abilities)
                {
                    content += ability;
                    count++;
                    if (count == 6)
                    {
                        break;
                    }
                    else
                    {
                        content += "\n";
                    }
                }
                content += "<b>Costs</b>:\n";
                content += "<b>Minerals</b>: " + unitId.costMinerals + "\n";
                content += "<b>Gas</b>: " + unitId.costGas + "\n";
                content += "<b>Fuel</b>: " + unitId.costFuel + "\n";
                content += "<b>Water</b>: " + unitId.costWater + "\n";
                content += "<b>Food</b>: " + unitId.costFood + "\n";
                content += "<b>Meds</b>: " + unitId.costMeds + "\n";
                content += "<b>People</b>: " + unitId.costPeople + "\n\n";
                FactionModel fm = new FactionModel();
                List<Faction> factions = fm.data;
                content += "<b>" + factions[1].name + "</b>:\n" + unitId.costFaction1 + "\n";
                content += "<b>" + factions[2].name + "</b>:\n" + unitId.costFaction2 + "\n";
                content += "<b>" + factions[3].name + "</b>:\n" + unitId.costFaction3 + "\n";
                content += "<b>" + factions[4].name + "</b>:\n" + unitId.costFaction4 + "\n";
                content += "<b>" + factions[5].name + "</b>:\n" + unitId.costFaction5 + "\n";
                content += "<b>" + factions[6].name + "</b>:\n" + unitId.costFaction6;
            }
            else if (isPerkInspector)
            {
                content = "";
                title = "Perk.name";
                if (perkId.name != "") { title = perkId.name; }
                content += "<b>Name</b>: " + perkId.name + "\n";
                content += "<b>Enabled</b>: " + perkId.isEnabled + "\n";
                content += "<b>Description</b>: " + perkId.description;
            }
            else if (isAbilityInspector)
            {
                content = "";
                title = "Ability.name";
                if (abilityId.name != "") { title = abilityId.name; }
                content += "<b>Name</b>: " + abilityId.name + "\n";
                content += "<b>Cooldown</b>: " + abilityId.cooldown.ToString() + "\n";
                content += "<b>Selfcast</b>: " + abilityId.isHarmful + "\n"; // remove later
                content += "<b>Helpful</b>: " + abilityId.isHelpful + "\n";
                content += "<b>Harmful</b>: " + abilityId.isHarmful + "\n";
                content += "<b>Friendly-fire</b>: " + abilityId.isHarmfulFriendly + "\n";
                content += "<b>Lockable</b>: " + abilityId.isLockable + "\n";
                content += "<b>Resistable</b>: " + abilityId.isResistible + "\n";
                content += "<b>Description</b>: " + abilityId.description + "\n";
            }
                

            if (GUI.Button(new Rect(153, 1, 17, 17), pinnedText))
            {
                if (!pinned)
                {
                    pinned = true;
                    pinnedText = "v";
                }
                else
                {
                    pinned = false;
                    pinnedText = "^";
                }
            }

            if (GUI.Button(new Rect(153-17, 1, 17, 17), minimizeButtonText))
            {
                if (!permanentClose)
                {
                    isItemInspector = false;
                    isUnitInspector = false;
                    isPerkInspector = false;
                    isAbilityInspector = false;
                    windowSize.x = 0;
                    windowSize.y = Screen.height - 20 + 2;
                    permanentClose = true;
                    title = "Inspector Idle";
                    minimizeButtonText = "+";
                }
                else
                {
                    permanentClose = false;
                    content = "Hover over an image to inspect";
                    title = "Inspector Active";
                    minimizeButtonText = "-";
                }
            }

            if (!permanentClose)
            {
                GUI.DragWindow();
            }
        }
    }
}