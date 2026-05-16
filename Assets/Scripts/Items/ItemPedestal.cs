using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class ItemPedestal : MonoBehaviour
{
    private Item item;

    [SerializeField]
    private SpriteRenderer itemSprite;

    [SerializeField]
    private ParticleSystem particle;

    private bool isPicked = false;

    private void Start()
    {
        List<Item> allItems = RoomManager.RM.items;
        List<Item> unlockedItems = new List<Item>();

        foreach (Item currentItem in allItems)
        {
            if (CompletionManager.Instance.IsUnlocked(currentItem.itemID.ToString()) || currentItem.isDefault)
            {
                unlockedItems.Add(currentItem);
            }
        }
        if (unlockedItems.Count == 0)
        {
            Debug.LogWarning("No unlocked items left in the pool!");
            return;
        }

        item = unlockedItems[Random.Range(0, unlockedItems.Count)];

        if (allItems.Count > 1)
        {
            RoomManager.RM.items.Remove(item);
        }
        itemSprite.sprite = item.itemSprite;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && collision.gameObject.GetComponentInParent<ItemManager>() != null && !isPicked)
        {
            AudioManager.Instance.PlaySFX("Collect");

            isPicked = true;
            itemSprite.sprite = null;
            ItemManager manager = collision.gameObject.GetComponentInParent<ItemManager>();
            if (item.isPassive) {
                manager.AddItem((PassiveItem)item);
            }
            else
            {
                ActiveItem newItem = manager.AddActiveItem((ActiveItem)item);
                if (newItem != null)
                {
                    item = newItem;
                    isPicked = false;
                    itemSprite.sprite = item.itemSprite;
                }
            }

            particle.Play();

        }
    }
}
