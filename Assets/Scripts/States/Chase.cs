using UnityEngine;
using UnityEngine.AI;

public class Chase : State
{
    public Chase(Enemy enemy) : base(enemy) { }

    public override void Enter()
    {
        base.Enter();
        Debug.Log($"{enemy} has entered {this} state");
        enemy.isChasing = true;
        enemy.currentTarget = enemy.player.transform.position;
        enemy.agent.SetDestination(enemy.currentTarget);
        enemy.animator.SetBool(enemy.chaseStateHash, true);
    }

    public override void Update() 
    { 
        base.Update();
        if (!enemy.IsPlayerInSight())
        {
            Debug.Log($"Reached the target in Chase State - Lost sight");
            enemy.ChangeState(enemy.waitState);
        }
    }   
    public override void Exit()
    {
        base.Exit();
        enemy.isChasing = false;
        enemy.animator.SetBool(enemy.chaseStateHash, false);
    }
    public override void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Enter attack state");
        base.OnTriggerEnter(other);
        if (other.gameObject.GetComponent<Player>())
        {
            enemy.ChangeState(enemy.attackState);
        }
    }

}
