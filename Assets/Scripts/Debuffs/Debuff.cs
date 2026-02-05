using System;
using UnityEngine;

public abstract class Debuff
{
    public int debuffID { protected set; get; }
    public int duration;
    public int currentDuration = 0;
    protected float magnitude;
    public bool isApplied = false;



    public Debuff(int duration, float magnitude)
    {
        this.duration = duration;
        this.magnitude = magnitude;
    }


    public void CheckDuration()
    {
        if (currentDuration >= duration)
        {
            isApplied = false;
        }
    }

    public abstract void OnAdd(Player player);

    public abstract void Effect(Player player);

    public abstract void OnRemove(Player player);

    public abstract void OnEnterRoom();
    public abstract void OnClearRoom();

}
