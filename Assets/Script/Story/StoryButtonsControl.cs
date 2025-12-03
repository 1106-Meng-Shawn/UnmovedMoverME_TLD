using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class StoryButtonsControl : MonoBehaviour
{

    public List<GameObject> bottomButtons;


    private float mouseNotOverInButtonTimer = 0f;
    public Button lockButton;


    private bool isLock = false;
    private bool recordIsLock = false;
    private bool isHidden = true;
    public Animation buttonsAnim;


    public Button autoButton;
    public Button skipButton;
    public Button historyButton;
    public Button saveButton;
    public Button settingButton;
    public Button loadButton;

    public Button quickSaveButton;
    public Button quickLoadButton;


    public Button skipToChooseButton;
    public Button skipNodeButton;
    private List<Button> allButtons;


    private TotalStoryManager totalStoryManager;

    private void Awake()
    {
        SetAllButtons();
        BottomButtonsAddListener();
    }

    private void Start()
    {
        totalStoryManager = TotalStoryManager.Instance;
    }

    void SetAllButtons()
    {
        allButtons = new List<Button>
        {
            autoButton,
            skipButton,
            historyButton,
            saveButton,
            settingButton,
            loadButton,
            quickSaveButton,
            quickLoadButton,
            skipToChooseButton,
            skipNodeButton
        };

    }

    public bool IsHittingBottomButtons()
    {
        foreach (var oj in bottomButtons)
        {
            var rect = oj.GetComponent<RectTransform>();
            var canvas = oj.GetComponentInParent<Canvas>();

            bool isHit = RectTransformUtility.RectangleContainsScreenPoint(
                rect,
                Input.mousePosition,
                canvas.worldCamera  // ?????
            ) && totalStoryManager.IsPointerOverStoryLayer();

            if (isHit) return true;
        }
        return false;
    }



    public void InitializeBottomButtons()
    {

        StartCoroutine(WaitForSecondsShowBottomButtons(0.1f));

    }

    IEnumerator WaitForSecondsShowBottomButtons(float delay)
    {
        yield return new WaitForSeconds(delay);

        mouseNotOverInButtonTimer = 0f;
        ShowBottomButtons();

    }

    private void BottomButtonsAddListener()
    {
        lockButton.onClick.AddListener(OnLockButtonClick);
        UpLockButtonSprite();


        autoButton.onClick.AddListener(OnAutoButtonClick);
        skipButton.onClick.AddListener(OnSkipButtonClick);

        historyButton.onClick.AddListener(OnHistoryButtonClick);
        settingButton.onClick.AddListener(OnSettingButtonClick);


        saveButton.onClick.AddListener(OnSaveButtonClick);
        loadButton.onClick.AddListener(OnLoadButtonClick);

        quickSaveButton.onClick.AddListener(OnQuickSaveButtonClick);
        quickLoadButton.onClick.AddListener(OnQuickLoadButtonClick);


        skipToChooseButton.onClick.AddListener(OnSkipToChooseButtonClick);
        skipNodeButton.onClick.AddListener(OnSkipNodeButtonClick);


    }

    void OnQuickLoadButtonClick()
    {
        LoadPanelManage.Instance.QuickSaveGame();
    }



    void OnQuickSaveButtonClick()
    {
        totalStoryManager.OnQuickSaveButtonClick();
    }



    void OnLoadButtonClick()
    {
        totalStoryManager.OnLoadButtonClick();
    }


    void OnSaveButtonClick()
    {
        LoadPanelManage.Instance.NormalSaveGame();
    }



    public void OnHistoryButtonClick()
    {

        totalStoryManager.OnHistoryButtonClick();
    }

    public void ShowOrHideoButtons()
    {
        if (isLock) return;

        bool mouseOver = IsHittingBottomButtons();

        if (mouseOver && isHidden)
        {
            mouseNotOverInButtonTimer = 0f;
            ShowBottomButtons();
        }
        else if (!mouseOver && !isHidden)
        {
            mouseNotOverInButtonTimer += Time.deltaTime;
            if (!isHidden && mouseNotOverInButtonTimer >= 1f)
            {
                HideBottomButtons();
            }
        }

    }

    void ShowBottomButtons()
    {
        if (isHidden)
        {
            buttonsAnim.Play("ShowBottomButton");
            isHidden = false;
        }

    }

    void HideBottomButtons()
    {

        if (!isHidden)
        {

            buttonsAnim.Play("HideBottomButton");
            isHidden = true;


        }
    }


    void OnLockButtonClick()
    {
        SwithchLockState();
    }

    void SwithchLockState()
    {
        isLock = !isLock;
        recordIsLock = isLock;
        UpLockButtonSprite();

    }

    void UpLockButtonSprite()
    {
        string iconPath = $"MyDraw/UI/Other/";

        if (isLock)
        {
            lockButton.image.sprite = Resources.Load<Sprite>(iconPath + "Lock");
            lockButton.gameObject.GetComponent<ButtonEffect>().SetChangeSprite(Resources.Load<Sprite>(iconPath + "LockSel"), Resources.Load<Sprite>(iconPath + "Lock"));

        }
        else
        {
            lockButton.image.sprite = Resources.Load<Sprite>(iconPath + "Unlock");
            lockButton.gameObject.GetComponent<ButtonEffect>().SetChangeSprite(Resources.Load<Sprite>(iconPath + "UnlockSel"), Resources.Load<Sprite>(iconPath + "Unlock"));

        }
    }

    void OnAutoButtonClick()
    {
        totalStoryManager.OnAutoButtonClick();
    }


    void OnSkipButtonClick()
    {
        totalStoryManager.OnSkipButtonClick();
    }

    void OnSkipNodeButtonClick()
    {
        totalStoryManager.OnSkipNodeButtonClick();
    }


    void OnSkipToChooseButtonClick()
    {
        totalStoryManager.OnSkipToChooseButtonClick();
    }


    public void SetButtonsInteractableWithException(Button exceptButton, bool isEnabled)
    {
        UpdateLockButtonState(isEnabled);

        foreach (var button in allButtons)
        {
            if (button == null) continue;

            bool shouldEnable = (button == exceptButton) || isEnabled;
            SetButtonState(button, shouldEnable);
        }
    }

    void UpdateLockButtonState(bool enableButtons)
    {
        string iconPath = $"MyDraw/UI/Other/";
        if (!enableButtons)
        {
            isLock = true;
            SetButtonState(lockButton, false);
            lockButton.image.sprite = Resources.Load<Sprite>(iconPath + "Lock");
            lockButton.gameObject.GetComponent<ButtonEffect>().SetChangeSprite(Resources.Load<Sprite>(iconPath + "LockSel"), Resources.Load<Sprite>(iconPath + "Lock"));

        }
        else
        {
            if (!recordIsLock)
            {
                isLock = false;
                lockButton.image.sprite = Resources.Load<Sprite>(iconPath + "Unlock");
                lockButton.gameObject.GetComponent<ButtonEffect>().SetChangeSprite(Resources.Load<Sprite>(iconPath + "UnlockSel"), Resources.Load<Sprite>(iconPath + "Unlock"));
            }
            SetButtonState(lockButton, true);
        }
    }

    void SetButtonState(Button button, bool isEnabled)
    {
        var color = button.image.color;
        color.a = isEnabled ? 1f : 0.5f;
        button.image.color = color;
    }

    void OnSettingButtonClick()
    {
        SettingsManager.Instance.OpenPanel();
    }



}
