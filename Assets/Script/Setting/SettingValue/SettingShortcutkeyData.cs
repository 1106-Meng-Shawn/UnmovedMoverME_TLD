using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

[Serializable]
public class SettingShortcutkeyData
{
    public GeneralShortcutkeyData GeneralShortcutkeyData = new();
    public GameShortcutkeyData GameShortcutkeyData = new();
    public BattleShortcutkeyData BattleShortcutkeyData = new();
    public ExploreShortcutkeyData ExploreShortcutkeyData = new();
    public TextShortcutkeyData TextShortcutkeyData = new();

    public Func<ShortcutKey, bool> CreateValidator(ShortcutKeyCategory category)
    {
        SettingShortcutkeyData settingShortcutkeyData = SettingValue.Instance
            .settingValueData
            .settingShortcutkeyData;

        return (key) => {
            string duplicate = settingShortcutkeyData.ValidateKey(key, category);
            if (duplicate != null)
            {
                return false;
            }
            return true;
        };
    }




    public string ValidateKey(ShortcutKey key, ShortcutKeyCategory category)
    {
        if (key == ShortcutKey.None)
            return null; // None is always valid

        // Check within the same category
        string duplicate = CheckDuplicateInCategory(key, GetDataObjectByCategory(category));
        if (duplicate != null)return $"{category}.{duplicate}";

        // Check in General
        duplicate = CheckDuplicateInCategory(key, GeneralShortcutkeyData);
        if (duplicate != null)return $"General.{duplicate}";

        // If this is General, check all other categories
        if (category == ShortcutKeyCategory.General)
        {
            duplicate = CheckDuplicateInAllOtherCategories(key);
            if (duplicate != null)return duplicate;
        }

        return null; // No conflict found
    }

    /// <summary>
    /// Check for duplicate key within a specific category
    /// </summary>
    private string CheckDuplicateInCategory(ShortcutKey key, object dataObject)
    {
        if (dataObject == null)
            return null;

        Type type = dataObject.GetType();
        PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (PropertyInfo prop in properties)
        {
            // Check if this is a ShortcutKey property
            if (prop.PropertyType == typeof(ShortcutKey))
            {
                ShortcutKey value = (ShortcutKey)prop.GetValue(dataObject);
                if (value == key)
                    return prop.Name;
            }
        }

        return null;
    }

    /// <summary>
    /// Check for duplicates in all categories except General (used when validating General keys)
    /// </summary>
    private string CheckDuplicateInAllOtherCategories(ShortcutKey key)
    {
        // Check GameShortcutkeyData
        string duplicate = CheckDuplicateInCategory(key, GameShortcutkeyData);
        if (duplicate != null)
            return $"Game.{duplicate}";

        // Check BattleShortcutkeyData
        duplicate = CheckDuplicateInBattleCategory(key);
        if (duplicate != null)
            return $"Battle.{duplicate}";

        // Check ExploreShortcutkeyData
        duplicate = CheckDuplicateInCategory(key, ExploreShortcutkeyData);
        if (duplicate != null)
            return $"Explore.{duplicate}";

        // Check TextShortcutkeyData
        duplicate = CheckDuplicateInCategory(key, TextShortcutkeyData);
        if (duplicate != null)
            return $"Text.{duplicate}";

        return null;
    }

    /// <summary>
    /// Special check for BattleShortcutkeyData which has arrays and lists
    /// </summary>
    private string CheckDuplicateInBattleCategory(ShortcutKey key)
    {
        // Check 2D array battlePositionKeys
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (BattleShortcutkeyData.battlePositionKeys[i, j] == key)
                    return $"battlePositionKeys[{i},{j}]";
            }
        }

        // Check nothingShortcutKey
        if (BattleShortcutkeyData.nothingShortcutKey == key)
            return "nothingShortcutKey";

        // Check SkillShortcutKeys list
        for (int i = 0; i < BattleShortcutkeyData.SkillShortcutKeys.Count; i++)
        {
            if (BattleShortcutkeyData.SkillShortcutKeys[i] == key)
                return $"SkillShortcutKeys[{i}]";
        }

        // Check enemyShortcutKey
        if (BattleShortcutkeyData.enemyShortcutKey == key)
            return "enemyShortcutKey";

        // Check playerShortcutKey
        if (BattleShortcutkeyData.playerShortcutKey == key)
            return "playerShortcutKey";

        // Check EscapeShortcutKey
        if (BattleShortcutkeyData.EscapeShortcutKey == key)
            return "EscapeShortcutKey";

        return null;
    }

    /// <summary>
    /// Get data object by category enum
    /// </summary>
    private object GetDataObjectByCategory(ShortcutKeyCategory category)
    {
        switch (category)
        {
            case ShortcutKeyCategory.General:
                return GeneralShortcutkeyData;
            case ShortcutKeyCategory.Game:
                return GameShortcutkeyData;
            case ShortcutKeyCategory.Battle:
                return BattleShortcutkeyData;
            case ShortcutKeyCategory.Explore:
                return ExploreShortcutkeyData;
            case ShortcutKeyCategory.Text:
                return TextShortcutkeyData;
            default:
                return null;
        }
    }
}

/// <summary>
/// Enum to identify shortcut key categories
/// </summary>
public enum ShortcutKeyCategory
{
    General,
    Game,
    Battle,
    Explore,
    Text
}

/// <summary>
/// Example: General shortcut keys
/// </summary>
[Serializable]
public class GeneralShortcutkeyData
{
    public ShortcutKey SwitchKey = ShortcutKey.Tab;
    public ShortcutKey SaveKey = ShortcutKey.F5;
    public ShortcutKey LoadKey = ShortcutKey.F6;
    public ShortcutKey SaveQuickKey = ShortcutKey.F7;
    public ShortcutKey LoadQuickKey = ShortcutKey.F8;
    public ShortcutKey SettingKey = ShortcutKey.F9;
    public ShortcutKey MusicKey = ShortcutKey.F10;
    public ShortcutKey GuideKey = ShortcutKey.F11;
    public ShortcutKey MenuKey = ShortcutKey.F12;
}



[Serializable]
public class GameShortcutkeyData
{
    // Event triggered when any shortcut key is modified
    public event Action OnShortcutKeysChanged;

    // Shortcut keys
    private ShortcutKey scoutKey = ShortcutKey.Alpha7;
    private ShortcutKey buildKey = ShortcutKey.Alpha8;
    private ShortcutKey negotiationKey = ShortcutKey.Alpha9;
    private ShortcutKey rightColKey = ShortcutKey.Alpha0;
    private ShortcutKey nextTurnKey = ShortcutKey.Space;
    private ShortcutKey electorKey = ShortcutKey.Alpha1;
    private ShortcutKey countryKey = ShortcutKey.Alpha2;
    private ShortcutKey regionKey = ShortcutKey.Alpha3;
    private ShortcutKey characterKey = ShortcutKey.Alpha4;
    private ShortcutKey itemKey = ShortcutKey.Alpha5;
    private ShortcutKey recruitKey = ShortcutKey.Alpha6;

    private ShortcutKey zoomOutKey = ShortcutKey.Q;
    private ShortcutKey arrowUpKey = ShortcutKey.W;
    private ShortcutKey zoomInKey = ShortcutKey.E;
    private ShortcutKey arrowLeftKey = ShortcutKey.A;
    private ShortcutKey arrowDownKey = ShortcutKey.S;
    private ShortcutKey arrowRightKey = ShortcutKey.D;


    public ShortcutKey ScoutKey { get => scoutKey; set => SetKey(ref scoutKey, value); }
    public ShortcutKey BuildKey { get => buildKey; set => SetKey(ref buildKey, value); }
    public ShortcutKey NegotiationKey { get => negotiationKey; set => SetKey(ref negotiationKey, value); }
    public ShortcutKey RightColKey { get => rightColKey; set => SetKey(ref rightColKey, value); }
    public ShortcutKey NextTurnKey { get => nextTurnKey; set => SetKey(ref nextTurnKey, value); }
    public ShortcutKey ElectorKey { get => electorKey; set => SetKey(ref electorKey, value); }
    public ShortcutKey CountryKey { get => countryKey; set => SetKey(ref countryKey, value); }
    public ShortcutKey RegionKey { get => regionKey; set => SetKey(ref regionKey, value); }
    public ShortcutKey CharacterKey { get => characterKey; set => SetKey(ref characterKey, value); }
    public ShortcutKey ItemKey { get => itemKey; set => SetKey(ref itemKey, value); }
    public ShortcutKey RecruitKey { get => recruitKey; set => SetKey(ref recruitKey, value); }

    public ShortcutKey ZoomOutKey { get => zoomOutKey; set => SetKey(ref zoomOutKey, value); }
    public ShortcutKey ArrowUpKey { get => arrowUpKey; set => SetKey(ref arrowUpKey, value); }
    public ShortcutKey ZoomInKey { get => zoomInKey; set => SetKey(ref zoomInKey, value); }
    public ShortcutKey ArrowRightKey { get => arrowRightKey; set => SetKey(ref arrowRightKey, value); }
    public ShortcutKey ArrowDownKey { get => arrowDownKey; set => SetKey(ref arrowDownKey, value); }
    public ShortcutKey ArrowLeftKey { get => arrowLeftKey; set => SetKey(ref arrowLeftKey, value); }


    private void SetKey(ref ShortcutKey field, ShortcutKey value)
    {
        if (field != value)
        {
            field = value;
            OnShortcutKeysChanged?.Invoke();
        }
    }

    // Public method to manually trigger the event (if needed)
    public void TriggerShortcutKeysChanged()
    {
        OnShortcutKeysChanged?.Invoke();
    }
}

/// <summary>
/// Example: Battle shortcut keys
/// </summary>
[Serializable]
public class BattleShortcutkeyData
{
    public ShortcutKey[,] battlePositionKeys = new ShortcutKey[3, 3]
    {
        { ShortcutKey.Q, ShortcutKey.W, ShortcutKey.E },
        { ShortcutKey.A, ShortcutKey.S, ShortcutKey.D },
        { ShortcutKey.Z, ShortcutKey.X, ShortcutKey.C }
    };

    public ShortcutKey nothingShortcutKey = ShortcutKey.Space;
    public List<ShortcutKey> SkillShortcutKeys = new() { ShortcutKey.Alpha1, ShortcutKey.Alpha2, ShortcutKey.Alpha3, ShortcutKey.Alpha4, ShortcutKey.Alpha5 };
    public ShortcutKey enemyShortcutKey = ShortcutKey.UpArrow;
    public ShortcutKey playerShortcutKey = ShortcutKey.DownArrow;
    public ShortcutKey EscapeShortcutKey = ShortcutKey.Alpha0;
}

/// <summary>
/// Example: Explore shortcut keys
/// </summary>
[Serializable]
public class ExploreShortcutkeyData
{
    // Add explore-specific keys here
}

[Serializable]
public class TextShortcutkeyData
{
    public ShortcutKey LockKey = ShortcutKey.Alpha1;
    public ShortcutKey LogKey = ShortcutKey.Alpha2;
    public ShortcutKey AutoKey = ShortcutKey.Alpha3;
    public ShortcutKey SkipKey = ShortcutKey.Alpha4;
    public ShortcutKey SkipToChoiceKey = ShortcutKey.Alpha5;
    public ShortcutKey SkipNodeKey = ShortcutKey.Alpha6;
}
