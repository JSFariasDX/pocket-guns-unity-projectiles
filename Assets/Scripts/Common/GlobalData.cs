using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public static class PrefKeys
{
    public const string PGUNS = "PGUNS";
    public const string TOP_SCORE = "TOP_SCORE";
    public const string ATTEMPTS = "ATTEMPTS";
}

public class GlobalData : MonoBehaviour
{
    public static GlobalData Instance;

    public DateTime? startTime { get; set; }
    public DateTime? endTime { get; set; }
    public int score { get; set; }
    public int shoots { get; set; }
    public int stepsMade { get; set; }
    public int killedEnemies { get; set; }
    public int attempts { get; set; }
    public int totalCoins { get; set; }
    public int topScore { get; set; }
    public float spawnPercentage { get; set; }
    public bool canPause { get; set; }

    void Awake()
    {
        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        Instance.attempts = GetValueOrZero(PrefKeys.ATTEMPTS);
        Instance.totalCoins = GetValueOrZero(PrefKeys.PGUNS);
        Instance.topScore = GetValueOrZero(PrefKeys.TOP_SCORE);
        PlayerPrefs.SetInt("Zoio", 1);
        PlayerPrefs.SetInt("Vermillion", 1);
        PlayerPrefs.SetInt("Ike", 1);
        PlayerPrefs.SetInt("Machinegun", 1);
        PlayerPrefs.SetInt("Plasmagun", 1);
        SetPause(true);
    }

    public void SetPause(bool info)
    {
        canPause = info;
    }

    public void SetCoin(int value) 
    {
        //collectedCoins = value;
        UpdateNpcPrices();
    }

    public void AddCoins(int value)
	{
        SetCoin(GetCoins() + value);
    }

    public void UseCoin(int value)
	{
        SetCoin(GetCoins() - value);
    }

    void UpdateNpcPrices()
	{
        var npcsList = FindObjectsOfType<NPC>();
        foreach (var npc in npcsList)
        {
            npc.CheckItemsPrice(GetCoins());
        }
    }

    public int GetCoins()
	{
        return 0;
        //return collectedCoins;
	}

    public void StartRun()
    {
        startTime = System.DateTime.Now;
        if(!canPause)
            SetPause(true);
    }

    public void EndRun(string feedbackText, float transitionDelay = 2f)
    {
        endTime = System.DateTime.Now;
        AddPGUNS(GameplayManager.Instance.TotalCoins);
        IncrementAttempts();
        CheckTopScore(score);
        PlayerPrefs.SetString("GameResult", feedbackText);

        MinimapManager minimapManager = FindObjectOfType<MinimapManager>();
        if (minimapManager) minimapManager.HideMinimap(true);

        Invoke(nameof(GoToEndRun), transitionDelay);
    }

    public string GetTotalTime()
    {
        TimeSpan difference = System.DateTime.Now.Subtract((DateTime)startTime);

        int seconds = PlayerPrefs.GetInt("SECONDS", 0);
        seconds += (int)difference.TotalSeconds;
        PlayerPrefs.SetInt("SECONDS", seconds);

        return $"{difference:mm\\:ss}";
    }

    public void AddScore(int value)
    {
        score += value;
    }

    void GoToEndRun()
    {
        SceneManager.LoadScene("End");
    }

    public void Reset()
    {
        startTime = null;
        endTime = null;
        score = 0;
        shoots = 0;
        stepsMade = 0;
        killedEnemies = 0;
    }

    void AddPGUNS(int toAdd)
    {
        int current = GetValueOrZero(PrefKeys.PGUNS);
        int total = current + toAdd;
        totalCoins = total;
        PlayerPrefs.SetInt(PrefKeys.PGUNS, total);
    }

    public bool hasPGUNSBalance(int toCheck)
    {
        int current = GetValueOrZero(PrefKeys.PGUNS);
        return current >= toCheck;
    }

    public void RemovePGUNS(int toRemove)
    {
        int current = GetValueOrZero(PrefKeys.PGUNS);
        int total = current - toRemove;
        totalCoins = total;
        PlayerPrefs.SetInt(PrefKeys.PGUNS, total);
    }

    void IncrementAttempts()
    {
        int current = GetValueOrZero(PrefKeys.ATTEMPTS);
        int total = current + 1;
        attempts = total;
        PlayerPrefs.SetInt(PrefKeys.ATTEMPTS, total);
    }

    int GetValueOrZero(string key)
    {
        int current = PlayerPrefs.GetInt(key, 0);
        return current;
    }

    void CheckTopScore(int justNow)
    {
        int current = GetValueOrZero(PrefKeys.TOP_SCORE);
        if (justNow > current)
        {
            topScore = justNow;
            PlayerPrefs.SetInt(PrefKeys.TOP_SCORE, justNow);
        }
    }

    public float GetPercentage()
    {
        return spawnPercentage;
    }

    public void AddPercentage(float amount)
    {
        float difference = 100 - spawnPercentage;

        if (amount <= difference)
            spawnPercentage += amount;
        else
            spawnPercentage = 100;
    }

    public void ResetPercentage()
    {
        spawnPercentage = 20;
    }
}
