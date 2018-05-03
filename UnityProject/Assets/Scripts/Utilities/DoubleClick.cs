using UnityEngine;
using System.Collections;


namespace Umbra.Utilities {
	public delegate void OnDoubleClickHandler();
	public class DoubleClick : MonoBehaviour {

		public float Delay = 0.3f;
		public event OnDoubleClickHandler DoubleClickHandler; 

		private float _doubleClickTime = 0;

		void OnMouseUp() {
			if (Time.time - _doubleClickTime < Delay) {
				DoubleClickHandler ();
			} else {
				_doubleClickTime = Time.time;
			}
		}
	}
}