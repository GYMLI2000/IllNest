using UnityEngine;

[CreateAssetMenu(fileName = "Vitamins", menuName = "Items/Passive/Vitamins")]
public class Vitamins : PassiveItem
{
    public override void Effect(Player player) { }

    public override void OnAdd(Player player)
    {
        player.maxHp += 4;
        player.diseaseImunity += 1f;
        player.currentHp = player.maxHp;
        player.UpdateHp();
    }

    public override void OnRemove(Player player)
    {
        player.maxHp -= 4;
        player.diseaseImunity -= 1f;
        player.UpdateHp();
    }
}
