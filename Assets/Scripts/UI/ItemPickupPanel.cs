using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ItemPickupPanel : MonoBehaviour
{
    [SerializeField]
    private ItemManager itemManager;

    [SerializeField]
    private TMP_Text itemText;

    [SerializeField]
    private Animator animator;
    private Queue<string> itemsPicked;

    private void Start()
    {
        itemsPicked = new Queue<string>();
        itemManager.OnPickup += ItemPickup;
    }

    private void ItemPickup(string itemName)
    {
        itemsPicked.Enqueue(itemName);
    }

    private void Update()
    {
        if (itemsPicked.Count > 0 && animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            itemText.text = itemsPicked.Dequeue();
            animator.SetTrigger("ItemPicked");
        }
    }
}
