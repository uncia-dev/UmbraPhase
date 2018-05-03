using UnityEngine;
using System.Collections;
using Umbra.Managers;
using Umbra.Utilities;
using Umbra.UI;
using Umbra.Data;

namespace Umbra.Scenes.BattleMap
{
    public class Battle : MonoBehaviour
    {

        private BattleManager _battleManager;
		private TileManager _tileManager;
        private CameraMovement _movementControl;

        void Awake()
        {
			_battleManager = BattleManager.Instance;
			_tileManager = TileManager.Instance;
            _movementControl = gameObject.AddComponent<CameraMovement>();
            _movementControl._zMin = 5;
            _movementControl._zMax = 20;
            //_movementControl._speed = 500;
            //_movementControl.cam = GameObject.Find("CameraFollow");
		_tileManager.loadMap();
			Debug.Log ("Loaded battle map: " + _tileManager.mapName);

        }

		void Start()
		{

		}

		// Update is called once per frame
		void Update() 
		{
			_movementControl.HandleMovement();
		}

    }
}
