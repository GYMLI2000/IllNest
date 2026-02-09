using UnityEngine;

public class ItemPedestal : MonoBehaviour
{
    private Item item;

    [SerializeField]
    private SpriteRenderer itemSprite;

    private void Start()
    {
        item = RoomManager.RM.items[Random.Range(0,RoomManager.RM.items.Count)];
        if (RoomManager.RM.items.Count > 1)
        {
            RoomManager.RM.items.Remove(item);
        }
        itemSprite.sprite = item.itemSprite;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && collision.gameObject.GetComponentInParent<ItemManager>() != null)
        {
            ItemManager manager = collision.gameObject.GetComponentInParent<ItemManager>();
            if (item.isPassive) {
                manager.AddItem((PassiveItem)item);
            }
            else
            {
                manager.AddActiveItem((ActiveItem)item);
            }

            itemSprite.sprite = null;
        }
    }
}
