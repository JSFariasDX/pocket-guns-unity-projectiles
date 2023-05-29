using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmaduradoController : MonoBehaviour
{
    [Header("Components")]
    Enemy enemy;

    [Header("Target")]
    public Transform target;
    public LayerMask targetMask;
    public Vector2 targetDirection;
    bool isAttacking = false;
    public bool canDetect = true;

    [Header("Parameters")]
    public float windingRate = 1;
    float winding = 0;
    public float coolingRate = 2;
    float cooling = 0;

    [Header("Move")]
    [SerializeField] private float attackSpeed = 20f;
    public float moveTime = 5;
    public float wallDetectionRange = .25f;
    float timer = 0;
    int randomMove = 0;
    Vector2 neutralDirection;
    bool canMove = true;

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

    [Header("Animation")]
    Rigidbody2D rig;
    Animator anim;
    string currentState;

    const string IDLE = "Idle";
    const string WIND = "Winding";
    const string ATTACK = "Attacking";

    void Start()
    {
        rig = GetComponent<Rigidbody2D>();
        enemy = GetComponent<Enemy>();
        anim = GetComponent<Animator>();

        enemy.agentSpeed = enemy.speed;
    }

    void FixedUpdate()
    {
        if (enemy.target && !target)
            target = enemy.target;

        if (canDetect)
        {
            FindTarget();
        }

        if (targetDirection != Vector2.zero)
        {
            if (winding < windingRate)
            {
                winding += Time.deltaTime;
            }
            else
            {
                Attack();
            }
        }
        else
        {
            if (timer >= moveTime)
            {
                SelectDirection();
            }
            else
            {
                timer += Time.deltaTime;
            }

            NeutralMove();
        }
        
        if(!canDetect)
        {
            if (cooling < coolingRate) cooling += Time.deltaTime;
            else canDetect = true;
        }

        if (isAttacking)
        {
            ChangeAnimationState(ATTACK);

            if (targetDirection.x > 0) // facing right
                transform.localScale = new Vector3(-1, 1, 1);
            else
                transform.localScale = new Vector3(1, 1, 1);

            DetectCollision();
        }
        else
        {
            ChangeAnimationState(IDLE);
        }
        
    }

    void SelectDirection()
    {
        randomMove = Random.Range(0, 5);
        timer = 0;
    }

    void NeutralMove()
    {
        switch (randomMove)
        {
            case 0:
                rig.constraints = RigidbodyConstraints2D.FreezeAll;
                neutralDirection = new Vector2(0, 0);
                break;
            case 1:
                rig.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
                neutralDirection = new Vector2(1, 0);
                break;
            case 2:
                rig.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
                neutralDirection = new Vector2(-1, 0);
                break;
            case 3:
                rig.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
                neutralDirection = new Vector2(0, 1);
                break;
            case 4:
                rig.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
                neutralDirection = new Vector2(0, -1);
                break;
            default:
                break;
        }

        if (canMove && canDetect)
        {
            transform.Translate(neutralDirection * enemy.speed * Time.deltaTime);
        }
    }

    void Attack()
    {
        if (!isAttacking)
		{
            enemy.PlayAttackSound();
		}
        isAttacking = true;
        //if(canMove)
        transform.Translate(targetDirection * attackSpeed * Time.deltaTime);
    }

    void FindTarget()
    {
        if (target == null)
        {
            return;
        }

        RaycastHit2D hitUp = Physics2D.Raycast(transform.position, transform.up, 50, targetMask);
        RaycastHit2D hitDown = Physics2D.Raycast(transform.position, -transform.up, 50, targetMask);
        RaycastHit2D hitLeft = Physics2D.Raycast(transform.position, -transform.right, 50, targetMask);
        RaycastHit2D hitRight = Physics2D.Raycast(transform.position, transform.right, 50, targetMask);

        if (isAttacking) return;

        if (hitUp.transform.GetComponent<Player>())
        {
            print(target.gameObject);
            rig.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
            targetDirection = new Vector2(0, 1);
        }

        if (hitDown.transform.GetComponent<Player>())
        {
            print(target.gameObject);
            rig.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
            targetDirection = new Vector2(0, -1);
        }

        if (hitLeft.transform.GetComponent<Player>())
        {
            print(target.gameObject);
            rig.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
            targetDirection = new Vector2(-1, 0);
        }

        if (hitRight.transform.GetComponent<Player>())
        {
            print(target.gameObject);
            rig.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
            targetDirection = new Vector2(1, 0);
        }
    }

    void DetectCollision()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, wallDetectionRange * 0.9f, targetMask);

        if (hit)
        {
            RadialShoot();
            ResetDirection();
        }
    }

    void ResetDirection()
    {
        targetDirection = Vector2.zero;
        winding = 0;
        cooling = 0;
        isAttacking = false;
        canDetect = false;
    }

    private void RadialShoot()
    {
        if (!willShoot)
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
    }

	private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == targetMask)
		{
            ResetDirection();
        }
    }

	public void ChangeAnimationState(string newState)
    {
        if (currentState == newState) return;

        anim.Play(newState);

        currentState = newState;
    }

    private void OnDrawGizmosSelected()
	{
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, wallDetectionRange);
	}

    private void OnHitted()
    {
    }
}
