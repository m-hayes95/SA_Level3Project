using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Enemy : MonoBehaviour
{
    public float maxHealth;
    public float acceptanceRadius;
    [FormerlySerializedAs("patrolWaitTime"), Range(1f, 5f)] public float maxPatrolWaitTime;
    
    private float health;
    private NavMeshAgent agent;
    private Vector3 destination;
    private float patrolRange = 10.0f; // recommended by unity as max value for range finding random point on nav mesh
    private Vector3 currentTarget;
    
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }
    
    private void Start()
    {
        health = maxHealth;
        UpdatePatrolTarget();
    }

    private void Update()
    {
        Patrol();
    }
    
    public void TakeDamage(float amount)
    {
        health -= amount;
        if (health <= 0)
        {
            Debug.Log($"{gameObject.name} was defeated");
            // Play animation and sound
            gameObject.SetActive(false); // Remove
            this.enabled = false;
        }
    }

    private void Patrol()
    {
        if (Vector3.Distance(transform.position, currentTarget) <= acceptanceRadius)
            UpdatePatrolTarget();
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(currentTarget, 0.5f);
    }
}
