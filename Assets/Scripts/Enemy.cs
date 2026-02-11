using System;
using System.Collections;
using Interfaces;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using Vector3 = UnityEngine.Vector3;

public class Enemy : StateMachine, IDamageable
{
    [Range(1f, 100f)]public float maxHealth;
    [Range(0f, 3f)] public float attackRadius;
    [Range(1f, 20f)] public float sightRange;
    [Range(1f, 100f)] public float damage;
    [Range(0f, 5f)] public float attackRate;
    [Range(0f,5f)] public float waitTimer = 2f;
    [Tooltip("Chance to spawn potions after destroyed (0 = always spawn, 5 = 1/6 chance)"),Range(0f, 5f)] 
    public int potionSpawnChance;
    public LayerMask damageLayer;
    public GameObject potion;
    public UnityEvent OnEnemyDeath;
    
    [Header("FX")]
    public GameObject deathEffect;
    public ParticleSystem hitEffect;
    public AudioSource hitAudioSource;
    public AudioSource deadAudioSource;
    
    [Header("Animation")]
    public Animator animator;
    
    public readonly int idleStateHash =  Animator.StringToHash("IsIdle");
    public readonly int moveStateHash =  Animator.StringToHash("IsMoving");
    public readonly int chaseStateHash =  Animator.StringToHash("IsChasing");
    public readonly int waitStateHash =  Animator.StringToHash("IsWaiting");
    public readonly int deathStateHash =  Animator.StringToHash("Dead");
    public readonly int attackStateHash =  Animator.StringToHash("Attack");
    public readonly int hitStateHash =  Animator.StringToHash("Hit");
    public readonly int testHash = Animator.StringToHash("Victory");
    
    
    [SerializeField] private float health;
    public NavMeshAgent agent;
    private Vector3 destination;
    public float patrolRange = 10.0f; // recommended by unity as max value for range finding random point on nav mesh
    public Vector3 currentTarget;
    public Player player;
    public bool canAttack = true;
    public bool isChasing = false;
    public bool isPatroling = false;
    public bool isWaiting = false;
    public bool isAttacking = false;
    public bool isDead = false;
    private Rigidbody rb;
    private Collider collider;

    // State variables
    public Patrol patrolState {  get; private set; }  
    public Chase chaseState {  get; private set; }  
    public Wait waitState {  get; private set; }  
    public Attack attackState {  get; private set; }  
    public Dead deadState {  get; private set; }  
    public TestState testState { get; private set; }
    
    
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        collider = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();

        // States do not inherit from monobehaviour so we need to set up a new instance of each state and pass in the this class for context
        patrolState = new Patrol(this);
        chaseState = new Chase(this);
        waitState = new Wait(this);
        attackState = new Attack(this);
        deadState = new Dead(this);
        testState = new TestState(this, animator, agent);
    }
    
    private void Start()
    {
        health = maxHealth;
     
        player = (Player)FindFirstObjectByType(typeof(Player));
        // Enter the first state
        InitializeState(testState);
        //StartCoroutine(currentState.Task()); 
    }

    private void Update()
    {
        //UpdateMoveAnimation();
        // Call the current states update method
        currentState.Update();
        Debug.Log($"{this} current state = {currentState}");
    }

    public void Damage(GameObject instigator,float amount)
    {
        if (isDead) return; // If is dead and takes damage, they trigger the hit animation
        if (instigator.GetComponent<Enemy>()) return;
        hitEffect.Play();
        hitAudioSource.Play();
        health -= amount;
        Debug.Log($"{name} was damaged by: {amount}");
        animator.SetTrigger(hitStateHash);
        if (health <= 0)
        {
            ChangeState(deadState);
        }
    }

    public bool IsPlayerInSight()
    {
        return Vector3.Distance(transform.position, player.transform.position) <= sightRange;
    }
    
    // If we want to add any trigger event states
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"enemy collided with : {other.gameObject}");
        currentState.OnTriggerEnter(other);
    }
    private void OnTriggerExit(Collider other)
    {
        currentState.OnTriggerExit(other);
    }
    private void OnTriggerStay(Collider other)
    {
        currentState.OnTriggerStay(other);
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
