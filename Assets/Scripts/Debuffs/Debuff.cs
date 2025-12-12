using System;
using UnityEngine;

public abstract class Debuff
{
    protected int duration;
    protected float magnitude;
    public float applyTime = 0;
    public bool isApplied = false;


    public Debuff(int duration, float magnitude)
    {
        this.duration = duration;
        this.magnitude = magnitude;
    }


    public void CheckDuration()
    {
        if (isApplied && Time.time >= applyTime + duration)
        {
            isApplied = false;
        }
    }

    public abstract void OnAdd(Player player);

    public abstract void Effect(Player player);

    public abstract void OnRemove(Player player);

}
