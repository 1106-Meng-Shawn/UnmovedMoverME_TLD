using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// ????????????????????????
/// </summary>
public class StoryInputHandler : MonoBehaviour
{
    private bool isAutoPlay = false;
    private bool isSkip = false;
    private bool isSkipAll = false;
    private bool skipUnread = false;

    [SerializeField] private StoryDataManager storyDataManager;
    [SerializeField] private StoryUIController uiController;
    [SerializeField] private StoryMediaController mediaController;
    [SerializeField] private TypewriterEffect typewriterEffect;
    [SerializeField] private StoryButtonsControl StoryButtonsControl;

    [SerializeField] private GameObject DialogueBox;
    [SerializeField] private GameObject PanelStory;


    /// <summary>
    /// ???????????
    /// </summary>
    public bool ClickNextSentence()
    {
        return IsPointerOverStoryLayer() &&
            (Input.GetMouseButtonDown(0) ||
             Input.GetKeyDown(KeyCode.Space) ||
             Input.GetKeyDown(KeyCode.KeypadEnter) ||
             Input.GetKeyDown(KeyCode.Return));
    }

    /// <summary>
    /// ?????????
    /// </summary>
    public bool IsPointerOverStoryLayer()
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        var raycastResults = new System.Collections.Generic.List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, raycastResults);

        if (raycastResults.Count > 0)
        {
            GameObject firstHit = raycastResults[0].gameObject;
            return firstHit.layer == LayerMask.NameToLayer("Story");
        }

        return true;
    }

    /// <summary>
    /// ????????
    /// </summary>
    public void OnAutoButtonClick()
    {
        isAutoPlay = !isAutoPlay;

        if (isAutoPlay)
        {
            StoryButtonsControl.SetButtonsInteractableWithException(StoryButtonsControl.autoButton, false);
            StartCoroutine(StartAutoPlay());
        }
        else
        {
            StoryButtonsControl.SetButtonsInteractableWithException(null, true);
        }
    }

    /// <summary>
    /// ??????
    /// </summary>
    private IEnumerator StartAutoPlay()
    {
        while (isAutoPlay)
        {
            if (!typewriterEffect.IsTyping() && IsShowText())
            {
                // DisplayNextLine() ?????????
            }
            yield return new WaitForSeconds(typewriterEffect.waitTime);
        }
    }

    /// <summary>
    /// ??????
    /// </summary>
    public void OnSkipButtonClick()
    {
        if (!isSkip && CanSkip())
        {
            StartSkip();
        }
        else if (isSkip)
        {
            StopCoroutine(SkipToMaxReachedLine());
            EndSkip();
        }
    }

    /// <summary>
    /// ??????
    /// </summary>
    public void OnSkipNodeButtonClick()
    {
        isSkipAll = true;
        var storyData = storyDataManager.GetStoryData();
        while (storyDataManager.GetCurrentLine() < storyData.Count && isSkipAll)
        {
            var data = storyData[storyDataManager.GetCurrentLine()];
            // DoEffect(data) ?????????
            storyDataManager.AdvanceLineIndex(data);
        }

        if (storyDataManager.GetCurrentLine() >= storyData.Count)
        {
            // MarkStoryAsCompleted() ?????????
        }
    }

    /// <summary>
    /// ??????????
    /// </summary>
    private IEnumerator SkipToMaxReachedLine()
    {
        while (isSkip)
        {
            if (CanSkip())
            {
                // DisplayNextLine() ?????????
            }
            else
            {
                EndSkip();
            }
            yield return new WaitForSeconds(Constants.DEFAULT_SKIP_WAITING_SECONDS);
        }
    }

    /// <summary>
    /// ????
    /// </summary>
    private void StartSkip()
    {
        isSkip = true;
        StoryButtonsControl.SetButtonsInteractableWithException(StoryButtonsControl.skipButton, false);
        typewriterEffect.typingSpeed = Constants.SKIP_TYPING_SPEED;
        StartCoroutine(SkipToMaxReachedLine());
    }

    /// <summary>
    /// ????
    /// </summary>
    private void EndSkip()
    {
        isSkip = false;
        StoryButtonsControl.SetButtonsInteractableWithException(null, true);
        typewriterEffect.typingSpeed = Constants.DEFAULT_TYPING_SPEED;
    }

    /// <summary>
    /// Ctrl ????
    /// </summary>
    public void CtrlSkip()
    {
        if (isSkip) isSkip = false;
        if (isAutoPlay) isAutoPlay = false;

        typewriterEffect.typingSpeed = Constants.SKIP_TYPING_SPEED;
        StartCoroutine(SkipWhilePressCtrl());
    }

    /// <summary>
    /// ?? Ctrl ??????
    /// </summary>
    private IEnumerator SkipWhilePressCtrl()
    {
        while (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        {
            while (typewriterEffect.IsTyping())
            {
                typewriterEffect.CompleteLine();
                yield return null;
            }

            // DisplayNextLine() ?????????
            yield return new WaitForSeconds(0.25f);
        }

        typewriterEffect.typingSpeed = Constants.DEFAULT_TYPING_SPEED;
    }

    /// <summary>
    /// ????????
    /// </summary>
    private bool IsShowText()
    {
        return DialogueBox.activeSelf || PanelStory.activeSelf;
    }

    /// <summary>
    /// ????????
    /// </summary>
    private bool CanSkip()
    {
        return storyDataManager.CanSkip(skipUnread);
    }

    // ==================== Getters ====================

    public bool GetIsAutoPlay() => isAutoPlay;
    public bool GetIsSkip() => isSkip;
    public bool GetIsSkipAll() => isSkipAll;
    public bool GetSkipUnread() => skipUnread;

    // ==================== Setters ====================

    public void SetSkipUnread(bool value) => skipUnread = value;
    public void SetIsAutoPlay(bool value) => isAutoPlay = value;
    public void SetIsSkip(bool value) => isSkip = value;
    public void SetIsSkipAll(bool value) => isSkipAll = value;
}