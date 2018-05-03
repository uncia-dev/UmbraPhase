using UnityEngine;
using System.Collections;
using Umbra.Data;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Umbra.Scenes.TradeMenu
{
    public class DraggableObject : MonoBehaviour
    {
        public Unit unitId;
        public Item itemId;
        public Perk perkId;
        public static bool onTradeMenu;

        public void defaultValues()
        {
            unitId = null;
            itemId = null;
            perkId = null;
        }

        public void setUnitId(Unit id)
        {
            unitId = id;
            itemId = null;
            perkId = null;
        }

        public void setItemId(Item id)
        {
            itemId = id;
            unitId = null;
            perkId = null;
        }

        public void setPerkId(Perk id)
        {
            perkId = id;
            unitId = null;
            itemId = null;
        }

        public void onHover()
        {
            if (!Umbra.UI.GUI_Window.permanentClose)
            {
                if (GetComponent<DraggableObject>().unitId != null)
                {
                    Umbra.UI.GUI_Window.isUnitGUI(GetComponent<DraggableObject>().unitId);
                    Umbra.UI.GUI_Window.ShowWindow();
                }
                else if (GetComponent<DraggableObject>().itemId != null)
                {
                    Umbra.UI.GUI_Window.isItemGUI(GetComponent<DraggableObject>().itemId);
                    Umbra.UI.GUI_Window.ShowWindow();
                }
                else if (GetComponent<DraggableObject>().perkId != null)
                {
                    Umbra.UI.GUI_Window.isPerkGUI(GetComponent<DraggableObject>().perkId);
                    Umbra.UI.GUI_Window.ShowWindow();
                }
                else
                {
                    Umbra.UI.GUI_Window.isNothing();
                }
            }
        }

        public void onExit()
        {
            Umbra.UI.GUI_Window.isNothing();
            Umbra.UI.GUI_Window.temporaryHide();
        }
    }
}