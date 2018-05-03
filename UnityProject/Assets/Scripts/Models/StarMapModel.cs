using UnityEngine;
using System.Collections;
using Umbra;
using Umbra.Data;
using System.Collections.Generic;

namespace Umbra.Models {
	public class StarMapModel {

		private GameState _gameState;

		public StarMapModel() {
			_gameState = GameStateManager.Instance.gameState;
		}

		public StarClusterState GetStarClusterState(int id) {
			StarClusterState scs;
			if (!_gameState.starClusterDictionary.TryGetValue (id, out scs)) {
				scs = new StarClusterState ();
				_gameState.starClusterDictionary [id] = scs;
			}

			return scs;
		}

		public StarClusterPlanetState getPlanetState(int id) {
			StarClusterPlanetState scps;
			if (!_gameState.starClusterPlanetDictionary.TryGetValue (id, out scps)) {
				scps = new StarClusterPlanetState ();
				_gameState.starClusterPlanetDictionary [id] = scps;
			}

			return scps;
		}

		public WormholeState getWormholeState(int id) {
			WormholeState ws;
			if (!_gameState.wormholeDictionary.TryGetValue (id, out ws)) {
				ws = new WormholeState ();
				_gameState.wormholeDictionary [id] = ws;
			}

			return ws;
		}


		public StarMapLevel getCurrentStarMap() {
			return _gameState.currentStarmap;
		}

		public void setCurrentStarMap(StarMapLevel starMap) {
			_gameState.currentStarmap = starMap;
		}
	}
}