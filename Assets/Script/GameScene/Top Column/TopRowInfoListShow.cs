using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;
using Unity.VisualScripting;

public class TopRowInfoListShow : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    public GameValue gameValue;
    public GameObject scrollViewPrefab;
    private GameObject scrollViewInstance;

    public GameObject regionRecordPrefab; 


    /*  public GameObject calculatePanel;
      private GameObject calculateInstance;*/


    private bool isMouseOver = false;

    public GameObject populationText;
    public GameObject foodText;
    public GameObject politicsText;
    public GameObject scienceText;
    public GameObject goldText;
    public GameObject faithText;
    public GameObject supportRateText;
    public GameObject mobilizedPopulationText;


    public Button negotiationTextButton;
    public Button buildTextButton;
    public Button scoutTextButton;


    private GameObject hoveredText;

    public CharacterAssistRowControl characterAssistRowControl;

    public RegionInfo regionInfo;


    public enum SortType { Food, Science, Politics, Gold, Faith, Population,mobilizedPopulation, supportRate}
    public SortType sortBy = SortType.Population;



    private List<GameObject> triggerableTexts;


    void Start()
    {
        negotiationTextButton?.onClick.AddListener(() => ToggleCharacterAssistRow(SBNType.Negotiation));
        buildTextButton?.onClick.AddListener(() => ToggleCharacterAssistRow(SBNType.Build));
        scoutTextButton?.onClick.AddListener(() => ToggleCharacterAssistRow(SBNType.Scout));



        triggerableTexts = new List<GameObject>
        {
            populationText,
            foodText,
            politicsText,
            scienceText,
            goldText,
            faithText,
            supportRateText,
            mobilizedPopulationText
        };

        AddEventTrigger(populationText);
        AddEventTrigger(foodText);
        AddEventTrigger(politicsText);
        AddEventTrigger(scienceText);
        AddEventTrigger(goldText);
        AddEventTrigger(faithText);
        AddEventTrigger(supportRateText);
        AddEventTrigger(mobilizedPopulationText);

    }

    private void Update()
    {
        if (gameValue == null) gameValue = GameObject.FindObjectOfType<GameValue>();

    }


    void ToggleCharacterAssistRow(SBNType type)
    {
        characterAssistRowControl.ToggleRow(type);
    }

    void AddEventTrigger(GameObject text)
    {
        // Make sure TMP_Text has an EventTrigger to listen to events
        EventTrigger trigger = text.gameObject.AddComponent<EventTrigger>();

        // Create new entry for pointer enter
        EventTrigger.Entry pointerEnterEntry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerEnter
        };
        pointerEnterEntry.callback.AddListener((eventData) => OnPointerEnter((PointerEventData)eventData));

        // Add the event listener
        trigger.triggers.Add(pointerEnterEntry);

        // Create new entry for pointer exit
        EventTrigger.Entry pointerExitEntry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerExit
        };
        pointerExitEntry.callback.AddListener((eventData) => OnPointerExit((PointerEventData)eventData));

        // Add the event listener
        trigger.triggers.Add(pointerExitEntry);
    }





    public void OnPointerEnter(PointerEventData eventData)
    {
        GameObject hoveredObject = eventData.pointerCurrentRaycast.gameObject;
        if (hoveredObject == null || !triggerableTexts.Contains(hoveredObject) )return;
        hoveredText = hoveredObject;
        SetSortTypeBasedOnHoveredText();
        if (eventData.pointerCurrentRaycast.gameObject != scrollViewInstance || eventData.pointerCurrentRaycast.gameObject != hoveredObject) { 
            HideRegionTooltip();
        }
        isMouseOver = true;
        ShowRegionTooltip();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isMouseOver = false;
        if (scrollViewInstance == null || eventData.pointerCurrentRaycast.gameObject != scrollViewInstance)
        {
            HideRegionTooltip();
        }
    }

    // Set the sorting type based on the hovered text
    private void SetSortTypeBasedOnHoveredText()
    {
        if (hoveredText == populationText)
            sortBy = SortType.Population;
        else if (hoveredText == foodText)
            sortBy = SortType.Food;
        else if (hoveredText == politicsText)
            sortBy = SortType.Politics;
        else if (hoveredText == scienceText)
            sortBy = SortType.Science;
        else if (hoveredText == goldText)
            sortBy = SortType.Gold;
        else if (hoveredText == faithText)
            sortBy = SortType.Faith;
        else if (hoveredText == supportRateText)
            sortBy = SortType.supportRate;
        else if (hoveredText == mobilizedPopulationText)
            sortBy = SortType.mobilizedPopulation;
    }



    void ShowRegionTooltip()
    {
        if (!isMouseOver) return;
        RectTransform hoveredRect = hoveredText.GetComponent<RectTransform>();
        scrollViewInstance = Instantiate(scrollViewPrefab);
        scrollViewInstance.transform.SetParent(hoveredRect, false); // Ensure it's a child of the same parent
        RectTransform scrollViewRect = scrollViewInstance.GetComponent<RectTransform>();
      //  ObservableList<RegionValue> playerRegion = gameValue.PlayerRegions;
        List<RegionValue> sortedRegions = SortRegionsByType(gameValue.GetPlayerRegions().ToArray());  
        PopulateScrollView(sortedRegions);
        scrollViewRect.anchoredPosition = new Vector2(0, -hoveredRect.rect.height );

    }


    List<RegionValue> SortRegionsByType(RegionValue[] regions)
    {
        List<RegionValue> sortedRegions = new List<RegionValue>(regions);

        switch (sortBy)
        {
            case SortType.Population:
                sortedRegions.Sort((r1, r2) => r2.GetRegionPopulation().CompareTo(r1.GetRegionPopulation()));
                break;
            case SortType.Food:
                sortedRegions.Sort((r1, r2) => r2.GetRegionResourceLastTax(0).CompareTo(r1.GetRegionResourceLastTax(0)));
                break;
            case SortType.Science:
                sortedRegions.Sort((r1, r2) => r2.GetRegionResourceLastTax(1).CompareTo(r1.GetRegionResourceLastTax(1)));
                break;
            case SortType.Politics:
                sortedRegions.Sort((r1, r2) => r2.GetRegionResourceLastTax(2).CompareTo(r1.GetRegionResourceLastTax(2)));
                break;
            case SortType.Gold:
                sortedRegions.Sort((r1, r2) => r2.GetRegionResourceLastTax(3).CompareTo(r1.GetRegionResourceLastTax(3)));
                break;
            case SortType.Faith:
                sortedRegions.Sort((r1, r2) => r2.GetRegionResourceLastTax(4).CompareTo(r1.GetRegionResourceLastTax(4)));
                break;
            case SortType.supportRate:
                sortedRegions.Sort((r1, r2) => r1.GetRegionSupportRate().CompareTo(r2.GetRegionSupportRate()));
                break;
            case SortType.mobilizedPopulation:
                sortedRegions.Sort((r1, r2) =>
                {
                    if (r1.GetRegionRecruitedPopulation() > r1.GetRegionAvailablePopulation()) return -1;
                    if (r2.GetRegionRecruitedPopulation() > r2.GetRegionAvailablePopulation()) return 1;

                    return (r2.GetRegionAvailablePopulation() - r2.GetRegionRecruitedPopulation()).CompareTo(r1.GetRegionAvailablePopulation() - r1.GetRegionRecruitedPopulation());
                });
                break;


        }

        return sortedRegions;
    }



    void PopulateScrollView(List<RegionValue> sortedRegions)
    {
        Transform contentTransform = scrollViewInstance.transform.Find("Background/Viewport/Content");

        // 清除旧内容
        foreach (Transform child in contentTransform)
        {
            Destroy(child.gameObject);
        }

        // 确保有 LayoutGroup
        VerticalLayoutGroup layoutGroup = contentTransform.GetComponent<VerticalLayoutGroup>();
        if (layoutGroup == null)
        {
            layoutGroup = contentTransform.gameObject.AddComponent<VerticalLayoutGroup>();
        }
       // layoutGroup.spacing = 10f;
        layoutGroup.childAlignment = TextAnchor.UpperCenter;
        layoutGroup.childForceExpandHeight = false;

        float fixedElementHeight = 60f;

        // 判断 sortBy 类型（RegionRecord 中 SetRegionValue 的参数）
        int sortIndex = (int)sortBy;

        // 创建每项
        foreach (var region in sortedRegions)
        {
            GameObject newItem = Instantiate(regionRecordPrefab, contentTransform);
            RegionRecord record = newItem.GetComponent<RegionRecord>();

            if (record != null)
            {
                record.SetRegionValue(sortIndex, region, regionInfo);
            }
        }

        // 计算内容高度
        float totalHeight = sortedRegions.Count * (fixedElementHeight + layoutGroup.spacing) + 10;
        float halfScreenHeight = Screen.height / 2f;

        // 设置 ScrollView 高度
        RectTransform scrollViewRectTransform = scrollViewInstance.GetComponent<RectTransform>();
        scrollViewRectTransform.sizeDelta = new Vector2(scrollViewRectTransform.sizeDelta.x,
            totalHeight > halfScreenHeight ? halfScreenHeight : totalHeight);

        // 设置 Content 高度
        RectTransform contentRectTransform = contentTransform.GetComponent<RectTransform>();
        contentRectTransform.sizeDelta = new Vector2(contentRectTransform.sizeDelta.x, totalHeight);
    }
    void HideRegionTooltip()
    {
        if (scrollViewInstance != null)
        {
            Destroy(scrollViewInstance);
        }
    }

}
