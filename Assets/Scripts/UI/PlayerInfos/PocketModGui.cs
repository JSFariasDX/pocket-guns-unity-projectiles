using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PocketModGui : MonoBehaviour
{
    public Pocket pocket;
    public TextMeshProUGUI modTitleText;
    public TextMeshProUGUI modValueText;

    public void Setup(Pocket pocket, string title, float value)
	{
        this.pocket = pocket;
        modTitleText.text = title;
        modValueText.text = "+" + value.ToString();
	}
}
