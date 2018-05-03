using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

namespace Umbra.Scenes.TradeMenu
{
    public class DragScript : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
    {

        public static GameObject draggedObject;
        public static Vector3 startPos;
        public static Transform startParent;
        public static Transform lastParent;
        public static GameObject menuBox;
        public static bool overSlot;
        public static bool hasDropped;
        public static bool isRoster;
        public static GameObject scrollView;
        public bool isDisabled;

        void Start()
        {
            hasDropped = true;
            menuBox = GameObject.Find("MenuBoxImage");
            if (isRoster && scrollView == null)
            {
                scrollView = GameObject.Find("ScrollView");
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (!isDisabled)
            {
                if (hasDropped == true)
                {
                    Umbra.UI.GUI_Window.onDraggingHide();
                    hasDropped = false;
                    overSlot = false;
                    lastParent = gameObject.transform.parent;

                    draggedObject = gameObject;
                    startPos = transform.position;
                    startParent = transform.parent;
                    GetComponent<CanvasGroup>().blocksRaycasts = false;

                }
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!isDisabled)
            {
                transform.position = Input.mousePosition; gameObject.transform.parent = menuBox.transform;
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!isDisabled)
            {
                Umbra.UI.GUI_Window.onDropShow();
                draggedObject = null;
                GetComponent<CanvasGroup>().blocksRaycasts = true;
                if (!overSlot)
                {
                    gameObject.transform.parent = lastParent;
                    transform.position = startPos;
                }
                hasDropped = true;
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
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

        public void OnPointerExit(PointerEventData eventData)
        {
            Umbra.UI.GUI_Window.isNothing();
            Umbra.UI.GUI_Window.temporaryHide();
        }
    }
}