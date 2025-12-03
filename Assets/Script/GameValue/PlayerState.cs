using System;
using System.Collections.Generic;
using UnityEngine;

#region Value
public enum Season
{
    Spring = 0,
    Summer = 1,
    Autumn = 2,
    Winter = 3,
}

#endregion

public class PlayerState
{
    public event Action OnPlayerStateChanged; 

    private string name;
    private string countryENName = GameValueConstants.Country_Prologue;
    private int currentYear = GameValueConstants.Year_Prologue;
    private Season currentSeason = GameValueConstants.Season_Prologue;
    private bool isLastJudgmentHappen = false;
    private string endName;

    public string Name
    {
        get => name;
        set => SetProperty(ref name, value);
    }

    public string CountryENName
    {
        get => countryENName;
        set => SetProperty(ref countryENName, value);
    }

    public int CurrentYear
    {
        get => currentYear;
        set => SetProperty(ref currentYear, value);
    }

    public Season CurrentSeason
    {
        get => currentSeason;
        set => SetProperty(ref currentSeason, value);
    }

    public bool IsLastJudgmentHappen
    {
        get => isLastJudgmentHappen;
        set => SetProperty(ref isLastJudgmentHappen, value);
    }

    public string ENDName
    {
        get => endName;
        set => SetProperty(ref endName, value);
    }

    private void SetProperty<T>(ref T field, T value)
    {
        if (!EqualityComparer<T>.Default.Equals(field, value))
        {
            field = value;
            OnPlayerStateChanged?.Invoke();
        }
    }

    public void AddSeason()
    {
        int nextSeason = ((int)currentSeason + 1) % 4; 
        CurrentSeason = (Season)nextSeason;
        if (nextSeason == (int)Season.Spring)
        {
            CurrentYear += 1;
        }
    }

    public bool IsEndGame()
    {
        Debug.LogWarning("Test remember change back");
        return CurrentYear == 1442;
    }

    public PlayerStateSaveData GetSaveData()
    {
        return new PlayerStateSaveData(this);
    }

    public void SetSaveData(PlayerStateSaveData saveData)
    {
        if (saveData == null) return;
        Name = saveData.Name;
        CountryENName = saveData.CountryENName;
        CurrentYear = saveData.CurrentYear;
        CurrentSeason = saveData.CurrentSeason;
        IsLastJudgmentHappen = saveData.IsLastJudgmentHappen;
        ENDName = saveData.ENDName;
    }
}

[System.Serializable]
public class PlayerStateSaveData
{
    public string Name;
    public string CountryENName;
    public int CurrentYear;
    public Season CurrentSeason;
    public bool IsLastJudgmentHappen;
    public string ENDName;
    public PlayerStateSaveData(PlayerState playerState)
    {
        if (playerState == null) return;

        Name = playerState.Name;
        CountryENName = playerState.CountryENName;
        CurrentYear = playerState.CurrentYear;
        CurrentSeason = playerState.CurrentSeason;
        IsLastJudgmentHappen = playerState.IsLastJudgmentHappen;
        ENDName = playerState.ENDName;
    }

}
