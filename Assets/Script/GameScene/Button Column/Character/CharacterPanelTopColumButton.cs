using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.TextCore.Text;
using System;
using static GetSprite;
using static BattlePanelManage;
public class CharacterPanelTopColumButton : MonoBehaviour
{


    public int panelType; // 1 is character, 2 is recruit
    public int valueType = 0;// 0 is battle value, 1 is Parameter value,2 is help value

    public Image attackIcon;
    public Image defenseIcon;
    public Image magicIcon;
    public Image speedIcon;
    public Image luckyIcon;

    public List<Sprite> battleValueSprite;
    public List<Sprite> ParameterValueSprite;
    public List<Sprite> HelpValueSprite;


    /* public Button ChangeToValueType1Button;
     public Button ChangeToValueType2Button;
     public Button ChangeToValueType3Button;*/

    public Button ValueTypeButton;
    public GameObject valueButtonsPanel;
    public List<Button> ValueTypeButtons;


    public int ForceType;

    public List<Sprite> ForceTypeSprites;
    public List<Button> ForceTypeButtons;

    public FavorabilityLevel FavorabilityType = FavorabilityLevel.Self; // 0 is all. 1 is normall. 2 is love
    public Button FavorabilityTypeButton;
    public Image FavorabilityImage;
    public GameObject FavorabilityButtonPanel;
    public List<Sprite> FavorabilityTypeSprites;
    public List<Button> FavorabilityTypeButtons;

    public List<CharacterColumnControl> characterColumControls = new List<CharacterColumnControl>();

    public Transform scrollContent;

    public enum SortField
    {
        Name, Favorability, Force, Health, Attack, Food, Defense, Magic, Speed, Lucky, None
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

    private SortStatus currentSort;


    public Button starButton;
    private bool isStar = false;

    void SetTheForceTypeByButton(int index)
    {
      //  SetTheForceType(index);
    }
    void ApplySort(SortField field, SortDirection direction)
    {
        currentSort = new SortStatus { Field = field, Direction = direction };

        if (!currentSort.IsSorted)
        {
            UpdateCharacterDisplay(); // ????
            return;
        }

        Func<Character, IComparable> keySelector = GetCharacterKeySelector(field);
        bool isAscending = direction == SortDirection.Ascending;

        if (field == SortField.Favorability)
        {
            // ???? Favorability
            characterColumControls = characterColumControls.OrderBy(c =>
            {
                int priority = c.character.FavorabilityLevel == 0 ? 0 : 1;
                int favor = c.character.Favorability;
                return isAscending ? (priority, favor) : (priority, -favor);
            }).ToList();
        }
        else
        {
            characterColumControls = isAscending
                ? characterColumControls.OrderBy(c => keySelector(c.character)).ToList()
                : characterColumControls.OrderByDescending(c => keySelector(c.character)).ToList();
        }

        UpdateCharacterDisplay();
    }

    Func<Character, IComparable> GetCharacterKeySelector(SortField field)
    {
        return field switch
        {
            SortField.Name => c => c.GetCharacterName(),
            SortField.Force => c => c.Force,
            SortField.Attack => c => c.GetValue(valueType, 0),
            SortField.Defense => c => c.GetValue(valueType, 1),
            SortField.Magic => c => c.GetValue(valueType, 2),
            SortField.Speed => c => c.GetValue(valueType, 3),
            SortField.Lucky => c => c.GetValue(valueType, 4),
            _ => c => 0 // fallback
        };
    }

    void OnClickSortButton(SortField field)
    {
        // Toggle ??
        if (currentSort.Field == field)
        {
            currentSort.Direction = currentSort.Direction switch
            {
                SortDirection.None => SortDirection.Ascending,
                SortDirection.Ascending => SortDirection.Descending,
                SortDirection.Descending => SortDirection.Ascending
            };
        }
        else
        {
            currentSort = new SortStatus
            {
                Field = field,
                Direction = SortDirection.Ascending
            };
        }

        ApplySort(currentSort.Field, currentSort.Direction);
        UpdateSortIcons(); // UI ????
    }

    #region SortButton
    void InitSortButtons()
    {
      /*  topRow.sortButtons[0].onClick.AddListener(() => OnSortButtonClicked(SortField.Name));
        topRow.sortButtons[1].onClick.AddListener(() => OnSortButtonClicked(SortField.Favorability));
        topRow.sortButtons[2].onClick.AddListener(() => OnSortButtonClicked(SortField.Force));
        topRow.sortButtons[3].onClick.AddListener(() => OnSortButtonClicked(SortField.Attack));
        topRow.sortButtons[4].onClick.AddListener(() => OnSortButtonClicked(SortField.Defense));
        topRow.sortButtons[5].onClick.AddListener(() => OnSortButtonClicked(SortField.Magic));
        topRow.sortButtons[6].onClick.AddListener(() => OnSortButtonClicked(SortField.Speed));
        topRow.sortButtons[7].onClick.AddListener(() => OnSortButtonClicked(SortField.Lucky));*/

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
            // ???????????????????
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
    }
    void UpdateSortIcons()
    {
     /*   for (int i = 0; i < topRow.sortButtons.Count; i++)
        {
            SortField field = (SortField)i;
            Image icon = topRow.sortButtons[i].GetComponent<Image>();

            if (currentSort.Field == field && currentSort.Direction != SortDirection.None)
            {
                if (field == SortField.Name)
                {
                    if (currentSort.Direction == SortDirection.Ascending)
                    {
                        icon.sprite = GetSortSprite(SortDirection.Descending.ToString());
                    }
                    else if (currentSort.Direction == SortDirection.Descending)
                    {
                        icon.sprite = GetSortSprite(SortDirection.Ascending.ToString());
                    }
                }
                else
                {
                    icon.sprite = GetSortSprite(currentSort.Direction.ToString());
                }
            }
            else
            {
                icon.sprite = GetSortSprite(SortDirection.None.ToString());
            }
        }*/
    }
    void OnLocaleChanged(UnityEngine.Localization.Locale newLocale)
    {
        if (currentSort.Field == SortField.Name && currentSort.Direction != SortDirection.None)
        {
            ApplySort(currentSort.Field, currentSort.Direction);
        }
        UpdateCharacterDisplay(); 

    }
    #endregion


    void Start()
    {
        if (starButton != null) starButton.onClick.AddListener(() => SetStar());


        for (int i = 0; i < ValueTypeButtons.Count; i++)
        {
            int index = i;
            ValueTypeButtons[i].onClick.AddListener(() => SetValueType(index));
        }

        ValueTypeButton.onClick.AddListener(SetValueTypeByButton);

        SetValueType(valueType);

        //  AddEventTrigger(ForceTypeImage.gameObject); what is this function do???


        for (int i = 0; i < ForceTypeButtons.Count; i++)
        {
            int index = i;
            ForceTypeButtons[i].onClick.AddListener(() => SetTheForceTypeByButton(index));
        }


        for (int i = 0; i < FavorabilityTypeButtons.Count; i++)
        {
            int index = i;
            if (FavorabilityTypeButtons[i] != null)
            {
                FavorabilityTypeButtons[i].onClick.AddListener(() => SetFavorabilityType((FavorabilityLevel)index));
                FavorabilityTypeButtons[i].gameObject.GetComponent<Image>().sprite = FavorabilityTypeSprites[index];
            }
        }

        if (FavorabilityTypeButton != null)
            FavorabilityTypeButton.onClick.AddListener(() => SetFavorabilityTypeByButton());

    }

    void SetStar()
    {
        isStar = !isStar;
        string iconPath = $"MyDraw/UI/Other/";
        if (isStar)
        {
            starButton.gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>(iconPath + "Star");
        }
        else
        {
            starButton.gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>(iconPath + "Unstar");
        }
        UpdateCharacterDisplay();

    }



    void SetValueTypeByButton(){
        
        if ((valueType + 1) == 3) {SetValueType(0);}
        else {SetValueType(valueType + 1);};
    }


    void SetFavorabilityTypeByButton()
    {
        SetFavorabilityType(EnumHelper.GetEnumByOffset<FavorabilityLevel>(FavorabilityType, 1));
    }

    void Update()
    {
        UpdateValuePanelVisibility();
        UpdateFavorabilityPanelVisibility();
    }


    void UpdateValuePanelVisibility()
    {
        bool isHoveringValue =
            IsMouseOverUI(ValueTypeButton.GetComponent<RectTransform>()) ||
            (valueButtonsPanel.activeSelf && IsMouseOverUI(valueButtonsPanel.GetComponent<RectTransform>()));

        valueButtonsPanel.SetActive(isHoveringValue);
    }

    void UpdateFavorabilityPanelVisibility()
    {
        if (FavorabilityButtonPanel == null) return;

        bool isHoveringFavorability =
            IsMouseOverUI(FavorabilityTypeButton.GetComponent<RectTransform>()) ||
            IsMouseOverUI(FavorabilityImage.GetComponent<RectTransform>()) ||
            (FavorabilityButtonPanel.activeSelf && IsMouseOverUI(FavorabilityButtonPanel.GetComponent<RectTransform>()));

        FavorabilityButtonPanel.SetActive(isHoveringFavorability);
    }




    bool IsMouseOverUI(RectTransform rect)
    {
        return RectTransformUtility.RectangleContainsScreenPoint(
            rect,
            Input.mousePosition,
            null // ??? Screen Space - Overlay UI?? null
        );
    }
    private void OnEnable()
    {
        SetValueType(valueType);
        ClearIsSort();
    }


    void SetValueType(int valueType){
        this.valueType = valueType;
        if (valueType == 0){
            SetValueIcon(battleValueSprite);
        } else if (valueType == 1){
            SetValueIcon(ParameterValueSprite);    
        }else if (valueType == 2){
            SetValueIcon(HelpValueSprite);
        }
        ClearIsSort();
        for (int i = 0; i < characterColumControls.Count; i++)
        {
            var columnControl = characterColumControls[i];
            columnControl.SetTheType(valueType);
        }
        ValueTypeButton.gameObject.GetComponent<Image>().sprite = GetCharacterValueSprite(valueType);
    }

    void SetFavorabilityType(FavorabilityLevel favorabilityType)
    {
        if (this.FavorabilityType == favorabilityType) return;
        this.FavorabilityType = favorabilityType;
      /*  for (int i = 0; i < characterColumControls.Count; i++)
        {
              var columnControl = characterColumControls[i];
            if (FavorabilityType == 1) { 
                columnControl.gameObject.SetActive(true); }
            else
            {
                if (columnControl.character.favorabilityLevel == 1) { columnControl.gameObject.SetActive(true); }
                else { columnControl.gameObject.SetActive(columnControl.character.favorabilityLevel == FavorabilityType); }
            }
        }*/
        FavorabilityImage.sprite = GetFavorabilitySprite(FavorabilityType);
        UpdateCharacterDisplay();
        //isSort[0] = 0;
        // sortButton[0].GetComponent<Image>().sprite = NoSortSprite;
        //  ClearIsSort();
    }


    void SetValueIcon(List<Sprite> valueSprite){
    if (valueSprite != null && valueSprite.Count >= 5) {
        attackIcon.sprite = valueSprite[0];
        defenseIcon.sprite = valueSprite[1];
        magicIcon.sprite = valueSprite[2];
        speedIcon.sprite = valueSprite[3];
        luckyIcon.sprite = valueSprite[4];
    }
    }

    void UpdateButtonIcons(int activeType, List<int> isSort)
    {

     /*   //NoSortSprite
        for (int i = 0; i < sortButton.Count; i++)
        {
            if (activeType != i) sortButton[i].gameObject.GetComponent<Image>().sprite = NoSortSprite;
            if (activeType == i)
            {
                if (isSort[i] == 0) sortButton[i].gameObject.GetComponent<Image>().sprite = SToBSortSprite;
                if (isSort[i] == 1) sortButton[i].gameObject.GetComponent<Image>().sprite = BToSSortSprite;
            }
        }*/

    }

    void ClearIsSort()
    {
    /*    for (int i = 0; i < isSort.Count; i++)
        {
            isSort[i] = 0;
            if (i < sortButton.Count) sortButton[i].gameObject.GetComponent<Image>().sprite = NoSortSprite;
        }
    */
    }



    void SortValue(int type, List<int> isSort)
    {
        if (type == 0) 
        {
            SortByForce(isSort[type] == 1);
        }else if (type == 1) {
            SortByAttack(isSort[type] == 1);
        }else if (type == 2) {
            SortByDefense(isSort[type] == 1);
        }else if (type == 3) {
            SortByMagic(isSort[type] == 1);
        }else if (type == 4) {
            SortBySpeed(isSort[type] == 1);
        }else if (type == 5) {
            SortByLucky(isSort[type] == 1);
        } else if (type == 6){
            SortByfavorbility(isSort[type] == 1);
        }

        isSort[type] = 1 - isSort[type];


        for (int i = 0 ; i < isSort.Count ; i++){
            if (i != type) {
            isSort[i] = 0; 
            }
        }
        
        UpdateButtonIcons(type, isSort);

    }


    void SortByForce(bool ascending)
    {
       if (ForceType == 0){
            characterColumControls = ascending ?
                characterColumControls.OrderBy(c => c.character.Force).ToList() :
                characterColumControls.OrderByDescending(c => c.character.Force).ToList();
        } else if (ForceType == 1){
            characterColumControls = ascending ?
                characterColumControls.OrderBy(c => c.character.Health).ToList() :
                characterColumControls.OrderByDescending(c => c.character.Health).ToList();
        } else if  (ForceType == 2){
            characterColumControls = ascending ?
                characterColumControls.OrderBy(c => c.character.GetMaxLimit()).ToList() :
                characterColumControls.OrderByDescending(c => c.character.GetMaxLimit()).ToList();
        }
        UpdateCharacterDisplay();
    }

    void SortByfavorbility(bool ascending)
    {

        if (panelType == 1){
            characterColumControls = ascending ?
                characterColumControls.OrderBy(c => c.character.Favorability).ToList() :
                characterColumControls.OrderByDescending(c => c.character.Favorability).ToList();
        } else if (panelType == 2){
            characterColumControls = ascending ?
                characterColumControls.OrderBy(c => c.character.RecruitCost).ToList() :
                characterColumControls.OrderByDescending(c => c.character.RecruitCost).ToList();
        }

        UpdateCharacterDisplay();
    }

    void SortByAttack(bool ascending)
    {
        characterColumControls = ascending ?
characterColumControls.OrderBy(c => c.character.GetValue(valueType, 0)).ToList() :
characterColumControls.OrderByDescending(c => c.character.GetValue(valueType, 0)).ToList();
characterColumControls.OrderByDescending(c => c.character.GetValue(valueType, 0)).ToList();

        UpdateCharacterDisplay();
    }

    void SortByDefense(bool ascending)
    {

        characterColumControls = ascending ?
characterColumControls.OrderBy(c => c.character.GetValue(valueType, 1)).ToList() :
characterColumControls.OrderByDescending(c => c.character.GetValue(valueType, 1)).ToList();

        UpdateCharacterDisplay();
    }

    void SortByMagic(bool ascending)
    {
        characterColumControls = ascending ?
characterColumControls.OrderBy(c => c.character.GetValue(valueType, 2)).ToList() :
characterColumControls.OrderByDescending(c => c.character.GetValue(valueType, 2)).ToList();



        UpdateCharacterDisplay();
    }

    void SortBySpeed(bool ascending)
    {
        characterColumControls = ascending ?
characterColumControls.OrderBy(c => c.character.GetValue(valueType, 3)).ToList() :
characterColumControls.OrderByDescending(c => c.character.GetValue(valueType, 3)).ToList();


        UpdateCharacterDisplay();
    }

    void SortByLucky(bool ascending)
    {

        characterColumControls = ascending ?
    characterColumControls.OrderBy(c => c.character.GetValue(valueType - 1,4)).ToList() :
    characterColumControls.OrderByDescending(c => c.character.GetValue(valueType - 1, 4)).ToList();

        UpdateCharacterDisplay();
    }


    void UpdateCharacterDisplay()
    {
        CharacterColumnControl PlayerCharacterColumControl = characterColumControls[0];
        for (int i = 0; i < characterColumControls.Count; i++)
        {
            var columnControl = characterColumControls[i];
            columnControl.transform.SetSiblingIndex(i);

            bool passStarFilter = !isStar || columnControl.character.Star;

            bool passFavorabilityFilter = FavorabilityType == FavorabilityLevel.Self || columnControl.character.FavorabilityLevel == FavorabilityType;
            if (columnControl.character.FavorabilityLevel == FavorabilityLevel.Self)
            {
                passFavorabilityFilter = true;
                PlayerCharacterColumControl = characterColumControls[i];
            }

            columnControl.gameObject.SetActive(passStarFilter && passFavorabilityFilter);

        }


      /*  for (int i = 0; i < isSort.Count; i++)
        {
            if (isSort[i] == 1) return;
        }*/

        PlayerCharacterColumControl.transform.SetSiblingIndex(0);
        characterColumControls.Remove(PlayerCharacterColumControl);
        characterColumControls.Insert(0, PlayerCharacterColumControl);

    }
}
