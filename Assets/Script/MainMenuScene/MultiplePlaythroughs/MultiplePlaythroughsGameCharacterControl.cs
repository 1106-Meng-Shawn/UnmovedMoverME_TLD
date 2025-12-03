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
using static GetSprite;

public class MultiplePlaythroughsGameCharacterControl : MonoBehaviour
{
    [Header("Top Row")]
    public TopRow topRow;
    #region TopRow
    [System.Serializable]
    public class TopRow
    {
        public bool isStar = false;
        public Button starButton;
        public Button tagButton;
        public List<Button> tagButtons;
        public int tagType = 0;


        public List<Button> sortButtons;

        public FavorabilityLevel favorabilityType = FavorabilityLevel.Self;
        public Button favorabilityTypeButton;

        public List<Button> favorabilityTypeButtons;

        public int forceType = 2;
        public List<Button> forceTypeButtons;

        public int valueType = 0;
        public Button valueTypeButton;
        public List<Button> valueTypeButtons;
        public List<Image> valueIcons;

        public Image itemImage;
        public CharacterClassFilter characterClassFilter;

         
        public void OnFilterClickListener(bool isAdd,Action action)
        {
           characterClassFilter.OnFilterClickListener(isAdd, action);

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
            for (int i = 0; i < tagButtons.Count; i++)
            {
                int index = i;
                tagButtons[i].onClick.AddListener(() => OnTagButtonsClick(index));
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
        }


    }


    [SerializeField] ScrollRect scrollRect;
    public MultiplePlaythroughsGameCharacterRowControl MultiplePlaythroughsGameCharacterRowControlPrefab;
    private List<MultiplePlaythroughsGameCharacterRowControl> MultiplePlaythroughsGameCharacterRowControls = new List<MultiplePlaythroughsGameCharacterRowControl>();

    public enum SortField
    {
        Name, Favorability, Force, Health, Limit, Attack, Food, Leadership, Defense, Science, Scout, Magic, Politics, Build, Speed, Gold, Negotiation, Lucky, Faith, Charm, None
    }

    public enum SortDirection
    {
        None, Ascending, Descending
    }

    public struct SortStatus
    {
        public SortField Field;
        public SortDirection Direction;

        public bool IsSorted => Field != SortField.None && Direction != SortDirection.None;
    }

    List<SortField> activeSortFields = new List<SortField>();
    private SortStatus currentSort;


    #endregion

    private void Awake()
    {
        InitTopRow();
    }

    void InitTopRow()
    {
        InitSortButtons();
        InitTopRowButtons();
        topRow.SetValueType(0);
        topRow.SetFavorabilityType(0);
        topRow.OnFilterClickListener(true, UpdateCharacterDisplay);
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

        for (int i = 0; i < topRow.sortButtons.Count; i++)
        {
            int index = i; // ??????????????
            topRow.sortButtons[i].onClick.AddListener(() =>
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
        for (int i = 0; i < topRow.sortButtons.Count; i++)
        {
            SortField field = activeSortFields[i];
            Image icon = topRow.sortButtons[i].GetComponent<Image>();

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


        Func<MultiplePlaythroughsGameCharacterRowControlData, IComparable> keySelector = GetCharacterKeySelector(field);
        bool isAscending = direction == SortDirection.Ascending;

        if (field == SortField.Favorability)
        {
            // ???? Favorability
            MultiplePlaythroughsGameCharacterRowControls = MultiplePlaythroughsGameCharacterRowControls.OrderBy(c =>
            {
                int priority = c.characterData.FavorabilityLevel == 0 ? 0 : 1;
                int favor = c.characterData.Favorability;
                return isAscending ? (priority, favor) : (priority, -favor);
            }).ToList();
        }
        else
        {
            MultiplePlaythroughsGameCharacterRowControls = isAscending
                ? MultiplePlaythroughsGameCharacterRowControls.OrderBy(c => keySelector(c.characterData)).ToList()
                : MultiplePlaythroughsGameCharacterRowControls.OrderByDescending(c => keySelector(c.characterData)).ToList();
        }

        UpdateCharacterDisplay();
    }


    //void UpdateDisplay()
    //{
    //    if (MultiplePlaythroughsGameCharacterRowControls.Count == 0) return;
    //    Transform parent = MultiplePlaythroughsGameCharacterRowControls[0].transform.parent;
    //    for (int i = 0; i < MultiplePlaythroughsGameCharacterRowControls.Count; i++)
    //    {
    //        MultiplePlaythroughsGameCharacterRowControls[i].transform.SetSiblingIndex(i);
    //    }
    //}


    public void UpdateCharacterDisplay()
    {
        MultiplePlaythroughsGameCharacterRowControl playerCharacterControl = null;

        for (int i = 0; i < MultiplePlaythroughsGameCharacterRowControls.Count; i++)
        {
            var columnControl = MultiplePlaythroughsGameCharacterRowControls[i];
            if (columnControl == null) continue; 

            columnControl.transform.SetSiblingIndex(i);

            var data = columnControl.GetCharacterData();
            bool passStarFilter = !topRow.isStar || data.Star;
            bool passFavorabilityFilter = topRow.favorabilityType == 0 || data.FavorabilityLevel == topRow.favorabilityType;
            bool passTagFilter = topRow.tagType == 0 || data.Tag == (CharacterTag)topRow.tagType;
            bool passClassFilter = topRow.characterClassFilter.PassFilter(data);

            if (data.FavorabilityLevel == 0 && !currentSort.IsSorted)
            {
                playerCharacterControl = columnControl;
            }

            columnControl.gameObject.SetActive(passStarFilter && passFavorabilityFilter && passTagFilter && passClassFilter);
        }

        if (!currentSort.IsSorted && playerCharacterControl != null)
        {
            playerCharacterControl.transform.SetSiblingIndex(0);
            MultiplePlaythroughsGameCharacterRowControls.Remove(playerCharacterControl);
            MultiplePlaythroughsGameCharacterRowControls.Insert(0, playerCharacterControl);
        }
    }



    Func<MultiplePlaythroughsGameCharacterRowControlData, IComparable> GetCharacterKeySelector(SortField field)
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
            SortField.Food => c => c.GetValue(1, 0),
            SortField.Science => c => c.GetValue(1, 1),
            SortField.Politics => c => c.GetValue(1, 2),
            SortField.Gold => c => c.GetValue(1, 3),
            SortField.Faith => c => c.GetValue(1, 4),
            SortField.Leadership => c => c.GetValue(2, 0),
            SortField.Scout => c => c.GetValue(2, 1),
            SortField.Build => c => c.GetValue(2, 2),
            SortField.Negotiation => c => c.GetValue(2, 3),
            SortField.Charm => c => c.GetValue(2, 4),
            SortField.Favorability => c => c.Favorability, // ???? fallback ??
            _ => c => -c.GetCharacterID()
        };
    }


    void InitTopRowButtons()
    {
        topRow.starButton.onClick.AddListener(() => SetTopRowStar());


        topRow.valueTypeButton.onClick.AddListener(SetValueTypeByButton);
        for (int i = 0; i < topRow.valueTypeButtons.Count; i++)
        {
            int index = i;
            topRow.valueTypeButtons[i].onClick.AddListener(() => SetValueType(index));
        }


        for (int i = 0; i < topRow.forceTypeButtons.Count; i++)
        {
            int index = i;
            topRow.forceTypeButtons[i].onClick.AddListener(() => SetForceType(index));
        }
        topRow.favorabilityTypeButton.onClick.AddListener(() => SetFavorabilityTypeByButton());


        for (int i = 0; i < topRow.favorabilityTypeButtons.Count; i++)
        {
            int index = i;
            if (topRow.favorabilityTypeButtons[i] != null)
            {
                topRow.favorabilityTypeButtons[i].onClick.AddListener(() => SetFavorabilityType((FavorabilityLevel)index));
            }
        }

        topRow.InitTagButton();

    }

    SortField GetForceField()
    {
        switch (topRow.forceType)
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
        switch (topRow.valueType)
        {
            case 0: return SortField.Attack;
            case 1: return SortField.Food;
            case 2: return SortField.Leadership;
        }
        return SortField.Attack;
    }

    SortField GetDefenseField()
    {
        switch (topRow.valueType)
        {
            case 0: return SortField.Defense;
            case 1: return SortField.Science;
            case 2: return SortField.Scout;

        }
        return SortField.Defense;
    }


    SortField GetMagicField()
    {
        switch (topRow.valueType)
        {
            case 0: return SortField.Magic;
            case 1: return SortField.Politics;
            case 2: return SortField.Build;

        }
        return SortField.Magic;
    }


    SortField GetSpeedField()
    {
        switch (topRow.valueType)
        {
            case 0: return SortField.Speed;
            case 1: return SortField.Gold;
            case 2: return SortField.Negotiation;

        }
        return SortField.Speed;
    }


    SortField GetLuckyField()
    {
        switch (topRow.valueType)
        {
            case 0: return SortField.Lucky;
            case 1: return SortField.Faith;
            case 2: return SortField.Charm;

        }
        return SortField.Lucky;
    }


    #endregion


    void SetTopRowStar()
    {
        topRow.isStar = !topRow.isStar;
        string iconPath = $"MyDraw/UI/Other/";
        if (topRow.isStar)
        {
            topRow.starButton.gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>(iconPath + "Star");
        }
        else
        {
            topRow.starButton.gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>(iconPath + "Unstar");
        }
       // UpdateCharacterDisplay();
    }

    void SetForceType(int forceType)
    {
        topRow.forceType = forceType;
        for (int i = 0; i < MultiplePlaythroughsGameCharacterRowControls.Count; i++)
        {
            var columnControl = MultiplePlaythroughsGameCharacterRowControls[i];
            columnControl.SetForceType(forceType);
        }
        UpdateSortIcons();

    }

    void SetValueTypeByButton()
    {

        if ((topRow.valueType + 1) == 3) { SetValueType(0); }
        else { SetValueType(topRow.valueType + 1); }
        ;
        UpdateSortIcons();


    }


    void SetFavorabilityTypeByButton()
    {
        SetFavorabilityType(EnumHelper.GetEnumByOffset(topRow.favorabilityType, 1));
    }

    public void SetFavorabilityType(FavorabilityLevel favorabilityType)
    {
        topRow.favorabilityType = favorabilityType;
        topRow.favorabilityTypeButton.gameObject.GetComponent<Image>().sprite = GetFavorabilitySprite(favorabilityType);

    }



    void SetValueType(int valueType)
    {
        topRow.SetValueType(valueType);
        topRow.valueType = valueType;
        for (int i = 0; i < MultiplePlaythroughsGameCharacterRowControls.Count; i++)
        {
            var columnControl = MultiplePlaythroughsGameCharacterRowControls[i];
            columnControl.SetType(valueType);
        }
        UpdateSortIcons();

    }

    public List<MultiplePlaythroughsGameCharacterRowControlData> SetCharacterList(List<Character> characters)
    {
        List<MultiplePlaythroughsGameCharacterRowControlData> characterData = new List<MultiplePlaythroughsGameCharacterRowControlData>();
        foreach (var row in MultiplePlaythroughsGameCharacterRowControls)
        {
            if (row != null)
                Destroy(row.gameObject);
        }
        MultiplePlaythroughsGameCharacterRowControls.Clear();

        foreach (var character in characters)
        {
            var newRow = Instantiate(MultiplePlaythroughsGameCharacterRowControlPrefab, scrollRect.content);
            newRow.Init(character);
            MultiplePlaythroughsGameCharacterRowControls.Add(newRow);
            characterData.Add(newRow.GetCharacterData());
        }
        UpdateCharacterDisplay();
        scrollRect.verticalNormalizedPosition = 1f;

        return characterData;
    }


    private void OnDestroy()
    {
        topRow.OnFilterClickListener(false, UpdateCharacterDisplay);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
