using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Room : MonoBehaviour
{
    private List<Door> doorList;
    public List<Door> DoorList => doorList;

    [SerializeField] private EnemySpawner enemySpawner;

    public bool isCleared { get; private set; } = false;

    private void Awake()
    {
        doorList = gameObject.GetComponentsInChildren<Door>().ToList<Door>();
    }

    public void ActivateRoom()
    {
        foreach (Door door in doorList)
        {
            if (door.connectedRoom != null)
            {
                door.ChangeLock(true);
            }
        }

        enemySpawner.SpawnEnemies();
    }

    public void ClearRoom()
    {
        isCleared = true;
        foreach (Door door in doorList)
        {
            door.ChangeLock(false);
        }
    }
}
