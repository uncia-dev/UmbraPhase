using UnityEngine;
using System.Collections.Generic;
using Umbra.Managers;
using Umbra.UI;
using UnityEngine.UI;
using Umbra.Utilities;
using Umbra.Data;
using UnityEngine.SceneManagement;
using Umbra.Models;
using Umbra.Scenes.StarMap;
using Umbra.Scenes.ExplorationMap;

namespace Umbra.Controller {
	public class StarMapController : MonoBehaviour {
		public Button StarmapButton;
		public StellarObjectDescBox descBox;

        private Planet _selected;
        private DiamondUIController _diamondUIController;
		private CameraMovement _movementControl;
		private Color _highlightedColor;
		private StarMapModel _starMap;
		private StarMapLevel _currentStarMap;
        private MapModel _mapModel;
        private Map map;
		private float zoomLevel = 120;

		private ShipMovement ship;

		void Awake() {
			_movementControl = gameObject.AddComponent<CameraMovement> ();
			_highlightedColor = StarmapButton.colors.highlightedColor;
			_starMap = new StarMapModel ();
            _mapModel = new MapModel();
			_diamondUIController = GameObject.Find ("DiamondUI").GetComponent<DiamondUIController> ();

			ship = GameObject.FindGameObjectWithTag ("PlayerShip").GetComponent<ShipMovement>();
		}

		// Use this for initialization
		void Start () {
			_currentStarMap = _starMap.getCurrentStarMap ();
			print (_currentStarMap);
			LoadStarMap (_currentStarMap);

			_diamondUIController.SetActiveSubmenu ("DefaultStarmap");

			var colors = StarmapButton.colors;
			colors.normalColor = _highlightedColor;
			StarmapButton.colors = colors;

			Camera.main.fieldOfView = zoomLevel;
		}
		
		void Update() {
			_movementControl.HandleMovement ();
		}

		public void LoadStarMap(StarMapLevel starMap) {
			GameObject level = GameObject.FindGameObjectWithTag ("StarMapLevel");
			if (level != null) {
				Destroy (level);
			}

			SceneManager.LoadScene (starMap.ToString(), LoadSceneMode.Additive);
			_starMap.setCurrentStarMap (starMap);
			_diamondUIController.OpenStarmap ();
			_diamondUIController.SetActiveSubmenu ("DefaultStarMap");

		}

        public void SelectPlanet(Planet pl)
		{
			if (_selected != null) {
				foreach (var line in _selected.GetComponentsInChildren<LineRenderer>()) {
					Destroy (line.gameObject);
				}
			}
			_selected = pl;
			GameStateManager.Instance.gameState.currentPlanetName = _selected.name;

			//map = _mapModel.data.Find(m => m.id.Equals(_selected.id.ToString())); //check if the map exists before creating a new one 
			map = _mapModel.getMapByID (_selected.mapID);
			// I do not think this is necessary any longer; if no map is found, don't load anything
			/*
            if (map == null)
            {
                map = _mapModel.createMap((_selected.id.ToString())); //if there wasnt any one in the data field then create a new one
            }
            */
            _mapModel.setCurrentMapByMap(map);

			zoomLevel = Camera.main.fieldOfView;
        }

        public void SetShipLocation(Vector3 position) {
			ship.transform.position = position;
			SnapCameraToShip ();
		}

		public void TravelToObject(GameObject obj) {
			ship.goToPlanet (obj);
		}

		private void SnapCameraToShip() {
			var newPos = ship.transform.position;
			newPos.z = Camera.main.transform.position.z;
			Camera.main.transform.position = newPos;
		}
	}

}