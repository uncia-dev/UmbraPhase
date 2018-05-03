using UnityEngine;
using System.Collections;
using Umbra;
using Umbra.Data;


namespace Umbra.Models {
	public class PlayerModel : Model<Player> {
		public PlayerModel() {
			data = GameStateManager.Instance.gameState.player;
		}
	}
}