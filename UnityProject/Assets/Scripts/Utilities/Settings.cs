using UnityEngine;
using System.Collections;

[System.Serializable]
public class Settings {
	public string savePath = "/umbra.usf";
	public float musicVolume = 0.5f;
    public int resHeight = Screen.currentResolution.height;
    public int resWidth = Screen.currentResolution.width;
    public bool windowedMode = !Screen.fullScreen;
    public int refreshRate = Screen.currentResolution.refreshRate;
    public int qualityLevel = QualitySettings.GetQualityLevel(); //0 is not used because it can makes things look bad beyond the point of repair via settings menu
}