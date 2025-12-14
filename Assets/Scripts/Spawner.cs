using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class Spawner : MonoBehaviour
{
    public GameObject objectToSpawn;
    [Range(0.0f, 10.0f)] public float spawnTimer;
    [Range(1.0f,50.0f)] public float spawnRange;
    [Tooltip("How far the player can be from the spawner to allow spawning"), Range(1.0f,500.0f)] 
    public float allowSpawnPlayerDistance = 50.0f;

    private Vector3 randSpawnPoint;
    private Vector3 lastSpawnPoint;
    private Vector3 center;
    private bool isSpawning = false;
    private Player player;
    private float distanceToPlayer;

    private void Start()
    {
        player = (Player)FindFirstObjectByType(typeof(Player));
    }
    private void Update()
    {
        if (isSpawning || !CheckCanSpawn()) return;
        
        StartCoroutine(SpawnNextObject());
    }

    private bool CheckCanSpawn()
    {
        bool canSpawn = true;
        distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);
        
        if (distanceToPlayer > allowSpawnPlayerDistance)
        {
            canSpawn = false;
            Debug.Log($"{name} : Player distance from spawner = {distanceToPlayer} , can spawn = {canSpawn}");
        }
        else
        {
            canSpawn = true;
            Debug.Log($"{name} : Player distance from spawner = {distanceToPlayer} , can spawn = {canSpawn}");
        }
        
        return canSpawn;
    }
    

    private Vector3 GetSpawnPosition()
    {
        float randX = transform.position.x + Random.Range(-spawnRange, spawnRange);
        float randZ = transform.position.z + Random.Range(-spawnRange, spawnRange);
        randSpawnPoint = new Vector3(randX, 0, randZ);
        return randSpawnPoint;
    }

    private IEnumerator SpawnNextObject()
    {
        isSpawning = true;
        yield return new WaitForSeconds(spawnTimer);
        GameObject newObject = Instantiate(objectToSpawn, GetSpawnPosition(), Quaternion.identity);
        lastSpawnPoint = newObject.transform.position;
        isSpawning = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(lastSpawnPoint, 1f);
        
        
        Vector3 spawnArea = new Vector3(spawnRange * 2, 1, spawnRange * 2);
        Gizmos.DrawWireCube(transform.position, spawnArea);
        Gizmos.DrawWireSphere(transform.position, allowSpawnPlayerDistance);
    }
}
