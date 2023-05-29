using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerEntryManager : MonoBehaviour
{
	PlayerControls controls;

	public List<PlayerEntryPanel> entries = new List<PlayerEntryPanel>();
	public List<PlayerEntryPanelSlot> playerEntryPanelSlots = new List<PlayerEntryPanelSlot>();
	[SerializeField] TextMeshProUGUI startingGameText;
	[SerializeField] GameObject loadingScreen;

	PlayerInputManager inputManager;
	float startingTimer;
	bool starting;
	bool started;

	[Header("Info")]
	public CaretakerAttributesPanel caretakerPanel;

	[Header("Background")]
	public BackgroundController background;

	public bool enterWithCheat = false;

	private void Awake()
	{
		controls = new PlayerControls();

		controls.PlayerEntry.Cancel.performed += _ => LeaveGame(false);
		controls.PlayerEntry.Cheat.performed += _ => enterWithCheat = true;
		controls.PlayerEntry.Cheat.canceled += _ => enterWithCheat = false;
	}

	public void LeaveGame(bool firstPlayer)
    {
		if (entries.Count <= 0 || firstPlayer)
		{
			//background.SetMenu(false);
			BackToMenu();
		}
    }

	void BackToMenu()
    {
		ScreenManager.Instance.ChangeScreen(Screens.MainMenu, false);
		Destroy(GameObject.Find("EntryCanvas"));
	}

	private void Start()
	{
		//Invoke("SetBackground", .4f);
		caretakerPanel.gameObject.SetActive(false);

		inputManager = GetComponent<PlayerInputManager>();
		startingGameText.text = "";
		//loadingScreen.SetActive(false);
	}

	void SetBackground()
    {
		background.SetMenu(true);
	}

	private void Update()
	{
		if (GetPlayerCount() > 0)
        {
			caretakerPanel.gameObject.SetActive(true);
			caretakerPanel.Setup(entries[0].selectedTent.inGameCharacter.GetComponent<Player>());
        }

		if (AllConfirmed() && !starting && GetPlayerCount() > 0)
		{
			caretakerPanel.transform.GetChild(0).GetComponent<UnityEngine.UI.Image>().color = Color.green;
			inputManager.DisableJoining();
			starting = true;
			startingTimer = 1;
		}

		startingTimer -= Time.deltaTime;
		if (startingTimer > 0) startingGameText.text = "";
		else if (starting && startingTimer <= 0 && !started)
        {
			Destroy(GetComponent<AudioListener>());
			StartGame();
        }
	}

	public void CancelStarting()
    {
		startingTimer = 0;
		starting = false;
		startingGameText.text = "";
		inputManager.EnableJoining();
	}

	int GetPlayerCount()
	{
		//int playerCount = 0;
		int playerCount = entries.Count;

		//foreach (PlayerEntryPanelSlot slot in playerEntryPanelSlots)
		//{
		//	if (slot.GetPanel())
		//	{
		//		playerCount++;
		//	}
		//}

		return playerCount;
	}

	bool AllConfirmed()
	{
		foreach (PlayerEntryPanel slot in entries)
		{
			if (!slot.IsConfirmed())
			{
				return false;
			}
		}

		return true;
	}

	void StartGame()
	{
		if (starting)
		{
			inputManager.DisableJoining();

			int index = 0;
			foreach (PlayerEntryPanel slot in entries)
			{
				// Player selection setup
				slot.GetCurrentPlayer().playerIndex = index;
				SelectionManager.Instance.AddCharacter(slot.GetCurrentPlayer());
				SelectionManager.Instance.AddPocket(slot.GetCurrentPocket().pocket);

				slot.GetComponent<PlayerInput>().SwitchCurrentActionMap("Gameplay");
				slot.background.transform.parent = slot.transform;
				slot.selectedTent.inGameCharacter.SetActive(false);
				slot.selectedTent.returnInteraction.SetActive(true);
				DontDestroyOnLoad(slot.gameObject);

				index++;
			}

            //FindObjectOfType<PauseManager>(true).gameObject.SetActive(true);
            //FindObjectOfType<StatsController>(true).gameObject.SetActive(true);
            FindObjectOfType<PlayerManager>().TutorialSpawnPlayer(Vector2.zero);
            FindObjectOfType<LobbyController>().StartGame();

            //GameObject.Find("Tutorial").SetActive(true);
            //MainMenu.GoToLobby();

            //MainMenu.StartGame(Difficulty.Easy);
            started = true;
			GetComponent<AudioSource>().Play();
			gameObject.SetActive(false);
		}
	}

	public void OnPlayerJoined(PlayerEntryPanel panel)
	{
		foreach(PlayerEntryPanelSlot slot in playerEntryPanelSlots)
		{
			if (!slot.GetPanel())
			{
				slot.SetPanel(panel);
				panel.slot = slot;
				
				return;
			}
		}

		entries.Add(panel);
	}

	public void OnPlayerLeft()
	{
		
	}

	private void OnEnable()
	{
		controls.Enable();
	}

	private void OnDisable()
	{
		controls.Disable();
	}
}
