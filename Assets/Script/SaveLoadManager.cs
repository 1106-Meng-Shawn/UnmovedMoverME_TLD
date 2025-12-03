using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Unity.VisualScripting;
using System;
using System.Runtime.CompilerServices;
using UnityEngine.EventSystems;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEditor;
using static GetSprite;


public class SaveLoadManager : MonoBehaviour
{
 /*   public static SaveLoadManager Instance { get; private set; }

    public GameObject saveLoadPanel;
    public GameObject settingMenu;
    public List<GameObject> noAboutSaveUI;

    public TextMeshProUGUI panelTitle;


    public Button SaveOrLoadButton;
    public Button[] saveLoadButtons;
    public Button[] pageButtons;

    public Button backButton;

    private bool isSave;
    private int currentPage = Constants.DEFAULT_START_INDEX;
    private readonly int slotsPerPage = Constants.SLOTS_PER_PAGE;
    private readonly int totalSlots = Constants.TOTAL_SLOTS;

    private string saveFolderPath;


    public GameValue gameValue;

    public NormalStoryControl normalStoryControl;
   // public static SaveLoadManager Instance { get; private set; }
    public bool IsConsuming = false;

    private byte[] screenshotData;
    private StorySaveData storySaveData;

    public ScreenShotter screenShotter;


    public SavePanelInfo savePanelInfo;

    public InputPanelControl inputPanelControl;


    public Image backGroundImage;
    public List<Sprite> backGroundSprites;

    public GameObject feedbackPrefab;

    //
    public static event Action OnHidePanel;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }


    private void Start()
    {

        if (screenShotter == null) screenShotter = FindObjectOfType<ScreenShotter>();
        if (gameValue == null) gameValue = FindObjectOfType<GameValue>();

        for (int i = 0; i < pageButtons.Length; i++)
        {
            int page = i;
            pageButtons[i].onClick.AddListener(() => ChangeThePage(page));

        }


        backButton.onClick.AddListener(GoBack);

        SaveOrLoadButtonInit();

    }


    void SaveOrLoadButtonInit()
    {
        if (SaveOrLoadButton != null) {
            SaveOrLoadButton.onClick.AddListener(GoToSaveOrLoad);
        }
    }

    void SetSaveSprite()
    {
        if (SaveOrLoadButton == null) return;
        Debug.Log("SetSaveSprite work");
        SaveOrLoadButton.gameObject.GetComponent<Image>().sprite = GetSpriteAtUIOther("Save");
        SaveOrLoadButton.gameObject.GetComponent<ButtonEffect>().SetChangeSprite(GetSpriteAtUIOther("SaveSel"), GetSpriteAtUIOther("Save"));
    }

    void SetLoadSprite()
    {
        if (SaveOrLoadButton == null) return;
        Debug.Log("SetLoadSprite work");
        SaveOrLoadButton.gameObject.GetComponent<Image>().sprite = GetSpriteAtUIOther("Load");
        SaveOrLoadButton.gameObject.GetComponent<ButtonEffect>().SetChangeSprite(GetSpriteAtUIOther("LoadSel"), GetSpriteAtUIOther("Load"));
    }


    void ChangeTheBackGround()

    {
        int idx = UnityEngine.Random.Range(0, backGroundSprites.Count);
        backGroundImage.sprite = backGroundSprites[idx];


    }

    void ChangeThePage(int i)
    {
        currentPage = i;
        UpdateUI();

    }

    private void Update()
    {
        IsConsuming = false;

            if (Input.GetMouseButtonDown(1) && saveLoadPanel.activeSelf)
            {

            if (inputPanelControl.gameObject.activeSelf) return;
                GoBack();
                IsConsuming = true; 
            }
        
    }

    public void GoToSaveOrLoad()
    {
        if (isSave)
        {
            ShowLoadPanel();
        }
        else
        {
            ShowSavePanel();
        }

    }



    void InitializeSaveFildPath()
    {
        saveFolderPath = Path.Combine(Application.persistentDataPath, Constants.SAVE_FILE_PATH);
        if (!Directory.Exists(saveFolderPath))
        {
            Directory.CreateDirectory(saveFolderPath);
        }
    }


    public void ShowSavePanel()
    {
        isSave = true;
        SetLoadSprite();
        panelTitle.text = Constants.SAVE_GAME;
        InitializeSaveFildPath();
        UpdateUI();
        saveLoadPanel.SetActive(true);
        savePanelInfo.SaveOrLoad(true);

    }



    public void ShowLoadPanel()
    {
        isSave = false;
        SetSaveSprite();
        panelTitle.text = Constants.LOAD_GAME;
        UpdateUI();
        saveLoadPanel.SetActive(true);
        savePanelInfo.SaveOrLoad(false);

    }

    private IEnumerator OnButtonHover(Button button)
    {
        yield return new WaitForSeconds(0.75f);

        var control = button.GetComponent<SaveAndLoadButtonControl>();
        SaveData saveData = control.GetSaveData();
        if (saveData != null)
        {
            savePanelInfo.OnTheButton(control.GetSaveData());
        }
    }


    public void SaveNeedDataToSaveLoadManager(StorySaveData storySaveData)
    {
        Texture2D screenshot = screenShotter.CaptureScreenshot();
        screenshotData = screenshot.EncodeToPNG();
        this.storySaveData = storySaveData;
    }

    private void UpdateUI()
    {
        if (isSave)
        {
            if (currentPage == 0)
            {
                // panelTitle.text = "Auto_Save_Game";
                panelTitle.text = "AUTO SAVE";

            }
            else if (currentPage == 1)
            {
                //  panelTitle.text = "Quick_Save_Game";
                panelTitle.text = "QUICK SAVE";

            }
            else
            {
                //  panelTitle.text = Constants.SAVE_GAME;
                panelTitle.text = "SAVE GAME";

            }

        }
        else
        {

            if (currentPage == 0)
            {
                // panelTitle.text = "Auto_Load_Game";
                panelTitle.text = "AUTO LOAD";
            }
            else if (currentPage == 1)
            {
                //  panelTitle.text = "Quick_Load_Game";
                panelTitle.text = "QUICK LOAD";

            }
            else
            {
                // panelTitle.text = Constants.LOAD_GAME;
                panelTitle.text = "LOAD GAME";

            }
        }




        for (int i = 0; i < slotsPerPage; i++)
            {
                int slotIndex = currentPage * slotsPerPage + i;

                if (slotIndex < totalSlots)
                {
                    UpdateSaveLoadButtons(saveLoadButtons[i], slotIndex);
                    LoadSButtonDataAndScreenshots(saveLoadButtons[i], slotIndex);

                }
                else
                {
                    saveLoadButtons[i].gameObject.SetActive(false);
                }
            }

        UpNewSave();

    }


    private void UpdateSaveLoadButtons(Button button, int index)
    {
        button.gameObject.SetActive(true);

        var savePath = GenerateDataPath(index);
        var fileExists = File.Exists(savePath);

        var saveAndLoadButtonControl = button.GetComponent<SaveAndLoadButtonControl>();
        saveAndLoadButtonControl.UpdateEmptySlotInfo(index, currentPage);

        button.GetComponentInChildren<RawImage>().texture = null;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => OnButtonClick(button, index));
        UpdateSaveLoadButtonsAddListener(button);
    }



    private void UpdateSaveLoadButtonsAddListener(Button button)
    {
        EventTrigger trigger = button.GetComponent<EventTrigger>();
        if (trigger == null) trigger = button.gameObject.AddComponent<EventTrigger>();
        trigger.triggers.Clear();


        EventTrigger.Entry entryEnter = new EventTrigger.Entry();
        entryEnter.eventID = EventTriggerType.PointerEnter;
        entryEnter.callback.AddListener((data) => {
            StartCoroutine(OnButtonHover(button));
        });
            trigger.triggers.Add(entryEnter);

        EventTrigger.Entry entryExit = new EventTrigger.Entry();
        entryExit.eventID = EventTriggerType.PointerExit;
        entryExit.callback.AddListener((data) => {
        StopAllCoroutines(); 
        });
        trigger.triggers.Add(entryExit);
    }


public void OnTopSaveButtonClick()
    {
        Debug.Log("work");
        SaveNeedDataToSaveLoadManager(null);
        ShowSavePanel();

    }

   public void OnSettingSaveButtonClick()
    {
        settingMenu.SetActive(false);

        SaveNeedDataToSaveLoadManager(null);
        settingMenu.SetActive(true);
        ShowSavePanel();
    }

    public void OnSettingLoadButtonClick()
    {
        ShowLoadPanel();
    }


    private void OnButtonClick(Button button, int index)
    {
        if (isSave)
        {
            SaveGame(index);
            LoadSButtonDataAndScreenshots(button, index);
            savePanelInfo.SetSaveData(button.GetComponent<SaveAndLoadButtonControl>().GetSaveData());
        }
        else
        {
            Debug.Log($"OnButtonClick(Button button, int {index})");
            LoadGame(index);
            //GoBack();
        }
        UpdateUI();
    }

    public void SetIsSave(bool isSave)
    {
        this.isSave = isSave;
    }

    public bool GetIsSave()
    {
        return isSave;
    }

    public void RemoveSave(string savePath)
    {
        SettingsManager.Instance.GetSettingValue().RemoveSavePath(savePath);
        UpNewSave();
    }

    public void UpNewSave()
    {
        if (currentPage == 0|| currentPage == 1)
        {
            return;
        }
        string lastPath = SettingsManager.Instance.GetSettingValue().GetPlayerLastSavePath();
        for (int i = 0; i < saveLoadButtons.Length; i++)
        {
             saveLoadButtons[i].GetComponent<SaveAndLoadButtonControl>().SetIsNew(lastPath);
        }
    }

    private void PrevPage()
    {
        if (currentPage > 0)
        {
            currentPage--;
            UpdateUI();
        }
    }

    private void NextPage()
    {
        if((currentPage + 1)* slotsPerPage < totalSlots)
        {
            currentPage++;
            UpdateUI();
        }
    }


    private void GoBack()
    {
        saveLoadPanel.SetActive(false);
        if (OnHidePanel != null) {
            OnHidePanel.Invoke();
            SaveLoadManager.OnHidePanel = null;
        }
    }

    private void LoadSButtonDataAndScreenshots(Button button, int index)
    {
        var savePath = GenerateDataPath(index);

        var saveButtonControl = button.gameObject.GetComponent<SaveAndLoadButtonControl>();

        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            var saveData = JsonConvert.DeserializeObject<SaveData>(json);

            if (saveButtonControl != null)
            {
              if (saveData == null) Debug.Log("saveLoadManager with saveData is null" + index);

                saveButtonControl.SaveDataToButton(index,saveData, savePath);
            }
            saveButtonControl.MakeAllButtonInteractable(true);
        }
        else
        {
            saveButtonControl.UpdateEmptySlotInfo(index, currentPage);

        }
    }


    private string GenerateDataPath(int index)
    {
        string suffix = null;

        if (index == -1)
        {
            suffix = "TempLoad";

        }
        else if (index < Constants.SLOTS_PER_PAGE )
        {
            suffix = "AS" + index +"-" + Constants.SLOTS_PER_PAGE;

        }
        else if (index < Constants.SLOTS_PER_PAGE * 2)
        {
            suffix = "QS" + index % Constants.SLOTS_PER_PAGE;

        }
        else
        {
            suffix = $"S{currentPage - 1}-{(index % Constants.SLOTS_PER_PAGE)}";

        }

        return Path.Combine(Application.persistentDataPath, Constants.SAVE_FILE_PATH, suffix + Constants.SAVE_FILE_EXTENSION);
    }




    public void AutoSaveGame()
    {
        int quickSaveStart = 0;
        int quickSaveEnd = 7;

        QuickAndAutoSaveHelper(quickSaveStart, quickSaveEnd);

    }


    public void QuickSaveGame()
    {
        if (saveLoadPanel.activeSelf) return;


        int quickSaveStart = 8;
        int quickSaveEnd = 15;

         if (QuickAndAutoSaveHelper(quickSaveStart, quickSaveEnd))
        {
            NotificationManage.Instance.ShowAtTop("Quick Save Success");
        } else
        {
            NotificationManage.Instance.ShowAtTop("Quick Save Fail");


        }

    }

    bool QuickAndAutoSaveHelper(int SaveStart, int SaveEnd)
    {
        Texture2D screenshot = screenShotter.CaptureScreenshot();
        screenshotData = screenshot.EncodeToPNG();

        int chosenSlot = -1;
        DateTime earliestTime = DateTime.MaxValue;

        for (int i = SaveStart; i <= SaveEnd; i++)
        {
            string path = GenerateDataPath(i);

            if (!File.Exists(path))
            {
                chosenSlot = i;
                break;
            }
            else
            {
                string json = File.ReadAllText(path);
                SaveData data = JsonConvert.DeserializeObject<SaveData>(json);

                if (data == null || !data.isLock)
                {
                    if (DateTime.TryParse(data.SaveTimeString, out DateTime parsedTime))
                    {
                        if (parsedTime < earliestTime)
                        {
                            earliestTime = parsedTime;
                            chosenSlot = i;
                        }
                    }
                }
            }
        }

        if (chosenSlot == -1)
        {
            return false;
        }

        SaveGame(chosenSlot);
        return true;

    }


    void SaveGame(int slotIndex)
    {
        string savePath = GenerateDataPath(slotIndex);

        if (File.Exists(savePath))
        {
            string existingJson = File.ReadAllText(savePath);
            SaveData existingData = JsonConvert.DeserializeObject<SaveData>(existingJson);

            if (existingData != null && existingData.isLock)
            {
               // may be need to add a pop text;
               return;
            }
        }

        if (gameValue == null) Debug.Log("game Value why is fuck");
        GameValueData gvData = new GameValueData(gameValue);
        if (gvData == null) {
            Debug.Log("gvData why is fuck");
            return;
        }

        string saveTime = System.DateTime.Now.ToString("G");
        string SaveNo = "BUG";
        if (slotIndex < Constants.SLOTS_PER_PAGE)
        {
            SaveNo = "AS" + slotIndex % Constants.SLOTS_PER_PAGE;

        }
        else if (slotIndex < Constants.SLOTS_PER_PAGE * 2)
        {
            SaveNo = "QS" + slotIndex % Constants.SLOTS_PER_PAGE;

        }
        else
        {
            SaveNo = $"S{currentPage - 1}-{(slotIndex % Constants.SLOTS_PER_PAGE)}";
        }


        var saveData = new SaveData(screenshotData, storySaveData, gvData, saveTime, savePath);
        string json = JsonConvert.SerializeObject(saveData, Formatting.Indented);
        File.WriteAllText(savePath, json);
        SettingsManager.Instance.ChangeSettingValueLastGameSavePath(savePath); // maybe need to change by only normal save, will update set new;

    }


    public void QuickLoadGame()
    {
        int quickLoadStart = 8;
        int quickLoadEnd = 15;

        int chosenSlot = -1;
        DateTime latestTime = DateTime.MinValue;

        for (int i = quickLoadStart; i <= quickLoadEnd; i++)
        {
            string path = GenerateDataPath(i);

            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                SaveData data = JsonConvert.DeserializeObject<SaveData>(json);

                if (data != null)
                {
                    if (DateTime.TryParse(data.SaveTimeString, out DateTime parsedTime))
                    {
                        if (parsedTime > latestTime)
                        {
                            latestTime = parsedTime;
                            chosenSlot = i;
                        }
                    }
                }
            }
        }

        if (chosenSlot == -1)
        {
            NotificationManage.Instance.ShowAtTop("Quick Load Fail");
            return;
        }

        Debug.Log("Load Game chose slot is." + chosenSlot);


        LoadGame(chosenSlot);

        NotificationManage.Instance.ShowAtTop("Quick Load Success");

}


    public void LoadGame(SaveData saveData)
    {
        if (saveData == null) return;
        LoadGame(saveData.savePath);
    }

    void LoadGame(int slotIndex)
    {
        var savePath = GenerateDataPath(slotIndex);
        LoadGame(savePath);
    }


    public void LoadGame(string savePath)
    {
        Debug.Log("LoadGame BY savePath");

        if (File.Exists(savePath))
        {

            string json = File.ReadAllText(savePath);
            var saveData = JsonConvert.DeserializeObject<SaveData>(json);

            SettingsManager.Instance.ChangePlayerName(saveData.gameValueData.playerName);

            savePath = GenerateDataPath(-1);
            string newJson = JsonConvert.SerializeObject(saveData, Formatting.Indented);
            File.WriteAllText(savePath, newJson);
            SceneManager.LoadScene("GameScene");

        }


    }*/


}



