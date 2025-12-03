using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.SocialPlatforms.Impl;
using static GeneralGameTools;

public static class GameCalculate
{
    // ==================== Constants ====================
    public const int AchievementFactor = 1000;        // 1 FSPGF = 1000 Achievement
    public const int BaseScienceCostPerLevel = 1000;  // Base science cost per power level
    public const int TierLevelStep = 10;              // Every 10 levels -> next tier

    // ==================== Conversion Rules ====================

    /// <summary>
    /// ???????????????
    /// ???? FSPGF?SBN?TotalRecruitedPopulation ???????????????
    /// </summary>
    private static readonly Dictionary<ValueType, int> achievementConversionRates = new Dictionary<ValueType, int>()
    {
        // FSPGF ??????????
        { ValueType.Food, AchievementFactor },
        { ValueType.Science, AchievementFactor },
        { ValueType.Politics, AchievementFactor },
        { ValueType.Gold, AchievementFactor },
        { ValueType.Faith, AchievementFactor },
        
        // SBN ????????
        { ValueType.Scout, AchievementFactor },
        { ValueType.Build, AchievementFactor },
        { ValueType.Negotiation, AchievementFactor },
        
        // ????
        { ValueType.TotalRecruitedPopulation, AchievementFactor },
    };

    // ==================== Category Checks ====================

    /// <summary>???????????????????</summary>
    public static bool IsBasicGameValue(ValueType type)
    {
        return achievementConversionRates.ContainsKey(type);
    }


    // ==================== Basic Conversion: Resource <-> Achievement ====================

    /// <summary>
    /// ??????????????????????
    /// </summary>
    public static int BasicGameValueToAchievement(ValueType type, int amount)
    {
        if (!achievementConversionRates.TryGetValue(type, out int conversionRate))
        {
            return 0;
        }

        return amount * conversionRate;
    }

    /// <summary>
    /// ??????????????????????
    /// </summary>
    public static int AchievementToBasicGameValue(ValueType type, int achievement)
    {
        if (!achievementConversionRates.TryGetValue(type, out int conversionRate))
        {
            return 0;
        }

        return achievement / conversionRate;
    }

    // ==================== Convenience Methods (Optional) ====================

    /// <summary>?????FSPGF ??????? BasicGameValueToAchievement?</summary>
    public static int FSPGFToAchievement(ValueType type, int amount)
    {
        if (!IsFSPGF(type)) return 0;
        return BasicGameValueToAchievement(type, amount);
    }

    /// <summary>???????? FSPGF???? AchievementToBasicGameValue?</summary>
    public static int AchievementToFSPGF(ValueType type, int achievement)
    {
        if (!IsFSPGF(type)) return 0;
        return AchievementToBasicGameValue(type, achievement);
    }

    /// <summary>?????SBN ???</summary>
    public static int SBNToAchievement(ValueType type, int amount)
    {
        if (!IsSBN(type)) return 0;
        return BasicGameValueToAchievement(type, amount);
    }

    /// <summary>???????? SBN</summary>
    public static int AchievementToSBN(ValueType type, int achievement)
    {
        if (!IsSBN(type)) return 0;
        return AchievementToBasicGameValue(type, achievement);
    }


    /// <summary>?????SBN ???</summary>
    public static int TotalRecruitedPopulationToAchievement(ValueType type, int amount)
    {
        if (!IsTotalRecruitedPopulation(type)) return 0;
        return BasicGameValueToAchievement(type, amount);
    }

    /// <summary>???????? SBN</summary>
    public static int AchievementToTotalRecruitedPopulation(ValueType type, int achievement)
    {
        if (!IsTotalRecruitedPopulation(type)) return 0;
        return AchievementToBasicGameValue(type, achievement);
    }


    // ==================== Power Type Conversion ====================

    /// <summary>
    /// ????????????FSPGFp?
    /// ????????????
    /// </summary>
    public static int ScienceToCharacterValue(ValueType type, int currentLevel, int targetLevel)
    {
        if (!IsCharacterValue(type)) return 0;
        return CalculateScienceCost(currentLevel, targetLevel);
    }

    /// <summary>
    /// ????????????FSPGFp?
    /// ???Achievement ? Science ? FSPGFp
    /// </summary>
    public static int AchievementToCharacterValue(ValueType type, int currentLevel, int targetLevel)
    {
        if (!IsCharacterValue(type)) return 0;

        return CalculateScienceCost(currentLevel, targetLevel);
    }

    /// <summary>
    /// ??????????FSPGFp?????????
    /// </summary>
    public static int CharacterValueToScience(ValueType type, int currentLevel, int targetLevel)
    {
        if (!IsCharacterValue(type)) return 0;
        return CalculateScienceCost(currentLevel, targetLevel);
    }

    /// <summary>
    /// ??????????FSPGFp?????????
    /// </summary>
    public static int CharacterToAchievement(ValueType type, int currentLevel, int targetLevel)
    {
        if (!IsCharacterValue(type)) return 0;

        int scienceCost = CalculateScienceCost(currentLevel, targetLevel);
        return FSPGFToAchievement(ValueType.Science, scienceCost);
    }

    // ==================== Growth Calculation ====================

    /// <summary>
    /// ?????????????????????
    /// ??????????
    /// </summary>
    private static int CalculateScienceCost(int currentLevel, int targetLevel)
    {
        if (targetLevel <= currentLevel) return 0;

        int totalCost = 0;

        for (int level = currentLevel + 1; level <= targetLevel; level++)
        {
            int tier = (level - 1) / TierLevelStep;  // ? 10 ???????
            float levelCost = BaseScienceCostPerLevel * (level - 1) * (tier + 1);  // ????
            totalCost += (int)Math.Round(levelCost);
        }

        return totalCost;
    }



    public static int GetRequiredExpToLvUp(int CurrentLevel,int MaxLevel)
    {
        bool HasUnlimitedLevel = MaxLevel == int.MaxValue;
        if (HasUnlimitedLevel || CurrentLevel < MaxLevel)
        {
            return (int)(100 * Math.Pow(1.1, CurrentLevel));
        }
        else
        {
            return 0;
        }
    }


}
