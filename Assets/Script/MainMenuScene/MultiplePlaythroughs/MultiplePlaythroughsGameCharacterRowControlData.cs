using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static FormatNumber;
public class MultiplePlaythroughsGameCharacterRowControlData
{

    #region Parameter
    private CharacterRole roleClass;

    public CharacterRole RoleClass
    {
        get => roleClass;
        set => SetProperty(ref roleClass, value);
    }


    private int baseMaxLevel = 0;
    public int BaseMaxLevel
    {
        get => baseMaxLevel;
        set => SetProperty(ref baseMaxLevel, value);
    }


    private bool isPersonBattle = false;
    public bool IsPersonBattle
    {
        get => isPersonBattle;
        set => SetProperty(ref isPersonBattle, value);
    }


    private int currentLevel = 0;
    public int CurrentLevel
    {
        get => currentLevel;
        set => SetProperty(ref currentLevel, value);
    }

    public int experience = 0;
    public int Experience
    {
        get => experience;
        set => SetProperty(ref experience, value);
    }

    private bool star;
    public bool Star
    {
        get => star;
        set => SetProperty(ref star, value);
    }

    private int maxForce;
    public int MaxForce
    {
        get => maxForce;
        set => SetProperty(ref maxForce, value);
    }

    private int force;

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

    int[,] characterValueArray = new int[3, 5]  // 3 ? 4 ?
    {
        {1, 2, 3, 4,5}, // battle // 0 is attack, 1 is defense, 2 is magic , 3 is speed, 4 is lucky 
        {5, 6, 7, 8, 5}, // Parameter // 0 is food, 1 is scienceP,2 is politics, 3 is gold , 4 is faith
        {9, 10, 11, 12, 5} //Help  // 0 is leadership, 1 is scout, 2 is build, 3 is negotiation, 4 is charm
    };

    public int[,] CharacterValueArray
    {
        get => characterValueArray;
        set => SetProperty(ref characterValueArray, value);
    }

    private List<Skill> skills = new List<Skill>();

    public List<Skill> Skills
    {
        get => skills;
        set => SetProperty(ref skills, value);
    }

    private List<Skill> skillLibrary = new List<Skill>();

    public List<Skill> SkillLibrary
    {
        get => skillLibrary;
        set => SetProperty(ref skillLibrary, value);
    }

    private CharacterTag tag;
    public CharacterTag Tag
    {
        get => tag;
        set => SetProperty(ref tag, value);
    }

    private string characterType;
    public string CharacterType
    {
        get => characterType;
        set => SetProperty(ref characterType, value);
    }

    Sprite icon;
    Sprite image;


    private int itemID = ItemConstants.NoneID;
    public int ItemID
    {
        get => itemID;
        set => SetProperty(ref itemID, value);
    }

    public event Action OnCharacterRowControlDataChanged;

    protected bool SetProperty<T>(ref T field, T value)
    {
        if (!EqualityComparer<T>.Default.Equals(field, value))
        {
            field = value;
            OnCharacterRowControlDataChanged?.Invoke();
            return true;
        }
        return false;
    }


    public Character character;

    #endregion

    public MultiplePlaythroughsGameCharacterRowControlData(Character character)
    {
        if (character == null)
        {
            Debug.LogError("Character is null in MultiplePlaythroughsGameCharacterRowControlData constructor!");
            return;
        }

        this.character = character;
        RestData();
    }


    public void RestData()
    {
        RoleClass = character.RoleClass;
        ItemID = character.ItemID;

        BaseMaxLevel = character.GetMaxLevel();
        IsPersonBattle = character.IsPersonBattle;
        CurrentLevel = character.GetCurrentLevel();
        Experience = character.GetExperience();
        Tag = character.Tag;
        Star = character.Star;
        MaxForce = character.MaxForce;
        Force = character.Force;
        BattleMoveNum = character.BattleMoveNum;
        Health = character.Health;
        Favorability = character.Favorability;
        FavorabilityLevel = character.FavorabilityLevel;
        CharacterType = character.GetCharacterKey();
        image = character.image;
        icon = character.icon;

        characterValueArray = new int[3, 5]  // 3 ? 4 ?
            {
                {character.GetValue(0,0), character.GetValue(0,1), character.GetValue(0,2), character.GetValue(0,3), character.GetValue(0,4)},
                {character.GetValue(1,0), character.GetValue(1,1), character.GetValue(1,2), character.GetValue(1,3), character.GetValue(1,4)},
                {character.GetValue(2,0), character.GetValue(2,1), character.GetValue(2,2), character.GetValue(2,3), character.GetValue(2,4)},
            };
        Skills = character.GetSkills();
        SkillLibrary = character.GetSkillLibrary();

        OnCharacterRowControlDataChanged?.Invoke();

    }


    public bool HasItem()
    {
        return ItemID != ItemConstants.NoneID;
    }

    public int GetCharacterID()
    {
        return character.GetCharacterID();
    }

    public String GetCharacterName()
    {
        return character.GetCharacterName();
    }

    public Sprite GetIcon()
    {
        return icon;
    }

    public Sprite GetImage()
    {
        return image;
    }


    public int GetMaxLimit()
    {
        return GetValue(2, 0) * 100 + GetValue(2, 4);
    }



    public int GetValue(ValueType type)
    {
        switch (type)
        {
            case ValueType.Attack: return GetValue(0, 0);
            case ValueType.Defense: return GetValue(0, 1);
            case ValueType.Magic: return GetValue(0, 2);
            case ValueType.Speed: return GetValue(0, 3);
            case ValueType.Lucky: return GetValue(0, 4);


            case ValueType.FoodP: return GetValue(1, 0);
            case ValueType.ScienceP: return GetValue(1, 1);
            case ValueType.PoliticsP: return GetValue(1, 2);
            case ValueType.GoldP: return GetValue(1, 3);
            case ValueType.FaithP: return GetValue(1, 4);

            case ValueType.Leadership: return GetValue(2, 0);
            case ValueType.Scout: return GetValue(2, 1);
            case ValueType.Build: return GetValue(2, 2);
            case ValueType.Negotiation: return GetValue(2, 3);
            case ValueType.Charm: return GetValue(2, 4);

            case ValueType.Move: return BattleMoveNum;
            case ValueType.Force: return Force;
            case ValueType.MaxForce: return MaxForce;

            default: return -1;
        }
    }


    public int GetValue(int valueType, int type)
    {
        ValueType valueTypeIndex = ValueType.Attack;
        switch (type)
        {
            case 0: valueTypeIndex = ValueType.Attack; break;
            case 1: valueTypeIndex = ValueType.Defense; break;
            case 2: valueTypeIndex = ValueType.Magic; break;
            case 3: valueTypeIndex = ValueType.Speed; break;
            case 4: valueTypeIndex = ValueType.Lucky; break;
        }

        return GetValue(valueType, valueTypeIndex);
    }


    public int GetValue(int valueType, ValueType type)
    {
        int skillOffest = 0;
        int index = ValueTypeToIndex(type);
        foreach (var skill in skills)
        {
            if (skill != null && skill.triggerType == SkillTriggerType.Passive)
            {
                skillOffest += skill.GetPassiveBonus(valueType, index);
            }
        }

        if (HasItem())
        {
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

    public string GetHealthAndMaxHealthString()
    {
        return GetString.GetHealthAndMaxHealthString(Health, GetMaxHealth());
    }

    public string GetClassRoleString()
    {
        return GetString.GetClassRoleString(roleClass);
    }

    public int GetMaxHealth()
    {
        return GetLevelConstant();
    }



    public bool HasUnlimitedLevel()
    {
        return baseMaxLevel == int.MaxValue;
    }

    public int GetMaxLevel()
    {
        return BaseMaxLevel;
    }

    public string GetLvAndMaxLevelString()
    {
        return GetString.GetLvAndMaxLevelString(currentLevel,baseMaxLevel);
    }


    int GetRequiredExpToLvUp()
    {
        return GameCalculate.GetRequiredExpToLvUp(CurrentLevel, GetMaxLevel());
    }


    public string GetCurrentExpString()
    {
        return FormatNumberToString(Experience);
    }


    public float GetExpRate()
    {
        if (CurrentLevel == BaseMaxLevel) return 1;
        return (float)Experience / GetRequiredExpToLvUp();
    }

    public void AddMaxLevel(int amount)
    {
        int NewLevel = BaseMaxLevel + amount;

        if (NewLevel <= CurrentLevel)
        {
            CurrentLevel = NewLevel;
            Experience = 0;
        }

        BaseMaxLevel += amount;
        Health = GetMaxHealth();
    }


    public void AddExperience(int amount)
    {
        Experience += amount;

        while (Experience >= GetRequiredExpToLvUp())
        {
            if (HasUnlimitedLevel()) break;
            if (CurrentLevel >= GetMaxLevel())
            {
                Experience = GetRequiredExpToLvUp();
                break;
            }

            Experience -= GetRequiredExpToLvUp();
            CurrentLevel++;
        }

        while (Experience < 0 && CurrentLevel > 1)
        {
            CurrentLevel--;
            Experience += GetRequiredExpToLvUp();
        }

        if (CurrentLevel <= 1 && Experience < 0)
        {
            Experience = 0;
        }
        Health = GetMaxHealth();
        OnCharacterRowControlDataChanged?.Invoke();
    }



    string GetRequiredExpToLvUpString()
    {
        return FormatNumberToString(GetRequiredExpToLvUp());
    }

    public string GetExpAndReqExpString()
    {
        return $"{GetCurrentExpString()} / {GetRequiredExpToLvUpString()}";
    }


    public int GetLevelConstant()
    {
        return MathTool.LevelAndForceMapValue(CurrentLevel);
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



    public void AddValue(ValueType type, int amount)
    {
        int valueType = 0, index = 0;
        switch (type)
        {
            case ValueType.Attack : valueType = 0; index = 0; break;
            case ValueType.Defense: valueType = 0; index = 1; break;
            case ValueType.Magic: valueType = 0; index = 2; break;
            case ValueType.Speed: valueType = 0; index = 3; break;
            case ValueType.Lucky: valueType = 0; index = 4; break;

            case ValueType.FoodP: valueType = 1; index = 0; break;
            case ValueType.ScienceP: valueType = 1; index = 1; break;
            case ValueType.PoliticsP: valueType = 1; index = 2; break;
            case ValueType.GoldP: valueType = 1; index = 3; break;
            case ValueType.FaithP: valueType = 1; index = 4; break;

            case ValueType.Leadership: valueType = 2; index = 0; break;
            case ValueType.Scout: valueType = 2; index = 1; break;
            case ValueType.Build: valueType = 2; index = 2; break;
            case ValueType.Negotiation: valueType = 2; index = 3; break;
            case ValueType.Charm: valueType = 2; index = 4; break;
        }

        var newArray = (int[,])characterValueArray.Clone();
        newArray[valueType, index] += amount;
        CharacterValueArray = newArray;
    }






    public void AddValue(int valueType, ValueType type, int amount)
    {
        int index = ValueTypeToIndex(type);
        var newArray = (int[,])characterValueArray.Clone();
        newArray[valueType, index] += amount;
        CharacterValueArray = newArray;
    }


    int ValueTypeToIndex(ValueType valueType)
    {
        int index = 0;
        switch (valueType)
        {
            case ValueType.Attack: index = 0; break;
            case ValueType.Defense: index = 1; break;
            case ValueType.Magic: index = 2; break;
            case ValueType.Speed: index = 3; break;
            case ValueType.Lucky: index = 4; break;
        }
        return index;
    }


    public bool IsImportant()
    {
        return character.IsImportant();
    }

    public MultiplePlaythroughsGameCharacterRowControlSaveData GetSaveData()
    {
        MultiplePlaythroughsGameCharacterRowControlSaveData save = new MultiplePlaythroughsGameCharacterRowControlSaveData();
        save.RoleClass = RoleClass;
        save.BaseMaxLevel = BaseMaxLevel;
        save.IsPersonBattle = IsPersonBattle;
        save.CurrentLevel = CurrentLevel;
        save.Experience = Experience;
        save.Star = Star;
        save.MaxForce = MaxForce;
        save.Force = Force;
        save.BattleMoveNum = BattleMoveNum;
        save.Health = Health;
        save.Favorability = Favorability;
        save.FavorabilityLevel = FavorabilityLevel;
        save.CharacterValueArray = CharacterValueArray;
        save.SkillIDs = Skills.Select(s => s.ID).ToList();
        save.SkillLibraryIDs = SkillLibrary.Select(s => s.ID).ToList();
        save.Tag = Tag;
        save.CharacterType = CharacterType;
        save.ItemID = ItemID;
        save.CharacterKey = character.GetCharacterKey();
        return save;
    }

    public void SetSaveData(MultiplePlaythroughsGameCharacterRowControlSaveData save)
    {
        RoleClass = save.RoleClass;
        BaseMaxLevel = save.BaseMaxLevel;
        IsPersonBattle = save.IsPersonBattle;
        CurrentLevel = save.CurrentLevel;
        Experience = save.Experience;
        Star = save.Star;
        MaxForce = save.MaxForce;
        Force = save.Force;
        BattleMoveNum = save.BattleMoveNum;
        Health = save.Health;
        Favorability = save.Favorability;
        FavorabilityLevel = save.FavorabilityLevel;

        CharacterValueArray = save.CharacterValueArray;

        Skills = new List<Skill>();
        SkillLibrary = new List<Skill>();

        foreach (var skillID in save.SkillLibraryIDs)
        {
            var skill = GameValue.Instance.GetSkill(skillID);
            if (skill != null)
                SkillLibrary.Add(skill);
        }

        foreach (var skillID in save.SkillIDs)
        {
            var skill = SkillLibrary.Find(s => s.ID == skillID);
            if (skill != null)
                Skills.Add(skill);
            else
                Debug.LogWarning($"[SetSaveData] Skill ID {skillID} not found in SkillLibrary!");
        }

        Tag = save.Tag;
        CharacterType = save.CharacterType;
        ItemID = save.ItemID;
    }



}


[System.Serializable]
public class MultiplePlaythroughsGameCharacterRowControlSaveData
{
    public string CharacterKey;
    public CharacterRole RoleClass;
    public int BaseMaxLevel;
    public bool IsPersonBattle;
    public int CurrentLevel;
    public int Experience;
    public bool Star;
    public int MaxForce;
    public int Force;
    public int BattleMoveNum;
    public int Health;
    public int Favorability;
    public FavorabilityLevel FavorabilityLevel;
    public int[,] CharacterValueArray;
    public List<int> SkillIDs;
    public List<int> SkillLibraryIDs;
    public CharacterTag Tag;
    public string CharacterType;
    public int ItemID;
}
