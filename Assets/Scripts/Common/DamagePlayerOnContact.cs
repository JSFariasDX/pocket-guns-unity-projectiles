using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagePlayerOnContact : MonoBehaviour
{
    public float Damage => damage;

    [Header("Settings")]
    [SerializeField] private bool isProjectile;
    [SerializeField] private bool onStay;
    [SerializeField, Min(0f)] private float damage;

    private Collider2D _collider2D;

    private void Awake()
    {
        _collider2D = GetComponent<Collider2D>();
    }

    private void CheckForPlayer(GameObject other)
    {
        if (!other.TryGetComponent<Player>(out Player player))
            return;
            
         DamagePlayer(player);
    }

    private void DamagePlayer(Player player)
    {
        if (damage == 0f || player.GetHealth().isInvulnerable)
            return;

        if (isProjectile)
            player.HandleDamage(_collider2D, true, false);

        else
            player.HandleDamage(_collider2D, false, true);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        CheckForPlayer(other.gameObject);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!onStay)
            return;

        CheckForPlayer(other.gameObject);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        CheckForPlayer(other.gameObject);
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        if (!onStay)
            return;

        CheckForPlayer(other.gameObject);
    }
}
