using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextUpdateByPlayerPrefs : MonoBehaviour
{
    public PlayerPrefsType playerPrefsType;
    public string playerPrefsKey;
	public TextMeshProUGUI textMesh;

	private void Start()
	{
		if (playerPrefsType == PlayerPrefsType.Float)
		{
			textMesh.text = PlayerPrefs.GetFloat(playerPrefsKey).ToString();
		}
		else if (playerPrefsType == PlayerPrefsType.Int)
		{
			textMesh.text = PlayerPrefs.GetInt(playerPrefsKey).ToString();
		}
		else
		{
			textMesh.text = PlayerPrefs.GetString(playerPrefsKey);
		}
	}
}

public enum PlayerPrefsType
{
    String, Float, Int
}
