using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static GetSprite;

public enum EventState
{
    New,
    UnCompleted,
    Clear
}


public class EventsRowPrefab : MonoBehaviour
{
    private bool isStar = false;
    private bool isComplete = false;

    public Button StarButton;
    public Button CheckButton;
    public Button DetailButton;
    public Button DeleteButton;


    public GameObject State;

    public TextMeshProUGUI Title;
    public TextMeshProUGUI DeadLine;

    public List<Sprite> StarSprites;
    public List<Sprite> DeleteSprites;


    public GameObject eventPanelPrefab;
    public GameObject eventSmallPanelPrefab;

    private EventPanelControl eventPanelControl;

    // [SerializeField] private EventNode eventData;
    [SerializeField] private StoryNode eventData;


    // public GameObject eventData;

    private EventState currentState;
    public GameObject newMark;
    public GameObject clearMark;



    private void Start()
    {
        DetailButton.onClick.AddListener(OnDetailButtonClick);
        StarButton.onClick.AddListener(OnStarButtonClick);
        CheckButton.onClick.AddListener(OnCheckButtonClick);


    }

    public void SetEventsRowPrefabNeed(StoryNode eventData, EventPanelControl eventPanelControl)
    {
        SetEventData(eventData);
        this.eventPanelControl = eventPanelControl;
        SetEventState(EventState.New);
    }

    public void SetClear(int OptionsNum)
    {
        eventData.selOption = OptionsNum;
        SetEventState(EventState.Clear);
    }

    void SetEventState(EventState state)
    {
        if (currentState == EventState.Clear) return;

        currentState = state;

        Sprite markSprite = null;
        bool showNewMark = false;
        bool showClearMark = false;

        switch (currentState)
        {
            case EventState.New:
                markSprite = GetMark("Exclamation");
                showNewMark = true;
                break;

            case EventState.UnCompleted:
                markSprite = GetMark("RedPoint");
                showNewMark = true;
                break;

            case EventState.Clear:
                markSprite = GetMark("RedPoint");
                showClearMark = true;
                break;
        }

        newMark.SetActive(showNewMark);
        clearMark.SetActive(showClearMark);

        if (markSprite != null)
            newMark.GetComponent<Image>().sprite = markSprite;


        RightColumnManage.Instance.CheckEventsList();

    }


    public void SetEventData(StoryNode eventData)
    {
        this.eventData = eventData;
        Title.text = eventData.GetTitle();

    }


    void OnCheckButtonClick()
    {

        if (eventData.GetOptionsNum() == 0 || eventData.GetOptionsNum() == 1)
        {
            eventPanelControl.FristOptionAction(eventData);
        } else
        {
            OnDetailButtonClick();
        }
    }



    void OnDetailButtonClick()
    {
        eventPanelControl.ShowEventPanel(eventData,this);
        SetEventState(EventState.UnCompleted);
    }


    void OnStarButtonClick()
    {
        if (isStar)
        {
            StarButton.gameObject.GetComponent<Image>().sprite = StarSprites[0];
            DeleteButton.gameObject.GetComponent<Image>().sprite = DeleteSprites[0];
            DeleteButton.interactable = true; // need to change by improtant event, can't be delete;
        } else
        {
            StarButton.gameObject.GetComponent<Image>().sprite = StarSprites[1];
            DeleteButton.gameObject.GetComponent<Image>().sprite = DeleteSprites[1];
            DeleteButton.interactable = false;

        }

        isStar = !isStar;
    }

    public EventState GetEventState()
    {
        return currentState;
    }
}
