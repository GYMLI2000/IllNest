using UnityEngine;

public class SchizophreniaProjectile : Projectile
{
    private float fadeTimer = 0f;

    private void Awake()
    {
        poolKey = "SchizophreniaProjectile";
        partPoolKey = "SchizophreniaProjectileParticle";
    }

    private void OnEnable()
    {
        fadeTimer = 0f;
    }

    protected override void AI()
    {
        parentObject.transform.Translate(direction * currentSpeed * Time.deltaTime, Space.World);
        transform.Rotate(0f, 0f, 360f * Time.deltaTime);
        currentSpeed += 1.5f * Time.deltaTime * (currentSpeed/2);


        SpriteRenderer sr = parentObject.GetComponentInChildren<SpriteRenderer>();

        if (sr != null)
        {
            fadeTimer += Time.deltaTime;

            float fadeProgress = Mathf.Clamp01(fadeTimer);
            float alpha = Mathf.Lerp(1f, 0.2f, fadeProgress);

            Color color = sr.color;
            color.a = alpha;
            sr.color = color;
        }
        else
        {
            Debug.LogWarning("No SpriteRenderer found on projectile.");

        }
    }


}
