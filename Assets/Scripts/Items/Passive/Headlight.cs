using UnityEngine;

[CreateAssetMenu(fileName = "Headlight", menuName = "Items/Passive/Headlight")]
public class Headlight : PassiveItem
{
    public Headlight()
    {
        itemName = "Headlight";
    }

    public override void Effect(Player player)
    {
    }

    public override void OnAdd(Player player)
    {
        player.playerLight.pointLightOuterRadius += 3f;
        player.range += 3;
    }

    public override void OnRemove(Player player)
    {
        player.playerLight.pointLightOuterRadius -= 3f;
        player.range -= 3;
    }
}
