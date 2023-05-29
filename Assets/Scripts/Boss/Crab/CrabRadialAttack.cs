using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrabRadialAttack : MonoBehaviour
{
    public bool CanPerformAttack { get; set; }
    
    [Header("Attack5 Settings")]
    [SerializeField] private Projectile projectilePrefab;
    [SerializeField] private Transform shootPoint;
    [SerializeField] private float damage;
    [SerializeField] private float speed;
    [SerializeField] private int count;
    [SerializeField] private int shootRadius;

    [Header("Sound Settings")]
    [SerializeField] private AudioClip attackClip;

    private Crab _crab;

    private void Awake()
    {
        _crab = GetComponentInParent<Crab>();
    }

    public void RadialShoot()
    {
        var target = _crab.GetEnemy().GetTarget();
		if (target == null)
			return;

		float angle = Toolkit2D.GetAngleBetweenTwoPoints(shootPoint.position, target.position) - shootRadius / 2;

		float angleStep = shootRadius / (count - 1);

		Vector2 startPoint = new Vector2(shootPoint.position.x, shootPoint.position.y);

        _crab.AudioSource.PlayOneShot(attackClip);

		for (int i = 0; i < count; i++)
		{
			float bulletDirXPosition = startPoint.x + Mathf.Sin((angle * Mathf.PI) / 180) * shootRadius;
			float bulletDirYPosition = startPoint.y + Mathf.Cos((angle * Mathf.PI) / 180) * shootRadius;

			Vector2 projectileVector = new Vector2(bulletDirXPosition, bulletDirYPosition);
			Vector3 projectileMoveDir = (projectileVector - startPoint).normalized;

			var projectile = GameObject.Instantiate(projectilePrefab, shootPoint.position, Quaternion.identity);
			projectile.Setup(transform.position + projectileMoveDir * 10, damage, speed, transform.parent);

			angle += angleStep;
		}
    }
}
