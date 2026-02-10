using Interfaces;
using UnityEngine;

public class Attack : State
{
    private float timeElapsed = 0f;
    private bool canAttack = true;
    private IDamageable hitActor;

    public Attack(Enemy enemy) : base(enemy) { }

    public override void Enter()
    {
        base.Enter();
        canAttack = true;
        enemy.isAttacking = true;
        timeElapsed = 0f;
        
    }
    public override void Exit()
    {
        base.Exit();
        enemy.isAttacking = false;
        hitActor = null;
    }
 
    public override void OnTriggerExit(Collider collider) 
    { 
        base.OnTriggerExit(collider);
        Debug.Log($"LEAVEEEEEEE");
        IDamageable hitActor = collider.GetComponent<IDamageable>();

        if (hitActor != null && collider.GetComponent<Enemy>() == null)
        {
            canAttack = false;  
            enemy.ChangeState(enemy.waitState);
        }
    }
    public override void OnTriggerStay(Collider collider)
    {
        hitActor ??= collider.GetComponent<IDamageable>(); 
        if (hitActor == null || collider.GetComponent<Player>() == null) 
            return;

        if ( canAttack)
            AttackPlayer(); // Attack first before timer
        
        timeElapsed += Time.deltaTime;
        if(timeElapsed >= enemy.attackRate)
        {
            canAttack = true;
            timeElapsed = 0f;
        }
    }

    private void AttackPlayer()
    {
        canAttack = false;
        hitActor.Damage(enemy.gameObject, enemy.damage);
        enemy.animator.SetTrigger(enemy.attackStateHash);
    }
  
}
