using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SorridenteController : MonoBehaviour
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
    int randomMove = 0;
    Vector2 neutralDirection;
    bool canMove = true;

    [Header("Teleport")]
    public bool isTeleporting = false;
    float teleportRange;

    [Header("Animation")]
    Animator anim;
    string currentState;

    [Header("Aim")]
    public Transform aimTransform;

    [Header("Fire")]
    public GameObject projectilePrefab;
    public float bulletSpeed = 5;

    const string IDLE = "sorridente_idle";
    const string IDLE_BACK = "sorridente_idle_back";
    const string IDLE_RIGHT = "sorridente_idle_side";

    const string TELEPORT = "sorridente_teleport";

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

        teleportRange = minimumRange + ((detectionRange - minimumRange) / 2);
        print(teleportRange);
    }

    // Update is called once per frame
    void Update()
    {
        if (enemy.target && target == null)
            target = enemy.target;

        if(!isTeleporting)
            FaceDirection();

        FindTargetDirection();
        DetectCollision();
    }

    private void FixedUpdate()
    {
        if (target == null) return;

        targetDistance = Vector2.Distance(transform.position, target ? target.position : transform.position);

        if (targetDistance < detectionRange && targetDistance > minimumRange)
        {
            //attackTimer = 0;

            if (isMoving && !isTeleporting)
                Move();

            if (targetDistance > minimumRange && targetDistance < teleportRange)
                StartTeleport();

            
        }
        else if (targetDistance <= minimumRange)
        {
            isMoving = false;

            //if (attackTimer < attackWaitTime)
            //    attackTimer += Time.deltaTime;

            //if (attackTimer >= attackWaitTime)
            //{
            //    if (fireTimer < fireRate)
            //        fireTimer += Time.deltaTime;

            //    Shoot();
            //}

            //StartTeleport();

            if (isTeleporting)
            {
                if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
                    isTeleporting = false;
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
                ChangeAnimationState(IDLE_BACK);
                transform.localScale = new Vector3(1, 1, 1);
                break;
            case Facing.down:
                ChangeAnimationState(IDLE);
                transform.localScale = new Vector3(1, 1, 1);
                break;
            case Facing.left:
                ChangeAnimationState(IDLE_RIGHT);
                transform.localScale = new Vector3(-1, 1, 1);
                break;
            case Facing.right:
                ChangeAnimationState(IDLE_RIGHT);
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

        if (targetAngle > 0 && targetAngle < 90) // Left
        {
            facingDirection = Facing.left;
        }
        else if (targetAngle > 90 && targetAngle < 180) // Down
        {
            facingDirection = Facing.down;
        }
        else if (targetAngle > 180 && targetAngle < 270) // Right
        {
            facingDirection = Facing.right;
        }
        else if (targetAngle > 270 && targetAngle < 360) // Up
        {
            facingDirection = Facing.up;
        }
    }

    void StartTeleport()
    {
        isTeleporting = true;

        ChangeAnimationState(TELEPORT);

        //Vector3 point = Random.insideUnitCircle.normalized * Random.Range(innerRadius, outerRadius);
        //Vector3 point = RandomOnEdge();

        //transform.position = new Vector3(point.x, point.y, 0);


    }

    public void Teleport()
    {
        Vector3 point = RandomOnEdge();

        transform.position = new Vector3(point.x, point.y, 0);
    }

    void DetectCollision()
    {
        RaycastHit2D hitUp = Physics2D.Raycast(transform.position, -targetDirection, wallRange, targetMask);

        Debug.DrawRay(transform.position, -targetDirection * wallRange, Color.blue);

        if (hitUp.transform)
            isMoving = false;
        else
            isMoving = true;

        //Collider2D[] col = Physics2D.OverlapCircleAll(transform.position, wallRange, targetMask);

        //if (col.Length > 0)
        //    isMoving = false;
        //else
        //    isMoving = true;

        //foreach (Collider2D item in col)
        //{
        //    if (item.transform != null)
        //    {
        //        if (item.transform.CompareTag("Player"))
        //        {
        //            item.transform.GetComponent<Health>().Decrease(10);
        //        }

        //        //ResetDirection();
        //    }
        //}
    }

    void SelectDirection()
    {
        int x = Random.Range(-1, 1);
        int y = Random.Range(-1, 1);

        neutralDirection = new Vector2(x, y);

        timer = 0;
    }

    void Move()
    {
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

    public void Shoot()
    {
        //if (fireTimer < fireRate) return;

        Vector2 direction = (aimTransform.position - target.position).normalized;

        GameObject projectile = Instantiate(projectilePrefab, aimTransform.position, Quaternion.identity);
        projectile.GetComponent<Rigidbody2D>().AddForce(-direction * bulletSpeed, ForceMode2D.Impulse);

        //fireTimer = 0;
    }

    private void SpawnSphereOnEdgeRandomly2D()
    {
        float radius = 3f;
        Vector3 randomPos = Random.insideUnitSphere * radius;
        randomPos += transform.position;
        randomPos.y = 0f;

        Vector3 direction = randomPos - transform.position;
        direction.Normalize();

        float dotProduct = Vector3.Dot(transform.forward, direction);
        float dotProductAngle = Mathf.Acos(dotProduct / transform.forward.magnitude * direction.magnitude);

        randomPos.x = Mathf.Cos(dotProductAngle) * radius + transform.position.x;
        randomPos.y = Mathf.Sin(dotProductAngle * (Random.value > 0.5f ? 1f : -1f)) * radius + transform.position.y;
        randomPos.z = transform.position.z;

        //GameObject go = Instantiate(_spherePrefab, randomPos, Quaternion.identity);
        //go.transform.position = randomPos;
    }

    Vector3 RandomOnEdge()
    {
        float radius = minimumRange;
        Vector3 randomPos = Random.insideUnitSphere * radius;
        randomPos += target.position;
        randomPos.y = 0f;

        Vector3 direction = randomPos - target.position;
        direction.Normalize();

        float dotProduct = Vector3.Dot(target.forward, direction);
        float dotProductAngle = Mathf.Acos(dotProduct / target.forward.magnitude * direction.magnitude);

        randomPos.x = Mathf.Cos(dotProductAngle) * radius + target.position.x;
        randomPos.y = Mathf.Sin(dotProductAngle * (Random.value > 0.5f ? 1f : -1f)) * radius + target.position.y;
        randomPos.z = target.position.z;

        return randomPos;
    }

    public void ChangeAnimationState(string newState)
    {
        if (currentState == newState) return;

        anim.Play(newState);

        currentState = newState;
    }
}
