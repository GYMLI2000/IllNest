using UnityEngine;

public abstract class ActiveItem : Item
{
    public int itemID { protected set; get; }
    public override bool isPassive => false;

    public int chargeRequired;

    public int currentCharge{get; protected set;}
    protected bool isActive;

    public void AddCharge(int amount)
    {
        currentCharge += amount;
        if (currentCharge > chargeRequired)
            currentCharge = chargeRequired;
    }


    public void Use(Player player)
    {
        Debug.Log($"cc:{currentCharge} |||| cr{chargeRequired}");

        if (currentCharge < chargeRequired) return;

        Activate(player);
        currentCharge = 0;
    }

    protected abstract void Activate(Player player);
    protected abstract void RemoveEffect();

    public abstract void OnAdd();
    public abstract void OnRemove();

    public abstract void UpdateItem(Player player);
}
