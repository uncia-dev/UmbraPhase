using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Umbra.UI {
	public class ChatBox : MonoBehaviour {

	    public static Text boxText;
	    public static GameObject chatBox;
	    public static GameObject scrollView;
	    public static GameObject scrollVertical;
        public static int spamCheck = 0;

	    public void onClickAppendButton()
	    {
	        appendText("This is a call to appendText(...) function.");
	    }

	    public void onClickClearButton()
	    {
	        scrollVertical.GetComponent<Scrollbar>().value = 1;
	        boxText.text = "";
            spamCheck = 0;
	    }

	    public void appendText(string text)
	    {
            spamCheck++;
            boxText.text = boxText.text + "\n" + text;

            if (spamCheck >= 8)
            {
                scrollVertical.GetComponent<Scrollbar>().value = 0;
            }
	    }

		// Use this for initialization
		void Start () {
	        scrollVertical = GameObject.Find("ScrollVertical");
	        chatBox = GameObject.Find("ChatContent");
	        scrollView = GameObject.Find("ScrollView");
	        boxText = chatBox.GetComponent<Text>();
		}
	}
}