using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class PlayerAttributesPanelManager : MonoBehaviour
{
	AttributeTab currentTab;

	[SerializeField] TextMeshProUGUI playerLabelPrefab;
	[SerializeField] Transform playerLabelParent;
	List<TextMeshProUGUI> labels = new List<TextMeshProUGUI>();

	[Header("Header")]
	[SerializeField] GameObject r1Label;
	[SerializeField] GameObject l1Label;

	[Header("Buttons")]
	[SerializeField] AttributeTabButton caretakerButton;
	[SerializeField] AttributeTabButton pocketButton;
	[SerializeField] AttributeTabButton powerUpButton;
	[SerializeField] AttributeTabButton weaponButton;

	[Header("Managers")]
	[SerializeField] CaretakerAttributeManager caretakerManager;
	[SerializeField] PocketAttributeManager pocketManager;
	[SerializeField] PowerUpAttributeManager powerUpManager;
	[SerializeField] WeaponAttributeManager weaponManager;

	private void OnEnable()
	{
		LoadAttributes();

		currentTab = AttributeTab.Caretaker;
		ActiveCurrentTab();

		FindObjectOfType<ItemPopup>().Hide();
	}

	void LoadAttributes()
	{
		CreateLabels();
		caretakerManager.CreatePanels();
		pocketManager.CreatePanels();
		powerUpManager.CreatePanels();
		weaponManager.CreatePanels();
	}

	void CreateLabels()
	{
		ClearLabels();

		//bool isGamepad = FindObjectOfType<TutorialManager>().isGamepad;
		//r1Label.gameObject.SetActive(isGamepad);
		//l1Label.gameObject.SetActive(isGamepad);

		foreach (Player player in GameplayManager.Instance.GetPlayers(false))
		{
			int index = player.GetPlayerIndex() + 1;
			TextMeshProUGUI label = Instantiate(playerLabelPrefab, playerLabelParent);
			label.text = "Player " + index;

			labels.Add(label);
		}
	}

	void ClearLabels()
	{
		foreach(TextMeshProUGUI label in labels) { Destroy(label.gameObject); }
		labels.Clear();
	}

	//public void HorizontalNavigate(InputAction.CallbackContext ctx, Player player)
	//{
	//	if (currentTab == AttributeTab.Pocket)
	//	{
	//		if (ctx.ReadValue<Vector2>().x > 0) pocketManager.GetPlayerPanel(player).Next();
	//		else if (ctx.ReadValue<Vector2>().x < 0) pocketManager.GetPlayerPanel(player).Previous();
	//	}
	//	else if (currentTab == AttributeTab.Weapon)
	//	{
	//		if (ctx.ReadValue<Vector2>().x > 0) weaponManager.GetPlayerPanel(player).Next();
	//		else if (ctx.ReadValue<Vector2>().x < 0) weaponManager.GetPlayerPanel(player).Previous();
	//	}
	//}

	//public void VerticalNavigate(InputAction.CallbackContext ctx, Player player)
	//{
	//	if (currentTab == AttributeTab.PowerUp)
	//	{
	//		if (ctx.ReadValue<Vector2>().y < 0) powerUpManager.GetPlayerOrganizer(player).Next(powerUpManager.GetPanels());
	//		else if (ctx.ReadValue<Vector2>().y > 0) powerUpManager.GetPlayerOrganizer(player).Previous(powerUpManager.GetPanels());
	//	}
	//}

	public void Navigate(InputAction.CallbackContext ctx, Player player)
    {
		if (currentTab == AttributeTab.Pocket)
		{
			if (ctx.ReadValue<Vector2>().x > 0) pocketManager.GetPlayerPanel(player).Next();
			else if (ctx.ReadValue<Vector2>().x < 0) pocketManager.GetPlayerPanel(player).Previous();
		}
		else if (currentTab == AttributeTab.Weapon)
		{
			if (ctx.ReadValue<Vector2>().x > 0) weaponManager.GetPlayerPanel(player).Next();
			else if (ctx.ReadValue<Vector2>().x < 0) weaponManager.GetPlayerPanel(player).Previous();
		}
		else if (currentTab == AttributeTab.PowerUp)
		{
			if (ctx.ReadValue<Vector2>().y < 0) powerUpManager.GetPlayerOrganizer(player).Next(powerUpManager.GetPanels());
			else if (ctx.ReadValue<Vector2>().y > 0) powerUpManager.GetPlayerOrganizer(player).Previous(powerUpManager.GetPanels());
		}
	}

	public void NextTab()
	{
		if ((int)currentTab + 1 > 3) currentTab = AttributeTab.Caretaker;
		else currentTab = currentTab + 1;

		ActiveCurrentTab();
	}

	public void PreviousTab()
	{
		if ((int)currentTab - 1 < 0) currentTab = AttributeTab.Weapon;
		else currentTab = currentTab - 1;

		ActiveCurrentTab();
	}

	public void SetTab(int tabId)
	{
		currentTab = (AttributeTab)tabId;
		ActiveCurrentTab();
	}

	void ActiveCurrentTab()
	{
		EventSystem.current.SetSelectedGameObject(null);

		caretakerManager.gameObject.SetActive(false);
		pocketManager.gameObject.SetActive(false);
		powerUpManager.gameObject.SetActive(false);
		weaponManager.gameObject.SetActive(false);

		caretakerButton.SetSelected(false);
		pocketButton.SetSelected(false);
		powerUpButton.SetSelected(false);
		weaponButton.SetSelected(false);

		if (currentTab == AttributeTab.Caretaker)
		{
			caretakerButton.SetSelected(true);
			EventSystem.current.SetSelectedGameObject(caretakerButton.gameObject);
			caretakerManager.gameObject.SetActive(true);
		}
		else if (currentTab == AttributeTab.Pocket)
		{
			pocketButton.SetSelected(true);
			EventSystem.current.SetSelectedGameObject(pocketButton.gameObject);
			pocketManager.gameObject.SetActive(true);
		}
		else if (currentTab == AttributeTab.PowerUp)
		{
			powerUpButton.SetSelected(true);
			EventSystem.current.SetSelectedGameObject(powerUpButton.gameObject);
			powerUpManager.gameObject.SetActive(true);
		}
		else if (currentTab == AttributeTab.Weapon)
		{
			weaponButton.SetSelected(true);
			EventSystem.current.SetSelectedGameObject(weaponButton.gameObject);
			weaponManager.gameObject.SetActive(true);
		}
	}

	enum AttributeTab 
	{ 
		Caretaker = 0, 
		Pocket = 1, 
		PowerUp = 2, 
		Weapon = 3
	}
}

