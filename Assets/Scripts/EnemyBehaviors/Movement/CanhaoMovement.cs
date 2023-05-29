using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanhaoMovement : MonoBehaviour
{
    [Header("Components")]
    Enemy enemy;

    [Header("Target")]
    public Transform target;
    float targetAngle;
    public float detectionRange = 15;
    public float minimumRange = 5;
    public LayerMask targetMask;

    [Header("Movement")]
    bool isMoving = false;
    Vector2 targetDirection;
    float targetDistance;
    public float wallRange = .25f;

    public float moveTime = 5;
    float timer = 0;
    Vector2 neutralDirection;
    public float wallDetectionRange = .25f;
    bool canMove = true;

    [Header("Animation")]
    Animator anim;
    string currentState;

    [Header("Aim")]
    public Transform aimTransform;

    [Header("Fire")]
    public GameObject projectilePrefab;
    public float bulletSpeed = 5;
    public float shotsPerMinute;
    public float attackWaitTime = 1;
    float fireRate;
    float fireTimer;
    float attackTimer;

    const string IDLE = "canhão_idle";
    const string IDLE_BACK = "canhão_idle_back";
    const string IDLE_RIGHT = "canhão_idle_side";

    const string MOVE = "canhão_move_front";
    const string MOVE_BACK = "canhão_move_back";
    const string MOVE_RIGHT = "canhão_move_side";

    public enum Facing
    {
        up, down, left, right
    }
    public Facing facingDirection;

    bool PlayerOnRadius()
    {
        if (targetDistance > detectionRange)
            return false;
        else
            return true;
    }

    // Start is called before the first frame update
    void Start()
    {
        enemy = GetComponent<Enemy>();
        anim = GetComponent<Animator>();

        fireRate = 1 / (shotsPerMinute / 60);
        fireTimer = fireRate;
        attackTimer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (enemy.target && !target)
            target = enemy.target;

        FaceDirection();
        FindTargetDirection();
        DetectCollision();
    }

    private void FixedUpdate()
    {
        if (target)
        {
            targetDistance = Vector2.Distance(transform.position, target ? target.position : transform.position);
        }

        if (targetDistance < detectionRange && targetDistance > minimumRange)
        {
            attackTimer = 0;

            if(isMoving)
                Move();
        }
        else if(targetDistance <= minimumRange)
        {
            isMoving = false;

            if (attackTimer < attackWaitTime)
                attackTimer += Time.deltaTime;

            if (attackTimer >= attackWaitTime)
            {
                if (fireTimer < fireRate)
                    fireTimer += Time.deltaTime;

                Shoot();
            }
        }
        else
        {
            if (timer >= moveTime)
                SelectDirection();
            else
                timer += Time.deltaTime;

            NeutralMove();
        }
    }

    void FaceDirection()
    {
        switch (facingDirection)
        {
            case Facing.up:
                if (isMoving)
                {
                    ChangeAnimationState(MOVE_BACK);
                }
                else
                {
                    ChangeAnimationState(IDLE_BACK);
                }
                transform.localScale = new Vector3(1, 1, 1);
                break;
            case Facing.down:
                if (isMoving)
                {
                    ChangeAnimationState(MOVE);
                }
                else
                {
                    ChangeAnimationState(IDLE);
                }
                transform.localScale = new Vector3(1, 1, 1);
                break;
            case Facing.left:
                if (isMoving)
                {
                    ChangeAnimationState(MOVE_RIGHT);
                }
                else
                {
                    ChangeAnimationState(IDLE_RIGHT);
                }
                transform.localScale = new Vector3(-1, 1, 1);
                break;
            case Facing.right:
                if (isMoving)
                {
                    ChangeAnimationState(MOVE_RIGHT);
                }
                else
                {
                    ChangeAnimationState(IDLE_RIGHT);
                }
                transform.localScale = new Vector3(1, 1, 1);
                break;
            default:
                break;
        }
    }

    void FindTargetDirection()
    {
        if (target == null) return;

        Vector2 direction = (transform.position - target.position).normalized;

        targetDirection = -direction;

        float angle = Mathf.Atan2(PlayerOnRadius() ? direction.y : -neutralDirection.y, PlayerOnRadius() ? direction.x : -neutralDirection.x) * Mathf.Rad2Deg;
        angle += 45;
        if (angle < 0) angle += 360;

        targetAngle = angle;

        if(targetAngle > 0 && targetAngle < 90) // Left
        {
            facingDirection = Facing.left;
        } 
        else if(targetAngle > 90 && targetAngle < 180) // Down
        {
            facingDirection = Facing.down;
        }
        else if(targetAngle > 180 && targetAngle < 270) // Right
        {
            facingDirection = Facing.right;
        }
        else if(targetAngle > 270 && targetAngle < 360) // Up
        {
            facingDirection = Facing.up;
        }
    }

    void Move()
    {
        if (target == null)
        {
            return;
        }
        //isMoving = true;

        Vector2 direction = (transform.position - target.position).normalized;

        transform.Translate(-direction * enemy.speed * Time.deltaTime);
    }

    void NeutralMove()
    {
        //Vector2 direction = (transform.position - (Vector3)neutralDirection).normalized;
        if (canMove)
        {
            isMoving = true;
            transform.Translate(neutralDirection * enemy.speed * Time.deltaTime);
        }
        else
        {
            isMoving = false;
        }
    }

    void DetectCollision()
    {
        //Vector2 direction = (transform.position - target.position).normalized;

        //RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, targetMask);

        //if (hit.transform)
        //    isMoving = false;
        //else
        //    isMoving = true;

        Collider2D[] col = Physics2D.OverlapCircleAll(transform.position, wallRange, targetMask);

        if (col.Length > 0)
            isMoving = false;
        else
            isMoving = true;

        foreach (Collider2D item in col)
        {
            if (item.transform != null)
            {
                if (item.transform.CompareTag("Player"))
                {
                    item.transform.GetComponent<Health>().Decrease(10);
                }

                //ResetDirection();
            }
        }
    }

    void SelectDirection()
    {
        int x = Random.Range(-1, 1);
        int y = Random.Range(-1, 1);

        neutralDirection = new Vector2(x, y);

        timer = 0;
    }

    void Shoot()
    {
        if (fireTimer < fireRate) return;

        GameObject projectile = Instantiate(projectilePrefab, aimTransform.position, Quaternion.identity);
        projectile.GetComponent<Rigidbody2D>().AddForce(targetDirection * bulletSpeed, ForceMode2D.Impulse);

        fireTimer = 0;
    }

    public void ChangeAnimationState(string newState)
    {
        if (currentState == newState) return;

        anim.Play(newState);

        currentState = newState;
    }
}
