using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.Video;

public class EndRun : MonoBehaviour
{
    public GameObject restartBtn;

    // Win part
    [SerializeField, TextArea] string gameOverText;
    [SerializeField] GameObject[] winObjects;

    // Game over part
    [SerializeField] GameObject[] gameOverObjects;

    // General
    [SerializeField] TextMeshProUGUI titleText;
    [SerializeField] TextMeshProUGUI dungeonText;
    [SerializeField] TextMeshProUGUI timeText;
    [SerializeField] Transform defeatedEnemiesParent;
	[SerializeField] DefeatedEnemiesGui defeatedEnemiesPrefab;
	[SerializeField] TextMeshProUGUI collectedCoinsText;
    [SerializeField] TextMeshProUGUI scoreText;

    [Header("Sounds")]
    public AudioClip wonSound;
    public AudioClip gameOverSound;
    public AudioClip endTheme;
    private AudioSource audioSource;

    [Header("Delay")]
    public float waitTime = 1.5f;
    float timer;
    bool loadNextScene = false;
    bool loaded = false;

    [Header("UI")]
    public CanvasGroup blackFade;
    public GameObject buttons;

    void SetupScreen()
	{
        dungeonText.text = GameplayManager.Instance.GetDungeonConfig().type.ToString();
        timeText.text = GlobalData.Instance.GetTotalTime();

		List<Player> players = new List<Player>(GameplayManager.Instance.GetPlayers(false));
		print(players.Count);
		players.Reverse();
		RectTransform rectTransform = defeatedEnemiesParent.transform.GetComponent<RectTransform>();
        float stepHeight = rectTransform.sizeDelta.y;
		foreach (Player player in players)
		{
			DefeatedEnemiesGui gui = Instantiate(defeatedEnemiesPrefab, defeatedEnemiesParent);
			gui.Setup("- " + player.characterName + " & " + player.GetCurrentPocket().pocketName, player.GetDefeatedEnemies());
			gui.transform.SetSiblingIndex(1);
			rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, rectTransform.sizeDelta.y + stepHeight);
		}

		collectedCoinsText.text = GameplayManager.Instance.TotalCoins.ToString("D7");
        scoreText.text = GlobalData.Instance.score.ToString("D7");
    }

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        SetupScreen();
        Cursor.visible = true;

        
        

        if (IsGameOver())
        {
            audioSource.PlayOneShot(gameOverSound);
            ActiveObjects(winObjects, false);
            ActiveObjects(gameOverObjects, true);

            int defeats = PlayerPrefs.GetInt("DEFEAT", 0);
            defeats++;
            PlayerPrefs.SetInt("DEFEAT", defeats);
        }
        else
        {
            audioSource.PlayOneShot(wonSound);
            ActiveObjects(winObjects, true);
            ActiveObjects(gameOverObjects, false);
        }

        EventSystem.current.SetSelectedGameObject(restartBtn);
        //ThemeMusicManager.Instance.FinalizeOST();
        MusicManager.Instance.StopAll();
        MusicManager.Instance.PlaySFX(endTheme);

        Clear();
    }

    bool IsGameOver()
	{
        return PlayerPrefs.GetString("GameResult").Equals("Game Over");
	}

    void ActiveObjects(GameObject[] list, bool active)
	{
        foreach(GameObject obj in list)
		{
            obj.SetActive(active);
		}
	}

    public static void Clear()
	{
        SelectionManager.Instance.Reset();

        PlayerEntryPanel[] panels;
        panels = FindObjectsOfType<PlayerEntryPanel>(true);


        for (int i = 0; i < panels.Length; i++)
        {
            Destroy(panels[i]);
        }

        if(FindObjectOfType<PauseManager>())
            Destroy(FindObjectOfType<PauseManager>().gameObject);

        foreach (PlayerEntryPanel entryPanel in FindObjectsOfType<PlayerEntryPanel>(true))
        {
            Destroy(entryPanel.gameObject);
        }

        // Players
        foreach (Player player in FindObjectsOfType<Player>(true))
		{
            Destroy(player.gameObject);
		}

        // Pocket menu
        foreach(PocketRadialMenu pocketMenu in FindObjectsOfType<PocketRadialMenu>(true))
        {
            Destroy(pocketMenu.transform.parent.gameObject);
        }

        // HUD
        Destroy(GameObject.FindGameObjectWithTag("HUDCanvas"));
    }

    private void Update()
    {
        if (timer < waitTime) timer += Time.unscaledDeltaTime;

        if(loadNextScene && timer >= waitTime)
        {
            if (blackFade.alpha >= 1 && !loaded)
            {
                ScreenManager.Instance.ChangeScreen(Screens.PlayerEntryMenu);
                loaded = true;
            }
        }

        if (loadNextScene)
        {
            if(blackFade.alpha < 1)
                blackFade.alpha += Time.unscaledDeltaTime;
        }

    }

    GameObject[] FindOrdened(string tag)
    {
        GameObject[] foundObs = GameObject.FindGameObjectsWithTag(tag);
        System.Array.Sort(foundObs, CompareObNames);
        return foundObs;
    }

    int CompareObNames(GameObject x, GameObject y)
    {
        return x.name.CompareTo(y.name);
    }

    public void Restart()
    {
        if (loadNextScene) return;

        GlobalData.Instance.Reset();
        GameObject pause = GameObject.FindGameObjectWithTag("PauseManager");
        Destroy(pause);

        //SceneManager.LoadScene("FirstDungeon");
        //ScreenManager.Instance.ChangeScreen(Screens.FirstRoom);

        buttons.SetActive(false);
        loadNextScene = true;
    }

    public void GoMainMenu()
    {
        //SceneManager.LoadScene("MainMenu");
        ScreenManager.Instance.ChangeScreen(Screens.MainMenu);
    }
}
