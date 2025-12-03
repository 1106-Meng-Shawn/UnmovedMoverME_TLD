using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;
using static GetSprite;
using static GetString;

public class CharacterExtrasObjectControl : MonoBehaviour
{
    [SerializeField] private Image ClassImage;
    [SerializeField] private Image CharacterIcon;
    [SerializeField] private TextMeshProUGUI NameText;
    [SerializeField] private Button StarButton;
    [SerializeField] private TextMeshProUGUI LevelText;
    [SerializeField] private Image FavorabilityImage;
    [SerializeField] private TextMeshProUGUI FavorabilityText;
    [SerializeField] private Button CharacterExtrasButton;

    private CharacterExtrasSaveData characterExtrasSaveData;


    void OnEnable()
    {
        LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
    }

    void OnDisable()
    {
        LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;

    }


    private void Awake()
    {
        InitButtons();
    }

    public void Init(CharacterExtrasSaveData saveData)
    {
        // ????????????
        if (characterExtrasSaveData != null)
            characterExtrasSaveData.ValueChangeListener(false, OnValueChanged);

        characterExtrasSaveData = saveData;


        // ?????
        ClassImage.sprite = GetRoleSprite(saveData.RoleClass);
        SetRoleClassIntro();
        CharacterIcon.sprite = saveData.GetCharacterIcon();
        SetCharacterName();
        FavorabilityImage.sprite = GetFavorabilitySprite(saveData.FavorabilityLevel);
        FavorabilityText.text = GetFavorabilityString(saveData.Favorability);
        LevelText.text = GetLvAndMaxLevelString(saveData.CurrentLevel, saveData.BaseMaxLevel);

        // ??????
        characterExtrasSaveData.ValueChangeListener(true, OnValueChanged);

        // ???????
        UpStarButtonSprite();
    }

    private void OnDestroy()
    {
        if (characterExtrasSaveData != null)
            characterExtrasSaveData.ValueChangeListener(false, OnValueChanged);
    }

    private void InitButtons()
    {
        StarButton.onClick.AddListener(OnStarButtonClick);
        CharacterExtrasButton.onClick.AddListener(OnCharacterExtrasButtonClick);
    }

    private void OnStarButtonClick()
    {
        characterExtrasSaveData.IsStar = !characterExtrasSaveData.IsStar;
    }

    private void OnCharacterExtrasButtonClick()
    {
        ExtrasPanelManager.Instance.SetCharacterExtrasSave(characterExtrasSaveData);
    }

    private void UpStarButtonSprite()
    {
        StarButton.image.sprite = GetSprite.UpStarButtonSprite(characterExtrasSaveData.IsStar);
    }

    private void OnValueChanged(string fieldName)
    {
        if (fieldName == nameof(CharacterExtrasSaveData.IsStar))
        {
            UpStarButtonSprite();
        }

    }



    private void OnLocaleChanged(Locale locale)
    {
        SetCharacterName(); 
        SetRoleClassIntro();
    }


    void SetCharacterName()
    {
        NameText.text = characterExtrasSaveData.GetCharacterName();
    }


    void SetRoleClassIntro()
    {
        ClassImage.gameObject.GetComponent<IntroPanelShow>().SetIntroName(GetClassRoleString(characterExtrasSaveData.RoleClass));
    }


}
