using UnityEngine;

public class DeppresionDebuff : Debuff
{
    //private float originalMoveSpeed;  -- Možná jestì pøedelám logiku aby to fungovalo i s itemy ktery speed nasobi

    public DeppresionDebuff(int duration, float magnitude) : base(duration, magnitude)
    {
        this.duration = duration;
        this.magnitude = magnitude;
        debuffID =2;
    }

    public override void Effect(Player player)
    {
        foreach (SpriteRenderer s in player.spriteRenderer)
        {
            if (s.color == Color.white)
            {
                s.color = new Color(0, 204f/255f, 1);
            }
        }
    }

    public override void OnAdd(Player player)
    {
        player.movementSpeed -= magnitude;
        player.projSpeed -= magnitude;

    }

    public override void OnRemove(Player player)
    {
        player.movementSpeed += magnitude;
        player.projSpeed += magnitude;
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
    }
}
