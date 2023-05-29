using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallSpiderController : MonoBehaviour
{
    [Header("Animation")]
    [SerializeField] private bool hasDirectionalAnimation = false;
    [SerializeField] private string dirAnimationString;

    [Header("AI")]
    [SerializeField] private float moveSpeed = 2;
    [SerializeField] private float wallRange = .5f;
    [SerializeField] private float playerRange = 3f;
    [SerializeField] private bool moveWhileShooting = true;
    Vector3 moveDir;
    float currentMoveModifier;

    [Header("Attack")]
    [SerializeField] private bool targetPlayer = true;
    [SerializeField] private bool enableLightOnAttack;
    public Projectile projectilePrefab;
    public Transform projectileSpawnPoint;
    public float projectileDamage;
    public float projectileSpeed;
    public float projectileCount;
    public float projectileBurstInterval;
    public float attackInterval;
    [SerializeField] private float waitAfterAttack = 0f;

    bool awake;
    bool isAggressive;
    private bool _isShooting;
    CircleCollider2D circleCol;
    Animator animator;
    Enemy enemy;

    float stoppedTime;
    float switchDirectionTimer;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
            animator = GetComponentInChildren<Animator>();
        
        circleCol = GetComponent<CircleCollider2D>();
        enemy = GetComponent<Enemy>();

        moveSpeed = moveSpeed * Random.Range(.8f, 1.2f);
        currentMoveModifier = 1;
        if (Random.value < .5f) currentMoveModifier = -1;

        stoppedTime = Random.Range(1.5f, 2.5f);
        //StartCoroutine(SwitchDirection(new Vector2(3, 4)));
	}

    private void FixedUpdate()
    {
        //moveDir = GetMoveDirection();

        moveDir = GetMovementAxis() * currentMoveModifier;

        if (isAggressive)
        {
            if (awake)
            {
                if (CanMove() && !OverlapingEnemy() && !WallInFront() && !HittingGate())
                {
                    //transform.Translate(transform.right * currentMoveModifier * Time.deltaTime);
                    transform.position = transform.position + moveDir * moveSpeed * Time.deltaTime;

                    WalkAnimation(true);

                    TryBreakObstacles(1.25f);
                }
                else
				{
                    WalkAnimation(false);

                    switchDirectionTimer += Time.deltaTime;
                }

                if (switchDirectionTimer > stoppedTime)
                {
                    currentMoveModifier *= -1;
                    stoppedTime = Random.Range(1.5f, 2.5f);
                    switchDirectionTimer = 0;
                }
            }
        }
		else
		{
            if (PlayerInRange())
            {
                SetAggressive();
            }
        }
    }

    private void WalkAnimation(bool isMoving)
    {
        if (!hasDirectionalAnimation)
            return;

        if (!isMoving)
        {
            animator.SetInteger(dirAnimationString, 0);
            return;
        }

        int dirParam = (int)currentMoveModifier;

        if (transform.localRotation.eulerAngles.z <= 90f)
            dirParam *= -1;

        animator.SetInteger(dirAnimationString, dirParam);
    }

    IEnumerator SwitchDirection(Vector2 stopInterval)
	{
        yield return new WaitForSeconds(Random.Range(stopInterval.x, stopInterval.y));

        currentMoveModifier *= -1;

        StartCoroutine(SwitchDirection(stopInterval));
    }

    bool PlayerInRange()
	{
        return Vector2.Distance(transform.position, enemy.GetNearPlayer().transform.position) < playerRange;

    }

    private bool HittingGate()
	{
        foreach(Collider2D col in Physics2D.OverlapCircleAll(transform.position, circleCol.radius * 1.6f))
		{
            if (col.GetComponentInParent<Gate>())
			{
                return true;
			}
		}

        return false;
	}

    private IEnumerator BurstShoot(float wait)
    {
        
        yield return new WaitForSeconds(wait);
        _isShooting = true;

        if (PlayerInRange())
        {
            var target = targetPlayer ? enemy.GetNearPlayer().transform.position : projectileSpawnPoint.position - projectileSpawnPoint.up;

            if (enableLightOnAttack)
            {
                if (GetComponent<EnemyLightsController>())
                    GetComponent<EnemyLightsController>().StartLoop();
            }

            for (int i = 0; i < projectileCount; i++)
            {
                Projectile projectile = GameObject.Instantiate(projectilePrefab, projectileSpawnPoint.position, Quaternion.identity);
                projectile.Setup(target, projectileDamage, projectileSpeed, transform);

                var orbitalProjectile = projectile as OrbitalProjectile;
                if (orbitalProjectile != null)
                {
                    orbitalProjectile.DecreaseMaxDistance((orbitalProjectile.GetMaxDistance() - .5f) / projectileCount * i);
                    orbitalProjectile.SetAutoDestroyMultiplier(orbitalProjectile.GetMaxDistance());
                }

                yield return new WaitForSeconds(projectileBurstInterval);
            }
        }

        if (waitAfterAttack > 0f)
            yield return new WaitForSeconds(waitAfterAttack);

        _isShooting = false;
        StartCoroutine(BurstShoot(attackInterval * Random.Range(.75f, 1.25f)));
    }

    bool CanMove()
	{
        if (!moveWhileShooting && _isShooting)
            return false;
        
        Vector3 circleOrigin = transform.position + new Vector3(moveDir.x, moveDir.y, 0) * wallRange;

        return HittingWall(circleOrigin, .05f);
	}

    bool WallInFront()
	{
        Vector2 origin = new Vector2(projectileSpawnPoint.position.x + moveDir.x * .5f, projectileSpawnPoint.position.y + moveDir.y * .5f);
        return HittingWall(origin, .05f);
    }

    bool OverlapingEnemy()
	{
        Vector3 circleOrigin = transform.position + new Vector3(moveDir.x, moveDir.y, 0) * wallRange;

        return Physics2D.OverlapCircle(circleOrigin, .15f, LayerMask.GetMask("Enemies"));
    }

    bool HittingWall(Vector2 origin, float radius)
	{
        return Physics2D.OverlapCircle(origin, radius, LayerMask.GetMask("Walls"));
    }

    Vector2 GetMovementAxis()
	{
        float zRot = Mathf.Abs(transform.eulerAngles.z);
        if (zRot == 0 || zRot == 180)
        {
            return Vector2.right;
        }
        else
        {
            return Vector2.up;
        }
    }

    Vector2 GetMoveDirection()
	{
        Vector2 axis = GetMovementAxis();

        if (axis == Vector2.right)
		{
            // Player is right from this
            if (enemy.GetNearPlayer().transform.position.x > transform.position.x)
			{
                currentMoveModifier = -1;
            }
            else
            {
                currentMoveModifier = 1;
            }
        }
		else
		{
            // Player is right from this
            if (enemy.GetNearPlayer().transform.position.y > transform.position.y)
            {
                currentMoveModifier = -1;
            }
            else
            {
                currentMoveModifier = 1;
            }
        }

        return axis * currentMoveModifier;
	}

    // Called in Enemy: onDamage
    public void OnHitted()
    {
        if (!isAggressive)
        {
            SetAggressive();
        }
    }

    public void SetAggressive()
	{
        if (!this.enabled)
            return;
        
        isAggressive = true;
        animator.SetTrigger("WakeUp");
        StartCoroutine(BurstShoot(attackInterval * Random.Range(.75f, 1.25f) * .5f));
        StartCoroutine(CallTryBreakObstacles(.7f));
        Invoke("SetAwake", 1);
    }

    void SetAwake()
	{
        awake = true;
	}

    IEnumerator CallTryBreakObstacles(float delay)
	{
        yield return new WaitForSeconds(delay);
        TryBreakObstacles(1.6f);
    }

    public void TryBreakObstacles(float radiusMod)
	{
        foreach(Collider2D col in Physics2D.OverlapCircleAll(circleCol.bounds.center, circleCol.radius * radiusMod, LayerMask.GetMask("Obstacles")))
		{
            BreakableObject breakable = col.GetComponent<BreakableObject>();
            if (breakable)
			{
                breakable.OnDamage(99999);
            }
		}
	}
}
