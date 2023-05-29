using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEnemyOnDeath : MonoBehaviour
{
    [SerializeField] private Enemy enemyToSpawn;

    public void Spawn(Room room)
    {
        var enemy = Instantiate(enemyToSpawn, transform.position, transform.rotation);

        enemy.currentRoom = room;
        enemy.currentRoom.enemies.Add(enemy);
    }
}
