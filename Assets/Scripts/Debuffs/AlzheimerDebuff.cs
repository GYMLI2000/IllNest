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
    public float maxRadius = 12f;
    public float shrinkSpeed = 4f;
    public float expandSpeed = 2f;

    float currentRadius;
    Rigidbody2D rb;

    public override void OnAdd(Player player)
    {
        fog = Camera.main.GetComponentInChildren<AlzheimerFogController>();
        player.passThrough++;
        if (fog != null)
        {
            fog.minRadius = minRadius;
            fog.currentRadius = (fog.maxRadius-2f/duration) * (currentDuration+1f) + 2f;
            fog.SetActive(true);
        }
    }

    public override void OnRemove(Player player)
    {
        var fog = Camera.main.GetComponentInChildren<AlzheimerFogController>();
        player.passThrough--;
        if (fog != null)
        {
            fog.SetActive(false);
        }
    }

    public override void Effect(Player player)
    {
        fog.currentRadius = (fog.maxRadius-2f/duration) * (currentDuration+1f) + 2f;
    }

    public override void OnEnterRoom()
    {
    }
    public override void OnClearRoom()
    {
        currentDuration++;
    }
}
