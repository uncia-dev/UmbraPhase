using UnityEngine;
using System.Collections;

namespace Umbra.Utilities {
	public class randomScript : MonoBehaviour {
	    public Texture2D tex;
		public GameObject Stars;
	    public int MAX_STARS = 500;
	    public int SIZE_MIN = 1;
	    public int SIZE_MAX = 10;
	    public int zMax = 1000;
	    public int zMin = 10;
	    public int yMin = -1000;
	    public int yMax = 2000;
	    public int xMin = -1000;
	    public int xMax = 2000;

		// Use this for initialization
		void Start () {

	        for (int i = 0; i < MAX_STARS; i++)
	        {

	            int z = Random.Range(zMin, zMax);
	            int size = Random.Range(SIZE_MIN, SIZE_MAX);
	            Vector3 pos = new Vector3(Random.Range(xMin, xMax), Random.Range(yMin, yMax), z);
	            Vector3 scale = new Vector3(size, size, 0);

	            GameObject star = (GameObject)Instantiate(Resources.Load("Objects/BackgroundStar"), pos, Quaternion.identity);
	            star.transform.localScale = scale;
	            star.GetComponent<SpriteRenderer>().color = new Color(15, 15, 15);
				star.transform.SetParent(Stars.transform);
			}
		}
	}
}