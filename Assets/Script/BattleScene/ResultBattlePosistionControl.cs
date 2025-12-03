using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;
using static GetSprite;
using static GetColor;
using UnityEngine.TextCore.Text;

public class ResultBattlePosistionControl : MonoBehaviour
{
    // Start is called before the first frame update
    public TMP_Text CharacterNameText;
    public Image CharacterIcon;
    public Image CharacterClass;
    public TMP_Text ForceText;
    public TMP_Text StatusText;


    public GameObject Status;



    public void SetResultBattlePosistion(PositionAtBattle positionAtBattle,bool canCapture)
    {
        BattleCharacterValue battleCharacterValue = positionAtBattle.GetBattleCharacterValue();
        if (battleCharacterValue != null)
        {
            Character character = battleCharacterValue.characterValue;
            CharacterNameText.text = battleCharacterValue.GetBattleCharacterName();
            CharacterIcon.sprite = character.icon;
            CharacterClass.gameObject.SetActive(true);
            CharacterClass.sprite = GetRoleSprite(character.RoleClass);
            if (GameValue.Instance.GetBattleData().isExplore)
            {
                ForceText.text = GetValueColorString($"{character.Health.ToString("N0")} / {character.GetMaxHealthString()}", ValueColorType.Pop);
            } else
            {

                ForceText.text = GetValueColorString($"{character.Force.ToString("N0")} / {character.MaxForce.ToString("N0")}", ValueColorType.Pop);

            }

            SetStatus(character, positionAtBattle.isEnemy, canCapture);
        }
        else 
        {
            CharacterNameText.text = "NONE";
            CharacterIcon.sprite = GetCharacterIconSprite();
            CharacterClass.gameObject.SetActive(false);
            ForceText.text = GetValueColorString($"0 / 0", ValueColorType.Pop);
            Status.SetActive(false);
        }

    }


    void SetStatus(Character character, bool isEnemy,bool canCapture)
    {
        if (isEnemy && canCapture)
        {
            int recruit = Random.Range(0, 100);
            if (recruit <= character.CaptureProbability)
            {
                Status.SetActive(true);
                character.SetCapture();
                StatusText.text = "CAPTURE!";
                return;

            }

        }
        Status.SetActive(false);

    }


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
