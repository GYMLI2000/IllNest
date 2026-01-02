using UnityEngine;

public class AlzheimerFogController : MonoBehaviour
{
    public Transform player;
    public SpriteRenderer fogSprite;

    public float minRadius = 0.5f;
    public float maxRadius = 1.5f;
    public float shrinkSpeed = 0.5f;
    public float expandSpeed = 1f;
    private bool canExpand = false;

    public float currentRadius;
    private Material fogMaterial;
    private bool active = false;

    void Awake()
    {
        if (!fogSprite || !player)
        {
            Debug.LogError("Fog Sprite nebo Player není pøiøazen!");
            enabled = false;
            return;
        }
    
        fogMaterial = Instantiate(fogSprite.sharedMaterial);
        fogSprite.material = fogMaterial;


        float spriteHeight = fogSprite.sprite.bounds.size.y;
        float spriteWidth = fogSprite.sprite.bounds.size.x;

        float cameraHeight = Camera.main.orthographicSize * 2f;
        float cameraWidth = cameraHeight * Camera.main.aspect;

        float scaleX = cameraWidth / spriteWidth;
        float scaleY = cameraHeight / spriteHeight;

        fogSprite.transform.localScale = new Vector3(scaleX, scaleY, 1f);

        currentRadius = maxRadius;
        fogSprite.enabled = false;
    }

    void Update()
    {
        if (!active) return;

        if (currentRadius == minRadius)
        {
            canExpand = true;
        }

        bool moving = player.GetComponent<Rigidbody2D>().linearVelocity.magnitude > 0.1f;

        currentRadius += (moving && canExpand ? expandSpeed : -shrinkSpeed) * Time.deltaTime;
        currentRadius = Mathf.Clamp(currentRadius, minRadius, maxRadius);

        UpdateMaterial();
    }

    void UpdateMaterial()
    {
        fogMaterial.SetVector("_PlayerPos", player.position);
        fogMaterial.SetFloat("_Radius", currentRadius);
    }

    public void SetActive(bool value)
    {
        active = value;
        fogSprite.enabled = value;
        canExpand = false;
    }
}
