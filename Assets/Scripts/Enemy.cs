using System;
using System.Collections;
using System.Numerics;
using Interfaces;
using UnityEngine;
using UnityEngine.AI;
using Quaternion = UnityEngine.Quaternion;
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
    public GameObject deathEffect;
    public ParticleSystem hitEffect;
    
    [Header("Animation")]
    public Animator animator;
    
    private readonly int idleStateHash =  Animator.StringToHash("IsIdle");
    private readonly int moveStateHash =  Animator.StringToHash("IsMoving");
    private readonly int chaseStateHash =  Animator.StringToHash("IsChasing");
    private readonly int deathStateHash =  Animator.StringToHash("Dead");
    private readonly int attackStateHash =  Animator.StringToHash("Attack");
    private readonly int hitStateHash =  Animator.StringToHash("Hit");
    
    
    [SerializeField] private float health;
    private NavMeshAgent agent;
    private Vector3 destination;
    private float patrolRange = 10.0f; // recommended by unity as max value for range finding random point on nav mesh
    private Vector3 currentTarget;
    private Player player;
    private bool canAttack = true;
    private bool isChasing = false;
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
        //UpdateMoveAnimation();
        
        Debug.Log($"Is player in sight {IsPlayerInSight()}");
        
        switch (sM)
        {
            case EnemyStateMachine.Idle:
                Idle();
                break;
            
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

    public void Damage(GameObject instigator,float amount)
    {
        if (isDead) return; // If is dead and takes damage, they trigger the hit animation
        if (instigator.GetComponent<Enemy>()) return;
        hitEffect.Play();
        health -= amount;
        Debug.Log($"{name} was damaged by: {amount}");
        animator.SetTrigger(hitStateHash);
        if (health <= 0)
        {
            sM = EnemyStateMachine.Destroyed; 
        }
    }

    private void Idle()
    {
        // Check if there is a valid patrol target before moving
        animator.SetBool(moveStateHash, false);
    }

    private void UpdateMoveAnimation()
    {
        if (agent.velocity.magnitude < 0.1f)
        {
            if (isChasing)
            {
                animator.SetBool(chaseStateHash, true);
                animator.SetBool(moveStateHash, false);
            }
            else
            {
                animator.SetBool(moveStateHash, true);
                animator.SetBool(chaseStateHash, false);
            }
            
        }
        else
        {
            animator.SetBool(idleStateHash, true);
            animator.SetBool(moveStateHash, false);
            animator.SetBool(chaseStateHash, false);
        }
    }
    private void Death()
    {
        isDead = true; // Do once
        
        player.GetComponent<EnemyDestroyedCounter>().AddToCounter();
        Debug.Log($"{gameObject.name} was defeated");
        // Play animation and sound
        animator.SetTrigger(deathStateHash);
        
        Invoke(nameof(DestroyEnemy), 3.0f);
    }

    private void DestroyEnemy()
    {
        Instantiate(deathEffect, transform.position, Quaternion.identity);
        ChanceToSpawnPotion();
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
        // Animation -------------------
        animator.SetBool(moveStateHash, true);
    }

    private Vector3 GetRandomPatrolPoint()
    {
        isChasing = false; //--------------------------Animation here ??
        Vector3 tryDestination = transform.position + Random.insideUnitSphere * patrolRange;
        return NavMesh.SamplePosition(tryDestination, out var hit, 1.0f, NavMesh.AllAreas) 
            ? hit.position : transform.position;
    }

    private void ChaseTarget()
    {
        agent.SetDestination(player.transform.position);
        isChasing = true;
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
                hitActor.GetComponent<IDamageable>().Damage(gameObject,damage);
                animator.SetTrigger(attackStateHash);
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
