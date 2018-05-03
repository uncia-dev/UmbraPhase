using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Umbra.UI
{
    public class Obj_GUI_Window: MonoBehaviour
    {

        private static Rect windowSize;
        private static string title;
        private static bool show = false;
        private static string text;
        private static List<string> tempstring;
        private GUIStyle mystyle;
        private GUIStyle headerstyle;

        public void Start()
        {
            //The window is centered and sized relative to the screen width and height
            float w = 0.5f;
            float h = 0.6f;
            windowSize.x = (Screen.width * (1 - w)) / 2;
            windowSize.y = (Screen.height * (1 - h)) / 2;
            windowSize.width = Screen.width * w;
            windowSize.height = Screen.height * h;
            tempstring = GameStateManager.Instance.gameState.Objectives;
            foreach (var i in tempstring)
            {
                text = text + i + "\n";
            }
            mystyle = new GUIStyle();
            headerstyle = new GUIStyle();
            mystyle.alignment = TextAnchor.MiddleCenter;
            mystyle.fontSize = 14;
            headerstyle.normal.textColor = mystyle.normal.textColor = Color.white;
            headerstyle.fontSize = 20;
            headerstyle.alignment = TextAnchor.UpperCenter;
        }
        public void OnGUI()
        {
            if (show)
            {
                
                windowSize = GUI.Window(0, windowSize, objWindow, "Objectives");
            }
        }
        void objWindow(int ID)
        {
            GUI.Label(new Rect(0, 20, windowSize.width, windowSize.height-20), text, mystyle); //text of objs
            if (GUI.Button(new Rect((windowSize.width)-20,0,20,20), "X")) //close button
            {
                show = false;
            }
           // GUI.DragWindow();
        }
        public static void Open()
        {
            show = true;
        }
    }
}
