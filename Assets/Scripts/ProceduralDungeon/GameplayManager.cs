using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using System.Linq;

public class GameplayManager : MonoBehaviour
{
    public static GameplayManager Instance;

    [SerializeField]
    List<DungeonConfig> dungeons;
    Queue<DungeonConfig> dungeonOrder = new Queue<DungeonConfig>();
    [SerializeField] private bool randomizeDungeons = true;
    [SerializeField] private int fixedStartDungeonCount = 1;
    [SerializeField] private int randomDungeonCount = 3;
    [SerializeField] private int fixedEndDungeonCount = 1;
    public int currentDungeonOrder = 0;
    public int PlusMinMaxRooms;
    public DungeonType currentDungeonType;
    public List<Player> players = new List<Player>();
    public List<GameObject> clearOnDungeonEnd = new List<GameObject>();

    [SerializeField] private CutsceneLoadingScreen cutsceneLoadingScreen;
    LoadingScreenManager loadingScreen;
    public bool IsCutsceneInProgress { get; private set; }

    public int CurrentCoins => _currentCoins;
    public int TotalCoins => _totalCoins;
    private int _currentCoins;
    private int _totalCoins;

    [Header("Keys")]
    public int keysNeeded;
    public List<BossKey> _keys = new List<BossKey>();
    public int TotalKeys => _keys.Count;

    public List<Image> keyIcons = new List<Image>();
    [SerializeField] AudioClip allKeysCollectedClip;

    bool isStarted = false;

    public bool isLoadingNextDungeon = false;
    DungeonConfig currentDungeonConfig;

    AudioSource _audioSource;

    private void OnValidate()
    {
        if (fixedStartDungeonCount + randomDungeonCount + fixedEndDungeonCount > dungeons.Count)
            Debug.LogWarning($"Ordering count config has a larger total than the entries on dungeons list.");
    }

    private void Update()
    {
        if(players.Count > 1)
        {
            AudioSource[] allSources = GameObject.FindObjectsOfType<AudioSource>();

            for (int i = 0; i < allSources.Length; i++)
            {
                if (allSources[i].spatialBlend > 0)
                    allSources[i].spatialBlend = 0;
            }
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();

        loadingScreen = FindObjectOfType<LoadingScreenManager>();

        SortDungeonsOrder();
        PlayerPrefs.SetInt("CurrentDungeon", 0);
        PlusMinMaxRooms = 0;
        _currentCoins = 0;
        _totalCoins = 0;
        StartNextDungeon();

    }

    private void SortDungeonsOrder()
    {
        if (!randomizeDungeons)
        {
            foreach(DungeonConfig config in dungeons)
            {
                dungeonOrder.Enqueue(config);
            }

            return;
        }

        for (int i = 0; i < fixedStartDungeonCount; i++)
        {
            dungeonOrder.Enqueue(dungeons[0]);
            dungeons.RemoveAt(0);
        }

        for (var i = 0; i < randomDungeonCount; i++)
        {
            int sorted = UnityEngine.Random.Range(0, dungeons.Count - fixedEndDungeonCount);
            dungeonOrder.Enqueue(dungeons[sorted]);
            dungeons.RemoveAt(sorted);
        }
        
        for (int i = 0; i < fixedEndDungeonCount; i++)
        {
            dungeonOrder.Enqueue(dungeons[0]);
            dungeons.RemoveAt(0);
        }
    }

    public void StartNextDungeon()
    {
        ActiveLoadingScreen(true);
        StartCoroutine(InvokeNext());
    }

    private IEnumerator InvokeNext()
    {
        yield return new WaitForSecondsRealtime(1 / loadingScreen.fadeSpeed);
        Next();

        yield return new WaitUntil(() => !RoomByRoomGenerator.Instance.isGenerating);
        
        IsCutsceneInProgress = false;

        GetComponent<AmbientSoundsController>().SetupAmbientSounds(GetDungeonConfig().ambientSoundPack, true);

        yield return new WaitForSeconds(1f);
        ActiveLoadingScreen(false);

    }

    public bool IsTutorial()
	{
        return SceneManager.GetActiveScene().name.Contains("Tutorial");
	}

    void Next()
    {
        isLoadingNextDungeon = true;
        ClearOnDungeonEnd();
        currentDungeonOrder++;
        GlobalData.Instance.ResetPercentage();
        DungeonConfig config = dungeonOrder.Dequeue();
        currentDungeonConfig = config;
        currentDungeonType = config.type;
        GameObject.FindGameObjectWithTag("DungeonName").GetComponent<TMPro.TextMeshProUGUI>().text = config.type.ToString();

        if (currentDungeonOrder > 1 && currentDungeonOrder < fixedStartDungeonCount + randomDungeonCount + fixedEndDungeonCount)
            PlusMinMaxRooms++;
        else
            PlusMinMaxRooms = 0;

        if (SceneManager.GetActiveScene().name.Contains("Mor"))
        {
            RoomByRoomGenerator.Instance.LoadPocketMorRoom();
        }
        else
        {
            RoomByRoomGenerator.Instance.RunProceduralGeneration(config);
        }

        if (currentDungeonOrder > 1)
        {
            RessAllPockets();
            ResetAllPlayers();
        }
        ConfigureLights(config);

        PlayerPrefs.SetInt("CurrentRoom", 0);
        PlayerPrefs.SetInt("CurrentDungeon", PlayerPrefs.GetInt("CurrentDungeon", 1) + 1);
        isLoadingNextDungeon = false;
    }

    public void ConfigureLights(DungeonConfig config)
    {
        foreach (var player in players)
        {
            player.SetLightIntensity(config.playerLightIntensity);
            player.SetLightRadius(config.playerLightRadius, config.lightInnerRadius);
            player.SetLightColor(Color.white);
        }
    }

    private void RessAllPockets()
    {
        GameObject[] pockets = GameObject.FindGameObjectsWithTag("Pet");
        foreach (var pocket in pockets)
        {
            pocket.GetComponent<Pocket>().Ressurect();
            RechargeSpecial(pocket.GetComponent<Special>());
        }
    }

    private void ResetAllPlayers()
    {
        foreach(Player player in GetPlayers(false))
        {
            player.ResetPlayerAfterDungeon();
        }
    }

    private void RechargeSpecial(Special special)
    {
        special.RestoreUses();
    }

    public void SpawnPlayers(Vector2Int spawnPoint)
	{
        CameraManager camManager = FindObjectOfType<CameraManager>();

        bool isLobby = SceneManager.GetActiveScene().name.Contains("Lobby");

        List<PlayerEntryPanel> entries = new List<PlayerEntryPanel>(FindObjectsOfType<PlayerEntryPanel>());
        for(int i = 0; i < entries.Count; i++)
		{
            PlayerEntryPanel entryPanel = entries[i];
            entryPanel.GetComponent<PlayerInput>().uiInputModule = FindObjectOfType<InputSystemUIInputModule>();
            Player player = SpawnPlayer(entryPanel.GetCurrentPlayer(), spawnPoint + Vector2Int.up * i);
            player.SetPlayerIndex(i);

   //         if (isLobby)
			//{
   //             UnlockedCharacters saveSystem = FindObjectOfType<UnlockedCharacters>();
   //             string firstKey = saveSystem.unlockedPockets.First().Key;
   //             entryPanel.SetCurrentPocketIndex(int.Parse(firstKey.Substring(0, 2)));
   //             entryPanel.SetCurrentPocketLevel(int.Parse(firstKey[2].ToString()));
			//}

            entryPanel.SetPlayer(player);
            player.entryPanel = entryPanel;

            if(!isLobby)
                player.StartPocket(entryPanel.GetCurrentPocket().pocket, entryPanel.GetCurrentPocketLevel());

            //player.SetCamTarget(camManager.AddTarget(player.transform));
            entryPanel.transform.SetParent(null);
        }

        Destroy(GameObject.Find("EntryCanvas"));

        return;
	}

    public void SetPlayerPositions(Vector3 pos)
	{
        for(int i = 0; i < players.Count; i++)
		{
            players[i].SetPosition(pos + Vector3.up * i);
        }
	}

    public void SetAllPlayersSpriteRenderers(bool set)
	{
        foreach(Player player in players)
		{
            player.SetAllSpriteRenderers(set);
		}
	}

    public List<Player> GetPlayers(bool onlyAlivePlayers)
	{
        List<Player> list = new List<Player>(players);
        if (onlyAlivePlayers) list.RemoveAll(p => p.GetIsDead());

        return list;
	}

    public Player SpawnPlayer(Player prefab, Vector2Int playerSpawnPoint)
    {
        Player spawnedPlayer = Instantiate(prefab, new Vector3(playerSpawnPoint.x, playerSpawnPoint.y, 0), Quaternion.identity);
        spawnedPlayer.SetPosition(new Vector3(playerSpawnPoint.x, playerSpawnPoint.y, 0));
        players.Add(spawnedPlayer);
        return spawnedPlayer;
    }

    public void ActiveLoadingScreen(bool active)
	{
        // if (!loadingScreen)
		// {
        //     loadingScreen = FindObjectOfType<MinimapManager>(true).loadingScreen;
        // }
        // loadingScreen.SetActive(active);

        //cutsceneLoadingScreen.gameObject.SetActive(active);

        if (active)
        {
            //cutsceneLoadingScreen.StartCutscene(() => { Next(); });
            if(!loadingScreen.GetIsLoading())
            loadingScreen.OpenLoading();
            IsCutsceneInProgress = true;
        }
        else
        {
            loadingScreen.CloseLoading();
        }
    }

    public void ClearOnDungeonEnd()
    {
        foreach (GameObject item in clearOnDungeonEnd)
        {
            Destroy(item.gameObject);
        }
        clearOnDungeonEnd.Clear();
    }

    public void AddCoins(int amount)
    {
        _totalCoins += amount;
        _currentCoins += amount;

        UpdateNpcPrices();
    }

    public void RemoveCoins(int amount)
    {
        _currentCoins -= amount;

        if (_currentCoins < 0)
            _currentCoins = 0;

        UpdateNpcPrices();
    }

    public void AddKeys(BossKey key)
    {
        _keys.Add(key);

        if (_keys.Count >= keysNeeded)
		{
            _audioSource.PlayOneShot(allKeysCollectedClip);
		}
    }

    public void UseKeys(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            _keys.RemoveAt(0);
        }
    }

    void UpdateNpcPrices()
    {
        var npcsList = FindObjectsOfType<NPC>();
        foreach (var npc in npcsList)
        {
            npc.CheckItemsPrice(CurrentCoins);
        }
    }

    public DungeonConfig GetDungeonConfig()
	{
        return currentDungeonConfig;
	}

    public GameObject GetRandomEgg()
    {
        List<PocketSelection> options = new List<PocketSelection>(GetPlayers(false)[0].GetInputController().GetPlayerEntryPanel().pocketOptions);

        int sortedEgg = UnityEngine.Random.Range(0, options.Count);
        return options[sortedEgg].pocket.gameObject;
    }
}
