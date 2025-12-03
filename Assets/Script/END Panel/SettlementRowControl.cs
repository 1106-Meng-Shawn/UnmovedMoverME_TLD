using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static FormatNumber;
using Unity.VisualScripting;
using System.Runtime.Versioning;
using UnityEngine.U2D;

public class SettlementRowControl : MonoBehaviour
{
    public Image fristElementImage;
    public Image secondElementImage;
    public Image thirdElementImage;
    public TextMeshProUGUI fristElementText;
    public TextMeshProUGUI secondElementText;
    public TextMeshProUGUI thirdElementText;

    public Image resourcesTypeImage;
    public TextMeshProUGUI resourcesTypeText;
    public TextMeshProUGUI coefficientText;
    public TextMeshProUGUI achievementText;
    private GameValue gameValue;
    private string resourcesType;
    private int achievementValue;
    private float achievementCoefficient;


    public void SetValue(string type,GameValue gameValue)
    {
        this.gameValue = gameValue;
        this.resourcesType = type;
        SetText();
    }

    void SetText()
    {
        switch (resourcesType)
        {
            case "Population": SetPopulationText(); break;
            case "Time": SetTimeText(); break;
            case "RCI": SetRCIText(); break;
            case "Food": SetResourceText(ValueType.Food); break;
            case "Science": SetResourceText(ValueType.Science); break;
            case "Politics": SetResourceText(ValueType.Politics); break;
            case "Gold": SetResourceText(ValueType.Gold); break;
            case "Faith": SetResourceText(ValueType.Faith); break;
            case "HelpValue": SetHelpValueText(); break;

        }
    }

    void SetPopulationText()
    {
        int totalPopulation = gameValue.GetTotalPopulation();
        resourcesTypeText.text = totalPopulation.ToString("N0");
        float totalSupportRate = gameValue.GetTotalSupportRate();

        coefficientText.text = FormatfloatNumber(totalSupportRate * 100) + "%";
        achievementValue = Mathf.FloorToInt(totalPopulation * totalSupportRate); // need to big changer

        achievementText.text = achievementValue.ToString("N0");
    }

    void SetResourceText(ValueType type)
    {
        float totalResource = gameValue.GetResource(type);
        resourcesTypeText.text = FormatfloatNumberByN(totalResource);
        
        float coefficient = 0;
        List<RegionValue> playerRegions = gameValue.GetPlayerRegions();

        foreach (var region in playerRegions)
        {
            coefficient += region.GetRegionResourceParameter(type);
        }

        coefficient = coefficient / playerRegions.Count;

        coefficientText.text = FormatfloatNumber(coefficient);


        achievementValue = Mathf.RoundToInt(totalResource * coefficient); // need to big changer

        achievementText.text = achievementValue.ToString("N0"); 

    }

    void SetTimeText()
    {
        Sprite seasonSprite = null;
        string imagePath = $"MyDraw/UI/GameUI/";
        switch (gameValue.GetCurrentYear())
        {
            case 0: seasonSprite = Resources.Load<Sprite>(imagePath+"SpringSeason"); break;
            case 1: seasonSprite = Resources.Load<Sprite>(imagePath + "SummerSeason"); break;
            case 2: seasonSprite = Resources.Load<Sprite>(imagePath + "FallSeason"); break;
            case 3: seasonSprite = Resources.Load<Sprite>(imagePath + "WinterSeason"); break;
        }
        resourcesTypeImage.sprite = seasonSprite;
        int currtentYear = gameValue.GetCurrentYear();
     //   int currentSeason = gameValue.GetCurrentSeason();
        resourcesTypeText.text = currtentYear.ToString();
      //  coefficientText.text = gameValue.GetSeason();

        if (currtentYear <  GameValueConstants.Year_Opening)
        {
            achievementCoefficient = 0.1f;
        }
        else if (currtentYear < GameValueConstants.Year_Midgame)
        {
            achievementCoefficient = 0.5f;
        }
        else if (currtentYear < GameValueConstants.Year_Climax)
        {
            achievementCoefficient = 0.75f;
        }
        else
        {
            achievementCoefficient = 1;
        }

        achievementText.text = FormatfloatNumber(achievementCoefficient * 100) + "%";

    }

    void SetRCIText()
    {
        int PlayerRegionNum = gameValue.GetPlayerRegions().Count;
        Debug.LogWarning("remember change PlayerCharacterNum and PlayerItemNum");
        int PlayerCharacterNum = gameValue.GetPlayerRegions().Count;
        int PlayerItemNum = gameValue.GetPlayerRegions().Count;

        fristElementText.text = PlayerRegionNum.ToString();
        secondElementText.text = PlayerCharacterNum.ToString();
        thirdElementText.text = PlayerItemNum.ToString();

        this.achievementValue = PlayerRegionNum + PlayerCharacterNum + PlayerItemNum;// need to big change

        achievementText.text = achievementValue.ToString();

    }


    void SetHelpValueText()
    {
        ResourceValue resourceValue = gameValue.GetResourceValue();
        float Scout = resourceValue.Scout;
        float Build = resourceValue.Build;
        float Negotiation = resourceValue.Negotiation;

        fristElementText.text = Scout.ToString();
        secondElementText.text = Build.ToString();
        thirdElementText.text = Negotiation.ToString();

        this.achievementValue = (int)(Scout + Build + Negotiation);// need to big change
        achievementText.text = achievementValue.ToString();

    }


    public int GetAchievement()
    {
        return achievementValue;
    }

    public float GetAchievementCoefficient()
    {
        return achievementCoefficient;
    }


}
