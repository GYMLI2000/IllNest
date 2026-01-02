using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public static RoomManager RM;

    private Room currentRoom;

    [Header("Settings")]
    [SerializeField] int roomCount = 12;
    [SerializeField] int roomWidth = 32;
    [SerializeField] int roomHeight = 18;
    [SerializeField] int roomGap = 6;
    [SerializeField] GameObject roomPrefab;

    private Dictionary<Vector2Int, RoomNode> map = new();
    private Dictionary<Vector2Int, Room> spawnedRooms = new();

    private void Awake()
    {
        RM = this;
    }

    private void Start()
    {
        GenerateLayout();
        SpawnRooms();
        ConnectDoors();
    }

    void GenerateLayout()
    {
        map.Clear();

        Vector2Int start = Vector2Int.zero;
        map[start] = new RoomNode { gridPos = start };

        List<Vector2Int> positions = new() { start };

        while (map.Count < roomCount)
        {
            Vector2Int basePos = positions[Random.Range(0, positions.Count)];
            Vector2Int dir = GetRandomDir();
            Vector2Int newPos = basePos + dir;

            if (map.ContainsKey(newPos))
                continue;

            map[newPos] = new RoomNode { gridPos = newPos };
            positions.Add(newPos);
        }

        // dopoèítání dveøí
        foreach (var node in map.Values)
        {
            Vector2Int p = node.gridPos;

            node.up    = map.ContainsKey(p + Vector2Int.up);
            node.down  = map.ContainsKey(p + Vector2Int.down);
            node.left  = map.ContainsKey(p + Vector2Int.left);
            node.right = map.ContainsKey(p + Vector2Int.right);
        }
    }

    Vector2Int GetRandomDir()
    {
        return Random.Range(0, 4) switch
        {
            0 => Vector2Int.up,
            1 => Vector2Int.down,
            2 => Vector2Int.left,
            _ => Vector2Int.right
        };
    }

    void SpawnRooms()
    {
        foreach (var kvp in map)
        {
            Vector2Int gridPos = kvp.Key;

            Vector3 worldPos = new Vector3(
                gridPos.x * (roomWidth + roomGap),
                gridPos.y * (roomHeight + roomGap),
                0
            );


            Room room = Instantiate(roomPrefab, worldPos, Quaternion.identity).GetComponent<Room>();
            spawnedRooms[gridPos] = room;
        }
    }

    void ConnectDoors()
    {
        foreach (var kvp in map)
        {
            Vector2Int pos = kvp.Key;
            RoomNode node = kvp.Value;
            Room room = spawnedRooms[pos];

            foreach (Door door in room.DoorList)
            {
                Vector2Int dir = Vector2Int.RoundToInt(door.DoorDir);

                Vector2Int targetPos = pos + dir;

                if (!spawnedRooms.ContainsKey(targetPos))
                {
                    door.gameObject.SetActive(false);
                    door.doorWall.SetActive(true);
                    continue;
                }

                Room targetRoom = spawnedRooms[targetPos];
                Door targetDoor = targetRoom.DoorList
                    .Find(d => Vector2Int.RoundToInt(d.DoorDir) == -dir);

                door.connectedRoom = targetRoom;
                door.connectedDoor = targetDoor;
            }
        }
    }

    public void MovePlayer(GameObject player, Room room, Transform entryPoint)
    {
        StartCoroutine(MoveRoutine(player, room, entryPoint));
    }

    private IEnumerator MoveRoutine(GameObject player, Room room, Transform entryPoint)
    {

        player.transform.position = entryPoint.position;
        currentRoom = room;
        if (!room.isCleared)
        {
            room.ActivateRoom();
        }

        CameraController.Instance.MoveCamera(room.transform.position);

        yield return null; // 1 frame ochrana

    }

}

public class RoomNode
{
    public Vector2Int gridPos;

    public bool up;
    public bool down;
    public bool left;
    public bool right;
}
