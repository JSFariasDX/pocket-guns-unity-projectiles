using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MidTotemHead : TotemHead
{
	[SerializeField] Transform firePoint;

	[Header("Burst")]
	[SerializeField] float projectileDamage;
	[SerializeField] float projectileSpeed;
    [SerializeField] int shootsCount;
    [SerializeField] float shootInterval;
    [SerializeField] float burstCooldown;
	[SerializeField] Projectile projectilePrefab;
	bool canShoot;

	[Header("Shockwave")]
	[SerializeField] float shockwaveDamage;
	[SerializeField] float shockwaveCooldown;
	[SerializeField] ShockwaveController shockwavePrefab;
	bool canShockwave;

	private void FixedUpdate()
	{
		if (IsAlive() && !IsGoingDown() && !IsRising())
		{
			if (canShoot && totem.GetCurrentState() < 4)
			{
				StartCoroutine(BurstShoot(totem.GetRandomPlayer().transform));
			}

			if (canShockwave && totem.GetCurrentState() == 4)
			{
				StartCoroutine(Shockwave());
			}
		}
	}

	public override void EnableAttacks()
	{
		canShoot = true;
		canShockwave = true;
		StartCoroutine(BurstShoot(totem.GetRandomPlayer().transform));
	}

	private IEnumerator BurstShoot(Transform target)
	{
		canShoot = false;
		StartCoroutine(Spin(shootInterval * shootsCount));

		for (int i = 0; i < shootsCount; i++)
		{
			if (totem.GetCurrentState() < 4) {
				Projectile projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
				projectile.Setup(target.position, projectileDamage, projectileSpeed, totem.transform);

				yield return new WaitForSeconds(shootInterval);
			}
		}

		yield return new WaitForSeconds(burstCooldown);
		canShoot = true;
	}

	private IEnumerator Shockwave()
	{
		canShockwave = false;
		StartCoroutine(Spin(.1f));

		yield return new WaitForSeconds(.2f);

		ShockwaveController shockwave = Instantiate(shockwavePrefab, firePoint.position, Quaternion.identity);
		shockwave.ResetEvent();

		yield return new WaitForSeconds(shockwaveCooldown);
		canShockwave = true;
	}
}
