using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class ExtrasValue : MonoBehaviour
{
    public List<ExtrasValue_BGMData> unLockBGMDatas = new List<ExtrasValue_BGMData>();
    public CharacterExtrasSaveLibrary characterExtrasSaveLibrary = new CharacterExtrasSaveLibrary();

    public static ExtrasValue Instance { get; private set; }
    private string savePath;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        Init();
    }

    public void Init()
    {
        savePath = Path.Combine(Application.persistentDataPath, "ExtrasValue.json");
        LoadOrCreateData();
    }

    public void SetGameValue(GameValue gameValue, List<CharacterENDSaveData> CharacterENDSaveDatas)
    {
        characterExtrasSaveLibrary.SetCharacterDatas(gameValue, CharacterENDSaveDatas);
    }

    private void LoadOrCreateData()
    {
        if (File.Exists(savePath))
        {
            try
            {
                string json = File.ReadAllText(savePath);
                var data = JsonConvert.DeserializeObject<ExtrasValueData>(json);
                if (data != null)
                {
                    characterExtrasSaveLibrary = data.characterExtrasSaveLibrary ?? new CharacterExtrasSaveLibrary();
                    unLockBGMDatas = data.unLockBGMDatas ?? new List<ExtrasValue_BGMData>();
                }
                else
                {
                    characterExtrasSaveLibrary = new CharacterExtrasSaveLibrary();
                    unLockBGMDatas = new List<ExtrasValue_BGMData>();
                    SaveData();
                }
            }
            catch
            {
                characterExtrasSaveLibrary = new CharacterExtrasSaveLibrary();
                unLockBGMDatas = new List<ExtrasValue_BGMData>();
                SaveData();
            }
        }
        else
        {
            characterExtrasSaveLibrary = new CharacterExtrasSaveLibrary();
            unLockBGMDatas = new List<ExtrasValue_BGMData>();
            SaveData();
        }
    }

    public void SaveData()
    {
        var data = new ExtrasValueData
        {
            characterExtrasSaveLibrary = characterExtrasSaveLibrary,
            unLockBGMDatas = unLockBGMDatas
        };
        string json = JsonConvert.SerializeObject(data, Formatting.Indented);
        File.WriteAllText(savePath, json);
    }

    public bool UnlockBGM(int id)
    {
        bool alreadyUnlocked = unLockBGMDatas.Any(b => b.ID == id);

        if (alreadyUnlocked)
        {
            return false;
        }
        ExtrasValue_BGMData bgmData = new ExtrasValue_BGMData(id);
        unLockBGMDatas.Add(bgmData);
        SaveData();
        return true;
    }

    /// <summary>
    /// 检查音乐是否已解锁
    /// </summary>
    public bool IsBGMUnlocked(int id)
    {
        return unLockBGMDatas.Any(b => b.ID == id);
    }

    /// <summary>
    /// 获取所有已解锁的BGM
    /// </summary>
    public List<ExtrasValue_BGMData> GetUnlockedBGMs()
    {
        return new List<ExtrasValue_BGMData>(unLockBGMDatas);
    }

    /// <summary>
    /// 根据ID获取已解锁的BGM
    /// </summary>
    public ExtrasValue_BGMData GetUnlockBGM(int id)
    {
        return unLockBGMDatas.FirstOrDefault(b => b.ID == id);
    }

    public List<CharacterExtrasSaveData> GetCharacterExtrasSaveDatas()
    {
        return characterExtrasSaveLibrary.GetCharacterExtrasSaveDatas();
    }

    public CharacterExtrasSaveData GetCharacterExtrasSaveData(string CharacterKey)
    {
        return characterExtrasSaveLibrary.GetCharacterExtrasSaveData(CharacterKey);
    }

    public bool HasCharacterExtrasSaveData()
    {
        return characterExtrasSaveLibrary.CharacterExtrasSaveDatas.Count != 0;
    }

    public void UpData(CharacterExtrasSaveData saveData)
    {
        if (saveData == null || string.IsNullOrEmpty(saveData.CharacterKey)) return;

        var existing = characterExtrasSaveLibrary.CharacterExtrasSaveDatas
            .FirstOrDefault(c => c.CharacterKey == saveData.CharacterKey);

        if (existing != null)
        {
            int index = characterExtrasSaveLibrary.CharacterExtrasSaveDatas.IndexOf(existing);
            characterExtrasSaveLibrary.CharacterExtrasSaveDatas[index] = saveData;
        }
        else
        {
            characterExtrasSaveLibrary.CharacterExtrasSaveDatas.Add(saveData);
        }
        SaveData();
    }
}

// 用于序列化的数据结构
[System.Serializable]
public class ExtrasValueData
{
    public CharacterExtrasSaveLibrary characterExtrasSaveLibrary;
    public List<ExtrasValue_BGMData> unLockBGMDatas;
}