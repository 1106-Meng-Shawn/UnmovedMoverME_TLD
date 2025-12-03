using System.Collections;
using System.Collections.Generic;
using System.Xml;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static GetColor;

public class BattleRecordControl : MonoBehaviour
{
    public Image BattleRecordBackImage;
    public TextMeshProUGUI BattleRecordText;
    private BattleCharacterValue battleCharacter;
    private Skill skill;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void BattleStartSet()
    {
        BattleRecordBackImage.color = GetRowColor(RowType.sel);
        BattleRecordText.text = "Battle Start";
        SetTextStyle(36, FontStyles.Bold, TextAlignmentOptions.Center);
    }

    public void TurnSet(int turn, bool isEnemy)
    {
        BattleRecordBackImage.color = isEnemy ? GetRowColor(RowType.enemy) : GetRowColor(RowType.player);
        BattleRecordText.text = isEnemy ? $"Turn {turn}   Enemy's Turn!" : $"Turn {turn}   Your Turn!";
        SetTextStyle(24);
    }

    public void CharacterSkillSet(BattleCharacterValue battleCharacterValue, Skill skill)
    {
        this.battleCharacter = battleCharacterValue;
        this.skill = skill;

        SetBackgroundColor(battleCharacterValue);
        BattleRecordText.text = $"{battleCharacterValue.GetBattleCharacterName()} use {skill.GetSkillName()}!";
        SetTextStyle(24);
    }

    public void CharacterValueChangeSet(BattleCharacterValue battleCharacterValue, List<(BattleValue valueIndex, int delta)> values)
    {
        this.battleCharacter = battleCharacterValue;

        SetBackgroundColor(battleCharacterValue);
        BattleRecordText.spriteAsset = GetSprite.LoadBattleValueSpriteAsset();

        string changesText = GetValueChangeText(values);
        BattleRecordText.text = $"{battleCharacterValue.GetBattleCharacterName()}  {changesText}!";
        SetTextStyle(24);
    }

    public void CriticalSet(BattleCharacterValue battleCharacterValue)
    {
        BattleRecordBackImage.color = battleCharacterValue.isEnemy ? GetRowColor(RowType.enemy) : GetRowColor(RowType.player);
        BattleRecordText.text = $"{battleCharacterValue.GetBattleCharacterName()} trigger a critical hit!";
        SetTextStyle(24, FontStyles.Bold);
    }

    public void BlockSet(BattleCharacterValue battleCharacterValue)
    {
        BattleRecordBackImage.color = battleCharacterValue.isEnemy ? GetRowColor(RowType.enemy) : GetRowColor(RowType.player);
        BattleRecordText.text = $"{battleCharacterValue.GetBattleCharacterName()} trigger a block!";
        SetTextStyle(24, FontStyles.Bold);
    }


    public void DamageTakenSet(BattleCharacterValue target, DamageResult damageResult)
    {
        BattleRecordBackImage.color = target.isEnemy ? GetRowColor(RowType.enemy) : GetRowColor(RowType.player);
        BattleRecordText.text = $"{target.GetBattleCharacterName()} takes {damageResult.Damage} damage!";
        SetTextStyle(24);
    }


    #region Helper function
    private void SetTextStyle(float fontSize, FontStyles style = FontStyles.Normal, TextAlignmentOptions alignment = TextAlignmentOptions.Left)
    {
        BattleRecordText.fontSize = fontSize;
        BattleRecordText.fontStyle = style;
        BattleRecordText.alignment = alignment;
    }

    private string GetValueChangeText(List<(BattleValue valueIndex, int delta)> values)
    {
        List<string> parts = new List<string>();
        foreach (var (valueIndex, delta) in values)
        {
            if (delta == 0) continue;
            string sign = delta > 0 ? "+" : "";
            string valueText = GetBattleValueColorString($"{sign}{delta}", valueIndex);
            parts.Add($"<sprite={(int)valueIndex}>{valueText}");
        }
        return string.Join(" ", parts);
    }
    private void SetBackgroundColor(BattleCharacterValue battleCharacterValue)
    {
        BattleRecordBackImage.color = battleCharacterValue.isEnemy
            ? GetRowColor(RowType.enemy)
            : GetRowColor(RowType.player);
    }
    #endregion

}
