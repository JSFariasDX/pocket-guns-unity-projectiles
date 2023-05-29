using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PocketMor : Boss
{
	float currentMoveSpeed;
	[SerializeField] float moveSpeed;
	[SerializeField] List<Transform> pocketMorPositions = new();

	[SerializeField] Canvas hpCanvas;
	[SerializeField] Transform eye;
	[SerializeField] Vector2Int attackInterval;
	[SerializeField] Transform firePoint;
	[SerializeField] GameObject defaultProjectilePrefab;
	[SerializeField] int currentAttack;

	[Header("Attack 1")]
	[SerializeField] float attack1Damage;
	[SerializeField] float attack1Speed;
	[SerializeField] int attack1ProjectileCount;

	[Header("Attack 2")]
	[SerializeField] float attack2Damage;
	[SerializeField] float attack2Speed;
	[SerializeField] int attack2ProjectileCount;
	[SerializeField] int attack2BurstCount;

	[Header("Attack 3")]
	[SerializeField] float attack3Damage;
	[SerializeField] float attack3Speed;
	[SerializeField] int attack3ProjectileCount;
	[SerializeField] int attack3AngleStep;

	[Header("Attack 5")]
	[SerializeField] float attack5Damage;
	[SerializeField] float attack5Speed;
	[SerializeField] int attack5ProjectileCount;

	[Header("Attack 6")]
	[SerializeField] float attack6Damage;
	[SerializeField] float attack6Speed;
	[SerializeField] float attack6MoveSpeed;
	[SerializeField] int attack6ProjectileCount;
	[SerializeField] Transform attack6Target;
	bool is6Attacking;

	[Header("Attack 7")]
	[SerializeField] Vector2Int attack7MovementRange;
	[SerializeField] float attack7MoveSpeed;
	[SerializeField] int currentAttack7Count;
	[SerializeField] int currentAttack7Movements;

	[Header("Attack 8")]
	[SerializeField] float attack8ProjDamage;
	[SerializeField] float attack8ProjSpeed;
	[SerializeField] Projectile attack8ProjPrefab;

	[Header("Attack 9")]
	[SerializeField] float attack9ProjDamage;
	[SerializeField] float attack9ProjSpeed;
	[SerializeField] int attack9ProjCount;

	Transform target;

	Transform currentTargetPosition;
	int currentTargetPositionIndex;
	int lastTargetPositionIndex;

	bool isDead;
	bool isStopped;
	int movementCount;
	Rigidbody2D rb;

	protected override void Start()
	{
		base.Start();

		isStopped = true;

		SetCurrentTargetPositionIndex(7);
		rb = GetComponent<Rigidbody2D>();
		currentMoveSpeed = moveSpeed;
		target = FindObjectOfType<PocketMorBattlePlatform>().transform;
		if (FindObjectOfType<Player>()) target = FindObjectOfType<Player>().transform;
	}

	private void Update()
	{
		eye.localPosition = (target.position - transform.position).normalized * .2f;
		eye.gameObject.SetActive(animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"));

		if (health.GetCurrentHealth() <= 0)
		{
			if (!isDead)
			{
				isDead = true;

				animator.SetTrigger("Dead");
			}
			onHealthEnd();
		}
	}

	protected override void FixedUpdate()
	{
		base.FixedUpdate();

		ArrivedToPositionControl();
		MoveToTargetPosition();
	}

	void MoveToTargetPosition()
	{
		if (isStopped) rb.velocity = Vector2.zero;
		else rb.velocity = (currentTargetPosition.position - transform.position).normalized * currentMoveSpeed;
	}

	void ArrivedToPositionControl()
	{
		if (!currentTargetPosition)
		{
			List<Transform> positions = new List<Transform>(pocketMorPositions);
			positions.RemoveAt(4);

			int index = Random.Range(0, positions.Count);
			SetCurrentTargetPositionIndex(pocketMorPositions.IndexOf(positions[index]));
			currentTargetPosition = pocketMorPositions[currentTargetPositionIndex];

			transform.position = currentTargetPosition.position;
		}

		if (Vector2.Distance(transform.position, currentTargetPosition.position) < 0.5f)
		{
			movementCount++;

			if (movementCount >= Random.Range(attackInterval.x, attackInterval.y))
			{
				if ((currentAttack == 7 && currentAttack7Count >= currentAttack7Movements) || currentAttack != 7)
				{
					currentMoveSpeed = moveSpeed;
					RandomizeAttack();
					StartAttack();
					movementCount = 0;
				}
			}

			if (currentAttack == 6 && is6Attacking)
			{
				SetCurrentTargetPositionIndex(GetTargetOppositePosition(currentTargetPositionIndex));
			}
			else if (currentAttack == 7 && currentAttack7Count < currentAttack7Movements)
			{
				currentAttack7Count++;
				SetCurrentTargetPositionIndex(GetNextAttack7TargetPosition(currentTargetPositionIndex));
			}
			else
			{
				animator.SetBool("Borrado", false);
				currentMoveSpeed = moveSpeed;
				SetCurrentTargetPositionIndex(GetNextClockwisePositionIndex(currentTargetPositionIndex, true));
			}
		}
	}

	Queue<int> lastAttacks = new Queue<int>();
	void RandomizeAttack()
	{
		lastAttacks.Enqueue(currentAttack);
		currentAttack = GetRandomAttackByPosition(currentTargetPositionIndex);
	}

	void StartAttack()
	{
		switch(currentAttack)
		{
			case 1: StartCoroutine(Attack1());break;
			case 2: StartCoroutine(Attack2());break;
			case 3: StartCoroutine(Attack3());break;
			case 5: StartCoroutine(Attack5());break;
			case 6: StartCoroutine(Attack6());break;
			case 7: StartCoroutine(Attack7());break;
			case 8: StartCoroutine(Attack8());break;
			case 9: StartCoroutine(Attack9());break;
		}
	}

	IEnumerator Attack1()
	{
		isStopped = true;
		yield return new WaitForSeconds(.5f);
		animator.SetBool("Attack1", true);

		RadialShoot(target, attack1Speed, attack1Damage, 360, attack1ProjectileCount);

		yield return new WaitForSeconds(.5f);
		animator.SetBool("Attack1", false);
		isStopped = false;
	}

	IEnumerator Attack2()
	{
		isStopped = true;
		
		yield return new WaitForSeconds(.5f);
		animator.SetBool("Attack2", true);
		
		for (int i = 0; i < attack2BurstCount; i++)
		{
			yield return new WaitForSeconds(.25f);
			RadialShoot(target, attack2Speed, attack2Damage, 360, attack2ProjectileCount);
		}

		yield return new WaitForSeconds(.5f);
		animator.SetBool("Attack2", false);
		isStopped = false;
	}

	IEnumerator Attack3()
	{
		isStopped = true;

		yield return new WaitForSeconds(.5f);
		animator.SetBool("Attack2", true);

		StartCoroutine(BurstRadialShot(target, attack3Speed, attack3Damage, 360, attack3AngleStep,attack3ProjectileCount, 0.2f));
		StartCoroutine(BurstRadialShot(target, attack3Speed, attack3Damage, 360, attack3AngleStep, attack3ProjectileCount, 0.2f, false, 90));
		StartCoroutine(BurstRadialShot(target, attack3Speed, attack3Damage, 360, attack3AngleStep, attack3ProjectileCount, 0.2f, false, 180));
		StartCoroutine(BurstRadialShot(target, attack3Speed, attack3Damage, 360, attack3AngleStep, attack3ProjectileCount, 0.2f, false, 270));
		StartCoroutine(BurstRadialShot(target, attack3Speed, attack3Damage, 360, attack3AngleStep, attack3ProjectileCount, 0.2f, true));
		StartCoroutine(BurstRadialShot(target, attack3Speed, attack3Damage, 360, attack3AngleStep, attack3ProjectileCount, 0.2f, true, 90));
		StartCoroutine(BurstRadialShot(target, attack3Speed, attack3Damage, 360, attack3AngleStep, attack3ProjectileCount, 0.2f, true, 180));
		StartCoroutine(BurstRadialShot(target, attack3Speed, attack3Damage, 360, attack3AngleStep, attack3ProjectileCount, 0.2f, true, 270));

		yield return new WaitForSeconds((0.2f * attack3ProjectileCount) + 1);
		animator.SetBool("Attack2", false);
		isStopped = false;
	}

	IEnumerator Attack5()
	{
		isStopped = true;
		yield return new WaitForSeconds(.25f);
		animator.SetBool("Attack2", true);

		RadialShoot(target, attack5Speed, attack5Damage, 315, attack5ProjectileCount, Random.Range(-90, 90));

		yield return new WaitForSeconds(.5f);
		animator.SetBool("Attack2", false);
		isStopped = false;
	}
	                                                                                                                                        
	IEnumerator Attack6()
	{
		is6Attacking = true;
		currentMoveSpeed = attack6MoveSpeed;
		isStopped = true;

		yield return new WaitForSeconds(.1f);
		animator.SetBool("Borrado", true);

		isStopped = false;

		for (int i = 0; i < 5; i++)
		{
			RadialShoot(target, attack6Speed, attack6Damage, 360, attack6ProjectileCount);
			yield return new WaitForSeconds(.3f);
		}

		yield return new WaitForSeconds(.5f);
		animator.SetBool("Borrado", false);

		is6Attacking = false;
	}

	IEnumerator Attack7()
	{
		currentAttack7Count = 0;
		isStopped = true;

		yield return new WaitForSeconds(.5f);
		animator.SetBool("Borrado", true);

		isStopped = false;

		currentMoveSpeed = attack7MoveSpeed;
		currentAttack7Movements = Random.Range(attack7MovementRange.x, attack7MovementRange.y);
	}

	IEnumerator Attack8()
	{
		isStopped = true;
		yield return new WaitForSeconds(.5f);
		animator.SetBool("Attack4", true);

		DefaultShoot(attack8ProjPrefab, target, attack8ProjSpeed, attack8ProjDamage);

		yield return new WaitForSeconds(.5f);
		animator.SetBool("Attack4", false);
		isStopped = false;
	}

	IEnumerator Attack9()
	{
		isStopped = true;
		yield return new WaitForSeconds(.5f);
		animator.SetBool("Final", true);

		for (int i = 0; i < attack9ProjCount; i++)
		{
			CinemachineShake.Instance.ShakeCamera(.5f, 5);
			Vector3 spawnPos = new Vector3(Random.Range(pocketMorPositions[3].position.x + 1.5f, pocketMorPositions[5].position.x - 1.5f),
				pocketMorPositions[7].position.y + 10, 0);
			DownShoot(defaultProjectilePrefab, spawnPos, attack9ProjSpeed, attack9ProjDamage);
			yield return new WaitForSeconds(.1f);
		}

		yield return new WaitForSeconds(2.5f);
		animator.SetBool("Final", false);
		isStopped = false;
	}

	public int GetNextClockwisePositionIndex(int current, bool isClockwise)
	{
		if (isClockwise)
		{
			switch (current)
			{
				case 0: return 3;
				case 3: return 6;
				case 6: return 7;
				case 7: return 8;
				case 8: return 5;
				case 5: return 2;
				case 2: return 1;
				case 1: return 0;
			}
		}
		else
		{
			switch (current)
			{
				case 0: return 1;
				case 1: return 2;
				case 2: return 5;
				case 5: return 8;
				case 8: return 7;
				case 7: return 6;
				case 6: return 3;
				case 3: return 0;
			}
		}

		return 0;
	}

	public int GetTargetOppositePosition(int current)
	{
		switch (current)
		{
			case 0: return 8;
			case 8: return 0;
			case 2: return 6;
			case 6: return 2;
			case 3: return 5;
			case 5: return 3;
			case 7: return 1;
			case 1: return 7;
		}                                                                        

		return currentTargetPositionIndex;
	}

	public int GetNextAttack7TargetPosition(int current)
	{
		List<int> targets = new List<int>();
		switch (current)
		{
			case 0: targets = new List<int> { 7, 8, 5 }; break;
			case 1: targets = new List<int> { 6, 7, 8 }; break;
			case 2: targets = new List<int> { 3, 6, 7 }; break;
			case 3: targets = new List<int> { 8, 5, 2 }; break;
			case 4: targets = new List<int> { 0, 1, 2, 3, 5, 6, 7, 8 }; break;
			case 5: targets = new List<int> { 0, 3, 6 }; break;
			case 6: targets = new List<int> { 1, 2, 5 }; break;
			case 7: targets = new List<int> { 0, 1, 2 }; break;
			case 8: targets = new List<int> { 3, 0, 1 }; break;
		}

		targets.Remove(lastTargetPositionIndex);
		return targets[Random.Range(0, targets.Count)];
	}

	void SetCurrentTargetPositionIndex(int current)
	{
		lastTargetPositionIndex = currentTargetPositionIndex;
		currentTargetPositionIndex = current;

		currentTargetPosition = pocketMorPositions[currentTargetPositionIndex];
	}

	void DefaultShoot(Projectile prefab, Transform target, float speed, float damage)
	{
		Vector3 projectileMoveDir = (target.transform.position - firePoint.position).normalized;

		Projectile projectile = GameObject.Instantiate(prefab, firePoint.position, Quaternion.identity);
		projectile.Setup(firePoint.transform.position + projectileMoveDir * 10, damage, speed, transform);
	}

	void DownShoot(GameObject prefab, Vector3 spawnPosition, float speed, float damage)
	{
		Projectile projectile = GameObject.Instantiate(prefab, spawnPosition, Quaternion.identity).GetComponent<Projectile>();
		projectile.Setup(projectile.transform.position + Vector3.down * 10, damage, speed, transform);
	}

	private void RadialShoot(Transform target, float speed, float damage, float radius, int shootsCount, float angleMod = 0)
	{
		float angle = Toolkit2D.GetAngleBetweenTwoPoints(firePoint.position, target.position) - radius / 2;

		angle += angleMod;

		float angleStep = radius / (shootsCount - 1);

		Vector2 startPoint = new Vector2(firePoint.position.x, firePoint.position.y);

		AudioClip bulletClip = null;

		for (int i = 0; i < shootsCount; i++)
		{
			float bulletDirXPosition = startPoint.x + Mathf.Sin((angle * Mathf.PI) / 180) * radius;
			float bulletDirYPosition = startPoint.y + Mathf.Cos((angle * Mathf.PI) / 180) * radius;

			Vector2 projectileVector = new Vector2(bulletDirXPosition, bulletDirYPosition);
			Vector3 projectileMoveDir = (projectileVector - startPoint).normalized;

			GameObject projectile = GameObject.Instantiate(defaultProjectilePrefab, firePoint.position, Quaternion.identity);
			projectile.GetComponent<Projectile>().Setup(firePoint.transform.position + projectileMoveDir * 10, damage, speed, transform);

			projectile.GetComponent<AudioSource>().enabled = false;

			if (bulletClip == null)
				bulletClip = projectile.GetComponent<AudioSource>().clip;

			angle += angleStep;
		}

		audioSource.PlayOneShot(bulletClip);
	}

	IEnumerator BurstRadialShot(Transform target, float speed, float damage, float radius, int angleStep, int shootsCount, float shootInterval, bool isInverse = false, float angleIncrease = 0)
	{
		float angle = Toolkit2D.GetAngleBetweenTwoPoints(firePoint.position, target.position) - radius / 2;

		angle += angleIncrease;

		//float angleStep = radius / (shootsCount - 1);

		Vector2 startPoint = new Vector2(firePoint.position.x, firePoint.position.y);

		AudioClip bulletClip = null;

		for (int i = 0; i < shootsCount; i++)
		{
			float bulletDirXPosition = startPoint.x + Mathf.Sin((angle * Mathf.PI) / 180) * radius;
			float bulletDirYPosition = startPoint.y + Mathf.Cos((angle * Mathf.PI) / 180) * radius;

			Vector2 projectileVector = new Vector2(bulletDirXPosition, bulletDirYPosition);
			Vector3 projectileMoveDir = (projectileVector - startPoint).normalized;

			GameObject projectile = GameObject.Instantiate(defaultProjectilePrefab, firePoint.position, Quaternion.identity);
			projectile.GetComponent<Projectile>().Setup(firePoint.transform.position + projectileMoveDir * 10, damage, speed, transform);

			projectile.GetComponent<AudioSource>().enabled = false;

			if (bulletClip == null)
				bulletClip = projectile.GetComponent<AudioSource>().clip;

			if (isInverse) angle -= angleStep;
			else angle += angleStep;

			yield return new WaitForSeconds(shootInterval);
		}

		audioSource.PlayOneShot(bulletClip);
	}

	int GetRandomAttackByPosition(int currentPosition)
	{
		List<int> attacks = new List<int>();
		switch (currentPosition)
		{
			case 0:
				attacks = new List<int> { 4, 6, 7, 8, 9 }; break;
			case 1:
				attacks = new List<int> { 1, 4, 5, 7, 8, 9 }; break;
			case 2:
				attacks = new List<int> { 4, 6, 7, 8, 9 }; break;
			case 3:
				attacks = new List<int> { 1, 4, 5, 7, 8, 9 }; break;
			case 4:
				attacks = new List<int> { 1, 2, 4, 5, 9 }; break;
			case 5:
				attacks = new List<int> { 1, 4, 5, 7, 8, 9 }; break;
			case 6:
				attacks = new List<int> { 4, 6, 7, 8, 9 }; break;
			case 7:
				attacks = new List<int> { 1, 2, 3, 4, 5, 7, 8, 9 }; break;
			case 8:
				attacks = new List<int> { 4, 6, 7, 8, 9 }; break;
		}

		attacks.Remove(currentAttack);
		attacks.Remove(lastAttacks.Dequeue());
		if (health.GetCurrentPercentage() > .2f) attacks.Remove(9);
		return attacks[Random.Range(0, attacks.Count)];
	}

	public void StartFight()
	{
		isStopped = false;

		healthBarInstance = Instantiate(healthBarPrefab, hpCanvas.transform);
		HealthBar h = healthBarInstance.GetComponentInChildren<HealthBar>();
		h.type = TrackType.Enemy;
		h.SetupBar(health);
	}

	public void OnHitted()
	{

	}



	public override void Die()
	{
		DropDNA(3);

		var bossRoom = FindObjectOfType<BossRoom>();
		if (bossRoom) bossRoom.FinishRoom();

		Invoke(nameof(Win), 5);

		CameraManager cameraManager = FindObjectOfType<CameraManager>();
		cameraManager.DisableTarget(cameraManager.GetTargetGroup().m_Targets[cameraManager.GetTargetGroup().m_Targets.Length - 1]);
		for (int i = 0; i < cameraManager.GetTargetGroup().m_Targets.Length; i++)
		{
			cameraManager.GetTargetGroup().m_Targets[i].radius = 1.75f;
		}

		gameObject.SetActive(false);
	}

	void Win()
	{
		GlobalData.Instance.EndRun("You Win");
	}
}
