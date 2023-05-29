using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class LoadingScreenManager : MonoBehaviour
{
    [Header("Components")]
    public CanvasGroup loadingCanvas;
    public float fadeSpeed = 2;
    bool opening = false;
    bool loading = false;
    [SerializeField] List<GameObject> icons = new List<GameObject>();
    GameObject currentIcon;

    [SerializeField] private AudioMixer SfxMixer;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (opening)
        {
            if (loadingCanvas.alpha < 1) loadingCanvas.alpha += (Time.unscaledDeltaTime * fadeSpeed);
            else
            {
                if (!currentIcon)
                {
                    SelectRandomLoadingIcon();
                }
            }
        }
        else
        {
            if (loadingCanvas.alpha >= 0) loadingCanvas.alpha -= (Time.unscaledDeltaTime * fadeSpeed);
            else
            {
                if (currentIcon)
                {
                    currentIcon.SetActive(false);
                    currentIcon = null;
                }
            }
        }

        if (loadingCanvas.alpha >= 1) loading = true;
        else loading = false;
    }

    public void OpenLoading()
    {
        opening = true;

        if(SfxMixer)
            SfxMixer.SetFloat("MasterVolume", Mathf.Log10(0.0001f) * 20);
        //if (ThemeMusicManager.Instance != null)
        //    ThemeMusicManager.Instance.StopAllSources();
        if (MusicManager.Instance != null)
            MusicManager.Instance.StopAll();

        //Time.timeScale = 0f;
    }

    public void CloseLoading()
    {
        opening = false;

        if (SfxMixer)
            SfxMixer.SetFloat("MasterVolume", Mathf.Log10(SettingsManager.Instance.Load().SFXVolume) * 20);
        //Time.timeScale = 1f;
    }

    void SelectRandomLoadingIcon()
    {
        currentIcon = icons[Random.Range(0, icons.Count)];
        currentIcon.SetActive(true);
    }

    public bool GetIsLoading()
    {
        return opening;
    }

    public bool GetCanLoad()
    {
        return loading;
    }
}
