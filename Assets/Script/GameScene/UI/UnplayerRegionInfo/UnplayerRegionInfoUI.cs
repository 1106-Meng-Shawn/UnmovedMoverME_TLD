using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class UnplayerRegionInfoUIOld : MonoBehaviour
{
    public GameObject unplayerRegionPanel;

    public RectTransform unplayerRegionPanelTransform;
    public float moveSpeed = 2f;

    private Vector2 targetPosition;

    public TMP_Text unplayerRegionNameText;
    public TMP_Text populationText;
    public TMP_Text AvailablePopulationText;
    public TMP_Text RecruitedText;

    public TMP_Text foodText;
    public TMP_Text foodBuyPrice;
    public TMP_Text foodSellPrice;
    public float foodBuyP;
    public float foodSellP;


    public TMP_Text scienceText;
    public TMP_Text scienceBuyPrice;
    public TMP_Text scienceSellPrice;
    public float scienceBuyP;
    public float scienceSellP;


    public TMP_Text politicsText; 
    public TMP_Text politicsBuyPrice;
    public TMP_Text politicsSellPrice;
    public float politicsBuyP;
    public float politicsSellP;


    public TMP_Text faithText;
    public TMP_Text faithBuyPrice;
    public TMP_Text faithSellPrice;
    public float faithBuyP;
    public float faithSellP;


    public TMP_Text goldText;
    public TMP_Text favorabilityLose;
    public TMP_Text favorabilityGet;
    public float favorabilityBuyP;
    public float favorabilitySellP;



    public TMP_Text favorabilityText;
    public TMP_Text lordNameText;

    public TMP_Text ExploreLevelText;

    public GameObject city1;
    public GameObject city2;
    public GameObject mainCity;

    public RegionValue regionValue;

   public string unplayerRegionNameAtUI = "NONE";


    private Vector3 ShowPosition;
    private Vector3 HidePosition;


    public Image UnPlayerRegionLord;
    public Sprite emptyLordSprite;



    public new  Animation animation;


    private void Start()
    {

        animation   = unplayerRegionPanel.GetComponent<Animation>();

            ShowPosition = new Vector2(-unplayerRegionPanelTransform.rect.width - 104, unplayerRegionPanelTransform.anchoredPosition.y);
            HidePosition = new Vector2(-1300, unplayerRegionPanelTransform.anchoredPosition.y);


        if (unplayerRegionPanel != null)
        {
            targetPosition = HidePosition;
            unplayerRegionPanelTransform.anchoredPosition = targetPosition;
            unplayerRegionPanel.SetActive(false);


        }

        foodBuyP = 0;
        foodBuyPrice.text = foodBuyP.ToString("F2");
        foodSellP = 0;
        foodSellPrice.text = foodSellP.ToString("F2");

        scienceBuyP = 0;
        scienceBuyPrice.text = scienceBuyP.ToString("F2");
        scienceSellP = 0;
        scienceSellPrice.text = scienceSellP.ToString("F2");

        politicsBuyP = 0;
        politicsBuyPrice.text = politicsBuyP.ToString("F2");
        politicsSellP = 0;
        politicsSellPrice.text = politicsSellP.ToString("F2");

        faithBuyP = 0;
        faithBuyPrice.text = faithBuyP.ToString("F2");
        faithSellP = 0;
        faithSellPrice.text = faithSellP.ToString("F2");

        favorabilityGet.text = (+1).ToString() + "%";
        favorabilityLose.text = (-1).ToString() + "%";



    }

    private void Update()
    {
      /*  if (string.IsNullOrEmpty(unplayerRegionNameAtUI) || unplayerRegionNameAtUI == "NONE") return;

       if(unplayerRegionPanel.activeSelf && regionValue ==null) regionValue = GameObject.Find(unplayerRegionNameAtUI+"Value").GetComponent<RegionValue>();

        if (regionValue != null){
            UpdateResourcePrice(ref foodBuyP, ref foodSellP, foodBuyPrice, foodSellPrice, regionValue.GetResourceSurplus(0), regionValue.GetResourceParameter(0), regionValue.populationGrowth, "food");
            UpdateResourcePrice(ref scienceBuyP, ref scienceSellP, scienceBuyPrice, scienceSellPrice, regionValue.GetResourceSurplus(1), regionValue.GetResourceParameter(1), regionValue.GetResourceParameter(1), "science");
            UpdateResourcePrice(ref politicsBuyP, ref politicsSellP, politicsBuyPrice, politicsSellPrice, regionValue.GetResourceSurplus(2), regionValue.GetResourceParameter(2), regionValue.GetResourceParameter(2), "politics");
            UpdateResourcePrice(ref faithBuyP, ref faithSellP, faithBuyPrice, faithSellPrice, regionValue.GetResourceSurplus(3), regionValue.GetResourceParameter(4), regionValue.GetResourceParameter(4), "faith");

        }

      //*/
        if (Input.GetMouseButtonDown(1) && unplayerRegionPanel.activeSelf) HideRegionInfo(); 

    }

    private void UpdateResourcePrice(ref float buyPrice, ref float sellPrice, TMP_Text buyPriceText, TMP_Text sellPriceText, float resourceValue, float resourceParameter, float growth, string resourceType)
    {
        if (resourceValue != 0)
        {
            float factor = 1 / resourceValue;

            buyPrice = ((regionValue.GetRegionRecruitedPopulation() - regionValue.GetRegionRecruitedPopulation()) * resourceParameter * (1 + growth) / resourceValue) * (1 + regionValue.GetTaxRate()) * (1 + factor);
            buyPriceText.text = buyPrice.ToString("F2");

            sellPrice = ((regionValue.GetRegionPopulation() - regionValue.GetRegionRecruitedPopulation()) * resourceParameter * (1 + growth) / resourceValue) * (1 - regionValue.GetTaxRate()) * (1 - factor);
            sellPriceText.text = sellPrice.ToString("F2");
        }
        else if (buyPrice != 0)
        {
            buyPrice = 0;
            buyPriceText.text = "Empty";
            sellPrice *= 2;
            sellPriceText.text = sellPrice.ToString("F2");
        }
    }



    public void MoveRegionInfoUI(RegionValue regionValue)
    {
        if (regionValue == this.regionValue  && unplayerRegionPanel.activeSelf) return;

        if (unplayerRegionPanel.activeSelf){
         //   StartCoroutine(ReplacePanel(regionValue));
           animation.Play("HideWithUn");
           this.regionValue = regionValue;
           animation.Play("ShowWithUn");
           // StartCoroutine(ReplacePanel()); 
        } else {
            unplayerRegionPanel.SetActive(true); 
            this.regionValue = regionValue;
            targetPosition = ShowPosition;
           // StartCoroutine(MovePanelToPosition(targetPosition));
           animation.Play("ShowWithUn");


        }


        ShowLord();

    }

    void ShowLord()
    {
        // may be add some function to hide lord, let player use exprole to unlock
        if (regionValue.lord != null)
        {
            UnPlayerRegionLord.sprite = regionValue.lord.image;
            lordNameText.text = regionValue.lord.GetCharacterName();

        }
        else
        {
            UnPlayerRegionLord.sprite = emptyLordSprite;
            lordNameText.gameObject.SetActive(false);

        }

    }

    public void HideRegionInfo()
    {
        animation.Play("HideWithUn");

        StartCoroutine(WaitForAnimationEnd());
    }

    private IEnumerator WaitForAnimationEnd()
    {
        float animationDuration = animation.GetClip("HideWithUn").length;

        yield return new WaitForSeconds(animationDuration);
        regionValue = null;
        unplayerRegionPanel.SetActive(false);
    }


    /*

        float timeElapsed = 0f;
        float moveDuration = 0.1f;


        private IEnumerator MovePanelToPosition(Vector2 target)
        {
            timeElapsed = 0f;  // Reset timeElapsed for the new movement
            Vector2 startPos = unplayerRegionPanelTransform.anchoredPosition;

            bool originPanelActive = unplayerRegionPanel.activeSelf;


            while (timeElapsed < moveDuration)
            {
                unplayerRegionPanelTransform.anchoredPosition = Vector2.Lerp(startPos, targetPosition, timeElapsed / moveDuration);
                timeElapsed += Time.deltaTime;
                yield return null;
            }

            unplayerRegionPanelTransform.anchoredPosition = targetPosition;

        }


        private IEnumerator ReplacePanel(RegionValue regionValue)
        {
            timeElapsed = 0f;  // Reset timeElapsed for the new movement
            targetPosition = HidePosition;
            Vector2 startPos = unplayerRegionPanelTransform.anchoredPosition;

            while (timeElapsed < moveDuration)
            {
                unplayerRegionPanelTransform.anchoredPosition = Vector2.Lerp(startPos, targetPosition, timeElapsed / moveDuration);
                timeElapsed += Time.deltaTime;
                yield return null;
            }

            unplayerRegionPanelTransform.anchoredPosition = targetPosition;

            this.regionValue = regionValue;

            timeElapsed = 0f;  // Reset timeElapsed for the new movement
            targetPosition = ShowPosition;

            startPos = unplayerRegionPanelTransform.anchoredPosition;

            while (timeElapsed < moveDuration)
            {
                unplayerRegionPanelTransform.anchoredPosition = Vector2.Lerp(startPos, targetPosition, timeElapsed / moveDuration);
                timeElapsed += Time.deltaTime;
                yield return null;
            }

            unplayerRegionPanelTransform.anchoredPosition = targetPosition;

            yield return null;

        }


        public void HideRegionInfo()
        {
            targetPosition = HidePosition;

            StartCoroutine(HidePanelToPosition(targetPosition));
            unplayerRegionNameAtUI = "NONE";
            regionValue = null;
        }

        private IEnumerator HidePanelToPosition(Vector2 target)
        {
            timeElapsed = 0f;  // Reset timeElapsed for the new movement
            Vector2 startPos = unplayerRegionPanelTransform.anchoredPosition;

            bool originPanelActive = unplayerRegionPanel.activeSelf;


            while (timeElapsed < moveDuration)
            {
                unplayerRegionPanelTransform.anchoredPosition = Vector2.Lerp(startPos, targetPosition, timeElapsed / moveDuration);
                timeElapsed += Time.deltaTime;
                yield return null;
            }

            unplayerRegionPanelTransform.anchoredPosition = targetPosition;

            unplayerRegionPanel.SetActive(false); 

            regionValue = null;
        }

    */


    public void ShowRegionInfoBase(string regionName, int population, int RecruitedPopulation, string lordName, float favorability, int cityNum,int ExploreLevel)
    {
        if (regionName == null) return;
        if (unplayerRegionNameText != null) unplayerRegionNameText.text = regionName;

        if (populationText != null) populationText.text = FormatNumber(population);
        if (favorabilityText != null) favorabilityText.text = FormatfloatNumber(favorability * 100) + "%";  // Favorability as percentage
        if (lordNameText != null) lordNameText.text = lordName;
        if (AvailablePopulationText != null) AvailablePopulationText.text = FormatNumber((int)(population * favorability * 0.1));
        if (RecruitedText != null) RecruitedText.text = FormatNumber(RecruitedPopulation);

        ExploreLevelText.text = ExploreLevel.ToString();


        setCity(cityNum);

    }

   string FormatfloatNumber(float value)
    {
        int valueInt = Mathf.RoundToInt(value * 100);
        if (valueInt % 100 == 0 || (valueInt * 10 % 1000) == 0)
        {
            return value.ToString("F0");
        }
        else if (valueInt % 10 == 0)
        {
            return value.ToString("F1");
        }
        else
        {
            return value.ToString("F2");
        }

    }




    string FormatNumber(float value)
    {
        if (value >= 1000000000)
        {
            // For Billions (B)
            int billions = Mathf.FloorToInt(value / 100000000f);
            float temp = Mathf.RoundToInt(billions) / 10f;


            // Check if value falls within the range
            if (temp == Mathf.Floor(temp))
            {
                return temp + "B";
            }
            else
            {
                // Round to one decimal point (0.1B, 1.1B, etc.)
                return temp.ToString("0.0") + "B";  // Always show 1 decimal point if it's a non-integer
            }
        }
        else if (value >= 1000000)
        {


            // For Millions (M)
            int millions = Mathf.FloorToInt(value / 100000f);
            float temp = Mathf.RoundToInt(millions) / 10f;

            // Check if value falls within the range
            if (temp == Mathf.Floor(temp))
            {
                return temp + "M";
            }
            else
            {
                // Round to one decimal point (1.1M, 2.1M, etc.)
                return temp.ToString("0.0") + "M";  // Always show 1 decimal point if it's a non-integer
            }
        }
        else if (value >= 1000)
        {
            // For Thousands (K)
            int hundred = Mathf.FloorToInt(value / 100f);
            float temp = Mathf.RoundToInt(hundred) / 10f;

            // Check if value falls within the range
            if (temp == Mathf.Floor(temp))
            {
                return temp.ToString("F0") + "K";
            }
            else
            {
                return temp.ToString("0.0") + "K";
            }
        }
        else
        {
            // For values under 1000, show as integer
            return value.ToString("F0");
        }
    }




    public void ShowRegionInfoValue(float food, float science, float politics, float gold, float faith)
    {
        foodText.text = FormatNumber(food);
        scienceText.text = FormatNumber(science);
        politicsText.text = FormatNumber(politics);
        goldText.text = FormatNumber(gold);
        faithText.text = FormatNumber(faith);
    }



    public void setRegionNameAtUI(string unplayerRegionName)
    {
        unplayerRegionNameAtUI = unplayerRegionName;

    }

    private void setCity(int cityNum)
    {

        if (cityNum == 1)
        {
            mainCity.SetActive(true);
            mainCity.transform.localPosition = new Vector3(0, 0, 0);

            if (city1 != null) city1.SetActive(false);
            if (city2 != null) city2.SetActive(false);
        }
        else if (cityNum == 2)
        {
            if (city1 != null)
            {
                city1.SetActive(true);
                city1.transform.localPosition = new Vector3(-45, 0, 0);
            }
            if (mainCity != null)
            {
                mainCity.SetActive(true);
                mainCity.transform.localPosition = new Vector3(45, 0, 0);
            }

            if (city2 != null) city2.SetActive(false);
        }
        else if (cityNum == 3)
        {
            if (city1 != null)
            {
                city1.SetActive(true);
                city1.transform.localPosition = new Vector3(-90, 0, 0);
            }
            if (city2 != null)
            {
                city2.SetActive(true);
                city2.transform.localPosition = new Vector3(0, 0, 0);
            }
            if (mainCity != null)
            {
                mainCity.SetActive(true);
                mainCity.transform.localPosition = new Vector3(90, 0, 0);
            }
        }

    }


}
