using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static EventsRowPrefab;
using static ExcelReader;
using static GetSprite;


public enum TaskSize
{
    Normal, Small
}

public enum TaskType
{
    All,Battle, Story
}

public enum TaskState
{
    New,
    UnCompleted,
    Clear
}


#region TasksRowPrefab
public class TasksRowPrefab : MonoBehaviour, IPointerEnterHandler,IPointerExitHandler
{
    public Button StarButton;
    public Button CheckButton;
    public TextMeshProUGUI Title;
    public TextMeshProUGUI DeadLine;
    public TextMeshProUGUI TaskTypeText;


    private TaskData taskData;

    public List<Sprite> StarSprites;

    public GameValue gameValue;

    public CharacterAssistRowControl characterAssistRowControl;

    [SerializeField] private TaskPanelControl taskPanelControl;

    public GameObject taskPanelPrefab;
    public GameObject taskSmallPanelPrefab;


    public GameObject newMark;
    public GameObject clearMark;

    public Image CharacterIcon;
    public Image ClockImage;

    public GameObject cityInfo;


    public bool isRightCol;
    public void OnPointerEnter(PointerEventData eventData)
    {
        SoundManager.Instance.PlaySelSFX();
        SetTaskState(TaskState.UnCompleted);
        ShowOutLine();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        CloseOutLine();
    }

    private void Start()
    {

        StarButton.onClick.AddListener(OnStarButtonClick);
        CheckButton.onClick.AddListener(OnCheckButtonClick);

    }

void OnDestroy()
    {
        if (taskData != null)
        {
            taskData.OnValueChanged -= UpUIData;
        }
    }

    public void SetTaskData(TaskData taskData)
    {
        this.taskData = taskData;
        this.gameValue = GameValue.Instance;
        this.characterAssistRowControl = CharacterAssistRowControl.Instance;
        this.taskData.OnValueChanged += UpUIData;
        SetTitleText();
        SetTaskType();
        SetTaskState(taskData.GetTaskState());
        SetCharacterType();
        SetDeadLine();
        AddTaskToCity();
        SetCityInfo();
        UpUIData();
    }

    void SetTitleText()
    {
        Title.text = taskData.GetTitle();
    }



    void UpUIData()
    {
        UpStarButtonSpire();
        UpdataStateType();
    }


    void SetCharacterType()
    {
        string characterKey = taskData.GetCharacterKey();

        if (characterKey == CharacterConstants.NoneKey)
        {
            CharacterIcon.gameObject.SetActive(false);
            return;
        }
        Character character = GameValue.Instance.GetCharacterByKey(characterKey);
        if (character != null)
        {
            CharacterIcon.gameObject.SetActive(true);
            CharacterIcon.sprite = character.icon;
        }
    }

    void SetTaskType()
    {
        switch (taskData.GetTaskType())
        {
            case TaskType.Story: StoryTypeSet(); break;
            case TaskType.Battle: BattleTypeSet(); break;
        }
    }

    void StoryTypeSet()
    {
        TaskTypeText.text = "ST";
        TaskTypeText.color = new Color32(255,255,255,255);
    }

    void BattleTypeSet()
    {
        TaskTypeText.text = "BT";
        TaskTypeText.color = new Color32(255, 0, 0, 255);
    }


    void OnStarButtonClick()
    {
        taskData.IsStar = !taskData.IsStar;
        UpStarButtonSpire();
    }

    void OnCheckButtonClick()
    {
        ToggleTaskPanel();
    }


    void UpStarButtonSpire()
    {
        if (StarButton == null) return;
        if (taskData.IsStar)
        {
            StarButton.gameObject.GetComponent<Image>().sprite = StarSprites[1];
        }
        else
        {
            StarButton.gameObject.GetComponent<Image>().sprite = StarSprites[0];

        }

    }

    public void ShowTaskPanel(PanelSaveData data)
    {
        if (data != null && !data.isActive) return;

        ToggleTaskPanel(data);
    }


    void ToggleTaskPanel(PanelSaveData data = null)
    {
        if (gameValue == null)
            gameValue = GameValue.Instance;

        if (taskData.TaskPanelInstance == null)
        {
            CreateTaskPanel(data);
        }
        else
        {
            if (!IsPanelOnTop(taskData.TaskPanelInstance))
            {
                taskData.TaskPanelInstance.transform.SetAsLastSibling();
            }
            else
            {
                taskData.TaskPanelInstance.GetComponent<TaskPanelControl>().ClosePanel();
            }
        }
    }


    void CreateTaskPanel(PanelSaveData data = null)
    {
        RectTransform PrefabLocation = BottomButton.Instance.PrefabLocation;
        Vector2 spawnPos;
        int siblingIndex = -1;

        if (data != null)
        {
            spawnPos = data.GetPosition();   
            siblingIndex = data.order;      
        }
        else
        {
            spawnPos = GetSpawnPosition(PrefabLocation.childCount);
        }

        // 选择 prefab
        GameObject prefab = (taskData.GetSizeType() == TaskSize.Small)
            ? taskSmallPanelPrefab
            : taskPanelPrefab;

        taskData.TaskPanelInstance = Instantiate(prefab, PrefabLocation);
        RectTransform instanceRect = taskData.TaskPanelInstance.GetComponent<RectTransform>();

        instanceRect.anchoredPosition = spawnPos;
        instanceRect.localScale = Vector3.one;
        if (siblingIndex >= 0 && siblingIndex < PrefabLocation.childCount)
        {
            taskData.TaskPanelInstance.transform.SetSiblingIndex(siblingIndex);
        }
            taskData.TaskPanelInstance.GetComponent<TaskPanelControl>().ShowTaskPanel(
            taskData, characterAssistRowControl, this
        );
    }

    bool IsPanelOnTop(GameObject panel)
    {
        Transform parent = panel.transform.parent;
        int lastIndex = parent.childCount - 1;
        return panel.transform.GetSiblingIndex() == lastIndex;
    }

    Vector2 GetSpawnPosition(int childCount)
    {
        RectTransform PrefabLocation = BottomButton.Instance.PrefabLocation;
        Vector2 center = PrefabLocation.rect.center;

        if (childCount <= 6)
            return center;

        float radius = Mathf.Min(PrefabLocation.rect.width, PrefabLocation.rect.height) * 0.5f;
        float angle = UnityEngine.Random.Range(0f, Mathf.PI * 2f);
        float distance = radius * 0.5f * Mathf.Sqrt(UnityEngine.Random.value);

        float offsetX = Mathf.Cos(angle) * distance;
        float offsetY = Mathf.Sin(angle) * distance;

        return center + new Vector2(offsetX, offsetY);
    }


    public void SetClear(int OptionsNum)
    {
        taskData.SetSelOption(OptionsNum);
        SetTaskState(TaskState.Clear);
    }


    public bool GetIsStar()
    {
        return taskData.IsStar;
    }

    public string GetTaskTitle()
    {
        return Title.text;
    }

    public int GetRemainTurn()
    {
        return 0;
    }

    public TaskType GetTaskType()
    {
        return taskData.GetTaskType();
    }


    void UpdataStateType()
    {
        Sprite markSprite = null;
        bool showNewMark = false;
        bool showClearMark = false;

        switch (taskData.GetTaskState())
        {
            case TaskState.New:
                markSprite = GetMark("Exclamation");
                showNewMark = true;
                break;

            case TaskState.UnCompleted:
                markSprite = GetMark("RedPoint");
                showNewMark = true;
                break;

            case TaskState.Clear:
            
                DeadLine.gameObject.SetActive(false);
                showClearMark = true;
                break;
        }

        if (newMark == null) return;
        newMark.SetActive(showNewMark);
        clearMark.SetActive(showClearMark);
        RightColumnManage.Instance.CheckTasksList();
        if (markSprite != null) newMark.GetComponent<Image>().sprite = markSprite;

    }

    void SetTaskState(TaskState state)
    {
        if (taskData.GetTaskState() == TaskState.Clear) return;
        taskData.SetTaskState(state);
        UpdataStateType();

    }

    public TaskData GetTaskData()
    {

        return taskData;
    }


    public TaskState GetTaskState() { 
    
        return taskData.GetTaskState();
    }

    public void ShowOutLine()
    {
        if (!isRightCol) return;
        if (!taskData.HasRegion()) return;
        Region region = taskData.GetRegionValue().region;
        if (RightColumnManage.Instance.tasksRowPrefab != this)
        {
            RightColumnManage.Instance.tasksRowPrefab = this;
            if (RightColumnManage.Instance.tasksRowPrefab.GetTaskData().GetRegionValue().region == region)
            {
                return;
            } else
            {
                region.ShowRegionPanel(taskData.GetCityIndex());
            }
        } else
        {
            region.ShowRegionPanel(taskData.GetCityIndex());
        }
    }

    void CloseOutLine()
    {
        if (!isRightCol) return;
        if (!taskData.HasRegion()) return;
    }

    void AddTaskToCity()
    {
        if (!isRightCol) return;
       // Debug.LogWarning("may be need remove, do better way");
        if (!taskData.HasRegion()) return;
        CityValue city = taskData.GetCityValue();
        if (city != null)
        {
            city.AddTaskData(taskData);
        }
    }

    void SetDeadLine()
    {
        int remTurn = taskData.GetDeadLineTurn();
        DeadLine.text = remTurn.ToString();
        ClockImage.sprite = GetClockSprite(remTurn <= 1);

        if (remTurn <= 1)
            DeadLine.color = Color.red;
        else
            DeadLine.color = Color.white; 
    }

    void SetCityInfo()
    {
        if (!isRightCol) return;
        Image cityCountryIcon = cityInfo.GetComponentInChildren<Image>();
        TextMeshProUGUI cityNameText = cityInfo.GetComponentInChildren<TextMeshProUGUI>();

        if (taskData.HasRegion())
        {

            CityValue city = taskData.GetCityValue();

            Sprite cityCountrySprite = city.GetCityCountryIcon();
            if (cityCountrySprite != null)
            {
                cityCountryIcon.gameObject.SetActive(true);
                cityCountryIcon.sprite = cityCountrySprite;
            }
            else
            {
                cityCountryIcon.gameObject.SetActive(false);
            }

            cityNameText.gameObject.SetActive(true);
            cityNameText.text = city.GetCityNameWithColor();

        } else
        {
            cityCountryIcon.gameObject.SetActive(false) ;
            cityNameText.gameObject.SetActive(false);

        }

    }
}
#endregion
#region TaskData


public class TaskData
{
    bool isStar;

    string taskKey;
    public GameObject TaskPanelInstance;


    public bool IsStar
    {
        get => isStar;
        set => SetProperty(ref isStar, value);
    }


    string storyKey;
    public StoryNode storyNode;
    TaskSize taskSize;
    TaskType taskType;
    TaskState taskState;

    int deadLineTurn = -1;
    string characterKey = CharacterConstants.NoneKey;
    int regionID = -1;
    int cityIndex = -1;


    public event Action OnValueChanged;

    public TaskData(ExcelTaskStory excelTaskStoryData, ExcelTaskValue excelTaskValue)
    {
        this.isStar = false;
        storyKey = excelTaskValue.storyKey;
        StoryNode storyNode = new StoryNode(excelTaskStoryData);
        this.storyNode = storyNode;

        deadLineTurn = excelTaskValue.deadLineTurn;
        characterKey = excelTaskValue.characterKey;
        regionID = excelTaskValue.regionID;
        cityIndex = excelTaskValue.cityIndex;

        if (excelTaskValue.taskID == Constants.RANDOM)
        {
            taskKey = GenerateUniqueTaskKey();
        }
        else
        {
            taskKey = excelTaskValue.taskID;
        }

        SetTaskState(TaskState.New);
    }



    public TaskData(ExcelTaskStory excelTaskStoryData, TaskSaveData saveData)
    {
        this.taskKey = saveData.taskKey;
        this.isStar = saveData.isStar;
        storyKey = saveData.storyKey;
        StoryNode storyNode = new StoryNode(excelTaskStoryData);
        this.storyNode = storyNode;

        deadLineTurn = saveData.deadLineTurn;
        characterKey = saveData.characterKey;
        regionID = saveData.regionID;
        cityIndex = saveData.cityIndex;
        SetTaskState((TaskState)saveData.taskState);
    }


    protected bool SetProperty<T>(ref T field, T value)
    {
        if (!EqualityComparer<T>.Default.Equals(field, value))
        {
            field = value;
            OnValueChanged?.Invoke();
            return true;
        }
        return false;
    }

    public string GetTaskKey()
    {
        return taskKey;
    }

    public string GetStoryKey()
    {
        return storyKey;
    }


    public int GetRegionID()
    {
        return regionID;
    }

    public TaskSize GetSizeType()
    {
        return taskSize;
    }

    public bool HasRegion()
    {
        return regionID != -1;
    }

    public int GetCityIndex()
    {
        if (HasRegion())
        {
            return cityIndex;
        }
        else
        {
            return -1;
        }
    }


    public RegionValue GetRegionValue()
    {
        return GameValue.Instance.GetRegionValue(regionID);
    }

    public CityValue GetCityValue()
    {
        if (HasRegion()) {
            return GameValue.Instance.GetRegionValue(regionID).GetCityValue(cityIndex);
        } else
        {
            return null;
        }

    }

    public int GetDeadLineTurn()
    {
        return deadLineTurn;
    }

    public void SetTaskState(TaskState taskState)
    {
        this.taskState = taskState;
        OnValueChanged?.Invoke();
    }

    public TaskSize GetTaskSize()
    {
        return taskSize;
    }

    public TaskType GetTaskType()
    {
        return taskType;
    }

    public TaskState GetTaskState()
    {
        return taskState;
    }



    public void SetSelOption(int selIndex)
    {
        storyNode.selOption = selIndex;
    }


    public string GetTitle()
    {
        return storyNode.GetTitle();
    }

    public string GetCharacterKey()
    {
        return characterKey;
    }

    public String GetOptionsAsk()
    {
        return storyNode.GetOptionsAsk();
    }

    public Sprite GetStoryImage()
    {
        string path = storyNode.storyImage;
        Sprite sprite = Resources.Load<Sprite>(path);
        return sprite;
    }

    public int GetOptionsNum()
    {
        return storyNode.GetOptionsNum();
    }

    public int GetSelOption()
    {
        return storyNode.selOption;
    }

    public String GetOptionText(int index)
    {
        return storyNode.GetOptionText(index);
    }

    public String GetOptionCostString(int index)
    {
        return storyNode.OptionsCost[index];
    }

    public int GetOptionsActionCount()
    {
        return storyNode.OptionsAction.Count;
    }

    public string GetOptionsActionString(int index)
    {
        return storyNode.OptionsAction[index];
    }

    public StoryNode GetStoryNode()
    {
        return storyNode;
    }

    private string GenerateUniqueTaskKey()
    {
        string newKey;
        do
        {
            newKey = $"{Constants.RANDOM}_{Guid.NewGuid().ToString()}";
        } while (GameValue.Instance.HasTask(newKey)); 

        return newKey;
    }


}

#endregion

#region TaskSaveData

public class TaskSaveData
{
    public string taskKey;
    public bool isStar;
    public string storyKey;

    public TaskSize taskSize;
    public TaskType taskType;
    public TaskState taskState;


    public int deadLineTurn = -1;
    public string characterKey = CharacterConstants.NoneKey;
    public int regionID = -1;
    public int cityIndex = -1;

    public TaskSaveData(TaskData taskData)
    {
        if (taskData == null) return;
        this.taskKey = taskData.GetTaskKey();
        this.isStar = taskData.IsStar;
        this.storyKey = taskData.GetStoryKey();

        taskSize = taskData.GetTaskSize();
        taskType = taskData.GetTaskType();
        taskState =  taskData.GetTaskState();

        deadLineTurn = taskData.GetDeadLineTurn();
        characterKey = taskData.GetCharacterKey();
        regionID = taskData.GetRegionID();
        cityIndex = taskData.GetCityIndex();
    }


}

#endregion