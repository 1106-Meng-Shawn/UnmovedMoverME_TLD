using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Burst.Intrinsics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;
using static FormatNumber;
using static GetColor;
using static GetSprite;
using static GetString;

public class CharacterColumnControl : MonoBehaviour, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler,IPointerUpHandler,IPointerEnterHandler
{
    public Button isStarButton;

    public Button CharacterRowButton;


    public Image charcterIcon;
    public Image stateIcon;
    public Image tagIcon;

    public TMP_Text charaterNameText;

    public Image roleIcon;

    public Image favorabilityImage;
    public TMP_Text favorabilityText;

    public Image ItemImage;

    public int forceType = 0;
    public Image forceImage;

    public Sprite negoNeedSpirte;
    public TMP_Text forceText;

  /*  public TMP_Text forceMaxText;
    public TMP_Text forceLimitText;*/

    public Image highLightImage;

    private Coroutine blinkCoroutine;
    private bool isBlinking = false;


    public Image attackIcon;
    public TextMeshProUGUI attackText;
    public Image defenseIcon;
    public TextMeshProUGUI defenseText;

    public  Image magicIcon;
    public TextMeshProUGUI magicText;

    public Image speedIcon;
    public TextMeshProUGUI speedText;

    public Image luckyIcon;
    public TextMeshProUGUI luckyText;


    public Image valueIcon;
    public int valueType = 0; // 0 is battleVaule, 1 is parameter value, 2 is HelpValue
    public List<Sprite> valueIconSprites;

    public List<Sprite> favorabilitySprites;



    public List<Sprite> battleValueSprite;
    public List<Sprite> ParameterValueSprite;
    public List<Sprite> HelpValueSprite;

    public Character character;
    public string characterName;

    /* public Color32 SelectColor;
     public Color32 canActiveColor;
     public Color32 cantActiveColor;*/


    [SerializeField] private Image column;

    private GameObject draggedIcon;
    private RegionInfo regionInfoUI;

    public int type; // 1 is normal. 2 is recruit
    public bool isSelected;

   // private float lastClickTime = 0f;
   // private const float doubleClickThreshold = 0.5f;

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
    private bool hasDragged = false;

    private Coroutine clickDelayCoroutine = null;
    private Coroutine holdForceCoroutine = null;

    [SerializeField] private bool enableHold = true;
    private Coroutine holdCoroutine = null;

    public Image PersonBattleImage;
    private Canvas uiCanvas;
    private bool endDragHandled = false;


    void OnEnable()
    {
        LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
        if (character != null)
        {
            character.OnCharacterChanged += UpdateCharacterPrefab;
        }
    }

    void OnDisable()
    {
        LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;
        character.OnCharacterChanged -= UpdateCharacterPrefab;
        isMouseOverPanel = false;
        RestoreColor();
        StopBlinking();

    }
    void OnDestroy()
    {
        CharacterPanelManage.Instance.RemoveCharacterRowControl(this);
        character.OnCharacterChanged -= UpdateCharacterPrefab;
        LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;

    }

    void Awake()
    {
    }



    private void Start()
    {
        SetRowColor();
        SetTheType(valueType);

//        if (type == 1) { regionInfoUI = FindObjectOfType<RegionInfoUI>(); }

        InitStar();
        uiCanvas = GeneralManager.Instance.uiCanvas;
//        CharacterRowButton.onClick.AddListener(() => SelectedCharacterRow());


    }



    void SetState()
    {
        if (stateIcon == null) return;
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
        SetRowColor();
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


    void InitStar()
    {
        UpStarButtonSprite();
        isStarButton.onClick.AddListener(() => SetIsStar());

    }

    public void SetRowColor()
    {
        if (type == 1)
        {
            if (CharacterPanelManage.Instance.characterRowControl == this)
            {
                column.color = GetRowColor(RowType.sel);
            }
            else
            {
                RestoreColor();

            }

        } else
        {
            SetRecruitColor();
        }

    }

    void SetRecruitColor()
    {
        if (isSelected)
        {
            column.color = GetRowColor(RowType.sel);
        }
        else
        {
            column.color = GetRowColor(RowType.canMove);

        }

    }

    public void SetItem(ItemBase itemBase)
    {
        if (type == 2) return;
        character.SetItem(itemBase);
        UpdateCharacterPrefab();
    }

    void UpStarButtonSprite()
    {
        if (isStarButton == null) return;
        string iconPath = $"MyDraw/UI/Other/";
        if (character.Star) { isStarButton.gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>(iconPath + "Star"); }
        else { isStarButton.gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>(iconPath + "Unstar"); }
    }

    void SetIsStar()
    {
        character.SetStar();
        UpStarButtonSprite();
    }


    public void CharacterColumnControlP(Character value)
    {
        if (value == null) Debug.Log("what happend at here");
        if (value != null)
        {
            character = value;
            character.OnCharacterChanged += UpdateCharacterPrefab;
            UpdateCharacterPrefab();
        }
    }


    public void UpdateCharacterPrefab()
    {
        UpStarButtonSprite();

        if (character.GetCountryENName() == CharacterConstants.Capture){
            charaterNameText.color = new Color32(240, 20, 20, 255);
            favorabilityImage.sprite = negoNeedSpirte;
            favorabilityText.text = GetColorString(character.RecruitCost.ToString("N0"), 2, 3);
        }else
        {
            favorabilityImage.sprite = GetFavorabilitySprite(character.FavorabilityLevel);
            favorabilityText.text = FormatNumberToString(character.Favorability);
        }


        if (character.HasItem()){
            ItemImage.sprite = character.GetItemWithCharacterSprite();
        }else {

            Sprite sprite = null;
            if (character.GetCountryENName() == CharacterConstants.Capture) { sprite = Resources.Load<Sprite>("MyDraw/Item/EmptyItemClose"); }
            else { sprite = Resources.Load<Sprite>("MyDraw/Item/EmptyItem"); }

           ItemImage.sprite = sprite;
        }
        SetItemIntro();

        SetCharacterName();


        PersonBattleImage.sprite = UpBattleButtonSprite(character.IsPersonBattle);

        forceImage.sprite = GetForceSprite(forceType);  
        ShowTheForceType(forceType);


        SetValueText();


        roleIcon.sprite = GetRoleSprite(character.RoleClass);
        SetRoleClassIntro();


        charcterIcon.sprite = character.icon;
        SetState();
        SetTag();
    }

    void SetTag()
    {
        if (tagIcon == null) return;


        if (character.Tag == 0)
        {
            tagIcon.gameObject.SetActive(false);
        } else
        {
            tagIcon.gameObject.SetActive(true);
            tagIcon.sprite = GetCharacterTag(character.Tag);
        }
    }

    private void OnLocaleChanged(Locale locale)
    {
        SetCharacterName(); // 你可以用 locale.Identifier 进一步做区分
        SetStateIntro();
        SetRoleClassIntro();
        SetItemIntro();
    }


    void SetCharacterName()
    {
        charaterNameText.text = character.GetCharacterName();

    }

    void SetRoleClassIntro()
    {
        if (roleIcon == null) return;
        roleIcon.gameObject.GetComponent<IntroPanelShow>().SetIntroName(character.GetClassRoleString());
    }

    void SetItemIntro()
    {
        if (ItemImage == null) return;
        IntroPanelShow introPanelShow = ItemImage.gameObject.GetComponent<IntroPanelShow>();
        if (character.HasItem())
        {
            ItemImage.gameObject.GetComponent<IntroPanelShow>().SetIntroName(character.GetItem().GetItemNameWithColorString());
        } else
        {
            ItemImage.gameObject.GetComponent<IntroPanelShow>().SetIntroName(null);

        }
    }



    public void SetTheType(int valueType){
        this.valueType = valueType;
        if (valueType == 0){
            SetValueIcon(battleValueSprite);
        } else if (valueType == 1){
            SetValueIcon(ParameterValueSprite);    
        }else if (valueType == 2){
            SetValueIcon(HelpValueSprite);
        }

        valueIcon.sprite = valueIconSprites[valueType];
        SetValueText();
    }

    public void SetTheForceType(int forceType){
        this.forceType = forceType;
        forceImage.sprite = GetForceSprite(forceType);  
        ShowTheForceType(forceType);
    }

    void ShowTheForceType(int forceType){
        if (forceType == 0){
            forceText.text = $"{character.Force.ToString()} / {character.MaxForce.ToString()}";
        } else if (forceType == 1){
            forceText.text = character.GetHealthAndMaxHealthString();
        } else if (forceType == 2){
            forceText.text = $"{GetMaxLimitString(character.GetMaxLimit())}";
        }

    }

    void SetValueIcon(List<Sprite> valueSprite){
    if (valueSprite.Count >= 5) {
        attackIcon.sprite = valueSprite[0];
        defenseIcon.sprite = valueSprite[1];
        magicIcon.sprite = valueSprite[2];
        speedIcon.sprite = valueSprite[3];
        luckyIcon.sprite = valueSprite[4];
    }
    }

    void SetValueText()
    {
        if (character == null) Debug.Log("why");
        attackText.text = GetColorString(FormatfloatNumber(character.GetValue(valueType, 0)), valueType, 0);
        defenseText.text = GetColorString(FormatfloatNumber(character.GetValue(valueType, 1)), valueType, 1);
        magicText.text = GetColorString(FormatfloatNumber(character.GetValue(valueType, 2)), valueType, 2);
        speedText.text = GetColorString(FormatfloatNumber(character.GetValue(valueType, 3)), valueType, 3);
        luckyText.text = GetColorString(FormatfloatNumber(character.GetValue(valueType, 4)), valueType, 4);

    }



    public void OnPointerClick(PointerEventData eventData)
    {

    }

    private void HandleDoubleClick()
    {
     /*   if (regionInfoUI == null || regionInfoUI.regionCoutyIcon == null) return;
        regionInfoUI.setLord(character);*/

    }



    private void HandleSingleClick()
    {

        if (CharacterPanelManage.Instance.characterRowControl != this)
        {
            CharacterPanelManage.Instance.SetCharacterColumnControl(this);

        }
        else
        {
            CharacterPanelManage.Instance.SetCharacterColumnControl(null);

        }

    }


    private bool isMouseOverPanel;
    public void OnPointerEnter(PointerEventData eventData)
    {
        SoundManager.Instance.PlaySelSFX();
        isMouseOverPanel = true;
        column.color = GetRowColor(RowType.sel);
     //   ChangeColorOnClick();
    }



    public void OnPointerExit(PointerEventData eventData)
    {
        // if(type == 1)
        isMouseOverPanel = false;
        SetRowColor();

    }

    void ChangeColorOnClick()
    {
        if (column != null)
        {
            column.color = new Color(column.color.r * 0.7f, column.color.g * 0.7f, column.color.b * 0.7f, column.color.a);
        }
    }

    public void RestoreColor()
    {
        if (isMouseOverPanel)
        {
            column.color = GetRowColor(RowType.sel);
            return;
        }

        if (type == 1)
        {
            if (character.CanMove())
            {
                column.color = GetRowColor(RowType.canMove);
            }
            else
            {
                column.color = GetRowColor(RowType.cantMove);
            }
        }
        else
        {
            SetRecruitColor();
        }
    }


    public void OnDrag(PointerEventData eventData)
    {
        if (type == 2) return;

        if (draggedIcon != null)
        {
            RectTransform draggedIconRect = draggedIcon.GetComponent<RectTransform>();
            RectTransform parentRect = draggedIcon.transform.parent.GetComponent<RectTransform>();

            Vector2 localPoint;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parentRect,
                eventData.position,
                uiCanvas.worldCamera,
                out localPoint))
            {
                draggedIconRect.localPosition = localPoint;
                draggedIconRect.sizeDelta = new Vector2 (125, 125);
                draggedIconRect.localScale = new Vector3(1,1,1);
            }
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

        if (eventData.pointerCurrentRaycast.gameObject == null) yield break;
        if (type == 2) yield break;


        Debug.Log($"{eventData.pointerCurrentRaycast.gameObject.name}");
        if (eventData.pointerCurrentRaycast.gameObject == RegionInfo.Instance.lordBackgroundImage.gameObject)
        {
            // regionInfoUI.setLord(character);
            RegionInfo.Instance.SetLord(character);
            endDragHandled = true;
            yield break;
        }

        if (eventData.pointerCurrentRaycast.gameObject == CharacterPanelManage.Instance.characterBackground.gameObject)
        {
            CharacterPanelManage.Instance.SetCharacterColumnControl(this);
            endDragHandled = true;
            yield break;
        }

        RegionColumControl regionColumControl = eventData.pointerCurrentRaycast.gameObject.GetComponent<RegionColumControl>();

        if (regionColumControl != null && !character.IsMoved)
        {
            regionColumControl.GetRegionValue().SetLord(character);
            endDragHandled = true;
            yield break;
        }

        endDragHandled = false;

    }

    public bool GetEndDragHandled()
    {
        return endDragHandled;
    }

    public void SetEndDragHandled(bool newEndDragHandled)
    {
       endDragHandled = newEndDragHandled;
    }



    public void Highlight(bool on)
    {
        if (type == 2) return; // un sure yet

        if (on)
        {
            StartBlinking(); // ?????????
        }
        else
        {
            StopBlinking(); // ????
        }
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
        float duration = 1f; // 
        float minAlpha = 0f;
        float maxAlpha = 127f / 255f;

        while (true)
        {
            float time = Time.time % duration;
            float normalized = Mathf.Sin((time / duration) * Mathf.PI * 2f) * 0.5f + 0.5f;

            float alpha = Mathf.Lerp(minAlpha, maxAlpha, normalized);

            Color color = highLightImage.color;
            color.a = alpha;
            highLightImage.color = color;

            yield return null;
        }
    }

    #region event point
    // --- Pointer Events ---

    public void OnBeginDrag(PointerEventData eventData)
    {
        hasDragged = true;
        endDragHandled = false;
        CeartDraggedIcon();
    }


    public void OnPointerDown(PointerEventData eventData)
    {
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
       // Destroy(draggedIcon);
        hasDragged = false;
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
            HandleSingleClickByType();

        clickDelayCoroutine = null;
    }

    private IEnumerator HoldForceIncreaseRoutine()
    {
        currentHoldTime = 0f;
        accumulatedForce = 0f;

        while (isHolding)
        {
            float t = currentHoldTime;
            float rawIncrease = forceIncreaseRate * (t + 0.3f) * t * 10f ;

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

    #endregion

    #region Click Type Dispatcher

    private void HandleSingleClickByType()
    {
        if (type == 1)
            HandleSingleClick_Type1();
        else if (type == 2)
            HandleSingleClick_Type2();
    }

    private void HandleDoubleClickByType()
    {
        if (type == 1)
        {
            if (forceType != 0) return;
            FillForceToMax();
        }
        else if (type == 2) HandleDoubleClickByType2();
    }

    private void HandleDoubleClickByType2()
    {
        if (GameValue.Instance.GetResourceValue().Negotiation < character.RecruitCost)
        {
            NotificationManage.Instance.ShowToTopByKey(NotificationKeyConstants.NotEnoughResource, GetResourceStringWithSprite(ValueType.Negotiation));
            CharacterAssistRowControl.Instance.ToggleRow(SBNType.Negotiation, character.RecruitCost);
        }
        else
        {
            var manager = RecruitPanelManage.Instance;
            GameValue.Instance.GetResourceValue().Negotiation -= character.RecruitCost;
            character.SetCountry(GameValue.Instance.GetPlayerCountryENName());
            manager.RemoveCharacterRow(this);
        }
    }


    private void HandleHoldByType()
    {
        if (forceType != 0) return;
        if (type == 1)
            holdForceCoroutine = StartCoroutine(HoldForceIncreaseRoutine());
        // else if (type == 2) Debug.Log("长按类型2 - 待定义");
    }

    #endregion

    #region Click Logic

    private void HandleSingleClick_Type1()
    {
        if (CharacterPanelManage.Instance.characterRowControl != this)
            CharacterPanelManage.Instance.SetCharacterColumnControl(this);
        else
            CharacterPanelManage.Instance.SetCharacterColumnControl(null);
    }

    private void HandleSingleClick_Type2()
    {
        isSelected = !isSelected;

        if (isSelected)
        {
            RecruitPanelManage.Instance.SetCharacterColumnControl(this);
          //  characterPanel.selectedCharacterColumnControlList.Add(this);
            column.color = GetRowColor(RowType.sel);
        }
        else
        {
            RecruitPanelManage.Instance.SetCharacterColumnControl(this);
            SetRowColor();
        }
    }

    #endregion

    #region Force & Drag Logic

    void ApplyForceIncrease(int increase)
    {
        if (hasDragged) return;
        int availableForce = Mathf.Min(increase,
            GameValue.Instance.GetResourceValue().TotalRecruitedPopulation,
            character.MaxForce - character.Force);

        if (character.MaxForce - character.Force == 0)
        {
            //NotificationManage.Instance.ShowAtTop("Your force are full");
            NotificationManage.Instance.ShowAtTopByKey(NotificationKeyConstants.Character_ForceFull,character.GetCharacterName());
        }
        else if (GameValue.Instance.GetResourceValue().TotalRecruitedPopulation == 0) {
            //NotificationManage.Instance.ShowAtTop("You don't have enough force.");
            NotificationManage.Instance.ShowAtTopByKey(NotificationKeyConstants.NotEnoughResource, GetResourceStringWithSprite(ValueType.TotalRecruitedPopulation));

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
            NotificationManage.Instance.ShowToTopByKey(NotificationKeyConstants.NotEnoughResource, GetResourceStringWithSprite(ValueType.TotalRecruitedPopulation));

        }


        if (available > 0)
        {
            GameValue.Instance.GetResourceValue().TotalRecruitedPopulation -= available;
            character.Force += available;
            //NotificationManage.Instance.ShowAtTop($"Your Add {available}");
            NotificationManage.Instance.ShowAtTopByKey(NotificationKeyConstants.Character_AddForce, character.GetCharacterName(),available.ToString());

        }
    }

    void CeartDraggedIcon()
    {
        if (type == 2) return;

        if (character.IsMoved)
        {
            // NotificationManage.Instance.ShowAtTop("character is moved");
            NotificationManage.Instance.ShowAtTopByKey(NotificationKeyConstants.Character_Moved);
            return;
        }

        if (character != null && draggedIcon == null)
        {
            draggedIcon = new GameObject("DraggedIcon");
            draggedIcon.transform.SetParent(transform.root);
            Image draggedImage = draggedIcon.AddComponent<Image>();
            draggedImage.sprite = character.icon;
            draggedImage.raycastTarget = true;

            draggedImage.rectTransform.sizeDelta = new Vector2(0.5f, 0.5f);
            RectTransform rt = draggedImage.rectTransform;
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.position = Input.mousePosition + new Vector3(20, 20, 0);
        }
    }

    bool CanHold()
    {
        return GameValue.Instance.GetResourceValue().TotalRecruitedPopulation > 0;
    }

    #endregion
}