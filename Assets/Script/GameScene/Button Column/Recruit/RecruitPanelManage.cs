using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using static BattlePanelManage;
using static GetColor;
using static GetSprite;
using static GetString;
using static FormatNumber;
using static PanelExtensions;
using TMPro;
using System;
using UnityEngine.EventSystems;
using UnityEngine.TextCore.Text;
using UnityEngine.InputSystem.Utilities;
public class RecruitPanelManage : PanelBase, IPointerEnterHandler, IPointerExitHandler
{
    #region variable
    public static RecruitPanelManage Instance { get; private set; }


    public ScrollRect scrollRect;
    public GameObject characterRowControPrefab;

    public CharacterColumnControl characterRowControl;
    public ObservableList<CharacterColumnControl> selCharacterRowControls = new ObservableList<CharacterColumnControl>();

    private List<CharacterColumnControl> characterRowControls;
    float moveCooldown = 0.2f;
    float lastMoveTime = 0f;

    private bool isMouseOverPanel = false;

    public Button closeButton;
    public TextMeshProUGUI TotalCostText;
    public Button GetButton;

    [Header("Character Info")]
    public CharacterInfoPanel characterInfoPanel;


    #region ChracterInfoPanel
    [System.Serializable]
    public class CharacterInfoPanel
    {
        public Image charcterImage;
        public TMP_Text characterNamText;
        //public Image favorabilityImage;
        // public TMP_Text favorabilityText;
        public Image CostImage;
        public TextMeshProUGUI CostText;
        public Image roleIcon;
       // public TMP_Text MaxForceLimitText;
      //  public TMP_Text LeaderShipText;
       // public TMP_Text CharmText;
      //  public Image characterForceImage;
       // public Image characterMAXForceImage;// may be can delet it
        public int valueType = 0; // 0 is battle, 1 is parameter, 2 is Help
        public List<Image> valueImages;
        public List<TextMeshProUGUI> valueTexts;
        public List<SkillButtonControl> skillButtons;
        public Image itemImage;
        public Character characterAtPanel;

        public List<Button> valueTypeButtons;

       // public TextMeshProUGUI ForceText;

       // public TextMeshProUGUI MaxForceText;

        public Button starButton;
        public Image personBattleImage;
        public Button deletButton;


        public Image moveImage;
        public TextMeshProUGUI moveText;

        public TMP_Text MaxForceLimitText;
        public TMP_Text HealthText;

        public TMP_Text LvText;
        public TMP_Text ForceText;

        public void InitButtons()
        {

            for (int j = 0; j < valueTypeButtons.Count; j++)
            {
                int index = j;
                valueTypeButtons[j].onClick.AddListener(() => SetValueType(index));
            }
            starButton.onClick.AddListener(OnStarButtonClick);
            deletButton.onClick.AddListener(OnDeletButtonClick);
        }

        void OnStarButtonClick()
        {
            characterAtPanel.Star = !characterAtPanel.Star;
            SetStarSprite(characterAtPanel.Star);
        }

        void OnDeletButtonClick()
        {
            Instance.RemoveCharacter();
        }


        void SetStarSprite(bool isStar)
        {
            starButton.interactable = characterAtPanel != null;
            starButton.gameObject.GetComponent<Image>().sprite = UpStarButtonSprite(isStar);
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

        public void SetItem(ItemBase itemBase)
        {
            characterAtPanel.SetItem(itemBase);
            SetCharacterValueAtPanel(characterAtPanel);
        }
        public void SetCharacterValueAtPanel(Character character)
        {
            if (characterAtPanel != null) characterAtPanel.OnCharacterChanged -= RefreshCharacterUI;
            characterAtPanel = character;
            if (characterAtPanel != null) characterAtPanel.OnCharacterChanged += RefreshCharacterUI;
            RefreshCharacterUI();
        }


        void RefreshCharacterUI()
        {
            if (characterAtPanel != null)
            {
                CharacterPanelValueActive(true);
                charcterImage.sprite = characterAtPanel.image;
                characterNamText.text = characterAtPanel.GetCharacterName();
                roleIcon.sprite = GetRoleSprite(characterAtPanel.RoleClass);
                //  favorabilityImage.sprite = GetFavorabilitySprite(characterAtPanel.FavorabilityLevel);
                CostText.text = GetColorString($"<b>{characterAtPanel.RecruitCost.ToString("N0")}</b>", 2, 3);


                moveImage.sprite = GetCharacterStateSprite("cantmove");
                moveText.text = GetValueColorString($"* {characterAtPanel.BattleMoveNum}", ValueColorType.CantMove);

                MaxForceLimitText.text = GetValueColorString($"{characterAtPanel.GetMaxLimit()}", ValueColorType.Pop);
                HealthText.text = characterAtPanel.GetHealthAndMaxHealthString();

                LvText.text = characterAtPanel.GetLvAndMaxLevelString();
                ForceText.text = GetValueColorString($"{characterAtPanel.Force} / {characterAtPanel.MaxForce}", ValueColorType.Pop);

                personBattleImage.sprite = UpBattleButtonSprite(characterAtPanel.IsPersonBattle);
                deletButton.gameObject.SetActive(characterAtPanel.CanDelete);


                SetValueType(valueType);
                SetSkillNames();
                if (itemImage != null)
                {
                    itemImage.sprite = characterAtPanel.GetItemWithCharacterSprite();
                }
                SetStarSprite(characterAtPanel.Star);
            }
            else
            {
                ResetPanel();

            }
        }

        void ResetPanel()
        {

            CharacterPanelValueActive(false);

            characterAtPanel = null;
            characterNamText.text = "NONE";
            roleIcon.sprite = null;
            MaxForceLimitText.text = GetValueColorString("0", ValueColorType.Pop); 
            HealthText.text = GetValueColorString("0 / 0", ValueColorType.Pop);

            LvText.text = $"Lv.{0} / {0}";
            ForceText.text = GetValueColorString("0 / 0", ValueColorType.Pop);

            SetValueTypeEmptyText();

            SetSkillNames();
            SetStarSprite(false);
        }
        void CharacterPanelValueActive(bool isActive)
        {
            //  favorabilityImage?.gameObject.SetActive(isActive);
            CostImage.gameObject.SetActive(isActive);
            CostText?.gameObject.SetActive(isActive);

            charcterImage?.gameObject.SetActive(isActive);
            characterNamText?.gameObject.SetActive(isActive);
            roleIcon?.gameObject.SetActive(isActive);
            itemImage?.gameObject.SetActive(isActive);

            moveImage.gameObject.SetActive(isActive);
            moveText.gameObject.SetActive(isActive);


            personBattleImage.gameObject.SetActive(isActive);
            deletButton.gameObject.SetActive(isActive);

    }
    void SetSkillNames()
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
        public List<Button> sortButtons;

      //  public Image favorabilityTypeButton;

        public int forceType = 2;
        public Button forceTypeButton;
        public List<Button> forceTypeButtons;

        public int valueType = 0;
        public Button valueTypeButton;
        public GameObject valueTypeButtonsPanel;
        public List<Button> valueTypeButtons;
        public List<Image> valueIcons;

        public Image itemImage;

        public void SetValueType(int valueType)
        {
            this.valueType = valueType;
            for (int i = 0; i < valueIcons.Count; i++)
            {
                valueIcons[i].sprite = GetCharacterValueSpritesData(valueType, i);
            }
            valueTypeButton.gameObject.GetComponent<Image>().sprite = GetCharacterValueSprite(valueType);

        }

        public void SetForceType(int foreceType)
        {
            this.forceType = foreceType;
            forceTypeButton.gameObject.GetComponent<Image>().sprite = forceTypeButtons[forceType].gameObject.GetComponent<Image>().sprite;
        }


    }
    //public CharacterAssistRowControl characterAssistRowControl;
    #endregion
    public enum SortField
    {
        Name, RecruitCost, Force, Health, Limit, Attack, Food, Leadership, Defense, Science, Scout, Magic, Politics, Build, Speed, Gold, Negotiation, Lucky, Faith, Charm, None
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

    List<SortField> activeSortFields;
    private SortStatus currentSort;

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
        selCharacterRowControls.OnListChanged -= UpdateCostText;
        GameValue.Instance.UnRegisterItemsChange(HasItem);


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
    }


    private void Start()
    {
        //  GameValue.Instance.PlayerCharacters.OnListChanged += DisplayCharacter;
        GameValue.Instance.RegisterItemsChange(HasItem);
        HasItem();

        selCharacterRowControls.OnListChanged += UpdateCostText;
        InitTopRow();
        DisplayCharacter();
        characterInfoPanel.SetCharacterValueAtPanel(null);
        characterInfoPanel.InitButtons();
        GetButton.onClick.AddListener(() => ToggleRow());
        closeButton.onClick.AddListener(ClosePanel);
    }




    void InitTopRow()
    {
        characterPanelTopRow.SetValueType(0);
     //   characterPanelTopRow.SetForceType(characterPanelTopRow.forceType);
        InitSortButtons();
        InitTopRowButtons();
    }

    #region Sort

    void InitSortButtons()
    {
        activeSortFields = new List<SortField>();

        SortField f0 = SortField.Name;
        SortField f1 = SortField.RecruitCost;
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

            characterRowControls = isAscending
                ? characterRowControls.OrderBy(c => keySelector(c.character)).ToList()
                : characterRowControls.OrderByDescending(c => keySelector(c.character)).ToList();

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
            SortField.RecruitCost => c => c.RecruitCost, // 如果需要 fallback 处理
            _ => c => -c.GetCharacterID()
        };
    }




    void InitTopRowButtons()
    {
        characterPanelTopRow.starButton.onClick.AddListener(() => SetTopRowStar());


        characterPanelTopRow.valueTypeButton.onClick.AddListener(SetValueTypeByButton);
        characterPanelTopRow.forceTypeButton.onClick.AddListener(SetForceTypeByButton);

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
            case 1: return SortField.Food;
            case 2: return SortField.Leadership;
        }
        return SortField.Attack;
    }

    SortField GetDefenseField()
    {
        switch (characterPanelTopRow.valueType)
        {
            case 0: return SortField.Defense;
            case 1: return SortField.Science;
            case 2: return SortField.Scout;

        }
        return SortField.Defense;
    }


    SortField GetMagicField()
    {
        switch (characterPanelTopRow.valueType)
        {
            case 0: return SortField.Magic;
            case 1: return SortField.Politics;
            case 2: return SortField.Build;

        }
        return SortField.Magic;
    }


    SortField GetSpeedField()
    {
        switch (characterPanelTopRow.valueType)
        {
            case 0: return SortField.Speed;
            case 1: return SortField.Gold;
            case 2: return SortField.Negotiation;

        }
        return SortField.Speed;
    }


    SortField GetLuckyField()
    {
        switch (characterPanelTopRow.valueType)
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

    void SetForceTypeByButton()
    {

        if ((characterPanelTopRow.forceType + 1) == 3) { SetForceType(0); }
        else { SetForceType(characterPanelTopRow.forceType + 1); }
    ;
        UpdateSortIcons();


    }


    void SetForceType(int forceType)
    {
        characterPanelTopRow.SetForceType(forceType);
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


    void SetValueType(int valueType)
    {
        characterPanelTopRow.SetValueType(valueType);
        for (int i = 0; i < characterRowControls.Count; i++)
        {
            var columnControl = characterRowControls[i];
            columnControl.SetTheType(valueType);
        }
        UpdateSortIcons();
    }


    public void SetCharacterColumnControl(CharacterColumnControl target)
    {
        if (target == null)
        {
            characterRowControl = null;
            characterInfoPanel.SetCharacterValueAtPanel(null);
            UpdateCostText();
            return;
        }

        if (selCharacterRowControls.Contains(target))
        {
            selCharacterRowControls.Remove(target);
            if (characterRowControl == target)
            {
                characterRowControl = null;
                characterInfoPanel.SetCharacterValueAtPanel(null);

            }
        }
        else
        {
            selCharacterRowControls.Add(target);
            characterRowControl = target;
            Character character = target?.character;
            characterInfoPanel.SetCharacterValueAtPanel(character);

        }
        UpdateCostText();
    }

    void ClearCharacterColumnControl()
    {
        characterRowControl = null;
        characterInfoPanel.SetCharacterValueAtPanel(null);
        foreach (var row in selCharacterRowControls)
        {
            row.isSelected = false;
            row.SetRowColor();
        }
        selCharacterRowControls.Clear();
    }

    void UpdateCostText()
    {
        int TotolCost = 0;
        foreach (var row in selCharacterRowControls)
        {
            TotolCost += row.character.RecruitCost;
        }

      //  TotalCostText.text = TotolCost.ToString("N0");


          if (characterRowControl != null)
          {
            //   TotalCostText.text = $"{TotolCost - characterRowControl.character.RecruitCost} + {characterRowControl.character.RecruitCost}";
            TotalCostText.text = GetColorString($"<b>{TotolCost.ToString("N0")}</b> \n(+{characterRowControl.character.RecruitCost})", 2, 3);
                Debug.Log("need change totoal cost color");

          } else
          {
            TotalCostText.text = GetColorString($"<b>COST</b>", 2, 3);
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
        /*
          if (isMouseOverPanel && Input.GetKey(KeyCode.UpArrow) && Time.time - lastMoveTime > moveCooldown)
          {
              lastMoveTime = Time.time;
              MoveSelCharaceterRowControl(1);
          }
          if (isMouseOverPanel && Input.GetKey(KeyCode.DownArrow) && Time.time - lastMoveTime > moveCooldown)
          {
              lastMoveTime = Time.time;
              MoveSelCharaceterRowControl(-1);
          }*/

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
                  SetCharacterColumnControl(controls[index]);
                  return;
              }

          } while (index != start);
      }
      public override void OpenPanel()
      {
          base.panel.transform.localPosition = Vector3.zero;
          UpdateCostText();
        base.panel.SetActive(true);
      }

    public override void SetPanelSaveData(PanelSaveData panelSaveData)
    {
        SetSaveData(this,base.panel, panelSaveData);
    }



    public void RemoveCharacterRow(CharacterColumnControl removeCharacterRow)
      {
        if (removeCharacterRow == characterRowControl)
        characterRowControls.Remove(removeCharacterRow);
        if (removeCharacterRow.character == characterInfoPanel.characterAtPanel)
        {
            characterInfoPanel.SetCharacterValueAtPanel(null);
        }
        CharacterColumnControl targetControl = selCharacterRowControls.FirstOrDefault(c => c.character == removeCharacterRow.character);
        if (targetControl != null)
        {
            selCharacterRowControls.Remove(targetControl);
        }
        Destroy(removeCharacterRow.gameObject);

      }

    public void RemoveCharacter()
    {
        Debug.Log("maybe need Randomly assign it to a country");
        characterRowControl.character.SetCountry(CharacterConstants.DIE);
        characterRowControls.Remove(characterRowControl);
        selCharacterRowControls.Remove(characterRowControl);
        Destroy(characterRowControl.gameObject);
        SetCharacterColumnControl(null);
        ApplySort(currentSort.Field, currentSort.Direction);

    }

    public override void ClosePanel()
      {
          base.panel.transform.position = Vector2.zero;
          ClearCharacterColumnControl();
          base.panel.SetActive(false);
      }

    public void DisplayCharacter()
      {
          foreach (Transform child in scrollRect.content)//scrollContent)
          {
              Destroy(child.gameObject);
          }

        // Character[] foundCharacterComponents = GameObject.FindObjectsOfType<Character>();
        characterRowControls.Clear();
        GameValue gameValue = GameValue.Instance;
          List<Character> CaptureCharacters = gameValue.GetCountryCharacters(CharacterConstants.Capture);
          foreach (var character in CaptureCharacters)
          {
                DisplayCharacterPanel(character);
          }

        SetForceType(characterPanelTopRow.forceType);
        LayoutRebuilder.ForceRebuildLayoutImmediate(scrollRect.content.GetComponent<RectTransform>());
      }



      bool ShouldDisplayCharacter(Character character)
      {
          return (character != null) && (character.GetCountryENName() == CharacterConstants.Capture);
      }

      void DisplayCharacterPanel(Character character)
      {
          GameObject characterPrefab = Instantiate(characterRowControPrefab, scrollRect.content);// scrollContent);

          CharacterColumnControl characterRowControl = characterPrefab.GetComponent<CharacterColumnControl>();
          if (characterRowControl != null)
          {
              characterRowControl.CharacterColumnControlP(character);
              characterRowControl.type = 2;
          }
          characterRowControls.Add(characterRowControl);

      }

      void UpdateCharacterDisplay()
      {
          CharacterColumnControl playerCharacterControl = null;

          for (int i = 0; i < characterRowControls.Count; i++)
          {
              var columnControl = characterRowControls[i];
              columnControl.transform.SetSiblingIndex(i);

              bool passStarFilter = !characterPanelTopRow.isStar || columnControl.character.Star;

        columnControl.gameObject.SetActive(passStarFilter);
        }

        if (!currentSort.IsSorted && playerCharacterControl != null)
        {
            playerCharacterControl.transform.SetSiblingIndex(0);
            characterRowControls.Remove(playerCharacterControl);
            characterRowControls.Insert(0, playerCharacterControl);
        }
    }

    void ToggleRow()
    {
        float needValue = 0;

        GameValue gameValue = GameValue.Instance;
        foreach (var control in selCharacterRowControls)
        {
             needValue += control.character.RecruitCost;
        }


        if (needValue <= gameValue.GetResourceValue().Negotiation)
        {
            GameValue.Instance.GetResourceValue().Negotiation -= needValue;
            RecruitCharacter();

        }
        else if (needValue > gameValue.GetResourceValue().Negotiation)
        {
            //   NotificationManage.Instance.ShowToTop("Not enough Faith");
            NotificationManage.Instance.ShowToTopByKey(NotificationKeyConstants.NotEnoughResource,GetResourceStringWithSprite(ValueType.Faith));
            CharacterAssistRowControl.Instance.ToggleRow(SBNType.Negotiation, needValue,this);

        }
    }

    public void RecruitCharacter()
    {
        List<CharacterColumnControl> toRemove = new List<CharacterColumnControl>(selCharacterRowControls);
        foreach (var control in toRemove)
        {
            control.isSelected = false;
            //GameValue.Instance.AddPlayerCharacter(control.character);
            control.character.SetCountry(GameValue.Instance.GetPlayerCountryENName());
            control.character.IsMoved = true;
            characterRowControls.Remove(control);
            if (characterRowControl != null && characterRowControl.character == control.character)
                SetCharacterColumnControl(null);
        }
        selCharacterRowControls.Clear();
        DisplayCharacter();
        foreach (var control in toRemove)
        {
            if (control != null && control.gameObject != null)
                Destroy(control.gameObject);
        }
    }

    public override PanelSaveData GetPanelSaveData()
    {
        PanelSaveData saveData = GetSaveData(this,base.panel, PanelType.Recruit);
        return saveData;
    }

}
