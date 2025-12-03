using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static FormatNumber;
using static GetColor;
using System;

public class SBNUITopControl : MonoBehaviour
{
    public Button ImageButton;
    public TextMeshProUGUI valueText;
    public Button TextButton;

    public SBNType type; // Negotiation is 3, bulid is 2, scout is 1;

    private double previousValue = 0;
    Color32 originalColor;

    private void Start()
    {
        if (GameValue.Instance != null)
          //  GameValue.Instance.RegisterResourceChanged(OnGameValueChanged);
            GameValue.Instance.RegisterSBNResourceChanged(OnGameValueChanged);
        else
            Debug.LogError("GameValue.Instance is null at Start!");

        double initialValue = 0;
        switch (type)
        {
            case SBNType.Scout: initialValue = GameValue.Instance.GetResourceValue().Scout; break;
            case SBNType.Build: initialValue = GameValue.Instance.GetResourceValue().Build; break;
            case SBNType.Negotiation: initialValue = GameValue.Instance.GetResourceValue().Negotiation; break;
        }
        previousValue = initialValue; // <-- 记录初始值
        valueText.text = GetColorString(FormatDoubleNumberToFormatNumber(initialValue), 2, (int)type);

        Image img = TextButton.GetComponent<Image>();
        originalColor = img.color;


        ImageButton.onClick.AddListener(ToggleCharacterAssistRowControl);
        TextButton.onClick.AddListener(ToggleCharacterAssistRowControl);
    }

    private void OnDisable()
    {
    //ImageButton.image.color = originalColor;
        TextButton.image.color = originalColor;
    }
    void ToggleCharacterAssistRowControl()
    {
        if (CharacterAssistRowControl.Instance.type == type && CharacterAssistRowControl.Instance.IsActive())
        {
            CharacterAssistRowControl.Instance.ToggleRow(SBNType.None);
        }
        else
        {
            CharacterAssistRowControl.Instance.ToggleRow(type);
        }
    }

    private void OnDestroy()
    {
        if (GameValue.Instance != null)
            GameValue.Instance.UnRegisterSBNResourceChanged(OnGameValueChanged);
    }

    private void OnGameValueChanged(string key, double val)
    {
        if (string.Equals(key, TransTypeToString(), StringComparison.OrdinalIgnoreCase))
        {
            valueText.text = GetColorString(FormatDoubleNumberToFormatNumber(val), 2, (int)type);

            bool isIncrease = val > previousValue;
            if (Math.Abs(val - previousValue) > 0.01f) // 变化明显再闪
                FlashColor(isIncrease);

            previousValue = val;
        }
    }

    void FlashColor(bool isIncrease)
    {
        Image img = TextButton.GetComponent<Image>();
        if (img == null) return;

        Color32 targetColor = isIncrease ? GetValueColor(ValueColorType.Increase) : GetValueColor(ValueColorType.Decrease);

        StopAllCoroutines();
        StartCoroutine(FlashColorCoroutine(img, targetColor));
    }

    IEnumerator FlashColorCoroutine(Image img, Color32 flashColor)
    {
        float flashTime = 0.05f;
        img.color = flashColor;
        yield return new WaitForSeconds(flashTime);
        img.color = originalColor;


        /* 
         *         float flashTime = 0.05f;

         * for (int i = 0; i < 2; i++)
         {
             img.color = flashColor;
             yield return new WaitForSeconds(flashTime);
             img.color = original;
             yield return new WaitForSeconds(flashTime);
         }*/
    }

    string TransTypeToString()
    {
        switch (type)
        {
            case SBNType.Scout: return "Scout"; 
            case SBNType.Build: return "Build"; 
            case SBNType.Negotiation: return "Negotiation";

        }
        return "BUG";
    }
}
