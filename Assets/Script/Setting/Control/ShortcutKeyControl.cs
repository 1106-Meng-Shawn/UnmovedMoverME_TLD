using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using static ShortcutKeyCodeString;

public class ShortcutKeyControl : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    #region UI References

    [Header("UI References")]
    [SerializeField] private Image ShortcutKeyIcon;
    [SerializeField] private Button ShortcutKeyButton;
    [SerializeField] private TextMeshProUGUI buttonText;

    #endregion

    #region Private Fields

    private ShortcutKey currentShortcutKey = ShortcutKey.None;
    private bool isWaitingForInput = false;
    private bool isMouseOverButton = false;
    private Color originalButtonColor;

    private Action<ShortcutKey> onKeyChanged;
    private Func<ShortcutKey> getKeyValue;
    private Func<ShortcutKey, bool> validator;

    #endregion

    #region Unity Lifecycle

    void Start()
    {
        InitializeComponents();
        SetupButtonListener();
    }

    void Update()
    {
        if (isWaitingForInput)
        {
            HandleKeyCapture();
        }
    }

    void OnDisable()
    {
        CancelKeyCapture();
    }

    #endregion

    #region Initialization

    private void InitializeComponents()
    {
        if (buttonText == null && ShortcutKeyButton != null)
        {
            buttonText = ShortcutKeyButton.GetComponentInChildren<TextMeshProUGUI>();
        }

        if (ShortcutKeyButton != null)
        {
            Image buttonImage = ShortcutKeyButton.GetComponent<Image>();
            if (buttonImage != null)
            {
                originalButtonColor = buttonImage.color;
            }
        }

    }

    private void SetupButtonListener()
    {
        if (ShortcutKeyButton != null)
        {
            ShortcutKeyButton.onClick.AddListener(StartListeningForKey);
        }
    }

    #endregion

    #region Binding Methods

    /// <summary>
    /// 绑定到外部的 ShortcutKey 变量（通过委托实现双向绑定）
    /// </summary>
    /// <param name="getter">获取当前值的函数</param>
    /// <param name="setter">设置新值的函数</param>
    //public void BindToKey(Func<ShortcutKey> getter, Action<ShortcutKey> setter)
    //{
    //    getKeyValue = getter;
    //    onKeyChanged = setter;

    //    currentShortcutKey = getKeyValue();
    //    UpdateButtonText(currentShortcutKey);
    //}


    public void BindToKey(Func<ShortcutKey> getter, Action<ShortcutKey> setter, Func<ShortcutKey, bool> keyValidator)
    {
        getKeyValue = getter;
        onKeyChanged = setter;
        validator = keyValidator;

        currentShortcutKey = getKeyValue();
        UpdateButtonText(currentShortcutKey);
    }


    private bool ValidateKey(ShortcutKey key)
    {
        if (validator == null)
            return true;
        return validator(key);
    }


    #endregion

    #region Pointer Events

    public void OnPointerEnter(PointerEventData eventData)
    {
        isMouseOverButton = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isMouseOverButton = false;
    }

    #endregion

    #region Key Capture Logic

    private void StartListeningForKey()
    {
        isWaitingForInput = true;
        isMouseOverButton = true;

        SetButtonState(true);
    }

    private void HandleKeyCapture()
    {
        // Cancel if mouse leaves button
        if (!isMouseOverButton)
        {
            CancelKeyCapture();
            return;
        }

        // Cancel on left mouse click
        if (Input.GetMouseButtonDown(0))
        {
            CancelKeyCapture();
            return;
        }

        CaptureKeyInput();
    }

    private void CaptureKeyInput()
    {
        ShortcutKey pressedKey = GetPressedShortcutKey();
        if (pressedKey == ShortcutKey.None) return;

        KeyCode code = ToKeyCode(pressedKey);
        if (IsBlackListed(code)) return;

        if (!ValidateKey(pressedKey))
        {
            ShowValidationError();
            return; 
        }

        currentShortcutKey = pressedKey;

        if (onKeyChanged != null)
        {
            onKeyChanged(pressedKey);
        }

        UpdateButtonText(pressedKey);
        isWaitingForInput = false;
        SetButtonState(false);
    }

    private void ShowValidationError()
    {
        //  NotificationManage.Instance.ShowAtTopByKey();
        Debug.Log("fail");
    }


    private void CancelKeyCapture()
    {
        isWaitingForInput = false;

        if (currentShortcutKey != ShortcutKey.None)
        {
            UpdateButtonText(currentShortcutKey);
        }

        SetButtonState(false);
    }

    #endregion

    #region UI Update Methods

    private void SetButtonState(bool isCapturing)
    {
        if (ShortcutKeyButton != null)
        {
            Image buttonImage = ShortcutKeyButton.GetComponent<Image>();
            if (buttonImage != null)
            {
                buttonImage.color = isCapturing ? Color.black : originalButtonColor;
            }
        }

        if (buttonText != null)
        {
            buttonText.text = isCapturing ? "Press any key..." : ToDisplayString(currentShortcutKey);
        }
    }

    private void UpdateButtonText(ShortcutKey key)
    {
        buttonText.text = ToDisplayString(key);
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// 获取当前快捷键
    /// </summary>
    public ShortcutKey GetCurrentKey()
    {
        return currentShortcutKey;
    }

    ///// <summary>
    ///// 设置快捷键（单向设置，不触发绑定回调）
    ///// </summary>
    //public void SetKey(ShortcutKey key)
    //{
    //    currentShortcutKey = key;
    //    UpdateButtonText(key);
    //}

    /// <summary>
    /// 设置快捷键并触发绑定回调（双向更新）
    /// </summary>
    public void SetKeyWithCallback(ShortcutKey key)
    {
        currentShortcutKey = key;

        if (onKeyChanged != null)
        {
            onKeyChanged(key);
        }

        UpdateButtonText(key);
    }

    /// <summary>
    /// 从绑定的源刷新显示
    /// </summary>
    public void RefreshFromBinding()
    {
        if (getKeyValue != null)
        {
            currentShortcutKey = getKeyValue();
            UpdateButtonText(currentShortcutKey);
        }
    }

    #endregion
}

