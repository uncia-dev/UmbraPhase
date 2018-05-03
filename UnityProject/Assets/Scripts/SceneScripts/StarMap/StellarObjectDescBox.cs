using System.Collections;
using UnityEngine;
using Umbra.UI;
using UnityEngine.UI;

namespace Umbra.Scenes.StarMap {
	public class StellarObjectDescBox : MonoBehaviour {
		public Collider2D[] Colliders;
		public GameObject ToggleMenus;
		public Text NamePanel;
		public GameObject ImagePanel;
		public Text Description;
		private DiamondUIController _diamondUI;


		// Use this for initialization
		void Awake() {
			Colliders = GetComponentsInChildren<BoxCollider2D> ();
			_diamondUI = GameObject.Find ("DiamondUI").GetComponent<DiamondUIController> ();

			NamePanel = transform.Find ("NamePanel/ObjectName").GetComponent<Text> ();
			ImagePanel = transform.Find ("DescPanel/DescImage").gameObject;
			Description = transform.Find ("DescPanel/DescText").GetComponent<Text> ();

			gameObject.SetActive (false);
		}


		void Update() {
			if (gameObject.activeSelf && Input.GetButtonDown ("Fire1")) {
				if (!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject ()) {
					gameObject.SetActive (false);
					_diamondUI.SetActiveSubmenu ("DefaultStarmap");
				}
			}
		}	
	}
}