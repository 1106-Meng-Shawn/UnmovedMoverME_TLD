using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.Localization.Settings;
//using static GetColor;
//using static GetSprite;

public enum RelationshipType
{
    Ally,           // ??
    Friendly,       // ??
    Neutral,        // ??
    Hostile,        // ??
    War,            // ??
    Overlord,       // ???
    Vassal,         // ???
    Elector,        // ???
    Protector,      // ???
    Protected       // ????
}


public static class CountryConstants
{
    #region Country Names
    // 主要国家
    public const string BABYLONIA = "Babylonia";
    public const string CARTHAGE = "Carthage";
    public const string CYPRUS = "Cyprus";
    public const string DEMON_KINGDOM = "Demon Kingdom";
    public const string EGYPT = "Egypt";
    public const string KINGDOM_OF_ENGLALAND = "Kingdom of Englaland";
    public const string EAST_ROMULUS_EMPIRE = "East Romulus Empire";
    public const string FREE_FRANCUS = "Free Francus";
    public const string KINGDOM_OF_FRANCUS = "Kingdom of Francus";
    public const string HUNGARY = "Hungary";
    public const string KNIGHTS_TEMPLAR = "State of the Knights Templar";
    public const string KALMAR_UNION = "Kalmar Union";
    public const string MACEDONIA = "Macedonia";
    public const string OTTOMAN_EMPIRE = "Ottoman Empire";
    public const string PERSIAN_EMPIRE = "Persian Empire";
    public const string POLISH_LITHUANIAN = "Polish-Lithuanian Commonwealth";
    public const string RUS_PRINCIPALITY = "Rus Principality";
    public const string UNION_OF_SALVATION = "Union of Salvation";
    public const string SAMI = "Sami";
    public const string SKY_FIRE_EMPIRE = "Sky Fire Empire";
    public const string TEUTONIC_ORDER = "State of the Teutonic Order";
    public const string GREECE = "Greece";
    public const string DEMIHUMAN_ALLIANCE = "Demihuman Alliance";
    public const string AVALON = "Avalon";
    public const string GIANTS_ISLAND = "Giant's Island";
    public const string MILITARY_NAPLES = "Military State of Naples";
    public const string KINGDOM_OF_SICILY = "Kingdom of Sicily";
    public const string HOLY_ROMULUS_EMPIRE = "Holy Romulus Empire";
    public const string FAIRY_TALE_KINGDOM = "Fairy Tale Kingdom";

    // 神圣罗马帝国选帝侯
    public const string TRIER = "Trier";
    public const string COLOGNE = "Cologne";
    public const string BOHEMIA = "Bohemia";
    public const string BRANDENBURG = "Brandenburg";
    public const string SAXONY = "Saxony";
    public const string MAINZ = "Mainz";
    public const string PALATINATE = "Platinate";

    // 神圣罗马帝国附庸
    public const string PAPAL_STATE = "Papal State";
    public const string BOLOGNA = "Bologna";
    public const string LOMBARDY = "Lombardy";
    public const string BAVARIA = "Bavaria";
    public const string NETHERLANDS = "Netherlands";
    public const string AUSTRIA = "Austria";
    public const string VENICE = "Venice";
    public const string BURGUNDY = "Burgundy";

    // 地理区域
    public const string DRAKENSBERG = "Drakensberg Mountain Range";
    public const string SCYTHIA = "Scythia";
    public const string SAHARA = "Sahara";
    public const string ARAB = "Arab";
    public const string SIBERIA = "Siberia";

    // 海域
    public const string MEDITERRANEAN_SEA = "Mediterranean Sea";
    public const string BLACK_SEA = "Black Sea";
    public const string CASPIAN_SEA = "Caspian Sea";
    public const string FOG_OCEAN = "Fog Ocean";
    public const string FROZEN_OCEAN = "Frozen Ocean";
    public const string NORTH_SEA = "North Sea";
    public const string BALTIC_SEA = "Baltic Sea";
    public const string PERSIAN_GULF = "Persin Gulf";
    public const string RED_SEA = "Red Sea";
    public const string MONSTER_OCEAN = "Monster Ocean";
    #endregion

    #region Country Data Structure
    public class CountryData
    {
        public string Name { get; set; }
        public string IconPath { get; set; }
        public Color32 Color { get; set; }

        public CountryData(string name, string iconName, Color32 color)
        {
            Name = name;
            IconPath = $"MyDraw/UI/Country/Icon/{iconName}";
            Color = color;
        }
    }
    #endregion

    #region Country Data Dictionary
    private static readonly Dictionary<string, CountryData> countryDataMap = new Dictionary<string, CountryData>()
    {
        // 主要国家
        { BABYLONIA, new CountryData(BABYLONIA, "BabylonIcon", new Color32(71, 130, 180, 255)) },
        { CARTHAGE, new CountryData(CARTHAGE, "CarthageIcon", new Color32(185, 85, 210, 255)) },
        { CYPRUS, new CountryData(CYPRUS, "CyprusIcon", new Color32(239, 127, 26, 255)) },
        { DEMON_KINGDOM, new CountryData(DEMON_KINGDOM, "DemonKingdomIcon", new Color32(75, 0, 136, 255)) },
        { EGYPT, new CountryData(EGYPT, "EgyptIcon", new Color32(0, 181, 0, 255)) },
        { KINGDOM_OF_ENGLALAND, new CountryData(KINGDOM_OF_ENGLALAND, "Englaland(John)Icon", new Color32(200, 13, 42, 255)) },
        { EAST_ROMULUS_EMPIRE, new CountryData(EAST_ROMULUS_EMPIRE, "EREIcon", new Color32(64, 0, 103, 255)) },
        { FREE_FRANCUS, new CountryData(FREE_FRANCUS, "FreeIcon", new Color32(255, 255, 89, 255)) },
        { KINGDOM_OF_FRANCUS, new CountryData(KINGDOM_OF_FRANCUS, "FrancusIcon", new Color32(0, 49, 227, 255)) },
        { HUNGARY, new CountryData(HUNGARY, "Hungary", new Color32(215, 69, 22, 255)) },
        { KNIGHTS_TEMPLAR, new CountryData(KNIGHTS_TEMPLAR, "JerusalemIcon", new Color32(211, 211, 210, 255)) },
        { KALMAR_UNION, new CountryData(KALMAR_UNION, "KalmarIcon", new Color32(0, 34, 91, 255)) },
        { MACEDONIA, new CountryData(MACEDONIA, "MacedoniaIcon", new Color32(215, 34, 38, 255)) },
        { OTTOMAN_EMPIRE, new CountryData(OTTOMAN_EMPIRE, "OttomanIcon", new Color32(0, 150, 42, 255)) },
        { PERSIAN_EMPIRE, new CountryData(PERSIAN_EMPIRE, "PersiaIcon", new Color32(50, 22, 122, 255)) },
        { POLISH_LITHUANIAN, new CountryData(POLISH_LITHUANIAN, "PLIcon", new Color32(230, 0, 0, 255)) },
        { RUS_PRINCIPALITY, new CountryData(RUS_PRINCIPALITY, "RusIcon", new Color32(0, 91, 188, 255)) },
        { UNION_OF_SALVATION, new CountryData(UNION_OF_SALVATION, "SalvationIcon", new Color32(255, 244, 0, 255)) },
        { SAMI, new CountryData(SAMI, "SamiIcon", new Color32(0, 50, 174, 255)) },
        { SKY_FIRE_EMPIRE, new CountryData(SKY_FIRE_EMPIRE, "SkyFireIcon", new Color32(0, 92, 162, 255)) },
        { TEUTONIC_ORDER, new CountryData(TEUTONIC_ORDER, "TeutonicIcon", new Color32(15, 15, 15, 255)) },
        { GREECE, new CountryData(GREECE, "GreeceIcon", new Color32(5, 94, 176, 255)) },
        { DEMIHUMAN_ALLIANCE, new CountryData(DEMIHUMAN_ALLIANCE, "CelticIcon", new Color32(0, 94, 185, 255)) },
        { AVALON, new CountryData(AVALON, "AvalonIcon", new Color32(255, 223, 0, 255)) },
        { GIANTS_ISLAND, new CountryData(GIANTS_ISLAND, "IrelandIcon", new Color32(21, 150, 95, 255)) },
        { MILITARY_NAPLES, new CountryData(MILITARY_NAPLES, "NaplesIcon", new Color32(207, 7, 32, 255)) },
        { KINGDOM_OF_SICILY, new CountryData(KINGDOM_OF_SICILY, "SicilyIcon", new Color32(254, 229, 110, 255)) },
        { HOLY_ROMULUS_EMPIRE, new CountryData(HOLY_ROMULUS_EMPIRE, "HREIcon", new Color32(255, 236, 0, 255)) },
        { FAIRY_TALE_KINGDOM, new CountryData(FAIRY_TALE_KINGDOM, "FairyTaleKingdomIcon", new Color32(224, 33, 93, 255)) },

        // 神圣罗马帝国选帝侯
        { TRIER, new CountryData(TRIER, "TrierIcon", new Color32(255, 9, 9, 255)) },
        { COLOGNE, new CountryData(COLOGNE, "CologneIcon", new Color32(34, 34, 34, 255)) },
        { BOHEMIA, new CountryData(BOHEMIA, "BohemiaIcon", new Color32(204, 0, 0, 255)) },
        { BRANDENBURG, new CountryData(BRANDENBURG, "BrandenburgIcon", new Color32(0, 102, 204, 255)) },
        { SAXONY, new CountryData(SAXONY, "SaxonyIcon", new Color32(91, 170, 56, 255)) },
        { MAINZ, new CountryData(MAINZ, "MainzIcon", new Color32(225, 0, 0, 255)) },
        { PALATINATE, new CountryData(PALATINATE, "PlatinateIcon", new Color32(255, 195, 0, 255)) },

        // 神圣罗马帝国附庸
        { PAPAL_STATE, new CountryData(PAPAL_STATE, "PapalStatesIcon", new Color32(255, 255, 255, 255)) },
        { BOLOGNA, new CountryData(BOLOGNA, "BolognaIcon", new Color32(11, 31, 63, 255)) },
        { LOMBARDY, new CountryData(LOMBARDY, "LombardyIcon", new Color32(156, 0, 21, 255)) },
        { BAVARIA, new CountryData(BAVARIA, "BavariaIcon", new Color32(89, 127, 241, 255)) },
        { NETHERLANDS, new CountryData(NETHERLANDS, "NetherlandsIcon", new Color32(255, 219, 67, 255)) },
        { AUSTRIA, new CountryData(AUSTRIA, "AustriaIcon", new Color32(237, 41, 57, 255)) },
        { VENICE, new CountryData(VENICE, "VeniceIcon", new Color32(226, 197, 111, 255)) },
        { BURGUNDY, new CountryData(BURGUNDY, "BurgundyIcon", new Color32(254, 228, 110, 255)) },

        // 地理区域
        { DRAKENSBERG, new CountryData(DRAKENSBERG, "DrakensbergIcon", new Color32(139, 69, 19, 255)) },
        { SCYTHIA, new CountryData(SCYTHIA, "ScythiaIcon", new Color32(255, 146, 0, 255)) },
        { SAHARA, new CountryData(SAHARA, "SaharaIcon", new Color32(255, 223, 127, 255)) },
        { ARAB, new CountryData(ARAB, "ArabIcon", new Color32(0, 98, 50, 255)) },
        { SIBERIA, new CountryData(SIBERIA, "SiberiaIcon", new Color32(228, 228, 228, 255)) },

        // 海域
        { MEDITERRANEAN_SEA, new CountryData(MEDITERRANEAN_SEA, "MediterraneanSeaIcon", new Color32(150, 180, 240, 180)) },
        { BLACK_SEA, new CountryData(BLACK_SEA, "BlackSeaIcon", new Color32(50, 50, 50, 180)) },
        { CASPIAN_SEA, new CountryData(CASPIAN_SEA, "CaspianSeaIcon", new Color32(120, 160, 150, 180)) },
        { FOG_OCEAN, new CountryData(FOG_OCEAN, "FogOceanIcon", new Color32(170, 150, 210, 180)) },
        { FROZEN_OCEAN, new CountryData(FROZEN_OCEAN, "FrozenOceanIcon", new Color32(200, 220, 230, 180)) },
        { NORTH_SEA, new CountryData(NORTH_SEA, "NorthSeaIcon", new Color32(150, 190, 240, 180)) },
        { BALTIC_SEA, new CountryData(BALTIC_SEA, "BalticSeaIcon", new Color32(180, 210, 230, 180)) },
        { PERSIAN_GULF, new CountryData(PERSIAN_GULF, "PersianGulfIcon", new Color32(100, 190, 180, 180)) },
        { RED_SEA, new CountryData(RED_SEA, "RedSeaIcon", new Color32(240, 120, 120, 180)) },
        { MONSTER_OCEAN, new CountryData(MONSTER_OCEAN, "MonsterOceanIcon", new Color32(255, 180, 100, 180)) }
    };
    #endregion

    #region Public Methods
    /// <summary>
    /// 获取国家图标路径
    /// </summary>
    public static string GetCountryIconPath(string countryName)
    {
        if (countryDataMap.TryGetValue(countryName, out CountryData data))
        {
            return data.IconPath;
        }

        Debug.LogWarning($"Country '{countryName}' not found in CountryConstants");
        return $"MyDraw/UI/Country/Icon/{countryName}Icon";
    }

    /// <summary>
    /// 获取国家颜色
    /// </summary>
    public static Color32 GetCountryColor(string countryName)
    {
        if (countryDataMap.TryGetValue(countryName, out CountryData data))
        {
            return data.Color;
        }

        Debug.LogWarning($"Country '{countryName}' not found in CountryConstants");
        return new Color32(0, 0, 0, 255);
    }

    /// <summary>
    /// 获取所有国家名列表
    /// </summary>
    public static string[] GetAllCountryNames()
    {
        var names = new List<string>(countryDataMap.Keys);
        names.Add(CharacterConstants.Capture);
        names.Add(CharacterConstants.DIE);
        return names.ToArray();
    }

    /// <summary>
    /// 检查国家是否存在
    /// </summary>
    public static bool IsValidCountry(string countryName)
    {
        return countryDataMap.ContainsKey(countryName);
    }

    /// <summary>
    /// 获取国家完整数据
    /// </summary>
    public static CountryData GetCountryData(string countryName)
    {
        return countryDataMap.TryGetValue(countryName, out CountryData data) ? data : null;
    }
    #endregion
}

public class CountryRelationship
{
    public RelationshipType Type;
    public int Value; // 关系值 -100 ~ 100
    public bool Independent;

    public CountryRelationship(RelationshipType type, int value, bool independent = true)
    {
        Type = type;
        Value = value;
        Independent = independent;
    }
}

public class CountryManager
{
    private Dictionary<string, Country> countries = new Dictionary<string, Country>();
    private Dictionary<string, Dictionary<string, CountryRelationship>> relations
        = new Dictionary<string, Dictionary<string, CountryRelationship>>();

    public void Init()
    {
        InitializeCountries();
        InitializeRelations();
    }

    public void InitializeCountries()
    {
        // 使用 CountryConstants 获取所有国家名
        string[] countryNames = CountryConstants.GetAllCountryNames();

        foreach (var name in countryNames)
        {
            countries[name] = new Country(name);
        }
    }

    #region Init Relations

    public void InitializeRelations()
    {
        foreach (var countryA in countries.Values)
        {
            foreach (var countryB in countries.Values)
            {
                if (countryA == countryB) continue;
                SetRelation(countryA.GetCountryENName(), countryB.GetCountryENName(), RelationshipType.Neutral);
            }
        }

        InitRelationOtherRelation();
        InitHRERelation();
    }

    private void InitRelationOtherRelation()
    {
        SetRelation(CountryConstants.KINGDOM_OF_ENGLALAND, CountryConstants.KINGDOM_OF_FRANCUS, RelationshipType.War);
    }

    private void InitHRERelation()
    {
        // 选帝侯
        SetRelation(CountryConstants.TRIER, CountryConstants.HOLY_ROMULUS_EMPIRE, RelationshipType.Elector);
        SetRelation(CountryConstants.COLOGNE, CountryConstants.HOLY_ROMULUS_EMPIRE, RelationshipType.Elector);
        SetRelation(CountryConstants.BOHEMIA, CountryConstants.HOLY_ROMULUS_EMPIRE, RelationshipType.Elector);
        SetRelation(CountryConstants.BRANDENBURG, CountryConstants.HOLY_ROMULUS_EMPIRE, RelationshipType.Elector);
        SetRelation(CountryConstants.MAINZ, CountryConstants.HOLY_ROMULUS_EMPIRE, RelationshipType.Elector);
        SetRelation(CountryConstants.SAXONY, CountryConstants.HOLY_ROMULUS_EMPIRE, RelationshipType.Elector);
        SetRelation(CountryConstants.PALATINATE, CountryConstants.HOLY_ROMULUS_EMPIRE, RelationshipType.Elector);

        // 附庸
        SetRelation(CountryConstants.BOLOGNA, CountryConstants.HOLY_ROMULUS_EMPIRE, RelationshipType.Vassal);
        SetRelation(CountryConstants.LOMBARDY, CountryConstants.HOLY_ROMULUS_EMPIRE, RelationshipType.Vassal);
        SetRelation(CountryConstants.BAVARIA, CountryConstants.HOLY_ROMULUS_EMPIRE, RelationshipType.Vassal);
        SetRelation(CountryConstants.NETHERLANDS, CountryConstants.HOLY_ROMULUS_EMPIRE, RelationshipType.Vassal);
        SetRelation(CountryConstants.AUSTRIA, CountryConstants.HOLY_ROMULUS_EMPIRE, RelationshipType.Vassal);
        SetRelation(CountryConstants.VENICE, CountryConstants.HOLY_ROMULUS_EMPIRE, RelationshipType.Vassal);
        SetRelation(CountryConstants.BURGUNDY, CountryConstants.HOLY_ROMULUS_EMPIRE, RelationshipType.Vassal);
        SetRelation(CountryConstants.PAPAL_STATE, CountryConstants.HOLY_ROMULUS_EMPIRE, RelationshipType.Vassal);
    }

    #endregion

    private RelationshipType GetReverseRelation(RelationshipType type)
    {
        return type switch
        {
            RelationshipType.Overlord => RelationshipType.Vassal,
            RelationshipType.Vassal => RelationshipType.Overlord,
            RelationshipType.Elector => RelationshipType.Overlord,
            RelationshipType.Protector => RelationshipType.Protected,
            RelationshipType.Protected => RelationshipType.Protector,
            _ => type
        };
    }

    public void SetRelation(string countryA, string countryB, RelationshipType type, int value = 50)
    {
        if (!relations.ContainsKey(countryA))
            relations[countryA] = new Dictionary<string, CountryRelationship>();
        if (!relations.ContainsKey(countryB))
            relations[countryB] = new Dictionary<string, CountryRelationship>();

        relations[countryA][countryB] = new CountryRelationship(type, value);
        relations[countryB][countryA] = new CountryRelationship(GetReverseRelation(type), value);
    }

    public Country GetCountry(string name)
    {
        if (string.IsNullOrEmpty(name)) return null;
        return countries.ContainsKey(name) ? countries[name] : null;
    }


    public String GetCountryName(string ENName)
    {
        return GetCountry(ENName).GetCountryName();
    }


    public Color32 GetCountryColor(string name)
    {
        Country country = GetCountry(name);
        if (country != null)
        {
            return country.GetColor();
        }
        else
        {
            return new Color32(255, 255, 255, 255);
        }
    }

    public Sprite GetCountryIcon(string name)
    {
        Country country = GetCountry(name);
        if (country != null)
        {
            return country.GetIcon();
        }
        else
        {
            string iconPath = $"MyDraw/UI/Country/Icon/";
            return Resources.Load<Sprite>(iconPath + $"NoneIcon");
        }
    }

    public CountryRelationship GetRelation(string countryA, string countryB)
    {
        if (relations.ContainsKey(countryA) && relations[countryA].ContainsKey(countryB))
            return relations[countryA][countryB];
        return null;
    }

    public List<Character> GetAllCharacters()
    {
        List<Character> allCharacters = new List<Character>();
        foreach (var kvp in countries)
        {
            Country country = kvp.Value;
            if (country == null) continue;
            allCharacters.AddRange(country.GetCharacters());
        }
        return allCharacters;
    }

    public List<RegionValue> GetAllRegionValues()
    {
        List<RegionValue> allRegionValues = new List<RegionValue>();
        foreach (var kvp in countries)
        {
            Country country = kvp.Value;
            if (country == null) continue;
            allRegionValues.AddRange(country.GetRegionValues());
        }
        return allRegionValues;
    }

    public void DebugForCountryEachRegion()
    {
        foreach (var country in countries.Values)
        {
            foreach (var region in country.GetRegionValues())
            {
                Debug.Log($"{country} has {region.GetRegionENName()}");
            }
        }
    }

    public bool IsVassal(string countryName)
    {
        if (!relations.ContainsKey(countryName)) return false;
        foreach (var rel in relations[countryName])
        {
            if (rel.Value.Type == RelationshipType.Vassal)
                return true;
        }
        return false;
    }

    public bool IsElector(string countryName)
    {
        if (!relations.ContainsKey(countryName)) return false;
        foreach (var rel in relations[countryName])
        {
            if (rel.Value.Type == RelationshipType.Elector)
                return true;
        }
        return false;
    }

    public bool IsProtected(string countryName)
    {
        if (!relations.ContainsKey(countryName)) return false;
        foreach (var rel in relations[countryName])
        {
            if (rel.Value.Type == RelationshipType.Protected)
                return true;
        }
        return false;
    }

    public bool IsIndependent(string checkCountry, string targetCountry)
    {
        if (checkCountry == targetCountry)
            return false;

        if (!relations.ContainsKey(checkCountry))
            return false;

        if (!relations[checkCountry].ContainsKey(targetCountry))
            return false;

        var rel = relations[checkCountry][targetCountry];
        return rel.Independent;
    }

    public bool IsAtWar(string countryA, string countryB)
    {
        if (!relations.ContainsKey(countryA)) return false;
        if (!relations[countryA].ContainsKey(countryB)) return false;

        var rel = relations[countryA][countryB];
        return rel.Type == RelationshipType.War || rel.Type == RelationshipType.Hostile;
    }

    public bool HasOverlord(string countryName)
    {
        if (string.IsNullOrEmpty(countryName)) return false;
        if (!relations.ContainsKey(countryName)) return false;

        foreach (var rel in relations[countryName])
        {
            if (rel.Value.Type == RelationshipType.Vassal ||
                rel.Value.Type == RelationshipType.Elector ||
                rel.Value.Type == RelationshipType.Protected)
            {
                return true;
            }
        }
        return false;
    }

    public string GetOverlord(string countryName)
    {
        if (!relations.ContainsKey(countryName)) return null;

        foreach (var rel in relations[countryName])
        {
            if (rel.Value.Type == RelationshipType.Vassal ||
                rel.Value.Type == RelationshipType.Elector ||
                rel.Value.Type == RelationshipType.Protected)
            {
                return rel.Key;
            }
        }
        return null;
    }

    public void AddRegion(string countryName, RegionValue regionValue)
    {
        Country newCountry = GetCountry(countryName);
        Country CurrentCountry = GetCountry(regionValue.GetCountryENName());
        if (CurrentCountry != null) CurrentCountry.RemoveRegion(regionValue);
        if (newCountry != null)
        {
            newCountry.AddRegion(regionValue);
        }
    }

    public void AddCharacter(string countryName, Character character)
    {
        Country newCountry = GetCountry(countryName);
        Country CurrentCountry = GetCountry(character.GetCountryENName());
        if (CurrentCountry != null) CurrentCountry.RemoveCharacter(character);
        if (newCountry == null)
        {
            character.SetCountry(countryName);
            return;
        }
        else
        {
            newCountry.AddCharacter(character);
        }
    }

    #region Save/Load
    public CountryManagerSaveData GetSaveData()
    {
        CountryManagerSaveData saveData = new CountryManagerSaveData();

        foreach (var country in countries.Values)
        {
            CountrySaveData countrySaveData = new CountrySaveData(country);
            saveData.countries.Add(countrySaveData);
        }

        HashSet<string> addedPairs = new HashSet<string>();
        foreach (var countryA in relations.Keys)
        {
            foreach (var countryB in relations[countryA].Keys)
            {
                string pairKey = countryA + "_" + countryB;
                string reverseKey = countryB + "_" + countryA;

                if (!addedPairs.Contains(pairKey) && !addedPairs.Contains(reverseKey))
                {
                    var rel = relations[countryA][countryB];
                    saveData.relations.Add(new CountryRelationSaveData(countryA, countryB, rel.Type, rel.Value, rel.Independent));
                    addedPairs.Add(pairKey);
                }
            }
        }

        return saveData;
    }

    public void SetSaveData(CountryManagerSaveData saveData)
    {
        countries.Clear();
        relations.Clear();

        foreach (var countrySave in saveData.countries)
        {
            Country country = new Country(countrySave);
            countries.Add(country.GetCountryENName(), country);
        }

        foreach (var relSave in saveData.relations)
        {
            SetRelation(relSave.countryA, relSave.countryB, relSave.type, relSave.value);
            relations[relSave.countryA][relSave.countryB].Independent = relSave.independent;
        }
    }
    #endregion
}

#region Supporting Classes
[System.Serializable]
public class CountryManagerSaveData
{
    public List<CountrySaveData> countries = new List<CountrySaveData>();
    public List<CountryRelationSaveData> relations = new List<CountryRelationSaveData>();

    public Sprite GetSaveCountrySprite(string country)
    {
        var foundCountry = countries.FirstOrDefault(c => c.saveName == country);
        if (foundCountry != null)
        {
            return Resources.Load<Sprite>(foundCountry.saveCountryIconPath);
        }
        return null;
    }

    public string GetSaveCountryStringWithColor(string country)
    {
        var foundCountry = countries.FirstOrDefault(c => c.saveName == country);
        if (foundCountry != null)
        {
            Color32 color = new Color32(
                foundCountry.countryColorR,
                foundCountry.countryColorG,
                foundCountry.countryColorB,
                foundCountry.countryColorA
            );

            string hexColor = ColorUtility.ToHtmlStringRGBA(color);
            return $"<color=#{hexColor}>{LocalizationSettings.StringDatabase.GetLocalizedString("GameCountryTable", foundCountry.saveName)}</color>";
        }
        return null;
    }
}

[System.Serializable]
public class CountryRelationSaveData
{
    public string countryA;
    public string countryB;
    public RelationshipType type;
    public int value;
    public bool independent;

    public CountryRelationSaveData(string a, string b, RelationshipType t, int v, bool independent)
    {
        countryA = a;
        countryB = b;
        type = t;
        value = v;
        this.independent = independent;
    }
}

public class Country
{
    string Name;
    Sprite countryIcon;
    string countryIconPath;
    Color32 countryColor;
    private List<Character> characters = new List<Character>();
    private List<RegionValue> regions = new List<RegionValue>();
    public Action OnRegionsChange;
    public Action OnCharactersChange;

    public Country(string name)
    {
        Name = name;
        countryIconPath = CountryConstants.GetCountryIconPath(name);
        countryIcon = Resources.Load<Sprite>(countryIconPath);
        countryColor = CountryConstants.GetCountryColor(name);
    }

    public Country(CountrySaveData saveData)
    {
        Name = saveData.saveName;
        countryIconPath = saveData.saveCountryIconPath;
        countryIcon = Resources.Load<Sprite>(countryIconPath);
        countryColor = new Color32(
            saveData.countryColorR,
            saveData.countryColorG,
            saveData.countryColorB,
            saveData.countryColorA
        );
    }

    public Sprite GetIcon()
    {
        return countryIcon;
    }

    public void SetIconPath(string newName)
    {
        countryIconPath = CountryConstants.GetCountryIconPath(newName);
        countryIcon = Resources.Load<Sprite>(countryIconPath);
    }

    public void RemoveRegion(RegionValue removeRegion)
    {
        regions.Remove(removeRegion);
        OnRegionsChange?.Invoke();
    }

    public List<RegionValue> GetRegionValues()
    {
        return regions;
    }

    public void AddRegion(RegionValue newRegion)
    {
        regions.Add(newRegion);
        OnRegionsChange?.Invoke();
    }

    public List<Character> GetCharacters()
    {
        return characters;
    }

    public void RemoveCharacter(Character removeCharacter)
    {
        characters.Remove(removeCharacter);
        OnCharactersChange?.Invoke();
    }

    public void AddCharacter(Character newCharacter)
    {
        characters.Add(newCharacter);
        OnCharactersChange?.Invoke();
    }

    public string GetIconPath()
    {
        return countryIconPath;
    }

    public Color32 GetColor()
    {
        return countryColor;
    }

    public string GetCountryENName()
    {
        return Name;
    }

    public string GetCountryName()
    {
        return LocalizationSettings.StringDatabase.GetLocalizedString("GameCountryTable", Name);
    }
}

[System.Serializable]
public class CountrySaveData
{
    public string saveName;
    public string saveCountryIconPath;
    public byte countryColorR;
    public byte countryColorG;
    public byte countryColorB;
    public byte countryColorA;

    public CountrySaveData(Country country)
    {
        if (country == null) return;
        saveName = country.GetCountryENName();
        saveCountryIconPath = country.GetIconPath();

        Color32 c = country.GetColor();
        countryColorR = c.r;
        countryColorG = c.g;
        countryColorB = c.b;
        countryColorA = c.a;
    }

    public Color32 ToColor32()
    {
        return new Color32(countryColorR, countryColorG, countryColorB, countryColorA);
    }

    public Color ToColor()
    {
        return new Color32(countryColorR, countryColorG, countryColorB, countryColorA);
    }
}
#endregion

