using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEditor.Progress;

public class ItemManager : MonoBehaviour
{
    public List<PassiveItem> passiveItems { get; private set; }
    = new List<PassiveItem>();
    private ActiveItem activeItem = null;
    public event Action<string> OnPickup;
    public event Action<Sprite> PickupActive;
    public event Action<int,int> ChangeActiveCharge;


    [SerializeField]
    private Player player;

    private void Start()
    {
        Enemy.EnemyDeath += AddCharge;
    }

    private void AddCharge(Enemy e)
    {
        if (activeItem !=null)
        {
            activeItem.AddCharge(1);
            ChangeActiveCharge?.Invoke(activeItem.currentCharge, activeItem.chargeRequired);
        }
    }

    public ActiveItem AddActiveItem(ActiveItem item)
    {

        ActiveItem oldItem = activeItem;
        if (oldItem != null) oldItem.OnRemove();

        activeItem = item;
        activeItem.OnAdd();
        PickupActive?.Invoke(activeItem.itemSprite);
        ChangeActiveCharge?.Invoke(activeItem.currentCharge, activeItem.chargeRequired);

        return oldItem;
    }

    public void UseActiveItem()
    {
        if (activeItem == null) return;
        activeItem.Use(player);
        ChangeActiveCharge?.Invoke(activeItem.currentCharge,activeItem.chargeRequired);

    }

    public void AddItem(PassiveItem item)
    {
        if (passiveItems.FirstOrDefault(d => d.GetType() == item.GetType()) != null)
        {
            //hrac uz ma item
        }
        else
        {
            passiveItems.Add(item);
            item.OnAdd(player);
            OnPickup?.Invoke(item.itemName);
        }
    }

    public void RemoveItem(PassiveItem item)
    {
        item.OnRemove(player);
        passiveItems.Remove(item);
    }

    private void Update()
    {
        for (int i = passiveItems.Count - 1; i >= 0; i--)
        {
            var item = passiveItems[i];

            item.Effect(player);
        }

        if (activeItem != null)
        {
            activeItem.UpdateItem(player);
        }
    }

}
