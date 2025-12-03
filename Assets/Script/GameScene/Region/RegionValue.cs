using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using static ExcelReader;
using static FormatNumber;
using static GetSprite;
using static RegionValue;
using static GetColor;


#region Value
public class RegionValueConstants
{
        public const int BrandenburgID = 1;
        public const int SaxonyID = 2;
        public const int CologneID = 3;
        public const int TrierID = 4;
        public const int BohemiaID = 5;
        public const int MainzID = 6;
        public const int PlatinateID = 7;

}
#endregion


public class RegionValue 
{
    #region Variables

    private int regionID;

    private Dictionary<string, string> regionName = new(); 

    public string relationship;
    private float taxRate;   
    public float GetTaxRate()
    {
        return taxRate;
    }
    public Character lord;

    private List<CityValue> cityValues = new List<CityValue>();
    

    public GameValue gameValue;
    public bool IsReduced;

    private bool isStar = false;
    public bool IsStar
    {
        get => isStar;
        set
        {
            if (isStar != value)
            {
                isStar = value;
                OnValueChanged?.Invoke(); // ????
            }
        }
    }

    private List<int> riverCounts = new List<int>();
    private float elevationMIN; public float GetElevationMIN() { return elevationMIN; }
    private float elevationMAX; public float GetElevationMAX() { return elevationMAX; }
    private float temperatureMIN; public float GetTemperatureMIN() { return temperatureMIN; }
    private float temperatureMAX; public float GetTemperatureMAX() { return temperatureMAX; }
    private float humidityMIN; public float GetHumidityMIN() { return humidityMIN; }
    private float humidityMAX; public float GetHumidityMAX() { return humidityMAX; }
    private bool hasSea; public bool GetHasSea() { return hasSea; }


    public Region region;

    public event Action OnValueChanged;


    #endregion


    public void NotifyChanged()
    {
        OnValueChanged?.Invoke();
    }


    public RegionValue(ExcelRegionData excelRegionData)
    {
        regionID = excelRegionData.ID;
        this.gameValue = GameValue.Instance;
        regionName = excelRegionData.regionName;
        relationship = excelRegionData.initRelation;
        if (excelRegionData.initLordKey != CharacterConstants.NoneKey)
        {
            SetInitLord(GameValue.Instance.GetCharacterByKey(excelRegionData.initLordKey));
            Character Load = GetLord();
        }
        for (int i = 0; i < excelRegionData.cityNum; i++) {

            int index = i;
            CityValue cityValue = new CityValue(excelRegionData.cityValueDatas[index],this, index);
            cityValues.Add(cityValue);
            cityValue.OnCityValueChanged += NotifyChanged;
        }
        SetCountry(GetCityCountry(0));
        riverCounts = excelRegionData.riverCount;
        string[] terrain = { "Sea", "Swamp", "Desert", "Plains", "Grassland", "Forest", "Hills", "Mountain", "SnowLand" };
        hasSea = excelRegionData.hasSea;
        elevationMIN = excelRegionData.elevationMIN;
        elevationMAX = excelRegionData.elevationMAX;
        temperatureMIN = excelRegionData.temperatureMIN;
        temperatureMAX = excelRegionData.temperatureMAX;
        humidityMIN = excelRegionData.humidityMIN;
        humidityMAX = excelRegionData.humidityMAX;

    }

    public void SetSaveData(RegionValueSaveData data)
    {
        relationship = data.relationship;
        taxRate = data.taxRate;

        SetCitySaveData(data.cityValues);
     //   SetCountry(GetCountryENName()); // dont fucking add the set country, even i dont know if add this, will give bugs, fuck bitch shit, go to hell, i hate c #, fuck!!!!!!!!!!

        if (data.lordKey != CharacterConstants.NoneKey)
        {
            Character foundLord = GameValue.Instance.GetCharacterByKey(data.lordKey);

            if (foundLord != null)
            {
                SetLord(foundLord);
                Debug.Log($"Region {GetRegionENName()} set lord {foundLord}");
            } else
            {
                Debug.Log($"Region {GetRegionENName()} set lord foundLord == null");
            }
        }
        else
        {
            SetLord(null);
        }
        var lord = GetLord();
        IsStar = data.isStar;
        OnValueChanged?.Invoke();
        region = CityConnetManage.Instance.GetRegion(regionID);
        region.SetRegionColor();
        
    }



    #region BuildPanel

    public int GetSeed(int index)
    {
        return cityValues[index].GetSeed();
    }
    public int GetMinRiverCount()
    {
        return riverCounts[0];
    }
    public int GetMaxRiverCount()
    {
        return riverCounts[1];
    }




    public Dictionary<Vector2Int, HexValue> GetHexValue(int index)
    {
        return cityValues[index].GetBuildValues();
    }
    public void SetHexValue(int index,Dictionary<Vector2Int, HexValue> newHexValue, int seed)
    {
        cityValues[index].SetBuildValues(newHexValue, seed);
    }
    #endregion

    #region Population

    public float GetPopulationGrowth()
    {
        // return resourceSurplus[0] > 0 ? Mathf.Min(0.5f, resourceSurplus[0] / population) : resourceSurplus[0] / population;
        return 0;
    }
    public String GetPopulationGrowthString()
    {
        return FormatfloatNumber(GetPopulationGrowth() * 100) + "%";
    }

    public int GetRegionMaxPopulation()
    {
        int maxPopulation = 0;
        for (int i = 0; i < cityValues.Count; i++)
        {
            maxPopulation += cityValues[i].maxPopulation;
        }

        return maxPopulation; // add some building 
    }

    public int GetRegionRecruitedPopulation()
    {
        int recruitedPopulation = 0;
        for (int i = 0; i < cityValues.Count; i++)
        {
            recruitedPopulation += cityValues[i].recruitedPopulation;
        }

        return recruitedPopulation; 
    }

    public int GetNextTurnPopulation()
    {
        return (int)(GetRegionPopulation() * (1 + GetPopulationGrowth()));
    }

    public int GetRegionAvailablePopulation()
    {
        int result = 0;
        for (int i = 0; i < cityValues.Count; i++)
        {
            result += (int)(cityValues[i].population * cityValues[i].supportRate * taxRate);
        }
        return result;
    }
    // Update the population based on food

    #endregion


    #region Get and Set

    public int GetRegionPopulation() {
        int totalRegionPop = 0;
        for (int i = 0; i < cityValues.Count; i++)
        {
            totalRegionPop += cityValues[i].population;
        }
        return totalRegionPop; 
    }
    public string GetRegionName()
    {
        string currentLanguage = LocalizationSettings.SelectedLocale.Identifier.Code;
        return regionName.TryGetValue(currentLanguage, out var text) ? text : regionName[LanguageCode.EN];
    }

    public string GetRegionNameWithColor()
    {
        CountryManager countryManager = GameValue.Instance.GetCountryManager();
    
        Color32 regionColor = countryManager.GetCountryColor(GetCityCountry(0));
        string currentLanguage = LocalizationSettings.SelectedLocale.Identifier.Code;
        string text = regionName.TryGetValue(currentLanguage, out var value) ? value : regionName[LanguageCode.EN];

        string hexColor = $"#{regionColor.r:X2}{regionColor.g:X2}{regionColor.b:X2}";
        return $"<color={hexColor}><b>{text.ToUpper()}</b></color>";
    }
    public string GetRegionENName()
    {
        return regionName[LanguageCode.EN];
    }

    public int GetRegionID()
    {
        return regionID;
    }

    public string GetCityName(int index)
    {
        if (index > cityValues.Count) return "bug out cityValue Count";
        return cityValues[index].GetCityName();
    }


    public List<string> GetCityNames()
    {
        List<string> names = new List<string>();
        foreach (var city in cityValues)
        {
            names.Add(city.GetCityENName());
        }
        return names;
    }

    public string GetCountryENName()
    {
        CountryManager countryManager = GameValue.Instance.GetCountryManager();
        if (countryManager == null)
        {
            Debug.LogError("CountryManager is null!");
            return null;
        }

        string cityCountry = GetCityCountry(0);
        if (string.IsNullOrEmpty(cityCountry))
        {
            return null;
        }

        if (countryManager.HasOverlord(cityCountry))
        {
            string overLordName = countryManager.GetOverlord(cityCountry);
            if (!string.IsNullOrEmpty(overLordName))
                return overLordName;
            else
                Debug.LogWarning($"No overlord found for {cityCountry}");
        }

        Country country = countryManager.GetCountry(cityCountry);
        if (country == null)
        {
            Debug.LogError($"Country {cityCountry} not found in CountryManager!");
            return null;
        }

        return country.GetCountryENName();
    }


    public String GetCountryName()
    {
        CountryManager countryManager = GameValue.Instance.GetCountryManager();
        if (countryManager.HasOverlord(GetCityCountry(0)))
        {
            string overLordName = countryManager.GetOverlord(GetCityCountry(0));
            return countryManager.GetCountry(overLordName).GetCountryName();
        }
        else
        {
            return countryManager.GetCountry(GetCityCountry(0)).GetCountryName();
        }
    }

    public String GetCountryNameWithColor()
    {
        CountryManager countryManager = GameValue.Instance.GetCountryManager();
        if (countryManager.HasOverlord(GetCityCountry(0)))
        {
            string overLordName = countryManager.GetOverlord(GetCityCountry(0));
            string OverLordNameWithColor = GetCountryColorString(countryManager.GetCountry(overLordName).GetCountryName(), countryManager.GetOverlord(GetCityCountry(0)));
            return OverLordNameWithColor;
        }
        else
        {
            string OverLordNameWithColor = GetColorString(countryManager.GetCountry(GetCityCountry(0)).GetCountryName(), GetCityCountryColor(0));
            return OverLordNameWithColor;
        }
    }

    public void SetCountry(string newCountryName)
    {
        GameValue.Instance.GetCountryManager().AddRegion(newCountryName, this);
        foreach (var city in cityValues)
        {
            if (city.cityCountry == GetCountryENName())
            {
                city.cityCountry = newCountryName;   
            }
        }
    }


    public void SetCountryColor()
    {
        region.SetRegionColor();
    }

    public Color32 GetCityCountryColor(int index)
    {
        if (cityValues.Count <= index)
        {
            Debug.LogWarning("Bug: cityValues.Count <= index");
            return new Color32(255, 0, 255, 255);
        }

        return cityValues[index].GetCityCountryColor();
    }


    public string GetCityCountry(int index)
    {
        if (cityValues.Count <= index) return null;
        return cityValues[index].GetCityCountry();
    }

    public string GetCityCountryWithColor(int index)
    {
        if (cityValues.Count <= index) return null;
        return cityValues[index].GetCityCountryNameWithColor();
    }

    public string GetCityENName(int index)
    {
        return cityValues[index].GetCityENName();
    }

    public List<CityValue> GetCityValues()
    {
        return cityValues;
    }




    public CityValue GetCityValue(int index)
    {
        if (index < 0 || index >= cityValues.Count)
            return null;

        return cityValues[index];
    }


    public int GetCityCountryNum()
    {
        return cityValues.Count;
    }

    public string GetCityCountryString(int index)
    {
        return LocalizationSettings.StringDatabase.GetLocalizedString("GameCountryTable", cityValues[index].GetCityCountry());
     //   return cityCountry[index];
    }

    public string GetCityCountryENName(int index)
    {
        return cityValues[index].GetCityCountry();
    }

    public float GetRegionSupportRate()
    {
        int totalRegionPop = 0;
        int supportRegionPop = 0;
        for (int i = 0; i < cityValues.Count; i++)
        {
            totalRegionPop += cityValues[i].population;
            supportRegionPop += (int)(cityValues[i].supportRate * cityValues[i].population);
        }

        if (totalRegionPop == 0) return 0f;
        return (float)supportRegionPop / totalRegionPop;
    }

    public String GetRegionSupportRateString()
    {
        return FormatNumberToString(GetRegionSupportRate() * 100) + "%";
    }

    public bool GetIsPlayerBuild(int cityIndex)
    {
        return GetCityValue(cityIndex).isPlayerBuild;
    }
    public void SetIsPlayerBuild(bool isBuild,int cityIndex)
    {
        GetCityValue(cityIndex).isPlayerBuild = isBuild;
    }


    #endregion



    private float CalculateTax(float baseValue, float growth, int index, float parameter)
    {
        if ( index != 4) return Mathf.Max(0, baseValue * parameter * (1 + growth) * taxRate);
        return Mathf.Max(0, baseValue * parameter * (1 + growth) * 1);

    }

    // A method that calculates the base value for resources
    private float CalculateBaseValue(int population, int recruitedPopulation, float parameter, float growth)
    {
        return Mathf.Max(0, (population - recruitedPopulation) * parameter * (1 + growth) * (1 - taxRate));
    }


      public String GetTaxRateString()
    {
        return FormatfloatNumber(taxRate * 100) + "%";
    }
    public bool HasLord()
    {
            return (lord != null);
    }

    public Character GetLord()
    {
            return lord;
    }

    public Sprite GetLordIcon() {
        if (GetLord() == null)
        {
            return Resources.Load<Sprite>($"MyDraw/UI/Region/Value/LordHint");
        }
        else
        {
            return GetLord().icon;
        }
    }

    public float GetRegionResourceLastTax(int resType)
    {
        float lastResourceTax = 0;
        for (int i = 0; i < cityValues.Count; i++)
        {
            lastResourceTax += cityValues[i].GetResourceLastTax(resType);
        }
        return lastResourceTax;
    }


    public float GetRegionResourceMax(int resType)
    {
        float resourceMax = 0;
        for (int i = 0; i < cityValues.Count; i++) {
            resourceMax += cityValues[i].GetResourceMax(resType);
        }
        return resourceMax;
    }

    public float GetRegionResourceNext(int resType)
    {
        float nextResource = 0;
        for (int i = 0; i < cityValues.Count; i++)
        {
            nextResource += cityValues[i].GetResourceNext(resType);
        }
        return nextResource;

    }


    public float GetRegionResoureNextTax(int resType)
    {
        float nextResourceTax = 0;
        for (int i = 0; i < cityValues.Count; i++)
        {
            nextResourceTax += cityValues[i].GetResoureNextTax(resType);
        }
        return nextResourceTax;

    }


    public void SetLord(Character character)
    {
        if(character == null) return;
        if (character.IsMoved) return;

        if (lord != null) lord.RemoveLord();
        if (character.HasLord()) {
            character.RemoveLord();
        }
        
        this.lord = character;
        this.lord.SetLordRegion(this);
        OnValueChanged?.Invoke();
       
    }

    public void SetInitLord(Character character)
    {
        if (character == null) return;
        this.lord = character;
        character.AddLordRegion(this);
    }


    public void MoveLord(){
        if (lord == null) return;
        lord.RemoveLord();
        lord = null;
        OnValueChanged?.Invoke();
    }

    #region Icon

    public Sprite GetCountryIcon()
    {
        CountryManager countryManager = GameValue.Instance.GetCountryManager();
        if (countryManager.HasOverlord(GetCityCountry(0)))
        {
            string overLordName = countryManager.GetOverlord(GetCityCountry(0));
            return countryManager.GetCountryIcon(overLordName);
        }
        else
        {
            return countryManager.GetCountryIcon(GetCityCountry(0));
        }
    }


    public Sprite GetRegionIcon()
    {
        CountryManager countryManager = GameValue.Instance.GetCountryManager();
        return countryManager.GetCountryIcon(GetCityCountry(0));

    }


    public Sprite GetCityIcon(int cityIndex)
    {
        return cityValues[cityIndex].GetCityCountryIcon();
    }

    #endregion

    public void SwitchLord (RegionValue region)
    {
        if (lord == null) {
            Character lord = region.GetLord();
            region.MoveLord();
            SetLord(lord);
            return;
        }

        Character temp = lord;
        SetLord(region.lord);
        region.SetLord(temp);
        
    }


    public float GetRegionResourceSurplus(int resourceType)
    {
        float resourceBaseParameters = 0;
        for (int i = 0; i < cityValues.Count; i++)
        {
            resourceBaseParameters = cityValues[i].resourceSurplus[resourceType];
        }
        return resourceBaseParameters ;
    }

    public float GetRegionResourceNextTax(int resourceType)
    {
        float resourceNextTax = 0;
        for (int i = 0; i < cityValues.Count; i++)
        {
            resourceNextTax = cityValues[i].GetResoureNextTax(resourceType);
        }
        return resourceNextTax;

    }



    public float GetRegionResourceParameter(ValueType valueType)
    {
        int index = 0;
        switch (valueType)
        {
            case ValueType.Food: index = 0; break;
            case ValueType.Science: index = 1; break;
            case ValueType.Politics: index = 2; break;
            case ValueType.Gold: index = 3; break;
            case ValueType.Faith: index = 4; break;

        }
        float resourceBaseParameters = 0;
        for (int i = 0; i < cityValues.Count; i++)
        {
            resourceBaseParameters = cityValues[i].resourceParameters[index];
        }
        return resourceBaseParameters / cityValues.Count;
    }


    public float GetRegionResourceGrowth(int index)
    {
        float regionResourceGrowth = 0;

        for (int i = 0; i < cityValues.Count; i++)
        {
            regionResourceGrowth = cityValues[i].GetResourceGrowth(index);
        }

        return regionResourceGrowth;
    }




    public string GetRegionResourceGrowthString(int index)
    {
        Debug.Log("Remember to add the calculation for the next turn");
        return "%";

    }




    void SetCitySaveData(List<CityValueSaveData> cityValueSaveDatas)
    {
        cityValues = new List<CityValue>();

        foreach (var cityData in cityValueSaveDatas)
        {
            cityValues.Add(new CityValue(cityData,this));

        }
    }

    public void OnDestroy()
    {
        region = null;
        RemoveAllListeners();
    }

    void RemoveAllListeners()
    {
        OnValueChanged = null;
        foreach (var cityValue in cityValues)
        {
            cityValue.RemoveAllListeners();
        }
    }
}

public class RegionValueSaveData
{
    //public string regionName;
    public int regionID;
    public string relationship;
    public string lordKey;
    public int population;
    public int recruitedPopulation;

    public float supportRate;
    public float taxRate;


    public int exploreLevel;
    public bool isStar;

    public List<CityValueSaveData> cityValues = new List<CityValueSaveData>();

    public RegionValueSaveData(RegionValue regionValue)
    {
        if (regionValue == null)
        {
            return;
        }


        regionID = regionValue.GetRegionID();
        relationship = regionValue.relationship;
      //  lordName = regionValue.lord != null ? regionValue.lord.GetCharacterENName() : "";
        lordKey = regionValue.lord == null ?  CharacterConstants.NoneKey : regionValue.lord.GetCharacterKey();
        taxRate = regionValue.GetTaxRate();
        cityValues = GetCityValues(regionValue.GetCityValues());
        isStar = regionValue.IsStar;
    }

    List<CityValueSaveData> GetCityValues(List<CityValue> cityValues)
    {
        List <CityValueSaveData> cityValueSaveDatas = new List <CityValueSaveData>();
        foreach (var cityValue in cityValues)
        {
            CityValueSaveData cityValueSaveData = new CityValueSaveData(cityValue);
            cityValueSaveDatas.Add(cityValueSaveData);
        }
        return cityValueSaveDatas;
    }

}
public class CityValue
{
    // === ?? ===
    public event Action OnCityValueChanged;
    public event Action OnTaskDatasChanged;


    private void NotifyValueChanged() => OnCityValueChanged?.Invoke();
    private void NotifynTaskDatasChanged() { 
        OnTaskDatasChanged?.Invoke();
        // Debug.Log("NotifynTaskDatasChanged");
        }

    // === ???? ===
    public int cityIndex;

    public string cityName;

    private string _cityCountry;
    public string cityCountry
    {
        get => _cityCountry;
        set
        {
            if (_cityCountry != value)
            {
                _cityCountry = value;
                NotifyValueChanged();
            }
        }
    }

    private int _exploreLevel;
    public int exploreLevel
    {
        get => _exploreLevel;
        set
        {
            if (_exploreLevel != value)
            {
                _exploreLevel = value;
                NotifyValueChanged();
            }
        }
    }

    public bool isPlayerBuild;

    private int _maxPopulation;
    public int maxPopulation
    {
        get => _maxPopulation;
        set
        {
            if (_maxPopulation != value)
            {
                _maxPopulation = value;
                NotifyValueChanged();
            }
        }
    }

    private int _population;
    public int population
    {
        get => _population;
        set
        {
            if (_population != value)
            {
                _population = value;
                NotifyValueChanged();
            }
        }
    }

    private int _recruitedPopulation;
    public int recruitedPopulation
    {
        get => _recruitedPopulation;
        set
        {
            if (_recruitedPopulation != value)
            {
                _recruitedPopulation = value;
                NotifyValueChanged();
            }
        }
    }

    public List<float> maxResourceSurplus = new();
    public List<float> resourceSurplus = new();
    public List<float> resourceMax = new();
    public List<float> resourceParameters = new() { 0, 0, 0, 0, 0 };
    public List<float> resourceTaxLastTurn = new() { 0, 0, 0, 0, 0 };

    private float _supportRate;
    public float supportRate
    {
        get => _supportRate;
        set
        {
            if (_supportRate != value)
            {
                _supportRate = value;
                NotifyValueChanged();
            }
        }
    }

    public List<int> ExploreItems = new();
    public List<int> ExploreMonster = new();

    public RegionValue regionValue;

    private Dictionary<Vector2Int, HexValue> buildValues = new();
    private int seed;

    public int GetSeed() => seed;

    public Dictionary<Vector2Int, HexValue> GetBuildValues() => buildValues;

    private List<TaskData> taskDatas = new List<TaskData>();
    public void SetBuildValues(Dictionary<Vector2Int, HexValue> newHexValue, int newSeed)
    {
        buildValues = newHexValue;
        seed = newSeed;
        NotifyValueChanged();
    }

    // === ???? ===
    public CityValue(ExcelCityValueData excelCityValue, RegionValue regionValue, int cityIndex)
    {
        this.cityName = excelCityValue.cityName;
        this.cityCountry = excelCityValue.cityCountry;
        exploreLevel = 0;
        isPlayerBuild = false;

        maxPopulation = excelCityValue.initMaxPop;
        population = excelCityValue.initPop;
        recruitedPopulation = excelCityValue.initRecuitedPop;

        resourceSurplus = new List<float>(excelCityValue.initResourceSurplus);
        resourceMax = new List<float>(excelCityValue.initResourceMax);
        resourceParameters = new List<float>(excelCityValue.baseParameter);
        supportRate = excelCityValue.InitSupporRate;

        ExploreItems = new List<int>(excelCityValue.ExploreItemsID);
        ExploreMonster = new List<int>(excelCityValue.ExploreMonsterID);

        this.regionValue = regionValue;
        this.cityIndex = cityIndex;
    }

    public CityValue(CityValueSaveData cityValue,RegionValue regionValue)
    {
        this.cityName = cityValue.cityName;
        cityCountry = cityValue.cityCountry;
        population = cityValue.population;
        recruitedPopulation = cityValue.recruitedPopulation;
        resourceSurplus = new List<float>(cityValue.resourceSurplus);
        resourceMax = new List<float>(cityValue.resourceMax);
        supportRate = cityValue.supportRate;
        exploreLevel = cityValue.exploreLevel;
        isPlayerBuild = cityValue.isPlayerBuild;
        buildValues = new Dictionary<Vector2Int, HexValue>(cityValue.buildValues);
        seed = cityValue.seed;
        this.regionValue = regionValue;
        NotifyValueChanged();
    }

    // === ???? ===

    public bool IsPlayerCity() => cityCountry == GameValue.Instance.GetPlayerCountryENName();

    public string GetCityName() => LocalizationSettings.StringDatabase.GetLocalizedString("GameCityTable", cityName);

    public string GetCityENName() => cityName;

    public string GetCityNameWithColor()
    {
        Color32 cityColor = GameValue.Instance.GetCountryColor(cityCountry);
        string currentLanguage = LocalizationSettings.SelectedLocale.Identifier.Code;
        string text = LocalizationSettings.StringDatabase.GetLocalizedString("GameCityTable", cityName);
        string hexColor = $"#{cityColor.r:X2}{cityColor.g:X2}{cityColor.b:X2}";
        return $"<color={hexColor}>{text}</color>";
    }


    public string GetCityCountry() => cityCountry;
    public string GetCityCountryNameWithColor() {
        string countryString = LocalizationSettings.StringDatabase.GetLocalizedString("GameCountryTable", cityCountry); ;
        Color32 cityCountryColor = GetCityCountryColor();
        return GetColorString(countryString, cityCountryColor);
    }

    public Sprite GetCityCountryIcon()
    {
        CountryManager countryManager = GameValue.Instance.GetCountryManager();
        return countryManager.GetCountryIcon(cityCountry);
    }

    public Color32 GetCityCountryColor()
    {
        String CityCountry = GetCityCountry();
        Color32 CityColor = GameValue.Instance.GetCountryColor(CityCountry);
        return CityColor;
    }


    public bool IsShowRegionPanel()
    {
        var playerCountry = GameValue.Instance.GetPlayerCountryENName();
        var regionCountry = regionValue.GetCountryENName();
        var cityCountry = regionValue.GetCityCountry(0);

        bool isPlayerRegion = regionCountry == playerCountry;
        bool isIndependent = GameValue.Instance.GetCountryManager().IsIndependent(cityCountry, playerCountry);

        return isPlayerRegion && !isIndependent;
    }



    public int GetExploreLevel() => exploreLevel;

    public int GetMaxPopulation() => maxPopulation;

    public int GetPopulation() => population;

    public string GetPopulationGrowthString()
    {
        Debug.Log("Calculate the growth rate by calculating the remaining food in the next round");
        return "10%";
    }

    public int GetNextTurnPopulation() => population;

    public string GetSupportRateString() => $"{supportRate * 100}%";

    public int GetAvailablePopulation() => (int)(population * supportRate * regionValue.GetTaxRate());

    public int GetRecruitedPopulation() => recruitedPopulation;

    public float GetResourceMax(int type) => resourceMax[type];

    public float GetResourceSurplus(int type) => resourceSurplus[type];

    public float GetResourceGrowth(int type)
    {
        Debug.Log("Remember to add the calculation for the next turn");
        return 0;
    }


    public string GetResourceGrowthString(int type)
    {
        Debug.Log("Remember to add the calculation for the next turn");
        return "%";
    }

    public float GetParameterWithLord(int type)
    {
        var Lord = regionValue.GetLord();
        return Lord != null
            ? resourceParameters[type] + Lord.GetValue(2, type)
            : resourceParameters[type];
    }

    public float GetResourceLastTax(int type)
    {
        Debug.Log("GetLastResourceTax Record the tax for the turn * region.TaxRate");
        return resourceParameters[type];
    }

    public float GetResourceNext(int type)
    {
        Debug.Log("Calculate the resource value plan to be obtained in the next round");
        return resourceParameters[type];
    }

    public float GetResoureNextTax(int type)
    {
        Debug.Log("GetLastResourceTax Record the tax for the turn * region.TaxRate");
        return resourceParameters[type];
    }

    public void AddTaskData(TaskData taskData)
    {
        taskDatas.Add(taskData);
        NotifynTaskDatasChanged();
    }

    public bool HasTask()
    {
        if (taskDatas.Count == 0)
        {
            return false;
        } else
        {
            return true;
        }
    }

    public List<TaskData> GetTaskDatas()
    {
        return taskDatas; 
    }

    public string GetBuildMaxString()
    {
        Debug.Log("haven't do yet,GetBuildMaxString");
        // Check building max values, check center 
        return "BUG";
    }

    public string GetBuildNumString()
    {
        Debug.Log("haven't do yet,GetBuildNumString");
        // Check building num values, check center 
        return "BUG";
    }

    public void RemoveAllListeners()
    {
       OnCityValueChanged = null;
       OnTaskDatasChanged = null;
    }

}


public class CityValueSaveData
{
        public string cityName;
        public string cityCountry;

        public int population;
        public int recruitedPopulation;
        public List<float> resourceSurplus = new List<float> { 0, 0, 0, 0, 0 };
        public List<float> resourceParameters = new List<float> { 0, 0, 0, 0, 0 };
        public List<float> resourceMax = new List<float> { 0, 0, 0, 0, 0 };
        public float supportRate;


        public int exploreLevel;
        public bool isPlayerBuild;
        public Dictionary<Vector2Int, HexValue> buildValues = new Dictionary<Vector2Int, HexValue>();
        public int seed;
        public List<string> storyNameAndIDs = new List<string>();

        public CityValueSaveData(CityValue cityValue)
        {
        if (cityValue == null) return;
            this.cityName = cityValue.GetCityENName();
            cityCountry = cityValue.GetCityCountry();
            exploreLevel = cityValue.GetExploreLevel();
            isPlayerBuild = cityValue.isPlayerBuild;

            population = cityValue.population;
            recruitedPopulation = cityValue.recruitedPopulation;
            resourceParameters = cityValue.resourceParameters;
            resourceSurplus = cityValue.resourceSurplus;
            resourceMax = cityValue.resourceMax;
            buildValues = cityValue.GetBuildValues();
            seed = cityValue.GetSeed();

             Debug.Log($"storyNameAndIDs. haven't make save");
        }
    

}