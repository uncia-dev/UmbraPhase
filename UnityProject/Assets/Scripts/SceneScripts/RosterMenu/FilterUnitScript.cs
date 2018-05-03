using UnityEngine;
using System.Collections;
using Umbra.Data;
using UnityEngine.UI;
using Umbra.CreateUnitMenu;
using Umbra.Models;
using Umbra.Scenes.TradeMenu;

namespace Umbra.Scenes.RosterMenu
{
    public class FilterUnitScript : MonoBehaviour
    {
        public string unitID;
        public Unit unitSample;
        public bool onCreateUnitMenu;

        public void onClickFilterButton()
        {

            UnitModel _unitModel = new UnitModel();

            Umbra.UI.GUI_Window.temporaryHide();
            //RosterMenu.filteredUnits = GameStateManager.Instance.gameState.units.FindAll(unit => unit.isRecruitable && unit.id == unitID);
            RosterMenu.filteredUnits =
                _unitModel.getAllUnits(1).FindAll(unit => unit.isCharacter == false && unit.id == unitID);

            RosterMenu.rosterMenuObj.GetComponent<RosterMenu>().UpdateGUI();

        }

        public void selectUnitType(GameObject unit)
        {
            if (CreateUnitScript.lastClicked != null)
            {
                CreateUnitScript.lastClicked.GetComponent<Image>().color = new Color(1, 1, 1, 1);
            }
            CreateUnitScript.lastClicked = unit;
            CreateUnitScript.unitID = unit.GetComponent<FilterUnitScript>().unitID;
            CreateUnitScript.resourceLabels[0].GetComponent<Text>().text = unitSample.costMinerals.ToString();
            CreateUnitScript.resourceLabels[1].GetComponent<Text>().text = unitSample.costGas.ToString();
            CreateUnitScript.resourceLabels[2].GetComponent<Text>().text = unitSample.costFuel.ToString();
            CreateUnitScript.resourceLabels[3].GetComponent<Text>().text = unitSample.costFood.ToString();
            CreateUnitScript.resourceLabels[4].GetComponent<Text>().text = unitSample.costWater.ToString();
            CreateUnitScript.resourceLabels[5].GetComponent<Text>().text = unitSample.costMeds.ToString();

            unit.GetComponent<Image>().color = Color.cyan;
            Umbra.UI.GUI_Window.temporaryHide();
        }

        public void onHover()
        {
            if (!Umbra.UI.GUI_Window.permanentClose)
            {
                Umbra.UI.GUI_Window.isUnitGUI(unitSample);
                Umbra.UI.GUI_Window.ShowWindow();
            }
        }

        public void onExit()
        {
            Umbra.UI.GUI_Window.isNothing();
            Umbra.UI.GUI_Window.temporaryHide();
        }
    }
}