using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance;

    [SerializeField]
    private float cameraSpeed;

    private Vector3 targetPosition;


    private void Awake()
    {
        Instance = this;
        targetPosition = new Vector3(0,0,-10);

    }

    private void Update()
    {
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
        transform.position=position;
        targetPosition=position;
    }
}
