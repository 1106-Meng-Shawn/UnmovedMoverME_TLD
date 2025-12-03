using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;
using static GetColor;
using static GetSprite;
using static GetString;
using static UITopControl;

public class CharacterInfo : MonoBehaviour
{

    [SerializeField] private bool isEnemy;

    public Image roleImage;
    public Image characterImage;


    public Button starButton;
    public Button tagButton;
    public List<Button> tagButtons;

    public Button personBattleButton;
    public Button deletButton;



    public TextMeshProUGUI nameText;
    public Image favorabilityLevelImage;
    public TextMeshProUGUI favorabilityText;
    public Image itemImage;

    public List<TextMeshProUGUI> valueTexts;
    public List<Button> valueButtons;

    public List<SkillButtonControl> skillButtons;

    public GameObject ForceOj;
    public TextMeshProUGUI forceMaxLimitText;
    public TextMeshProUGUI leadershipText;
    public TextMeshProUGUI charmText;
    public TextMeshProUGUI ForceText;
    public Slider ForceChangeSlider;

    public GameObject HealthOj;
    public TextMeshProUGUI healthText;
    public Slider HealthSlider;
    public TextMeshProUGUI LvText;
    public TextMeshProUGUI ExperienceText;
    public Slider ExperienceSlider;


    public TextMeshProUGUI MaxForceText;
    public Slider MaxForceChangeSlider;

    private Character characterAtPanel;
    public bool isExplore;

    public Image moveImage;
    public TextMeshProUGUI moveText;

    public Button lordButton;
    private bool isInitializing = false;

    private bool isForceChangedByMax = false;

    public List<GameObject> HideObjects;

    public GameObject characterBackground;


    private void Start()
    {
        LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
        InitCharacterPanel();
        lordButton?.onClick.AddListener(OnLordButtonClick);
        if (!isEnemy) personBattleButton.onClick.AddListener(OnPersonBattleButtonClick);

    }

    void OnLocaleChanged(UnityEngine.Localization.Locale newLocale)
    {
        UpCharacterUI();

    }

    private void OnDestroy()
    {
        LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;
        if (characterAtPanel != null) characterAtPanel.OnCharacterChanged -= UpCharacterUI;

    }

    void InitCharacterPanel()
    {
        for (int i = 0; i < 5; i++)
        {
            int index = i;
            valueButtons[i].onClick.AddListener(() => AddValue(index));

        }
        if (valueTexts.Count == 6) valueButtons[5].onClick.AddListener(() => AddValue(5));

        InitStar();
        InitForce();
        InitTagButton();
    }

    void InitTagButton()
    {
        if (isEnemy) return;

        tagButton.onClick.AddListener(OnTagButtonClick);
        for (int i = 0; i < 4; i++)
        {
            int index = i;
            tagButtons[i].onClick.AddListener(() => OnTagButtonsClick(index));

        }
    }

    void OnTagButtonClick()
    {
        int index = ((int)characterAtPanel.Tag + 1) % tagButtons.Count;
        SetTagType(index);

    }

    void OnTagButtonsClick(int index)
    {
        SetTagType(index);
    }

    void SetTagType(int tagType)
    {
        characterAtPanel.Tag = (CharacterTag)tagType;
        tagButton.image.sprite = GetCharacterTag((CharacterTag)tagType);
    }


    void OnPersonBattleButtonClick()
    {
        if (BattlePanelManage.Instance.IsPersonBattleInBattlePanel())
        {
            // NotificationManage.Instance.ShowAtTop("Exploration can only be fought person");
            NotificationManage.Instance.ShowAtTopByKey(NotificationKeyConstants.Exploration_SingleOnly);
            return;
        }

        characterAtPanel.IsPersonBattle = !characterAtPanel.IsPersonBattle;
        SetPersonBattleValues();
    }

    void InitStar()
    {
        starButton.onClick.AddListener(() => SetIsStar());
        SetStarButtonSprite();

    }

    void SetIsStar()
    {
        if (characterAtPanel != null) characterAtPanel.SetStar();
        SetStarButtonSprite();
    }

    void SetStarButtonSprite()
    {
        if (characterAtPanel == null)
        {
            starButton.GetComponent<Image>().sprite = UpStarButtonSprite(false);

        }
        else
        {
            starButton.GetComponent<Image>().sprite = UpStarButtonSprite(characterAtPanel.Star);

        }
    }

    void AddValue(int index)
    {
        // need change
        if (characterAtPanel == null) return;
        // for battle info scout
        if (index == 5)
        {
            characterAtPanel.AddValue(2, 1, 1);
            return;

        }


        characterAtPanel.AddValue(0, index, 1);

    }



    public void SetCharacter(Character character)
    {
        isInitializing = true;
        if (characterAtPanel != null) characterAtPanel.OnCharacterChanged -= UpCharacterUI;
        characterAtPanel = character;
        if (characterAtPanel != null) characterAtPanel.OnCharacterChanged += UpCharacterUI;

        //  ValidateCharacter();
        RefreshSliderValues();
        if (characterAtPanel == null)
        {
            ResetPanel();
        }
        else
        {
            UpCharacterUI();
        }
        isInitializing = false;


    }

    void ResetPanel()
    {

        CharacterPanelValueActive(false);

        characterAtPanel = null;
        nameText.text = "NONE";
        roleImage.sprite = null;
        characterImage.sprite = null;
        itemImage.sprite = null;

        starButton.GetComponent<Image>().sprite = UpStarButtonSprite(false);

        /* normalText.text = GetValueColorString($"0", "pop");
         maxText.text = GetValueColorString($"0", "pop");*/
        ForceText.text = GetValueColorString($"0", ValueColorType.Pop);
        MaxForceText.text = GetValueColorString($"0", ValueColorType.Pop);
        forceMaxLimitText.text = GetValueColorString($"0", ValueColorType.Pop);
        SetPersonBattleValues();
        if (isEnemy)
        {
            LvText.text = "Lv.0";
            healthText.text = GetValueColorString($"0 / 0", ValueColorType.Pop);
            HealthSlider.value = 1f;
        }
        else
        {
            LvText.text = "Lv.0 / 0";
            healthText.text = GetValueColorString($"0 / 0", ValueColorType.Pop);
            ExperienceText.text = "0 / 0";
            HealthSlider.value = 1f;
            ExperienceSlider.value = 1f;

        }


        valueTexts[0].text = "A";
        valueTexts[1].text = "D";
        valueTexts[2].text = "M";
        valueTexts[3].text = "S";
        valueTexts[4].text = "L";

        SetSkillName();

        leadershipText.text = GetValueColorString($"0", ValueColorType.Leadership) + GetValueColorString($" * 100", ValueColorType.Pop);
        charmText.text = GetValueColorString($"0", ValueColorType.Charm);
        //  characterPanel.forceMaxLimitText.text = GetValueColorString($"0", "pop");
        RefreshSliderValues();
    }

    void CharacterPanelValueActive(bool isActive)
    {
        tagButton?.gameObject.SetActive(isActive);
        nameText.gameObject.SetActive(isActive);
        roleImage.gameObject.SetActive(isActive);
        characterImage.gameObject.SetActive(isActive);
        itemImage.gameObject.SetActive(isActive);
        favorabilityLevelImage.gameObject.SetActive(isActive);
        favorabilityText.gameObject.SetActive(isActive);

        moveImage.gameObject.SetActive(isActive);
        moveText.gameObject.SetActive(isActive);
        lordButton?.gameObject.SetActive(isActive);

        personBattleButton?.gameObject.SetActive(isActive);  
        deletButton?.gameObject.SetActive(isActive);
        if (isEnemy) UnlockHideOject();

    }

    public void UnlockHideOject()
    {
        Debug.Log("work");

        int hideCount = Mathf.Max(0, BattlePanelManage.Instance.battleArray.unlockScoutLevel - 1);

        for (int i = 0; i < HideObjects.Count; i++)
        {
            HideObjects[i].gameObject.SetActive(i >= hideCount);
        }
    }

    public void UpCharacterUI()
    {
        CharacterPanelValueActive(true);
        SetCharacterName();
        SetCharacterValue();
        SetSkillName();


    }

    void SetCharacterName()
    {
        if (characterAtPanel == null) return;

        nameText.text = characterAtPanel.GetCharacterName();
    }
    void SetCharacterValue()
    {
        if (characterAtPanel == null) return;

        characterImage.sprite = characterAtPanel.image;
        favorabilityLevelImage.sprite = GetFavorabilitySprite(characterAtPanel.FavorabilityLevel);
        favorabilityText.text = characterAtPanel.Favorability.ToString();

        roleImage.sprite = GetRoleSprite(characterAtPanel.RoleClass);
        itemImage.sprite = characterAtPanel.GetItemWithCharacterSprite();

        ForceText.text = GetValueColorString($"{characterAtPanel.Force.ToString()}", ValueColorType.Pop);
        MaxForceText.text = GetValueColorString($"{characterAtPanel.MaxForce.ToString()}", ValueColorType.Pop);


        if (!isEnemy)
        {
            RefreshSliderValues();
            leadershipText.text = GetValueColorString($"{characterAtPanel.GetValue(2, 0)}", ValueColorType.Leadership) + GetValueColorString($" * 100", ValueColorType.Pop);
            charmText.text = GetValueColorString($"{characterAtPanel.GetValue(2, 4)}", ValueColorType.Charm);
            forceMaxLimitText.text = GetValueColorString($"{characterAtPanel.GetMaxLimit()}", ValueColorType.Pop);

            tagButton.image.sprite = GetCharacterTag(characterAtPanel.Tag);
            deletButton.gameObject.SetActive(characterAtPanel.CanDelete);
        }
        personBattleButton.image.sprite = UpBattleButtonSprite(IsPersonBattle());


        SetPersonBattleValues();

        RefreshHealthValues();

        for (int i = 0; i < 5; i++)
        {
            valueTexts[i].text = characterAtPanel.GetValue(0, i).ToString();

        }

        // battle panel info scout
        if (valueTexts.Count == 6) valueTexts[5].text = GetValueColorString(characterAtPanel.GetValue(2, 1).ToString(), ValueColorType.Scout);

        if (characterAtPanel.IsMoved && !BattlePanelManage.Instance.isInExplore)
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
                string RemoveString = $"{characterAtPanel.GetLordRegion(0)?.GetRegionNameWithColor()} \n<color=#FF0000>{LocalizationSettings.StringDatabase.GetLocalizedString("GameSetting", "RemoveLord")}</color>";
                lordButton.GetComponent<IntroPanelShow>().SetIntroName(RemoveString);

            }

        }
        else
        {
            lordButton?.gameObject.SetActive(false);
        }
    }

    void OnLordButtonClick()
    {
        if (characterAtPanel.HasLord())
        {
            characterAtPanel.RemoveLord();

        }
        lordButton.gameObject.SetActive(false);
    }



    void SetSkillName()
    {
        for (int i = 0; i < skillButtons.Count; i++)
        {

            if (characterAtPanel != null)
            {
                skillButtons[i].SetSkill(characterAtPanel.GetSkill(i));
            }
            else
            {
                skillButtons[i].SetSkill(null);
            }
        }
    }


    public void InitForce()
    {
        if (isEnemy) return;
        ForceChangeSlider.onValueChanged.AddListener(OnForceSliderChanged);
        MaxForceChangeSlider.onValueChanged.AddListener(OnMaxSliderChanged);
    }


    void OnForceSliderChanged(float value)
    {
        if (!ValidateCharacter()) return;
        if (isInitializing) return;

        if (!isExplore)
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
                    //NotificationManage.Instance.ShowAtTop("You don't have enough force");
                    NotificationManage.Instance.ShowAtTopByKey(NotificationKeyConstants.NotEnoughResource, GetResourceStringWithSprite(ValueType.TotalRecruitedPopulation));

            }
            else
            {
                isForceChangedByMax = false; // 重置标志
            }

            GameValue.Instance.GetResourceValue().TotalRecruitedPopulation -= needTroops;
            characterAtPanel.Force = newForce;
            ForceText.text = GetValueColorString($"{newForce}", ValueColorType.Pop);
        }
        else
        {
            int newHealth = CalculateNewHealth(value);
            GameValue.Instance.GetResourceValue().Gold -= newHealth;
            characterAtPanel.Health += newHealth;
            ForceText.text = GetValueColorString($"{characterAtPanel.Health}", ValueColorType.Pop);
        }
    }

    void OnMaxSliderChanged(float value)
    {
        if (!ValidateCharacter()) return;
        if (isInitializing) return;

        if (!isExplore)
        {
            int newMaxForce = CalculateNewMaxForce(value);
            int requiredGold = (newMaxForce - characterAtPanel.MaxForce) * 100;

            // 消息提示（不中断流程）
            if (newMaxForce == 1 && characterAtPanel.MaxForce != 1)
                NotificationManage.Instance.ShowAtTopByKey(NotificationKeyConstants.Force_Minimum);
            //NotificationManage.Instance.ShowAtTop("Minimum troop strength is 1!");
            else if (newMaxForce >= 9999 && characterAtPanel.MaxForce != 9999)
                //NotificationManage.Instance.ShowAtTop("The character's maximum has been reached!");
                NotificationManage.Instance.ShowAtTopByKey(NotificationKeyConstants.Force_Max,characterAtPanel.GetCharacterName());
            else if (newMaxForce >= characterAtPanel.GetMaxLimit() && characterAtPanel.MaxForce < characterAtPanel.GetMaxLimit())
              //  NotificationManage.Instance.ShowAtTop("The character's limit has been reached!");
                NotificationManage.Instance.ShowAtTopByKey(NotificationKeyConstants.Character_LimitReached,characterAtPanel.GetCharacterName());
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
        else
        {
            Debug.Log("Cancel health add settings ");
        }

        RefreshSliderValues();
    }

    // ------------------------------ ???? ------------------------------

    int CalculateNewForce(float sliderValue)
    {
        int max = characterAtPanel.MaxForce - characterAtPanel.Force <= GameValue.Instance.GetResourceValue().TotalRecruitedPopulation
            ? characterAtPanel.MaxForce
            : characterAtPanel.Force + GameValue.Instance.GetResourceValue().TotalRecruitedPopulation;

        return Mathf.Clamp(Mathf.RoundToInt(sliderValue * max), 1, characterAtPanel.MaxForce);
    }

    int CalculateNewHealth(float sliderValue)
    {
        int max = Mathf.Min(characterAtPanel.GetMaxHealth() - characterAtPanel.Health, (int)GameValue.Instance.GetResourceValue().Gold);
        return Mathf.RoundToInt(sliderValue * max);
    }

    int CalculateNewMaxForce(float sliderValue)
    {
        int maxLimit = characterAtPanel.GetMaxLimit();
        int goldLimitedMax = characterAtPanel.MaxForce + (int)(GameValue.Instance.GetResourceValue().Gold / 100);
        int maxPossible = Mathf.Min(maxLimit, goldLimitedMax);
        return Mathf.Max(1, Mathf.RoundToInt(sliderValue * maxPossible));
    }

    int CalculateNewMaxHealth(float sliderValue)
    {
        int max = Mathf.Min(9999 - characterAtPanel.GetMaxHealth(), (int)GameValue.Instance.GetResourceValue().Science);
        return Mathf.RoundToInt(sliderValue * max);
    }

    // ------------------------------ UI ?? ------------------------------

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

    void RefreshHealthValues()
    {
        if (isEnemy)
        {
            LvText.text = characterAtPanel.GetCurrentLevel().ToString();
            healthText.text = characterAtPanel.GetHealthAndMaxHealthString();
        }
        else
        {
            LvText.text = characterAtPanel.GetLvAndMaxLevelString();
            healthText.text = characterAtPanel.GetHealthAndMaxHealthString();
            ExperienceText.text = characterAtPanel.GetExpAndReqExpString();
            HealthSlider.value = characterAtPanel.GetHealthAndMaxHealthRate();
            ExperienceSlider.value = characterAtPanel.GetExpRate();

        }
    }

    public void RefreshSliderValues()
    {
        if (!ValidateCharacter()) return;

        if (!isExplore)
        {
            // ?? MaxForceSlider ??
            RefreshForceValue();

        }
        else
        {
            // ?? MaxHealthSlider ??
            if (characterAtPanel.GetMaxHealth() >= 9999 || GameValue.Instance.GetResourceValue().Science <= 0)
                MaxForceChangeSlider.value = 1f;
            else
                MaxForceChangeSlider.value = 0f;

        }
    }

    void RefreshForceValue()
    {
        // --- MaxForceChangeSlider ---
        int maxLimit = characterAtPanel.GetMaxLimit();
        int goldLimitedMax = characterAtPanel.MaxForce + (int)(GameValue.Instance.GetResourceValue().Gold / 100);
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
        if (isEnemy) return false;


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


    bool IsPersonBattle()
    {
        if (characterAtPanel == null) return BattlePanelManage.Instance.IsPersonBattleInBattlePanel();
        return characterAtPanel.IsPersonBattle || BattlePanelManage.Instance.IsPersonBattleInBattlePanel();
    }

    void SetPersonBattleValues()
    {
        HealthOj.gameObject.SetActive(IsPersonBattle());
        ForceOj.gameObject.SetActive(!IsPersonBattle());
    }

    public GameObject GetCharacterBackground()
    {
        return characterBackground;
    }

}