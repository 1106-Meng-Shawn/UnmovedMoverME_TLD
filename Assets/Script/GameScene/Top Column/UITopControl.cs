using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using static FormatNumber;
using System.Collections;

public class UITopControl : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public enum StatType {  Food, Science, Politics, Gold, Faith,Population, MobilizedPopulation, SupportRate, AchievementFull, AchievementFormat }

    [Header("Type Setting")]
    public string type; // ???????? food, population...?

    [Header("UI References")]
    public TextMeshProUGUI valueText;

    [Header("Prefab References")]
    // public GameObject scrollViewPrefab;
    public ScrollRect scrollRect;
    public GameObject regionRecordPrefab;
    private GameValue gameValue;
    private bool isMouseOver = false;

    public RegionInfo regionInfo;

    private StatType statType;

    private float lastValue = -1f;
    Color32 originalColor;

    void Start()
    {
        gameValue = GameValue.Instance;
        if (gameValue != null)
        {
            //   gameValue.OnGameValueChanged += UpdateValueText;
            gameValue.RegisterResourceChanged(UpdateValueText);
        }

        if (!string.IsNullOrEmpty(type))
        {
            type = type.ToLower();
            System.Enum.TryParse(type, true, out statType);
        }

        UpdateValueText();
        HideRegionTooltip();

        Image bg = valueText.transform.parent.GetComponent<Image>();
        originalColor = bg.color;

    }

    private void OnDestroy()
    {
        if (gameValue != null)
        {
            //  gameValue.OnGameValueChanged -= UpdateValueText;
            gameValue.UnRegisterResourceChanged(UpdateValueText);

        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isMouseOver = true;
        ShowRegionTooltip();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isMouseOver = false;
        HideRegionTooltip();
    }

    void UpdateValueText()
    {
        if (gameValue == null) return;

        float currentValue = 0f;
        string displayText = "-";

        switch (statType)
        {
            case StatType.Population:
                currentValue = gameValue.GetTotalPopulation();
                break;
            case StatType.Food:
                currentValue = gameValue.GetResourceValue().Food;
                break;
            case StatType.Science:
                currentValue = gameValue.GetResourceValue().Science;
                break;
            case StatType.Politics:
                currentValue = gameValue.GetResourceValue().Politics;
                break;
            case StatType.Gold:
                currentValue = gameValue.GetResourceValue().Gold;
                break;
            case StatType.Faith:
                currentValue = gameValue.GetResourceValue().Faith;
                break;
            case StatType.MobilizedPopulation:
                currentValue = gameValue.GetResourceValue().TotalRecruitedPopulation;
                break;
            case StatType.SupportRate:
                currentValue = gameValue.GetTotalSupportRate() * 100f;
                break;
            case StatType.AchievementFull:
                currentValue = gameValue.GetAchievement();
                break;
            case StatType.AchievementFormat:
                currentValue = gameValue.GetAchievement();

                break;
        }

        if (statType == StatType.AchievementFull)
        {
            displayText = currentValue.ToString("N0");
        }
        else
        {
            displayText = statType == StatType.SupportRate ? currentValue.ToString("N0") + "%" : FormatNumberToString(currentValue);
        }
        valueText.text = displayText;

        // 比较变化值
        if (lastValue >= 0f && Mathf.Abs(currentValue - lastValue) > 0.01f)
        {
            bool isIncrease = currentValue > lastValue;
            FlashColor(isIncrease);
        }

        lastValue = currentValue;
    }


    void FlashColor(bool isIncrease)
    {
        Image bg = valueText.transform.parent.GetComponent<Image>();
        if (bg == null) return;

        Color32 targetColor = isIncrease ? new Color32(0, 255, 0, 255) : new Color32(255, 0, 0, 255); // 绿色 or 红色

        if (gameObject.activeInHierarchy)
        {
            StopAllCoroutines();
            StartCoroutine(FlashColorCoroutine(bg, originalColor, targetColor));
        }
    }

    IEnumerator FlashColorCoroutine(Image img, Color original, Color flashColor)
    {
        float flashTime = 0.05f;
        img.color = flashColor;
        yield return new WaitForSeconds(flashTime);
        img.color = original;
    }


    void ShowRegionTooltip()
    {
        if (statType == StatType.AchievementFull || statType == StatType.AchievementFormat) return;
        if (BattlePanelManage.Instance.BattlePanel.activeSelf) return;
        List<RegionValue> regions = new List<RegionValue>(gameValue.GetPlayerRegions());
        List<RegionValue> sorted = SortRegionsByStat(regions);

        Transform content = scrollRect.content;
        scrollRect.gameObject.SetActive(true);

        foreach (var region in sorted)
        {
            GameObject item = Instantiate(regionRecordPrefab, content);
            RegionRecord record = item.GetComponent<RegionRecord>();
            if (record != null)
            {
                record.SetRegionValue((int)statType, region, regionInfo);
            }
        }

        RectTransform viewportTransform = scrollRect.viewport.GetComponent<RectTransform>();

        RefreshAllLayouts(scrollRect.transform);
        float contentHeight = content.GetComponent<RectTransform>().rect.height;
        float finalHeight = Mathf.Min(contentHeight, Screen.height / 2f);
        viewportTransform.sizeDelta = new Vector2(viewportTransform.sizeDelta.x, finalHeight);

    }

    public static void RefreshAllLayouts(Transform root)
    {
        // 遍历 root 以及所有子物体
        foreach (var layoutGroup in root.GetComponentsInChildren<LayoutGroup>(true))
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(layoutGroup.GetComponent<RectTransform>());
        }

        foreach (var fitter in root.GetComponentsInChildren<ContentSizeFitter>(true))
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(fitter.GetComponent<RectTransform>());
        }
    }


    void HideRegionTooltip()
    {
        Transform content = scrollRect.content;
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }

        scrollRect.gameObject.SetActive(false);
    }

    List<RegionValue> SortRegionsByStat(List<RegionValue> list)
    {
        switch (statType)
        {
            case StatType.Population:
                list.Sort((a, b) => b.GetRegionPopulation().CompareTo(a.GetRegionPopulation())); break;
            case StatType.Food:
                list.Sort((a, b) => b.GetRegionResourceLastTax(0).CompareTo(a.GetRegionResourceLastTax(0))); break;
            case StatType.Science:
                list.Sort((a, b) => b.GetRegionResourceLastTax(1).CompareTo(a.GetRegionResourceLastTax(1))); break;
            case StatType.Politics:
                list.Sort((a, b) => b.GetRegionResourceLastTax(2).CompareTo(a.GetRegionResourceLastTax(2))); break;
            case StatType.Gold:
                list.Sort((a, b) => b.GetRegionResourceLastTax(3).CompareTo(a.GetRegionResourceLastTax(3))); break;
            case StatType.Faith:
                list.Sort((a, b) => b.GetRegionResourceLastTax(4).CompareTo(a.GetRegionResourceLastTax(4))); break;
            case StatType.SupportRate:
                list.Sort((a, b) => a.GetRegionSupportRate().CompareTo(b.GetRegionSupportRate())); break;
            case StatType.MobilizedPopulation:
                list.Sort((a, b) =>
                {
                    int aVal = a.GetRegionAvailablePopulation() - a.GetRegionRecruitedPopulation();
                    int bVal = b.GetRegionAvailablePopulation() - b.GetRegionRecruitedPopulation();
                    return bVal.CompareTo(aVal);
                });
                break;
        }
        return list;
    }

}
