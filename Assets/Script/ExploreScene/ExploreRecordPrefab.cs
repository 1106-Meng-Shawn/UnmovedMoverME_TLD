using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using static ExcelReader;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization;

public class ExploreRecordPrefab : MonoBehaviour
{
    public TextMeshProUGUI recordContent;
    private ExploreRecordData exploreRecordData;


    void OnEnable()
    {
        LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
    }

    void OnDisable()
    {
        LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;
    }

    private void OnDestroy()
    {
        LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;

    }

    private void OnLocaleChanged(Locale locale)
    {
        SetContentText();
    }



    public void SetExploreRecordData(ExploreRecordData exploreRecordData)
    {
        this.exploreRecordData = exploreRecordData;
        SetContentText();
    }

    void SetContentText()
    {
        recordContent.text = exploreRecordData.GetFloorRecordString();
    }

}

public class ExploreRecordData
{
    public int floor;
    public ExploreCardData exploreCardData;

    public ExploreRecordData(int floor, ExploreCardData exploreCardData = null)
    {
        this.floor = floor;
        this.exploreCardData = exploreCardData;
    }

    public string GetFloorRecordString()
    {
        if (exploreCardData == null) {
            return $"Floor {floor} : Start Exploring.";
        } else
        {
            return $"Floor {floor} : {exploreCardData.GetTypeDescribe()}.";

        }
     //   return "Bug in ExploreRecordData not string";
    }
}