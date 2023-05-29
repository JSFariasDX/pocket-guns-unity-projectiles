using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurstGun : Gun
{
    [Header("Burst Settings")]
    public int burstShootCount;
    public float burstShootsInterval;

	protected override IEnumerator Shoot()
	{
		backfire.enabled = true;
		GlobalData.Instance.shoots = GlobalData.Instance.shoots + 1;
		currentBullets--;
		audioSource.Play();
		fireFloorLight.enabled = true;

		for (int i = 0; i < burstShootCount; i++)
		{
			InstantiateBullet();
			yield return new WaitForSeconds(burstShootsInterval);
		} 

		float recoil = bulletForce * basePushback;
		player.OnPushback(recoil);
		SetRumble(0.05f, fireRate);
		yield return new WaitForSeconds(fireRate);

		backfire.enabled = false;
		fireFloorLight.enabled = false;
		isShooting = false;
	}
}
