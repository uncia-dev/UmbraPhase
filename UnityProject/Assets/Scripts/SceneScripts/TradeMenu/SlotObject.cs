using UnityEngine;
using System.Collections;
using Umbra.Data;

namespace Umbra.Scenes.TradeMenu
{
    public class SlotObject : MonoBehaviour
    {

        public bool isUnitSlot;
        public bool isDisabled;
        public bool isRemoval;
		public bool isPersonal;
        public bool isGround;
        public bool isFlying;

        //returns false if the player is not dropping a unit object into a unit slot
        //same applies to item object
        public bool canDrop(GameObject obj)
        {
            if (isDisabled)
            {
                return false;
            }
            if (isUnitSlot && obj.GetComponent<SlotObject>().isUnitSlot)
            {
                return true;
            }
            if (!isUnitSlot && !obj.GetComponent<SlotObject>().isUnitSlot && DraggableObject.onTradeMenu)
            {
                return true;
            }
            if (!DraggableObject.onTradeMenu)
            {
				if (DragScript.draggedObject.GetComponent<DraggableObject>().unitId.unitClass.Equals("Infantry") && isPersonal)
				{
					return true;
				}				
                if (DragScript.draggedObject.GetComponent<DraggableObject>().unitId.unitClass.Equals("GroundVehicle") && isGround)
                {
                    return true;
                }
                if (DragScript.draggedObject.GetComponent<DraggableObject>().unitId.unitClass.Equals("FlyingVehicle") && isFlying)
                {
                    return true;
                }
            }
            return false;
        }
    }
}