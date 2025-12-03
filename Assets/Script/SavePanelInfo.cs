using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;

using UnityEngine.UI;
using UnityEngine.Rendering;
using Newtonsoft.Json;
using DG.Tweening.Plugins.Core.PathCore;
using static GetSprite;

public class SavePanelInfo : MonoBehaviour
{
    public static SavePanelInfo Instance { get; private set; }


    public RawImage screenshot;
    public Button StarButton;

    public TextMeshProUGUI IDText;

    public TextMeshProUGUI GameValueTime;
    public TextMeshProUGUI SaveTimeText;

    public TextMeshProUGUI PopulationText;
    public TextMeshProUGUI totalRecruitedPopulationText;

    public TextMeshProUGUI supportRateText;

    public TextMeshProUGUI PlayerCountryNameText;
    public TextMeshProUGUI PlayerNameText;


    public TextMeshProUGUI totalFood;
    public TextMeshProUGUI totalScience;
    public TextMeshProUGUI totalPolitics;
    public TextMeshProUGUI totalGold;
    public TextMeshProUGUI totalFaith;

    public TextMeshProUGUI characterNumText;
    public TextMeshProUGUI regionCountText;
    public TextMeshProUGUI itemNumText;
    public TextMeshProUGUI negotiationNumText;

    public TextMeshProUGUI buildText;
    public TextMeshProUGUI scoutText;
    public TextMeshProUGUI achievementNumText;

    public TextMeshProUGUI NoteText;
    public Button NoteButton;
    public Button LoadButton;
    public Button DeleteButton;
    public Button NoteChangeButton;

    //private SaveData saveData;
    public InputPanelControl inputPanelControl;
    public List<SaveAndLoadButtonControl> SaveAndLoadButtonControls;
    private SaveData saveData;


   // private SaveAndLoadButtonControl saveAndLoadButtonControl;
  //  public  SaveLoadManager saveLoadManager;

    public Sprite[] starSprite; //0 is Un, 1 is star

    public Sprite[] deleteSprite;

    [SerializeField] private Texture loadTextTure;// 0 is unsel, 1 is sel
    [SerializeField] private Texture quickLoadTextTure; // 0 is unsel, 1 is sel
    [SerializeField] private List<Sprite> loadSprites;// 0 is unsel, 1 is sel
    [SerializeField] private List<Sprite> quickLoadSprites; // 0 is unsel, 1 is sel

    public Image coutryImage;
    // public Image isNewImage;
    public TextMeshProUGUI TypeText;

    public Image regionNumIcon;
    public Image itemNumIcon;
    public Image characterNumIcon;

    public Sprite noneCountrySprite;

    public Texture[] saveAndLoadSprite;

    public GameObject notePanel;
    private SaveAndLoadButtonControl saveAndLoadButtonControl;
    private bool isAuto;

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
        saveData = null; 
        SetSaveData(null);
        StarButton.onClick.AddListener(OnStarButtonClick);
        LoadButton.onClick.AddListener(OnLoadButtonClick);
        DeleteButton.onClick.AddListener(OnDeleteButtonClick);
        NoteButton.onClick.AddListener(OnNoteButtonClick);


    }

    void UpEmptyScreenshot(bool isSave)
    {
        if (saveData == null)
        {

            if (isSave)
            {
                screenshot.texture = saveAndLoadSprite[0];
            }
            else
            {
                screenshot.texture = saveAndLoadSprite[1];

            }


        }

    }


    public void SaveOrLoad(bool isSave)
    {
        UpEmptyScreenshot(isSave);
    }

    public SaveData GetSaveData()
    {
        return saveData;
    }

    public void OnTheButton(SaveData data)
    {
        if (data == null) return;
        SetSaveData(data);
      //  SetSaveAndLoadButtonControl(data);
    }

    void OnStarButtonClick()
    {
        if (saveData == null) return;
       
        UpLockImage();
        LoadPanelManage.Instance.SetStar(saveData);
    }

    void OnDeleteButtonClick()
    {
        if (saveData == null) return;

        if (!isAuto)LoadPanelManage.Instance.RemoveSaveData(saveData);
        saveAndLoadButtonControl.OnDeleteButtonClick();
        SetSaveData(null);

    }


    void OnNoteButtonClick()
    {
        if (saveData == null) return;
        ShowNoteInputPanel(saveData);
    }



    public void ShowNoteInputPanel(SaveData saveData)
    {
        inputPanelControl.ShowInputPanel(() =>SaveNoteData(saveData), true, saveData.NoteString);
    }


    void SaveNoteData(SaveData saveData)
    {
        string noteString = inputPanelControl.inputField.text;
        saveData.NoteString = noteString;
        string savePath = saveData.savePath;
        string json = JsonUtility.ToJson(saveData, true); 
        File.WriteAllText(savePath, json);
    }

    private void UpPanelAndButton()
    {

        for (int i = 0; i < SaveAndLoadButtonControls.Count; i++)
        {
            if (SaveAndLoadButtonControls[i].GetSaveData() == saveData)
            {
                string json = File.ReadAllText(saveData.savePath);
                saveData = JsonConvert.DeserializeObject<SaveData>(json);
                SaveAndLoadButtonControls[i].SetSaveData(saveData);
            }
        }

        SetSaveData(saveData);

    }


    public void UpLockImage()
    {

        if (saveData == null)
        {
            StarButton.gameObject.GetComponent<Image>().sprite = starSprite[0];
            DeleteButton.gameObject.GetComponent<Image>().sprite = deleteSprite[0];
        }
        else if (!saveData.isLock)
        {
            StarButton.gameObject.GetComponent<Image>().sprite = starSprite[0];
            DeleteButton.gameObject.GetComponent<Image>().sprite = deleteSprite[1];
            DeleteButton.interactable = true;

        }
        else
        {
            StarButton.gameObject.GetComponent<Image>().sprite = starSprite[1];
            DeleteButton.gameObject.GetComponent<Image>().sprite = deleteSprite[0];
            DeleteButton.interactable = false;
        }

    }


    void OnLoadButtonClick()
    {
        if (saveData == null) return;
        LoadPanelManage.Instance.LoadGame(saveData);
    }

    public void SetSaveButton(SaveAndLoadButtonControl saveAndLoadButtonControl, bool isAuto = false)
    {
        this.saveAndLoadButtonControl = saveAndLoadButtonControl;
        this.isAuto = isAuto;
        SetSaveData(saveAndLoadButtonControl?.GetSaveData());
    }
    public void SetSaveData(SaveData saveData)
    {
        this.saveData = saveData;
        bool isNull = saveData == null;

        if (isNull || saveData.NoteString == null)
        {
            NoteText.text = "<i> EMPTY...</i>";
        }
        else
        {
            NoteText.text = saveData.NoteString;
        }

        SetTextOrHide(SaveTimeText, saveData?.SaveTimeString);
        string playerCountryName = saveData?.gameValueData?.GetPlayerCountry();
        SetTextOrHide(PlayerCountryNameText, saveData?.gameValueData?.CountryManagerSaveData?.GetSaveCountryStringWithColor(playerCountryName));
        SetTextOrHide(PlayerNameText, saveData?.gameValueData?.GetPlayerName());
        SetTextOrHide(PopulationText, saveData?.gameValueData?.TotalPopulation);
        SetTextOrHide(totalRecruitedPopulationText, saveData?.gameValueData?.ResourceValueSaveData.TotalRecruitedPopulation);
        SetTextOrHide(totalFood, saveData?.gameValueData?.ResourceValueSaveData.Food);
        SetTextOrHide(totalScience, saveData?.gameValueData?.ResourceValueSaveData.Science);
        SetTextOrHide(totalPolitics, saveData?.gameValueData?.ResourceValueSaveData.Politics);
        SetTextOrHide(totalGold, saveData?.gameValueData?.ResourceValueSaveData.Gold);
        SetTextOrHide(totalFaith, saveData?.gameValueData?.ResourceValueSaveData.Faith);
        SetTextOrHide(characterNumText, saveData?.gameValueData?.playerCharacterNum);
        SetTextOrHide(regionCountText, saveData?.gameValueData?.playerRegionNum);
        SetTextOrHide(itemNumText, saveData?.gameValueData?.playerItemCount);
        SetTextOrHide(negotiationNumText, saveData?.gameValueData?.ResourceValueSaveData.Negotiation);
        SetTextOrHide(buildText, saveData?.gameValueData?.ResourceValueSaveData.Build);
        SetTextOrHide(scoutText, saveData?.gameValueData?.ResourceValueSaveData.Scout);
        SetTextOrHide(achievementNumText, saveData?.gameValueData?.GetAchievementString());

     
        if (isNull)
        {
            SetEmptyScreeshot();
            coutryImage.sprite = noneCountrySprite;
            TypeText.gameObject.SetActive(false);
            GameValueTime.gameObject.SetActive(false);
            supportRateText.gameObject.SetActive(false);
            notePanel.gameObject.SetActive(false);
            regionNumIcon.gameObject.SetActive(false);
            characterNumIcon.sprite = Resources.Load<Sprite>($"MyDraw/item/3/CommandSword");
            itemNumIcon.sprite = Resources.Load<Sprite>($"MyDraw/item/EmptyItemClose");
        }
        else
        {
            coutryImage.sprite = saveData.gameValueData.CountryManagerSaveData.GetSaveCountrySprite(playerCountryName);
            GameValueTime.gameObject.SetActive(true);
            GameValueTime.text = saveData.gameValueData.GetTurnString();
            float rate = saveData.gameValueData.SupportRate;
            supportRateText.text = (rate * 100f).ToString("F1") + "%";
            supportRateText.gameObject.SetActive(true);
            notePanel.gameObject.SetActive(true);
            Texture2D tex = new Texture2D(2, 2);
            tex.LoadImage(saveData.savedScreenshotData);
            screenshot.texture = tex;
            regionNumIcon.gameObject.SetActive(true);
            regionNumIcon.gameObject.SetActive(saveData.gameValueData.hasReichszepter);
            regionCountText.gameObject.SetActive(saveData.gameValueData.hasReichszepter);

            if (saveData.gameValueData.hasZeremonienschwert)
            {
                characterNumIcon.sprite = Resources.Load<Sprite>($"MyDraw/item/5/Zeremonienschwert");
            }else { characterNumIcon.sprite = Resources.Load<Sprite>($"MyDraw/item/3/CommandSword"); }
            if (saveData.gameValueData.hasReichsapfel){itemNumIcon.sprite = Resources.Load<Sprite>($"MyDraw/item/5/Reichsapfel");
            }else{itemNumIcon.sprite = Resources.Load<Sprite>($"MyDraw/item/EmptyItemClose");}

        }

        LoadButton.interactable = !isNull;
        StarButton.interactable = !isNull;
        DeleteButton.interactable = !isNull;
        NoteChangeButton.interactable = !isNull;

        UpLockImage();
        SetLoadButtonEffect();

        if (isAuto)
        {
            StarButton.interactable = false;
        }
    }

    void SetEmptyScreeshot()
    {
        if (LoadPanelManage.Instance.saveType == LoadPanelManage.SaveType.Normal)
        {
            screenshot.texture = loadTextTure;
        } else
        {
            screenshot.texture = quickLoadTextTure;

        }
    }

    void SetLoadButtonEffect()
    {
        if (LoadPanelManage.Instance.saveType == LoadPanelManage.SaveType.Normal)
        {
            LoadButton.gameObject.GetComponent<ButtonEffect>().SetChangeSprite(loadSprites[1], loadSprites[0]);
        }
        else
        {
            LoadButton.gameObject.GetComponent<ButtonEffect>().SetChangeSprite(quickLoadSprites[1], quickLoadSprites[0]);

        }
    }

    public void SetSavePenelInfoNote(string note)
    {
        if (string.IsNullOrWhiteSpace(note) || string.IsNullOrWhiteSpace(note))
        {
            NoteText.text = "<i> EMPTY...</i>";
        }
        else
        {
            NoteText.text = saveData.NoteString;
        }

    }

    private void SetTextOrHide(TextMeshProUGUI textComponent, object value)
    {
        if (value == null || string.IsNullOrEmpty(value.ToString()))
        {
            textComponent.gameObject.SetActive(false);
            return;
        }

        string display;
        switch (value)
        {
            case double dv:
                display = FormatDoubleNumberToFormatNumber(dv);
                break;

            case float fv:
                display = FormatNumber(fv);
                break;

            case int iv:
                display = FormatNumber((float)iv);
                break;

            default:
                // ?????? ToString()
                display = value.ToString();
                break;
        }

        if (string.IsNullOrEmpty(display))
        {
            textComponent.gameObject.SetActive(false);
            return;
        }

        textComponent.text = display;
        textComponent.gameObject.SetActive(true);
    }

    public SaveAndLoadButtonControl GetSaveAndLoadButtonControl()
    {
        return saveAndLoadButtonControl;
    }

    string FormatNumber(float value)
    {
        if (value >= 1000000000)
        {
            // For Billions (B)
            int billions = Mathf.FloorToInt(value / 100000000f);
            float temp = Mathf.RoundToInt(billions) / 10f;


            // Check if value falls within the range
            if (temp == Mathf.Floor(temp))
            {
                return temp + "B";
            }
            else
            {
                // Round to one decimal point (0.1B, 1.1B, etc.)
                return temp.ToString("0.0") + "B";  // Always show 1 decimal point if it's a non-integer
            }
        }
        else if (value >= 1000000)
        {


            // For Millions (M)
            int millions = Mathf.FloorToInt(value / 100000f);
            float temp = Mathf.RoundToInt(millions) / 10f;

            // Check if value falls within the range
            if (temp == Mathf.Floor(temp))
            {
                return temp + "M";
            }
            else
            {
                // Round to one decimal point (1.1M, 2.1M, etc.)
                return temp.ToString("0.0") + "M";  // Always show 1 decimal point if it's a non-integer
            }
        }
        else if (value >= 1000)
        {
            // For Thousands (K)
            int hundred = Mathf.FloorToInt(value / 100f);
            float temp = Mathf.RoundToInt(hundred) / 10f;

            // Check if value falls within the range
            if (temp == Mathf.Floor(temp))
            {
                return temp.ToString("F0") + "K";
            }
            else
            {
                return temp.ToString("0.0") + "K";
            }
        }
        else
        {
            // For values under 1000, show as integer
            return value.ToString("F0");
        }
    }

    string FormatDoubleNumberToFormatNumber(double value)
    {
        double temp = 1000;

        double epsilon = 0.01;

        if (value < temp || Mathf.Abs((float)(value - temp)) < epsilon)
        {
            return FormatDoubleNumberd(value);

        }
        else
        {
            return FormatNumber((float)value);

        }
    }

    string FormatDoubleNumberd(double value)
    {
        int valueInt = (int)(value * 10);
        if (valueInt % 10 == 0)
        {
            return value.ToString("F0");
        }
        else
        {
            return value.ToString("F1");
        }
    }
}



