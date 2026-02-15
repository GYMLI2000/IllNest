using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    [SerializeField]
    private Player player;
    [SerializeField]
    private ItemManager itemManager;


    [SerializeField]
    private GameObject healthBar;
    [SerializeField]
    private Image heartPrefab;
    [SerializeField]
    private Sprite healthEmpty;
    [SerializeField]
    private Sprite healthHalf;
    [SerializeField]
    private Sprite healthFull;
    private List<Image> hearts = new();

    [SerializeField]
    private Image ActiveItemImg;
    [SerializeField]
    private Image ActiveItemChargeFill;




    private void Awake()
    {
        player.changeHp += UpdateHealth;
        player.changeMaxHp += UpdateMaxHealth;
        itemManager.PickupActive += UpdateActiveItemImage;
        itemManager.ChangeActiveCharge += UpdateActiveItemCharge;

    }

    private void UpdateActiveItemImage(Sprite img)
    {
        if (!ActiveItemImg.isActiveAndEnabled)
        {
            ActiveItemImg.enabled = true;
        }
        ActiveItemImg.sprite = img;
    }

    private void UpdateActiveItemCharge(int currentCharge, int maxCharge)
    {
        ActiveItemChargeFill.fillAmount = (float)currentCharge/(float)maxCharge;
    }

    private void UpdateHealth(int currentHp)
    {
        foreach (var heart in hearts)
        {
            if (currentHp >= 2)
            {
                heart.sprite = healthFull;
                currentHp -= 2;
            }
            else if (currentHp == 1)
            {
                heart.sprite = healthHalf;
                currentHp = 0;
            }
            else
            {
                heart.sprite = healthEmpty;
            }
        }
    }

    private void UpdateMaxHealth(int maxHp)
    {
        foreach (Transform child in healthBar.transform)
        {
            Destroy(child.gameObject);
        }

        hearts.Clear();

        int heartCount = Mathf.CeilToInt(maxHp / 2f);

        for (int i = 0; i < heartCount; i++)
        {
            Image heart = Instantiate(heartPrefab, healthBar.transform);
            heart.sprite = healthEmpty;
            hearts.Add(heart);
        }
    }
}
