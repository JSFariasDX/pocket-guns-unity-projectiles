using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    public LoadingScreenManager loadingScreen;

    public AudioClip enterPortalSound;
    [SerializeField]
    public bool isEndRun = false;

    Animator animator;

    public bool IsEndRun { get => isEndRun; set => isEndRun = value; }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        loadingScreen = FindObjectOfType<LoadingScreenManager>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerCollider"))
        {
            if (ScreenManager.currentScreen == Screens.Lobby)
            {
                loadingScreen.OpenLoading();
                MainMenu.StartGame(Difficulty.Easy);

                foreach (var item in GameplayManager.Instance.players)
                {
                    Destroy(item.gameObject);
                }
            }
            else
                CompletedDungeon(other.gameObject);
        }
    }

    public void CompletedDungeon(GameObject other)
    {
        animator.SetTrigger("Close");
        other.GetComponentInParent<Player>().SetAllSpriteRenderers(false);
        GetComponent<AudioSource>().PlayOneShot(enterPortalSound);

        if (SceneManager.GetSceneByName("LabBossRoom").isLoaded)
        {
            SceneManager.UnloadSceneAsync("LabBossRoom");
        }

        if (SceneManager.GetSceneByName("ForestBossRoom").isLoaded)
        {
            SceneManager.UnloadSceneAsync("ForestBossRoom");
        }

        if (SceneManager.GetSceneByName("CaveBossRoom").isLoaded)
        {
            SceneManager.UnloadSceneAsync("CaveBossRoom");
        }

        if (SceneManager.GetSceneByName("GlacierBossRoom").isLoaded)
        {
            SceneManager.UnloadSceneAsync("GlacierBossRoom");
        }

        if (SceneManager.GetSceneByName("SinisterLabBossRoom").isLoaded)
        {
            SceneManager.UnloadSceneAsync("SinisterLabBossRoom");
        }

        if (IsEndRun)
        {
            foreach (var item in GameplayManager.Instance.players)
            {
                item.GetInputController().GetPlayerEntryPanel().GetUnlock().SaveData();
            }

            GlobalData.Instance.EndRun("You Won");
        }
        else
        {
            foreach(Player player in GameplayManager.Instance.GetPlayers(false))
            {
                //player.transform.position = new Vector3(99999, 99999);

                if (player.GetIsDead())
                {
                    player.Revive();
                    continue;
                }

                player.GetHealth().Increase(player.GetHealth().GetMaxHealth() / 2f);
            }
            GameplayManager.Instance.StartNextDungeon();
        }
        
        var unloadAssets = Resources.UnloadUnusedAssets();

        var elevators = FindObjectsOfType<BossRoomElevator>();
        foreach (BossRoomElevator e in elevators)
            Destroy(e.gameObject);

        int dungeons = PlayerPrefs.GetInt("DUNGEONS", 0);
        dungeons++;
        PlayerPrefs.SetInt("DUNGEONS", dungeons);
        
        Destroy(gameObject);
    }
}
