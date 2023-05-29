using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossActivation : MonoBehaviour
{
    private bool _bossActivated;

    private BossRoom _bossRoom;

    private void Awake()
    {
        _bossRoom = GetComponentInParent<BossRoom>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_bossActivated)
            return;

        if (other.CompareTag("PlayerCollider"))
        {
            _bossRoom.ActivateBoss(other.gameObject);
            _bossActivated = true;
        }
    }
}
