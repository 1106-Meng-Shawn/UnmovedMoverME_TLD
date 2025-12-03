using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;
using static GetColor;
using static GetString;

[System.Serializable]
public class GameValueSaveData
{
    public ResourceValueSaveData ResourceValueSaveData;
    public PlayerStateSaveData PlayerStateSaveData;

    /*public List<CharacterSaveData> allCharacters = new List<CharacterSaveData>();
    public List<ItemSaveData> allItems = new List<ItemSaveData>();
    public List<RegionValueData> allRegions = new List<RegionValueData>();*/

    public Dictionary<int, CharacterSaveData> allCharacters = new Dictionary<int, CharacterSaveData>();
    public Dictionary<int, ItemSaveData> allItems = new Dictionary<int, ItemSaveData>();
    public Dictionary<int, RegionValueSaveData> allRegions = new Dictionary<int, RegionValueSaveData>();

    public float SupportRate;

    public bool hasReichszepter = false;
    public bool hasReichsapfel = false;
    public bool hasZeremonienschwert = false;
    public int TotalPopulation = 0;
    public int playerCharacterNum = 0;
    public int playerItemCount = 0;
    public int playerRegionNum = 0;

    public CountryManagerSaveData CountryManagerSaveData;
    public StoryControlSaveData StoryControlSaveData;

    public GameValueSaveData(GameValue gameValue)
    {
        if (gameValue == null)
        {
            return;
        }



        ResourceValueSaveData = gameValue.GetResourceValue().GetSaveData();
        PlayerStateSaveData = gameValue.GetPlayerState().GetSaveData();
        CountryManagerSaveData = gameValue.GetCountryManager().GetSaveData();
        StoryControlSaveData = gameValue.GetStoryControl().GetSaveData();

        SupportRate = gameValue.GetTotalSupportRate();

        // Characters
        List<Character> allCurrentCharactersInGame = gameValue.GetAllCharacters();
        allCharacters = new Dictionary<int, CharacterSaveData>();
        foreach (var character in allCurrentCharactersInGame)
        {
            CharacterSaveData saveData = new CharacterSaveData(character);
            allCharacters[character.GetCharacterID()] = saveData; 
        }
        playerCharacterNum = gameValue.GetPlayerCharacters().Count;

        // Regions
        List<RegionValue> allRegionArray = gameValue.GetAllRegionValues();
        playerRegionNum = 0;
        TotalPopulation = 0;
        allRegions = new Dictionary<int, RegionValueSaveData>();
        foreach (var region in allRegionArray)
        {
            RegionValueSaveData saveData = new RegionValueSaveData(region);
            allRegions[region.GetRegionID()] = saveData; 
            if (region.GetCountryENName() == gameValue.GetPlayerCountryENName())
            {
                playerRegionNum ++;
                TotalPopulation += region.GetRegionPopulation();

            }
        }

        // Items
        allItems = new Dictionary<int, ItemSaveData>();
        foreach (var item in gameValue.GetAllItems())
        {
            if (item != null)
            {
                int id = item.GetID();

                if (id == ItemConstants.ReichszepterID) hasReichszepter = true;
                if (id == ItemConstants.ReichsapfelID) hasReichsapfel = true;
                if (id == ItemConstants.ZeremonienschwertID) hasZeremonienschwert = true;

                allItems[id] = new ItemSaveData(item); 

                if (item.PlayerHas()) playerItemCount++;
            }
        }
    }

    public String GetTurnString()
    {
        string currentLanguage = LocalizationSettings.SelectedLocale.Identifier.Code;

        switch (currentLanguage)
        {
            case "en": return UpdateTurnTextEN();
            default: return UpdateTurnTextCHJA();
        }

        return "BUG IN GameValue GetTurnString";

    }

    string UpdateTurnTextEN()
    {
        string year = GetCurrentYear().ToString();
        string season = GetCurrentSeasonString();

        Color32 seasonColor = GetSeasonColor(PlayerStateSaveData.CurrentSeason);
        string hexColor = ColorUtility.ToHtmlStringRGB(seasonColor);
        return $"{year} <color=#{hexColor}>{season}</color>";

    }

    string UpdateTurnTextCHJA()
    {
        string year = GetCurrentYear().ToString();

        string[] localizedSeasons = new string[4];

        for (int i = 0; i < 4; i++)
        {
            string seasonName = GetSeasonString((Season)i);
            if (i == (int)GetCurrentSeason())
            {
                Color32 seasonColor = GetSeasonColor((Season)i);
                string hexColor = ColorUtility.ToHtmlStringRGB(seasonColor);
                localizedSeasons[i] = $"<b><color=#{hexColor}>{seasonName}</color></b>";
            }
            else
            {
                localizedSeasons[i] = seasonName;
            }
        }

        return $"{year} {localizedSeasons[0]}{localizedSeasons[1]}{localizedSeasons[2]}{localizedSeasons[3]}";
    }


    string GetCurrentSeasonString()
    {
        return GetSeasonString(GetCurrentSeason());
    }

    public string GetAchievementString()
    {
        return GetValueColorString(GetAchievement().ToString("N0"), ValueColorType.Achievement);
    }


    public string GetPlayerCountry()
    {
        return PlayerStateSaveData.CountryENName;
    }


    int GetCurrentYear()
    {
        return PlayerStateSaveData.CurrentYear;
    }

    Season GetCurrentSeason()
    {
        return PlayerStateSaveData.CurrentSeason;
    }

    public int GetAchievement()
    {
        return (int)ResourceValueSaveData.Achievement;
    }

    public String GetPlayerName()
    {
        return PlayerStateSaveData.Name;
    }



}
