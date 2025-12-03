using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization.Settings;
using UnityEngine.TextCore.Text;
using static ExcelReader;
using static FormatNumber;
using static GetColor;
using static GetSprite;
using static GetString;


#region Value
public class CharacterConstants
{
    public const string PlayerKey = "Player";

    public const string NoneKey = "-1";

    public static string PlayerType = "Player";

    public const int MaxMoveCount = 12;

    public const string Random = "RAND";
    public const string MonsterCategory = "Monster";
    public const string NormalCategory = "Normal";
    public const string ImportantCategory = "";


    public static string DIE = "DIE";
    public static string Capture = "Capture";

    public const string MariaKey = "Maria";


    public const string EmiliaKey = "Emilia";
    public const string LiyaKey = "Liya";
    public const string FriederikeKey = "Friederike";
    public const string PurpleMaidKey = "37";

    public const string PinkMaidKey = "22";
    public const string PinkMaid2Key = "25";
    public const string BuleMaidKey = "36";
    public const string BuleMaid2key = "-1";

    public const string BlackMaidKey = "38";
    public const string BlackMaid2Key = "-1";
    public const string GreenMaidKey = "-1";
    public const string GreenMaid2Key = "-1";

    public const string MageMaidKey = "-1";
    public const string BerserkerMaidKey = "-1";
    public const string HiyoriKey = "39";


    public static int GetMaxMoveShowCount(int totalMoveNum)
    {
        return Mathf.Min(totalMoveNum, MaxMoveCount);
    }

}

public enum CharacterRole
{
    Swordsman, Lancer, Shieldbearer,
    Archer, Alchemist, Assassin, Priest, Commander, 
    Cavalry, Knight, Magician, Berserker, Bard, Brawler,
    Commoner, Monster,Special,
    All, Cance, Custom
}


public enum CharacterTag
{
    None, Battle, Explore, Lord, Help
}

public enum CharacterCategory
{
    Important, Normal,Monster
}


#endregion

public class Character
{
    [Header("Basic Info")]
    private int characterID;
    private string characterKey;
    private CharacterCategory Category;

    private Dictionary<string, string> characterName = new();
    private bool star;

    private bool isPersonBattle = false;

    public bool IsPersonBattle
    {
        get => isPersonBattle;
        set => SetProperty(ref isPersonBattle, value);
    }


    private bool canDelete = false;

    public bool CanDelete
    {
        get => canDelete;
        set => SetProperty(ref canDelete, value);
    }


    private int baseMaxLevel = 0;
    private int currentLevel = 0;

    private int CurrentLevel
    {
        get => currentLevel;
        set => SetProperty(ref currentLevel, value);
    }

    private int experience = 0;

    private int Experience
    {
        get => experience;
        set => SetProperty(ref experience, value);
    }

    public bool Star
    {
        get => star;
        set => SetProperty(ref star, value);
    }


    private string country;
  /*  public string Country
    {
        get => country;
        set => SetProperty(ref country, value);
    } */


    private int itemID = ItemConstants.NoneID;
    public int ItemID
    {
        get => itemID;
        set => SetProperty(ref itemID, value);
    }


    // public List<Sprite> roleSprites;
    //   private int roleClass; // Swordsman 1, Lancer 2, Shieldbearer 3, 
    // Archer 4, Alchemist 5, Assassin 6, Priest 7, Commander 8, 
    // Cavalry 9, Knight 10, Mage 11, Berserker 12, None 13, Monster 14, bard 15, Special 16
    // public Sprite roleIcon;
    private CharacterRole roleClass;

    public CharacterRole RoleClass
    {
        get => roleClass;
        set => SetProperty(ref roleClass, value);
    }

    private List<RegionValue> lordRegions = new List<RegionValue>();


    private bool isMoved;
    public bool IsMoved
    {
        get => isMoved;
        set => SetProperty(ref isMoved, value);
    }


    private int captureProbability;

    public int CaptureProbability
    {
        get => captureProbability;
        set => SetProperty(ref captureProbability, value);
    }


    private int recruitCost;

    public int RecruitCost
    {
        get => recruitCost;
        set => SetProperty(ref recruitCost, value);
    }


    private BattlePosition battlePosition;


    // public Sprite maxForceSprite;

    private int maxForce;
    public int MaxForce
    {
        get => maxForce;
        set => SetProperty(ref maxForce, value);
    }


    //public Sprite forceSprite;

    private int force ;

    public int Force
    {
        get => force;
        set => SetProperty(ref force, value);
    }


    private int battleMoveNum;

    public int BattleMoveNum
    {
        get => battleMoveNum;
        set => SetProperty(ref battleMoveNum, value);
    }

    int health;

    public int Health
    {
        get => health;
        set => SetProperty(ref health, value);
    }


    int favorability;

    public int Favorability
    {
        get => favorability;
        set => SetProperty(ref favorability, value);
    }


    FavorabilityLevel favorabilityLevel; // 0 is player, 1 is normal , 2 is lover;

    public FavorabilityLevel FavorabilityLevel
    {
        get => favorabilityLevel;
        set => SetProperty(ref favorabilityLevel, value);
    }

    private Dictionary<int, CharacterEND> characterENDs = new();


    int[,] CharacterValueArray = new int[3, 5]  // 3 ? 4 ?
    {
        {1, 2, 3, 4,5}, // battle // 0 is attack, 1 is defense, 2 is magic , 3 is speed, 4 is lucky 
        {5, 6, 7, 8, 5}, // Parameter // 0 is food, 1 is scienceP,2 is politics, 3 is gold , 4 is faith
        {9, 10, 11, 12, 5} //Help  // 0 is leadership, 1 is scout, 2 is build, 3 is negotiation, 4 is charm
    };

    public Sprite icon;
    public Sprite image;

    private List<Skill> skills = new List<Skill>();
    private List<Skill> skillLibrary = new List<Skill>();

    private string type;
    public string Type
    {
        get => type;
        set => SetProperty(ref type, value);
    }

    public event Action OnCharacterChanged;

    private CharacterTag tag; // 0 is null, 1 is battle,2 is explore , 3 is lord ,4 is help
    public CharacterTag Tag
    {
        get => tag;
        set => SetProperty(ref tag, value);
    }

    protected bool SetProperty<T>(ref T field, T value)
    {
        if (!EqualityComparer<T>.Default.Equals(field, value))
        {
            field = value;
            OnCharacterChanged?.Invoke(); 
            return true;
        }
        return false;
    }

    public Character(CharacterSaveData saveData, ExcelCharacterData excelCharacterData)
    {
        List<Skill> allSkills = GameValue.Instance.GetSkillList();
        Star = saveData.star;
        IsMoved = false;
        characterID = saveData.characterID;
        GenerateCharacterKey(excelCharacterData, saveData.Category);
        this.Category = saveData.Category;

        SetCountry(saveData.country);
        roleClass = (CharacterRole)saveData.roleClass;

        SetItem(GameValue.Instance.GetItem(saveData.itemWithCharacterID));
        battleMoveNum = saveData.battleMoveNum;

        IsPersonBattle = saveData.isPersonBattle;
        currentLevel = saveData.CurrentLevel;
        baseMaxLevel = saveData.baseMaxLevel;
        experience = saveData.Experience;
      //  SetInitLevel(excelCharacterData);
        MaxForce = saveData.MaxForce;
        Force = saveData.Force;
        favorability = saveData.favorability;
        favorabilityLevel = saveData.favorabilityLevel;
        CharacterValueArray = saveData.CharacterValueArray;

        // maybe need save the data, because excel maybe use by random value
        CanDelete = excelCharacterData.canDeleted;
        CaptureProbability = excelCharacterData.captureProbalilty;
        RecruitCost = excelCharacterData.recruitCost;
        characterName = excelCharacterData.characterName;
        SetCharacterType(excelCharacterData.characterType);

        Tag = (CharacterTag)saveData.tag;

        for (int i = 0; i < saveData.currentSkillIDs.Count; i++)
        {
            int id = saveData.currentSkillIDs[i];
            Skill matchedSkill = allSkills.Find(skill => skill.ID == id);
            skills.Add(matchedSkill);
        }

        for (int i = 0; i < saveData.skillLibraryIDs.Count; i++)
        {
            int id = saveData.skillLibraryIDs[i];
            Skill matchedSkill = allSkills.Find(skill => skill.ID == id);
            skillLibrary.Add(matchedSkill);
        }

    }

    public Character(ExcelCharacterData excelCharacterData,CharacterCategory Category)
    {
        GenerateCharacterKey(excelCharacterData, Category);
        List<Skill> allSkills = GameValue.Instance.GetSkillList();
        star = false;
        IsMoved = false;
        characterID = excelCharacterData.ID;
        this.Category = Category;

        SetCountry(excelCharacterData.country);
        roleClass = (CharacterRole)excelCharacterData.roleClass;

        SetItem(GameValue.Instance.GetItem(excelCharacterData.itemWithCharacter));
        battleMoveNum = excelCharacterData.moveNum;

        CanDelete = excelCharacterData.canDeleted;
        isPersonBattle = excelCharacterData.isPersonBattle;

        SetInitLevel(excelCharacterData);

        MaxForce = excelCharacterData.maxForce;
        Force = excelCharacterData.force;
        favorability = excelCharacterData.favorability;
        favorabilityLevel = (FavorabilityLevel)excelCharacterData.favorabilityLevel;
        CaptureProbability = excelCharacterData.captureProbalilty;
        RecruitCost = excelCharacterData.recruitCost;


        List<int> values = excelCharacterData.initValues;
        CharacterValueArray = new int[3, 5]  
        {
            {values[0], values[1], values[2], values[3],values[4]},
            {values[5], values[6], values[7], values[8], values[9]},
            {values[10], values[11], values[12], values[13], values[14]} 
        };


        characterName = excelCharacterData.characterName;

        SetCharacterType(excelCharacterData.characterType);
        Tag = 0;

        for (int i = 0; i < excelCharacterData.initSkillIDs.Count; i++)
        {
            int id = excelCharacterData.initSkillIDs[i];
            Skill matchedSkill  = allSkills.Find(skill => skill.ID == id);
            skills.Add(matchedSkill);
        }

        for (int i = 0; i < excelCharacterData.initSkillLibraryIDs.Count; i++)
        {
            int id = excelCharacterData.initSkillIDs[i];
            Skill matchedSkill = allSkills.Find(skill => skill.ID == id);
            skillLibrary.Add(matchedSkill);
        }

    }

    void GenerateCharacterKey(ExcelCharacterData excelCharacterData, CharacterCategory Category)
    {
        if (excelCharacterData.CharacterKey != CharacterConstants.Random)
        {
            characterKey = excelCharacterData.CharacterKey;
        }
        else
        {
            string newKey;
            do
            {
                int randValue = UnityEngine.Random.Range(0, 1000000); 
                newKey = $"{CharacterConstants.Random}_{Category}_{excelCharacterData.ID}_{randValue}";
            }
            while (GameValue.Instance.GetCharacterByKey(newKey) != null);

            characterKey = newKey;
        }
    }


    void SetInitLevel(ExcelCharacterData excelCharacterData)
    {
        if (excelCharacterData.maxLevel == 0)
        {
            baseMaxLevel = int.MaxValue;
            CurrentLevel = excelCharacterData.currentLevel;
        } else if (excelCharacterData.maxLevel < 0)
        {
            int RanMaxLevel = UnityEngine.Random.Range(0,Mathf.Abs(excelCharacterData.maxLevel));
            baseMaxLevel = RanMaxLevel;
            int RanCurrentLevel = UnityEngine.Random.Range(0, baseMaxLevel);
            currentLevel = RanCurrentLevel;
        } else
        {
            baseMaxLevel = excelCharacterData.maxLevel;
            CurrentLevel = excelCharacterData.currentLevel;
        }

        health = GetMaxHealth();
    }


    public void SetCharacterType(string type)// or can change
    {
        this.type = type;
        icon =  GetSprite.GetCharacterIcon(GetCharacterKey(), type);
        image = GetSprite.GetCharacterImage(GetCharacterKey(), type);
        Debug.Log(image);
        OnCharacterChanged?.Invoke();
    }



    public void SetStar()
    {
        Star = !Star;
    }


    public void SetPlayerName(string playerName)
    {
        if (type != CharacterConstants.PlayerType)
            return;

        if (characterName == null || characterName.Count == 0)
            return;

        foreach (var key in characterName.Keys.ToList())
        {
            characterName[key] = playerName;
        }
    }

    public void SetCharacterName(string newName)
    {
        List<string> keys = new List<string>(characterName.Keys);
        foreach (string key in keys)
        {
            characterName[key] = newName;
        }
        OnCharacterChanged?.Invoke();
        Debug.Log($"{newName} set");
    }


    public void SetCountry(string newCountry)
    {
        GameValue.Instance.GetCountryManager().AddCharacter(newCountry, this);
        country = newCountry;
    }

    public string GetCountryENName()
    {
        return country;
    }



    public void SetMultipleData(MultiplePlaythroughsGameCharacterRowControlData data)
    {
        Star = data.Star;
        Experience = data.Experience;
        CurrentLevel = data.CurrentLevel;
        baseMaxLevel = data.BaseMaxLevel;
        MaxForce = data.MaxForce;
        Force = data.Force;
        Favorability = data.Favorability;   
        FavorabilityLevel = data.FavorabilityLevel;
        CharacterValueArray = data.CharacterValueArray;
        SetItem(GameValue.Instance.GetItem(data.ItemID));
        BattleMoveNum = data.BattleMoveNum;
        Tag = data.Tag;
        RoleClass = data.RoleClass;
        //SetCharacterType(data.CharacterType);
    }


    public void SetItem(ItemBase item)
    {

        if (item != null && item.TriggerEffect(ItemType.OneTime))
        {
            item.UseOneTimeEffect(this);
            OnCharacterChanged?.Invoke();
            return;
        }

        GameValue.Instance.GetItem(itemID)?.RemoveCharacter(this);
        if (item != null){
            item.RecordCharacter(this);
            item.UseEffect(this);
        }
        OnCharacterChanged?.Invoke();
    }

    public bool HasItem()
    {
        return itemID != ItemConstants.NoneID;
    }


    public string GetCharacterENName()
    {
        return characterName[LanguageCode.EN];
    }

    public void SetBattlePositionToCharacter(BattlePosition newBattlePosition)
    {
        if (battlePosition != null) { battlePosition.ClearPosition(); }
        battlePosition = newBattlePosition;
        OnCharacterChanged?.Invoke();

    }

    public bool IsAtBattlePosition()
    {
        return battlePosition != null;
    }
    public int GetLevelConstant()
    {
        return MathTool.LevelAndForceMapValue(currentLevel);//currentLevel * GameValueConstants.levelConstants;
    }



    public int GetMaxHealth()
    {
        return GetLevelConstant();
    }

    public string GetMaxHealthString()
    {
        return GetMaxHealth().ToString();
    }

    public float GetHealthAndMaxHealthRate()
    {
        return (float)Health / GetMaxHealth();
    }

    public string GetHealthAndMaxHealthString()
    {
        return GetValueColorString($"{FormatNumberToString(Health)} / {FormatNumberToString(GetMaxHealth())}", ValueColorType.Pop);
    }


    public Dictionary<string,string> GetCharacterNameDictionary()
    {
        return characterName;
    }


    public string GetCharacterName()
    {
        string currentLanguage = LocalizationSettings.SelectedLocale.Identifier.Code;
        return characterName.TryGetValue(currentLanguage, out var text) ? text : characterName[LanguageCode.EN];
    }
    public string GetCharacterKey()
    {
        return characterKey;
    }
    public int GetCharacterID()
    {
        return characterID;
    }
    public void RandomValue()
    {
        int randomName = UnityEngine.Random.Range(1, 999);
        List<string> keys = new List<string>(characterName.Keys);
        foreach (string key in keys)
        {
            characterName[key] += randomName;
        }


        int randomP = UnityEngine.Random.Range(-500, 500);

        randomP = UnityEngine.Random.Range(-2, 2);
        MakeSureDontLessNumInt(ref CharacterValueArray[0,0], randomP,1);

        randomP = UnityEngine.Random.Range(-2, 2);
        MakeSureDontLessNumInt(ref CharacterValueArray[0,1], randomP,1);

        randomP = UnityEngine.Random.Range(-2, 2);
        MakeSureDontLessNumInt(ref CharacterValueArray[0,2], randomP,0);

        randomP = UnityEngine.Random.Range(-2, 2);
        MakeSureDontLessNumInt(ref CharacterValueArray[0,3], randomP,1);

        randomP = UnityEngine.Random.Range(-2, 2);
        MakeSureDontLessNumInt(ref CharacterValueArray[0,4], randomP, 1);

        randomP = UnityEngine.Random.Range(-2, 2);
        MakeSureDontLessNumInt(ref battleMoveNum, randomP,1);
    }
    private void MakeSureDontLessNumInt(ref int value, int random,int num)
    {
        if (value + random <= num)
        {
            value = num;
        } else
        {
            value += random;
        }

    }

    public int GetBaseMaxLevel()
    {
        return baseMaxLevel;
    }

    public ItemBase GetItem()
    {
        if (HasItem())
        {
            return GameValue.Instance.GetItem(ItemID);
        }
        else
        {
            return null;

        }
    }

    public Sprite GetItemWithCharacterSprite()
    {
        if (HasItem())
        {
            return GameValue.Instance.GetItem(ItemID).icon;
        }
        else
        {
            return Resources.Load<Sprite>("MyDraw/Item/EmptyItem");

        }
    }

    public int TotalSkillNum()
    {
        return skillLibrary.Count;
    }
    public string TotalSkillName(int index)
    {
        return skillLibrary[index].GetSkillENName();
    }

    public Skill TotalSkill(int index)
    {
        return skillLibrary[index];
    }


    public string CurrentSkillName(int index)
    {
        if (skills[index] != null) return skills[index].GetSkillName();
        return "NONE";
    }
    public Skill GetSkill(int index)
    {
        if (skills[index] != null) return skills[index];
        return null;
    }

    public int GetSkillID(int index)
    {
        if (skills[index] != null) return skills[index].ID;
        return -1;
    }

    public List<Skill> GetSkills()
    {
        return skills;
    }

    public List<Skill> GetSkillLibrary()
    {
        return skillLibrary;
    }


    public int[,] GetCharacterValueArray()
    {
        return CharacterValueArray;
    }
    public int GetMaxLimit(){
       // return (int)(leadership + (double)charm/100) * 100; 
        return GetValue(2,0) * 100 + GetValue(2, 4); 
    }

    public float GetHelplerValue(int index)
    {
        // return (int)(leadership + (double)charm/100) * 100; 
        return GetValue(2, index) + (float)GetValue(2, 4) / 10;
    }

    public void AddFavorabilityValue(int value)
    {
        favorability += value;
    }
    public void SetValue(int valueType, int index, int NewValue)
    {

        CharacterValueArray[valueType,index] = NewValue;
        OnCharacterChanged?.Invoke();

    }
    public void AddValue(int valueType, int index, int AddValue)
    {
        CharacterValueArray[valueType,index] += AddValue;
        OnCharacterChanged?.Invoke();

    }
    public int GetValue(int valueType,int index)
    {
        int skillOffest = 0;
        foreach (var skill in skills)
        {
            if (skill != null && skill.triggerType == SkillTriggerType.Passive )
            {
               skillOffest += skill.GetPassiveBonus(valueType, index);
            }
        }

        if (HasItem()) {
            return CharacterValueArray[valueType, index] + AddItemValue(valueType, index) + skillOffest; 
        }
        else
        {
            return CharacterValueArray[valueType, index] + skillOffest;
        }
    }
    int AddItemValue(int ValueType, int index)
    {
        if (!HasItem()) return 0;

        return GameValue.Instance.GetItem(itemID).UseItemValue(ValueType, index);
    }
    public CharacterCategory GetCategory()
    {
        return Category;
    }

    public string GetCharacterFileType()
    {
        return type;
    }

    public bool HasUnlimitedLevel()
    {
        return baseMaxLevel == int.MaxValue;
    }

    public int GetMaxLevel()
    {
        return baseMaxLevel;
    }

    public string GetLvAndMaxLevelString()
    {
        return GetString.GetLvAndMaxLevelString(CurrentLevel, baseMaxLevel);
    }


    public int GetRequiredExpToLvUp()
    {
        return GameCalculate.GetRequiredExpToLvUp(CurrentLevel, GetMaxLevel());
    }


    public float GetExpRate()
    {
        return (float)Experience / GetRequiredExpToLvUp();
    }

    public string GetRequiredExpToLvUpString()
    {
        return FormatNumberToString(GetRequiredExpToLvUp());
    }

    public string GetExpAndReqExpString()
    {
        return GetString.GetExpAndReqExpString(Experience, GetRequiredExpToLvUp());
    }


    public void AddExp(int addExp)
    {
        experience += addExp;

        while ((HasUnlimitedLevel() || CurrentLevel < GetMaxLevel()) &&
               experience >= GetRequiredExpToLvUp())
        {
            experience -= GetRequiredExpToLvUp();
            currentLevel++;
        }

        if (!HasUnlimitedLevel() && CurrentLevel >= GetMaxLevel())
        {
            experience = 0;
        }
    }

    public int GetCurrentLevel()
    {
        return CurrentLevel;
    }

    public int GetExperience()
    {
        return Experience;
    }



    #region CharacterEND

    public void AddCharacterEND(int ENDID, CharacterEND characterEND)
    {
        characterENDs.Add(ENDID, characterEND);
    }

    public CharacterEND GetPlayerEND(bool isGE)
    {
        if (isGE)
        {
            return characterENDs[2];
        }
        else
        {
            return characterENDs[1];
        }
    }
    public CharacterEND GetCharacterEND(GameValue gameValue)
    {
        if (gameValue.IsLastJudgmentHappen()) return characterENDs[0];


        switch (characterID)
        {
            case 1: return EmiliaEND(gameValue);
            case 15: return JaneEND(gameValue);
            case 23: return HannaEND(gameValue);
            default: return null;
        }
    }

    CharacterEND EmiliaEND(GameValue gameValue)
    {
        int ENDp = 3;
        if (!(gameValue.GetCharacterByKey(CharacterConstants.PlayerKey).country == CharacterConstants.DIE))
        {

            return characterENDs[ENDp];
        } else
        {
            return characterENDs[ENDp+1];

        }
    }
    CharacterEND HannaEND(GameValue gameValue)
    {
        int ENDp = 5;
        return characterENDs[ENDp];


        /*   int ENDp = 5;
           if ((gameValue.GetHannaGold() >= 10000000000L) && favorabilityLevel == 2)
           {
               return characterENDs[ENDp];
           }
           else if (favorabilityLevel == 2)
           {
               return characterENDs[ENDp+1];

           }
           else if (gameValue.GetHannaGold() >= 10000000000L)
           {
               return characterENDs[ENDp+2];

           }
           else
           {
               return characterENDs[ENDp+3];
           }*/
    }
    CharacterEND JaneEND(GameValue gameValue)
    {
        int ENDp = 9;
        if ((country == gameValue.GetPlayerCountryENName()) && favorabilityLevel == FavorabilityLevel.Romance)
        {

            return characterENDs[ENDp];
        } else if ((country == gameValue.GetPlayerCountryENName()))
        {

            return characterENDs[ENDp+1];
        }
        else
        {
            return characterENDs[ENDp + 2];

        }
    }



    #endregion

    public void SetCapture()
    {
        SetCountry(CharacterConstants.Capture);
        IsMoved = true;
        MaxForce = 1;
        Force = 1;
    }

    public bool IsHaveEND()
    {
        return characterENDs.Count > 0;
    }

     public bool HasLord()
    {
        return (lordRegions.Count != 0);
    }

    public bool CanMove()
    {
        return (!HasLord()) && !IsMoved;
    }

    public bool CheckCanMove()
    {
        if (IsMoved)
        {
           // NotificationManage.Instance.ShowAtTop("Character is Moved");
           NotificationManage.Instance.ShowAtTopByKey(NotificationKeyConstants.Character_Moved, GetCharacterName());
            return false;
        }
        else if (HasLord())
        {
            NotificationManage.Instance.ShowAtTopByKey(NotificationKeyConstants.Character_IsLord,GetCharacterName());
            //NotificationManage.Instance.ShowAtTop("Character is lord");
            return false;
        }

        return true;

    }


    public RegionValue GetLordRegion(int index)
    {
        if (index > lordRegions.Count) return null;


        return lordRegions[index];
    }




    public void SetLordRegion(RegionValue regionValue)
    {
        RemoveLord();
        regionValue.lord = this;
        lordRegions.Add(regionValue);
        OnCharacterChanged?.Invoke();
    }



    public void RemoveLord()
    {
        foreach (var region in lordRegions)
        {
            region.lord = null;
        }

        lordRegions.Clear();
        OnCharacterChanged?.Invoke();

    }

    public void AddLordRegion(RegionValue regionValue)
    {
        lordRegions.Add(regionValue);
        regionValue.lord = this;
        OnCharacterChanged?.Invoke();
    }


    public string GetClassRoleString()
    {
        return GetString.GetClassRoleString(roleClass);
    }


    public bool IsImportant()
    {
        return Category == CharacterCategory.Important;
    }

}

public class CharacterEND
{
    private int ENDID;
    private int characterID;
    private string characterENDIcon;
    private bool isGE;
    private bool isTE;
    private Dictionary<string, string> Contents = new();

    public int GetCharacterID()
    {
        return characterID;
    }

    public bool IsGE()
    {
        return isGE;
    }

    public bool IsTE()
    {
        return isTE;
    }

    public string GetENDIcon()
    {
        return characterENDIcon;
    }

    public int GetENDID()
    {
        return ENDID;
    }

    public string GetContent()
    {
        string currentLanguage = LocalizationSettings.SelectedLocale.Identifier.Code;

        return Contents.TryGetValue(currentLanguage, out var text) ? text : Contents["en"];
    }

    public CharacterEND(ExcelCharacterENDData ENDData)
    {
        this.ENDID = ENDData.ENDID;
        characterID = ENDData.CharacterID;
        isGE = ENDData.isGE;
        isTE = ENDData.isTE;
        characterENDIcon = ENDData.CharacterENDIcon;
        Contents = ENDData.ENDContent;
    }



}


public class CharacterSaveData
{
    public int characterID;
    public string characterKey;
    public string characterName;
    public bool star;
    public string country;
    public int tag;
    public CharacterCategory Category;
    public int itemWithCharacterID;
    public int roleClass; 
    public bool isMove;
    public int CaptureProbability;
    public int RecruitCost;
    public int MaxForce;
    public int Force;
    public int battleMoveNum;
    public bool isPersonBattle;
    public int CurrentLevel;
    public int baseMaxLevel;
    public int Experience;
    public int health;
    public int favorability;
    public FavorabilityLevel favorabilityLevel;
    public int[,] CharacterValueArray = new int[3, 5];
    public List<int> currentSkillIDs = new List<int>();
    public List<int> skillLibraryIDs = new List<int>();

    public string type;

    public CharacterSaveData(Character character)
    {
        if (character == null)
        {
            return;
        }
     //   UnityEngine.Debug.Log(characterID + character.GetCharacterENName());
        characterID = character.GetCharacterID();
        characterName = character.GetCharacterENName();
        characterKey = character.GetCharacterKey();
        star = character.Star;
        country = character.GetCountryENName();

        Category = character.GetCategory();
        type = character.Type;
        tag = (int)character.Tag;

        itemWithCharacterID = character.ItemID;

        roleClass = (int)character.RoleClass;
        isMove = character.IsMoved;


         MaxForce = character.MaxForce;
         Force = character.Force;
         battleMoveNum = character.BattleMoveNum;

         isPersonBattle = character.IsPersonBattle;
         CurrentLevel = character.GetCurrentLevel();
        Experience = character.GetExperience();
        baseMaxLevel = character.GetBaseMaxLevel();

        health = character.Health;

         favorability = character.Favorability;

         favorabilityLevel = character.FavorabilityLevel;

        CharacterValueArray = character.GetCharacterValueArray();


        for (int i = 0; i < 5; i++)
        {
            currentSkillIDs.Add(character.GetSkillID(i));

        }

        for (int i = 0; i < character.TotalSkillNum(); i++)
        {
            skillLibraryIDs.Add(character.TotalSkill(i).ID);
        }

    }

}