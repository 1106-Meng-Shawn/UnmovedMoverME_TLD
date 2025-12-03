using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static SettingValue;
using static GetColor;
using System.Runtime.InteropServices.WindowsRuntime;

public class SettingConstant
{
    public const string SettingFilePath = "SettingValue.json";
}



public class SettingValue : MonoBehaviour
{
    #region Test

    public bool testIsMultiplePlaythroughs = false;
    #endregion

    public static SettingValue Instance { get; private set; }

    private string settingFilePath;
    public SettingValueData settingValueData { get; private set; } = new SettingValueData();

    public StoryMultiplePlaythroughs StoryMultiplePlaythroughsData { get; private set; } = new StoryMultiplePlaythroughs();



    [SerializeField] SettingValue_Library settingValue_Library;

    [System.Serializable]
    public class SettingValueData
    {
        public string currentPlayerName;
        public bool IsMultiplePlaythroughs = false;
        public int maxAchievement = 0;

        public List<string> TotalGameSavePath = new();

        public DisplaySettingValue displaySettingValue = new();
        public SoundSettingValue soundSettingValue = new();
        public TextSettingValue textSettingValue = new();
        public SaveData saveData = null;
        public SettingShortcutkeyData settingShortcutkeyData = new();

    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // ??????

        settingFilePath = Path.Combine(Application.persistentDataPath,SettingConstant.SettingFilePath);
        Load();
        settingValueData.IsMultiplePlaythroughs = testIsMultiplePlaythroughs;
    }

    public void Load()
    {
        if (File.Exists(settingFilePath))
        {
            string json = File.ReadAllText(settingFilePath);
            settingValueData = JsonConvert.DeserializeObject<SettingValueData>(json);
            settingValueData.TotalGameSavePath ??= new List<string>();
            settingValueData.displaySettingValue ??= new DisplaySettingValue();
            settingValueData.soundSettingValue ??= new SoundSettingValue();
            settingValueData.textSettingValue ??= new TextSettingValue();
            settingValueData.saveData ??= null;
            settingValueData.settingShortcutkeyData ??= new SettingShortcutkeyData();
        }
        else
        {
            settingValueData = new SettingValueData();
            SaveSettingValue(); 
        }
    }

    public void SaveSettingValue()
    {
        string json = JsonConvert.SerializeObject(settingValueData, Formatting.Indented);
        File.WriteAllText(settingFilePath, json);
        Debug.Log($"SaveSettingValue(){settingFilePath}");
    }

    public bool HasSaveData()
    {
        return settingValueData.saveData != null;
    }

    public void SetPlayerName(string playerName)
    {
        settingValueData.currentPlayerName = playerName;
        SaveSettingValue();
    }

    public string GetPlayerName()
    {
        return settingValueData.currentPlayerName;
    }

    public int GetMaxAchievement()
    {
        return settingValueData.maxAchievement;
    }

    public int GetRemainingAchievement()
    {
        return settingValueData.maxAchievement - StoryMultiplePlaythroughsData.GetAchievementCost();
    }



    public DisplaySettingValue GetDisplaySettingValue()
    {
        return settingValueData.displaySettingValue;
    }

    public SoundSettingValue GetSoundSettingValue()
    {
        return settingValueData.soundSettingValue;
    }

    public TextSettingValue GetTextSettingValue()
    {
       return settingValueData.textSettingValue;
    }

    public bool IsMultiplePlaythroughs()
    {
        return settingValueData.IsMultiplePlaythroughs;
    }

    public void AllowMultiplePlaythroughs()
    {
        settingValueData.IsMultiplePlaythroughs = true;
    }

    public void SaveContinueData()
    {
        SaveData ContinueSaveData = LoadPanelManage.Instance.GetCurrentSaveData();
        if (ContinueSaveData != null) {
            settingValueData.saveData = ContinueSaveData;
            SaveSettingValue();
        }
    }

    private void OnDestroy()
    {
        if (Instance != null && Instance == this) SaveContinueData();
    }

    public BGMScriptableObject GetBGM(int id)
    {
        return settingValue_Library.GetBGMByID(id);
    }

    public BGMScriptableObject GetBGM(string title)
    {
        return settingValue_Library.GetBGMByTitle(title);
    }


}