using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;
using static GetSprite;
using static GetColor;

public class CharacterAssistPreFabControl : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler
{
    public Image CharacterAssist;
    public Image CharacterIcon;
    public Image CharacterClass;
    public TMP_Text CharacterName;
    public Character character;

    public Image ValueImage;
    public TMP_Text CharacterValue;

    public List<Sprite> ValueTypes;

    public CharacterAssistRowControl characterAssistRowControl;

    public Button selectButton;

    public bool isSelected;

    public Color32 SelectColor;
    public Color32 canActiveColor;
    public Color32 cantActiveColor;

    public Image stateIcon;
    int type = 0;


    void OnEnable()
    {
        LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
    }

    void OnDisable()
    {
        LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;
        this.character.OnCharacterChanged -= UpdataUI;

    }

    private void OnLocaleChanged(Locale locale)
    {
        SetCharacterName(); // ???? locale.Identifier ??????
        SetStateIntro();
        SetRoleClassIntro();
    }


    private void Start()
    {
        selectButton.onClick.AddListener(() => SetSelect());

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!character.CanMove()) { CharacterAssist.color = cantActiveColor; }
        else  { CharacterAssist.color = SelectColor; }
    }



    public void OnPointerExit(PointerEventData eventData)
    {
        SetColor();
    }


    public void SetTheNegotiation(Character character)
    {
        type = 1;
        SetTheBase(character);

    }

    public void SetTheBuild(Character character)
    {
        type = 2;
        SetTheBase(character);
    }

    public void SetTheScout(Character character)
    {
        type = 3;
        SetTheBase(character);
    }

    void SetColor()
    {
        if (!character.CanMove()) { CharacterAssist.color = cantActiveColor; }
        else if (isSelected) { CharacterAssist.color = SelectColor; }
        else { CharacterAssist.color = canActiveColor; }
    }




    void SetTheBase(Character character)
    {
        this.character = character;
        this.character.OnCharacterChanged += UpdataUI;
        UpdataUI();
    }

    void UpdataUI()
    {
        CharacterIcon.sprite = character.icon;
        SetRoleClassIntro();
        CharacterClass.sprite = GetRoleSprite(character.RoleClass);
        SetCharacterName();
        SetColor();
        SetState();
        CharacterValue.text = GetColorString(character.GetValue(2, type).ToString() + " + " + (((double)character.GetValue(2, 4)) / 10).ToString(), 2, type);
        ValueImage.sprite = GetHelpValueSpirte(type);


    }

    void SetCharacterName()
    {
        CharacterName.text = character.GetCharacterName();

    }

    void SetSelect()
    {
        if (!character.CheckCanMove()) return;
        if (character.IsMoved || character.HasLord()) return;
        if (isSelected)
        {
            characterAssistRowControl.SelectedCharacters.Remove(character);
            isSelected = false;
        }
        else
        {
            characterAssistRowControl.SelectedCharacters.Add(character);
            isSelected = true;
        }
        SetColor();
        characterAssistRowControl.ValueUpdate();
    }

    void SetState()
    {
        if (stateIcon == null) return;
        stateIcon.gameObject.SetActive(true);
        if (character.IsMoved)
        {
            stateIcon.sprite = GetCharacterStateSprite("cantmove");
        }
        else if (character.HasLord())
        {
            stateIcon.sprite = GetCharacterStateSprite("lord");
        }
        else
        {
            stateIcon.gameObject.SetActive(false);
        }
        SetStateIntro();
    }


    void SetStateIntro()
    {
        if (character.IsMoved)
        {
            stateIcon.gameObject.GetComponent<IntroPanelShow>().SetIntroName("Moved");
        }
        else if (character.HasLord())
        {
            stateIcon.gameObject.GetComponent<IntroPanelShow>().SetIntroName(character.GetLordRegion(0).GetRegionNameWithColor());
        }
        else
        {
            stateIcon.gameObject.GetComponent<IntroPanelShow>().SetIntroName(null);
        }

    }

    void SetRoleClassIntro()
    {
        CharacterClass.gameObject.GetComponent<IntroPanelShow>().SetIntroName(character.GetClassRoleString());
    }



}
