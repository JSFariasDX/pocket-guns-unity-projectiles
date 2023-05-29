using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AggressiveStateController
{
    public AggressiveBehaviour behaviourType;
    EnemyBehaviourController controller;
	NavMeshAgent agent;
	private bool _enableLightWhenAggressive;

	private float _buffMultiplier;

	private Coroutine _shootCoroutine;

    public AggressiveStateController(AggressiveBehaviour state, EnemyBehaviourController controller, bool enableLight)
	{
        this.behaviourType = state;
        this.controller = controller;
        agent = controller.agent;
		_enableLightWhenAggressive = enableLight;

		Setup();
	}

	private void Setup()
	{
		canShoot = true;

		dashTime = controller.GetRandomDashResetTime();
		resetActionTime = Random.Range(.75f, 1.25f) * 5;
		_returnToNeutralTime = Random.Range(.75f, 1.25f) * 4;

		_buffMultiplier = 1f;
	}

	public void ActivateOozeBuff(float newMultiplier)
    {
        _buffMultiplier = newMultiplier;
    }

    public void DeactivateOozeBuff(float standardMultiplier)
    {
        _buffMultiplier = standardMultiplier;
    }

	public void Update()
	{
		if (!controller.GetTarget())
		{
			return;
		}

		if (behaviourType == AggressiveBehaviour.Chase) Chase();
		else if (behaviourType == AggressiveBehaviour.ChaseAndSafeShoot) ChaseAndSafeShoot();
		else if (behaviourType == AggressiveBehaviour.ChaseAndShoot) ChaseAndShoot();
		else if (behaviourType == AggressiveBehaviour.RandomMoveAndSafeShoot) RandomMoveAndSafeShoot();
		else if (behaviourType == AggressiveBehaviour.Shoot) OnlyShoot();
		else if (behaviourType == AggressiveBehaviour.ChaseAndTeleport) ChaseAndTeleport();
		else if (behaviourType == AggressiveBehaviour.ChaseTeleportAndShoot) ChaseTeleportAndShoot();
		else if (behaviourType == AggressiveBehaviour.DigPatrolAndShoot) DigPatrolAndShoot();
		else if (behaviourType == AggressiveBehaviour.WobblyChase) WobblyChase();
		else if (behaviourType == AggressiveBehaviour.WobblyChaseDashAndShoot) WobblyChaseDashAndShoot();
		else if (behaviourType == AggressiveBehaviour.WobblySafeChaseAndShoot) WobblySafeChaseAndShoot();
		else if (behaviourType == AggressiveBehaviour.SafeChaseInvincibleAndAction) SafeChaseInvincibleAndAction();
		else if (behaviourType == AggressiveBehaviour.ChaseAndMeleeAttack) ChaseAndMeleeAttack();
		else if (behaviourType == AggressiveBehaviour.DashChaseAndChainShoot) DashChaseAndChainShoot();
	}

	private void Chase()
	{
		if (controller.GetTarget() == null)
        {
			return;
        }

		agent.speed = controller.moveSpeed * _buffMultiplier;
		agent.isStopped = false;
		agent.destination = controller.GetTarget().transform.position;
	}

	private void ChaseAndShoot()
	{
		// Shoot
		if (canShoot)
		{
			_shootCoroutine = controller.StartCoroutine(Shoot(controller.GetTarget().transform, 1 / controller.shootsPerSecond));
			controller.BlinkLight();
		}

		Chase();
	}

	private void ChaseAndSafeShoot()
	{
		// Shoot
		if (canShoot)
		{
			_shootCoroutine = controller.StartCoroutine(Shoot(controller.GetTarget().transform, 1 / controller.shootsPerSecond));
		}

		// Movement controller
		if (controller.GetPlayerDistance() < controller.safeRange.x)
		{
			// Back to safe zone
			agent.speed = controller.moveSpeed * _buffMultiplier;
			agent.isStopped = false;
			bool hasPlayer = controller.GetTarget() != null;
			if (!hasPlayer)
            {
				return;
            }
			Vector3 safeDirection = (controller.GetTarget().transform.position - controller.transform.position).normalized;
			Vector3 safeDestination = controller.transform.position - safeDirection;
			agent.destination = safeDestination;
		}
		else if (controller.GetPlayerDistance() <= controller.safeRange.y)
		{
			agent.isStopped = true;
		}
		else
		{
			// Chase player
			agent.speed = controller.moveSpeed * _buffMultiplier;
			agent.isStopped = false;
			agent.destination = controller.GetTarget().transform.position;
		}
	}

	private void RandomMoveAndSafeShoot()
	{

	}

	private void OnlyShoot()
	{
		agent.speed = 0;
		// Shoot
		if (canShoot)
		{
			_shootCoroutine = controller.StartCoroutine(Shoot(controller.GetTarget().transform, 1 / controller.shootsPerSecond));
			controller.BlinkLight();
		}
	}

	private void ChaseAndTeleport()
	{
		if (controller.canTeleport)
		{
			controller.StartCoroutine(controller.Teleport(controller.GetTarget().transform, controller.prepareTeleportTime, Random.Range(controller.resetTeleportTimeInterval.x, controller.resetTeleportTimeInterval.y)));
		}
		else
		{
			if (!controller.preparingTeleport)
			{
				Chase();
			}
		}
	}

	private void ChaseTeleportAndShoot()
	{
		bool hasPlayer = controller.GetTarget() != null;
		// Shoot
		if (canShoot && !controller.preparingTeleport)
		{
			_shootCoroutine = controller.StartCoroutine(Shoot(controller.GetTarget().transform, 1 / controller.shootsPerSecond));
		}

		ChaseAndTeleport();
	}

	private void WobblyChase()
	{
		agent.destination = controller.GetTarget().transform.position;
		agent.speed = controller.moveSpeed * _buffMultiplier;
		agent.isStopped = false;

		
		Wobbly(GetPlayerDirection());
	}

	bool dashing;
	float dashTimer;
	float dashTime;
	private void WobblyChaseDashAndShoot()
	{
		if (!dashing)
		{
			dashTimer += Time.fixedDeltaTime;
			WobblyChase();

			if (dashTimer > dashTime)
			{
				dashTime = controller.GetRandomDashResetTime();
				controller.StartCoroutine(StartDash(Random.Range(controller.dashDirectionOffset.x, controller.dashDirectionOffset.y), true));
			}
		}
	}

	public void WobblySafeChaseAndShoot()
	{
		// Shoot
		if (canShoot)
		{
			_shootCoroutine = controller.StartCoroutine(Shoot(controller.GetTarget().transform, 1 / controller.shootsPerSecond));
		}

		Wobbly(GetPlayerDirection());

		// Movement controller
		if (controller.GetPlayerDistance() < controller.safeRange.x)
		{
			// Back to safe zone
			agent.speed = controller.moveSpeed * _buffMultiplier;
			agent.isStopped = false;
			bool hasPlayer = controller.GetTarget() != null;
			if (!hasPlayer)
			{
				return;
			}
			Vector3 safeDirection = (controller.GetTarget().transform.position - controller.transform.position).normalized;
			Vector3 safeDestination = controller.transform.position - safeDirection;
			agent.destination = safeDestination;
		}
		else if (controller.GetPlayerDistance() <= controller.safeRange.y)
		{
			agent.isStopped = true;
		}
		else
		{
			// Chase player
			agent.speed = controller.moveSpeed * _buffMultiplier;
			agent.isStopped = false;
			agent.destination = controller.GetTarget().transform.position;
		}
	}

	float actionTimer;
	float resetActionTime;
	float _returnTimer;
	float _returnToNeutralTime;

	private void SafeChaseInvincibleAndAction()
	{		
		if (controller.enemy.GetHealth().IsDamageEnabled() && !controller.IsActing())
		{
			ChaseAndSafeShoot();
		}

		if (actionTimer < resetActionTime)
		{
			actionTimer += Time.deltaTime;
			return;
		}

		if (actionTimer != 0f && controller.enemy.GetHealth().IsDamageEnabled())
		{
			agent.isStopped = true;
			controller.animator.SetBool("Invincible", true);
			controller.enemy.GetHealth().SetDamageEnabled(false);
			controller.SetActionEnabled(true);
			resetActionTime = Random.Range(.75f, 1.25f) * 5;
		}

		if (_returnTimer < _returnToNeutralTime && !controller.IsActing())
		{
			_returnTimer += Time.deltaTime;
			return;
		}

		if (_returnTimer != 0f || controller.IsActing())
		{
			if (_shootCoroutine != null)
				controller.StopCoroutine(_shootCoroutine);

			agent.speed = controller.moveSpeed * _buffMultiplier;
			agent.isStopped = false;
			controller.animator.SetBool("Invincible", false);
			controller.enemy.GetHealth().SetDamageEnabled(true);
			actionTimer = 0f;
			_returnTimer = 0f;
			controller.SetActionEnabled(false);
			_returnToNeutralTime = Random.Range(.75f, 1.25f) * 5;
		}
	}

	float stoppedTime;
	private void DigPatrolAndShoot()
	{
		if (agent.velocity.magnitude <= .5f && !agent.isStopped) stoppedTime += Time.deltaTime;

		if (controller.agent.remainingDistance < .25f || (stoppedTime > 1 && !agent.isStopped))
		{
			stoppedTime = 0;
			controller.StartCoroutine(RestartDigPatrolAndShoot());
		}
	}

	bool patrolRestarted;
	bool canAttack;
	private IEnumerator RestartDigPatrolAndShoot()
	{
		if (!patrolRestarted)
		{
			// EMERGE ATTACK
			patrolRestarted = true;
			agent.isStopped = true;
			controller.animator.SetTrigger("JumperAttack");
			controller.animator.SetTrigger("Attack");
			controller.EnableMovementVfxEmission(false);
			controller.enemy.SetInvulnerable(false);
			EmergeAttack();

			yield return new WaitForSeconds(.5f);

			if (canAttack) 
				_shootCoroutine = controller.StartCoroutine(Shoot(controller.GetTarget().transform, 1 / controller.shootsPerSecond));

			yield return new WaitForSeconds(controller.GetPatrolStopTime());

			// DIG
			canAttack = true;
			patrolRestarted = false;
			agent.speed = controller.moveSpeed * _buffMultiplier;
			agent.isStopped = false;
			controller.animator.SetTrigger("Dig");
			controller.enemy.SetInvulnerable(true);

			controller.enemy.audioSource.PlayOneShot(controller.digSound);
			agent.destination = controller.GetRandomNavMeshPoint(controller.GetTarget().transform, controller.patrolRange);
			controller.EnableMovementVfxEmission(true);
		}
	}

	private void EmergeAttack()
	{
		if (!controller.attackOnEmerge)
            return;
        
        var hasPlayer = controller.GetTarget() != null;
		if (!hasPlayer && controller.followTarget)
			return;
        
		var angle = controller.followTarget ? Toolkit2D.GetAngleBetweenTwoPoints(controller.transform.position, controller.GetTarget().transform.position) - controller.emergeRadialShootRadius / 2 : controller.defaultRotation;
		var angleStep = controller.emergeRadialShootRadius / (controller.emergeRadialCount - 1);

		var startPoint = controller.transform.position;

		for (int i = 0; i < controller.emergeRadialCount; i++)
		{
			float bulletDirXPosition = startPoint.x + Mathf.Sin((angle * Mathf.PI) / 180) * controller.emergeRadialShootRadius;
			float bulletDirYPosition = startPoint.y + Mathf.Cos((angle * Mathf.PI) / 180) * controller.emergeRadialShootRadius;

			var projectileVector = new Vector2(bulletDirXPosition, bulletDirYPosition);
			var projectileMoveDir = (projectileVector - (Vector2)startPoint).normalized;

			var projectile = GameObject.Instantiate(controller.emergeProjectile, (Vector2)controller.transform.position + (projectileMoveDir * controller.emergeProjSpawnOffset), Quaternion.identity);
			projectile.Setup((Vector2)controller.transform.position + projectileMoveDir, controller.emergeProjDamage, controller.emergeProjSpeed, controller.transform);

			angle += angleStep;
		}

		controller.enemy.audioSource.PlayOneShot(controller.emergeAttackClip);
	}

	private void ChaseAndMeleeAttack()
    {
		Transform target = controller.GetTarget().transform;
		agent.destination = target.position;

		agent.speed = controller.moveSpeed * _buffMultiplier;
		controller.agent.isStopped = controller.animator.GetCurrentAnimatorStateInfo(0).IsTag("Stopped");

		if (agent.destination.x < controller.transform.position.x) controller.enemy.GetSpriteRenderer().flipX = true;
		else if (agent.destination.x > controller.transform.position.x) controller.enemy.GetSpriteRenderer().flipX = false;

		if (Vector2.Distance(controller.transform.position, target.position) < .5f)
        {
			controller.TriggerAttackAnimation();
			controller.BlinkLight();
        }
    }

	private void DashChaseAndChainShoot()
    {
		if (!dashing)
		{
			dashTimer += Time.fixedDeltaTime;

			if (dashTimer > dashTime)
			{
				dashTime = controller.GetRandomDashResetTime();
				controller.StartCoroutine(StartDash(Random.Range(controller.dashDirectionOffset.x, controller.dashDirectionOffset.y), true));
			}
		}
	}

	#region Tools
	private Vector3 GetPlayerDirection()
	{
		Vector3 axis = controller.GetTarget().transform.position - controller.transform.position;
		axis.Normalize();
		return axis;
	}

	private void Wobbly(Vector3 axis)
	{
		float sin = Mathf.Sin(90 * Mathf.Deg2Rad);
		float cos = Mathf.Cos(90 * Mathf.Deg2Rad);
		float tx = axis.x;
		float ty = axis.y;
		axis.x = (cos * tx) - (sin * ty);
		axis.y = (sin * tx) + (cos * ty);

		// Wobbly
		if (controller.wobblingEnabled)
		{
			controller.transform.position = controller.transform.position + axis * Mathf.Sin(Time.time * controller.zigZagSpeed) * controller.zigZagMagnitude / 100;
		}
	}

	private IEnumerator StartDash(float angle, bool shoot)
	{
		// Get dash direction
		Vector3 axis = controller.GetTarget().transform.position - controller.transform.position;
		axis.Normalize();
		float sin = Mathf.Sin(angle * Mathf.Deg2Rad);
		float cos = Mathf.Cos(angle * Mathf.Deg2Rad);
		float tx = axis.x;
		float ty = axis.y;
		axis.x = (cos * tx) - (sin * ty);
		axis.y = (sin * tx) + (cos * ty);
		agent.isStopped = true;

		float dashSpeed = controller.dashSpeed / 10;
		float dashTime = controller.dashTime / 10;
		float iterationInterval = .01f;
		float iterations = dashTime / iterationInterval;

		dashing = true;
		controller.enemy.GetHealth().SetInvulnerability(true);
		controller.enemy.SetSpriteRendererAlpha(.5f);
		controller.EnableMovementVfxEmission(true);
		Vector3 scale = controller.transform.localScale;
		controller.transform.localScale = new Vector3(controller.transform.localScale.x, controller.transform.localScale.y * .7f, controller.transform.localScale.z);

		for (int i = 0; i < iterations; i++)
		{
			controller.transform.Translate(axis * dashSpeed);
			yield return new WaitForSeconds(iterationInterval);
		}

		controller.enemy.SetSpriteRendererAlpha(1);
		controller.transform.localScale = scale;
		controller.EnableMovementVfxEmission(false);
		controller.enemy.GetHealth().SetInvulnerability(false);

		if (shoot)
		{
			yield return new WaitForSeconds(.25f);

			_shootCoroutine = controller.StartCoroutine(Shoot(controller.GetTarget().transform, 0));

			yield return new WaitForSeconds(.5f);
		}

		dashTimer = 0;
		dashing = false;
		agent.isStopped = false;
	}

	bool canShoot;
	public IEnumerator Shoot(Transform target, float shootResetTime)
	{
		if (controller.shootType == ShootType.None)
			yield break;
		
		canShoot = false;

		yield return new WaitForSeconds((shootResetTime * Random.Range(.7f, 1.3f) / 2) / _buffMultiplier);

		controller.enemy.PlayAttackSound();
		controller.TriggerAttackAnimation();

		if (_enableLightWhenAggressive)
		{
			if (controller.TryGetComponent<EnemyLight>(out EnemyLight light))
				light.BlinkLight(false);
		}

		if (controller.shootType == ShootType.Single) DefaultShoot(target);
		else if (controller.shootType == ShootType.Radial) RadialShoot(target);
		else if (controller.shootType == ShootType.DoubleRadial) DoubleRadial(target);
		else if (controller.shootType == ShootType.Burst) controller.StartCoroutine(BurstShoot(target));
		else if (controller.shootType == ShootType.RadialBurst) controller.StartCoroutine(BurstRadialShoot(target));
		else if (controller.shootType == ShootType.RandomShoot) RandomShoot();
		else if (controller.shootType == ShootType.OrbitalChainShoot) controller.StartCoroutine(OrbitalChainShoot(target));

		yield return new WaitForSeconds(shootResetTime * Random.Range(.7f, 1.3f) / 2);

		canShoot = true;
	}

	private void DefaultShoot(Transform target)
	{
		bool hasPlayer = controller.GetTarget() != null;
		if (!hasPlayer)
        {
			return;
        }

		var randomX = Random.Range(-controller.targetOffset.x, controller.targetOffset.x);
        var randomY = Random.Range(-controller.targetOffset.y, controller.targetOffset.y);
        var offset = new Vector3(randomX, randomY, 0f);

		GameObject projectile = GameObject.Instantiate(controller.projectilePrefab, controller.projectileSpawnPoint.position, Quaternion.identity);
		projectile.GetComponent<Projectile>().Setup(controller.GetTarget().transform.position + offset, controller.projectileDamage, controller.projectileSpeed * _buffMultiplier, controller.transform);
	}

	private void RadialShoot(Transform target, bool invertTarget = false)
	{
		bool hasPlayer = controller.GetTarget() != null;
		if (!hasPlayer)
		{
			return;
		}
		float angle = Toolkit2D.GetAngleBetweenTwoPoints(controller.projectileSpawnPoint.position, target.position) - controller.radialShootRadius / 2;

		if (invertTarget)
			angle += 180f;

		float angleStep = controller.radialShootRadius / (controller.radialCount - 1);

		Vector2 startPoint = new Vector2(controller.projectileSpawnPoint.position.x, controller.projectileSpawnPoint.position.y);

		AudioClip bulletClip = null;

		for (int i = 0; i < controller.radialCount; i++)
		{
			float bulletDirXPosition = startPoint.x + Mathf.Sin((angle * Mathf.PI) / 180) * controller.radialShootRadius;
			float bulletDirYPosition = startPoint.y + Mathf.Cos((angle * Mathf.PI) / 180) * controller.radialShootRadius;

			Vector2 projectileVector = new Vector2(bulletDirXPosition, bulletDirYPosition);
			Vector3 projectileMoveDir = (projectileVector - startPoint).normalized;

			GameObject projectile = GameObject.Instantiate(controller.projectilePrefab, controller.projectileSpawnPoint.position, Quaternion.identity);
			projectile.GetComponent<Projectile>().Setup(controller.transform.position + projectileMoveDir * 10, controller.projectileDamage, controller.projectileSpeed * _buffMultiplier, controller.transform);

			projectile.GetComponent<AudioSource>().enabled = false;

			if (bulletClip == null)
				bulletClip = projectile.GetComponent<AudioSource>().clip;

			angle += angleStep;
		}

		controller.enemy.audioSource.PlayOneShot(bulletClip);
	}

	private void DoubleRadial(Transform target)
	{
		RadialShoot(target);
		RadialShoot(target, true);
	}

	private IEnumerator BurstShoot(Transform target)
	{
		for (int i = 0; i < controller.burstCount; i++)
		{
			DefaultShoot(target);
			yield return new WaitForSeconds(controller.burstShootInterval);
		}
	}

	private IEnumerator BurstRadialShoot(Transform target)
	{
		for (int i = 0; i < controller.burstCount; i++)
		{
			RadialShoot(target);
			yield return new WaitForSeconds(controller.burstShootInterval);
		}
	}

	private IEnumerator OrbitalChainShoot(Transform target)
	{
		for (int i = 0; i < controller.burstCount; i++)
		{
			bool hasPlayer = controller.GetTarget() != null;
			var targetDirection = controller.targetPlayer ? controller.GetTarget().transform.position : controller.transform.position - controller.transform.up;
			if (hasPlayer)
			{
				GameObject projectile = GameObject.Instantiate(controller.projectilePrefab, controller.projectileSpawnPoint.position, Quaternion.identity);
				OrbitalProjectile orbitalProjectile = projectile.GetComponent<OrbitalProjectile>();
				orbitalProjectile.Setup(targetDirection, controller.projectileDamage, controller.projectileSpeed * _buffMultiplier, controller.transform);
				orbitalProjectile.DecreaseMaxDistance((orbitalProjectile.GetMaxDistance() - .5f) / controller.burstCount * i);
				orbitalProjectile.SetAutoDestroyMultiplier(orbitalProjectile.GetMaxDistance());
				yield return new WaitForSeconds(controller.burstShootInterval);
			}
		}
	}


	private void RandomShoot()
	{
		int count = controller.GetRandomShootCount();

		AudioClip bulletClip = null;

		for (int i = 0; i < count; i++)
		{
			GameObject projectile = GameObject.Instantiate(controller.projectilePrefab, controller.projectileSpawnPoint.position, Quaternion.identity);
			Vector3 targetDirection = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0).normalized;
			Vector3 targetPos = projectile.transform.position + targetDirection;
			projectile.GetComponent<Projectile>().Setup(targetPos, controller.projectileDamage, controller.projectileSpeed * _buffMultiplier, controller.transform);

			if (bulletClip == null)
				bulletClip = projectile.GetComponent<AudioSource>().clip;
		}

		controller.enemy.audioSource.PlayOneShot(bulletClip);
	}

	private IEnumerator ResetTeleport(float resetTeleportTime)
	{
		yield return new WaitForSeconds(resetTeleportTime);
		controller.canTeleport = true;
	}

	#endregion
}

public enum AggressiveBehaviour
{
    Chase, 
	ChaseAndShoot, 
	ChaseAndSafeShoot, 
	RandomMoveAndSafeShoot, 
	Shoot, 
	ChaseAndTeleport, 
	ChaseTeleportAndShoot, 
	DigPatrolAndShoot, 
	WobblyChase, 
	WobblyChaseDashAndShoot, 
	WobblySafeChaseAndShoot,
	SafeChaseInvincibleAndAction,
	ChaseAndMeleeAttack, // Active collider only in attack animation
	DashChaseAndChainShoot,
}