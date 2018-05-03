using UnityEngine;
using System.Collections;
using Umbra.Utilities;
using UnityEngine.UI;
using Umbra.UI;
using Umbra.Data;
using Umbra.Models;
using Umbra.Controller;
using Umbra.Scenes.ExplorationMap;

namespace Umbra.Scenes.StarMap {
	public class Planet : StarMapObject {
		
		public bool isResourceFountain = false;
		public int resourcesMinerals;
		public int resourcesGas;
		public int resourcesFuel;
		public int resourcesWater;
		public int resourcesFood;
		public bool isVisitable = false;
		public float orbitSpeed = 1;
		public float rotationSpeed = -0.1f;
		public string mapID = ""; // id of the exploration map attached to this planet

		public StarClusterPlanetState state;

		private StarMapModel _model;
        private StarMapController _starMapController;

		protected override void Awake() {
			base.Awake ();
			_model = new StarMapModel ();
			state = _model.getPlanetState (id);
            _starMapController = GameObject.Find("StarMap").GetComponent<StarMapController>();
        }

		protected override void OnMouseUp() {
			base.OnMouseUp ();
			diamondUI.SetActiveSubmenu ("PlanetSelected");
			diamondUI.EnableLandButton (isVisitable);
			_starMapController.TravelToObject (gameObject);
            _starMapController.SelectPlanet(this);
        }
       
        protected override void Update() {
			base.Update ();
			transform.RotateAround (transform.parent.transform.position, Vector3.back, orbitSpeed * Time.deltaTime);
			transform.Rotate (new Vector3 (0, 0, rotationSpeed));
		}
	}
}