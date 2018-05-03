using UnityEngine;
using System.Collections;
using Umbra.UI;
using Umbra.Controller;
using UnityEngine.UI;

public class MapWomhole : MonoBehaviour {
	public Sprite image;
	private DiamondUIController diamondUI;
	private StarMapController starMapController;

	void Start() {
		diamondUI = GameObject.Find ("DiamondUI").GetComponent<DiamondUIController> ();
		starMapController = GameObject.Find ("StarMap").GetComponent<StarMapController> ();
	}

	void OnMouseUp() {
		diamondUI.SetActiveSubmenu("LocalWormholeSelected");
		var descBox = starMapController.descBox;
		descBox.gameObject.SetActive (true);
		descBox.NamePanel.text = "Wormhole";
		descBox.ImagePanel.GetComponent<Image> ().sprite = image;
		descBox.ImagePanel.GetComponent<RectTransform> ().localScale = new Vector3 (0.5f, 1, 1);
		descBox.Description.text = "";
	}
}
