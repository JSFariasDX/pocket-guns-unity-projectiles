using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Assets.SimpleLocalization;

[System.Serializable]
public class CollectibleDisplayInfo
{
	[SerializeField] string title;
    [TextArea, SerializeField] string description;
	[SerializeField] Sprite sprite;


	public CollectibleDisplayInfo(string title, Sprite sprite)
	{
		this.title = title;
		description = LocalizationManager.Localize("PowerUp." + title);
		this.sprite = sprite;
	}

	public string GetTitle() { return title; }
	public string GetDescription() { return description; }
	public Sprite GetSprite() { return sprite; }
}
