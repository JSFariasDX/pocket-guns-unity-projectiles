using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadialGun : Gun
{
    [Header("Radial Settings")]
    [HideInInspector] public int radialShootCount;
    [HideInInspector] public float radialShootRadius;

	protected override void InstantiateBullet()
	{
		radialShootCount = gunConfig.bulletAmountPerFire;
		radialShootRadius = gunConfig.radialAngle;

		//Vector2 targetPosition = firepoint.position + (firepoint.position - player.transform.position);
		float angle = Toolkit2D.GetAngleBetweenTwoPoints(firepoint.position, GetShootTargetPosition()) - radialShootRadius / 2;
		angle += Random.Range(-shootInstability, shootInstability);
		float angleStep = radialShootRadius / (radialShootCount - 1);

		Vector2 startPoint = new Vector2(firepoint.position.x, firepoint.position.y);

		for (int i = 0; i < radialShootCount; i++)
		{
			float bulletDirXPosition = startPoint.x + Mathf.Sin((angle * Mathf.PI) / 180) * radialShootRadius;
			float bulletDirYPosition = startPoint.y + Mathf.Cos((angle * Mathf.PI) / 180) * radialShootRadius;

			Vector2 projectileVector = new Vector2(bulletDirXPosition, bulletDirYPosition);
			Vector3 projectileMoveDir = (projectileVector - startPoint).normalized;

			GameObject projectile = Instantiate(bulletPrefab, firepoint.position, Quaternion.identity);

			Bullet bullet = projectile.GetComponent<Bullet>();
			//bullet.Setup(firepoint.position + projectileMoveDir, damage, bulletForce, bulletDistance);
			bullet.Setup(this, firepoint.position + projectileMoveDir, bulletOnHitSolidSound, false);
            // Bullet sprite/color
            //SpriteRenderer bulletSprite = bullet.GetComponentInChildren<SpriteRenderer>();
            //bulletSprite.material.SetColor("_Tint", currentTint);
            //if (originalTint == null)
            //{
            //    originalTint = bs.material.GetColor("_Tint");
            //    currentTint = bs.material.GetColor("_Tint");
            //}
            //bullet.SetDamage(damage);
            //bullet.livingTime = bulletDistance;
            //Rigidbody2D bulletRB = bullet.GetComponent<Rigidbody2D>();
            //bulletRB.AddForce(projectileMoveDir * bulletForce, ForceMode2D.Impulse);

            if (gunConfig.scatteredShots)
            {
                angle += angleStep + Random.Range(-angleStep, angleStep);
            }
            else
            {
                angle += angleStep;
            }
        }
	}
}
