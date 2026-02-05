using UnityEngine;

public class DeppresionDebuff : Debuff
{
    //private float originalMoveSpeed;  -- Možná jestì pøedelám logiku aby to fungovalo i s itemy ktery speed nasobi

    private float startTime;
    private float switchTime = 8f;
    private bool manic = false;
    private bool manicClear = false;
    private bool deppresionClear = false;
    private Color currentColor;

    public DeppresionDebuff(int duration, float magnitude) : base(duration, magnitude)
    {
        this.duration = duration;
        this.magnitude = magnitude;
        debuffID =2;
    }

    public override void Effect(Player player)
    {
        if (!manic && Time.time - startTime >= switchTime)
        {
            manic = true;
            player.movementSpeed += magnitude * 2;
            player.projSpeed += magnitude * 2;
            currentColor = new Color(240f/255f, 163f/255f, 163f/255f);

            startTime = Time.time;
        }
        else if (manic && Time.time - startTime >= switchTime)
        {
            manic = false;
            player.movementSpeed -= magnitude * 2;
            player.projSpeed -= magnitude * 2;
            currentColor = new Color(0, 204f/255f, 1);

            startTime = Time.time;
        }

        foreach (SpriteRenderer s in player.spriteRenderer)
        {
            if (s.color == Color.white || Time.time - startTime == 0)
            {
                s.color = currentColor;
            }
        }
    }

    public override void OnAdd(Player player)
    {
        manic = false;
        currentColor = new Color(0, 204f/255f, 1);
        startTime = Time.time;
        player.movementSpeed -= magnitude;
        player.projSpeed -= magnitude;
        manicClear = false;
        deppresionClear = false;

    }

    public override void OnRemove(Player player)
    {
        if (manic)
        {
            player.movementSpeed -= magnitude;
            player.projSpeed -= magnitude;
        }
        else
        {
            player.movementSpeed += magnitude;
            player.projSpeed += magnitude;
        }
        foreach (SpriteRenderer s in player.spriteRenderer)
        {
            s.color = Color.white;
        }
    }

    public override void OnEnterRoom()
    {
    }
    public override void OnClearRoom()
    {
        if (manic && !manicClear)
        {
            manicClear = true;
            currentDuration++;
        }
        else if (!manic && !deppresionClear)
        {
            deppresionClear = true;
            currentDuration++;
        }
    }
}
