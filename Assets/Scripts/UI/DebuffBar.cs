using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class DebuffBar : MonoBehaviour
{
    [SerializeField]
    private Image[] debuffIcons;
    [SerializeField]
    private Sprite[] debuffSprites;
    [SerializeField]
    private DebuffManager debuffManager;

    private void Awake()
    {
        debuffManager.changeDebuff += UpdateDebuffIcons;
        UpdateDebuffIcons(new List<Debuff>());
    }

    private void UpdateDebuffIcons(List<Debuff> activeDebuffs)
    {
        for (int i = 0; i < debuffIcons.Length; i++)
        {
            if (i < activeDebuffs.Count)
            {
                debuffIcons[i].sprite = debuffSprites[activeDebuffs[i].debuffID -1];
                debuffIcons[i].enabled = true;
            }
            else
            {
                debuffIcons[i].enabled = false;
            }
        }
    }

}
