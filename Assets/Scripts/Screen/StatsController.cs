using System;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class StatsController : MonoBehaviour
{
    PlayerControls controls;

    [Header("Components")]
    public CanvasGroup statsCanvas;
    public GameObject returnButton;

    [Header("Texts")]
    public TextMeshProUGUI timePlayedValueText;
    public TextMeshProUGUI encountersValueText;
    public TextMeshProUGUI defeatsValueText;
    public TextMeshProUGUI monstersValueText;
    public TextMeshProUGUI dungeonsValueText;
    public TextMeshProUGUI shotsFiredValueText;
    public TextMeshProUGUI powerUpsValueText;
    public TextMeshProUGUI coinsValueText;
    public TextMeshProUGUI DNAValueText;

    bool isEnable = false;

    //private void Awake()
    //{
    //    controls = new PlayerControls();

    //    controls.UI.Return.performed += _ => EnableStats(false);
    //}

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EnableStats(bool enable)
    {
        SetupValues();

        if (enable)
            PauseManager.Instance.SimplePause();
        else
            PauseManager.Instance.SimpleResume();

        List<Player> player = GameplayManager.Instance.GetPlayers(false);
        foreach (var item in player)
        {
            item.GetInputController().SetMapInput(enable ? "UI" : "Gameplay");
        }

        statsCanvas.alpha = enable ? 1 : 0;
        statsCanvas.interactable = enable;
        statsCanvas.blocksRaycasts = enable;

        isEnable = enable;
    }

    void SetupValues()
    {
        int seconds = PlayerPrefs.GetInt("SECONDS", 0);
        //int seconds = 96846574;
        TimeSpan span = TimeSpan.FromSeconds(seconds);
        string time = string.Format("{0:D2}:{1:D2}:{2:D2}",
                span.Days * 24 + span.Hours,
                span.Minutes, 
                span.Seconds);

        timePlayedValueText.text = time;
        encountersValueText.text = PlayerPrefs.GetInt("ROOMS", 0).ToString();
        defeatsValueText.text = PlayerPrefs.GetInt("DEFEAT", 0).ToString();
        monstersValueText.text = PlayerPrefs.GetInt("ENEMIES", 0).ToString();
        dungeonsValueText.text = PlayerPrefs.GetInt("DUNGEONS", 0).ToString();
        shotsFiredValueText.text = PlayerPrefs.GetInt("SHOTS", 0).ToString();
        powerUpsValueText.text = PlayerPrefs.GetInt("POWER UP", 0).ToString();
        coinsValueText.text = PlayerPrefs.GetInt("COINS", 0).ToString();
        DNAValueText.text = PlayerPrefs.GetInt("DNA", 0).ToString();
    }

    public bool GetStatsEnabled()
    {
        return isEnable;
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
