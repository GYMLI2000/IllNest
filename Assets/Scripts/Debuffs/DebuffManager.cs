using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DebuffManager : MonoBehaviour
{
    public event Action<List<Debuff>> changeDebuff;
    private List<Debuff> activeDebuffs = new List<Debuff>();

    [SerializeField]
    private Player target;

    

    public void AddDebuff(Debuff debuff)
    {
        if (activeDebuffs.FirstOrDefault(d => d.GetType() == debuff.GetType()) != null)
        {
            debuff.applyTime = Time.time;
        }
        else
        {
            activeDebuffs.Add(debuff);
            debuff.isApplied = true;
            debuff.applyTime = Time.time;
            debuff.OnAdd(target);
            changeDebuff?.Invoke(activeDebuffs);
        }
    }

    public void RemoveDebuff(Debuff debuff)
    {
        debuff.OnRemove(target);
        activeDebuffs.Remove(debuff);
        changeDebuff?.Invoke(activeDebuffs);
    }

    private void Update()
    {
        for (int i = activeDebuffs.Count - 1; i >= 0; i--)
        {
            var debuff = activeDebuffs[i];
            debuff.CheckDuration();

            if (!debuff.isApplied)
            {
                RemoveDebuff(debuff);
            }
            else
            {
                debuff.Effect(target);
            }
        }
    }

    public void ClearDebuffs()
    {
        foreach (var debuff in activeDebuffs)
        {
            debuff.OnRemove(target);
        }
        activeDebuffs.Clear();
        changeDebuff?.Invoke(activeDebuffs);
    }
}
