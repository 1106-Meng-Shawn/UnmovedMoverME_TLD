using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.TextCore.Text;

[System.Serializable]
public class CharacterExtrasSaveLibrary
{
    public List<CharacterExtrasSaveData> CharacterExtrasSaveDatas = new List<CharacterExtrasSaveData>();

    public void SetCharacterDatas(GameValue gameValue, List<CharacterENDSaveData> characterENDSaveDatas)
    {
        if (gameValue == null)
        {
            Debug.LogError("[CharacterExtrasSaveLibrary] gameValue is null!");
            return;
        }

        if (characterENDSaveDatas == null)
        {
            Debug.LogWarning("[CharacterExtrasSaveLibrary] characterENDSaveDatas is null — no END data will be used.");
            characterENDSaveDatas = new List<CharacterENDSaveData>();
        }

        List<Character> allCharacters = gameValue.GetCountryCharacters(gameValue.GetPlayerCountryENName());
        if (allCharacters == null || allCharacters.Count == 0)
        {
            Debug.LogWarning("[CharacterExtrasSaveLibrary] No characters found in GameValue.");
            return;
        }

        foreach (var character in allCharacters)
        {
            if (character == null) continue;

            var existingData = CharacterExtrasSaveDatas.FirstOrDefault(c => c.CharacterKey == character.GetCharacterKey());

            var endData = characterENDSaveDatas.FirstOrDefault(c => c.characterKey == character.GetCharacterKey());

            if (existingData == null)
            {
                var newData = new CharacterExtrasSaveData(character,  endData );
                CharacterExtrasSaveDatas.Add(newData);
            }
            else
            {
                existingData.SetCharacterExtrasSaveData(character,  endData );
            }
        }

        ExtrasValue.Instance.SaveData();
    }

    public CharacterExtrasSaveData GetCharacterExtrasSaveData(string CharacterKey)
    {
        return CharacterExtrasSaveDatas.FirstOrDefault(c => c.CharacterKey == CharacterKey);
    }


    public List<CharacterExtrasSaveData> GetCharacterExtrasSaveDatas()
    {
        return CharacterExtrasSaveDatas;
    }
}



[System.Serializable]
public class CharacterExtrasSaveData
{
    [NonSerialized] public Action<string> OnValueChanged; // ??????????

    #region --- ???? ---

    [SerializeField] private bool isStar;
    [SerializeField] private string characterKey;
    [SerializeField] private Dictionary<string, string> characterNameDictionary = new();
    [SerializeField] private CharacterCategory category;
    [SerializeField] private CharacterRole roleClass;
    [SerializeField] private int maxForce;
    [SerializeField] private int force;
    [SerializeField] private int battleMoveNum;
    [SerializeField] private int currentLevel;
    [SerializeField] private int baseMaxLevel;
    [SerializeField] private int experience;
    [SerializeField] private int health;
    [SerializeField] private int favorability;
    [SerializeField] private FavorabilityLevel favorabilityLevel;
    [SerializeField] private int[,] characterValueArray = new int[3, 5];

    [SerializeField] private List<int> currentSkillIDs = new List<int>();
    [SerializeField] private List<int> skillLibraryIDs = new List<int>();
    [SerializeField] private List<string> characterFileTypeList = new List<string>();
    [SerializeField] private List<MultiplePlaythroughsGameCharacterRowControlSaveData> rowSaveDatas = new();

    [SerializeField] private List<int> characterTECount = new List<int>();
    [SerializeField] private int geCount = 0;
    [SerializeField] private int beCount = 0;
    [SerializeField] private List<int> characterGECount = new List<int>();
    [SerializeField] private List<int> characterBECount = new List<int>();

    #endregion

    #region --- ???? ---

    public bool IsStar { get => isStar; set => SetValue(ref isStar, value, nameof(IsStar)); }
    public string CharacterKey { get => characterKey; set => SetValue(ref characterKey, value, nameof(CharacterKey)); }
    public Dictionary<string, string> CharacterNameDictionary { get => characterNameDictionary; set => SetValue(ref characterNameDictionary, value, nameof(CharacterNameDictionary)); }
    public CharacterCategory Category { get => category; set => SetValue(ref category, value, nameof(Category)); }
    public CharacterRole RoleClass { get => roleClass; set => SetValue(ref roleClass, value, nameof(RoleClass)); }
    public int MaxForce { get => maxForce; set => SetValue(ref maxForce, value, nameof(MaxForce)); }
    public int Force { get => force; set => SetValue(ref force, value, nameof(Force)); }
    public int BattleMoveNum { get => battleMoveNum; set => SetValue(ref battleMoveNum, value, nameof(BattleMoveNum)); }
    public int CurrentLevel { get => currentLevel; set => SetValue(ref currentLevel, value, nameof(CurrentLevel)); }
    public int BaseMaxLevel { get => baseMaxLevel; set => SetValue(ref baseMaxLevel, value, nameof(BaseMaxLevel)); }
    public int Experience { get => experience; set => SetValue(ref experience, value, nameof(Experience)); }
    public int Health { get => health; set => SetValue(ref health, value, nameof(Health)); }
    public int Favorability { get => favorability; set => SetValue(ref favorability, value, nameof(Favorability)); }
    public FavorabilityLevel FavorabilityLevel { get => favorabilityLevel; set => SetValue(ref favorabilityLevel, value, nameof(FavorabilityLevel)); }

    public int[,] CharacterValueArray { get => characterValueArray; set => SetValue(ref characterValueArray, value, nameof(CharacterValueArray)); }
    public List<int> CurrentSkillIDs { get => currentSkillIDs; set => SetValue(ref currentSkillIDs, value, nameof(CurrentSkillIDs)); }
    public List<int> SkillLibraryIDs { get => skillLibraryIDs; set => SetValue(ref skillLibraryIDs, value, nameof(SkillLibraryIDs)); }
    public List<string> CharacterFileTypeList  { get => characterFileTypeList; set => SetValue(ref characterFileTypeList, value, nameof(CharacterFileTypeList)); } 
    public List<MultiplePlaythroughsGameCharacterRowControlSaveData> RowSaveDatas { get => rowSaveDatas; set => SetValue(ref rowSaveDatas, value, nameof(RowSaveDatas)); }
    public List<int> CharacterTECount { get => characterTECount; set => SetValue(ref characterTECount, value, nameof(CharacterTECount)); }
    public int GECount { get => geCount; set => SetValue(ref geCount, value, nameof(GECount)); }
    public int BECount { get => beCount; set => SetValue(ref beCount, value, nameof(BECount)); }
    public List<int> CharacterGECount { get => characterGECount; set => SetValue(ref characterGECount, value, nameof(CharacterGECount)); }
    public List<int> CharacterBECount { get => characterBECount; set => SetValue(ref characterBECount, value, nameof(CharacterBECount)); }

    #endregion

    #region --- ???? ---

    public CharacterExtrasSaveData() { }

    public CharacterExtrasSaveData(Character character, CharacterENDSaveData characterENDSaveData)
    {
        if (character == null) return;

        IsStar = false;
        CharacterKey = character.GetCharacterKey();
        Category = character.GetCategory();
        CharacterNameDictionary = character.GetCharacterNameDictionary();
        RoleClass = character.RoleClass;
        MaxForce = character.MaxForce;
        Force = character.Force;

        BattleMoveNum = character.BattleMoveNum;
        CurrentLevel = character.GetCurrentLevel();
        BaseMaxLevel = character.GetBaseMaxLevel();

        Experience = character.GetExperience();
        Health = character.GetMaxHealth();
        Favorability = character.Favorability;
        FavorabilityLevel = character.FavorabilityLevel;

        var sourceArray = character.GetCharacterValueArray();
        for (int i = 0; i < 3; i++)
            for (int j = 0; j < 5; j++)
                characterValueArray[i, j] = sourceArray[i, j];

        var skills = character.GetSkills();
        if (skills != null)
            foreach (var skill in skills)
                if (skill != null)
                    currentSkillIDs.Add(skill.ID);

        var skillLib = character.GetSkillLibrary();
        if (skillLib != null)
            foreach (var skill in skillLib)
                if (skill != null)
                    skillLibraryIDs.Add(skill.ID);

        CharacterFileTypeList.Add(character.GetCharacterFileType());
        ApplyENDData(characterENDSaveData);
    }

    #endregion

    #region --- ???? ---

    public void SetCharacterExtrasSaveData(Character character, CharacterENDSaveData characterENDSaveData)
    {
        if (character == null) return;
        if (CharacterKey != character.GetCharacterKey()) return;

        MaxForce = Mathf.Max(MaxForce, character.MaxForce);
        Force = Mathf.Max(Force, character.Force);
        BattleMoveNum = Mathf.Max(BattleMoveNum, character.BattleMoveNum);
        CurrentLevel = Mathf.Max(CurrentLevel, character.GetCurrentLevel());
        BaseMaxLevel = Mathf.Max(BaseMaxLevel, character.GetBaseMaxLevel());
        Experience = Mathf.Max(Experience, character.GetExperience());
        Health = Mathf.Max(Health, character.GetMaxHealth());
        Favorability = Mathf.Max(Favorability, character.Favorability);
        FavorabilityLevel = (FavorabilityLevel)Mathf.Max((int)FavorabilityLevel, (int)character.FavorabilityLevel);

        var newArray = character.GetCharacterValueArray();
        for (int i = 0; i < 3; i++)
            for (int j = 0; j < 5; j++)
                characterValueArray[i, j] = Mathf.Max(characterValueArray[i, j], newArray[i, j]);

        var newSkills = character.GetSkills();
        if (newSkills != null)
            foreach (var skill in newSkills)
                if (skill != null && !currentSkillIDs.Contains(skill.ID))
                    currentSkillIDs.Add(skill.ID);

        var newSkillLib = character.GetSkillLibrary();
        if (newSkillLib != null)
            foreach (var skill in newSkillLib)
                if (skill != null && !skillLibraryIDs.Contains(skill.ID))
                    skillLibraryIDs.Add(skill.ID);

        if (!CharacterFileTypeList.Contains(character.GetCharacterFileType()))
            CharacterFileTypeList.Add(character.GetCharacterFileType());

        ApplyENDData(characterENDSaveData);
    }

    #endregion

    #region --- END ???? ---

    private void ApplyENDData(CharacterENDSaveData endData)
    {
        if (!endData.HasValue()) return;
        if (endData.IsLastJudgmentHappen && endData.characterKey != CharacterConstants.MariaKey) return;

        if (endData.TEID != -1 && !CharacterTECount.Contains(endData.TEID))
            CharacterTECount.Add(endData.TEID);

        if (endData.GEID != -1 && !CharacterGECount.Contains(endData.GEID))
        {
            CharacterGECount.Add(endData.GEID);
            GECount++;
        }

        if (endData.BEID != -1 && !CharacterBECount.Contains(endData.BEID))
        {
            CharacterBECount.Add(endData.BEID);
            BECount++;
        }
    }

    #endregion

    #region --- ???? ---

    public float GetExpRate() => (float)Experience / GetRequiredExpToLvUp();

    public int GetRequiredExpToLvUp() => GameCalculate.GetRequiredExpToLvUp(CurrentLevel, BaseMaxLevel);

    public int GetMaxLimit() => GetValue(2, 0) * 100 + GetValue(2, 4);

    public int GetValue(int valueType, int index) => characterValueArray[valueType, index];

    public bool HasEND()
    {
        if (CharacterKey == CharacterConstants.PlayerKey) return false;
        if (Category != CharacterCategory.Important) return false;
        return GECount != 0 || BECount != 0;
    }

    public Sprite GetCharacterIcon()
        => GetSprite.GetCharacterIcon(CharacterKey, CharacterFileTypeList[0]);

    public string GetCharacterName()
    {
        string lang = LocalizationSettings.SelectedLocale.Identifier.Code;
        return CharacterNameDictionary.TryGetValue(lang, out var name)
            ? name : CharacterNameDictionary[LanguageCode.EN];
    }

    #endregion

    #region --- ?????? ---
    private void SetValue<T>(ref T field, T newValue, string fieldName)
    {
        if (!EqualityComparer<T>.Default.Equals(field, newValue))
        {
            field = newValue;
            OnValueChanged?.Invoke(fieldName);
            ExtrasValue.Instance.UpData(this);
        }
    }

    public void ValueChangeListener(bool isAdd, Action<string> action)
    {
        if (isAdd)
            OnValueChanged += action;
        else
            OnValueChanged -= action;
    }

    #endregion
}
