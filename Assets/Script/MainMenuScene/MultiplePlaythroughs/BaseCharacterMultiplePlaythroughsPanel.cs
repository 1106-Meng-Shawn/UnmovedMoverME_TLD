using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.SmartFormat.Utilities;
using UnityEngine.UI;
using static GetColor;
using static GetSprite;
using static GetString;
using static MultiplePlaythroughsGameCharacterRowControl;

public abstract class BaseCharacterMultiplePlaythroughsPanel : MonoBehaviour
{
    [Header("Character Info")]
    [SerializeField] protected CharacterInfoGroup characterInfo;

    [Header("Character Visuals")]
    [SerializeField] protected CharacterVisualsGroup visuals;

    [Header("Top Row Values")]
    [SerializeField] protected TopRowGroup topRow;

    [Header("Force and Max Force")]
    [SerializeField] protected ForceGroup forceGroup;

    [Header("Value Groups")]
    [SerializeField] protected MultiplePlaythroughsValueRowGroup battleRowGroups = new();
    [SerializeField] protected MultiplePlaythroughsValueRowGroup parameterRowGroups = new();
    [SerializeField] protected MultiplePlaythroughsValueRowGroup helpRowGroups = new();

    [Header("Skill Section")]
    [SerializeField] protected SkillGroup skillGroup;

    [Header("Character Data")]
    [SerializeField] protected MultiplePlaythroughsGameCharacterRowControlData data;


    #region --- Group Definitions ---

    [System.Serializable]
    public struct CharacterInfoGroup
    {
        public TextMeshProUGUI nameText;
        public Image favorabilityImage;
        public TextMeshProUGUI favorabilityText;
        public TextMeshProUGUI moveText;
    }

    [System.Serializable]
    public struct CharacterVisualsGroup
    {
        public Image characterImage;
        public Image classRoleImage;
        public Image itemImage;
        public Button starButton;
    }

    [System.Serializable]
    public struct TopRowGroup
    {
        public Image characterIconImage;
        public Button rowNameButton;
        public TextMeshProUGUI rowNameText;
        public Button nameChangeButton;
        public CharacterValueRowControl move;
        public Button rowClassRoleButton;
        public CharacterValueRowControl favorability;
        public CharacterValueRowControl level;
        public TextMeshProUGUI healthText;
        public CharacterValueRowControl experience;
    }

    [System.Serializable]
    public struct ForceGroup
    {
        public TextMeshProUGUI maxLimitText;
        public TextMeshProUGUI leaderShipText;
        public TextMeshProUGUI charmText;
        public CharacterValueRowControl force;
        public CharacterValueRowControl maxForce;
    }

    [System.Serializable]
    public class MultiplePlaythroughsValueRowGroup
    {
        public List<CharacterValueRowControl> controls = new();
    }

    [System.Serializable]
    public struct SkillGroup
    {
        public List<SkillButtonControl> Skills;
        public ScrollRect SkillLibraryScrollRect;
        public List<SkillButtonControl> SkillLibraryLists;
        public SkillButtonControl SkillPrefab;
    }

    #endregion


    #region --- Logic Methods ---

    public virtual void Init() { }

    public virtual void SetData(MultiplePlaythroughsGameCharacterRowControlData data)
    {
        if (this.data != null)this.data.OnCharacterRowControlDataChanged -= UpDisplay;
        this.data = data;
        if (this.data != null)this.data.OnCharacterRowControlDataChanged += UpDisplay;
        SetValueRowData();
        if (this.data != null)UpDisplay();
    }

    void SetValueRowData()
    {
        topRow.move.InitCharacter(data, ValueType.Move);
        topRow.favorability.InitCharacter(data, ValueType.Favorability);
        topRow.level.InitCharacter(data, ValueType.Level);
        topRow.experience.InitCharacter(data, ValueType.Experience);


        InitBattleRowGroups();
        InitParameterRowGroups();
        InitHelpRowGroups();
    }

    void InitBattleRowGroups()
    {
        battleRowGroups.controls[0].InitCharacter(data, ValueType.Attack);
        battleRowGroups.controls[1].InitCharacter(data, ValueType.Defense);
        battleRowGroups.controls[2].InitCharacter(data, ValueType.Magic);
        battleRowGroups.controls[3].InitCharacter(data, ValueType.Speed);
        battleRowGroups.controls[4].InitCharacter(data, ValueType.Lucky);
    }

    void InitParameterRowGroups()
    {
        parameterRowGroups.controls[0].InitCharacter(data, ValueType.FoodP);
        parameterRowGroups.controls[1].InitCharacter(data, ValueType.ScienceP);
        parameterRowGroups.controls[2].InitCharacter(data, ValueType.PoliticsP);
        parameterRowGroups.controls[3].InitCharacter(data, ValueType.GoldP);
        parameterRowGroups.controls[4].InitCharacter(data, ValueType.FaithP);
    }

    void InitHelpRowGroups()
    {
        helpRowGroups.controls[0].InitCharacter(data, ValueType.Leadership);
        helpRowGroups.controls[1].InitCharacter(data, ValueType.Scout);
        helpRowGroups.controls[2].InitCharacter(data, ValueType.Build);
        helpRowGroups.controls[3].InitCharacter(data, ValueType.Negotiation);
        helpRowGroups.controls[4].InitCharacter(data, ValueType.Charm);
    }


    public virtual void UpDisplay()
    {
        UpCharacterImage();
        UpName();
        UpFavorability();
        UpMove();
        UpLevelHealthExp();
        UpMaxLimit();

        forceGroup.maxForce.UpdateRowDisplay();
        forceGroup.force.UpdateRowDisplay();
    }

    void UpCharacterImage()
    {
        visuals.characterImage.sprite = data.GetImage();
        topRow.characterIconImage.sprite = data.GetIcon();
    }

    #region Set Value
    void UpName()
    {
        characterInfo.nameText.text = data.GetCharacterName();
        topRow.rowNameText.text = data.GetCharacterName();
    }

    void UpFavorability()
    {
        characterInfo.favorabilityImage.sprite = GetFavorabilitySprite(data.FavorabilityLevel);
        characterInfo.favorabilityText.text = GetFavorabilityString(data.Favorability);
        topRow.favorability.UpdateRowDisplay();
    }

    void UpMove()
    {
        characterInfo.moveText.text = GetMoveNumString(data.BattleMoveNum);
        topRow.move.UpdateRowDisplay();
    }

    void UpLevelHealthExp()
    {
        topRow.healthText.text = data.GetHealthAndMaxHealthString();
        topRow.level.UpdateRowDisplay();
    }

    void UpMaxLimit()
    {
        forceGroup.maxLimitText.text = GetMaxLimitString(data.GetMaxLimit());
        forceGroup.leaderShipText.text = GetValueColorString(data.GetValue(2, 0).ToString(), ValueColorType.Leadership);
        forceGroup.charmText.text = GetValueColorString(data.GetValue(2, 4).ToString(), ValueColorType.Charm);
    }
    #endregion

    #endregion




    #region ExtraData

    private CharacterExtrasSaveData characterExtrasSave;

    public virtual void SetExtraData(CharacterExtrasSaveData characterExtrasSave)
    {
        if (this.characterExtrasSave != null) this.characterExtrasSave.ValueChangeListener(false, OnDataValueChanged);
        this.characterExtrasSave = characterExtrasSave;
        this.characterExtrasSave.ValueChangeListener(true, OnDataValueChanged);
        DisplayCharacterExtras(this.characterExtrasSave);
        visuals.starButton.onClick.RemoveAllListeners();
        visuals.starButton.onClick.AddListener(OnStarButtonClick);
    }

    private void OnDestroy()
    {
        if (characterExtrasSave != null) characterExtrasSave.ValueChangeListener(false, OnDataValueChanged);
    }

    private void OnStarButtonClick()
    {
        if (characterExtrasSave == null) return;
        characterExtrasSave.IsStar = !characterExtrasSave.IsStar;
    }

    private void OnDataValueChanged(string fieldName)
    {
        if (fieldName == nameof(CharacterExtrasSaveData.IsStar))
        {
            visuals.starButton.image.sprite = GetSprite.UpStarButtonSprite(characterExtrasSave.IsStar);
        }
    }

    void DisplayCharacterExtras(CharacterExtrasSaveData data)
    {
        string currentLanguage = LocalizationSettings.SelectedLocale.Identifier.Code;
        string defaultName = data.CharacterNameDictionary["en"];
        string characterName = data.CharacterNameDictionary.TryGetValue(currentLanguage, out var name) ? name : defaultName;

        DisplayBasicInfo(data, characterName, defaultName);
        DisplayTopRow(data, characterName, defaultName);
        DisplayForceGroup(data);
        DisplayRowGroup(battleRowGroups, data, 0, new ValueType[] { ValueType.Attack, ValueType.Defense, ValueType.Magic, ValueType.Speed, ValueType.Lucky });
        DisplayRowGroup(parameterRowGroups, data, 1, new ValueType[] { ValueType.FoodP, ValueType.ScienceP, ValueType.PoliticsP, ValueType.GoldP, ValueType.Faith });
        DisplayRowGroup(helpRowGroups, data, 2, new ValueType[] { ValueType.Leadership, ValueType.Scout, ValueType.Build, ValueType.Negotiation, ValueType.Charm });
    }

    private void DisplayBasicInfo(CharacterExtrasSaveData data, string characterName, string defaultName)
    {
        characterInfo.nameText.text = characterName;
        characterInfo.favorabilityImage.sprite = GetFavorabilitySprite(data.FavorabilityLevel);
        characterInfo.favorabilityText.text = GetFavorabilityString(data.Favorability);
        characterInfo.moveText.text = GetMoveNumString(data.BattleMoveNum);

        visuals.characterImage.sprite = GetCharacterImage(data.CharacterKey, data.CharacterFileTypeList[0]);
        visuals.starButton.image.sprite = GetSprite.UpStarButtonSprite(data.IsStar);
        visuals.classRoleImage.sprite = GetRoleSprite(data.RoleClass);
    }

    private void DisplayTopRow(CharacterExtrasSaveData data, string characterName, string defaultName)
    {
        topRow.characterIconImage.sprite = GetCharacterIcon(defaultName, data.CharacterFileTypeList[0]);
        topRow.rowNameText.text = characterName;
        topRow.move.SetValue(null, GetMoveNumString(data.BattleMoveNum));
        topRow.rowClassRoleButton.image.sprite = GetRoleSprite(data.RoleClass);
        topRow.favorability.SetValue(GetFavorabilitySprite(data.FavorabilityLevel), GetFavorabilityString(data.Favorability));
        topRow.level.SetValue(null, GetLvAndMaxLevelString(data.CurrentLevel, data.BaseMaxLevel));
        topRow.healthText.text = GetHealthAndMaxHealthString(data.Health, data.Health);

        float requiredExp = data.GetRequiredExpToLvUp();
        float expRate = data.GetExpRate();
        topRow.experience.SetValue(null, GetExpAndReqExpString(data.Experience, (int)requiredExp), expRate);
    }

    private void DisplayForceGroup(CharacterExtrasSaveData data)
    {
        forceGroup.maxLimitText.text = GetMaxLimitString(data.GetMaxLimit());
        forceGroup.leaderShipText.text = GetValueColorString(data.GetValue(2, 0).ToString(), ValueType.Leadership);
        forceGroup.charmText.text = GetValueColorString(data.GetValue(2, 4).ToString(), ValueType.Charm);
        forceGroup.force.SetValue(null, data.Force.ToString());
        forceGroup.maxForce.SetValue(null, data.MaxForce.ToString());
    }

    private void DisplayRowGroup(MultiplePlaythroughsValueRowGroup rowGroup, CharacterExtrasSaveData data, int rowIndex, ValueType[] types)
    {
        for (int i = 0; i < rowGroup.controls.Count; i++)
        {
            int value = data.GetValue(rowIndex, i);
            rowGroup.controls[i].SetValue(GetValueSprite(types[i]), GetValueColorString(value.ToString(), types[i]));
        }
    }

    #endregion

}
