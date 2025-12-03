using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Globalization;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization.Settings;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;
using static GetSprite;
using static GetColor;
using static BattlePanelManage;


public class CharacterPanel : MonoBehaviour//,IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
 /*   public int type;//1 is normal ,2 is recruit;


    public GameObject prefab;
    //  public Transform scrollContent;
    //public string characterNameAtPanel;
    public ScrollRect scrollRect;

    public Button characterCloseButton;

    public Image roleIcon;


    public Image charcterImage;
    public TMP_Text characterNamText;

    public Image FavorabilityImage;
    public TMP_Text FavorabilityText;


    public TMP_Text MaxForceLimitText;
    public TMP_Text LeaderShipText;
    public TMP_Text CharmText;

    public Image characterForceImage;

    public Image characterMAXForceImage;// may be can delet it
 
    public int ForceType; // 0 is force, 1 is health

    public TMP_Text characterMAXForceText;
    public TMP_Text characterForceText;


    public int ValueType = 0; // 0 is battle, 1 is parameter, 2 is Help

    public List<Image> ValueImages;

    public TextMeshProUGUI attackValueText;
    public TextMeshProUGUI defenseValueText;
    public TextMeshProUGUI magicValueText;
    public TextMeshProUGUI speedValueText;
    public TextMeshProUGUI luckyValueText;

    public TMP_Text skillName1;
    public TMP_Text skillName2;
    public TMP_Text skillName3;
    public TMP_Text skillName4;
    public TMP_Text skillName5;

    public Image itemImage;


    public Character characterAtPanel;
    public RegionInfo regionInfoUI;

    public CharacterPanelTopColumButton characterPanelTopColumButton;


    public CharacterColumnControl characterColumnControl;
    public List<CharacterColumnControl> selectedCharacterColumnControlList;

    public CharacterPanelButton characterPanelButton;

    float moveCooldown = 0.2f;
    float lastMoveTime = 0f;


    private void Start()
    {
     //   if (type == 1) { regionInfoUI = FindObjectOfType<RegionInfoUI>(); }
        if (characterCloseButton != null)
        {
            characterCloseButton.onClick.AddListener(() => TogglePanel());
        }
        ResetPanel();

    }


    private bool isMouseOverPanel = false; 

    public void OnPointerEnter(PointerEventData eventData)
    {
        isMouseOverPanel = true;  
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isMouseOverPanel = false;  
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if ( isMouseOverPanel && Input.GetMouseButtonDown(1))
        {
            TogglePanel();
        } 
    }

    void MoveSelCharaceterRowControl(int num)
    {
        if (type == 2) return;

        var controls = characterPanelTopColumButton.characterColumControls;

        if (controls == null || controls.Count == 0) return;

        if (characterColumnControl == null)
        {
            foreach (var ctrl in controls)
            {
                if (ctrl.gameObject.activeSelf)
                {
                    setcharacterColumnControl(ctrl);
                    return;
                }
            }
            return; 
        }

        int index = controls.IndexOf(characterColumnControl);
        if (index == -1) return;

        int start = index;
        do
        {
            index -= num;

            if (index < 0) index = controls.Count - 1;
            else if (index >= controls.Count) index = 0;

            if (controls[index].gameObject.activeSelf)
            {
                setcharacterColumnControl(controls[index]);
                //   ScrollToBottom(characterColumnControl.gameObject.transform as RectTransform, scrollRect);
                //   ScrollToBottom(scrollRect);
              //  ScrollToShowMinimal(characterColumnControl.gameObject.transform as RectTransform, scrollRect);
                return;
            }

        } while (index != start);



    }

    public void ScrollToShowMinimal(RectTransform target, ScrollRect scrollRect)
    {
        //bug can't work
        Canvas.ForceUpdateCanvases();

        RectTransform content = scrollRect.content;
        RectTransform viewport = scrollRect.viewport;

        float contentHeight = content.rect.height;
        float viewportHeight = viewport.rect.height;

        // 目标在 content 中的局部坐标（pivot 是左上，Y 向下为负）
        Vector2 localPos = target.localPosition;

        float targetTop = -localPos.y; // localPosition 是从 content pivot（上）往下算的
        float targetBottom = targetTop - target.rect.height;

        // 当前视口在 content 中的位置（0 = 顶部）
        float currentScrollTop = (1f - scrollRect.verticalNormalizedPosition) * (contentHeight - viewportHeight);
        float currentScrollBottom = currentScrollTop + viewportHeight;

        float newScrollTop = currentScrollTop;

        if (targetTop < currentScrollTop)
        {
            // 目标在视口上方 → 向上滚
            newScrollTop = targetTop;
        }
        else if (targetBottom > currentScrollBottom)
        {
            // 目标在视口下方 → 向下滚
            newScrollTop = targetBottom - viewportHeight;
        }
        else
        {
            // 目标完全在视口内，不移动
            return;
        }

        // 限制滚动位置在合理范围
        newScrollTop = Mathf.Clamp(newScrollTop, 0, contentHeight - viewportHeight);
        float newNormalized = 1f - (newScrollTop / (contentHeight - viewportHeight));

        scrollRect.verticalNormalizedPosition = newNormalized;
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
    }


    public void TogglePanel()
    {
        gameObject.transform.position = Vector2.zero;
        ResetPanel();
        gameObject.SetActive(false);
        if (characterPanelButton != null) characterPanelButton.SetCharacter(null);
    }

    private void OnEnable()
    {
        DisplayCharacter();


    }


    public void DisplayCharacter()
    {

        foreach (Transform child in scrollRect.content)//scrollContent)
        {
            Destroy(child.gameObject);
        }

        // Character[] foundCharacterComponents = GameObject.FindObjectsOfType<Character>();
        GameValue gameValue = FindObjectOfType<GameValue>();
        List<Character> allCharacters = gameValue.GetCurrentCharactersInGame();


        characterPanelTopColumButton.characterColumControls.Clear();

        foreach (var character in allCharacters)
        {
            if (ShouldDisplayCharacter(character))
            {
                DisplayCharacterPanel(character);
            }

        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(scrollRect.content.GetComponent<RectTransform>());  //scrollContent.GetComponent<RectTransform>());
    }

    bool ShouldDisplayCharacter(Character character)
    {
        return (character != null) &&
               ((character.Country == GameValue.Instance.PlayerCountry && type == 1) ||
               (character.Country == "Capture" && type == 2));
    }

    void DisplayCharacterPanel(Character character)
    {
        GameObject characterPrefab = Instantiate(prefab, scrollRect.content);// scrollContent);
        characterPrefab.tag = "Prefab";

        CharacterColumnControl characterColumControl = characterPrefab.GetComponent<CharacterColumnControl>();
        if (characterColumControl != null)
        {
            characterColumControl.CharacterColumnControlP(character);
            characterColumControl.type = type;
            characterColumControl.forceType = characterPanelTopColumButton.ForceType;
            characterPanelTopColumButton.characterColumControls.Add(characterColumControl);
        }
        if (character.FavorabilityLevel == 0) { 
            characterColumControl.transform.SetAsFirstSibling();
            characterPanelTopColumButton.characterColumControls.Remove(characterColumControl);
            characterPanelTopColumButton.characterColumControls.Insert(0, characterColumControl);

        }


    }

    public void SetValueType(int typeValue)
    {
        this.ValueType = typeValue;

        for (int i = 0; i < ValueImages.Count; i++)
        {
            ValueImages[i].sprite = GetCharacterValueSpritesData(ValueType, i);
        }

        if (ValueImages[0].sprite == null) Debug.Log("fuck");

        if (characterAtPanel != null)
        {
            attackValueText.text = GetColorString(characterAtPanel?.GetValue(ValueType, 0).ToString("F0"), ValueType, 0);
            defenseValueText.text = GetColorString(characterAtPanel?.GetValue(ValueType, 1).ToString("F0"), ValueType, 1);
            magicValueText.text = GetColorString(characterAtPanel?.GetValue(ValueType, 2).ToString("F0"), ValueType, 2);
            speedValueText.text = GetColorString(characterAtPanel?.GetValue(ValueType, 3).ToString("F0"), ValueType, 3);
            luckyValueText.text = GetColorString(characterAtPanel?.GetValue(ValueType, 4).ToString("F0"), ValueType, 4);

        } else
        {
            SetValueTypeEmptyText();
        }

    }

    void SetValueTypeEmptyText()
    {
        if (ValueType == 0)
        {
            attackValueText.text = GetColorString("A", ValueType, 0);
            defenseValueText.text = GetColorString("D", ValueType, 1);
            magicValueText.text = GetColorString("M", ValueType, 2);
            speedValueText.text = GetColorString("S", ValueType, 3);
            luckyValueText.text = GetColorString("L", ValueType, 4);
        }
        else if (ValueType == 1)
        {
            attackValueText.text = GetColorString("F", ValueType, 0);
            defenseValueText.text = GetColorString("S", ValueType, 1);
            magicValueText.text = GetColorString("P", ValueType, 2);
            speedValueText.text = GetColorString("G", ValueType, 3);
            luckyValueText.text = GetColorString("F", ValueType, 4);
        }
        else if (ValueType == 2) {
            attackValueText.text = GetColorString("L", ValueType, 0);
            defenseValueText.text = GetColorString("S", ValueType, 1);
            magicValueText.text = GetColorString("B", ValueType, 2);
            speedValueText.text = GetColorString("N", ValueType, 3);
            luckyValueText.text = GetColorString("C", ValueType, 4);

        }
    }

    public void SetForceType(int TypeForce){
        this.ForceType = TypeForce;
        //if (characterAtPanel == null ) return;



        MaxForceLimitText.text = GetValueColorString(characterAtPanel?.GetMaxLimit().ToString(), "pop"); ; 

        if(type ==2) {
            characterMAXForceText.text = characterAtPanel?.MaxHealth.ToString("N0");
            return;}

        LeaderShipText.text = GetValueColorString(characterAtPanel?.GetValue(2, 0).ToString(), "leadership") + GetValueColorString("*100", "pop");
        CharmText.text = GetValueColorString(characterAtPanel?.GetValue(2, 4).ToString(), "charm");
            


        if (TypeForce == 0){
            characterForceImage.sprite = Resources.Load<Sprite>($"MyDraw/UI/Region/Value/Population/RecruitmentPopulation");
            characterForceText.text = characterAtPanel?.Force.ToString("N0");

            characterMAXForceText.text = characterAtPanel?.MaxForce.ToString("N0");
        } else if (TypeForce == 1){
            characterForceImage.sprite = Resources.Load<Sprite>($"MyDraw/UI/Character/CharacterValueIcon/Health") ;  
            characterForceText.text = characterAtPanel?.Health.ToString("N0");

            characterMAXForceText.text = characterAtPanel?.MaxHealth.ToString("N0");

        }

    }

    public void SetItem(ItemBase itemBase)
    {
        characterAtPanel.SetItem(itemBase);
        setCharacterValueAtPanel(characterAtPanel);
    }



    public void setCharacterValueAtPanel(Character character)
    {
        characterAtPanel = character;

        if (character != null)
        {
            //            characterNameAtPanel = character.characterName;

            CharacterPanelValueActive(true);



            charcterImage.sprite = character.image;

            string currentLanguage = LocalizationSettings.SelectedLocale.Identifier.Code;
            characterNamText.text = character.GetCharacterName();

            roleIcon.sprite = GetRoleSprite(character.RoleClass);



            if (type == 1)FavorabilityImage.sprite = GetFavorabilitySprite(character.FavorabilityLevel);
            if (type == 1){FavorabilityText.text = character.Favorability.ToString();}
            else if( type == 2) {FavorabilityText.text = character.RecruitCost.ToString();}


            SetForceType(ForceType);

            SetValueType(ValueType);

            if (characterPanelButton != null) characterPanelButton.SetTheForceType();

            skillName1.text = character.CurrentSkillName(0);
            skillName2.text = character.CurrentSkillName(1);
            skillName3.text = character.CurrentSkillName(2);
            skillName4.text = character.CurrentSkillName(3);
            skillName5.text = character.CurrentSkillName(4);
            if (itemImage != null)
            {
                if (character.ItemWithCharacter != null)
                {
                    itemImage.sprite = character.ItemWithCharacter.icon;
                }
                else
                {
                    Sprite sprite = Resources.Load<Sprite>("MyDraw/Item/EmptyItem");
                    itemImage.sprite = sprite;
                }
            }
        } else
        {
                ResetPanel();

        }
        if (characterPanelButton != null)
        {
            characterPanelButton.SetCharacter(character);
        }
    }

    void ResetPanel()
    {

        CharacterPanelValueActive(false);

        characterAtPanel = null;
        characterNamText.text = "NONE";
        roleIcon.sprite = null;
        if (characterPanelButton != null) characterPanelButton.SetTheForceType();
        characterMAXForceText.text = "0";
        if (characterForceText != null) characterForceText.text = "0";


        MaxForceLimitText.text = GetValueColorString("0", "pop"); ;
        if (LeaderShipText != null) LeaderShipText.text = GetValueColorString("0", "leadership") + GetValueColorString("*100", "pop");
        if (CharmText != null)  CharmText.text = GetValueColorString("0", "charm");
        SetValueTypeEmptyText();
        skillName1.text = "NONE";
        skillName2.text = "NONE";
        skillName3.text = "NONE";
        skillName4.text = "NONE";
        skillName5.text = "NONE";

    }

    void CharacterPanelValueActive(bool isActive)
    {
        FavorabilityImage?.gameObject.SetActive(isActive);
       FavorabilityText?.gameObject.SetActive(isActive);

        charcterImage?.gameObject.SetActive(isActive);
        characterNamText?.gameObject.SetActive(isActive);
        roleIcon?.gameObject.SetActive(isActive);
        itemImage?.gameObject.SetActive(isActive);

    }

    public void setcharacterColumnControl(CharacterColumnControl characterColumnControl)
    {
        RestoreColor();
        this.characterColumnControl = characterColumnControl;
        if (characterColumnControl == null)
        {
            setCharacterValueAtPanel(null);
            ResetPanel();
        } else
        {
            this.characterColumnControl.Highlight(true);
            setCharacterValueAtPanel(characterColumnControl.character);
        }
        ChangeColorColumnSelect();
    }

    void ChangeColorColumnSelect()
    {
        if (characterColumnControl != null)
        {
            characterColumnControl.gameObject.GetComponent<Image>().color = GetRowColor("Sel");
        }
    }

    void RestoreColor()
    {
        if (characterColumnControl != null)
        {
            characterColumnControl.RestoreColor();
            characterColumnControl.Highlight(false);

        }
    }*/
}
