using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static FormatNumber;
using static GetColor;

public class ResourceValueRowControl : BaseValueRowControl
{
    private MultiplePlaythroughsGameTotalCountryData countryData;

    public void InitResource(ValueType type, int step = 1)
    {
        base.Init(type, step);
        countryData = SettingValue.Instance.StoryMultiplePlaythroughsData
            .MultiplePlaythroughsGameTotalData
            .MultiplePlaythroughsGameTotalCountryData;

        countryData.OnAdditionChanged += OnAdditionChanged;
        InitSlider(OnSliderValueChanged);
        UpdateRowDisplay();
    }

    private void OnDestroy()
    {
        if (countryData != null)
            countryData.OnAdditionChanged -= OnAdditionChanged;
    }

    public override void UpdateRowDisplay()
    {
        float value = GetCurrentValue();
        string formatted = FormatNumberToString(value);
        valueText.text = GetValueColorString(formatted, valueType);

        if (resourceSlider != null)
        {
            resourceSlider.maxValue = CalculateMaxAchievableValue();
            resourceSlider.value = value;
        }
    }

    private void OnAdditionChanged(ValueType changedType, int newValue)
    {
        if (changedType == valueType)
            UpdateRowDisplay();
    }

    private void OnSliderValueChanged(float newValue)
    {
        float current = GetCurrentValue();
        int delta = Mathf.RoundToInt(newValue - current);
        if (delta != 0)
            countryData.AddAddition(valueType, delta);
    }

    private float GetCurrentValue()
    {
        return GameValue.Instance.GetResource(valueType) + countryData.GetAddition(valueType);
    }

    private float CalculateMaxAchievableValue()
    {
        return GameValue.Instance.GetResource(valueType) +
               SettingValue.Instance.GetRemainingAchievement();
    }

    public override void ResetAddition()
    {
        countryData.ResetAddition(valueType);
        UpdateRowDisplay();
    }

}