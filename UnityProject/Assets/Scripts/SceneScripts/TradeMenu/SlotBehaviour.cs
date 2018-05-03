using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using Umbra.Data;
using Umbra.Scenes.RosterMenu;
using Umbra.Scenes.CharMenu;
using Umbra.Models;

namespace Umbra.Scenes.TradeMenu
{
    public class SlotBehaviour : MonoBehaviour, IDropHandler
    {
        //graphical image swap
        public void OnDrop(PointerEventData eventData)
        {
            Umbra.UI.GUI_Window.onDropShow();
            try
            {
                DragScript.draggedObject.transform.parent = DragScript.lastParent;
                if (GetComponent<SlotObject>().isRemoval && (!DragScript.draggedObject.GetComponent<DraggableObject>().unitId.parent.isEmpty() || DragScript.lastParent.GetComponent<SlotObject>().isGround || DragScript.lastParent.GetComponent<SlotObject>().isFlying))
                {
                    if (DragScript.lastParent.GetComponent<SlotObject>().isPersonal)
                    {
                        CharacterList charList = RosterMenu.RosterMenu.rosterMenuObj.GetComponent<RosterMenu.RosterMenu>().charList;
                        PlayerModel pm = new PlayerModel();
                        Unit unitId = DragScript.draggedObject.GetComponent<DraggableObject>().unitId;
                        //unitId.battlegroupRef = null; - Character units are NOT part of a battlegroup; Battlemanager takes care of that
                        charList.magnified.character.units[0] = null;
                        //pm.data.chosenPersonalUnitRef = null;
                    }
                    else if (DragScript.lastParent.GetComponent<SlotObject>().isGround)
                    {
                        CharacterList charList = RosterMenu.RosterMenu.rosterMenuObj.GetComponent<RosterMenu.RosterMenu>().charList;
                        PlayerModel pm = new PlayerModel();
                        Unit unitId = DragScript.draggedObject.GetComponent<DraggableObject>().unitId;
                        //unitId.battlegroupRef = null; - Character units are NOT part of a battlegroup; Battlemanager takes care of that
                        charList.magnified.character.units[1] = null;
                        //pm.data.chosenGroundVehicleUnitRef = null;
                    }
                    else if (DragScript.lastParent.GetComponent<SlotObject>().isFlying)
                    {
                        CharacterList charList = RosterMenu.RosterMenu.rosterMenuObj.GetComponent<RosterMenu.RosterMenu>().charList;
                        //PlayerModel pm = new PlayerModel();
                        Unit unitId = DragScript.draggedObject.GetComponent<DraggableObject>().unitId;
                        //unitId.battlegroupRef = null; - Character units are NOT part of a battlegroup; Battlemanager takes care of that
                        charList.magnified.character.units[2] = null;
                        //pm.data.chosenFlyingVehicleUnitRef = null;
                    }
                    else
                    {
                        // TODO: FIX ME - possible fix
                        //Battlegroup bg = DragScript.draggedObject.GetComponent<DraggableObject>().unitId.battlegroupRef;
                        //bg.RemoveUnit(DragScript.draggedObject.GetComponent<DraggableObject>().unitId);
                        CharacterModel _characterModel = new CharacterModel();
                        Unit u = DragScript.draggedObject.GetComponent<DraggableObject>().unitId;
                        _characterModel.battlegroupRemoveUnit(_characterModel.getCharacter(u.parent), u);
                        RosterMenu.RosterMenu.instantiatedObjects.Remove(DragScript.draggedObject);
                        Destroy(DragScript.draggedObject);
                    }
                    GameStateManager.Instance.SaveGame();
                    RosterMenu.RosterMenu.rosterMenuObj.GetComponent<RosterMenu.RosterMenu>().UpdateGUI();
                }
                else
                {
                    try
                    {
                        //Don't allow a unit to be dropped into item slot, vice-versa
                        if (GetComponent<SlotObject>().canDrop(DragScript.lastParent.gameObject))
                        {
                            if (!GetComponent<SlotObject>().isRemoval)
                            {
                                bool differentSide = false;
                                DragScript.overSlot = true;
                                if (DraggableObject.onTradeMenu && DragScript.draggedObject.transform.parent.parent != GetComponent<SlotObject>().transform.parent)
                                {
                                    differentSide = true;
                                }
                                //if this slot doesn't have an object in it
                                if (!(transform.childCount > 0))
                                {
                                    DragScript.draggedObject.transform.SetParent(transform);
                                }
                                else
                                {
                                    //perform object swap
                                    transform.GetChild(0).parent = DragScript.startParent;
                                    DragScript.draggedObject.transform.SetParent(transform);
                                }

                                if (DraggableObject.onTradeMenu && differentSide)
                                {
                                    TradeMenu.TradeMenuScript.tradeMenuObj.GetComponent<TradeMenu.TradeMenuScript>().Trade();
                                }
                                if (!DraggableObject.onTradeMenu) { RosterMenu.RosterMenu.rosterMenuObj.GetComponent<RosterMenu.RosterMenu>().TransferBtn(); };
                            }
                        }
                    }
                    catch (System.Exception ex)
                    {
                        //ignore exception to prevent purposeful breaking of the system.
                        //OnEndDrag handles resetting the draggable object backs to it's original location.
                        //this is rare when the user is purposely trying to break the drag-and-drop system by rapidly clicking + holding
                        //in various directions in the shortest amount of time
                    }
                }
            }
            catch (System.Exception ex)
            {
            }
            DragScript.hasDropped = true;
        }
    }
}