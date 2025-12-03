using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static GetSprite;


public class ExploreRightCol : MonoBehaviour
{
    public Image seasonBackground;
    public Button showButton;
    public bool isShow = false;
    public new Animation animation;

    private Dictionary<string, Sprite> iconCache = new Dictionary<string, Sprite>();
    private string[] showIconNames = new string[] { "RightColShow", "RightColUnShow", "RightColClose", "RightColUnClose" };
    public GameObject exploreRecordPrefab;
    public ScrollRect scrollRect;
    private List<ExploreRecordPrefab> exploreRecords = new List<ExploreRecordPrefab>();

    public static ExploreRightCol Instance { get; private set; }


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
        CacheShowIcons(showIconNames);
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
        seasonBackground.sprite = GetSeasonSpriteCol(GameValue.Instance.GetCurrentSeason());
    }

    void ToggleColumn()
    {

        if (isShow)
        {
            animation.Play("ExploreHideRightColumn");
            isShow = false;
            showButton.GetComponent<Image>().sprite = iconCache["RightColUnShow"];
            showButton.GetComponent<ButtonEffect>().SetChangeSprite(iconCache["RightColShow"], iconCache["RightColUnShow"]);
            return;
        }


        if (!isShow)
        {
            animation.Play("ExploreShowRightColumn");
            isShow = true;
            showButton.GetComponent<Image>().sprite = iconCache["RightColUnClose"];
            showButton.GetComponent<ButtonEffect>().SetChangeSprite(iconCache["RightColClose"], iconCache["RightColUnClose"]);
        }

    }


    public void SetExploreRecord(ExploreRecordData exploreRecordData)
    {
        Transform content = scrollRect.content;
        GameObject newRecordGO = Instantiate(exploreRecordPrefab, content);
        ExploreRecordPrefab recordPrefab = newRecordGO.GetComponent<ExploreRecordPrefab>();
        recordPrefab.SetExploreRecordData(exploreRecordData);
        exploreRecords.Add(recordPrefab);
    }
}
