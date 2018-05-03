using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using Umbra.Utilities;
using Umbra.Data;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.SceneManagement;
using System.Linq;
using System.Runtime.Serialization;

namespace Umbra {

	/// <summary>
	/// Game state.
	/// </summary>

	public class GameStateManager : Singleton<GameStateManager>{
		public GameScene currentScene { get; private set; }
		public GameState gameState { get; private set; }
		public Settings settings { get; set; } //need to be able to modify settings anywhere

        //global string that stores the path the current played Game is being saved and loaded to
        //Must set this upon continuation, new game, and load game
        public string currentSavePath = "";

		public GameScene GameScene { get; private set; }

		private Stack<GameScene> _sceneStack = new Stack<GameScene>();

		void Awake() {
			settings = new Settings ();

            DirectoryInfo directory = new DirectoryInfo(Application.persistentDataPath);
            FileInfo latest = null;
            bool exists = false;
            try
            {
                latest = directory.GetFiles().OrderByDescending(f => f.LastWriteTime).First();
                exists = File.Exists(Application.persistentDataPath + "/" + latest.Name);
            }
            catch (System.Exception ex)
            {
				print (ex);
            }

            if (exists)
            {
                currentSavePath = "/" + latest.Name;
                LoadGame();
            }
            /*else
            {
                CreateNewGame("Default");
            } */
        }

		/*
		 * Return false if a unit has an ability not in the ability dictionary
		 */
		public bool databaseCheckUnitAbilities() {

			if (gameState.abilityDictionary != null && gameState.unitDictionary != null) {

				foreach (KeyValuePair<string, Unit> e in gameState.unitDictionary) {
					foreach (string a in e.Value.abilities) {
						if (a != "" && !gameState.abilityDictionary.ContainsKey (a)) {
							Debug.Log ("Ability " + a + " is not in the Ability dictionary.");
							return false;
						}
					}
				}

			}

			return true;

		}

		/// <summary>
		/// Sets the scene without adding it to the stack. Use for scenes that you don't want to pop back to
		/// </summary>
		/// <param name="gameState">Game state.</param>
		/// <param name="sceneChangeFunc">The callback function that handles the scene change logic, use lambdas for short functions</param>
		public void SetScene(GameScene gameScene) {
			this.GameScene = gameScene;
			SceneManager.LoadScene (GameScene.ToString ());
		}

		/// <summary>
		/// Set the current game scene and add it to the scene stack
		/// </summary>
		/// <param name="gameState">Game state.</param>
		/// <param name="sceneChangeFunc">Scene change func.</param>
		public void PushScene(GameScene gameScene, bool byPassSave=false) {
			if (!byPassSave) SaveGame ();
			_sceneStack.Push(gameScene);
			SetScene (gameScene);
		}

		/// <summary>
		/// Returns to the previous scene on the stack
		/// </summary>
		public void PopScene() {
			SaveGame ();
			if (_sceneStack.Count <= 1) {
				_sceneStack.Push (GameScene.StarMap);
			} else {
				_sceneStack.Pop ();
			}
			SetScene (_sceneStack.Peek());
		}

		public void CreateNewGame(string saveName) {
            //set global save path
            currentSavePath = "/" + saveName + ".ufs";
            settings.savePath = currentSavePath;
			gameState = new GameState ();
			gameState.init();
			settings = new Settings ();
			SaveGame ();
		}

		public bool LoadGame() {

			//var saveFile = settings.savePath;
			if (File.Exists(Application.persistentDataPath + currentSavePath))
			{
				BinaryFormatter bf = new BinaryFormatter();
				SurrogateSelector surrogateSelector = new SurrogateSelector ();
				Vector3SerializationSurrogate vss = new Vector3SerializationSurrogate ();
				surrogateSelector.AddSurrogate (typeof(Vector3), new StreamingContext (StreamingContextStates.All), vss);

				bf.SurrogateSelector = surrogateSelector;

                FileStream f = File.Open(Application.persistentDataPath + currentSavePath, FileMode.Open);
				try
				{
                    print("Loading file: " + Application.persistentDataPath + currentSavePath);
					gameState = (GameState)bf.Deserialize(f);
					settings = (Settings)bf.Deserialize(f);
					f.Close();
					//Debug.Log("[GameData] Game data loaded from memory.");
				}
				catch
				{
					f.Close();
					Debug.Log("[GameData] Error: Unsupported save type");
                    File.Delete(Application.persistentDataPath + currentSavePath);
					Debug.Log("[GameData] Save deleted >:(");
				}

				gameState.databaseLoad ();
				return true && gameState.databaseIsLoaded();
            }

			//isSave = false;
			//Debug.Log("[GameData] Game data not loaded from memory.");
			return false;
		}

		public void SaveGame() {
			if (gameState != null) {
				BinaryFormatter bf = new BinaryFormatter();
                FileStream f = File.Create(Application.persistentDataPath + currentSavePath);
				try
				{
                    print("Saving file: " + Application.persistentDataPath + currentSavePath);
					bf.Serialize(f, gameState);
					bf.Serialize(f, settings);
	
				} catch (Exception e){
					// no current game
					Debug.Log("[GameData] No current save game to save " + e.Message);
					f.Close();
                    File.Delete(Application.persistentDataPath + currentSavePath);
				}
				f.Close();
			}
		}
	}
}