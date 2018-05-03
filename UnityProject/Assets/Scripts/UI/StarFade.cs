using UnityEngine;
using System.Collections;


// this should be attached to every UI star

namespace Umbra.UI
{
    public class StarFade : MonoBehaviour
    {

        public SpriteRenderer _sprite;
        public float _min = 0.0f;
        public float _max = 1f;
        public float _dur = 1.5f;
        private float t;

        private Color c;

        // Use this for initialization
        void Start()
        {
            t = Time.time;
            c = new Color(1.0f, 1.0f, 1.0f, 0.0f);
        }

        void Awake()
        {
            _sprite = GetComponent<SpriteRenderer>();
            _sprite.color = c;
        }

        // Update is called once per frame
        void Update()
        {
            float ti = (Time.time - t) / _dur;
            c.a = Mathf.SmoothStep(_min, _max, ti);
            _sprite.color = c;
        }
    }
}
