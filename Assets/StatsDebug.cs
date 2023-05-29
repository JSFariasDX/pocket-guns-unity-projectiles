using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StatsDebug : MonoBehaviour
{
    [SerializeField] public Player player;
    [SerializeField] TextMeshProUGUI statsText;

    // Update is called once per frame
    void Update()
    {
        if (!player) return;

        statsText.text = player.GetStatusText();
    }
}
