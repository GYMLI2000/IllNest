using UnityEngine;

[System.Serializable]
public abstract class Item : ScriptableObject
{
    public Sprite itemSprite;
    public string itemName;
    public abstract bool isPassive { get; }

}
