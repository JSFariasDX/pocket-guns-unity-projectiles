using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRoomElevatorDoor : MonoBehaviour
{
    [SerializeField] bool openOnTrigger;

    private Animator _animator;
    private List<GameObject> _playerList;

    bool locked = false;

    private void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
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

    public void Open()
	{
        if (!locked)
        {
            _animator.SetBool("IsOpen", true);
        }
    }

    public void Close()
	{
        if (_playerList.Count <= 0)
		{
            _animator.SetBool("IsOpen", false);
		}
    }

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.CompareTag("PlayerCollider"))
		{
			AddPlayerToList(other.gameObject);

			if (!locked && openOnTrigger)
				_animator.SetBool("IsOpen", true);
		}
	}

	private void OnTriggerExit2D(Collider2D other)
	{
		if (other.CompareTag("PlayerCollider"))
		{
			RemovePlayerFromList(other.gameObject);

			if (!locked && openOnTrigger)
			{
				if (_playerList.Count <= 0)
					_animator.SetBool("IsOpen", false);
			}
		}
	}
}
