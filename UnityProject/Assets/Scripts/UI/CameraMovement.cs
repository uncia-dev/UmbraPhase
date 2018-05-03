using UnityEngine;
using System.Collections;

namespace Umbra.Utilities {
	public class CameraMovement : MonoBehaviour {
		private float _pan = -1;
		private float _speed = 50;
		public float _zMin = 15;
		public float _zMax = 120;
		private Rect window;

		void Start() {
			window = new Rect (0, 0, Screen.width, Screen.height);
		}

		public void HandleMovement() {
			// simple click and drag
			if (window.Contains(Input.mousePosition)) {
                if (Input.GetMouseButton(0))
                {
                    float x = Input.GetAxis("Mouse X") * _pan;
                    float y = Input.GetAxis("Mouse Y") * _pan;
                    Camera.main.transform.Translate(x, y, 0);
                }
                float fov = Camera.main.fieldOfView;
                fov += Input.GetAxis("Mouse ScrollWheel") * _speed;
                fov = Mathf.Clamp(fov, _zMin, _zMax);
                Camera.main.fieldOfView = fov;
                Camera.main.orthographicSize = fov;
			}
		}
	}
}