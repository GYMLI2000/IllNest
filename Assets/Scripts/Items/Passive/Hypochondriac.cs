using UnityEngine;

[CreateAssetMenu(fileName = "Hypochondriac", menuName = "Items/Passive/Hypochondriac")]
public class Hypochondriac : PassiveItem
{
    private int currentCount;


    public Hypochondriac()
    {
        itemName = "Hypochondriac";
    }

    public override void Effect(Player player)
    {
        if (player.debuffManager.activeDebuffs.Count != currentCount)
        {
            player.movementSpeed -= currentCount;
            player.atkCooldown += 0.1f * currentCount;
            currentCount = player.debuffManager.activeDebuffs.Count;
            player.movementSpeed += currentCount;
            player.atkCooldown -= 0.1f * currentCount;
        }
    }

    public override void OnAdd(Player player)
    {
        currentCount = player.debuffManager.activeDebuffs.Count;
        player.movementSpeed += currentCount;
        player.atkCooldown -= 0.1f * currentCount;
    }

    public override void OnRemove(Player player)
    {
        player.movementSpeed -= currentCount;
        player.atkCooldown += 0.1f * currentCount;
    }
}
