using UnityEngine;
using System.Collections;
using Umbra.Managers;
using Umbra.Utilities;
using Umbra.Data;

namespace Umbra.Utilities {
    // used to connect the actor to the unit
    public class ActorUnitBehaviourScript : MonoBehaviour {

		//public static Vector3 mousePos;
		ActorManager am = ActorManager.Instance;
		TileManager tm = TileManager.Instance;

        // References to the actor and unit data for this object
        public Actor actor;
        public Unit unit;

	    // Use this for initialization
	    void Start () {

	    }
	
	    // Update is called once per frame
	    void Update () {
	        
	    }

		void OnMouseDown()
		{
			// See TileScript; it's handling the movement and UI control for now
		}

    }
}