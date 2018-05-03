using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Umbra.Managers;
using Umbra.Data;

namespace Umbra.Scenes.SplashScreen {
	public class SplashScreen : MonoBehaviour {

		public Canvas TitleCanvas;
		public Canvas LogoCanvas;

		private MusicManager _musicManager;
		private GameStateManager _manager;
		private Image _teamLogo, _gameLogo;

		void Awake() {
			_musicManager = MusicManager.Instance;
			_manager = GameStateManager.Instance;
		}

		// Use this for initialization
		void Start () {
			_teamLogo = LogoCanvas.GetComponentInChildren<Image>();
			_gameLogo = TitleCanvas.GetComponentInChildren<Image>();

			_teamLogo.CrossFadeAlpha (0f, 0f, false);
			_gameLogo.CrossFadeAlpha (0f, 0f, false);

			StartCoroutine ("animateIntro");
		}

		void Update() {
			if (Input.GetKey (KeyCode.Escape) || Input.GetKey(KeyCode.Return)) {
				StopCoroutine ("animateIntro");
				_manager.PushScene (GameScene.MainMenu);
			}
		}

		private IEnumerator animateIntro() {
			_teamLogo.CrossFadeAlpha (1f, 3f, false);
			yield return new WaitForSeconds (3);
			_teamLogo.CrossFadeAlpha (0f, 3f, false);
			yield return new WaitForSeconds (4);
			_gameLogo.CrossFadeAlpha (1f, 3f, false);
			_musicManager.setPlaylist ("Menu");
			yield return new WaitForSeconds (6);
			_manager.PushScene (GameScene.MainMenu);
		}
	}
}