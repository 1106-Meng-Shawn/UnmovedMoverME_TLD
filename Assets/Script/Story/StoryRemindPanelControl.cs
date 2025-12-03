using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static ExcelReader;
using UnityEngine.Localization.Settings;
using System.Collections.Generic;
using System.IO;

public class StoryRemindPanelManager : MonoBehaviour
{
    // Start is called before the first frame update
    public static StoryRemindPanelManager Instance { get; private set; }
    public GameObject StoryRemindPanel;

    public TMP_Text titleText;
    public TMP_Text introductionText;
    public Button checkButton;
    public Button RereadButton;
    public Image storyImage;

    private TotalStoryManager totalStoryManager; // Reference to the NormalStoryControl instance
    public StoryControl storyControl; // Reference to the StoryControl instance

  //  private readonly string textPath = Constants.TEXT_PATH;
    private readonly string excelFileExtension = Constants.EXCEL_FILE_FXTENSION;



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
        totalStoryManager = TotalStoryManager.Instance;

        if (storyControl == null) {
            storyControl = GameValue.Instance.GetStoryControl();
         }

    }


    public void StoryHappenNode(StoryNode node,string fileName)
    {
        node.isHappend = true;

        if (totalStoryManager.IsFileCompleted(node.currentFileName))
        {
            Init(node,fileName);
            StoryRemindPanel.SetActive(true);
        }
        else
        {
            StoryRemindPanel.SetActive(false);
            OnRereadButtonClicked(node);
        }

    }



    public void StoryHappenNode(StoryNode node)
    {
        node.isHappend = true;

        if (totalStoryManager.IsFileCompleted(node.currentFileName))
        {
            Init(node);
            StoryRemindPanel.SetActive(true);
        }
        else
        {
            StoryRemindPanel.SetActive(false);
            OnRereadButtonClicked(node);
        }
        

    }

    public bool CheckStoryTrigger()
    {
        if (storyControl == null) storyControl = GameValue.Instance.GetStoryControl();
        StoryNode TriggerStory = storyControl.GetStoryNode();
        if (TriggerStory == null) return false;
        string triggerStoryFileName = TriggerStory.currentFileName;

        if (TriggerStory != null)
        {
             if(totalStoryManager.IsFileCompleted(triggerStoryFileName))
              {
                     Init(TriggerStory);
                StoryRemindPanel.SetActive(true);

              } else
              {
                Debug.Log("work");
                  OnRereadButtonClicked(TriggerStory);
              }
            return true;

        }
        else
        {
            StoryRemindPanel.SetActive(false);
            return false;
        }

    }

    public void Init(StoryNode storyNode,string fileName)
    {
        if (fileName == null)
        {
            Debug.LogError("fileName is null");
            return;
        } else if (storyNode == null)
        {
            Debug.LogError("storynode is null");
            return;
        }

        string filePath = Path.Combine(Application.streamingAssetsPath, $"Text/{storyNode.currentFileName + excelFileExtension}");

        ExcelPlotData excelData = GetExcelFilePath(filePath);

//        Debug.Log(path);


        titleText.text = excelData.speaker;
        introductionText.text = GetContentText(excelData.contents);
        Sprite sprite = Resources.Load<Sprite>("MyDraw/"+excelData.avatarImageFileName);
        storyImage.sprite = sprite;


        checkButton.onClick.RemoveAllListeners();
        RereadButton.onClick.RemoveAllListeners();


        checkButton.onClick.AddListener(() => OnCheckButtonClicked(storyNode));
        RereadButton.onClick.AddListener(() => OnRereadButtonClicked(storyNode));
    }


    public string GetContentText(Dictionary<string, string> contents)
    {
        string currentLanguage = LocalizationSettings.SelectedLocale.Identifier.Code;
        return contents.TryGetValue(currentLanguage, out var text) ? text : contents["en"];
    }


    public void Init(StoryNode storyNode)
    {
        if (storyNode == null)
        {
            Debug.LogError("StoryNode is null");
            return;
        }
        titleText.text = storyNode.GetTitle();
        introductionText.text = storyNode.GetTitle();
        Debug.Log(storyNode.storyImage);
        Sprite sprite = Resources.Load<Sprite>(storyNode.storyImage);
        storyImage.sprite = sprite; // Assuming StoryNode has a Sprite property

        checkButton.onClick.RemoveAllListeners();
        RereadButton.onClick.RemoveAllListeners();

        checkButton.onClick.AddListener(() => OnCheckButtonClicked(storyNode));
        RereadButton.onClick.AddListener(() => OnRereadButtonClicked(storyNode));
    }
    void OnCheckButtonClicked(StoryNode storyNode)
    {
        if (storyNode == null)
        {
            Debug.LogError("StoryNode is null");
            return;
        }
        // Logic to handle the reread button click
        StoryRemindPanel.SetActive(false);
        totalStoryManager.OnStoryReminderCheckButtonClick(storyNode.currentFileName);

        storyNode.isHappend = true;
    }

    public void OnRereadButtonClicked(StoryNode storyNode, string fileName)
    {
        storyNode.isHappend = true; // Mark the node as handled

        if (storyNode == null)
        {
            Debug.LogError("StoryNode is null");
            return;
        }

        storyNode.isHappend = true;

        string Path = storyNode.Prefix + "/" + storyNode.StoryLine + "/" + fileName;

        totalStoryManager.InitializeAndLoadStory(Path, Constants.DEFAULT_START_LINE, Constants.DEFAULT_SHEET_INDEX);


        // Delay setting the gameObject inactive
        StoryRemindPanel.SetActive(false);

    }


    void OnRereadButtonClicked(StoryNode storyNode)
    {
        storyNode.isHappend = true; // Mark the node as handled

        if (storyNode == null)
        {
            Debug.LogError("StoryNode is null");
            return;
        }

        storyNode.isHappend = true;
        Debug.Log(storyNode.currentFileName);
        totalStoryManager.InitializeAndLoadStory(storyNode.currentFileName,Constants.DEFAULT_START_LINE, Constants.DEFAULT_SHEET_INDEX);
        StoryRemindPanel.SetActive(false);

    }

}
