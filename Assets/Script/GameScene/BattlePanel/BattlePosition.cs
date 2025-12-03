using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using static GetSprite;
using static GetColor;
using static GetString;
using UnityEngine.TextCore.Text;
using System.Collections.Generic;

public class BattlePosition : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler,IPointerDownHandler, IPointerUpHandler
{
    public Image classImage;
    public Image profileIcon;
    public Image background;
    public Image highLightImage;

    public TMP_Text nameText;

    public TMP_Text AttackValueText;
    public TMP_Text DefenseValueText;
    public TMP_Text MagicValueText;
    public TMP_Text SpeedValueText;
    public TMP_Text LuckyValueText;

    public TMP_Text ForceText;

    public GameObject moveOj;
    public TMP_Text moveText;

    public Character characterAtBattlePosition;

    public bool isInExplore;
    public BattleCharacterValue battleCharacterValue;

    public bool isScoutPosistion;

    public bool isEnemy;
    public GameObject hideValueRow;
    public GameObject hideValueBlock;

    public UnityEvent onCharacterChanged = new UnityEvent();

    private Coroutine blinkCoroutine;

    // 拖拽相关
    private GameObject draggedIcon;
    private bool isDragging;

    [Header("Button and Hold Parameter")]
    [SerializeField] private float doubleClickThreshold = 0.25f; // 双击判定间隔
    [SerializeField] private float holdDelay = 0.25f;             // 长按开始时间
    [SerializeField] private float forceIncreaseRate = 1f;       // 力增长速度


    private float lastClickTime = 0f;
    private float currentHoldTime = 0f;
    private float accumulatedForce = 0f;

    private bool isHolding = false;
    private bool holdStarted = false;
    private bool isDouble = false;

    private Coroutine clickDelayCoroutine = null;
    private Coroutine holdForceCoroutine = null;

    [SerializeField] private bool enableHold = true;
    private Coroutine holdCoroutine = null;

    [SerializeField] private Image buttonImage;           // 按钮背景图（你按钮的图）
    [SerializeField] private Color flashColor = Color.white; // 闪一下的颜色
    [SerializeField] private float flashDuration = 0.1f;      // 闪一下持续时间

    private Color originalColor;
    private int hideLevel = 0;
    private bool hasDragged = false;

    public List<GameObject> HideObject;

    public Image PersonBattleType;
    private Canvas uiCanvas;
    void Start()
    {
        UpdateBattlePosition();
        uiCanvas = GeneralManager.Instance.uiCanvas;
    }

    void OnDestroy()
    {
       
        if (characterAtBattlePosition != null)
        {
            characterAtBattlePosition.OnCharacterChanged -= UpdateBattlePosition;
        }
        characterAtBattlePosition = null;


    }

    // --- Pointer Events ---
    public void OnPointerDown(PointerEventData eventData)
    {
        CeartDraggedIcon();

        //if (!CanHold()) return;

        float timeSinceLastClick = Time.time - lastClickTime;
        lastClickTime = Time.time;
        isHolding = true;
        isDouble = false;
        holdStarted = false;
        accumulatedForce = 0f;
        currentHoldTime = 0f;

        if (timeSinceLastClick < doubleClickThreshold)
        {

            if (clickDelayCoroutine != null)
            {
                StopCoroutine(clickDelayCoroutine);
                clickDelayCoroutine = null;
            }
            isDouble = true;
            HandleDoubleClickByType();
            return;
        }

        // 启动长按检测
        if (enableHold)
        {
            holdCoroutine = StartCoroutine(HoldRoutine());
        }

        clickDelayCoroutine = StartCoroutine(ClickDelayRoutine());
    }

    void CeartDraggedIcon()
    {

        if (characterAtBattlePosition != null && draggedIcon == null)
        {
            draggedIcon = new GameObject("DraggedIcon");
            draggedIcon.transform.SetParent(transform.root);
            Image draggedImage = draggedIcon.AddComponent<Image>();
            draggedImage.sprite = characterAtBattlePosition.icon;
            draggedImage.raycastTarget = false;

            draggedImage.rectTransform.sizeDelta = new Vector2(0.5f, 0.5f);
            RectTransform rt = draggedImage.rectTransform;
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.position = Input.mousePosition + new Vector3(20, 20, 0);
        }
    }


    public void OnPointerUp(PointerEventData eventData)
    {
        Destroy(draggedIcon);
        draggedIcon = null;

        // 如果是长按或双击，不触发单击
        if (holdStarted || isDouble)
        {
            ResetHoldState();
            return;
        }

        // 正常单击：已在 ClickDelayRoutine 中处理，无需此处做任何事

        ResetHoldState();
    }

    private IEnumerator HoldRoutine()
    {
        yield return new WaitForSeconds(holdDelay);
        if (isHolding)
        {
            holdStarted = true;

            // 取消单击触发
            if (clickDelayCoroutine != null)
            {
                StopCoroutine(clickDelayCoroutine);
                clickDelayCoroutine = null;
            }

            HandleHoldByType();
        }
    }

    private IEnumerator ClickDelayRoutine()
    {
        yield return new WaitForSeconds(holdDelay);

        // 确保不是长按/双击再触发单击
        if (!holdStarted && !isDouble)
            HandleSingleClick();

        clickDelayCoroutine = null;
    }

    private IEnumerator HoldForceIncreaseRoutine()
    {
        currentHoldTime = 0f;
        accumulatedForce = 0f;

        while (isHolding)
        {
            float t = currentHoldTime;
            float rawIncrease = forceIncreaseRate * (t + 0.3f) * t * 10f;

            accumulatedForce += rawIncrease;
            int increase = Mathf.FloorToInt(accumulatedForce);
            accumulatedForce -= increase;

            if (increase > 0)
            {
                ApplyForceIncrease(increase);
            }

            currentHoldTime += Time.deltaTime;
            yield return null;
        }
    }

    void ResetHoldState()
    {
        isHolding = false;
        holdStarted = false;
        currentHoldTime = 0f;
        accumulatedForce = 0f;

        if (holdCoroutine != null)
        {
            StopCoroutine(holdCoroutine);
            holdCoroutine = null;
        }

        if (holdForceCoroutine != null)
        {
            StopCoroutine(holdForceCoroutine);
            holdForceCoroutine = null;
        }
    }


    private void HandleSingleClick()
    {
        if (isEnemy)
        {
            BattlePanelManage.Instance.SetCharacterToPanel(characterAtBattlePosition,true);
        }
        else
        {
            BattlePanelManage.Instance.SetCharacterToPanel(characterAtBattlePosition);
        }
    }


    private void HandleDoubleClickByType()
    {

        if (BattlePanelManage.Instance.IsPersonBattleInBattlePanel())
        {

        }
        else
        {
            FillForceToMax();
        }
    }

    private void HandleDoubleClickByType2()
    {
    }


    private void HandleHoldByType()
    {
        // if (forceType != 0) return;
        if (BattlePanelManage.Instance.IsPersonBattleInBattlePanel())
        {

        }
        else
        {
            holdForceCoroutine = StartCoroutine(HoldForceIncreaseRoutine());
        }
        // else if (type == 2) Debug.Log("长按类型2 - 待定义");
    }

    void ApplyForceIncrease(int increase)
    {
        if (isEnemy) return;
        if (hasDragged) return;
        if (characterAtBattlePosition == null) return;
        int availableForce = Mathf.Min(increase,
            GameValue.Instance.GetResourceValue().TotalRecruitedPopulation,
            characterAtBattlePosition.MaxForce - characterAtBattlePosition.Force);

        if (characterAtBattlePosition.MaxForce - characterAtBattlePosition.Force == 0)
        {
            // NotificationManage.Instance.ShowAtTop("Your force are full");
            NotificationManage.Instance.ShowAtTopByKey(NotificationKeyConstants.Force_AllFull);
        }
        else if (GameValue.Instance.GetResourceValue().TotalRecruitedPopulation == 0)
        {
            //NotificationManage.Instance.ShowAtTop("You don't have enough force.");
            NotificationManage.Instance.ShowAtTopByKey(NotificationKeyConstants.NotEnoughResource,GetResourceStringWithSprite(ValueType.TotalRecruitedPopulation));

        }

        if (availableForce > 0)
        {
            characterAtBattlePosition.Force += availableForce;
            GameValue.Instance.GetResourceValue().TotalRecruitedPopulation -= availableForce;
        }
    }

    void FillForceToMax()
    {
        if (isEnemy) return;   
        int needed = characterAtBattlePosition.MaxForce - characterAtBattlePosition.Force;
        int available = Mathf.Min(needed, GameValue.Instance.GetResourceValue().TotalRecruitedPopulation);

        if (needed == 0)
        {
            NotificationManage.Instance.ShowAtTopByKey(NotificationKeyConstants.Force_AllFull);
        }
        else if (GameValue.Instance.GetResourceValue().TotalRecruitedPopulation == 0)
        {
            NotificationManage.Instance.ShowAtTopByKey(NotificationKeyConstants.NotEnoughResource, GetResourceStringWithSprite(ValueType.TotalRecruitedPopulation));
        }


        if (available > 0)
        {
            GameValue.Instance.GetResourceValue().TotalRecruitedPopulation -= available;
            characterAtBattlePosition.Force += available;
            //  NotificationManage.Instance.ShowAtTop($"Your Add {needed}");
            NotificationManage.Instance.ShowAtTopByKey(NotificationKeyConstants.NotEnoughResource, GetResourceStringWithSprite(ValueType.TotalRecruitedPopulation));

        }
    }

    private void UpdateBattlePosition()
    {
        if (characterAtBattlePosition != null)
        {
            profileIcon.sprite = characterAtBattlePosition.icon;
            nameText.text = characterAtBattlePosition.GetCharacterName();

            classImage.gameObject.SetActive(true);
            classImage.sprite = GetRoleSprite(characterAtBattlePosition.RoleClass);

            AttackValueText.text = characterAtBattlePosition.GetValue(0, 0).ToString();
            DefenseValueText.text = characterAtBattlePosition.GetValue(0, 1).ToString();
            MagicValueText.text = characterAtBattlePosition.GetValue(0, 2).ToString();
            SpeedValueText.text = characterAtBattlePosition.GetValue(0, 3).ToString();
            LuckyValueText.text = characterAtBattlePosition.GetValue(0, 4).ToString();

            if (moveText != null) moveText.text  = GetValueColorString($"* {characterAtBattlePosition.BattleMoveNum}", ValueColorType.CanMove);


            if (!isEnemy)background.color = GetRowColor(RowType.sel);

            bool isPersonBattle = BattlePanelManage.Instance.IsPersonBattleInBattlePanel() || characterAtBattlePosition.IsPersonBattle;

            moveOj.gameObject.SetActive(!isPersonBattle);

            if (PersonBattleType != null) {
                PersonBattleType.gameObject.SetActive(true);
                PersonBattleType.sprite = UpBattleButtonSprite(isPersonBattle);
            }

            if (isPersonBattle)
            {
                ForceText.text = characterAtBattlePosition.GetHealthAndMaxHealthString();
            }
            else
            {
                ForceText.text = GetValueColorString($"{characterAtBattlePosition.Force.ToString()} / {characterAtBattlePosition.MaxForce.ToString()}", ValueColorType.Pop);
            }


        }
        else
        {
            ClearPosition();
        }

        onCharacterChanged?.Invoke();

        if (isScoutPosistion)
        {
            if (characterAtBattlePosition != null)
            {
                ForceText.text = GetValueColorString($"{characterAtBattlePosition.GetValue(2, 1)} + {(((double)characterAtBattlePosition.GetValue(2, 4)) / 10)}", ValueColorType.Scout);
            }
            else
            {
                ForceText.text = GetValueColorString($"{0} + {0}", ValueColorType.Scout);
            }
        }
    }

    public void ClearPosition()
    {
        if (characterAtBattlePosition != null)
        {
            characterAtBattlePosition.OnCharacterChanged -= UpdateBattlePosition;
        }

        characterAtBattlePosition = null;

        profileIcon.sprite = Resources.Load<Sprite>("MyDraw/UI/Character/CharacterValueIcon/CharacterIcon");

        classImage.gameObject.SetActive(false);
        PersonBattleType?.gameObject.SetActive(false);


        moveOj.gameObject.SetActive(!BattlePanelManage.Instance.IsPersonBattleInBattlePanel());

        nameText.text = "NONE";
        AttackValueText.text = "A";
        DefenseValueText.text = "D";
        MagicValueText.text = "M";
        SpeedValueText.text = "S";
        LuckyValueText.text = "L";


        if (moveText != null) moveText.text = GetValueColorString($"* {0}", ValueColorType.CanMove);

        if (!isEnemy)  background.color = GetRowColor(RowType.canMove);

        ForceText.text = GetValueColorString("0 / 0", ValueColorType.Pop);
    }

    // 拖拽相关实现
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (isEnemy) return;
        if (characterAtBattlePosition == null || draggedIcon != null) return;

        draggedIcon = new GameObject("DraggedIcon");
        draggedIcon.transform.SetParent(transform.root, false);
        Image draggedImage = draggedIcon.AddComponent<Image>();
        draggedImage.sprite = characterAtBattlePosition.icon;
        draggedImage.raycastTarget = false;

        RectTransform draggedRect = draggedImage.rectTransform;
        draggedRect.pivot = new Vector2(0.5f, 0.5f);
        draggedRect.anchorMin = draggedRect.anchorMax = new Vector2(0.5f, 0.5f);
        draggedRect.sizeDelta = new Vector2(100f, 100f);
        draggedRect.position = Input.mousePosition + new Vector3(20, 20, 0);

        isDragging = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isEnemy) return;

        if (draggedIcon != null)
        {
            RectTransform draggedIconRect = draggedIcon.GetComponent<RectTransform>();
            RectTransform canvasRect = transform.root.GetComponent<RectTransform>();

            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect,
                Input.mousePosition,
                uiCanvas.worldCamera, // Screen Space - Overlay 时传 null
                out localPoint
            );

            draggedIconRect.localPosition = localPoint;

            // 缩放建议设为 1，保持一致。250f 是错误用法。
            draggedIconRect.localScale = new Vector3(250f, 250f, 1f);
            hasDragged = true;
        }
    }


    public void OnEndDrag(PointerEventData eventData)
    {
        if (isEnemy || characterAtBattlePosition == null) return;

        CleanupDragVisuals();
        hasDragged = false;
        GameObject hitObject = eventData.pointerCurrentRaycast.gameObject;
        if (IsClickingSelfIcon(hitObject)) return;

        BattlePosition targetPosition = hitObject?.GetComponent<BattlePosition>();

        if (targetPosition != null)
        {
            if (targetPosition.characterAtBattlePosition != null)
                SwapCharacters(targetPosition);
            else
                MoveToEmptyPosition(targetPosition);
        }
        else
        {
            HandleRemoveFromBattle();
        }

        UpdateBattlePosition();
    }

    void CleanupDragVisuals()
    {
        if (draggedIcon != null)
        {
            Destroy(draggedIcon);
            draggedIcon = null;
        }
        isDragging = false;
    }

    bool IsClickingSelfIcon(GameObject hitObject)
    {
        if (hitObject != null && hitObject.name == "CharacterBattleImage")
        {
            BattlePanelManage.Instance.SetCharacterToPanel(characterAtBattlePosition);
            return true;
        }
        return false;
    }

    void SwapCharacters(BattlePosition targetPosition)
    {
        var sourceCharacter = characterAtBattlePosition;
        var targetCharacter = targetPosition.characterAtBattlePosition;

        if (sourceCharacter != null) sourceCharacter.OnCharacterChanged -= UpdateBattlePosition;
        if (targetCharacter != null) targetCharacter.OnCharacterChanged -= targetPosition.UpdateBattlePosition;

        var sourceValue = battleCharacterValue;
        var targetValue = targetPosition.battleCharacterValue;

        sourceCharacter?.SetBattlePositionToCharacter(null);
        targetCharacter?.SetBattlePositionToCharacter(null);

        targetPosition.SetCharacterToPosition(sourceCharacter);
        SetCharacterToPosition(targetCharacter);

        if (isInExplore)
        {
            targetPosition.battleCharacterValue = sourceValue;
            battleCharacterValue = targetValue;
        }
    }

    void MoveToEmptyPosition(BattlePosition targetPosition)
    {
        targetPosition.SetCharacterToPosition(characterAtBattlePosition);
        characterAtBattlePosition = null;

        if (isInExplore && targetPosition.battleCharacterValue == null)
        {
            targetPosition.battleCharacterValue = battleCharacterValue;
            battleCharacterValue = null;
        }
    }

    void HandleRemoveFromBattle()
    {
        characterAtBattlePosition?.SetBattlePositionToCharacter(null);
        SetCharacterToPosition(null);

        if (isInExplore)
            battleCharacterValue = null;
    }

    public void SetCharacterToPosition(Character character)
    {
        if (characterAtBattlePosition != null)
        {
            characterAtBattlePosition.OnCharacterChanged -= UpdateBattlePosition;
            characterAtBattlePosition.SetBattlePositionToCharacter(null);
        }

        characterAtBattlePosition = character;

        if (character != null)
        {
            character.SetBattlePositionToCharacter(this);
            character.OnCharacterChanged += UpdateBattlePosition;
        }

        if (isEnemy) SetHide(0);

        BattlePanelManage.Instance.RefreshHighlightStatus();
        UpdateBattlePosition();
    }



    public void SetHide(int level)
    {
        
        hideLevel = level;
        hideValueBlock.SetActive(hideLevel < 1);
        hideValueRow.SetActive(hideLevel < 2);

    }



    // 高亮闪烁
    public void Highlight(bool on)
    {
        if (on)
            StartBlinking();
        else
            StopBlinking();
    }

    public void StartBlinking()
    {
        if (blinkCoroutine == null)
        {
            blinkCoroutine = StartCoroutine(LoopAlpha());
            highLightImage.gameObject.SetActive(true);
        }
    }

    public void StopBlinking()
    {
        if (blinkCoroutine != null)
        {
            StopCoroutine(blinkCoroutine);
            blinkCoroutine = null;
            highLightImage.gameObject.SetActive(false);
        }
    }

    private IEnumerator LoopAlpha()
    {
        float duration = 1f;
        float minAlpha = 0f;
        float maxAlpha = 127f / 255f;

        while (true)
        {
            float time = Time.time % duration;
            float normalized = Mathf.Sin((time / duration) * Mathf.PI * 2f) * 0.5f + 0.5f;

            Color color = highLightImage.color;
            color.a = Mathf.Lerp(minAlpha, maxAlpha, normalized);
            highLightImage.color = color;

            yield return null;
        }
    }

    public void SetHideObject(int hideLevel)
    {
        HideObject[0].gameObject.SetActive(hideLevel < 2);
        HideObject[1].gameObject.SetActive(hideLevel < 3);
        HideObject[2].gameObject.SetActive(hideLevel < 4);

    }

}
