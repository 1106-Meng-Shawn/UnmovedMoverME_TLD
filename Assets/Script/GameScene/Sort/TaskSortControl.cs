using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static GetSprite;

public class TaskSortControl : MonoBehaviour
{
    public ScrollRect scrollRect;
    public GameObject taskContentPrefab;

    public List<TasksRowPrefab> tasksRows = new List<TasksRowPrefab>();

    [Header("Buttons")]
    public Button isTaskStarButton;
    public Button nameSortButton;
    public Button remSortButton;
    public Button typeButton;
    public GameObject typeButtonsPanel;
    public List<Button> typeButtons;

    private bool isStar = false;

    private TaskType currentType = TaskType.All;
    private SortField currentSortField = SortField.None;
    private SortState currentSortState = SortState.None;

    enum SortField { None, Name, Remain }
    enum SortState { None, Ascending, Descending }

    private void Awake()
    {
        InitButtons();
    }

    public void ResetState()
    {
        currentType = TaskType.All;
        currentSortField = SortField.None;
        currentSortState = SortState.None;
        ToggleStarFilter(false);
    }

    public void SetTaskList(List<TaskData> taskDatas)
    {
        ResetState();
        foreach (Transform child in scrollRect.content)
        {
            Destroy(child.gameObject);
        }
        tasksRows.Clear();
        foreach (var taskData in taskDatas)
        {
            GameObject taskObj = Instantiate(taskContentPrefab, scrollRect.content);
            TasksRowPrefab taskUI = taskObj.GetComponent<TasksRowPrefab>();
            if (taskUI != null)
            {
                taskUI.SetTaskData(taskData);
                tasksRows.Add(taskUI);
            }
        }
        RefreshTaskRows();
    }

    void InitButtons()
    {
        isTaskStarButton?.onClick.AddListener(OnStarButtonClick);
        nameSortButton?.onClick.AddListener(() => OnClickSort(SortField.Name));
        remSortButton?.onClick.AddListener(() => OnClickSort(SortField.Remain));
        typeButton?.onClick.AddListener(CycleTaskType);

        for (int i = 0; i < typeButtons.Count; i++)
        {
            int index = i;
            typeButtons[i].onClick.AddListener(() => SetTypeButton((TaskType)index));
        }
    }

    void OnStarButtonClick()
    {
        bool newStar = !isStar;
        ToggleStarFilter(newStar);
    }

    void ToggleStarFilter(bool newStar)
    {
        isStar = newStar;
        isTaskStarButton.image.sprite = UpStarButtonSprite(isStar);
        RefreshTaskRows();
    }


    void CycleTaskType()
    {
        currentType = (TaskType)(((int)currentType + 1) % 3);
        RefreshTaskRows();
    }

    void SetTypeButton(TaskType newType)
    {
        currentType = newType;
        RefreshTaskRows();
    }

    void OnClickSort(SortField field)
    {
        if (currentSortField == field)
        {
            currentSortState = NextSortState(currentSortState);
        }
        else
        {
            currentSortField = field;
            currentSortState = SortState.Descending;
        }
        RefreshTaskRows();
    }

    SortState NextSortState(SortState state) => state switch
    {
        SortState.None => SortState.Descending,
        SortState.Descending => SortState.Ascending,
        SortState.Ascending => SortState.None,
        _ => SortState.None
    };

    public void RefreshTaskRows()
    {
        IEnumerable<TasksRowPrefab> filtered = tasksRows;

        // ??
        if (isStar) filtered = filtered.Where(t => t.GetIsStar());
        switch (currentType)
        {
            case TaskType.Story:
                filtered = filtered.Where(t => t.GetTaskType() == TaskType.Story);
                break;
            case TaskType.Battle:
                filtered = filtered.Where(t => t.GetTaskType() == TaskType.Battle);
                break;
        }

        // ??
        filtered = (currentSortField, currentSortState) switch
        {
            (SortField.Name, SortState.Descending) => filtered.OrderBy(t => t.GetTaskTitle()),
            (SortField.Name, SortState.Ascending) => filtered.OrderByDescending(t => t.GetTaskTitle()),
            (SortField.Remain, SortState.Descending) => filtered.OrderByDescending(t => t.GetRemainTurn()),
            (SortField.Remain, SortState.Ascending) => filtered.OrderBy(t => t.GetRemainTurn()),
            _ => filtered
        };

        // ???? hierarchy
        int index = 0;
        foreach (var row in filtered)
        {
            row.gameObject.SetActive(true);
            row.transform.SetSiblingIndex(index++);
        }

        // ??????
        foreach (var row in tasksRows.Except(filtered))
        {
            row.gameObject.SetActive(false);
        }

        UpdateSortIcons();
        UpdateTypeButtonText();
    }

    void UpdateSortIcons()
    {
        nameSortButton.GetComponent<Image>().sprite =
            GetSortSprite(currentSortField == SortField.Name ? currentSortState.ToString() : SortState.None.ToString());

        remSortButton.GetComponent<Image>().sprite =
            GetSortSprite(currentSortField == SortField.Remain ? currentSortState.ToString() : SortState.None.ToString());
    }

    void UpdateTypeButtonText()
    {
        var srcBtn = typeButtons[(int)currentType];
        var srcText = srcBtn.GetComponentInChildren<TextMeshProUGUI>();
        var dstText = typeButton.GetComponentInChildren<TextMeshProUGUI>();

        dstText.text = srcText.text;
        dstText.color = srcText.color;
    }

    public void ShowTaskPanel(PanelSaveData panelSaveData)
    {
        string taskID = panelSaveData.GetTaskID();

        if (string.IsNullOrEmpty(taskID)) return;

        foreach (var taskRow in tasksRows)
        {
            if (taskRow.GetTaskData().GetTaskKey() == taskID) 
            {
                taskRow.ShowTaskPanel(panelSaveData);
                break; 
            }
        }
    }


}
