using UnityEngine;

public class FreezingProjectile : Projectile
{
    [Header("Freezing Settings")]
    [SerializeField] private float temperatureChange;

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.root.TryGetComponent<Player>(out Player player))
        {
            if (player.GetHealth().isInvulnerable)
                return;

            player.ChangeTemperature(-temperatureChange);
        }

        base.OnTriggerEnter2D(other);
    }
}
