using UnityEngine;

public class AlzheimerFogController : MonoBehaviour
{
    public Transform player;
    public SpriteRenderer fogSprite;

    public float minRadius = 0.5f;
    public float maxRadius = 2f;
    public float shrinkSpeed = 1f;
    public float expandSpeed = 2f;

    public float currentRadius;
    public float targetRadius;
    public float transitionSpeed = 15f;

    private Material fogMaterial;
    private bool active = false;
    private bool isFadingOut = false; // Tracks if we are currently zooming out

    void Awake()
    {
        if (!fogSprite || !player)
        {
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
        targetRadius = maxRadius;
        fogSprite.enabled = false;
    }

    void Update()
    {
        if (!active) return;

        // Smoothly glide currentRadius to the targetRadius
        currentRadius = Mathf.Lerp(currentRadius, targetRadius, Time.deltaTime * transitionSpeed);

        // If we are transitioning out and the circle is huge, disable everything
        if (isFadingOut && currentRadius >= 29.5f)
        {
            active = false;
            fogSprite.enabled = false;
            isFadingOut = false;
            return;
        }

        if (!isFadingOut && currentRadius <= minRadius + 0.01f)
        {
            shrinkSpeed = 0.3f;
        }

        UpdateMaterial();
    }

    void UpdateMaterial()
    {
        fogMaterial.SetVector("_PlayerPos", player.position);
        fogMaterial.SetFloat("_Radius", currentRadius);
    }

    public void SetActive(bool value)
    {
        if (value)
        {
            // Start the zoom-in transition
            active = true;
            isFadingOut = false;
            fogSprite.enabled = true;
            currentRadius = 30f;
        }
        else if (active && !isFadingOut)
        {
            // Start the zoom-out transition instead of instantly vanishing
            isFadingOut = true;
            targetRadius = 30f;
        }
    }
}