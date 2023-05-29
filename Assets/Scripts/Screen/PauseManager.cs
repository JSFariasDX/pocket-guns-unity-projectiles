using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance;

    public GameObject pauseMenuUI;
    public GameObject simplePauseMenuUI;
    LoadingScreenManager loadingScreen;

    [Header("Buttons")]
    public Button resumeButton;
    public Button attributesButton;
    public Button settingsButton;
    public Button quickRestartButton;
    public Button quitButton;

    [Header("Panels")]
    public GameObject pauseMenuPanel;
    public PlayerAttributesPanelManager playerAttributesPanel;
    public GameObject settingsUI;
    [SerializeField]
    GameObject focusOnPause;

    bool isPauseActive = false;
    private bool _isPauseMenuActive;

    [Header("Sounds")]
    public AudioClip resumeSfx;
	public AudioClip pauseSfx;
	AudioSource audioSource;

    [Header("Delay")]
    public float pauseDelay = .5f;
    //float delayTime;

    private void Awake()
    {
        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        audioSource = GetComponent<AudioSource>();
        loadingScreen = FindObjectOfType<LoadingScreenManager>();
        //delayTime = pauseDelay;

        //PlayerEntryManager.ClearEntries();
    }

	private void OnEnable()
	{
		
	}

    private void Update()
    {
        //if (delayTime > 0) delayTime -= Time.unscaledDeltaTime;
    }

    public float ReturnPauseDelay()
    {
        return 0;
        //return delayTime;
    }

    public void StartFocus()
    {
        EventSystem.current.SetSelectedGameObject(focusOnPause);
    }

    public void OnBack()
	{
        //if (delayTime > 0) return;

        //StatsController stats = FindObjectOfType<StatsController>();
        //      EvolvePocketMenu evolvePocketMenu = FindObjectOfType<EvolvePocketMenu>();

        bool stayPaused = DisableLobbyMenus();

        if (playerAttributesPanel.gameObject.activeSelf)
        {
            playerAttributesPanel.gameObject.SetActive(false);
            pauseMenuPanel.SetActive(true);
            EventSystem.current.SetSelectedGameObject(attributesButton.gameObject);
        }
        else if (settingsUI.activeSelf)
        {
            settingsUI.SetActive(false);
            pauseMenuPanel.SetActive(true);
            EventSystem.current.SetSelectedGameObject(settingsButton.gameObject);
        }
        //else if (evolvePocketMenu && evolvePocketMenu.IsEnabled())
        //{
        //    evolvePocketMenu.ClosePanel();
        //}
        //else if (stats)
        //{
        //    if (stats.GetStatsEnabled()) 
        //    {
        //        stats.EnableStats(false);
        //    } 
        //}
        else if (IsGamePaused() && !stayPaused)
        {
            Resume(pausedPlayer);
        }
    }

 //   public void OnAttributeHorizontalNavigate(InputAction.CallbackContext ctx, Player player)
	//{
 //       playerAttributesPanel.HorizontalNavigate(ctx, player);
 //   }

 //   public void OnAttributeVerticalNavigate(InputAction.CallbackContext ctx, Player player)
	//{
 //       playerAttributesPanel.VerticalNavigate(ctx, player);
 //   }

    public void OnAttributeNavigate(InputAction.CallbackContext ctx, Player player)
    {
        playerAttributesPanel.Navigate(ctx, player);
    }

    bool DisableLobbyMenus()
	{
        bool result = false;

        EvolvePocketMenu evolvePocketMenu = FindObjectOfType<EvolvePocketMenu>();
        if (evolvePocketMenu)
        {
            if (evolvePocketMenu.IsEnabled())
            {
                evolvePocketMenu.Close();
                result = true;
            }
        }

        PocketDnaTradePanel pocketDnaTradePanel = FindObjectOfType<PocketDnaTradePanel>();
        if (pocketDnaTradePanel)
		{
            if (pocketDnaTradePanel.IsEnabled())
			{
                pocketDnaTradePanel.Close();
                result = true;
            }
        }

        StatsController stats = FindObjectOfType<StatsController>();
        if (stats)
        {
            if (stats.GetStatsEnabled())
            {
                stats.EnableStats(false);
                result = false;
            }
        }

        OpenBioengineerPanel openBioengineerPanel = FindObjectOfType<OpenBioengineerPanel>();
        if (openBioengineerPanel)
        {
            if (openBioengineerPanel.IsEnabled())
            {
                openBioengineerPanel.EnableOptions(false);
                result = false;
            }
        }

        return result;
    }

    public void OnPauseInput(InputAction.CallbackContext ctx, Player player)
    {
        if (!ScreenManager.Instance.IsGameplay() || GameplayManager.Instance.IsCutsceneInProgress || GlobalData.Instance.canPause == false || player.GetInputController().GetPlayerEntryPanel().selectedSpot != null)
        {
            return;
        }

        if (IsGamePaused())
        {
            Resume(player);
        } 
        else
        {
            if (player)
            {
                if (pauseMenuUI.GetComponentInChildren<StatsDebug>()) pauseMenuUI.GetComponentInChildren<StatsDebug>().player = player;
            }
            if (GameplayManager.Instance.IsTutorial())
			{
                Pause(GameplayManager.Instance.GetPlayers(false)[0]);
			}
			else
			{
                Pause(player);
			}
        }
    }

    public void NextAttributeTab()
    {
        if (playerAttributesPanel.isActiveAndEnabled) playerAttributesPanel.NextTab();
    }
    public void PreviousAttributeTab()
    {
        if (playerAttributesPanel.isActiveAndEnabled) playerAttributesPanel.PreviousTab();
    }

    public void SimplePause()
	{
        isPauseActive = true;
        Time.timeScale = 0;

        if (Gamepad.current != null)
        {
            FindObjectOfType<RumbleManager>().StopRumble();
        }
		audioSource.PlayOneShot(pauseSfx);
    }

    public void SimpleResume()
	{
        isPauseActive = false;
        Time.timeScale = 1;
		audioSource.PlayOneShot(resumeSfx);
    }

    Player pausedPlayer;
    public void Pause(Player player)
    {
        pausedPlayer = player;

		foreach (PocketRadialMenu pocketMenu in FindObjectsOfType<PocketRadialMenu>())
		{
			if (pocketMenu != null && pocketMenu.isOpen)
			{
				//pocketMenu.Close();
				return;
			}
		}

		if (Gamepad.current != null)
        {
            EventSystem.current.SetSelectedGameObject(focusOnPause);
            FindObjectOfType<RumbleManager>().StopRumble();
        }

		audioSource.PlayOneShot(pauseSfx);
        isPauseActive = true;
        _isPauseMenuActive = true;
        Time.timeScale = 0f;
        StartUI(player.GetInputController().GetInput().playerIndex > 0);
    }

    public void Resume(Player player)
    {
        if (!IsGamePaused()) return;

        if (player) 
            if (player != pausedPlayer) 
                return;

        PocketRadialMenu pocketMenu = GameObject.FindObjectOfType<PocketRadialMenu>();

        if (pocketMenu != null && pocketMenu.isOpen)
            return;

        pauseMenuPanel.SetActive(true);
        playerAttributesPanel.gameObject.SetActive(false);
        settingsUI.SetActive(false);

        audioSource.PlayOneShot(resumeSfx);
        CloseUI();
        isPauseActive = false;
        _isPauseMenuActive = false;
        Time.timeScale = 1f;

        if (player)
		{
            player.GetInputController().SetMapInput("Gameplay");
		}
		else
		{
            foreach (PlayerInputController p in FindObjectsOfType<PlayerInputController>())
            {
                p.SetMapInput("Gameplay");
            }
        }
    }

    public void ButtonResume()
	{
        Resume(null);
    }

    public void QuickRestart()
    {
        Resume(null);

        GameObject HUD = GameObject.Find("HUD");
        print("<color=red>" + HUD.name + " | " + HUD.transform.GetInstanceID().ToString() + "</color>");
        Destroy(HUD);
        loadingScreen.OpenLoading();
        StartCoroutine(QuickRestartDelay());
    }

    IEnumerator QuickRestartDelay()
    {
        yield return new WaitUntil(() => loadingScreen.GetCanLoad());

        
        MainMenu.StartGame(Difficulty.Easy);

        foreach (var item in GameplayManager.Instance.players)
        {
            Destroy(item.gameObject);
        }
    }

    public void GoToMainMenu()
    {
        Resume(null);
        GlobalData.Instance.GetTotalTime();

        Cursor.visible = true;
        ScreenManager.Instance.ChangeScreen(Screens.MainMenu, false);
    }

    void StartUI(bool useSimpleUi)
    {
        if (useSimpleUi) simplePauseMenuUI.SetActive(true);
        else pauseMenuUI.SetActive(true);
		Cursor.visible = true;
    }

    void CloseUI()
    {
        SettingsMenu settings = GetComponent<SettingsMenu>();

        if (settings.settingsMenu.activeSelf)
            settings.OpenCloseSettings();

        pauseMenuUI.SetActive(false);
        simplePauseMenuUI.SetActive(false);
        isPauseActive = false;
        Cursor.visible = false;
    }

    public bool IsGamePaused()
    {
        return isPauseActive;
    }

	public bool IsPauseMenuActive()
    {
        return _isPauseMenuActive;
    }
}
