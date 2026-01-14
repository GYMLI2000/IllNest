using UnityEngine;

public class AlzheimerDebuff : Debuff
{
    public AlzheimerDebuff(int duration, float magnitude) : base(duration, magnitude)
    {
        this.duration = duration;
        this.magnitude = magnitude;
        debuffID =3;
    }


    AlzheimerFogController fog;

    public float minRadius = 0.4f;
    public float maxRadius = 15f;
    public float shrinkSpeed = 4f;
    public float expandSpeed = 2f;

    float currentRadius;
    Rigidbody2D rb;

    public override void OnAdd(Player player)
    {
        fog = Camera.main.GetComponentInChildren<AlzheimerFogController>();
        if (fog != null)
        {
            fog.minRadius = minRadius;
            fog.maxRadius = maxRadius;
            fog.shrinkSpeed = shrinkSpeed * magnitude;
            fog.expandSpeed = expandSpeed;
            fog.currentRadius = fog.maxRadius-1;
            fog.SetActive(true);
        }
    }

    public override void OnRemove(Player player)
    {
        var fog = Camera.main.GetComponentInChildren<AlzheimerFogController>();
        if (fog != null)
        {
            fog.SetActive(false);
        }
    }

    public override void Effect(Player player)
    {
        if (fog.currentRadius == fog.maxRadius)
        {
            isApplied = false;
        }
    }
}
