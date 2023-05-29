using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyEncounter", menuName = "Enemy Encounter")]
public class EnemiesEncounter : ScriptableObject
{
    [SerializeField] public bool enabled;
    [SerializeField] public int challengeLevel;
    [SerializeField] public List<EnemyEncounter> encounters = new List<EnemyEncounter>();

    public int GetEnemiesCount()
	{
        int enemiesCount = 0;

        foreach(EnemyEncounter encounter in encounters)
		{
            enemiesCount += encounter.enemyCount;
		}

        return enemiesCount;
	}
}

[System.Serializable]
public struct EnemyEncounter
{
    [SerializeField] public Enemy enemyPrefab;
    [SerializeField] public int enemyCount;
}

