using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static GetColor;

public enum TextColorType
{
    Normal,
    Read,
    Outline,
    DialogBox
}


public class TextPanelControl : SettingPanelBase
{
    // Start is called before the first frame update
    [Header("Text Panel UI")]
    public TypewriterEffect typewriterEffect;
    public TextMeshProUGUI testText;
    public Slider textSpeedSlider;
    public Slider autoSpeedSlider;
    [SerializeField] Slider outlineWidth;


    public Toggle skipUnreadToggle;
    public Toggle changeReadColorToggle;
    [SerializeField] TextBackgroundControl textBackgroundControl;
    [SerializeField] PickColor pickColor;

    private TextColorType currentColorType = TextColorType.Normal;

    [Header("Buttons")]
    [SerializeField] Buttons buttons;


    #region Buttons
    [System.Serializable]
    public class Buttons
    {
        public Button NormalTextColorButton;
        public Button ReadTextColorButton;
        public Button OutLineColorButton;
        public Button DialogueBoxColorButton;

    }
    #endregion


    public override void Init()
    {
        textSpeedSlider.value = 1 - (typewriterEffect.typingSpeed / 0.1f);
        textSpeedSlider.onValueChanged.AddListener(OnTextSpeedChanged);

        typewriterEffect.waitTime = Constants.DEFAULT_WAITING_SECONDS;
        autoSpeedSlider.value = 1 - (typewriterEffect.waitTime / 4f);
        autoSpeedSlider.onValueChanged.AddListener(OnAutoSpeedChanged);

        skipUnreadToggle.isOn = SettingValue.Instance.GetTextSettingValue().skipUnread;
        changeReadColorToggle.isOn = SettingValue.Instance.GetTextSettingValue().changeReadColor;
        skipUnreadToggle.onValueChanged.AddListener(OnSkipUnreadToggleChanged);
        changeReadColorToggle.onValueChanged.AddListener(OnChangeReadColorToggleChanged);
        InitButtons();
    }


    void InitButtons()
    {
        buttons.NormalTextColorButton.onClick.AddListener(() => OnColorButtonClick(TextColorType.Normal));
        buttons.ReadTextColorButton.onClick.AddListener(() => OnColorButtonClick(TextColorType.Read));
        buttons.OutLineColorButton.onClick.AddListener(() => OnColorButtonClick(TextColorType.Outline));
        buttons.DialogueBoxColorButton.onClick.AddListener(() => OnColorButtonClick(TextColorType.DialogBox));

    }


    void OnColorButtonClick(TextColorType type)
    {
        currentColorType = type;
        Color32 currentColor = GetCurrentColor(type);
        pickColor.SetColor(currentColor, OnPickColorChanged);
    }


    void OnPickColorChanged(Color32 newColor)
    {
        switch (currentColorType)
        {
            case TextColorType.Normal: SettingValue.Instance.GetTextSettingValue().NormalTextColor = newColor; break;
            case TextColorType.Read: SettingValue.Instance.GetTextSettingValue().ReadTextColor = newColor; break;
            case TextColorType.Outline: SettingValue.Instance.GetTextSettingValue().OutLineColor = newColor; break;
            case TextColorType.DialogBox: SettingValue.Instance.GetTextSettingValue().DialogBoxColor = newColor; break;
        }
    }



    Color32 GetCurrentColor(TextColorType type)
    {
        return type switch
        {
            TextColorType.Normal => SettingValue.Instance.GetTextSettingValue().NormalTextColor,
            TextColorType.Read => SettingValue.Instance.GetTextSettingValue().ReadTextColor,
            TextColorType.Outline => SettingValue.Instance.GetTextSettingValue().OutLineColor,
            TextColorType.DialogBox => SettingValue.Instance.GetTextSettingValue().DialogBoxColor,
            _ => UnityEngine.Color.white
        };
    }


    public override void OnDefaultButtonClick()
    {
        TextSettingValue textValue = new TextSettingValue();
        SetSettingValue(textValue);

    }


    void SetSettingValue(TextSettingValue textValue)
    {
        typewriterEffect.SetTypingSpeedAndWaitTime(textValue.typingSpeed, textValue.waitTime);
        autoSpeedSlider.value = 1 - (typewriterEffect.waitTime / 4f);
        textSpeedSlider.value = 1 - (typewriterEffect.typingSpeed / 0.1f);

        SettingValue.Instance.GetTextSettingValue().skipUnread = textValue.skipUnread;
        SettingValue.Instance.GetTextSettingValue().changeReadColor = textValue.changeReadColor;
        skipUnreadToggle.isOn = textValue.skipUnread;
        changeReadColorToggle.isOn = textValue.changeReadColor;
    }


    private void OnSkipUnreadToggleChanged(bool isOn)
    {
        SettingValue.Instance.GetTextSettingValue().skipUnread = isOn;
    }

    private void OnChangeReadColorToggleChanged(bool isOn)
    {
        SettingValue.Instance.GetTextSettingValue().changeReadColor = isOn;
    }

    private void OnTextSpeedChanged(float value)
    {
        typewriterEffect.typingSpeed = Mathf.Lerp(0.1f, 0f, value);
        SettingValue.Instance.GetTextSettingValue().typingSpeed = typewriterEffect.typingSpeed;

    }

    private void OnAutoSpeedChanged(float value)
    {
        typewriterEffect.waitTime = Mathf.Lerp(4f, 0.01f, value);
        SettingValue.Instance.GetTextSettingValue().waitTime = typewriterEffect.waitTime;
    }   

    public override void OpenPanel()
    {
        base.OpenPanel();
        textBackgroundControl.OpenPanel("Anna");
        OnColorButtonClick(currentColorType);

    }




    // it have bug for don't wait when typespeed is 0, and then auto wait speed or skip speed maybe change, right now auto will fast than the skip
    private IEnumerator TestText()
    {
        while (testText.gameObject.activeSelf)
        {
            if (!typewriterEffect.IsTyping())
            {
                typewriterEffect.StartTyping(testText.text, testText);
            }

            yield return null;
        }
    }



    // Update is called once per frame
    void Update()
    {
        
    }
}

public class TextSettingValue
{
    public event System.Action OnValueChanged;

    private float _typingSpeed = TextSettingConstants.TYPING_SPEED;
    public float typingSpeed
    {
        get => _typingSpeed;
        set { _typingSpeed = value; NotifyChanged(); }
    }

    private float _waitTime = TextSettingConstants.SKIP_TYPING_SPEED;
    public float waitTime
    {
        get => _waitTime;
        set { _waitTime = value; NotifyChanged(); }
    }

    private bool _skipUnread;
    public bool skipUnread
    {
        get => _skipUnread;
        set { _skipUnread = value; NotifyChanged(); }
    }

    private bool _changeReadColor = true;
    public bool changeReadColor
    {
        get => _changeReadColor;
        set { _changeReadColor = value; NotifyChanged(); }
    }


    Vector4 _normalTextColorVector = ColorToVector(TextSettingConstants.NORMAL_TEXT_COLOR);
    Vector4 _readTextColorVector = ColorToVector(TextSettingConstants.READ_TEXT_COLOR);
    Vector4 _outLineColorVector = ColorToVector(TextSettingConstants.OUTLINE_COLOR);
    Vector4 _dialogBoxColorVector = ColorToVector(TextSettingConstants.DIALOG_BOX_COLOR);

    #region Color
    public Color32 NormalTextColor
    {
        get => VectorToColor(_normalTextColorVector);
        set { _normalTextColorVector = ColorToVector(value); NotifyChanged(); }
    }

    public Color32 ReadTextColor
    {
        get => VectorToColor(_readTextColorVector);
        set { _readTextColorVector = ColorToVector(value); NotifyChanged(); }
    }

    public Color32 OutLineColor
    {
        get => VectorToColor(_outLineColorVector);
        set { _outLineColorVector = ColorToVector(value); NotifyChanged(); }
    }

    public Color32 DialogBoxColor
    {
        get => VectorToColor(_dialogBoxColorVector);
        set { _dialogBoxColorVector = ColorToVector(value); NotifyChanged(); }
    }
    #endregion
    private void NotifyChanged()
    {
        OnValueChanged?.Invoke();
    }
}


public static class TextSettingConstants
{
    public const float TYPING_SPEED = 0.05f;
    public const float SKIP_TYPING_SPEED = 0.01f;

    public readonly static Color32 NORMAL_TEXT_COLOR = new Color32(255,255,255,255);
    public readonly static Color32 READ_TEXT_COLOR = new Color32(245, 245, 245, 100);
    public readonly static Color32 OUTLINE_COLOR = new Color32(0, 0, 0, 255);
    public readonly static Color32 DIALOG_BOX_COLOR = new Color32(245,245,245,100);


}