using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

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
    [SerializeField] GameObject itemPedestalPrefab;
    [SerializeField] List<GameObject> bossRoomPrefabs;
    [SerializeField] GameObject currentBossRoomPrefab;
    [SerializeField] Sprite bossDoor;
    [SerializeField] List<GameObject> roomLayouts;
    public List<Enemy> lesserEnemies;
    public List<Item> items;

    [SerializeField] public int currentFloor { get; private set; } = 1;
    [SerializeField] public int maxFloors { get; private set; } = 2;

    private Dictionary<Vector2Int, RoomNode> map = new();
    private Dictionary<Vector2Int, Room> spawnedRooms = new();

    public event Action roomClear;
    public event Action roomEnter;
    public event Action floorEnter;

    public event Action<Vector2Int> OnMapUpdated;
    public Dictionary<Vector2Int, RoomNode> GetMap() => map;

    private void Awake()
    {
        RM = this;
    }

    private void Start()
    {
        currentBossRoomPrefab = bossRoomPrefabs[0];
        GenerateLayout();
        SpawnRooms();
        ConnectDoors();

        UpdateMapDiscovery(Vector2Int.zero);
    }

    private void OnEnable()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.StartMusicSystem();
    }

    void GenerateLayout()
    {
        map.Clear();
        Vector2Int start = Vector2Int.zero;
        map[start] = new RoomNode { gridPos = start };

        List<Vector2Int> positions = new() { start };

        while (map.Count < roomCount)
        {
            Vector2Int basePos = positions[UnityEngine.Random.Range(0, positions.Count)];
            Vector2Int dir = GetRandomDir();
            Vector2Int newPos = basePos + dir;

            if (map.ContainsKey(newPos))
                continue;

            map[newPos] = new RoomNode { gridPos = newPos };
            positions.Add(newPos);
        }

        foreach (var node in map.Values)
        {
            Vector2Int p = node.gridPos;
            node.up = map.ContainsKey(p + Vector2Int.up);
            node.down = map.ContainsKey(p + Vector2Int.down);
            node.left = map.ContainsKey(p + Vector2Int.left);
            node.right = map.ContainsKey(p + Vector2Int.right);
        }
    }

    Vector2Int GetRandomDir()
    {
        return UnityEngine.Random.Range(0, 4) switch
        {
            0 => Vector2Int.up,
            1 => Vector2Int.down,
            2 => Vector2Int.left,
            _ => Vector2Int.right
        };
    }

    void SpawnRooms()
    {
        AssignRoomTypes();

        // Use a copy of the keys because we might add the Boss Arena to the map during the loop
        List<Vector2Int> keys = new List<Vector2Int>(map.Keys);

        foreach (var gridPos in keys)
        {
            RoomNode node = map[gridPos];
            Vector3 worldPos = new Vector3(gridPos.x * (roomWidth + roomGap), gridPos.y * (roomHeight + roomGap), 0);

            Room room = Instantiate(roomPrefab, worldPos, Quaternion.identity, this.transform).GetComponent<Room>();
            room.roomType = node.roomType;
            room.worldPos = worldPos;
            room.gridPos = gridPos;
            spawnedRooms[gridPos] = room;

            if (room.roomType == RoomType.Start)
            {
                room.ClearRoom();
                room.enemySpawner = null;
            }
            else if (room.roomType == RoomType.Item)
            {
                room.ClearRoom();
                room.enemySpawner = null;
                Instantiate(itemPedestalPrefab, worldPos, Quaternion.identity, room.grid);
            }
            else if (room.roomType == RoomType.Boss)
            {
                room.ClearRoom();
                room.enemySpawner = null;

                Vector2Int exitDir = GetBossExitDir(node);
                Vector2Int arenaGridPos = gridPos + exitDir;

               
                Vector3 arenaPos = worldPos + new Vector3(exitDir.x * (roomWidth + roomGap) *4, exitDir.y * (roomHeight + roomGap) * 4, 0);

                BossRoom bossArena = Instantiate(currentBossRoomPrefab, arenaPos, Quaternion.identity, this.transform).GetComponent<BossRoom>();
                bossArena.roomType = RoomType.Boss;
                bossArena.worldPos = arenaPos;
                bossArena.gridPos = arenaGridPos;

                
                spawnedRooms[arenaGridPos] = bossArena;
                if (!map.ContainsKey(arenaGridPos))
                {
                    map[arenaGridPos] = new RoomNode { gridPos = arenaGridPos, roomType = RoomType.Boss };
                }
            }
            else
            {
                int layoutIndex = UnityEngine.Random.Range(0, roomLayouts.Count);
                EnemySpawner roomLayout = Instantiate(roomLayouts[layoutIndex], room.grid).GetComponent<EnemySpawner>();
                room.enemySpawner = roomLayout;
                roomLayout.parentRoom = room;
            }
        }
    }

    void ConnectDoors()
    {
        foreach (var kvp in spawnedRooms)
        {
            Vector2Int pos = kvp.Key;
            Room room = kvp.Value;

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

                bool isArenaConnection = (room is BossRoom || targetRoom is BossRoom);

                if (isArenaConnection)
                {
                    bool validBossLink = (room is BossRoom && targetRoom.roomType == RoomType.Boss && !(targetRoom is BossRoom)) ||
                                         (targetRoom is BossRoom && room.roomType == RoomType.Boss && !(room is BossRoom));

                    if (!validBossLink)
                    {
                        door.gameObject.SetActive(false);
                        door.doorWall.SetActive(true);
                        continue;
                    }
                }
                Door targetDoor = targetRoom.DoorList.Find(d => Vector2Int.RoundToInt(d.DoorDir) == -dir);

                if (targetDoor != null)
                {
                    door.connectedRoom = targetRoom;
                    door.connectedDoor = targetDoor;
                    door.gameObject.SetActive(true);
                    door.doorWall.SetActive(false);

                    if (room.roomType == RoomType.Boss && targetRoom.roomType == RoomType.Boss)
                    {
                        var sr = door.GetComponentInChildren<SpriteRenderer>();
                        if (sr != null) sr.sprite = bossDoor;
                    }
                }
            }
        }
    }

    public Vector2Int GetPlayerPos()
    {
        if (currentRoom != null)
        {
            Vector2Int playerPos = currentRoom.gridPos;
            return playerPos;
        }
        return Vector2Int.zero;
    }

    private void AssignRoomTypes()
    {
        // 1. Set Start
        map[Vector2Int.zero].roomType = RoomType.Start;

        // 2. Get all rooms EXCEPT start, sorted by distance (furthest first)
        var potentialCandidates = map.Keys
            .Where(p => p != Vector2Int.zero)
            .OrderByDescending(p => Vector2Int.Distance(Vector2Int.zero, p))
            .ToList();

        // 3. Try to find the best Boss Foyer
        Vector2Int? bossPos = null;

        // First pass: Look for a Dead End (1 connection) that has space for an Arena
        foreach (var pos in potentialCandidates)
        {
            int connections = (map[pos].up ? 1 : 0) + (map[pos].down ? 1 : 0) +
                              (map[pos].left ? 1 : 0) + (map[pos].right ? 1 : 0);

            if (connections == 1 && HasEmptyNeighbor(pos))
            {
                bossPos = pos;
                break;
            }
        }

        // Second pass: If no dead end with space, take ANY room with space (furthest first)
        if (bossPos == null)
        {
            foreach (var pos in potentialCandidates)
            {
                if (HasEmptyNeighbor(pos))
                {
                    bossPos = pos;
                    break;
                }
            }
        }

        // Final Check: If we STILL haven't found a spot (extremely rare), regenerate the map
        if (bossPos == null)
        {
            Debug.LogWarning("No room had space for a boss! Regenerating...");
            GenerateLayout();
            SpawnRooms();
            return; // Exit this execution, the new call will handle it
        }

        // Assign the Boss Foyer
        map[bossPos.Value].roomType = RoomType.Boss;
        potentialCandidates.Remove(bossPos.Value);

        // 4. Assign Item Room (Take the next furthest available room)
        if (potentialCandidates.Count > 0)
        {
            // Pick a random one from the remaining furthest rooms to keep it interesting
            int index = Random.Range(0, Mathf.Min(3, potentialCandidates.Count));
            Vector2Int itemPos = potentialCandidates[index];
            map[itemPos].roomType = RoomType.Item;
        }
    }

    private bool HasEmptyNeighbor(Vector2Int pos)
    {
        return !map.ContainsKey(pos + Vector2Int.up) || !map.ContainsKey(pos + Vector2Int.down) ||
               !map.ContainsKey(pos + Vector2Int.left) || !map.ContainsKey(pos + Vector2Int.right);
    }

    private Vector2Int GetBossExitDir(RoomNode foyerNode)
    {
        // Must return a direction that is NOT in the map to avoid collisions
        if (!map.ContainsKey(foyerNode.gridPos + Vector2Int.up)) return Vector2Int.up;
        if (!map.ContainsKey(foyerNode.gridPos + Vector2Int.down)) return Vector2Int.down;
        if (!map.ContainsKey(foyerNode.gridPos + Vector2Int.left)) return Vector2Int.left;
        return Vector2Int.right;
    }

    public void MovePlayer(GameObject player, Room room, Transform entryPoint)
    {
        StartCoroutine(MoveRoutine(player, room, entryPoint));
    }

    private IEnumerator MoveRoutine(GameObject player, Room room, Transform entryPoint)
    {
        if (entryPoint != null)
        {
            player.transform.position = entryPoint.position;
        }
        else
        {
            player.transform.position = room.worldPos;
        }

        if (currentRoom != null) currentRoom.LeaveRoom();

        room.EnterRoom();
        currentRoom = room;
        UpdateMapDiscovery(room.gridPos);

        if (!room.isCleared)
        {
            room.ActivateRoom();
            roomEnter?.Invoke();
        }

        if (CameraController.Instance != null)
            CameraController.Instance.MoveCamera(room.transform.position);

        yield return null;
    }

    public void ClearRoom(Room room)
    {
        room.ClearRoom();
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX("RoomClear");
        roomClear?.Invoke();
    }

    private void UpdateMapDiscovery(Vector2Int currentPos)
    {
        if (map.TryGetValue(currentPos, out RoomNode currentNode))
        {
            // Mark current room as visited
            currentNode.state = RoomState.Visited;

            // Check adjacent rooms and mark them as discovered if they exist and are hidden
            if (currentNode.up && map.ContainsKey(currentPos + Vector2Int.up))
                if (map[currentPos + Vector2Int.up].state == RoomState.Hidden) map[currentPos + Vector2Int.up].state = RoomState.Discovered;

            if (currentNode.down && map.ContainsKey(currentPos + Vector2Int.down))
                if (map[currentPos + Vector2Int.down].state == RoomState.Hidden) map[currentPos + Vector2Int.down].state = RoomState.Discovered;

            if (currentNode.left && map.ContainsKey(currentPos + Vector2Int.left))
                if (map[currentPos + Vector2Int.left].state == RoomState.Hidden) map[currentPos + Vector2Int.left].state = RoomState.Discovered;

            if (currentNode.right && map.ContainsKey(currentPos + Vector2Int.right))
                if (map[currentPos + Vector2Int.right].state == RoomState.Hidden) map[currentPos + Vector2Int.right].state = RoomState.Discovered;
        }

        // Tell the UI to update
        OnMapUpdated?.Invoke(currentPos);
    }


    public void LoadNextFloor()
    {

        foreach (var kvp in spawnedRooms)
        {
            if (kvp.Value != null)
            {
                Destroy(kvp.Value.gameObject);
            }
        }


        spawnedRooms.Clear();
        map.Clear();
        currentRoom = null;


        // Difficulty increase ??
        currentBossRoomPrefab = bossRoomPrefabs[Mathf.Min(bossRoomPrefabs.Count - 1, bossRoomPrefabs.IndexOf(currentBossRoomPrefab) + 1)];
        roomCount += 5;
        currentFloor++;
        // end

        GenerateLayout();
        SpawnRooms();
        ConnectDoors();


        Player playerScript = FindAnyObjectByType<Player>();
        var player = playerScript.gameObject;
        if (player != null)
        { 
            Room startRoom = spawnedRooms[Vector2Int.zero];

            player.transform.position = startRoom.worldPos;


            if (CameraController.Instance != null)
            {
                CameraController.Instance.ChangeCameraPosition(startRoom.transform.position);
            }
            else
            {
                Debug.LogWarning("CameraController not found! Make sure you have a CameraController in your scene.");
            }



            startRoom.EnterRoom();
            currentRoom = startRoom;

            floorEnter?.Invoke();
            UpdateMapDiscovery(Vector2Int.zero);

        }
        else
        {
            Debug.LogWarning("Player not found! Make sure your player has the 'Player' tag.");
        }
    }
}

public class RoomNode
{
    public Vector2Int gridPos;

    public bool up;
    public bool down;
    public bool left;
    public bool right;

    public RoomType roomType = RoomType.Normal;
    public RoomState state = RoomState.Hidden;
}

public enum RoomType
{
    Start,
    Normal,
    Item,
    Boss
}

public enum RoomState
{
    Hidden,      // Not on map yet
    Discovered,  // Adjacent to a visited room (shows unentered icon)
    Visited      // Player has been inside (shows true room icon)
}
