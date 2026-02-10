using UnityEngine;

public class Wait : State
{
    float timerElapsed = 0f;
    public Wait(Enemy enemy) : base(enemy) { }

    public override void Enter()
    {
        base.Enter();
        timerElapsed = 0f;
        enemy.isWaiting = true;
        enemy.animator.SetBool(enemy.waitStateHash, true);
        Debug.Log($"{enemy} has entered {this} state");
    }
    public override void Exit()
    {
        base.Exit();
        enemy.isWaiting = false;
        enemy.animator.SetBool(enemy.waitStateHash, false);
    }

    public override void Update()
    {
        base.Update();
        timerElapsed += Time.deltaTime;
        Debug.Log($"wait timer elapsed = {timerElapsed}");
        if (timerElapsed >= enemy.waitTimer)
        {
            timerElapsed = 0f;
            // Check if they should chase the player or patrol
            if (enemy.IsPlayerInSight())
            {
                enemy.ChangeState(enemy.patrolState);
            }
            else
                enemy.ChangeState(enemy.patrolState);
        }
        
    }
}
