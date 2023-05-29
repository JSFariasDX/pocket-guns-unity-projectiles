using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Room
{
    public CreateRandomWalkDataSO randomWalkData;
    public RoomType roomType;
    public RoomEventState currentState; // ok
    public List<Gate> gates = new List<Gate>();
    public List<GameObject> obstacles = new List<GameObject>(); // ok
    public List<Enemy> enemies = new List<Enemy>();
    public Vector2Int centerPosition;
    [SerializeField] private List<Vector2Int> roomPositionList = new List<Vector2Int>(); // only for inspector view
    public HashSet<Vector2Int> roomPositions = new HashSet<Vector2Int>();
    public GameObject centralLight;
    public bool decidedReward;

    private RoomByRoomGenerator generator;

    public AudioClip clearSound;

    OST roomOST;

    public Room(CreateRandomWalkDataSO randomWalkData, RoomType roomType, Vector2Int centerPosition, HashSet<Vector2Int> roomPositions, RoomByRoomGenerator generator)
    {
        this.randomWalkData = randomWalkData;
        this.roomType = roomType;
        currentState = RoomEventState.NotStarted;
        this.centerPosition = centerPosition;
        this.roomPositions = roomPositions;

        if (this.roomPositions != default)
            roomPositionList = new List<Vector2Int>(roomPositions);

        this.generator = generator;
    }

    public RoomSize GetRoomSize()
	{
        if (roomPositionList.Count < 180) return RoomSize.Small;
        else if (roomPositionList.Count < 306) return RoomSize.Medium;
        else return RoomSize.Big;
	}

    public int GetEnemiesSpawnModifier()
	{
        if (randomWalkData.size == Size.Small) return 1;
        else if (randomWalkData.size == Size.Medium) return 2;
        else return 3;
	}

    public int GetObstaclesCount()
    {
        int positionsCount = roomPositions.Count;

        if (positionsCount >= (int)RoomSizesMinPositions.Big)
        {
            return Random.Range(8, 12);
        }
        else if (positionsCount >= (int)RoomSizesMinPositions.Medium)
        {
            return Random.Range(4, 8);
        }
        else
        {
            return Random.Range(0, 4);
        }
    }

    public Vector2Int GetRandomPoint()
    {
        int sorted = Random.Range(0, roomPositions.Count);
        int i = 0;
        Vector2Int result = Vector2Int.zero;

        foreach (Vector2Int position in roomPositions)
        {
            if (i == sorted)
            {
                result = position;
                return result;
            }
            i++;
        }
        return result;
    }

    public bool NeedStartEvent()
    {
        bool isEnemy = roomType == RoomType.EnemiesDefault || roomType == RoomType.EnemiesSurvival || roomType == RoomType.Boss;
        if (isEnemy && currentState == RoomEventState.NotStarted)
        {
            return true;
        }

        return false;
    }

    public void StartRoomEvent()
	{
        if (generator.isGenerating) return;
        bool isEnemy = roomType == RoomType.EnemiesDefault || roomType == RoomType.Boss;
        if (isEnemy)
        {
            currentState = RoomEventState.Running;
            StartEnemiesDefaultEvent();
        }
        else
		{
            FinishRoom();
		}
    }

    private void StartEnemiesDefaultEvent()
	{
        // Set players pos
        Player mainPlayer = GetPlayersInThisRoom()[0];

        List<Player> players = GameplayManager.Instance.GetPlayers(true);

        for (int i = 0; i < players.Count; i++)
        {
            if(players[i] != mainPlayer)
            {
                players[i].SetPosition(mainPlayer.transform.position);
            }
        }

        if (roomType == RoomType.Boss)
        {
            generator.PlaceBoss(this);
            //MusicManager.Instance.StartBossTheme();
        }
        else
        {
            generator.PlaceEnemiesOnRoom(this);
        }

        //ThemeMusicManager.Instance.PlayAction();
        //ThemeMusicManager.Instance.PlayDanger();

        //Lock all gates
        bool playSound = true;
        foreach (Gate gate in generator.gates)
        {
            if (playSound)
            {
                playSound = false;
            }
            gate.isLocked = true;
            gate.Close(true);
        }
    }

    List<Player> GetPlayersInThisRoom()
    {
        List<Player> players = new List<Player>();
        foreach(Player player in GameplayManager.Instance.GetPlayers(false))
        {
            if (player.currentRoom == this)
            {
                players.Add(player);
            }
        }

        return players;
    }

    public void RemoveEnemyFromList(Enemy enemy)
    {
        enemies.Remove(enemy);

        enemies.RemoveAll(e => e == null);

        if (enemies.Count == 0)
        {
            FinishRoom();
        }
    }

    private void FinishRoom()
    {
        currentState = RoomEventState.Finished;

        //ThemeMusicManager.Instance.StopAction();

        if (!generator) generator = GameObject.FindObjectOfType<RoomByRoomGenerator>();

        if (IsEnemyRoom())
        {
            //ThemeMusicManager.Instance.PlayFX(clearSound);
            //ThemeMusicManager.Instance.StopAction();
            //ThemeMusicManager.Instance.StopDanger();
            MusicManager.Instance.PlaySFX(clearSound);
            generator.SortRoomReward(this, out _);

            bool playSound = true;
            foreach (Gate gate in generator.gates)
            {
                if (playSound)
                {
                    // PLAY ROOM UNLOCK FX
                    playSound = false;
                }
                gate.Unlock();
            }
        }
        else if (roomType == RoomType.Boss)
        {
            //GameObject.FindObjectOfType<TutorialTrigger>().StartTutorial();

            //ThemeMusicManager.Instance.RevertTheme();
            //ThemeMusicManager.Instance.EndAllTracksButMain();
            MusicManager.Instance.StartCorridorTheme();
            generator.InstantiateEgg();
        }

        if (!generator) generator = GameObject.FindObjectOfType<RoomByRoomGenerator>();

        if (!generator)
        {
            return;
        }
		
        int roomsCleared = PlayerPrefs.GetInt("ROOMS", 0);
        roomsCleared++;
        PlayerPrefs.SetInt("ROOMS", roomsCleared);
        GameObject.FindObjectOfType<ClearController>().Clear();

        generator.NextRoom();
    }

    public Vector3 GetRandomFreePosition(Collider2D collider)
	{
        List<Vector3> freePositions = new List<Vector3>();

        foreach(Vector2Int position in roomPositions)
		{
            if (!collider.IsTouchingLayers(LayerMask.GetMask("Obstacles")))
			{
                freePositions.Add(new Vector3(position.x, position.y, 0));
			}
		}

        return freePositions[Random.Range(0, freePositions.Count)];
	}

    public void DestroyObstacles()
	{
        foreach(GameObject obstacle in obstacles)
		{
            GameObject.Destroy(obstacle);
		}
	}

    bool IsEnemyRoom()
    {
        return roomType == RoomType.EnemiesDefault || roomType == RoomType.EnemiesSurvival;
    }

    public void SetOST(OST ost)
    {
        roomOST = ost;
    }

    public OST GetOST()
    {
        return roomOST;
    }
}
#region Enums

public enum RoomEventState
{
    NotStarted, Running, Finished
}

public enum RoomType
{
    PlayerSpawn, Empty, NPC, EnemiesDefault, EnemiesSurvival, Boss
}

public enum RoomSize
{
    Small, Medium, Big
}

#endregion
