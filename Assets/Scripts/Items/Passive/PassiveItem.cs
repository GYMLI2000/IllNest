using UnityEngine;

public abstract class PassiveItem : Item
{
    public int itemID { protected set; get; }
    public override bool isPassive => true;



    public abstract void OnAdd(Player player);

    public abstract void Effect(Player player);

    public abstract void OnRemove(Player player);

}
