using UnityEngine;

[CreateAssetMenu(fileName = "Hallucination", menuName = "Items/Passive/Hallucination")]
public class Hallucination : PassiveItem
{
    HallucinationEffect effect = new HallucinationEffect();

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
