using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiantCocoon : Boss
{
	[Header("Settings")]
	[SerializeField] private float sightRange;

	[Header("Eye")]
	[SerializeField] private GameObject eye;
	private Health _eyeHealth;
	private Enemy _eyeEnemy;
	[SerializeField] private Transform eyePosition;
	[SerializeField] Transform target;
	[SerializeField] float eyeRadius = .1f;
	Vector3 targetDirection;

	[Header("Default Attack")]
	[SerializeField] private Projectile defaultProjectilePrefab;
	[SerializeField] private Projectile windProjectilePrefab;
	[SerializeField] private float projectileDamage = 10f;
	[SerializeField] private float projectileSpeed = 8f;

	[SerializeField] float attackInterval = 3;
	float interval = 0;

	[Header("Omnidirectional Shots")]
	[SerializeField] int amountOfShots = 6;

	[Header("Shockwave Attack")]
	[SerializeField] GameObject shockwavePrefab;
	[SerializeField] private float smallShockwaveSize = 18.75f;
	[SerializeField] private float bigShockwaveSize = 25f;

	[Header("Shield")]
	public GameObject shields;
	public float rotationSpeed = 2;
	CircularOrganizer shieldOrganizer;

	[Header("Spawn enemies")]
	public Transform spawnPoints;
	[SerializeField] private int enemyCount = 1;
	public GameObject casuloPrefab;
	public GameObject spawnParticle;

	[Header("Sounds")]
	[SerializeField] private AudioClip shoutClip;

	private GameObject[] _spawnedEnemies;

	public bool isStarted;
	bool wokeUp = false;
	bool canBeFurious = false;
	bool isFurious = false;
	bool isDead;

    bool isShooting = false;
    bool isShockwave = false;

	int attacks = 0;

	private void Awake()
	{
		_eyeHealth = eye.GetComponent<Health>();
		_eyeEnemy = eye.GetComponent<Enemy>();
	}

    protected override void Start()
	{
		_spawnedEnemies = new GameObject[spawnPoints.childCount];

		shieldOrganizer = GetComponentInChildren<CircularOrganizer>();

		//Invoke("OmnidirectionalShooting", 5);

		interval = attackInterval;

		base.Start();
	}

    private void Update()
    {
		if (!wokeUp)
			_eyeHealth.isInvulnerable = true;
		else
			_eyeHealth.isInvulnerable = false;

		eyePosition.gameObject.SetActive(wokeUp);

		if (!isDead)
		{
			if (_eyeHealth.GetCurrentPercentage() <= .5f)
			{
				if (!canBeFurious)
				{
					animator.SetTrigger("Evolve");
					canBeFurious = true;
				}

				animator.SetBool("Can Be Furious", canBeFurious);
				animator.SetBool("Furious", isFurious);
			}
		}

		shieldOrganizer.startAngle += Time.deltaTime * rotationSpeed;

        if (isFurious)
        {
            shields.SetActive(true);
            shieldOrganizer.startAngle += Time.deltaTime * rotationSpeed;
        }
        else
        {
            shields.SetActive(false);
        }

		target = GetRandomPlayer().transform;

		if (wokeUp)
		{
			if (interval <= attackInterval)
				interval += Time.deltaTime;
			else
			{
				Attack();
			}
		}
    }

	public void SetFurious()
    {
		isFurious = true;
    }

    protected override void FixedUpdate()
	{
		base.FixedUpdate();

		if (!isStarted)
		{

            if (IsPlayerOnRange())
            {
                isStarted = true;
                OnSeePlayer();
            }
        }
        else
        {
			if(!isDead)
				EyeTracker();
		}

		if (_eyeHealth.GetCurrentHealth() <= 0 && !isDead)
		{
			animator.SetTrigger("Die");
			StopAllCoroutines();
			isDead = true;
		}
	}

	void EyeTracker()
    {
		target = enemy.GetTarget();
		if (target)
        {
			targetDirection = target.transform.position - eyePosition.position;

			targetDirection = Vector2.ClampMagnitude(targetDirection, eyeRadius);
			eye.transform.position = eyePosition.position + targetDirection;
		}
	}

	void Attack()
    {
		int attackIndex = Random.Range(0, 3);

        switch (attackIndex)
        {
			case 0:
				if (isFurious)
					ShootSequence(10, .1f);
				else
					ShootSequence(5, .25f);
				break;

			case 1:
				if (isFurious)
					ShootShockwave(false);
				else
					ShootShockwave(true);
				break;

			case 2:
				StartCoroutine(ShootOmni(isFurious));
				break;

            default:
				break;
        }

		if(attacks >= 3)
        {
			SpawnEnemy();
			attacks = 0;
        }

		attacks++;
		interval = 0;
    }

	#region Attacks

	// Standard
	public void ShootOnce()
	{
		Projectile projectile = Instantiate(defaultProjectilePrefab, eye.transform.position, Quaternion.identity);
		projectile.Setup(target.position, projectileDamage, projectileSpeed, transform);
	}

	void ShootSequence(int amount, float interval)
    {
		StartCoroutine(BurstShots(amount, interval));
    }

	IEnumerator BurstShots(int amount, float interval)
	{
		if (canBeFurious == true && isFurious == false) yield return null;

		for (int i = 0; i < amount; i++)
		{
			animator.SetTrigger("Attack");

			yield return new WaitForSeconds(interval);
		}
	}

	void OmnidirectionalShooting(float offset)
    {
		float angle = Toolkit2D.GetAngleBetweenTwoPoints(eye.transform.position, target.position) - 360 / 2;
		angle += offset;

		float step = 360 / (amountOfShots);

		Vector2 startPoint = new Vector2(eye.transform.position.x, eye.transform.position.y);

		for (int i = 0; i < amountOfShots; i++)
        {
			float bulletDirXPosition = startPoint.x + Mathf.Sin((angle * Mathf.PI) / 180) * 360;
			float bulletDirYPosition = startPoint.y + Mathf.Cos((angle * Mathf.PI) / 180) * 360;

			Vector2 projectileVector = new Vector2(bulletDirXPosition, bulletDirYPosition);
			Vector3 projectileMoveDir = (projectileVector - startPoint).normalized;

			Projectile projectile = Instantiate(windProjectilePrefab, eye.transform.position, Quaternion.identity);
			projectile.Setup(eye.transform.position + projectileMoveDir * 10, projectileDamage, 3, transform);

			angle += step;
		}
    }

	void ShootShockwave(bool reduced)
    {
		if (canBeFurious == true && isFurious == false) return;

		if (isShockwave) return;

		GameObject shockwave = Instantiate(shockwavePrefab, transform.position, Quaternion.identity);

		ShockwaveController controller = shockwave.GetComponent<ShockwaveController>();


		if (reduced)
        {
			controller.Setup(3, smallShockwaveSize);
        }
        else
        {
			controller.Setup(3, bigShockwaveSize);
		}

		controller.ResetEvent();
	}

	void SpawnEnemy()
    {
		var spawnedCount = 0;

        for (int i = 0; i < spawnPoints.childCount; i++)
        {
			if (_spawnedEnemies[i] != null)
				continue;
			
			GameObject enemyGO = Instantiate(casuloPrefab, spawnPoints.GetChild(i).position, Quaternion.identity, null);
			GameObject particle = Instantiate(spawnParticle, enemyGO.transform.position, Quaternion.identity);
			_spawnedEnemies[i] = enemyGO;

			var spawnedEnemy = enemyGO.GetComponent<Enemy>();
			spawnedEnemy.SetCurrentRoom(enemy.currentRoom);

			particle.GetComponent<SpawnParticle>().SetupEnemySpawn(spawnedEnemy);

			spawnedCount++;
			if (spawnedCount >= enemyCount)
				break;
        }
    }

    #endregion

    void StartIdleAnimation()
	{
		eye.SetActive(true);
	}

	void OnSeePlayer()
	{
		target = enemy.GetPlayer().transform;

		animator.SetTrigger("WakeUp");
		audioSource.PlayOneShot(shoutClip);

		StartBossTheme();

		SetCinemachineTarget();
	}

	public void StartBossRoutine()
	{
		wokeUp = true;

		SetHealthBar(_eyeHealth);

		_eyeEnemy.SetCurrentRoom(enemy.currentRoom);

		//StartCoroutine(ShootOmni());
	}

	IEnumerator ShootOmni(bool furious)
    {
		OmnidirectionalShooting(0);

		yield return new WaitForSeconds(1);

		OmnidirectionalShooting((360 / amountOfShots) / 2);

		if (isFurious)
		{

			yield return new WaitForSeconds(1);

			OmnidirectionalShooting(0);
		}
	}

	public override void onHealthEnd()
	{
		eye.gameObject.SetActive(false);
		animator.SetBool("Dead", true);
		animator.SetTrigger("Die");

		base.onHealthEnd();
	}


	bool IsPlayerOnRange()
	{
		return Physics2D.OverlapCircle(transform.position, sightRange, LayerMask.GetMask("Player"));
	}

	private void OnHitted()
	{
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, sightRange);
	}
}
