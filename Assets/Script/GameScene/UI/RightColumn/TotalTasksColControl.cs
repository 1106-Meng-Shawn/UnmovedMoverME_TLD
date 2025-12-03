using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;
using static ExcelReader;
using static GetSprite;

public class TotalTasksColControl : MonoBehaviour, ITotalColControl
{
    [Header("UI References")]
    public GameObject Content;
    [SerializeField] private GameObject rowPrefab;
    public RectTransform PrefabLocation;

    [Header("Controls")]
    public Button isStarButton;
    public Button nameSortButton;
    public Button remSortButton;
    public Button typeButton;
    public GameObject typeButtonsPanel;
    public List<Button> typeButtons;


    [Header("Dependencies")]
    public CharacterAssistRowControl characterAssistRowControl;
    public GameValue gameValue;
    public StoryControl storyControl;

    [Header("Story Graphs")]
    public StoryGraph taskGraph;
    public TaskSortControl taskSortControl;
    private bool isTypePanelOpen = false;

    public void InitStart()
    {
        if (gameValue == null) gameValue = GameValue.Instance;
        LoadTaskData();
    }

    public void LoadTaskData()
    {
        List<TaskData> tasksDatas = new List<TaskData> ();
        foreach (var node in GameValue.Instance.GetTotalTaskList())
        {
            tasksDatas.Add(node);
        }
        taskSortControl.SetTaskList(tasksDatas);
    }

    void TogglePanel()
    {
        bool isHovering = IsMouseOverTypeUI();

        if (isHovering)
        {
            if (!isTypePanelOpen)
            {
                typeButtonsPanel.SetActive(true);
                isTypePanelOpen = true;
            }
        }
        else
        {
            if (isTypePanelOpen)
            {
                typeButtonsPanel.SetActive(false);
                isTypePanelOpen = false;
            }
        }
    }

    bool IsMouseOverTypeUI()
    {
        Vector2 mousePos = Input.mousePosition;

        bool overButton = RectTransformUtility.RectangleContainsScreenPoint(
            typeButton.transform.parent.GetComponent<RectTransform>(),
            mousePos,
            null 
        );

        bool overPanel = typeButtonsPanel.activeSelf && RectTransformUtility.RectangleContainsScreenPoint(
            typeButtonsPanel.GetComponent<RectTransform>(),
            mousePos,
            null
        );

        return overButton || overPanel;
    }


    public string CheckTaskList()
    {
        bool hasUncompleted = false;

        foreach (var task in taskSortControl.tasksRows)
        {
            var state = task.GetTaskState();

            if (state == TaskState.New)
            {
                return "exclamation"; 
            }

            if (state == TaskState.UnCompleted) hasUncompleted = true;
        }

        return hasUncompleted ? "redpoint" : "clear";
    }

    GameValue ITotalColControl.gameValue
    {
        get => gameValue;
        set => gameValue = value;
    }


    public void ShowOrHide(bool isShow)
    {
        gameObject.SetActive(isShow);
        if (isShow) {
            LoadTaskData();
            taskSortControl.RefreshTaskRows(); 
        }
    }

    public void ShowTaskPanel(PanelSaveData panelSaveData)
    {
        taskSortControl.ShowTaskPanel(panelSaveData);
    }

}
