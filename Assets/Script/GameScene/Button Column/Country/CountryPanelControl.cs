using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Video;

using static GetString;

public class CountryPanelControl : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    public static CountryPanelControl Instance { get; private set; }
    public GameObject CountryPanel;
    public GameValue gameValue;
    public Button FaithTransformAll;
    public Button FaithTransformfood;
    public Button FaithTransformScience;
    public Button FaithTransformPolitics;
    public Button FaithTransformGold;

    public TMP_Text getRecruit;
    public TMP_Text costFaith;
    public Slider recruitByfaith;
    public Button check;
    public TMP_Text MaxGetRecruit;

    public Button countryCloseButton;

    public GameObject feedbackPrefab;

    public Animation animation;
    public Button LeftImageButton;
    private bool isHide = false;

    [Header("Electoral State Buttons")]
    public ElectoralButtons electoralButtons;
    string iconPath = "MyDraw/UI/CountryPanel/CountryPanel";

    [System.Serializable]
    public class ElectoralButtons
    {
        public Button TeirerButton;
        public Button CologneButton;
        public Button MainzButton;
        public Button PlatinateButton;
        public Button SaxonyButton;
        public Button BrandenburgButton;
        public Button BohemiaButton;
    }

    [Header("Reichskleinodien Buttons")]
    public Reichskleinodien reichskleinodien;
    [System.Serializable]
    public class Reichskleinodien
    {
        public Button regionButton;
        public Button electorButton;
        public Button recruitButton;
        public Button characterButton;
        public Button countryButton;
        public Button itemButton;
        public ButtonEffect recruitButtonEffect;
    }

    private int lastClickedElectorIndex = -1;  
    private float lastClickTime = 0f;
    private float doubleClickThreshold = 0.4f;

    [Header("Other")]
    public BottomButton bottomButton;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        countryCloseButton.onClick.AddListener(() => ClosePanel());

     /*   FaithTransformAll.onClick.AddListener(OnFaithTransformAll);
        FaithTransformfood.onClick.AddListener(OnFaithTransformFood);
        FaithTransformScience.onClick.AddListener(OnFaithTransformScience);
        FaithTransformPolitics.onClick.AddListener(OnFaithTransformPolitics);
        FaithTransformGold.onClick.AddListener(OnFaithTransformGold);

        recruitByfaith.onValueChanged.AddListener(OnSliderValueChanged);

        check.onClick.AddListener(OnCheckButtonClicked);

        UpdateMaxGetRecruit();*/

        InitElectoralStateButtons();
        //GameValue.Instance.PlayerRegions.OnListChanged += UpdateElectorButtonsByRegion;
        GameValue.Instance.RegisterPlayerRegionsChanged(UpdateElectorButtonsByRegion);
        UpdateElectorButtonsByRegion();


        InitReichskleinodienButtons();
        GameValue.Instance.RegisterItemsChange(UpdateInitReichskleinodienButtonsByItem);
        UpdateInitReichskleinodienButtonsByItem();
       // gameValue = GameValue.Instance;

        InitAnimationButton();
    }

    void OnDestroy()
    {
        if (GameValue.Instance != null)
            GameValue.Instance.UnRegisterPlayerRegionsChanged(UpdateElectorButtonsByRegion);
    }

    void InitAnimationButton()
    {
        LeftImageButton.onClick.AddListener(OnLeftImageButtonClick);
    }

    void OnLeftImageButtonClick()
    {
        isHide = !isHide;
        string iconPath = $"MyDraw/UI/GameUI/";
       if (isHide)
       {
            bool result = animation.Play("LeftImageHide");
            LeftImageButton.GetComponent<Image>().sprite = Resources.Load<Sprite>(iconPath + "RightColShow");
            LeftImageButton.GetComponent<ButtonEffect>().SetChangeSprite(Resources.Load<Sprite>(iconPath + "RightColShow"), Resources.Load<Sprite>(iconPath + "RightColUnShow"));
       }
       else
       {
            bool result = animation.Play("LeftImageShow");
            LeftImageButton.GetComponent<Image>().sprite = Resources.Load<Sprite>(iconPath + "RightColUnClose");
            LeftImageButton.GetComponent<ButtonEffect>().SetChangeSprite(Resources.Load<Sprite>(iconPath + "RightColClose"), Resources.Load<Sprite>(iconPath + "RightColUnClose"));

        }
    }


    void InitElectoralStateButtons()
    {
        electoralButtons.TeirerButton.onClick.AddListener(() => OnElectorClicked(RegionValueConstants.TrierID));
        electoralButtons.CologneButton.onClick.AddListener(() => OnElectorClicked(RegionValueConstants.CologneID));
        electoralButtons.MainzButton.onClick.AddListener(() => OnElectorClicked(RegionValueConstants.MainzID));
        electoralButtons.PlatinateButton.onClick.AddListener(() => OnElectorClicked(RegionValueConstants.PlatinateID));
        electoralButtons.SaxonyButton.onClick.AddListener(() => OnElectorClicked(RegionValueConstants.SaxonyID));
        electoralButtons.BrandenburgButton.onClick.AddListener(() => OnElectorClicked(RegionValueConstants.BrandenburgID));
        electoralButtons.BohemiaButton.onClick.AddListener(() => OnElectorClicked(RegionValueConstants.BohemiaID));
    }

    void UpdateElectorButtonsByRegion()
    {
        electoralButtons.TeirerButton.gameObject.SetActive(HasRegion(RegionValueConstants.TrierID));
        electoralButtons.CologneButton.gameObject.SetActive(HasRegion(RegionValueConstants.CologneID));
        electoralButtons.MainzButton.gameObject.SetActive(HasRegion(RegionValueConstants.MainzID));
        electoralButtons.PlatinateButton.gameObject.SetActive(HasRegion(RegionValueConstants.PlatinateID));
        electoralButtons.SaxonyButton.gameObject.SetActive(HasRegion(RegionValueConstants.SaxonyID));
        electoralButtons.BrandenburgButton.gameObject.SetActive(HasRegion(RegionValueConstants.BrandenburgID));
        electoralButtons.BohemiaButton.gameObject.SetActive(HasRegion(RegionValueConstants.BohemiaID));
    }


    void UpdateInitReichskleinodienButtonsByItem()
    {
        reichskleinodien.regionButton.gameObject.SetActive(HasItem(ItemConstants.ReichszepterID));
        reichskleinodien.electorButton.gameObject.SetActive(HasItem(ItemConstants.KronungsevangeliarID));
        RecruitSet();
        reichskleinodien.countryButton.gameObject.SetActive(HasItem(ItemConstants.ReichskroneID));
        CharacterSet();
        ItemSet();
    }


    void RecruitSet()
    {

        if (HasItem(ItemConstants.GlovesID))
        {
            ItemBase item = GameValue.Instance.GetItem(ItemConstants.GlovesID);

            Sprite UnselSprite = Resources.Load<Sprite>(iconPath + "GlovesUnsel");
           Sprite SelSprite = Resources.Load<Sprite>(iconPath + "GlovesSel");

            reichskleinodien.recruitButtonEffect.SetChangeSprite(SelSprite, UnselSprite);

        }
        else
        {
            reichskleinodien.characterButton.gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>(iconPath + "CommonSword");

            Sprite UnselSprite = Resources.Load<Sprite>(iconPath + "AppointmentGlovesUnsel");
            Sprite SelSprite = Resources.Load<Sprite>(iconPath + "AppointmentGlovesSel");

            reichskleinodien.recruitButtonEffect.SetChangeSprite(SelSprite, UnselSprite);


        }

    }



    void CharacterSet()
    {

        if (HasItem(ItemConstants.ZeremonienschwertID))
        {
            reichskleinodien.characterButton.gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>(iconPath + "Zeremonienschwert");

        }
        else
        {
            reichskleinodien.characterButton.gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>(iconPath + "CommonSword");

        }

    }

    void ItemSet()
    {

        if (HasItem(ItemConstants.ReichsapfelID)) {

            reichskleinodien.itemButton.gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>(iconPath + "Reichsapfel");

        }
        else
        {
            reichskleinodien.itemButton.gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>(iconPath + "EmptyItemClose");

        }

    }



    bool HasItem(int itemID)
    {
        return GameValue.Instance.HasItem(itemID);
    }



    bool HasRegion(int regionID)
    {
        var regions = GameValue.Instance.GetPlayerRegions();
        if (regions == null) return false;
        return regions.Any(r => r.GetRegionID() == regionID);
    }


    void OnElectorClicked(int index)
    {
        if (lastClickedElectorIndex == index && Time.time - lastClickTime < doubleClickThreshold)
        {
            // ?????????????/??
            Region region = GameValue.Instance.GetRegionValue(index).region;
            region.ZoomToCity(0);
            region.ShowRegionPanel(0); // ??????
            lastClickedElectorIndex = -1; // ??
        }
        else
        {
            // ??????????
            Region region = GameValue.Instance.GetAllRegionValues()[index].region;
            region.ShowRegionPanel();

            lastClickedElectorIndex = index;
            lastClickTime = Time.time;
        }
    }

    void InitReichskleinodienButtons()
    {
        reichskleinodien.regionButton.onClick.AddListener(() => bottomButton.TogglePanelByKey("Region"));
        reichskleinodien.electorButton.onClick.AddListener(() => bottomButton.TogglePanelByKey("Elector"));
        reichskleinodien.recruitButton.onClick.AddListener(() => bottomButton.TogglePanelByKey("Recruit"));
        reichskleinodien.characterButton.onClick.AddListener(() => bottomButton.TogglePanelByKey("Character"));
        reichskleinodien.countryButton.onClick.AddListener(() => bottomButton.TogglePanelByKey("Country"));
        reichskleinodien.itemButton.onClick.AddListener(() => bottomButton.TogglePanelByKey("Item"));

    }


    private bool isMouseOverPanel = false; 

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
        if ( isMouseOverPanel  && Input.GetMouseButtonDown(1))
        {
            ClosePanel();
        }
    }
    public void ClosePanel()
    {
        CountryPanel.SetActive(false);
    }


    public void ShowPanel()
    {
        CountryPanel.SetActive(true);
        UpdateElectorButtonsByRegion();
        UpdateInitReichskleinodienButtonsByItem();
    }


    private int maxRecruit;

    private void Update()
    {
        if (gameValue == null) gameValue = GameObject.FindObjectOfType<GameValue>();
        if (Mathf.FloorToInt(gameValue.GetResourceValue().Faith / 100) != maxRecruit) UpdateMaxGetRecruit();
    }

    private void OnFaithTransformAll()
    {
        if (gameValue.GetResourceValue().Faith >= 5000)
        {
            gameValue.GetResourceValue().Faith -= 5000;
            gameValue.GetResourceValue().Food += 1000;
            gameValue.GetResourceValue().Science += 1000;
            gameValue.GetResourceValue().Politics += 1000;
            gameValue.GetResourceValue().Gold += 1000;

            UpdateUI();
        }
        else
        {
            NotificationManage.Instance.ShowAtTopByKey(NotificationKeyConstants.NotEnoughResource, GetResourceStringWithSprite(ValueType.Faith));
        }
    }

    private void OnFaithTransformFood()
    {
        if (gameValue.GetResourceValue().Faith >= 5000)
        {
            gameValue.GetResourceValue().Faith -= 5000;
            gameValue.GetResourceValue().Food += 3000;

            UpdateUI();
        }
        else
        {
            // NotificationManage.Instance.ShowToTop("Not enough faith!");
            NotificationManage.Instance.ShowAtTopByKey(NotificationKeyConstants.NotEnoughResource, GetResourceStringWithSprite(ValueType.Faith));
        }
    }

    private void OnFaithTransformScience()
    {
        if (gameValue.GetResourceValue().Faith >= 5000)
        {
            gameValue.GetResourceValue().Faith -= 5000;
            gameValue.GetResourceValue().Science += 3000;

            UpdateUI();
        }
        else
        {
            NotificationManage.Instance.ShowAtTopByKey(NotificationKeyConstants.NotEnoughResource, GetResourceStringWithSprite(ValueType.Faith));
        }
    }

    private void OnFaithTransformPolitics()
    {
        if (gameValue.GetResourceValue().Faith >= 5000)
        {
            gameValue.GetResourceValue().Faith -= 5000;
            gameValue.GetResourceValue().Politics += 3000;

            UpdateUI();
        }
        else
        {
            NotificationManage.Instance.ShowAtTopByKey(NotificationKeyConstants.NotEnoughResource, GetResourceStringWithSprite(ValueType.Faith));
        }
    }

    private void OnFaithTransformGold()
    {
        if (gameValue.GetResourceValue().Faith >= 5000)
        {
            gameValue.GetResourceValue().Faith -= 5000;
            gameValue.GetResourceValue().Gold += 3000;

            UpdateUI();
        }
        else
        {
            NotificationManage.Instance.ShowAtTopByKey(NotificationKeyConstants.NotEnoughResource, GetResourceStringWithSprite(ValueType.Faith));
        }
    }

    private void OnSliderValueChanged(float value)
    {
        if (recruitByfaith.value == 0)
        {
            getRecruit.text = "+ 1";
            costFaith.text = "- 100";
            UpdateMaxGetRecruit();
        }
        else
        {
            int recruitAmount = Mathf.FloorToInt(recruitByfaith.value * maxRecruit);
            getRecruit.text = "+ " + FormatNumber(recruitAmount);
            costFaith.text = "- " + FormatNumber(recruitAmount * 100);
            MaxGetRecruit.text = "Check";
        }
    }

    private void OnCheckButtonClicked()
    {
        int recruitAmount = Mathf.FloorToInt(recruitByfaith.value * maxRecruit);
        int cost = recruitAmount * 100;

        if (recruitByfaith.value == 0 && maxRecruit != 0)
        {
            recruitByfaith.value = 1;
            return;
        }


        if (gameValue.GetResourceValue().Faith >= cost)
        {
            gameValue.GetResourceValue().Faith -= cost;
            gameValue.GetResourceValue().TotalRecruitedPopulation += recruitAmount;

            UpdateUI();
        }
        else
        {
            NotificationManage.Instance.ShowToTopByKey(NotificationKeyConstants.NotEnoughResource,GetResourceStringWithSprite(ValueType.Faith));
        }
    }

    private void UpdateMaxGetRecruit()
    {
        maxRecruit = Mathf.FloorToInt(gameValue.GetResourceValue().Faith / 100);
        MaxGetRecruit.text = FormatNumber(maxRecruit); 
        recruitByfaith.interactable = maxRecruit != 0;


    }

    private void UpdateUI()
    {
        UpdateMaxGetRecruit();
        costFaith.text = "- 100"; 
        recruitByfaith.value = 0;
    }


    string FormatNumber(int value)
    {
        if (value >= 1000000000)
        {
            int billions = Mathf.FloorToInt(value / 100000000f);
            float temp = Mathf.RoundToInt(billions) / 10f;


            if (temp == Mathf.Floor(temp))
            {
                return temp + "B";
            }
            else
            {
                return temp.ToString("0.0") + "B";  
            }
        }
        else if (value >= 1000000)
        {


            int millions = Mathf.FloorToInt(value / 100000f);
            float temp = Mathf.RoundToInt(millions) / 10f;

            if (temp == Mathf.Floor(temp))
            {
                return temp + "M";
            }
            else
            {
                return temp.ToString("0.0") + "M";  
            }
        }
        else if (value >= 1000)
        {
            int hundred = Mathf.FloorToInt(value / 100f);
            float temp = Mathf.RoundToInt(hundred) / 10f;

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
            return value.ToString();
        }
    }
}
