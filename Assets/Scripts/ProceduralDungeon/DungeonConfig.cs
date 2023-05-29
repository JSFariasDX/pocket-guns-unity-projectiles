using System.Collections.Generic;
using UnityEngine;

public enum DungeonType
{
    Lab,
    Forest,
    Cave,
    Glacier,
    Volcano,
    Swamp,
    SinisterLab,
};

[CreateAssetMenu(fileName = "DungeonConfig_", menuName = "PCG/Dungeon Config")]
public class DungeonConfig : ScriptableObject
{
    public DungeonType type = DungeonType.Lab;
    [Header("Tileset and Background")]
    public DungeonPaintAssets paintAsset;
    public Sprite background;
    public Material TilesetMaterial;
    public Material BackgroundMaterial;
    public bool spawnLights;
    public Color32 filterColor = Color.white;
    public string filterSortingLayer = "ColorFilters";

    [Header("Chances")]
    public int minSortedRooms = 5;
    public int maxSortedRooms = 11;
    [Range(0, 100)]
    public int centerTemplateSpawnChance = 40;
    [Range(0, 100)]
    public int cornerBoxesSpawnChance = 80;
    [Range(0, 100)]
    public int primaryOrSecondaryChanceOfPrimary = 30;
    [Range(0, 100)]
    public int roomRewardChance = 20;
    public NpcSpawnSO NpcSpawnChances;

    [Header("Prefabs")]
    public GameObject tankPrefab;
    public float playerLightIntensity = 0.56f;
    public float playerLightRadius = 12f;
    public float lightInnerRadius = 0f;
    public GameObject AmbientLightPrefab;
    public GameObject FloorLightPrefab;
    public GameObject SpotLightPrefab;
    public float distanceFromCenterToDarkSpot = 8f;
    public int lightPaddingFromEachOther = 3;
    public bool PlaceLightsInBossRoom = true;
    public Gate doorPrefab;
    public GameObject BossSpawn;
    public GameObject bossPrefab;
    public GameObject BossElevatorPrefab;
    public Vector2 ElevatorOffset;
    public int bossRoomBaseSize;
    public bool BossRoomIgnoreUpRightCorridors = false;

    [Header("Rooms")]
    public GameObject spriteMaskPrefab;

    [Header("Keys")]
    public GameObject keyChestPrefab;
    public int keysNeeded = 1;
    public List<BossKey> possibleKeys = new List<BossKey>();

    [Header("Room rewards")]
    public List<GameObject> powerUpRewards = new List<GameObject>();
    public List<GameObject> healRewards = new List<GameObject>();
    public List<GameObject> gunRewards = new List<GameObject>();

    [Header("Obstacles")]
    public List<PlaceableObject> primaryBreakableObstacles = new List<PlaceableObject>();
    public List<PlaceableObject> primaryUnbreakableObstacles = new List<PlaceableObject>();
    public List<PlaceableObject> secondaryBreakableObstacles = new List<PlaceableObject>();
    public List<PlaceableObject> roomCenterTemplates = new List<PlaceableObject>();

    [Header("OST and SFX")]
    public OST baseOST;
    public OST bossOST;
    public AudioClip clearRoomSound;
    public AmbientSounds ambientSoundPack;

    [Header("Music")]
    public AudioClip mainClip;
    public AudioClip corridorClip;
    public AudioClip bossClip;

    [Header("VFX")]
    public SpawnParticle enemySpawnParticlePrefab;
    public SpawnParticle rewardSpawnParticle;

    [Header("Enemy Settings")]
    public string enemiesEncountersFolder;
    public List<EnemiesEncounter> possibleEncountersData = new List<EnemiesEncounter>();

    [Header("Exit Portal")]
    public GameObject portalPrefab;

    [SerializeField]
    public bool isLastDungeon = false;

    [Header("Layer Decorations")]
    public List<PlaceableObject> layer1Decorations;
    public List<PlaceableObject> layer2Decorations;
    public List<PlaceableObject> layer3Decorations;
    public List<GameObject> cornerDecorations;
    public List<PlaceableObject> externalDecorations;
    public int externalDecorationCount = 1;
}
