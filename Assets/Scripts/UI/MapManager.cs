using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MapManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private RectTransform mapContainer;
    [SerializeField] private GameObject mapIconPrefab; // A simple UI Image prefab

    [Header("Map Settings")]
    [SerializeField] private float iconSpacing = 50f;

    [Header("Icons")]
    [SerializeField] private Sprite unenteredIcon; // The default (?) mark
    [SerializeField] private Sprite normalRoomIcon;
    [SerializeField] private Sprite itemRoomIcon;
    [SerializeField] private Sprite bossRoomIcon;
    [SerializeField] private Sprite startRoomIcon;
    [SerializeField] private Sprite playerIndicatorIcon; // Optional: Icon to show where player is

    // Keep track of spawned UI icons
    private Dictionary<Vector2Int, GameObject> spawnedIcons = new();

    private void Start()
    {
        // Subscribe to the event
        RoomManager.RM.OnMapUpdated += RedrawMap;
        RoomManager.RM.floorEnter += ClearMapUI;
    }

    public void ShowMap()
    {
        if (mapContainer != null)
        {
            if (!mapContainer.gameObject.activeSelf)
            {
                mapContainer.gameObject.SetActive(true);
                RedrawMap(RoomManager.RM.GetPlayerPos());
            }
            else
            {
                mapContainer.gameObject.SetActive(false);
            }
        }
    }

    private void OnDestroy()
    {
        if (RoomManager.RM != null)
            RoomManager.RM.OnMapUpdated -= RedrawMap;
            RoomManager.RM.floorEnter -= ClearMapUI;

    }

    private void RedrawMap(Vector2Int currentPlayerPos)
    {
        var mapData = RoomManager.RM.GetMap();

        foreach (var kvp in mapData)
        {
            Vector2Int pos = kvp.Key;
            RoomNode node = kvp.Value;

            // If the room is hidden, skip it entirely
            if (node.state == RoomState.Hidden) continue;

            // Spawn the icon if it doesn't exist yet
            if (!spawnedIcons.ContainsKey(pos))
            {
                GameObject newIcon = Instantiate(mapIconPrefab, mapContainer);
                RectTransform rt = newIcon.GetComponent<RectTransform>();
                rt.anchoredPosition = new Vector2(pos.x * iconSpacing, pos.y * iconSpacing);
                spawnedIcons[pos] = newIcon;
            }

            Image iconImage = spawnedIcons[pos].GetComponent<Image>();

            // Apply the correct visual based on state
            if (node.state == RoomState.Discovered)
            {
                iconImage.sprite = unenteredIcon;
                iconImage.color = Color.gray; // Dim unentered rooms slightly
            }
            else if (node.state == RoomState.Visited)
            {
                iconImage.sprite = GetIconForType(node.roomType);
                iconImage.color = Color.white; // Full brightness for visited rooms

                // Highlight the room the player is currently in
                if (pos == currentPlayerPos)
                {
                    iconImage.sprite = playerIndicatorIcon ; // Or swap to a player indicator sprite!
                }
            }
        }
    }

    private Sprite GetIconForType(RoomType type)
    {
        return type switch
        {
            RoomType.Start => startRoomIcon,
            RoomType.Item => itemRoomIcon,
            RoomType.Boss => bossRoomIcon,
            _ => normalRoomIcon
        };
    }

    // Optional: Call this from RoomManager.LoadNextFloor() to clear the UI
    public void ClearMapUI()
    {
        foreach (var icon in spawnedIcons.Values) Destroy(icon);
        spawnedIcons.Clear();
    }
}
