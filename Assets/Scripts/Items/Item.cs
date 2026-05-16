using UnityEngine;

[System.Serializable]
public abstract class Item : ScriptableObject
{
    public Sprite itemSprite;
    public string itemName;
    public abstract bool isPassive { get; }
    public int itemID { protected set; get; }
    public bool isDefault { protected set; get; } = false;

}
