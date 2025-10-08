using UnityEngine;

public class Door : MonoBehaviour
{

    public Room connectedRoom;
    public Door connectedDoor;


    [SerializeField]
    private Vector3 doorDir;

    [SerializeField]
    private Transform exitPoint;
    
    public Vector3 DoorDir => doorDir;

    

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && connectedDoor != null && connectedRoom != null)
        {
            RoomManager.RM.MovePlayer(collision.gameObject, connectedRoom , connectedDoor.exitPoint);
            //Debug.Log($"{collision.gameObject.name}, {connectedRoom.name}, {connectedDoor.exitPoint.position}");
        }
    }
}
