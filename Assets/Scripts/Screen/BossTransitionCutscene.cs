using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossTransitionCutscene : MonoBehaviour
{
    public bool CanTeleportToBossRoom { get; private set; }
    
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void BossRoomCutscene()
    {
        CanTeleportToBossRoom = false;
        gameObject.SetActive(false);
        gameObject.SetActive(true);
    }

    public void SetTeleportToBossRoom(int value)
    {
        CanTeleportToBossRoom = value != 0 ? true : false;
    }
}
