using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SnakeController : SnakeSegment
{
	private enum AttackBehaviour
	{
		Dash,
		Shoot
	}

	[Header("Body Settings")]
	public int bodySize;
	public SnakeSegment segmentPrefab;
    public List<SnakeSegment> segments = new List<SnakeSegment>();

	[Header("Move Settings")]
	public float patrolSpeed;
	public float moveSpeed;
	public float wobblySpeed;
	public float wobblyMagnitude;

	[Header("General Settings")]
	[SerializeField] private AttackBehaviour attackBehaviour = AttackBehaviour.Dash;
	public float sightRadius;
	public LayerMask obstacleLayers;
	public AudioClip attackClip;

	[Header("Dash Settings")]
	public float dashSpeed;
	public Vector2 dashInterval;

	[Header("Shoot Settings")]
	[SerializeField] private Projectile projectilePrefab;
	[SerializeField] private float projectileDamage;
	[SerializeField] private float projectileSpeed;
	[SerializeField] private Vector2 shootWait;
	[SerializeField] private Vector2 shootInterval;
	[SerializeField] private Transform shootPoint;

	bool isAggressive;
	bool isDashing;
	bool dashEnabled;
	Vector3 dashDirection;
	Vector3 currentNeutralDirection;

	private bool _isShooting;
	private float _elapsedShootTime;
	private float _currentShootInterval;

	Rigidbody2D rig;
	Enemy enemy;
	NavMeshAgent agent;
	Animator animator;
	AudioSource audioSource;

	protected override void Start()
	{
		base.Start();
		BuildSegments();
		SetupAgent();
		animator = sprite.GetComponent<Animator>();
		audioSource = GetComponent<AudioSource>();

		Invoke("EnableDash", Random.Range(dashInterval.x, dashInterval.y));

		StartCoroutine(SwitchDirection());

		rig = GetComponent<Rigidbody2D>();
		enemy = GetComponent<Enemy>();

		wobblySpeed = wobblySpeed * Random.Range(.75f, 1.25f);
		if (Random.value < .5f) wobblySpeed = wobblySpeed * -1;
		wobblyMagnitude = wobblyMagnitude * Random.Range(.75f, 1.25f);

		_elapsedShootTime = 0f;
		_currentShootInterval = Random.Range(shootInterval.x, shootInterval.y);
	}

	protected override void FixedUpdate()
	{
		base.FixedUpdate();

		if (PlayerOnRange())
			isAggressive = true;

		if (isAggressive) Aggressive();
		else Neutral();
	}

	bool PlayerOnRange()
	{
		return Physics2D.OverlapCircle(transform.position, sightRadius, LayerMask.GetMask("Player"));
	}

	void Neutral()
	{
		rig.velocity = currentNeutralDirection * patrolSpeed;
		Toolkit2D.RotateAt(sprite.transform, transform.position + currentNeutralDirection);

		if (Physics2D.Raycast(transform.position, currentNeutralDirection, 1, obstacleLayers))
		{
			currentNeutralDirection = GetRandomDirection();
		}

		WobbleMovement(GetPlayerDirection());
	}

	void Aggressive()
	{
		switch (attackBehaviour)
		{
			case AttackBehaviour.Dash: DashBehaviour(); break;
			case AttackBehaviour.Shoot: ShootBehaviour(); break;
		}

		if (GetComponent<EnemyLightsController>())
			GetComponent<EnemyLightsController>().StartLoop();
	}

	private void WobbleMovement(Vector3 axis)
	{
		float sin = Mathf.Sin(90 * Mathf.Deg2Rad);
		float cos = Mathf.Cos(90 * Mathf.Deg2Rad);
		float tx = axis.x;
		float ty = axis.y;
		axis.x = (cos * tx) - (sin * ty);
		axis.y = (sin * tx) + (cos * ty);

		// Wobbly
		rig.position = transform.position + axis * Mathf.Sin(Time.time * wobblySpeed) * wobblyMagnitude / 100;
	}

	private void DashBehaviour()
    {
        animator.SetBool("IsAttacking", isDashing);
        if (isDashing)
        {
            rig.velocity = dashDirection * dashSpeed;
            agent.speed = 0;
            agent.transform.position = transform.position;
            Toolkit2D.RotateAt(sprite.transform, transform.position + dashDirection);
            return;
        }

        if (dashEnabled)
        {
            if (!Physics2D.Raycast(transform.position, GetPlayerDirection(), 3, obstacleLayers))
                StartCoroutine(StartDash());
        }

        NormalMovement();

        WobbleMovement(GetPlayerDirection());
    }

    private void ShootBehaviour()
	{
		animator.SetBool("IsAttacking", _isShooting);

		if (_elapsedShootTime < _currentShootInterval)
		{
			_elapsedShootTime += Time.deltaTime;

			NormalMovement();

			WobbleMovement(GetPlayerDirection());
			return;
		}

		if (_isShooting)
		{
			Toolkit2D.RotateAt(sprite.transform, transform.position + GetPlayerDirection());
			return;
		}

		StartCoroutine(Shoot());
	}

    private void NormalMovement()
    {
        agent.speed = moveSpeed;
        agent.destination = enemy.GetPlayer().transform.position;
        Vector3 dir = (agent.transform.position - transform.position).normalized;
        rig.velocity = dir * moveSpeed;
        Toolkit2D.RotateAt(sprite.transform, transform.position + GetPlayerDirection());
    }

	private void EnableDash()
	{
		dashEnabled = true;
	}

	private IEnumerator StartDash()
	{
		dashEnabled = false;
		isDashing = true;
		dashDirection = GetPlayerDirection();
		audioSource.PlayOneShot(enemy.attackSound);

		yield return new WaitForSeconds(1);
		isDashing = false;

		yield return new WaitForSeconds(Random.Range(dashInterval.x, dashInterval.y));
		dashEnabled = true;
	}

	private IEnumerator Shoot()
	{
		rig.velocity = Vector2.zero;
		agent.speed = 0f;
		agent.transform.position = transform.position;

		_isShooting = true;
		var currentInterval = Random.Range(shootWait.x, shootWait.y);

		yield return new WaitForSeconds(currentInterval);

		var projectile = Instantiate(projectilePrefab, shootPoint.position, Quaternion.identity);
		projectile.Setup(enemy.GetTarget().transform.position, projectileDamage, projectileSpeed, transform);
		audioSource.PlayOneShot(attackClip);

		yield return new WaitForSeconds(currentInterval);

		_isShooting = false;
		_elapsedShootTime = 0f;
		_currentShootInterval = Random.Range(shootInterval.x, shootInterval.y);
	}

	public Vector3 GetPlayerDirection()
	{
		return (enemy.GetPlayer().transform.position - transform.position).normalized;
	}

	private void BuildSegments()
	{
		segments.Add(this);

        for (int i = 0; i < bodySize; i++)
		{
			InstantiateSegments();
		}

		for (int i = 0; i < segments.Count - 1; i++)
		{
			segments[i].Child = segments[i + 1];
			if (i - 1 >= 0)
			{
				segments[i].Parent = segments[i - 1];
			}
		}

		foreach(SnakeSegment segment in segments)
		{
			foreach(SnakeSegment s in segments)
			{
				Physics2D.IgnoreCollision(segment.GetComponent<Collider2D>(), s.GetComponent<Collider2D>());
			}
		}
	}

	private SnakeSegment InstantiateSegments()
	{
		SnakeSegment segment = Instantiate(segmentPrefab, transform.position, Quaternion.identity);
		SnakeSegment parent = segments[segments.Count - 1];
		segment.SetupJoint(parent.GetComponent<Rigidbody2D>());
		segment.Parent = parent;
		segment.transform.position = parent.transform.position + new Vector3(0, 0, 1);
		segments.Add(segment);

		return this;
	}

	Vector2 GetRandomDirection()
	{
		List<Vector2> directions = new List<Vector2>();
		directions.Add(Vector2.up);
		directions.Add(Vector2.right);
		directions.Add(Vector2.down);
		directions.Add(Vector2.left);

		directions.Remove(currentNeutralDirection);

		if (Physics2D.Raycast(transform.position, Vector2.up, 1, obstacleLayers)) directions.Remove(Vector2.up);
		if (Physics2D.Raycast(transform.position, Vector2.right, 1, obstacleLayers)) directions.Remove(Vector2.right);
		if (Physics2D.Raycast(transform.position, Vector2.down, 1, obstacleLayers)) directions.Remove(Vector2.down);
		if (Physics2D.Raycast(transform.position, Vector2.left, 1, obstacleLayers)) directions.Remove(Vector2.left);

		Vector2 dir = directions[Random.Range(0, directions.Count)];

		return dir;
	}

	IEnumerator SwitchDirection()
	{
		while (true)
		{
			currentNeutralDirection = GetRandomDirection();
			yield return new WaitForSeconds(Random.Range(5, 7));
		}
	}

	private void SetupAgent()
	{
		agent = new GameObject().AddComponent<NavMeshAgent>();
		agent.Warp(transform.position);

		NavMeshAgent thisAgent = GetComponent<NavMeshAgent>();
		agent.radius = thisAgent.radius;
		agent.height = thisAgent.height;
		agent.baseOffset = thisAgent.baseOffset;
		agent.angularSpeed = 1000;
		agent.acceleration = 10000;
		agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
		agent.avoidancePriority = 50;

		agent.updateUpAxis = false;
		agent.updateRotation = false;

		agent.speed = moveSpeed * 8;
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (((1 << collision.gameObject.layer) & obstacleLayers) != 0)
		{
			if (isAggressive)
			{
				if (isDashing)
				{
					//CinemachineShake.Instance.ShakeCamera(4, .5f);
					GetComponent<Cinemachine.CinemachineImpulseSource>().GenerateImpulse(.75f);
					audioSource.PlayOneShot(attackClip);
				}

				isDashing = false;
				animator.SetTrigger("WallHit");
			}
		}
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, sightRadius);
	}
}