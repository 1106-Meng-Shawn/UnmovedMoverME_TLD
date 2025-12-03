using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static GetSprite;


public class ColumnControl : MonoBehaviour
{

    public GameObject Content;


    [SerializeField]  private int Type;
    [SerializeField] private GameObject rowPrefab;

    [SerializeField] private List<GameObject> rowPrefabs;

    public CharacterAssistRowControl characterAssistRowControl;



    public EventPanelControl eventPanelControl;
    public RectTransform PrefabLocation;

    public GameValue gameValue;



    public StoryControl storyControl;
  // public StoryNode eventData;
   // public StoryNode taskData;
    //public EventGraph eventGraph;
     public StoryGraph eventGraph;


    // for task Scroll view
    public List<Button> buttons; // 0 is star , 1 is nameSort, 2 is remSort, 3 is typeButton;
    private bool eventIsStar = false;
    enum SortState { Default, Ascending, Descending }
    string taskCurrentSortField = "none"; // 可选值: "name", "time", etc.
    SortState taskCurrentSortState = SortState.Default;

    private void Start()
    {
        if (gameValue == null) gameValue = FindObjectOfType<GameValue>();
        InitTopRowButton();
    }

    void InitTopRowButton()
    {
        switch (Type)
        {
            case 1:
                UpEventTopRowButton();
                break;
            case 2:
                UpTastTopRowButton();
                break;

        }

    }

    void UpEventTopRowButton(){
        if (buttons.Count == 0) return;
        // 0 is star button
        buttons[0].gameObject.GetComponent<Image>().sprite = UpStarButtonSprite(eventIsStar);
        buttons[0].onClick.AddListener(OnStarButtonClick);
        buttons[1].onClick.AddListener(OnNameSortButtonClick);
        buttons[2].onClick.AddListener(OnRemSortButtonClick);
        buttons[3].onClick.AddListener(OnTypeButtonClick);

    }

    void OnStarButtonClick()
    {
        eventIsStar = !eventIsStar;
        buttons[0].gameObject.GetComponent<Image>().sprite = UpStarButtonSprite(eventIsStar);

    }

    void OnNameSortButtonClick()
    {

    }


    void OnRemSortButtonClick()
    {

    }

    void OnClickSort(string field)
    {
        if (taskCurrentSortField == field)
        {
            // 循环切换该字段的排序状态
            switch (taskCurrentSortState)
            {
                case SortState.Default:
                    taskCurrentSortState = SortState.Descending;
                    break;
                case SortState.Descending:
                    taskCurrentSortState = SortState.Ascending;
                    break;
                case SortState.Ascending:
                    taskCurrentSortState = SortState.Default;
                    break;
            }
        }
        else
        {
            // 切换到新字段，状态置为降序
            taskCurrentSortField = field;
            taskCurrentSortState = SortState.Descending;
        }

        ApplyTaskSort(taskCurrentSortField, taskCurrentSortState);
    }


    void ApplyTaskSort(string field, SortState state)
    {
        List<(GameObject go, TasksRowPrefab task)> taskPairs = new List<(GameObject, TasksRowPrefab)>();

        for (int i = 0; i < rowPrefabs.Count; i++)
        {
            var task = rowPrefabs[i].GetComponent<TasksRowPrefab>();
            taskPairs.Add((rowPrefabs[i], task));
        }

        switch (field)
        {
            case "name":
                if (state == SortState.Default)
                    //  taskPairs = taskPairs.OrderBy(t => t.task.defaultIndex).ToList();;
                    Debug.Log("haven't wirte sort default task by name");
                else if (state == SortState.Ascending)
                    taskPairs = taskPairs.OrderBy(t => t.task.GetTaskTitle()).ToList();
                else
                    taskPairs = taskPairs.OrderByDescending(t => t.task.GetTaskTitle()).ToList();
                break;

            case "rem":
                if (state == SortState.Default)
                    //   taskPairs = taskPairs.OrderBy(t => t.task.defaultIndex).ToList();
                    Debug.Log("haven't wirte sort default task by rem");
                else if (state == SortState.Ascending)
                    taskPairs = taskPairs.OrderBy(t => t.task.GetRemainTurn()).ToList();
                else
                    taskPairs = taskPairs.OrderByDescending(t => t.task.GetRemainTurn()).ToList();
                break;
        }

        for (int i = 0; i < taskPairs.Count; i++)
        {
            rowPrefabs[i] = taskPairs[i].go;
            rowPrefabs[i].transform.SetSiblingIndex(i);
        }
    }


    void OnTypeButtonClick()
    {

    }




    void UpTastTopRowButton()
    {

    }




    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpRowPreData()
    {
        switch (Type)
        {
            case 1: 
                UpEventData();
                break;
            case 2: 
                UpTaskData(); 
                break;

        }
    }

    void UpEventData()
    {
        if (eventGraph == null || eventGraph.rootNodes == null)
        {
            eventGraph = new StoryGraph();
            eventGraph.GenerateEventGraph();
        }

        List<StoryNode> needHappendEvent = eventGraph.rootNodes;

        for (int i = 0; i < needHappendEvent.Count; i++)
        {
            GameObject row = Instantiate(rowPrefab, Content.transform);


            row.GetComponent<EventsRowPrefab>().SetEventsRowPrefabNeed(needHappendEvent[i], eventPanelControl);
            rowPrefabs.Add(row);
        }

    }

    void UpTaskData()
    {



        List<TaskData> needHappenTask = GameValue.Instance.GetTotalTaskList();
        Debug.LogWarning("need to change storyNode to taskData, and taskGraph be TaskData");
        for (int i = 0; i < needHappenTask.Count; i++)
        {
            GameObject row = Instantiate(rowPrefab, Content.transform);
            row.GetComponent<TasksRowPrefab>().SetTaskData(needHappenTask[i]);
            rowPrefabs.Add(row);
        }

    }



    public void ShowOrHide(bool isShow)
    {
        gameObject.SetActive(isShow);

        if (isShow) UpRowPreData();

    }

}
