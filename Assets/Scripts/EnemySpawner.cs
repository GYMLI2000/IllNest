using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    private List<Enemy> enemies;


    [SerializeField]
    private Transform spawnPoint;
    
    private List<Transform> spawnPoints = new();

    public Room parentRoom;

    private List<Enemy> currentEnemies = new();

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

    
    public void SpawnEnemies()
    {
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
    } 






    public void RemoveEnemy(Enemy enemy)
    {
        Debug.Log("Checking Enemies  " + currentEnemies.Count);

        currentEnemies.RemoveAll(e => e == null || !e.isActiveAndEnabled);

        currentEnemies.Remove(enemy);
        if (currentEnemies.Count <= 0)
        {
            RoomManager.RM.ClearRoom(parentRoom);
        }

        Debug.Log("Checked   " + currentEnemies.Count);

    }


}
