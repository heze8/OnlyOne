using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossCaller : MonoBehaviour
{
    public GameObject boss;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag.Equals("Player"))
        {
            SpawnBoss();
        }
    }

    private void SpawnBoss()
    {
        EnemyManager.Instance.StopAllCoroutines();
        Instantiate(boss, transform.position, Quaternion.identity);
        Debug.Log("Spawning boss");
        Destroy(this);
    }
}
