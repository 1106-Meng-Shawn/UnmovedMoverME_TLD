// CountryRegionPanelControl.cs
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static GetSprite;

public class CountryRegionPanelControl : MonoBehaviour
{
    public ScrollRect scrollRect;
    public GameObject CountryRegionRow;
    public List<CountryRegionRowControl> countryRegionRowControls = new List<CountryRegionRowControl>();

    public List<Image> valueIcon;
    public List<Button> sortButtons;
    public Button resTypeButton;
    public List<Button> resTypeButtons;

    private int currentValue = 5; // 当前资源类型索引

    public enum SortDirection { None, Ascending, Descending }
    public enum SortFieldType { None, RegionName, LordName, Value }

    public class SortStatus
    {
        public SortFieldType FieldType;
        public int ResourceTypeIndex; // 0-5, 对应人口、食物等
        public int ValueIndex;        // 0-6，对应当前显示的资源属性（如最大值、当前值等）
        public SortDirection Direction;

        public bool IsSorted => Direction != SortDirection.None;

        public bool Matches(int resType, int valueIndex)
        {
            return FieldType == SortFieldType.Value && ResourceTypeIndex == resType && ValueIndex == valueIndex;
        }
    }

    private SortStatus currentSort;

    // 文化比较器，支持多语言排序
    private CompareInfo compareInfo = CultureInfo.CurrentCulture.CompareInfo;
    private CompareOptions compareOptions = CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace;

    void Start()
    {
        // GameValue.Instance.PlayerRegions.OnListChanged += OnPlayerRegionsChanged;
        GameValue.Instance.RegisterPlayerRegionsChanged(OnPlayerRegionsChanged);
        InitButtons();
    }

    void OnEnable() => DisplayRegions();

    void OnDestroy()
    {
        if (GameValue.Instance != null)
            GameValue.Instance.UnRegisterPlayerRegionsChanged(OnPlayerRegionsChanged);
    }

    void InitButtons()
    {
        resTypeButton.onClick.AddListener(OnResTypeButtonClick);
        for (int i = 0; i < resTypeButtons.Count; i++)
        {
            int index = i;
            resTypeButtons[i].onClick.AddListener(() => OnResTypeButtonsClick(index));
        }
        InitSortButtons();
    }

    void InitSortButtons()
    {
        currentSort = new SortStatus
        {
            FieldType = SortFieldType.None,
            ResourceTypeIndex = -1,
            ValueIndex = -1,
            Direction = SortDirection.None
        };

        for (int i = 0; i < sortButtons.Count; i++)
        {
            int index = i;
            sortButtons[i].onClick.AddListener(() => OnSortButtonClicked(index));
        }

        UpdateSortIcons();
    }

    void OnSortButtonClicked(int index)
    {
        SortFieldType fieldType;
        int valueIndex = -1;

        if (index == 0) fieldType = SortFieldType.LordName;
        else if (index == 1) fieldType = SortFieldType.RegionName;
        else
        {
            fieldType = SortFieldType.Value;
            valueIndex = index - 2;
        }

        bool isSameField =
            fieldType == currentSort.FieldType &&
            (fieldType == SortFieldType.Value ? currentSort.Matches(currentValue, valueIndex) : true);

        if (isSameField)
        {
            // 升序 -> 降序 -> 无序 的排序循环
            currentSort.Direction = currentSort.Direction switch
            {
                SortDirection.Ascending => SortDirection.Descending,
                SortDirection.Descending => SortDirection.None,
                _ => SortDirection.Ascending
            };

            if (currentSort.Direction == SortDirection.None)
            {
                currentSort.FieldType = SortFieldType.None;
                currentSort.ValueIndex = -1;
                currentSort.ResourceTypeIndex = -1;
            }
        }
        else
        {
            currentSort = new SortStatus
            {
                FieldType = fieldType,
                ResourceTypeIndex = currentValue,
                ValueIndex = valueIndex,
                Direction = SortDirection.Ascending // 新字段默认升序开始
            };
        }

        ApplySort();
        UpdateSortIcons();
    }

    void ApplySort()
    {
        if (currentSort == null) return;

        if (!currentSort.IsSorted)
        {
            // 无排序，恢复默认顺序（按RegionID）
            countryRegionRowControls = countryRegionRowControls.OrderBy(c => c.GetRegionID()).ToList();
        }
        else
        {
            Func<CountryRegionRowControl, IComparable> keySelector = currentSort.FieldType switch
            {
                SortFieldType.RegionName => r => r.GetRegionName() ?? "",
                SortFieldType.LordName => r => GetLordSortKey(r, currentSort.Direction),
                SortFieldType.Value => r => r.GetRowValue(currentSort.ValueIndex),
                _ => r => 0
            };

            countryRegionRowControls = currentSort.Direction == SortDirection.Ascending
                ? countryRegionRowControls.OrderBy(keySelector).ToList()
                : countryRegionRowControls.OrderByDescending(keySelector).ToList();
        }

        RefreshRows();
    }

    /// <summary>
    /// 针对 LordName 的排序键生成（带多语言排序支持）
    /// 升序时有 lord 在前，无 lord 后；降序反过来。
    /// </summary>
    IComparable GetLordSortKey(CountryRegionRowControl row, SortDirection dir)
    {
        string name = row.GetLordName();

        bool hasLord = !string.IsNullOrEmpty(name);

        if (dir == SortDirection.Ascending)
        {
            return (hasLord ? 0 : 1, hasLord ? name : "");
        }
        else if (dir == SortDirection.Descending)
        {
            return (hasLord ? 1 : 0, hasLord ? name : "");
        }
        else
        {
            return (0, "");
        }
    }

    void UpdateSortIcons()
    {
        if (currentSort == null) return;

        for (int i = 0; i < sortButtons.Count; i++)
        {
            string spriteName = "None";

            // regionName 和 lordName 图标显示顺序为：降序图标 -> 升序图标 -> 无序图标
            if (i == 0 && currentSort.FieldType == SortFieldType.LordName)
                spriteName = GetDisplayIconName(currentSort.Direction);
            else if (i == 1 && currentSort.FieldType == SortFieldType.RegionName)
                spriteName = GetDisplayIconName(currentSort.Direction);
            else if (i >= 2 && currentSort.FieldType == SortFieldType.Value &&
                     currentSort.ValueIndex == i - 2 &&
                     currentSort.ResourceTypeIndex == currentValue)
                spriteName = currentSort.Direction.ToString();

            var img = sortButtons[i].GetComponent<Image>();
            img.sprite = GetSortSprite(spriteName);
        }
    }

    // 图标显示映射：升序显示降序图标，降序显示升序图标，无序显示无图标
    string GetDisplayIconName(SortDirection dir)
    {
        return dir switch
        {
            SortDirection.Ascending => "Descending",
            SortDirection.Descending => "Ascending",
            _ => "None"
        };
    }

    void RefreshRows()
    {
        for (int i = 0; i < countryRegionRowControls.Count; i++)
            countryRegionRowControls[i].transform.SetSiblingIndex(i);
    }

    void OnResTypeButtonClick() => SetCurrentValue((currentValue + 1) % resTypeButtons.Count);

    void OnResTypeButtonsClick(int index) => SetCurrentValue(index);

    void SetCurrentValue(int newCurrentValue)
    {
        currentValue = newCurrentValue;

        foreach (var row in countryRegionRowControls)
            row.SetCurrentValueType(currentValue);

        resTypeButton.GetComponent<Image>().sprite = resTypeButtons[currentValue].GetComponent<Image>().sprite;

        UpdateSortIcons();
        SetResValueImages();
    }

    void SetResValueImages()
    {
        string[] typeNames = { "Food", "Science", "Politics", "Gold", "Faith", "Population" };
        string typeName = typeNames[Mathf.Clamp(currentValue, 0, typeNames.Length - 1)];

        if (typeName == "Population") { UpPopulationIcon(); return; }
        valueIcon[0].sprite = Resources.Load<Sprite>($"MyDraw/UI/Region/Value/{typeName}/{typeName}MAX");


        valueIcon[1].sprite = Resources.Load<Sprite>($"MyDraw/UI/Region/Value/{typeName}/{typeName}NOW");
        valueIcon[2].sprite = Resources.Load<Sprite>($"MyDraw/UI/Region/Value/{typeName}/{typeName}Growth%");
        valueIcon[3].sprite = Resources.Load<Sprite>($"MyDraw/UI/Region/Value/{typeName}/{typeName}P");
        valueIcon[4].sprite = Resources.Load<Sprite>($"MyDraw/UI/Region/Value/{typeName}/{typeName}Take");
        valueIcon[5].sprite = Resources.Load<Sprite>($"MyDraw/UI/Region/Value/{typeName}/{typeName}NEXT");
        valueIcon[6].sprite = Resources.Load<Sprite>($"MyDraw/UI/Region/Value/{typeName}/{typeName}NEXTTake");
    }

    void UpPopulationIcon()
    {
        valueIcon[0].sprite = Resources.Load<Sprite>("MyDraw/UI/Region/Value/Population/RegionPopulationMAX");
        valueIcon[1].sprite = Resources.Load<Sprite>("MyDraw/UI/Region/Value/Population/RegionPopulationNOW");
        valueIcon[2].sprite = Resources.Load<Sprite>("MyDraw/UI/Region/Value/Population/RegionPopulationGrowth%");
        valueIcon[3].sprite = Resources.Load<Sprite>("MyDraw/UI/Region/Value/Population/RegionPopulationNEXT");
        valueIcon[4].sprite = Resources.Load<Sprite>("MyDraw/UI/Region/Value/Population/SupportRate");
        valueIcon[5].sprite = Resources.Load<Sprite>("MyDraw/UI/Region/Value/Population/AvailablePopulation");
        valueIcon[6].sprite = Resources.Load<Sprite>("MyDraw/UI/Region/Value/Population/RecruitmentPopulation");
    }

    void OnPlayerRegionsChanged() => DisplayRegions();

    void DisplayRegions()
    {
        foreach (Transform child in scrollRect.content)
        {
            if (child.CompareTag("Prefab")) Destroy(child.gameObject);
        }

        var allRegions = GameValue.Instance.GetAllRegionValues();
        countryRegionRowControls.Clear();

        foreach (var region in allRegions)
        {
            if (region != null && region.GetCountryENName() == GameValue.Instance.GetPlayerCountryENName())
            {
                GameObject rowObj = Instantiate(CountryRegionRow, scrollRect.content);
                rowObj.tag = "Prefab";

                var rowCtrl = rowObj.GetComponent<CountryRegionRowControl>();
                rowCtrl.SetRegionValue(region);
                rowCtrl.SetCurrentValueType(currentValue);
                countryRegionRowControls.Add(rowCtrl);
            }
        }

        SetCurrentValue(currentValue);
        ApplySort();

        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(scrollRect.content.GetComponent<RectTransform>());
        scrollRect.verticalScrollbar.value = 1;
    }
}
