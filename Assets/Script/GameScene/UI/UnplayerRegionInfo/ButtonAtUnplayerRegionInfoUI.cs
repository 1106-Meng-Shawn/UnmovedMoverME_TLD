using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ButtonAtUnplayerRegionInfoUI : MonoBehaviour
{
    public Button foodBuy;
    public Button foodSell;

    public Button scienceBuy;
    public Button scienceSell;

    public Button politicsBuy;
    public Button politicsSell;

    public Button moneySend;
    public Button moneyAsk;

    public Button FaithBuy;
    public Button FaithSell;


    public Button DecareWarButton;

    public Button ExploreButton;

    public GameObject PreBattle;



    public Button recruitedIncrease100;
    public Button recruitedIncrease500;
    public Button recruitedIncreaseMAX;

    public GameValue gameValue;



   // public UnplayerRegionInfoUI unplayerRegionInfoUI;



     public string regionName;
     public RegionValue regionValue;
    public TopRowTextShow topRowTextShow;

    public Image[] cityIcon;

    public Image RegionCountry;


   // public BattlePanelCharacterInfo battlePanelCharacterInfo;
    public GameObject enemyArrayRowInfo;

    public List<Sprite> CountryIcon;




    void Start()
    {


        foodBuy.onClick.AddListener(() => HandleResourceTransaction("Food", true));
        foodSell.onClick.AddListener(() => HandleResourceTransaction("Food", false));
        AddHoldButtonListener(foodBuy, () => HandleResourceTransaction("Food", true));
        AddHoldButtonListener(foodSell, () => HandleResourceTransaction("Food", false));




        scienceBuy.onClick.AddListener(() => HandleResourceTransaction("Science", true));
        scienceSell.onClick.AddListener(() => HandleResourceTransaction("Science", false));
        AddHoldButtonListener(scienceBuy, () => HandleResourceTransaction("Science", true));
        AddHoldButtonListener(scienceSell, () => HandleResourceTransaction("Science", false));



        politicsBuy.onClick.AddListener(() => HandleResourceTransaction("Politics", true));
        politicsSell.onClick.AddListener(() => HandleResourceTransaction("Politics", false));
        AddHoldButtonListener(politicsBuy, () => HandleResourceTransaction("Politics", true));
        AddHoldButtonListener(politicsSell, () => HandleResourceTransaction("Politics", false));


        FaithBuy.onClick.AddListener(() => HandleResourceTransaction("Faith", true));
        FaithSell.onClick.AddListener(() => HandleResourceTransaction("Faith", false));
        AddHoldButtonListener(FaithBuy, () => HandleResourceTransaction("Faith", true));
        AddHoldButtonListener(FaithSell, () => HandleResourceTransaction("Faith", false));


        moneySend.onClick.AddListener(() => HandleMoneyTransaction(true));
        moneyAsk.onClick.AddListener(() => HandleMoneyTransaction(false));
        AddHoldButtonListener(moneySend, () => HandleMoneyTransaction(true));
        AddHoldButtonListener(moneyAsk, () => HandleMoneyTransaction(false));

        recruitedIncrease100.onClick.AddListener(() => RecruitedIncrease(100));
        AddHoldButtonListener(recruitedIncrease100, () => RecruitedIncrease(100));
        recruitedIncrease500.onClick.AddListener(() => RecruitedIncrease(500));
        AddHoldButtonListener(recruitedIncrease500, () => RecruitedIncrease(500));
        recruitedIncreaseMAX.onClick.AddListener(() => RecruitedIncreaseMAX());


        DecareWarButton.onClick.AddListener(() => ActivePanel(PreBattle, false));
        ExploreButton.onClick.AddListener(() => ActivePanel(PreBattle,true));

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


    void Update()
    {

        if (gameValue == null) gameValue = GameObject.FindObjectOfType<GameValue>();


      /*  if (unplayerRegionInfoUI != null)
        {

            if (regionName != null && regionName != "NONE")
            {
                regionValue = unplayerRegionInfoUI.regionValue;
                topRowTextShow.UpdateTextDisplay();
                SetCityIcon();
            }
        }*/

    }

    void HandleResourceTransaction(string resourceType, bool isBuying)
    {
        int num = 100;
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        {
            num *= 10;
        }

        float price = 0;
        /*
        // Common logic for calculating price for different resources
        switch (resourceType)
        {
            case "Food":
                price = (isBuying) ? unplayerRegionInfoUI.foodBuyP : unplayerRegionInfoUI.foodSellP;
             //   ProcessFoodTransaction(ref gameValue.food, ref regionValue.GetResourceSurplus(0), ref gameValue.gold, price, num, isBuying, regionValue);
                break;

            case "Science":
                price = (isBuying) ? unplayerRegionInfoUI.scienceBuyP : unplayerRegionInfoUI.scienceSellP;
             //   ProcessTransaction(ref gameValue.science, ref regionValue.science, ref gameValue.gold, price, num, isBuying, regionValue);
                break;

            case "Politics":
                price = (isBuying) ? unplayerRegionInfoUI.politicsBuyP : unplayerRegionInfoUI.politicsSellP;
             //   ProcessTransaction(ref gameValue.politics, ref regionValue.politics, ref gameValue.gold, price, num, isBuying, regionValue);
                break;

            case "Faith":
                price = (isBuying) ? unplayerRegionInfoUI.faithBuyP : unplayerRegionInfoUI.faithSellP;
             //   ProcessTransaction(ref gameValue.faith, ref regionValue.faith, ref gameValue.gold, price, num, isBuying, regionValue);
                break;
        }
        */


    }

    void ProcessFoodTransaction(ref float gameResource, ref float regionResource, ref float goldResource, float price, int num, bool isBuying, RegionValue region)
    {
        if (isBuying)
        {
            if (regionResource >= num && goldResource >= price * num && region.GetPopulationGrowth() >= 0.05)
            {
                gameResource += num;
                regionResource -= num;
                goldResource -= num * price;
            }
        }
        else
        {
            if (price * num <= region.GetRegionResourceSurplus(3))
            {
                num = (int)(region.GetRegionResourceSurplus(3) / price);
            }
            if (gameResource >= num)
            {
                goldResource += num * price;
                gameResource -= num;
                regionResource += num;
            }
            else if (gameResource < num)
            {
                goldResource += gameResource * price;
                regionResource += gameResource;
                gameResource = 0;
            }
        }
    }



    // Helper function to process transactions (buy/sell)
    void ProcessTransaction(ref float gameResource, ref float regionResource, ref float goldResource, float price, int num, bool isBuying, RegionValue region)
    {
        if (isBuying)
        {
            if (regionResource >= num && goldResource >= price * num)
            {
                gameResource += num;
                regionResource -= num;
                goldResource -= num * price;
            }
        }
        else
        {
            if (price * num <= region.GetRegionResourceSurplus(3))
            {
                num = (int)(region.GetRegionResourceSurplus(3) / price);
            }
                if (gameResource >= num)
                {
                    goldResource += num * price;
                    gameResource -= num;
                    regionResource += num;
                }
                else if (gameResource < num)
                {
                    goldResource += gameResource * price;
                    regionResource += gameResource;
                    gameResource = 0;
                }

           
        }
    }

    void HandleMoneyTransaction(bool isSending)
    {
        int num = 1000;
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        {
            num *= 10;
        }



        if (isSending)
        {

            if (gameValue.GetResourceValue().Gold >= num)
            {
                gameValue.GetResourceValue().Gold -= num;
                //regionValue.gold += num;
             //   regionValue.supportRate += (float)(0.01 * (num / 1000));
            }
        }
        else
        {

            if (regionValue.GetRegionSupportRate() >= 0.7 && regionValue.GetRegionResourceSurplus(3) >= num)
            {
             //   regionValue.gold -= num;
                gameValue.GetResourceValue().Gold += num;
             //   regionValue.supportRate -= (float)(0.01 * (num / 1000));
            }
        }
    }


    void RecruitedIncrease(int num)
    {
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        {
            num *= 10;
        }

        if (regionValue == null) return;

        int recruitedDifference = Mathf.Min(num, regionValue.GetRegionAvailablePopulation() - regionValue.GetRegionRecruitedPopulation());
        if (recruitedDifference <= 0) return;  

        int foodCost = recruitedDifference;
        int goldCost = recruitedDifference;

        void Recruit(int recruitedAmount, ref float food, ref float gold)
        {
            gameValue.GetResourceValue().TotalRecruitedPopulation += recruitedAmount;
        //    regionValue.GetRecruitedPopulation() += recruitedAmount;
            food -= recruitedAmount;
            gold -= recruitedAmount;
        }

  /*      if (gameValue.Food >= foodCost && gameValue.Gold >= goldCost)
        {
            Recruit(recruitedDifference, ref gameValue.Food, ref gameValue.Gold);
        }
        else if (gameValue.food >= foodCost && gameValue.gold < goldCost)
        {
            int recruitFromGold = (int)(gameValue.gold);
            Recruit(recruitFromGold, ref gameValue.food, ref gameValue.gold);
        }
        else if (gameValue.food < foodCost && gameValue.gold >= goldCost)
        {
            int recruitFromFood = (int)(gameValue.food);
            Recruit(recruitFromFood, ref gameValue.food, ref gameValue.gold);
        }*/
    }



    void RecruitedIncreaseMAX()
    {
       /* if (regionValue == null) return;

        int recruitedPopulationDifference = regionValue.GetRegionAvailablePopulation() - regionValue.GetRegionRecruitedPopulation();
        if (recruitedPopulationDifference <= 0) return;

        int recruitAmount = Mathf.Min(recruitedPopulationDifference, (int)(gameValue.Food), (int)(gameValue.Gold));

       
        gameValue.TotalRecruitedPopulation += recruitAmount;
     //   regionValue.RecruitedPopulation += recruitAmount;
        gameValue.Food -= recruitAmount;
        gameValue.Gold -= recruitAmount;*/
    }


    void ActivePanel(GameObject panel,bool isExplore)
    {
        if (panel != null)
        {
            BattlePanelValue value = panel.GetComponent<BattlePanelValue>();

            if (value != null && !isExplore)
            {
             /*   int battleCity = findBattleCity(regionValue);
                if (battleCity == -1) return;
                value.battleCity = battleCity;*/
                value.BattleRegionValue = regionValue;
                value.battleCountryName = regionValue.GetCountryENName();
                value.battleRegionName = regionName;
                value.SetEnemyToPosition();


            }
            else if (value != null && isExplore)
            {
                value.BattleRegionValue = regionValue;
                value.battleCountryName = regionValue.GetCountryENName();
                value.battleRegionName = regionName;
            }
            value.SetIsExplore(isExplore);
            panel.SetActive(true);
        }
    }


    private void SetCityIcon()
    {
        if (regionValue == null) return;
        RegionCountry.sprite = regionValue.GetCountryIcon();
        for (int i = 0; i < regionValue.GetCityCountryNum(); i++)
        {
            /*if (i == regionValue.cityNum-1) { 
            cityIcon[cityIcon.Length-1].sprite = 
                    //CountryIcon[TransTransfCountryNameToInt(regionValue.cityCountry[i])];
            //RegionCountry.sprite = cityIcon[cityIcon.Length - 1].sprite;  

             return; }*/
            //cityIcon[i].sprite = CountryIcon[TransTransfCountryNameToInt(regionValue.cityCountry[i])];
            cityIcon[i].sprite = regionValue.GetCityIcon(i);
        }
    }


    /*  private void setCityIcon()
      {
          if (regionValue == null ) return;
          for (int i = 0; i < regionValue.cityNum; i++)
          {
              if (i == regionValue.cityNum - 1) { 
              cityIcon[cityIcon.Length-1].sprite = CountryIcon[TransTransfCountryNameToInt(regionValue.cityCountry[i])];
              RegionCountry.sprite = cityIcon[cityIcon.Length - 1].sprite; 
              return; }
              cityIcon[i].sprite = CountryIcon[TransTransfCountryNameToInt(regionValue.cityCountry[i])];
          }
      }*/


        public int TransTransfCountryNameToInt(string countryName)
    {
        switch (countryName)
        {
            case "Arab": return 1;
            case "Babylon": return 2;
            case "Carthage": return 3;
            case "Demon Kingdom": return 4;
            case "Egypt": return 5;
            case "Kingdom of Englaland": return 6;
            case "East Romulus Empire":return 7;
            case "Kingdom of Francus": return 8;
            case "Holy Romulus Empire": return 9;
            case "Hungary": return 10;
            case "State of the Knights Templar": return 11;
            case "Kalmar Union": return 12;
            case "Macedonia": return 13;
            case "Ottoman Empire":return 14;
            case "Persian Empire": return 15;
            case "Polish-Lithuanian Commonwealth": return 16;
            case "Rus Principality": return 17;
            case "Sami":return 18;
            case "Sky Fire Empire": return 19;
            case "State of the Teutonic Order":return 20;
            case "Scythia":return 21;
            case "Greece":return 22;
            case "Demihuman Alliance":return 23;
            case "Avalon":return 24;
            case "Giant's Island":return 25;
            case "Military State of Naples":return 26;
            case "Kingdom of Sicily":return 27;



            default: return 0;
        }


    

    }
}
