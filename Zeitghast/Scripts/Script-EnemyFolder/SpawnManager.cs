using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [Header("Spawn Manager Settings")]
    [SerializeField] private float spawnInterval = 1f;
    private float spawnIntervalTimer;

    [SerializeField] private int enemyCap = 5;

    private List<SpawnSet> spawnSets;

    [Header("Enemy Spawn Portal Settings")]
    
    [SerializeField] private GameObject spawnPortal;
    [SerializeField] private float timeToSpawnEnemyFromPortal;

    private List<SpawnSet> ActiveSequence;

    private List<GameObject> activeEnemies;

    // Start is called before the first frame update

    private void Awake()
    {
        //Get our Spawn sets
        spawnSets =  GetComponentsInChildren<SpawnSet>().ToList();

        setup();
    }

    void Start()
    {
      
    }

    // Update is called once per frame
    void Update()
    {
        if (Timer.gamePaused) return;

        SpawnEnemyUpdate();
    }

    private void setup()
    {
        ActiveSequence = new List<SpawnSet>();
        activeEnemies = new List<GameObject>();
    }

    private void SpawnEnemyUpdate()
    {
        removeDeadEnemies();

        if (activeEnemies.Count >= enemyCap)
        {
            // Reset Spawn Interval
            spawnIntervalTimer = spawnInterval;
            return;
        }

        spawnIntervalTimer -= Time.deltaTime;

        if (spawnIntervalTimer <= 0)
        {

            spawnEnemy();

            // Reset Spawn Interval
            spawnIntervalTimer = spawnInterval;
        }
    }

    private void spawnEnemy()
    {
        // Get spawn set 
        SpawnSet currentSpawn = getNextSpawnSet();

        // Get random enemy
        GameObject enemyToSpawn = currentSpawn.Enemies.GetRandomElement();

        // Spawn random enemy at SpawnSet transform
        GameObject spawnedEnemy = Instantiate(enemyToSpawn, currentSpawn.transform.position, Quaternion.identity);

        // Store active enemy
        activeEnemies.Add(spawnedEnemy);

        // Set Enemy Inactive (to be set active later)
        spawnedEnemy.SetActive(false);

        createEnemySpawnPortal(spawnedEnemy);
        
        StartCoroutine(activateEnemyWithDelay(spawnedEnemy, timeToSpawnEnemyFromPortal));
    }

    private void createEnemySpawnPortal(GameObject enemy)
    {
        if (spawnPortal == null) return;

        // Portal Spawn:
        GameObject portal = Instantiate(spawnPortal, enemy.transform.position, transform.rotation);
        portal.transform.SetParent(transform);
        portal.SetActive(true);

        // Get Portal Scale:
        EnemyMovement enemyMovement = enemy.gameObject.GetComponent<EnemyMovement>();
        if (enemyMovement != null)
        {
            portal.transform.localScale = enemyMovement.spawnPortalScale;
        }
    }

    private void removeDeadEnemies()
    {
        foreach (var enemy in activeEnemies.ToList())
        {
            if (enemy != null) continue;
            
            activeEnemies.Remove(enemy);
        }
    }

    private SpawnSet getNextSpawnSet()
    {
        SpawnSet result;
        //Empty Sequence
        if (ActiveSequence.Count == 0) 
        {
            ActiveSequence = new List<SpawnSet>(spawnSets);
        }

        //Get nextSequence
        result = ActiveSequence.GetRandomElement();
        ActiveSequence.Remove(result);

        return result;
    }

    private IEnumerator activateEnemyWithDelay(GameObject enemyObject, float timeToSpawn)
    {
        yield return new WaitForSeconds(timeToSpawn);

        enemyObject.SetActive(true);
    }

    private void Cleanup()
    {

    }
}
