using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public static CursorManager Instance { get; private set; }

    [Header("Cursor Textures")]
    public Texture2D defaultCursor;
    public Texture2D aimCursor;

    [Header("Click Hotspots")]
    public Vector2 defaultHotspot = Vector2.zero;
    public Vector2 aimHotspot = new Vector2(16, 16);

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        SetDefaultCursor();
    }

    public void SetAimCursor()
    {
        Cursor.SetCursor(aimCursor, aimHotspot, CursorMode.Auto);
    }

    public void SetDefaultCursor()
    {
        Cursor.SetCursor(defaultCursor, defaultHotspot, CursorMode.Auto);
    }
}