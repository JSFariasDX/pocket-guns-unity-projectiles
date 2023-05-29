using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSpawner : MonoBehaviour
{
    public Transform[] spawnPoints;

    public Transform GetRandomSpawnPoint()
    {
        int sorted = Random.Range(0, spawnPoints.Length);
        return spawnPoints[sorted];
    }
}
