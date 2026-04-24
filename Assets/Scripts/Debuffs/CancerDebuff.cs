using System;
using UnityEngine;

public class CancerDebuff : Debuff
{
    private float timer = 0f;

    private void OnEnemyDeath(HostileEntity entity)
    {
        currentDuration +=15;
        manager.DebuffChanged();
    }

    private void OnCancerHit()
    {
        currentDuration += 3;
        manager.DebuffChanged();
    }

    public CancerDebuff(int duration, float magnitude) : base(duration, magnitude)
    {
        this.duration = duration;
        this.magnitude = magnitude;
        debuffID =4;
    }

    public override void Effect(Player player)
    {
        timer += Time.deltaTime;
        if (timer >= 0.5f)
        {
            currentDuration--;
            manager.DebuffChanged();
            timer = 0f;
            if (currentDuration <= 0)
            {
                player.TakeDamage(100);
            }
        }
    }

    public override void OnAdd(Player player)
    {
        Enemy.EntityDeath += OnEnemyDeath;
        CancerBoss.OnCancerHit += OnCancerHit;
        currentDuration = duration/2;
        UIManager.UM.ChangeHealthColor(new Color(241f/255f, 77f/255f, 50f/255f));
        AudioManager.Instance.PlaySFX("CancerDebuff");
    }

    public override void OnClearRoom()
    {
        isApplied = false;
    }

    public override void OnEnterRoom()
    {
    }

    public override void OnRemove(Player player)
    {
        UIManager.UM.ChangeHealthColor(Color.white);
        Enemy.EntityDeath -= OnEnemyDeath;
        CancerBoss.OnCancerHit -= OnCancerHit;
    }
}
