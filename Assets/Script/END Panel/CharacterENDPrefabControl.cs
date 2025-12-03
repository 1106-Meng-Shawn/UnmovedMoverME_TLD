using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.TextCore.Text;


public class CharacterENDPrefabControl : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image CharacterIcon;
    private CharacterENDControl characterENDConrtrol;
    private Character character;
    private CharacterEND characterEND;


    public void SetCharacter( Character character, CharacterEND characterEND,CharacterENDControl characterENDControl)
    {
        if (character == null)
        {
            Debug.Log("CharacterENDPrefabControl character is null");
            return;
        }

        this.character = character;
        this.characterEND = characterEND;
        this.characterENDConrtrol = characterENDControl;


        if (!string.IsNullOrEmpty(this.characterEND.GetENDIcon()))
        {
            CharacterIcon.sprite = Resources.Load<Sprite>($"MyDraw/Character/{character.GetCharacterFileType()}/{this.characterEND.GetENDIcon()}");
        }
        else
        {
            if (this.characterEND.IsGE())
            {
                CharacterIcon.sprite = Resources.Load<Sprite>($"MyDraw/Character/{character.GetCharacterFileType()}/{character.GetCharacterFileType()}GEIcon");
            }
            else
            {
                CharacterIcon.sprite = Resources.Load<Sprite>($"MyDraw/Character/{character.GetCharacterFileType()}/{character.GetCharacterFileType()}BEIcon");
            }
        }

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        characterENDConrtrol.ShowENDDetail(CharacterIcon.sprite, character, characterEND);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        characterENDConrtrol.HideENDDetail();
    }
}
