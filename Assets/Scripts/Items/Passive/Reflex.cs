using UnityEngine;

[CreateAssetMenu(fileName = "Reflex", menuName = "Items/Passive/Reflex")]
public class Reflex : PassiveItem
{
    ReflexEffect effect = new ReflexEffect();

    public override void Effect(Player player)
    {

    }

    public override void OnAdd(Player player)
    {
        player.projEffects.Add(effect);
    }

    public override void OnRemove(Player player)
    {
        player.projEffects.Remove(effect);
    }
}
