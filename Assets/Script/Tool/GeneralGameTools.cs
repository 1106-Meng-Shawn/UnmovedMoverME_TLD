using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class GeneralGameTools
{

    public static bool IsCtrlDown()
    {
        return (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl));
    }


    public static bool IsBasicGameValue(ValueType type)
    {
        bool IsBasicGameValue = type == ValueType.TotalRecruitedPopulation ||
               IsSBN(type) ||
               IsFSPGF(type);

        return IsBasicGameValue;
    }

    public static bool IsSBN(ValueType type)
    {
        bool IsSBN = type == ValueType.Scout ||
               type == ValueType.Build ||
               type == ValueType.Negotiation;

        return IsSBN;
    }

    public static bool IsFSPGF(ValueType type)
    {
        bool IsFSPGF = type == ValueType.Food;
        return IsFSPGF;
    }


    public static bool IsTotalRecruitedPopulation(ValueType type)
    {
        return type == ValueType.TotalRecruitedPopulation;
    }

    public static bool IsCharacterValue(ValueType type)
    {
        bool IsCharacterValue = type == ValueType.Attack ||
               type == ValueType.Defense ||
               type == ValueType.Magic ||
               type == ValueType.Speed ||
               type == ValueType.Lucky ||

               type == ValueType.Leadership ||
               type == ValueType.Scout ||
               type == ValueType.Build ||
               type == ValueType.Negotiation ||
               type == ValueType.Charm ||

               type == ValueType.FoodP ||
               type == ValueType.ScienceP ||
               type == ValueType.PoliticsP ||
               type == ValueType.GoldP ||
               type == ValueType.FaithP;

        return IsCharacterValue;
    }

}
