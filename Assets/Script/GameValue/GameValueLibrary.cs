using ExcelDataReader;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.TextCore.Text;
using static ExcelReader;

public class GameValueLibrary
{
    private List<ExcelItemData> InitItemDatas = new List<ExcelItemData>();
    private List<ExcelRegionData> InitRegionDatas = new List<ExcelRegionData>();
    private List<ExcelCharacterData> InitImportantCharacterDatas = new List<ExcelCharacterData>();
    private List<ExcelCharacterData> InitNormalCharacterDatas = new List<ExcelCharacterData>();
    private List<ExcelCharacterData> InitMonsterCharacterDatas = new List<ExcelCharacterData>();
    private List<ExcelCharacterENDData> ExcelCharacterENDs = new List<ExcelCharacterENDData>();
    private List<ExcelTaskStory> ExcelTaskStoryNodes = new List<ExcelTaskStory>();

    private Dictionary<int, Character> monsterCache = new Dictionary<int, Character>();
    private Dictionary<int, Character> normalCache = new Dictionary<int, Character>();
  //  private List<ItemBase> itemBases = new List<ItemBase>();
    private List<Skill> skills = new List<Skill>();

    public GameValueLibrary()
    {
        InitItemDatas = GetItemData();
        ExcelCharacterENDs = GetChracterENDData();
        InitImportantCharacterDatas = GetImportantCharacterData();
        InitNormalCharacterDatas = GetImportantCharacterData();
        InitMonsterCharacterDatas = GetImportantCharacterData();
        InitRegionDatas = GetRegionData();
        skills = GetInitSkillList();
        ExcelTaskStoryNodes = ReadTaskStory();
    }

    #region Item
    public ItemBase GetInitItemBase(int ID)
    {
        ExcelItemData itemData = InitItemDatas.Find(item => item.ID == ID);
        ItemBase item = new ItemBase(itemData);
        return item;
    }

    public List<ItemBase> GetInitItemBaseList()
    {
        List<ItemBase> allInitItem = new List<ItemBase>();
        if (InitItemDatas.Count == 0) Debug.Log("huge bug");
        foreach (var itemData in InitItemDatas)
        {
            ItemBase item = new ItemBase(itemData);
            allInitItem.Add(item);
        }
        return allInitItem;
    }


    #endregion

    #region Skill

    public List<Skill> GetSkillList()
    {
        return skills;
    }

    List<Skill> GetInitSkillList()
    {
        List<ExcelSkillData> ExcelSkillDatas = GetSkillData();
        List<Skill> allSkillData = new List<Skill>();
        if (ExcelSkillDatas.Count == 0) Debug.Log("huge bug");
        foreach (var skillData in ExcelSkillDatas)
        {
            Skill skillBase = new Skill(skillData);
            allSkillData.Add(skillBase);
            if (skillBase == null) Debug.Log("what fuck happend in GetInitSkillList()");
        }
        return allSkillData;
    }


    public Skill GetSkill(int skillID)
    {
        Skill skill = skills.Find(s => s.ID == skillID);
        if (skill == null)Debug.LogWarning($"[GetSkill] Skill with ID {skillID} not found!");
        return skill;
    }
    #endregion


    #region Character

    public void SetSaveData(Dictionary<int, CharacterSaveData> characterSaveDataDict)
    {
        foreach (var characterData in characterSaveDataDict.Values)
        {
            int id = characterData.characterID;

            if (characterSaveDataDict.TryGetValue(id, out var saveData))
            {
                Character character = new Character(saveData, GetCharacterExcelData(saveData.characterID, saveData.Category));
            }
        }
    }

    public ExcelCharacterData GetCharacterExcelData(int characterID, CharacterCategory category = CharacterCategory.Important)
    {
        List<ExcelCharacterData> targetList = null;

        // ???????????
        switch (category)
        {
            case CharacterCategory.Normal:
                targetList = InitNormalCharacterDatas;
                break;
            case CharacterCategory.Monster:
                targetList = InitMonsterCharacterDatas;
                break;
            case CharacterCategory.Important:
                targetList = InitImportantCharacterDatas;
                break;
            default:
                targetList = InitNormalCharacterDatas;
                break;
        }

        return targetList.Find(data => data.ID == characterID);
    }


    public List<Character> GenerateInitAllImportantCharacters()
    {
        List<Character> allImportantCharacters = new List<Character>();
        allImportantCharacters = InitImportantCharacters();
        List<Character> haveEndCharacter = AttachCharacterENDs(allImportantCharacters);
        AddJudgmentEND(haveEndCharacter);
        return allImportantCharacters;
    }

    List<Character> GenerateInitMonsterCharacters()
    {
        List<Character> InitMonsterCharacters = GetCharacterDataHelp(InitMonsterCharacterDatas, CharacterCategory.Monster);
        return InitMonsterCharacters;
    }

    List<Character> GenerateInitNormalCharacters()
    {
        List<Character> InitNormalCharacters = GetCharacterDataHelp(InitMonsterCharacterDatas, CharacterCategory.Normal);
        return InitNormalCharacters;
    }


    public Character GenerateNewMonster(int MonsterID, List<ItemBase> allItems)
    {
        //  Character Monster = GenerateInitMonsterCharacters(allItems).Find(Monster => Monster.GetCharacterID() == MonsterID);
        ExcelCharacterData monsterExcelData = InitMonsterCharacterDatas.Find(monsterExcelData => monsterExcelData.ID == MonsterID);
        Character Monster = new Character(monsterExcelData, CharacterCategory.Monster);

        return Monster;
    }

    public Character GenerateNewNormalCharacters(int NormalID)
    {
        ExcelCharacterData normalExcelData = InitMonsterCharacterDatas.Find(monsterExcelData => monsterExcelData.ID == NormalID);
        Character Normal = new Character(normalExcelData, CharacterCategory.Normal);
        return Normal;
    }

    #endregion

    #region Character helper function


    List<Character> InitImportantCharacters()
    {
        List<Character> characters = GetCharacterDataHelp(InitImportantCharacterDatas, CharacterCategory.Important);
        return characters;
    }

  /*  void SyncPlayerNameToGlobal()
    {
        SettingsManager settingsManager = SettingsManager.Instance;
        if (settingsManager != null)
        {
            GameValue.Instance.SetPlayerName(settingsManager.GetName());
        }
    }*/

    List<Character> AttachCharacterENDs(List<Character> allImportantCharacters)
    {
        List<Character> haveEndCharacter = new List<Character>();

        for (int i = 1; i < ExcelCharacterENDs.Count; i++)
        {
            CharacterEND END = new CharacterEND(ExcelCharacterENDs[i]);
            int charID = END.GetCharacterID();

            if (charID >= allImportantCharacters.Count)
            {
                Debug.LogWarning("In Gamevalue , END.GetCharacterID() > allImportantCharacters.Count, dont have character????");
            }
            else if (charID != -1)
            {
                Character character = allImportantCharacters[charID];
                character.AddCharacterEND(END.GetENDID(), END);

                if (!haveEndCharacter.Contains(character) && character.GetCharacterID() != 0)
                {
                    haveEndCharacter.Add(character);
                }
            }
        }

        return haveEndCharacter;
    }

    void AddJudgmentEND(List<Character> haveEndCharacter)
    {
        CharacterEND judgmentEND = new CharacterEND(ExcelCharacterENDs[0]);

        for (int i = 0; i < haveEndCharacter.Count; i++)
        {
            haveEndCharacter[i].AddCharacterEND(judgmentEND.GetENDID(), judgmentEND);
        }
    }
    List<Character> GetCharacterDataHelp(List<ExcelCharacterData> readDatas, CharacterCategory Category)
    {
        List<Character> characterList = new List<Character>();
        for (int i = 0; i < readDatas.Count; i++)
        {
            Character character = new Character(readDatas[i], Category);
            characterList.Add(character);
        }
        return characterList;
    }


    #endregion

    #region Task
    public ExcelTaskStory GetExcelTaskStory(string key)
    {
        return ExcelTaskStoryNodes.Find(node => node.key == key);
    }
    #endregion

    #region Region

    public void SetSaveData(Dictionary<int, RegionValueSaveData> regionSaveDataDict)
    {
        Debug.Log($"SetSaveData(Dictionary<int, RegionValueSaveData> regionSaveDataDict) have {regionSaveDataDict.Values.Count}, gameValue has region {GameValue.Instance.GetAllRegionValues().Count}");

        foreach (var regionData in regionSaveDataDict.Values)
        {
            int id = regionData.regionID;
            if (regionSaveDataDict.TryGetValue(id, out var saveData))
            {
                ExcelRegionData excelRegion = GetRegionExcelData(saveData.regionID);
                RegionValue region = new RegionValue(excelRegion);
                region.SetSaveData(saveData);
            } 
        }
        Debug.Log($"SetSaveData(Dictionary<int, RegionValueSaveData> regionSaveDataDict) have {regionSaveDataDict.Values.Count}, gameValue has region {GameValue.Instance.GetAllRegionValues().Count}");
    }

    public ExcelRegionData GetRegionExcelData(int regionID)
    {
        List<ExcelRegionData> targetList = InitRegionDatas;

        return targetList.Find(data => data.ID == regionID);
    }

    public List<RegionValue> GenerateInitRegionValue()
    {
        if (InitRegionDatas.Count == 0) Debug.Log("huge bug");
        List<RegionValue> allRegions = new List<RegionValue>();
        for (int i = 0; i < InitRegionDatas.Count; i++)
        {
            RegionValue region = new RegionValue(InitRegionDatas[i]);
            allRegions.Add(region);
        }
        return allRegions;
    }


    #endregion
}
