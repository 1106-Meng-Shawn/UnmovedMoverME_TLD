using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Localization.Settings;
using static BattlePanelManage;
using static ExcelReader;
using static ExprParser;
using static PositionAtBattle;
using static UnityEngine.GraphicsBuffer;
using static SkillCalculation;

public enum SkillFunctionType { NONE, Attack, Heal, Buff, Debuff, Passive }
public enum SkillTargetType { NONE, Self, AllySingle, AllyArea, AllyAll, EnemySingle, EnemyArea, EnemyAll }
public enum SkillRangeType { NONE, Melee, Ranged, Self }
public enum SkillTriggerType { NONE, Active, Passive }
public enum SkillTargetLifeStatus { Alive,Dead,Both }


/// <summary>
/// Common string constants used in skill scripts and parsing.
/// </summary>
public static class SkillConstants
{
    public const string ApplyDamage = "ApplyDamage";
    public const string ApplyHeal = "ApplyHeal";
    public const string AddValue = "AddValue";
    public const string AddCharacterValue = "AddCharacterValue";
    public const string AddCaptureProbability = "AddCaptureProbability";

    public const string All = "All";
    public const string Self = "Self";
    public const string Target = "Target";
    public const string AllyAll = "AllyAll";
    public const string TargetAll = "TargetAll";

    public const float CritRate = 1.5f;
    public const float BlockRate = 1.5f;


}

public struct BattleArray
{
    public BattleCharacterValue User;
    public BattleCharacterValue Target;
    public BattleCharacterValue[,] UserAlly;
    public BattleCharacterValue[,] TargetAlly;

    public BattleArray(BattleCharacterValue user,
                             BattleCharacterValue target,
                             BattleCharacterValue[,] userAlly,
                             BattleCharacterValue[,] targetAlly)
    {
        User = user;
        Target = target;
        UserAlly = userAlly;
        TargetAlly = targetAlly;
    }
}

public struct SkillExecutionContext
{
    public BattleArray BattleArray;
    public bool IsVisual;
    public Skill Skill;
    public Action OnComplete;

    public SkillExecutionContext(BattleArray battleArray,
                             bool isVisual,
                             Skill skill,
                             Action onComplete)
    {
        BattleArray = battleArray;
        IsVisual = isVisual;
        Skill = skill;
        OnComplete = onComplete;
    }

    public SkillExecutionContext(BattleCharacterValue user,
                                 BattleCharacterValue target,
                                 BattleCharacterValue[,] userAlly,
                                 BattleCharacterValue[,] targetAlly,
                                 bool isVisual,
                                 Skill skill,
                                 Action onComplete)
    {
        BattleArray = new BattleArray(user,target,userAlly,targetAlly);
        IsVisual = isVisual;
        Skill = skill;
        OnComplete = onComplete;
    }
}



public class Skill
{
    public int ID;
    public int skillRare;
    public SkillFunctionType functionType;
    public SkillTargetType targetType;
    public SkillRangeType rangeType;
    public SkillTriggerType triggerType;
    public int moveCost;
    public int speedCost;

    public SkillTargetLifeStatus targetLifeStatus;
    public int distance;
    public string skillEffect;

    private Dictionary<string, string> skillName = new();
    private Dictionary<string, string> skillDescribe = new();

    /// <summary>
    /// Compiled skill effects: (user, trigger, playerChars, enemyChars, isVisual)
    /// </summary>
    private readonly List<Action<SkillExecutionContext>> compiledEffects = new();

    // ==============================
    // Constructor
    // ==============================
    public Skill(ExcelSkillData data)
    {
        ID = data.ID;
        skillRare = data.skillRare;
        functionType = ParseEnum<SkillFunctionType>(data.functionType);
        targetType = ParseEnum<SkillTargetType>(data.targetType);
        rangeType = ParseEnum<SkillRangeType>(data.rangeType);
        triggerType = ParseEnum<SkillTriggerType>(data.triggerType);
        moveCost = data.moveCost;
        speedCost = data.speedCost;

        targetLifeStatus = ParseEnum<SkillTargetLifeStatus>(data.targetLifeStatus);
        distance = data.distance;
        skillEffect = data.skillEffect;
        skillName = data.skillNames;
        skillDescribe = data.skillDescribe;

        ParseSkillEffects(ParseSkillEffect());
    }

    // ==============================
    // Localization
    // ==============================
    public string GetSkillName()
    {
        string lang = LocalizationSettings.SelectedLocale.Identifier.Code;
        return skillName.TryGetValue(lang, out var name)
            ? name
            : skillName.GetValueOrDefault("en", "Unnamed Skill");
    }

    public string GetSkillENName()
    {
        return skillName.GetValueOrDefault("en", "Unnamed Skill");
    }


    public string GetSkillDescribe()
    {
        string lang = LocalizationSettings.SelectedLocale.Identifier.Code;
        return skillDescribe.TryGetValue(lang, out var desc)
            ? desc
            : skillDescribe.GetValueOrDefault("en", "No description available.");
    }


    // ==============================
    // Parse & Compile Skill Effects
    // ==============================
    void ParseSkillEffects(List<ParsedSkillEffect> effects)
    {
        if (effects == null || effects.Count == 0) return;

        foreach (var effect in effects)
        {
            switch (effect.FunctionName)
            {
                case SkillConstants.AddValue:HandleAddValue(effect);break;
                case SkillConstants.ApplyDamage:HandleApplyDamage(effect);break;
                case SkillConstants.ApplyHeal: HandleApplyHeal(effect); break;
                case SkillConstants.AddCharacterValue:break;
                default:Debug.LogWarning($"[Skill {ID}] Unknown skill effect: {effect.FunctionName}");break;
            }
        }
    }

    // ==========================================================
    // AddValue([valueIndexes], [values], [targets])
    // ==========================================================
    void HandleAddValue(ParsedSkillEffect effect)
    {

        var parsedArgs = AddValueArgs.Parse(effect.Args);

        compiledEffects.Add(ctx =>
        {
            try
            {
                SkillCalculation.AddValue(ctx, parsedArgs);
            }
            catch (Exception ex)
            {
                Debug.LogError($"[Skill {ID} - {GetSkillENName()} {parsedArgs}] Error executing AddValue: {ex.Message}");
                ctx.OnComplete?.Invoke();
            }
        });
    }

    void HandleApplyDamage(ParsedSkillEffect effect)
    {
        var parsedArgs = ApplyDamageArgs.Parse(effect.Args);

        compiledEffects.Add(ctx =>
        {
            try
            {
                SkillCalculation.ApplyDamage(ctx, parsedArgs);
            }
            catch (Exception ex)
            {
                Debug.LogError($"[Skill {ID} - {GetSkillENName()} {parsedArgs}] Error executing ApplyDamage: {ex.Message}");
                ctx.OnComplete?.Invoke();
            }
        });
    }


    void HandleApplyHeal(ParsedSkillEffect effect)
    {
        var parsedArgs = ApplyHealArgs.Parse(effect.Args);

        compiledEffects.Add(ctx =>
        {
            try
            {
                SkillCalculation.ApplyHeal(ctx, parsedArgs);
            }
            catch (Exception ex)
            {
                Debug.LogError($"[Skill {ID} - {GetSkillENName()} {parsedArgs}] Error executing ApplyDamage: {ex.Message}");
                ctx.OnComplete?.Invoke();
            }
        });
    }



    public void UseSkill(BattleCharacterValue user, BattleCharacterValue target,
                         BattleCharacterValue[,] userAlly, BattleCharacterValue[,] targetAlly, Action onSkillDone)
    {
        BattleRightCol.Instance.SetCharacterSkillRecord(user, this);
        var ctx = new SkillExecutionContext(user, target, userAlly, targetAlly, true, this, null);
        EffectManager.instance.StartCoroutine(RunEffectsSequentially(ctx, onSkillDone));
    }

    private IEnumerator RunEffectsSequentially(SkillExecutionContext ctx, Action onSkillDone)
    {
        for (int i = 0; i < compiledEffects.Count; i++)
        {
            var effect = compiledEffects[i];
            bool finished = false;

            BattleArray newBattleArray = new BattleArray(ctx.BattleArray.User, ctx.BattleArray.Target, ctx.BattleArray.UserAlly, ctx.BattleArray.TargetAlly);
            var effectCtx = new SkillExecutionContext(
                newBattleArray,
                ctx.IsVisual, this,
                () =>
                {
                    var stackTrace = new System.Diagnostics.StackTrace(true);
                    for (int j = 0; j < stackTrace.FrameCount; j++)
                    {
                        var frame = stackTrace.GetFrame(j);
                        var method = frame.GetMethod();
                    }
                    finished = true;
                }
            );

            effect.Invoke(effectCtx);

            if (finished)
            {
                Debug.LogError($"[Skill] ? Effect '{effect.GetType().Name}' called OnAnimationComplete immediately!");
            }

            while (!finished)
                yield return null;

        }

        ApplySkillCost(this, ctx.BattleArray.User);
        onSkillDone?.Invoke();
    }
    public void ApplySkill(BattleArray battleArray)
    {
        var ctx = new SkillExecutionContext(battleArray, false, this, null);

        foreach (var effect in compiledEffects)
        {
            effect.Invoke(ctx);
        }

        ApplySkillCost(this, battleArray.User);
    }



    // ==========================================================
    // SkillEffect Parser
    // ==========================================================
    List<ParsedSkillEffect> ParseSkillEffect()
    {
        var result = new List<ParsedSkillEffect>();
        if (string.IsNullOrEmpty(skillEffect)) return result;

        string[] parts = skillEffect.Split(new[] { ';', '\n' }, StringSplitOptions.RemoveEmptyEntries);

        foreach (string raw in parts)
        {
            string trimmed = raw.Trim();
            if (string.IsNullOrEmpty(trimmed)) continue;

            string funcName = trimmed.Contains("(") ? trimmed.Substring(0, trimmed.IndexOf('(')) : trimmed;
            string argStr = "";

            if (trimmed.Contains("("))
            {
                int start = trimmed.IndexOf('(') + 1;
                int end = trimmed.LastIndexOf(')');
                if (end > start)
                    argStr = trimmed.Substring(start, end - start);
            }

            result.Add(new ParsedSkillEffect
            {
                FunctionName = funcName,
                Args = argStr
            });
        }

        return result;
    }

    public int GetPassiveBonus(int valueType, int index)
    {
        if (string.IsNullOrEmpty(skillEffect)) return 0;

        var lines = skillEffect.Split(';', StringSplitOptions.RemoveEmptyEntries);

        int totalBonus = 0;
        foreach (var line in lines)
        {
            var trimmed = line.Trim();

            if (trimmed.StartsWith(SkillConstants.AddCharacterValue, StringComparison.OrdinalIgnoreCase))
            {
                // ?? AddCharacterValue(0,1,5)
                int[] args = ParseArgsToIntArray(trimmed);
                if (args.Length >= 3)
                {
                    int type = args[0];
                    int idx = args[1];
                    int add = args[2];

                    if (type == valueType && idx == index)
                        totalBonus += add; // ??
                }
            }
        }

        return totalBonus;
    }


    private void ApplySkillCost(Skill skill, BattleCharacterValue user)
    {
        user.MoveSpeed -= skill.speedCost;
        if (!user.IsPersonBattle())
        {
            user.MoveNum -= skill.moveCost;
        }
    }

    private T ParseEnum<T>(string value) where T : struct =>
        Enum.TryParse<T>(value, true, out var result) ? result : default;

    // ==============================
    // Helper checks
    // ==============================
    public bool IsForSelf() => targetType == SkillTargetType.Self;
    public bool IsForAlly() => targetType is SkillTargetType.AllySingle or SkillTargetType.AllyArea or SkillTargetType.AllyAll;
    public bool IsForEnemy() => targetType is SkillTargetType.EnemySingle or SkillTargetType.EnemyArea or SkillTargetType.EnemyAll;
    public bool RequiresTargetSelection() => targetType is SkillTargetType.AllySingle or SkillTargetType.EnemySingle or SkillTargetType.EnemyArea or SkillTargetType.AllyArea;

    public bool IsInRange(BattleArray battleArray)
    {

        if (IsForAlly())
        {
            return IsInRangeForAllyHelper(battleArray);
        }
        else if (IsForEnemy())
        {
            return IsInRangeForEnemyHelper(battleArray);
        }


        return false;
    }


    bool IsInRangeForAllyHelper(BattleArray battleArray)
    {
        BattleCharacterValue User = battleArray.User;
        BattleCharacterValue Target = battleArray.Target;
        BattleCharacterValue[,] UserAlly = battleArray.UserAlly;
        BattleCharacterValue[,] TargetAlly = battleArray.TargetAlly;
        int userRow, userCol, targetRow, targetCol;
        if (!FindPosition(UserAlly, User, out userRow, out userCol)) return false;
        if (!FindPosition(UserAlly, Target, out targetRow, out targetCol)) return false;
        int rowDiff = Math.Abs(userRow - targetRow);
        return rowDiff < distance && CheckUseByDieTarget(Target);
    }


    bool IsInRangeForEnemyHelper(BattleArray battleArray)
    {
        BattleCharacterValue User = battleArray.User;
        BattleCharacterValue Target = battleArray.Target;
        BattleCharacterValue[,] UserAlly = battleArray.UserAlly;
        BattleCharacterValue[,] TargetAlly = battleArray.TargetAlly;

        int userRow, userCol, targetRow, targetCol;

        // ? User ? UserAlly ??????
        if (!FindPosition(UserAlly, User, out userRow, out userCol)) return false;
        // ? Target ? TargetAlly ??????
        if (!FindPosition(TargetAlly, Target, out targetRow, out targetCol)) return false;

        int baseDistance = userRow + targetRow;

        // ? 1) ??????
        if (baseDistance < distance) return true && CheckUseByDieTarget(Target);

        // ? 2) ???????? offset
        int offset = 0;

        // 2.1) ?? User ??????????
        for (int r = userRow - 1; r >= 0; r--)
        {
            if (CanRowAllFalse(UserAlly, r)) offset++;
        }

        // 2.2) ?? Target ????????
        for (int r = targetRow - 1; r >= 0; r--)
        {
            if (CanRowAllFalse(TargetAlly, r)) offset++;
        }

        // ? 3) ????????????
        return (distance + offset > baseDistance) && CheckUseByDieTarget(Target);
    }

    bool CanRowAllFalse(BattleCharacterValue[,] array, int row)
    {
        for (int c = 0; c < array.GetLength(1); c++)
        {
            if (array[row, c] != null && array[row, c].CanBlock())
                return false;
        }
        return true;
    }

    private bool FindPosition(BattleCharacterValue[,] array, BattleCharacterValue target, out int row, out int col)
    {
        row = -1;
        col = -1;
        for (int r = 0; r < array.GetLength(0); r++)
        {
            for (int c = 0; c < array.GetLength(1); c++)
            {
                if (array[r, c] == target)
                {
                    row = r;
                    col = c;
                    return true;
                }
            }
        }
        return false;
    }

    bool CheckUseByDieTarget(BattleCharacterValue target)
    {
        return targetLifeStatus switch
        {
            SkillTargetLifeStatus.Alive => target.IsAlive(),
            SkillTargetLifeStatus.Dead => !target.IsAlive(),
            _ => true
        };
    }


    public int EstimatedSpeed(BattleCharacterValue target)
    {
        return target.MoveSpeed - speedCost;
    }
}
public class ParsedSkillEffect
{
    public string FunctionName;
    public string Args;
}
