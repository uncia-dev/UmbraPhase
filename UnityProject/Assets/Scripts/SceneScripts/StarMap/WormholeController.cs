using UnityEngine;
using System.Collections;
using Umbra.Data;
using Umbra.Scenes.StarMap;
using System.Collections.Generic;
using Umbra.Utilities;
using Umbra.Models;
using Umbra.UI;
using UnityEngine.UI;

namespace Umbra.Controller {	
	public class WormholeController : MonoBehaviour {
		private StarMapModel _model;
		private Dictionary<int, Wormhole> _wormholeDict;

		private Wormhole _currentLocation;
		private Wormhole _selected;
        private DiamondUIController _diamondUI;
		private StarMapController _starMapController;
		private string errorMessage;

		// Use this for initialization
		void Start () {
			_wormholeDict = new Dictionary<int, Wormhole> ();
			_model = new StarMapModel ();
		
			foreach (var wormhole in GetComponentsInChildren<Wormhole>()) {
				wormhole.state = _model.getWormholeState (wormhole.id);
				_wormholeDict [wormhole.id] = wormhole;
				if (wormhole.state.isCurrentLocation) {
					_currentLocation = wormhole;
				}
			}

			_diamondUI = GameObject.Find ("DiamondUI").GetComponent<DiamondUIController>();
			_starMapController = GameObject.Find ("StarMap").GetComponent<StarMapController> ();
		}

		public void OpenMap() {
			if (_currentLocation != null) {
				SelectWormhole (_currentLocation);
			} else {
				SelectWormhole (_wormholeDict [0]);
			}
		}

		public void SelectWormhole(Wormhole wh) {
			if (_selected != null) {
				foreach (var line in _selected.GetComponentsInChildren<LineRenderer>()) {
					Destroy (line.gameObject);
				}
			}

			_selected = wh;
			var connected = wh.getConnected ();
			var sortingOrder = GetComponent<Canvas> ().sortingOrder + 1;
			foreach (var wormhole in connected) {
				var lineRenderer = new GameObject ().AddComponent<LineRenderer> ();
				lineRenderer.name = "LineRenderer";
				lineRenderer.transform.SetParent (wh.transform);
				lineRenderer.material = new Material(Shader.Find("Particles/Additive"));	
				lineRenderer.SetColors(new Color(0f, 0.75f, 1f), new Color(0f, 0.75f, 1f));
				lineRenderer.SetWidth(0.1F, 0.1F);
				lineRenderer.SetVertexCount(2);
				lineRenderer.SetPosition (0, _selected.transform.position);
				lineRenderer.SetPosition (1, wormhole.transform.position);
				lineRenderer.gameObject.layer = gameObject.layer;
				lineRenderer.sortingOrder = sortingOrder;
			}

			var wormholeMenu = _diamondUI.SetActiveSubmenu("WormholeSelected");
			var button = wormholeMenu.GetComponentInChildren<Button> ();

			if (_selected.transform.IsDirectChildOf (_currentLocation.transform) || _currentLocation.transform.IsDirectChildOf(_selected.transform)) {

				button.interactable = true;
			} else {
				button.interactable = false;
			}
		}

		public void LoadStarMap() {
			_starMapController.LoadStarMap (_selected.starMap);
			_selected.state.isVisited = true;
			SetCurrentLocation ();

		}

		public void SetCurrentLocation() {
			_currentLocation = _selected;
			foreach (var wormhole in _wormholeDict.Values) {
				wormhole.state.isCurrentLocation = false;
			}
			_selected.state.isCurrentLocation = true;
		}
	}
}