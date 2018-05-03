using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Umbra.Managers;
using Umbra.Data;

namespace Umbra.Scenes.MainMenu {
	public class MainMenu : MonoBehaviour {

		private MusicManager _musicManager;

		void Awake() {
			_musicManager = MusicManager.Instance;
		}

		void Start() {
			_musicManager.setPlaylist ("Menu");

		}
			
		public void StartNewGame() {
			GameStateManager.Instance.PushScene (GameScene.NewCharacter);
		}

        public void LoadGame()
        {
            GameStateManager.Instance.PushScene(GameScene.LoadGame);
        }

        public void SettingsMenu()
        {
            GameStateManager.Instance.PushScene(GameScene.SettingsMenu);
        }

        public void ExitGame()
        {
            Application.Quit();
        }
	}
}