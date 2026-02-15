using UnityEngine;

[CreateAssetMenu(fileName = "Vaccine", menuName = "Items/Passive/Vaccine")]
public class Vaccine : PassiveItem
{

    public Vaccine()
    {
        itemName = "Vaccine";
    }

    public override void Effect(Player player)
    {

    }

    public override void OnAdd(Player player)
    {
        player.diseaseImunity += 3;
    }

    public override void OnRemove(Player player)
    {
        player.diseaseImunity -= 3;
    }
}
