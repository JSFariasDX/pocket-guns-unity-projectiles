using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MuncherController : MonoBehaviour
{
    [Header("Components")]
    Enemy enemy;

    [Header("Target")]
    public Transform target;
    public float detectionRange = 15;
    public float shootRange = 5;

    [Header("Movement")]
    Vector3 patrolDirection;
    float changePatrolDirectionTimer;

    [Header("Aim")]
    public Transform aimTransform;

    [Header("Fire")]
    public GameObject projectilePrefab;
    public float bulletSpeed = 5;
    public float shootsPerSecond;
    bool canShoot;

    NavMeshAgent agent;

    // Start is called before the first frame update
    void Start()
    {
        enemy = GetComponent<Enemy>();
        SetupAgent();

        canShoot = true;
    }

    private void Update()
    {
        if (enemy.GetPlayer() != null)
        {
            AggressiveControl();
        }
        else
        {
            Patrol();
        }
    }

    private bool OverlapingEnemy()
	{
        foreach(RaycastHit2D enemy in Physics2D.RaycastAll(transform.position + agent.velocity, agent.velocity, .5f, LayerMask.GetMask("Enemies", "FlyEnemies")))
		{
            if (enemy.collider.gameObject != gameObject)
            {
                if (!enemy.collider.GetComponent<NavMeshAgent>().isStopped)
				{
                    return true;
				}
            }
		}

        return false;
	}

    private void AggressiveControl()
    {
        float playerDistance = Vector3.Distance(transform.position, enemy.GetPlayer().transform.position);
        bool isPlayerInRange = playerDistance < detectionRange;

        if (isPlayerInRange)
        {
            if (playerDistance > shootRange)
            {
                //agent.isStopped = false;
                ChasePlayer();
            }
            else
            {
                agent.isStopped = true;
                if (target && canShoot)
                {
                    StartCoroutine(Shoot(1 / shootsPerSecond));
                }
            }
        }
    }

    void ChasePlayer()
    {
        agent.isStopped = false;
        target = enemy.GetPlayer().transform;
        agent.destination = target.transform.position;
    }

    void SelectPatrolDirection()
    {
        if (changePatrolDirectionTimer <= -2f)
        {
            // Reset timer
            changePatrolDirectionTimer = Random.Range(3, 5);

            // Switch direction
            float xRandom = Random.Range(-1, 1);
            float yRandom = Random.Range(-1, 1);
            Vector3 randomPosition = transform.position + new Vector3(xRandom, yRandom);
            patrolDirection = randomPosition - transform.position;
            patrolDirection.Normalize();
        }
    }

    void Patrol()
    {
        changePatrolDirectionTimer -= Time.deltaTime;
        if (changePatrolDirectionTimer <= 0)
        {
            agent.isStopped = true;
        }
        else if (changePatrolDirectionTimer <= -1.5f)
        {
            agent.isStopped = false;
            SelectPatrolDirection();
            changePatrolDirectionTimer = 2.5f;
        }
        else
        {
            agent.isStopped = false;
            agent.stoppingDistance = 0;
            agent.destination = transform.position + patrolDirection;
        }
    }

    IEnumerator Shoot(float shootResetTime)
	{
        canShoot = false;

        GameObject projectile = Instantiate(projectilePrefab, aimTransform.position, Quaternion.identity);
        Vector2 targetDirection = (target.transform.position - transform.position).normalized;
        projectile.GetComponent<Rigidbody2D>().AddForce(targetDirection * bulletSpeed, ForceMode2D.Impulse);

        yield return new WaitForSeconds(shootResetTime);

        canShoot = true;
    }

    private void SetupAgent()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateUpAxis = false;
        agent.updateRotation = false;
        agent.speed = enemy.speed;
    }

	private void OnTriggerStay2D(Collider2D collision)
	{
		if (collision.CompareTag("Player"))
		{
            collision.GetComponent<Health>().Decrease(10);
        }
    }
}
