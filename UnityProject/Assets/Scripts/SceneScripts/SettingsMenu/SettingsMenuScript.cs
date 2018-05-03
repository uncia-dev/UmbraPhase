using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;
using System.Linq;
using Umbra.Data;

namespace Umbra.Scenes.SettingsMenu
{
    public class SettingsMenuScript : MonoBehaviour
    {
        public GameObject slider;
        public GameObject volumeLbl;
        public static Settings updatedSettings = new Settings(); // keep an instance of updated settings and then re-assign save file's settings with this if hasBenUpdated flagged true
        public static bool hasBeenUpdated;
        public GameObject returnBtn;
        public GameObject saveBtn;
        public Resolution[] supportedResolutions;
        public int selectedRes = 0;
        public GameObject resLabel;
        public GameObject windowedToggle;
        public GameObject refreshRateSlider;
        public GameObject refreshRateLabel;
        public bool hasLoadedRecent;
        public GameObject graphicSlider;
        public GameObject graphicLabel;
        public bool justOpenedMenu;

        public void ResLeft()
        {
            if (supportedResolutions.Length >= 1)
            {
                changesHaveBeenMade();
                selectedRes--;
                if (selectedRes == -1)
                {
                    selectedRes = supportedResolutions.Length - 1;
                }
                resLabel.GetComponent<Text>().text = supportedResolutions[selectedRes].width.ToString() + " x " + supportedResolutions[selectedRes].height.ToString();
            }
        }

        public void ResRight()
        {
            if (supportedResolutions.Length >= 1)
            {
                changesHaveBeenMade();
                selectedRes++;
                if (selectedRes == supportedResolutions.Length)
                {
                    selectedRes = 0;
                }
                resLabel.GetComponent<Text>().text = supportedResolutions[selectedRes].width.ToString() + " x " + supportedResolutions[selectedRes].height.ToString();
            }
        }

        void Start()
        {
            justOpenedMenu = true;
            refreshRateSlider.GetComponent<Slider>().value = Screen.currentResolution.refreshRate;

            if (Screen.fullScreen)
            {
                windowedToggle.GetComponent<Toggle>().isOn = false;
            }
            else
            {
                windowedToggle.GetComponent<Toggle>().isOn = true;
            }

            resLabel.GetComponent<Text>().text = Screen.currentResolution.ToString();
            supportedResolutions = Screen.resolutions.Where(r => r.width >= 800).ToArray();
            saveBtn.SetActive(false);
            bool active = false;
            DirectoryInfo directory = new DirectoryInfo(Application.persistentDataPath);
            FileInfo latest = null;
            try
            {
                latest = directory.GetFiles().OrderByDescending(f => f.LastWriteTime).First();
                active = File.Exists(Application.persistentDataPath + "/" + latest.Name);
            }
            catch (System.Exception ex)
            {
            }

            if (active)
            {
                hasLoadedRecent = true;
                //retrieves settings from most recently written save
                GameStateManager.Instance.currentSavePath = "/" + latest.Name;
                GameStateManager.Instance.LoadGame();
                slider.GetComponent<Slider>().value = GameStateManager.Instance.settings.musicVolume * 10;
                volumeLbl.GetComponent<Text>().text = GameStateManager.Instance.settings.musicVolume.ToString();
                windowedToggle.GetComponent<Toggle>().isOn = GameStateManager.Instance.settings.windowedMode;
                resLabel.GetComponent<Text>().text = GameStateManager.Instance.settings.resWidth.ToString() + " x " + GameStateManager.Instance.settings.resHeight.ToString();
                refreshRateSlider.GetComponent<Slider>().value = GameStateManager.Instance.settings.refreshRate;
                refreshRateLabel.GetComponent<Text>().text = GameStateManager.Instance.settings.refreshRate.ToString() + "Hz";
                graphicSlider.GetComponent<Slider>().value = GameStateManager.Instance.settings.qualityLevel;
                setGraphicLabel();
            }
            else
            {
                //default
                slider.GetComponent<Slider>().value = updatedSettings.musicVolume * 10;
                windowedToggle.GetComponent<Toggle>().isOn = !Screen.fullScreen;
                resLabel.GetComponent<Text>().text = Screen.currentResolution.width.ToString() + " x " + Screen.currentResolution.height.ToString();
                refreshRateSlider.GetComponent<Slider>().value = Screen.currentResolution.refreshRate;
                refreshRateLabel.GetComponent<Text>().text = Screen.currentResolution.refreshRate.ToString();
                graphicSlider.GetComponent<Slider>().value = QualitySettings.GetQualityLevel();
                setGraphicLabel();
            }
            justOpenedMenu = false;
        }

        public void updateGraphicSlider()
        {
            changesHaveBeenMade();
            updatedSettings.qualityLevel = (int) graphicSlider.GetComponent<Slider>().value;
            setGraphicLabel();
        }

        public void setGraphicLabel()
        {
            int level = (int) graphicSlider.GetComponent<Slider>().value;

            //0 is not used because it destroys the graphics beyond repair of this menu
            if (level == 0)
            {
                graphicLabel.GetComponent<Text>().text = "Very Low";
            }
            if (level == 1)
            {
                graphicLabel.GetComponent<Text>().text = "Low";
            }
            if (level == 2)
            {
                graphicLabel.GetComponent<Text>().text = "Medium";
            }
            if (level == 3)
            {
                graphicLabel.GetComponent<Text>().text = "High";
            }
            if (level == 4)
            {
                graphicLabel.GetComponent<Text>().text = "Very High";
            }
            if (level == 5)
            {
                graphicLabel.GetComponent<Text>().text = "Super";
            }
        }

        public void refreshRateMoved()
        {
            changesHaveBeenMade();
            updatedSettings.refreshRate = (int)(refreshRateSlider.GetComponent<Slider>().value);
            refreshRateLabel.GetComponent<Text>().text = ((int)(refreshRateSlider.GetComponent<Slider>().value)).ToString() +"Hz";
        }

        public void sliderMove()
        {
            changesHaveBeenMade();
            float volume = slider.GetComponent<Slider>().value / 10;
            updatedSettings.musicVolume = volume;
            volumeLbl.GetComponent<Text>().text = volume.ToString();
        }

        public void changesHaveBeenMade()
        {
            if (!justOpenedMenu)
            {
                if (!saveBtn.activeSelf)
                {
                    saveBtn.SetActive(true);
                }
                returnBtn.GetComponentInChildren<Text>().text = "Discard";
            }
        }

        public void ReturnButton()
        {
            GameStateManager.Instance.PushScene(GameScene.MainMenu);
        }

        public void SaveSettings()
        {
            returnBtn.GetComponentInChildren<Text>().text = "Return";
            string currRes = resLabel.GetComponent<Text>().text;
            hasBeenUpdated = true;
            foreach (var res in supportedResolutions)
            {
                if (res.ToString().Contains(currRes))
                {
                    updatedSettings.resHeight = res.height;
                    updatedSettings.resWidth = res.width;
                }
            }
            updatedSettings.windowedMode = windowedToggle.GetComponent<Toggle>().isOn;
            //Apply settings
            try
            {
                if (updatedSettings.resHeight == 0)
                {
                    updatedSettings.resHeight = Screen.currentResolution.height;
                    updatedSettings.resWidth = Screen.currentResolution.width;
                }
                Screen.SetResolution(updatedSettings.resWidth, updatedSettings.resHeight, !updatedSettings.windowedMode);
                QualitySettings.SetQualityLevel(updatedSettings.qualityLevel);

                //gets applied to the most recent save by default
                if (hasLoadedRecent)
                {
                    GameStateManager.Instance.settings = updatedSettings;
                    GameStateManager.Instance.SaveGame();
                }
            }
            catch (System.Exception ex)
            {
                Debug.Log("Could not update settings.");
            }
        }

        public static void ApplySettings(Settings setting)
        {
            //Apply settings
            try
            {
                Screen.SetResolution(setting.resWidth, setting.resHeight, !setting.windowedMode, GameStateManager.Instance.settings.refreshRate);
                QualitySettings.SetQualityLevel(setting.qualityLevel);
            }
            catch (System.Exception ex)
            {
                Debug.Log("Could not update settings.");
            }
        }
    }
}