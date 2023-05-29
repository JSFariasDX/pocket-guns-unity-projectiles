using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwarmController : MonoBehaviour
{
    [Header("Components")]
    Health health;

    [Header("Insects")]
    public GameObject insectPrefab;
    public int amountToSpawn = 10;
    int amountSpawned;
    List<GameObject> myInsects = new List<GameObject>();

    [Header("Spawn")]
    [Range(.05f, 1)] public float spawnInterval = .2f;
    float spawnTimer;
    public float alive;
    float alivePercentage;
    //[Range(0, 1)]
    float soundPercentage = 1;
    float pitchValue;

    [Header("Sound")]
    public AudioClip idleClip;
    AudioSource source;
    float volume = 1;

    // Start is called before the first frame update
    void Start()
    {
        spawnTimer = spawnInterval;
        alive = amountToSpawn;

        health = GetComponentInParent<Health>();

        source = GetComponentInParent<AudioSource>();
        source.clip = idleClip;
        source.loop = true;
        source.volume = 1f;

        pitchValue = Random.Range(.75f, 3);

        source.pitch = pitchValue;
        source.Play();
    }

    // Update is called once per frame
    void Update()
    {
        alive = Mathf.Clamp(alive, 0, amountToSpawn);
        alivePercentage = alive * health.GetCurrentPercentage();

        soundPercentage = health.GetCurrentPercentage();

        source.volume = Mathf.Lerp(.15f, 1, soundPercentage);

        if (amountSpawned < amountToSpawn)
        {
            if (spawnTimer < spawnInterval)
                spawnTimer += Time.deltaTime;
            else
                SpawnInsect();

            //health.isInvulnerable = true;
        }
        else
        {
            for (int i = 0; i < myInsects.Count; i++)
            {
                if(i > alivePercentage)
                {
                    //myInsects[i].GetComponent<InsectController>().Die();
                    //myInsects[i].SetActive(false);
                    GameObject e = myInsects[i];
                    myInsects.Remove(e);
                    e.GetComponent<InsectController>().Die();
                }
            }

            //health.isInvulnerable = false;
        }

        if (PauseManager.Instance.IsGamePaused())
        {
            if(source.isPlaying)
                source.Pause();
        }
        else
        {
            if(!source.isPlaying)
                source.UnPause();
        }
    }

    void SpawnInsect()
    {
        GameObject insect = Instantiate(insectPrefab, transform.position, Quaternion.identity, transform);

        if (!myInsects.Contains(insect))
            myInsects.Add(insect);

        amountSpawned++;
        spawnTimer = 0;
    }
}
