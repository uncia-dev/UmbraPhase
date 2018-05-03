using UnityEngine;
using System.Collections;
using Umbra.Managers;

public class ActorScript : MonoBehaviour {

    // the actor object that this gameobject represents
    public Actor Actor;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnMouseDown()
    {
        //ActorManager.Instance.selectedActor = Actor;
    }

}
