using UnityEngine;

public enum JournalCategory
{
    Enemy,
    Disease,
    Item
}

[CreateAssetMenu(fileName = "NewJournalEntry", menuName = "Journal/Entry")]
public class JournalEntry : ScriptableObject
{

    [Header("Identification")]
    public int entryID;   
    public string taskName;
    public JournalCategory category;

    [Header("Content")]
    public string entryName;
    [TextArea(3, 10)]
    public string descText;

    [TextArea(5, 10)]
    public string statsText; 

    [TextArea(2, 5)]
    public string additionalText;

    [TextArea(2, 5)]
    public string taskText;

    public Sprite entryImage;   
}