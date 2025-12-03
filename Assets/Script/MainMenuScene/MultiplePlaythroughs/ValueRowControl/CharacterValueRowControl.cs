using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GetColor;
using static GetSprite;
using static MultiplePlaythroughsGameCharacterRowControl;

public class CharacterValueRowControl : BaseValueRowControl
{
    private MultiplePlaythroughsGameCharacterRowControlData characterData;

    public void InitCharacter(MultiplePlaythroughsGameCharacterRowControlData data, ValueType type, int step = 1)
    {
        characterData = data;
        base.Init(type, step);
        UpdateRowDisplay();
        InitButtons();
    }

    public override void UpdateRowDisplay()
    {
        if (characterData == null) return;

        int value = characterData.GetValue(valueType);
        if (valueIcon != null) valueIcon.sprite = GetMultipleValueSprite();
        if (valueText != null) valueText.text = GetMultipleValueString(value);

        if (resourceSlider != null)
        {
            switch (valueType)
            {
                case ValueType.Force:
                    resourceSlider.maxValue = characterData.MaxForce;
                    resourceSlider.value = characterData.Force;
                    break;

                case ValueType.MaxForce:
                    resourceSlider.maxValue = characterData.GetMaxLimit();
                    resourceSlider.value = characterData.MaxForce;
                    break;

                case ValueType.Experience:
                    resourceSlider.value = characterData.GetExpRate();

                 break;


                default:
                    resourceSlider.value = value;
                    break;
            }
        }
    }

    Sprite GetMultipleValueSprite()
    {
        switch (valueType)
        {
            case ValueType.Move:
                return GetCharacterStateSprite("move");
            case ValueType.Favorability:
                return GetFavorabilitySprite(characterData.FavorabilityLevel);
            default:
                return GetValueSprite(valueType);
        }

    }

    private string GetMultipleValueString(int value)
    {
        switch (valueType)
        {
            case ValueType.Move:
                return GetString.GetMoveNumString(value);
            case ValueType.Favorability:
                return  GetString.GetFavorabilityString(characterData.Favorability);
            case ValueType.Level:
                return characterData.GetLvAndMaxLevelString();
            case ValueType.Experience:
                return characterData.GetExpAndReqExpString();

            default:
                return GetValueColorString(value.ToString(), valueType);
        }
    }

    public override void ResetAddition()
    {
        characterData.RestData();
        UpdateRowDisplay();
    }


    public void SetValue(Sprite spriteValue = null,string stringValue = null,float sliderValue = -1)
    {
        if (spriteValue != null) base.valueIcon.sprite = spriteValue;
        if (stringValue != null) base.valueText.text = stringValue;
        if (sliderValue != -1) base.resourceSlider.value = sliderValue;

    }


    void InitButtons()
    {
        if (base.plusButton != null)
        {
            base.plusButton.Setup(OnCharacterValueButtonClick, new SpecialButtonData { step = 1 });
            if (base.valueType != ValueType.Experience)
            {
                base.plusButton.RegisterBothCtrlModifier((delta) => delta * 10);
            }
            else
            {

            }
        }

        if (base.minusButton != null)
        {
            base.minusButton.Setup(OnCharacterValueButtonClick, new SpecialButtonData { step = -1 });
            if (base.valueType != ValueType.Experience)
            {
                base.minusButton.RegisterBothCtrlModifier((delta) => delta * 10);
            } else
            {

            }
        }
    }

    void OnCharacterValueButtonClick(SpecialButtonData data, int delta)
    {
        switch (base.valueType)
        {
            case ValueType.Move: characterData.BattleMoveNum += delta; break;
            case ValueType.Favorability: characterData.Favorability += delta; break;
            case ValueType.Level: characterData.BaseMaxLevel += delta; break;
            case ValueType.Experience: ExperienceButtonClick(data, delta); break;
            default:OnValueButtonClick(data, delta);break;
        }

        UpdateRowDisplay();
    }

    void ExperienceButtonClick(SpecialButtonData data, int delta)
    {
        if (characterData.CurrentLevel < characterData.BaseMaxLevel)
        {
            characterData.AddExperience(delta);
        }
    }

    void OnValueButtonClick(SpecialButtonData data, int delta)
    {
        characterData.AddValue(base.valueType, delta);
    }


}
