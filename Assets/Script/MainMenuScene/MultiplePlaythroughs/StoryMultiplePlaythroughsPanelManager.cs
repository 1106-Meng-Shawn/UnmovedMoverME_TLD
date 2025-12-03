using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;
using static GetColor;
using static StoryMultiplePlaythroughsPanelManager;

public class StoryMultiplePlaythroughsPanelManager : MonoBehaviour
{

    public static StoryMultiplePlaythroughsPanelManager Instance;

    public enum StoryMultiplePlaythroughsPanelType
    {
        Game,Player
    }
    StoryMultiplePlaythroughsPanelType currentType = StoryMultiplePlaythroughsPanelType.Game;

    [SerializeField] private MultiplePlaythroughsGamePanelControl MultiplePlaythroughsGamePanelControl;
    [SerializeField] private MultiplePlaythroughsPlayerPanelControl MultiplePlaythroughsPlayerPanelControl;
    [SerializeField] CharacterMultiplePlaythroughsPanel characterMultiplePlaythroughsPanel;


    [Header("TopRow")]
    public TopRow topRow;
    [System.Serializable]
    public class TopRow
    {
        public Button MultiplePlaythroughsGamePanelButton;
        public Button MultiplePlaythroughsPlayerPanelButton;
        public TextMeshProUGUI MaxAchievementText;
        public TextMeshProUGUI RemainingAchievementText;
    }

    [Header("BottomRow")]
    public BottomRow bottomRow;
    [System.Serializable]
    public class BottomRow
    {
        public Button DefaultButton;
        public Button SaveButton;
        public Button LoadButton;
        public Button NextButton;
    }


    private StoryMultiplePlaythroughs data;

    private void Awake()
    {
        InitInstance();
        InitTopRow();
        InitBottomRow();
    }


    void InitInstance()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void Start()
    {
        data = SettingValue.Instance.StoryMultiplePlaythroughsData;
        RegisterListeners();
    }

    private void OnDestroy()
    {
        UnregisterListeners();
    }

    private void RegisterListeners()
    {
       // LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
        data.OnAchievementCostChange += ShowRemainingAchievementText;
    }

    private void UnregisterListeners()
    {
     //   LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;
        if (data != null) data.OnAchievementCostChange -= ShowRemainingAchievementText;
    }



    #region TopRow

    void InitTopRow()
    {
        topRow.MultiplePlaythroughsPlayerPanelButton.onClick.AddListener(OnMultiplePlaythroughsPlayerPanelButtonClick);
        topRow.MultiplePlaythroughsGamePanelButton.onClick.AddListener(OnMultiplePlaythroughsGamePanelButtonClick);
        topRow.MaxAchievementText.text = GetAchievementString(SettingValue.Instance.GetMaxAchievement());
        ShowRemainingAchievementText();
    }

    void ShowRemainingAchievementText()
    {
        topRow.RemainingAchievementText.text = GetAchievementString(SettingValue.Instance.GetMaxAchievement() - SettingValue.Instance.StoryMultiplePlaythroughsData.GetAchievementCost());
    }

    string GetAchievementString(int amount)
    {
        string AchievementString = amount.ToString("N0");
        return GetValueColorString(AchievementString, ValueColorType.Achievement);
    }


    public bool IsCurrentAchievementEnough(int cost)
    {
        bool IsEnough = false;
        int currentAchievement = SettingValue.Instance.GetMaxAchievement() - SettingValue.Instance.StoryMultiplePlaythroughsData.GetAchievementCost();
        IsEnough = cost <= currentAchievement;
        return IsEnough;
    }




    void OnMultiplePlaythroughsPlayerPanelButtonClick()
    {
        ShowMultiplePlaythroughsPlayerPanel();
    }

    void OnMultiplePlaythroughsGamePanelButtonClick()
    {
        ShowMultiplePlaythroughsGamePanel();
    }

    public void ShowMultiplePlaythroughsPlayerPanel()
    {
        MultiplePlaythroughsGamePanelControl.ClosePanel();
        CloseCharacterMultiplePlaythroughsPanel();
        MultiplePlaythroughsPlayerPanelControl.ShowPanel();
        SetCurrentType(StoryMultiplePlaythroughsPanelType.Player);
    }

    void ShowMultiplePlaythroughsGamePanel()
    {
        MultiplePlaythroughsPlayerPanelControl.ClosePanel();
        CloseCharacterMultiplePlaythroughsPanel();
        MultiplePlaythroughsGamePanelControl.ShowPanel();
        SetCurrentType(StoryMultiplePlaythroughsPanelType.Game);
    }

    void SetCurrentType(StoryMultiplePlaythroughsPanelType storyMultiplePlaythroughsPanelType)
    {
        currentType = storyMultiplePlaythroughsPanelType;
        SetNextButtonText();
    }

    void SetNextButtonText()
    {
        switch (currentType)
        {
            case StoryMultiplePlaythroughsPanelType.Game: bottomRow.NextButton.GetComponentInChildren<TextMeshProUGUI>().text = "Player";break;
            case StoryMultiplePlaythroughsPanelType.Player: bottomRow.NextButton.GetComponentInChildren<TextMeshProUGUI>().text = "Start"; break;
        }
    }

    #endregion

    #region BottomRow

    void InitBottomRow()
    {
        bottomRow.DefaultButton.onClick.AddListener(OnDefaultButtonClick);
        bottomRow.SaveButton.onClick.AddListener(OnSaveButtonClick);
        bottomRow.LoadButton.onClick.AddListener(OnLoadButtonClick);
        bottomRow.NextButton.onClick.AddListener(OnNextButtonClick);
    }

    void OnDefaultButtonClick()
    {
        switch (currentType)
        {
            case StoryMultiplePlaythroughsPanelType.Game: MultiplePlaythroughsGamePanelControl.Default(); break;
            case StoryMultiplePlaythroughsPanelType.Player: MultiplePlaythroughsPlayerPanelControl.Default(); break;
        }

    }

    void OnSaveButtonClick()
    {
    }


    void OnLoadButtonClick()
    {
        MultiplePlayLoadPanelManager.Instance.ShowPanel();
    }


    void OnNextButtonClick()
    {
        switch (currentType)
        {
            case StoryMultiplePlaythroughsPanelType.Game: ShowMultiplePlaythroughsPlayerPanel();break;
            case StoryMultiplePlaythroughsPanelType.Player: StartGame(); break;
        }
    }

    void StartGame()
    {
        string PlayerName = MultiplePlaythroughsPlayerPanelControl.GetPlayerName();
        List<MultiplePlaythroughsGameCharacterRowControlData> dataLists = MultiplePlaythroughsGameCIInheritance.Instance.GetCharacterDatas();


        foreach (var data in dataLists)
        {
           Character character =  GameValue.Instance.GetCharacterByID(data.GetCharacterID());
           if (character != null)
           {
                character.SetMultipleData(data);
           } else{
                //  Character newCharacter = new Character(data);
                Debug.LogWarning("Havent do yet, for creat new character by MultiplePlaythroughsGameCharacterRowControlData");
           }
        }
        SceneTransferManager.Instance.StartNewGame(PlayerName);
    }



    #endregion

    // Update is called once per frame


    public void ShowCharacterMultiplePlaythroughsPanel(MultiplePlaythroughsGameCharacterRowControlData data)
    {
        characterMultiplePlaythroughsPanel.ShowPanel(data);
    }

    public void CloseCharacterMultiplePlaythroughsPanel()
    {
        characterMultiplePlaythroughsPanel.ClosePanel();
    }



    void Update()
    {
        
    }
}

public class StoryMultiplePlaythroughs
{
    public MultiplePlaythroughsGameTotalData MultiplePlaythroughsGameTotalData = new MultiplePlaythroughsGameTotalData();

    public event Action OnAchievementCostChange;

    public void InvokeAchievementCostChange()
    {
        OnAchievementCostChange?.Invoke();
    }

    public int GetAchievementCost()
    {
        return MultiplePlaythroughsGameTotalData.GetAchievementCost();
    }

}

public class StoryMultiplePlaythroughsSaveData
{


    public int GetAchievementCost()
    {
        Debug.LogWarning("StoryMultiplePlaythroughsSaveData GetAchievementCost HAVENT DO YET");
        return 555;
    }

    public float GetAchievementCoefficient()
    {
        Debug.LogWarning("StoryMultiplePlaythroughsSaveData GetAchievementCoefficient HAVENT DO YET");
        return 1.5f;
    }


    public Sprite GetCountryIcon()
    {
        Debug.LogWarning("StoryMultiplePlaythroughsSaveData GetCountryIcon HAVENT DO YET");
        return null;
    }


}