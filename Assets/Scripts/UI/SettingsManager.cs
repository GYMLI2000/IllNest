using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    private bool controlsDirty = false;
    public TMP_Text applyText;
    [SerializeField]
    private GameObject menu;
    [SerializeField]
    private GameObject confirmation;

    [Header("Panely")]
    public GameObject graphicsPanel;
    public GameObject audioPanel;
    public GameObject controlsPanel;


    [Header("Graphics")]
    public TextMeshProUGUI resolutionText;
    private List<Vector2Int> standardResolutions = new List<Vector2Int>
    {
        new Vector2Int(3840, 2160), 
        new Vector2Int(2560, 1440), 
        new Vector2Int(1920, 1080),
        new Vector2Int(1600, 900), 
        new Vector2Int(1366, 768),
        new Vector2Int(1280, 720)
    };
    private int selectedResIndex = 2;
    private int activeResIndex = 2;

    [Header("Controls Settings")]
    public InputActionAsset inputActions; 
    public GameObject controlRowPrefab;
    public Transform controlRowParent;
    private const string SaveKey = "InputOverrides";
    private InputActionRebindingExtensions.RebindingOperation _activeOperation;

    [Header("Audio Settings")]
    public AudioMixer mainMixer;
    public Slider musicSlider;
    public Slider sfxSlider;
    public Slider masterSlider;

    private float activeMusicVol;
    private float activeSFXVol;
    private float activeMasterVol;

    void Start()
    {

        LoadControls(); ;
        ResetAudio();
        ResetGraphics();

        for (int i = 0; i < standardResolutions.Count; i++)
        {
            if (standardResolutions[i].x == Screen.width && standardResolutions[i].y == Screen.height)
            {
                activeResIndex = i;
                break;
            }
        }

        selectedResIndex = activeResIndex;
        UpdateResolutionUI();
        UpdateControlsUI();
        OpenGraphics(); // Otevøe první grafiku
    }

    private void Update()
    {
        RefreshApplyButtonVisuals();
    }
    public void ControlsChanged()
    {
        controlsDirty = true;
    }
    public void Exit(bool exit)
    {
        if (!RefreshApplyButtonVisuals() || exit)
        {
            ResetAll();
            confirmation.SetActive(false);
            if (menu != null)
            {
                menu.SetActive(true);
            }
            this.gameObject.SetActive(false);
        }
        else
        {
            Debug.Log(confirmation);
            confirmation.SetActive(true);
        }
    }

    public void CloseConfirmation()
    {
        confirmation.SetActive(false);
    }

    #region Pøepínání Panelù
    public void OpenGraphics() => ShowPanel(graphicsPanel);
    public void OpenAudio() => ShowPanel(audioPanel);
    public void OpenControls() => ShowPanel(controlsPanel);

    private void ShowPanel(GameObject panelToShow)
    {
        graphicsPanel.SetActive(panelToShow == graphicsPanel);
        audioPanel.SetActive(panelToShow == audioPanel);
        controlsPanel.SetActive(panelToShow == controlsPanel);
    }
    #endregion

    public void ApplyAll()
    {
        if (graphicsPanel.activeSelf) ApplyGraphics();

        if (controlsPanel.activeSelf)
        {
            SaveControls();
            controlsDirty = false;
        }

        if (audioPanel.activeSelf) ApplyAudio();
    }

    public void ResetAll()
    {
        if (graphicsPanel.activeSelf) ResetGraphics();

        if (controlsPanel.activeSelf)
        {
            ResetControls();
            controlsDirty = false;
        }

        if (audioPanel.activeSelf) ResetAudio();
    }

    #region Grafika (Apply/Reset)
    public void NextResolution()
    {
        selectedResIndex = (selectedResIndex + 1) % standardResolutions.Count;
        UpdateResolutionUI();
    }

    public void PreviousResolution()
    {
        selectedResIndex--;
        if (selectedResIndex < 0) selectedResIndex = standardResolutions.Count - 1;
        UpdateResolutionUI();
    }

    public void ApplyGraphics()
    {
        activeResIndex = selectedResIndex;
        PlayerPrefs.SetInt("graphicsIndex", activeResIndex);
        PlayerPrefs.Save();
        Vector2Int res = standardResolutions[activeResIndex];
        Screen.SetResolution(res.x, res.y, Screen.fullScreen);
    }

    public void ResetGraphics()
    {
        activeResIndex = PlayerPrefs.GetInt("graphicsIndex");
        selectedResIndex = activeResIndex;
        UpdateResolutionUI();
    }

    private void UpdateResolutionUI()
    {
        resolutionText.text = $"{standardResolutions[selectedResIndex].x} x {standardResolutions[selectedResIndex].y}";
    }
    #endregion

    public void UpdateControlsUI()
    {
        if (inputActions == null) return;

        foreach (Transform child in controlRowParent)
        {
            Destroy(child.gameObject);
        }

        var playerActions = inputActions.FindActionMap("Player").actions;

        foreach (var action in playerActions)
        {
            switch (action.name)
            {
                case "Move":
                    for (int i = 1; i < action.bindings.Count; i++)
                    {
                        CreateRow(action, i, action.bindings[i].name);
                    }
                    break;
                case "Attack":
                    CreateRow(action, 0, "Attack");
                    break;
                case "ActiveItem":
                    CreateRow(action, 0, "Active Item");
                    break;
            }
        }
    }

    private void CreateRow(InputAction action, int bindingIndex, string label)
    {
        GameObject go = Instantiate(controlRowPrefab, controlRowParent);
        ControlRowUI rowScript = go.GetComponent<ControlRowUI>();
        rowScript.Setup(action, bindingIndex, label,this);
    }

    public void SaveControls()
    {
        string overrides = inputActions.SaveBindingOverridesAsJson();
        PlayerPrefs.SetString(SaveKey, overrides);
        PlayerPrefs.Save();

        Player player = FindFirstObjectByType<Player>();
        if (player != null)
        {
            player.LoadBindingOverrides();
        }
    }

    public void LoadControls()
    {
        if (PlayerPrefs.HasKey(SaveKey))
        {
            inputActions.LoadBindingOverridesFromJson(PlayerPrefs.GetString(SaveKey));
        }
    }

    public void ResetControls()
    {
        foreach (var map in inputActions.actionMaps)
        {
            map.RemoveAllBindingOverrides();
        }
        PlayerPrefs.DeleteKey(SaveKey);
        PlayerPrefs.Save();
        UpdateControlsUI();
        Player player = FindFirstObjectByType<Player>();
        if (player != null)
        {
            player.LoadBindingOverrides();
        }
    }

    public void RegisterRebindOperation(InputActionRebindingExtensions.RebindingOperation op)
    {
        _activeOperation = op;
    }

    public void StopAnyActiveRebinding()
    {
        if (_activeOperation != null)
        {
            _activeOperation.Cancel();
            _activeOperation = null;
        }
    }

    #region Audio
    public void SetMusicVolume(float value)
    {
        ApplyVolumeToMixer("MusicVol", value);
    }

    public void SetSFXVolume(float value)
    {
        ApplyVolumeToMixer("SFXVol", value);
    }

    public void SetMasterVolume(float value)
    {
        ApplyVolumeToMixer("Master", value);
    }

    private void ApplyVolumeToMixer(string parameter, float value)
    {
        float db = Mathf.Log10(Mathf.Max(0.0001f, value)) * 20;
        mainMixer.SetFloat(parameter, db);
    }

    public void ApplyAudio()
    {
        activeMusicVol = musicSlider.value;
        activeSFXVol = sfxSlider.value;
        activeMasterVol = masterSlider.value;

        PlayerPrefs.SetFloat("MusicVolume", activeMusicVol);
        PlayerPrefs.SetFloat("SFXVolume", activeSFXVol);
        PlayerPrefs.SetFloat("Master", activeMasterVol);
        PlayerPrefs.Save();

        ApplyVolumeToMixer("MusicVol", musicSlider.value);
        ApplyVolumeToMixer("SFXVol", sfxSlider.value);
        ApplyVolumeToMixer("Master", masterSlider.value);
    }

    public void ResetAudio()
    {
        float music = PlayerPrefs.GetFloat("MusicVolume", 0.75f);
        float sfx = PlayerPrefs.GetFloat("SFXVolume", 0.75f);
        float master = PlayerPrefs.GetFloat("Master", 0.75f);

        musicSlider.value = music;
        sfxSlider.value = sfx;
        masterSlider.value = master;

        activeMusicVol = music;
        activeSFXVol = sfx;
        activeMasterVol = master;

        ApplyVolumeToMixer("MusicVol", music);
        ApplyVolumeToMixer("SFXVol", sfx);
        ApplyVolumeToMixer("Master", master);
    }

    public void LoadAudio()
    {
        activeMusicVol = PlayerPrefs.GetFloat("MusicVolume", 0.75f);
        activeSFXVol = PlayerPrefs.GetFloat("SFXVolume", 0.75f);
        activeMasterVol = PlayerPrefs.GetFloat("Master", 0.75f);

        musicSlider.value = activeMusicVol;
        sfxSlider.value = activeSFXVol;
        masterSlider.value = activeMasterVol;

        ApplyVolumeToMixer("MusicVol", activeMusicVol);
        ApplyVolumeToMixer("SFXVol", activeSFXVol);
        ApplyVolumeToMixer("Master", activeMasterVol);
    }
    #endregion

    public bool RefreshApplyButtonVisuals()
    {
        if (applyText == null) return false;

        bool hasChanges = false;

        if (selectedResIndex != activeResIndex) hasChanges = true;

        if (!Mathf.Approximately(musicSlider.value, activeMusicVol)) hasChanges = true;
        if (!Mathf.Approximately(sfxSlider.value, activeSFXVol)) hasChanges = true;
        if (!Mathf.Approximately(masterSlider.value, activeMasterVol)) hasChanges = true;

        if (controlsDirty) hasChanges = true;

        applyText.color = hasChanges ? Color.green : Color.gray;

        return hasChanges;
    }
}
