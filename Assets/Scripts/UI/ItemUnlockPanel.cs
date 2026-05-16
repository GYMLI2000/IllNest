using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ItemUnlockPanel : MonoBehaviour
{
    [SerializeField]
    private TMP_Text itemText;

    [SerializeField]
    private Animator animator;
    private Queue<string> itemsUnlocked;

    private void Start()
    {
        itemsUnlocked = new Queue<string>();
        CompletionManager.Instance.OnUnlock += ItemUnlock;
        CompletionManager.Instance.OnDiscover += EntryDiscovered;
    }

    private void ItemUnlock(string itemName)
    {
        itemsUnlocked.Enqueue(itemName + " Unlocked");
    }

    private void EntryDiscovered()
    {
        itemsUnlocked.Enqueue("New Entry Unlocked");
    }

    private void Update()
    {
        if (itemsUnlocked.Count > 0 && animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            itemText.text = itemsUnlocked.Dequeue();
            animator.SetTrigger("Show");
        }
    }

    private void OnDisable()
    {
        if (CompletionManager.Instance != null)
        {
            CompletionManager.Instance.OnUnlock -= ItemUnlock;
            CompletionManager.Instance.OnDiscover -= EntryDiscovered;
        }
    }
}
