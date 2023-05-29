using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemeoController : MonoBehaviour
{
    public enum TwinType
    {
        Pai, Filho
    }
    public TwinType twinType;

    [Header("Components")]
    Enemy enemy;
    Health health;

    [Header("Target")]
    public Transform target;
    public float detectionRange = 15;
    public float minimumRange = 5;

    [Header("Move")]
    bool isMoving = false;
    Vector2 targetDirection;
    float targetDistance = 15;

    public float moveTime = 5;
    float timer = 0;
    Vector2 neutralDirection;
    bool canMove = true;

    [Header("Twins")]
    public List<Transform> twinSpawnPoints = new List<Transform>();
    public GameObject twinPrefab;
    bool isDividing = false;

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

    [Header("Animation")]
    Animator anim;
    string currentState;

    const string IDLE = "gemeo_idle";
    const string SPLIT = "gemeo_divide";

    // Start is called before the first frame update
    void Start()
    {
        enemy = GetComponent<Enemy>();
        health = GetComponent<Health>();
        anim = GetComponent<Animator>();

        //target = GameObject.FindGameObjectWithTag("Player").transform;

        if (twinType == TwinType.Pai)
        {
            isDividing = true;
            ChangeAnimationState(SPLIT);
        }

        fireRate = 1 / (shotsPerMinute / 60);
        fireTimer = fireRate;
        attackTimer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (enemy.target && !target)
            target = enemy.target;

        FindTargetDirection();
    }

    private void FixedUpdate()
    {
        targetDistance = Vector2.Distance(transform.position, target ? target.position : transform.position);

        if (targetDistance < detectionRange && targetDistance > minimumRange)
        {
            if (!isDividing)
            {
                isMoving = true;

                attackTimer = 0;

                Move();
            }
        }
        else if (targetDistance <= minimumRange)
        {
            if (twinType == TwinType.Pai)
            {
                isMoving = false;

                if (attackTimer < attackWaitTime)
                    attackTimer += Time.deltaTime;
            }
            else
            {
                if (!isDividing)
                {
                    isMoving = true;

                    attackTimer = 0;

                    Move();
                }
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

        if (twinType == TwinType.Pai)
        {
            if (attackTimer >= attackWaitTime)
            {
                if (fireTimer < fireRate)
                    fireTimer += Time.deltaTime;

                Shoot();
            }
        }
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

    void FindTargetDirection()
    {
        Vector2 direction = (transform.position - target.position).normalized;

        targetDirection = -direction;
    }

    void Shoot()
    {
        if (fireTimer < fireRate) return;

        GameObject projectile = Instantiate(projectilePrefab, aimTransform.position, Quaternion.identity);
        projectile.GetComponent<Rigidbody2D>().AddForce(targetDirection * bulletSpeed, ForceMode2D.Impulse);

        fireTimer = 0;
    }

    public void Death()
    {
        isDividing = true;
        ChangeAnimationState(SPLIT);
    }

    public void DuplicateItself()
    {
        for (int i = 0; i < twinSpawnPoints.Count; i++)
        {
            GameObject twin = Instantiate(twinPrefab, twinSpawnPoints[i].position, Quaternion.identity);
            twin.GetComponent<GemeoController>().target = target;
        }

        Destroy(gameObject);
    }

    public void ChangeAnimationState(string newState)
    {
        if (currentState == newState) return;

        anim.Play(newState);

        currentState = newState;
    }
}
