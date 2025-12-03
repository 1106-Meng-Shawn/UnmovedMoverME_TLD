using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GetColor;

#region enum

public enum ValueType
{
    Attack,
    Defense,
    Magic,
    Speed,
    Lucky,

    // ???
    Food,
    Science,
    Politics,
    Gold,
    Faith,

    FoodP,
    ScienceP,
    PoliticsP,
    GoldP,
    FaithP,

    // ???
    Leadership,
    Scout,
    Build,
    Negotiation,
    Charm,

    Favorability,
    Move,
    Force,
    MaxForce,
    Experience,

    Population,
    TotalRecruitedPopulation,
    Achievement,
    Level
}


public enum RowType
{
   canMove,cantMove, sel,enemy,player
}

public enum ValueColorType
{
    // ????
    CanMove,
    CantMove,
    Increase,
    Decrease,

    // ???
    Enemy,
    Player,
    Achievement,

    // ???
    Attack,
    Defense,
    Magic,
    Speed,
    Lucky,

    // ???
    Food,
    Science,
    Politics,
    Gold,
    Faith,

    // ???
    Leadership,
    Scout,
    Build,
    Negotiation,
    Charm,

    // ??
    Pop,
}

#endregion
public class GetColor 
{

    public static Vector4 ColorToVector(Color32 color)
    {
        return new Vector4(color.r, color.g, color.b, color.a);
    }


    public static Color32 VectorToColor(Vector4 vec)
    {
        return new Color32((byte)vec.x,(byte)vec.y,(byte)vec.z,(byte)vec.w);
    }


    public static string GetCountryColorString(string originalString, string CountryName)
    {
        Color32 CountryColor = GameValue.Instance.GetCountryColor(CountryName);
        string hexColor = ColorUtility.ToHtmlStringRGBA(CountryColor);
        return $"<color=#{hexColor}>{originalString}</color>";
    }



    public static string GetColorString(string originalString,Color32 color)
    {
        string hexColor = ColorUtility.ToHtmlStringRGBA(color);
        return $"<color=#{hexColor}>{originalString}</color>";
    }

    public static Color GetSkillFunctionTypeColor(SkillFunctionType functionType)
    {
        if (functionType == SkillFunctionType.Attack) return new Color32(220, 53, 69, 255);
        if (functionType == SkillFunctionType.Heal) return new Color32(40, 167, 69, 255);
        if (functionType == SkillFunctionType.Buff) return new Color32(51, 102, 204, 255);
        if (functionType == SkillFunctionType.Debuff) return new Color32(153, 102, 204, 255);
        if (functionType == SkillFunctionType.NONE) return new Color32(0, 0, 0, 0);
        if (functionType == SkillFunctionType.Passive) return  new Color32(108, 117, 125, 255);

        Debug.Log($"Unknown skill type {functionType} in GetSkillColor");
        return Color.black;
    }

    public static Color GetSkillRareColor(int rarity)
    {
        return rarity switch
        {
            1 => new Color32(255, 255, 255, 255), // ?? ?
            2 => new Color32(0, 255, 0, 255),     // ?? ?
            3 => new Color32(0, 128, 255, 255),   // ?? ?
            4 => new Color32(160, 32, 240, 255),  // ?? ?
            5 => new Color32(255, 165, 0, 255),   // ?? ?
            _ => Color.white                        // ?? ?
        };
    }


    public static Color32 GetRowColor(RowType type)
    {
        switch (type)
        {
            case RowType.canMove: return new Color32(230, 190, 120, 255);
            case RowType.cantMove: return new Color32(192, 144, 72, 255);
            case RowType.sel: return new Color32(220, 150, 50, 255);
            case RowType.enemy: return new Color32(192, 144, 72, 255);
            case RowType.player: return new Color32(230, 190, 120, 255);

        }

        return new Color32(0,0, 0, 0);
    }


    public static string GetColorString(string originalString, int valueType, int index)
    {
        Color32 color = GetValueColor(valueType, index);
        string hexColor = ColorUtility.ToHtmlStringRGBA(color);
        return $"<color=#{hexColor}>{originalString}</color>";
    }

    public static string GetRegionValueColorString(string originalString, int index)
    {
        Color32 color = GetRegionValueColor(index);
        string hexColor = ColorUtility.ToHtmlStringRGBA(color);
        return $"<color=#{hexColor}>{originalString}</color>";
    }

    public static Color32 GetRegionValueColor(int index)
    {

        switch (index)
        {
            case 0: 
            case 1: 
            case 2:
            case 3:
            case 4:
                return GetParameterValueColor(index);
            case 5:
                return GetParameterValueColor(index);

            default:
                return GetValueColor(ValueColorType.Pop);

        }

        return new Color32(0, 0, 0, 255);
    }

    


    public static Color32 GetValueColor(ValueType type)
    {
        switch (type)
        {
            // ??
            case ValueType.Attack: return GetValueColor(ValueColorType.Attack);
            case ValueType.Defense: return GetValueColor(ValueColorType.Defense);
            case ValueType.Magic: return GetValueColor(ValueColorType.Magic);
            case ValueType.Speed: return GetValueColor(ValueColorType.Speed);
            case ValueType.Lucky: return GetValueColor(ValueColorType.Lucky);

            // ??
            case ValueType.Food:
            case ValueType.FoodP:
                return GetValueColor(ValueColorType.Food);
            case ValueType.Science:
            case ValueType.ScienceP:
                return GetValueColor(ValueColorType.Science);
            case ValueType.Politics:
            case ValueType.PoliticsP:
                return GetValueColor(ValueColorType.Politics);
            case ValueType.Gold:
            case ValueType.GoldP:
                return GetValueColor(ValueColorType.Gold);
            case ValueType.Faith:
            case ValueType.FaithP:
                return GetValueColor(ValueColorType.Faith);

            case ValueType.Leadership: return GetValueColor(ValueColorType.Leadership);
            case ValueType.Scout: return GetValueColor(ValueColorType.Scout);
            case ValueType.Build: return GetValueColor(ValueColorType.Build);
            case ValueType.Negotiation: return GetValueColor(ValueColorType.Negotiation);
            case ValueType.Charm: return GetValueColor(ValueColorType.Charm);

            case ValueType.TotalRecruitedPopulation: return GetValueColor(ValueColorType.Pop);

            case ValueType.Achievement: return GetValueColor(ValueColorType.Achievement);

            default:
                return new Color32(234, 234, 234, 255);
        }
    }



    public static Color32 GetValueColor(ValueColorType type)
    {
        switch (type)
        {
            // ??
            case ValueColorType.Attack: return GetValueColor(0, 0);
            case ValueColorType.Defense: return GetValueColor(0, 1);
            case ValueColorType.Magic: return GetValueColor(0, 2);
            case ValueColorType.Speed: return GetValueColor(0, 3);
            case ValueColorType.Lucky: return GetValueColor(0, 4);

            // ??
            case ValueColorType.Food: return GetValueColor(1, 0);
            case ValueColorType.Science: return GetValueColor(1, 1);
            case ValueColorType.Politics: return GetValueColor(1, 2);
            case ValueColorType.Gold: return GetValueColor(1, 3);
            case ValueColorType.Faith: return GetValueColor(1, 4);

            // ??
            case ValueColorType.Leadership: return GetValueColor(2, 0);
            case ValueColorType.Scout: return GetValueColor(2, 1);
            case ValueColorType.Build: return GetValueColor(2, 2);
            case ValueColorType.Negotiation: return GetValueColor(2, 3);
            case ValueColorType.Charm: return GetValueColor(2, 4);

            // ?? / ??
            case ValueColorType.Pop: return new Color32(234, 234, 234, 255);
            case ValueColorType.CanMove: return new Color32(0, 255, 0, 255);
            case ValueColorType.CantMove: return new Color32(115, 115, 115, 255);

            case ValueColorType.Increase: return new Color32(80, 200, 120, 255);
            case ValueColorType.Decrease: return new Color32(220, 70, 70, 255);
            case ValueColorType.Enemy: return new Color32(192, 144, 72, 255);
            case ValueColorType.Player: return new Color32(230, 190, 120, 255);
            case ValueColorType.Achievement: return new Color32(254, 250, 67, 255);

            default:
                return new Color32(234, 234, 234, 255);
        }
    }

    public static string GetValueColorString(string originalString, ValueType type)
    {
        Color32 color = GetValueColor(type);
        string hexColor = ColorUtility.ToHtmlStringRGBA(color);
        return $"<color=#{hexColor}>{originalString}</color>";
    }


    public static string GetValueColorString(string originalString, ValueColorType type)
    {
        Color32 color = GetValueColor(type);
        string hexColor = ColorUtility.ToHtmlStringRGBA(color);
        return $"<color=#{hexColor}>{originalString}</color>";
    }


    public static Color32 GetValueColor(int valueType, int index)
    {

        switch (valueType)
        {
            case 0: return GetBattleValueColor((BattleValue)index); 
            case 1: return GetParameterValueColor(index); 
            case 2: return GetHelpValueColor(index); 
        }

        return new Color32(0, 0, 0, 255);
    }

    public static string GetBattleValueColorString(string originalString, BattleValue type)
    {
        Color32 color = GetBattleValueColor(type);
        string hexColor = ColorUtility.ToHtmlStringRGBA(color);
        return $"<color=#{hexColor}>{originalString}</color>";
    }




    public static Color32 GetBattleValueColor(BattleValue index)
    {
        switch (index)
        {
            case BattleValue.Attack: return new Color32(255, 0, 0, 255);
            case BattleValue.Defense: return new Color32(0, 168, 255, 255);
            case BattleValue.Magic: return new Color32(228, 0, 255, 255);
            case BattleValue.Speed: return new Color32(244, 246, 244, 255);
            case BattleValue.Lucky: return new Color32(254, 254, 0, 255);

        }

        return new Color32(255, 255, 255, 255);

    }

    public static Color32 GetParameterValueColor(int index)
    {
        switch (index)
        {
            case 0: return new Color32(80, 125, 2, 255);
            case 1: return new Color32(0, 0, 196, 255);
            case 2: return new Color32(165, 18, 159, 255);
            case 3: return new Color32(254, 212, 1, 255);
            case 4: return new Color32(254, 254, 254, 255);

        }

        return new Color32(255, 255, 255, 255);

    }

    public static Color32 GetHelpValueColor(int index)
    {
        switch (index)
        {
            case 0: return new Color32(217, 20, 0, 255);
            case 1: return new Color32(255, 138, 0, 255);
            case 2: return new Color32(150, 70, 0, 255);
            case 3: return new Color32(255, 252, 0, 255);
            case 4: return new Color32(236, 122, 0, 255);

        }

        return new Color32(255, 255, 255, 255);

    }


    public static Color32 GetSeasonColor(Season season)
    {
        switch (season)
        {
            case Season.Spring: return new Color32(0, 147, 0, 255);
            case Season.Summer: return new Color32(216, 60, 73, 255);
            case Season.Autumn: return new Color32(120, 68, 0, 255);
            case Season.Winter: return new Color32(0, 109, 202, 255);
            default: return new Color32(0, 0, 0, 0);
        }

    }



}
