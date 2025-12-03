using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


public static class CharacterExtrasScrollRowConstants
{
    public const string ShowAnimatorName = "ShowExtrasCharacterRow";
    public const string HideAnimatorName = "HideExtrasCharacterRow";

}


public class CharacterExtrasScrollRow : MonoBehaviour
{
    [Header("Bottom Row")]
    [SerializeField] private FilterAndSortControl filterAndSortControl;
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private Button CloseButton;
    [SerializeField] private CharacterExtrasObjectControl characterExtrasObjectControlPrefab;
    [SerializeField] private Animator animator;
    private bool isShow;


    private List<CharacterExtrasObjectControl> characterExtrasObjects = new List<CharacterExtrasObjectControl>();
    private List<CharacterExtrasSaveData> allCharacterData = new List<CharacterExtrasSaveData>();

    #region Initialization

    private void Awake()
    {
        InitPanel();
        InitButtons();
    }

    void InitButtons()
    {
        CloseButton.onClick.AddListener(ClosePanel);
    }

    void InitPanel()
    {
        isShow = false;
        animator.Play(CharacterExtrasScrollRowConstants.HideAnimatorName, 0, 1f);
        animator.Update(0);

    }

    private void Start()
    {
       filterAndSortControl.Init(OnFilterChanged, OnSortChanged);
    }

    #endregion


    private void ShowPanel()
    {
        if (isShow) return;
        isShow = true;
        animator.Play(CharacterExtrasScrollRowConstants.ShowAnimatorName);
    }

    private void ClosePanel()
    {
        if (!isShow) return;
        isShow = false;
        animator.Play(CharacterExtrasScrollRowConstants.HideAnimatorName);
        filterAndSortControl.ResetFilterAndSort();

    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1)) ClosePanel();
    }



    #region Data Management

    public void SetCharacterExtrasSaveDataList(List<CharacterExtrasSaveData> characterExtrasSaveDataList)
    {
        if (characterExtrasSaveDataList == null)
        {
            Debug.LogWarning("[CharacterExtrasScrollRow] CharacterExtrasSaveDataList is null.");
            return;
        }
        allCharacterData = new List<CharacterExtrasSaveData>(characterExtrasSaveDataList);
        RefreshDisplay();
    }
    #endregion

    #region Filter & Sort Callbacks
    private void OnFilterChanged()
    {
        if (!isShow) ShowPanel();
        RefreshDisplay();
    }

    private void OnSortChanged()
    {
        RefreshDisplay();
    }
    #endregion

    #region Display Refresh
    private void RefreshDisplay()
    {
        var filteredData = FilterCharacters(allCharacterData);
        var sortedData = SortCharacters(filteredData);
        UpdateDisplay(sortedData);
    }

    private void UpdateDisplay(List<CharacterExtrasSaveData> dataList)
    {
        ClearOldObjects();

        foreach (var saveData in dataList)
        {
            if (saveData == null) continue;

            var newObj = Instantiate(characterExtrasObjectControlPrefab, scrollRect.content);
            newObj.gameObject.SetActive(true);
            newObj.Init(saveData);
            characterExtrasObjects.Add(newObj);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(scrollRect.content);
    }
    #endregion

    #region Filtering Logic
    private List<CharacterExtrasSaveData> FilterCharacters(List<CharacterExtrasSaveData> dataList)
    {
        if (dataList == null || dataList.Count == 0)
            return new List<CharacterExtrasSaveData>();

        var filtered = dataList;

        if (filterAndSortControl.IsStarFilterActive)filtered = filtered.Where(IsStarCharacter).ToList();
        if (filterAndSortControl.ClassFilter != null)filtered = filtered.Where(PassClassFilter).ToList();
        if (filterAndSortControl.FavorabilityFilter != null)filtered = filtered.Where(PassFavorabilityFilter).ToList();
        filtered = filtered.Where(PassCurrentFilter).ToList();

        return filtered;
    }

    private bool IsStarCharacter(CharacterExtrasSaveData data)
    {
        return data?.IsStar ?? false;
    }

    private bool PassClassFilter(CharacterExtrasSaveData data)
    {
        return filterAndSortControl.ClassFilter.PassFilter(data);
    }

    private bool PassFavorabilityFilter(CharacterExtrasSaveData data)
    {
        return filterAndSortControl.FavorabilityFilter.PassFilter(data);
    }

    #endregion

    #region Sorting Logic
    private List<CharacterExtrasSaveData> SortCharacters(List<CharacterExtrasSaveData> dataList)
    {
        if (filterAndSortControl == null)
            return dataList;

        var sortStatus = filterAndSortControl.GetCurrentSortStatus();
        if (sortStatus.Field == SortField.None)
            return dataList;

        var sorted = new List<CharacterExtrasSaveData>(dataList);

        sorted = sortStatus.Field switch
        {
            SortField.Favorability => SortByValue(sorted, d => GetFavorability(d), sortStatus.Direction),
            SortField.Level => SortByValue(sorted, d => GetLevel(d), sortStatus.Direction),
            _ => sorted
        };

        return sorted;
    }

    private List<CharacterExtrasSaveData> SortByValue(
        List<CharacterExtrasSaveData> dataList,
        Func<CharacterExtrasSaveData, float> getValue,
        SortDirection direction)
    {
        if (direction == SortDirection.Ascending)
            return dataList.OrderBy(getValue).ToList();
        else
            return dataList.OrderByDescending(getValue).ToList();
    }
    #endregion

    #region Value Getters (根据你的数据结构调整)
    private float GetFavorability(CharacterExtrasSaveData data) => data?.Favorability ?? 0f;
    private float GetLevel(CharacterExtrasSaveData data) => data?.CurrentLevel ?? 0f;


    private bool PassCurrentFilter(CharacterExtrasSaveData data)
    {
        if (filterAndSortControl == null)
            return true;

        FilterType type = filterAndSortControl.TransFilterType(GetCategory(data));
        return filterAndSortControl.PassCurrentFilter(type);
    }


    private CharacterCategory GetCategory(CharacterExtrasSaveData data) => data.Category;

    #endregion

    #region Cleanup
    private void ClearOldObjects()
    {
        foreach (var obj in characterExtrasObjects)
        {
            if (obj != null)
                Destroy(obj.gameObject);
        }
        characterExtrasObjects.Clear();
    }
    #endregion

}