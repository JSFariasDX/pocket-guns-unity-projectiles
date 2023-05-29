using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunStatusDebugs : MonoBehaviour
{
    [SerializeField] GameObject gunStatusPrefab;
    List<GunDebugs> debugs = new List<GunDebugs>();

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < GameplayManager.Instance.players.Count; i++)
        {
            GenerateGunStatus();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (PauseManager.Instance.IsGamePaused())
        {
            for (int i = 0; i < debugs.Count; i++)
            {
                debugs[i].player = GameplayManager.Instance.players[i];
            }
        }
    }

    void GenerateGunStatus()
    {
        GameObject gunStatus = Instantiate(gunStatusPrefab, transform.position, Quaternion.identity, transform);

        if (!debugs.Contains(gunStatus.GetComponent<GunDebugs>()))
            debugs.Add(gunStatus.GetComponent<GunDebugs>());
    }
}
