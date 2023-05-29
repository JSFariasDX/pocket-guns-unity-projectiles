using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public enum Difficulty
{
    Easy = 1,
    Normal = 2,
    Hard = 3,
    Insane = 4,
}

public enum MainMenuTabs
{
    Play = 0,
    Chars = 1,
    Pockets = 2,
    Guns = 3,
}

public class MainMenu : MonoBehaviour
{
    PlayerControls controls;

    TutorialManager tutorial;

    public bool controlTabs = false;

    int currentTab = 0;
    [SerializeField] GameObject[] tabs;
    [SerializeField] GameObject[] contents;

    public CharacterSelectionController characterController;
    public PocketSelectionController pocketController;
    public EventSystem eventSystem;

    [Header("UI")]
    public RectTransform coinsCounter;
    [SerializeField] int choosedTab;
    public GameObject practiceScreen;
    public GameObject menuScreen;
    public TMPro.TextMeshProUGUI returnText;

    [Header("Buttons")]
    public GameObject yesButton;
    public GameObject playButton;

    public BackgroundController background;

    [SerializeField]
    OST baseOST;

    bool tutorialActive = false;

    Color32 unselectedTabColor = new Color32(0, 217, 255, 255);
    Color32 selectedTabColor = new Color32(255, 255, 0, 255);

    

    public TMPro.TextMeshProUGUI versionText;

    [Header("Audios")]
    public AudioSource switchTabSound;

    private void Awake()
    {
        SelectTab(MainMenuTabs.Play);

        controls = new PlayerControls();

        controls.UI.NextTab.performed += HandleNextTab;
        controls.UI.PreviousTab.performed += HandlePreviousTab;
        controls.UI.Cancel.performed += _ => AskPractice(false);

        //controls.UI.Any.performed += _ => SetCanLoad();

        SaveDataHandler.LoadPlayerData();
    }

    //private void OnEnable()
    //{
    //    controls.UI.Enable();
    //}

    //private void OnDisable()
    //{
    //    controls.UI.NextTab.performed -= HandleNextTab;
    //    controls.UI.PreviousTab.performed -= HandlePreviousTab;
    //    controls.UI.Disable();
    //}

    private void Start()
    {
        tutorial = FindObjectOfType<TutorialManager>();

        AskPractice(false);

        versionText.text = "version " + Application.version;

        Cursor.visible = true;

        //if (baseOST != ThemeMusicManager.Instance.GetCurrentOST())
        //{
        //    ThemeMusicManager.Instance.ResetClips();
        //    ThemeMusicManager.Instance.SetTheme(baseOST);
        //    ThemeMusicManager.Instance.StartTheme();
        //}

        if (!MusicManager.Instance.HasClip())
        {
            MusicManager.Instance.StopAll();
            MusicManager.Instance.PlayMenuTheme(baseOST.baseTrack);
        }

        EndRun.Clear();
    }

    private void Update()
    {
        string buttonName = InputControlPath.ToHumanReadableString(controls.FindAction("Cancel").bindings[tutorial.isGamepad ? 0 : 1].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);

        if (tutorial.isGamepad)
        {
            for (int i = 0; i < tutorial.gamepadButtons.Count; i++)
            {
                if (tutorial.gamepadButtons[i].name == buttonName)
                {
                    switch (tutorial.type)
                    {
                        case ControllerType.DualShock:
                            returnText.spriteAsset = tutorial.DSSprites;
                            returnText.text = "Press <sprite name=\"" + tutorial.gamepadButtons[i].PlayStationIcon.name + "\"> to return";
                            break;
                        case ControllerType.Xbox:
                            returnText.spriteAsset = tutorial.XboxSprites;
                            returnText.text = "Press <sprite name=\"" + tutorial.gamepadButtons[i].XboxIcon.name + "\"> to return";
                            break;
                        case ControllerType.Switch:
                            returnText.spriteAsset = tutorial.SwitchSprites;
                            returnText.text = "Press <sprite name=\"" + tutorial.gamepadButtons[i].SwitchIcon.name + "\"> to return";
                            break;
                        case ControllerType.Keyboard:
                            returnText.spriteAsset = tutorial.DSSprites;
                            returnText.text = "Press <sprite name=\"" + tutorial.gamepadButtons[i].PlayStationIcon.name + "\"> to return";
                            break;
                        default:
                            break;
                    }
                }
                else
                    continue;
            }
        }
        else
        {
            for (int i = 0; i < tutorial.keyboard.Count; i++)
            {
                if (tutorial.keyboard[i].name == buttonName)
                {
                    returnText.spriteAsset = tutorial.KeyboardSprites;
                    returnText.text = "Press <sprite name=\"" + tutorial.keyboard[i].icon.name + "\"> to return";
                    break;
                }
                else
                    continue;
            }
        }
    }


    public void AskPractice(bool active)
    {
        if (active)
        {
            practiceScreen.SetActive(true);
            menuScreen.SetActive(false);

            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(yesButton);
        }
        else
        {
            if (practiceScreen.activeSelf)
            {
                practiceScreen.SetActive(false);
                menuScreen.SetActive(true);

                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(playButton);
            }
        }

        //background.SetMenu(active);

        tutorialActive = active;
    }

    public void PlayEasy()
    {
        StartGame(Difficulty.Easy);
    }

    public static void GoToLobby()
    {
        ScreenManager.Instance.ChangeScreen(Screens.Lobby);
    }

    public static void StartGame(Difficulty difficulty)
    {
        GlobalData.Instance.Reset();
        ScreenManager.Instance.ChangeScreen(Screens.FirstRoom);

        //print(ScreenManager.Instance.loadingProgress);
    }

    public static void StartPocketMorBattle()
	{
        GlobalData.Instance.Reset();
        ScreenManager.Instance.ChangeScreen(Screens.PocketMor);
    }

    public void StartTutorial()
    {
        GlobalData.Instance.Reset();
        ScreenManager.Instance.ChangeScreen(Screens.QueenRoom, false);
    }

    void SetCanLoad()
    {
        print("I'm pressing!");

        //if(ScreenManager.Instance.loadingProgress >= 1)
            //ScreenManager.Instance.SetCanLoad(true);
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void SelectTab(MainMenuTabs t) {
        return;
        switchTabSound.Play();

        choosedTab = (int)t;
        foreach (GameObject tab in tabs) {
            SetTabColor(tab, unselectedTabColor);
        }
        foreach (GameObject tabContent in contents)
        {
            tabContent.SetActive(false);
            //tabContent.GetComponent<CanvasGroup>().alpha = 0;
            //tabContent.GetComponent<CanvasGroup>().blocksRaycasts = false;
            //if(choosedTab != 0)
            //    tabContent.transform.GetChild(1).gameObject.SetActive(false);
        }
        SetTabColor(tabs[choosedTab], selectedTabColor);

        contents[choosedTab].SetActive(true);
        //contents[choosedTab].GetComponent<CanvasGroup>().alpha = 1;
        //contents[choosedTab].GetComponent<CanvasGroup>().blocksRaycasts = true;
        //if (choosedTab != 0)
        //    contents[choosedTab].transform.GetChild(1).gameObject.SetActive(true);
        currentTab = choosedTab;
 
        if (t == MainMenuTabs.Chars)
		{
            eventSystem.SetSelectedGameObject(characterController.GetSelectedButton().gameObject);
        }
        else if (t == MainMenuTabs.Pockets)
		{
            eventSystem.SetSelectedGameObject(pocketController.GetSelectedButton().gameObject);
        }
    }

    void SetTabColor(GameObject t, Color32 c)
    {
        t.GetComponentInChildren<TMPro.TextMeshProUGUI>().color = c;
    }

    void HandleNextTab(InputAction.CallbackContext ctx)
    {
        if (!controlTabs) return;

        int nextTab = currentTab + 1;
        if (nextTab > tabs.Length - 1)
        {
            SelectTab(MainMenuTabs.Play);
        } else
        {
            MainMenuTabs t = (MainMenuTabs)nextTab;
            SelectTab(t);
        }
    }

    void HandlePreviousTab(InputAction.CallbackContext ctx)
    {
        if (!controlTabs) return;

        int nextTab = currentTab - 1;
        if (nextTab < 0)
        {
            SelectTab(MainMenuTabs.Guns);
        }
        else
        {
            MainMenuTabs t = (MainMenuTabs)nextTab;
            SelectTab(t);
        }
    }

    public void SwitchScene(string sceneName)
	{
        StartCoroutine(LoadSwitchScene(sceneName));
	}

    IEnumerator LoadSwitchScene(string name)
    {
        yield return new WaitForSecondsRealtime(.5f);

        SceneManager.LoadScene(name);
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
