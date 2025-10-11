using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{

    public static RoomManager RM;

    private Room currentRoom;
    private List<Room> roomList;

    [SerializeField]
    private int roomWidth;
    [SerializeField]
    private int roomHeight;


    [SerializeField]
    private GameObject roomPrefab;

    public void MovePlayer(GameObject player, Room room ,Transform entryPoint)
    {
        player.transform.position = entryPoint.position;
        currentRoom = room;
        CameraController.Instance.MoveCamera(room.transform.position);
    }

    public void CreateRoom()
    {

        Room startingRoom = Instantiate(roomPrefab, Vector3.zero , Quaternion.identity).GetComponent<Room>();

        roomList.Add(startingRoom);

        foreach (Door door in startingRoom.DoorList)
        {
            Vector3 newDoorDir = door.DoorDir;
            newDoorDir.x *= roomWidth/2 + 8;
            newDoorDir.y *= roomHeight/2 + 8;

            Room nextRoom = Instantiate(roomPrefab, door.transform.position + newDoorDir , Quaternion.identity).GetComponent<Room>(); // vytváøení místnosti


            door.connectedRoom = nextRoom; //propojení místností
            Door oppositeDoor = nextRoom.DoorList.Find(d => d.DoorDir == door.DoorDir*-1); //najde dveøe opaèné v protejsi mistnosti
            oppositeDoor.connectedRoom = startingRoom;


            oppositeDoor.connectedDoor = door;  //propojení dveøí
            door.connectedDoor = oppositeDoor;
        }

    }

    private void Start()
    {
        CreateRoom();
    }

    private void Awake()
    {
        RM = this;

        roomList = new List<Room>();
    }
}
