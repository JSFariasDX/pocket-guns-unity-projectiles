using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehaviourController : MonoBehaviour
{
    [Header("States Settings")]
    public bool isAggressive;
    public AggressiveTrigger aggressiveTrigger;
    public NeutralState neutralState;
    public AggressiveBehaviour aggressiveState;
    public string animatorTriggerWhenAggressive;
    [SerializeField] string attackAnimatorTrigger;
    [SerializeField] private bool enableLightWhenAggressive;

    [Header("Movement Settings")]
    public bool avoidObstacles;
    public float moveSpeed;
    public float patrolRange = 3;
    public Vector2 patrolStoppingTimeRange;
    public bool useAxisAnimation;
    public ParticleSystem movementVfx;

    [Header("Wobbly and Dash Settings")]
    public bool wobblingEnabled = true;
    public float zigZagSpeed;
    public float zigZagMagnitude;
    public float dashTime = 3;
    public Vector2 dashResetTimeRange;
    public float dashSpeed = 1;
    public Vector2 dashDirectionOffset = new Vector2(-50f, 50f);

    [Header("Teleport Settings")]
    public float teleportRange;
    public float prepareTeleportTime;
    public Vector2 resetTeleportTimeInterval;
    public GameObject teleportParticlePrefab;
    public AudioClip teleportSound;

    [Header("Dig Settings")]
    public bool attackOnEmerge;
    public bool followTarget;
    public float defaultRotation;
    public Projectile emergeProjectile;
    public float emergeProjSpawnOffset;
    public float emergeProjDamage;
    public float emergeProjSpeed;
    public AudioClip emergeAttackClip;
    public int emergeRadialCount;
    public int emergeRadialShootRadius;

    [Header("Attack Settings")]
    public bool targetPlayer = true;
    public Vector2 targetOffset = Vector2.zero;
    public ShootType shootType;
    public int radialCount;
    public int burstCount;
    [SerializeField] private Vector2Int randomShootCount;
    public float radialShootRadius;
    public float burstShootInterval;
    public Transform projectileSpawnPoint;
    public GameObject projectilePrefab;
    public float projectileDamage = 15;
    public float projectileSpeed = 5;
    public float shootsPerSecond;

    [Header("AI Settings")]
    [Tooltip("Used in the patrol state, to avoid objects, walls...")] public LayerMask obstacleLayers;
    [Tooltip("Used in the patrol state, to avoid objects, walls...")] public float obstacleDetectionRange;
    public float playerDetectionRange;
    public Vector2 safeRange;
    public EnemyAction action;
    public ActionTrigger actionTrigger;

    [Header("Divide Settings")]
    public bool divideOnDie;
    public GameObject childrenPrefab;
    public AudioClip divideSound;

    [Header("Effects")]
    EnemyLightsController lights;

    [Header("Sounds")]
    public AudioClip warningSound; // used in prepare teleport
    public AudioClip digSound;

    public AggressiveStateController aggressiveStateController { get; private set; }
    private NeutralStateController neutralStateController;

    [HideInInspector] public NavMeshAgent agent;
    [HideInInspector] public Animator animator;
    [HideInInspector] public Enemy enemy;
    private Player player;

    private void Awake()
	{
        //player = FindObjectOfType<Player>();
        animator = GetComponent<Animator>();
        enemy = GetComponent<Enemy>();
        SetupAgent();


        enemy.agentSpeed = moveSpeed;
        
        EnableMovementVfxEmission(false);

        neutralStateController = new NeutralStateController(neutralState, this);
        aggressiveStateController = new AggressiveStateController(aggressiveState, this, enableLightWhenAggressive);

        canTeleport = true;
    }

	private void Start()
	{
        if (GetComponent<EnemyLightsController>())
            lights = GetComponent<EnemyLightsController>();

		if (aggressiveTrigger == AggressiveTrigger.OnStart)
		{
            SetAggressive();
        }

        zigZagMagnitude = zigZagMagnitude * Random.Range(.9f, 1.1f);
        zigZagSpeed = zigZagSpeed * Random.Range(.9f, 1.1f);
        if (Random.value < .5f) zigZagSpeed = zigZagSpeed * -1f;
    }

    private void FixedUpdate()
	{
        AggressiveTriggerController();

        if (isAggressive)
		{
            aggressiveStateController.Update();
		}
        else
		{
            neutralStateController.Update();
		}

        if(animator != null)
            AnimatorController();
    }

    private void AnimatorController()
	{
        if (useAxisAnimation)
		{   
            if (agent.velocity != Vector3.zero)
			{
                animator.SetBool("IsWalking", true);
			}
			else
			{
                animator.SetBool("IsWalking", false);
            }

            Vector2 animatorVector = GetPlayerVectorDistance();
            animator.SetFloat("Horizontal", animatorVector.x);
            animator.SetFloat("Vertical", animatorVector.y);
        }

        animator.SetBool("Invincible", !enemy.GetHealth().IsDamageEnabled());
    }

    private void AggressiveTriggerController()
	{
        if (isAggressive) return;

        if (aggressiveTrigger == AggressiveTrigger.AlwaysAggressive)
		{
            SetAggressive();
        }
        else if ((GetTarget() != null) && aggressiveTrigger == AggressiveTrigger.PlayerOnRange)
		{
            if (GetPlayerDistance() < playerDetectionRange)
			{
                SetAggressive();
			}
        }
	}

    public void SetAggressive()
	{
        if (animator != null)
            animator.SetTrigger(animatorTriggerWhenAggressive);

        isAggressive = true;
	}

    public void OnHitted()
	{
        SetAggressive();

        if (!IsActionEnabled())
            return;

        if (actionTrigger == ActionTrigger.Hitted)
        {
            if (enemy.lastShooterPlayer == null)
                return;

            UseAction(enemy.lastShooterPlayer.transform);
            Health enemyHealth = enemy.GetHealth();
            
            if (enemyHealth)
                enemyHealth.SetDamageEnabled(true);
        }
	}

    bool isActing;
    public bool IsActing()
	{
        return isActing;
	}

    public Player GetTarget()
	{
        if (enemy.GetTarget()) return enemy.GetPlayer();
        else return null;
	}

    public float GetPlayerDistance()
	{
		try
		{
            return Vector2.Distance(transform.position, enemy.GetPlayer().transform.position);
		}
		catch
		{
            return 0;
		}
	}

    private void SetupAgent()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateUpAxis = false;
        agent.updateRotation = false;

        if (avoidObstacles)
        {
            agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
        }
        else
        {
            agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
        }

        agent.speed = moveSpeed;
        agent.Warp(transform.position + new Vector3(Random.Range(-.25f, .25f), Random.Range(-.25f, .25f), 0));
    }

    private Vector2 GetPlayerVectorDistance()
	{
        if (enemy.GetPlayer() == null)
        {
            return Vector2.zero;
        }
        return new Vector2(enemy.GetPlayer().transform.position.x - transform.position.x, enemy.GetPlayer().transform.position.y - transform.position.y);
	}

    public void Divide()
	{
        enemy.audioSource.PlayOneShot(divideSound);
        GameObject children = Instantiate(childrenPrefab, transform.position, Quaternion.identity);
        foreach(Enemy child in children.GetComponentsInChildren<Enemy>())
		{
            child.currentRoom = enemy.currentRoom;
            child.currentRoom.enemies.Add(child);
		}
        Destroy(gameObject);
	}

    public Vector3 GetRandomNavMeshPoint(Transform origin, float range)
    {
        Vector3 finalPosition = Vector3.zero;

        NavMeshPath navMeshPath = new NavMeshPath();
        agent.CalculatePath(finalPosition, navMeshPath);

        //while (finalPosition == Vector3.zero || NavMesh.Raycast(origin.position, finalPosition, out NavMeshHit navHit, agent.areaMask))
        while (finalPosition == Vector3.zero || navMeshPath.status != NavMeshPathStatus.PathComplete)
        {
            Vector3 randomDirection = Random.insideUnitCircle.normalized * range;

            randomDirection += origin.position;
            NavMeshHit hit;
            NavMesh.SamplePosition(randomDirection, out hit, 3, 1);
            finalPosition = hit.position;

            agent.CalculatePath(finalPosition, navMeshPath);
        }

        return finalPosition;
    }

    public float GetPatrolStopTime()
	{
        return Random.Range(patrolStoppingTimeRange.x, patrolStoppingTimeRange.y);
    }

    public void EnableMovementVfxEmission(bool enable)
	{
        if (!movementVfx) return;

        var emission = movementVfx.emission;
        emission.enabled = enable;
    }

    public void EnableWobbling()
	{
        wobblingEnabled = true;
	}

    [HideInInspector] public bool canTeleport;
    [HideInInspector] public bool preparingTeleport;
    public IEnumerator Teleport(Transform target, float prepareTeleportTime, float resetTeleportTime, bool shoot = false)
    {
        canTeleport = false;

        yield return new WaitForSeconds(resetTeleportTime * Random.Range(.7f, 1.3f));

        preparingTeleport = true;
        agent.isStopped = true;

		// Teleport visual feedback
		Vector3 teleportPosition = GetRandomNavMeshPoint(target, teleportRange);

        GameObject teleportFeedback = GameObject.Instantiate(teleportParticlePrefab, teleportPosition, Quaternion.identity);
        enemy.audioSource.PlayOneShot(warningSound);

        yield return new WaitForSeconds(prepareTeleportTime);

        if (useAxisAnimation)
        {
            animator.SetTrigger("Teleport");
        }

        enemy.audioSource.PlayOneShot(teleportSound);
        agent.Warp(teleportPosition);

        preparingTeleport = false;

        yield return new WaitForSeconds(.5f);

        if (shoot)
        {
            StartCoroutine(aggressiveStateController.Shoot(GetTarget().transform, 1 / shootsPerSecond));
        }

        yield return new WaitForSeconds(.5f);

        agent.isStopped = false;
        canTeleport = true;
        SetActing(false);
    }

    bool actionEnabled;
    public void UseAction(Transform target)
	{
        if (actionEnabled)
        {
            SetActing(true);
            if (action == EnemyAction.TeleportAndShoot)
            {
                StartCoroutine(Teleport(target, prepareTeleportTime, 0, true));
                StartCoroutine(HitLag(.6f));
            }
            SetActionEnabled(false);
        }
    }

    private void OnDestroy()
    {
        if(action == EnemyAction.TeleportAndShoot)
            Time.timeScale = 1;
    }

    IEnumerator HitLag(float time)
	{
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(time);
        Time.timeScale = 1;
	}

    public bool IsActionEnabled()
	{
        return actionEnabled;
	}

    public void SetActing(bool acting)
	{
        isActing = acting;
	}

    public void SetActionEnabled(bool enabled)
	{
        actionEnabled = enabled;
	}

    public IEnumerator EnableAction(float time)
	{
        yield return new WaitForSeconds(time);
        actionEnabled = true;
	}

    public int GetRandomShootCount()
	{
        return Random.Range(randomShootCount.x, randomShootCount.y + 1);
	}

    public float GetRandomDashResetTime()
    {
        return Random.Range(dashResetTimeRange.x, dashResetTimeRange.y);
    }
    public void TriggerAttackAnimation()
    {
        if (!attackAnimatorTrigger.Equals("") && animator.GetCurrentAnimatorStateInfo(0).IsName("Chasing"))
        {
            animator.SetTrigger(attackAnimatorTrigger);
        }
    }

    public void BlinkLight()
    {
        if(lights != null)
            lights.StartLoop();
    }

    private void OnDrawGizmosSelected()
	{
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, playerDetectionRange);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, safeRange.x);
        Gizmos.DrawWireSphere(transform.position, safeRange.y);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.right * obstacleDetectionRange);
	}
}

public enum AggressiveTrigger
{
    AlwaysAggressive, PlayerOnRange, OnStart
}

public enum ShootType
{
    None, Single, Radial, DoubleRadial, Burst, RadialBurst, RandomShoot, OrbitalChainShoot
}

public enum EnemyAction
{
    None, TeleportAndShoot
}

public enum ActionTrigger
{
    None, Hitted
}