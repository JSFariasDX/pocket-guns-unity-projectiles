using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DefeatedEnemiesGui : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI playerNameText;
    [SerializeField] TextMeshProUGUI defeatedsCountText;

    public void Setup(string playerName, int defeatedsCount)
	{
        playerNameText.text = playerName;
        defeatedsCountText.text = defeatedsCount.ToString();
    }
}
