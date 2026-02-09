using UnityEngine;

[CreateAssetMenu(fileName = "Viagra", menuName = "Items/Passive/Viagra")]
public class Viagra : PassiveItem
{
    public override void Effect(Player player)
    {
        
    }

    public override void OnAdd(Player player)
    {
        player.damageMult += 1;
        player.firerateMult -= 0.35f;
        player.projSize *= 2;
    }

    public override void OnRemove(Player player)
    {
        player.damageMult -= 1;
        player.firerateMult += 0.35f;
        player.projSize /= 2;
    }
}
