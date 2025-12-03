using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static FormatNumber;
using static GetSprite;
using static GetColor;
using System;


public class RegionRecord : MonoBehaviour
{

    public Button button;
    public TextMeshProUGUI regionName;
    public TextMeshProUGUI regionRecordValue;
    public Image valueTypeImage;
    private RegionValue regionValue;
    private RegionInfo regionInfo;
    private int type;


    [SerializeField] private float doubleClickThreshold = 0.3f;

    private float lastClickTime = -1f;

    private void Start()
    {
        if (button == null)
        {
            Debug.LogError("Button reference not set!");
            return;
        }

        button.onClick.AddListener(HandleClick);
    }

    private void HandleClick()
    {
        float timeSinceLastClick = Time.time - lastClickTime;

        if (timeSinceLastClick <= doubleClickThreshold)
        {
            HandleDoubleClick();
            lastClickTime = -1f;
        }
        else
        {
            HandleSingleClick();
            lastClickTime = Time.time;
        }
    }

    private void HandleSingleClick()
    {
        if (regionInfo != null && regionValue != null)
        {
            regionInfo.ShowPanel(regionValue);
        }
    }

    private void HandleDoubleClick()
    {
        if (regionInfo != null && regionValue != null)
        {
            regionInfo.ShowPanel(regionValue,0);
            regionValue.region?.ZoomToCity(0); 
        }
    }

    public void SetRegionValue(int type, RegionValue regionValue,RegionInfo regionInfo)
    {
        this.regionInfo = regionInfo;
        this.regionValue = regionValue;
        this.type = type; 
        switch (type) {
            case 5: SetPopulation(); break;
            case 6: SetMobilizedPopulation(); break;
            case 7: SetSupportRate(); break;
            default:
                SetResourceThisTake(); break;

        }
    }

    void SetPopulation()
    {
        regionName.text = regionValue.GetRegionName();
        regionRecordValue.text = GetRegionValueColorString(FormatNumberToString(regionValue.GetRegionPopulation()), type);
        valueTypeImage.sprite = GetCurrentPopulationIcon();
    }


    void SetResourceThisTake()
    {
        regionName.text = regionValue.GetRegionName();
        regionRecordValue.text = GetRegionValueColorString(FormatNumberToString(regionValue.GetRegionResourceLastTax(type)),type);
        valueTypeImage.sprite = GetLastResourceTaxIcon(type);

    }



    void SetMobilizedPopulation()
    {
        regionName.text = regionValue.GetRegionName();
        regionRecordValue.text = $"{FormatNumberToString(regionValue.GetRegionRecruitedPopulation())}/{FormatNumberToString(regionValue.GetRegionAvailablePopulation())}";
        valueTypeImage.sprite = GetRecruitmentPopulationSprite();

    }

    void SetSupportRate()
    {
        regionName.text = regionValue.GetRegionName();
        regionRecordValue.text = regionValue.GetRegionSupportRateString();
        valueTypeImage.sprite = GetSupportRateSprite();

    }



}
