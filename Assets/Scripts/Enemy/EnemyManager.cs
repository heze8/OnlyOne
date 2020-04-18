using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyManager : Singleton<EnemyManager>
{
    public GameObject[] enemy;
    [SerializeField]
    public List<Vector3> spawnPoints;
    public float timeBetweenEnemies = 1f;
    public float timeBetweenWaves = 10f;
    public float spawnDistance = 60f;
    public float minDistanceFromPlayer = 25f;
    public float enemyPerWave = 1.3f;
    private float enemyNum = 0;

    public void r(List<Vector3> temp)
    {
        spawnPoints = temp;
    }

    Vector3 returnSpawn(Vector3 pos)
    {
        List<Tuple<Vector3, int>> potentialSpawnPoints = new List<Tuple<Vector3, Int32>>();
        //float minDistance = 999f;
        foreach (Vector3 v3 in spawnPoints)
        {
            float distance = Vector3.Distance(pos, v3);
/*            if (minDistance > distance)
            {
                minDistance = distance;
            }*/
            if (distance <= spawnDistance && distance >= minDistanceFromPlayer)
            {
                potentialSpawnPoints.Add(new Tuple<Vector3, Int32>(v3, 2 * (Int32) (spawnDistance - distance)));
            }
        }
        
        if (potentialSpawnPoints.Count == 0)
        {
            return new Vector3();
        }
        
        return ValueRandomGenerator.Random(potentialSpawnPoints);
    }

    // class Wave 
    // {
    //     int enemiesNum; 
    // }

	public void Start()
	{
	    StartCoroutine(StartNextWave());
	}
    private float enemiesSpawned = 0;
    IEnumerator StartNextWave()
    {
        Debug.Log("starting next wave :" + Mathf.CeilToInt(enemiesSpawned));
        enemyNum += enemyPerWave; // increases enemies by one each wave.
        yield return new WaitForSeconds(timeBetweenWaves);
        StartCoroutine(SpawnEnemies());
        StartCoroutine(StartNextWave());
    }

    // Coroutine to spawn all of our enemies
    IEnumerator SpawnEnemies()
    {
        while (enemiesSpawned < enemyNum)
        {
            Vector3 spawn = returnSpawn(GameManager.Instance.player.transform.position);
            if (spawn.Equals(new Vector3()))
            {
                yield return new WaitForSeconds(timeBetweenEnemies);
            } else {
                enemiesSpawned++;
                // Create an instance of the enemy prefab at the randomly selected spawn point's position and rotation.
                Instantiate(enemy[Random.Range(0, enemy.Length)], spawn, enemy[0].transform.rotation);
                yield return new WaitForSeconds(timeBetweenEnemies);
            }
        }
        yield return null;
    }
    
    // // called by an enemy when they're defeated
    // public void EnemyDefeated()
    // {
    //     _enemiesInWaveLeft--;
        
    //     // We start the next wave once we have spawned and defeated them all
    //     if (_enemiesInWaveLeft == 0 && _spawnedEnemies == _totalEnemiesInCurrentWave)
    //     {
    //         StartNextWave();
    //     }
    // }
}

