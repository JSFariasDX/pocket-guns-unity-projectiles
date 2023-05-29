using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyController : MonoBehaviour
{
    [Header("Player Spawn Points")]
    public List<Transform> playerSpawns = new List<Transform>();
    [Range(0, 1)]
    public float randomThreshold = 1;

    [Header("Pockets Spawn")]
    public Transform pocketSpawnArea;
    bool spawned = false;

    [Header("Enable stuff")]
    public List<GameObject> disabledStuff = new List<GameObject>();

    // Start is called before the first frame update
    public void StartGame()
    {
        List<Player> players = GameplayManager.Instance.GetPlayers(false);

        for (int i = 0; i < players.Count; i++)
        {
            for (int j = 0; j < playerSpawns.Count; j++)
            {
                if (playerSpawns[j].name.Contains(players[i].characterName))
                {
                    Vector2 newPos = playerSpawns[j].position + new Vector3(Random.Range(-randomThreshold, randomThreshold), Random.Range(-randomThreshold, randomThreshold), 0);

                    players[i].SetPosition(newPos);
                    //players[i].GetInputController().SetMapInput("Gameplay");
                }
            }

            players[i].playerLight.gameObject.SetActive(false);
        }

        for (int i = 0; i < disabledStuff.Count; i++)
        {
            disabledStuff[i].SetActive(true);
        }

        //PauseManager.Instance.SimpleResume();
    }

    public void SpawnPockets(Pocket current)
    {
        if (spawned) return;

        List<Player> players = GameplayManager.Instance.GetPlayers(false);

        for (int i = 0; i < players[0].entryPanel.unlockedPockets.Count; i++)
        {
            GameObject pocket = Instantiate(players[0].entryPanel.unlockedPockets[i].pocket.gameObject, new Vector3(pocketSpawnArea.position.x + Random.Range(-10, 10), pocketSpawnArea.position.y + Random.Range(-10, 10), 0), Quaternion.identity);
            pocket.GetComponent<Pocket>().level = players[0].entryPanel.unlockedPockets[i].pocketLevel;
            pocket.GetComponent<Pocket>().pocketType = pocket.GetComponent<Pocket>().level == 0 ? PetType.Egg : PetType.Default;
            pocket.GetComponent<Special>().enabled = false;
        }

        spawned = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
