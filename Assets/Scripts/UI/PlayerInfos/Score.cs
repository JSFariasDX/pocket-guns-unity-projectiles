using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Score : MonoBehaviour
{
    string scoreKey = "score";
    public int currentScore { get; set; }

    private void Awake()
    {
        currentScore = PlayerPrefs.GetInt(scoreKey);
    }

    public void SetScore(int v)
    {
        PlayerPrefs.SetInt(scoreKey, v);
    }
}
