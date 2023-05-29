using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class ItemPopup : MonoBehaviour
{
	[Header("Components")]
	TutorialManager tutorial;

	public GameObject currentItem;

	public GameObject container;
	public GameObject gunInfos;
	public GameObject itemInfos;
	public float infoDisplayTime;
	public Image buttonIcon;

	[Header("Gun Infos")]
	public TextMeshProUGUI gunNameText;
	public TextMeshProUGUI damageText;
	public TextMeshProUGUI bulletsText;
	public TextMeshProUGUI fireRateText;

	[Header("Item Infos")]
	public TextMeshProUGUI itemNameText;
	public TextMeshProUGUI itemDescriptionText;

	[Header("General Settings")]
	public List<Color> rarityColors = new List<Color>();

	[Header("UI")]
	Sprite icon;

	private Player currentPlayer;

	private void Start()
	{
		GetComponent<Canvas>().worldCamera = Camera.main;
		gunInfos.SetActive(false);
		itemInfos.SetActive(false);

		tutorial = FindObjectOfType<TutorialManager>();
	}

	public void Hide()
	{
		currentPlayer = null;
		container.SetActive(false);
		transform.SetParent(null);
	}

	public void ShowGunInfo(Transform loot, Gun gun, Player player)
	{
		container.SetActive(true);
		transform.SetParent(loot);
		transform.localPosition = Vector3.up;
		transform.localScale = Vector3.one * .7f;
		gunInfos.SetActive(true);
		itemInfos.SetActive(false);

		currentItem = gun.gameObject;

		gunNameText.text = gun.gunName;
		gunNameText.color = rarityColors[(int)gun.rarity];
		damageText.text = "Damage: " + gun.damage;
		bulletsText.text = "Bullets: " + gun.maxBullets;
		fireRateText.text = "Fire Rate: " + gun.fireRate.ToString("F");

		currentPlayer = player;

		SetIcon();

		//StartCoroutine(EnableInfo(gunInfos, gun.gameObject));
	}

	public void ShowItemInfo(Transform popUpPosition, Collectible collectible, Player player)
	{
		container.SetActive(true);
		transform.SetParent(popUpPosition, true);
		transform.localPosition = Vector3.up * 1.5f;
		itemInfos.SetActive(true);
		gunInfos.SetActive(false);

        currentItem = collectible.gameObject;

        itemNameText.text = collectible.GetName();
        itemDescriptionText.text = collectible.GetDescription();
        itemDescriptionText.text = "";
        itemDescriptionText.text = collectible.GetDescription();

        currentPlayer = player;

        SetIcon();
	}

	public void ShowPocketInfo(Transform popUpPosition, Pocket pocket, Player player)
    {
		container.SetActive(true);
		transform.SetParent(popUpPosition, true);
		transform.localPosition = Vector3.up * 1.5f;
		itemInfos.SetActive(true);
		gunInfos.SetActive(false);

		currentItem = pocket.gameObject;

		itemNameText.text = pocket.pocketName;
		itemDescriptionText.text = pocket.specialDescription;
		itemDescriptionText.text = "";
		itemDescriptionText.text = pocket.specialDescription;

		currentPlayer = player;

		SetIcon();
	}

	void SetIcon()
    {
		bool isUsingController = FindObjectOfType<PlayerInputController>();

		string buttonName;
		bool isGamepad;

		if (isUsingController)
		{
			buttonName = InputControlPath.ToHumanReadableString(currentPlayer.GetInputController().GetInput().actions.FindAction("Interact").bindings[currentPlayer.GetInputController().IsGamepad() ? 0 : 1].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
			InputControlPath.ToHumanReadableString(currentPlayer.GetInputController().GetInput().actions.FindAction("Interact").bindings[currentPlayer.GetInputController().IsGamepad() ? 0 : 1].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
			isGamepad = currentPlayer.GetInputController().IsGamepad();
		}
		else
		{
			buttonName = InputControlPath.ToHumanReadableString(tutorial.control.FindAction("Interact").bindings[tutorial.isGamepad ? 0 : 1].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
			InputControlPath.ToHumanReadableString(tutorial.control.FindAction("Interact").bindings[tutorial.isGamepad ? 0 : 1].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
			isGamepad = tutorial.isGamepad;
		}

		if (isGamepad)
		{
			if (isUsingController)
			{
				for (int i = 0; i < tutorial.gamepadButtons.Count; i++)
				{
					if (tutorial.gamepadButtons[i].name == buttonName)
					{
						switch (currentPlayer.GetInputController().GetControllerType())
						{
							case ControllerType.DualShock:
								icon = tutorial.gamepadButtons[i].PlayStationIcon;
								break;
							case ControllerType.Xbox:
								icon = tutorial.gamepadButtons[i].XboxIcon;
								break;
							case ControllerType.Switch:
								icon = tutorial.gamepadButtons[i].SwitchIcon;
								break;
							case ControllerType.Keyboard:
								icon = tutorial.gamepadButtons[i].PlayStationIcon;
								break;
							default:
								icon = tutorial.gamepadButtons[i].PlayStationIcon;
								break;
						}
					}
					else
						continue;
				}
			}
			else
			{
				for (int i = 0; i < tutorial.gamepadButtons.Count; i++)
				{
					if (tutorial.gamepadButtons[i].name == buttonName)
					{
						switch (tutorial.type)
						{
							case ControllerType.DualShock:
								icon = tutorial.gamepadButtons[i].PlayStationIcon;
								break;
							case ControllerType.Xbox:
								icon = tutorial.gamepadButtons[i].XboxIcon;
								break;
							case ControllerType.Switch:
								icon = tutorial.gamepadButtons[i].SwitchIcon;
								break;
							case ControllerType.Keyboard:
								icon = tutorial.gamepadButtons[i].PlayStationIcon;
								break;
							default:
								icon = tutorial.gamepadButtons[i].PlayStationIcon;
								break;
						}
					}
					else
						continue;
				}
			}
		}
		else
		{
			for (int i = 0; i < tutorial.keyboard.Count; i++)
			{
				if (tutorial.keyboard[i].name == buttonName)
				{
					icon = tutorial.keyboard[i].icon;
					break;
				}
				else
					continue;
			}
		}

		buttonIcon.sprite = icon;
	}
}
