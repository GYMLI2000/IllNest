using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    private List<Enemy> enemies;

    [SerializeField]
    private List<Enemy> lesserEnemies;

    [SerializeField]
    private Transform spawnPoint;
    
    private List<Transform> spawnPoints = new();

    public Room parentRoom;

    private List<Enemy> currentEnemies = new();

    private bool spawnedEnemies = false;

    private void Awake()
    {
        if (spawnPoint == null)
        {
            return;
        }

        foreach (Transform child in spawnPoint)
        {
            if (child != spawnPoint)
            {
                spawnPoints.Add(child);
            }
        }
    }

    private void Update()
    {
        if(!spawnedEnemies) return;
        currentEnemies.RemoveAll(e => e == null || !e.isActiveAndEnabled);
        if (currentEnemies.Count <= 0)
        {
            RoomManager.RM.ClearRoom(parentRoom);
            spawnedEnemies = false;
        }
    }

    public void SpawnEnemies()
    {
        currentEnemies.Clear();
        bool spawnedMainEnemy = false;
        List<Transform> availableSpawnPoints = new List<Transform>(spawnPoints);

        for (int i = 0; i < 4 && availableSpawnPoints.Count > 0; i++)
        {
            int index = Random.Range(0, availableSpawnPoints.Count);
            Transform spawnPoint = availableSpawnPoints[index];
            availableSpawnPoints.RemoveAt(index);

            Enemy enemy;

            if (!spawnedMainEnemy)
            {
                enemy = enemies[Random.Range(0, enemies.Count)];
                spawnedMainEnemy = true;
            }
            else
            {
                enemy = RoomManager.RM.lesserEnemies[Random.Range(0, RoomManager.RM.lesserEnemies.Count)];
            }


            Enemy spawnedEnemy = enemy.SpawnEnemy(this, spawnPoint.position);

            if (currentEnemies.Contains(spawnedEnemy))
            {
                Debug.LogWarning($"Spawned enemy {spawnedEnemy.name} is already in the list!"); 
            }
            currentEnemies.Add(spawnedEnemy);

        }

        spawnedEnemies = true;
    } 






    public void RemoveEnemy(Enemy enemy)
    {
        Debug.Log("Checking Enemies  " + currentEnemies.Count);

        currentEnemies.Remove(enemy);

        Debug.Log("Checked   " + currentEnemies.Count);

    }


}
