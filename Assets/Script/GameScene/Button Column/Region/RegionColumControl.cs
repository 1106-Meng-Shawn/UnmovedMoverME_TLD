using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using static FormatNumber;
using static GetSprite;
using static GetColor;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization;
using static RegionColumControl;
using UnityEngine.Events;
using Unity.VisualScripting;


public class RegionColumControl : MonoBehaviour,IDragHandler, IEndDragHandler,IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Image regionIcon;


    public Image lordIcon;
    public Sprite noLord;


    public TMP_Text regionNameText;
    public TMP_Text regionPopulationText;
    public TMP_Text regionAvailablePopulationText;
    public TMP_Text regionRecruitedText;
    public TMP_Text regionPopulationGrowthText;
    public TMP_Text regionSupportRateText;
    public TMP_Text regionTaxRateText;

    public TMP_Text nextPopulationText;
    public TMP_Text regionTotalText;
    public TMP_Text regionParameterText;
    public TMP_Text regionGrowthText;
    public TMP_Text regionNextText;
    public TMP_Text regionTaxText;

    public int ResourceType = 0; // food = 0, science = 1, politics = 2, gold = 3, faith = 4


    private RegionValue regionValue;

   // public RegionValue Region => region.Value;


    public string regionName;
    private int Count;

    public Image resourceTypeImage;

 //   public List<Sprite> resourceTypeSprite;

    private RegionPanel regionPanel;
    public Image rowImage;

    private float lastClickTime = 0f;
    private const float doubleClickThreshold = 0.3f;

    private string[] resourceTypeNames = { "Food", "Science", "Politics", "Gold", "Faith" };

    private GameValue gameValue;
    [Header("Region Col Buttons")]
    public RegionRowButton regionRowButton;
    [System.Serializable]
    public class RegionRowButton
    {
        public Button starButton;

        public Button RecruitedPlusButton;
        public Button RecruitedMinsButton;

        public Button TaxRatePlusButton;
        public Button TaxRateMinsButton;

        public Button TotalResourcePlusButton;
        public Button TotalResourceMinsButton;

        public Button ParameterResourcePlusButton;
        public Button GrowthResourcePlusButton;

        public RegionColumControl regionColumControl;

        public GameObject feedbackPrefab;   
    }




    public void SetRegionPanel(RegionPanel regionPanel){
        this.regionPanel = regionPanel;
    }

    void OnEnable()
    {
        LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
        if (regionValue != null) UpdateFromRegion();
    }

    void OnDisable()
    {
        LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;
    }



    private void OnLocaleChanged(Locale locale)
    {
        SetRegionName(); // ???? locale.Identifier ??????
    }


    void Awake()
    {
        if (regionValue != null)
        {
            regionValue.OnValueChanged += UpdateFromRegion;
        }
    }

    void OnDestroy()
    {
        if (regionValue != null)
        {
            regionValue.OnValueChanged -= UpdateFromRegion;
        }

    }

    // Start is called before the first frame update
    void Start()
    {
        gameValue = GameValue.Instance;
        resourceTypeImage.sprite = GetValueSprite(resourceTypeNames[ResourceType]);
        rowImage = GetComponent<Image>();
        // rowImage.color = Color.gray;// need to change or delet;
        rowImage.color = new Color32(144, 144, 144, 80);

        SetRegionName();
        UpdateFromRegion();

        InitButtons();

    }

    void UpdateFromRegion()
    {
        UpdateRegionPrefab(regionValue); 
    }

    void InitButtons()
    {
            BindButton(regionRowButton.RecruitedPlusButton, () => RecruitedIncrease(100));
            BindButton(regionRowButton.RecruitedMinsButton, () => RecruitedDecrease(100));
            BindButton(regionRowButton.TaxRatePlusButton, () => taxRateIncrease());
            BindButton(regionRowButton.TaxRateMinsButton, () => taxRateDecrease());

            Action increaseAction = null;
            Action decreaseAction = null;
            Action parameterAction = null;
            Action growthAction = null;

            switch (ResourceType)
            {
                case 0:
                    increaseAction = () => foodIncrease();
                    decreaseAction = () => foodDecrease();
                    parameterAction = () => foodParameterChange();
                    growthAction = () => foodGrowthChange();
                    break;
                case 1:
                    increaseAction = () => scienceIncrease();
                    decreaseAction = () => scienceDecrease();
                    parameterAction = () => scienceParameterChange();
                    growthAction = () => scienceGrowthChange();
                    break;
                case 2:
                    increaseAction = () => politicsIncrease();
                    decreaseAction = () => politicsDecrease();
                    parameterAction = () => politicParameterChange();
                    growthAction = () => politicGrowthChange();
                    break;
                case 3:
                    increaseAction = () => goldIncrease();
                    decreaseAction = () => goldDecrease();
                    parameterAction = () => goldParameterChange();
                    growthAction = () => goldGrowthChange();
                    break;
                case 4:
                    increaseAction = () => faithIncrease();
                    decreaseAction = () => faithDecrease();
                    parameterAction = () => faithParameterChange();
                    growthAction = () => faithGrowthChange();
                    break;
            }

            BindButton(regionRowButton.TotalResourcePlusButton, increaseAction);
            BindButton(regionRowButton.TotalResourceMinsButton, decreaseAction);
            BindButton(regionRowButton.ParameterResourcePlusButton, parameterAction);
            BindButton(regionRowButton.GrowthResourcePlusButton, growthAction);


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
        UpStarButtonSpriteInRow();
        regionRowButton.starButton.onClick.AddListener(OnStarButtonClick);
    }

    void OnStarButtonClick()
    {
        regionValue.IsStar = !regionValue.IsStar;
        UpStarButtonSpriteInRow();

    }

    

    void UpStarButtonSpriteInRow()
    {
        regionRowButton.starButton.gameObject.GetComponent<Image>().sprite = UpStarButtonSprite(regionValue.IsStar);
    }


    void BindButton(Button button, Action action)
    {
        if (button == null || action == null) return;

        button.onClick.AddListener(() => action());

        HoldButton holdButton = button.GetComponent<HoldButton>();
        if (holdButton != null)
        {
            holdButton.onHoldClick.AddListener(() => action());
        }
    }


    // This method is used to set up the region and resource types
    public void RegionColumControlP( int type, RegionValue value)
    {
        ResourceType = type;
        regionValue = value;

        resourceTypeImage.sprite = GetValueSprite(resourceTypeNames[ResourceType]);

        if (value != null)
        {
            UpdateRegionPrefab(value);

        }
    }

    public void SetResourceType(int type){
        ResourceType = type;
        resourceTypeImage.sprite = GetValueSprite(resourceTypeNames[ResourceType]);

    }


    public RegionValue GetRegionValue()
    {
        return regionValue;
    }

    public float GetResourceValue(int type)
    {
     //   for (int i = 0; i < Region.)

        return regionValue.GetRegionResourceSurplus(type);
    }



    void SetRegionName()
    {
        regionNameText.text = regionValue.GetRegionName();

    }


    // Updates the region UI prefab with current region data
    public void UpdateRegionPrefab(RegionValue region)
    {
        // Update the basic information
        //    regionNOText.text = Count.ToString(); may be delte it

        //  profile.sprite = region.lord == null ? noLord : region.lord.icon;


        Debug.Log("UpdateRegionPrefab work");

        SetLordIconSprite();

        SetRegionName();

        regionIcon.sprite = region.GetRegionIcon();

     /*   regionPopulationText.text = FormatNumberToString(region.GetPopulation());
        regionAvailablePopulationText.text = FormatNumberToString(region.GetAvailablePopulation());
        regionRecruitedText.text = FormatNumberToString(region.GetRecruitedPopulation());
        regionPopulationGrowthText.text = region.GetPopulationGrowthString();
        regionSupportRateText.text = region.GetSupportRateString();
        regionTaxRateText.text =  region.GetTaxRateString();

        // Calculate and update next population
        float nextPopulation = Mathf.Max(2, region.GetPopulationGrowth() * (1 + region.GetPopulationGrowth()));
        nextPopulationText.text = FormatNumberToString(nextPopulation);
        nextPopulationText.color = nextPopulation < region.GetPopulation() ? Color.red : Color.green;

        // Update resource information
        regionParameterText.text = GetColorString(region.GetParameterWithLordString(ResourceType), 1, ResourceType);
        regionTotalText.text = GetColorString(region.GetResourceSurplusString(ResourceType), 1, ResourceType); 
        regionGrowthText.text = region.GetResourceGrowthString(ResourceType);
        regionNextText.text = region.GetResourceNextString(ResourceType);*/

        // Set the next resource text color based on the values

        // Update tax resource text and its color
        regionTaxText.text = region.GetTaxRateString();
        // regionTaxText.color = taxResource[ResourceType] > nextTaxResource[ResourceType] ? Color.red : Color.green;

        UpStarButtonSpriteInRow();
    }

    void SetLordIconSprite()
    {
        if (regionValue.GetLord() == null)
        {
            lordIcon.raycastTarget = false;
            lordIcon.sprite = noLord;
            lordIcon.gameObject.GetComponent<IntroPanelShow>().SetCharacter(null);

        }
        else
        {
            lordIcon.raycastTarget = true;
            lordIcon.sprite = regionValue.GetLord().icon;
            lordIcon.gameObject.GetComponent<IntroPanelShow>().SetCharacter(regionValue.GetLord());


        }

    }


    // food = 0, science = 1, politics = 2, gold = 3, faith = 4


    // Get resource values based on the type

    public float GetParameterValue(int ResourceType)
    {
        return 0;
    }

    public float GetGrowthValue(int ResourceType)
    {
        return 0;
    }

    public float GetTaxValue(int ResourceType)
    {
        return 0;
    }



    private GameObject draggedIcon;


    public void OnPointerDown(PointerEventData eventData)
    {
        // =============== ???????? ===============
        if (isMouseOverPanel && eventData.button == PointerEventData.InputButton.Right)
        {
            regionPanel.TogglePanel();
            return;
        }

        // =============== ?????? ===============
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            float timeSinceLastClick = Time.time - lastClickTime;
            lastClickTime = Time.time;

            if (timeSinceLastClick < doubleClickThreshold)
            {
                HandleDoubleClick();
                return;
            }
        }

        // =============== ???????? ===============
        regionPanel.SetTheSelectedRegionColumControl(this);

        if (regionValue.lord == null || eventData.button != PointerEventData.InputButton.Left)
            return;

        // ??????????????
        if (draggedIcon != null)
        {
            Destroy(draggedIcon);
            draggedIcon = null;
        }

        // ??????
        CreateDraggedIcon(regionValue.lord.icon);
    }

    private void HandleDoubleClick()
    {
        // Debug.Log("Double click detected!");
        regionPanel.ShowRegionInfo(regionValue);
    }

    private void CreateDraggedIcon(Sprite sprite)
    {
        draggedIcon = new GameObject("DraggedIcon");
        draggedIcon.transform.SetParent(transform.root);

        Image draggedImage = draggedIcon.AddComponent<Image>();
        draggedImage.sprite = sprite;
        draggedImage.raycastTarget = false;

        RectTransform rect = draggedImage.rectTransform;
        rect.sizeDelta = new Vector2(0.5f, 0.5f);
        rect.pivot = rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);

        Vector3 offset = new Vector3(20, 20, 0);
        rect.position = Input.mousePosition + offset;
    }



public void OnPointerUp(PointerEventData eventData)
    {
         Destroy(draggedIcon);
         draggedIcon = null;
    }


        public void OnDrag(PointerEventData eventData)
    {

        if (draggedIcon != null)
        {
            RectTransform draggedIconRect = draggedIcon.GetComponent<RectTransform>();
            RectTransform parentRect = draggedIcon.transform.parent.GetComponent<RectTransform>();

            Vector2 localPoint;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parentRect,
                eventData.position,
                null,  // ? ??? null?
                out localPoint))
            {
                draggedIconRect.localPosition = localPoint;

                // ?????????????
                draggedIconRect.localScale = new Vector3(250, 250, 1);
            }
        }


    }


    public void OnEndDrag(PointerEventData eventData)
    {
        if (draggedIcon != null)
        {
            var iconCanvasGroup = draggedIcon.GetComponent<CanvasGroup>();
            if (iconCanvasGroup != null)
                iconCanvasGroup.blocksRaycasts = false;

            Destroy(draggedIcon);
            draggedIcon = null;
        }

        if (regionValue.lord == null) return;

        GameObject targetObject = eventData.pointerEnter;

        RegionColumControl regionColumControl = targetObject?.GetComponent<RegionColumControl>()
            ?? targetObject?.GetComponentInParent<RegionColumControl>();

        if (regionColumControl != null)
        {
            if (regionColumControl == this) return;

            RegionValue temp = regionColumControl.regionValue;

            if (temp.lord != null)
                temp.SwitchLord(regionValue);
            else
                temp.SetLord(regionValue.lord);

            return;
        }

        regionValue.MoveLord(); // ??????????
    }



    public void SetStarActive(bool isStarred)
    {
        if (isStarred && !regionValue.IsStar)
        {
            gameObject.SetActive(false);
        } else
        {
            gameObject.SetActive(true);
        }
    }



    private bool isMouseOverPanel;
    public void OnPointerEnter(PointerEventData eventData)
    {
        isMouseOverPanel = true;  
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isMouseOverPanel = false;  
    }




    #region Row Button //need to big fix to change,because it is old, and change the play

    private void RemoveButtonListeners()
    {
        regionRowButton.TotalResourcePlusButton.onClick.RemoveAllListeners();
        regionRowButton.TotalResourceMinsButton.onClick.RemoveAllListeners();
        regionRowButton.ParameterResourcePlusButton.onClick.RemoveAllListeners();
        regionRowButton.GrowthResourcePlusButton.onClick.RemoveAllListeners();

        RemoveHoldButtonListener(regionRowButton.TotalResourcePlusButton);
        RemoveHoldButtonListener(regionRowButton.TotalResourceMinsButton);
        RemoveHoldButtonListener(regionRowButton.ParameterResourcePlusButton);
        RemoveHoldButtonListener(regionRowButton.GrowthResourcePlusButton);
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

            popNotification($"Your {resourceName} resouce is not enough"); return;

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

            popNotification($"{regionValue.GetRegionName()}'s {resourceName} resouce is not enough"); return;

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

        popNotification($"Your politics resouce is not enough");

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


    public void GrowthChange(string resourceName, ref float resourceAmount, float resourceCost, ref float growthParameter, float divisor)
    {

     /*   ModifyValuesForCtrl(ref resourceCost, 10);
        ModifyValuesForCtrl(ref divisor, 0.1f);

        if (resourceAmount >= resourceCost)
        {
            resourceAmount -= resourceCost;
            growthParameter += 10 / divisor;
            float randomIncrement = System.Random.Range(-1 / divisor, 0f);
            growthParameter += randomIncrement;


            growthParameter = (float)Mathf.Round(growthParameter * 10000) / 10000f;

        }

        popNotification($"Your {resourceName} resouce is not enough"); return;
     */
    }

    public void foodParameterChange()
    {
        //   ParameterChange("food",ref gameValue.politics, 1000, ref regionValue.foodParameter, 0.1f);
    }

    public void foodGrowthChange()
    {
      /*  float nextTurnfood = (Region.GetResourceSurplus(0) + ((regionValue.GetPopulation() - regionValue.GetRecruitedPopulation()) * regionValue.GetResourceParameter(0) * (1 + regionValue.GetResourceGrowth(0)) * (1 - regionValue.GetTaxRate())))
                             - regionValue.GetPopulation() * (1 + regionValue.GetPopulationGrowth());
        if (nextTurnfood <= 1000)
        {
            nextTurnfood = 1000;
        }*/
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

    public void taxRateDecrease()
    {
        /*







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

    }

    private void RecruitedIncrease(int num)
    {
     /*   if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        {
            num *= 10;  // If Control key is held, increase the num by a factor of 10
        }

        if (regionValue == null) return;  // Ensure regionValue is not null

        if (regionValue.GetResourceSurplus(0) <= 0) { popNotification($"Not enough food in the {regionValue.GetRegionName()}!"); return; }
        if (regionValue.GetRecruitedPopulation() == regionValue.GetPopulation()) { popNotification($"The number of recruits in {regionValue.GetRegionName()} is already at the maximum!"); return; }


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
                    gameValue.totalRecruitedPopulation += recruitAmount;
                    //    regionValue.RecruitedPopulation = regionValue.GetAvailablePopulation();  // Set to AvailablePoulation
                    //  regionValue.foodSurplus -= recruitAmount;
                }
                else
                {
                    // If there is no overflow, continue recruiting normally
                    gameValue.totalRecruitedPopulation += num;
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
                    gameValue.totalRecruitedPopulation += recruitAmount;
                    //  regionValue.RecruitedPopulation = regionValue.GetAvailablePopulation();  // Set to AvailablePoulation
                    //  regionValue.foodSurplus -= recruitAmount;
                }
                else
                {
                    // If there is no overflow, continue recruiting normally
                    gameValue.totalRecruitedPopulation += num;
                    //  regionValue.RecruitedPopulation += num;
                    //  regionValue.foodSurplus -= num;
                }

            }
        }
        else if (num > regionValue.GetResourceSurplus(0) && regionValue.GetResourceSurplus(0) > 0)
        {
            // If food surplus is less than num, recruit as much as the food surplus allows
            int recruitAmount = (int)(regionValue.GetResourceSurplus(0));
            gameValue.totalRecruitedPopulation += recruitAmount;
            //   regionValue.RecruitedPopulation += recruitAmount;
            //  regionValue.foodSurplus -= recruitAmount;
        }*/
    }

    private void RecruitedDecrease(int num)
    {
        /*if (regionValue == null) return;  // Ensure regionValue is not null

        if (regionValue.GetRecruitedPopulation() <= 0) { popNotification($"The number of recruits in {regionValue.GetRegionName()} is already at the minimum!"); return; }
        if (gameValue.totalRecruitedPopulation <= 0) { popNotification("The total number of recruits is not enough!"); return; }


        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        {
            num *= 10;  // If Control key is held, increase the num by a factor of 10
        }



        // Ensure there is enough food and recruited population to decrease
        if (gameValue.totalRecruitedPopulation > 0)
        {
            // Check if num can be reduced without going below zero in RecruitedPopulation
            int reduceAmount = Mathf.Min(num, regionValue.GetRecruitedPopulation());

            // Reduce recruited population and update the total recruited population
            gameValue.totalRecruitedPopulation -= reduceAmount;

            //            regionValue.RecruitedPopulation -= reduceAmount;

            // Increase the food surplus by the reduced amount
            // regionValue.foodSurplus += reduceAmount;

            // Ensure RecruitedPopulation doesn't go below zero
            //     regionValue.RecruitedPopulation = Mathf.Max(0, regionValue.RecruitedPopulation);

        }*/


    }



    private void popNotification(string content)
    {
       /* GameObject feedbackObject = Instantiate(feedbackPrefab, Vector3.zero, Quaternion.identity);
        MessageFeedback feedback = feedbackObject.GetComponent<MessageFeedback>();

        feedback.ShowMessageAtTop(content);
        return;
       */
    }


    #endregion

}
