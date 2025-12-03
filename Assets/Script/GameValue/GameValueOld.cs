using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Unity.VisualScripting.Generated.PropertyProviders;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;
using static ExcelReader;
using static GetColor;

public class GameValueOld : MonoBehaviour
{
    #region variable
    public static GameValueOld Instance { get; private set; }
    [SerializeField] private float food;
    public float Food
    {
        get => food;
        set => SetProperty(ref food, value);
    }
    [SerializeField] private float science;
    public float Science
    {
        get => science;
        set => SetProperty(ref science, value);
    }
    [SerializeField] private float politics;
    public float Politics
    {
        get => politics;
        set => SetProperty(ref politics, value);
    }
    [SerializeField] private float gold;
    public float Gold
    {
        get => gold;
        set => SetProperty(ref gold, value);
    }
    [SerializeField] private float faith;
    public float Faith
    {
        get => faith;
        set => SetProperty(ref faith, value);
    }

    [SerializeField] private int totalRecruitedPopulation = 0;
    public int TotalRecruitedPopulation
    {
        get => totalRecruitedPopulation;
        set => SetProperty(ref totalRecruitedPopulation, value);
    }
    public event Action OnGameValueChanged;
    public event System.Action<string, double> OnSBNValueChanged;
    [SerializeField] private float scout = 0;
    [SerializeField]    private float build = 0;
    [SerializeField] private float negotiation = 0;
    private bool isLastJudgmentHappen = false;
    private long HannaGold = 0;
    private int achievement = 0;
    private int currentYear = 1368;
    private int currentSeason = 2;
    [SerializeField] private string playerCountry;
    public string PlayerCountry
    {
        get => playerCountry;
        set => SetProperty(ref playerCountry, value);
    }
    public string playerName;
      private List<Character> CurrentCharactersInGame = new List<Character>();
      private List<Character> allImportantCharacters = new List<Character>();
    //  private List<ItemBase> allItems = new List<ItemBase>();
      private List<Skill> allSkills = new List<Skill>();
      private readonly List<RegionValue> allRegions = new List<RegionValue>();
      private List<Character> InitNormalsCharacters = new List<Character>();
      private List<Character> InitMonsters = new List<Character>();
      private ObservableList<Character> playerCharacters = new ObservableList<Character>();
    //  private ObservableList<ItemBase> playerItems = new ObservableList<ItemBase>();
      private ObservableList<RegionValue> playerRegions = new ObservableList<RegionValue>();
      public ObservableList<Character> PlayerCharacters => playerCharacters;
     // public ObservableList<ItemBase> PlayerItems => playerItems;
      public ObservableList<ItemBase> AllItems = new ObservableList<ItemBase>();
      public ObservableList<RegionValue> PlayerRegions => playerRegions;
   //   public ItemEventSystem ItemEventSystem = new ItemEventSystem();
      public List<ItemBase> storeCanSell = new List<ItemBase>();
      private List<ExcelGameENDData> gameENDDatas = new List<ExcelGameENDData>();

    public GameValueLibrary gameValueLibrary;

    [SerializeField] private string endName;

    private CountryManager countryManager;
    private StoryControl storyControl;

    private BattleData battleData;
    private ExploreData exploreData;


    public string ENDName
    {
        get => endName;
        set => SetProperty(ref endName, value);
    }


    protected bool SetProperty<T>(ref T field, T value)
    {
        if (!EqualityComparer<T>.Default.Equals(field, value))
        {
            field = value;
            OnGameValueChanged?.Invoke();
            return true;
        }
        return false;
    }
    #endregion

   /* private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        DontDestroyOnLoad(gameObject);

        InitGameValue();
    }


    private void Start()
    {
        
    }


    void InitGameValue()
    {
        countryManager = new CountryManager();
        countryManager.Init();
        storyControl = new StoryControl();
        storyControl.Init();

        gameValueLibrary = new GameValueLibrary();


    }

    public void SetPlayerCountry(string newCountry)
    {
        foreach (var character in playerCharacters)
        {
            //character.GetCountryENName() = newCountry;


        }


        foreach (var region in allRegions)
        {
            if (region.GetCountryENName() == PlayerCountry)
            {
             //   region.Country = newCountry;
                foreach (var city in region.GetCityValues())
                {
                    city.cityCountry = newCountry;
                }
            }

            if (region.GetCountryENName() == newCountry && !playerRegions.Contains(region) ){ 
                playerRegions.Add(region);
            }
        }

        PlayerCountry = newCountry;


    }

    public int GetCurrentYear()
    {
        return currentYear;
    }

    public void SetCurrentYear(int newYear)
    {
        currentYear = newYear;
    }


    public int GetCurrentSeason()
    {
        return currentSeason;
    }

    public void SetCurrentSeason(int newSeason)
    {
        currentSeason = newSeason;
    }

    private string[] seasons = { "Spring", "Summer", "Fall", "Winter" };

    public string GetCurrentSeasonString()
    {
        return LocalizationSettings.StringDatabase.GetLocalizedString("GameSetting", seasons[currentSeason]);

        return seasons[currentSeason];
    }




    public void SetPlayerName(string playerName)
    {
        this.playerName = playerName;

        Character playerCharacter = CurrentCharactersInGame.FirstOrDefault(c => c.GetCharacterFileType() == "Player");

        if (playerCharacter != null)
        {
            playerCharacter.SetPlayerName(playerName);
        }
        SettingsManager settingsManager = SettingsManager.Instance;
        if (settingsManager != null)
        {
            settingsManager.ChangePlayerName(playerName);
        }
    }

    public Character FindCharacterByName(string characterName)
    {
        var character = GetCurrentCharactersInGame()
            .FirstOrDefault(c => c.GetCharacterENName() == characterName);

        if (character == null)
            Debug.LogWarning($"Character with name '{characterName}' not found.");

        return character;
    }

    public Character FindCharacterByID(int id)
    {
        var character = GetCurrentCharactersInGame()
            .FirstOrDefault(c => c?.GetCharacterID() == id);

        if (character == null)
        {
            Debug.LogWarning($"Character with ID {id} not found.");
        }

        return character;
    }

    public Character FindCharacter(string CharacterName)
    {
        if (CharacterName == "Player") return GetCurrentCharactersInGame()[0]; // return player character


            foreach (var character in GetCurrentCharactersInGame())
        {
            if (character.GetCharacterENName() == CharacterName) // maybe need to change, because even player type is player or player name can change be same with other character but, just make sure;
            {
                return character;
            }
        }

        Debug.Log($"didn't found {CharacterName}");


        return null;
    }


    public RegionValue FindRegionValueByID(int id)
    {
        var regionValue = GetAllRegionValues().FirstOrDefault(c => c?.GetRegionID() == id);

        if (regionValue == null)
        {
            Debug.LogWarning($"Character with ID {id} not found.");
        }

        return regionValue;
    }


    public ItemBase FindItem(string ItemName)
    {
        foreach (var item in AllItems)
        {
            if (item.GetItemENName() == ItemName) 
            {
                return item;
            }
        }
        return null;
    }
  /*  public void UpdatePlayerItems()
    {
        playerItems.Clear();
        foreach (var item in allItems)
        {
            if (item != null && item.GetItemNum() != 0)
            {
                playerItems.Add(item);
                if (item.TriggerEffect("UIChange")) item.UseUIChangeEffect();
            }
        }
    }

    public void UpdateValuesFromRegions(RegionValue[] regions, string playerCountyName)
    {
        int totalRegionPopulation = 0;
        int totalSupportPopulation = 0;
        float totalFood = 0;
        float totalScience = 0;
        float totalPolitics = 0;
        float totalGold = 0;
        float totalFaith = 0;

        Debug.Log("Maybe have bug for region.GetRegionResourceLastTax(0),because add it is last turn not this turn maybe can get,just make sure");
        foreach (var region in regions)
        {
            if (region.GetCountryENName() == playerCountyName)
            {
                totalRegionPopulation += region.GetRegionPopulation();
                totalFood += region.GetRegionResourceLastTax(0);
                totalScience += region.GetRegionResourceLastTax(1);
                totalPolitics += region.GetRegionResourceLastTax(2);
                totalGold += region.GetRegionResourceLastTax(3);
                totalFaith += region.GetRegionResourceLastTax(4);

                totalSupportPopulation += Mathf.RoundToInt(region.GetRegionPopulation() * region.GetRegionSupportRate());
            }
        }

        this.food += totalFood;
        this.science += totalScience;
        this.politics += totalPolitics;
        this.gold += totalGold;
        this.faith += totalFaith;
    }

    public void UpdatePopulationFromRegions(RegionValue[] regions, string playerCountyName)
    {
        int totalRegionPopulation = 0;
        int totalSupportPopulation = 0;
        int totalRegionCount = 0;

        foreach (var region in regions)
        {
            if (region.GetCountryENName() == playerCountyName)
            {
                totalRegionPopulation += region.GetRegionPopulation();
                totalRegionCount++;
                totalSupportPopulation += Mathf.RoundToInt(region.GetRegionPopulation() * region.GetRegionSupportRate());
            }
        }

       // this.count = totalRegionCount;

    }

    public List<Character> GetCurrentCharactersInGame()
    {
        return CurrentCharactersInGame;
    }

    public int GetPlayerItemsCount()
    {
        int count = 0;
        foreach (var item in AllItems)
        {
            if (item.PlayerHas())
                count += 1; 
        }
        return count;
    }

    public ItemBase GetItem(int itemID)
    {
        return AllItems.FirstOrDefault(item => item.GetID() == itemID);
    }

    public bool HasItem(int itemID)
    {
        ItemBase item = AllItems.FirstOrDefault(item => item.GetID() == itemID);
        return item.PlayerHas();
    }



    public bool IsLastJudgmentHappen()
    {
        return isLastJudgmentHappen;
    }



    public int GetAchievement()
    {
        return achievement;
    }

    public List<Character> GetAllImportantCharacters()
    {
        return allImportantCharacters;
    }
    public List<RegionValue> GetAllRegionValues()
    {
        return allRegions;
    }

    public void SetGameValueData(GameValueSaveData gameValueData)
    {
        
        SetPlayerName(gameValueData.GetPlayerName());
        countryManager.SetSaveData(gameValueData.CountryManagerSaveData);
     //   if (CurrentCharactersInGame.Count == 0) GenerateInitCharacter();

       /* foreach (var charData in gameValueData.GetAllCharacters())
        {
        /*   if (charData.type == "Important")
            {
                GetCurrentCharacterInGame(charData.currentID).UpValue(charData, allSkills);

            } else if (charData.type == "Normal")
            {
                Character newCharacter = InitNormalsCharacters[charData.characterID].DeepCopy();
                newCharacter.UpValue(charData, allSkills);
                newCharacter.SetCurrentIDInList(CurrentCharactersInGame.Count);
                CurrentCharactersInGame.Add(newCharacter);

            }
            else if(charData.type == "Monster")
            {
                Character newCharacter = InitMonsters[charData.characterID].DeepCopy();
                newCharacter.UpValue(charData, allSkills);
                newCharacter.SetCurrentIDInList(CurrentCharactersInGame.Count);
                CurrentCharactersInGame.Add(newCharacter);

            }
        

        }*/

     /*   foreach (var regionData in gameValueData.GetAllRegions())
        {
            RegionValue matchingRegionValue = allRegions.Find(r => r.GetRegionENName() == regionData.regionName);
            if (matchingRegionValue != null)
            {
                Region region = CityConnetManage.Instance.GetRegion(matchingRegionValue.GetRegionID());
                region.SetRegionValue();
                matchingRegionValue.SetRegionData(regionData, CurrentCharactersInGame);
            }
        }
     
        storyControl.SetSaveData(gameValueData.StoryControlSaveData);

    }

    public Character GetCurrentCharacterInGame(int currentID)
    {
        return CurrentCharactersInGame[currentID];
    }

    public List<Character> GetCurrentCharactersInGame(string coutryName)
    {
        List<Character> coutryCharacters = new List<Character>();
        foreach (var character in CurrentCharactersInGame)
        {
            if (character.GetCountryENName() == coutryName)
            {
                coutryCharacters.Add(character);
            }
        }

        foreach (var character in InitNormalsCharacters)
        {
            if (character.GetCountryENName() == coutryName)
            {
                Character NewCharacter = character.DeepCopy();
                coutryCharacters.Add(NewCharacter);
            }
        }
        return coutryCharacters;

    }

    public void AddPlayerCharacter(Character newCharacter)
    {
       // newCharacter.Country = playerCountry;
        playerCharacters.Add(newCharacter);
    }


  /*  public int GetPlayerItemNum()
    {
        return playerItems.Count;
    }


    public Character GetPlayerCharacterByID(int ID)
    {
        if (ID < CurrentCharactersInGame.Count) return CurrentCharactersInGame[ID];

        return null;
    }



    public int GetPlayerCharacterNum()
    {
        return playerCharacters.Count;
    }

    public String GetSeason()
    {
        return seasons[currentSeason];
    }

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

        Color32 seasonColor = GetTurnColor(GetCurrentSeason());
        string hexColor = ColorUtility.ToHtmlStringRGB(seasonColor);
        return $"{year} <color=#{hexColor}>{season}</color>";

    }

    string UpdateTurnTextCHJA()
    {
        string year = GetCurrentYear().ToString();
        int currentSeason = GetCurrentSeason();

        string[] seasonKeys = { "Spring", "Summer", "Fall", "Winter" };
        string[] localizedSeasons = new string[4];

        for (int i = 0; i < 4; i++)
        {
            string seasonName = LocalizationSettings.StringDatabase.GetLocalizedString("GameSetting", seasonKeys[i]);
            if (i == currentSeason)
            {
                Color32 seasonColor = GetTurnColor(i);
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



    public long GetHannaGold()
    {
        return HannaGold;
    }


    public int GetPlayerRegionsNum()
    {
        return playerRegions.Count;
    }


    public float GetTotalResources(int type)
    {
        switch (type)
        {
            case 0: return food;
            case 1: return science;
            case 2: return politics;
            case 3: return gold;
            case 4: return faith;
        }
        return -1;
    }

    public int GetTotalPopulation()
    {
        int TotalPopulation = 0;
        foreach (var region in playerRegions)
        {
            TotalPopulation += region.GetRegionPopulation();
        }

        return TotalPopulation;
    }


    public int GetTotalSupportPopulation()
    {
        int TotalSupportPopulation = 0;
        foreach (var region in playerRegions)
        {
            TotalSupportPopulation += Mathf.RoundToInt(region.GetRegionPopulation() * region.GetRegionSupportRate());
        }

        return TotalSupportPopulation;
    }

    public float GetTotalSupportRate()
    {
        float support = GetTotalSupportPopulation();
        float total = GetTotalPopulation();
        return support / total;
    }

    #region Properties with Change Notification

    public float Negotiation
    {
        get => negotiation;
        set
        {
            if (negotiation != value)
            {
                negotiation = value;
                Debug.Log($"Negotiation changed to {value}, triggering event");
                OnSBNValueChanged?.Invoke("Negotiation", negotiation);
            }
        }
    }

    public float Build
    {
        get => build;
        set
        {
            if (build != value)
            {
                build = value;
                OnSBNValueChanged?.Invoke(nameof(Build), build);
            }
        }
    }

    public float Scout
    {
        get => scout;
        set
        {
            if (scout != value)
            {
                scout = value;
                OnSBNValueChanged?.Invoke(nameof(Scout), scout);
            }
        }
    }
    #endregion

    #region Battle
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

    #region Country



    public CountryManager GetCountryManager()
    {
        return countryManager;
    }


    public Sprite GetPlayerCountryIcon()
    {
        return countryManager.GetCountry(playerCountry).GetIcon();
    }




    public Sprite GetCountryIcon(string countryName)
    {
        return countryManager.GetCountryIcon(countryName);
    }


    public Color32 GetCountryColor(string country)
    {
        if (countryManager == null) { return Color.yellow; }
        if (countryManager.GetCountry(country) == null) { return Color.red; }
        return countryManager.GetCountry(country).GetColor();
    }

    public string GetCountryColorString(string originalString, string country)
    {
        Color32 color = countryManager.GetCountryColor(country);
        string hexColor = ColorUtility.ToHtmlStringRGBA(color);
        return $"<color=#{hexColor}>{originalString}</color>";
    }

    #endregion

    #region Story

    public List<TaskData> GetTotalTaskList()
    {
        return storyControl.GetTotalTaskList();
    }
    public StoryControl GetStoryControl()
    {
        return storyControl;
    }

    public TaskData GetTaskData(string taskID)
    {
        return storyControl.GetTaskDataByTaskKey(taskID);
    }

    public bool HasTask(string taskID)
    {
        return storyControl.HasTask(taskID);
    }


    #endregion

    #region Save and Load
    public string GenerateDataPath(int index)
    {
        string suffix = null;

        if (index == -1)
        {
            suffix = AutoSaveNameConstants.TEMPLOAD;

        }


        return Path.Combine(Application.persistentDataPath, Constants.SAVE_FILE_PATH, suffix + Constants.SAVE_FILE_EXTENSION);
    }


    public void LoadSaveData()
    {
        string loadPath = GenerateDataPath(-1);
        if (File.Exists(loadPath))
        {
            string json = File.ReadAllText(loadPath);
            var saveData = JsonConvert.DeserializeObject<SaveData>(json);
            GameValue.Instance.SetGameValueData(saveData.gameValueData);
            if (SceneManager.GetActiveScene().name == "GameScene")
            {
                Camera camera = Camera.main;
                camera.transform.position = new Vector3(
                    saveData.cameraSaveDate.cameraPositionX,
                    saveData.cameraSaveDate.cameraPositionY,
                    saveData.cameraSaveDate.cameraPositionZ
                );
                camera.orthographicSize = saveData.cameraSaveDate.cameraSize;
            }

            BottomButton.Instance.SetTotalPanelSaveData(saveData.totalPanelSaveData);
            NotificationManage.Instance.ShowToTop("Load is Successfully");
        }
    }*/
   // #endregion
}