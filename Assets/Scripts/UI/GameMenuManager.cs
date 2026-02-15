using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenuManager : MonoBehaviour
{
    [SerializeField]
    private Player player;

    [SerializeField]
    private GameObject menu;

    [SerializeField]
    private GameObject inventory;
    [SerializeField]
    private GameObject items;
    [SerializeField]
    private GameObject inventoryItemPrefab;

    private void Start()
    {
        player.openMenu += OpenMenu;
    }

    public void MainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }

    public void Resume()
    {
        Time.timeScale = 1.0f;
        menu.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
        player.enabled = true;

    }

    public void OpenMenu()
    {
        if (!menu.activeSelf)
        {
            Time.timeScale = 0f;
            player.enabled = false;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            menu.SetActive(true);
        }
        else
        {
            Resume();
        }
    }

    public void OpenInventory()
    {
        if (!inventory.activeSelf)
        {
            menu.SetActive(false);
            inventory.SetActive(true);

            foreach (Transform item in items.transform)
            {
                Destroy(item.gameObject);
            }

            foreach (PassiveItem item in player.itemManager.passiveItems)
            {
                InventoryItem invItem = Instantiate(inventoryItemPrefab, items.transform).GetComponent<InventoryItem>();
                invItem.text.text = item.name;
                invItem.image.sprite = item.itemSprite;
            }
        }
        else
        {
            menu.SetActive(true);
            inventory.SetActive(false);
        }
    }
}
