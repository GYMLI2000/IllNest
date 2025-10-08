using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Room : MonoBehaviour
{
    private List<Door> doorList;
    public List<Door> DoorList => doorList;

    private void Awake()
    {
        doorList = gameObject.GetComponentsInChildren<Door>().ToList<Door>();
    }

}
