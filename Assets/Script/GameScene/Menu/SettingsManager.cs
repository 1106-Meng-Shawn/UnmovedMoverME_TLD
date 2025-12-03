using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;
using static SettingValue;

/// <summary>
/// Defines all available setting panel types
/// </summary>
public enum SettingPanelType
{
    DisplayPanel,
    SoundPanel,
    TextPanel,
    ControlPanel,
    ConfirmPanel
}

/// <summary>
/// Manages all settings panels, animations, and configuration persistence.
/// Uses a dictionary-based approach for easy panel management.
/// </summary>
public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance { get; private set; }

    [Header("Main Setting Panel")]
    public GameObject settingPanel;

    [Header("All Setting Panels")]
    public SettingPanelEntry DisplayPanel;
    public SettingPanelEntry SoundPanel;
    public SettingPanelEntry TextPanel;
    public SettingPanelEntry ControlPanel;
    public SettingPanelEntry ConfirmPanel;

    [Header("Background Animation")]
    public Image backgroundImage;
    public Image forFunImage;
    public string sheetFolder = "MyDraw/ForFun/Sheet";
    public string backgroundFolder = "MyDraw/ForFun/Background";
    public float frameRate = 0.75f;

    [Header("Control Buttons")]
    public Button closeButton;
    public Button defaultButton;

    // Panel management
    private Dictionary<SettingPanelType, SettingPanelEntry> panelDictionary;
    private SettingPanelType currentPanel = SettingPanelType.DisplayPanel;

    // Animation data
    private Dictionary<string, Sprite[]> allSheets = new Dictionary<string, Sprite[]>();
    private Dictionary<string, Sprite> allBackgrounds = new Dictionary<string, Sprite>();
    private List<string> sheetKeys = new List<string>();
    private Sprite[] currentFrames;
    private Coroutine animCoroutine;

    // State management
    private bool isInputPanelActive = false;

    /// <summary>
    /// Struct to encapsulate a setting panel with its button and controller
    /// </summary>
    [Serializable]
    public struct SettingPanelEntry
    {
        public Button topButton;
        public SettingPanelBase panel;
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        InitSettingSprite();
        InitializePanelDictionary();
        InitializeButtonListeners();
        InitializePanels();
    }

    /// <summary>
    /// Initialize the panel dictionary for easy lookup
    /// </summary>
    private void InitializePanelDictionary()
    {
        panelDictionary = new Dictionary<SettingPanelType, SettingPanelEntry>
        {
            { SettingPanelType.DisplayPanel, DisplayPanel },
            { SettingPanelType.SoundPanel, SoundPanel },
            { SettingPanelType.TextPanel, TextPanel },
            { SettingPanelType.ControlPanel, ControlPanel },
            { SettingPanelType.ConfirmPanel, ConfirmPanel }
        };
    }

    /// <summary>
    /// Initialize all button listeners
    /// </summary>
    private void InitializeButtonListeners()
    {
        // Setup top panel buttons using dictionary keys
        foreach (var kvp in panelDictionary)
        {
            if (kvp.Value.topButton != null)
            {
                SettingPanelType panelType = kvp.Key;
                kvp.Value.topButton.onClick.AddListener(() => OnTopButtonClicked(panelType));
            }
        }

        // Setup control buttons
        closeButton.onClick.AddListener(ClosePanel);
        defaultButton.onClick.AddListener(OnDefaultButtonClick);
    }

    /// <summary>
    /// Initialize all panels
    /// </summary>
    private void InitializePanels()
    {
        foreach (var entry in panelDictionary.Values)
        {
            if (entry.panel != null)
            {
                entry.panel.Init();
                entry.panel.ClosePanel();
            }
        }
        panelDictionary[currentPanel].panel.OpenPanel();
    }


    void InitSettingSprite()
    {
        LoadAllSheets();
        LoadAllBackground();
    }


    /// <summary>
    /// Load all sprite sheets from resources
    /// </summary>
    private void LoadAllSheets()
    {
        Sprite[] sprites = Resources.LoadAll<Sprite>(sheetFolder);
        foreach (var sprite in sprites)
        {
            string fileName = sprite.texture.name;
            if (!allSheets.ContainsKey(fileName))
            {
                allSheets[fileName] = new Sprite[] { sprite };
                sheetKeys.Add(fileName);
            }
            else
            {
                var list = new List<Sprite>(allSheets[fileName]);
                list.Add(sprite);
                allSheets[fileName] = list.ToArray();
            }
        }
    }

    /// <summary>
    /// Load all background sprites from resources
    /// </summary>
    private void LoadAllBackground()
    {
        Sprite[] sprites = Resources.LoadAll<Sprite>(backgroundFolder);

        foreach (var sprite in sprites)
        {
            string fileName = sprite.texture.name;

            if (!allBackgrounds.ContainsKey(fileName))
            {
                allBackgrounds[fileName] = sprite;
            }
        }
    }

    /// <summary>
    /// Handle top button clicks to switch panels
    /// </summary>
    void OnTopButtonClicked(SettingPanelType type)
    {
        ShowPanel(type);
    }

    /// <summary>
    /// Show a specific settings panel
    /// </summary>
    void ShowPanel(SettingPanelType type)
    {
        settingPanel.gameObject.SetActive(true);
        panelDictionary[currentPanel].panel.ClosePanel();
        currentPanel = type;
        panelDictionary[currentPanel].panel.OpenPanel();
        SetRandomSprite();
    }

    /// <summary>
    /// Handle default button click - resets current panel to defaults
    /// </summary>
    void OnDefaultButtonClick()
    {
        panelDictionary[currentPanel].panel.OnDefaultButtonClick();

    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1) && !isInputPanelActive)
        {
            ClosePanel();
        }
    }

    // ==================== Sprite Animation Methods ====================

    /// <summary>
    /// Set sprite by name
    /// </summary>
    public void SetSprite(string name)
    {
        if (!allSheets.ContainsKey(name))
        {
            Debug.LogWarning($"Can't find {name} this SpriteSheet!");
            return;
        }
        ApplySprite(name);
    }

    /// <summary>
    /// Set sprite by index
    /// </summary>
    public void SetSprite(int index)
    {
        if (index < 0 || index >= sheetKeys.Count)
        {
            Debug.LogWarning($"Index {index} out of range!");
            return;
        }
        ApplySprite(sheetKeys[index]);
    }

    /// <summary>
    /// Set a random sprite for background
    /// </summary>
    public void SetRandomSprite()
    {
        if (sheetKeys.Count == 0)
        {
            Debug.Log("Sheetkeys.Count == 0");
            return;
        }
        int randomIndex = UnityEngine.Random.Range(0, sheetKeys.Count);
        ApplySprite(sheetKeys[randomIndex]);
    }

    /// <summary>
    /// Apply sprite and start animation
    /// </summary>
    private void ApplySprite(string key)
    {
        if (!allSheets.ContainsKey(key))
        {
            Debug.LogWarning($"[ApplySprite] allSheets can't found key: {key}");
            return;
        }

        currentFrames = allSheets[key];

        if (allBackgrounds.ContainsKey(key))
        {
            backgroundImage.sprite = allBackgrounds[key];
        }
        else
        {
            backgroundImage.sprite = null;
            Debug.LogWarning($"[ApplySprite] allBackgrounds can't found {key}, background set null");
        }

        if (animCoroutine != null)
            StopCoroutine(animCoroutine);

        animCoroutine = StartCoroutine(PlayAnimation());
    }

    /// <summary>
    /// Play sprite sheet animation
    /// </summary>
    private IEnumerator PlayAnimation()
    {
        int index = 0;

        while (true)
        {
            if (currentFrames.Length > 0)
            {
                forFunImage.sprite = currentFrames[index];
                index = (index + 1) % currentFrames.Length;
            }
            yield return new WaitForSeconds(frameRate);
        }
    }

    /// <summary>
    /// Stop the background animation
    /// </summary>
    void StopAnimation()
    {
        if (animCoroutine != null)
            StopCoroutine(animCoroutine);
    }

    // ==================== Panel State Methods ====================

    /// <summary>
    /// Notify when input panel state changes
    /// </summary>
    public void NotifyInputPanelState(bool isActive)
    {
        isInputPanelActive = isActive;
    }

    /// <summary>
    /// Show settings panel
    /// </summary>
    public void OpenPanel()
    {
        settingPanel.gameObject.SetActive(true);
        ShowPanel(currentPanel);
    }

    /// <summary>
    /// Get settings file path
    /// </summary>
    public string GetSettingFilePath()
    {
        string settingFilePath = Path.Combine(Application.persistentDataPath, Constants.SAVE_FILE_PATH, SettingConstant.SettingFilePath);
        return settingFilePath;
    }

    /// <summary>
    /// Close settings panel and save changes
    /// </summary>
    public void ClosePanel()
    {
        SaveSettingValue();
        settingPanel.gameObject.SetActive(false);
        StopAnimation();
    }

    // ==================== Setting Value Getters/Setters ====================

    public string GetPlayerName()
    {
        return SettingValue.Instance.GetPlayerName();
    }

    public void SetPlayerName(string newName)
    {
        GameValue.Instance.SetPlayerName(newName);
        SettingValue.Instance.SetPlayerName(newName);
        SaveSettingValue();
    }

    public DisplaySettingValue GetDisplaySettingValue()
    {
        return SettingValue.Instance.GetDisplaySettingValue();
    }

    public SoundSettingValue GetSoundSettingValue()
    {
        return SettingValue.Instance.GetSoundSettingValue();
    }

    public TextSettingValue GetTextSettingValue()
    {
        return SettingValue.Instance.GetTextSettingValue();
    }

    public bool HasSaveData()
    {
        return SettingValue.Instance.HasSaveData();
    }

    public void SaveSettingValue()
    {
        SettingValue.Instance.SaveSettingValue();
    }

    public bool IsMultiplePlaythroughs()
    {
        return SettingValue.Instance.settingValueData.IsMultiplePlaythroughs;
    }

    public void AllowMultiplePlaythroughs()
    {
        SettingValue.Instance.settingValueData.IsMultiplePlaythroughs = true;
    }

    public void OpenGeneralControlPanel()
    {
        ControlPanel.panel.GetComponent<ControlPanelManager>().OpenPanel(ControlPanelType.General);
    }

}