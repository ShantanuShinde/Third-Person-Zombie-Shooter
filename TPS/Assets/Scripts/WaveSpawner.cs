using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class WaveSpawner : MonoBehaviour
{
    public Slider wavePercent;
    public Text wave1, wave2;
    [System.Serializable]
    public class Wave
    {
        public string name;

        public List<GameObject> enemies;
        public int count;
        public float rate;
    }

    enum SpawnStates { WAITING, COUNTING, SPAWNING }

    SpawnStates spawnState = SpawnStates.COUNTING;

    public Wave wave;

    public List<Transform> spawners;

    private int nextWave = 0;

    public float timeBetweenWaves = 5f;

    public float waveCountDown;

    float searchCountDown = 1f;

    public int currentEnemyNum;

    private void Start()
    {
        waveCountDown = timeBetweenWaves;

        wavePercent.value = 0;

    }

    private void Update()
    {
        if(spawnState == SpawnStates.WAITING)
        {
            wavePercent.value = currentEnemyNum;
            if (!EnemyIsAlive())
            {
                //print("Wave finished. Time till next wave: " + waveCountDown);
                wave.count+=3;
                waveCountDown = timeBetweenWaves;
                spawnState = SpawnStates.COUNTING;
            }
            else
            {
                return;
            }
        }

        if(waveCountDown <= 0)
        {
            if(spawnState != SpawnStates.SPAWNING)
            {
                if (nextWave == 3)
                {
                    return;
                }
                StartCoroutine(SpawnWave(wave));
                nextWave++;
                wave1.text = nextWave.ToString();
                wave2.text = (nextWave - 1).ToString();
                wavePercent.maxValue = wave.count;
                wavePercent.value = wave.count;
                currentEnemyNum = wave.count;
            }
        }
        else
        {
            waveCountDown -= Time.deltaTime;
        }
    }

    bool EnemyIsAlive()
    {
        searchCountDown -= Time.deltaTime;
        if (searchCountDown <= 0)
        {
            searchCountDown = 1f;
            if (GameObject.FindGameObjectWithTag("Zombie") == null)
            {
                return false;
            }
            
        }
        return true;
    }

    IEnumerator SpawnWave(Wave wave)
    {
        spawnState = SpawnStates.SPAWNING;
        
        for(int i = 0; i< wave.count; i++)
        {
            GameObject enemy = wave.enemies[Random.Range(0, wave.enemies.Count)];
            SpawnEnemy(enemy);
            yield return new WaitForSeconds(1f / wave.rate);
        }
       

        spawnState = SpawnStates.WAITING;

        yield break;
    }

    void SpawnEnemy(GameObject _enemy)
    {
        Transform spawner = spawners[Random.Range(0, spawners.Count)];
        Instantiate(_enemy, spawner);
    }
}
  
