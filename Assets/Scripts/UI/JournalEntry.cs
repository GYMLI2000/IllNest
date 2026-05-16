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

    public string statsText; 

    [TextArea(2, 5)]
    public string additionalText; 

    public Sprite entryImage;   
}