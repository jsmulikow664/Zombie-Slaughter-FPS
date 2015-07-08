using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    public Transform[] spawnPoints;  	// Array of spawn points to be used.
    public GameObject[] enemyPrefabs; 	// Array of different Enemies that are used.
    public float amountEnemies = 20;  			// Total number of enemies to spawn.
    public float yieldTimeMin = 2;  				// Minimum amount of time before spawning enemies randomly.
    public float yieldTimeMax = 5;  				// Don't exceed this amount of time between spawning enemies randomly.
    public int enemiesToSpawnAtTheSameTime = 1;
    private bool wait = false;
    public MeshRenderer[] disableRenderers;

    void Start()
    {
        for (int i = 0; i < disableRenderers.Length; i++)
        {
            disableRenderers[i].enabled = false;
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(Spawn());
        }
    }

    IEnumerator Spawn()
    {
        if (wait) yield break;

        if (amountEnemies > 0)
        {
            wait = true;
            yield return new WaitForSeconds(Random.Range(yieldTimeMin, yieldTimeMax));  // How long to wait before another enemy is instantiated.
            for (int s = 0; s < enemiesToSpawnAtTheSameTime; s++)
            {
                GameObject obj = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)]; // Randomize the different enemies to instantiate.
                Transform pos = spawnPoints[Random.Range(0, spawnPoints.Length)];  // Randomize the spawnPoints to instantiate enemy at next.

                Instantiate(obj, pos.position, pos.rotation);
            }
            amountEnemies--;
            wait = false;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}