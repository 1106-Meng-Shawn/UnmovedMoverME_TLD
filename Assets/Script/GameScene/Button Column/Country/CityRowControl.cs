using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static GetSprite;
using static GetColor;
using static FormatNumber;
using TMPro;
using UnityEngine.Localization.Settings;

public class CityRowControl : MonoBehaviour
{
    public Image CityCountryIcon;
    public Image CityIcon;
    public TextMeshProUGUI CityNameText;
    public Image ValueTypeIcon;
    public List<TextMeshProUGUI> ValueTexts;
    private  int currentValueType = 5; // 5 is population, 0 is food, 1 is science , 2 is politics, 3 is gold , 4 is faith
    private CityValue cityValue;

    private void Start()
    {
        // ???????????
        LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
    }

    private void OnDestroy()
    {
        LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;
        if (cityValue != null)
        {
            cityValue.OnCityValueChanged -= UpdataCityValue;
        }

    }

    private void OnLocaleChanged(UnityEngine.Localization.Locale locale)
    {
        SetCityName(); // ???????????????
    }

    public void SetCityValue(CityValue newCityValue)
    {
        if (newCityValue == null)
        {
            Destroy(this);
            return;
        }
        if (cityValue != null)
        {
            cityValue.OnCityValueChanged -= UpdataCityValue;
        }
        cityValue = newCityValue;
        cityValue.OnCityValueChanged += UpdataCityValue;
        UpdataCityValue();
    }

    public void SetCurrentValueType(int currentValueType) { 
    

        this.currentValueType = currentValueType;
        ValueTypeIcon.sprite = GetRegionValueSprite(currentValueType);


    }


    void UpdataCityValue()
    {
        CityCountryIcon.GetComponent<IntroPanelShow>().SetIntroName(cityValue.GetCityCountryNameWithColor());
        CityCountryIcon.sprite = GameValue.Instance.GetCountryIcon(cityValue.GetCityCountry());
        CityIcon.sprite = GetCitySprite(cityValue.cityIndex);
        SetCityName();
        ValueTypeIcon.sprite = GetRegionValueSprite(currentValueType);
        UpValueText();
    }

    void SetCityName()
    {
        CityNameText.text = GetCountryColorString(cityValue.GetCityName(), cityValue.GetCityCountry());
    }

    void UpValueText()
    {

        if (currentValueType == 5) { UpPopulationValueText(); return; }
        ValueTexts[0].text = GetRegionValueColorString(FormatNumberToString(cityValue.GetResourceMax(currentValueType)), currentValueType);
        ValueTexts[1].text = GetRegionValueColorString(FormatNumberToString(cityValue.GetResourceSurplus(currentValueType)), currentValueType);
        ValueTexts[2].text = GetRegionValueColorString(cityValue.GetResourceGrowthString(currentValueType), currentValueType);// regionValue.population.ToString("N0");
        ValueTexts[3].text = GetRegionValueColorString(FormatNumberToString(cityValue.GetParameterWithLord(currentValueType)), currentValueType);
        ValueTexts[4].text = GetRegionValueColorString(FormatNumberToString(cityValue.GetResourceLastTax(currentValueType)), currentValueType);
        ValueTexts[5].text = GetRegionValueColorString(FormatNumberToString(cityValue.GetResourceNext(currentValueType)), currentValueType);
        ValueTexts[6].text = GetRegionValueColorString(FormatNumberToString(cityValue.GetResoureNextTax(currentValueType)), currentValueType);
    }

    void UpPopulationValueText()
    {
        ValueTexts[0].text = GetRegionValueColorString(FormatNumberToString(cityValue.GetMaxPopulation()), currentValueType);
        ValueTexts[1].text = GetRegionValueColorString(FormatNumberToString(cityValue.GetPopulation()), currentValueType);
        ValueTexts[2].text = GetRegionValueColorString(cityValue.GetPopulationGrowthString(), currentValueType);// regionValue.population.ToString("N0");
        ValueTexts[3].text = GetRegionValueColorString(FormatNumberToString(cityValue.GetNextTurnPopulation()), currentValueType);
        ValueTexts[4].text = GetRegionValueColorString(cityValue.GetSupportRateString(), currentValueType);
        ValueTexts[5].text = GetRegionValueColorString(FormatNumberToString(cityValue.GetAvailablePopulation()), currentValueType);
        ValueTexts[6].text = GetRegionValueColorString(FormatNumberToString(cityValue.GetRecruitedPopulation()), currentValueType);

    }

}
