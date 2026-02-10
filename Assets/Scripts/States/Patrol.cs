using UnityEngine;
using UnityEngine.AI;

public class Patrol : State
{
    private float totalPatrolTimer = 5f;
    float timeElapsed = 0f;
    public Patrol(Enemy enemy) : base(enemy) { }
    public override void Enter()
    {
        base.Enter();
        timeElapsed = 0f;
        enemy.isPatroling = true;
        Debug.Log($"{enemy} has entered {this} state");
        UpdatePatrolTarget();
    }

    public override void Update()
    {
        base.Update();

        // Make sure not to get stuck if cant reach target
        
        timeElapsed += Time.deltaTime;
        if (timeElapsed >= totalPatrolTimer)
        {
            Debug.Log($"{enemy} could not reach target in time: {timeElapsed} ");
            UpdatePatrolTarget();
            timeElapsed = 0f;
        }

        // if we see the player, chase
        if (enemy.IsPlayerInSight())
        {
            // Chase
            enemy.ChangeState(enemy.chaseState);
        }

        // Reached the target
        if (Vector3.Distance(enemy.transform.position, enemy.currentTarget) <= enemy.agent.stoppingDistance)
        {
            // Change to Attack
            Debug.Log($"Reached the target in Patrol State");
            enemy.ChangeState(enemy.waitState);
        }
        
    }
    public override void Exit()
    {
        base.Exit();
        enemy.isPatroling=false;
        Debug.Log($"{enemy} has exited PATROL state");
    }

    private void UpdatePatrolTarget()
    {
        enemy.currentTarget = GetRandomPatrolPoint();
        enemy.agent.SetDestination(enemy.currentTarget);
        Debug.Log($"{enemy} is moving {enemy.currentTarget}");
        // Animation -------------------
        enemy.animator.SetBool(enemy.moveStateHash, true);
    }

    private Vector3 GetRandomPatrolPoint()
    {
        float patrolRange = 10f;
        Vector3 tryDestination = enemy.transform.position + Random.insideUnitSphere * patrolRange;
        // Change zero later when I know it works
        return NavMesh.SamplePosition(tryDestination, out var hit, patrolRange, NavMesh.AllAreas)
            ? hit.position : Vector3.zero;
    }
}
