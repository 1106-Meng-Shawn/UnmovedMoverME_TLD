using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;
using static GetString;

public class MultiplePlaythroughsGameTotalCountryControl : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image countryIcon;
    [SerializeField] private TextMeshProUGUI countryText;

    [Header("Start Buttons")]
    [SerializeField] private Button prologueStartButton;
    [SerializeField] private Button openingStartButton;

    [SerializeField] private Button defaultValueButton;


    [Header("Value Rows")]
    public MultiplePlaythroughsValueRows valueRows;

    [Header("Game Mode Panel")]
    public GameModePanel gameModePanel;

    [Header("References")]
    [SerializeField] private MultiplePlaythroughsGameCIInheritance multiplePlaythroughsGameCIInheritance;


    private MultiplePlaythroughsGameTotalCountryData data;
    private readonly Dictionary<Button, (string country, int year, Season season)> startButtonInfo = new();

    #region Nested Classes
    [System.Serializable]
    public class MultiplePlaythroughsValueRows
    {
        public ResourceValueRowControl FoodRow;
        public ResourceValueRowControl ScienceRow;
        public ResourceValueRowControl PoliticsRow;
        public ResourceValueRowControl GoldRow;
        public ResourceValueRowControl FaithRow;
        public ResourceValueRowControl TotalRecruitedPopulationRow;
        public ResourceValueRowControl ScoutRow;
        public ResourceValueRowControl BuildRow;
        public ResourceValueRowControl NegotiationRow;
    }

    [System.Serializable]
    public class GameModePanel
    {
        public TextMeshProUGUI GameModeText;
        public TextMeshProUGUI GameModeAchievementCoefficientText;
        public Button EasyButton;
        public Button NormalButton;
        public Button HardButton;
        public Button LegendaryButton;
    }
    #endregion

    #region Unity Lifecycle
    private void Awake()
    {
        InitializeStartButtons();
        InitializeGameModeButtons();
    }

    private void Start()
    {
        data = SettingValue.Instance.StoryMultiplePlaythroughsData.MultiplePlaythroughsGameTotalData.MultiplePlaythroughsGameTotalCountryData;

        RegisterListeners();
        InitializeValueRows();
        RefreshStartButtons();
        RefreshGameModeUI();
        UpdateCountryDisplay();
    }

    private void OnDestroy()
    {
        UnregisterListeners();
    }
    #endregion

    #region Initialization
    private void InitializeStartButtons()
    {
        startButtonInfo[prologueStartButton] = (CountryConstants.BURGUNDY, GameValueConstants.Year_Prologue, GameValueConstants.Season_Prologue);
        startButtonInfo[openingStartButton] = (CountryConstants.HOLY_ROMULUS_EMPIRE, GameValueConstants.Year_Opening, GameValueConstants.Season_Opening);

        foreach (var kvp in startButtonInfo)
        {
            var (country, year, season) = kvp.Value;
            kvp.Key.onClick.AddListener(() => OnStartButtonClicked(country, year, season));
        }
    }

    private void InitializeValueRows()
    {
        defaultValueButton.onClick.AddListener(OnDefaultValueButtonClick);

        valueRows.FoodRow.InitResource(ValueType.Food);
        valueRows.ScienceRow.InitResource(ValueType.Science);
        valueRows.PoliticsRow.InitResource(ValueType.Politics);
        valueRows.GoldRow.InitResource(ValueType.Gold);
        valueRows.FaithRow.InitResource(ValueType.Faith);
        valueRows.TotalRecruitedPopulationRow.InitResource(ValueType.TotalRecruitedPopulation);
        valueRows.ScoutRow.InitResource(ValueType.Scout);
        valueRows.BuildRow.InitResource(ValueType.Build);
        valueRows.NegotiationRow.InitResource(ValueType.Negotiation);
    }

    private void InitializeGameModeButtons()
    {
        gameModePanel.EasyButton.onClick.AddListener(() => OnGameModeSelected(DifficultyGame.Easy));
        gameModePanel.NormalButton.onClick.AddListener(() => OnGameModeSelected(DifficultyGame.Normal));
        gameModePanel.HardButton.onClick.AddListener(() => OnGameModeSelected(DifficultyGame.Hard));
        gameModePanel.LegendaryButton.onClick.AddListener(() => OnGameModeSelected(DifficultyGame.Legendary));
    }

    void OnDefaultValueButtonClick()
    {
        valueRows.FoodRow.ResetAddition();
        valueRows.ScienceRow.ResetAddition();
        valueRows.PoliticsRow.ResetAddition();
        valueRows.GoldRow.ResetAddition();
        valueRows.FaithRow.ResetAddition();
        valueRows.TotalRecruitedPopulationRow.ResetAddition();
        valueRows.ScoutRow.ResetAddition();
        valueRows.BuildRow.ResetAddition();
        valueRows.NegotiationRow.ResetAddition();
    }
    #endregion

    #region UI Updates
    private void RefreshStartButtons()
    {
        foreach (var kvp in startButtonInfo)
        {
            var text = kvp.Key.GetComponentInChildren<TextMeshProUGUI>();
            if (text != null)
            {
                var (_, year, season) = kvp.Value;
                text.text = $"{year} {GetSeasonString(season)}";
            }
        }
    }

    private void RefreshGameModeUI()
    {
        UpdateGameModeText();
        UpdateGameModeButtonTexts();
    }

    private void UpdateCountryDisplay()
    {
        countryIcon.sprite = GameValue.Instance.GetCountryIcon(data.StartCountry);
        countryText.text = GameValue.Instance.GetCountryName(data.StartCountry);
    }

    private void UpdateGameModeText()
    {
        gameModePanel.GameModeText.text = "Game Mode";
        float coeff = GameValueConstants.GetAchievementCoefficient(data.DifficultyGame);
        gameModePanel.GameModeAchievementCoefficientText.text = $"× {coeff.ToString("G")}";
    }

    private void UpdateGameModeButtonTexts()
    {
        gameModePanel.EasyButton.GetComponentInChildren<TextMeshProUGUI>().text = DifficultyGame.Easy.ToString();
        gameModePanel.NormalButton.GetComponentInChildren<TextMeshProUGUI>().text = DifficultyGame.Normal.ToString();
        gameModePanel.HardButton.GetComponentInChildren<TextMeshProUGUI>().text = DifficultyGame.Hard.ToString();
        gameModePanel.LegendaryButton.GetComponentInChildren<TextMeshProUGUI>().text = DifficultyGame.Legendary.ToString();
    }
    #endregion

    #region Event Handling
    private void RegisterListeners()
    {
        LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
        data.OnStartCountryChanged += _ => UpdateCountryDisplay();
    }

    private void UnregisterListeners()
    {
        LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;
        if (data != null)
            data.OnStartCountryChanged -= _ => UpdateCountryDisplay();
    }

    private void OnLocaleChanged(Locale newLocale)
    {
        RefreshStartButtons();
        RefreshGameModeUI();
        UpdateCountryDisplay();
    }

    private void OnStartButtonClicked(string country, int year, Season season)
    {
        data.SetStartState(country, year, season);
        multiplePlaythroughsGameCIInheritance.SetCharacterList(GetStartYearCharacters(country));
    }

    List<Character> GetStartYearCharacters(string country){
        List<Character> StartYearCharacters = new List<Character>();

        switch (country)
        {
            case GameValueConstants.Country_Prologue:
                StartYearCharacters.AddRange(GameValue.Instance.GetCountryCharacters(GameValueConstants.Country_Prologue));
                break;
            case GameValueConstants.Country_Opening:
                StartYearCharacters.AddRange(GameValue.Instance.GetCountryCharacters(GameValueConstants.Country_Prologue));
                StartYearCharacters.AddRange(GameValue.Instance.GetCountryCharacters(GameValueConstants.Country_Opening));
                break;
        }

        return StartYearCharacters;
    }



    private void OnGameModeSelected(DifficultyGame mode)
    {
        data.DifficultyGame = mode;
        UpdateGameModeText();
    }
    #endregion
}

[Serializable]
public class MultiplePlaythroughsGameTotalCountryData
{
    #region Private Fields
    private string startCountry = GameValueConstants.Country_Prologue;
    private int startYear = GameValueConstants.Year_Prologue;
    private Season startSeason = GameValueConstants.Season_Prologue;
    private DifficultyGame difficultyGame = GameValueConstants.DefaultGameDifficulty;

    // ????
    private int foodAdditionCount = 0;
    private int scienceAdditionCount = 0;
    private int politicsAdditionCount = 0;
    private int goldAdditionCount = 0;
    private int faithAdditionCount = 0;
    private int totalRecruitedPopulationAdditionCount = 0;
    private int scoutAdditionCount = 0;
    private int buildAdditionCount = 0;
    private int negotiationAdditionCount = 0;
    #endregion

    #region Public Properties with Notification
    public string StartCountry
    {
        get => startCountry;
        set => SetField(ref startCountry, value, OnStartCountryChanged);
    }

    public int StartYear
    {
        get => startYear;
        set => SetField(ref startYear, value, OnStartYearChanged);
    }

    public Season StartSeason
    {
        get => startSeason;
        set => SetField(ref startSeason, value, OnStartSeasonChanged);
    }

    public DifficultyGame DifficultyGame
    {
        get => difficultyGame;
        set => SetField(ref difficultyGame, value, OnDifficultyGameChanged);
    }

    // ???????????????
    public int FoodAdditionCount
    {
        get => foodAdditionCount;
        set => SetAdditionField(ref foodAdditionCount, value, ValueType.Food);
    }

    public int ScienceAdditionCount
    {
        get => scienceAdditionCount;
        set => SetAdditionField(ref scienceAdditionCount, value, ValueType.Science);
    }

    public int PoliticsAdditionCount
    {
        get => politicsAdditionCount;
        set => SetAdditionField(ref politicsAdditionCount, value, ValueType.Politics);
    }

    public int GoldAdditionCount
    {
        get => goldAdditionCount;
        set => SetAdditionField(ref goldAdditionCount, value, ValueType.Gold);
    }

    public int FaithAdditionCount
    {
        get => faithAdditionCount;
        set => SetAdditionField(ref faithAdditionCount, value, ValueType.Faith);
    }

    public int TotalRecruitedPopulationAdditionCount
    {
        get => totalRecruitedPopulationAdditionCount;
        set => SetAdditionField(ref totalRecruitedPopulationAdditionCount, value, ValueType.TotalRecruitedPopulation);
    }

    public int ScoutAdditionCount
    {
        get => scoutAdditionCount;
        set => SetAdditionField(ref scoutAdditionCount, value, ValueType.Scout);
    }

    public int BuildAdditionCount
    {
        get => buildAdditionCount;
        set => SetAdditionField(ref buildAdditionCount, value, ValueType.Build);
    }

    public int NegotiationAdditionCount
    {
        get => negotiationAdditionCount;
        set => SetAdditionField(ref negotiationAdditionCount, value, ValueType.Negotiation);
    }
    #endregion

    #region Events
    public event Action<ValueType, int> OnAdditionChanged;
    public event Action<string> OnStartCountryChanged;
    public event Action<int> OnStartYearChanged;
    public event Action<Season> OnStartSeasonChanged;
    public event Action<DifficultyGame> OnDifficultyGameChanged;
    private Dictionary<ValueType, Action> reseter;
    #endregion

    #region Dictionaries
    private Dictionary<ValueType, Func<int>> getter;
    private Dictionary<ValueType, Action<int>> adder;
    #endregion

    #region Constructor
    public MultiplePlaythroughsGameTotalCountryData()
    {
        InitializeDictionaries();
    }

    private void InitializeDictionaries()
    {
        getter = new Dictionary<ValueType, Func<int>>()
        {
            { ValueType.Food, () => foodAdditionCount },
            { ValueType.Science, () => scienceAdditionCount },
            { ValueType.Politics, () => politicsAdditionCount },
            { ValueType.Gold, () => goldAdditionCount },
            { ValueType.Faith, () => faithAdditionCount },
            { ValueType.TotalRecruitedPopulation, () => totalRecruitedPopulationAdditionCount },
            { ValueType.Scout, () => scoutAdditionCount },
            { ValueType.Build, () => buildAdditionCount },
            { ValueType.Negotiation, () => negotiationAdditionCount },
        };

        adder = new Dictionary<ValueType, Action<int>>()
        {
            { ValueType.Food, v => FoodAdditionCount += v },
            { ValueType.Science, v => ScienceAdditionCount += v },
            { ValueType.Politics, v => PoliticsAdditionCount += v },
            { ValueType.Gold, v => GoldAdditionCount += v },
            { ValueType.Faith, v => FaithAdditionCount += v },
            { ValueType.TotalRecruitedPopulation, v => TotalRecruitedPopulationAdditionCount += v },
            { ValueType.Scout, v => ScoutAdditionCount += v },
            { ValueType.Build, v => BuildAdditionCount += v },
            { ValueType.Negotiation, v => NegotiationAdditionCount += v },
        };

            reseter = new Dictionary<ValueType, Action>()
        {
            { ValueType.Food, () => FoodAdditionCount = 0 },
            { ValueType.Science, () => ScienceAdditionCount = 0 },
            { ValueType.Politics, () => PoliticsAdditionCount = 0 },
            { ValueType.Gold, () => GoldAdditionCount = 0 },
            { ValueType.Faith, () => FaithAdditionCount = 0 },
            { ValueType.TotalRecruitedPopulation, () => TotalRecruitedPopulationAdditionCount = 0 },
            { ValueType.Scout, () => ScoutAdditionCount = 0 },
            { ValueType.Build, () => BuildAdditionCount = 0 },
            { ValueType.Negotiation, () => NegotiationAdditionCount = 0 },
        };

    }
    #endregion

    #region Unified Notification Methods

    /// <summary>
    /// ??????????????
    /// </summary>
    private void SetField<T>(ref T field, T value, Action<T> onChanged) 
    {
        if (!EqualityComparer<T>.Default.Equals(field, value))
        {
            field = value;
            onChanged?.Invoke(value);
        }
    }

    private void SetAdditionField(ref int field, int value, ValueType type)
    {
        if (field != value)
        {
            field = value;
            NotifyAdditionChanged(type, value);
        }
    }

    private void NotifyAdditionChanged(ValueType type, int newValue)
    {
        OnAdditionChanged?.Invoke(type, newValue);

        // ????????
        SettingValue.Instance?.StoryMultiplePlaythroughsData?.InvokeAchievementCostChange();
    }

    #endregion

    #region Public Methods

    public int GetAddition(ValueType type)
    {
        return getter.ContainsKey(type) ? getter[type]() : 0;
    }

    public void AddAddition(ValueType type, int value)
    {
        if (adder.ContainsKey(type))
        {
            adder[type](value); // ?????? setter?????
        }
    }

    public void ResetAddition(ValueType type)
    {
        if (reseter.ContainsKey(type))
        {
            reseter[type].Invoke();
            NotifyAdditionChanged(type, 0);
        }
    }


    /// <summary>
    /// ??????
    /// </summary>
    public void SetStartState(string startCountry, int startYear, Season startSeason)
    {
        StartCountry = startCountry;
        StartYear = startYear;
        StartSeason = startSeason;
    }

    public string GetStartTimeString()
    {
        return $"{StartYear} {GetSeasonString(StartSeason)}";
    }

    /// <summary>
    /// ???????
    /// </summary>
    public int GetAchievementCost()
    {
        return foodAdditionCount +
               scienceAdditionCount +
               politicsAdditionCount +
               goldAdditionCount +
               faithAdditionCount +
               totalRecruitedPopulationAdditionCount +
               scoutAdditionCount +
               buildAdditionCount +
               negotiationAdditionCount;
    }

    private string GetSeasonString(Season season)
    {
        return season.ToString();
    }
    
    #endregion
}

