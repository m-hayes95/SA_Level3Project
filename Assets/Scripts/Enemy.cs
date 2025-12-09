using System;
using System.Collections;
using System.Numerics;
using Interfaces;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

public class Enemy : MonoBehaviour, IDamageable
{
    [Range(1f, 100f)]public float maxHealth;
    [Range(1f, 5f)] public float acceptanceRadius;
    [Range(0f, 3f)] public float attackRadius;
    [Range(1f, 20f)] public float sightRange;
    [Range(1f, 100f)] public float damage;
    [Range(0f, 5f)] public float attackRate;
    [Tooltip("Chance to spawn potions after destroyed (0 = always spawn, 5 = 1/6 chance)"),Range(0f, 5f)] 
    public int potionSpawnChance;
    public LayerMask damageLayer;
    public GameObject potion;
    
    [SerializeField] private float health;
    private NavMeshAgent agent;
    private Vector3 destination;
    private float patrolRange = 10.0f; // recommended by unity as max value for range finding random point on nav mesh
    private Vector3 currentTarget;
    private Player player;
    private bool canAttack = true;
    private bool isDead = false;

    private enum EnemyStateMachine
    {
        Idle,
        Patrol,
        Wait,
        Chase,
        Attack,
        Destroyed
    };
    [SerializeField]private EnemyStateMachine sM;
    
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }
    
    private void Start()
    {
        health = maxHealth;
        UpdatePatrolTarget();
        sM = EnemyStateMachine.Patrol;
        player = (Player)FindFirstObjectByType(typeof(Player));
    }

    private void Update()
    {
        Debug.Log($"Is player in sight {IsPlayerInSight()}");
        switch (sM)
        {
            case EnemyStateMachine.Patrol:
                
                if (IsPlayerInSight())
                {
                    sM = EnemyStateMachine.Chase;
                }
                
                Patrol();
                
                break;
            case EnemyStateMachine.Chase:
                
                if (!IsPlayerInSight())
                {
                    sM = EnemyStateMachine.Wait;
                }

                if (Vector3.Distance(transform.position, player.transform.position) <= attackRadius)
                {
                    sM = EnemyStateMachine.Attack;
                }
                
                ChaseTarget();
                
                break;
            case EnemyStateMachine.Wait:
                
                Wait();
                sM = EnemyStateMachine.Patrol;
                
                break;
            
            case EnemyStateMachine.Attack:
                // need to improve this as atm the enemy will spam switch states whilst attacking
                if(canAttack)
                    Attack();
                sM = EnemyStateMachine.Wait; 
                break;
            
            case EnemyStateMachine.Destroyed:
                if (!isDead)
                    Death();
                break;
            
            default:
                break;
            
        }
    }

    public void Damage(float amount)
    {
        health -= amount;
        Debug.Log($"{name} was damaged by: {amount}");
        if (health <= 0)
        {
            sM = EnemyStateMachine.Destroyed; 
        }
    }

    private void Death()
    {
        isDead = true; // Do once
        ChanceToSpawnPotion();
        Debug.Log($"{gameObject.name} was defeated");
        // Play animation and sound
        gameObject.SetActive(false); // Remove
        this.enabled = false;
    }

    private void ChanceToSpawnPotion()
    {
        int rand = Random.Range(0, potionSpawnChance);
        if (rand == 0)
        {
            Instantiate(potion, transform.position, transform.rotation);
        }
    }

    private void Patrol()
    {
        if (Vector3.Distance(transform.position, currentTarget) <= acceptanceRadius)
            UpdatePatrolTarget();
    }

    private void Wait()
    {
        UpdatePatrolTarget(); // when switching to patrol again the target is not updated as we are not at the target point
    }

    private void UpdatePatrolTarget()
    {
        currentTarget = GetRandomPatrolPoint();
        agent.SetDestination(currentTarget);
        Debug.Log($"{gameObject.name} is moving {currentTarget}");
    }

    private Vector3 GetRandomPatrolPoint()
    {
        Vector3 tryDestination = transform.position + Random.insideUnitSphere * patrolRange;
        return NavMesh.SamplePosition(tryDestination, out var hit, 1.0f, NavMesh.AllAreas) 
            ? hit.position : transform.position;
    }

    private void ChaseTarget()
    {
        agent.SetDestination(player.transform.position);
    }

    private bool IsPlayerInSight()
    {
        return Vector3.Distance(transform.position, player.transform.position) <= sightRange;
    }

    private void Attack()
    {
        canAttack = false;
        Collider[] hitActors = Physics.OverlapSphere(transform.position, attackRadius, damageLayer);
        foreach (Collider hitActor in hitActors)
        {
            if (hitActor.GetComponent<IDamageable>() != null && hitActor.gameObject != gameObject)
            {
                hitActor.GetComponent<IDamageable>().Damage(damage);
            }
        }
        Invoke(nameof(ResetAttack), attackRate);
    }

    private void ResetAttack()
    {
        canAttack = true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(currentTarget, 0.5f);
        Gizmos.DrawWireSphere(transform.position, sightRange);
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
}
