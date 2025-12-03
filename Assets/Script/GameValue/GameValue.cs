using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

using static ExcelReader;
using static GetColor;
using static GetString;
using UnityEngine.TextCore.Text;

#region Value
public static class GameValueConstants
{
    public const string Country_Prologue = CountryConstants.BURGUNDY;
    public const string Country_Opening = CountryConstants.HOLY_ROMULUS_EMPIRE;

    public const Season Season_Prologue = Season.Autumn;
    public const int Year_Prologue = 1441;

    public const Season Season_Opening = Season.Spring;
    public const int Year_Opening = 1442;

    public const int Year_Midgame = 1447;
    public const int Year_Climax = 1453;      
    public const int Year_End = 1455;         
    public const Season Season_End = Season.Spring;

    //public const int levelConstants = 100;
    public const int maxValueLimit = 100;

    public const DifficultyGame DefaultGameDifficulty = DifficultyGame.Normal;



    public static float GetAchievementCoefficient(DifficultyGame difficultyGame)
    {
        switch (difficultyGame)
        {
            case DifficultyGame.Easy : return 0.75f;
            case DifficultyGame.Normal: return 1f;
            case DifficultyGame.Hard: return 1.25f;
            case DifficultyGame.Legendary: return 1.5f;
            default:
                Debug.LogError($"GetAchievementCoefficient{difficultyGame}");
                return 0f;
        }

    }
}

public enum DifficultyGame
{
    Easy, Normal,Hard, Legendary
}

#endregion

public class GameValue : MonoBehaviour
{
    public static GameValue Instance { get; private set; }
    ResourceValue resourceValue = new ResourceValue();
    PlayerState playerState = new PlayerState();
    BattleData CurrentBattle { get; set; }
    ExploreData CurrentExplore { get; set; }

    GameValueLibrary gameValueLibrary;
    private List<ItemBase> allItems = new List<ItemBase>();
    private List<int> CurrentItemsInStore = new List<int>(); 
    public event Action ItemsChange;
    CountryManager countryManager = new CountryManager();
    StoryControl storyControl = new StoryControl();

    BattleData battleData;
    ExploreData exploreData;
    BuildData buildData;
    public DifficultyGame currentDifficultyGame = GameValueConstants.DefaultGameDifficulty;
    public List<StoryCharacterImageDataBase> allCharacterDBs = new List<StoryCharacterImageDataBase>();


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        InitGameValue();


        GetComponent<GameValueTest>().SetTestValue(this);

#if UNITY_EDITOR
                GetComponent<GameValueTest>().SetTestValue(this);
#endif

    }


#if UNITY_EDITOR
    [ContextMenu("Refresh Character DBs")]
    public void RefreshCharacterDBs()
    {
        allCharacterDBs.Clear();
        string[] guids = AssetDatabase.FindAssets("t:StoryCharacterImageDataBase", new string[] { "Assets/ScriptableObject/CharacterImage" });
        foreach (var guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            var db = AssetDatabase.LoadAssetAtPath<StoryCharacterImageDataBase>(path);
            if (db != null)
                allCharacterDBs.Add(db);
        }

        Debug.Log($"[Editor Mode] Refreshed {allCharacterDBs.Count} Character DBs");
    }
#endif


    private void Start()
    {
    }

    void InitGameValue()
    {
        gameValueLibrary = new GameValueLibrary();
        Init();
    }


    public void Init()
    {
        countryManager.Init();
        storyControl.Init();
        allItems = gameValueLibrary.GetInitItemBaseList();
        gameValueLibrary.GenerateInitAllImportantCharacters();
        gameValueLibrary.GenerateInitRegionValue();
    }


    #region Resource

    public void AddResource(ValueType type, float amount)
    {
        resourceValue.Add(type, amount);
    }


    public float GetResource(ValueType type)
    {
        return resourceValue.GetResourceValue(type);
    }


    #endregion
    #region PlayerState

    public void AddReason()
    {
        playerState.AddSeason();
    }

    public void RegisterOnPlayerStateChanged(Action callback)
    {
        playerState.OnPlayerStateChanged += callback;
    }

    public void UnregisterOnPlayerStateChanged(Action callback)
    {
        playerState.OnPlayerStateChanged -= callback;
    }
    #endregion
    #region Item

    public List<ItemBase> GetAllItems()
    {
        return allItems;
    }

    public List<int> GetStoreSellItems()
    {
        return CurrentItemsInStore;
    }

    public List<ItemBase> GetStoreCanSellItem()
    {
        List<ItemBase> CanSellItems = new List<ItemBase>();
        foreach (ItemBase item in allItems)
        {
            if (item.CanSell())
            {
                CanSellItems.Add(item);
            }
        }
        return CanSellItems;
    }

    public List<ItemBase> GetPlayerItems()
    {
        List<ItemBase> playerItems = new List<ItemBase>();
        foreach (ItemBase item in allItems)
        {
            if (item.PlayerHas())
            {
                playerItems.Add(item);
            }
        }
        return playerItems;
    }

    public void AddSellItem(ItemBase itemBase)
    {
        CurrentItemsInStore.Add(itemBase.GetID());
    }

    #endregion
    #region Registering a Listener

    public void RegisterItemsChange(Action action)
    {
        ItemsChange += action;
    }

    public void UnRegisterItemsChange(Action action)
    {
        ItemsChange -= action;
    }

    public void OnItemsChange()
    {
        ItemsChange?.Invoke();
    }

    public void RegisterSBNResourceChanged(Action<string,double> callback)
    {
        resourceValue.OnSBNValueChanged += callback;
    }

    public void UnRegisterSBNResourceChanged(Action<string, double> callback)
    {
        resourceValue.OnSBNValueChanged -= callback;
    }


    public void RegisterResourceChanged(Action callback)
    {
        resourceValue.OnResourceChanged += callback;
    }

    public void UnRegisterResourceChanged(Action callback)
    {
        resourceValue.OnResourceChanged -= callback;
    }

    public void RegisterPlayerStateChanged(Action callback)
    {
        playerState.OnPlayerStateChanged += callback;
    }

    public void UnRegisterPlayerStateChanged(Action callback)
    {
        playerState.OnPlayerStateChanged -= callback;
    }

    public void RegisterPlayerCharacterChanged(Action callback)
    {
        RegisterCountryCharactersChanged(GetPlayerCountryENName(), callback);
    }

    public void UnRegisterPlayerCharacterChanged(Action callback)
    {
        UnRegisterCountryCharactersChanged(GetPlayerCountryENName(), callback);
    }

    public void RegisterPlayerRegionsChanged(Action callback)
    {
        RegisterCountryRegionsChanged(GetPlayerCountryENName(), callback);
    }

    public void UnRegisterPlayerRegionsChanged( Action callback)
    {
        UnRegisterCountryRegionsChanged(GetPlayerCountryENName(), callback);
    }


    public void RegisterCountryCharactersChanged(string CountryName,Action callback)
    {
        Country country = GetCountryManager().GetCountry(CountryName);
        if (country != null) country.OnCharactersChange += callback;
    }

    public void UnRegisterCountryCharactersChanged(string CountryName, Action callback)
    {
        Country country = GetCountryManager().GetCountry(CountryName);
        if (country != null) country.OnCharactersChange -= callback;
    }

    public void RegisterCountryRegionsChanged(string CountryName, Action callback)
    {
        Country country = GetCountryManager().GetCountry(CountryName);
        if (country != null) country.OnRegionsChange += callback;
    }

    public void UnRegisterCountryRegionsChanged(string CountryName, Action callback)
    {
        Country country = GetCountryManager().GetCountry(CountryName);
        if (country != null) country.OnRegionsChange -= callback;
    }

    #endregion
    #region TurnString

    public String GetTurnString()
    {
        string currentLanguage = LocalizationSettings.SelectedLocale.Identifier.Code;

        switch (currentLanguage)
        {
            case "en": return UpdateTurnTextEN();
            default: return UpdateTurnTextCHJA();
        }

        return "BUG IN GameValue GetTurnString";

    }

    string UpdateTurnTextEN()
    {
        string year = GetCurrentYear().ToString();
        string season = GetCurrentSeasonString();

        Color32 seasonColor = GetSeasonColor(GetCurrentSeason());
        string hexColor = ColorUtility.ToHtmlStringRGB(seasonColor);
        return $"{year} <color=#{hexColor}>{season}</color>";

    }

    string UpdateTurnTextCHJA()
    {
        string year = GetCurrentYear().ToString();
        int currentSeason = (int)GetCurrentSeason();

        string[] localizedSeasons = new string[4];

        for (int i = 0; i < 4; i++)
        {
            string seasonName = GetSeasonString((Season)i);
            if (i == currentSeason)
            {
                Color32 seasonColor = GetSeasonColor((Season)i);
                string hexColor = ColorUtility.ToHtmlStringRGB(seasonColor);
                localizedSeasons[i] = $"<b><color=#{hexColor}>{seasonName}</color></b>";
            }
            else
            {
                localizedSeasons[i] = seasonName;
            }
        }

        return $"{year} {localizedSeasons[0]}{localizedSeasons[1]}{localizedSeasons[2]}{localizedSeasons[3]}";
    }


    public string GetCurrentSeasonString()
    {
        return GetSeasonString(GetCurrentSeason());
    }


    #endregion
    #region Get


    //public SkillEffectRegistry GetSkillEffectRegistry()
    //{
    //    return skillEffectRegistry;
    //}

    public ExcelCharacterData GetCharacterExcelData(int CharacterID, CharacterCategory Category)
    {
        return gameValueLibrary.GetCharacterExcelData(CharacterID,Category);
    }

    public GameValueSaveData GetGameValueSaveData()
    {
        GameValueSaveData gameValueSaveData = new GameValueSaveData(this);
        return gameValueSaveData;
    }

    public StoryControl GetStoryControl()
    {
        return storyControl;
    }
    public PlayerState GetPlayerState()
    {
        return playerState;
    }
    public List<Character> GetPlayerCharacters()
    {
        return GetCountryManager().GetCountry(GetPlayerCountryENName()).GetCharacters();
    }

    public List<Character> GetCountryCharacters(string CountryName)
    {
        return GetCountryManager().GetCountry(CountryName).GetCharacters();
    }

    public Country GetCountry(string CountryName)
    {
        return GetCountryManager().GetCountry(CountryName);
    }

    public ResourceValue GetResourceValue()
    {
        return resourceValue;
    }


    public float GetTotalSupportRate()
    {
        float support = GetTotalSupportPopulation();
        float total = GetTotalPopulation();
        return support / total;
    }
    public int GetTotalSupportPopulation()
    {
        int TotalSupportPopulation = 0;
        foreach (var region in GetPlayerRegions())
        {
            TotalSupportPopulation += Mathf.RoundToInt(region.GetRegionPopulation() * region.GetRegionSupportRate());
        }

        return TotalSupportPopulation;
    }
    public int GetTotalPopulation()
    {
        int TotalPopulation = 0;
        foreach (var region in GetPlayerRegions())
        {
            TotalPopulation += region.GetRegionPopulation();
        }
        return TotalPopulation;
    }

    public int GetTotalRecruitedPopulation()
    {
        return resourceValue.TotalRecruitedPopulation;
    }


    public Color32 GetCountryColor(string countryName)
    {
        return GetCountryManager().GetCountryColor(countryName);
    }


    public List<Character> GetAllCharacters()
    {
        return GetCountryManager().GetAllCharacters();
    }

    public List<RegionValue> GetAllRegionValues()
    {
        return GetCountryManager().GetAllRegionValues();
    }

    public RegionValue GetRegionValue(int regionID)
    {
        return GetAllRegionValues().FirstOrDefault(c => c.GetRegionID() == regionID);
    }


    public Character GetCharacterByID(int ID)
    {
        return GetAllCharacters().FirstOrDefault(c => c.GetCharacterID() == ID);
    }



    public Character GetCharacterByKey(string CharacterKey)
    {
        if (CharacterKey == CharacterConstants.NoneKey)
        {
            return null;
        }

        return GetAllCharacters().FirstOrDefault(c => c.GetCharacterKey() == CharacterKey);
    }

    public Character GetCharacterByName(string name)
    {
        Character character = null;
        if (name == CharacterConstants.PlayerType)
        {
            character = GetCharacterByKey(CharacterConstants.PlayerKey);
        }
        else
        {
            character = GetAllCharacters().FirstOrDefault(c => c.GetCharacterENName() == name);
        }
        if (character == null) Debug.Log($"Cant found {name}");
        return character;
    }

    public CountryManager GetCountryManager()
    {
        return countryManager;
    }

    public List<RegionValue> GetPlayerRegions()
    {
        string playerCountry = GetPlayerCountryENName();
        List<RegionValue> playerRegions = GetCountryRegions(playerCountry);
        return playerRegions;
    }


    public List<RegionValue> GetCountryRegions(string CountryName)
    {
        List<RegionValue> CountryRegions = new List<RegionValue>();

        foreach (var region in GetAllRegionValues())
        {
            if (region.GetCountryENName() == CountryName)
            {
                CountryRegions.Add(region);
            }
        }
        return CountryRegions;
    }


    public int GetAchievement()
    {
        return resourceValue.Achievement;
    }

    public int GetCurrentYear()
    {
        return playerState.CurrentYear;
    }


    public Season GetCurrentSeason()
    {
        return playerState.CurrentSeason;
    }

    public string GetCountryName(string countryENName)
    {
        return countryManager.GetCountryName(countryENName);
    }

    public String GetPlayerCountryName()
    {
        return countryManager.GetCountryName(GetPlayerCountryENName()); ;
    }


    public String GetPlayerCountryENName()
    {
        return playerState.CountryENName;
    }

    public Sprite GetPlayerCountryIcon()
    {
        return countryManager.GetCountryIcon(GetPlayerCountryENName());
    }



    public Sprite GetCountryIcon(string countryName)
    {
        return countryManager.GetCountryIcon(countryName);
    }

    public int GetCurrentTurn()
    {
        int yearsPassed = playerState.CurrentYear - GameValueConstants.Year_Prologue;
        int seasonsPassed = (int)playerState.CurrentSeason - (int)GameValueConstants.Season_Prologue;

        int totalTurns = yearsPassed * 4 + seasonsPassed;

        if (totalTurns < 0)
        {
            Debug.Log("Turn BUG");
            return -1;
        }
            
        return totalTurns+1;
    }

    public List<Skill> GetSkillList()
    {
        return gameValueLibrary.GetSkillList();
    }


    public Skill GetSkill(int skillID)
    {
        return gameValueLibrary.GetSkill(skillID);
    }

    public List<TaskData> GetTotalTaskList()
    {
        return storyControl.GetTotalTaskList();
    }

    public ItemBase GetItem(int ItemID)
    {
        foreach (var item in allItems)
        {
            if (item.GetID() == ItemID)
            {
                return item;
            }
        }
        return null;
    }


    public ItemBase GetItem(string ItemName)
    {
        foreach (var item in allItems)
        {
            if (item.GetItemENName() == ItemName)
            {
                return item;
            }
        }
        return null;
    }

    public Sprite GetCharacterImage(string characterKey,string type)
    {
        return GetStoryCharacter(characterKey).GetTypeDefaultStandImage(type);
    }

    public Sprite GetCharacterIcon(string characterKey, string type)
    {
        return GetStoryCharacter(characterKey).GetTypeDefaultStandIcon(type);
    }

    StoryCharacterImageDataBase GetStoryCharacter(string characterKey)
    {

        StoryCharacterImageDataBase findData = allCharacterDBs.Find(c => c.CharacterKey == characterKey);
        if (findData == null) { Debug.Log($"cant find {characterKey}"); return null; }

        return findData;
    }

    #endregion
    #region Set



    public void SetPlayerName(string playerName)
    {
        playerState.Name = playerName;
        Character player = GetCharacterByKey(CharacterConstants.PlayerKey);
        player.SetPlayerName(playerName);
    }

    public void SetSaveData(GameValueSaveData gameValueData)
    {
        countryManager.SetSaveData(gameValueData.CountryManagerSaveData);
        playerState.SetSaveData(gameValueData.PlayerStateSaveData);
        resourceValue.SetSaveData(gameValueData.ResourceValueSaveData);
        storyControl.SetSaveData(gameValueData.StoryControlSaveData);

        foreach (var item in allItems)
        {
            if (gameValueData.allItems.TryGetValue(item.GetID(), out var saveData))
            {
                item.SetSaveData(saveData);
            }
        }

        gameValueLibrary.SetSaveData(gameValueData.allCharacters);
        gameValueLibrary.SetSaveData(gameValueData.allRegions);

        int cityCount = 0;
        foreach (var region in GetAllRegionValues())
        {
            cityCount += region.GetCityValues().Count;
        }

    }
    public void SetEND(string ENDName)
    {
        playerState.ENDName = ENDName;
    }

    public void SetPlayerCountry(string newCountryName)
    {

        playerState.CountryENName = newCountryName;
        return;


        string currentCountryName = playerState.CountryENName;
        Country currentCountry = GetCountry(currentCountryName);
        Country newCountry = GetCountry(newCountryName);

        if (currentCountry == null || newCountry == null) return;

        // ? ????????
        var oldCharacters = new List<Character>(GetCountryCharacters(currentCountryName));
        foreach (var character in oldCharacters)
        {
            character.SetCountry(newCountryName);
            newCountry.AddCharacter(character);
        }

        // ? ????????
        var oldRegions = new List<RegionValue>(GetPlayerRegions());
        foreach (var regionValue in oldRegions)
        {
            regionValue.SetCountry(newCountryName);
            newCountry.AddRegion(regionValue);
        }

        var oldRegionsListener = currentCountry.OnRegionsChange;
        var oldCharactersListener = currentCountry.OnCharactersChange;

        currentCountry.OnRegionsChange = null;
        currentCountry.OnCharactersChange = null;

        if (oldRegionsListener != null) newCountry.OnRegionsChange += oldRegionsListener;
        if (oldCharactersListener != null) newCountry.OnCharactersChange += oldCharactersListener;

        playerState.CountryENName = newCountryName;

        newCountry.OnRegionsChange?.Invoke();
        newCountry.OnCharactersChange?.Invoke();
    }

    #endregion
    #region IS

    public bool IsLastJudgmentHappen()
    {
        return playerState.IsLastJudgmentHappen;
    }

    #endregion
    #region Has
    public bool HasTask(string taskID)
    {
        return storyControl.HasTask(taskID);
    }

    public bool HasItem(int ItemID)
    {
        return GetItem(ItemID).PlayerHas();
    }

    #endregion
    #region Task
    public ExcelTaskStory GetExcelTaskStory(string key)
    {
        return gameValueLibrary.GetExcelTaskStory(key);
    }
    #endregion
    #region Battle and Explore

    public void SetBuildData(BuildData buildData)
    {
        this.buildData = buildData;
    }


    public BuildData GetBuildData()
    {
        return buildData;
    }


    public void SetBattleData(BattleData battleData)
    {
        this.battleData = battleData;
    }

    public BattleData GetBattleData()
    {
        return battleData;
    }

    public void SetExploreData(ExploreData exploreData)
    {
        this.exploreData = exploreData;
    }

    public ExploreData GetExploreData()
    {
        return exploreData;
    }

    #endregion

}