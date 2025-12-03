using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System.Xml;
using static GetSprite;

public class RegionPanel : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
   // public RegionInfo regionInfo;

    public GameObject prefab;
    public Transform scrollContent;
    public string playerEmpireName;

    public int ResourceType = 0; // food = 0, science = 1, politics = 2, gold = 3, faith = 4
    private float[] resourceTotal = new float[5];
    private float[] resourceParameter = new float[5];
    private float[] resourceGrowth = new float[5];
    private float[] nextResource = new float[5];
    private float[] taxResource = new float[5];
    private float[] nextTaxResource = new float[5];

    public Button starButton;

    public Button regionCloseButton;

    //  public List<Button> resourceButtons;
    public Button resourceButton;
    public List<Button> resourceButtons;
    public GameObject resourceButtonPanel;



    /* public Button foodResourceButton;
     public Button scienceResourceButton;
     public Button politicsResourceButton;
     public Button goldResourceButton;
     public Button faithResourceButton;*/

    public Scrollbar scrollbar;

    public Image CountryImage;


    public Sprite SToBSortSprite;
    public Sprite BToSSortSprite;
    public Sprite NoSortSprite;

    public List<int> isSort; 
     // 0 sortResourceButton
    // 1 parameter , 2 growth, 3 take
    // 4 total, 5 available, 6 recruite
    // 7 population growth . 8 support rat, 9 tax


    
    public List<Button> SortButton;

// 0 - 9
    /*    public Button sortResourceButton; 0
    public Button sortParameterButton; 1
    public Button sortGrowthButton; 2 
    public Button sortNextTakeButton; 3 
    public Button sortPopulationButton; 4
    public Button sortAvailablePopulationButton; 5
    public Button sortRecruitedButton; 6
    public Button sortPopulationGrowthRateButton; 7
    public Button sortSupporRateButton; 8
    public Button sortTaxRateButton;   9 */ 



    public List<Image> ResourceTypeImage; //0 is or total, 1 is parameter, 2 is growth , 3 is next take;
    public List<Sprite> foodResourceSprite; 
    public List<Sprite> scienceResourceSprite; 
    public List<Sprite> politicsResourceSprite; 
    public List<Sprite> goldResourceSprite; 
    public List<Sprite> faithResourceSprite; 


    private int currentResourceType;

    public List<RegionColumControl> regionColumControls = new List<RegionColumControl>();

    public RegionColumControl SelectedRegionColumControl;


    private Dictionary<int, float[]> lastResourceValues = new Dictionary<int, float[]>();

    private bool isMouseOverPanel = false;

    float moveCooldown = 0.2f;
    float lastMoveTime = 0f;

    bool isStar = false;

    Sprite selSprite ; // just this one look like better in Ui
    Sprite unSelSprite ;

    public Color32 selRegionColColor = new Color32(255,255,255,255);

    private bool isTypePanelOpen = false;

    private void Awake()
    {
        selSprite = Resources.Load<Sprite>("MyDraw/UI/Other/SettingBoxUnsel");
        unSelSprite = Resources.Load<Sprite>("MyDraw/UI/GameUI/Box");
    }



    private void Start()
    {


        ClearIsSort();
        if (regionCloseButton != null)
        {
            regionCloseButton.onClick.AddListener(() => TogglePanel());
        }


        InitResourceTypeButton();
        currentResourceType = ResourceType;

        SortButton[0].onClick.AddListener(() => SortRegionsBy(ResourceType, "Resource", 0));
        SortButton[1].onClick.AddListener(() => SortRegionsBy(ResourceType, "Parameter", 1));
        SortButton[2].onClick.AddListener(() => SortRegionsBy(ResourceType, "Growth", 2));
        SortButton[3].onClick.AddListener(() => SortRegionsBy(ResourceType, "Tax", 3));


        SortButton[4].onClick.AddListener(() => SortRegionsBy("Population", 4));
        SortButton[5].onClick.AddListener(() => SortRegionsBy("AvailablePopulation", 5));
        SortButton[6].onClick.AddListener(() => SortRegionsBy("Recruited", 6));
        SortButton[7].onClick.AddListener(() => SortRegionsBy("PopulationGrowthRate", 7));
        SortButton[8].onClick.AddListener(() => SortRegionsBy("SupportRate", 8));
        SortButton[9].onClick.AddListener(() => SortRegionsBy("TaxRate", 9));

        DisplayRegions();

        UpUI();

        InitStarButton();
    }

    void InitResourceTypeButton()
    {
        resourceButton.onClick.AddListener(SetResourceType);

        for (int i = 0; i < resourceButtons.Count; i++) {
            int index = i;
            resourceButtons[i].onClick.AddListener(() => SetResourceType(index));
        }
    }

    private void InitStarButton()
    {
        starButton.gameObject.GetComponent<Image>().sprite = UpStarButtonSprite(isStar);
        starButton.onClick.AddListener(OnStarButtonClick);
    }

    void OnStarButtonClick()
    {
        isStar = !isStar;
        starButton.gameObject.GetComponent<Image>().sprite = UpStarButtonSprite(isStar);
        UpdateStarDisplay();

    }

    void UpdateStarDisplay()
    {
        foreach (var regionColum in regionColumControls)
        {
            regionColum.SetStarActive(isStar);
        }
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        isMouseOverPanel = true;  
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isMouseOverPanel = false;  
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if ( isMouseOverPanel && Input.GetMouseButtonDown(1))
        {
            TogglePanel();
        }
    }

void MoveTheSelectedRegionColumControl(int num)
{
    if (SelectedRegionColumControl == null) {
            SetTheSelectedRegionColumControl(regionColumControls[0]);
            return;

    }

        int i = regionColumControls.IndexOf(SelectedRegionColumControl);
    
    if (i == -1) return; 
    i -= num;
    if (i < 0){i = regionColumControls.Count - 1;}
    else if (i >= regionColumControls.Count){i = 0;}

    SetTheSelectedRegionColumControl(regionColumControls[i]);
}


    void Update()
    {
        if (isMouseOverPanel && Input.GetKey(KeyCode.UpArrow) && Time.time - lastMoveTime > moveCooldown)
        {
            lastMoveTime = Time.time;
            MoveTheSelectedRegionColumControl(1);
        }


        if (isMouseOverPanel && Input.GetKey(KeyCode.DownArrow) && Time.time - lastMoveTime > moveCooldown)
        {
            lastMoveTime = Time.time;
            MoveTheSelectedRegionColumControl(-1);
        }

        ToggleTypePanel();

    }


    void ToggleTypePanel()
    {
        bool isHovering = IsMouseOverTypeUI();

        if (isHovering)
        {
            if (!isTypePanelOpen)
            {
                resourceButtonPanel.SetActive(true);
                isTypePanelOpen = true;
            }
        }
        else
        {
            if (isTypePanelOpen)
            {
                resourceButtonPanel.SetActive(false);
                isTypePanelOpen = false;
            }
        }
    }


    bool IsMouseOverTypeUI()
    {
        Vector2 mousePos = Input.mousePosition;

        bool overButton = RectTransformUtility.RectangleContainsScreenPoint(
            resourceButton.transform.parent.GetComponent<RectTransform>(),
            mousePos,
            null // Camera ?? null??? Screen Space - Overlay
        );

        bool overPanel = resourceButtonPanel.activeSelf && RectTransformUtility.RectangleContainsScreenPoint(
            resourceButtonPanel.GetComponent<RectTransform>(),
            mousePos,
            null
        );

        return overButton || overPanel;
    }



    void UpdateButtonIcons(int activeType){

        //NoSortSprite
        for (int i = 0; i < SortButton.Count ; i++){
            if (activeType != i) {
                isSort[i] = 0;
                SortButton[i].gameObject.GetComponent<Image>().sprite = NoSortSprite;
            } else if (activeType == i){
                if (isSort[i] == 0)SortButton[i].gameObject.GetComponent<Image>().sprite = SToBSortSprite;
                if (isSort[i] == 1)SortButton[i].gameObject.GetComponent<Image>().sprite = BToSSortSprite;
            }
        }

    }


    void SetResourceTypeImage(int type){

        if (type == 0) SetResourceTypeImageHelp(foodResourceSprite);
        if (type == 1) SetResourceTypeImageHelp(scienceResourceSprite);
        if (type == 2) SetResourceTypeImageHelp(politicsResourceSprite);
        if (type == 3) SetResourceTypeImageHelp(goldResourceSprite);
        if (type == 4) SetResourceTypeImageHelp(faithResourceSprite);
    }

    void SetResourceTypeImageHelp(List<Sprite> resourceSprite){
        resourceButton.gameObject.GetComponent<Image>().sprite = resourceSprite[0];
        for (int i = 0; i < ResourceTypeImage.Count ; i ++){
            ResourceTypeImage[i].sprite = resourceSprite[i];
        }

    }


    void UpUI()
    {
        CountryImage.sprite = GetCountryIcon();
    }

    public Sprite GetCountryIcon()
    {
        string iconPath = $"MyDraw/UI/Country/Icon/";
        switch (playerEmpireName)
        {
            case "Arab": return Resources.Load<Sprite>(iconPath + "Arab");
            case "Babylon": return Resources.Load<Sprite>(iconPath + "BabylonIcon");
            case "Carthage": return Resources.Load<Sprite>(iconPath + "CarthageIcon");
            case "Demon Kingdom": return Resources.Load<Sprite>(iconPath + "DemonKingdomIcon");
            case "Egypt": return Resources.Load<Sprite>(iconPath + "EgyptIcon");
            case "Kingdom of Englaland": return Resources.Load<Sprite>(iconPath + "Englaland(John)");
            case "East Romulus Empire": return Resources.Load<Sprite>(iconPath + "EREIcon");
            case "Kingdom of Francus": return Resources.Load<Sprite>(iconPath + "FrancusIcon");
            case "Holy Romulus Empire": return Resources.Load<Sprite>(iconPath + "HREIcon");
            case "Hungary": return Resources.Load<Sprite>(iconPath + "Hungary");
            case "State of the Knights Templar": return Resources.Load<Sprite>(iconPath + "JerusalemIcon");
            case "Kalmar Union": return Resources.Load<Sprite>(iconPath + "KalmarIcon");
            case "Macedonia": return Resources.Load<Sprite>(iconPath + "MacedoniaIcon");
            case "Ottoman Empire": return Resources.Load<Sprite>(iconPath + "OttomanIcon");
            case "Persian Empire": return Resources.Load<Sprite>(iconPath + "PersiaIcon");
            case "Polish-Lithuanian Commonwealth": return Resources.Load<Sprite>(iconPath + "PLIcon");
            case "Rus Principality": return Resources.Load<Sprite>(iconPath + "RusIcon");
            case "Sami": return Resources.Load<Sprite>(iconPath + "SamiIcon");
            case "Sky Fire Empire": return Resources.Load<Sprite>(iconPath + "SkyFireIcon");
            case "State of the Teutonic Order": return Resources.Load<Sprite>(iconPath + "TeutonicIcon");
            case "Scythia": return Resources.Load<Sprite>(iconPath + "ScythiaIcon");
            case "Greece": return Resources.Load<Sprite>(iconPath + "GreeceIcon");
            case "Demihuman Alliance": return Resources.Load<Sprite>(iconPath + "CelticIcon");
            case "Avalon": return Resources.Load<Sprite>(iconPath + "AvalonIcon"); ;
            case "Giant's Island": return Resources.Load<Sprite>(iconPath + "IrelandIcon");
            case "Military State of Naples": return Resources.Load<Sprite>(iconPath + "NaplesIcon");
            case "Kingdom of Sicily": return Resources.Load<Sprite>(iconPath + "SicilyIcon");


            default: return null;
        }
    }

    private void SetResourceType()
    {
        ResourceType = (ResourceType+1)%5;
        UpdateResourceType();
    }

    private void SetResourceType(int index)
    {
        ResourceType = index;
        UpdateResourceType();
    }

    void UpdateResourceType()
    {
        SetResourceTypeImage(ResourceType);
        SetResourceTypeToRegionColumControls();
        ClearResourceSort();

    }

    void SetResourceTypeToRegionColumControls(){
        for (int i = 0; i < regionColumControls.Count; i++)
        {
            regionColumControls[i].SetResourceType(ResourceType);
        }

    }




    void DisplayRegions()
    {
        // Step 1: Find all regions with the player's empire name
        //   Region[] foundRegionObjects = GameObject.FindObjectsOfType<Region>();
        List<RegionValue> allRegionValues = GameValue.Instance.GetAllRegionValues();
        regionColumControls.Clear();


        // Step 3: Iterate through all found regions and create or update the UI for them
        foreach (var regionObject in allRegionValues)
        {
           // RegionValue region = regionObject.GetRegionValue();

            if (regionObject != null)
                //&& region.GetCountry() == playerEmpireName)
            {

                // Step 4: Instantiate new region prefab
                GameObject regionPrefab = Instantiate(prefab, scrollContent);

                // Step 5: Get RegionColumControl component from the prefab
                RegionColumControl regionColumControl = regionPrefab.GetComponent<RegionColumControl>();

                if (regionColumControl != null)
                {
                    // Step 6: Set the region's data to the RegionColumControl component
                    regionColumControl.RegionColumControlP(ResourceType, regionObject);
                    regionColumControl.SetRegionPanel(this); 

                    regionColumControls.Add(regionColumControl);  // Add to the list for sorting

                }

            }
        }

        scrollbar.value = 1;
        // Step 8: Force layout rebuild to correctly display content
        LayoutRebuilder.ForceRebuildLayoutImmediate(scrollContent.GetComponent<RectTransform>());
    }

    public void SetTheSelectedRegionColumControl(RegionColumControl regionColumControl){
        if (SelectedRegionColumControl != null){
            SelectedRegionColumControl.rowImage.color = new Color32(144, 144, 144, 80);
            ;
        }
        SelectedRegionColumControl = regionColumControl;
        SelectedRegionColumControl.rowImage.color = selRegionColColor;
    }

    // This function will toggle the panel visibility

    public void ShowRegionPanel()
    {
        gameObject.SetActive(true);

        SetResourceTypeImage(ResourceType);
    }


    public void TogglePanel()
    {
        ClearIsSort();
        gameObject.SetActive(false);
    
    }


    void SortRegionsBy(int resourceType, string criterion, int activeType)
    {

        bool isDescending = isSort[activeType] == 0; 
        regionColumControls.Sort((a, b) =>
        {
            float aValue = 0;
            float bValue = 0;

            switch (criterion)
            {
                case "Resource":
                    aValue = a.GetResourceValue(resourceType);
                    bValue = b.GetResourceValue(resourceType);
                    break;

                case "Parameter":
                    aValue = a.GetParameterValue(resourceType);
                    bValue = b.GetParameterValue(resourceType);
                    break;

                case "Growth":
                    aValue = a.GetGrowthValue(resourceType);
                    bValue = b.GetGrowthValue(resourceType);
                    break;

                case "Tax":
                    aValue = a.GetTaxValue(resourceType);
                    bValue = b.GetTaxValue(resourceType);
                    break;
            }

            return isDescending ? bValue.CompareTo(aValue) : aValue.CompareTo(bValue);
        });

        isSort[activeType] = 1 - isSort[activeType];

        UpdateButtonIcons(activeType);

        UpdateRegionDisplay();
        LayoutRebuilder.ForceRebuildLayoutImmediate(scrollContent.GetComponent<RectTransform>());
    }



    void SortRegionsBy(string criterion, int activeType)
    {

        bool isDescending = isSort[activeType] == 0;
        
        regionColumControls.Sort((a, b) =>
        {
            float aValue = 0;
            float bValue = 0;

            switch (criterion)
            {
                case "Population":
                    aValue = a.GetRegionValue().GetRegionPopulation();
                    bValue = b.GetRegionValue().GetRegionPopulation();
                    break;

                case "AvailablePopulation":
                    aValue = a.GetRegionValue().GetRegionAvailablePopulation();
                    bValue = b.GetRegionValue().GetRegionAvailablePopulation();
                    break;

                case "Recruited":
                    aValue = a.GetRegionValue().GetRegionRecruitedPopulation();
                    bValue = b.GetRegionValue().GetRegionRecruitedPopulation();
                    break;

                case "PopulationGrowthRate":
                    aValue = a.GetRegionValue().GetPopulationGrowth();
                    bValue = b.GetRegionValue() .GetPopulationGrowth();
                    break;

                case "SupportRate":
                    aValue = a.GetRegionValue().GetRegionSupportRate();
                    bValue = b.GetRegionValue().GetRegionSupportRate();
                    break;

                case "TaxRate":
                    aValue = a.GetRegionValue().GetTaxRate();
                    bValue = b.GetRegionValue().GetTaxRate();
                    break;
            }

            return isDescending ? bValue.CompareTo(aValue) : aValue.CompareTo(bValue);
        });
        isSort[activeType] = 1 - isSort[activeType];
        UpdateButtonIcons(activeType);

        UpdateRegionDisplay();
    }

    void UpdateRegionDisplay()
    {

        for (int i = 0; i < regionColumControls.Count; i++)
        {
            var regionColumControl = regionColumControls[i];
            regionColumControl.transform.SetSiblingIndex(i);
        }

    }



    void ClearIsSort(){
        for (int i = 0 ; i < isSort.Count ; i++){
            isSort[i] = 0;
            SortButton[i].gameObject.GetComponent<Image>().sprite = NoSortSprite;
        }
   
    }

    void ClearResourceSort(){
        for (int i = 0 ; i < 4 ; i++){
            isSort[i] = 0;
            SortButton[i].gameObject.GetComponent<Image>().sprite = NoSortSprite;
        }
    }


    public void ShowRegionInfo(RegionValue regionValue)
    {
        //   regionInfo.ShowPanel(region);
        // gameObject.SetActive(false);
        regionValue.region.ShowRegionPanel();
    }

}

