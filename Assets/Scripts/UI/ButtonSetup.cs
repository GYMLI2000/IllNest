using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonSetup : MonoBehaviour
{

    void OnEnable()
    {
        Button[] buttons = FindObjectsByType<Button>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None
        );

        foreach (Button btn in buttons)
        {
            btn.onClick.RemoveListener(PlayClick);
            btn.onClick.AddListener(PlayClick);
            EventTrigger trigger = btn.gameObject.GetComponent<EventTrigger>();
            if (trigger == null) trigger = btn.gameObject.AddComponent<EventTrigger>();

            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerEnter;
            entry.callback.AddListener((data) => { AudioManager.Instance.PlaySFX("Hover"); });
            trigger.triggers.Add(entry);
        }
    }

    void PlayClick()
    {
        AudioManager.Instance.PlaySFX("Click");
    }

}
