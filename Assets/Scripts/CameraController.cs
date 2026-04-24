using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance;

    [SerializeField]
    private float cameraSpeed;
    [SerializeField]
    private Transform playerTransform;

    private Vector3 targetPosition;
    private Camera cam;

    private bool isBossMode = false;
    private float minX, maxX, minY, maxY;


    private void Awake()
    {
        Instance = this;
        cam = GetComponent<Camera>();
        targetPosition = new Vector3(0,0,-10);

    }

    private void Update()
    {

        if (isBossMode && playerTransform != null)
        {
            float camHeight = cam.orthographicSize;
            float camWidth = camHeight * cam.aspect;

            float finalX, finalY;

            // Check if room width is smaller than camera view
            if (maxX - minX < camWidth * 2)
                finalX = (minX + maxX) / 2f; // Center it
            else
                finalX = Mathf.Clamp(playerTransform.position.x, minX + camWidth, maxX - camWidth);

            // Check if room height is smaller than camera view
            if (maxY - minY < camHeight * 2)
                finalY = (minY + maxY) / 2f; // Center it
            else
                finalY = Mathf.Clamp(playerTransform.position.y, minY + camHeight, maxY - camHeight);

            targetPosition = new Vector3(finalX, finalY, -10);
        }


        transform.position = Vector3.MoveTowards(
            transform.position,
            new Vector3(targetPosition.x,targetPosition.y,-10),
            cameraSpeed* Time.deltaTime
            );
    }

    public void MoveCamera(Vector3 position)
    {
        targetPosition = position;
    }


    public void ChangeCameraPosition(Vector3 position)
    {
        transform.position=position - new Vector3(0,0,10);
        targetPosition=position;
    }

    public void StartBossCamera(float _minX, float _maxX, float _minY, float _maxY)
    {
        minX = _minX;
        maxX = _maxX;
        minY = _minY;
        maxY = _maxY;
        isBossMode = true;
    }
    public void StopBossCamera()
    {
        isBossMode = false;
    }
}
