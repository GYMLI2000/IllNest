using UnityEngine;

[CreateAssetMenu(fileName = "Paralysis", menuName = "Items/Passive/Paralysis")]
public class Paralysis : PassiveItem
{

    ParalysisEffect effect = new ParalysisEffect();

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
