using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeatPlayer : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        var player = other.CompareTag("PlayerCollider");
        if (!player)
            return;
        
        other.GetComponentInParent<Player>().IsHeating = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        var player = other.CompareTag("PlayerCollider");
        if (!player)
            return;
        
        other.GetComponentInParent<Player>().IsHeating = false;
    }
}
