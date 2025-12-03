using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;
using static GetSprite;

public class BuildingPanelControl : MonoBehaviour
{

    public Button starButton;
    bool isStar;

    public Image buildingIcon;
    public TextMeshProUGUI buildingName;
    public TextMeshProUGUI buildingDescribe;

    public Image buildingTypeIcon;


    public Image costIcon;
    public TextMeshProUGUI costText;

    private BuildingValue buildingValue;


    void Start()
    {
        UpStarButtonSprite();
        starButton.onClick.AddListener(OnStarButton);
    }

    public void SetBuildingValue(BuildingValue value)
    {
        buildingValue = value;
        UpUIData();
    }


    void UpUIData()
    {
        buildingIcon.sprite = GetBuildingSprite(buildingValue.GetBuildType());
        buildingName.text = buildingValue.GetBuildName();
        buildingDescribe.text = buildingValue.GetBuildName();
        buildingTypeIcon.sprite = GetValueSprite(buildingValue.GetBuildType());
        costIcon.sprite = GetValueSprite(buildingValue.GetBuildCostType());
        costText.text = buildingValue.GetBuildCost().ToString("N0");
        
    }
    


    void UpStarButtonSprite()
    {
        starButton.gameObject.GetComponent<Image>().sprite = GetSprite.UpStarButtonSprite(isStar);
    }

    void OnStarButton()
    {
        isStar = !isStar;
        UpStarButtonSprite();
    }

}

public class BuildingValue
{
    private string buildType;

    private string buildCostType;
    private float buildCost;


    private Dictionary<string, string> buildName = new();
    private Dictionary<string, string> buildDescride = new();

    public BuildingValue()
    {
        buildType = "Edge";
        buildCostType = "Build";
        buildCost = 10;
    }

    public string GetBuildType()
    {
        return buildType;
    }

    public string GetBuildCostType()
    {
        return buildCostType;
    }

    public float GetBuildCost()
    {
        return buildCost;
    }

    public string GetBuildName()
    {
        string currentLanguage = LocalizationSettings.SelectedLocale.Identifier.Code;
        return buildName.TryGetValue(currentLanguage, out var text) ? text : buildName["en"];
    }

    public string GetBuildDescride()
    {
        string currentLanguage = LocalizationSettings.SelectedLocale.Identifier.Code;
        return buildDescride.TryGetValue(currentLanguage, out var text) ? text : buildDescride["en"];
    }

}