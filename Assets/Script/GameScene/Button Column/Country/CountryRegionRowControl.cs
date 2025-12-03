using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static GetSprite;
using static GetColor;
using static FormatNumber;
using TMPro;
using UnityEngine.Localization.Settings;
using static RegionColumControl;
using System.Globalization;
using UnityEngine.EventSystems;


public class CountryRegionRowControl : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public Button starButton;
    public Image RegionCountryIcon;
    public Image RegionLordIcon;
    public TextMeshProUGUI RegionNameText;
    public Image ValueTypeIcon;
    public List<TextMeshProUGUI> ValueTexts;
    private int currentValueType = 5; // 5 is population, 0 is food, 1 is science , 2 is politics, 3 is gold , 4 is faith
    private RegionValue regionValue;
    public GameObject cityRowPrefab;
    private List<CityRowControl> cityRowControlList = new List<CityRowControl>();
    private GameObject draggedIcon;
    private bool hasDragged = false;

    private Canvas uiCanvas;

    private void OnEnable()
    {
        if (regionValue != null) {
            UpdataRegionValue();
        }

    }

    private void OnDisable()
    {
         regionValue.OnValueChanged -= UpdataRegionValue;
    }



    private void Start()
    {
        // ???????????
        InitStarButton();
        LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
        uiCanvas = GeneralManager.Instance.uiCanvas;
    }

    private void InitStarButton()
    {
        UpStarSprite();
        starButton.onClick.AddListener(OnStarButtonClick);
    }

    void OnStarButtonClick()
    {
        regionValue.IsStar = !regionValue.IsStar;
        UpStarSprite();

    }

    void UpStarSprite()
    {
        starButton.gameObject.GetComponent<Image>().sprite = UpStarButtonSprite(regionValue.IsStar);
    }



    private void OnDestroy()
    {
        LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;

    }

    private void OnLocaleChanged(UnityEngine.Localization.Locale locale)
    {
        SetRegionName(); // ???????????????
    }

    public void SetRegionValue(RegionValue newRegionValue)
    {
        if (newRegionValue == null)
        {
            Destroy(this);
            return;
        }
        if (newRegionValue != null)
        {
           // regionValue.OnValueChanged -= UpdataRegionValue;
        }
        regionValue = newRegionValue;
        regionValue.OnValueChanged += UpdataRegionValue;
        UpdataRegionValue();
        foreach (var cityValue in newRegionValue.GetCityValues())
        {
            GameObject cityRowGO = Instantiate(cityRowPrefab, this.transform); // ???????? GameObject
            CityRowControl cityRowControl = cityRowGO.GetComponent<CityRowControl>();
            if (cityRowControl != null)
            {
                cityRowControl.SetCityValue(cityValue);
                cityRowControlList.Add(cityRowControl);
            }
        }
    }

    public void SetCurrentValueType(int currentValueType)
    {
        this.currentValueType = currentValueType;
        ValueTypeIcon.sprite = GetRegionValueSprite(currentValueType);

        foreach (var cityRowControl in cityRowControlList)
        {
            cityRowControl.SetCurrentValueType(currentValueType);
        }
    }


        void UpdataRegionValue()
        {
            UpStarSprite();
            RegionCountryIcon.sprite = GameValue.Instance.GetCountryIcon(regionValue.GetCountryName());
            RegionCountryIcon.GetComponent<IntroPanelShow>().SetIntroName(regionValue.GetCountryNameWithColor());
            SetLordIcon();
            SetRegionName();
            ValueTypeIcon.sprite = GetRegionValueSprite(currentValueType);
            UpValueText();
        }

    public void SwitchLord(RegionValue region)
    {
        regionValue.SwitchLord(region);
    }

    void SetLordIcon()
    {
        if (regionValue.GetLord() == null)
        {
            RegionLordIcon.raycastTarget = false;
            RegionLordIcon.gameObject.GetComponent<IntroPanelShow>().SetCharacter(null);

        }
        else
        {
            RegionLordIcon.raycastTarget = true;
            RegionLordIcon.gameObject.GetComponent<IntroPanelShow>().SetCharacter(regionValue.GetLord());
            RegionLordIcon.gameObject.GetComponent<IntroPanelShow>().SetIntroName(regionValue.GetLord().GetCharacterName());
        }
        RegionLordIcon.sprite = regionValue.GetLordIcon();
    }

    void SetRegionName()
    {
        RegionNameText.text = $"<b>{regionValue.GetRegionName().ToUpper()}</b>";
        RegionNameText.color = regionValue.region.GetRegionColor();
    }

    void UpValueText()
    {
        if (currentValueType == 5) { UpPopulationValueText(); return; }
        ValueTexts[0].text = GetRegionValueColorString(FormatNumberToString(regionValue.GetRegionResourceMax(currentValueType)), currentValueType);
        ValueTexts[1].text = GetRegionValueColorString(FormatNumberToString(regionValue.GetRegionResourceSurplus(currentValueType)), currentValueType);
        ValueTexts[2].text = GetRegionValueColorString(regionValue.GetRegionResourceGrowthString(currentValueType), currentValueType);// regionValue.population.ToString("N0");
        ValueTexts[3].text = GetRegionValueColorString(FormatNumberToString(regionValue.GetRegionResourceParameter(CurrentValueTypeToValueType())), currentValueType);
        ValueTexts[4].text = GetRegionValueColorString(FormatNumberToString(regionValue.GetRegionResourceLastTax(currentValueType)), currentValueType);
        ValueTexts[5].text = GetRegionValueColorString(FormatNumberToString(regionValue.GetRegionResourceNext(currentValueType)), currentValueType);
        ValueTexts[6].text = GetRegionValueColorString(FormatNumberToString(regionValue.GetRegionResourceNextTax(currentValueType)), currentValueType);
    }

    void UpPopulationValueText()
    {
        ValueTexts[0].text = GetRegionValueColorString(FormatNumberToString(regionValue.GetRegionMaxPopulation()), currentValueType);
        ValueTexts[1].text = GetRegionValueColorString(FormatNumberToString(regionValue.GetRegionPopulation()), currentValueType);
        ValueTexts[2].text = GetRegionValueColorString(regionValue.GetPopulationGrowthString(), currentValueType);// regionValue.population.ToString("N0");
        ValueTexts[3].text = GetRegionValueColorString(FormatNumberToString(regionValue.GetNextTurnPopulation()), currentValueType);
        ValueTexts[4].text = GetRegionValueColorString(regionValue.GetRegionSupportRateString(), currentValueType);
        ValueTexts[5].text = GetRegionValueColorString(FormatNumberToString(regionValue.GetRegionAvailablePopulation()), currentValueType);
        ValueTexts[6].text = GetRegionValueColorString(FormatNumberToString(regionValue.GetRegionRecruitedPopulation()), currentValueType);
    }

    public int GetRegionID()
    {
        return regionValue.GetRegionID();
    }



    public string GetRegionName()
    {
        return regionValue.GetRegionName();
    }

    public string GetLordName()
    {
        if (regionValue.GetLord() != null)
        {
            return regionValue.GetLord().GetCharacterName();
        } else
        {
            return null;
        }
    }

    public float GetRowValue(int index)
    {
        if (currentValueType == 5){
            return GetPopValue(index);
        }
        switch (index)
        {
            case 0: return regionValue.GetRegionResourceMax(currentValueType);
            case 1: return regionValue.GetRegionResourceSurplus(currentValueType);
            case 2: return regionValue.GetRegionResourceGrowth(currentValueType);
            case 3: return regionValue.GetRegionResourceParameter(CurrentValueTypeToValueType());
            case 4: return regionValue.GetRegionResourceLastTax(currentValueType);
            case 5: return regionValue.GetRegionResourceNext(currentValueType);
            case 6: return regionValue.GetRegionResourceNextTax(currentValueType);
        }
        Debug.Log("CountryRegionRowControl, GetRowValue have bug");
        return 0;

    }

    float GetPopValue(int index)
    {
        switch (index)
        {
            case 0: return regionValue.GetRegionMaxPopulation();
            case 1: return regionValue.GetRegionPopulation();
            case 2: return regionValue.GetPopulationGrowth();
            case 3: return regionValue.GetNextTurnPopulation();
            case 4: return regionValue.GetRegionSupportRate();
            case 5: return regionValue.GetRegionAvailablePopulation();
            case 6: return regionValue.GetRegionRecruitedPopulation();
        }
        Debug.Log("CountryRegionRowControl, GetPopValue have bug");
        return 0;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
            hasDragged = true;
            CreateDraggedIcon();
    }


    void CreateDraggedIcon()
    {
        if (!regionValue.HasLord())
        {
            //NotificationManage.Instance.ShowAtTop("This Region Don't have lord");
            NotificationManage.Instance.ShowAtTopByKey(NotificationKeyConstants.Region_NoLord,regionValue.GetRegionName());
            hasDragged = false;
            return;
        }
        if (draggedIcon == null)
        {
            draggedIcon = new GameObject("DraggedIcon");
            draggedIcon.transform.SetParent(transform.root);
            Image draggedImage = draggedIcon.AddComponent<Image>();
            draggedImage.sprite = regionValue.GetLordIcon();
            draggedImage.raycastTarget = true;

            draggedImage.rectTransform.sizeDelta = new Vector2(0.5f, 0.5f);
            RectTransform rt = draggedImage.rectTransform;
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.position = Input.mousePosition + new Vector3(20, 20, 0);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {

        if (draggedIcon != null)
        {
            RectTransform draggedIconRect = draggedIcon.GetComponent<RectTransform>();
            RectTransform canvasRect = transform.root.GetComponent<RectTransform>();

            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect,
                Input.mousePosition,
                uiCanvas.worldCamera,
                out localPoint
            );

            draggedIconRect.localPosition = localPoint;
            draggedIconRect.localScale = new Vector3(250f, 250f, 1f);
            hasDragged = true;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        StartCoroutine(HandleEndDrag());
    }

    IEnumerator HandleEndDrag()
    {
        if (draggedIcon != null)
        {
            Destroy(draggedIcon);
            draggedIcon = null;
        }

        hasDragged = false;

        yield return null;

        PointerEventData pointerEventData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        List<RaycastResult> raycastResults = new List<RaycastResult>();

        GraphicRaycaster raycaster = GetComponentInParent<Canvas>().GetComponent<GraphicRaycaster>();
        raycaster.Raycast(pointerEventData, raycastResults);

        GameObject hitObject = null;

        if (raycastResults.Count > 0)
        {
            hitObject = raycastResults[0].gameObject;
        }

        if (hitObject == null)
        {
            regionValue.MoveLord();
        }
        else if (hitObject == RegionInfo.Instance.lordBackgroundImage.gameObject)
        {
            RegionInfo.Instance.SetLord(regionValue.GetLord());
            regionValue.MoveLord();

        }
        else if (hitObject.GetComponent<CountryRegionRowControl>() != null)
        {
            hitObject.GetComponent<CountryRegionRowControl>().SwitchLord(regionValue);
        } else
        {
            regionValue.MoveLord();

        }

        UpdataRegionValue();
        /*  if (hitObject != lordBackgroundImage.gameObject)
          {
              regionAtInfo.MoveLord();
          }*/
    }


    ValueType CurrentValueTypeToValueType()
    {
        switch (currentValueType)
        {
            case 0: return ValueType.Food;
            case 1: return ValueType.Science;
            case 2: return ValueType.Politics;
            case 3: return ValueType.Gold;
            case 4: return ValueType.Food;
            case 5: return ValueType.Population;
            default:
                Debug.LogError($"CurrentValueTypeToValueType({currentValueType})");
                return ValueType.Food;
               
        }
    }
}
