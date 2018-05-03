using UnityEngine;
using System.Collections;

public class MainMenuFadeIn : MonoBehaviour {

	CanvasGroup _menu;

	void Awake () {
		_menu = GetComponent<CanvasGroup> ();
	}

	// Use this for initialization
	void Start () {
		_menu.alpha = 0;
		StartCoroutine ("FadeIn");
	}
	
	IEnumerator FadeIn() {
		float time = 3f;
		while (_menu.alpha < 1) {
			_menu.alpha += Time.deltaTime / time;
			yield return null;
		}
	}

}
