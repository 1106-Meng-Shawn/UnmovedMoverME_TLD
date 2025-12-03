using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TipPanelManager : MonoBehaviour
{
    public static TipPanelManager Instance;
    public GameObject TipPrefab;
    public List<GameObject> Tips;

    public int MaxTips;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }


    private void Start()
    {
    }

    private void Update()
    {
        
    }


    public void AddFavorability(string characterName, int changesValue)
    {

        Character character = GameValue.Instance.GetCharacterByName(characterName);
        if (character == null) return;
        character.AddFavorabilityValue(changesValue); // maybe need chaneg

        GameObject tip = Instantiate(TipPrefab, this.transform);
        tip.GetComponent<HintControl>().ShowFavorabilityAdd(character, changesValue);
        Tips.Add(tip);

        if (Tips.Count > MaxTips)
        {
            GameObject oldest = Tips[0];
            Tips.RemoveAt(0);
            Destroy(oldest);
        }
        //     Tips.Add(tip);
    }

    public void ChangeFavorabilityLevel(string characterName, FavorabilityLevel changesValue)
    {
        Character character = GameValue.Instance.GetCharacterByName(characterName);
        character.FavorabilityLevel = changesValue;
        Debug.Log("do i need make a tip to make sure player know favorabilitylevel change?");
    }

    public void AddItem(string itemName, int value)
    {

        ItemBase item = GameValue.Instance.GetItem(itemName);
        item.ItemNumAdd(value);
     //   GameValue.Instance.UpdatePlayerItems();
        GameObject tip = Instantiate(TipPrefab, this.transform);
        tip.GetComponent<HintControl>().ShowItemAdd(item, value);
        Tips.Add(tip);
        if (Tips.Count > MaxTips)
        {
            GameObject oldest = Tips[0];
            Tips.RemoveAt(0);
            Destroy(oldest);
        }

    }

    public void CharacterTypeChange(string CharaceterName, string type)
    {

        Character character = GameValue.Instance.GetCharacterByName(CharaceterName);
        if (character == null) return;
        character.SetCharacterType(type); 
    }

    public void RemoveHint(GameObject tip)
    {
        if (Tips.Contains(tip)) Tips.Remove(tip);
    }


    public void ChangeRegionCountry(string regionName, string countryName)
    {
        List<RegionValue> allRegions = GameValue.Instance.GetAllRegionValues();
        RegionValue targetRegion = allRegions.FirstOrDefault(r => r.GetRegionENName() == regionName);

        if (targetRegion != null)
        {
            ChangeRegionCountry(targetRegion, countryName);
        }
        else
        {
            Debug.LogWarning($"Region '{regionName}' not found.");
        }
    }


    void ChangeRegionCountry(RegionValue regionValue,string countryName)
    {
        List<CityValue> cityValues = regionValue.GetCityValues();
        regionValue.MoveLord();
        foreach (var city in cityValues)
        {
            city.cityCountry = countryName;
        }

        regionValue.SetCountryColor();

    }

    public void ChangeCityCountry(string regionName, string cityIndexStr, string countryName)
    {
        List<RegionValue> allRegions = GameValue.Instance.GetAllRegionValues();
        RegionValue targetRegion = allRegions.FirstOrDefault(r => r.GetRegionENName() == regionName);

        if (targetRegion == null)
        {
            Debug.LogWarning($"Region '{regionName}' not found.");
            return;
        }

        // ???? cityIndex
        if (!int.TryParse(cityIndexStr, out int cityIndex))
        {
            Debug.LogWarning($"Invalid city index: '{cityIndexStr}'");
            return;
        }

        // ???????
        ChangeCityCountry(targetRegion, cityIndex, countryName);
    }


    void ChangeCityCountry(RegionValue regionValue,int cityIndex, string countryName)
    {
        regionValue.GetCityValue(cityIndex).cityCountry = countryName;
    }

    void ChangeCityCountry(CityValue cityValue, string countryName)
    {
        cityValue.cityCountry = countryName;
    }



}
