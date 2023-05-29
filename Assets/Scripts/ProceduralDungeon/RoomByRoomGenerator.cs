using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

#region Enums
public enum RoomLimits
{
    Small = 5,
    Medium = 10,
    Large = 15,
};

public enum RoomSizesMinPositions
{
    Small = 30,
    Medium = 50,
    Big = 100,
}

public enum RelativePosition
{
    Up = 1,
    Right = 2,
    Left = 3,
    Down = 4,
}
#endregion

public class RoomByRoomGenerator : SRWDungeonGenerator
{
    public static RoomByRoomGenerator Instance;

    public bool isGenerating = false;

    DungeonConfig config;
    Vector2Int playerSpawnPoint;
    int roomsCount;
    public List<Room> rooms = new List<Room>();
    public List<Gate> gates = new List<Gate>();
    public Gate BossGate { get; private set; }

    List<Enemy> instantiatedEnemies = new List<Enemy>();
    List<EnemiesEncounter> possibleEncountersData = new List<EnemiesEncounter>();
    [Header("Minimap")]
    public SpriteRenderer minimapIconPrefab;
    public SpriteRenderer sinisterMerchantIconPrefab;
    public SpriteRenderer merchantIconPrefab;
    public SpriteRenderer bossIconPrefab;
    public SpriteRenderer finishedRoomIconPrefab;
    public SpriteRenderer rewardIconPrefab;
    public SpriteRenderer startRoomIconPrefab;
    public Color minimapWallColor;
    public Color minimapFloorColor;

    [Header("VFX")]
    public SpawnParticle enemySpawnParticlePrefab;
    public SpawnParticle rewardSpawnParticle;

    List<GameObject> lights = new List<GameObject>();
    List<RectInt> roomRects = new List<RectInt>();
    List<RectInt> lightRects = new List<RectInt>();
    public List<RectInt> obstacleRects = new List<RectInt>();
    List<RectInt> layer1Rects = new List<RectInt>();
    List<RectInt> layer2Rects = new List<RectInt>();
    List<RectInt> layer3Rects = new List<RectInt>();
    List<CreateRandomWalkDataSO> roomSizesParameters = new List<CreateRandomWalkDataSO>();
    List<Vector2Int> roomCenters = new List<Vector2Int>();
    List<GameObject> minimapObjects = new List<GameObject>();
    List<RectInt> externalDecorationsRects = new List<RectInt>();
    List<RectInt> gatePreventionRects = new List<RectInt>();
    List<GameObject> chests = new List<GameObject>();

    Dictionary<Vector2Int, HashSet<Vector2Int>> roomsDictionary = new Dictionary<Vector2Int, HashSet<Vector2Int>>();
    Dictionary<Vector2Int, GameObject> placedObstaclesDictionary = new Dictionary<Vector2Int, GameObject>();
    Dictionary<Vector2Int, GameObject> placedDoorsDictionary = new Dictionary<Vector2Int, GameObject>();
    Dictionary<Vector2Int, GameObject> placedNPCsDictionary = new Dictionary<Vector2Int, GameObject>();
    Dictionary<Vector2Int, GameObject> placedLayer1Dictionary = new Dictionary<Vector2Int, GameObject>();
    Dictionary<Vector2Int, GameObject> placedLayer2Dictionary = new Dictionary<Vector2Int, GameObject>();
    Dictionary<Vector2Int, GameObject> placedLayer3Dictionary = new Dictionary<Vector2Int, GameObject>();
    Dictionary<Vector2Int, GameObject> placedCornerDictionary = new Dictionary<Vector2Int, GameObject>();
    Dictionary<Vector2Int, GameObject> placedExternalDecorationDictionary = new Dictionary<Vector2Int, GameObject>();

    HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();
    HashSet<Vector2Int> wallPositions = new HashSet<Vector2Int>();
    HashSet<Vector2Int> corridorPositions = new HashSet<Vector2Int>();

    Player player;
    List<Player> players = new List<Player>();

    Vector2Int bossRoomCenter;

    public NavMeshSurface navMeshSurface;

    RectInt aroundFloor;

    private void Awake()
    {
        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        else if (Instance != this)
        {
            SafeDestroy(gameObject);
        }
    }

    public void LoadPocketMorRoom()
	{
        isGenerating = true;

        SpawnPlayerOnPocketMorRoom();
        if (Application.isPlaying) StartCoroutine(PlayMusic());

        isGenerating = false;
    }

    public override void RunProceduralGeneration(DungeonConfig dungeonConfig)
    {
        isGenerating = true;

        GameplayManager.Instance.keysNeeded = dungeonConfig.keysNeeded;

        Debug.Log($"Starting generation of {dungeonConfig.name}");
        FindObjectOfType<ChallengeManager>().Setup(dungeonConfig.enemiesEncountersFolder);
        ResetDungeon();
        config = dungeonConfig;
        tilemapVisualizer.SetAsset(config.paintAsset);
        Debug.Log("Setup color filter");
        SpriteRenderer colorFilterSR = GameObject.FindGameObjectWithTag("ColorFilter").GetComponent<SpriteRenderer>();
        colorFilterSR.color = config.filterColor;
        colorFilterSR.sortingLayerName = config.filterSortingLayer;
        Debug.Log("Setup background");
        SpriteRenderer backgroundSpriteRenderer = GameObject.FindGameObjectWithTag("Background").GetComponent<SpriteRenderer>();
        backgroundSpriteRenderer.sprite = config.background;
        if (config.BackgroundMaterial != null) backgroundSpriteRenderer.material = config.BackgroundMaterial;
        roomSizesParameters.Add(smallRoomParams);
        roomSizesParameters.Add(mediumRoomParams);
        roomSizesParameters.Add(bigRoomParams);
        CreateRooms();
        if (dungeonConfig.TilesetMaterial != null) tilemapVisualizer.SetTilemapRendererMaterial(dungeonConfig.TilesetMaterial);
        if (config.spawnLights) SetupAmbientLights();
        PlaceCornerObstacles();
        CreateMinimapRoomIcons();
                
        Debug.Log("Applying dungeon context to objects...");
        DungeonContext[] context = FindObjectsOfType(typeof(DungeonContext)) as DungeonContext[];
        ResetShadows();
        foreach (var item in context)
        {
            item.SetContext(config.type);
        }
        if (config.externalDecorations.Count > 0)
        {
            InstantiateExternalDecorations();
        } else
        {
            Debug.Log("Dungeon doesnt have external decorations.");
        }

        FindObjectOfType<LightManager>().Setup(config);

        FindObjectOfType<NavMeshSurface>().BuildNavMeshAsync();

        SpawnPlayer();
        SpawnKeyChests();

        if (Application.isPlaying)
        {
            StartCoroutine(PlayMusic());
        }

        Debug.Log("Procedural generation is complete");
        isGenerating = false;
    }

    private void ResetShadows()
    {
        //ShadowCaster2DFromComposite sc = tilemapVisualizer.wallTilemap.gameObject.GetComponent<ShadowCaster2DFromComposite>();
        //if (sc)
        //{
        //    Destroy(sc);
        //}
        //tilemapVisualizer.wallTilemap.gameObject.AddComponent(Type.GetType("ShadowCaster2DFromComposite"));
    }

    private IEnumerator PlayMusic()
    {
        Debug.Log("Waiting for cutscene to start music...");
        yield return new WaitUntil(() => !GameplayManager.Instance.IsCutsceneInProgress);

        //if (ThemeMusicManager.Instance)
        //{
        //    Debug.Log("Setup theme");
        //    ThemeMusicManager.Instance.SetTheme(config.baseOST);
        //    bool startBaseDrums = true;
        //    Debug.Log("Starting theme");
        //    ThemeMusicManager.Instance.StartTheme(startBaseDrums);
        //}

        if (MusicManager.Instance)
        {
            Debug.Log("Setup theme");
            //    ThemeMusicManager.Instance.SetTheme(config.baseOST);
            //    bool startBaseDrums = true;
            MusicManager.Instance.SetTheme(GetDungeonConfig());
            Debug.Log("Starting theme");
            //    ThemeMusicManager.Instance.StartTheme(startBaseDrums);
            MusicManager.Instance.StartMainTheme();
        }
    }

    private void InstantiateExternalDecorations()
    {
        Debug.Log("Instantiating external decorations...");
        int required = config.externalDecorationCount;
        int instantiated = 0;
        int maxTries = required * 3;
        int tries = 0;
        Vector2Int startSquare = new Vector2Int(tilemapVisualizer.minX, tilemapVisualizer.minY);
        aroundFloor = RectHelper.DrawRectFromPoint(startSquare, tilemapVisualizer.maxX - tilemapVisualizer.minX, tilemapVisualizer.maxY - tilemapVisualizer.minY);
        HashSet<Vector2Int> externalPositions = new HashSet<Vector2Int>();
        foreach (var position in aroundFloor.allPositionsWithin)
        {
            externalPositions.Add(position);
        }
        externalPositions.ExceptWith(floorPositions);
        while (instantiated < required && tries < maxTries)
        {
            tries++;
            int sortedTree = Random.Range(0, config.externalDecorations.Count - 1);
            PlaceableObject placeable = config.externalDecorations[sortedTree];
            Vector2Int position = RoomCreationHelper.GetRandomPoint(externalPositions);
            RectInt externalRect = RectHelper.DrawRectFromCorner(position, placeable.sizeX, placeable.sizeY);
            bool wallConstraint = !placeable.ignoreWallsOnPlacement && CheckRectCollideWithWalls(externalRect);
            if (!wallConstraint && !CheckRectCollideWithFloor(externalRect) && IsCloseToRoomCenters(position, 15) && !RectHelper.CheckOverlap(externalRect, externalDecorationsRects))
            {
                GameObject decoration = Instantiate(placeable.prefab, new Vector3(position.x, position.y, 0), Quaternion.identity);
                placedExternalDecorationDictionary[position] = decoration;
                externalDecorationsRects.Add(externalRect);
                instantiated++;
            }
        }
        if (instantiated >= required)
        {
            Debug.Log("Max tries reached, exiting...");
        }
    }

    private bool IsCloseToRoomCenters(Vector2Int position, int maxDistance)
    {
        bool isClose = false;
        foreach (var roomCenter in roomCenters)
        {
            // Debug.Log(Vector2Int.Distance(position, roomCenter));
            // Debug.Log(maxDistance);
            if (Vector2Int.Distance(position, roomCenter) <= maxDistance)
            {
                isClose = true;
            }
        }
        return isClose;
    }

    private void PlaceBossRoom()
    {
        Debug.Log("Placing Boss Room...");
        int bossRoomPadding = 5;
        // Adds + 4 during drawing;
        int bossRoomBaseSize = config.bossRoomBaseSize;
        CreateRandomWalkDataSO roomSizeParam = roomSizesParameters[2];

        bool isPlaced = false;
        while (!isPlaced)
        {
            Vector2Int currentPoint = roomCenters[Random.Range(0, roomCenters.Count)];
            int originSize = (int)GetRoomSize(currentPoint);
            int nextPadding = RoomCreationHelper.GetPaddingToNextRoom(originSize, bossRoomPadding);
            Vector2Int roomSpawnPoint = RoomCreationHelper.GetNextRoomCenter(currentPoint, nextPadding, config.BossRoomIgnoreUpRightCorridors);
            var rect = RectHelper.GetNextRoomRect(roomSpawnPoint, bossRoomBaseSize);
            bool isNotFromNPCRoom = !placedNPCsDictionary.ContainsKey(currentPoint);
            if (!RectHelper.CheckOverlap(rect, roomRects) && isNotFromNPCRoom)
            {
                bossRoomCenter = roomSpawnPoint;
                GameObject bossSpawn;

                if (config.BossElevatorPrefab != null)
                {
                    Debug.Log("Placing Boss Elevator...");
                    GenerateRoom(currentPoint, roomSpawnPoint, 2, roomSizeParam, rect, false, false, RoomType.Empty);
                    PlaceBossRoomElevator(roomSpawnPoint);
                    CreateMinimapIcon(bossIconPrefab, bossRoomCenter);
                }
                else
                {
                    GenerateRoom(currentPoint, roomSpawnPoint, 2, roomSizeParam, rect, false, false, RoomType.Boss);
                    bossSpawn = Instantiate(config.BossSpawn);
                    Vector3 spawn = new Vector3(roomSpawnPoint.x, roomSpawnPoint.y, 0);
                    bossSpawn.transform.position = spawn;
                    GameObject portal = Instantiate(config.portalPrefab, bossSpawn.transform);
                    Portal portalInstance = portal.GetComponent<Portal>();
                    portalInstance.name = "Portal";
                    if (config.isLastDungeon)
                    {
                        portalInstance.IsEndRun = config.isLastDungeon;
                    }
                    portal.SetActive(false);
                }

                isPlaced = true;
            }
        }

        foreach (Gate g in gates)
        {
            if (g.room.centerPosition == bossRoomCenter)
            {
                BossGate = g;
                break;
            }
        }
    }

    public void InstantiateEgg()
    {
        GameObject bossSpawn = GameObject.FindGameObjectWithTag("BossSpawn");
        var portal = bossSpawn.transform.Find("Portal").GetComponent<Portal>();
        portal.isEndRun = config.isLastDungeon;
        Vector3 eggSpawn = new Vector3(bossSpawn.transform.position.x, bossSpawn.transform.position.y - 3, 0);
        GameObject eggGO = Instantiate(GameplayManager.Instance.GetRandomEgg(), eggSpawn, Quaternion.identity);
        eggGO.GetComponent<Pocket>().pocketType = PetType.Egg;
    }

    private void PlaceNPCRoom(GameObject npcPrefab)
    {
        int npcRoomPadding = (int)RoomLimits.Medium;
        int currentRoomSizeIndex = 1;
        CreateRandomWalkDataSO roomSizeParam = roomSizesParameters[currentRoomSizeIndex];

        bool isPlaced = false;
        while (!isPlaced)
        {
            Vector2Int currentPoint = roomCenters[Random.Range(0, roomCenters.Count)];
            int originSize = (int)GetRoomSize(currentPoint);
            int nextPadding = RoomCreationHelper.GetPaddingToNextRoom(originSize, npcRoomPadding);
            Vector2Int roomSpawnPoint = RoomCreationHelper.GetNextRoomCenter(currentPoint, nextPadding);
            var rect = RectHelper.GetNextRoomRect(roomSpawnPoint, 6);
            bool isNotFromNPCRoom = !placedNPCsDictionary.ContainsKey(currentPoint);
            if (!RectHelper.CheckOverlap(rect, roomRects) && isNotFromNPCRoom)
            {
                GenerateRoom(currentPoint, roomSpawnPoint, currentRoomSizeIndex, roomSizeParam, rect, false, false, RoomType.NPC);
                Vector3 NPCSpawn = new Vector3(roomSpawnPoint.x, roomSpawnPoint.y + 1, 0);
                GameObject npc = Instantiate(npcPrefab, NPCSpawn, Quaternion.identity);
                placedNPCsDictionary[roomSpawnPoint] = npc;
                isPlaced = true;
            }
        }
    }

    List<GameObject> masks = new List<GameObject>();
    public void PlaceSpriteMasks(HashSet<Vector2Int> positions)
    {
        Debug.Log("Placing spritemasks for minimap...");
        foreach (Vector2Int position in positions)
        {
            GameObject mask = Instantiate(config.spriteMaskPrefab, new Vector3(position.x, position.y, 0), Quaternion.identity);
            mask.GetComponentInChildren<SpriteRenderer>().color = minimapWallColor;
            masks.Add(mask);
        }
    }

    public void PlaceBoss(Room room)
    {
        GameObject bossSpawn = GameObject.FindGameObjectWithTag("BossSpawn");
        GameObject boss;

        if (config.BossElevatorPrefab != null)
            boss = Instantiate(config.bossPrefab, new Vector3(bossSpawn.transform.position.x, bossSpawn.transform.position.y, 0), Quaternion.identity, bossSpawn.transform);
        else
            boss = Instantiate(config.bossPrefab, new Vector3(room.centerPosition.x, room.centerPosition.y, 0), Quaternion.identity, bossSpawn.transform);

        Enemy bossEnemy = boss.GetComponent<Enemy>();
        bossEnemy.currentRoom = room;
        instantiatedEnemies.Add(bossEnemy);
        room.enemies.Add(bossEnemy);
    }

    public void PlaceBossRoomElevator(Vector2Int roomCenter)
    {
        var spawnPosition = new Vector3(roomCenter.x + config.ElevatorOffset.x, roomCenter.y + config.ElevatorOffset.y, 0);
        GameObject bossElevator = Instantiate(config.BossElevatorPrefab, spawnPosition, Quaternion.identity);
    }

    public void AddToInstantiatedEnemies(Enemy enemy)
    {
        instantiatedEnemies.Add(enemy);
    }

    public void SortRoomReward(Room room, out GameObject drop, RewardType rewardType = RewardType.None, bool limitSpawn = true, Transform spawnTransform = null)
    {
        drop = null;
        
        if (limitSpawn && (room.roomType == RoomType.Boss || room.decidedReward)) return;

        GameObject rewardPrefab = DecideRoomReward(rewardType);
        if (rewardPrefab != null)
        {
            int percentage = Random.Range(1, 100);

            if (percentage <= GlobalData.Instance.GetPercentage())
            {
                //Vector3 spawnPos;

                //if (spawnTransform == null)
                //    spawnPos = new Vector3(room.centerPosition.x, room.centerPosition.y, 0);
                //else
                //    spawnPos = spawnTransform.position;

                //LayerMask blockLayer = LayerMask.GetMask("Obstacles", "Holes");
                //bool overlappedWithObstacle = Physics2D.OverlapCircle(spawnPos, .5f, blockLayer);
                //int padding = 1;
                //while (overlappedWithObstacle)
                //{
                //    overlappedWithObstacle = Physics2D.OverlapCircle(spawnPos, .5f, blockLayer);
                //    int newX = room.centerPosition.x + padding;
                //    int newY = room.centerPosition.y + padding;
                //    spawnPos = new Vector3(newX, newY, 0);
                //    padding++;
                //}
                //GameObject reward = Instantiate(rewardPrefab, spawnPos, Quaternion.identity);
                //minimapObjects.Add(reward);
                //SpawnParticle spawnParticle = Instantiate(config.rewardSpawnParticle, spawnPos, Quaternion.identity);
                //spawnParticle.SetupRewardSpawn(reward);
                //SpriteRenderer icon = Instantiate(rewardIconPrefab, reward.transform);
                //minimapObjects.Add(icon.gameObject);

                drop = SpawnReward(room, rewardPrefab, spawnTransform);
                GlobalData.Instance.ResetPercentage();

            }
            else
            {
                if (spawnTransform == null)
                {
                    SpriteRenderer icon = Instantiate(finishedRoomIconPrefab, new Vector3(room.centerPosition.x, room.centerPosition.y, 0), Quaternion.identity);
                    minimapObjects.Add(icon.gameObject);
                }
                GlobalData.Instance.AddPercentage(20);
            }
        }
        else
        {
            GlobalData.Instance.AddPercentage(20);
        }

        

        if (limitSpawn)
            room.decidedReward = true;
    }

    private SpriteRenderer CreateMinimapIcon(SpriteRenderer iconPrefab, Vector2 position)
    {
        SpriteRenderer icon = Instantiate(iconPrefab, position, Quaternion.identity);
        icon.sortingOrder = 2;

        minimapObjects.Add(icon.gameObject);

        return icon;
    }

    private GameObject DecideRoomReward(RewardType rewardType)
    {
        // 40 cura
        // 20 arma
        // 20 power up
        // 20 memento

        int rewardChance = Random.Range(0, 100);

        if (rewardType == RewardType.Heal) rewardChance = 61;
        else if (rewardType == RewardType.Gun) rewardChance = 41;
        else if (rewardType == RewardType.PowerUp) rewardChance = 21;
        else if (rewardType == RewardType.Memento) rewardChance = 1;

        if (rewardChance >= 60)
        {
            int rewardIndex = Random.Range(0, config.healRewards.Count);
            return config.healRewards[rewardIndex];
        }
        else if (rewardChance >= 40)
        {
            int rewardIndex = Random.Range(0, config.gunRewards.Count);
            return config.gunRewards[rewardIndex];
        }
        else if (rewardChance >= 20)
        {
            int rewardIndex = Random.Range(0, config.powerUpRewards.Count);
            return config.powerUpRewards[rewardIndex];
        }
        else if (rewardChance >= 0)
        {
            int rewardIndex = Random.Range(0, config.healRewards.Count);
            return config.healRewards[rewardIndex];
        }
        return null;
    }


    public void PlaceEnemiesOnRoom(Room room)
    {
        int enemiesSpawned = 0;

        List<Vector2Int> positions = new List<Vector2Int>(roomsDictionary[room.centerPosition]);
        Queue<Vector2Int> positionsQueue = new Queue<Vector2Int>(Shuffle(positions));

        positions = new List<Vector2Int>(wallPositions);

        List<Enemy> enemiesToSpawn = FindObjectOfType<ChallengeManager>().GetChallengeForRoom(room.GetRoomSize());
        Queue<Enemy> spawnQueue = new Queue<Enemy>(enemiesToSpawn);

        Debug.Log(spawnQueue.Count);

        while (spawnQueue.Count > 0)
        {
            Enemy currentEnemy = spawnQueue.Peek();
            Debug.Log($"Spawning {currentEnemy.name}");
            Vector2Int spawn = positionsQueue.Dequeue();

            RectInt enemyRect = RectHelper.GetNextRoomRect(spawn, 1);
            if (floorPositions.Contains(spawn) && !CheckRectCollideWithWalls(enemyRect) && !RectHelper.CheckOverlap(enemyRect, obstacleRects))
            {
                Enemy enemy = Instantiate(currentEnemy.gameObject, new Vector3(spawn.x, spawn.y, 0), Quaternion.identity).GetComponent<Enemy>();
                if (enemy.wallEnemy)
                {
                    spawn = GetNearWallPosition(spawn, positions);
                    positions.Remove(spawn);
                    enemy.transform.position = new Vector3(spawn.x, spawn.y, 0);
                    enemy.SetPlacementDirection(GetWallDir(spawn));
                }

                spawnQueue.Dequeue();

                enemy.currentRoom = room;
                instantiatedEnemies.Add(enemy);
                room.enemies.Add(enemy);

                Vector2 center = transform.position;
                if (enemy.GetComponent<SpriteRenderer>()) center = enemy.GetComponent<SpriteRenderer>().bounds.center;
                else if (enemy.transform.Find("Graphics")) center = enemy.transform.Find("Graphics").GetComponent<SpriteRenderer>().bounds.center;

                SpawnParticle spawnParticle = Instantiate(config.enemySpawnParticlePrefab, center, Quaternion.identity);
                spawnParticle.SetupEnemySpawn(enemy);
                enemiesSpawned++;
            }

            if (positionsQueue.Count <= 0)
                spawnQueue.Clear();
        }
    }

    public void PlaceKeyChest(Room room)
    {
        List<Vector2Int> positions = new List<Vector2Int>(roomsDictionary[room.centerPosition]);
        Queue<Vector2Int> positionsQueue = new Queue<Vector2Int>(Shuffle(positions));

        while (positionsQueue.Count > 0)
        {
            Vector2Int spawn = positionsQueue.Dequeue();

            RectInt chestRect = RectHelper.GetNextRoomRect(spawn, 2);
            if (floorPositions.Contains(spawn) && !CheckRectCollideWithWalls(chestRect) && !RectHelper.CheckOverlap(chestRect, obstacleRects))
            {
                GameObject chest = Instantiate(GetDungeonConfig().keyChestPrefab, new Vector3(spawn.x, spawn.y, 0), Quaternion.identity);
                chests.Add(chest);
                obstacleRects.Add(chestRect);
                break;
            }
        }
    }

    public GameObject SpawnReward(Room room, GameObject rewardSelected, Transform desiredPosition = null)
    {
        List<Vector2Int> positions = new List<Vector2Int>(roomsDictionary[room.centerPosition]);
        Queue<Vector2Int> positionsQueue = new Queue<Vector2Int>(Shuffle(positions));

        GameObject thisReward = null;
        if (desiredPosition == null)
        {
            while (positionsQueue.Count > 0)
            {
                Vector2Int spawn = positionsQueue.Dequeue();

                RectInt chestRect = RectHelper.GetNextRoomRect(spawn, 2);
                if (floorPositions.Contains(spawn) && !CheckRectCollideWithWalls(chestRect) && !RectHelper.CheckOverlap(chestRect, obstacleRects))
                {
                    GameObject reward = Instantiate(rewardSelected, new Vector3(spawn.x, spawn.y, 0), Quaternion.identity);
                    minimapObjects.Add(reward);
                    SpawnParticle spawnParticle = Instantiate(config.rewardSpawnParticle, new Vector3(spawn.x, spawn.y, 0), Quaternion.identity);
                    spawnParticle.SetupRewardSpawn(reward);
                    SpriteRenderer icon = Instantiate(rewardIconPrefab, reward.transform);
                    icon.GetComponent<InstantiateOnDestroy>().SetPosition(new Vector3(room.centerPosition.x, room.centerPosition.y, 0));
                    minimapObjects.Add(icon.gameObject);
                    thisReward = reward;
                    print("<color=yellow>REWARD: " + thisReward.name + " | POSITION: " + new Vector3(spawn.x, spawn.y, 0) + "</color>");
                    break;
                }
            }
        }
        else
        {
            GameObject reward = Instantiate(rewardSelected, desiredPosition.position, Quaternion.identity);
            minimapObjects.Add(reward);
            SpawnParticle spawnParticle = Instantiate(config.rewardSpawnParticle, desiredPosition.position, Quaternion.identity);
            spawnParticle.SetupRewardSpawn(reward);
            SpriteRenderer icon = Instantiate(rewardIconPrefab, reward.transform);
            icon.GetComponent<InstantiateOnDestroy>().SetPosition(new Vector3(room.centerPosition.x, room.centerPosition.y, 0));
            minimapObjects.Add(icon.gameObject);
            thisReward = reward;
            print("<color=yellow>REWARD: " + thisReward.name + " | POSITION: Player's Position </color>");
        }
        return thisReward;
    }

    private Direction GetWallDir(Vector2Int wallPosition)
	{
        List<Direction> dirs = new List<Direction>();
        if (floorPositions.Contains(wallPosition + new Vector2Int(0, -1))) dirs.Add(Direction.Up);
        if (floorPositions.Contains(wallPosition + new Vector2Int(-1, 0))) dirs.Add(Direction.Right);
        if (floorPositions.Contains(wallPosition + new Vector2Int(0, 1))) dirs.Add(Direction.Down);
        if (floorPositions.Contains(wallPosition + new Vector2Int(1, 0))) dirs.Add(Direction.Left);

        if (dirs.Count == 0) return Direction.None;
        else if (dirs.Count == 1) return dirs[0];
        else return Direction.Corner;
    }

    private Vector2Int GetNearWallPosition(Vector2Int origin, List<Vector2Int> positions)
	{
        Vector2Int near = origin;
        foreach(Vector2Int pos in positions)
		{
            if (near == origin) near = pos;
			else
			{
                Direction dir = GetWallDir(pos);
                if (dir != Direction.None && dir != Direction.Corner)
                {
                    if (Vector2.Distance(origin, pos) < Vector2.Distance(origin, near))
                    {
                        near = pos;
                    }
                }
			}
		}

        return near;
	}

    private void PlaceCornerObstacles()
    {
        foreach (var room in roomsDictionary)
        {
            if (room.Key != bossRoomCenter)
            {
                DecideCornerObstacles(room.Value);
            }
        }
    }

    private void CreateRooms()
    {
        roomsCount = Random.Range(config.minSortedRooms + GameplayManager.Instance.PlusMinMaxRooms, config.maxSortedRooms + GameplayManager.Instance.PlusMinMaxRooms);
        Debug.Log($"Generating {roomsCount} rooms...");
        Vector2Int startingPoint = Vector2Int.zero;
        Vector2Int currentPoint = startingPoint;
        int currentRoomSizeIndex = 0;
        int nextPadding = 0;

        while (roomRects.Count < roomsCount)
        {
            Debug.Log($"Room {roomRects.Count + 1}");
            CreateRandomWalkDataSO roomSizeParam = roomSizesParameters[currentRoomSizeIndex];
            int currentLimit = roomSizeParam.limitDistanceFromCenter;
            int originSize = 0;
            if (roomCenters.Count > 0)
            {
                currentPoint = roomCenters[Random.Range(0, roomCenters.Count)];
                originSize = (int)GetRoomSize(currentPoint);
            }
            
            nextPadding = RoomCreationHelper.GetPaddingToNextRoom(originSize, currentLimit);
            Vector2Int spawnRoomPoint = RoomCreationHelper.GetNextRoomCenter(currentPoint, nextPadding);
            var rect = RectHelper.GetNextRoomRect(spawnRoomPoint, currentLimit);
            Debug.Log("Check new room overlap");
            if (!RectHelper.CheckOverlap(rect, roomRects))
            {
                Debug.Log("Room didn't overlap");
                GenerateRoom(currentPoint, spawnRoomPoint, currentRoomSizeIndex, roomSizeParam, rect);
                currentRoomSizeIndex = Random.Range(0, 3);
            } else
            {
                Debug.Log("Room overlapped!");
            }
        }

        config.NpcSpawnChances.Setup();
        var npcRoomCount = config.NpcSpawnChances.GetNpcRoomCount();
        Debug.Log($"Spawning {npcRoomCount} NPC rooms...");

        bool removeExplorer = false;
        //List<PlayerEntryPanel> entries = new List<PlayerEntryPanel>(FindObjectsOfType<PlayerEntryPanel>());

        //foreach (var item in entries)
        //{
        //    if (item.GetCurrentPlayer().characterName.Contains("Irwin"))
        //        removeExplorer = true;
        //}

        for (int i = 0; i < npcRoomCount; i++)
        {
            var npc = config.NpcSpawnChances.GetNpcType(removeExplorer);
            PlaceNPCRoom(npc.gameObject);
        }
        
        PlaceBossRoom();
        Debug.Log("Painting floor...");
        tilemapVisualizer.PaintFloorTiles(floorPositions);
        wallPositions = WallGenerator.CreateWalls(floorPositions, tilemapVisualizer);
        // WebDecorations
        if (config.cornerDecorations.Count > 0)
            InstantiateCornerDecorations(config.cornerDecorations, placedCornerDictionary);
        if (config.SpotLightPrefab != null)
        {
            if (config.spawnLights) 
            {
                AddLightAtDarkCorners();
                AddLightAtCorridors();
            }
        }
        PlaceSpriteMasks(wallPositions);
    }

    private void FillContinuousWalls(Room room)
    {
        // Improve this logic
        Debug.Log("Spawning obstacles next to walls...");
        List<Vector2Int> cornersList = GetCornersPositions(room.roomPositions);
        int i = 1;
        foreach (var position in cornersList)
        {
            if (i < cornersList.Count)
            {
                Vector2Int nextPosition = cornersList[i];
                bool isHorizontal = position.x == nextPosition.x;
                bool isVertical = position.y == nextPosition.y;
                if (isHorizontal || isVertical)
                {
                    float distance = Vector2Int.Distance(position, nextPosition);
                    if (distance > 3)
                    {
                        Vector2Int spawnPosition;
                        if (isHorizontal)
                        {
                            spawnPosition = GetHorizontalCentralPosition(position, nextPosition);
                        }
                        else
                        {
                            spawnPosition = GetVerticalCentralPosition(position, nextPosition);
                        }

                        if (room.roomPositions.Contains(spawnPosition))
                        {
                            PlaceableObject placeable = GetPrimaryOrSecondaryObstacle();
                            GameObject spawnedObstacle = InstantiateObstacleIfDoesntCollide(placeable, spawnPosition);
                            if (spawnedObstacle)
                            {
                                room.obstacles.Add(spawnedObstacle);
                            }
                        }
                    }
                }
            }
            i++;
        }
    }

    GameObject InstantiateObstacleIfDoesntCollide(PlaceableObject placeable, Vector2Int spawnPosition)
    {
        Vector3 spawnPoint = new Vector3(spawnPosition.x + .5f, spawnPosition.y + .5f, 0);
        RectInt rect = RectHelper.DrawRectFromCorner(spawnPosition, placeable.sizeX, placeable.sizeY);
        if (RectHelper.CheckOverlap(rect, obstacleRects) == false)
        {
            GameObject obstacle = Instantiate(placeable.prefab, spawnPoint, Quaternion.identity);
            Placeable isPlaceable = obstacle.GetComponent<Placeable>();
            if (isPlaceable != null)
            {
                RelativePosition relativeToWall = GetWallDirection(spawnPosition);
                isPlaceable.SetRotation(relativeToWall);
            }
            obstacleRects.Add(rect);
            placedObstaclesDictionary[spawnPosition] = obstacle;

            return obstacle;
        }

        return null;
    }

    private RelativePosition GetWallDirection(Vector2Int spawnPosition)
    {
        Vector3 spawnPoint = new Vector3(spawnPosition.x, spawnPosition.y, 0);
        foreach (var direction in Direction2d.cardinalDirectionsList)
        {
            Vector2Int toCheck = spawnPosition + direction;
            if (wallPositions.Contains(toCheck))
            {
                Vector3 wallPoint = new Vector3(toCheck.x, toCheck.y, 0);
                return GetRelativePosition(spawnPoint, wallPoint);
            }
        }
        return RelativePosition.Right;
    }

    private Vector2Int GetVerticalCentralPosition(Vector2Int position, Vector2Int nextPosition)
    {
        int center = Mathf.FloorToInt((position.y + nextPosition.y) / 2);
        return new Vector2Int(position.y, center);

    }

    private Vector2Int GetHorizontalCentralPosition(Vector2Int position, Vector2Int nextPosition)
    {
        int center = Mathf.FloorToInt((position.x + nextPosition.x) / 2);
        return new Vector2Int(center, position.y);
    }

    PlaceableObject GetPrimaryOrSecondaryObstacle()
    {
        Debug.Log("Deciding between primary or secondary obstacle...");
        int chanceResult = Random.Range(0, 100);
        int chanceLimit = 100 - config.primaryOrSecondaryChanceOfPrimary;
        if (chanceResult > chanceLimit)
        {
            Debug.Log("Primary it is!");
            return GetPrimaryBreakableOrUnbreakable();
        } else
        {
            Debug.Log("Secondary it is!");
            return GetSecondaryObstacle();
        }
    }

    private PlaceableObject GetSecondaryObstacle()
    {
        int sortedObstacle = Random.Range(0, config.secondaryBreakableObstacles.Count);
        return config.secondaryBreakableObstacles[sortedObstacle];
    }

    private PlaceableObject GetPrimaryBreakableOrUnbreakable()
    {
        int sortedChance = Random.Range(0, 100);
        int chanceLimit = 100 - 20;
        if (sortedChance > chanceLimit && config.primaryUnbreakableObstacles.Count > 0)
        {
            int sorted = Random.Range(0, config.primaryUnbreakableObstacles.Count);
            return config.primaryUnbreakableObstacles[sorted];
        } else
        {
            int sorted = Random.Range(0, config.primaryBreakableObstacles.Count);
            return config.primaryBreakableObstacles[sorted];
        }
    }

    private void GenerateRoom(Vector2Int previousPoint, Vector2Int currentPoint, int currentRoomSizeIndex, CreateRandomWalkDataSO roomSizeParam, RectInt rect, bool placeObstacles = true, bool isRandom = true, RoomType roomType = RoomType.EnemiesDefault)
    {
        var corridor = new HashSet<Vector2Int>();
        if (roomRects.Count > 0)
        {
            corridor = RoomCreationHelper.CreateCorridor(previousPoint, currentPoint);
            if (corridor.Count > 0)
            {
                corridorPositions.UnionWith(corridor);
                floorPositions.UnionWith(corridor);
            } else {
                return;
            }
        }
        roomRects.Add(rect);
        int lightRange = Mathf.FloorToInt(roomSizeParam.limitDistanceFromCenter * .6f);
        int lightSize = roomSizeParam.limitDistanceFromCenter + lightRange;
        Room room = CreateRoom(roomSizeParam, currentPoint, currentRoomSizeIndex, roomType, isRandom, rect);

        if (roomType == RoomType.Boss)
        {
            room.SetOST(config.bossOST);
        }
        room.clearSound = config.clearRoomSound;
        // Layer1Decorations
        bool shouldSpawnLayer1 = Random.value > .3;
        if (shouldSpawnLayer1)
        {
            InstantiateLayerDecoration(config.layer1Decorations, 1, 5, room.roomPositions);
        }
        // Layer2Decorations
        bool shouldSpawnLayer2 = Random.value > .5;
        if (shouldSpawnLayer2)
        {
            InstantiateLayerDecoration(config.layer2Decorations, 2, 3, room.roomPositions);
        }
        // Layer3Decorations
        bool shouldSpawnLayer3 = Random.value > .85;
        if (shouldSpawnLayer3)
        {
            InstantiateLayerDecoration(config.layer3Decorations, 3, 2, room.roomPositions);
        }
        AddLights(new Vector3(currentPoint.x, currentPoint.y, 0), lightSize, room);
        
        FloorStats fs = new FloorStats();
        fs.GetFloorInfo(room.roomPositions);
        Vector2Int startSquare = new Vector2Int(fs.minX, fs.minY);
        RectInt preventGateArea = RectHelper.DrawRectFromPoint(startSquare, fs.maxX - fs.minX + 1, fs.maxY - fs.minY + 1);
        gatePreventionRects.Add(preventGateArea);

        floorPositions.UnionWith(room.roomPositions);
        SaveRoomData(currentPoint, room.roomPositions);
        roomCenters.Add(currentPoint);

        FillHoles(floorPositions);
        GenerateGates(room, corridor, roomType, preventGateArea);

        if (placeObstacles)
        {
            DecideCenterTemplate(room);
            FillContinuousWalls(room);
        }
    }

    private void InstantiateCornerDecorations(List<GameObject> cornerDecorations, Dictionary<Vector2Int, GameObject> placedCornerDictionary)
    {
        Debug.Log("Placing corner decorations...");
        List<Vector2Int> corners = new List<Vector2Int>();
        List<string> cornerAssetID = new List<string>()
        {
            AssetId.wallDiagonalCornerDownLeft,
            AssetId.wallDiagonalCornerDownRight,
            AssetId.wallDiagonalCornerUpRight,
            AssetId.wallDiagonalCornerUpLeft,
        };
        foreach (var asset in tilemapVisualizer.assetsPositions)
        {
            if(cornerAssetID.Contains(asset.Value))
            {
                corners.Add(asset.Key);
            }
        }
        foreach (var corner in corners)
        {
            bool shouldSpawn = Random.value > .5;
            if (shouldSpawn)
            {
                int sortedAssetIndex = Random.Range(0, cornerDecorations.Count - 1);
                GameObject cornerAsset = Instantiate(cornerDecorations[sortedAssetIndex], new Vector3(corner.x + .5f, corner.y + .5f, 0), Quaternion.identity);
                if (tilemapVisualizer.assetsPositions[corner] == AssetId.wallDiagonalCornerDownRight)
                {
                    cornerAsset.GetComponent<Placeable>().FlipX(true);
                } else if (tilemapVisualizer.assetsPositions[corner] == AssetId.wallDiagonalCornerUpRight)
                {
                    cornerAsset.GetComponent<Placeable>().FlipX(true);
                    cornerAsset.GetComponent<Placeable>().FlipY(true);
                } else if (tilemapVisualizer.assetsPositions[corner] == AssetId.wallDiagonalCornerUpLeft)
                {
                    cornerAsset.GetComponent<Placeable>().FlipY(true);
                }
                placedCornerDictionary[corner] = cornerAsset;
            }
        }
    }

    private void CreateMinimapRoomIcons()
	{
        foreach(Room room in rooms)
		{
            if (room.roomType == RoomType.Boss)
			{
                CreateMinimapIcon(bossIconPrefab, room.centerPosition);
            }
        }

        CreateFloorMinimapIcons();
    }

    private void CreateFloorMinimapIcons()
	{
        foreach(Vector2Int position in floorPositions)
        {
            SpriteRenderer minimapIcon = GameObject.Instantiate(minimapIconPrefab, new Vector3(position.x, position.y, 0), Quaternion.identity);
            minimapIcon.transform.Translate(.5f, .5f, 0);
            minimapIcon.color = minimapFloorColor;
            minimapIcon.sortingOrder = 0;
            minimapObjects.Add(minimapIcon.gameObject);
        }
	}

    private Room CreateRoom(CreateRandomWalkDataSO randomWalkData, Vector2Int startingPoint, int roomSizeIndex, RoomType type = RoomType.EnemiesDefault, bool isRandom = true, RectInt? rect = null)
    {
        HashSet<Vector2Int> roomFloor;
        if (!isRandom && rect != null)
        {
            roomFloor = RoomCreationHelper.CreateRoomFloorFromRectInt((RectInt)rect);
        }
        else
        {
            roomFloor = RunRandomWalk(roomSizesParameters[roomSizeIndex], startingPoint);
        }
        foreach (var position in roomFloor)
        {
            roomFloor.Add(position);
        }

        Room room = new Room(randomWalkData, type, startingPoint, roomFloor, this);
        rooms.Add(room);

        return room;
    }

    private void DecideCornerObstacles(HashSet<Vector2Int> room)
    {
        List<Vector2Int> cornersList = GetCornersPositions(room);
        bool shouldSpawn = Random.value > .5;
        int spawnLimit = 100 - config.cornerBoxesSpawnChance;
        foreach (var position in cornersList)
        {
            if (shouldSpawn && !corridorPositions.Contains(position))
            {
                int spawnChance = Random.Range(0, 100);
                if (spawnChance > spawnLimit)
                {
                    int obstacleIndex = Random.Range(0, config.secondaryBreakableObstacles.Count);
                    PlaceableObject sortedObstacle = config.secondaryBreakableObstacles[obstacleIndex];
                    InstantiateObstacleIfDoesntCollide(sortedObstacle, position);
                }
            }
            shouldSpawn = !shouldSpawn;
        }
    }

    private List<Vector2Int> GetCornersPositions(HashSet<Vector2Int> room)
    {
        List<Vector2Int> cornerPositions = new List<Vector2Int>();
        foreach (var position in room)
        {
            int wallsAround = CountWallsAroundPosition(room, position);
            if (wallsAround == 2)
            {
                cornerPositions.Add(position);
            }
        }
        return cornerPositions;
    }

    private int CountWallsAroundPosition(HashSet<Vector2Int> room, Vector2Int position)
    {
        int walls = 0;
        foreach (var direction in Direction2d.cardinalDirectionsList)
        {
            Vector2Int checkPosition = position + direction;
            if (room.Contains(checkPosition) == false)
            {
                walls++;
            }
        }
        return walls;
    }

    void DecideCenterTemplate(Room room)
    {
        int chanceResult = Random.Range(0, 100);
        int chanceLimit = 100 - config.centerTemplateSpawnChance;
        Debug.Log("Deciding if room will have center template...");
        if (chanceResult > chanceLimit)
        {
            Debug.Log("It will, spawning...");
            bool isSpawned = false;
            int tries = 0, maxTries = config.roomCenterTemplates.Count;
            Vector2Int geometricCenter = RoomUtils.GetGeometricCenter(room.roomPositions);
            int currentTemplate = Random.Range(0, config.roomCenterTemplates.Count);
            while ((tries <= maxTries) && !isSpawned)
            {
                if (tries > 0)
                {
                    currentTemplate = GetNextTemplate(currentTemplate);
                }
                PlaceableObject template = config.roomCenterTemplates[currentTemplate];
                var rect = RectHelper.DrawRectFromCorner(geometricCenter, template.sizeX, template.sizeY);
                if (!RectHelper.CheckRectIsOutOfBounds(rect, room.roomPositions) && !RectHelper.CheckOverlap(rect, obstacleRects))
                {
                    Debug.Log("Center template fit");
                    isSpawned = true;
                    room.obstacles.Add(SpawnCenterTemplate(geometricCenter, template, rect));
                }
                else
                {
                    Debug.Log("Center template doesn't fit");
                    tries++;
                }
            }
            if (tries > maxTries)
            {
                Debug.Log("Max tries reached, exiting...");
            }
        }
    }

    int GetNextTemplate(int currentTemplate)
    {
        Debug.Log("Getting next template...");
        if (currentTemplate >= config.roomCenterTemplates.Count)
        {
            return 0;
        }
        else
        {
            return currentTemplate++;
        }
    }

    GameObject SpawnCenterTemplate(Vector2Int spawnPosition, PlaceableObject template, RectInt rect)
    {
        Debug.Log("Spawning center template...");
        obstacleRects.Add(rect);
        GameObject roomCenterTemplate = Instantiate(template.prefab, new Vector3(spawnPosition.x, spawnPosition.y, 0), Quaternion.identity);
        placedObstaclesDictionary[spawnPosition] = roomCenterTemplate;

        return roomCenterTemplate;
    }

    private void GenerateGates(Room room, HashSet<Vector2Int> corridor, RoomType roomType, RectInt preventGateArea)
    {
        corridor.ExceptWith(room.roomPositions);
        if (corridor.Count == 0)
        {
            return;
        }
        Debug.Log("Creating corridor...");
        Vector2Int potentialPosition = Vector2Int.zero;
        foreach (var position in corridor)
        {
            potentialPosition = position;
        }
        Vector3 doorPosition = new Vector3(potentialPosition.x, potentialPosition.y, 0);
        Vector3 centerPosition = new Vector3(room.centerPosition.x, room.centerPosition.y, 0);
        RelativePosition relativePosition = GetRelativePosition(centerPosition, doorPosition);
        
        Vector2Int bestPosition = GateCreationHelper.GetBestDoorPosition(corridor, relativePosition, room.centerPosition, floorPositions, preventGateArea);
        Vector3 gateSpawnPoint = new Vector3(bestPosition.x, bestPosition.y, 0);

        Debug.Log("Placing gate at best position...");
        Gate gate = Instantiate(config.doorPrefab, gateSpawnPoint, Quaternion.identity);
        RectInt rect = RectHelper.DrawRectFromCorner(bestPosition, 5, 5);
        obstacleRects.Add(rect);
        gate.GetComponent<Gate>().SetRelativeOffset(relativePosition);
        if (Application.isPlaying)
        {
            gate.SetGateType(roomType);
        }
        room.gates.Add(gate);
        gates.Add(gate);
        gate.room = room;

        placedDoorsDictionary[bestPosition] = gate.gameObject;

        gate.transform.rotation *= GateCreationHelper.GetDoorRotation(relativePosition);
    }

    public void InstantiateLayerDecoration(List<PlaceableObject> decorations, int layer, int count, HashSet<Vector2Int> possiblePositions)
    {
        Debug.Log($"Instantiate layer {layer} decorations");
        if (!(decorations.Count > 0))
        {
            return;
        }
        int instantiated = 0;
        int tries = 0;
        int maxTries = count * 2;
        while (tries <= maxTries && instantiated < count)
        {
            tries++;
            int sorted = Random.Range(0, decorations.Count - 1);
            Vector2Int position = RoomCreationHelper.GetRandomPoint(possiblePositions);
            Vector3 pos = new Vector3(position.x, position.y, 0);
            PlaceableObject placeable = decorations[sorted];
            RectInt rect = RectHelper.DrawRectFromPoint(position, placeable.sizeX, placeable.sizeY);
            var rectsList = GetLayerRectsList(layer);
            var layerDictionarie = GetLayerDictionarie(layer);
            if (!RectHelper.CheckOverlap(rect, rectsList) && !RectHelper.CheckRectIsOutOfBounds(rect, possiblePositions))
            {
                GameObject o = Instantiate(placeable.prefab, pos, Quaternion.identity);
                layerDictionarie[position] = o;
                rectsList.Add(rect);
                instantiated++;
            }
        }
        if (tries > maxTries)
        {
            Debug.Log($"Max tries reached, exiting...");
        }
    }

    private List<RectInt> GetLayerRectsList(int layer)
    {
        if (layer == 1)
        {
            return layer1Rects;
        } else if (layer == 2)
        {
            return layer2Rects;
        } else if (layer == 3)
        {
            return layer3Rects;
        }
        return layer1Rects;
    }

    private Dictionary<Vector2Int, GameObject> GetLayerDictionarie(int layer)
    {
        if (layer == 1)
        {
            return placedLayer1Dictionary;
        }
        else if (layer == 2)
        {
            return placedLayer2Dictionary;
        }
        else if (layer == 3)
        {
            return placedLayer3Dictionary;
        }
        return placedLayer1Dictionary;
    }

    RelativePosition GetRelativePosition(Vector3 vectorA, Vector3 vectorB)
    {
        Vector3 relative = vectorA - vectorB;

        float x = relative.x;
        float y = relative.y;

        //UP
        if (y > 0 && (x > -y && x < y))
        {
            return RelativePosition.Up;
        }

        //RIGHT
        if (x > 0 && (y > -x && y < x))
        {
            return RelativePosition.Right;
        }

        //LEFT
        if (x < 0 && (y > x && y < -x))
        {
            return RelativePosition.Left;
        }

        //DOWN
        if (y < 0 && (x > y && x < -y))
        {
            return RelativePosition.Down;
        }

        return RelativePosition.Down;
    }

    private void AddLightAtDarkCorners()
    {
        Debug.Log("Placing light at dark corners...");
        foreach (var center in roomCenters)
        {
            if (!config.PlaceLightsInBossRoom && center == bossRoomCenter)
                continue;

            HashSet<Vector2Int> darkSpots = GetDarkSpots(center, roomsDictionary[center]);
            HashSet<Vector2Int> wallPositions = new HashSet<Vector2Int>();
            int i = 0;
            foreach (var direction in Direction2d.cardinalDirectionsList)
            {
                var spots = FindEmptyNeighbour(roomsDictionary[center], darkSpots, direction);
                AddSpotlights(spots, i);
                i++;
            }

        }
    }

    private void AddLightAtCorridors()
    {
        Debug.Log("Placing light at corridors...");
        HashSet<Vector2Int> onlyCorridors = corridorPositions;
        foreach (var room in roomsDictionary)
        {
            onlyCorridors.ExceptWith(room.Value);
        }
        int i = 0;
        foreach (var direction in Direction2d.cardinalDirectionsList)
        {
            var spots = FindEmptyNeighbour(corridorPositions, onlyCorridors, direction);
            AddSpotlights(spots, i);
            i++;
        }
    }

    private void AddSpotlights(HashSet<Vector2Int> positions, int direction)
    {
        foreach (var position in positions)
        {
            int lightPadding = config.lightPaddingFromEachOther;
            RectInt rect = RectHelper.GetNextRoomRect(position, lightPadding);
            bool isCorner = WallGenerator.IsCorner(wallPositions, position);
            if (!isCorner && !RectHelper.CheckOverlap(rect, lightRects) && !RectHelper.CheckOverlap(rect, obstacleRects))
            {
                Vector3 pos = new Vector3(position.x, position.y, 0);
                GameObject spot = Instantiate(config.SpotLightPrefab, pos, Quaternion.identity);
                spot.GetComponent<WallLight>().SetPlacementDirection(direction);
                lights.Add(spot);
                lightRects.Add(rect);
            }
        }
    }

    private void FillHoles(HashSet<Vector2Int> floor)
    {
        List<Vector2Int> holesToFix = new List<Vector2Int>();
        foreach (var position in floor)
        {
            foreach (var direction in Direction2d.cardinalDirectionsList)
            {
                Vector2Int neighbour = position + direction;
                Vector2Int nextNeighbour = neighbour + direction;
                if (!floor.Contains(neighbour) && floor.Contains(nextNeighbour))
                {
                    holesToFix.Add(neighbour);
                }
            }
        }
        floor.UnionWith(holesToFix);
    }

    private void AddLights(Vector3 position, int size, Room room)
    {
        if (config.AmbientLightPrefab)
        {
            Debug.Log("Adding ambient lights...");
            GameObject ambientLight = Instantiate(config.AmbientLightPrefab, position, Quaternion.identity);
            if (ambientLight.GetComponent<Light2D>()) ambientLight.GetComponent<Light2D>().pointLightOuterRadius = size;
            lights.Add(ambientLight);
        }
        

        if (config.FloorLightPrefab && config.spawnLights)
        {
            Debug.Log("Adding floor lights...");
            GameObject floorLight = Instantiate(config.FloorLightPrefab, position, Quaternion.identity);
            if (Application.isPlaying && GameplayManager.Instance.currentDungeonType == DungeonType.Lab)
            {
                floorLight.GetComponent<Light2D>().pointLightOuterRadius = size;
            }
            
            lights.Add(floorLight);
            room.centralLight = floorLight;
        }
    }

    private void SetupAmbientLights()
	{
        List<Room> shuffledRoomList = Shuffle(new List<Room>(rooms));
        for (int i = 0; i < shuffledRoomList.Count / 2; i++)
		{
            Room room = shuffledRoomList[i];
            if (GameplayManager.Instance.currentDungeonType == DungeonType.Lab)
            {
                room.centralLight.GetComponent<Light2DBlink>().StartBlink();
            }
        }
	}

    public List<Room> GetCombatRooms()
    {
        List<Room> theseRooms = new List<Room>();

        for (int i = 0; i < rooms.Count; i++)
        {
            if (rooms[i].roomType == RoomType.EnemiesDefault)
                theseRooms.Add(rooms[i]);
        }

        return theseRooms;
    }

    public List<T> Shuffle<T>(List<T> list)
    {
        System.Random rng = new System.Random();
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }

        return list;
    }

    private void ClearLights()
    {
        // Debug.Log($"Cleaning {lights.Count} lights");
        for (int i = lights.Count - 1; i >= 0; i--)
        {
            SafeDestroy(lights[i].gameObject);
        }
        lights.Clear();
        lightRects.Clear();
    }

    private void ClearObstacles()
    {
        // Debug.Log($"Cleaning {placedObstaclesDictionary.Count} obstacles");
        foreach (var obstacle in placedObstaclesDictionary)
        {
            SafeDestroy(obstacle.Value.gameObject);
        }
        placedObstaclesDictionary.Clear();
        obstacleRects.Clear();
    }

    private void ClearDoors()
    {
        // Debug.Log($"Cleaning {placedDoorsDictionary.Count} doors");
        foreach (var door in placedDoorsDictionary)
        {
            SafeDestroy(door.Value.gameObject);
        }
        placedDoorsDictionary.Clear();
    }

    private void ClearExternalLayer()
    {
        // Debug.Log($"Cleaning {placedDoorsDictionary.Count} doors");
        foreach (var decoration in placedExternalDecorationDictionary)
        {
            SafeDestroy(decoration.Value.gameObject);
        }
        placedExternalDecorationDictionary.Clear();
        externalDecorationsRects.Clear();
    }

    private void ClearEnemies()
    {
        // Debug.Log($"Cleaning {instantiatedEnemies.Count} lights");
        for (int i = instantiatedEnemies.Count - 1; i >= 0; i--)
        {
            SafeDestroy(instantiatedEnemies[i].gameObject);
        }
    }

    private void ClearNPCS()
    {
        // Debug.Log($"Cleaning {placedNPCsDictionary.Count} NPCs");
        foreach (var npc in placedNPCsDictionary)
        {
            SafeDestroy(npc.Value.gameObject);
        }
        placedNPCsDictionary.Clear();
    }

    private void ClearLayers()
    {
        // Debug.Log($"Cleaning {placedLayer1Dictionary.Count} layer1 decoration");
        foreach (var decoration in placedLayer1Dictionary)
        {
            SafeDestroy(decoration.Value.gameObject);
        }
        placedLayer1Dictionary.Clear();
        layer1Rects.Clear();
        // Debug.Log($"Cleaning {placedLayer2Dictionary.Count} layer2 decoration");
        foreach (var decoration in placedLayer2Dictionary)
        {
            SafeDestroy(decoration.Value.gameObject);
        }
        placedLayer2Dictionary.Clear();
        layer2Rects.Clear();
        // Debug.Log($"Cleaning {placedLayer3Dictionary.Count} layer3 decoration");
        foreach (var decoration in placedLayer3Dictionary)
        {
            SafeDestroy(decoration.Value.gameObject);
        }
        placedLayer3Dictionary.Clear();
        foreach (var decoration in placedCornerDictionary)
        {
            SafeDestroy(decoration.Value.gameObject);
        }
        placedCornerDictionary.Clear();
        layer3Rects.Clear();
    }

    private void ClearLists()
    {
        floorPositions.Clear();
        wallPositions.Clear();
        corridorPositions.Clear();
        roomSizesParameters.Clear();
        roomRects.Clear();
        roomCenters.Clear();
        roomsDictionary.Clear();
    }

    void SpawnKeyChests()
    {
        List<Room> combatRooms = Shuffle(GetCombatRooms());
        Queue<Room> combatRoomQueue = new Queue<Room>(combatRooms);

        while (chests.Count < GameplayManager.Instance.keysNeeded)
        {
            Room current = combatRoomQueue.Dequeue();
            print("|———————————————| Chest is in: " + current.roomType + " room type");
            PlaceKeyChest(current);
            if (combatRoomQueue.Count <= 0)
            {
                GameplayManager.Instance.keysNeeded = chests.Count;
                break;
            }
        }
    }

    private void SpawnPlayer()
    {
        if (!Application.isPlaying)
        {
            return;
        }

        // Sorting room
        Room playerSpawnRoom = rooms[Random.Range(0, rooms.Count)];
        Debug.Log($"Sorted room for player start {playerSpawnRoom}");
        bool isSpawned = false;
		while (!isSpawned)
		{
            bool isNotpreventedRoom = playerSpawnRoom.roomType != RoomType.Boss && playerSpawnRoom.roomType != RoomType.NPC;

            if (isNotpreventedRoom && playerSpawnRoom.randomWalkData.size == Size.Small)
			{
                Debug.Log($"Spawn point is {playerSpawnRoom.centerPosition}");
                playerSpawnRoom.roomType = RoomType.PlayerSpawn;
				playerSpawnPoint = playerSpawnRoom.centerPosition;
				isSpawned = true;
			}
			else
			{
                
                playerSpawnRoom = rooms[Random.Range(0, rooms.Count)];
                Debug.Log($"This room was prevented from spawning, trying with {playerSpawnRoom}");
            }
		}
		
        Vector3 spawnPosition = new Vector3(playerSpawnPoint.x, playerSpawnPoint.y, 0);
        if (GameplayManager.Instance.players.Count <= 0)
        {
            Debug.Log($"Spawning players...");
            GameplayManager.Instance.SpawnPlayers(playerSpawnPoint);
        } 
        else
        {
            Debug.Log($"Players are spawned, setting position...");
            GameplayManager.Instance.SetPlayerPositions(spawnPosition);
            GameplayManager.Instance.SetAllPlayersSpriteRenderers(true);
        }
        
        SpriteRenderer icon = Instantiate(startRoomIconPrefab, spawnPosition, Quaternion.identity);
        minimapObjects.Add(icon.gameObject);

        foreach (Collider2D hit in Physics2D.OverlapCircleAll(spawnPosition, 3, LayerMask.GetMask("Obstacles", "Holes")))
		{
            SafeDestroy(hit.gameObject);
		}
    }

    private void SpawnPlayerOnPocketMorRoom()
    {
        if (!Application.isPlaying)
        {
            return;
        }

        Vector3 spawnPosition = PocketMorManager.Instance.transform.position;
        if (GameplayManager.Instance.players.Count <= 0)
        {
            GameplayManager.Instance.SpawnPlayers(new Vector2Int((int)spawnPosition.x, (int)spawnPosition.y));
            foreach(Player p in GameplayManager.Instance.players)
			{
                p.EnablePlayerLight(false);
            }
        }
        else
        {
            GameplayManager.Instance.SetPlayerPositions(spawnPosition);
            GameplayManager.Instance.SetAllPlayersSpriteRenderers(true);
        }

        SpriteRenderer icon = Instantiate(startRoomIconPrefab, spawnPosition, Quaternion.identity);
        minimapObjects.Add(icon.gameObject);

        foreach (Collider2D hit in Physics2D.OverlapCircleAll(spawnPosition, 3, LayerMask.GetMask("Obstacles", "Holes")))
        {
            SafeDestroy(hit.gameObject);
        }
    }

    private bool CheckRectCollideWithWalls(RectInt rect)
    {
        foreach (var position in rect.allPositionsWithin)
        {
            if (wallPositions.Contains(position) || corridorPositions.Contains(position))
            {
                return true;
            }
        }
        return false;
    }

    private bool CheckRectCollideWithFloor(RectInt rect)
    {
        foreach (var position in rect.allPositionsWithin)
        {
            if (floorPositions.Contains(position))
            {
                return true;
            }
        }
        return false;
    }

    private void SaveRoomData(Vector2Int roomCenter, HashSet<Vector2Int> roomFloor)
    {
        roomsDictionary[roomCenter] = roomFloor;
    }

    private HashSet<Vector2Int> GetDarkSpots(Vector2Int roomCenter, HashSet<Vector2Int> roomFloor)
    {
        float darkSpotDistance = config.distanceFromCenterToDarkSpot;
        HashSet<Vector2Int> spots = new HashSet<Vector2Int>();
        foreach (var position in roomFloor)
        {
            float distance = Vector2Int.Distance(roomCenter, position);
            if (distance > darkSpotDistance)
            {
                spots.Add(position);
            }
        }
        return spots;
    }

    private HashSet<Vector2Int> FindEmptyNeighbour(HashSet<Vector2Int> roomFloor, HashSet<Vector2Int> positionsToLook, Vector2Int direction)
    {
        HashSet<Vector2Int> wantedPositions = new HashSet<Vector2Int>();
        foreach (var position in positionsToLook)
        {
            Vector2Int neighbour = position + direction;
            if (!roomFloor.Contains(neighbour) && wallPositions.Contains(neighbour))
            {
                wantedPositions.Add(neighbour);
            }
        }
        return wantedPositions;
    }

    void OnDrawGizmos()
    {
        // Green
        Gizmos.color = new Color(0.0f, 1.0f, 0.0f);
        foreach (var lightRect in lightRects)
        {
            DrawRect(lightRect);
        }
        Gizmos.color = new Color(1.0f, 0.5f, 0.0f);
        foreach (var roomRect in roomRects)
        {
            DrawRect(roomRect);
        }
        Gizmos.color = new Color(1.0f, 1f, 0.5f);
        foreach (var obstaclesRect in obstacleRects)
        {
            DrawRect(obstaclesRect);
        }

        Gizmos.color = new Color(.5f, 1f, 0.5f);
        foreach (var door in placedDoorsDictionary)
        {
            RectInt doorRect = new RectInt(door.Key, new Vector2Int(1, 1));
            DrawRect(doorRect);
        }
        Gizmos.color = Color.blue;
        foreach (var layer1Rect in layer1Rects)
        {
            DrawRect(layer1Rect);
        }
        foreach (var layer2Rect in layer2Rects)
        {
            DrawRect(layer2Rect);
        }
        foreach (var layer3Rect in layer3Rects)
        {
            DrawRect(layer3Rect);
        }
        Gizmos.color = Color.red;
        DrawRect(aroundFloor);
        foreach (var external in externalDecorationsRects)
        {
            DrawRect(external);
        }
        Gizmos.color = Color.cyan;
        foreach (var gatePrevent in gatePreventionRects)
        {
            DrawRect(gatePrevent);
        }
    }

    void DrawRect(RectInt rect)
    {
        Gizmos.DrawWireCube(new Vector3(rect.center.x, rect.center.y, 0.01f), new Vector3(rect.size.x, rect.size.y, 0.01f));
    }

    RoomLimits GetRoomSize(Vector2Int roomCenter)
    {
        int positionsCount = roomsDictionary[roomCenter].Count;
        if (positionsCount >= (int)RoomSizesMinPositions.Big)
        {
            return RoomLimits.Large;
        }
        else if (positionsCount >= (int)RoomSizesMinPositions.Medium)
        {
            return RoomLimits.Medium;
        }
        else
        {
            return RoomLimits.Small;
        }
    }

    void SafeDestroy(GameObject go)
    {
        if (Application.isPlaying)
        {
            Destroy(go);
        }
        else
        {
            DestroyImmediate(go);
        }
    }

    private EnemiesEncounter GetEnemyEncounter(int challengeLevel)
    {
        EnemiesEncounter[] encountersData = Resources.LoadAll<EnemiesEncounter>("Data/EnemiesEncounter/" + config.enemiesEncountersFolder);

        possibleEncountersData = new List<EnemiesEncounter>();
        foreach (EnemiesEncounter encounterData in encountersData)
        {
            if (encounterData.enabled)
            {
                if (encounterData.challengeLevel == challengeLevel)
                {
                    possibleEncountersData.Add(encounterData);
                }
                else if (challengeLevel >= 11)
                {
                    if (encounterData.challengeLevel >= 11)
                    {
                        possibleEncountersData.Add(encounterData);
                    }
                }
            }
        }

        EnemiesEncounter encounter;
        if (possibleEncountersData.Count > 0) encounter = possibleEncountersData[Random.Range(0, possibleEncountersData.Count)];
        else encounter = encountersData[Random.Range(0, encountersData.Length)];

        return encounter;
    }

    private int GetChallengeLevel()
    {
        int currentRoom = PlayerPrefs.GetInt("CurrentRoom");
        int third = roomsCount / 3;

        float rand = Random.value;
        if (currentRoom <= third)
        {
            if (rand < .6f) return 2;
            else if (rand < .95f) return 3;
            else return 4;
        }
        else if (currentRoom <= third * 2)
        {
            if (rand < .2f) return 3;
            else if (rand < .5f) return 4;
            else if (rand < .8f) return 5;
            else return 6;
        }
        else
        {
            if (rand < .1f) return 6;
            else if (rand < .5f) return 7;
            else if (rand < .7f) return 8;
            else if (rand < .85f) return 9;
            else if (rand < .95f) return 10;
            else return 11;
        }
    }

    public void SetCurrentRoom(int currentRoom)
    {
        PlayerPrefs.SetInt("CurrentRoom", currentRoom);
    }

    public void NextRoom()
    {
        PlayerPrefs.SetInt("CurrentRoom", GetChallengeLevel() + 1);
    }

    private void FixedUpdate()
    {
        if (SceneManager.GetActiveScene().name.Contains("PocketMor")) return;

        if (!isGenerating)
        {
            foreach (Player player in GameplayManager.Instance.players)
            {
                SetPlayerRoom(player);
            }
        }
    }

    void SetPlayerRoom(Player player)
    {
        if (isGenerating) return;

        if (!player) player = FindObjectOfType<Player>();

        if (!player)
        {
            return;
        }

        if (player.currentRoom != null)
        {
            if (player.currentRoom.currentState == RoomEventState.Running)
            {
                return;
            }
        }

        foreach (Room room in rooms)
        {
            Vector2Int playerPosition = player.GetIntPosition();
            if (room.roomPositions.Contains(playerPosition))
            {
                player.currentRoom = room;
                if (room.NeedStartEvent() && !player.IsInGateActivationArea())
                {
                    room.StartRoomEvent();
                }
                return;
            }
        }

        player.currentRoom = null;
    }

    private void ClearSpriteMasks()
	{
        foreach(GameObject mask in masks)
		{
            SafeDestroy(mask);
		}
        masks.Clear();
	}

    private void ResetDungeon()
	{
        PlayerPrefs.SetInt("CurrentRoom", 1);
        ClearMinimap();
        rooms.Clear();
        gates.Clear();
        instantiatedEnemies.Clear();
        possibleEncountersData.Clear();
        tilemapVisualizer.Clear();
        ClearLights();
        ClearSpriteMasks();
        ClearObstacles();
        ClearLists();
        ClearDoors();
        ClearEnemies();
        ClearNPCS();
        ClearLayers();
        ClearFragments();
        ClearChests();

        ClearExternalLayer();
        ClearGatePrevention();
        GameObject bossSpawn = GameObject.FindGameObjectWithTag("BossSpawn");
        if (bossSpawn != null)
            SafeDestroy(bossSpawn);
        GameObject[] remainingIcons = GameObject.FindGameObjectsWithTag("MinimapIcon");
        foreach (var icon in remainingIcons)
        {
            SafeDestroy(icon);
        }
    }

    void ClearChests()
    {
        foreach (var item in chests)
        {
            SafeDestroy(item);
        }
        chests.Clear();
    }

    private void ClearGatePrevention()
    {
        gatePreventionRects.Clear();
    }

    private void ClearMinimap()
    {
        foreach (GameObject minimapObj in minimapObjects)
        {
            SafeDestroy(minimapObj);
        }
    }

    private void ClearFragments()
    {
        GameObject[] allFragments = GameObject.FindGameObjectsWithTag("Fragment");
        foreach (GameObject fragment in allFragments)
        {
            SafeDestroy(fragment);
        }
    }

    public DungeonConfig GetDungeonConfig()
	{
        return config;
	}
}

public enum RewardType
{
    None, Gun, Heal, PowerUp, Memento
}
