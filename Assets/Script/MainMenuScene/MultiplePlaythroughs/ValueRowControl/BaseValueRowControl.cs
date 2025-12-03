using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using UnityEngine.Localization.SmartFormat.Utilities;
using UnityEngine.UI;
using static CountryConstants;
using static FormatNumber;
using static GetColor;
using static GetSprite;
using static MultiplePlaythroughsGameCharacterRowControl;

#region Base Class
public abstract class BaseValueRowControl : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] protected Image valueIcon;
    [SerializeField] protected TextMeshProUGUI valueText;
    [SerializeField] protected SpecialButton plusButton;
    [SerializeField] protected SpecialButton minusButton;
    [SerializeField] protected Slider resourceSlider;
    [SerializeField] protected Image fillImage;
    [SerializeField] protected Image backgroundImage;

    protected ValueType valueType;
    protected int step = 1;

    protected virtual void Awake()
    {
        SetupUI();
    }

    public virtual void Init(ValueType type, int step = 1)
    {
        this.valueType = type;
        this.step = step;
        SetupUI();
    }

    protected virtual void SetupUI()
    {
        if (fillImage != null)
            fillImage.color = GetValueColor(valueType);

        if (backgroundImage != null)
            backgroundImage.color = GetValueColor(ValueType.Achievement);
    }

    protected virtual void InitSlider(Action<float> onChanged)
    {
        if (resourceSlider == null) return;

        resourceSlider.onValueChanged.RemoveAllListeners();
        resourceSlider.minValue = 0;
        resourceSlider.onValueChanged.AddListener(v => onChanged?.Invoke(v));
    }


    public abstract void UpdateRowDisplay();

    public abstract void ResetAddition();

}
#endregion

