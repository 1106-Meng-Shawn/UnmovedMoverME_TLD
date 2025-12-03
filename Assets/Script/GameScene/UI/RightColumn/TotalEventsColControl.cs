using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public class TotalEventsColControl : MonoBehaviour, ITotalColControl
{

    public GameValue gameValue;
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
    public EventPanelControl eventPanelControl;
    public StoryControl storyControl;

    [Header("Story Graphs")]
    public StoryGraph eventGraph;

    private List<EventsRowPrefab> eventsRows = new List<EventsRowPrefab>();

    private int type = 0; // 0: all, 1: story, 2: battle
    private bool isStar = false;

    enum SortState { Default, Ascending, Descending }
    private string currentSortField = "none";
    private SortState currentSortState = SortState.Default;

    private bool isTypePanelOpen = false; 
    
    GameValue ITotalColControl.gameValue
    {
        get => gameValue;
        set => gameValue = value;
    }


    // Start is called before the first frame update
    public void InitStart()
    {
        LoadEventsData();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ShowOrHide(bool isShow)
    {
        gameObject.SetActive(isShow);
        if (isShow) RefreshEventRows();
    }

    void RefreshEventRows()
    {
     //   ClearEventsRows();
       // ApplyTaskFilters();
    }


    void ClearEventsRows()
    {
        foreach (var row in eventsRows)
        {
            Destroy(row.gameObject);
        }
        eventsRows.Clear();
    }

    public void LoadEventsData()
    {
        if (eventGraph == null || eventGraph.rootNodes == null)
        {
            eventGraph = new StoryGraph();
            eventGraph.GenerateEventGraph();
        }

        foreach (var node in eventGraph.rootNodes)
        {
            GameObject row = Instantiate(rowPrefab, Content.transform);
            EventsRowPrefab eventRow = row.GetComponent<EventsRowPrefab>();
            eventRow.SetEventsRowPrefabNeed(node, eventPanelControl);
                //(node, PrefabLocation, storyRemindPanelControl, gameValue, characterAssistRowControl);
            eventsRows.Add(eventRow);
        }
        RightColumnManage.Instance.CheckEventsList();
    }

    public string CheckEventsList()
    {
        bool hasUncompleted = false;

        foreach (var eventRow in eventsRows)
        {
            var state = eventRow.GetEventState();

            if (state == EventState.New)
                return "exclamation";

            if (state == EventState.UnCompleted)
                hasUncompleted = true;
        }

        return hasUncompleted ? "redpoint" : "Clear";
    }

    public int CountTriggerableEvents()
    {
        int count = 0;

        foreach (var eventRow in eventsRows)
        {
            var state = eventRow.GetEventState();

            if (state == EventState.New)
            {
                count++;
            }
        }

        return count;
    }



}
