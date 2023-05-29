using System.Collections.Generic;
using UnityEngine;

public class SprayProjectile : Projectile
{
    [Header("Spray Settings")]
    [SerializeField] private List<Projectile> sprayShots;
    [SerializeField] private Vector2 targetOffset;

    [Header("Distance Settings")]
    [SerializeField] private float closeDistance;
    [SerializeField] private float closeOffsetWeight;

    public override void Setup(Vector3 targetPosition, float damage, float speed, Transform shooterTransform)
    {
        foreach (Projectile shot in sprayShots)
        {
            var randomX = Random.Range(-targetOffset.x, targetOffset.x);
            var randomY = Random.Range(-targetOffset.y, targetOffset.y);
            var offset = new Vector3(randomX, randomY, 0f);

            var distanceToTarget = Vector2.Distance(transform.position, targetPosition);

            if (distanceToTarget <= closeDistance)
                offset *= closeOffsetWeight;

            shot.Setup(targetPosition + offset, damage, speed, shooterTransform);
            shot.transform.SetParent(null);
        }

        Destroy(gameObject, 1f);
    }
}
