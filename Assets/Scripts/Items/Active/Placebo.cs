using NUnit.Framework;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

[CreateAssetMenu(fileName = "Placebo", menuName = "Items/Active/Placebo")]
public class Placebo : ActiveItem
{
    private List<Debuff> debuffs = new List<Debuff>();

    private Player player;

    public Placebo()
    {
        chargeRequired = 3;
        currentCharge = chargeRequired;
        itemName = "Placebo";
    }

    public override void OnAdd()
    {
        RoomManager.RM.roomClear += RemoveEffect;
    }

    public override void OnRemove()
    {
        RoomManager.RM.roomClear -= RemoveEffect;
    }

    public override void UpdateItem(Player player) { }

    protected override void Activate(Player player)
    {
        this.player = player;
        debuffs.Clear();

        for (int i = player.debuffManager.activeDebuffs.Count-1; i>= 0; i--)
        {
            Debuff debuff = player.debuffManager.activeDebuffs[i];
            debuffs.Add(debuff);
            player.debuffManager.RemoveDebuff(debuff);
        }

        player.debuffManager.ChangeDebuffs(false);
    }

    protected override void RemoveEffect()
    {
        if(player == null) return;

        player.debuffManager.ChangeDebuffs(true);

        foreach (Debuff debuff in debuffs)
        {
            player.debuffManager.AddDebuff(debuff);
        }

        debuffs.Clear();
    }
}
