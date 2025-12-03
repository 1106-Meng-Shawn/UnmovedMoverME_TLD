using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

using static GetColor;
using static FormatNumber;

public static class GetString
{
    public static string[] SeasonsKey = { "Spring", "Summer", "Fall", "Winter" };


    public static string GetFavorabilityString(int Favorability)
    {
        return FormatNumberToString(Favorability);
    }

    public static string GetResourceStringWithSprite(ValueType type)
    {
        return "";
    }

    public static string GetMaxLimitString(int MaxLimit)
    {
        return GetValueColorString(MaxLimit.ToString(),ValueType.TotalRecruitedPopulation);
    }


    public static string GetSeasonString(Season season)
    {
        return LocalizationSettings.StringDatabase.GetLocalizedString("GameSetting", SeasonsKey[(int)season]); ;
    }

    public static string GetClassRoleString(CharacterRole roleClass)
    {
        string classRoleName = null;

        string currentLanguage = LocalizationSettings.SelectedLocale.Identifier.Code;
        classRoleName = LocalizationSettings.StringDatabase.GetLocalizedString("ClassRole", roleClass.ToString());

        return classRoleName;
    }

    public static string GetHealthAndMaxHealthString(int Health, int maxHealth)
    {
        return GetValueColorString($"{FormatNumberToString(Health)} / {FormatNumberToString(maxHealth)}", ValueColorType.Pop);
    }


    public static string GetLvAndMaxLevelString(int CurrentLevel, int MaxLevel)
    {   
        if (MaxLevel == int.MaxValue) return $"Lv.{CurrentLevel} / ? bug";
        return $"Lv.{CurrentLevel} / {MaxLevel}";
    }

    public static string GetExpAndReqExpString(int CurrentExp, int NeedExp)
    {
        return $"{FormatNumberToString(CurrentExp)} / {FormatNumberToString(NeedExp)}";
    }



    public static string GetMoveNumString(int MoveCount)
    {
        string MoveString = $"* {MoveCount}";
        return GetValueColorString(MoveString,ValueColorType.CanMove);
    }

    public static string GetTimeString(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        string timeString = string.Format("{0:00}:{1:00}", minutes, seconds);
        return timeString;
    }


}
