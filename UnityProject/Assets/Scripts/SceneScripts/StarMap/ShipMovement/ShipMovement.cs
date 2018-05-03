using UnityEngine;
using System.Collections;
using Umbra.Models;
using Umbra.Scenes.StarMap;
using Umbra.Controller;
using Umbra.UI;

public class ShipMovement : MonoBehaviour
{
    private Vector3 _oldPos;
    private Vector3 _newPos;
    private float _lerpTime = 1f;
    private float _deltaTime = 0f;

    float speed = 5f;
    Vector3 destination;
    private bool moving;
	private bool snap;

    private static GameObject lastPlanetDest;
    private static bool travelToPlanet;

    private Vector3 tmpMousePos;

    private StarMapObject starMapScript;
    private Planet planetScript;
	private DiamondUIController diamondUI;

    StarbaseModel _shipModel;

    Sprite shipMovingSprite;
    Sprite shipDormantSprite;
	private Vector3 mousePos;

    void Start()
    {
        tmpMousePos = Input.mousePosition;
        _shipModel = new StarbaseModel();
        shipMovingSprite = Resources.Load<Sprite>("StarMap_Ship_Moving");
        shipDormantSprite = Resources.Load<Sprite>("StarMap_Ship");

		diamondUI = GameObject.Find ("DiamondUI").GetComponent<DiamondUIController> ();
    }

    public void goToPlanet(GameObject planet)
    {
		if (Input.mousePosition == mousePos) {
			travelToPlanet = true;
			lastPlanetDest = planet;
		}
    }

    public void rotateShip()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 15));
        float AngleRad = Mathf.Atan2(mousePos.y - transform.position.y, mousePos.x - transform.position.x);
        float AngleDeg = (180 / Mathf.PI) * AngleRad;
        transform.rotation = Quaternion.Euler(0, 0, AngleDeg - 180);
    }

    void Update()
    {
		if (Input.GetMouseButtonDown (0)) {
			mousePos = Input.mousePosition;
		}
		if (!moving && Input.mousePosition != tmpMousePos && transform.parent == null)
        {
            rotateShip();
        }
         

		if (Input.mousePosition == mousePos && diamondUI.currentMap == Umbra.Data.MapButtons.Starmap && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject ()) {
			if (travelToPlanet) {
				travelToPlanet = false;
				if (transform.parent != lastPlanetDest.transform) {
					starMapScript = lastPlanetDest.GetComponent<Umbra.Scenes.StarMap.StarMapObject> ();
					planetScript = lastPlanetDest.GetComponent<Umbra.Scenes.StarMap.Planet> ();
					rotateShip ();
					transform.parent = lastPlanetDest.transform;
					destination = Camera.main.ScreenToWorldPoint (new Vector3 (Input.mousePosition.x, Input.mousePosition.y, 15));
					moving = true;
					gameObject.GetComponent<SpriteRenderer> ().sprite = shipMovingSprite;
				}
			} else {
				if (Input.GetMouseButtonUp (0)) {
					rotateShip ();
					transform.parent = null;
					starMapScript = null;
					planetScript = null;
					destination = Camera.main.ScreenToWorldPoint (new Vector3 (Input.mousePosition.x, Input.mousePosition.y, 15));
					moving = true;
					gameObject.GetComponent<SpriteRenderer> ().sprite = shipMovingSprite;
				}
			}
		}

        if (moving)
        {
            transform.position = Vector3.MoveTowards(transform.position, destination, speed * Time.deltaTime);

            if (starMapScript != null)
            {
				starMapScript.UpdateDesc("\nDistance to " + starMapScript.Name + " " + Vector3.Distance(transform.position, destination).ToString() + " AU");
            }
            if (Vector3.Distance(transform.position, destination) <= 1.5f)
            {
                gameObject.GetComponent<SpriteRenderer>().sprite = shipDormantSprite;
                if (transform.parent != null)
                {
                    string appendDesc = "";
                    appendDesc += "\nVisitable: " + planetScript.isVisitable;
                    appendDesc += "\nOrbital Speed: " + planetScript.orbitSpeed;
                    appendDesc += "\nMinerals: " + planetScript.resourcesMinerals;
                    appendDesc += "\nGas: " + planetScript.resourcesGas;
                    appendDesc += "\nFuel: " + planetScript.resourcesFuel;
                    appendDesc += "\nWater: " + planetScript.resourcesWater;
                    appendDesc += "\nFood: " + planetScript.resourcesFood;
                    appendDesc += "\n" + _shipModel.data.name + " is now orbitting this planet.";
                    starMapScript.UpdateDesc(starMapScript.Description + appendDesc);
                }
                tmpMousePos = Input.mousePosition;
                moving = false;
            }
        }
    }
}