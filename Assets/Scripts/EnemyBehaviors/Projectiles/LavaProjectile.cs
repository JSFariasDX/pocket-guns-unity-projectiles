using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaProjectile : Projectile
{
    [Header("Lava Settings")]
    [SerializeField] private float lavaDamage;

    public override void Setup(Vector3 targetPosition, float damage, float speed, Transform shooterTransform)
    {
        base.Setup(targetPosition, damage, speed, shooterTransform);
        this.damage = lavaDamage;
    }

    public override void OnTrigger()
    {
    }
}
