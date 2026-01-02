using UnityEngine;

public class Door : MonoBehaviour
{
    [HideInInspector] public Room connectedRoom;
    [HideInInspector] public Door connectedDoor;

    [SerializeField] private Vector2 doorDir;   
    [SerializeField] private Transform entryPoint;
    [SerializeField] private Collider2D doorLock;
    [SerializeField] private Animator doorAnimator;

    public GameObject doorWall;

    public Vector2 DoorDir => doorDir;
    public Transform EntryPoint => entryPoint;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        Debug.Log("Player entered door");

        if (connectedRoom == null || connectedDoor == null)
            return;

        RoomManager.RM.MovePlayer(
            other.transform.parent.gameObject,
            connectedRoom,
            connectedDoor.EntryPoint
        );
    }

    public void ChangeLock(bool doLock)
    {

        doorAnimator.SetTrigger(doLock ? "Lock" : "Unlock");
        doorLock.enabled = doLock;
    }
}
