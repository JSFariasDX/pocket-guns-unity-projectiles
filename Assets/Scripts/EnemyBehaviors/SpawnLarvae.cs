using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnLarvae : MonoBehaviour
{
    [Header("Components")]
    Enemy enemy;

    [Header("Larvae")]
    public GameObject larvaePrefab;
    public float spawnPerMinute;
    float spawnRate;
    float spawnTimer;
    public float spawnAmount = 5;
    public List<Enemy> larvaes = new List<Enemy>();
    public Transform larvaeParent;

    // Start is called before the first frame update
    void Start()
    {
        enemy = GetComponent<Enemy>();

        spawnRate = 1 / (spawnPerMinute / 60);
        spawnTimer = spawnRate * Random.Range(.75f, 1.25f);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (spawnTimer > 0 && GetLarvaeCount() < spawnAmount)
		{
            spawnTimer -= Time.fixedDeltaTime;
		}

        if (spawnTimer < 0) 
        {
            if (GetLarvaeCount() < spawnAmount)
			{
                SpawnLarva();
			}
        }
    }

    int GetLarvaeCount()
	{
        int count = 0;

        foreach(Enemy larvae in larvaes)
		{
            if (larvae) count++;
		}

        return count;
	}

    void SpawnLarva()
    {
        GameObject larvae = Instantiate(larvaePrefab, transform.position, Quaternion.identity);
        larvae.transform.parent = null;
        Health larvaeHealth = larvae.GetComponent<Health>();
        larvaeHealth.toTrack = larvae;
        Enemy e_larvae = larvae.GetComponent<Enemy>();
        e_larvae.SetCurrentRoom(enemy.currentRoom);
        larvaes.Add(e_larvae);

        spawnTimer = spawnRate * Random.Range(.75f, 1.25f);
    }
}
