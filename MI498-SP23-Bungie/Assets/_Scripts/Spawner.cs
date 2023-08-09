using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

[System.Serializable]
public struct GameObjectAndProbability
{

}

public class Spawner : MonoBehaviour
{
    [Header("Parameters")] 
    [Tooltip("The prefabs to spawn")]
    public GameObject[] spawnPrefab;
    public float[] probabilities;

    [Tooltip("The amount of time between spawns")]
    public float spawnTime = 1f;
    [Tooltip("Sphere collider to represent spawn area")]
    public SphereCollider collider;
    [Tooltip("Whether or not the number of objects that get spawned should be infinite")]
    public bool spawnInfinitely;
    [Tooltip("The limit on the number of objects to spawn if there is a finite amount")]
    public int spawnLimit = 10;
    [Tooltip("Whether or not to restore zombies here if they are killed.")]
    public bool restore = true;

    private float _startingTime;
    private float _currentTime;

    private List<EnemyStats> _stats;

    public int numEnemiesSpawned;

    private int spawnDetectLayerMask;

    public bool flyingZombies = false;

    private void Awake()
    {
        spawnDetectLayerMask = 1 >> LayerMask.NameToLayer("RCC") | 1 >> LayerMask.NameToLayer("Zombie");
        _startingTime = Time.time;
        _currentTime = Time.time;

        if (collider == null)
        {
            Debug.LogError($"{gameObject.name} is missing a SphereCollider");
        }
        for(int i = 1; i < probabilities.Length; i++)
        {
            probabilities[i] = probabilities[i-1]+probabilities[i];
        }
        _stats = new List<EnemyStats>(spawnPrefab.Length);
        for(int i = 0; i < spawnPrefab.Length; i++)
        {
            _stats.Add(spawnPrefab[i].GetComponent<EnemyStats>());
        }

        for(int i = 0; i < spawnLimit; i++)
        {
            Spawn();
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        // If the spawned item isn't an enemy, is set to spawn infinitely, or some can still be spawned
        if (_stats == null || spawnInfinitely || numEnemiesSpawned < spawnLimit)
        {
            SpawnUpdate();
        }
    }

    void SpawnUpdate()
    {
        _currentTime += Time.deltaTime;

        if (_currentTime - _startingTime >= spawnTime)
        {
            Spawn();
        }
    }

    void Spawn()
    {
        float rand = Random.Range(0, probabilities[probabilities.Length - 1]);
        int idx = 0;
        for (int i = 1; i < probabilities.Length; i++)
        {
            if (probabilities[i-1] < rand && rand <= probabilities[i])
            {
                idx = i;
                break;
            }
        }
        Vector3 spawnPos = CalculateRandomSpawnPosition(spawnPrefab[idx]);
        if (!flyingZombies)
        {
            
            NavMeshHit myNavHit;
            if (NavMesh.SamplePosition(spawnPos, out myNavHit, 100, -1))
            {
                GameObject newSpawn = Instantiate(spawnPrefab[idx], myNavHit.position, Quaternion.identity);
                EnemyStats spawnStats = newSpawn.GetComponent<EnemyStats>();

                if (spawnStats != null)
                {
                    spawnStats.spawner = this;
                    numEnemiesSpawned++;
                }


                _startingTime = Time.time;
                _currentTime = Time.time;
            }
        }
        else
        {
            GameObject newSpawn = Instantiate(spawnPrefab[0], spawnPos, Quaternion.identity);
            EnemyStats spawnStats = newSpawn.GetComponent<EnemyStats>();

            if (spawnStats != null)
            {
                spawnStats.spawner = this;
                numEnemiesSpawned++;
            }


            _startingTime = Time.time;
            _currentTime = Time.time;
        }

    }

    Vector3 CalculateRandomSpawnPosition(GameObject selected)
    {
        Vector3 pointToTry = Vector3.zero;
        RaycastHit[] hits;
        
        do
        {
            pointToTry = new Vector3(
                Random.Range(collider.bounds.min.x, collider.bounds.max.x),
                transform.position.y,
                Random.Range(collider.bounds.min.z, collider.bounds.max.z)
            );
            CapsuleCollider prefabCollider = selected.GetComponent<CapsuleCollider>(); 
            hits = Physics.SphereCastAll(pointToTry, prefabCollider.radius, Vector3.up,
                prefabCollider.height, spawnDetectLayerMask);
        } while (hits.Length != 0);

        return pointToTry;
    }

    // Call this if you want permanently killed zombies to spawn again
    public void Reset()
    {
        numEnemiesSpawned = 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        SetCarInTrigger(other, true);
    }

    private void OnTriggerExit(Collider other)
    {
        SetCarInTrigger(other, false);
    }

    void SetCarInTrigger(Collider other, bool inTrigger)
    {
        CarManager carManager = other.gameObject.GetComponent<CarManager>();
        if (carManager != null)
        {
            carManager.inEnemySpawner = inTrigger;
        }
    }
}
