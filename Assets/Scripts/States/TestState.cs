using UnityEngine;
using UnityEngine.AI;

public class TestState : State
{
    private Animator animator;
    public TestState(Enemy enemy, Animator animator, NavMeshAgent agent) : base(enemy) { 
        this.animator = animator;
    }

    public override void Enter()
    {
        base.Enter();
        animator.CrossFade(enemy.testHash, .25f);
    }
    public override void Update()
    {
        base.Update();
    }
}
