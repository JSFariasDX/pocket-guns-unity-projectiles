using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlamingProjectile : Projectile
{
    [Header("Flaming Settings")]
    [SerializeField] private Projectile lavaProjectile;

    public override void OnTrigger()
    {
        var projectile = Instantiate(lavaProjectile, transform.position, Quaternion.identity);
        projectile.Setup(Vector3.zero, 0f, 0f, shooterTransform);
        base.OnTrigger();
    }
}
