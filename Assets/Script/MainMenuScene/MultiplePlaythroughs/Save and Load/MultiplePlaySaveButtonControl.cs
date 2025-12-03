using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static GetSprite;
using static GetString;
using static GetColor;
using static FormatNumber;

public class MultiplePlaySaveButtonControl : MonoBehaviour
{
    [SerializeField] Button LoadButton;

    [SerializeField] Image CountryIcon;

    [SerializeField] Image CharacterNumIcon;
    [SerializeField] TextMeshProUGUI CharacterNumText;
    [SerializeField] Image ItemNumIcon;
    [SerializeField] TextMeshProUGUI ItemNumText;

    [SerializeField] Image AchievementCostIcon;
    [SerializeField] TextMeshProUGUI AchievementCostText;
    [SerializeField] Image AchievementCoefficientIcon;
    [SerializeField] TextMeshProUGUI AchievementCoefficientText;

    [SerializeField] TextMeshProUGUI DifficultyText;
    [SerializeField] TextMeshProUGUI GameTimeText;
    [SerializeField] TextMeshProUGUI SaveTimeText;
    [SerializeField] TextMeshProUGUI PlayerNameText;

    [SerializeField] Button StarButton;
    [SerializeField] Button DeleteButton;

    [SerializeField] Button NoteButton;
    [SerializeField] Button NoteRowButton;
    [SerializeField] TextMeshProUGUI NoteText;

    private MultiplePlaySaveData multiplePlaySaveData;

    private void Awake()
    {
        InitButtons();
    }

    void InitButtons()
    {
        LoadButton.onClick.AddListener(OnLoadButtonClick);
        StarButton.onClick.AddListener(OnStartButtonClick);
        DeleteButton.onClick.AddListener(OnDeleteButtonClick);
        NoteButton.onClick.AddListener(OnNoteButtonClick);
        NoteRowButton.onClick.AddListener(OnNoteButtonClick);
    }

    public void Init(MultiplePlaySaveData multiplePlaySaveData)
    {
        this.multiplePlaySaveData = multiplePlaySaveData;
        UpdateUI();
    }


    void OnLoadButtonClick()
    {

    }

    void OnStartButtonClick()
    {
        multiplePlaySaveData.IsStar = !multiplePlaySaveData.IsStar;
    }

    void OnDeleteButtonClick()
    {
        if (!string.IsNullOrEmpty(multiplePlaySaveData.savePath))
        {
            if (File.Exists(multiplePlaySaveData.savePath))
            {
                File.Delete(multiplePlaySaveData.savePath);
            }
            else
            {
                Debug.LogWarning($"Cant find?{multiplePlaySaveData.savePath}");
            }
        }
        else
        {
            Debug.LogWarning("?? savePath is null?");
        }

        Destroy(gameObject);
    }

    void OnNoteButtonClick()
    {

    }

    void UpdateUI()
    {
        CountryIcon.sprite = multiplePlaySaveData.GetCountryIcon();
        UpCharacterNumRow();
        UpItemNumRow();
        SaveTimeText.text = multiplePlaySaveData.SaveTimeString;
        NoteText.text = multiplePlaySaveData.NoteString;
        UpStarButtonSprite();
        UpAchievementCostText();
        UpAchievementCoefficientText();
        
    }

    void UpCharacterNumRow()
    {
        
    }

    void UpItemNumRow()
    {

    }



    void UpAchievementCostText()
    {
        int AchievementCost = multiplePlaySaveData.GetAchievementCost();
        string AchievementCostString = FormatNumberToString(AchievementCost);
        AchievementCostText.text = GetValueColorString(AchievementCostString,ValueColorType.Achievement) ;
    }

    void UpAchievementCoefficientText()
    {
        float AchievementCoefficient = multiplePlaySaveData.GetAchievementCoefficient();
        AchievementCoefficientText.text = GetValueColorString(AchievementCoefficient.ToString("g"), ValueColorType.Achievement);
    }



    void UpStarButtonSprite()
    {
        StarButton.image.sprite = GetSprite.UpStarButtonSprite(multiplePlaySaveData.IsStar);
        UpDeleteButtonSprite();
    }

    private void UpDeleteButtonSprite()
    {
        DeleteButton.gameObject.GetComponent<Image>().sprite = GetSprite.UpDeleteButtonSprite(multiplePlaySaveData.IsStar);
        DeleteButton.interactable = !multiplePlaySaveData.IsStar;
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

[Serializable]
public class MultiplePlaySaveData
{
    public string savePath;
    public string SaveTimeString;
    public string NoteString;
    public bool IsStar = false;
    public StoryMultiplePlaythroughsSaveData StoryMultiplePlaythroughsSaveData;

    public MultiplePlaySaveData(StoryMultiplePlaythroughsSaveData storyMultiplePlaythroughsSaveData)
    {
        StoryMultiplePlaythroughsSaveData = storyMultiplePlaythroughsSaveData;

        SaveTimeString = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        string directoryPath = GetSaveDirectoryPath();
        EnsureDirectoryExists(directoryPath);
        savePath = GetSaveFilePath(directoryPath, SaveTimeString);
        SaveToJsonFile(savePath);
    }

    private string GetSaveDirectoryPath()
    {
        return Path.Combine(Application.persistentDataPath, "MultiplePlaythroughs");
    }

    private void EnsureDirectoryExists(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
    }

    private string GetSaveFilePath(string directoryPath, string timeString)
    {
        string fileName = $"MultiplePlay_{timeString}.json";
        return Path.Combine(directoryPath, fileName);
    }

    private void SaveToJsonFile(string fullPath)
    {
        string json = JsonUtility.ToJson(this, true);
        File.WriteAllText(fullPath, json);
    }


    public int GetAchievementCost()
    {
        return StoryMultiplePlaythroughsSaveData.GetAchievementCost();
    }

    public float GetAchievementCoefficient()
    {
        return StoryMultiplePlaythroughsSaveData.GetAchievementCoefficient();
    }

    public Sprite GetCountryIcon()
    {
        return StoryMultiplePlaythroughsSaveData.GetCountryIcon();
    }

}
