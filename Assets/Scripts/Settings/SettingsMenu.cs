using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] SettingsController settings;

    [Header("Components")]
    public GameObject settingsMenu;
    public GameObject mainMenu;
    public BackgroundController background;
    SoundSettings sound;

    private void Awake()
    {
        sound = SettingsManager.Instance.GetSounds();

        sound.SetVolume(SettingsManager.Instance.Load());
    }


    private void Start()
    {
        settings.LoadSettings();
    }


    public void OpenCloseSettings()
    {
        if (settingsMenu.activeSelf)
        {
            settingsMenu.SetActive(false);
            if (mainMenu != null)
                mainMenu.SetActive(true);

            if (background)
                background.SetMenu(false);
        }
        else
        {
            settingsMenu.SetActive(true);
            if (mainMenu != null)
                mainMenu.SetActive(false);

            if (background)
                background.SetMenu(true);
        }

        settings.LoadSettings();
    }
}
