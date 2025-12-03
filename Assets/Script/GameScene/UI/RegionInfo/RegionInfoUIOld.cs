using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;


public class RegionInfoUIOld : MonoBehaviour
{
    public GameObject regionInfoPanel;

    public RectTransform regionInfoPanelTransform;  
    public float moveSpeed = 2f;          

    private Vector2 targetPosition;

    public TMP_Text regionNameText;
    public TMP_Text populationText;

    public TMP_Text foodText;
    public TMP_Text scienceText;
    public TMP_Text politicsText;
    public TMP_Text faithText;
    public TMP_Text goldText;

    public TMP_Text supportRateText;

    public TMP_Text foodNextText;
    public TMP_Text scienceNextText;
    public TMP_Text politicsNextText;
    public TMP_Text faithNextText;
    public TMP_Text goldNextText;

    public TMP_Text foodTaxText;
    public TMP_Text scienceTaxText;
    public TMP_Text politicsTaxText;
    public TMP_Text faithTaxText;
    public TMP_Text goldTaxText;

    public TMP_Text foodParameter;
    public TMP_Text scienceParameter;
    public TMP_Text politicsParameter;
    public TMP_Text faithParameter;
    public TMP_Text goldParameter;

    public TMP_Text foodGrowth;
    public TMP_Text scienceGrowth;
    public TMP_Text politicsGrowth;
    public TMP_Text faithGrowth;
    public TMP_Text goldGrowth;

    public TMP_Text populationNexthText;
    public TMP_Text populationGrowthText;
    public TMP_Text taxRateText;

    public TMP_Text ExploreLevelText;


    public TMP_Text AvailablePopulationText;
    public TMP_Text RecruitedPopulationText;

    public GameObject city1;
    public GameObject city2;
    public GameObject mainCity;


    public UnityEngine.UI.Image lordImage;
    public Sprite NoLordImage;
    public TMP_Text lordNameText;



    [HideInInspector] public string regionNameAtUI;
    [HideInInspector] public RegionValue regionAtUI;


    public new Animation animation;


    private void Start()
    {

        animation = regionInfoPanel.GetComponent<Animation>();
        if (regionInfoPanel != null)
        {
            targetPosition = new Vector2(-Screen.width, regionInfoPanelTransform.anchoredPosition.y);
            regionInfoPanelTransform.anchoredPosition = targetPosition;
            regionInfoPanel.SetActive(false);

        }

    }



    private void Update()
    {
        if (regionAtUI != null && regionAtUI.lord == null)
        {
            lordImage.sprite = NoLordImage;
            lordNameText.text = "NONE";

            if (regionAtUI != null)  
            {
                regionAtUI.lord = null;
            }
        }

        if (Input.GetMouseButtonDown(1) && regionInfoPanel.activeSelf) HideRegionInfo(); 
    }

  /*  public void setLord(Character character)
    {
        if (character.IsMoved) return;
        if (regionAtUI.lord != null)
        {
            regionAtUI.lord.LordRegion = null;
        }

        if (regionAtUI != null)
        {
            regionAtUI.lord = character;
        }

        if (character != null)
        {
            character.LordRegion = regionAtUI;
            lordImage.sprite = character.image;
            string currentLanguage = LocalizationSettings.SelectedLocale.Identifier.Code;
            lordNameText.text = character.GetCharacterName();
        }
        else
        {
            lordImage.sprite = NoLordImage;
            lordNameText.text = "NONE";
        }

        regionAtUI.lord = character;
    } */

    public void setRegionNameAtUI(string regionName)
    {
        regionNameAtUI = regionName;

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



    public void ShowRegionInfoBase(string regionName, int population,int AvailablePoulation, int RecruitedPopulation, float supportRate, float taxRate, float populationGrowth, int cityNum, RegionValue region)
    {
        if (regionName == null) return;

            if (regionNameText != null) regionNameText.text = regionName;
        //regionNameAtUI = regionName;

        if (region.lord != null)
        {
            lordImage.sprite = region.lord.image;
            string currentLanguage = LocalizationSettings.SelectedLocale.Identifier.Code;
            lordNameText.text = region.lord.GetCharacterName();

        } else
        {
            lordImage.sprite = NoLordImage;
            lordNameText.text = "NONE";
        }


//        ExploreLevelText.text = region.ExploreLevel.ToString();

        if (populationText != null) populationText.text = FormatNumber(population); 
            if (supportRateText != null) supportRateText.text = FormatfloatNumber(supportRate * 100) + "%";

            if (populationGrowthText != null) populationGrowthText.text =  FormatfloatNumber(populationGrowth * 100) + "%";

            if (populationGrowthText != null)
            {
                populationGrowthText.color = populationGrowth > 0 ? Color.green : Color.red;
            }

            if (populationNexthText != null)
            {
                UpdateTextWithColor(populationNexthText, population, Mathf.Max(2, population * (1 + populationGrowth)));
            }

            if (taxRateText != null) taxRateText.text = FormatfloatNumber(taxRate * 100) + "%";

            if (AvailablePopulationText != null) AvailablePopulationText.text = FormatNumber(AvailablePoulation);
            if (RecruitedPopulationText != null) RecruitedPopulationText.text = FormatNumber(RecruitedPopulation); 

            if (RecruitedPopulationText != null)
            {
                RecruitedPopulationText.color = (population * supportRate * taxRate >= RecruitedPopulation) ? Color.green : Color.red;
            }

            setCity(cityNum);


            regionAtUI = region;
    }


    public void ShowRegionInfoValue(int population, int RecruitedPopulation , float populationGrowth, string resourceName, float resourceValue, float resourceParameter, float growthRate, float taxRate)
    {
        TMP_Text resourceText = GetResourceTextComponent(resourceName); 
        TMP_Text nextResourceText = GetNextResourceTextComponent(resourceName); 
        TMP_Text taxResourceText = GetTaxResourceTextComponent(resourceName);
        TMP_Text parameterText = GetParameterTextComponent(resourceName);
        TMP_Text growthText = GetGrowthTextComponent(resourceName);


        resourceText.text = FormatNumber(resourceValue); 
        if (resourceValue < 0)
        {
            resourceText.color = Color.red;
        } else
        {
            resourceText.color = Color.white;
        }


        if (regionAtUI.lord != null)
        {
            if (resourceName == "food") {resourceParameter =  resourceParameter + (regionAtUI.lord.GetValue(1,0) / 10f);}
            if (resourceName == "science") resourceParameter = resourceParameter + (regionAtUI.lord.GetValue(1, 1) / 10f);
            if (resourceName == "politics") resourceParameter = resourceParameter + (regionAtUI.lord.GetValue(1, 2) / 10f);
            if (resourceName == "faith") resourceParameter = resourceParameter +(regionAtUI.lord.GetValue(1, 3) / 10f);
            if (resourceName == "gold") resourceParameter = resourceParameter +(regionAtUI.lord.GetValue(1, 4) / 10f);
            
        }


        float nextValue = Mathf.Max(0, (population * (1 + populationGrowth) - RecruitedPopulation) * resourceParameter   * (1+growthRate) );

        if (resourceName == "food")
        {

            float foodSurplus = resourceValue; 

            float nextfoodSurplus = (foodSurplus + (population - RecruitedPopulation) * (1 + growthRate) * resourceParameter * (1 - taxRate)) - (population * (1 + populationGrowth));
            UpdateTextWithColor(nextResourceText, foodSurplus, nextfoodSurplus);

            float foodtax = Mathf.Max(0, population / (1 + populationGrowth)  * resourceParameter  * taxRate);
            float nextfoodtax = Mathf.Max(0, population * resourceParameter * (1 + growthRate) * taxRate);

            UpdateTextWithColor(taxResourceText, foodtax, nextfoodtax);

        } else if (resourceName == "faith")
        {
            UpdateTextWithColor(nextResourceText, resourceValue , nextValue );
            UpdateTextWithColor(taxResourceText, resourceValue , nextValue );

        }
        else
        {
            UpdateTextWithColor(nextResourceText, Mathf.Max(0, (resourceValue * (1 - taxRate))), Mathf.Max(0, nextValue * (1 - taxRate)));
            UpdateTextWithColor(taxResourceText, Mathf.Max(0, (resourceValue * taxRate)), Mathf.Max(0, (nextValue * taxRate)));


        }

            parameterText.text = FormatfloatNumber(resourceParameter);  


            growthText.text = FormatfloatNumber(growthRate * 100) + "%";  

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




    private TMP_Text GetResourceTextComponent(string resourceName)
    {
        if (resourceName == "food") return foodText;
        if (resourceName == "science") return scienceText;
        if (resourceName == "politics") return politicsText;
        if (resourceName == "faith") return faithText;
        if (resourceName == "gold") return goldText;

        return null;
    }

    private TMP_Text GetNextResourceTextComponent(string resourceName)
    {
        if (resourceName == "food") return foodNextText;
        if (resourceName == "science") return scienceNextText;
        if (resourceName == "politics") return politicsNextText;
        if (resourceName == "faith") return faithNextText;
        if (resourceName == "gold") return goldNextText;

        return null;
    }

    private TMP_Text GetTaxResourceTextComponent(string resourceName)
    {
        if (resourceName == "food") return foodTaxText;
        if (resourceName == "science") return scienceTaxText;
        if (resourceName == "politics") return politicsTaxText;
        if (resourceName == "faith") return faithTaxText;
        if (resourceName == "gold") return goldTaxText;

        return null;
    }

    private TMP_Text GetParameterTextComponent(string resourceName)
    {
        if (resourceName == "food") return foodParameter;
        if (resourceName == "science") return scienceParameter;
        if (resourceName == "politics") return politicsParameter;
        if (resourceName == "faith") return faithParameter;
        if (resourceName == "gold") return goldParameter;

        return null;
    }

    private TMP_Text GetGrowthTextComponent(string resourceName)
    {
        if (resourceName == "food") return foodGrowth;
        if (resourceName == "science") return scienceGrowth;
        if (resourceName == "politics") return politicsGrowth;
        if (resourceName == "faith") return faithGrowth;
        if (resourceName == "gold") return goldGrowth;

        return null;
    }




    public void MoveRegionInfoUI(RegionValue regionValue)
    {


        if (regionValue == this.regionAtUI && regionInfoPanel.activeSelf) return;


        if (regionInfoPanel.activeSelf){
          //  StartCoroutine(ReplacePanel(regionValue));
           animation.Play("Hide");
           this.regionAtUI = regionValue;
           animation.Play("Show");

           // StartCoroutine(ReplacePanel()); 
        } else {
            regionInfoPanel.SetActive(true); 
            this.regionAtUI = regionValue;

            targetPosition = new Vector2(-regionInfoPanelTransform.rect.width - 104, regionInfoPanelTransform.anchoredPosition.y);

        //    StartCoroutine(MovePanelToPosition(targetPosition));
           animation.Play("Show");

        }
    }

        public void HideRegionInfo()
    {

        animation.Play("Hide");
        StartCoroutine(WaitForAnimationEnd());


    }


    private IEnumerator WaitForAnimationEnd()
    {
        float animationDuration = animation.GetClip("Hide").length;

        yield return new WaitForSeconds(animationDuration);
        regionAtUI = null;
        regionInfoPanel.SetActive(false);

    }





    /*    float timeElapsed = 0f;
        float moveDuration = 0.75f;

        private IEnumerator ReplacePanel(RegionValue regionValue)
    {
        timeElapsed = 0f;  // Reset timeElapsed for the new movement
        targetPosition = new Vector2(-Screen.width, regionInfoPanelTransform.anchoredPosition.y);
        Vector2 startPos = regionInfoPanelTransform.anchoredPosition;

        while (timeElapsed < moveDuration)
        {
            regionInfoPanelTransform.anchoredPosition = Vector2.Lerp(startPos, targetPosition, timeElapsed / moveDuration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        regionInfoPanelTransform.anchoredPosition = targetPosition;

        this.regionAtUI = regionValue;


        timeElapsed = 0f;  // Reset timeElapsed for the new movement
        targetPosition = new Vector2(-regionInfoPanelTransform.rect.width - 104, regionInfoPanelTransform.anchoredPosition.y);

        startPos = regionInfoPanelTransform.anchoredPosition;

        while (timeElapsed < moveDuration)
        {
            regionInfoPanelTransform.anchoredPosition = Vector2.Lerp(startPos, targetPosition, timeElapsed / moveDuration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        regionInfoPanelTransform.anchoredPosition = targetPosition;

        yield return null;

    }




    private IEnumerator MovePanelToPosition(Vector2 target)
    {
        timeElapsed = 0f;
        Vector2 startPos = regionInfoPanelTransform.anchoredPosition;

        while (timeElapsed < moveDuration)
        {
            regionInfoPanelTransform.anchoredPosition = Vector2.Lerp(startPos, targetPosition, timeElapsed / moveDuration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        regionInfoPanelTransform.anchoredPosition = targetPosition;
    }

    public void HideRegionInfo()
    {
        targetPosition = new Vector2(-Screen.width, regionInfoPanelTransform.anchoredPosition.y);

        StartCoroutine(HidePanelToPosition(targetPosition));
        /*unplayerRegionNameAtUI = "NONE";
        regionValue = null;
    }

    private IEnumerator HidePanelToPosition(Vector2 target)
    {
        timeElapsed = 0f;  // Reset timeElapsed for the new movement
        Vector2 startPos = regionInfoPanelTransform.anchoredPosition;

        bool originPanelActive = regionInfoPanel.activeSelf;


        while (timeElapsed < moveDuration)
        {
            regionInfoPanelTransform.anchoredPosition = Vector2.Lerp(startPos, targetPosition, timeElapsed / moveDuration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        regionInfoPanelTransform.anchoredPosition = targetPosition;

        regionInfoPanel.SetActive(false); 

        regionAtUI = null;
    }*/

    void UpdateTextWithColor(TMP_Text textComponent, float currentValue, float newValue)
    {
        textComponent.text = FormatNumber(newValue);

        if (currentValue < newValue && newValue >= 1)
        {
            textComponent.color = Color.green; 
        }
        else
        {
            textComponent.color = Color.red; 
        }
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

