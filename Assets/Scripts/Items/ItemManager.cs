using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEditor.Progress;

public class ItemManager : MonoBehaviour
{
    private List<PassiveItem> passiveItems = new List<PassiveItem>();
    private ActiveItem activeItem;

    [SerializeField]
    private Player player;


    public void AddActiveItem(ActiveItem item)
    {
        activeItem = item;
    }

    public void UseActiveItem()
    {
        // pøi stistku nejakyho tlacitka = efekt
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
    }

}
