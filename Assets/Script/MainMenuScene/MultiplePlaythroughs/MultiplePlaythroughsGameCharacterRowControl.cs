using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;
using static FormatNumber;
using static GetColor;
using static GetSprite;
using static GetString;


public class MultiplePlaythroughsGameCharacterRowControl : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler, IPointerUpHandler, IPointerDownHandler,IPointerClickHandler
{
    [SerializeField] private Button isStarButton;

    [SerializeField] private Button CharacterRowButton;

    [SerializeField] private Image characterIcon;
    [SerializeField] private Image tagIcon;

    [Header("Name and Move ")]
    public NameAndMove nameAndMove = new();

    [System.Serializable]
    public struct NameAndMove
    {
        
        public TMP_Text characterNameText;
        public TMP_Text MoveText;

        public SpecialButton MovePlusButton;
        public SpecialButton MoveMinsButton;
    }

    [SerializeField] private Image roleIcon;

    [Header("Favorability")]
    public Favorability favorability = new();

    [System.Serializable]
    public struct Favorability
    {
        public Button favorabilityButton;
        public TMP_Text favorabilityText;
        public SpecialButton favorabilityPlusButton;
        public SpecialButton favorabilityMinusButton;
    }

    [SerializeField] private Image ItemImage;

    private int forceType = 2;

    #region Force
    [Header("MaxLimit")]
    public MaxLimit maxLimit = new();

    [System.Serializable]
    public struct MaxLimit
    {
        public GameObject MaxLimitOj;

        public TextMeshProUGUI MaxLimitText;
        public TextMeshProUGUI LeaderText;
        public TextMeshProUGUI CharmText;

    }

    [Header("Level And Health")]
    public LevelAndHealth levelAndHealth = new();

    [System.Serializable]
    public struct LevelAndHealth
    {
        public GameObject LevelAndHealthOj;
        public SpecialButton LevelMinusButton;
        public TextMeshProUGUI LevelText;
        public SpecialButton LevelPlusButton;

        public SpecialButton ExperienceMinusButton;
        public TextMeshProUGUI ExperienceText;
        public Slider ExperienceSlider;
        public SpecialButton ExperiencePlusButton;

        public TextMeshProUGUI HealthText;
    }

    [Header("Force")]
    public Force force = new();

    [System.Serializable]
    public struct Force
    {
        public GameObject ForceOj;
        public TextMeshProUGUI ForceText;
        public Slider ForceChangeSlider;

        public TextMeshProUGUI MaxForceText;
        public Slider MaxForceChangeSlider;
    }

    #endregion




    [System.Serializable]
    public struct AttributeUI
    {
        public ValueType type; // ?????? "Attack"
        public TextMeshProUGUI valueText;
        public SpecialButton plusButton;
        public SpecialButton minusButton;
    }

    [Header("Character Attributes")]
    public List<AttributeUI> attributes = new();


    [SerializeField] private Image valueIcon;
    [SerializeField] private int valueType = 0; // 0 is battleVaule, 1 is parameter value, 2 is HelpValue
    [SerializeField] public MultiplePlaythroughsGameCharacterRowControlData characterData;

    [SerializeField] private Image column;
    [SerializeField] private Image PersonBattleImage;

    bool isMouseOverPanel = false;


    void OnDisable()
    {
        isMouseOverPanel = false;
        RestoreColor();

    }

    void Awake()
    {
        LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
    }

    private void Start()
    {
        SetRowColor();
        SetType(valueType);
        InitStar();
        InitButtons();

    }

    void OnDestroy()
    {
        LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;
        characterData.OnCharacterRowControlDataChanged -= UpdateCharacterPrefab;

    }


    public void Init(Character character)
    {
        MultiplePlaythroughsGameCharacterRowControlData characterRowControlData = new MultiplePlaythroughsGameCharacterRowControlData(character);
        characterData = characterRowControlData;
        characterData.OnCharacterRowControlDataChanged += UpdateCharacterPrefab;
        UpdateCharacterPrefab();
    }


    void SetState()
    {
        SetRowColor();
    }


    void InitStar()
    {
        UpStarButtonSprite();
        isStarButton.onClick.AddListener(OnStarButtonClick);

    }

    void OnStarButtonClick()
    {
        SetIsStar();
    }


    public void SetRowColor()
    {
        column.color = GetRowColor(RowType.player);
    }


    public void SetItem(ItemBase itemBase)
    {
        characterData.ItemID =  itemBase.GetID();
        UpdateCharacterPrefab();
    }

    void UpStarButtonSprite()
    {
        if (isStarButton == null) return;
        string iconPath = $"MyDraw/UI/Other/";
        if (characterData.Star) { isStarButton.gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>(iconPath + "Star"); }
        else { isStarButton.gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>(iconPath + "Unstar"); }
    }

    void SetIsStar()
    {
        characterData.Star = !characterData.Star;
        UpStarButtonSprite();
    }


    public void UpdateCharacterPrefab()
    {
        UpStarButtonSprite();

        favorability.favorabilityButton.image.sprite = GetFavorabilitySprite(characterData.FavorabilityLevel);
        favorability.favorabilityText.text = GetFavorabilityString(characterData.Favorability);

        force.ForceText.text = GetValueColorString($"{characterData.Force}", ValueColorType.Pop);
        force.MaxForceText.text = GetValueColorString($"{characterData.MaxForce}", ValueColorType.Pop);


        if (characterData.HasItem())
        {
            ItemImage.sprite = characterData.GetItemWithCharacterSprite();
        }
        else
        {

            Sprite sprite = null;
            sprite = Resources.Load<Sprite>("MyDraw/Item/EmptyItem"); 

            ItemImage.sprite = sprite;
        }
        SetItemIntro();
        SetCharacterName();

        nameAndMove.MoveText.text = GetMoveNumString(characterData.BattleMoveNum);

        PersonBattleImage.sprite = UpBattleButtonSprite(characterData.IsPersonBattle);
        ShowForceType(forceType);
        roleIcon.sprite = GetRoleSprite(characterData.RoleClass);
        SetRoleClassIntro();
        characterIcon.sprite = characterData.GetIcon();
        SetState();
        SetTag();
        SetValueText();
        ShowMaxLimitText();
        ShowLevelAndHealthText();
    }

    void SetTag()
    {
        if (tagIcon == null) return;


        if (characterData.Tag == 0)
        {
            tagIcon.gameObject.SetActive(false);
        }
        else
        {
            tagIcon.gameObject.SetActive(true);
            tagIcon.sprite = GetCharacterTag(characterData.Tag);
        }
    }

    private void OnLocaleChanged(Locale locale)
    {
        SetCharacterName(); 
        SetRoleClassIntro();
        SetItemIntro();
    }


    void SetCharacterName()
    {
        nameAndMove.characterNameText.text = characterData.GetCharacterName();
    }

    void SetRoleClassIntro()
    {
        if (roleIcon == null) return;
        roleIcon.gameObject.GetComponent<IntroPanelShow>().SetIntroName(characterData.GetClassRoleString());
    }

    void SetItemIntro()
    {
        if (ItemImage == null) return;
        IntroPanelShow introPanelShow = ItemImage.gameObject.GetComponent<IntroPanelShow>();
        if (characterData.HasItem())
        {
            ItemImage.gameObject.GetComponent<IntroPanelShow>().SetIntroName(characterData.GetItem().GetItemNameWithColorString());
        }
        else
        {
            ItemImage.gameObject.GetComponent<IntroPanelShow>().SetIntroName(null);

        }
    }


    public void SetType(int valueType)
    {
        this.valueType = valueType;
        //SetValueIcon();
        valueIcon.sprite = GetValueTypeSprite(valueType);
        SetValueText();
    }


    void ShowMaxLimitText()
    {
        maxLimit.MaxLimitText.text = GetMaxLimitString(characterData.GetMaxLimit());
        maxLimit.LeaderText.text = GetValueColorString(characterData.GetValue(2, 0).ToString(), ValueColorType.Leadership);
        maxLimit.CharmText.text = GetValueColorString(characterData.GetValue(2, 4).ToString(), ValueColorType.Charm);
    }

    void ShowLevelAndHealthText()
    {
        levelAndHealth.LevelText.text = characterData.GetLvAndMaxLevelString();
        levelAndHealth.ExperienceText.text = characterData.GetExpAndReqExpString();
        levelAndHealth.HealthText.text = characterData.GetHealthAndMaxHealthString();
        levelAndHealth.ExperienceSlider.value = characterData.GetExpRate();
    }

    //void ShowMaxLimitText()
    //{
    //    maxLimit.MaxLimitText.text = GetMaxLimitString(characterData.GetMaxLimit());
    //    maxLimit.LeaderText.text = GetValueColorString(characterData.GetValue(2, 0).ToString(), ValueColorType.Leadership);
    //    maxLimit.CharmText.text = GetValueColorString(characterData.GetValue(2, 4).ToString(), ValueColorType.Charm);
    //}


    void OnForceSliderChanged(float value)
    {

        if (forceType == 0)
        {
            if (characterData.MaxForce == 1)
            {
                // ??????????????1?
                force.ForceChangeSlider.value = force.ForceChangeSlider.maxValue; // ?? .value = 1
                force.ForceText.text = GetValueColorString("1", ValueColorType.Pop);
                return;
            }

            int newForce = CalculateNewForce(value);
            int needTroops = newForce - characterData.Force;


            if (newForce == 1 && characterData.Force != 1)
                 NotificationManage.Instance.ShowAtTopByKey(NotificationKeyConstants.Force_Minimum);
            else if (newForce >= characterData.MaxForce && characterData.Force < characterData.MaxForce)
                 NotificationManage.Instance.ShowAtTopByKey(NotificationKeyConstants.Force_Max, characterData.GetCharacterName());
            else if (needTroops > 0 && GameValue.Instance.GetResourceValue().TotalRecruitedPopulation < needTroops)
                 NotificationManage.Instance.ShowAtTopByKey(NotificationKeyConstants.NotEnoughResource, GetResourceStringWithSprite(ValueType.TotalRecruitedPopulation));

            GameValue.Instance.GetResourceValue().TotalRecruitedPopulation -= needTroops;
            characterData.Force = newForce;
            force.ForceText.text = GetValueColorString($"{newForce}", ValueColorType.Pop);
        }
    }

    void OnMaxSliderChanged(float value)
    {
        if (forceType == 0)
        {
            int newMaxForce = CalculateNewMaxForce(value);
            int requiredGold = (newMaxForce - characterData.MaxForce) * 100;

            if (newMaxForce == 1 && characterData.MaxForce != 1)
                NotificationManage.Instance.ShowAtTopByKey(NotificationKeyConstants.Force_Minimum);
            //else if (newMaxForce >= 9999 && characterData.MaxForce != 9999)
            //    NotificationManage.Instance.ShowAtTopByKey(NotificationKeyConstants.Force_Max);
            else if (newMaxForce >= characterData.GetMaxLimit() && characterData.MaxForce < characterData.GetMaxLimit())
                NotificationManage.Instance.ShowAtTopByKey(NotificationKeyConstants.Character_LimitReached);
            else if (requiredGold > 0 && GameValue.Instance.GetResourceValue().Gold < requiredGold)
                NotificationManage.Instance.ShowAtTopByKey(NotificationKeyConstants.NotEnoughResource, GetResourceStringWithSprite(ValueType.Gold));


            //   GameValue.Instance.GetResourceValue().Gold -= requiredGold; change to Achievement Points

            if (newMaxForce < characterData.Force)
            {
               // GameValue.Instance.GetResourceValue().TotalRecruitedPopulation += (characterData.Force - newMaxForce);
                characterData.Force = newMaxForce;
                force.ForceText.text = GetValueColorString($"{newMaxForce}", ValueColorType.Pop);
                force.ForceChangeSlider.SetValueWithoutNotify((float)newMaxForce / newMaxForce);
            }

            characterData.MaxForce = newMaxForce;
            force.MaxForceText.text = GetValueColorString($"{newMaxForce}", ValueColorType.Pop);
        }

        RefreshSliderValues();
    }


    // ------------------------------ ???? ------------------------------

    int CalculateNewForce(float sliderValue)
    {
        Debug.LogWarning("Change to Achievement Points");
        int max = characterData.MaxForce - characterData.Force > GameValue.Instance.GetResourceValue().TotalRecruitedPopulation
            ? characterData.Force + GameValue.Instance.GetResourceValue().TotalRecruitedPopulation : characterData.MaxForce;

        return Mathf.Clamp(Mathf.RoundToInt(sliderValue * max), 1, characterData.MaxForce);
    }

    int CalculateNewMaxForce(float sliderValue)
    {
        int maxLimit = characterData.GetMaxLimit();
        int goldLimitedMax = characterData.MaxForce + (int)(GameValue.Instance.GetResource(ValueType.Gold) / 100);
        int maxPossible = Mathf.Min(maxLimit, goldLimitedMax);

        return Mathf.Max(1, Mathf.RoundToInt(sliderValue * maxPossible));
    }

    public void RefreshSliderValues()
    {
        RefreshForceValue();
    }

    void RefreshForceValue()
    {
        int maxLimit = characterData.GetMaxLimit();
        int goldLimitedMax = characterData.MaxForce + (int)(GameValue.Instance.GetResource(ValueType.Gold) / 100);
        int maxPossible = Mathf.Min(maxLimit, goldLimitedMax);

        force.MaxForceChangeSlider.value = Mathf.Clamp01((float)characterData.MaxForce / maxPossible);

        // --- ForceChangeSlider ---
        int currentLimit = characterData.MaxForce;
        int popLimitedMax = characterData.Force + GameValue.Instance.GetResourceValue().TotalRecruitedPopulation;
        int currentPossible = Mathf.Min(currentLimit, popLimitedMax);

        force.ForceChangeSlider.value = Mathf.Clamp01((float)characterData.Force / currentPossible);
    }

    public void SetForceType(int forceType)
    {
        this.forceType = forceType;
        ShowForceType(forceType);
    }

    void ShowForceType(int forceType)
    {
        maxLimit.MaxLimitOj.SetActive(forceType == 0);
        force.ForceOj.SetActive(forceType == 1);
        levelAndHealth.LevelAndHealthOj.SetActive(forceType == 2);

    }



    public void SetAttribute(ValueType type, string value)
    {
        var attr = attributes.Find(a => a.type == type);
        if (attr.valueText != null)
            attr.valueText.text = value;
    }

    public void InitButtons()
    {
        InitMoveButtons();
        InitValueButtons();
        InitFavorabilityButtons();
        InitLevelButtons();
        InitForceButtons();
    }

    void InitMoveButtons()
    {
        nameAndMove.MovePlusButton.Setup(OnMoveButtonClick, new SpecialButtonData { step = 1 });
        nameAndMove.MovePlusButton.RegisterBothCtrlModifier((delta) => delta * 10);
        nameAndMove.MoveMinsButton.Setup(OnMoveButtonClick, new SpecialButtonData { step = -1 });
        nameAndMove.MoveMinsButton.RegisterBothCtrlModifier((delta) => delta * 10);


    }

    void InitLevelButtons()
    {

        levelAndHealth.LevelPlusButton.Setup(OnLevelButtonClick, new SpecialButtonData { step = 1 });
        levelAndHealth.LevelPlusButton.RegisterBothCtrlModifier((delta) => delta * 10);

        levelAndHealth.LevelMinusButton.Setup(OnLevelButtonClick, new SpecialButtonData { step = -1 });
        levelAndHealth.LevelMinusButton.RegisterBothCtrlModifier((delta) => delta * 10);

        // ? ??????Ctrl?????????
        levelAndHealth.ExperiencePlusButton.Setup(ExperienceButtonClick, new SpecialButtonData { step = 1 });
        //levelAndHealth.ExperiencePlusButton.RegisterCtrlAlternativeAction(() => AddCharacterLevel(1));
        levelAndHealth.ExperiencePlusButton.RegisterCtrlAlternativeAction(ExperienceButtonCtrlClick);

        levelAndHealth.ExperienceMinusButton.Setup(ExperienceButtonClick, new SpecialButtonData { step = -1 });
        //levelAndHealth.ExperienceMinusButton.RegisterCtrlAlternativeAction(() => AddCharacterLevel(-1));
        levelAndHealth.ExperienceMinusButton.RegisterCtrlAlternativeAction(ExperienceButtonCtrlClick);


    }

    void InitFavorabilityButtons()
    {
        favorability.favorabilityPlusButton.Setup(OnFavorabilityButtonClick, new SpecialButtonData { step = 1 });
        favorability.favorabilityPlusButton.RegisterBothCtrlModifier((delta) => delta * 10);
        favorability.favorabilityMinusButton.Setup(OnFavorabilityButtonClick, new SpecialButtonData { step = -1 });
        favorability.favorabilityMinusButton.RegisterBothCtrlModifier((delta) => delta * 10);

    }

    void InitForceButtons()
    {
        force.ForceChangeSlider.onValueChanged.AddListener(OnForceSliderChanged);
        force.MaxForceChangeSlider.onValueChanged.AddListener(OnMaxSliderChanged);
    }

    void InitValueButtons()
    {
        foreach (var attr in attributes)
        {
            attr.plusButton.Setup(OnValueButtonClick, new SpecialButtonData { valueType = attr.type, step = 1 });
            attr.plusButton.RegisterBothCtrlModifier((delta) => delta * 10);

            attr.minusButton.Setup(OnValueButtonClick, new SpecialButtonData { valueType = attr.type, step = -1 });
            attr.minusButton.RegisterBothCtrlModifier((delta) => delta * 10);
        }

    }

    void SetValueText()
    {
        SetAttribute(ValueType.Attack, GetColorString(FormatfloatNumber(characterData.GetValue(valueType, ValueType.Attack)), valueType, 0));
        SetAttribute(ValueType.Defense, GetColorString(FormatfloatNumber(characterData.GetValue(valueType, ValueType.Defense)), valueType, 1));
        SetAttribute(ValueType.Magic, GetColorString(FormatfloatNumber(characterData.GetValue(valueType, ValueType.Magic)), valueType, 2));
        SetAttribute(ValueType.Speed, GetColorString(FormatfloatNumber(characterData.GetValue(valueType, ValueType.Speed)), valueType, 3));
        SetAttribute(ValueType.Lucky, GetColorString(FormatfloatNumber(characterData.GetValue(valueType, ValueType.Lucky)), valueType, 4));
    }


    void OnMoveButtonClick(SpecialButtonData data, int delta)
    {
        characterData.BattleMoveNum += delta;
    }



    void OnFavorabilityButtonClick(SpecialButtonData data, int delta)
    {
        characterData.Favorability += delta;
    }


    void OnLevelButtonClick(SpecialButtonData data, int delta)
    {
        if (characterData.HasUnlimitedLevel()) return;
        characterData.AddMaxLevel(delta);
    }

    void ExperienceButtonClick(SpecialButtonData data, int delta)
    {
        characterData.AddExperience(delta);
    }

    void ExperienceButtonCtrlClick()
    {
        //characterData.AddExperience(delta);
        if (characterData.CurrentLevel < characterData.BaseMaxLevel)
        {
            characterData.CurrentLevel += 1;
        }
    }



    void OnValueButtonClick(SpecialButtonData data, int delta)
    {
        characterData.AddValue(valueType,data.valueType, delta);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (characterData.character.GetCharacterKey() != CharacterConstants.PlayerKey)
        {
            StoryMultiplePlaythroughsPanelManager.Instance.ShowCharacterMultiplePlaythroughsPanel(characterData);
        } else
        {
            StoryMultiplePlaythroughsPanelManager.Instance.ShowMultiplePlaythroughsPlayerPanel();

        }
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
        SetRowColor();

    }

    public void OnPointerDown(PointerEventData eventData)
    {
        ChangeColorOnDown();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        RestoreColor();
    }


    void ChangeColorOnDown()
    {
        if (column != null)
        {
            // column.color = new Color(column.color.r * 0.7f, column.color.g * 0.7f, column.color.b * 0.7f, column.color.a);
            column.color = GetRowColor(RowType.player);
        }
    }

    public void RestoreColor()
    {
        if (isMouseOverPanel)
        {
            column.color = GetRowColor(RowType.sel);
        } else
        {
            column.color = GetRowColor(RowType.player);

        }

    }

    public MultiplePlaythroughsGameCharacterRowControlData GetCharacterData()
    {
        return characterData;
    }
}