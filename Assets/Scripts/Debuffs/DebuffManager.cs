using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public class DebuffManager : MonoBehaviour
{
    public event Action<List<Debuff>> changeDebuff;
    public List<Debuff> activeDebuffs { get; private set; } = new List<Debuff>();

    [SerializeField]
    private Player target;

    private void Awake()
    {
        RoomManager.RM.roomEnter += OnRoomEnter;
        RoomManager.RM.roomClear += OnRoomClear;
    }

    public void OnRoomEnter()
    {
        foreach (var debuff in activeDebuffs)
        {
            debuff.OnEnterRoom();
        }
    }

    private void OnRoomClear()
    {
        foreach (var debuff in activeDebuffs)
        {
            debuff.OnClearRoom();
        }
        changeDebuff?.Invoke(activeDebuffs);
    }

    public void AddDebuff(Debuff debuff)
    {
        if (activeDebuffs.FirstOrDefault(d => d.GetType() == debuff.GetType()) != null)
        {
            //debuff uz je na hracovi
        }
        else
        {
            activeDebuffs.Add(debuff);
            debuff.isApplied = true;
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
