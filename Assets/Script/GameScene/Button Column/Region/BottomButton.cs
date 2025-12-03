using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class BottomButton : MonoBehaviour
{
    public static BottomButton Instance { get; private set; }

    #region Serializable Classes

    [Serializable]
    public struct PanelEntry
    {
        public string key;
        public Button button;
        public PanelBase panel;
        public int itemID;
        public string defaultIconPath;
        public bool useItemIcon;

        public void CloseEntry()
        {
            if (button != null) button.gameObject.SetActive(false);
            if (panel != null) panel.ClosePanel();
        }
    }

    #endregion

    #region Inspector Fields

    [Header("Panel Configuration")]
    [SerializeField] private List<PanelEntry> panelEntries = new List<PanelEntry>();

    [Header("Other Settings")]
    public Vector3 InitPosition = new Vector3(0, 0, 0);
    public RectTransform PrefabLocation;
    public List<GameObject> otherPanels = new List<GameObject>();

    #endregion

    #region Constants

    private const string ICON_PATH = "MyDraw/Item";

    #endregion

    #region Private Fields

    private Dictionary<string, PanelEntry> panelDictionary = new Dictionary<string, PanelEntry>();
    private Dictionary<KeyCode, PanelEntry> hotkeyDictionary = new Dictionary<KeyCode, PanelEntry>();

    #endregion

    #region Unity Lifecycle

    private void Awake()
    {
        InitializeSingleton();
        InitializePanelDictionary();
    }

    private void Start()
    {
        RegisterEventListeners();
        InitializeAllPanels();
        UpdateHotkeyDictionary();
    }

    private void Update()
    {
        HandleHotkeyInput();
    }

    private void OnDestroy()
    {
        UnregisterEventListeners();
        UnregisterAllItemChanges();
    }

    #endregion

    #region Initialization

    private void InitializeSingleton()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void InitializePanelDictionary()
    {
        panelDictionary.Clear();
        foreach (var entry in panelEntries)
        {
            if (!string.IsNullOrEmpty(entry.key))
            {
                panelDictionary[entry.key.ToLower()] = entry;
            }
        }
    }

    private void InitializeAllPanels()
    {
        foreach (var entry in panelEntries)
        {
            if (entry.button != null)
            {
                InitializePanelEntry(entry);
            }
        }
    }

    private void InitializePanelEntry(PanelEntry entry)
    {
        // Add left-click listener
        entry.button.onClick.AddListener(() => TogglePanel(entry));

        // Add right-click listener
        AddRightClickListener(entry.button, entry.panel?.gameObject);

        // Register item change listener
        if (entry.itemID > 0 || entry.useItemIcon)
        {
            GameValue.Instance.RegisterItemsChange(() => UpdatePanelVisualState(entry));
            UpdatePanelVisualState(entry);
        }
    }

    private void AddRightClickListener(Button button, GameObject panel)
    {
        if (button == null) return;

        EventTrigger eventTrigger = button.GetComponent<EventTrigger>();
        if (eventTrigger == null)
        {
            eventTrigger = button.gameObject.AddComponent<EventTrigger>();
        }

        EventTrigger.Entry rightClickEntry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerDown
        };
        rightClickEntry.callback.AddListener((data) => OnRightClick(data, panel));
        eventTrigger.triggers.Add(rightClickEntry);
    }

    #endregion

    #region Event Management

    private void RegisterEventListeners()
    {
        if (SettingsManager.Instance != null)
        {
            GameShortcutkeyData shortcutData = SettingValue.Instance.settingValueData?.settingShortcutkeyData?.GameShortcutkeyData;
            shortcutData.OnShortcutKeysChanged += UpdateHotkeyDictionary;

        }
        else
        {
            Debug.LogWarning("BottomButton: SettingsManager.Instance is null, cannot register event listener");
        }
    }

    private void UnregisterEventListeners()
    {
        GameShortcutkeyData shortcutData = SettingValue.Instance.settingValueData?.settingShortcutkeyData?.GameShortcutkeyData;
        shortcutData.OnShortcutKeysChanged -= UpdateHotkeyDictionary;
    }

    private void UnregisterAllItemChanges()
    {
        if (GameValue.Instance == null) return;

        foreach (var entry in panelEntries)
        {
            if (entry.itemID > 0 || entry.useItemIcon)
            {
                GameValue.Instance.UnRegisterItemsChange(() => UpdatePanelVisualState(entry));
            }
        }
    }

    private void OnRightClick(BaseEventData data, GameObject panel)
    {
        PointerEventData pointerData = data as PointerEventData;
        if (pointerData != null &&
            pointerData.button == PointerEventData.InputButton.Right &&
            panel != null &&
            panel.activeSelf)
        {
            PanelBase panelBase = panel.GetComponent<PanelBase>();
            panelBase?.ClosePanel();
        }
    }

    #endregion

    #region Hotkey Management

    private void UpdateHotkeyDictionary()
    {
        hotkeyDictionary.Clear();
        GameShortcutkeyData shortcutData = SettingValue.Instance.settingValueData?.settingShortcutkeyData?.GameShortcutkeyData;
        foreach (var entry in panelEntries)
        {
            KeyCode keyCode = GetKeyCodeForPanel(entry.key);
            if (keyCode != KeyCode.None)
            {
                hotkeyDictionary[keyCode] = entry;
            }
        }

        Debug.Log($"BottomButton: Hotkey dictionary updated with {hotkeyDictionary.Count} entries");
    }

    private KeyCode GetKeyCodeForPanel(string panelKey)
    {
        GameShortcutkeyData shortcutData = SettingValue.Instance.settingValueData?.settingShortcutkeyData?.GameShortcutkeyData;
        if (shortcutData == null) return KeyCode.None;

        switch (panelKey.ToLower())
        {
            case "elector": return ShortcutKeyCodeString.ToKeyCode(shortcutData.ElectorKey);
            case "country": return ShortcutKeyCodeString.ToKeyCode(shortcutData.CountryKey);
            case "region": return ShortcutKeyCodeString.ToKeyCode(shortcutData.RegionKey);
            case "character": return ShortcutKeyCodeString.ToKeyCode(shortcutData.CharacterKey);
            case "item": return ShortcutKeyCodeString.ToKeyCode(shortcutData.ItemKey);
            case "recruit": return ShortcutKeyCodeString.ToKeyCode(shortcutData.RecruitKey);
            default: return KeyCode.None;
        }
    }

    private void HandleHotkeyInput()
    {
        foreach (var kvp in hotkeyDictionary)
        {
            if (Input.GetKeyDown(kvp.Key))
            {
                TogglePanel(kvp.Value);
                break; // Only process one hotkey per frame
            }
        }
    }

    #endregion

    #region Visual State Updates

    private void UpdatePanelVisualState(PanelEntry entry)
    {
        if (entry.button == null) return;

        UpdateButtonVisibility(entry);
        UpdateButtonIcon(entry);
    }

    private void UpdateButtonVisibility(PanelEntry entry)
    {
        if (entry.itemID > 0 && GameValue.Instance != null)
        {
            bool hasItem = GameValue.Instance.HasItem(entry.itemID);
            entry.button.transform.parent.gameObject.SetActive(hasItem);
        }
    }

    private void UpdateButtonIcon(PanelEntry entry)
    {
        if (!entry.useItemIcon) return;

        Image buttonImage = entry.button.GetComponent<Image>();
        if (buttonImage == null || GameValue.Instance == null) return;

        int iconItemID = entry.itemID > 0 ? entry.itemID : GetDefaultIconItemID(entry.key);

        if (iconItemID > 0 && GameValue.Instance.HasItem(iconItemID))
        {
            ItemBase item = GameValue.Instance.GetItem(iconItemID);
            if (item != null)
            {
                buttonImage.sprite = item.icon;
            }
        }
        else if (!string.IsNullOrEmpty(entry.defaultIconPath))
        {
            buttonImage.sprite = Resources.Load<Sprite>($"{ICON_PATH}/{entry.defaultIconPath}");
        }
    }

    private int GetDefaultIconItemID(string key)
    {
        switch (key.ToLower())
        {
            case "item": return 7;
            case "recruit": return 6;
            default: return 0;
        }
    }

    #endregion

    #region Panel Toggle Logic

    public void TogglePanelByKey(string key)
    {
        key = key.ToLower();

        if (key == "countryregion")
        {
            ToggleCountryRegionPanel();
            return;
        }

        if (panelDictionary.TryGetValue(key, out PanelEntry entry))
        {
            TogglePanel(entry);
        }
        else
        {
            Debug.LogWarning($"Unknown panel key: {key}");
        }
    }

    private void TogglePanel(PanelEntry entry)
    {
        if (!CanTogglePanel()) return;

        GameObject panelObj = entry.panel?.gameObject;
        PanelBase panelBase = entry.panel;

        if (panelObj == null || panelBase == null) return;

        if (panelBase.IsActive())
        {
            HandleActivePanelToggle(panelObj, panelBase);
        }
        else
        {
            HandleInactivePanelToggle(panelObj, panelBase);
        }
    }

    private bool CanTogglePanel()
    {
        if (BattlePanelManage.Instance?.BattlePanel != null && BattlePanelManage.Instance.BattlePanel.activeSelf)
            return false;

        GameObject current = EventSystem.current?.currentSelectedGameObject;
        if (current != null && current.GetComponent<Button>() == null)
            return false;

        return true;
    }

    private void HandleActivePanelToggle(GameObject panelObj, PanelBase panelBase)
    {
        if (IsPanelTopMost(panelObj))
        {
            panelObj.transform.SetAsFirstSibling();
            panelBase.ClosePanel();
        }
        else
        {
            panelObj.transform.SetAsLastSibling();
        }
    }

    private void HandleInactivePanelToggle(GameObject panelObj, PanelBase panelBase)
    {
        panelBase.OpenPanel();
        panelObj.transform.SetAsLastSibling();
    }

    private bool IsPanelTopMost(GameObject panel)
    {
        Transform parentTransform = panel.transform.parent;
        int siblingCount = parentTransform.childCount;

        for (int i = siblingCount - 1; i >= 0; i--)
        {
            Transform sibling = parentTransform.GetChild(i);
            if (sibling.gameObject.activeSelf)
            {
                return sibling.gameObject == panel;
            }
        }
        return false;
    }

    public void CloseAllPanel()
    {
        foreach (var entry in panelEntries)
        {
            PanelBase panelBase = entry.panel;
            panelBase?.ClosePanel();
        }
    }

    private void ToggleCountryRegionPanel()
    {
        CountryPanelManage.Instance?.ToggleCountryRegionPanel();
    }

    #endregion

    #region Save/Load System

    public TotalPanelSaveData SavePanelData()
    {
        TotalPanelSaveData totalPanelSaveData = new TotalPanelSaveData();

        SaveConfiguredPanels(totalPanelSaveData);
        SaveFixedPanels(totalPanelSaveData);
        SaveOtherPanels(totalPanelSaveData);

        return totalPanelSaveData;
    }

    private void SaveConfiguredPanels(TotalPanelSaveData totalPanelSaveData)
    {
        foreach (var entry in panelEntries)
        {
            PanelSaveData saveData = GetPanelSaveData(entry);
            if (saveData != null)
            {
                totalPanelSaveData.allPanels.Add(saveData);
            }
        }
    }

    private void SaveFixedPanels(TotalPanelSaveData totalPanelSaveData)
    {
        totalPanelSaveData.characterAssistRow = CharacterAssistRowControl.Instance?.GetPanelSaveData();
        totalPanelSaveData.rightColSaveData = RightColumnManage.Instance?.GetPanelSaveData();
        totalPanelSaveData.regionInfoSaveData = RegionInfo.Instance?.GetPanelSaveData();
        totalPanelSaveData.unRegionInfoSaveData = UnplayerRegionInfo.Instance?.GetPanelSaveData();
    }

    private void SaveOtherPanels(TotalPanelSaveData totalPanelSaveData)
    {
        foreach (var other in otherPanels)
        {
            if (other != null)
            {
                TaskPanelControl taskPanel = other.GetComponent<TaskPanelControl>();
                if (taskPanel != null)
                {
                    totalPanelSaveData.allPanels.Add(taskPanel.GetPanelSaveData());
                }
            }
        }
    }

    private PanelSaveData GetPanelSaveData(PanelEntry entry)
    {
        if (entry.panel == null) return null;

        PanelType panelType = GetPanelType(entry.key);

        switch (panelType)
        {
            case PanelType.Elector:
                return ElectorPanelManage.Instance?.GetPanelSaveData();
            case PanelType.Country:
                return CountryPanelManage.Instance?.GetPanelSaveData();
            case PanelType.Region:
                return RegionPanelManage.Instance?.GetPanelSaveData();
            case PanelType.Character:
                return CharacterPanelManage.Instance?.GetPanelSaveData();
            case PanelType.Items:
                return entry.panel.GetComponent<ItemPanelManager>()?.GetPanelSaveData();
            case PanelType.Recruit:
                return RecruitPanelManage.Instance?.GetPanelSaveData();
            default:
                return null;
        }
    }

    public void SetTotalPanelSaveData(TotalPanelSaveData totalPanelSaveData)
    {
        if (totalPanelSaveData == null) return;

        LoadFixedPanels(totalPanelSaveData);
        LoadConfiguredPanels(totalPanelSaveData);
    }

    private void LoadFixedPanels(TotalPanelSaveData totalPanelSaveData)
    {
        CharacterAssistRowControl.Instance?.SetPanelSaveData(totalPanelSaveData.characterAssistRow);
        RegionInfo.Instance?.SetPanelSaveData(totalPanelSaveData.regionInfoSaveData);
        UnplayerRegionInfo.Instance?.SetPanelSaveData(totalPanelSaveData.unRegionInfoSaveData);
        RightColumnManage.Instance?.SetPanelSaveData(totalPanelSaveData.rightColSaveData);
    }

    private void LoadConfiguredPanels(TotalPanelSaveData totalPanelSaveData)
    {
        foreach (var panelData in totalPanelSaveData.allPanels.OrderBy(p => p.order))
        {
            SetPanelSaveData(panelData);
        }
    }

    private void SetPanelSaveData(PanelSaveData panelData)
    {
        switch (panelData.panelType)
        {
            case PanelType.Elector:
                ElectorPanelManage.Instance?.SetPanelSaveData(panelData);
                break;
            case PanelType.Country:
                CountryPanelManage.Instance?.SetPanelSaveData(panelData);
                break;
            case PanelType.Region:
                RegionPanelManage.Instance?.SetPanelSaveData(panelData);
                break;
            case PanelType.Character:
                CharacterPanelManage.Instance?.SetPanelSaveData(panelData);
                break;
            case PanelType.Items:
                var itemPanel = GetPanelByType(PanelType.Items);
                itemPanel?.GetComponent<ItemPanelManager>()?.SetPanelSaveData(panelData);
                break;
            case PanelType.Recruit:
                RecruitPanelManage.Instance?.SetPanelSaveData(panelData);
                break;
            case PanelType.Task:
                RightColumnManage.Instance?.ShowTaskPanel(panelData);
                break;
        }
    }

    #endregion

    #region Helper Methods

    private PanelType GetPanelType(string key)
    {
        switch (key.ToLower())
        {
            case "elector": return PanelType.Elector;
            case "country": return PanelType.Country;
            case "region": return PanelType.Region;
            case "character": return PanelType.Character;
            case "item": return PanelType.Items;
            case "recruit": return PanelType.Recruit;
            default: return PanelType.None;
        }
    }

    private GameObject GetPanelByType(PanelType panelType)
    {
        foreach (var entry in panelEntries)
        {
            if (GetPanelType(entry.key) == panelType)
            {
                return entry.panel?.gameObject;
            }
        }
        return null;
    }

    public void SetTotalPanelSaveData()
    {
        Debug.Log("SetTotalPanelSaveData: No implementation yet");
    }

    #endregion
}

#region Supporting Classes

public static class PanelExtensions
{
    public static PanelSaveData GetSaveData(PanelBase panel, GameObject panelObj, PanelType panelType = PanelType.None)
    {
        return new PanelSaveData
        {
            isActive = panelObj.activeSelf,
            localPosX = panelObj.transform.localPosition.x,
            localPosY = panelObj.transform.localPosition.y,
            localPosZ = panelObj.transform.localPosition.z,
            order = panelObj.transform.GetSiblingIndex(),
            panelType = panelType
        };
    }

    public static void SetSaveData(PanelBase panel, GameObject panelObj, PanelSaveData panelSaveData)
    {
        if (panelSaveData.isActive)
        {
            panel.OpenPanel();
        }
        else
        {
            panel.ClosePanel();
        }

        panelObj.transform.localPosition = panelSaveData.GetPosition();
        panelObj.transform.SetSiblingIndex(panelSaveData.order);
    }
}

public class TotalPanelSaveData
{
    public PanelSaveData characterAssistRow;
    public PanelSaveData rightColSaveData;
    public PanelSaveData regionInfoSaveData;
    public PanelSaveData unRegionInfoSaveData;
    public List<PanelSaveData> allPanels = new List<PanelSaveData>();
}

public class PanelSaveData
{
    public int order = -1;
    public bool isActive;
    public float localPosX;
    public float localPosY;
    public float localPosZ;
    public PanelType panelType;
    public Dictionary<CustomDataType, string> customData = new Dictionary<CustomDataType, string>();

    public void SetPosition(Vector3 pos)
    {
        localPosX = pos.x;
        localPosY = pos.y;
        localPosZ = pos.z;
    }

    public Vector3 GetPosition()
    {
        return new Vector3(localPosX, localPosY, localPosZ);
    }

    public string GetTaskID()
    {
        if (customData.TryGetValue(CustomDataType.Task, out string taskID))
        {
            return taskID;
        }
        return null;
    }
}

public enum PanelType
{
    None, Elector, Region, Character, City,
    Country, Items, Recruit, Task
}

public enum CustomDataType
{
    None, PanelType, ScrollViewValue, Sel,
    Task, Region, Character, City
}

#endregion