using UnityEngine;
using System.Collections;
using Umbra.Utilities;
using UnityEngine.UI;
using Umbra.Data;
using Umbra.Controller;
using Umbra.UI;
using Umbra.Models;
using System.Collections.Generic;
using System.Linq;

namespace Umbra.Scenes.StarMap {
	public class Wormhole : StarMapObject {
		public StarMapLevel starMap;
		public WormholeState state;

		private WormholeController _wormholeController;
		private StarMapModel _model;
		private List<Wormhole> _connected;

		protected override void Awake() {
			base.Awake ();
		}

		protected override void Start() {
			_model = new StarMapModel ();
			state = _model.getWormholeState (id);

			_connected = new List<Wormhole> ();
			foreach (Transform t in transform) {
				_connected.Add (t.GetComponent<Wormhole> ());
			}

			var parent = transform.parent.GetComponent<Wormhole> ();
			if (parent != null) {
				_connected.Add (parent);
			}

			_wormholeController = GameObject.Find ("WormholeMapOverlay").GetComponent<WormholeController> ();
		}

		protected override void OnMouseUp() {
			base.OnMouseUp ();

			_wormholeController.SelectWormhole (this);
		}

		public List<Wormhole> getConnected() {
			return _connected;
		}
	}
}