using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Newtonsoft.Json;
using Unity.VisualScripting;
using static GetSprite;
using System.Linq;
using System;
using UnityEngine.SceneManagement;

public class SaveConstants
{
    public static string TURNBEGIN = "Save_TurnBegin";
    public static string BATTLEBEGIN = "Save_BattleBegin";

    public static string EXPLOREBEGIN = "Save_ExploreBegin";
    public static string IMPORTANTCHOICES = "Save_ImportantChocies";
    public static string TURNEND = "Save_TurnEnd";


}

public class LoadPanelManage : MonoBehaviour
{
    public GameObject LoadPanel;
    public TextMeshProUGUI autoTitle;
    public Button backButton;
    public Image saveListBackground;
    private List<Sprite> saveListBackgroundSprites = new List<Sprite>();
    public static LoadPanelManage Instance { get; private set; }

    [Header("TotalSave")]
    public TotalSave totalSave;

    public enum SaveType
    {
        Normal,
        Quick,
        Auto
    }

    public SaveType saveType;

    [System.Serializable]
    #region TotalSave
    public class TotalSave
    {
        public TextMeshProUGUI saveTitle;
        public ScrollRect scrollRect;

        // ???????????????? UI?
        public List<SaveData> currentQuickSaves = new List<SaveData>();
        public List<SaveData> currentSaves = new List<SaveData>();

        // UI ?? Prefab
        public GameObject saveButtonPrefab;
        
        // ????
        public Button showQuickSaveButton;
        public Button showNormalSaveButton;

        public Button timeSortButton;
        public Button achievementSortButton;
        public Button starButton;
        public Button deleteButton;
        public bool isStar;

        public enum SortState { None, Desc,Asc}
        public SortState timeSortState = SortState.None;
        public SortState achievementSortState = SortState.None;

        public bool HasSave()
        {
            bool HasQuickSave = currentQuickSaves.Count != 0;
            bool HasCurrentSaves = currentSaves.Count != 0;

            return HasQuickSave || HasCurrentSaves;
        }

        public void InitButtons()
        {
            showQuickSaveButton.onClick.AddListener(ShowQuickSave);
            showNormalSaveButton.onClick.AddListener(ShowNormalSave);
            starButton.onClick.AddListener(OnStarButtonClick);
            timeSortButton.onClick.AddListener(OnTimeSortClick);
            achievementSortButton.onClick.AddListener(OnAchievementSortClick);
            deleteButton.onClick.AddListener(OnDeleteButtonClick);

        }


        void OnStarButtonClick()
        {
            isStar = !isStar;
            SetStarButton(isStar);
            LoadPanelManage.Instance.ApplyStarFilter();
        }

        void OnDeleteButtonClick()
        {
            LoadPanelManage.Instance.DeleteAllUnstarButton();
        }


        public void SetStarButton(bool isStar)
        {
            this.isStar = isStar;
            starButton.image.sprite = UpStarButtonSprite(isStar);
        }

        void ShowQuickSave()
        {
            if (LoadPanelManage.Instance.saveType == SaveType.Quick) return;
            string quickSavePath = LoadPanelManage.Instance.GetQuickSavePath();
            LoadPanelManage.Instance.LoadAllSaves(quickSavePath, SaveType.Quick);
            LoadPanelManage.Instance.RefreshSaveButtons(SaveType.Quick);
            SavePanelInfo.Instance.SetSaveData(null);
            LoadPanelManage.Instance.ResetSaveBackGround();

        }


        void ShowNormalSave()
        {
            if (LoadPanelManage.Instance.saveType == SaveType.Normal) return;
            string normalSavePath = LoadPanelManage.Instance.GetNormalSavePath();
            LoadPanelManage.Instance.LoadAllSaves(normalSavePath, SaveType.Normal);
            LoadPanelManage.Instance.RefreshSaveButtons(SaveType.Normal);
            SavePanelInfo.Instance.SetSaveData(null);
            LoadPanelManage.Instance.ResetSaveBackGround();
        }

        void OnTimeSortClick()
        {
            timeSortState = (SortState)(((int)timeSortState + 1) % 3);
            // ???????
            achievementSortState = SortState.None;
            UpdateSortButtonSprites();
            LoadPanelManage.Instance.ApplySort();
        }

        void OnAchievementSortClick()
        {
            achievementSortState = (SortState)(((int)achievementSortState + 1) % 3);
            timeSortState = SortState.None;
            UpdateSortButtonSprites();
            LoadPanelManage.Instance.ApplySort();
        }

        public void UpdateSortButtonSprites()
        {
            // ??????
            switch (timeSortState)
            {
                case SortState.None:
                    timeSortButton.image.sprite = GetSortSprite("none");
                    break;
                case SortState.Asc:
                    timeSortButton.image.sprite = GetSortSprite("ascending");
                    break;
                case SortState.Desc:
                    timeSortButton.image.sprite = GetSortSprite("descending");
                    break;
            }

            // ??????
            switch (achievementSortState)
            {
                case SortState.None:
                    achievementSortButton.image.sprite = GetSortSprite("none");
                    break;
                case SortState.Asc:
                    achievementSortButton.image.sprite = GetSortSprite("ascending");
                    break;
                case SortState.Desc:
                    achievementSortButton.image.sprite = GetSortSprite("descending");
                    break;
            }
        }


    }
    #endregion
    public SaveAndLoadButtonControl turnBeginAutoSaveButton;
    public SaveAndLoadButtonControl battleBeginAutoSaveButton;
    public SaveAndLoadButtonControl exploreAutoSaveButton;
    public SaveAndLoadButtonControl importantChoicesSaveButton;
    public SaveAndLoadButtonControl turnEndAutoSaveButton;

    public SavePanelInfo savePanelInfo;

    // ????
    private string saveFolderPath;
    private string normalSavePath;
    private string quickSavePath;
    private string autoSavePath;

    public string GetNormalSavePath() => normalSavePath;
    public string GetQuickSavePath() => quickSavePath;
    public string GetAutoSavePath() => autoSavePath;

    public static event Action OnHidePanel;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        InitializeSaveFolderPath();
        totalSave.InitButtons();
        backButton.onClick.AddListener(CloseLoadPanel);

        for (int i = 0; i < 4; i++) {
            int index = i;
            saveListBackgroundSprites.Add(GetSeasonSpriteCol((index)));
        }

    }   

    private void Update()
    {
        // If the load panel is open, skip input checks
        if (!LoadPanel.activeSelf)
        {
            if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))&& Input.GetKeyDown(KeyCode.X))
            {
                QuickSaveGame();
            }

            if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && Input.GetKeyDown(KeyCode.L))
            {
                QuickLoadGame();
            }

        }

        if (LoadPanel.activeSelf)
        {
            if (Input.GetMouseButtonDown(1))
            {
                CloseLoadPanel();
            }
        }


    }
    public bool HasSave()
    {
        bool hasTotalSave = totalSave != null && totalSave.HasSave();

        bool hasAutoSave =
            (turnBeginAutoSaveButton != null && turnBeginAutoSaveButton.HasSaveData()) ||
            (battleBeginAutoSaveButton != null && battleBeginAutoSaveButton.HasSaveData()) ||
            (exploreAutoSaveButton != null && exploreAutoSaveButton.HasSaveData()) ||
            (importantChoicesSaveButton != null && importantChoicesSaveButton.HasSaveData()) ||
            (turnEndAutoSaveButton != null && turnEndAutoSaveButton.HasSaveData());

        return hasTotalSave || hasAutoSave;
    }


    public void QuickLoadGame()
    {

    }


    /// <summary>
    /// Perform a quick save operation.
    /// </summary>

    #region SaveFolderPath
    void InitializeSaveFolderPath()
    {
        saveFolderPath = Path.Combine(Application.persistentDataPath, Constants.SAVE_FILE_PATH);

        normalSavePath = Path.Combine(saveFolderPath, "Normal");
        quickSavePath = Path.Combine(saveFolderPath, "Quick");
        autoSavePath = Path.Combine(saveFolderPath, "Auto");

        CreateFolderIfNotExists(saveFolderPath);
        CreateFolderIfNotExists(normalSavePath);
        CreateFolderIfNotExists(quickSavePath);
        CreateFolderIfNotExists(autoSavePath);
    }

    private void CreateFolderIfNotExists(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
    }

    public string GetSavePath(SaveType type)
    {
        return type switch
        {
            SaveType.Normal => normalSavePath,
            SaveType.Quick => quickSavePath,
            SaveType.Auto => autoSavePath,
            _ => saveFolderPath,
        };
    }
    #endregion

    #region Save Load Logic
    /// <summary>
    /// ??????????? UI
    /// </summary>
    public void LoadAllSaves(string folderPath, SaveType type)
    {
        if (!Directory.Exists(folderPath))
        {
            Debug.LogWarning($"Save folder not found: {folderPath}");
            return;
        }

        saveType = type;

        string[] files = Directory.GetFiles(folderPath, "*.json");

        if (type == SaveType.Normal)
        {
            totalSave.saveTitle.text = "PLAYER SAVE";
            totalSave.currentSaves.Clear();

        }
        else if (type == SaveType.Quick)
        {
            totalSave.saveTitle.text = "QUICK SAVE";
            totalSave.currentQuickSaves.Clear();
            
        }

        foreach (string file in files)
        {
                string json = File.ReadAllText(file);
                SaveData saveData = JsonConvert.DeserializeObject<SaveData>(json);

                if (type == SaveType.Normal)
                    totalSave.currentSaves.Add(saveData);
                else if (type == SaveType.Quick)
                    totalSave.currentQuickSaves.Add(saveData);
        }
    }


    void LoadAutoButtons()
    {
        var path = GetAutoSavePath();

        if (!Directory.Exists(path))
        {
            Debug.LogWarning($"Auto save folder not found: {path}");
            return;
        }
        turnBeginAutoSaveButton.SetSaveData(null);
        battleBeginAutoSaveButton.SetSaveData(null);
        exploreAutoSaveButton.SetSaveData(null);
        importantChoicesSaveButton.SetSaveData(null);
        turnEndAutoSaveButton.SetSaveData(null);

        string[] files = Directory.GetFiles(path, "*.json");

        foreach (string file in files)
        {
            string json = File.ReadAllText(file);
            SaveData saveData = JsonConvert.DeserializeObject<SaveData>(json);

            string fileName = Path.GetFileNameWithoutExtension(file);

            switch (fileName)
            {
                case "Save_TurnBegin":
                    turnBeginAutoSaveButton.SetSaveData(saveData);
                    break;
                case "Save_BattleBegin":
                    battleBeginAutoSaveButton.SetSaveData(saveData);
                    break;
                case "Save_ExploreBegin":
                    exploreAutoSaveButton.SetSaveData(saveData);
                    break;
                case "Save_ImportantChoices":
                    importantChoicesSaveButton.SetSaveData(saveData);
                    break;
                case "Save_TurnEnd":
                    turnEndAutoSaveButton.SetSaveData(saveData);
                    break;
                default:
                    Debug.LogWarning($"Unrecognized auto save file: {fileName}");
                    break;
            }
        }
    }



    SaveData LoadSaveDataFromFile(string filePath)
    {
        if (!File.Exists(filePath))
        {
            return null;
        }

        string json = File.ReadAllText(filePath);
        SaveData saveData = JsonConvert.DeserializeObject<SaveData>(json);
        return saveData;
    }
    /// <summary>
    /// ?? content ??????? UI
    /// </summary>
    public void RefreshSaveButtons(SaveType type)
    {
        foreach (Transform child in totalSave.scrollRect.content)
        {
            Destroy(child.gameObject);
        }

        var savesToShow = type == SaveType.Quick ? totalSave.currentQuickSaves : totalSave.currentSaves;

        for (int i = savesToShow.Count - 1; i >= 0; i--)
        {
            var saveData = savesToShow[i];
            GameObject buttonObj = Instantiate(totalSave.saveButtonPrefab, totalSave.scrollRect.content);
            SaveAndLoadButtonControl buttonCtrl = buttonObj.GetComponent<SaveAndLoadButtonControl>();

            if (buttonCtrl != null)
            {
                buttonCtrl.SetSaveData(saveData);
            }
        }
    }
    #endregion

    #region Save 

    public void NormalSaveGame()
    {
        bool success = SaveGame(normalSavePath);
        if (success)
            //NotificationManage.Instance.ShowToTop("Game Saved Successfully");
            NotificationManage.Instance.ShowToTopByKey(NotificationKeyConstants.Game_SaveSuccess);

        else
            //NotificationManage.Instance.ShowToTop("Save Failed!");
            NotificationManage.Instance.ShowToTopByKey(NotificationKeyConstants.Game_SaveFail);

    }

    public void QuickSaveGame()
    {
        bool success = SaveGame(quickSavePath);
        if (success)
            //NotificationManage.Instance.ShowToTop("Quick Save Successful");
            NotificationManage.Instance.ShowToTopByKey(NotificationKeyConstants.QuickSave_Success);

        else
            //NotificationManage.Instance.ShowToTop("Quick Save Failed!");
            NotificationManage.Instance.ShowToTopByKey(NotificationKeyConstants.QuickSave_Fail);
    }

    public void AutoSaveGameTurnBegin()
    {
        bool success = SaveGame(autoSavePath, SaveConstants.TURNBEGIN);
    }
    public void AutoSaveGameBattleBegin()
    {
        bool success = SaveGame(autoSavePath, SaveConstants.BATTLEBEGIN);
    }

    public void AutoSaveGameExploreBegin()
    {
        Debug.Log("AutoSaveGameExploreBegin save");
        bool success = SaveGame(autoSavePath, SaveConstants.EXPLOREBEGIN);
    }
    public void AutoSaveGameImportantChocies()
    {
        bool success = SaveGame(autoSavePath, SaveConstants.IMPORTANTCHOICES);
    }
    public void AutoSaveGameTurnEnd()
    {
        bool success = SaveGame(autoSavePath, SaveConstants.TURNEND);
    }

    bool SaveGame(string saveFolder, string SpecifyName = null)
    {
        string fileName = $"Save_{System.DateTime.Now:yyyyMMdd_HHmmss}.json";
        if (string.IsNullOrEmpty(SpecifyName))
        {
            fileName = $"Save_{System.DateTime.Now:yyyyMMdd_HHmmss}.json";
        } else
        {
            fileName = $"{SpecifyName}.json";
        }
        string fullPath = Path.Combine(saveFolder, fileName);
        SaveData saveData = GetCurrentSaveData(saveFolder, SpecifyName);
        try
        {
            string json = JsonConvert.SerializeObject(saveData, Formatting.Indented);
            File.WriteAllText(fullPath, json);
            Debug.Log($"Game saved successfully at {fullPath}");

            return true;

        }
        catch (System.Exception ex)
        {
            Debug.LogError($"SaveGame failed: {ex.Message}");
            return false;
        }
    }

    public SaveData GetCurrentSaveData(string saveFolder = null, string SpecifyName = null)
    {
        Debug.Log("=== GetCurrentSaveData Started ===");

        // 步骤 1: 捕获截图
        Debug.Log("Step 1: Capturing screenshot...");
        if (ScreenShotter.Instance == null)
        {
            Debug.LogError("❌ ScreenShotter.Instance is null!");
            return null;
        }

        Texture2D screenshot = ScreenShotter.Instance.CaptureScreenshot();
        if (screenshot == null)
        {
            Debug.LogError("❌ Screenshot is null! Check ScreenShotter.CaptureScreenshot()");
            return null;
        }
        Debug.Log($"✅ Screenshot captured: {screenshot.width}x{screenshot.height}");

        // 步骤 2: 编码截图
        Debug.Log("Step 2: Encoding screenshot to PNG...");
        byte[] screenshotData = screenshot.EncodeToPNG();
        if (screenshotData == null || screenshotData.Length == 0)
        {
            Debug.LogError("❌ Screenshot encoding failed!");
            return null;
        }
        Debug.Log($"✅ Screenshot encoded: {screenshotData.Length} bytes");

        // 步骤 3: 获取故事数据
        Debug.Log("Step 3: Getting story save data...");
        StorySaveData storySaveData = null;
        if (TotalStoryManager.Instance != null)
        {
            storySaveData = TotalStoryManager.Instance.GetSaveStoryData();
            Debug.Log($"✅ Story data retrieved: {(storySaveData != null ? "Success" : "Null")}");
        }
        else
        {
            Debug.LogWarning("⚠️ TotalStoryManager.Instance is null");
        }

        // 步骤 4: 获取面板数据
        Debug.Log("Step 4: Getting panel save data...");
        TotalPanelSaveData totalPanelSaveData = null;
        if (BottomButton.Instance != null)
        {
            totalPanelSaveData = BottomButton.Instance.SavePanelData();
            Debug.Log($"✅ Panel data retrieved: {(totalPanelSaveData != null ? "Success" : "Null")}");
        }
        else
        {
            Debug.LogWarning("⚠️ BottomButton.Instance is null");
        }

        // 步骤 5: 获取游戏数值数据
        Debug.Log("Step 5: Getting GameValue save data...");
        if (GameValue.Instance == null)
        {
            Debug.LogError("❌ GameValue.Instance is null!");
            return null;
        }

        GameValueSaveData gvData = GameValue.Instance.GetGameValueSaveData();
        if (gvData == null)
        {
            Debug.LogError("❌ GameValueData is null, cannot save!");
            Debug.LogError("   Check GameValue.GetGameValueSaveData() implementation");
            return null;
        }
        Debug.Log("✅ GameValue data retrieved successfully");

        // 步骤 6: 生成文件名和路径
        Debug.Log("Step 6: Generating filename and path...");
        string saveTime = System.DateTime.Now.ToString("G");
        string fileName;

        if (string.IsNullOrEmpty(SpecifyName))
        {
            fileName = $"Save_{System.DateTime.Now:yyyyMMdd_HHmmss}.json";
        }
        else
        {
            fileName = $"{SpecifyName}.json";
        }

        string fullPath = saveFolder != null ? Path.Combine(saveFolder, fileName) : SettingConstant.SettingFilePath;
        Debug.Log($"✅ Save path: {fullPath}");

        // 步骤 7: 创建 SaveData 对象
        Debug.Log("Step 7: Creating SaveData object...");
        try
        {
            var currentSaveData = new SaveData(screenshotData, storySaveData, gvData, totalPanelSaveData, saveTime, fullPath);

            if (currentSaveData == null)
            {
                Debug.LogError("❌ SaveData constructor returned null!");
                return null;
            }

            Debug.Log("✅ SaveData created successfully");
            Debug.Log("=== GetCurrentSaveData Completed Successfully ===");
            return currentSaveData;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"❌ Exception while creating SaveData: {ex.Message}");
            Debug.LogError($"   Stack trace: {ex.StackTrace}");
            return null;
        }
    }



    #endregion

    public void ShowLoadPanel()
    {
        LoadPanel.gameObject.SetActive(true);
        totalSave.SetStarButton(false);
        SavePanelInfo.Instance.SetSaveData(null);
        totalSave.timeSortState = TotalSave.SortState.None;
        totalSave.achievementSortState = TotalSave.SortState.None;
        totalSave.UpdateSortButtonSprites();
        ResetSaveBackGround();
        // ???? Normal ??
        LoadAutoButtons();
        LoadAllSaves(normalSavePath, SaveType.Normal);
        RefreshSaveButtons(SaveType.Normal);
    }

    void CloseLoadPanel()
    {
        LoadPanel.gameObject.SetActive(false);
    }


    #region remove and Star logic

    public void SetStar(SaveData saveData)
    {
        if (totalSave.currentSaves.Contains(saveData))
        {
            totalSave.currentSaves.Remove(saveData);
        }

        if (totalSave.currentQuickSaves.Contains(saveData))
        {
            totalSave.currentQuickSaves.Remove(saveData);
        }

        foreach (Transform child in totalSave.scrollRect.content)
        {
            SaveAndLoadButtonControl buttonCtrl = child.GetComponent<SaveAndLoadButtonControl>();
            if (buttonCtrl != null && buttonCtrl.GetSaveData() == saveData)
            {
                buttonCtrl.OnStarButtonClick();
                break;
            }
        }
    }


    public void RemoveSaveData(SaveData saveData)
    {
        if (totalSave.currentSaves.Contains(saveData))
        {
            totalSave.currentSaves.Remove(saveData);
        }

        if (totalSave.currentQuickSaves.Contains(saveData))
        {
            totalSave.currentQuickSaves.Remove(saveData);
        }

        foreach (Transform child in totalSave.scrollRect.content)
        {
            SaveAndLoadButtonControl buttonCtrl = child.GetComponent<SaveAndLoadButtonControl>();
            if (buttonCtrl != null && buttonCtrl.GetSaveData() == saveData)
            {
                Destroy(child.gameObject); 
                break;
            }
        }
    }

    public void DeleteAllUnstarButton()
    {
        List<Transform> buttonsToRemove = new List<Transform>();

        foreach (Transform child in totalSave.scrollRect.content)
        {
            SaveAndLoadButtonControl buttonCtrl = child.GetComponent<SaveAndLoadButtonControl>();
            if (buttonCtrl != null && !buttonCtrl.IsStar())
            {
                // ???? saveType ???????
                if (saveType == SaveType.Normal)
                {
                    totalSave.currentSaves.Remove(buttonCtrl.GetSaveData());
                }
                else if (saveType == SaveType.Quick)
                {
                    totalSave.currentQuickSaves.Remove(buttonCtrl.GetSaveData());
                }

                buttonCtrl.DeleteFile();
                buttonsToRemove.Add(child);
            }
        }

        if (buttonsToRemove.Count == 0)
        {
            //NotificationManage.Instance.ShowAtTop("No UnStar Save to delete");
            NotificationManage.Instance.ShowAtTopByKey(NotificationKeyConstants.Save_NoUnstar);

            return;
        }

        foreach (var button in buttonsToRemove)
        {
            Destroy(button.gameObject);
        }

        //NotificationManage.Instance.ShowAtTop("Deleted All UnStar Save");
        NotificationManage.Instance.ShowAtTopByKey(NotificationKeyConstants.Save_DeletedAll);

    }
    #endregion

    #region sort logic
    public void ApplyStarFilter()
    {
        bool filterOn = totalSave.isStar;
        var content = totalSave.scrollRect.content;

        foreach (Transform child in content)
        {
            var ctrl = child.GetComponent<SaveAndLoadButtonControl>();

            bool isThisStarred = (ctrl != null) && ctrl.IsStar();
            bool shouldShow = !filterOn || isThisStarred;

            if (child.gameObject.activeSelf != shouldShow)
                child.gameObject.SetActive(shouldShow);
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(content);
    }

    public void ApplySort()
    {
        var savesToShow = saveType == SaveType.Quick ? totalSave.currentQuickSaves : totalSave.currentSaves;

        // ??????? (? SaveTime ???Desc)
        IEnumerable<SaveData> sorted = savesToShow.OrderByDescending(s => s.GetSaveDateTime());

        if (totalSave.timeSortState == TotalSave.SortState.Desc)
        {
            sorted = savesToShow.OrderByDescending(s => s.GetSaveDateTime());
        }
        else if (totalSave.timeSortState == TotalSave.SortState.Asc)
        {
            sorted = savesToShow.OrderBy(s => s.GetSaveDateTime());
        }
        else if (totalSave.achievementSortState == TotalSave.SortState.Desc)
        {
            sorted = savesToShow.OrderByDescending(s => s.GetAchievement());
        }
        else if (totalSave.achievementSortState == TotalSave.SortState.Asc)
        {
            sorted = savesToShow.OrderBy(s => s.GetAchievement());
        }
        foreach (Transform child in totalSave.scrollRect.content)
        {
            Destroy(child.gameObject);
        }

        foreach (var saveData in sorted)
        {
            GameObject buttonObj = Instantiate(totalSave.saveButtonPrefab, totalSave.scrollRect.content);
            SaveAndLoadButtonControl buttonCtrl = buttonObj.GetComponent<SaveAndLoadButtonControl>();

            if (buttonCtrl != null)
            {
                buttonCtrl.SetSaveData(saveData);
            }
        }
    }
    #endregion

    #region SaveBackground

    public void SetSaveBackGround(int year, int season)
    {
        Debug.Log("may need to set the sprite according to different years");
        saveListBackground.sprite = saveListBackgroundSprites[season];
    }

    public void ResetSaveBackGround()
    {
        GameValue gameValue = GameValue.Instance;
        Debug.Log("maybe need to change gameValue,Because gameValue may always exist");
        if (gameValue != null) {
            saveListBackground.sprite = saveListBackgroundSprites[(int)gameValue.GetCurrentSeason()];
        } else
        {
            int month = DateTime.Now.Month;
            int seasonIndex = GetSeasonIndexByMonth(month);
            saveListBackground.sprite = saveListBackgroundSprites[seasonIndex];
        }
    }

    private int GetSeasonIndexByMonth(int month)
    {
        if (month >= 3 && month <= 5) return 0; // spring
        if (month >= 6 && month <= 8) return 1; // summer
        if (month >= 9 && month <= 11) return 2; // fall
        return 3; // winter?12,1,2?
    }

    public void LoadGame(string savePath)
    {
        LoadGameByPath(savePath);
    }

    public void LoadGame(SaveData saveData)
    {
        if (saveData == null) return;
        LoadGameByPath(saveData.savePath);
    }

    void LoadGameByPath(string savePath)
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            SaveData saveData = JsonConvert.DeserializeObject<SaveData>(json);
            SettingsManager.Instance.SetPlayerName(saveData.gameValueData.GetPlayerName());
            SceneTransferManager.Instance.LoadScene(Scene.GameScene, saveData);
        }
    }
    #endregion
}
