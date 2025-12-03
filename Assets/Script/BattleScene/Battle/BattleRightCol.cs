using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static GetSprite;

public class BattleRightCol : MonoBehaviour
{
    public Image seasonBackground;
    public Button showButton;
    public Button settingButton;

    public bool isShow = false;
    public new Animation animation;

    private Dictionary<string, Sprite> iconCache = new Dictionary<string, Sprite>();
    private string[] showIconNames = new string[] { "RightColShow", "RightColUnShow", "RightColClose", "RightColUnClose" };
    public GameObject exploreRecordPrefab;
    public ScrollRect scrollRect;
    private List<BattleRecordControl> battleRecords = new List<BattleRecordControl>();

    public static BattleRightCol Instance { get; private set; }




    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;

    }


    private void Start()
    {
        SetRightCol();
        showButton.onClick.AddListener(ToggleColumn);
        settingButton.onClick.AddListener(OnSettingButtonClick);
        CacheShowIcons(showIconNames);
    }


    void OnSettingButtonClick()
    {
        SettingsManager.Instance.OpenPanel();
    }


    private void Update()
    {
        if (isShow)
        {
            if (Input.GetMouseButtonDown(1)) ToggleColumn();
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


    public void SetRightCol()
    {
        Debug.Log($"GameValue currentSeason is {GameValue.Instance.GetCurrentSeason()}");
        seasonBackground.sprite = GetSeasonSpriteCol(GameValue.Instance.GetCurrentSeason());
    }

    void ToggleColumn()
    {
        CharacterOperControl.Instance.SwitchButtonActive();

        if (isShow)
        {
            CloseCol();
        }
        else
        {
            ShowCol();
        }


    }


    public void ShowCol()
    {
        if (!isShow)
        {
            animation.Play("ExploreShowRightColumn");
            isShow = true;
            showButton.GetComponent<Image>().sprite = iconCache["RightColUnClose"];
            showButton.GetComponent<ButtonEffect>().SetChangeSprite(iconCache["RightColClose"], iconCache["RightColUnClose"]);
        }

    }

    public void CloseCol()
    {
        if (isShow)
        {
            animation.Play("ExploreHideRightColumn");
            isShow = false;
            showButton.GetComponent<Image>().sprite = iconCache["RightColUnShow"];
            showButton.GetComponent<ButtonEffect>().SetChangeSprite(iconCache["RightColShow"], iconCache["RightColUnShow"]);
            return;
        }

    }


    public void StartBattleRecord()
    {
        ClearBattleRecords();
        AddRecord(r => r.BattleStartSet());
    }

    public void SetTurnRecord(int turn, bool isEnemy)
    {
        AddRecord(r => r.TurnSet(turn, isEnemy));
    }

    public void SetCharacterSkillRecord(BattleCharacterValue battleCharacterValue, Skill skill)
    {
        AddRecord(r => r.CharacterSkillSet(battleCharacterValue, skill));
    }

    public void ValueChangeRecord(BattleCharacterValue battleCharacterValue, List<(BattleValue valueIndex, int delta)> values)
    {
        AddRecord(r => r.CharacterValueChangeSet(battleCharacterValue, values));
    }

    public void CriticalRecord(BattleCharacterValue battleCharacterValue)
    {
        AddRecord(r => r.CriticalSet(battleCharacterValue));
    }

    public void BlockRecord(BattleCharacterValue battleCharacterValue)
    {
        AddRecord(r => r.BlockSet(battleCharacterValue));
    }

    public void DamageTakenRecord(BattleCharacterValue battleCharacterValue, DamageResult damageResult)
    {
        AddRecord(r => r.DamageTakenSet(battleCharacterValue, damageResult));
    }



    private void AddRecord(System.Action<BattleRecordControl> initAction)
    {
        Transform content = scrollRect.content;
        GameObject newRecordGO = Instantiate(exploreRecordPrefab, content);
        BattleRecordControl record = newRecordGO.GetComponent<BattleRecordControl>();

        initAction?.Invoke(record);  

        battleRecords.Add(record);

        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)scrollRect.content);
    }


    void ClearBattleRecords()
    {
        foreach (Transform child in scrollRect.content)
        {
            Destroy(child.gameObject);
        }
        battleRecords.Clear();
    }


}
