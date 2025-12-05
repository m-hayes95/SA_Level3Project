using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class Spawner : MonoBehaviour
{
    public GameObject objectToSpawn;
    public float spawnTimer;
    public float spawnRange;

    private Vector3 randSpawnPoint;
    private Vector3 lastSpawnPoint;
    private Vector3 center;
    private bool isSpawning = false;

    private void Update()
    {
        if (isSpawning) return;
        
        StartCoroutine(SpawnNextObject());
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
    }
}
