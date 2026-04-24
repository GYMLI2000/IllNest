using UnityEngine;

public class Staircase : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            CameraController.Instance.StopBossCamera();
            RoomManager.RM.LoadNextFloor();
        }
    }
}
