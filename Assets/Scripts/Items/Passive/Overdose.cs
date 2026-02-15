using UnityEngine;

[CreateAssetMenu(fileName = "Overdose", menuName = "Items/Passive/Overdose")]
public class Overdose : PassiveItem
{
    OverdoseEffect effect = new OverdoseEffect();

    public Overdose()
    {
        itemName = "Overdose";
    }

    public override void Effect(Player player) { }

    public override void OnAdd(Player player)
    {
        player.projEffects.Add(effect);
    }

    public override void OnRemove(Player player)
    {
        player.projEffects.Remove(effect);
    }
}
