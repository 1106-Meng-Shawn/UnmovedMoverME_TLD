using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Runtime.Versioning;
using UnityEditor;

public class SettlementPanelControl : MonoBehaviour
{
    public SettlementRowControl gameTimeRow;
    public SettlementRowControl populationRow;
    public SettlementRowControl RCIRow;
    public SettlementRowControl foodRow;
    public SettlementRowControl scienceRow;
    public SettlementRowControl politicsRow;
    public SettlementRowControl goldRow;
    public SettlementRowControl faithRow;
    public SettlementRowControl HelpValueRow;

    private GameValue gameValue;

    public TextMeshProUGUI achievementText;
    public TextMeshProUGUI totalAchievementText;


    void Start()
    {
        gameValue = GameValue.Instance;
        SetSettlementText();
    }

    void Update()
    {
        
    }

    public void InitSettlementPanel()
    {
        SetSettlementText();
    }

    public void ShowSettlementPanel()
    {
        gameObject.SetActive(true);

    }

    public void HideSettlementPanel()
    {
        gameObject.SetActive(false);

    }



    void SetSettlementText()
    {

        gameTimeRow.SetValue("Time", gameValue);
        populationRow.SetValue("Population", gameValue);
        RCIRow.SetValue("RCI", gameValue);
        foodRow.SetValue("Food", gameValue);
        scienceRow.SetValue("Science", gameValue);
        politicsRow.SetValue("Politics", gameValue);
        goldRow.SetValue("Gold", gameValue);
        faithRow.SetValue("Faith", gameValue);
        HelpValueRow.SetValue("HelpValue", gameValue);

        achievementText.text = gameValue.GetAchievement().ToString("N0");

        int totalAchievement = 0;
        totalAchievement += populationRow.GetAchievement();
        totalAchievement += RCIRow.GetAchievement();
        totalAchievement += foodRow.GetAchievement();
        totalAchievement += scienceRow.GetAchievement();
        totalAchievement += politicsRow.GetAchievement();
        totalAchievement += goldRow.GetAchievement();
        totalAchievement += faithRow.GetAchievement();
        totalAchievement += HelpValueRow.GetAchievement();

        totalAchievement = Mathf.RoundToInt(totalAchievement * gameTimeRow.GetAchievementCoefficient());

        totalAchievementText.text = totalAchievement.ToString("N0");


    }
}
