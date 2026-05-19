using System.Collections; 
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ItemUnlockPanel : MonoBehaviour
{
    [SerializeField]
    private TMP_Text itemText;

    [SerializeField]
    private Animator animator;

    [SerializeField]
    private float animationDuration = 2.0f;

    private Queue<string> itemsUnlocked;
    private bool isDisplaying = false; // Locks the queue while animating

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
        if (itemsUnlocked.Count > 0 && !isDisplaying)
        {
            StartCoroutine(DisplayNextItem());
        }
    }

    private IEnumerator DisplayNextItem()
    {
        isDisplaying = true;
        itemText.text = itemsUnlocked.Dequeue();
        animator.SetTrigger("Show");

        yield return new WaitForSeconds(animationDuration);

        isDisplaying = false;
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