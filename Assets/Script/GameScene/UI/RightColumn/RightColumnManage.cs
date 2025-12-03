using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Localization.Settings;
using static GetSprite;


public interface ITotalColControl
{
    GameValue gameValue { get; set; }
    void ShowOrHide(bool show);
}

public class RightColumnManage : PanelBase
{
    public static RightColumnManage Instance { get; private set; }
    public TextMeshProUGUI rightColTitle;
    public Button ShowButton;
    public bool isShow = false;

    public new Animation animation;

    public Button tasksButton;
    public Button eventsButton;
    public Button newsButton;

    //public List<ColumnControl> ColumnControls;
    public TotalTasksColControl totalTasksColControl;
    public TotalEventsColControl totalEventsColControl;
    public TotalNewsColControl totalNewsColControl;


    public GameValue gameValue;

    private int currentColumn = 0;

    private ButtonEffect[] buttonEffects;
    private Dictionary<string, Sprite> iconCache = new Dictionary<string, Sprite>();
    private string[] iconNames = new string[] { "TASKS", "EVENTS", "REPORTS" };
    private string[] showIconNames = new string[] { "RightColShow", "RightColUnShow", "RightColClose", "RightColUnClose" };


    public TasksRowPrefab tasksRowPrefab;


    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;

        buttonEffects = new ButtonEffect[]
        {
            tasksButton.GetComponent<ButtonEffect>(),
            eventsButton.GetComponent<ButtonEffect>(),
            newsButton.GetComponent<ButtonEffect>()
        };

        // ??????
        CacheIcons(iconNames);
        CacheShowIcons(showIconNames);

        if (gameValue == null)
            gameValue = FindObjectOfType<GameValue>();

    }


    void Start()
    {
        // ?????????

        // ??????
        RectTransform rect = GetComponent<RectTransform>();
        Vector2 anchoredPos = rect.anchoredPosition;
        anchoredPos.x = 1173;
        rect.anchoredPosition = anchoredPos;

        // ????
        ShowButton.onClick.AddListener(() => ShowPanel());
        tasksButton.onClick.AddListener(() => ToggleColumn(0));
        eventsButton.onClick.AddListener(() => ToggleColumn(1));
        newsButton.onClick.AddListener(() => ToggleColumn(2));



        InitColData();
    }


    void InitColData()
    {
        totalTasksColControl.InitStart();
        totalEventsColControl.InitStart();

    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1) && isShow)ClosePanel();

        if (Input.GetKeyDown(KeyCode.Alpha0)) ToggleColumn(currentColumn);

        if (isShow && Input.GetKeyDown(KeyCode.Tab)) ToggleColumn(GetNextCurrentCol());

    }

    int GetNextCurrentCol()
    {
        return (currentColumn + 1) % 3;
    }


    void CacheIcons(string[] names)
    {
        string iconPath = $"MyDraw/UI/GameUI/";
        foreach (var name in names)
        {
            if (!iconCache.ContainsKey(name + "IconSel"))
                iconCache[name + "IconSel"] = Resources.Load<Sprite>(iconPath + name + "IconSel");

            if (!iconCache.ContainsKey(name + "IconUnSel"))
                iconCache[name + "IconUnSel"] = Resources.Load<Sprite>(iconPath + name + "IconUnSel");
        }
    }

    void CacheShowIcons(string[] names)
    {
        string iconPath = $"MyDraw/UI/GameUI/";
        foreach (var name in names)
        {
            if (!iconCache.ContainsKey(name))
                iconCache[name] = Resources.Load<Sprite>(iconPath + name);
        }
    }


    public void ToggleEventCol()
    {
        ToggleColumn(1);
    }

    public void ShowPanel()
    {
        ToggleColumn(currentColumn);
    }
    

    public override void ClosePanel()
    {
        animation.Play("HideRightColumn");
        isShow = false;
        tasksRowPrefab = null;
        ShowButton.GetComponent<Image>().sprite = iconCache["RightColUnShow"];
        ShowButton.GetComponent<ButtonEffect>().SetChangeSprite(iconCache["RightColShow"], iconCache["RightColUnShow"]);
        ChangeButtonUnsel();

    }

    public override  PanelSaveData GetPanelSaveData()
    {
        PanelSaveData saveData = new PanelSaveData();
        saveData.isActive = isShow;
        return saveData;
    }

    public override void SetPanelSaveData(PanelSaveData panelSaveData)
    {
        if (panelSaveData.isActive)
        {
            ShowPanel();
        }
    }



    void ToggleColumn(int index)
    {
        bool isSameColumn = (index == currentColumn);

        if (isSameColumn && isShow)
        {
            ClosePanel();
            return;
        }

        currentColumn = index;

        if (!isShow)
        {
            animation.Play("ShowRightColumn");
            isShow = true;
            ShowButton.GetComponent<Image>().sprite = iconCache["RightColUnClose"];
            ShowButton.GetComponent<ButtonEffect>().SetChangeSprite(iconCache["RightColClose"], iconCache["RightColUnClose"]);
        }

        HideAllColumns();

        switch (index)
        {
            case 0:
                SetupGameValue(totalTasksColControl);
                totalTasksColControl.ShowOrHide(true);
                break;
            case 1:
                SetupGameValue(totalEventsColControl);
                totalEventsColControl.ShowOrHide(true);
                break;
            case 2:
                SetupGameValue(totalNewsColControl);
                totalNewsColControl.ShowOrHide(true);
                break;
        }

        ChangeButtonSel();
        ChangeTitle();
    }

    void HideAllColumns()
    {
        totalTasksColControl.ShowOrHide(false);
        totalEventsColControl.ShowOrHide(false);
        totalNewsColControl.ShowOrHide(false);
    }

    void SetupGameValue(ITotalColControl col)
    {
        if (col.gameValue == null)
            col.gameValue = gameValue;
    }

    void ChangeButtonSel()
    {
        for (int i = 0; i < buttonEffects.Length; i++)
        {
            string key = iconNames[i];
            if (i == currentColumn)
            {
                buttonEffects[i].GetComponent<Image>().sprite = iconCache[key + "IconSel"];
                buttonEffects[i].SetChangeSprite(iconCache[key + "IconSel"], iconCache[key + "IconSel"]);
            }
            else
            {
                buttonEffects[i].GetComponent<Image>().sprite = iconCache[key + "IconUnSel"];
                buttonEffects[i].SetChangeSprite(iconCache[key + "IconSel"], iconCache[key + "IconUnSel"]);
            }
        }
    }


    void ChangeTitle()
    {
        string key = iconNames[currentColumn];
        rightColTitle.text = LocalizationSettings.StringDatabase.GetLocalizedString("GameSetting", key);

        Image parentImage = rightColTitle.transform.parent.GetComponent<Image>();

        if (parentImage != null)
        {
            parentImage.color = GetColorForColumn();
        }

    }

    Color32 GetColorForColumn()
    {
        switch (currentColumn) {
            case 0: return new Color32(230, 190, 120, 255);
            case 1: return new Color32(140, 85, 35, 255);
            case 2: return new Color32(255,255,255,255);

        }

        return Color.white;

    }

    void ChangeButtonUnsel()
    {
        for (int i = 0; i < buttonEffects.Length; i++)
        {
            string key = iconNames[i];
            buttonEffects[i].GetComponent<Image>().sprite = iconCache[key + "IconUnSel"];
            buttonEffects[i].SetChangeSprite(iconCache[key + "IconSel"], iconCache[key + "IconUnSel"]);
        }
    }

    public void CheckTasksList()
    {
        string type = totalTasksColControl.CheckTaskList();
        if (type == "clear")
        {
            tasksButton.transform.GetChild(0).gameObject.SetActive(false);
        }
        else
        {
            tasksButton.transform.GetChild(0).gameObject.SetActive(true);
            tasksButton.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = GetMark(type);

        }

    }

    public void CheckEventsList()
    {
        string type = totalEventsColControl.CheckEventsList();

        if (type == "Clear")
        {
            eventsButton.transform.GetChild(0).gameObject.SetActive(false);
        }
        else
        {
            eventsButton.transform.GetChild(0).gameObject.SetActive(true);
            eventsButton.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = GetMark(type);

        }

        if (type == "exclamation") {
            TurnManager.Instance.SetTurnState(TurnState.Event);
        }
        else
        {
            TurnManager.Instance.SetTurnState(TurnState.Next);
        }
    }

    public int CountTriggerableEvents()
    {
        return totalEventsColControl.CountTriggerableEvents();
    }


    public void ShowTaskPanel(PanelSaveData taskPanelSaveData)
    {
        totalTasksColControl.ShowTaskPanel(taskPanelSaveData);
    }

}
