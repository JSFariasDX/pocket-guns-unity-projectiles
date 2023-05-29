using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameCheats : MonoBehaviour
{
    private bool _isInvulnerable;

    private PauseManager _pauseManager;

    private Player _player;
    private RoomByRoomGenerator _generator;

    private GameObject _recentDrop;

    private void Awake()
    {
        _pauseManager = GetComponentInParent<PauseManager>();
    }

    private void Update()
    {
        if (!_pauseManager.IsPauseMenuActive())
            return;
        
        if (Keyboard.current.digit1Key.wasPressedThisFrame)
        {
            ToggleInvincible();
        }
        else if (Keyboard.current.digit2Key.wasPressedThisFrame)
        {
            MaxDamage();
        }
        else if (Keyboard.current.digit3Key.wasPressedThisFrame)
        {
            Add100Coins();
        }
        else if (Keyboard.current.digit4Key.wasPressedThisFrame)
        {
            SpawnWeapon();
        }
        else if (Keyboard.current.digit5Key.wasPressedThisFrame)
        {
            SpawnItem();
        }
        /*else if (Keyboard.current.digit6Key.wasPressedThisFrame)
        {
            SpawnHeal();
        }*/
        else if (Keyboard.current.digit7Key.wasPressedThisFrame)
        {
            TeleportToBoss();
        }
        else if (Keyboard.current.digit8Key.wasPressedThisFrame)
        {
            NextDungeon();
        }
        else if (Keyboard.current.digit9Key.wasPressedThisFrame)
        {
            OpenAllChests();
        }
    }

    private Player GetPlayer()
    {
        if (_player == null)
            _player = FindObjectOfType<Player>();
        
        return _player;
    }

    private Player[] GetPlayers()
    {
        Player[] allPlayers = GameplayManager.Instance.GetPlayers(false).ToArray();
        return allPlayers;
    }

    private RoomByRoomGenerator GetGenerator()
    {
        if (_generator == null)
            _generator = FindObjectOfType<RoomByRoomGenerator>();

        return _generator;
    }

    private void ToggleInvincible()
    {
        var player = GetPlayer();

        _isInvulnerable = !_isInvulnerable;

        player.GetHealth().SetInvulnerability(_isInvulnerable);
        player.GetCurrentPocket().GetHealth().SetInvulnerability(_isInvulnerable);

        ExitPause();
    }

    private void MaxDamage()
    {
        var player = GetPlayer();

        foreach(Gun gun in player.currentGuns)
		{
			gun.damage = 10000;
		}

        ExitPause();
    }

    private void Add100Coins()
    {
        GameplayManager.Instance.AddCoins(100);
        
        ExitPause();
    }

    private void SpawnWeapon()
    {
        SpawnDrop(RewardType.Gun);

        ExitPause();
    }

    private void SpawnItem()
    {
        SpawnDrop(RewardType.PowerUp);

        ExitPause();
    }

    private void SpawnHeal()
    {
        SpawnDrop(RewardType.Heal);

        ExitPause();
    }

    private void TeleportToBoss()
    {
        var generator = GetGenerator();

        var bossGate = generator.BossGate;

        foreach (Player player in GetPlayers())
        {
            player.transform.position = new Vector3(bossGate.transform.position.x, bossGate.transform.position.y, 0);
        }

        ExitPause();
    }

    private void NextDungeon()
    {
        var player = GetPlayer();
        var generator = GetGenerator();
        
        if (player.currentRoom != null)
            player.currentRoom.currentState = RoomEventState.Finished;

        var enemies = FindObjectsOfType<Enemy>();
        foreach (Enemy enemy in enemies)
            Destroy(enemy.gameObject);

        var boss = FindObjectOfType<Boss>(true);
        if (boss != null)
            boss.DestroyBoss();

        ExitPause();

        if (generator.GetDungeonConfig().isLastDungeon)
            GlobalData.Instance.EndRun("You Won");
        else
        {
            GameplayManager.Instance.StartNextDungeon();
            foreach (Player p in GetPlayers())
            {
                //player.transform.position = new Vector3(99999, 99999);
            }
        }

        if (SceneManager.GetSceneByName("LabBossRoom").isLoaded)
        {
            var unloadLab = SceneManager.UnloadSceneAsync("LabBossRoom");
        }

        if (SceneManager.GetSceneByName("ForestBossRoom").isLoaded)
        {
            var unloadForest = SceneManager.UnloadSceneAsync("ForestBossRoom");
        }

        if (SceneManager.GetSceneByName("CaveBossRoom").isLoaded)
        {
            var unloadCave = SceneManager.UnloadSceneAsync("CaveBossRoom");
        }

        if (SceneManager.GetSceneByPath("GlacierBossRoom").isLoaded)
        {
            var unloadGlacier = SceneManager.UnloadSceneAsync("GlacierBossRoom");
        }
        
        var elevators = FindObjectsOfType<BossRoomElevator>();
        foreach (BossRoomElevator e in elevators)
            Destroy(e.gameObject);

        var unloadAssets = Resources.UnloadUnusedAssets();
    }

    void OpenAllChests()
    {
        Chest[] chests = FindObjectsOfType<Chest>();

        foreach (var item in chests)
        {
            item.OpenChest();
        }

        ExitPause();
    }

    private void ExitPause()
    {
        _pauseManager.Resume(null);
    }

    private void SpawnDrop(RewardType rewardType)
    {
        var player = GetPlayer();
        var generator = GetGenerator();

        GlobalData.Instance.AddPercentage(100f);
        generator.SortRoomReward(player.currentRoom, out _recentDrop, rewardType, false, player.transform);
        GlobalData.Instance.ResetPercentage();
    }
}
