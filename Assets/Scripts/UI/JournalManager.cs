using System.Collections.Generic;
using TMPro; // Assuming you use TextMeshPro for UI
using UnityEngine;
using UnityEngine.UI;

public class JournalManager : MonoBehaviour
{
    [Header("Database")]
    [SerializeField]
    private List<JournalEntry> allEntries = new List<JournalEntry>();


    public static List<JournalEntry> journalEntries = new List<JournalEntry>();

    [Header("UI References - List")]
    [SerializeField]
    public Color selectedColor;
    public Image enemiesTabButton;
    public Image itemsTabButton;
    public Image diseasesTabButton;

    public Transform scrollListContentParent;
    public GameObject entryButtonPrefab; // A simple UI button with a Text component

    [Header("UI References - Details Panel")]
    public GameObject detailsPanel;
    public TextMeshProUGUI titleText;
    public Image entryImage;
    public TextMeshProUGUI descText;
    public TextMeshProUGUI statsText;
    public TextMeshProUGUI additionalText;

    private void Start()
    {
        // Hide details panel on start
        detailsPanel.SetActive(false);
    }

    private void Awake()
    {
        journalEntries = allEntries;
    }

    // --- BUTTON EVENT METHDOS ---
    // Link your 3 UI Buttons to these methods in the Inspector
    public void OnClickEnemiesTab() => PopulateList(JournalCategory.Enemy);
    public void OnClickDiseasesTab() => PopulateList(JournalCategory.Disease);
    public void OnClickItemsTab() => PopulateList(JournalCategory.Item);

    private void PopulateList(JournalCategory targetCategory)
    {
        // 1. Clear the current list
        foreach (Transform child in scrollListContentParent)
        {
            Destroy(child.gameObject);
        }

        detailsPanel.SetActive(false);

        // 2. Instantiate buttons for the selected category
        foreach (JournalEntry entry in allEntries)
        {
            if (entry.category == targetCategory)
            {
                GameObject newButton = Instantiate(entryButtonPrefab, scrollListContentParent);

                // Get the text component on your prefab to set the name
                TextMeshProUGUI buttonText = newButton.GetComponentInChildren<TextMeshProUGUI>();
                Button btn = newButton.GetComponent<Button>();

                bool isUnlocked = CheckUnlocked(entry);

                // Check if it's discovered. If not, hide the name.
                bool isDiscovered = PlayerPrefs.GetInt(entry.entryID + "_discovered", 0) == 1;
                buttonText.text = isDiscovered ? entry.entryName : "???";

                if (!isUnlocked) buttonText.text = "Locked";

                // Add click event to the button
                btn.onClick.AddListener(() => DisplayEntryDetails(entry));
            }
        }

        switch (targetCategory)
        {
            case JournalCategory.Enemy:
                enemiesTabButton.color = selectedColor;
                itemsTabButton.color = Color.white;
                diseasesTabButton.color = Color.white;
                break;
            case JournalCategory.Item:
                enemiesTabButton.color = Color.white;
                itemsTabButton.color = selectedColor;
                diseasesTabButton.color = Color.white;
                break;
            case JournalCategory.Disease:
                enemiesTabButton.color = Color.white;
                itemsTabButton.color = Color.white;
                diseasesTabButton.color = selectedColor;
                break;
        }
    }

    private bool CheckUnlocked(JournalEntry entry)
    {
        bool isUnlocked = CompletionManager.Instance.IsUnlocked(entry.entryID.ToString());
        if (isUnlocked || entry.category != JournalCategory.Item || entry.entryID < 3500 || (entry.entryID > 4000 && entry.entryID < 4500))
        {
            return true;
        }
        return false;
    }

    private void DisplayEntryDetails(JournalEntry entry)
    {
        detailsPanel.SetActive(true);

        // Fetch States
        bool isDiscovered = PlayerPrefs.GetInt(entry.entryID + "_discovered", 0) == 1;

        // We use the CompletionManager from your previous code to check Unlocked status!
        bool isUnlocked = CompletionManager.Instance.IsUnlocked(entry.taskName);


        if (!CheckUnlocked(entry))
        {
            titleText.text = "Locked Item";
            entryImage.sprite = null;
            descText.text = entry.descText; // Maybe they know what it is...
            statsText.text = "Stats hidden. Complete the task to unlock!";
            additionalText.text = "Task: " + entry.taskName;
            return;
        }
        else if (!isDiscovered)
        {
            // Undiscovered State: Show almost nothing
            titleText.text = "Unknown Entity";
            entryImage.sprite = null; // Or a silhouette/question mark sprite
            descText.text = "You have not encountered this yet.";
            statsText.text = "???";
            additionalText.text = "";
            return;
        }
        else
        {
            // Discovered AND Unlocked Item (Full Info)
            titleText.text = entry.entryName;
            entryImage.sprite = entry.entryImage;
            descText.text = entry.descText;
            statsText.text = entry.statsText;
            additionalText.text = entry.additionalText;
            return;
        }

    }

}