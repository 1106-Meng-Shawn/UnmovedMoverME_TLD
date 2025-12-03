using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static GetSprite;

public enum SortField
{
    Name, Favorability, Force, Health, Limit, Attack, FoodP, Leadership, Defense, ScienceP, Scout, Magic, PoliticsP, Build, Speed, GoldP, Negotiation, Lucky, FaithP, Charm,Level ,None
}

public enum SortDirection
{
    None, Ascending, Descending
}

public class SortStatus
{
    public SortField Field;
    public SortDirection Direction;

    public bool IsSorted => Field != SortField.None && Direction != SortDirection.None;

    public void ToggleSort(SortField targetField)
    {
        if (Field != targetField)
        {
            Field = targetField;
            Direction = SortDirection.Descending;
            return;
        }

        switch (Direction)
        {
            case SortDirection.Descending:
                Direction = SortDirection.Ascending;
                break;

            case SortDirection.Ascending:
                Field = SortField.None;
                Direction = SortDirection.None;
                break;

            default:
                Direction = SortDirection.Descending;
                break;
        }
    }
}

public enum FilterType
{
    None, Important, Normal, Monster
}


public class FilterAndSortControl : MonoBehaviour
{
    [Header("⭐ Sorting State")]
    private SortStatus sortStatus = new SortStatus();

    [Header("⭐ Star Filter")]
    [SerializeField] private Button StarButton;
    private bool isStar = false;

    [Header("🎭 Sub Filters")]
    [SerializeField] public CharacterClassFilter ClassFilter;
    [SerializeField] public FavorabilityFilter FavorabilityFilter;

    [Header("📊 Value Sort Buttons")]
    [SerializeField] public List<ValueSortControl> ValueSorts = new List<ValueSortControl>();


    [Header("Filter Buttons")]
    [SerializeField] public List<FilterButtons> filterButtons = new List<FilterButtons>();
    private List<FilterType> currentFilterTypes = new List<FilterType>();

    #region Structs
    [System.Serializable]
    public struct FilterButtons
    {
        public Button FilterButton;
        public FilterType FilterType;
    }
    #endregion


    private Action OnFilterClick;
    private Action OnSortClick;


    
    public void Init(Action OnFilterClick, Action OnSortClick)
    {
        InitFilters(OnFilterClick);
        InitSortButtons(OnSortClick);
        UpdateAllSortIcons();
    }


    public void ResetFilterAndSort()
    {
        ResetFilters();
    }

    #region 🔹 Filter 初始化
    private void InitFilters(Action filterCallback)
    {
        OnFilterClick = filterCallback;
        InitStarFilter();
        InitClassFilter();
        InitFavorabilityFilter();
        InitFilterButtons();
        ResetFilterAndSort();
    }

    private void InitStarFilter()
    {
        if (StarButton == null) return;

        StarButton.onClick.RemoveAllListeners();
        StarButton.onClick.AddListener(() =>
        {
            isStar = !isStar;
            StarButton.image.sprite = UpStarButtonSprite(isStar);
            OnFilterClick?.Invoke();
        });
        StarButton.image.sprite = UpStarButtonSprite(isStar);
    }

    private void InitClassFilter()
    {
        if (ClassFilter == null) return;
        ClassFilter.OnFilterClickListener(true, () => OnFilterClick?.Invoke());
    }

    private void InitFavorabilityFilter()
    {
        if (FavorabilityFilter == null) return;
        FavorabilityFilter.OnFilterClickListener(true, () => OnFilterClick?.Invoke());
    }

    private void InitFilterButtons()
    {
        foreach (var fb in filterButtons)
        {
            if (fb.FilterButton == null) continue;
            fb.FilterButton.onClick.RemoveAllListeners();
            fb.FilterButton.onClick.AddListener(() => OnFilterButtonClick(fb.FilterType));
            UpdateFilterButtonVisual(fb.FilterButton, fb.FilterType);
        }
    }

    private void OnFilterButtonClick(FilterType clickedType)
    {
        // 🧭 所有有效类型（排除 None）
        var allTypes = filterButtons
            .Select(fb => fb.FilterType)
            .Where(t => t != FilterType.None)
            .Distinct()
            .ToList();

        // 🟦 1️⃣ 当前是 None 且点击 None → 清空所有
        if (currentFilterTypes.Count == 1 && currentFilterTypes.Contains(FilterType.None) && clickedType == FilterType.None)
        {
            currentFilterTypes.Clear(); // 完全空
        }
        // 🟦 2️⃣ 点击 None → 切换为仅 None
        else if (clickedType == FilterType.None)
        {
            currentFilterTypes.Clear();
            currentFilterTypes.Add(FilterType.None);
        }
        // 🟦 3️⃣ 处理普通类型
        else
        {
            // 如果当前是 None，先清空
            if (currentFilterTypes.Contains(FilterType.None))
                currentFilterTypes.Clear();

            if (currentFilterTypes.Contains(clickedType))
            {
                // 移除该类型
                currentFilterTypes.Remove(clickedType);

                // 如果现在空了，完全清空（不是回到 None）
                if (currentFilterTypes.Count == 0)
                    currentFilterTypes.Clear();
            }
            else
            {
                // 添加该类型
                currentFilterTypes.Add(clickedType);

                // 如果已经全选所有类型，则重置为 None
                if (currentFilterTypes.Count == allTypes.Count)
                {
                    currentFilterTypes.Clear();
                    currentFilterTypes.Add(FilterType.None);
                }
            }
        }

        UpdateAllFilterButtonVisuals();
        OnFilterClick?.Invoke();

    }

    void ResetFilters()
    {
        currentFilterTypes.Clear();
        currentFilterTypes.Add(FilterType.None);
        UpdateAllFilterButtonVisuals();
        currentFilterTypes.Clear();
    }


    private void UpdateAllFilterButtonVisuals()
    {
        foreach (var fb in filterButtons)
        {
            UpdateFilterButtonVisual(fb.FilterButton, fb.FilterType);
        }
    }

    private void UpdateFilterButtonVisual(Button btn, FilterType type)
    {
        if (btn == null) return;
        if (currentFilterTypes.Count == 1 && currentFilterTypes.Contains(FilterType.None))
        {
            SetButtonAlpha(btn, 1f);
            return;
        }

        bool isActive = currentFilterTypes.Contains(type);
        SetButtonAlpha(btn, isActive ? 1f : 0.5f);
    }

    private void SetButtonAlpha(Button btn, float alpha)
    {
        var color = btn.image.color;
        color.a = alpha;
        btn.image.color = color;
    }



    #endregion

    #region 🔹 Sort 初始化
    private void InitSortButtons(Action sortCallback)
    {
        OnSortClick = sortCallback;

        if (FavorabilityFilter != null) FavorabilityFilter.SetSortButton(sortStatus, OnSortChange);

        foreach (var vs in ValueSorts)
        {
            if (vs.SortButton == null) continue;

            SortField field = MapValueTypeToSortField(vs.ValueType);
            if (field == SortField.None) continue;

            SetSortButton(vs.SortButton, field, () =>
            {
                sortStatus.ToggleSort(field);
                OnSortChange();
            });
        }
    }


    void OnSortChange()
    {
        UpdateAllSortIcons();
        OnSortClick?.Invoke();

    }


    private void SetSortButton(Button sortButton, SortField sortField, Action onClick)
    {
        sortButton.onClick.RemoveAllListeners();
        sortButton.onClick.AddListener(() => onClick?.Invoke());
    }
    #endregion

    #region 🔹 Sort Icon 刷新
    private void UpdateAllSortIcons()
    {

        if (FavorabilityFilter != null)
        {
            Debug.Log($"sortStatus.Field IS {sortStatus.Field}");
            if (sortStatus.Field == SortField.Favorability)
            {
                FavorabilityFilter.SortFavorabilityButton.image.sprite = GetSortSprite(sortStatus.Direction == SortDirection.Ascending ? "Ascending" : "Descending");
            } else
            {
                FavorabilityFilter.SortFavorabilityButton.image.sprite = GetSortSprite("None");
            }

        }

        foreach (var vs in ValueSorts)
        {
            if (vs.SortButton == null) continue;
            SortField field = MapValueTypeToSortField(vs.ValueType);


            if (sortStatus.Field == field)
            {
                vs.SortButton.image.sprite = GetSortSprite(sortStatus.Direction == SortDirection.Ascending ? "Ascending" : "Descending");
            }
            else
            {
                vs.SortButton.image.sprite = GetSortSprite("None");
            }
        }
    }
    #endregion

    #region 🔹 公共接口
    public void OnFilterClickListener(bool isAdd, Action callback)
    {
        if (isAdd) OnFilterClick += callback;
        else OnFilterClick -= callback;

        ClassFilter?.OnFilterClickListener(isAdd, callback);
        FavorabilityFilter?.OnFilterClickListener(isAdd, callback);
    }

    public void OnSortClickListener(bool isAdd, Action callback)
    {
        if (isAdd) OnSortClick += callback;
        else OnSortClick -= callback;
    }

    public SortStatus GetCurrentSortStatus() => sortStatus;
    public bool IsStarFilterActive => isStar;

    public bool PassCurrentFilter(FilterType filterType)
    {
        if (currentFilterTypes.Count == 1 && currentFilterTypes.Contains(FilterType.None)) return true;
        return currentFilterTypes.Contains(filterType);

    }
    #endregion

    #region 🔹 辅助方法
    private SortField MapValueTypeToSortField(ValueType type)
    {
        return type switch
        {
            ValueType.Attack => SortField.Attack,
            ValueType.Defense => SortField.Defense,
            ValueType.Magic => SortField.Magic,
            ValueType.Speed => SortField.Speed,
            ValueType.Lucky => SortField.Lucky,
            ValueType.FoodP => SortField.FoodP,
            ValueType.ScienceP => SortField.ScienceP,
            ValueType.PoliticsP => SortField.PoliticsP,
            ValueType.GoldP => SortField.GoldP,
            ValueType.FaithP => SortField.FaithP,
            ValueType.Leadership => SortField.Leadership,
            ValueType.Scout => SortField.Scout,
            ValueType.Build => SortField.Build,
            ValueType.Negotiation => SortField.Negotiation,
            ValueType.Charm => SortField.Charm,
            ValueType.Favorability => SortField.Favorability,
            ValueType.Force => SortField.Force,
            ValueType.Level => SortField.Level,
            _ => SortField.None
        };
    }

    public FilterType TransFilterType(CharacterCategory category)
    {
        return category switch
        {
            CharacterCategory.Important => FilterType.Important,
            CharacterCategory.Normal => FilterType.Normal,
            CharacterCategory.Monster => FilterType.Monster,
            _ => FilterType.None
        };
    }




    #endregion
}
