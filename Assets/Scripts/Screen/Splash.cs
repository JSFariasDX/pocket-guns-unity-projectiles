using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class Splash : MonoBehaviour
{
    public AudioClip theme;
    bool isFlashing = false;

    [Header("UI")]
    public TextMeshProUGUI versionText;

    [SerializeField]
    OST baseOST;

    [Header("Audio Settings")]
    public AudioMixer musicMixer;
    public AudioMixer SFXMixer;

    [Header("SFX")]
    public AudioClip pressAnyButtonSFX;

    private void Start()
    {
        SetSoundVolume(SettingsManager.Instance.Load());

        //ThemeMusicManager.Instance.SetTheme(baseOST);
        //ThemeMusicManager.Instance.StartTheme();
        MusicManager.Instance.PlayMenuTheme(baseOST.baseTrack);
        InputSystem.onAnyButtonPress.CallOnce(ctrl => GoToNextScreen());

        versionText.text = "version " + Application.version;

        
    }

	void SetSoundVolume(Settings settings)
    {
        musicMixer.SetFloat("MasterVolume", Mathf.Log10(settings.musicVolume) * 20);
        SFXMixer.SetFloat("MasterVolume", Mathf.Log10(settings.musicVolume) * 20);
    }

    void Update()
    {
        if (!isFlashing)
        {
            StartCoroutine("FlashText");
        }
    }

    void GoToNextScreen()
    {
        MusicManager.Instance.PlaySFX(pressAnyButtonSFX);
        ScreenManager.Instance.ChangeScreen(Screens.MainMenu);
    }

    IEnumerator FlashText()
    {
        isFlashing = true;
        GameObject textObject = GameObject.Find("PressText");
        TMPro.TextMeshProUGUI t = textObject.GetComponent<TMPro.TextMeshProUGUI>();
        t.color = Color.white;
        yield return new WaitForSeconds(0.5f);
        t.color = Color.yellow;
        yield return new WaitForSeconds(0.5f);
        isFlashing = false;
    }
}
