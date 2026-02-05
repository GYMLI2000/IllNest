using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class ParkinsonDebuff : Debuff
{
    private bool hasMissed = false;
    public ParkinsonDebuff(int duration, float magnitude) : base(duration, magnitude)
    {
        this.duration = duration;
        this.magnitude = magnitude;
        debuffID =1;
    }

    public override void OnAdd(Player player)
    {
        player.animator.SetBool("ParkinsonDebuff", true);
        if (player.atkCooldown - 0.2f > 0f)
        {
            player.atkCooldown -= 0.2f;
        }
        Debug.Log(player.animator.GetCurrentAnimatorStateInfo(1).IsName("ParkinsonDebuff"));
        Projectile.ProjectileHit += OnProjectileHit;
    }

    private void OnProjectileHit(bool hasHit)
    {
        if (!hasHit)
        {
            hasMissed = true;
        }
        Debug.Log("Projectile hit: " + hasHit);
    }

    public override void Effect(Player player)
    {
        if (player.rb.linearVelocity.magnitude <= 0.01f)
        {
            player.shootAngle = 0;
            player.animator.SetBool("ParkinsonDebuff", false);
        }
        else
        {
            float value;
            if (Random.value < 0.5f)
            {
                value = Random.Range(-1f, -.3f);
            }
            else
            {
                value = Random.Range(.3f, 1f);
            }
            player.shootAngle = (int)(value * magnitude);
            player.animator.SetBool("ParkinsonDebuff", true);
        }


    }
    
    public override void OnRemove(Player player)
    {
        player.shootAngle = 0;
        player.atkCooldown += 0.2f;
        player.animator.SetBool("ParkinsonDebuff", false);
        Projectile.ProjectileHit -= OnProjectileHit;
    }


    public override void OnEnterRoom()
    {
        hasMissed = false;
    }
    public override void OnClearRoom()
    {
        if (!hasMissed)
        {
            currentDuration++;
        }
    }
}
