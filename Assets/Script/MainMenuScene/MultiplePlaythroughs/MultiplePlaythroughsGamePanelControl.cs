using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiplePlaythroughsGamePanelControl : MonoBehaviour
{
    public void ShowPanel()
    {
        gameObject.SetActive(true);
    }

    public void ClosePanel()
    {
        gameObject.SetActive(false);
    }

    public void Default()
    {

    }

}


public class MultiplePlaythroughsGameTotalData
{
   public MultiplePlaythroughsGameTotalCountryData MultiplePlaythroughsGameTotalCountryData = new MultiplePlaythroughsGameTotalCountryData();
   public MultiplePlaythroughsGameCIInheritanceData MultiplePlaythroughsGameCIInheritanceData = new MultiplePlaythroughsGameCIInheritanceData();

    public int GetAchievementCost()
    {
        return MultiplePlaythroughsGameTotalCountryData.GetAchievementCost() + MultiplePlaythroughsGameCIInheritanceData.GetAchievementCost();
    }

}