using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Larva : MonoBehaviour
{
    Enemy enemy;

    public float detectionRange;

    [Header("Move")]
    Vector3 patrolDirection;
    float changePatrolDirectionTimer;

    Transform target;
    NavMeshAgent agent;

    private void Start()
    {
        enemy = GetComponent<Enemy>();
        SetupAgent();
    }

    // Update is called once per frame
    void Update()
    {
        if (enemy.GetPlayer() != null)
        {
            CheckPlayerRange();
        }
        else
        {
            Patrol();
        }
    }

    private void CheckPlayerRange()
    {
        float playerDistance = Vector3.Distance(transform.position, enemy.GetPlayer().transform.position);
        bool isPlayerInRange = playerDistance < detectionRange;
        if (isPlayerInRange)
        {
            agent.isStopped = false;
            ChasePlayer();
        }
    }

    void ChasePlayer()
    {
        target = enemy.GetPlayer().transform;
        agent.destination = target.transform.position;
    }

    void SelectPatrolDirection()
    {
        // Switch direction
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

    private void SetupAgent()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateUpAxis = false;
        agent.updateRotation = false;
        agent.speed = enemy.speed;
    }
}
