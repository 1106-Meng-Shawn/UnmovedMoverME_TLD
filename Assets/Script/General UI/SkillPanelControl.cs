using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.Localization.Settings;
using static GetColor;
using UnityEngine.Localization.Components;

public class SkillPanelControl : MonoBehaviour
{
    public TextMeshProUGUI skillName;
    public TextMeshProUGUI skillFunctionTypeText;
    public TextMeshProUGUI skillTargetTypeText;
    public TextMeshProUGUI skillDesribeText;

    public GameObject ActiveRow;
    public TextMeshProUGUI skillRangeText;
    public TextMeshProUGUI skillMoveNumText;

    public GameObject PassiveRow;

//    [SerializeField] private bool isFollow = false;

    public void ShowSkillPanel(bool isPersonBattle,Skill skill)
    {
        gameObject.SetActive(true);
        SetSkillText(isPersonBattle,skill);
    }

    void SetSkillText(bool isPersonBattle,Skill skill)
    {
        if (skill == null) return;
        skillName.text = skill.GetSkillName();
        skillName.color = GetSkillRareColor(skill.skillRare);
        skillDesribeText.text = skill.GetSkillDescribe();


        SetLocalizedText(skillFunctionTypeText, "SkillTag", skill.functionType.ToString());
        skillFunctionTypeText.color = GetSkillFunctionTypeColor(skill.functionType);
        SetLocalizedText(skillTargetTypeText, "SkillTag", skill.targetType.ToString());



        if (skill.triggerType == SkillTriggerType.Active)
        {
            ActiveRow.SetActive(true);
            PassiveRow.SetActive(false);

            SetLocalizedText(skillRangeText, "SkillTag", skill.rangeType.ToString());
            skillMoveNumText.transform.parent.gameObject.SetActive(!isPersonBattle);
            skillMoveNumText.text = GetValueColorString($" * {skill.moveCost}", ValueColorType.CanMove);

        }
        else if (skill.triggerType == SkillTriggerType.Passive)
        {
            ActiveRow.SetActive(false);
            PassiveRow.SetActive(true);

        }
    }

    private void SetLocalizedText(TextMeshProUGUI textComponent, string tableName, string key)
    {
        key = key.ToUpper();
        var localizeEvent = textComponent.GetComponent<LocalizeStringEvent>();
        if (localizeEvent == null)
        {
            localizeEvent = textComponent.gameObject.AddComponent<LocalizeStringEvent>();
        }

        localizeEvent.StringReference.TableReference = tableName;
        localizeEvent.StringReference.TableEntryReference = key;
        localizeEvent.RefreshString();
    }

}
