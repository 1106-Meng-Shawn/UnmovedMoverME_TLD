using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;
using static ExprParser;

#region Skill AddValue Args
public class AddValueArgs
{
    public class AddValueEntry
    {
        public int ValueIndex;           // 0~4 ?? BattleValue
        public int ValueAdd;             // ????
        public List<string> TargetTypes = new(); // Self, Target, AllyAll, EnemyAll ?
    }

    public List<AddValueEntry> Entries { get; } = new();

    // ?? topArgs
    public static AddValueArgs Parse(string rawArgs)
    {
        var result = new AddValueArgs();

        if (string.IsNullOrWhiteSpace(rawArgs))
            throw new ArgumentException("AddValue requires at least 1 argument.");

        rawArgs = rawArgs.Trim();

        // ????????
        bool hasBraces = rawArgs.Contains('{') && rawArgs.Contains('}');
        List<string> blocks;

        if (hasBraces)
        {
            // ?? { ... } ?
            blocks = SplitTopLevelBlocks(rawArgs, '{', '}');
        }
        else
        {
            // ????? ? ????
            blocks = new List<string> { rawArgs };
        }

        foreach (var block in blocks)
        {
            var entry = ParseSingleEntry(block);
            if (entry != null)
                result.Entries.Add(entry);
        }

        return result;
    }

    private static AddValueEntry ParseSingleEntry(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
            return null;

        string trimmed = raw.Trim();
        if (trimmed.StartsWith("{") && trimmed.EndsWith("}"))
            trimmed = trimmed[1..^1].Trim();

        // ?????index, add, [targets]
        string[] parts = SplitTopLevel(trimmed, ',').Select(p => p.Trim()).ToArray();
        if (parts.Length < 2)
        {
            Debug.LogWarning($"Invalid AddValue entry: {raw}");
            return null;
        }

        if (!int.TryParse(parts[0], out int valueIndex))
        {
            Debug.LogWarning($"Invalid valueIndex: {parts[0]} in {raw}");
            return null;
        }

        if (!int.TryParse(parts[1], out int valueAdd))
        {
            Debug.LogWarning($"Invalid valueAdd: {parts[1]} in {raw}");
            return null;
        }

        var entry = new AddValueEntry
        {
            ValueIndex = valueIndex,
            ValueAdd = valueAdd
        };

        if (parts.Length >= 3)
        {
            string targetPart = parts[2];
            if (targetPart.StartsWith("[") && targetPart.EndsWith("]"))
            {
                string inner = targetPart[1..^1];
                foreach (var t in SplitTopLevel(inner, ','))
                    entry.TargetTypes.Add(t.Trim());
            }
            else
            {
                entry.TargetTypes.Add(targetPart);
            }
        }
        else
        {
            entry.TargetTypes.Add(SkillConstants.Target); // ????
        }

        return entry;
    }
    public Dictionary<BattleCharacterValue, Dictionary<int, int>> CollectChanges(SkillExecutionContext ctx, Func<SkillExecutionContext, IEnumerable<string>, List<BattleCharacterValue>> getTargetListFunc)
    {
        var pending = new Dictionary<BattleCharacterValue, Dictionary<int, int>>();

        foreach (var entry in Entries)
        {
            // ??? ctx ??????????
            var targets = getTargetListFunc(ctx, entry.TargetTypes);
            if (targets == null || targets.Count == 0) continue;

            foreach (var actualTarget in targets)
            {
                if (actualTarget == null) continue;

                // ???????
                if (!actualTarget.IsAlive()) continue;

                if (!pending.TryGetValue(actualTarget, out var dict))
                {
                    dict = new Dictionary<int, int>();
                    pending[actualTarget] = dict;
                }

                if (!dict.ContainsKey(entry.ValueIndex))
                    dict[entry.ValueIndex] = 0;

                dict[entry.ValueIndex] += entry.ValueAdd;
            }
        }

        return pending;
    }

}

#endregion

#region ApplyDamageArgs
public struct DamageResult
{
    public int Damage;
    public bool IsCritical;
    public bool IsBlock;
    public bool IsDodged;
}

public class ApplyDamageArgs
{
    public class Entry
    {
        public float Rate = 1f;
        public int Repeat = 1;
        public float Chance = 1f;
        public List<string> Targets = new();
        public bool SingleCritRoll = false;
        public bool? ForcedCrit = null;
    }

    public List<Entry> Entries { get; } = new();

    /// <summary>
    /// ????? MakeDamage(...) ??????
    /// ???
    ///   "{ rate=1.5, target=TargetAll, repeat=2 }, { rate=2.0, targets=[ { target=SelfAll, repeat=3 }, { target=TargetAll, repeat=1 } ], singleCritRoll=true }"
    ///   ? "rate=1.5, target=TargetAll, repeat=2"
    /// </summary>
    public static ApplyDamageArgs Parse(string rawArgs)
    {
        ApplyDamageArgs result = new();
        if (string.IsNullOrWhiteSpace(rawArgs))
            return result;

        rawArgs = rawArgs.Trim();

        // === ???????? ===
        bool hasBraces = rawArgs.Contains('{') && rawArgs.Contains('}');
        List<string> blocks;

        if (hasBraces)
        {
            // ???? { ... } ?
            blocks = SplitTopLevelBlocks(rawArgs, '{', '}');
        }
        else
        {
            // ?????????? Entry
            blocks = new List<string> { rawArgs };
        }

        foreach (var block in blocks)
        {
            var list = ParseSingleEntry(block);
            if (list != null && list.Count > 0)
                result.Entries.AddRange(list);
        }

        return result;
    }

    private static List<Entry> ParseSingleEntry(string block)
    {
        var entries = new List<Entry>();
        if (string.IsNullOrWhiteSpace(block)) return entries;

        string inner = block.Trim();
        if (inner.StartsWith("{") && inner.EndsWith("}"))
            inner = inner[1..^1].Trim();

        var parts = SplitTopLevel(inner, ',');

        float rate = 1f;
        int repeat = 1;
        float chance = 1f;
        bool singleCritRoll = false;
        bool? forcedCrit = null;
        List<string> targetList = new();
        List<string> nestedTargetsBlocks = new();

        foreach (var p in parts)
        {
            var kv = p.Split(new[] { '=' }, 2);
            if (kv.Length != 2) continue;

            var key = kv[0].Trim().ToLower();
            var val = kv[1].Trim();

            switch (key)
            {
                case "rate":
                    if (float.TryParse(val, out var r)) rate = r;
                    break;
                case "repeat":
                    if (int.TryParse(val, out var rep)) repeat = Math.Max(1, rep);
                    break;
                case "chance":
                    if (float.TryParse(val, out var ch)) chance = Mathf.Clamp(ch, 0f, 1f);
                    break;
                case "targets":
                    targetList.AddRange(SkillCalculation.ParseTargetList(val));
                    break;
                case "singlecritroll":
                    singleCritRoll = ParseBool(val);
                    break;
                case "forcedcrit":
                    forcedCrit = ParseBoolNullable(val);
                    break;
                default:
                    Debug.LogWarning($"Unknown ApplyDamage arg key: {key}");
                    break;
            }
        }

        // ?? targets
        if (nestedTargetsBlocks.Count > 0)
        {
            foreach (var nt in nestedTargetsBlocks)
            {
                string t = nt.Trim();
                if (t.StartsWith("[") && t.EndsWith("]"))
                    t = t[1..^1];

                var innerBlocks = SplitTopLevelBlocks(t, '{', '}');
                foreach (var ib in innerBlocks)
                {
                    var nestedEntries = ParseSingleEntry(ib);
                    foreach (var ne in nestedEntries)
                    {
                        ne.Rate = Math.Abs(ne.Rate - 1f) > 1e-6 ? ne.Rate : rate;
                        ne.Chance = Math.Abs(ne.Chance - 1f) > 1e-6 ? ne.Chance : chance;
                        ne.Repeat = ne.Repeat != 1 ? ne.Repeat : repeat;
                        ne.SingleCritRoll = singleCritRoll;
                        ne.ForcedCrit = forcedCrit;
                        entries.Add(ne);
                    }
                }
            }
        }
        else
        {
            if (targetList.Count == 0)
                targetList.Add(SkillConstants.Target);

            entries.Add(new Entry
            {
                Rate = rate,
                Repeat = repeat,
                Chance = chance,
                Targets = targetList,
                SingleCritRoll = singleCritRoll,
                ForcedCrit = forcedCrit
            });
        }

        return entries;
    }

}

#endregion

#region ApplyHealArgs
public class ApplyHealArgs
{
    public class Entry
    {
        public float Rate = 0f;                 // ????
        public int FixedValue = 0;              // ?????
        public List<string> Targets = new();    // ????
    }

    public List<Entry> Entries { get; } = new();

    /// <summary>
    /// ????: "{ fixed=100, rate=1.5, targets=[Target1,Target2], repeat=2 }"
    /// ???????
    /// </summary>
    public static ApplyHealArgs Parse(string rawArgs)
    {
        var result = new ApplyHealArgs();
        if (string.IsNullOrWhiteSpace(rawArgs)) return result;

        bool hasBraces = rawArgs.Contains('{') && rawArgs.Contains('}');
        List<string> blocks = hasBraces ? SplitTopLevelBlocks(rawArgs, '{', '}') : new List<string> { rawArgs };

        foreach (var block in blocks)
        {
            var entry = ParseSingleEntry(block);
            if (entry != null) result.Entries.Add(entry);
        }

        return result;
    }

    private static Entry ParseSingleEntry(string block)
    {
        var entry = new Entry();
        if (string.IsNullOrWhiteSpace(block)) return entry;

        string inner = block.Trim();
        if (inner.StartsWith("{") && inner.EndsWith("}"))
            inner = inner[1..^1].Trim();

        var parts = SplitTopLevel(inner, ',');

        foreach (var p in parts)
        {
            var kv = p.Split(new[] { '=' }, 2);
            if (kv.Length != 2) continue;

            var key = kv[0].Trim().ToLower();
            var val = kv[1].Trim();

            switch (key)
            {
                case "fixed":
                    if (int.TryParse(val, out var f)) entry.FixedValue = f;
                    break;
                case "rate":
                    if (float.TryParse(val, out var r)) entry.Rate = r;
                    break;
                case "targets":
                    val = val.Trim('[', ']');
                    entry.Targets = val.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                       .Select(x => x.Trim())
                                       .ToList();
                    break;
                default:
                    Debug.LogWarning($"Unknown ApplyHeal arg key: {key}");
                    break;
            }
        }

        return entry;
    }

}
#endregion

public static class SkillCalculation
{
    #region Heal Calculation
    private static int CalculateHeal(BattleCharacterValue user, BattleCharacterValue target, int fixedValue = 0, float rate = 0f)
    {
        if (user == null || target == null) return 0;

        // 1. ?????????????Force?
        float basePower = user.IsPersonBattle()  ? user.characterValue.GetLevelConstant() : ForceParametersCal(user.characterValue.Force);

        float magicValue = user.characterValue.GetValue(2,3); 

        float heal = (basePower * magicValue / GameValueConstants.maxValueLimit) * rate + fixedValue;

        float randomFactor = GetRandomFactor();
        heal *= randomFactor;

        return Mathf.Max(0, Mathf.RoundToInt(heal));
    }

    #endregion

    // ----------------------
    #region Damage Calculation
    private static DamageResult CalculateDamage(BattleCharacterValue attacker,BattleCharacterValue defender,bool? forcedCrit = null,bool? forcedBlock = null,float hitChance = 1f, float rate = 1f)  // ??? 0~1float rate = 1f)
    {
        float atkForce = attacker.IsPersonBattle() ? attacker.characterValue.GetLevelConstant() : ForceParametersCal(attacker.characterValue.Force);
        int defenseValue = Mathf.Max(1, defender.GetValueAt(BattleValue.Defense));
        float baseDmg = atkForce * attacker.GetValueAt(BattleValue.Attack) / defenseValue;

        // ????
        bool dodged = UnityEngine.Random.value > hitChance;
        float dmg = 0;
        bool crit = forcedCrit ?? (UnityEngine.Random.Range(0, 100) < attacker.GetValueAt(BattleValue.Lucky));
        bool block = false;
        if (!dodged)
        {
            block = forcedBlock ?? (UnityEngine.Random.Range(0, 100) < defender.GetValueAt(BattleValue.Lucky));
            dmg = baseDmg * rate * GetRandomFactor();
            if (crit) dmg *= SkillConstants.CritRate;
            if (block) dmg /= SkillConstants.BlockRate;

        }

        return new DamageResult
        {
            Damage = Mathf.Max(0, (int)dmg),
            IsCritical = crit,
            IsBlock = block,
            IsDodged = dodged
        };
    }

    static float GetRandomFactor()
    {
        return UnityEngine.Random.Range(0.8f, 1.2f);
    }


    private static float ForceParametersCal(int force)
    {
        if (force <= 500) return force;
        if (force <= 2000) return 500 + (force - 500) * 0.5f;
        return 500 + (force - 500) * 0.5f + (force - 2000) * 0.25f;
    }
    #endregion

    // ----------------------
    #region Actual Effects

    public static void AddValue(SkillExecutionContext ctx, AddValueArgs args)
    {
        if (args == null || args.Entries.Count == 0) { ctx.OnComplete?.Invoke(); return; }

        var pending = args.CollectChanges(ctx, GetTargetList);
        if (pending == null || pending.Count == 0) { ctx.OnComplete?.Invoke(); return; }

        // ? ??????? .OrderBy(idx => idx) ??
        var allValueIndices = pending.Values
            .SelectMany(d => d.Keys)
            .Distinct()
            .OrderBy(idx => idx)  // ? ?????? valueIndex ??????
            .ToList();

        var targets = new List<BattleCharacterValue>();
        var values = new List<List<(BattleValue valueIndex, int delta)>>();

        foreach (var kv in pending)
        {
            var actualTarget = kv.Key;
            if (actualTarget == null) continue;

            var indexToDelta = kv.Value;
            var listForTarget = new List<(BattleValue valueIndex, int delta)>();

            foreach (var idx in allValueIndices)  // ? ?? allValueIndices ????
            {
                indexToDelta.TryGetValue(idx, out int delta);
                if (delta != 0)
                {
                    if (!ctx.IsVisual)
                    {
                       int current = actualTarget.GetValueAt((BattleValue)idx);
                       actualTarget.SetValueAt((BattleValue)idx, current + delta);
                    }
                }
                BattleValue battleValueidx = (BattleValue)idx;
                listForTarget.Add((battleValueidx, delta));
            }

            if (listForTarget.Count > 0)
            {
                targets.Add(actualTarget);
                values.Add(listForTarget);
            }
        }

        if (ctx.IsVisual && targets.Count > 0)
        {
            EffectManager.instance.ValueChangeAnimation(targets, values, ctx.OnComplete);
        }
        else
        {
            ctx.OnComplete?.Invoke();
        }
    }

    public static void ApplyDamage(SkillExecutionContext ctx, ApplyDamageArgs args, bool showVisual = true)
    {
        if (args == null || args.Entries.Count == 0)
        {
            ctx.OnComplete?.Invoke();
            return;
        }

        var allBatchRequests = new List<DamageAnimationBatchRequest>();

        foreach (var entry in args.Entries)
        {
            var targets = GetTargetList(ctx, entry.Targets);
            if (targets == null || targets.Count == 0) continue;

            foreach (var actualTarget in targets)
            {
                BattleCharacterValue User = ctx.BattleArray.User;
                BattleCharacterValue Target = ctx.BattleArray.Target;
                BattleCharacterValue[,] UserAlly = ctx.BattleArray.UserAlly;
                BattleCharacterValue[,] TargetAlly = ctx.BattleArray.TargetAlly;

                if (actualTarget == null || !actualTarget.IsAlive()) continue;

                bool? sharedCrit = entry.SingleCritRoll ? entry.ForcedCrit : null;
                bool? sharedBlock = null;

                if (entry.SingleCritRoll && sharedCrit == null)
                {
                    var tmp = CalculateDamage(User, actualTarget);
                    sharedCrit = tmp.IsCritical;
                    sharedBlock = tmp.IsBlock;
                }

                var targetResults = new List<DamageResult>();

                for (int i = 0; i < entry.Repeat; i++)
                {
                    DamageResult result = CalculateDamage(User, actualTarget, sharedCrit, sharedBlock, entry.Chance, entry.Rate);

                    if (!showVisual)
                    {
                        actualTarget.ApplyDamage(result);
                    }
                    else
                    {
                        targetResults.Add(new DamageResult
                        {
                            Damage = result.Damage,
                            IsCritical = result.IsCritical,
                            IsBlock = result.IsBlock,
                            IsDodged = result.IsDodged
                        });
                    }
                }

                if (showVisual && targetResults.Count > 0)
                {
                    var request = new DamageAnimationRequest(actualTarget, User, ctx.Skill, targetResults);
                    allBatchRequests.Add(new DamageAnimationBatchRequest
                    {
                        DamageRequests = new List<DamageAnimationRequest> { request }
                    });
                }
            }
        }

        if (showVisual && allBatchRequests.Count > 0)
        {
            EffectManager.instance.PlayDamageBatches(allBatchRequests, ctx.OnComplete);
        }
        else
        {
            ctx.OnComplete?.Invoke();
        }
    }


    public static void ApplyHeal(SkillExecutionContext ctx, ApplyHealArgs args, bool showVisual = true)
    {
        if (args == null || args.Entries.Count == 0)
        {
            ctx.OnComplete?.Invoke();
            return;
        }

        var allBatchRequests = new List<HealAnimationBatchRequest>();

        foreach (var entry in args.Entries)
        {
            var targets = GetTargetList(ctx, entry.Targets);
            if (targets == null || targets.Count == 0) continue;

            foreach (var actualTarget in targets)
            {
                if (actualTarget == null || !actualTarget.IsAlive()) continue;

                BattleCharacterValue user = ctx.BattleArray.User;

                // ?????
                int healValue = CalculateHeal(user, actualTarget, entry.FixedValue, entry.Rate);

                if (!showVisual)
                {
                    // ??????
                    actualTarget.ApplyHeal(healValue);
                }
                else
                {
                    // ???????????
                    var healResults = new List<int> { healValue };

                    var request = new HealAnimationRequest(actualTarget, ctx.Skill, healResults);

                    allBatchRequests.Add(new HealAnimationBatchRequest
                    {
                        HealRequests = new List<HealAnimationRequest> { request }
                    });
                }
            }
        }

        // ??????
        if (showVisual && allBatchRequests.Count > 0)
        {
            EffectManager.instance.PlayHealBatches(allBatchRequests, ctx.OnComplete);
        }
        else
        {
            ctx.OnComplete?.Invoke();
        }
    }



    public static void AddCaptureProbability(SkillExecutionContext ctx, string[] args)
    {
        if (args.Length < 1 || !int.TryParse(args[0], out int addChance))
        {
            ctx.OnComplete?.Invoke();
            return;
        }

        BattleCharacterValue User = ctx.BattleArray.User;
        BattleCharacterValue Target = ctx.BattleArray.Target;
        BattleCharacterValue[,] UserAlly = ctx.BattleArray.UserAlly;
        BattleCharacterValue[,] TargetAlly = ctx.BattleArray.TargetAlly;


        if (Target != null)
        {
            Target.characterValue.CaptureProbability = Mathf.Clamp(
                Target.characterValue.CaptureProbability + addChance, 0, 100);
        }

        ctx.OnComplete?.Invoke();
    }


    #endregion

    #region Healper Function
    public static List<BattleCharacterValue> GetTargetList(SkillExecutionContext ctx, string targetType)
    {
        return GetTargetList(ctx, new[] { targetType });
    }

    public static List<BattleCharacterValue> GetTargetList(SkillExecutionContext ctx, IEnumerable<string> targetTypes)
    {
        var result = new List<BattleCharacterValue>();
        if (targetTypes == null) return result;

        foreach (var t in targetTypes)
        {
            if (string.IsNullOrWhiteSpace(t)) continue;
            string targetType = t.Trim();

            BattleCharacterValue User = ctx.BattleArray.User;
            BattleCharacterValue Target = ctx.BattleArray.Target;
            BattleCharacterValue[,] UserAlly = ctx.BattleArray.UserAlly;
            BattleCharacterValue[,] TargetAlly = ctx.BattleArray.TargetAlly;

            switch (targetType)
            {
                case SkillConstants.Self:
                    if (User != null) result.Add(User);
                    continue;
                case SkillConstants.Target:
                    if (Target != null) result.Add(Target);
                    continue;
                case SkillConstants.AllyAll:
                    if (UserAlly != null)
                        foreach (var c in UserAlly)
                            if (c != null) result.Add(c);
                    continue;
                case SkillConstants.TargetAll:
                    if (TargetAlly != null)
                        foreach (var c in TargetAlly)
                            if (c != null) result.Add(c);
                    continue;
                case SkillConstants.All:
                    if (TargetAlly != null)
                        foreach (var c in TargetAlly)
                            if (c != null) result.Add(c);
                    if (UserAlly != null)
                        foreach (var c in UserAlly)
                            if (c != null) result.Add(c);
                    continue;
            }

            // ?? Self[x,y] ? Target[x,y]
            bool isSelf = targetType.StartsWith("Self[") && targetType.EndsWith("]");
            bool isTarget = targetType.StartsWith("Target[") && targetType.EndsWith("]");

            if (!isSelf && !isTarget) continue;

            var inside = targetType[(isSelf ? 5 : 7)..^1];
            var offsets = inside.Split(',');
            if (offsets.Length != 2) continue;
            if (!int.TryParse(offsets[0], out int xOffset) || !int.TryParse(offsets[1], out int yOffset)) continue;

            BattleCharacterValue[,] array = isSelf ? UserAlly : TargetAlly;
            if (array == null) continue;

            int rows = array.GetLength(0);
            int cols = array.GetLength(1);

            if (yOffset == 0) // ??
            {
                int row = xOffset;
                if (row >= 0 && row < rows)
                    for (int col = 0; col < cols; col++)
                        if (array[row, col] != null)
                            result.Add(array[row, col]);
            }
            else if (xOffset == 0) // ??
            {
                int col = yOffset;
                if (col >= 0 && col < cols)
                    for (int row = 0; row < rows; row++)
                        if (array[row, col] != null)
                            result.Add(array[row, col]);
            }
            else // ????
            {
                int row = xOffset;
                int col = yOffset;
                if (row >= 0 && row < rows && col >= 0 && col < cols && array[row, col] != null)
                    result.Add(array[row, col]);
            }
        }

        return result;
    }


    public static List<string> ParseTargetList(string input)
    {
        var list = new List<string>();
        if (string.IsNullOrWhiteSpace(input)) return list;

        if (input.StartsWith("[") && input.EndsWith("]"))
        {
            var inner = input[1..^1];
            foreach (var s in inner.Split(',', StringSplitOptions.RemoveEmptyEntries))
                list.Add(s.Trim());
        }
        else list.Add(input.Trim());

        return list;
    }
    #endregion
}
