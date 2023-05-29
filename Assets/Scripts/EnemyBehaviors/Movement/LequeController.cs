using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LequeController : MonoBehaviour
{
    [Header("Components")]
    Enemy enemy;

    [Header("Target")]
    public Transform target;
    public float detectionRange = 15;
    public float minimumRange = 5;

    [Header("Movement")]
    bool isMoving = false;
    Vector2 targetDirection;
    float targetDistance;

    public float moveTime = 5;
    float timer = 0;
    int randomMove = 0;
    Vector2 neutralDirection;
    bool canMove = true;

    [Header("Animation")]
    Animator anim;
    string currentState;

    [Header("Aim")]
    public Transform aimTransform;

    [Header("Fire")]
    public GameObject projectilePrefab;
    public Transform coneAim;
    public float bulletSpeed = 5;
    public float attackWaitTime = 1;
    float fireRate;
    float fireTimer;
    float attackTimer;
    bool isAttacking;

    const string IDLE = "leque_idle";
    const string ATTACK = "leque_attack";

    // Start is called before the first frame update
    void Start()
    {
        enemy = GetComponent<Enemy>();
        anim = GetComponent<Animator>();

        //fireRate = 1 / (shotsPerMinute / 60);
        fireRate = 1.083f;
        fireTimer = fireRate;
        attackTimer = 0;

        print(fireRate);
    }

    // Update is called once per frame
    void Update()
    {
        if (enemy.target && !target)
            target = enemy.target;

        FindTargetDirection();
        Aim();

        if (!isAttacking) ChangeAnimationState(IDLE);
    }

    private void FixedUpdate()
    {
        targetDistance = Vector2.Distance(transform.position, target ? target.position : transform.position);

        if (targetDistance < detectionRange && targetDistance > minimumRange)
        {
            isMoving = true;

            attackTimer = 0;

            Move();
        }
        else if (targetDistance <= minimumRange)
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

    void FindTargetDirection()
    {
        Vector2 direction = (transform.position - target.position).normalized;

        targetDirection = -direction;
    }

    void Move()
    {
        Vector2 direction = (transform.position - target.position).normalized;

        transform.Translate(-direction * enemy.speed * Time.deltaTime);
    }

    void SelectDirection()
    {
        int x = Random.Range(-1, 1);
        int y = Random.Range(-1, 1);

        neutralDirection = new Vector2(x, y);

        timer = 0;
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

    void Aim()
    {
        Vector2 aimDir = -(aimTransform.position - target.position).normalized;

        float angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;

        aimTransform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    void Shoot()
    {
        if (fireTimer < fireRate) return;

        // Shoot in a cone
        ChangeAnimationState(ATTACK);

        fireTimer = 0;
    }

    public void Fire()
    {
        for (int i = 0; i < coneAim.childCount; i++)
        {
            Vector2 shotDirection = -(aimTransform.position - coneAim.GetChild(i).position).normalized;

            GameObject projectile = Instantiate(projectilePrefab, aimTransform.position, Quaternion.identity);
            projectile.GetComponent<Rigidbody2D>().AddForce(shotDirection * bulletSpeed, ForceMode2D.Impulse);
        }
    }

    public void StopFire()
    {
        isAttacking = false;
    }

    public void ChangeAnimationState(string newState)
    {
        if (currentState == newState) return;

        anim.Play(newState);

        currentState = newState;
    }
}
