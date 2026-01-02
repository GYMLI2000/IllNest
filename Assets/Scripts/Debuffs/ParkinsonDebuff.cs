using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class ParkinsonDebuff : Debuff
{
    private float lowSpeedTimer = 0f;
    public ParkinsonDebuff(int duration, float magnitude) : base(duration, magnitude)
    {
        
    }

    public override void OnAdd(Player player)
    {
        player.animator.SetBool("ParkinsonDebuff", true);
        Debug.Log(player.animator.GetCurrentAnimatorStateInfo(1).IsName("ParkinsonDebuff"));
    }

    public override void Effect(Player player)
    {
        if (player.rb.linearVelocity.magnitude <= 0.01f)
        {
            lowSpeedTimer += Time.deltaTime;
        }
        else
        {
            lowSpeedTimer = 0f;
        }

        if (lowSpeedTimer < 1f)
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
        else
        {
            player.shootAngle = 0;
            player.animator.SetBool("ParkinsonDebuff", false);
        }

    }
    
    public override void OnRemove(Player player)
    {
        player.shootAngle = 0;
        player.animator.SetBool("ParkinsonDebuff", false);
    }

}
