using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NeutralStateController
{
    public NeutralState state;
	EnemyBehaviourController controller;
    NavMeshAgent agent;

    public NeutralStateController(NeutralState state, EnemyBehaviourController controller)
	{
        this.state = state;
		this.controller = controller;
        agent = controller.agent;
        Setup();
	}

    private void Setup()
    {
        if (state == NeutralState.Patrol)
        {
            return;
        }
        else if (state == NeutralState.Stopped)
        {
            Stopped();
        }
        else if (state == NeutralState.DigPatrol)
		{
            return;
		}
    }

    public void Update()
	{
        if (state == NeutralState.Patrol) Patrol();
        else if (state == NeutralState.Stopped) Stopped();
        else if (state == NeutralState.DigPatrol) DigPatrol();
	}

    bool patrolRestarted;
    private void Patrol()
	{
        if (agent.remainingDistance < .25f)
		{
            controller.StartCoroutine(RestartPatrol());
		}
	}

    private void DigPatrol()
	{
        if (agent.remainingDistance < .25f)
		{
            controller.StartCoroutine(RestartDigPatrol());
		}
	}

    private IEnumerator RestartPatrol()
    {
        if (!patrolRestarted)
        {
            agent.isStopped = true;
            patrolRestarted = true;

            yield return new WaitForSeconds(controller.GetPatrolStopTime());

            patrolRestarted = false;
            
            if (!controller.isAggressive)
            {
                agent.destination = controller.GetRandomNavMeshPoint(agent.transform, controller.patrolRange);
                agent.isStopped = false;
            }
        }
    }


    private IEnumerator RestartDigPatrol()
	{
        if (!patrolRestarted)
        {
            // DIG
            patrolRestarted = false;
            if (!controller.isAggressive)
            {
                agent.isStopped = false;
                controller.animator.SetTrigger("Dig");
                controller.enemy.audioSource.PlayOneShot(controller.digSound);
                agent.destination = controller.GetRandomNavMeshPoint(agent.transform, controller.patrolRange);
                controller.EnableMovementVfxEmission(true);
            }

            yield return new WaitForSeconds(controller.GetPatrolStopTime());

            // EMERGE
            if (!controller.isAggressive)
            {
                patrolRestarted = true;
                agent.isStopped = true;
                controller.animator.SetTrigger("Emerge");
                controller.EnableMovementVfxEmission(false);
            }
        }
	}

    private void Stopped()
	{
        agent.speed = 0;
	}

    private Vector3 GetPatrolDirection()
    {
        Vector3 randomDirection = Random.insideUnitSphere * 3;

        randomDirection += agent.transform.position;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, 3, 1);
        Vector3 finalPosition = hit.position;

        return finalPosition;
    }
}

public enum NeutralState
{
    Patrol, Stopped, DigPatrol
}