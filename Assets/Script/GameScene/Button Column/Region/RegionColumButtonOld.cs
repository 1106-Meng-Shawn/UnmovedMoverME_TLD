using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static GetSprite;


public class RegionColumButtonOld : MonoBehaviour
{
 /*  public Button starButton;

    public Button RecruitedPlusButton;
    public Button RecruitedMinsButton;

    public Button TaxRatePlusButton;
    public Button TaxRateMinsButton;

    public Button TotalResourcePlusButton;
    public Button TotalResourceMinsButton;

    public Button ParameterResourcePlusButton;
    public Button GrowthResourcePlusButton;

    public RegionColumControl regionColumControl;

    private int ResourceType = 0;

    public GameValue gameValue;
    public RegionValue regionValue;

    public GameObject feedbackPrefab;

    public void setRegion(RegionValue regionValue)
    {
        this.regionValue = regionValue;
    }

    void Start()
    {
        gameValue = GameObject.Find("GameValue")?.GetComponent<GameValue>();
        if (RecruitedPlusButton != null)
        {
            RecruitedPlusButton.onClick.AddListener(() => RecruitedIncrease(100));

            HoldButton holdButton = RecruitedPlusButton.GetComponent<HoldButton>();
            if (holdButton != null)
            {
                holdButton.onHoldClick.AddListener(() => RecruitedIncrease(100));
            }
        }

        if (RecruitedMinsButton != null)
        {
            RecruitedMinsButton.onClick.AddListener(() => RecruitedDecrease(100));

            HoldButton holdButton = RecruitedMinsButton.GetComponent<HoldButton>();
            if (holdButton != null)
            {
                holdButton.onHoldClick.AddListener(() => RecruitedDecrease(100));
            }
        }

        if (TaxRatePlusButton != null)
        {
            TaxRatePlusButton.onClick.AddListener(() => taxRateIncrease());

            HoldButton holdButton = TaxRatePlusButton.GetComponent<HoldButton>();
            if (holdButton != null)
            {
                holdButton.onHoldClick.AddListener(() => taxRateIncrease());
            }
        }

        if (TaxRateMinsButton != null)
        {
            TaxRateMinsButton.onClick.AddListener(() => taxRateDecrease());

            HoldButton holdButton = TaxRateMinsButton.GetComponent<HoldButton>();
            if (holdButton != null)
            {
                holdButton.onHoldClick.AddListener(() => taxRateDecrease());
            }
        }

        ResourceType = regionColumControl.ResourceType;

        if (ResourceType == 0)
        {
            TotalResourcePlusButton.onClick.AddListener(() => foodIncrease());
            TotalResourceMinsButton.onClick.AddListener(() => foodDecrease());
            ParameterResourcePlusButton.onClick.AddListener(() => foodParameterChange());
            GrowthResourcePlusButton.onClick.AddListener(() => foodGrowthChange());

            AddHoldButtonListener(TotalResourcePlusButton, () => foodIncrease());
            AddHoldButtonListener(TotalResourceMinsButton, () => foodDecrease());
            AddHoldButtonListener(ParameterResourcePlusButton, () => foodParameterChange());
            AddHoldButtonListener(GrowthResourcePlusButton, () => foodGrowthChange());
        }
        else if (ResourceType == 1)
        {
            TotalResourcePlusButton.onClick.AddListener(() => scienceIncrease());
            TotalResourceMinsButton.onClick.AddListener(() => scienceDecrease());
            ParameterResourcePlusButton.onClick.AddListener(() => scienceParameterChange());
            GrowthResourcePlusButton.onClick.AddListener(() => scienceGrowthChange());

            AddHoldButtonListener(TotalResourcePlusButton, () => scienceIncrease());
            AddHoldButtonListener(TotalResourceMinsButton, () => scienceDecrease());
            AddHoldButtonListener(ParameterResourcePlusButton, () => scienceParameterChange());
            AddHoldButtonListener(GrowthResourcePlusButton, () => scienceGrowthChange());
        }
        else if (ResourceType == 2)
        {
            TotalResourcePlusButton.onClick.AddListener(() => politicsIncrease());
            TotalResourceMinsButton.onClick.AddListener(() => politicsDecrease());
            ParameterResourcePlusButton.onClick.AddListener(() => politicParameterChange());
            GrowthResourcePlusButton.onClick.AddListener(() => politicGrowthChange());

            AddHoldButtonListener(TotalResourcePlusButton, () => politicsIncrease());
            AddHoldButtonListener(TotalResourceMinsButton, () => politicsDecrease());
            AddHoldButtonListener(ParameterResourcePlusButton, () => politicParameterChange());
            AddHoldButtonListener(GrowthResourcePlusButton, () => politicGrowthChange());
        }
        else if (ResourceType == 3)
        {
            TotalResourcePlusButton.onClick.AddListener(() => goldIncrease());
            TotalResourceMinsButton.onClick.AddListener(() => goldDecrease());
            ParameterResourcePlusButton.onClick.AddListener(() => goldParameterChange());
            GrowthResourcePlusButton.onClick.AddListener(() => goldGrowthChange());

            AddHoldButtonListener(TotalResourcePlusButton, () => goldIncrease());
            AddHoldButtonListener(TotalResourceMinsButton, () => goldDecrease());
            AddHoldButtonListener(ParameterResourcePlusButton, () => goldParameterChange());
            AddHoldButtonListener(GrowthResourcePlusButton, () => goldGrowthChange());
        }
        else if (ResourceType == 4)
        {
            TotalResourcePlusButton.onClick.AddListener(() => faithIncrease());
            TotalResourceMinsButton.onClick.AddListener(() => faithDecrease());
            ParameterResourcePlusButton.onClick.AddListener(() => faithParameterChange());
            GrowthResourcePlusButton.onClick.AddListener(() => faithGrowthChange());

            AddHoldButtonListener(TotalResourcePlusButton, () => faithIncrease());
            AddHoldButtonListener(TotalResourceMinsButton, () => faithDecrease());
            AddHoldButtonListener(ParameterResourcePlusButton, () => faithParameterChange());
            AddHoldButtonListener(GrowthResourcePlusButton, () => faithGrowthChange());
        }

        InitStarButton();
    }

    private void AddHoldButtonListener(Button button, UnityAction action)
    {
        if (button != null)
        {
            HoldButton holdButton = button.GetComponent<HoldButton>();
            if (holdButton != null)
            {
                holdButton.onHoldClick.AddListener(action);
            }
        }
    }


    private void InitStarButton()
    {
        starButton.gameObject.GetComponent<Image>().sprite = UpStarButtonSprite(regionValue.GetIsStar());
        starButton.onClick.AddListener(OnStarButtonClick);
    }

    void OnStarButtonClick()
    {
        regionValue.SetIsStar(!regionValue.GetIsStar());
        starButton.gameObject.GetComponent<Image>().sprite = UpStarButtonSprite(regionValue.GetIsStar());

    }

    private void Update()
    {
        if (ResourceType != regionColumControl.ResourceType)
        {
            ResourceType = regionColumControl.ResourceType;

            RemoveButtonListeners();

            if (ResourceType == 0)
            {
                TotalResourcePlusButton.onClick.AddListener(() => foodIncrease());
                TotalResourceMinsButton.onClick.AddListener(() => foodDecrease());
                ParameterResourcePlusButton.onClick.AddListener(() => foodParameterChange());
                GrowthResourcePlusButton.onClick.AddListener(() => foodGrowthChange());

                AddHoldButtonListener(TotalResourcePlusButton, () => foodIncrease());
                AddHoldButtonListener(TotalResourceMinsButton, () => foodDecrease());
                AddHoldButtonListener(ParameterResourcePlusButton, () => foodParameterChange());
                AddHoldButtonListener(GrowthResourcePlusButton, () => foodGrowthChange());
            }
            else if (ResourceType == 1)
            {
                TotalResourcePlusButton.onClick.AddListener(() => scienceIncrease());
                TotalResourceMinsButton.onClick.AddListener(() => scienceDecrease());
                ParameterResourcePlusButton.onClick.AddListener(() => scienceParameterChange());
                GrowthResourcePlusButton.onClick.AddListener(() => scienceGrowthChange());

                AddHoldButtonListener(TotalResourcePlusButton, () => scienceIncrease());
                AddHoldButtonListener(TotalResourceMinsButton, () => scienceDecrease());
                AddHoldButtonListener(ParameterResourcePlusButton, () => scienceParameterChange());
                AddHoldButtonListener(GrowthResourcePlusButton, () => scienceGrowthChange());
            }
            else if (ResourceType == 2)
            {
                TotalResourcePlusButton.onClick.AddListener(() => politicsIncrease());
                TotalResourceMinsButton.onClick.AddListener(() => politicsDecrease());
                ParameterResourcePlusButton.onClick.AddListener(() => politicParameterChange());
                GrowthResourcePlusButton.onClick.AddListener(() => politicGrowthChange());

                AddHoldButtonListener(TotalResourcePlusButton, () => politicsIncrease());
                AddHoldButtonListener(TotalResourceMinsButton, () => politicsDecrease());
                AddHoldButtonListener(ParameterResourcePlusButton, () => politicParameterChange());
                AddHoldButtonListener(GrowthResourcePlusButton, () => politicGrowthChange());
            }
            else if (ResourceType == 3)
            {
                TotalResourcePlusButton.onClick.AddListener(() => goldIncrease());
                TotalResourceMinsButton.onClick.AddListener(() => goldDecrease());
                ParameterResourcePlusButton.onClick.AddListener(() => goldParameterChange());
                GrowthResourcePlusButton.onClick.AddListener(() => goldGrowthChange());

                AddHoldButtonListener(TotalResourcePlusButton, () => goldIncrease());
                AddHoldButtonListener(TotalResourceMinsButton, () => goldDecrease());
                AddHoldButtonListener(ParameterResourcePlusButton, () => goldParameterChange());
                AddHoldButtonListener(GrowthResourcePlusButton, () => goldGrowthChange());
            }
            else if (ResourceType == 4)
            {
                TotalResourcePlusButton.onClick.AddListener(() => faithIncrease());
                TotalResourceMinsButton.onClick.AddListener(() => faithDecrease());
                ParameterResourcePlusButton.onClick.AddListener(() => faithParameterChange());
                GrowthResourcePlusButton.onClick.AddListener(() => faithGrowthChange());

                AddHoldButtonListener(TotalResourcePlusButton, () => faithIncrease());
                AddHoldButtonListener(TotalResourceMinsButton, () => faithDecrease());
                AddHoldButtonListener(ParameterResourcePlusButton, () => faithParameterChange());
                AddHoldButtonListener(GrowthResourcePlusButton, () => faithGrowthChange());
            }
        }
    }

    private void RemoveButtonListeners()
    {
        TotalResourcePlusButton.onClick.RemoveAllListeners();
        TotalResourceMinsButton.onClick.RemoveAllListeners();
        ParameterResourcePlusButton.onClick.RemoveAllListeners();
        GrowthResourcePlusButton.onClick.RemoveAllListeners();

        RemoveHoldButtonListener(TotalResourcePlusButton);
        RemoveHoldButtonListener(TotalResourceMinsButton);
        RemoveHoldButtonListener(ParameterResourcePlusButton);
        RemoveHoldButtonListener(GrowthResourcePlusButton);
    }

    private void RemoveHoldButtonListener(Button button)
    {
        HoldButton holdButton = button.GetComponent<HoldButton>();
        if (holdButton != null)
        {
            holdButton.onHoldClick.RemoveAllListeners();
        }
    }


    private string getResourceType(int num)
    {
        switch (num)
        {
            case 0: return "food";
            case 1: return "science";
            case 2: return "politics";
            case 3: return "gold";
            case 4: return "faith";
            default: return "unknown";
        }
    }


    public void foodIncrease()
    {
      //  IncreaseResource("food", ref gameValue.food, ref regionValue.foodSurplus);
    }

    public void foodDecrease()
    {
     //   decreaseResource("food", ref gameValue.food, ref regionValue.foodSurplus);
    }

    public void scienceIncrease()
    {
      //  IncreaseResource("science", ref gameValue.science, ref regionValue.science);
    }

    public void scienceDecrease()
    {
      //  decreaseResource("science", ref gameValue.science, ref regionValue.science);
    }

    public void politicsIncrease()
    {
     //   IncreaseResource("politics", ref gameValue.politics, ref regionValue.politics);
    }

    public void politicsDecrease()
    {
       // decreaseResource("politics", ref gameValue.politics, ref regionValue.politics);
    }

    public void goldIncrease()
    {
      //  IncreaseResource("gold", ref gameValue.gold, ref regionValue.gold);
    }

    public void goldDecrease()
    {
       // decreaseResource("gold", ref gameValue.gold, ref regionValue.gold);
    }

    public void faithIncrease()
    {
      //  IncreaseResource("faith", ref gameValue.faith, ref regionValue.faith);
    }

    public void faithDecrease()
    {
      //  decreaseResource("faith", ref gameValue.faith, ref regionValue.faith);
    }


    private void ModifyValuesForCtrl(ref float value, float multiplier)
    {
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        {
            value *= multiplier;
        }
    }
    private void IncreaseResource(string resourceName, ref float gameResource, ref float regionResource)
    {

        if (gameResource == 0)
        {

            NotificationManage.Instance.ShowToTop($"Your {resourceName} resouce is not enough"); return;

        }

        float increaseAmount = 100;

        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        {
            increaseAmount = 1000;
        }

        if (gameResource >= increaseAmount)
        {
            gameResource -= increaseAmount;
            regionResource += increaseAmount;
        }
        else if (gameResource < increaseAmount && gameResource > 0)
        {
            regionResource += gameResource;
            gameResource = 0;
        }

    }


    private void decreaseResource(string resourceName, ref float gameResource, ref float regionResource)
    {

        if (regionResource == 0)
        {

            NotificationManage.Instance.ShowToTop($"{regionValue.GetRegionName()}'s {resourceName} resouce is not enough"); return;

        }

        float decreaseAmount = 100;

        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        {
            decreaseAmount = 1000;
        }

        if (regionResource >= decreaseAmount)
        {
            regionResource -= decreaseAmount;
            gameResource += decreaseAmount;
        }
        else if (regionResource < decreaseAmount && regionResource > 0)
        {
            gameResource += regionResource;
            regionResource = 0;
        }

    }


    public void ParameterChange(string resourceName, ref float resourceAmount, float resourceCost, ref float parameter, float increment)
    {
        ModifyValuesForCtrl(ref resourceCost, 10);
        ModifyValuesForCtrl(ref increment, 10);

        if (resourceAmount >= resourceCost)
        {
            resourceAmount -= resourceCost;
            parameter += increment;
            parameter = (float)Mathf.Round(parameter * 100) / 100f;
            return;
        }

        NotificationManage.Instance.ShowToTop($"Your politics resouce is not enough"); 

    }
    public void scienceParameterChange()
    {
      //  ParameterChange("science",ref gameValue.politics, 1000, ref regionValue.scienceParameter, 0.1f);
    }


    public void politicParameterChange()
    {
      //  ParameterChange("poliitic",ref gameValue.politics, 1000, ref regionValue.politicsParameter, 0.1f);
    }

    public void goldParameterChange()
    {
       // ParameterChange("gold", ref gameValue.politics, 1000, ref regionValue.goldParameter, 0.1f);
    }

    public void faithParameterChange()
    {
     //   ParameterChange("faith",ref gameValue.politics, 5000, ref regionValue.faithParameter, 0.1f);
    }


    public void GrowthChange(string resourceName,ref float resourceAmount, float resourceCost, ref float growthParameter, float divisor)
    {

        ModifyValuesForCtrl(ref resourceCost, 10);
        ModifyValuesForCtrl(ref divisor, 0.1f);

        if (resourceAmount >= resourceCost)
        {
            resourceAmount -= resourceCost;
            growthParameter += 10 / divisor;
            float randomIncrement = Random.Range(-1 / divisor, 0f);
            growthParameter += randomIncrement;


            growthParameter = (float)Mathf.Round(growthParameter * 10000) / 10000f;

        }

        NotificationManage.Instance.ShowToTop($"Your {resourceName} resouce is not enough"); return;

    }

    public void foodParameterChange()
    {
     //   ParameterChange("food",ref gameValue.politics, 1000, ref regionValue.foodParameter, 0.1f);
    }

    public void foodGrowthChange()
    {
        float nextTurnfood = (regionValue.GetResourceSurplus(0) + ((regionValue.GetPopulation() - regionValue.GetRecruitedPopulation()) * regionValue.GetResourceParameter(0) * (1 + regionValue.GetResourceGrowth(0)) * (1 - regionValue.GetTaxRate())))
                             - regionValue.GetPopulation() * (1 + regionValue.GetPopulationGrowth());
        if (nextTurnfood <= 1000)
        {
            nextTurnfood = 1000;
        }
      //  GrowthChange("food", ref gameValue.politics, 1000, ref regionValue.foodGrowth, nextTurnfood * 10);
    }

    public void scienceGrowthChange()
    {
      //  float nextTurnScience = (regionValue.population - regionValue.RecruitedPopulation) * regionValue.scienceParameter * (1 + regionValue.scienceGrowth) * (1 - regionValue.taxRate);
        //GrowthChange("science",ref gameValue.politics, 1000, ref regionValue.scienceGrowth, nextTurnScience);
    }

    public void politicGrowthChange()
    {
       // float nextTurnPolitics = (regionValue.population - regionValue.RecruitedPopulation) * regionValue.politicsParameter * (1 + regionValue.politicsGrowth) * (1 - regionValue.taxRate);

        //GrowthChange("politics", ref gameValue.politics, 1000, ref regionValue.politicsGrowth, nextTurnPolitics);
    }

    public void goldGrowthChange()
    {
      //  float nextTurnGold = (regionValue.population - regionValue.RecruitedPopulation) * regionValue.goldParameter * (1 + regionValue.goldGrowth) * (1 - regionValue.taxRate);

        //GrowthChange("gold",ref gameValue.politics, 1000, ref regionValue.goldGrowth, nextTurnGold);
    }

    public void faithGrowthChange()
    {
     //   float nextTurnFaith = (regionValue.population - regionValue.RecruitedPopulation) * regionValue.faithParameter * (1 + regionValue.faithGrowth);

       // GrowthChange("faith", ref gameValue.politics, 5000, ref regionValue.faithGrowth, nextTurnFaith);
    }



    public void taxRateIncrease()
    {
     /*   if (regionValue.taxRate > 0.999) { popNotification($"The tax rate of {regionValue.GetRegionName()} is already 100%!"); return; }


        if (regionValue != null && regionValue.taxRate <= 0.999)
        {
            if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
            {
                regionValue.taxRate = (float)(regionValue.taxRate + 0.01);

            }
            else
            {
                regionValue.taxRate = (float)(regionValue.taxRate + 0.001);

            }
        }
        else
        {
            regionValue.taxRate = 1;
        }*/
    }
/*
  public void taxRateDecrease()
  {








      if (regionValue.taxRate < 0.001) { popNotification($"The tax rate of {regionValue.GetRegionName()} is already 0%!"); return; }

      if (regionValue != null && regionValue.taxRate >= 0.001)
      {

          if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
          {
              regionValue.taxRate = (float)(regionValue.taxRate - 0.01);

          } else
          {
              regionValue.taxRate = (float)(regionValue.GetTaxRate( - 0.001);

          }

      }
      else
      {
          regionValue.taxRate = 0;
      }*/
/*
}

private void RecruitedIncrease(int num)
{
   if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
   {
       num *= 10;  // If Control key is held, increase the num by a factor of 10
   }

   if (regionValue == null) return;  // Ensure regionValue is not null

   if (regionValue.GetResourceSurplus(0) <= 0) { NotificationManage.Instance.ShowToTop($"Not enough food in the {regionValue.GetRegionName()}!"); return; }
   if (regionValue.GetRecruitedPopulation() == regionValue.GetPopulation()) { NotificationManage.Instance.ShowToTop($"The number of recruits in {regionValue.GetRegionName()} is already at the maximum!"); return; }


   // Check if there is enough food surplus for recruitment
   if (num <= regionValue.GetResourceSurplus(0) && regionValue.GetResourceSurplus(0) > 0)
   {
       if (regionValue.GetRecruitedPopulation() < regionValue.GetAvailablePopulation())
       {
           // Check if adding the num would exceed AvailablePoulation
           if (regionValue.GetRecruitedPopulation() + num > regionValue.GetAvailablePopulation())
           {
               // If so, first set RecruitedPopulation to AvailablePoulation
               int recruitAmount = regionValue.GetAvailablePopulation() - regionValue.GetRecruitedPopulation();
               gameValue.TotalRecruitedPopulation += recruitAmount;
           //    regionValue.RecruitedPopulation = regionValue.GetAvailablePopulation();  // Set to AvailablePoulation
             //  regionValue.foodSurplus -= recruitAmount;
           }
           else
           {
               // If there is no overflow, continue recruiting normally
               gameValue.TotalRecruitedPopulation += num;
            //   regionValue.RecruitedPopulation += num;
           //    regionValue.foodSurplus -= num;
           }
       }
       else
       {
           if (regionValue.GetRecruitedPopulation() + num > regionValue.GetPopulation())
           {
               // If so, first set RecruitedPopulation to AvailablePoulation
               int recruitAmount = regionValue.GetPopulation() - regionValue.GetAvailablePopulation();
               gameValue.TotalRecruitedPopulation += recruitAmount;
             //  regionValue.RecruitedPopulation = regionValue.GetAvailablePopulation();  // Set to AvailablePoulation
             //  regionValue.foodSurplus -= recruitAmount;
           }
           else
           {
               // If there is no overflow, continue recruiting normally
               gameValue.TotalRecruitedPopulation += num;
             //  regionValue.RecruitedPopulation += num;
             //  regionValue.foodSurplus -= num;
           }

       }
   }
   else if (num > regionValue.GetResourceSurplus(0) && regionValue.GetResourceSurplus(0) > 0)
   {
       // If food surplus is less than num, recruit as much as the food surplus allows
       int recruitAmount = (int)(regionValue.GetResourceSurplus(0));
       gameValue.TotalRecruitedPopulation += recruitAmount;
    //   regionValue.RecruitedPopulation += recruitAmount;
     //  regionValue.foodSurplus -= recruitAmount;
   }
}

private void RecruitedDecrease(int num)
{
   if (regionValue == null) return;  // Ensure regionValue is not null

   if (regionValue.GetRecruitedPopulation() <= 0) { NotificationManage.Instance.ShowToTop($"The number of recruits in {regionValue.GetRegionName()} is already at the minimum!"); return; }
   if (gameValue.TotalRecruitedPopulation <= 0) { NotificationManage.Instance.ShowToTop("The total number of recruits is not enough!"); return; }


   if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
   {
       num *= 10;  // If Control key is held, increase the num by a factor of 10
   }



   // Ensure there is enough food and recruited population to decrease
   if (gameValue.TotalRecruitedPopulation > 0)
   {
       // Check if num can be reduced without going below zero in RecruitedPopulation
       int reduceAmount = Mathf.Min(num, regionValue.GetRecruitedPopulation());

       // Reduce recruited population and update the total recruited population
       gameValue.TotalRecruitedPopulation -= reduceAmount;

//            regionValue.RecruitedPopulation -= reduceAmount;

       // Increase the food surplus by the reduced amount
      // regionValue.foodSurplus += reduceAmount;

       // Ensure RecruitedPopulation doesn't go below zero
  //     regionValue.RecruitedPopulation = Mathf.Max(0, regionValue.RecruitedPopulation);

   }


}

}*/
