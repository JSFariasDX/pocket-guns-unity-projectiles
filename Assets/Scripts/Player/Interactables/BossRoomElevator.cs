using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BossRoomElevator : Interactable
{
    [Header("Teleport Settings")]
    [SerializeField] private string sceneName;
    [SerializeField] private bool isOut;
    [SerializeField] private Transform outPosition;

    [Header("Interact Settings")]
    [SerializeField] private string teleportName;
    [SerializeField] private string teleportDescription;
    [SerializeField] BossRoomElevatorDoor elevatorDoor;
    [SerializeField] GameObject keyHoleObject;
    [SerializeField] Collider2D lockCollider;
    [SerializeField] GameObject unlockEffect;
    [SerializeField] Collider2D startBossCollider;
    [SerializeField] GameObject popup;
    [SerializeField] Transform startBossPopupPosition;

    [Header("Merchant")]
    public GameObject merchant;
    bool showMerchant = true;

    [Header("SFX")]
    public AudioClip enterPortalSound;
    [SerializeField] AudioClip withoutKeyClip;
    [SerializeField] AudioClip unlockingClip;
    AudioSource _audioSource;

    private BossTransitionCutscene _cutscene;
    private bool _bossSceneLoaded;
    private bool _teleportedStarted;

    [Header("Keys")]
    public bool locked = false;

    private void Awake()
    {
        _cutscene = FindObjectOfType<BossTransitionCutscene>(true);
        _audioSource = GetComponent<AudioSource>();
    }

    protected override void Start()
    {
        base.Start();

        SetName("Unlock");
        SetDescription(teleportDescription);

        if (merchant)
        {
            for (int i = 0; i < GameplayManager.Instance.players.Count; i++)
            {
                if (GameplayManager.Instance.players[i].characterName.Contains("Irwin"))
                    showMerchant = false;
            }

            merchant.SetActive(showMerchant);
        }
    }

    public override void OnInteract(Player player)
    {
        if (locked)
        {
            SetName("Entrance is Locked");
            SetDescription("You will need " + GameplayManager.Instance.keysNeeded + " keys to open");

            // Display locked message
            if (GameplayManager.Instance.TotalKeys >= GameplayManager.Instance.keysNeeded)
            {
                // Unlock door
                GameplayManager.Instance.UseKeys(GameplayManager.Instance.keysNeeded);
                FindObjectOfType<CoinGui>().RemoveKeys();
                SetName("Entrance Unlocked");
                SetDescription(teleportDescription);

                popup.transform.position = startBossPopupPosition.position;
                lockCollider.enabled = false;
                startBossCollider.enabled = true;
                elevatorDoor.Open();
                Destroy(Instantiate(unlockEffect, keyHoleObject.transform.position, Quaternion.identity), 1);
                Destroy(keyHoleObject.gameObject);

                if (_audioSource.isPlaying)
                {
                    _audioSource.Stop();
                }
                _audioSource.PlayOneShot(unlockingClip);

                locked = false;
            }
            else
            {
                // Door is locked
                if (!_audioSource.isPlaying)
                {
                    _audioSource.PlayOneShot(withoutKeyClip);
                }
            }
        }
        else
        {
            GoToBossRoom();
            SetDescription(teleportDescription);
        }
    }

    private void TeleportPlayers(Vector3 newPosition)
    {
        foreach(Player player in GameplayManager.Instance.GetPlayers(false))
        {
            player.transform.position = newPosition;
        }
    }

    private BossRoomElevator GetOtherElevator(bool other)
    {
        BossRoomElevator elevator = null;
        foreach (BossRoomElevator e in FindObjectsOfType<BossRoomElevator>())
        {
            if (e.isOut == !other)
            {
                elevator = e;
                break;
            }
        }

        return elevator;
    }

    public void GoToBossRoom()
    {
        if (_teleportedStarted)
            return;

        _teleportedStarted = true;
        StartCoroutine(BossRoomTransition());
    }

    private IEnumerator BossRoomTransition()
    {
        _cutscene.BossRoomCutscene();

        yield return new WaitUntil(() => _cutscene.CanTeleportToBossRoom);

        if (!_bossSceneLoaded && !isOut)
        {
            SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
            GetComponent<AudioSource>().PlayOneShot(enterPortalSound);
            MusicManager.Instance.StartCorridorTheme();
            _bossSceneLoaded = true;
            _teleportedStarted = false;
            yield break;
        }

        if (isOut)
        {
            TeleportPlayers(GetOtherElevator(isOut).outPosition.position);
            MusicManager.Instance.StartMainTheme();
        }
        else
            TeleportPlayers(FindObjectOfType<BossRoom>().PlayerSpawn.position);

        GetComponent<AudioSource>().PlayOneShot(enterPortalSound);
        _teleportedStarted = false;
    }
}
