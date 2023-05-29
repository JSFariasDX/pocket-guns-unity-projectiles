using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootOnDeath : MonoBehaviour
{
    [Header("General Settings")]
    [SerializeField] private ShootType shootType;
    [SerializeField] private Projectile projectilePrefab;
    [SerializeField] private float projectileDamage;
    [SerializeField] private float projectileSpeed;

    [Header("Radial Settings")]
    [SerializeField] private int radialCount;
    [SerializeField] private float radialShootRadius;

    private EnemyBehaviourController _controller;

    private void Awake()
    {
        _controller = GetComponent<EnemyBehaviourController>();
    }

    public void Shoot()
	{
        switch (shootType)
        {
            case ShootType.Single: DefaultShoot(); break;
            case ShootType.Radial: RadialShoot(); break;
        }
    }

	private void DefaultShoot()
	{
		bool hasPlayer = _controller.GetTarget() != null;
		if (!hasPlayer)
        {
			return;
        }
		var projectile = GameObject.Instantiate(projectilePrefab, transform.position, Quaternion.identity);
		projectile.Setup(_controller.GetTarget().transform.position, projectileDamage, projectileSpeed, transform);
	}

	private void RadialShoot()
	{
		var target = _controller.GetTarget();
		if (target == null)
			return;

		float angle = Toolkit2D.GetAngleBetweenTwoPoints(transform.position, target.transform.position) - radialShootRadius / 2;
		float angleStep = radialShootRadius / (radialCount - 1);

		Vector2 startPoint = new Vector2(transform.position.x, transform.position.y);

		AudioClip bulletClip = null;

		for (int i = 0; i < radialCount; i++)
		{
			float bulletDirXPosition = startPoint.x + Mathf.Sin((angle * Mathf.PI) / 180) * radialShootRadius;
			float bulletDirYPosition = startPoint.y + Mathf.Cos((angle * Mathf.PI) / 180) * radialShootRadius;

			Vector2 projectileVector = new Vector2(bulletDirXPosition, bulletDirYPosition);
			Vector3 projectileMoveDir = (projectileVector - startPoint).normalized;

			var projectile = GameObject.Instantiate(projectilePrefab, transform.position, Quaternion.identity);
			projectile.Setup(transform.position + projectileMoveDir, projectileDamage, projectileSpeed, transform);

			if (bulletClip == null)
				bulletClip = projectile.GetComponent<AudioSource>().clip;

			angle += angleStep;
		}

		_controller.enemy.audioSource.PlayOneShot(bulletClip);
	}
}
