using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    [SerializeField]
    private Player player;


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

    private void Awake()
    {
        player.changeHp += UpdateHealth;
        player.changeMaxHp += UpdateMaxHealth;

    }

    private void UpdateHealth(int currentHp)
    {
        foreach (var heart in healthBar.GetComponentsInChildren<Image>())
        {
            if (currentHp > 1)
            {
                heart.sprite = healthFull;
                currentHp -= 2;
            }
            else if (currentHp == 1)
            {
                heart.sprite = healthHalf;
                currentHp--;
            }
            else
            {
                heart.sprite = healthEmpty;
            }
        }
    }

    private void UpdateMaxHealth(int maxHp)
    {
        for (int i = 0; i < maxHp/2; i++)
        {
            Image heart = Instantiate(heartPrefab, healthBar.transform.position+Vector3.right*80*i, Quaternion.identity,healthBar.transform);
            heart.sprite = healthEmpty;
        }
    }
}
