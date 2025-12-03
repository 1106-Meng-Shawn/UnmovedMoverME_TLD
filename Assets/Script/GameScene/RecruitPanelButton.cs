using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.UI;


public class RecruitPanelButton : MonoBehaviour
{
 /*   public Button captureButton;

    public GameValue gameValue;

    public CharacterAssistRowControl characterAssistRowControl;
    public CharacterPanel characterPanel;

    public List<Button> ValueTypes;


    private void Start()
    {
        for (int i = 0; i < ValueTypes.Count; i++)
        {
            int index = i;  
            ValueTypes[i].onClick.AddListener(() => characterPanel.SetValueType(index));
        }


        captureButton.onClick.AddListener(() => ToggleRow());

    }

    private void Update()
    {
        if (gameValue == null) gameValue = GameObject.FindObjectOfType<GameValue>();
    }

    void ToggleRow()
    {
        int needValue = 0;

            if (characterPanel != null && characterPanel.selectedCharacterColumnControlList != null)
            {
                foreach (var control in characterPanel.selectedCharacterColumnControlList)
                {
                    needValue += control.character.RecruitCost;
                }
            }


        if (needValue <= gameValue.Negotiation)
        {
            gameValue.Negotiation -= needValue;

            if (characterPanel != null && characterPanel.selectedCharacterColumnControlList != null)
            {
                foreach (var control in characterPanel.selectedCharacterColumnControlList)
                {
                    control.isSelected = false;
                    control.character.Country = gameValue.PlayerCountry;
                    GameValue.Instance.PlayerCharacters.Add(control.character);
                    control.character.IsMoved = true;
                   
                    if (characterPanel.characterAtPanel == control.character) characterPanel.setCharacterValueAtPanel(null);
                }
                characterPanel.selectedCharacterColumnControlList.Clear();
                characterPanel.DisplayCharacter();
            }

        //    gameValue.UpdateCharacter();

        } else if (needValue > gameValue.Negotiation)
        {
        //    characterAssistRowControl.characterPanel = characterPanel;
            characterAssistRowControl.ToggleRow(1);

        }
    }*/
}
