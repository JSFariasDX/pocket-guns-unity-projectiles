using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallSpiderBackup : MonoBehaviour
{
    [Header("AI")]
    [SerializeField] private float moveSpeed = 2;
    [SerializeField] private float wallRange = .5f;
    [SerializeField] private float playerRange = 3f;
    Vector3 moveDir;
    float currentMoveModifier;

    [Header("Attack")]
    public Projectile projectilePrefab;
    public Transform projectileSpawnPoint;
    public float projectileDamage;
    public float projectileSpeed;
    public float projectileCount;
    public float projectileBurstInterval;
    public float attackInterval;

    bool awake;
    bool isAggressive;
    CircleCollider2D circleCol;
    Animator animator;
    Enemy enemy;
    Vector2 limits;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        circleCol = GetComponent<CircleCollider2D>();
        enemy = GetComponent<Enemy>();

        moveSpeed = moveSpeed * Random.Range(.8f, 1.2f);

        currentMoveModifier = 1;
        if (Random.value < .5f) currentMoveModifier = -1;
    }

    private void FixedUpdate()
    {
        //moveDir = GetMovementDirection();

        if (isAggressive)
        {
            if (awake)
            {
                Vector2 axis = GetMovementAxis();
                Vector3 moveAxis = axis;

                if (CanMove())
                {
                    transform.position = transform.position + moveAxis * currentMoveModifier * moveSpeed * Time.deltaTime;
                }
                else
                {
                    currentMoveModifier *= -1;
                }

                //            if ((limits.x == 0 && currentMoveModifier < 0) || (limits.y == 0 && currentMoveModifier > 0))
                //{
                //                if (CanMove() && !OverlapingEnemy() && !WallInFront() && !HittingGate())
                //                {
                //                }
                //	else
                //	{
                //                    if (currentMoveModifier < 0)
                //		{
                //                        if (axis == Vector2.right) limits.x = transform.position.x;
                //                        else limits.x = transform.position.y;
                //		}
                //		else
                //		{
                //                        if (axis == Vector2.right) limits.y = transform.position.x;
                //                        else limits.y = transform.position.y;
                //                    }
                //	}
                //}
                //else
                //{
                //                if (axis == Vector2.right)
                //	{
                //                    if (transform.position.x < limits.x) currentMoveModifier = 1;
                //                    else if (transform.position.x > limits.y) currentMoveModifier = -1;
                //	}
                //	else
                //	{
                //                    if (transform.position.y < limits.x) currentMoveModifier = 1;
                //                    else if (transform.position.y > limits.y) currentMoveModifier = -1;
                //                }
                //            }
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

    bool PlayerInRange()
    {
        return Vector2.Distance(transform.position, enemy.GetNearPlayer().transform.position) < playerRange;

    }

    private bool HittingGate()
    {
        foreach (Collider2D col in Physics2D.OverlapCircleAll(transform.position, circleCol.radius * 1.6f))
        {
            if (col.GetComponent<Gate>())
            {
                return true;
            }
        }

        return false;
    }

    private IEnumerator BurstShoot(float wait)
    {
        yield return new WaitForSeconds(wait);

        if (PlayerInRange())
        {

            Player targetPlayer = enemy.GetNearPlayer();
            for (int i = 0; i < projectileCount; i++)
            {
                Projectile projectile = GameObject.Instantiate(projectilePrefab, projectileSpawnPoint.position, Quaternion.identity);
                projectile.Setup(targetPlayer.transform.position, projectileDamage, projectileSpeed, transform);

                yield return new WaitForSeconds(projectileBurstInterval);
            }
        }

        StartCoroutine(BurstShoot(attackInterval * Random.Range(.75f, 1.25f)));
    }

    bool CanMove()

    {
        Vector3 circleOrigin = transform.position + new Vector3(moveDir.x, moveDir.y, 0) * wallRange;

        return !OverlapingEnemy() && !WallInFront() && !HittingGate() && HittingWall(circleOrigin, .05f) && !HittingObstacle();
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

    bool HittingObstacle()
    {
        return Physics2D.OverlapCircle(transform.position, circleCol.radius * 1.6f, LayerMask.GetMask("Obstacles"));
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

    Vector2 GetMovementDirection()
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
        foreach (Collider2D col in Physics2D.OverlapCircleAll(circleCol.bounds.center, circleCol.radius * radiusMod, LayerMask.GetMask("Obstacles")))
        {
            BreakableObject breakable = col.GetComponent<BreakableObject>();
            if (breakable)
            {
                breakable.OnDamage(99999);
            }
        }
    }
}
