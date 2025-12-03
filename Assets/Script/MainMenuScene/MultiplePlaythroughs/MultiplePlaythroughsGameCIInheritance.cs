using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public enum InheritanceType
{
    CharacterLevel,
    CharacterFavorability,
    CharacterSkill,
    Item
}

public class MultiplePlaythroughsGameCIInheritance : MonoBehaviour
{

    public static MultiplePlaythroughsGameCIInheritance Instance;

    [SerializeField] private Toggle CharacterLevelInheritance;
    [SerializeField] private Toggle CharacterFavorabilityInheritance;
    [SerializeField] private Toggle CharacterSkillInheritance;
    [SerializeField] private Toggle ItemInheritance;

    [SerializeField] private TextMeshProUGUI CharacterNum;
    [SerializeField] private Button CharacterRestButton;
    [SerializeField] private Button CharacterAddButton;
    [SerializeField] private Button CharacterDeleteAllButton;


    private MultiplePlaythroughsGameCIInheritanceData data;
    [SerializeField] private MultiplePlaythroughsGameCharacterControl MultiplePlaythroughsGameCharacterControl;

    private List<MultiplePlaythroughsGameCharacterRowControlData> characterDatas = new List<MultiplePlaythroughsGameCharacterRowControlData>();


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }



    private void Start()
    {
        data = SettingValue.Instance.StoryMultiplePlaythroughsData
            .MultiplePlaythroughsGameTotalData
            .MultiplePlaythroughsGameCIInheritanceData;

        InitToggles();
        RegisterListeners();
        InitButtons();
    }

    private void InitToggles()
    {
        CharacterLevelInheritance.isOn = data.CharacterLevelAndValueInheritance;
        CharacterFavorabilityInheritance.isOn = data.CharacterFavorabilityInheritance;
        CharacterSkillInheritance.isOn = data.CharacterSkillInheritance;
        ItemInheritance.isOn = data.ItemInheritance;
    }


    void InitButtons()
    {
         CharacterRestButton.onClick.AddListener(OnRestButtonClick);
         CharacterAddButton.onClick.AddListener(OnAddButtonClick);
        CharacterDeleteAllButton.onClick.AddListener(OnDeleteButtonClick);
    }

    void OnRestButtonClick()
    {
        foreach (var data in characterDatas)
        {
            data.RestData();
        }
    }

    void OnAddButtonClick()
    {

    }

    void OnDeleteButtonClick()
    {

    }


    private void RegisterListeners()
    {
        // UI ? Data
        CharacterLevelInheritance.onValueChanged.AddListener(v => data.CharacterLevelAndValueInheritance = v);
        CharacterFavorabilityInheritance.onValueChanged.AddListener(v => data.CharacterFavorabilityInheritance = v);
        CharacterSkillInheritance.onValueChanged.AddListener(v => data.CharacterSkillInheritance = v);
        ItemInheritance.onValueChanged.AddListener (v => data.ItemInheritance = v);

        data.OnInheritanceChanged += OnInheritanceChanged;
    }

    private void OnInheritanceChanged(InheritanceType type, bool value)
    {
        switch (type)
        {
            case InheritanceType.CharacterLevel: OnLevelInheritanceChange(value); break;
            case InheritanceType.CharacterFavorability: OnFavorabilityInheritanceChange(value);break;
            case InheritanceType.CharacterSkill: OnSkillInheritanceChange(value); break;
            case InheritanceType.Item: OnItemInheritanceChange(value); break;

        }
    }

    void OnLevelInheritanceChange(bool value)
    {
        CharacterLevelInheritance.SetIsOnWithoutNotify(value);
    }

    void OnFavorabilityInheritanceChange(bool value)
    {
        CharacterFavorabilityInheritance.SetIsOnWithoutNotify(value);
    }

    void OnSkillInheritanceChange(bool value)
    {
        CharacterSkillInheritance.SetIsOnWithoutNotify(value);
    }

    void OnItemInheritanceChange(bool value)
    {
        ItemInheritance.SetIsOnWithoutNotify(value);
    }


    public void SetCharacterList(List<Character> characters)
    {
        characterDatas = MultiplePlaythroughsGameCharacterControl.SetCharacterList(characters);
    }


    public MultiplePlaythroughsGameCharacterRowControlData GetCharacterData(string CharacterKey)
    {
        return characterDatas.Find(c => c.character.GetCharacterKey() == CharacterKey);
    }

    public List<MultiplePlaythroughsGameCharacterRowControlData> GetCharacterDatas()
    {
        return characterDatas;
    }




    private void OnDestroy()
    {
        if (data != null)
            data.OnInheritanceChanged -= OnInheritanceChanged;
    }
}

public class MultiplePlaythroughsGameCIInheritanceData
{
    private bool _characterLevelAndValueInheritance;
    private bool _characterFavorabilityInheritance;
    private bool _characterSkillInheritance;


    private bool _itemInheritance;

    public event Action<InheritanceType, bool> OnInheritanceChanged;

    public bool CharacterLevelAndValueInheritance
    {
        get => _characterLevelAndValueInheritance;
        set
        {
            if (_characterLevelAndValueInheritance != value)
            {
                _characterLevelAndValueInheritance = value;
                OnInheritanceChanged?.Invoke(InheritanceType.CharacterLevel, value);
            }
        }
    }

    public bool CharacterFavorabilityInheritance
    {
        get => _characterFavorabilityInheritance;
        set
        {
            if (_characterFavorabilityInheritance != value)
            {
                _characterFavorabilityInheritance = value;
                OnInheritanceChanged?.Invoke(InheritanceType.CharacterFavorability, value);
            }
        }
    }

    public bool CharacterSkillInheritance
    {
        get => _characterSkillInheritance;
        set
        {
            if (_characterSkillInheritance != value)
            {
                _characterSkillInheritance = value;
                OnInheritanceChanged?.Invoke(InheritanceType.CharacterSkill, value);
            }
        }
    }

    public bool ItemInheritance
    {
        get => _itemInheritance;
        set
        {
            if (_itemInheritance != value)
            {
                _itemInheritance = value;
                OnInheritanceChanged?.Invoke(InheritanceType.Item, value);
            }
        }
    }


    public int GetAchievementCost()
    {
        int cost = 0;
        return cost;
    }
}
