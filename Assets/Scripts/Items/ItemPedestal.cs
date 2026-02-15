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
        item = RoomManager.RM.items[Random.Range(0,RoomManager.RM.items.Count)];
        if (RoomManager.RM.items.Count > 1)
        {
            RoomManager.RM.items.Remove(item);
        }
        itemSprite.sprite = item.itemSprite;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && collision.gameObject.GetComponentInParent<ItemManager>() != null && !isPicked)
        {
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
