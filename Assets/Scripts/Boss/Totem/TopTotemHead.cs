using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopTotemHead : TotemHead
{
	[Header("Top Head Settings")]
	[SerializeField] Transform firePoint;

	[Header("Meteor Settings")]
    [SerializeField] MeteorProjectile meteorProjectilePrefab;
	[SerializeField] int meteorShootsCount;
	[SerializeField] float meteorShootInterval;
	[SerializeField] float meteorShootCooldown;

	[Header("Chase Shot Settings")]
	[SerializeField] ChaseTargetProjectile chaseProjectilePrefab;
	[SerializeField] float chaseShootSpeed;
	[SerializeField] int chaseShootsCount;
	[SerializeField] float chaseShootInterval;
	[SerializeField] float chaseShootCooldown;
	[SerializeField] float chaseProjectileLifeTime = 7;

	[Header("Ricochet Settings")]
	[SerializeField] private Vector2 targetOffset;
	[SerializeField] float moveSpeed;
	[SerializeField] float collideRadius;
	[SerializeField] Transform[] collidePoints;
	[SerializeField] LayerMask collideLayers;
	Vector2 moveDir;
	bool ricochetStarted;

	bool canMeteorShoot;
	bool canChaseShoot;
	bool canRicochet;

	private Collider2D _collider;

	protected override void Awake()
	{
		base.Awake();
		_collider = GetComponentInParent<Collider2D>();
	}

	private void FixedUpdate()
	{
		if (IsAlive() && !IsGoingDown() && !IsRising())
		{
			if (canMeteorShoot && totem.GetCurrentState() == 3)
			{
				StartCoroutine(MeteorShoot());
			}
			
			if (canChaseShoot && totem.GetCurrentState() == 4)
			{
				StartCoroutine(ChaseShoot());
			}

			if (canRicochet && totem.GetCurrentState() == 5)
			{
				totem.transform.Translate(moveDir * moveSpeed * Time.deltaTime);
				Ricochet();
			}
		}
	}

	public void SetCanRicochet(bool value)
	{
		canRicochet = value;
	}

	public override void EnableAttacks()
	{
		totem.BottomHead.ActiveLineLasers(true);

		canMeteorShoot = true;
		canChaseShoot = true;
		canRicochet = true;

		StartCoroutine(MeteorShoot());
	}

	IEnumerator MeteorShoot()
	{
		canMeteorShoot = false;

		StartCoroutine(Spin(meteorShootsCount * meteorShootInterval));
		
		for (int i = 0; i < meteorShootsCount; i++)
		{
			Projectile projectile = Instantiate(meteorProjectilePrefab, firePoint.position, Quaternion.identity);
			projectile.Setup(transform.position, totem.GetEnemy().damage, 0, totem.transform);

			yield return new WaitForSeconds(meteorShootInterval);
		}

		yield return new WaitForSeconds(meteorShootCooldown);
		canMeteorShoot = true;
	}

	IEnumerator ChaseShoot()
	{
		while (true)
		{
			canChaseShoot = false;
			StartCoroutine(Spin(chaseShootsCount * chaseShootInterval));

			for (int i = 0; i < chaseShootsCount; i++)
			{
				Projectile projectile = Instantiate(chaseProjectilePrefab, firePoint.position, Quaternion.identity);
				projectile.Setup(totem.GetRandomPlayer().transform, totem.GetEnemy().damage, chaseShootSpeed, totem.transform);
				Destroy(projectile.gameObject, chaseProjectileLifeTime);

				yield return new WaitForSeconds(chaseShootInterval);
			}

			yield return new WaitForSeconds(chaseShootCooldown);
			canMeteorShoot = true;
		}
	}

	void Ricochet()
	{
		_collider.isTrigger = true;
		animator.SetBool("SpeedSpin", true);
		if (WallColliding() || !ricochetStarted)
		{
			ricochetStarted = true;
			Player target = totem.GetEnemy().GetRandomPlayer();

			var randomDirection = Random.value < 0.5f;
			var randomX = Random.Range(-targetOffset.x, targetOffset.x);
			var randomY = Random.Range(-targetOffset.y, targetOffset.y);

			if (target != null)
			{
				var randomPosition = target.transform.position + new Vector3(randomX, randomY, 0f);
				if (randomDirection)
					moveDir = (randomPosition + Vector3.up) - totem.transform.position;
				else
					moveDir = (target.transform.position + Vector3.up) - totem.transform.position;
					
				moveDir.Normalize();
			}
			else 
			{
				moveDir = Vector2.zero;
				moveSpeed = 0;
			}
			FindObjectOfType<CinemachineImpulseSource>().GenerateImpulse();
		}
	}

	bool WallColliding()
	{
		foreach(Transform point in collidePoints)
		{
			if (Physics2D.OverlapCircle(point.transform.position, collideRadius, collideLayers))
				return true;
		}
		return false;
	}

	public void DestroyBoss()
	{
		totem.DestroyBoss();
	}

	private void OnDrawGizmos()
	{
		foreach (Transform point in collidePoints)
		{
			Gizmos.color = Color.green;
			Gizmos.DrawWireSphere(point.transform.position, collideRadius);
		}
	}
}
