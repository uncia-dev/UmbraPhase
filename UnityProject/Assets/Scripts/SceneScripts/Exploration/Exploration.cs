using UnityEngine;
using System.Collections;
using Umbra.Managers;
using Umbra.Utilities;
using Umbra.UI;
using Umbra.Data;
using UnityEngine.UI;

namespace Umbra.Scenes.ExplorationMap
{
    public class Exploration : MonoBehaviour
    {

        public string plname;
        public TileManager tm;
        public ActorManager am;
		public ExplorationManager em;
        private CameraMovement _movementControl;
        public GameObject outpostPlacementLbl;

        void Awake()
        {
            tm = TileManager.Instance;
            am = ActorManager.Instance;
            em = ExplorationManager.Instance;
            _movementControl = gameObject.AddComponent<CameraMovement>();
            _movementControl._zMin = 5;
            _movementControl._zMax = 30;
            //_movementControl._speed = 500;
            //_movementControl.cam = GameObject.Find("CameraFollow");
            tm.loadMap(); //want to call the load map function and that function will use the map stored tof figure out what to load (this needs to stay in awake and not start or stuff breaks)
            // remove this later - currently loadmap() keeps forcing the default map, which breaks everything
        }

		void Start(){
            outpostPlacementLbl.SetActive(false);
		}

        // Update is called once per frame
        void Update()
        {
            _movementControl.HandleMovement();

            if (em.waitingOnOutpost)
            {
                if (!outpostPlacementLbl.activeSelf)
                {
                    outpostPlacementLbl.SetActive(true);
                }
                Vector3 pos = Input.mousePosition;
                pos.x += 10 + outpostPlacementLbl.GetComponent<RectTransform>().rect.width / 2;
                outpostPlacementLbl.transform.position = pos;
            }
            else
            {
                if (outpostPlacementLbl.activeSelf)
                {
                    outpostPlacementLbl.GetComponent<Text>().color = Color.white;
                    outpostPlacementLbl.GetComponent<Text>().text = "Build at...";
                    outpostPlacementLbl.SetActive(false);
                }
            }
        }
    }
}
