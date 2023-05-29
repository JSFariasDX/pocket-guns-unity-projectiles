using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FragProjectile : Projectile
{
	[SerializeField] float fragProjSpeed;
	[SerializeField] float fragProjDamage;
	[SerializeField] float fragRadius;
	[SerializeField] int fragsCount;
	[SerializeField] Projectile fragProjPrefab;

	private void OnDestroy()
	{
		RadialShoot(FindObjectOfType<PocketMor>().transform, fragProjSpeed, fragProjDamage, fragRadius, fragsCount);
	}

	private void RadialShoot(Transform target, float speed, float damage, float radius, int shootsCount, float angleMod = 0)
	{
		float angle = Toolkit2D.GetAngleBetweenTwoPoints(transform.position, target.position) - radius / 2;

		angle += angleMod;

		float angleStep = radius / shootsCount;

		Vector2 startPoint = new Vector2(transform.position.x, transform.position.y);

		AudioClip bulletClip = null;

		for (int i = 0; i < shootsCount; i++)
		{
			float bulletDirXPosition = startPoint.x + Mathf.Sin((angle * Mathf.PI) / 180) * radius;
			float bulletDirYPosition = startPoint.y + Mathf.Cos((angle * Mathf.PI) / 180) * radius;

			Vector2 projectileVector = new Vector2(bulletDirXPosition, bulletDirYPosition);
			Vector3 projectileMoveDir = (projectileVector - startPoint).normalized;

			Projectile projectile = GameObject.Instantiate(fragProjPrefab, transform.position, Quaternion.identity);
			projectile.Setup(transform.position + projectileMoveDir * 10, damage, speed, transform);

			projectile.GetComponent<AudioSource>().enabled = false;

			if (bulletClip == null)
				bulletClip = projectile.GetComponent<AudioSource>().clip;

			angle += angleStep;
		}
	}
}
