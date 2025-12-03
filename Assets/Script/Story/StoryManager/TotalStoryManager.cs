using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System.Linq;
using UnityEngine.EventSystems;
using static ExcelReader;
using UnityEngine.Rendering.HighDefinition;

/// <summary>
/// ???????????????????
/// </summary>
public class TotalStoryManager : MonoBehaviour
{
    #region Singleton
    public static TotalStoryManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
    #endregion

    #region Inspector References
    [SerializeField] private StoryDataManager storyDataManager;
    [SerializeField] private StoryUIController uiController;
    [SerializeField] public StoryMediaController MediaController;
    [SerializeField] private StorySaveLoadManager saveLoadManager;
    [SerializeField] private StoryInputHandler inputHandler;
    [SerializeField] private StoryTextEffect storyTextEffect;


    [SerializeField] private GameObject storyPanel;
    [SerializeField] private List<GameObject> noAboutStorys;
    [SerializeField] private List<GameObject> SaveLoadClose;
    [SerializeField] private StoryButtonsControl StoryButtonsControl;
    [SerializeField] private HistoryManager historyManager;
    [SerializeField] private TypewriterEffect typewriterEffect;

    #endregion

    #region Private Fields
    private string saveSpeakingContent;
    private List<bool> SaveLoadCloseActive;
    private bool choiceTriggered = false;
    private bool isLoad = false;
    #endregion

    private void Start()
    {
        InitManager();
        saveLoadManager.LoadOrCreateProgress();
    }

    private void InitManager()
    {
        historyManager = HistoryManager.Instance;
    }

    private void Update()
    {
        if (!storyPanel.activeSelf) return;

        StoryButtonsControl.ShowOrHideoButtons();

        if (IsShowText() &&
            inputHandler.ClickNextSentence())
        {
            if (!StoryButtonsControl.IsHittingBottomButtons())
            {
                DisplayNextLine();
            }
        }

        if ((IsShowText()) &&
            IsPointerOverStoryLayer() &&
            (Input.GetAxis("Mouse ScrollWheel") != 0))
        {
            StoryButtonsControl.OnHistoryButtonClick();
        }

        if (Input.GetMouseButtonDown(1))
        {


            if (IsPointerOverStoryLayer())
            {

                if (IsShowText())
                {
                    Debug.Log("Opening UI");
                    uiController.OpenUI();
                }
                else
                {
                    Debug.Log("Closing UI");
                    uiController.CloseUI();
                }
            }

        }


        if ((Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl)) &&
            (uiController.GetStoryPanel().activeSelf))
        {
            inputHandler.CtrlSkip();
        }
    }

    #region Story Initialization

    /// <summary>
    /// ????????
    /// </summary>
    public void InitializeAndLoadStory(string fileName, int lineNumber, int sheetIndex)
    {
        ShowStoryPanel();
        TriggerNoStoryGameObejcts(false);

        storyDataManager.Initialize(lineNumber, sheetIndex);
        StoryButtonsControl.InitializeBottomButtons();
        storyDataManager.LoadStoryFromFile(fileName, sheetIndex);
        historyManager.InitializeHistory();

        if (isLoad)
        {
            RecoverLastBackgroundAndCharacter();
            isLoad = false;
        }
        DisplayNextLine();
    }

    /// <summary>
    /// ????
    /// </summary>
    public void LoadStory(string fileName, int lineNumber, int sheetIndex)
    {
        ShowStoryPanel();
        TriggerNoStoryGameObejcts(false);

        storyDataManager.SetCurrentLine(lineNumber);
        storyDataManager.SetCurrentSheetIndex(sheetIndex);
        storyDataManager.GetReadSheetIndex().Add(sheetIndex);

        StoryButtonsControl.InitializeBottomButtons();
        storyDataManager.LoadStoryFromFile(fileName, sheetIndex);

        if (isLoad)
        {
            RecoverLastBackgroundAndCharacter();
            isLoad = false;
        }

        DisplayNextLine();
    }

    void ShowStoryPanel()
    {
        OpenUI();
        storyPanel.SetActive(true);
    }

    /// <summary>
    /// ????????
    /// </summary>
    public void GoToSheet(int sheetIndex, int lineNumber)
    {
        storyPanel.SetActive(true);
        TriggerNoStoryGameObejcts(false);

        storyDataManager.SetCurrentLine(lineNumber);
        storyDataManager.SetCurrentSheetIndex(sheetIndex);
        storyDataManager.GetReadSheetIndex().Add(sheetIndex);

        storyDataManager.LoadStoryFromFile(storyDataManager.GetCurrentStoryFileName(), sheetIndex);

        if (isLoad)
        {
            RecoverLastBackgroundAndCharacter();
            isLoad = false;
        }
    }

    #endregion

    #region Display Methods

    /// <summary>
    /// ?????
    /// </summary>
    /// 
    public void SkipThisLine()
    {
        NextLineAdd();
        DisplayNextLine();
    }


    public void DisplayNextLine()
    {
        if (choiceTriggered) return;

        storyDataManager.UpdateMaxReadLine();
        UpdateGlobalMaxReachLineIndices();

        var currentData = storyDataManager.GetCurrentData();

        if (StoryDataManager.NotNullNorEmpty(currentData.ID))
        {
            storyTextEffect.DoEffect(currentData.ID);
        }

        if (typewriterEffect.IsTyping())
        {
            typewriterEffect.CompleteLine();
        }
        else
        {
            DisplayThisLine();
        }
    }

    /// <summary>
    /// ?????
    /// </summary>
    public void DisplayThisLine()
    {
        var data = storyDataManager.GetCurrentData();

        UpdateSaveContent(data);

        if (data.storyType != Constants.VOICEOVER)
        {
            uiController.DisplayThisLine(data, MediaController);
        }

        DoEventEffect(data);

        if (data.storyType == Constants.VOICEOVER)
        {
            HandleVoiceover(data);
            return;
        }

        storyDataManager.AdvanceLineIndex(data);
    }

    /// <summary>
    /// ????
    /// </summary>
    private void HandleVoiceover(ExcelPlotData data)
    {
        Debug.Log("HandleVoiceover");
    }

    /// <summary>
    /// ??????
    /// </summary>
    private void UpdateSaveContent(ExcelPlotData data)
    {
        //saveSpeakingContent = !string.IsNullOrWhiteSpace(data.speaker)
        //    ? $"{data.speaker.Trim()}: {StoryUIController.RemoveBracesAndContent(StoryUIController.GetContentText(data.contents))}"
        //    : StoryUIController.RemoveBracesAndContent(StoryUIController.GetContentText(data.contents));
        Debug.LogWarning("Havent do yet");
    }

    /// <summary>
    /// ?????
    /// </summary>
    private void DoEventEffect(ExcelPlotData data)
    {
        if (StoryDataManager.NotNullNorEmpty(data.effect))
        {
            storyTextEffect.DoEffect(data.effect);
        }
    }

    /// <summary>
    /// ???????????
    /// </summary>
    private void RecoverLastBackgroundAndCharacter()
    {
        var storyData = storyDataManager.GetStoryData();
        if (storyData == null || storyData.Count == 0) return;

        int currentLine = storyDataManager.GetCurrentLine();
        if (currentLine <= 0) return;

        var data = storyData[currentLine - 1];

        if (StoryDataManager.NotNullNorEmpty(data.backgroundImageFileName))
        {
            MediaController.UpdateBackgroundImage(data.backgroundImageFileName);
        }

        if (StoryDataManager.NotNullNorEmpty(data.backgroundMusicFileName))
        {
            SoundManager.Instance.PlayBGM(data.backgroundMusicFileName);
        }

        MediaController.UpdateCharacterImages(data, true);
    }

    #endregion

    #region Story

    public void ChangeCharacterImage()
    {
        // MediaController.UpdateCharacterImageByFileName(string pureFileName);
        Debug.LogWarning("havent do in sentence change image yet");
    }

    #endregion

    #region Choice Methods

    /// <summary>
    /// ????
    /// </summary>
    public void ShowChoices(int choicesNum)
    {
        choiceTriggered = true;
        var data = storyDataManager.GetCurrentData();
        NextLineAdd();
        uiController.ShowChoices(choicesNum, data, MediaController);
    }

    /// <summary>
    /// ?????
    /// </summary>
    public void OnChoiceSelected(string Option, string capturedContent)
    {
        choiceTriggered = false;
        DisplayNextLine();
    }

    /// <summary>
    /// ?????
    /// </summary>
    public void OnSkipToChooseButtonClick()
    {
        choiceTriggered = false;
        var storyData = storyDataManager.GetStoryData();

        while (storyDataManager.GetCurrentLine() < storyData.Count && gameObject.activeSelf)
        {
            var data = storyData[storyDataManager.GetCurrentLine()];

            if (choiceTriggered)
            {
                break;
            }

            RecordHistory(data.speaker, (StoryUIController.GetContentText(data.contents)), data.avatarImageFileName);
            storyDataManager.AdvanceLineIndex(data);
        }

        if (storyDataManager.GetCurrentLine() < storyData.Count && storyData[storyDataManager.GetCurrentLine()].ID != Constants.END_OF_STORY)
        {
            DisplayNextLine();
        }
    }

    #endregion

    #region Button Click Handlers

    /// <summary>
    /// ????????
    /// </summary>
    public void OnAutoButtonClick()
    {
        inputHandler.OnAutoButtonClick();
    }

    /// <summary>
    /// ????????
    /// </summary>
    public void OnQuickSaveButtonClick()
    {
        ShowOrCloseActive(false);
        StorySaveData storySaveData = GetSaveStoryData();
        LoadPanelManage.Instance.QuickSaveGame();
        ShowOrCloseActive(true);
    }

    /// <summary>
    /// ??????
    /// </summary>
    public void OnLoadButtonClick()
    {
        ShowOrCloseActive(false);
        LoadPanelManage.OnHidePanel += RestoreSaveLoadClose;
        LoadPanelManage.Instance.ShowLoadPanel();
    }

    /// <summary>
    /// ??????
    /// </summary>
    public void OnHistoryButtonClick()
    {
        CloseUI();
        historyManager.ShowHistory();
    }

    /// <summary>
    /// ??????
    /// </summary>
    public void OnSkipButtonClick()
    {
        inputHandler.OnSkipButtonClick();
    }

    /// <summary>
    /// ??????????
    /// </summary>
    public void OnSkipNodeButtonClick()
    {
        inputHandler.OnSkipNodeButtonClick();
    }

    /// <summary>
    /// ??????????
    /// </summary>
    public void OnStoryReminderCheckButtonClick(string fileName)
    {
        storyDataManager.Initialize(Constants.DEFAULT_START_LINE, Constants.DEFAULT_SHEET_INDEX);
        inputHandler.SetIsSkipAll(true);
        storyDataManager.LoadStoryFromFile(fileName, Constants.DEFAULT_SHEET_INDEX);
        saveLoadManager.SaveProgress(fileName, storyDataManager.GetMaxReadLine());

        var storyData = storyDataManager.GetStoryData();
        while (storyDataManager.GetCurrentLine() < storyData.Count && inputHandler.GetIsSkipAll())
        {
            var data = storyData[storyDataManager.GetCurrentLine()];
            storyDataManager.AdvanceLineIndex(data);
        }

        MarkStoryAsCompleted(fileName);
        TriggerNoStoryGameObejcts(true);
    }

    #endregion

    #region UI Control

    public void CloseStoryPanel()
    {
        storyPanel.SetActive(false);
    }

    public void OpenUI()
    {
        uiController.OpenUI();
    }
    public void CloseUI()
    {
        //if (inputHandler.GetIsSkip()) return;
        uiController.CloseUI();
    }

    bool IsShowText()
    {
        return uiController.IsShowText();
    }

    public void TriggerNoStoryGameObejcts(bool active)
    {
        foreach (var obj in noAboutStorys)
        {
            obj.SetActive(active);
        }
        storyPanel.SetActive(!active);
    }

    #endregion

    #region History & Save

    /// <summary>
    /// ????
    /// </summary>
    private void RecordHistory(string speaker, string content, string iconPath)
    {
        HistoryData data = new HistoryData(speaker, content, iconPath);
        historyManager.RecordHistory(data);
    }

    /// <summary>
    /// ??????
    /// </summary>
    public StorySaveData GetSaveStoryData()
    {
        if (!storyPanel.activeSelf) return null;

        return new StorySaveData(
            storyDataManager.GetCurrentStoryFileName(),
            storyDataManager.GetCurrentLine(),
            storyDataManager.GetCurrentSheetIndex(),
            saveSpeakingContent,
            historyManager.GetHistoryRecords(),
            uiController.GetCurrentPanelTextdatas(),
            storyDataManager.GetReadSheetIndex()
        );
    }

    /// <summary>
    /// ??????
    /// </summary>
    public void LoadStoryData(StorySaveData storySaveData)
    {
        if (storySaveData != null)
        {
            isLoad = true;

            //historyRecords = new LinkedList<HistoryData>(storySaveData.savedHistoryRecords);
            //if (historyRecords.Count > 0)
            //    historyRecords.RemoveLast();

            var lineNumber = storySaveData.savedLine - 1;
            var sheetIndexNumber = storySaveData.saveSheetIndex;

            uiController.ClearPanelText();
            uiController.SetCurrentPanelTextdatas(new LinkedList<PanelSotryData>(storySaveData.savedPanelSotryDatas));

            List<PanelSotryData> tempList = storySaveData.savedPanelSotryDatas;

            storyDataManager.LoadStoryFromFile(storySaveData.savedStoryFileName, storySaveData.saveSheetIndex);
            var storyData = storyDataManager.GetStoryData();

            if (storyData[lineNumber].storyType == Constants.PANEL)
            {
                for (int i = 0; i < tempList.Count; i++)
                {
                    // ??????UI
                }
            }

            InitializeAndLoadStory(storySaveData.savedStoryFileName, lineNumber, sheetIndexNumber);
            storyDataManager.SetReadSheetIndex(storySaveData.saveReadSheet);
        }
    }

    /// <summary>
    /// ????????
    /// </summary>
    private void UpdateGlobalMaxReachLineIndices()
    {
        var fileName = storyDataManager.GetCurrentStoryFileName();
        var sheetIndex = storyDataManager.GetCurrentSheetIndex();
        var maxLines = storyDataManager.GetMaxReadLine();

        saveLoadManager.SaveProgress(fileName, maxLines);
    }

    /// <summary>
    /// ????
    /// </summary>
    public void SaveProgress()
    {
        var fileName = storyDataManager.GetCurrentStoryFileName();
        var maxLines = storyDataManager.GetMaxReadLine();
        saveLoadManager.SaveProgress(fileName, maxLines);
    }


    public void MarkStoryAsCompleted(string fileName)
    {
        saveLoadManager.MarkStoryAsCompleted(fileName);
    }

    #endregion

    #region UI State Management

    /// <summary>
    /// ??SaveLoadClose??
    /// </summary>
    private void RestoreSaveLoadClose()
    {
        ShowOrCloseActive(true);
        LoadPanelManage.OnHidePanel -= RestoreSaveLoadClose;
    }

    /// <summary>
    /// ?????????
    /// </summary>
    private void ShowOrCloseActive(bool isShow)
    {
        if (isShow)
        {
            for (int i = 0; i < SaveLoadClose.Count; i++)
            {
                SaveLoadClose[i].SetActive(SaveLoadCloseActive.ElementAtOrDefault(i));
            }
        }
        else
        {
            SaveLoadCloseActive = SaveLoadClose.Select(go => go.activeSelf).ToList();
            SaveLoadClose.ForEach(go => go.SetActive(false));
        }
    }

    #endregion

    #region HelperFunction

    void NextLineAdd()
    {
        int nextLine = storyDataManager.GetCurrentLine() + 1;
        storyDataManager.SetCurrentLine(nextLine);
    }

    #endregion

    #region Settings

    /// <summary>
    /// ??????
    /// </summary>
    public void SetSkipUnread(bool value)
    {
        inputHandler.SetSkipUnread(value);
    }

    /// <summary>
    /// ????????
    /// </summary>
    public void SetChangeReadColor(bool value)
    {
        uiController.SetChangeReadColor(value);
    }

    /// <summary>
    /// ??????
    /// </summary>
    public bool GetSkipUnread()
    {
        return inputHandler.GetSkipUnread();
    }

    /// <summary>
    /// ????????
    /// </summary>
    public bool GetChangeReadColor()
    {
        return uiController.GetIsChoice();
    }

    #endregion

    #region Getters

    public bool GetChoiceTriggered() => choiceTriggered;
    public string GetCurrentStoryFileName() => storyDataManager.GetCurrentStoryFileName();
    public int GetCurrentLine() => storyDataManager.GetCurrentLine();
    public List<ExcelPlotData> GetStoryData() => storyDataManager.GetStoryData();
    public List<int> GetReadSheetIndex() => storyDataManager.GetReadSheetIndex();
    public bool GetIsSkipAll() => inputHandler.GetIsSkipAll();

    #endregion

    #region Setters
    public void SetIsSkipAll(bool value) => inputHandler.SetIsSkipAll(value);

    public void SetChoiceTriggered(bool value) => choiceTriggered = value;
    public void SetCurrentLine(int line) => storyDataManager.SetCurrentLine(line);

    #endregion

    #region IS

    public bool IsSkipAll()
    {
        return inputHandler.GetIsSkipAll();
    }
    public bool IsPointerOverStoryLayer()
    {
        return inputHandler.IsPointerOverStoryLayer();
    }

    public bool IsFileCompleted(string fileName)
    {
        saveLoadManager.LoadOrCreateProgress();
        string progressFilePath = Path.Combine(Application.persistentDataPath, Constants.SAVE_FILE_PATH, "story_progress.json");
        string json = File.ReadAllText(progressFilePath);
        TotalStoryProgress totalStoryProgress = JsonUtility.FromJson<TotalStoryProgress>(json);

        if (totalStoryProgress == null || totalStoryProgress.progressList == null)
        {
            Debug.Log("here");
            return false;
        }

        foreach (var progress in totalStoryProgress.progressList)
        {
            if (progress.fileName == fileName)
            {
                return progress.storyCompleted;
            }
        }

        return false;
    }

    #endregion
}
