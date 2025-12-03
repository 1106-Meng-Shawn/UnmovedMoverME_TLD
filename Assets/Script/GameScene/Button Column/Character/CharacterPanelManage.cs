using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization.Settings;
using UnityEngine.TextCore.Text;
using System.Linq;
using UnityEngine.UI;
using static BattlePanelManage;
using static GetColor;
using static GetSprite;
using static GetString;
using static FormatNumber;
using static PanelExtensions;

public class CharacterPanelManage : PanelBase, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    #region variable
    public static CharacterPanelManage Instance { get; private set; }

    public GameObject characterPanel;
    public Vector3 initPosition =  Vector3.zero;


    public ScrollRect scrollRect;
    public GameObject characterRowControPrefab;

    public CharacterColumnControl characterRowControl;
    private List<CharacterColumnControl> characterRowControls;
    float moveCooldown = 0.2f;
    float lastMoveTime = 0f;

    private bool isMouseOverPanel = false;

    public Button closeButton;


    [Header("Character Info")]
    public CharacterInfoPanel characterInfoPanel;
    public Image characterBackground;

    [System.Serializable]
    #region CharacterInfoPanel
    public class CharacterInfoPanel
    {
        public Image charcterImage;
        public TMP_Text characterNamText;
        public Image favorabilityImage;
        public TMP_Text favorabilityText;
        public Image roleIcon;
        public TMP_Text MaxForceLimitText;
        public TMP_Text LeaderShipText;
        public TMP_Text CharmText;
        public Image characterForceImage;
        public Image characterMAXForceImage;// may be can delet it
        public int forceType; // 0 is force, 1 is health
        public int valueType = 0; // 0 is battle, 1 is parameter, 2 is Help
        public List<Image> valueImages;
        public List<TextMeshProUGUI> valueTexts;
        public List<SkillButtonControl> skillButtons;
        public Button itemButton;
        private Character characterAtPanel;

        public List<Button> forceTypeButtons;
        public List<Button> valueTypeButtons;

        public TextMeshProUGUI ForceText;
        public Slider ForceChangeSlider;
        public Button ForceChangeCheck;

        public TextMeshProUGUI MaxForceText;
        public Slider MaxForceChangeSlider;
        public Button MaxForceChangeCheck;

        public Button starButton;
        public Button tagButton;
        public List<Button> tagButtons;


        public Button personButton;
        public Button deleteButton;


        public Image moveImage;
        public TextMeshProUGUI moveText;

        public Button lordButton;
        bool isInitializing = true;
        private bool isForceChangedByMax = false;


        public TextMeshProUGUI LvText;
        public TextMeshProUGUI ExperienceText;
        public Slider ExperienceSlider;
        public TextMeshProUGUI HealthText;
        public Slider healthSlider;

        public GameObject HealthGameObject;
        public GameObject ForceGameObject;

        public void InitButtons()
        {
            for (int i = 0; i < forceTypeButtons.Count; i++)
            {
                int index = i;
                forceTypeButtons[i].onClick.AddListener(() => SetForceType(index));
            }

            for (int j = 0; j < valueTypeButtons.Count; j++)
            {
                int index = j;
                valueTypeButtons[j].onClick.AddListener(() => SetValueType(index));
            }
            InitForce();
            starButton.onClick.AddListener(OnStarButtonClick);
            tagButton?.onClick.AddListener(OnTagButtonClick);
            InitTagButtons();

            lordButton?.onClick.AddListener(OnLordButtonClick);
            personButton.onClick.AddListener(OnPersonButtonClick);
            deleteButton.onClick.AddListener(OnDeleteButtonClick);
            itemButton?.onClick.AddListener(OnItemButtonClick);


        }

        void OnStarButtonClick()
        {

            characterAtPanel.Star = !characterAtPanel.Star;
            SetStarSprite(characterAtPanel.Star);
        }

        void OnTagButtonClick()
        {
            int nextTagInt = ((int)characterAtPanel.Tag + 1) % tagButtons.Count;
            characterAtPanel.Tag = (CharacterTag)nextTagInt;
            tagButton.image.sprite = GetCharacterTag(characterAtPanel.Tag);

        }

        void InitTagButtons()
        {
            for (int i = 0; i < tagButtons.Count; i++) {
                int index = i;
                tagButtons[i].onClick.AddListener(()=>OnTagButtonsClick(index));
            }
        }

        void OnTagButtonsClick(int index)
        {
            characterAtPanel.Tag = (CharacterTag)index;
            tagButton.image.sprite = GetCharacterTag(characterAtPanel.Tag);
        }

        void OnPersonButtonClick()
        {

            characterAtPanel.IsPersonBattle = !characterAtPanel.IsPersonBattle;
            SetBattleTypeSprite(characterAtPanel.IsPersonBattle);
            SetForceType(characterAtPanel.IsPersonBattle);
        }

        void OnItemButtonClick()
        {
            characterAtPanel.SetItem(null);

        }


        void OnDeleteButtonClick()
        {
            CharacterPanelManage.Instance.RemoveCharacter();
        }
        void OnLordButtonClick()
        {
            if (characterAtPanel.HasLord())
            {
                characterAtPanel.RemoveLord();

            }
            lordButton.gameObject.SetActive(false);
        }




        void SetStarSprite(bool isStar){
            starButton.interactable = characterAtPanel != null;
            starButton.gameObject.GetComponent<Image>().sprite = UpStarButtonSprite(isStar);
        }

        void SetBattleTypeSprite(bool isPerson)
        {
            personButton.gameObject.SetActive(characterAtPanel != null);
            personButton.gameObject.GetComponent<Image>().sprite = UpBattleButtonSprite(isPerson);
        }



        public void SetValueType(int typeValue)
        {
            this.valueType = typeValue;

            for (int i = 0; i < valueImages.Count; i++)
            {
                valueImages[i].sprite = GetCharacterValueSpritesData(valueType, i);
            }

            if (valueImages[0].sprite == null) Debug.Log("fuck");

            if (characterAtPanel != null)
            {
                for (int i = 0; i < valueTexts.Count; i++)
                {
                    int index = i;
                    valueTexts[i].text = GetColorString(characterAtPanel?.GetValue(valueType, index).ToString("F0"), valueType, i);
                }
            }
            else
            {
                SetValueTypeEmptyText();
            }
        }


        void InitForce()
        {
            ForceChangeSlider.onValueChanged.AddListener(OnForceSliderChanged);
            MaxForceChangeSlider.onValueChanged.AddListener(OnMaxSliderChanged);
        }


        void OnForceSliderChanged(float value)
        {
            if (!ValidateCharacter()) return;
            if (isInitializing) return;

            if (forceType == 0)
            {
                if (characterAtPanel.MaxForce == 1)
                {
                    // 强制固定滑条在最小值（通常是1）
                    ForceChangeSlider.value = ForceChangeSlider.maxValue; // 或者 .value = 1
                    ForceText.text = GetValueColorString("1", ValueColorType.Pop);
                    return;
                }



                int newForce = CalculateNewForce(value);
                int needTroops = newForce - characterAtPanel.Force;

                // 只有不是自动调整才弹窗
                if (!isForceChangedByMax)
                {
                    if (newForce == 1 && characterAtPanel.Force != 1)
                      //  NotificationManage.Instance.ShowAtTop("Minimum troop strength is 1!");
                      NotificationManage.Instance.ShowAtTopByKey(NotificationKeyConstants.Force_Minimum);

                    else if (newForce >= characterAtPanel.MaxForce && characterAtPanel.Force < characterAtPanel.MaxForce)
                        //NotificationManage.Instance.ShowAtTop("Force has reached the maximum!");
                        NotificationManage.Instance.ShowAtTopByKey(NotificationKeyConstants.Force_Max,characterAtPanel.GetCharacterName());

                    else if (needTroops > 0 && GameValue.Instance.GetResourceValue().TotalRecruitedPopulation < needTroops)
                        //NotificationManage.Instance.ShowAtTop("Not enough troops to increase force!");
                        NotificationManage.Instance.ShowAtTopByKey(NotificationKeyConstants.NotEnoughResource,GetResourceStringWithSprite(ValueType.TotalRecruitedPopulation));

                }
                else
                {
                    isForceChangedByMax = false; // 重置标志
                }

                GameValue.Instance.GetResourceValue().TotalRecruitedPopulation -= needTroops;
                characterAtPanel.Force = newForce;
                ForceText.text = GetValueColorString($"{newForce}", ValueColorType.Pop);
            }
        }

        void OnMaxSliderChanged(float value)
        {
            if (!ValidateCharacter()) return;
            if (isInitializing) return;

            if (forceType == 0)
            {
                int newMaxForce = CalculateNewMaxForce(value);
                int requiredGold = (newMaxForce - characterAtPanel.MaxForce) * 100;

                // 消息提示（不中断流程）
                if (newMaxForce == 1 && characterAtPanel.MaxForce != 1)
                    //NotificationManage.Instance.ShowAtTop("Minimum troop strength is 1!");
                    NotificationManage.Instance.ShowAtTopByKey(NotificationKeyConstants.Force_Minimum);
                //else if (newMaxForce >= 9999 && characterAtPanel.MaxForce != 9999)
                //    //NotificationManage.Instance.ShowAtTop("The character's maximum has been reached!");
                //    NotificationManage.Instance.ShowAtTopByKey(NotificationKeyConstants.Force_Max);
                else if (newMaxForce >= characterAtPanel.GetMaxLimit() && characterAtPanel.MaxForce < characterAtPanel.GetMaxLimit())
                    //  NotificationManage.Instance.ShowAtTop("The character's limit has been reached!");
                    NotificationManage.Instance.ShowAtTopByKey(NotificationKeyConstants.Force_Max);
                else if (requiredGold > 0 && GameValue.Instance.GetResourceValue().Gold < requiredGold)
                    //NotificationManage.Instance.ShowAtTop("Not enough Gold!");
                    NotificationManage.Instance.ShowAtTopByKey(NotificationKeyConstants.NotEnoughResource, GetResourceStringWithSprite(ValueType.Gold));


                GameValue.Instance.GetResourceValue().Gold -= requiredGold;

                if (newMaxForce < characterAtPanel.Force)
                {
                    isForceChangedByMax = true;
                    GameValue.Instance.GetResourceValue().TotalRecruitedPopulation += (characterAtPanel.Force - newMaxForce);
                    characterAtPanel.Force = newMaxForce;
                    ForceText.text = GetValueColorString($"{newMaxForce}", ValueColorType.Pop);
                    ForceChangeSlider.SetValueWithoutNotify((float)newMaxForce / newMaxForce);
                }

                characterAtPanel.MaxForce = newMaxForce;
                MaxForceText.text = GetValueColorString($"{newMaxForce}", ValueColorType.Pop);
            }

            RefreshSliderValues();
        }


        // ------------------------------ 计算新值 ------------------------------

        int CalculateNewForce(float sliderValue)
        {
            int max = characterAtPanel.MaxForce - characterAtPanel.Force > GameValue.Instance.GetResourceValue().TotalRecruitedPopulation
                ? characterAtPanel.Force + GameValue.Instance.GetResourceValue().TotalRecruitedPopulation : characterAtPanel.MaxForce;

            return Mathf.Clamp(Mathf.RoundToInt(sliderValue * max), 1, characterAtPanel.MaxForce);
        }

        int CalculateNewMaxForce(float sliderValue)
        {
            int maxLimit = characterAtPanel.GetMaxLimit();
            int goldLimitedMax = characterAtPanel.MaxForce + (int)(GameValue.Instance.GetResource(ValueType.Gold) / 100);
            int maxPossible = Mathf.Min(maxLimit, goldLimitedMax);

            return Mathf.Max(1, Mathf.RoundToInt(sliderValue * maxPossible));
        }


        int CalculateNewHealth(float sliderValue)
        {
            int max = Mathf.Min(characterAtPanel.GetMaxHealth() - characterAtPanel.Health, (int)GameValue.Instance.GetResourceValue().Gold);
            return Mathf.RoundToInt(sliderValue * max);
        }


        int CalculateNewMaxHealth(float sliderValue)
        {
            int max = Mathf.Min(9999 - characterAtPanel.GetMaxHealth(), (int)GameValue.Instance.GetResourceValue().Science);
            return Mathf.RoundToInt(sliderValue * max);
        }

        // ------------------------------ UI 显示 ------------------------------

        void DisplayPreviewText(TextMeshProUGUI text, int original, int updated)
        {
            if (original == updated)
            {
                text.text = original.ToString("N0");
            }
            else
            {
                string sign = updated > original ? "+" : "-";
                text.text = $"{original:N0} {sign} {Mathf.Abs(updated - original):N0}";
            }
        }

        public void RefreshSliderAndText()
        {
            OnForceSliderChanged(ForceChangeSlider.value);
            OnMaxSliderChanged(MaxForceChangeSlider.value);
            RefreshSliderValues();
        }

        public void RefreshSliderValues()
        {
            if (!ValidateCharacter()) return;

            if (forceType == 0)
            {
                // 设置 MaxForceSlider 的值
                RefreshForceValue();

            } 
        }

        // 修复后的 RefreshForceValue()
        void RefreshForceValue()
        {
            // --- MaxForceChangeSlider ---
            int maxLimit = characterAtPanel.GetMaxLimit();
            int goldLimitedMax = characterAtPanel.MaxForce + (int)(GameValue.Instance.GetResource(ValueType.Gold) / 100);
            int maxPossible = Mathf.Min(maxLimit, goldLimitedMax);

            MaxForceChangeSlider.value = Mathf.Clamp01((float)characterAtPanel.MaxForce / maxPossible);

            // --- ForceChangeSlider ---
            int currentLimit = characterAtPanel.MaxForce;
            int popLimitedMax = characterAtPanel.Force + GameValue.Instance.GetResourceValue().TotalRecruitedPopulation;
            int currentPossible = Mathf.Min(currentLimit, popLimitedMax);

            ForceChangeSlider.value = Mathf.Clamp01((float)characterAtPanel.Force / currentPossible);
        }


        bool ValidateCharacter()
        {
            if (characterAtPanel == null)
            {
                ForceChangeSlider.interactable = false;
                MaxForceChangeSlider.interactable = false;
                ForceChangeSlider.value = 1;
                MaxForceChangeSlider.value = 1;
                return false;
            }
            ForceChangeSlider.interactable = true;
            MaxForceChangeSlider.interactable = true;
            return true;
        }



        void SetValueTypeEmptyText()
        {
            if (valueType == 0)
            {
                valueTexts[0].text = GetColorString("A", valueType, 0);
                valueTexts[1].text = GetColorString("D", valueType, 1);
                valueTexts[2].text = GetColorString("M", valueType, 2);
                valueTexts[3].text = GetColorString("S", valueType, 3);
                valueTexts[4].text = GetColorString("L", valueType, 4);
            }
            else if (valueType == 1)
            {
                valueTexts[0].text = GetColorString("F", valueType, 0);
                valueTexts[1].text = GetColorString("S", valueType, 1);
                valueTexts[2].text = GetColorString("P", valueType, 2);
                valueTexts[3].text = GetColorString("G", valueType, 3);
                valueTexts[4].text = GetColorString("F", valueType, 4);
            }
            else if (valueType == 2)
            {
                valueTexts[0].text = GetColorString("L", valueType, 0);
                valueTexts[1].text = GetColorString("S", valueType, 1);
                valueTexts[2].text = GetColorString("B", valueType, 2);
                valueTexts[3].text = GetColorString("N", valueType, 3);
                valueTexts[4].text = GetColorString("C", valueType, 4);

            }
        }

        void SetForceType(bool isPersonBattle)
        {
            if (isPersonBattle) {
                SetForceType(1);
            }else
            {
                SetForceType(0);
            }
        }


        public void SetForceType(int TypeForce)
        {
            this.forceType = TypeForce;

            ForceGameObject.SetActive(TypeForce == 0);
            HealthGameObject.SetActive(TypeForce == 1);
            if (characterAtPanel == null)
            {
                ResetPanel();
                return;
            }

            if (TypeForce == 0)
            {
                MaxForceLimitText.text = GetValueColorString(characterAtPanel?.GetMaxLimit().ToString(), ValueColorType.Pop);
                LeaderShipText.text = GetValueColorString((characterAtPanel?.GetValue(2, 0).ToString() ?? "0"), ValueColorType.Leadership)
                                    + GetValueColorString("*100", ValueColorType.Pop);
                CharmText.text = GetValueColorString((characterAtPanel?.GetValue(2, 4).ToString() ?? "0"), ValueColorType.Charm);
                characterForceImage.sprite = GetBattleForceType("Force");
                ForceText.text = characterAtPanel?.Force.ToString();

                MaxForceText.text = characterAtPanel?.MaxForce.ToString();
            }
            else if (TypeForce == 1)
            {

                LvText.text = characterAtPanel.GetLvAndMaxLevelString();
                ExperienceText.text = characterAtPanel.GetExpAndReqExpString();
                if (characterAtPanel.GetRequiredExpToLvUp() == 0)
                {
                    ExperienceSlider.value = 1;
                } else
                {
                    ExperienceSlider.value = characterAtPanel.GetExpRate();
                }
                healthSlider.value = characterAtPanel.GetHealthAndMaxHealthRate();
                HealthText.text = characterAtPanel.GetHealthAndMaxHealthString();


            }



        }
        public void SetItem(ItemBase itemBase)
        {
            if (characterAtPanel == null) return;
            characterAtPanel.SetItem(itemBase);
            SetCharacterValueAtPanel(characterAtPanel);
        }
        public void SetCharacterValueAtPanel(Character character)
        {
            isInitializing = true;

            if (characterAtPanel != null) characterAtPanel.OnCharacterChanged -= RefreshCharacterUI;
            characterAtPanel = character;
            if (characterAtPanel != null) characterAtPanel.OnCharacterChanged += RefreshCharacterUI;
            RefreshCharacterUI();
            RefreshSliderValues();

            isInitializing = false;

        }


        public void RefreshCharacterUI()
        {
            if (characterAtPanel != null)
            {
                CharacterPanelValueActive(true);
                charcterImage.sprite = characterAtPanel.image;
                characterNamText.text = characterAtPanel.GetCharacterName();
                roleIcon.sprite = GetRoleSprite(characterAtPanel.RoleClass);

                tagButton.image.sprite = GetCharacterTag(characterAtPanel.Tag);

                favorabilityImage.sprite = GetFavorabilitySprite(characterAtPanel.FavorabilityLevel);
                favorabilityText.text = characterAtPanel.Favorability.ToString();


                if (characterAtPanel.IsMoved)
                {
                    moveImage.sprite = GetCharacterStateSprite("cantmove");
                    moveText.text = GetValueColorString($"* {characterAtPanel.BattleMoveNum}", ValueColorType.CantMove);
                }
                else
                {
                    moveImage.sprite = GetCharacterStateSprite("move");
                    moveText.text = GetValueColorString($"* {characterAtPanel.BattleMoveNum}", ValueColorType.CanMove);
                }

                if (characterAtPanel.HasLord())
                {
                    if (lordButton != null)
                    {
                        lordButton.gameObject.SetActive(true);
                        string RemoveString = $"{characterAtPanel.GetLordRegion(0).GetRegionNameWithColor()} \n<color=#FF0000>{LocalizationSettings.StringDatabase.GetLocalizedString("GameSetting", "RemoveLord")}</color>";

                        lordButton.GetComponent<IntroPanelShow>().SetIntroName(RemoveString);

                    }

                }
                else
                {
                    lordButton?.gameObject.SetActive(false);
                }

                personButton.image.sprite = UpBattleButtonSprite(characterAtPanel.IsPersonBattle);
                deleteButton.gameObject.SetActive(characterAtPanel.CanDelete);
                SetForceType(characterAtPanel.IsPersonBattle);
                SetValueType(valueType);
                SetSkillNames();
                if (itemButton != null)
                {
                    if (characterAtPanel.HasItem())
                    {
                        ItemBase itemBaseWithCharacter = characterAtPanel.GetItem();
                        itemButton.interactable = true;
                        itemButton.image.sprite = itemBaseWithCharacter.icon;
                        itemButton.GetComponent<IntroPanelShow>().SetItem(itemBaseWithCharacter);
                    }
                    else
                    {
                        itemButton.interactable = false;
                        Sprite sprite = Resources.Load<Sprite>("MyDraw/Item/EmptyItem");
                        itemButton.GetComponent<IntroPanelShow>().SetItem(null);
                        itemButton.image.sprite = sprite;
                    }
                }
                SetStarSprite(characterAtPanel.Star);
            }
            else
            {
                ResetPanel();

            }
            RefreshSliderValues();
        }

        void ResetPanel()
        {

            CharacterPanelValueActive(false);

            characterAtPanel = null;
            characterNamText.text = "NONE";
            roleIcon.sprite = null;
         // if (characterPanelButton != null) characterPanelButton.SetTheForceType();
            MaxForceText.text = "0";
            if (ForceText != null) ForceText.text = "0";


            MaxForceLimitText.text = GetValueColorString("0", ValueColorType.Pop); ;
            if (LeaderShipText != null) LeaderShipText.text = GetValueColorString("0", ValueColorType.Leadership) + GetValueColorString(" *100", ValueColorType.Pop);
            if (CharmText != null) CharmText.text = GetValueColorString("0", ValueColorType.Charm);


            SetValueTypeEmptyText();

            SetSkillNames();
            SetStarSprite(false);


            LvText.text = $"Lv.{0} / {0}";
            ExperienceText.text = $"{0} / {0}";
            ExperienceSlider.value = 1;
            healthSlider.value = 1;
            HealthText.text = $"{0} / {0}";

        }
        void CharacterPanelValueActive(bool isActive)
        {
            tagButton?.gameObject.SetActive(isActive);
            favorabilityImage?.gameObject.SetActive(isActive);
            favorabilityText?.gameObject.SetActive(isActive);

            charcterImage?.gameObject.SetActive(isActive);
            characterNamText?.gameObject.SetActive(isActive);
            roleIcon?.gameObject.SetActive(isActive);
            itemButton?.gameObject.SetActive(isActive);

            moveImage.gameObject.SetActive(isActive);
            moveText.gameObject.SetActive(isActive);
            lordButton.gameObject.SetActive(isActive);
            personButton.gameObject.SetActive(isActive);
            deleteButton.gameObject.SetActive(isActive);
        }
        void SetSkillNames()
        {
            for (int i = 0; i < skillButtons.Count; i++)
            {

                if (characterAtPanel != null)
                {
                    skillButtons[i].SetSkill(characterAtPanel.GetSkill(i));
                } else
                {
                    skillButtons[i].SetSkill(null);
                }
            }
        }

        public void OnDestroy()
        {
            if (characterAtPanel != null) characterAtPanel.OnCharacterChanged -= RefreshCharacterUI;
        }
    }
    #endregion
    [Header("Top Row")]
    public CharacterPanelTopRow characterPanelTopRow;
    #region CharacterPanelTopRow
    [System.Serializable]
    public class CharacterPanelTopRow
    {
        public bool isStar = false;
        public Button starButton;
        public Button tagButton;
        public List<Button> tagButtons;
        public int tagType = 0;


        public List<Button> sortButtons;

        public FavorabilityLevel favorabilityType = FavorabilityLevel.Self;
        public Button favorabilityTypeButton;

        public GameObject favorbilityTypeButtonsPanel;
        public List<Button> favorabilityTypeButtons;

        public int forceType = 0;
        public List<Button> forceTypeButtons;

        public int valueType = 0;
        public Button valueTypeButton;
        public GameObject valueTypeButtonsPanel;
        public List<Button> valueTypeButtons;
        public List<Image> valueIcons;

        public Image itemImage;
        public CharacterClassFilter characterClassFilter;

        public void InitCharacterClassFilter()
        {
            characterClassFilter.OnFilterClickListener(true, CharacterPanelManage.Instance.UpdateCharacterDisplay);

        }

        public void SetValueType(int valueType)
        {
            this.valueType = valueType;
            for (int i = 0; i < valueIcons.Count; i++)
            {
                valueIcons[i].sprite = GetCharacterValueSpritesData(valueType, i);
            }
            valueTypeButton.gameObject.GetComponent<Image>().sprite = GetCharacterValueSprite(valueType);
        }

        public void SetFavorabilityType(FavorabilityLevel favorabilityType)
        {
            this.favorabilityType = favorabilityType;
            favorabilityTypeButton.gameObject.GetComponent<Image>().sprite = GetFavorabilitySprite(favorabilityType);

        }

        public void InitTagButton()
        {
            tagButton.onClick.AddListener(OnTagButtonClick);
            for (int i = 0;i < tagButtons.Count; i++)
            {
                int index = i;
                tagButtons[i].onClick.AddListener(()=>OnTagButtonsClick(index));
            }
        }

        void OnTagButtonClick()
        {
            int Type = (tagType + 1) % 4;
            SetTopRowTag(Type);
        }


        void OnTagButtonsClick(int index)
        {
            SetTopRowTag(index);
        }

        void SetTopRowTag(int tag)
        {
            tagType = tag;
            tagButton.image.sprite = GetCharacterTag((CharacterTag)tagType);
            CharacterPanelManage.Instance.UpdateCharacterDisplay();
        }

        public void OnDestroy()
        {
            characterClassFilter.OnFilterClickListener(false, CharacterPanelManage.Instance.UpdateCharacterDisplay);

        }



    }
    #endregion


    List<SortField> activeSortFields = new List<SortField>();
    private SortStatus currentSort = new SortStatus();

    #endregion

    void HasItem()
    {
        string iconPath = "MyDraw/Item";
        if (GameValue.Instance.HasItem(ItemConstants.ReichsapfelID))
        {
            ItemBase item = GameValue.Instance.GetItem(ItemConstants.ReichsapfelID);
            characterPanelTopRow.itemImage.sprite = item.icon;
        }
        else
        {
            characterPanelTopRow.itemImage.sprite = Resources.Load<Sprite>(iconPath + "/EmptyItemClose");
        }
    }

    public void OnDestroy()
    {
        //  GameValue.Instance.UnRegisterResourceChanged(characterInfoPanel.RefreshSliderAndText);
        GameValue.Instance.UnRegisterPlayerCharacterChanged(DisplayCharacter);
        GameValue.Instance.UnRegisterItemsChange(HasItem);
        characterInfoPanel.OnDestroy();
        LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;
        characterPanelTopRow.OnDestroy();

    }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;

        characterRowControls = new List<CharacterColumnControl>();
        InitTopRow();
        ClosePanel();
    }


    private void Start()
    {
        GameValue.Instance.RegisterPlayerCharacterChanged(DisplayCharacter);
        LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
        ApplySort(SortField.None, SortDirection.Ascending);
       // DisplayCharacter();
        characterInfoPanel.SetCharacterValueAtPanel(null);
        characterInfoPanel.InitButtons();
        closeButton.onClick.AddListener(ClosePanel);
        GameValue.Instance.RegisterItemsChange(HasItem);
        HasItem();
    }


    void OnLocaleChanged(UnityEngine.Localization.Locale newLocale)
    {
       characterInfoPanel.RefreshCharacterUI();

    }



    void InitTopRow()
    {
        InitSortButtons();
        InitTopRowButtons();
        characterPanelTopRow.SetValueType(0);
        characterPanelTopRow.SetFavorabilityType(0);
        characterPanelTopRow.InitCharacterClassFilter();
    }

    #region Sort

    void InitSortButtons()
    {
        activeSortFields = new List<SortField>();

        SortField f0 = SortField.Name;
        SortField f1 = SortField.Favorability;
        SortField f2 = GetForceField();
        SortField f3 = GetValueField(0);
        SortField f4 = GetValueField(1);
        SortField f5 = GetValueField(2);
        SortField f6 = GetValueField(3);
        SortField f7 = GetValueField(4);

        activeSortFields.AddRange(new[] { f0, f1, f2, f3, f4, f5, f6, f7 });

        for (int i = 0; i < characterPanelTopRow.sortButtons.Count; i++)
        {
            int index = i; // 非常重要，防止闭包引用问题！
            characterPanelTopRow.sortButtons[i].onClick.AddListener(() =>
            {
                OnSortButtonClicked(activeSortFields[index]);
            });
        }
        UpdateSortIcons();
    }
    void OnSortButtonClicked(SortField field)
    {
        if (currentSort.Field != field)
        {
            if (field == SortField.Name)
            {
                ApplySort(field, SortDirection.Ascending);
            }
            else
            {
                ApplySort(field, SortDirection.Descending);
            }
        }
        else
        {
            if (field == SortField.Name)
            {
                switch (currentSort.Direction)
                {
                    case SortDirection.Ascending:
                        ApplySort(field, SortDirection.Descending); // ??->??
                        break;
                    case SortDirection.Descending:
                        ApplySort(SortField.None, SortDirection.None); // ??->??
                        break;
                    default:
                        ApplySort(field, SortDirection.Ascending); // ??->??
                        break;
                }
            }
            else
            {
                switch (currentSort.Direction)
                {
                    case SortDirection.Descending:
                        ApplySort(field, SortDirection.Ascending); // ??->??
                        break;
                    case SortDirection.Ascending:
                        ApplySort(SortField.None, SortDirection.None); // ??->??
                        break;
                    default:
                        ApplySort(field, SortDirection.Descending); // ??->??
                        break;
                }
            }
        }
        UpdateSortIcons();
    }

    void UpdateSortIcons()
    {
        UpdateActiveSortFields();
        for (int i = 0; i < characterPanelTopRow.sortButtons.Count; i++)
        {
            SortField field = activeSortFields[i];
            Image icon = characterPanelTopRow.sortButtons[i].GetComponent<Image>();

            if (currentSort.Field == field && currentSort.Direction != SortDirection.None)
            {
                if (field == SortField.Name)
                {
                    icon.sprite = GetSortSprite(
                        currentSort.Direction == SortDirection.Ascending ? "Descending" : "Ascending"
                    );
                }
                else
                {
                    icon.sprite = GetSortSprite(currentSort.Direction.ToString());
                }
            }
            else
            {
                icon.sprite = GetSortSprite("None");
            }
        }

    }

    void UpdateActiveSortFields()
    {
        activeSortFields[2] = GetForceField();
        activeSortFields[3] = GetValueField(0);
        activeSortFields[4] = GetValueField(1);
        activeSortFields[5] = GetValueField(2);
        activeSortFields[6] = GetValueField(3);
        activeSortFields[7] = GetValueField(4);
    }


    void ApplySort(SortField field, SortDirection direction)
    {
        currentSort = new SortStatus { Field = field, Direction = direction };


        Func<Character, IComparable> keySelector = GetCharacterKeySelector(field);
        bool isAscending = direction == SortDirection.Ascending;

        if (field == SortField.Favorability)
        {
            // ???? Favorability
            characterRowControls = characterRowControls.OrderBy(c =>
            {
                int priority = c.character.FavorabilityLevel == 0 ? 0 : 1;
                int favor = c.character.Favorability;
                return isAscending ? (priority, favor) : (priority, -favor);
            }).ToList();
        }
        else
        {
            characterRowControls = isAscending
                ? characterRowControls.OrderBy(c => keySelector(c.character)).ToList()
                : characterRowControls.OrderByDescending(c => keySelector(c.character)).ToList();
        }

        UpdateCharacterDisplay();
    }

    Func<Character, IComparable> GetCharacterKeySelector(SortField field)
    {
        return field switch
        {
            SortField.Name => c => c.GetCharacterName(),
            SortField.Force => c => c.Force,
            SortField.Health => c => c.Health,
            SortField.Limit => c => c.GetMaxLimit(),
            SortField.Attack => c => c.GetValue(0, 0),
            SortField.Defense => c => c.GetValue(0, 1),
            SortField.Magic => c => c.GetValue(0, 2),
            SortField.Speed => c => c.GetValue(0, 3),
            SortField.Lucky => c => c.GetValue(0, 4),
            SortField.FoodP => c => c.GetValue(1, 0),
            SortField.ScienceP => c => c.GetValue(1, 1),
            SortField.PoliticsP => c => c.GetValue(1, 2),
            SortField.GoldP => c => c.GetValue(1, 3),
            SortField.FaithP => c => c.GetValue(1, 4),
            SortField.Leadership => c => c.GetValue(2, 0),
            SortField.Scout => c => c.GetValue(2, 1),
            SortField.Build => c => c.GetValue(2, 2),
            SortField.Negotiation => c => c.GetValue(2, 3),
            SortField.Charm => c => c.GetValue(2, 4),
            SortField.Favorability => c => c.Favorability, // 如果需要 fallback 处理
            _ => c => -c.GetCharacterID()
        };
    }

    public void RemoveCharacter()
    {
        Debug.Log("maybe need wirte a country class.... to change remove character country");
        characterRowControl.character.SetCountry(CharacterConstants.DIE);

        int returnPop = 0;
        int returnGold = 0;

        returnPop = characterRowControl.character.Force - 1;
        characterRowControl.character.Force = 1;
        GameValue.Instance.GetResourceValue().TotalRecruitedPopulation += returnPop;
        Debug.Log("maybe need add class Parameters,The gold recovery for different professions is different");
        returnGold = characterRowControl.character.MaxForce - 1;
        characterRowControl.character.Force = 1;
        GameValue.Instance.GetResourceValue().Gold += returnGold;

        //NotificationManage.Instance.ShowToTop($"Recruited Population +{returnPop}, Gold +{returnGold}.");
        NotificationManage.Instance.ShowToTopByKey(NotificationKeyConstants.Recruited_PopGold, returnPop.ToString(),returnGold.ToString());

        characterRowControls.Remove(characterRowControl);
        Destroy(characterRowControl.gameObject);
        SetCharacterColumnControl(null);
        ApplySort(currentSort.Field, currentSort.Direction);
    }


    void InitTopRowButtons()
    {
        characterPanelTopRow.starButton.onClick.AddListener(() => SetTopRowStar());


        characterPanelTopRow.valueTypeButton.onClick.AddListener(SetValueTypeByButton);
        for (int i = 0; i < characterPanelTopRow.valueTypeButtons.Count; i++)
        {
            int index = i;
            characterPanelTopRow.valueTypeButtons[i].onClick.AddListener(() => SetValueType(index));
        }


        for (int i = 0; i < characterPanelTopRow.forceTypeButtons.Count; i++)
        {
            int index = i;
            characterPanelTopRow.forceTypeButtons[i].onClick.AddListener(() => SetForceType(index));
        }

        characterPanelTopRow.favorabilityTypeButton.onClick.AddListener(() => SetFavorabilityTypeByButton());


        for (int i = 0; i < characterPanelTopRow.favorabilityTypeButtons.Count; i++)
        {
            int index = i;
            if (characterPanelTopRow.favorabilityTypeButtons[i] != null)
            {
                characterPanelTopRow.favorabilityTypeButtons[i].onClick.AddListener(() => SetFavorabilityType((FavorabilityLevel)index));
            }
        }

        characterPanelTopRow.InitTagButton();

    }

    SortField GetForceField()
    {
        switch (characterPanelTopRow.forceType)
        {
            case 0: return SortField.Force; 
            case 1: return SortField.Health;
            case 2: return SortField.Limit;

        }
        return SortField.Force;
    }

    SortField GetValueField(int index)
    {
        switch (index)
        {
            case 0: return GetAttackField();
            case 1: return GetDefenseField();
            case 2: return GetMagicField();
            case 3: return GetSpeedField();
            case 4: return GetLuckyField();

        }
        return SortField.Attack;
    }

    SortField GetAttackField()
    {
        switch (characterPanelTopRow.valueType)
        {
            case 0: return SortField.Attack;
            case 1: return SortField.FoodP;
            case 2: return SortField.Leadership;
        }
        return SortField.Attack;
    }

    SortField GetDefenseField()
    {
        switch (characterPanelTopRow.valueType)
        {
            case 0: return SortField.Defense;
            case 1: return SortField.ScienceP;
            case 2: return SortField.Scout;

        }
        return SortField.Defense;
    }


    SortField GetMagicField()
    {
        switch (characterPanelTopRow.valueType)
        {
            case 0: return SortField.Magic;
            case 1: return SortField.PoliticsP;
            case 2: return SortField.Build;

        }
        return SortField.Magic;
    }


    SortField GetSpeedField()
    {
        switch (characterPanelTopRow.valueType)
        {
            case 0: return SortField.Speed;
            case 1: return SortField.GoldP;
            case 2: return SortField.Negotiation;

        }
        return SortField.Speed;
    }


    SortField GetLuckyField()
    {
        switch (characterPanelTopRow.valueType)
        {
            case 0: return SortField.Lucky;
            case 1: return SortField.FaithP;
            case 2: return SortField.Charm;

        }
        return SortField.Lucky;
    }


    #endregion


    void SetTopRowStar()
    {
        characterPanelTopRow.isStar = !characterPanelTopRow.isStar;
        string iconPath = $"MyDraw/UI/Other/";
        if (characterPanelTopRow.isStar)
        {
            characterPanelTopRow.starButton.gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>(iconPath + "Star");
        }
        else
        {
            characterPanelTopRow.starButton.gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>(iconPath + "Unstar");
        }
        UpdateCharacterDisplay();
    }

    void SetForceType(int forceType)
    {
        characterPanelTopRow.forceType = forceType;
        for (int i = 0; i < characterRowControls.Count; i++)
        {
            var columnControl = characterRowControls[i];
            columnControl.SetTheForceType(forceType);
        }
        UpdateSortIcons();

    }

    void SetValueTypeByButton()
    {

        if ((characterPanelTopRow.valueType + 1) == 3) { SetValueType(0); }
        else { SetValueType(characterPanelTopRow.valueType + 1); }
        ;
        UpdateSortIcons();


    }


    void SetFavorabilityTypeByButton()
    {
        SetFavorabilityType(EnumHelper.GetEnumByOffset<FavorabilityLevel>(characterPanelTopRow.favorabilityType,1));
    }


    void SetValueType(int valueType)
    {
        characterPanelTopRow.SetValueType(valueType);
        characterPanelTopRow.valueType = valueType;
        for (int i = 0; i < characterRowControls.Count; i++)
        {
            var columnControl = characterRowControls[i];
            columnControl.SetTheType(valueType);
        }
        UpdateSortIcons();

    }

    void SetFavorabilityType(FavorabilityLevel valueType)
    {
        characterPanelTopRow.SetFavorabilityType(valueType);
        UpdateCharacterDisplay();
        UpdateSortIcons(); 
    }


    public void SetCharacter(Character newCharacter)
    {
        if (newCharacter == null) return;
        CharacterColumnControl target = characterRowControls.FirstOrDefault(ctrl => ctrl.character == newCharacter);

        if (target != null)
        {
            SetCharacterColumnControl(target);
        }
        else
        {
            Debug.Log($"Can't find CharacterColumnControl: {newCharacter.GetCharacterENName()}");
        }
    }

    public void SetCharacterColumnControl(CharacterColumnControl newSelection)
    {
        // 1. ????????????
        if (this.characterRowControl != null)
        {
            this.characterRowControl.RestoreColor();
            this.characterRowControl.Highlight(false);
        }

        // 2. ????
        this.characterRowControl = newSelection;

        // 3. ?????????????????
        if (this.characterRowControl != null)
        {
            this.characterRowControl.Highlight(true);
            this.characterRowControl.GetComponent<Image>().color = GetRowColor(RowType.sel);
            characterInfoPanel.SetCharacterValueAtPanel(this.characterRowControl.character);
        }
        else
        {
            characterInfoPanel.SetCharacterValueAtPanel(null);
        }
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        isMouseOverPanel = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isMouseOverPanel = false;
    }





    void Update()
    {
        if (isMouseOverPanel && Input.GetKey(KeyCode.UpArrow) && Time.time - lastMoveTime > moveCooldown)
        {
            lastMoveTime = Time.time;
            MoveSelCharaceterRowControl(1);
        }
        if (isMouseOverPanel && Input.GetKey(KeyCode.DownArrow) && Time.time - lastMoveTime > moveCooldown)
        {
            lastMoveTime = Time.time;
            MoveSelCharaceterRowControl(-1);
        }

        if (isMouseOverPanel && Input.GetMouseButtonDown(1)) 
        {
            ClosePanel();
        }

    }

    void MoveSelCharaceterRowControl(int num)
    {
        var controls = characterRowControls;

        if (controls == null || controls.Count == 0) return;

        if (characterRowControl == null)
        {
            foreach (var ctrl in controls)
            {
                if (ctrl.gameObject.activeSelf)
                {
                    SoundManager.Instance.PlaySelSFX();
                    SetCharacterColumnControl(ctrl);
                    return;
                }
            }
            return;
        }

        int index = controls.IndexOf(characterRowControl);
        if (index == -1) return;

        int start = index;
        do
        {
            index -= num;

            if (index < 0) index = controls.Count - 1;
            else if (index >= controls.Count) index = 0;

            if (controls[index].gameObject.activeSelf)
            {
                SoundManager.Instance.PlaySelSFX();
                SetCharacterColumnControl(controls[index]);
                return;
            }

        } while (index != start);
    }
    public override void OpenPanel()
    {
        // gameObject.transform.position = Vector2.zero;
        //  characterPanel.transform.position = initPosition;
        DisplayCharacter(); 
        characterPanel.SetActive(true);
    }


    public override void ClosePanel()
    {
       // characterPanel.transform.position = initPosition;
        SetCharacterColumnControl(null);
        characterPanel.SetActive(false);

    }

    public override void SetPanelSaveData(PanelSaveData panelSaveData)
    {
        SetSaveData(this,characterPanel, panelSaveData);
    }


    public void DisplayCharacter()
    {
        foreach (Transform child in scrollRect.content)//scrollContent)
        {
            Destroy(child.gameObject);
        }

        // Character[] foundCharacterComponents = GameObject.FindObjectsOfType<Character>();
        GameValue gameValue = GameValue.Instance;
        List<Character> playerCharacters = gameValue.GetPlayerCharacters();
        foreach (var character in playerCharacters)
        {
            DisplayCharacterPanel(character);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(scrollRect.content.GetComponent<RectTransform>());
    }


    void DisplayCharacterPanel(Character character)
    {
        GameObject characterPrefab = Instantiate(characterRowControPrefab, scrollRect.content);// scrollContent);

        CharacterColumnControl characterRowControl = characterPrefab.GetComponent<CharacterColumnControl>();
        if (characterRowControl != null)
        {
            characterRowControl.CharacterColumnControlP(character);
            characterRowControl.type = 1;
        }
        if (character.FavorabilityLevel == 0)
        {
            characterRowControl.transform.SetAsFirstSibling();

        }
        characterRowControls.Add(characterRowControl);

    }

    public void SetItem(ItemBase itemBase)
    {
        characterInfoPanel.SetItem(itemBase);
    }


    public void UpdateCharacterDisplay()
    {
        CharacterColumnControl playerCharacterControl = null;

        for (int i = 0; i < characterRowControls.Count; i++)
        {
            var columnControl = characterRowControls[i];
            columnControl.transform.SetSiblingIndex(i);

            bool passStarFilter = !characterPanelTopRow.isStar || columnControl.character.Star;
            bool passFavorabilityFilter = characterPanelTopRow.favorabilityType == 0 || columnControl.character.FavorabilityLevel == characterPanelTopRow.favorabilityType;
            bool passTagFilter = characterPanelTopRow.tagType == 0 || columnControl.character.Tag == (CharacterTag)characterPanelTopRow.tagType;
            if ((CharacterTag)characterPanelTopRow.tagType == CharacterTag.Lord && columnControl.character.HasLord()) passTagFilter = true;
            bool passClassFilter = characterPanelTopRow.characterClassFilter.PassFilter(columnControl.character);


            if (columnControl.character.FavorabilityLevel == 0)
            {
                passFavorabilityFilter = true;
                if (!currentSort.IsSorted) 
                {
                    playerCharacterControl = columnControl;
                }
            }

            columnControl.gameObject.SetActive(passStarFilter && passFavorabilityFilter && passTagFilter && passClassFilter);
        }

        if (!currentSort.IsSorted && playerCharacterControl != null)
        {
            playerCharacterControl.transform.SetSiblingIndex(0);
            characterRowControls.Remove(playerCharacterControl);
            characterRowControls.Insert(0, playerCharacterControl);
        }
    }


    public void OnBeginDrag(PointerEventData eventData)
    {
        if (IsPointerOnCharacterBackground(eventData) && characterRowControl != null)
        {
            GetComponent<DraggablePanel>().shouldBlockPanelDrag = true;
            characterRowControl.OnBeginDrag(eventData);
        }
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (characterRowControl != null)
        {
            characterRowControl.OnDrag(eventData);
        }
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        StartCoroutine(HandleEndDrag(eventData));
    }

    private IEnumerator HandleEndDrag(PointerEventData eventData)
    {
        if (characterRowControl != null)
        {
            characterRowControl.OnEndDrag(eventData);

            yield return null;

            if (characterRowControl.GetEndDragHandled())
            {
                characterRowControl.SetEndDragHandled(false);
                yield break;
            }
        }

        yield return null;

       if (GetComponent<DraggablePanel>().shouldBlockPanelDrag && eventData.pointerCurrentRaycast.gameObject != characterBackground.gameObject)
        {
            SetCharacterColumnControl(null);
        }
        GetComponent<DraggablePanel>().shouldBlockPanelDrag = false;

    }

    private bool IsPointerOnCharacterBackground(PointerEventData eventData)
    {
        return eventData.pointerEnter != null && eventData.pointerEnter == characterBackground.gameObject;
    }

    public override PanelSaveData GetPanelSaveData()
    {
        PanelSaveData saveData = GetSaveData(this,characterPanel,PanelType.Character);
        return saveData;
    }

    public void RemoveCharacterRowControl(CharacterColumnControl characterRowControl)
    {
         characterRowControls.Remove(characterRowControl);

    }

}
