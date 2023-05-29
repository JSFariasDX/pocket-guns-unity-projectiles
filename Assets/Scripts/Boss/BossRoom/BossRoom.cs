using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRoom : MonoBehaviour
{
    [field: SerializeField] public Transform PlayerSpawn { get; private set; }

    [SerializeField] private OST bossOST;
    [SerializeField] private Gate bossGate;

    [Header("Minimap Icons")]
    [SerializeField] private GameObject bossIcon;
    [SerializeField] private GameObject finishedIcon;

    private RoomByRoomGenerator _generator;
    private Room _room;
    AudioSource _audioSource;
    
    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();

        var allPockets = FindObjectsOfType<Pocket>();

        foreach (Pocket pocket in allPockets)
        {
            if (pocket.GetPlayer() != null)
                continue;
            
            pocket.gameObject.SetActive(false);
        }

        _generator = FindObjectOfType<RoomByRoomGenerator>();
        _room = new Room(null, RoomType.Boss, default, null, _generator);

        foreach(Player player in GameplayManager.Instance.GetPlayers(true))
        {
            player.transform.position = PlayerSpawn.position;
        }
    }

    public void ActivateBoss(GameObject other)
    {
        _generator.gates.Add(bossGate);
        //_room.SetOST(bossOST);
        //MusicManager.Instance.StartCorridorTheme();

        other.GetComponentInParent<Player>().currentRoom = _room;

        _room.StartRoomEvent();

        _audioSource.Stop();
    }

    public void FinishRoom()
    {
        bossIcon.SetActive(false);
        finishedIcon.SetActive(true);
    }
}
