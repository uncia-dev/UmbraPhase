using UnityEngine;
using System.Collections;
using Umbra.Controller;

namespace Umbra.Controller {
	public class LevelController : MonoBehaviour {
		// Use this for initialization

		public GameObject startPosition;
		void Awake () {
			if (startPosition == null) {
				startPosition = GameObject.Find ("Wormhole");
			}

			GameObject.Find ("StarMap").GetComponent<StarMapController> ().SetShipLocation (startPosition.transform.position);
		}
	}
}
