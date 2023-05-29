using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class CutsceneLoadingScreen : MonoBehaviour
{
    public bool IsCutsceneInProgress { get; private set; }

    [SerializeField] private AudioMixer SfxMixer;

    [Header("Background References")]
    [SerializeField] private Image backgroundImage;
    [SerializeField] private List<Sprite> backgroundSprites;

    [Header("Player References")]
    [SerializeField] private Image player1Image;

    private System.Action _cutsceneAction;

    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        gameObject.SetActive(false);
    }

    public void StartCutscene(System.Action action)
    {
        //_animator.SetTrigger("Start");

        SfxMixer.SetFloat("MasterVolume", Mathf.Log10(0.0001f) * 20);
        //if (ThemeMusicManager.Instance != null)
        //    ThemeMusicManager.Instance.StopAllSources();
        if (MusicManager.Instance != null)
            MusicManager.Instance.StopAll();

        IsCutsceneInProgress = true;
        Time.timeScale = 0f;

        var backgroundIndex = PlayerPrefs.GetInt("CurrentDungeon");
        Debug.Log($"current dungeon: {backgroundIndex}");

        if (backgroundIndex >= backgroundSprites.Count)
            backgroundIndex = backgroundSprites.Count - 1;

        backgroundImage.sprite = backgroundSprites[backgroundIndex];
        player1Image.sprite = SelectionManager.Instance.currentCharacters[0].cutsceneSprite;

        gameObject.SetActive(true);
    }

    public void OnCutsceneEnd()
    {
        SfxMixer.SetFloat("MasterVolume", Mathf.Log10(SettingsManager.Instance.Load().SFXVolume) * 20);
        Time.timeScale = 1f;
        IsCutsceneInProgress = false;
        // _cutsceneAction?.Invoke();
    }
}
