using UnityEngine;
using UnityEngine.AI;

public class TestMove : MonoBehaviour
{
    NavMeshAgent agent;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Start()
    {
        agent.SetDestination(Vector3.zero);
    }
}
