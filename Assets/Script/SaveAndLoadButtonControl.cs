using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.IO;
using Newtonsoft.Json;
using System;
using JetBrains.Annotations;
using Unity.VisualScripting;
using System.Runtime.InteropServices.WindowsRuntime;
using DG.Tweening.Plugins.Core.PathCore;
using static GetSprite;
using static GetColor;
using UnityEngine.EventSystems;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization;

public class SaveAndLoadButtonControl : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public TextMeshProUGUI GameTimeText;
    public TextMeshProUGUI SaveTimeText;
    public TextMeshProUGUI SaveIndexText;
    public TextMeshProUGUI CountryNameText;
    public TextMeshProUGUI PlayerNameText;
    public TextMeshProUGUI AchievementPointsText;


    public TextMeshProUGUI NoteText;


    public RawImage ScreenShot;
    public Button StarButton;
    public Button DeleteButton;
    public Button NoteButton;
    public Button NoteRowButton;
    public Image NoteImage;


    //public GameObject isNewImage;
    public TextMeshProUGUI TypeText;

    private SaveData saveData = null;
    private bool isAutoOrQuickSave;
    private int buttonIndex;
    private int currentPage;
    public Sprite unStarSprite;
    public Sprite starSprite;
    
    public Sprite[] deleteSprite;

    public Image countryIconImage;
    public Image AchievementPointsImage;

    public Texture defaultAutoScreenShot;
    public Texture defaultSaveScreenShot;
    public Texture defaultLoadScreenShot;

   // public SaveLoadManager saveLoadManager;
    public Button SaveButton;

    // public List<Sprite> countryIconSprite;
    private SavePanelInfo savePanelInfo;
    [SerializeField] private bool isAuto = false;

    void OnEnable()
    {
        LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
    }

    void OnDisable()
    {
        LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;
    }

    private void OnDestroy()
    {
        LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;
    }

    private void OnLocaleChanged(Locale locale)
    {
        SetCountryName();
    }



    private void Start()
    {
        if (StarButton != null) StarButton.onClick.AddListener(OnStarButtonClick);
        DeleteButton.onClick.AddListener(OnDeleteButtonClick);
        if (NoteButton != null) NoteButton.onClick.AddListener(OnNoteButtonClick);
        if (NoteRowButton != null) NoteRowButton.onClick.AddListener(OnNoteButtonClick);
        SaveButton.onClick.AddListener(OnSaveButtonClick);
        InitSaveButton();
        savePanelInfo = SavePanelInfo.Instance;        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        SoundManager.Instance.PlaySelSFX();
        if (saveData == null) return;
        // LoadPanelManage.Instance.SetSaveBackGround(saveData.gameValueData.currentYear,saveData.gameValueData.currentSeason);
        SavePanelInfo.Instance.SetSaveButton(this, isAuto);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //  LoadPanelManage.Instance.ResetSaveBackGround();

    }
    void InitSaveButton()
    {
        ColorBlock cb = SaveButton.colors;  
        cb.normalColor = GetRowColor(RowType.player);
        cb.highlightedColor = GetRowColor(RowType.sel);
        cb.pressedColor = GetRowColor(RowType.player);
        cb.selectedColor = GetRowColor(RowType.sel);
        SaveButton.colors = cb;  
    }

    public void OnStarButtonClick()
    {
        if (saveData == null) return;

        saveData.isLock = !saveData.isLock;

        UpdateLockState();

       if (!string.IsNullOrEmpty(GetSavePath()))
        {
            string json = JsonConvert.SerializeObject(saveData, Formatting.Indented);
            File.WriteAllText(GetSavePath(), json);
        }
    }

    public void OnNoteButtonClick()
    {
        if (saveData == null) return;
        SavePanelInfo.Instance.ShowNoteInputPanel(saveData);
    }

    public void OnSaveButtonClick()
    {
        LoadGame();
    }

    public void LoadGame()
    {
        LoadPanelManage.Instance.LoadGame(saveData);
    }



    public void OnDeleteButtonClick()
    {
        if (saveData.isLock)
        {
            //   NotificationManage.Instance.ShowAtTop("This save is star");
            NotificationManage.Instance.ShowAtTopByKey(NotificationKeyConstants.Save_Starred);
            return;
        }

        LoadPanelManage.Instance.RemoveSaveData(saveData);
        if (savePanelInfo.GetSaveAndLoadButtonControl() == this)SavePanelInfo.Instance.SetSaveButton(null);
        DeleteFile();
        UpDataInfoUI();
        LoadPanelManage.Instance.ResetSaveBackGround();

    }

    public void DeleteFile()
    {
        if (!string.IsNullOrEmpty(GetSavePath()) && File.Exists(GetSavePath()))
        {
            File.Delete(GetSavePath());
        }
        this.saveData = null;
    }

    void SetTheTextActive(bool active)
      {

          AchievementPointsText.gameObject.SetActive(active);
          PlayerNameText.gameObject.SetActive(active);
          NoteText.gameObject.SetActive(active);
          SaveTimeText.gameObject.SetActive(active);

      }

      public  void MakeAllButtonInteractable(bool isInteractable)
      {
        return;
          if (StarButton != null) StarButton.interactable = isInteractable;
          DeleteButton.interactable = isInteractable;
          NoteButton.interactable = isInteractable;


          Color32 c = NoteImage.color;

          if (isInteractable)
          {
              c = new Color32(255, 255, 255, 255);
          }
          else
          {
              c = new Color32(200, 200, 200, 128);

          }
          NoteImage.color = c;
          countryIconImage.color = c;
          AchievementPointsImage.color = c;


          UpdateLockState();


      }


      private void UpdateLockState()
      {
          if (saveData == null || saveData.isLock)
          {
              if (saveData == null) {
                if (StarButton != null) StarButton.gameObject.GetComponent<Image>().sprite = unStarSprite; }
              else {
                if (StarButton != null) StarButton.gameObject.GetComponent<Image>().sprite = starSprite; };
              DeleteButton.gameObject.GetComponent<Image>().sprite = deleteSprite[0];
              DeleteButton.interactable = false;
          }
          else
          {
            if (StarButton != null) StarButton.gameObject.GetComponent<Image>().sprite = unStarSprite;
              DeleteButton.gameObject.GetComponent<Image>().sprite = deleteSprite[1];
              DeleteButton.interactable = true;
          }

           savePanelInfo = SavePanelInfo.Instance;
          if (savePanelInfo.GetSaveData() == this.saveData) savePanelInfo.UpLockImage();

      }


      public void SaveDataToButton(int index, SaveData data, string filePath)
      {
          if (data == null) return;

          buttonIndex = index;
          isAutoOrQuickSave = index < Constants.SLOTS_PER_PAGE;
          saveData = data;
          UpDataInfoUI();
      }

      void SetIsAuto()
      {
        TypeText.gameObject.SetActive(true);
        TypeText.text = "AUTO";

      }

    void SetIsQuick()
    {
        TypeText.gameObject.SetActive(true);
        TypeText.text = "Quick";

    }

    public void SetIsNew(string lastPath)
    {
       // TypeText.gameObject.SetActive(lastPath == savePath);
        TypeText.text = "NEW";

        //  isNewImage.GetComponent<Image>().sprite = Resources.Load<Sprite>($"MyDraw/UI/Other/NEWSave");

    }

      public void SetButtonInteractable(bool state)
      {
          GetComponent<Button>().interactable = state;
      }


    public void UpDataInfoUI()
    {
           if (saveData == null || saveData.gameValueData == null)
           {
               UpdateUIWithoutSaveData();
           }
           else
           {
               UpdateUIWithSaveData();
           }
    }

    private void UpdateUIWithSaveData()
    {
        string savePlayerCountry = saveData.gameValueData.GetPlayerCountry();
        if (!string.IsNullOrEmpty(savePlayerCountry))
            countryIconImage.sprite = saveData.gameValueData.CountryManagerSaveData.GetSaveCountrySprite(savePlayerCountry);
        else
            countryIconImage.sprite = GameValue.Instance.GetCountryIcon("None");

        SetCountryName();

        if (saveData.savedScreenshotData != null && saveData.savedScreenshotData.Length > 0)
        {
            Texture2D tex = new Texture2D(2, 2);
            if (!tex.LoadImage(saveData.savedScreenshotData))
                Debug.LogWarning("Failed to load screenshot data!");
            else
                ScreenShot.texture = tex;
        }
        else
        {
            SetEmptyScreeshot();
        }

        SetTheTextActive(true);

        if (PlayerNameText != null)
            PlayerNameText.text = saveData.gameValueData.GetPlayerName();

        if (GameTimeText != null)
            GameTimeText.text = saveData.gameValueData.GetTurnString();

        if (AchievementPointsText != null)
            AchievementPointsText.text = saveData.gameValueData.GetAchievementString();

        if (NoteText != null)
            NoteText.text = !string.IsNullOrWhiteSpace(saveData.NoteString) ? saveData.NoteString : "<i>Enter...</i>";

        if (SaveTimeText != null)
            SaveTimeText.text = saveData.SaveTimeString;

        UpdateLockState();
    }

    private void UpdateUIWithoutSaveData()
    {
        countryIconImage.sprite = GameValue.Instance.GetCountryIcon("None");
        SetCountryName();
        SetEmptyScreeshot();
        SetTheTextActive(false);

        if (PlayerNameText != null)
            PlayerNameText.text = "";

        if (GameTimeText != null)
            GameTimeText.text = "";

        if (AchievementPointsText != null)
            AchievementPointsText.text = "";

        if (NoteText != null)
            NoteText.text = "<i>Enter...</i>";

        if (SaveTimeText != null)
            SaveTimeText.text = "";

        UpdateLockState();
    }




    public void UpSaveDataNoteString(string noteString)
    {
        string json = System.IO.File.ReadAllText(GetSavePath());
        string note = string.IsNullOrWhiteSpace(noteString) ? null : noteString;
        saveData.WriteTheNote(note);

        string updatedJson = JsonConvert.SerializeObject(saveData, Formatting.Indented);
        File.WriteAllText(GetSavePath(), updatedJson);

        NoteText.text = !string.IsNullOrWhiteSpace(note) ? noteString : "<i>Enter...</i>";

        if (savePanelInfo.GetSaveData() == saveData) savePanelInfo.SetSaveData(saveData);

    }

    void SetCountryName()
    {
        if (saveData == null || saveData.gameValueData == null || string.IsNullOrEmpty(saveData.gameValueData.GetPlayerCountry()))
        {
            CountryNameText.gameObject.SetActive(false);
        }
        else
        {
            CountryNameText.gameObject.SetActive(true);
            String savePlayerCountry = saveData.gameValueData.GetPlayerCountry();
            CountryNameText.text = saveData.gameValueData.CountryManagerSaveData.GetSaveCountryStringWithColor(savePlayerCountry);
        }

    }

    string CurrentSeasonIntToString(int currentSeason)
    {
        switch (currentSeason)
        {
            case 0:
                return "Spring";
            case 1:
                return "Summer";
            case 2:
                return "Fall";
            case 3:
                return "Winter";

            default:
                return "BUG";
        }
    }

    public bool IsAutoOrQuickSave()
    {
        return isAutoOrQuickSave;
    }

    public void SetSaveData(SaveData saveData)
    {
        this.saveData = saveData;
        UpDataInfoUI();
    }

    public SaveData GetSaveData()
    {
        return saveData;
    }

    public String GetSavePath()
    {
        return saveData.savePath;
    }

    public bool IsStar()
    {
        return saveData.isLock;
    }

    void SetEmptyScreeshot()
    {
        ScreenShot.texture = defaultAutoScreenShot;
    }

    public bool HasSaveData()
    {
        return saveData != null;
    }


}

[System.Serializable]
public class SaveData
{
    public byte[] savedScreenshotData;
    public StorySaveData storySaveData;
    public bool isLock = false;
    public CameraData cameraSaveData;
    public GameValueSaveData gameValueData;
    public TotalPanelSaveData totalPanelSaveData;
    public string savePath;
    public string SaveTimeString;
    public string NoteString;

    public SaveData(byte[] savedScreenshotData, StorySaveData storySaveData, GameValueSaveData gameValueData,TotalPanelSaveData totalPanelSaveData ,string saveTimeString, string savePath)
    {
        this.cameraSaveData = new CameraData(true);
        this.savedScreenshotData = savedScreenshotData;
        this.storySaveData = storySaveData;
        this.gameValueData = gameValueData;

        this.totalPanelSaveData = totalPanelSaveData;
        SaveTimeString = saveTimeString;
        this.savePath = savePath;
        if (storySaveData != null) NoteString = storySaveData.savedSpeakingContent;
    }

    public void Lock()
    {
        isLock = !isLock;
    }

    public void WriteTheNote(string note)
    {
        NoteString = note;
    }

    public int GetAchievement()
    {
        return gameValueData.GetAchievement();
    }

    public DateTime GetSaveDateTime()
    {
        DateTime.TryParse(SaveTimeString, out var dt);
        return dt;
    }


}
