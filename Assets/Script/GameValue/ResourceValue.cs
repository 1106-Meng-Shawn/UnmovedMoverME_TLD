using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using static MultiplePlaythroughsGameCharacterRowControl;

#region Value

public class ResourceConstants
{
    public static float InitFood = 0;
    public static float InitScience = 0;
    public static float InitPolitics = 0;
    public static float InitGold = 0;
    public static float InitFaith = 0;
    public static int InitTotalRecruitedPopulation = 0;

    public static float InitScout = 0;
    public static float InitBuild = 0;
    public static float InitNegotiation = 0;
    public static int InitAchievement = 0;
}


#endregion

[System.Serializable]
public class ResourceValue
{
    public event Action OnResourceChanged;
    public event System.Action<string, double> OnSBNValueChanged;
    //  private float[] resources = new float[5] { ResourceConstants.InitFood, ResourceConstants.InitScience, ResourceConstants.InitPolitics, ResourceConstants.InitGold, ResourceConstants.InitFaith }; 

    private float food = ResourceConstants.InitFood;
    private float science = ResourceConstants.InitScience;
    private float politics = ResourceConstants.InitPolitics;
    private float gold = ResourceConstants.InitGold;
    private float faith = ResourceConstants.InitFaith;

    private int totalRecruitedPopulation = ResourceConstants.InitTotalRecruitedPopulation;

    private float scout = ResourceConstants.InitScout;
    private float build = ResourceConstants.InitBuild;
    private float negotiation = ResourceConstants.InitNegotiation;
    private int achievement = 0;


    public float Food
    {
        get => food;
        set => SetProperty(ref food, value, nameof(Food));
    }


    public float Science
    {
        get => science;
        set => SetProperty(ref science, value, nameof(Science));
    }


    public float Politics
    {
        get => politics;
        set => SetProperty(ref politics, value, nameof(Politics));
    }


    public float Gold
    {
        get => gold;
        set => SetProperty(ref gold, value, nameof(Gold));
    }


    public float Faith
    {
        get => faith;
        set => SetProperty(ref faith, value, nameof(Faith));
    }



    public int TotalRecruitedPopulation
    {
        get => totalRecruitedPopulation;
        set => SetProperty(ref totalRecruitedPopulation, value);
    }

    public float Scout
    {
        get => scout;
        set => SetProperty(ref scout, value, nameof(Scout));
    }

    public float Build
    {
        get => build;
        set => SetProperty(ref build, value, nameof(Build));
    }

    public float Negotiation
    {
        get => negotiation;
        set => SetProperty(ref negotiation, value, nameof(Negotiation));
    }

    public int Achievement
    {
        get => achievement;
        set => SetProperty(ref achievement, value);
    }

    public void Add(ValueType type, float amount)
    {
        switch (type)
        {
            case ValueType.Food: Food += (int)amount; break;
            case ValueType.Science: Science += (int)amount; break;
            case ValueType.Politics: Politics += (int)amount; break;
            case ValueType.Gold: Gold += (int)amount; break;
            case ValueType.Faith: Faith += (int)amount; break;
            case ValueType.TotalRecruitedPopulation: TotalRecruitedPopulation += (int)amount;break;
            case ValueType.Scout:Scout += amount;break;
            case ValueType.Build:Build += amount;break;
            case ValueType.Negotiation:Negotiation += amount;break;
            case ValueType.Achievement:Achievement += (int)amount;break;
            default:throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }


    public float GetResourceValue(ValueType type)
    {
        switch (type)
        {
            case ValueType.Food: return Food;
            case ValueType.Science: return Science;
            case ValueType.Politics: return Politics;
            case ValueType.Gold: return Gold;
            case ValueType.Faith: return Faith;

            case ValueType.TotalRecruitedPopulation: return TotalRecruitedPopulation;

            case ValueType.Scout: return Scout;
            case ValueType.Build: return Build;
            case ValueType.Negotiation: return Negotiation;
            case ValueType.Achievement: return Achievement;
            default:
                Debug.LogError($"{type}");
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }

    private void SetProperty<T>(ref T field, T value)
    {
        if (!EqualityComparer<T>.Default.Equals(field, value))
        {
            field = value;
            OnResourceChanged?.Invoke();
        }
    }

    private bool SetProperty<T>(ref T field, T value, string sbnKey = null)
    {
        if (!EqualityComparer<T>.Default.Equals(field, value))
        {
            field = value;
            OnResourceChanged?.Invoke();

            if (sbnKey != null && value is IConvertible)
            {
                OnSBNValueChanged?.Invoke(sbnKey, Convert.ToDouble(value));
            }

            return true;
        }
        return false;
    }


    public ResourceValueSaveData GetSaveData()
    {
        ResourceValueSaveData resourceValueSaveData = new ResourceValueSaveData(this);
        return resourceValueSaveData;
    }

    public void SetSaveData(ResourceValueSaveData saveData)
    {
        if (saveData == null) return;

        Food = saveData.Food;
        Science = saveData.Science;
        Politics = saveData.Politics;
        Gold = saveData.Gold;
        Faith = saveData.Faith;

        TotalRecruitedPopulation = saveData.TotalRecruitedPopulation;
        Scout = saveData.Scout;
        Build = saveData.Build;
        Negotiation = saveData.Negotiation;
        Achievement = saveData.Achievement;
    }
}

[System.Serializable]
public class ResourceValueSaveData
{
    public float Food;
    public float Science;
    public float Politics;
    public float Gold;
    public float Faith;

    public int TotalRecruitedPopulation;
    public float Scout;
    public float Build;
    public float Negotiation;
    public int Achievement;
    public ResourceValueSaveData(ResourceValue resourceValue)
    {
        if (resourceValue == null)
        {
            Debug.LogWarning("resourceValue is null!");
            return;
        }

         Food = resourceValue.Food;
         Science = resourceValue.Science;
         Politics = resourceValue.Politics;
         Gold = resourceValue.Gold;
         Faith = resourceValue.Faith;

        TotalRecruitedPopulation = resourceValue?.TotalRecruitedPopulation ?? 0;
        Scout = resourceValue?.Scout ?? 0;
        Build = resourceValue?.Build ?? 0;
        Negotiation = resourceValue?.Negotiation ?? 0;
        Achievement = resourceValue?.Achievement ?? 0;
    }

}
