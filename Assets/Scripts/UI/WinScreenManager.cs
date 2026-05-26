using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WinScreenManager : MonoBehaviour
{
    public static WinScreenManager Instance;
    [SerializeField] private GameObject winScreenUI;
    [SerializeField] private TMP_Text timeText;
    [SerializeField] private TMP_Text killText;
    [SerializeField] private TMP_Text cureText;
    [SerializeField] private TMP_Text discoverText;
    [SerializeField] private TMP_Text unlockText;
    [SerializeField] private TMP_Text itemText;

    private float timeTaken;
    private Dictionary<string, int> runProgress = new Dictionary<string, int>();

    private void Awake()
    {
        Instance = this;
        winScreenUI.SetActive(false);
        HostileEntity.EntityDeath += KillEnemy;
        DebuffManager.cureDebuff += CureDisease;
        CompletionManager.Instance.OnDiscover += DiscoverEntry;
        CompletionManager.Instance.OnUnlock += UnlockItem;

    }


    private void DiscoverEntry() => UpdateStats("discover");
    private void KillEnemy(HostileEntity enemy) => UpdateStats("kill");
    private void CureDisease() => UpdateStats("cure");
    private void UnlockItem(string itemName) => UpdateStats("unlock");
    public void CollectItem() => UpdateStats("item");

    private void UpdateStats(string statname)
    {
        if (!runProgress.ContainsKey(statname))
        {
            runProgress[statname] = 0;
        }

        runProgress[statname]++;
    }

    private void Update()
    {
        timeTaken += Time.deltaTime;
    }


    public void Show()
    {
        Time.timeScale = 0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        winScreenUI.SetActive(true);
        timeText.text = $"Time: {timeTaken:0.00}s";
        killText.text = $"Enemies Killed: {runProgress.GetValueOrDefault("kill", 0)}";
        cureText.text = $"Diseases Cured: {runProgress.GetValueOrDefault("cure", 0)}";
        itemText.text = $"Items Collected: {runProgress.GetValueOrDefault("item", 0)}";
        discoverText.text = $"Entries Discovered: {runProgress.GetValueOrDefault("discover", 0)}";
        unlockText.text = $"Items Unlocked: {runProgress.GetValueOrDefault("unlock", 0)}";
    }

    private void OnDisable()
    {
        HostileEntity.EntityDeath -= KillEnemy;
        DebuffManager.cureDebuff -= CureDisease;
        CompletionManager.Instance.OnDiscover -= DiscoverEntry;
        CompletionManager.Instance.OnUnlock -= UnlockItem;
    }

    public void Menu()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
    }

}
