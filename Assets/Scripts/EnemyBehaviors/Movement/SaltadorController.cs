using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SaltadorController : MonoBehaviour
{
    [Header("Components")]
    Enemy enemy;

    [Header("Target")]
    public Transform target;
    public LayerMask targetMask;
    public float detectionRange = 15;
    public float minimumRange = 5;

    [Header("Movement")]
    Vector2 targetDirection;
    float targetDistance;
    bool canMove = true;
    public float wallRange = .25f;

    [Header("Jump")]
    public AnimationCurve jumpCurve;
    [SerializeField] private bool moveHorizontally;
    [SerializeField] private AnimationCurve horizontalCurve;
    public Transform graphics;
    private float _jumpDelta;
    private float _horizontalDelta;

    [SerializeField] bool jumping = false;
    [SerializeField] bool willJump = false;
    [SerializeField] bool onAir = false;
    [SerializeField] bool isAttacking = false;
    [SerializeField] bool canAttack = false;
    [SerializeField] bool canComeDown = false;

    [Header("Attack")]
    public Transform shadow;
    float shadowSize = 2;
    public float attackDelay;
    public float getDownTimer = 2;
    float downTime;
    bool canCount = false;

    [Header("Shoot Settings")]
    [SerializeField] private bool willShoot;
    [SerializeField] private bool followTarget;
    [SerializeField] private Projectile projectilePrefab;
    [SerializeField] private float projectileDamage;
    [SerializeField] private float projectileSpeed;
    [SerializeField] private float projectileSpawnOffset = 0f;
    [SerializeField] private int radialCount;
    [SerializeField] private float radialShootRadius;
    [SerializeField] private float defaultRotation;
    private bool _canShoot;

    Rigidbody2D rb;
    Vector2 dashDirection;

    Vector3 graphicsStartPosition;
    Vector3 shadowStartPosition;
    Vector2 colliderOffset;

    NavMeshAgent agent;
    CircleCollider2D col;

    [Header("Animation")]
    [SerializeField]string currentState;
    [SerializeField] AnimationCurve fallCurve;
    Animator anim;

    [Header("Sounds")]
    public AudioClip jumpSound;
    public AudioClip landSound;
    [SerializeField] private AudioClip shootClip;

    const string IDLE = "Idle";
    const string MOVE = "Move";
    const string ATTACK = "Attack";

    // Start is called before the first frame update
    void Start()
    {
        enemy = GetComponent<Enemy>();

        SetupAgent();

        anim = GetComponent<Animator>();
        col = GetComponent<CircleCollider2D>();
        agent = GetComponent<NavMeshAgent>();

        graphicsStartPosition = graphics.localPosition;
        shadowStartPosition = shadow.localPosition;
        colliderOffset = col.offset;

        var horizontalDuration = horizontalCurve.keys[horizontalCurve.length - 1].time;
        _horizontalDelta = horizontalDuration * 0.5f;

        downTime = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (feltOnHole) return;

        if (enemy.target && !target)
            target = enemy.target;

        if (jumping && !isAttacking)
        {
            _jumpDelta += Time.deltaTime;
            _horizontalDelta += Time.deltaTime;

            var verticalOffset = jumpCurve.Evaluate(_jumpDelta);
            var horizontalOffset = 0f;

            if (moveHorizontally)
                horizontalOffset = horizontalCurve.Evaluate(_horizontalDelta);

            graphics.localPosition = new Vector3(graphicsStartPosition.x + horizontalOffset, graphicsStartPosition.y + verticalOffset, 0);
            col.offset = new Vector2(colliderOffset.x + horizontalOffset, colliderOffset.y + verticalOffset);
            shadow.localPosition = new Vector3(shadowStartPosition.x + horizontalOffset, shadowStartPosition.y);
        }

        onAir = graphics.localPosition.y > 0 ? true : false;
        if (onAir)
        {
            onGroundTimer = 0;
        }
        else
		{
            onGroundTimer += Time.deltaTime;

            if (!willJump && !isAttacking)
            {
                if (onGroundTimer < 0.1f)
                {
                    ChangeAnimationState(IDLE);
                }
            }
        }

        if (canCount)
        {
            if (downTime < getDownTimer)
			{
                downTime += Time.deltaTime;
			}
        }

        shadowSize = Mathf.Lerp(2, 1f, (graphics.localPosition.y / 10));
        shadow.localScale = new Vector3(shadowSize, shadowSize, shadowSize);
    }

    [SerializeField] float onGroundTimer;
    private void FixedUpdate()
    {
        FallOnHole();
        if (feltOnHole)
        {
            feltCurve += Time.deltaTime;
            graphics.transform.localScale = Vector3.one * fallCurve.Evaluate(feltCurve * 1.5f);
            if (graphics.transform.localScale.x <= 0)
            {
                enemy.onDie();
            }
            return;
        }

        if (target == null) return;

        targetDistance = Vector2.Distance(transform.position, target ? target.position : transform.position);

        if (targetDistance <= detectionRange && targetDistance > minimumRange)
        {
            if (!willJump && currentState.Equals(IDLE))
            {
                StartJump();
            }

            if (onAir)
            {
                Move();
            }
        } 
        else if (targetDistance <= minimumRange)
        {
            StopJump();

            if (onAir)
            {
                Move();
			}
			else
			{
                if (onGroundTimer > 0.1f)
				{
                    JumperAttack();
				}
			}
        }

        if (canAttack) graphics.localPosition = new Vector3(graphics.localPosition.x, graphics.localPosition.y + Time.deltaTime * 20, 0);

        if (!canAttack)
        {
            if (!canComeDown)
            {
                if (targetDistance <= .25f || downTime >= getDownTimer)
                {
                    if (!OverlappingHoleOrObstacle())
                    {
                        canComeDown = true;
                    }
                }
            }
            else
            {
                ComeDown();
            }
        }

        agent.isStopped = currentState.Equals(IDLE);
	}

    [SerializeField]float feltCurve;
    bool feltOnHole;
    void FallOnHole()
    {
        if (feltOnHole) return;

        if (!onAir)
        {
            Collider2D hole = GetOverlappingHole();
            if (hole)
            {
                //enemy.onDie();
                graphics.transform.localPosition = new Vector3(0, 0, 0);
                transform.position = hole.transform.position + new Vector3(.5f, .5f, 0);
                feltOnHole = true;
				//RotateOverLifetime transformTool = graphics.gameObject.AddComponent<RotateOverLifetime>();
                //Destroy(shadow.gameObject);
                shadow.GetComponent<SpriteRenderer>().enabled = false;
				//transformTool.rotateSpeed = 270;
                agent.enabled = false;
                col.enabled = false;
				//transformTool.rescalingSpeed = .2f;
			}
        }
    }

    void Move()
    {
        // if (!canMove) return;
        if (canMove)
		{
            agent.destination = enemy.GetPlayer().transform.position;
		}
	}

    public void Jump()
    {
        _jumpDelta = 0;
        jumping = true;
    }

    public void ResetHorizontalDelta()
    {
        _horizontalDelta = 0f;
    }

    void JumperAttack()
    {
        agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
        isAttacking = true;

        ChangeAnimationState(ATTACK);
    }

    Collider2D GetOverlappingHole()
	{
        foreach(Collider2D hole in Physics2D.OverlapCircleAll(shadow.transform.position, .05f, LayerMask.GetMask("Holes")))
		{
            return hole;
		}
        return null;
	}

    bool OverlappingHoleOrObstacle()
    {
        return Physics2D.OverlapCircle(shadow.transform.position, .05f, LayerMask.GetMask("Obstacles", "Holes"));
    }

    void ComeDown()
    {
        if (isAttacking)
        {
            if (graphics.localPosition.y > 0)
            {
                graphics.localPosition = new Vector3(graphics.localPosition.x, graphics.localPosition.y - Time.deltaTime * 25, 0);
                GetComponentInChildren<SpriteRenderer>().enabled = true;
                canMove = false;
                _canShoot = true;
            }
            else
            {
                graphics.localPosition = Vector3.zero;
                //agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
                agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
                col.enabled = true;

                canMove = true;
                willJump = false;
                isAttacking = false;
                canComeDown = false;

                RadialShoot();
            }

            canCount = false;
            downTime = 0;
        }
    }

    private void RadialShoot()
    {
        if (!willShoot || !_canShoot)
            return;
        
        var hasPlayer = enemy.GetTarget() != null;
		if (!hasPlayer && followTarget)
			return;
        
		var angle = followTarget ? Toolkit2D.GetAngleBetweenTwoPoints(transform.position, target.position) - radialShootRadius / 2 : defaultRotation;
		var angleStep = radialShootRadius / (radialCount - 1);

		var startPoint = transform.position;

        AudioClip bulletClip = null;

		for (int i = 0; i < radialCount; i++)
		{
			float bulletDirXPosition = startPoint.x + Mathf.Sin((angle * Mathf.PI) / 180) * radialShootRadius;
			float bulletDirYPosition = startPoint.y + Mathf.Cos((angle * Mathf.PI) / 180) * radialShootRadius;

			var projectileVector = new Vector2(bulletDirXPosition, bulletDirYPosition);
			var projectileMoveDir = (projectileVector - (Vector2)startPoint).normalized;

			var projectile = GameObject.Instantiate(projectilePrefab, (Vector2)transform.position + (projectileMoveDir * projectileSpawnOffset), Quaternion.identity);
			projectile.Setup((Vector2)transform.position + projectileMoveDir, projectileDamage, projectileSpeed, transform);

            if (bulletClip == null)
				bulletClip = projectile.GetComponent<AudioSource>().clip;

			angle += angleStep;
		}

        enemy.audioSource.PlayOneShot(bulletClip);

        _canShoot = false;
    }

    void DetectCollision()
    {
        RaycastHit2D hitUp = Physics2D.Raycast(transform.position, -targetDirection, wallRange, targetMask);

        Debug.DrawRay(transform.position, -targetDirection * wallRange, Color.blue);

        if (hitUp.transform) canMove = false;
        else canMove = true;
    }

    public void StartJump()
    {
        ChangeAnimationState(MOVE);
        willJump = true;
    }

    public void StopJump()
    {
        willJump = false;
    }

    public void SetCanAttack()
    {
        canCount = true;
        canAttack = true;
        col.enabled = false;
    }

    public void CanAttackOff()
    {
        GetComponentInChildren<SpriteRenderer>().enabled = false;
        canAttack = false;
        //StartCoroutine(AttackFalse(attackDelay));
    }

    IEnumerator AttackFalse(float duration)
    {
        yield return new WaitForSeconds(duration);

        //isAttacking = false;
        canAttack = true;
    }

    public void ChangeAnimationState(string newState)
    {
        if (currentState == newState) return;

        anim.Play(newState);

        currentState = newState;

        if (currentState == IDLE)
		{
            PlayLandSound();
		}
		else
		{
            PlayJumpSound();
		}
    }

    private void SetupAgent()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateUpAxis = false;
        agent.updateRotation = false;
        agent.speed = enemy.speed;
    }

    public void SetAgressive()
    {
    }

    // Called in animation
    public void PlayJumpSound()
	{
        enemy.audioSource.PlayOneShot(jumpSound);
    }

    // Called in animation
    public void PlayLandSound()
	{
        enemy.audioSource.PlayOneShot(landSound);
    }

    public void OnHitted()
    {

    }
}
