using Assets.SimpleLocalization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaretakerDisplayInfos : MonoBehaviour
{
    public string caretakerName;
    [TextArea]public string description;
    public Sprite icon;
    public int hp;
    public int speed;
    public int dashCooldown;
    public int damage;

	private void Start()
	{
        Setup();
	}

	public void Setup()
	{
        caretakerName = GetComponent<Player>().characterName;
        description = LocalizationManager.Localize("Caretaker." + caretakerName);
	}
}
