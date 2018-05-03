using UnityEngine;
using System.Collections;
using Umbra.Utilities;
using Umbra.UI;
using UnityEngine.UI;
using Umbra.Controller;
using Umbra.Data;

namespace Umbra.Scenes.StarMap {
	public class StarMapObject : MonoBehaviour {
		public int id;
		public DoubleClick DoubleClick;
		public float PanSpeedModifier = 1.5f;
		public string Name = "Default Name";
		public string Description = "";
		public Sprite Image;

		protected DiamondUIController diamondUI;
		protected StarMapController starMapController;

		private float _lerpTime = 1f;
		private float _deltaTime = 0f;
		private bool _interpolate = false;
		private Vector3 _oldPos;
		private Vector3 _newPos;
		private float _distance;

        private Text planetDesc;

		protected virtual void Awake() {
			DoubleClick = gameObject.AddComponent<DoubleClick>();
			DoubleClick.DoubleClickHandler += OnDoubleClick;
			starMapController = GameObject.Find ("StarMap").GetComponent<StarMapController> ();
		}

		protected virtual void Start() {
			diamondUI = GameObject.Find ("DiamondUI").GetComponent<DiamondUIController>();
			Image = gameObject.GetComponent<SpriteRenderer> ().sprite;
		}

		protected virtual void OnMouseUp() {
			var descBox = starMapController.descBox;
			descBox.gameObject.SetActive (true);
			descBox.NamePanel.text = Name;
			descBox.ImagePanel.GetComponent<Image> ().sprite = Image;
			descBox.ImagePanel.GetComponent<RectTransform> ().localScale = new Vector3 (0.5f, 1, 1);
			planetDesc = descBox.Description;
			descBox.Description.text = Description;
		}

        public void UpdateDesc(string desc)
        {
            planetDesc.text = desc;
        }

		void OnDoubleClick () {
			var camera = Camera.main;
			_oldPos = camera.transform.position;
			_newPos = new Vector3 (transform.position.x, transform.position.y, camera.transform.position.z);
			_interpolate = true;
		}

		protected virtual void Update() {
			if (_interpolate) {
				_deltaTime += Time.deltaTime;

				if (_deltaTime > _lerpTime)
					_deltaTime = _lerpTime;

				float t = _deltaTime / _lerpTime;
				t = t * t * (3f - 2f * t);
				Camera.main.transform.position = Vector3.Lerp (_oldPos, _newPos, t * PanSpeedModifier);
				if (Camera.main.transform.position.Equals(_newPos)){
					_interpolate = false;
					_deltaTime = 0;
				}
			}
		}
	}
}