using UnityEngine;
using System.Collections;

namespace Umbra.Scenes.MainMenu {
	public class cameraMove : MonoBehaviour {

	    public float moveAmount;
	    public Camera cam;
	    public Canvas menu;

	    float movx, movy;
	    float decay = 0.001f;
	    bool flipx, flipy = true;

	    int xmove, ymove, zmove;

	    public float dampTime = 0.15f;
	    Vector3 velocity = Vector3.zero;

		// Use this for initialization
		void Start () {
	        xmove = Random.Range(-50, 50);
	        ymove = Random.Range(-50, 50);
	        movx = moveAmount - (moveAmount/2);
	        movy = moveAmount - (moveAmount / 2);
		}
		
		// Update is called once per frame
		void Update () {
	        Transform t = cam.GetComponent<Transform>();
	        float x = t.position.x;
	        float y = t.position.y;
	        float z = t.position.z;

	        // x
	        if (flipx && Random.Range(-2, 700) < 0)
	            flipx = false;
	        else if (flipx == false)
	            movx -= decay;

	        if (movx < 0)
	        {
	            xmove = Random.Range(-50, 50);
	            flipx = true;
	            movx = 0;
	        }
	        else if (flipx && movx < moveAmount)
	        {
	            movx += decay;
	        }

	        // y
	        if (flipy && Random.Range(-2, 700) < 0)
	            flipy = false;
	        else if (flipy == false)
	            movy -= decay;

	        if (movy < 0)
	        {
	            ymove = Random.Range(-50, 50);
	            flipy = true;
	            movy = 0;
	        }
	        else if (flipy && movy < moveAmount)
	        {
	            movy += decay;
	        }

	        // z
	        /* if (flipz && Random.Range(-2, 700) < 0)
	            flipz = false;
	        else if (flipz == false)
	            movz -= decay;

	        if (movz < 0)
	        {
	            zmove = Random.Range(-50, 50);
	            flipz = true;
	            movz = 0;
	        }
	        else if (flipz && movz < moveAmount)
	        {
	            movz += decay;
	        } */

	        if (xmove < 0) { x += movx; }
	        else { x -= movx;  }
	        if (ymove < 0) { y += movy; }
	        else { y -= movy;  }
	        /* if (zmove < 0) { z += movz; }
	        else { z -= movz; } */

	        t.position = Vector3.SmoothDamp(t.position, new Vector3(x, y, z), ref velocity, dampTime);
	        //t.position = Vector3.Lerp(t.position, new Vector3(x, y, z), Time.deltaTime);
		}

	    // randomly moves camera in a direction 
	    float moveSpeed = 2000;
	    public void randomMove()
	    {
	        float x = transform.position.x;
	        float y = transform.position.y;
	        float z = transform.position.z;
	        float pwidth = menu.GetComponent<RectTransform>().rect.width + x;
	        float pheight = menu.GetComponent<RectTransform>().rect.height + y;

	        x += (Random.Range(-50, 50) > 0) ? moveSpeed : -moveSpeed;
	        y += (Random.Range(-50, 50) > 0) ? moveSpeed : -moveSpeed;
	        z += (Random.Range(-50, 50) > 0) ? moveSpeed*2 : -moveSpeed;

	        // TODO: fix going out of bounds
	       // if (x >= menu.GetComponent<Transform>().position.x) something like this=

	        if (x > pwidth) {
	            x -= moveSpeed*2;
	        }
	        else if (x < menu.GetComponent<Transform>().position.x){
	            x += moveSpeed*2;
	        }
	        if (y > pheight)
	        {
	            y -= moveSpeed * 2;
	        }
	        else if (y < menu.GetComponent<Transform>().position.y)
	        {
	            y += moveSpeed * 2;
	        }

	        transform.position = Vector3.SmoothDamp(transform.position, new Vector3(x, y, z), ref velocity, 0.15f);
	    }

	}
}