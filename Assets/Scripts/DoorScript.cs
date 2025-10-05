using System;
using UnityEngine;

public class DoorScript : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log(gameObject.name);
        }
    }
}
