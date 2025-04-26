using UnityEngine;

public class TroopSpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public Transform[] spawnPoints;
    public float timeBetweenWaves = 10f;
    private int waveNumber = 0;

    private Animator animator;

    void Start()
    {
        InvokeRepeating(nameof(SpawnWave), 2f, timeBetweenWaves);
    }

    void SpawnWave()
    {
        waveNumber++;
        Debug.Log("Spawning wave " + waveNumber);
        for (int i = 0; i < 3; i++)  // 3 enemies per wave
        {
            int randomSpawnIndex = Random.Range(0, spawnPoints.Length);
            Instantiate(enemyPrefab, spawnPoints[randomSpawnIndex].position, Quaternion.identity);
        }
    }
}
