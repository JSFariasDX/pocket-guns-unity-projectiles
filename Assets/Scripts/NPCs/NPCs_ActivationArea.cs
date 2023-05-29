using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCs_ActivationArea : MonoBehaviour
{
    private List<GameObject> _playerList;

    private NPC _npc;

    private void Awake()
    {
        _npc = GetComponentInParent<NPC>();
        _playerList = new();
    }

    private void AddPlayerToList(GameObject player)
    {
        if (_playerList.Contains(player))
            return;
        
        _playerList.Add(player);
    }

    private void RemovePlayerFromList(GameObject player)
    {
        if (!_playerList.Contains(player))
            return;
        
        _playerList.Remove(player);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerCollider"))
        {
            AddPlayerToList(other.gameObject);

            if (_npc.GetStatus() == NPCStatus.Deactivated)
                _npc.OnActivate();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("PlayerCollider"))
        {
            RemovePlayerFromList(other.gameObject);

            if (_playerList.Count <= 0 && _npc.GetStatus() == NPCStatus.Activated)
                _npc.OnDeactivate();
        }
    }
}
