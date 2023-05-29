using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGameTools : MonoBehaviour
{
    public static DungeonType currentDungeon;
    public static float elapsedTime;
    public static List<int> defeatedEnemies = new List<int>();
    public static int collectedCoins;
    public static int score;
    
    public static void ResetValues()
	{
        currentDungeon = DungeonType.Lab;
        elapsedTime = 0;
        defeatedEnemies.Clear();
        collectedCoins = 0;
        score = 0;
	}

    public static void AutoSetup()
	{
        currentDungeon = FindObjectOfType<RoomByRoomGenerator>().GetDungeonConfig().type;
        //elapsedTime = GlobalData.Instance.endTime;
	}

    public static void SetCurrentDungeon(DungeonType dungeon)
	{
        currentDungeon = dungeon;
	}

    public static void SetElapsedTime(float time)
	{
        elapsedTime = time;
	}

    public static void SetDefeatedEnemies(List<Player> players)
	{
        for (int i = 0; i < players.Count; i++)
		{
            defeatedEnemies.Add(players[i].GetDefeatedEnemies());
		}
	}

    public static void SetCollectedCoins(int coins)
	{
        collectedCoins = coins;
	}

    public static void SetScore(int value)
	{
        score = value;
	}
}
