using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using static FormatNumber;
public class TopRowTextShow : MonoBehaviour
{
    public GameValue playerGameValue;

    public TextMeshProUGUI populationText;
    public TextMeshProUGUI foodText;
    public TextMeshProUGUI scienceText;
    public TextMeshProUGUI politicsText;
    public TextMeshProUGUI goldText;
    public TextMeshProUGUI faithText;
    public TextMeshProUGUI mobilizedPopulationText;
    public TextMeshProUGUI supportRateText;

    public TextMeshProUGUI achievementText;


    private List<RegionValue> allRegions = new List<RegionValue>();


    void Start()
    {
        //UpdatePlayerEmpireData("Holy Romulus Empire");  
        if (playerGameValue == null) playerGameValue = GameObject.FindObjectOfType<GameValue>();
        RecordAllRegionData();
    //    playerGameValue.UpdatePopulationFromRegions(allRegions.ToArray(), "Holy Romulus Empire");
        UpdateTextDisplay();

    }

    public void StartNewTurn()
    {

      /*  foreach (RegionValue region in allRegions)
        {
            region.UpdateGrowthValues();
        }

        RecordAllRegionData();
        playerGameValue.UpdateValuesFromRegions(allRegions.ToArray(), "Holy Romulus Empire");
        playerGameValue.UpdatePopulationFromRegions(allRegions.ToArray(), "Holy Romulus Empire");

        foreach (RegionValue region in allRegions)
        {
            region.UpdateGrowthValues();
        }

        */

    }

    void RecordAllRegionData()
    {
        Region[] allRegionsArray = FindObjectsOfType<Region>();
        allRegions.Clear();

        foreach (var region in allRegionsArray)
        {
            if (region.GetRegionValue() != null) 
            {
                allRegions.Add(region.GetRegionValue());  
            }
        }

    }


    public void UpdateTextDisplay()
    {
        populationText.text = FormatNumberToString(playerGameValue.GetTotalPopulation()); 
        foodText.text = FormatNumberToString(playerGameValue.GetResourceValue().Food); 
        scienceText.text = FormatNumberToString(playerGameValue.GetResourceValue().Science); 
        politicsText.text = FormatNumberToString(playerGameValue.GetResourceValue().Politics); 
        goldText.text = FormatNumberToString(playerGameValue.GetResourceValue().Gold); 
        faithText.text = FormatNumberToString(playerGameValue.GetResourceValue().Faith);
        supportRateText.text = FormatfloatNumber(playerGameValue.GetTotalSupportRate() * 100) + "%";


        mobilizedPopulationText.text = FormatNumberToString(playerGameValue.GetResourceValue().TotalRecruitedPopulation);

        achievementText.text = playerGameValue.GetAchievement().ToString("N0");
    }



}
