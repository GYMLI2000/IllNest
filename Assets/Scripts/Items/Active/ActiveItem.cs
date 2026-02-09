using UnityEngine;

public abstract class ActiveItem : Item
{
    public int itemID { protected set; get; }
    public override bool isPassive => false;
}
