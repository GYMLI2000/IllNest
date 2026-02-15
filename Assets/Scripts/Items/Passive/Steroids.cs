using Unity.VisualScripting;
using UnityEngine;


[CreateAssetMenu(fileName = "Steroids", menuName = "Items/Passive/Steroids")]
public class Steroids : PassiveItem
{
    public Steroids()
    {
        itemName = "Steroids";
    }

    public override void Effect(Player player)
    {
    }

    public override void OnAdd(Player player)
    {
        player.damage += 1;
        player.damageMult += .65f;
        player.projSpeed += 3;
        player.maxHp -= 2;
        player.UpdateHp();
    }

    public override void OnRemove(Player player)
    {
        player.damage -= 1;
        player.damageMult -= .65f;
        player.projSpeed -= 3;
    }
}
