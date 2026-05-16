using UnityEngine;
using System.Collections.Generic;
using System;

public class CompletionManager : MonoBehaviour
{
    public static CompletionManager Instance { get; private set; }
    private Dictionary<string, int> currentRunProgress = new Dictionary<string, int>();

    public Action<string> OnUnlock;
    public Action OnDiscover;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ResetRunProgress()
    {
        currentRunProgress.Clear();
    }

    public void CheckCompletion(string taskKey, int addAmount, int targetAmount)
    {

        if (PlayerPrefs.GetInt(taskKey + "_unlocked", 0) == 1)
        {
            return;
        }

        if (!currentRunProgress.ContainsKey(taskKey))
        {
            currentRunProgress[taskKey] = 0;
        }

        currentRunProgress[taskKey] += addAmount;
        int currentAmount = currentRunProgress[taskKey];

        Debug.Log($"Task {taskKey} run progress: {currentAmount}/{targetAmount}");

        if (currentAmount >= targetAmount)
        {
            UnlockReward(taskKey);
        }
    }

    public void ResetCompletionProgress(string taskKey)
    {
        if (PlayerPrefs.GetInt(taskKey + "_unlocked", 0) == 1)
        {
            return;
        }

        currentRunProgress[taskKey] = 0;

    }

    private void UnlockReward(string taskKey)
    {
        if (PlayerPrefs.GetInt(taskKey + "_unlocked", 0) == 1)
        {
            return;
        }

        PlayerPrefs.SetInt(taskKey + "_unlocked", 1);
        PlayerPrefs.Save();

        JournalEntry entry = JournalManager.journalEntries.Find(entry => entry.entryID.ToString() == taskKey);
        OnUnlock?.Invoke(entry != null ? entry.entryName : taskKey);

        Debug.Log("Item Permanently Unlocked for completing: " + taskKey);

    }

    public void DiscoverEntry(int id)
    {
        if (PlayerPrefs.GetInt(id + "_discovered", 0) == 1)
        {
            return;
        }
        PlayerPrefs.SetInt(id + "_discovered", 1);
        PlayerPrefs.Save();
        Debug.Log("Journal Entry Discovered: " + id);
        OnDiscover?.Invoke();
    }

    public bool IsUnlocked(string taskKey)
    {
        return PlayerPrefs.GetInt(taskKey + "_unlocked", 0) == 1;
    }
}