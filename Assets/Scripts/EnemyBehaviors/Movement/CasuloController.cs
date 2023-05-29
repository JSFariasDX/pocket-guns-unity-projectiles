using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CasuloController : MonoBehaviour
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

    // Start is called before the first frame update
    void Start()
    {
        enemy = GetComponent<Enemy>();


        fireRate = 1 / (shotsPerMinute / 60);
        fireTimer = fireRate;
        attackTimer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (enemy.target && !target)
            target = enemy.target;

        if (target)
        {
            FindTargetDirection();

            targetDistance = Vector2.Distance(transform.position, target ? target.position : transform.position);
        }

        if (targetDistance <= detectionRange)
        {
            if (attackTimer < attackWaitTime)
                attackTimer += Time.deltaTime;
        }
        else
        {
            attackTimer = 0;
        }

        if (attackTimer >= attackWaitTime)
        {
            if (fireTimer < fireRate)
                fireTimer += Time.deltaTime;

            Shoot();
        }
    }

    void FindTargetDirection()
    {
        if (target == null) return;

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
}
