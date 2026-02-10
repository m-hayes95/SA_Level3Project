using UnityEngine;

public class Dead : State
{
    public Dead(Enemy enemy) : base(enemy) { }

    public override void Enter()
    {
        base.Enter();
        Death();
    }
    public override void Update()
    {
        base.Update(); 
    }
    private void Death()
    {
        enemy.isDead = true; // Do once
        // Turn off collisions when dead
        enemy.GetComponent<Collider>().enabled = false;
        // Add points to player, play sounds and animations
        enemy.OnEnemyDeath?.Invoke();

        Debug.Log($"{enemy.gameObject.name} was defeated");
    }
}
