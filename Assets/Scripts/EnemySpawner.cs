using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    private Enemy[] enemies;

    [SerializeField]
    private Transform[] spawnPoints;

    [SerializeField] private Room parentRoom;

    private List<Enemy> currentEnemies = new();

    public void SpawnEnemies()
    {

        foreach (Transform spawnPoint in spawnPoints)
        {
            Enemy enemy = enemies[Random.Range(0, enemies.Length)];
            currentEnemies.Add(enemy.SpawnEnemy(this, spawnPoint.position));
        }
    }

    public void RemoveEnemy(Enemy enemy)
    {
        Debug.Log("Checking Enemies  " + currentEnemies.Count);

        currentEnemies.Remove(enemy);
        if (currentEnemies.Count == 0)
        {
            parentRoom.ClearRoom();
        }

        Debug.Log("Checked   " + currentEnemies.Count);

    }


}
