using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BossBarPanel : MonoBehaviour
{
    [SerializeField] 
    private Slider bossBar;
    [SerializeField] 
    private Image fill;
    [SerializeField] 
    private Image handle;
    private Color barColor;

    private void Awake()
    {
        Boss.ActivateBossBar += ActivateBossBar;
        Boss.ChangeBossBar += ChangeBar;
        Boss.DisableBossBar += DisableBossBar;
    }

    private void ActivateBossBar(Color fillColor, Sprite bossIcon,int maxHP, int currentHP)
    {
        if (bossBar.gameObject == null) return;
        bossBar.gameObject.SetActive(true);
        barColor = fillColor;
        fill.color = barColor;
        handle.sprite = bossIcon;
        bossBar.value = (float)(currentHP)/(float)maxHP;
    }

    private void ChangeBar(int maxHP,int currentHP, bool isNegative)
    {
        if (bossBar.gameObject == null) return;
        bossBar.value = (float)currentHP/(float)maxHP;

        StartCoroutine(HpChangeCoroutine(isNegative ? Color.red : Color.green));
    }

    private void DisableBossBar()
    {
        bossBar.gameObject.SetActive(false);

    }

    private void OnDestroy()
    {
        Boss.ActivateBossBar -= ActivateBossBar;
        Boss.ChangeBossBar -= ChangeBar;
        Boss.DisableBossBar -= DisableBossBar;
    }

    private IEnumerator HpChangeCoroutine(Color color)
    {

        fill.color = color;

        yield return new WaitForSeconds(0.1f);

        fill.color = barColor;
    }
}
