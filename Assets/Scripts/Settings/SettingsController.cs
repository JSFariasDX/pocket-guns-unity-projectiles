using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;

public class SettingsController : MonoBehaviour
{
    PlayerControls controls;

    [HideInInspector]
    public TutorialManager tutorial;

    [Header("Video Settings")]
    public TMP_Dropdown resolutionDropdown;
    public GameObject resolutionsButton;
    [SerializeField] bool resolutionSelected = false;
    public TextMeshProUGUI resolutionText;
    Resolution[] resolutions;
    Resolution currentResolution;
    int resolutionIndex = 0;
    bool fullscreen = true;

    [Header("Controls Settings")]
    public bool vibration = true;
    public GameObject controlsMenu;
    public GameObject settingsBackground;
    public GameObject controllerMenu;
    public GameObject keyboardMenu;

    [Header("Audio Settings")]
    public AudioMixer musicMixer;
    public AudioMixer SFXMixer;
    float musicVolume;
    float SFXVolume;

    [Header("UI")]
    // Video
    public Toggle fullscreenToggle;

    // Controls
    public Toggle vibrationToggle;

    // Music
    public Slider musicSlider;
    public Slider SFXSlider;

    [SerializeField] GameObject openSettingsFocusObject;
    [SerializeField] GameObject controlsPanelFocusObject;
    [SerializeField] GameObject returnButton;

    private void Awake()
    {
        controls = new PlayerControls();

        Settings config = new Settings
        {
            fullscreen = true,

            width = 1280,
            height = 720,

            vibration = true,

            musicVolume = 0,
            SFXVolume = 0
        };

        string json = JsonUtility.ToJson(config);

        tutorial = FindObjectOfType<TutorialManager>();
		resolutions = Screen.resolutions;

        LoadSettings();

        if(resolutionSelected)
            controls.Debug.ResolutionChange.performed += ctx => SelectResolution(ctx.ReadValue<Vector2>().x);

        controls.Debug.ResolutionChange.canceled += ctx => SelectResolution(0);
    }

    public void CloseSettings()
    {
        SettingsMenu menu = GetComponentInParent<SettingsMenu>();
        menu.OpenCloseSettings();
    }

    // Start is called before the first frame update
    void Start()
    {
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();

        int currentResIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + "x" + resolutions[i].height + "x" + resolutions[i].refreshRate + "Hz";
            options.Add(option);

            if(resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
            {
                currentResIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResIndex;
        resolutionDropdown.RefreshShownValue();

        musicSlider.value = GetAudioMixerLevel(musicMixer);
        SFXSlider.value = GetAudioMixerLevel(SFXMixer);

        LoadSettings();
    }

    void SelectResolution(float direction)
    {
        if (!resolutionSelected) return;

        if (direction > 0)
        {
            if (resolutionIndex >= resolutions.Length - 1)
                resolutionIndex = 0;
            else
                resolutionIndex++;
        }
        else if(direction < 0)
        {
            if (resolutionIndex <= 0)
                resolutionIndex = resolutions.Length - 1;
            else
                resolutionIndex--;
        }

        SetResolution(resolutionIndex);
    }

    public void SelectResolutionButton(float direction)
    {
        if (direction > 0)
        {
            if (resolutionIndex >= resolutions.Length - 1)
                resolutionIndex = 0;
            else
                resolutionIndex++;
        }
        else if (direction < 0)
        {
            if (resolutionIndex <= 0)
                resolutionIndex = resolutions.Length - 1;
            else
                resolutionIndex--;
        }

        SetResolution(resolutionIndex);
    }

    // Update is called once per frame
    void Update()
    {
        if (EventSystem.current.currentSelectedGameObject == resolutionsButton) resolutionSelected = true;
        else resolutionSelected = false;

        resolutionText.text = resolutions[resolutionIndex].width + "x" + resolutions[resolutionIndex].height + " (" + resolutions[resolutionIndex].refreshRate + "Hz)";

        if (PauseManager.Instance != null && PauseManager.Instance.IsGamePaused())
        {
            if (EventSystem.current.currentSelectedGameObject == null)
            {
                if (controlsMenu.activeSelf)
                {
                    EventSystem.current.SetSelectedGameObject(controlsPanelFocusObject);
                }
                else
                {
                    EventSystem.current.SetSelectedGameObject(openSettingsFocusObject);
                }
            }
        }

        if (tutorial.isGamepad)
        {
            keyboardMenu.SetActive(false);
            controllerMenu.SetActive(true);
        }
        else
        {
            keyboardMenu.SetActive(true);
            controllerMenu.SetActive(false);
        }
    }

    public void OpenCloseControlsMenu()
    {
        if (controlsMenu.activeSelf)
        {
            controlsMenu.SetActive(false);
            settingsBackground.SetActive(true);
        }
        else
        {
            controlsMenu.SetActive(true);
            settingsBackground.SetActive(false);
        }
    }

    // Video
    public void SetResolution(int index)
    {
        Resolution res = resolutions[index];
        currentResolution = res;
    }

    void ApplyResolution(int width, int height)
    {
        resolutions = Screen.resolutions;

        Screen.SetResolution(width, height, fullscreen);

        int currentResIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            if (resolutions[i].width == width && resolutions[i].height == height)
            {
                currentResIndex = i;
            }
        }

        resolutionIndex = currentResIndex;

        resolutionDropdown.value = currentResIndex;
        resolutionDropdown.RefreshShownValue();
    }

    public void SetMinRes()
    {
        ApplyResolution(640, 320);
    }

    public void SetMidRes()
    {
        ApplyResolution(1440, 720);
    }

    public void SetMaxRes()
    {
        ApplyResolution(1920, 1080);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        fullscreen = isFullscreen;
    }

    // Sound
    public void SetMusicVolume(float volume)
    {
        musicVolume = volume;
        musicMixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 20);
    }

    public void SetSFXVolume(float volume)
    {
        SFXVolume = volume;
        SFXMixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 20);
    }

    // Controls
    public void SetVibration(bool isVibrationActive)
    {
        vibration = isVibrationActive;
    }

    public float GetAudioMixerLevel(AudioMixer mixer)
    {
        float value;
        bool result = mixer.GetFloat("MasterVolume", out value);
        if (result)
        {
            return value;
        }
        else
        {
            return 0f;
        }
    }

    public void SaveSettings()
    {
        ApplyResolution(currentResolution.width, currentResolution.height);

        if (vibration == true)
            PlayerPrefs.SetInt("Rumble", 1);
        else
            PlayerPrefs.SetInt("Rumble", 0);

        PlayerPrefs.SetFloat("Music Volume", musicVolume);
        PlayerPrefs.SetFloat("SFX Volume", SFXVolume);

        Settings config = new Settings
        {
            fullscreen = fullscreen,

            width = currentResolution.width,
            height = currentResolution.height,

            vibration = vibration,

            musicVolume = musicVolume,
            SFXVolume = SFXVolume
        };

        SettingsManager.Instance.Save(config);

        print("SAVED");
    }

    void LoadValues(Settings loadedValues)
    {
        SetFullscreen(loadedValues.fullscreen);
        ApplyResolution(loadedValues.width, loadedValues.height);
        SetVibration(loadedValues.vibration);
        SetMusicVolume(loadedValues.musicVolume);
        SetSFXVolume(loadedValues.SFXVolume);
    }

    public void LoadSettings()
    {
        LoadValues(SettingsManager.Instance.Load());
        RefreshUI();

        print("LOADED");
    }

    void RefreshUI()
    {
        int currentResIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            if (resolutions[i].width == currentResolution.width && resolutions[i].height == currentResolution.height)
            {
                currentResIndex = i;
            }
        }

        resolutionDropdown.value = currentResIndex;
        resolutionDropdown.RefreshShownValue();

        fullscreenToggle.isOn = fullscreen;

        vibrationToggle.isOn = vibration;

        musicSlider.value = musicVolume;
        SFXSlider.value = SFXVolume;
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }
}

public class Settings
{
    // Video
    public bool fullscreen = true;
    public int width = 1280;
    public int height = 720;

    // Controls
    public bool vibration = true;

    // Sound
    public float musicVolume = 0;
    public float SFXVolume = 0;
}
