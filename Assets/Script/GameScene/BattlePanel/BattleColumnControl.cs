using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.TextCore.Text;
using System;
using static GetSprite;
using static GetColor;
using static GetString;

using UnityEngine.Localization.Settings;
using Unity.VisualScripting;
public class BattleColumnControl : MonoBehaviour, IDragHandler, IBeginDragHandler,IEndDragHandler, IPointerDownHandler, IPointerUpHandler,IPointerEnterHandler,IPointerExitHandler

{
    public Button battleRowButton;
    public Image hightImage;

    public Image charcterIcon;
    public Image stateIcon;
    public Image tagIcon;


    public Image roleIcon;

    public Image itemIcon;
    public Sprite itemIconOriginal;


    public TMP_Text charaterNameText;
    public TMP_Text MoveNumText;
    [SerializeField] private Transform moveNumTransform;
    [SerializeField] private GameObject movePrefab;


    /*  
     *      public Image forceUPImage;
       public Image forceDownImage;
     public TMP_Text forceText;
       public TMP_Text forceDownText;*/
    public Image forceIcon;
    public TextMeshProUGUI forceText;



    public TMP_Text attackText;
    public TMP_Text defenseText;
    public TMP_Text magicText;
    public TMP_Text speedText;
    public TMP_Text luckyText;


    public Character character;
    public string characterName;



    private Color originalColor;
    public Image column;

    private GameObject draggedIcon;

    public bool isExplore;
    public bool isInExplore;
    public BattleCharacterValue battleCharacterValue;

   /* public Sprite scoutSprite;
    public Sprite charmSprite;*/


    public Image favorabilityImage;
    public TMP_Text favorabilityText;

    public List<Sprite> favorabilityTypeSprite;

    public Button starButton;
    private bool isMouseOverPanel;

    private Coroutine blinkCoroutine;
    private bool shouldBlinkOnEnable = false;

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
    private bool hasDragged = false;

    private bool isScout;// need to big change
    public Image PersonBattleType;
    private Canvas uiCanvas;

    private void OnEnable()
    {
        LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;

        if (shouldBlinkOnEnable && blinkCoroutine == null)
        {
            blinkCoroutine = StartCoroutine(LoopAlpha());
            hightImage.gameObject.SetActive(true);
        }
    }


    private void OnDisable()
    {
        LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;
    }


    private void Start()
    {
        column = GetComponent<Image>();
        originalColor = column.color;
        InitStar();
        uiCanvas = GeneralManager.Instance.uiCanvas;
       // battleRowButton.onClick.AddListener(OnBattleRowButtonClick);

    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        hasDragged = true;
        CreateDraggedIcon();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //if (!CanHold()) return;

        if (hasDragged) return;
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
        BattlePanelManage.Instance.SetCharacterToPanel(character);
    }


    private void HandleDoubleClickByType()
    {

        if (BattlePanelManage.Instance.isExplore || BattlePanelManage.Instance.isInExplore)
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
        if (BattlePanelManage.Instance.isExplore || BattlePanelManage.Instance.isInExplore)
        {

        } else
        {
            holdForceCoroutine = StartCoroutine(HoldForceIncreaseRoutine());
        }
        // else if (type == 2) Debug.Log("长按类型2 - 待定义");
    }

    void ApplyForceIncrease(int increase)
    {
        if (hasDragged) return;
        int availableForce = Mathf.Min(increase,
            GameValue.Instance.GetResourceValue().TotalRecruitedPopulation,
            character.MaxForce - character.Force);

        if (character.MaxForce - character.Force == 0)
        {
            NotificationManage.Instance.ShowToTopByKey(NotificationKeyConstants.Character_ForceFull, character.GetCharacterName());
        }
        else if (GameValue.Instance.GetResourceValue().TotalRecruitedPopulation == 0)
        {
            //NotificationManage.Instance.ShowAtTop("You don't have enough force.");
            NotificationManage.Instance.ShowAtTopByKey(NotificationKeyConstants.NotEnoughResource,GetResourceStringWithSprite(ValueType.TotalRecruitedPopulation));
        }

        if (availableForce > 0)
        {
            character.Force += availableForce;
            GameValue.Instance.GetResourceValue().TotalRecruitedPopulation -= availableForce;
        }
    }

    void FillForceToMax()
    {
        int needed = character.MaxForce - character.Force;
        int available = Mathf.Min(needed, GameValue.Instance.GetResourceValue().TotalRecruitedPopulation);

        if (needed == 0) {
            //NotificationManage.Instance.ShowToTop("Your force are full");
            NotificationManage.Instance.ShowToTopByKey(NotificationKeyConstants.Character_ForceFull,character.GetCharacterName());

        }
        else if (GameValue.Instance.GetResourceValue().TotalRecruitedPopulation == 0) {
            //NotificationManage.Instance.ShowToTop("You don't have enough force.");
            NotificationManage.Instance.ShowToTopByKey(NotificationKeyConstants.NotEnoughResource,GetResourceStringWithSprite(ValueType.TotalRecruitedPopulation));
        }


        if (available > 0)
        {
            GameValue.Instance.GetResourceValue().TotalRecruitedPopulation -= available;
            character.Force += available;
            //NotificationManage.Instance.ShowAtTop($"Your Add {available}");
            NotificationManage.Instance.ShowAtTopByKey(NotificationKeyConstants.Resource_Add,GetResourceStringWithSprite(ValueType.TotalRecruitedPopulation),available.ToString());

        }
    }

    void CreateDraggedIcon()
    {

        if (!isInExplore)
        {
            if (!character.CheckCanMove()) return;
        }



        if (character != null && draggedIcon == null)
        {
            draggedIcon = new GameObject("DraggedIcon");
            draggedIcon.transform.SetParent(transform.root);
            Image draggedImage = draggedIcon.AddComponent<Image>();
            draggedImage.sprite = character.icon;
            draggedImage.raycastTarget = false;

            draggedImage.rectTransform.sizeDelta = new Vector2(0.5f, 0.5f);
            RectTransform rt = draggedImage.rectTransform;
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.position = Input.mousePosition + new Vector3(20, 20, 0);
        }
    }

    private void OnDestroy()
    {
        if (character != null)
            character.OnCharacterChanged -= RefreshUI;
    }


    private void RefreshUI()
    {
        UpdateCharacterPrefab(character);
    }



    public void Init(Character value)
    {
        this.character = value;
        isInExplore = BattlePanelManage.Instance.isInExplore;
        UpdateCharacterPrefab(value);
        character.OnCharacterChanged += RefreshUI;

    }

    private void OnLocaleChanged(UnityEngine.Localization.Locale newLocale)
    {
        SetCharacterName();
        SetStateIntro();
        SetRoleClassIntro();
    }


    void SetCharacterName()
    {
        charaterNameText.text = character.GetCharacterName();

    }

    public void UpdateCharacterPrefab(Character character)
    {

        if (character == null)return;

        UpStarButtonSprite();

        UpdateRowColor();

        SetCharacterName();
        CreateMoveNumPrefab(character.BattleMoveNum);

        bool isExplore = character.IsPersonBattle || BattlePanelManage.Instance.IsPersonBattleInBattlePanel();

        moveNumTransform.gameObject.SetActive(!isExplore);
        PersonBattleType.sprite = UpBattleButtonSprite(isExplore);
        if (isExplore) {
            forceIcon.sprite = Resources.Load<Sprite>("MyDraw/UI/CharacterValueIcon/Health");
            forceText.text = character.GetHealthAndMaxHealthString();
        }
        else {
            forceIcon.sprite = Resources.Load<Sprite>("MyDraw/UI/CharacterValueIcon/Force");
            forceText.text = GetValueColorString($"{character.Force.ToString()} / {character.MaxForce.ToString()}", ValueColorType.Pop);
            }

        if (isScout) {
            forceIcon.sprite = Resources.Load<Sprite>("MyDraw/UI/CharacterValueIcon/scoutAndCharm");
            forceText.text = $"{ GetColorString(FormatNumber(character.GetValue(2, 1)),2,1)}+{GetColorString(FormatfloatNumber((float)character.GetValue(2, 4) / 10),2,4)}";
            PersonBattleType.sprite = GetValueSprite("scout");

        }

        favorabilityImage.sprite = GetFavorabilitySprite(character.FavorabilityLevel);
        favorabilityText.text = FormatNumber(character.Favorability);

        attackText.text = GetColorString(FormatfloatNumber(character.GetValue(0, 0)), 0, 0);
        defenseText.text = GetColorString(FormatfloatNumber(character.GetValue(0, 1)),0,1);
        magicText.text = GetColorString(FormatfloatNumber(character.GetValue(0, 2)),0,2);
        speedText.text = GetColorString(FormatfloatNumber(character.GetValue(0, 3)),0,3);
        luckyText.text = GetColorString(FormatfloatNumber(character.GetValue(0, 4)),0,4);

        charcterIcon.sprite = character.icon;

        roleIcon.sprite = GetRoleSprite(character.RoleClass);
        SetRoleClassIntro();

        itemIcon.sprite = character.GetItemWithCharacterSprite();


        SetItemIntro();


        SetState();
        SetTag();
    }

    void SetState()
    {
        if (isInExplore)
        {
            stateIcon.gameObject.SetActive(false);
            return;
        }


        stateIcon.gameObject.SetActive(true);
        if (character.IsMoved)
        {
            stateIcon.sprite = GetCharacterStateSprite("cantmove");
        }
        else if (character.HasLord())
        {
            stateIcon.sprite = GetCharacterStateSprite("lord");
        }
        else
        {
            stateIcon.gameObject.SetActive(false);
        }
    }

    void SetItemIntro()
    {
        IntroPanelShow introPanelShow = itemIcon.gameObject.GetComponent<IntroPanelShow>();
        if (character.HasItem())
        {
            itemIcon.gameObject.GetComponent<IntroPanelShow>().SetIntroName(character.GetItem().GetItemNameWithColorString());
        }
        else
        {
            itemIcon.gameObject.GetComponent<IntroPanelShow>().SetIntroName(null);

        }
    }

    void SetTag()
    {
        if (character.Tag == 0)
        {
            tagIcon.gameObject.SetActive(false);
        } else
        {
            tagIcon.gameObject.SetActive(true);
            tagIcon.sprite = GetCharacterTag(character.Tag);
        }
    }

    public void CreateMoveNumPrefab(int count)
    {
        foreach (Transform child in moveNumTransform)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < CharacterConstants.GetMaxMoveShowCount(count); i++)
        {
            GameObject obj = Instantiate(movePrefab, moveNumTransform);
            obj.transform.localScale = Vector3.one; // 保持缩放一致
        }
    }

    public void UpdateRowColor()
    {
        if (ShouldGray())
        {
            column.color = GetRowColor(RowType.cantMove);
        }
        else if (IsSelected())
        {
            column.color = GetRowColor(RowType.sel);
        }
        else
        {
            column.color = GetRowColor(RowType.canMove);
        }
        SetStateIntro();
     }

    void SetStateIntro()
    {
        if (character.IsMoved)
        {
            stateIcon.gameObject.GetComponent<IntroPanelShow>().SetIntroName("Moved");
        }
        else if (character.HasLord())
        {
            stateIcon.gameObject.GetComponent<IntroPanelShow>().SetIntroName(character.GetLordRegion(0).GetRegionNameWithColor());
        }
        else
        {
            stateIcon.gameObject.GetComponent<IntroPanelShow>().SetIntroName(null);
        }

    }

    void SetRoleClassIntro()
    {
        roleIcon.gameObject.GetComponent<IntroPanelShow>().SetIntroName(character.GetClassRoleString());
    }



    public void SetIsScout(bool isScout)
    {
        this.isScout = isScout;
        UpdateCharacterPrefab(this.character);
    }

    private bool IsUnavailable()
    {
        if (isInExplore)
        {
            return character.IsAtBattlePosition();
        }

        return !character.CanMove();
    }

    private bool IsSelected()
    {
        return BattlePanelManage.Instance.selBattleColumnControl == this || character.IsAtBattlePosition();
    }

    private bool ShouldGray()
    {
        return IsUnavailable() && !IsSelected();
    }


    void InitStar()
    {
        UpStarButtonSprite();
        starButton.onClick.AddListener(() => SetIsStar());

    }


    void UpStarButtonSprite()
    {
        string iconPath = $"MyDraw/UI/Other/";
        if (character.Star) { starButton.gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>(iconPath + "Star"); }
        else { starButton.gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>(iconPath + "Unstar"); }
    }



    void SetIsStar()
    {
        character.SetStar();
        UpStarButtonSprite();

    }

    string FormatfloatNumber(float value)
    {
        if (value * 10 % 10 == 0)
        {
            return value.ToString("F0");
        }
        else
        {
            return value.ToString("F1");
        }
    }


    public void RestoreColor()
    {

        if (character.IsAtBattlePosition() || isMouseOverPanel)
        {
            column.color = GetRowColor(RowType.sel);
        }
        else if (!character.CanMove() && !BattlePanelManage.Instance.isInExplore)
        {
            column.color = GetRowColor(RowType.cantMove);
        }
        else
        {
            column.color = GetRowColor(RowType.canMove);
        }
        StopBlinking();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        SoundManager.Instance.PlaySelSFX();
        isMouseOverPanel = true;
        column.color = GetRowColor(RowType.sel);
    }



    public void OnPointerExit(PointerEventData eventData)
    {
        // if(type == 1)
        isMouseOverPanel = false;
        UpdateRowColor();

    }

    public void OnDrag(PointerEventData eventData)
    {

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
            draggedIconRect.sizeDelta = new Vector2(125, 125);
            draggedIconRect.localScale = new Vector3(1f,1f,1f);
            hasDragged = true;
        }
    }


    public void OnEndDrag(PointerEventData eventData)
    {
        hasDragged = false;
        StartCoroutine(HandleEndDrag(eventData));
    }

    private IEnumerator HandleEndDrag(PointerEventData eventData)
    {
        Destroy(draggedIcon);
        draggedIcon = null;

        yield return null; // 等待一帧，确保销毁完成

        if (eventData.pointerCurrentRaycast.gameObject != null)
        {

        }


        if (!(!isInExplore && (character.IsMoved || character.HasLord()))) {

            GameObject hitObject = eventData.pointerCurrentRaycast.gameObject;


            BattlePosition battlePosition = hitObject.GetComponent<BattlePosition>();

            if (battlePosition != null)
            {
                if (!battlePosition.isEnemy)
                {
                    battlePosition.SetCharacterToPosition(character);


                    if (isInExplore)
                    {
                        BattleCharacterValue battleCharacterValue = gameObject.GetComponent<BattleCharacterValue>();
                        battlePosition.battleCharacterValue = battleCharacterValue;
                    }
                }

            }
            UpdateRowColor();



        }

    }


    string FormatNumber(float value)
    {
        return value.ToString("N0");
    }


    public void Highlight(bool on)
    {
        if (on)
        {
            StartBlinking(); // 你之前写的高亮逻辑
        }
        else
        {
            StopBlinking(); // 停止高亮
        }
    }
    public void StartBlinking()
    {
        shouldBlinkOnEnable = true;

        if (blinkCoroutine == null)
        {
            if (gameObject.activeSelf) blinkCoroutine = StartCoroutine(LoopAlpha());
            hightImage.gameObject.SetActive(true);
        }
    }

    public void StopBlinking()
    {
        shouldBlinkOnEnable = false;

        if (blinkCoroutine != null)
        {
            StopCoroutine(blinkCoroutine);
            blinkCoroutine = null;
            hightImage.gameObject.SetActive(false);
        }
    }


    private IEnumerator LoopAlpha()
    {
        float duration = 1f; // 一个完整周期
        float minAlpha = 0f;
        float maxAlpha = 127f / 255f;

        while (true)
        {
            float time = Time.time % duration;
            float normalized = Mathf.Sin((time / duration) * Mathf.PI * 2f) * 0.5f + 0.5f;

            float alpha = Mathf.Lerp(minAlpha, maxAlpha, normalized);

            Color color = hightImage.color;
            color.a = alpha;
            hightImage.color = color;

            yield return null;
        }
    }



}
