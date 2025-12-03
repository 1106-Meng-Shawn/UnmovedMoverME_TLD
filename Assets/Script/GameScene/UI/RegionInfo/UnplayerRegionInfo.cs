using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static FormatNumber;
using static GetSprite;
using static GetColor;

using System;
using Unity.VisualScripting;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization;


public class UnplayerRegionInfo : PanelBase
{
    public static UnplayerRegionInfo Instance { get; private set; }
    public GameObject regionTitle;
    public Image regionCountryImage;
    public Image subordinateCountryImage;
    public Button HidePanelButton;
    public List<TextMeshProUGUI> resPTexts;
    public List<TextMeshProUGUI> resGrowthTexts;


    public TextMeshProUGUI populationNowText;
    public TextMeshProUGUI populationNextText;
    public TextMeshProUGUI populationGrowthText;

    public List<TextMeshProUGUI> resNowTexts;
    public List<TextMeshProUGUI> resNextTexts;
    public List<TextMeshProUGUI> resNextTaxTexts;
    public TextMeshProUGUI AvailablePopulationText;
    public TextMeshProUGUI RecruitedPopulationText;

    public TextMeshProUGUI SupportRateText;
    public List<Image> cityIcons;

    public Image lordImage;
    public TextMeshProUGUI lordNameText;
    private RegionValue regionAtInfo = null;
    public new Animation animation;
    public GameObject preBattlePanel;
    //public BuildRegionPanelControl buildPanel;

    public Button starButton;
    private CityConnetManage cityConnetManage;
    public CityValue cityAtInfo;
    private int cityIndex = -1;
    public CityInfoControl cityInfoControl;


    public bool isPlayerRegionInfo;
    public ScrollRect scrollRect;
    private List<CityValueInfo> cityValueInfos = new List<CityValueInfo>();

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
        if (regionAtInfo != null)
        {
            regionAtInfo.OnValueChanged -= UpUIData;
        }
        regionAtInfo = null;
    }

    private void OnLocaleChanged(Locale locale)
    {
        UpRegionTitle(); // ???? locale.Identifier ??????
    }


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
        InitCityValueInfos();
        gameObject.SetActive(false);

    }
    private void Start()
    {
        HidePanelButton.onClick.AddListener(ClosePanel);
        if (starButton != null) starButton.onClick.AddListener(OnStarButtonClick);
        cityConnetManage = CityConnetManage.Instance;
     //   

    }

    void InitCityValueInfos()
    {
        cityValueInfos.Clear();

        if (scrollRect == null) return;
        foreach (Transform child in scrollRect.content)
        {
            CityValueInfo info = child.GetComponent<CityValueInfo>();
            if (info != null)
            {
                cityValueInfos.Add(info);
            }
        }

    }

    private void InitStarButton()
    {
        if (starButton != null) starButton.gameObject.GetComponent<Image>().sprite = UpStarButtonSprite(regionAtInfo.IsStar);
    }


    void OnStarButtonClick()
    {

        regionAtInfo.IsStar = !regionAtInfo.IsStar;
        starButton.gameObject.GetComponent<Image>().sprite = UpStarButtonSprite(regionAtInfo.IsStar);

    }



    private void Update()
    {
        if (Input.GetMouseButtonDown(1)) ClosePanel();
    }


    public override void OpenPanel()
    {
        OpenPanel(regionAtInfo, cityIndex);
    }

    public override  PanelSaveData GetPanelSaveData()
    {
        PanelSaveData panelSaveData = new PanelSaveData();
        panelSaveData.isActive = IsActive();
        if (regionAtInfo != null)
        {
            panelSaveData.customData.Add(CustomDataType.Region, regionAtInfo.GetRegionID().ToString());
            panelSaveData.customData.Add(CustomDataType.City, cityIndex.ToString());
        }
        return panelSaveData;
    }

    public override void SetPanelSaveData(PanelSaveData panelSaveData)
    {
        if (panelSaveData == null) return;
        if (panelSaveData.isActive)
        {
            RegionValue saveRegionValue = null;
            if (panelSaveData.customData.TryGetValue(CustomDataType.Region, out string regionIDStr))
            {
                int regionID;
                if (int.TryParse(regionIDStr, out regionID))
                {
                    saveRegionValue = GameValue.Instance.GetRegionValue(regionID);
                }
            }

            int saveCityIndex = -1;
            if (panelSaveData.customData.TryGetValue(CustomDataType.City, out string cityIndexStr))
            {
                int parsedCityIndex;
                if (int.TryParse(cityIndexStr, out parsedCityIndex))
                {
                    saveCityIndex = parsedCityIndex;
                }
            }
            OpenPanel(saveRegionValue, saveCityIndex);
        }
    }


    public override void ClosePanel()
    {
        regionAtInfo?.region.CloseAllCountryOutline();
        regionAtInfo = null;
        cityAtInfo = null;
        StopCityLine();
        StartCoroutine(PlayAndHide());
    }

    private IEnumerator PlayAndHide()
    {
        animation.Play("Hide");
        yield return new WaitForSeconds(animation["Hide"].length);
        gameObject.SetActive(false);
    }



    public void OpenPanel(RegionValue region, int cityIndex = -1)
    {
        Debug.Log($"ShowPanel(RegionValue region, int cityIndex = -1) work");
        if (RegionInfo.Instance.IsActive()) RegionInfo.Instance.ClosePanel();
        gameObject.SetActive(true);

        if (regionAtInfo != null)
        {
            regionAtInfo.OnValueChanged -= UpUIData;
        }

        if (region != regionAtInfo)
        {
            Debug.Log($"ShowPanel(RegionValue region, int cityIndex = -1) Show");

            animation.Play("Show");

        }
        else if (cityIndex == -1)
        {
            region.region.ZoomToCity(0);
        }
        else if (cityIndex != -1)
        {
            region.region.ZoomToCity(cityIndex);
        }

        regionAtInfo = region;
        regionAtInfo.region.ApplySelectionEffect();
        regionAtInfo.OnValueChanged += UpUIData;

        this.cityIndex = cityIndex;
        cityAtInfo = cityIndex != -1 ? region.GetCityValue(cityIndex) : null;

        UpUIData();

        if (cityIndex != -1)
        {
            cityInfoControl.ShowCityInfo(regionAtInfo.GetCityValue(cityIndex));
        }
        else
        {
            cityInfoControl.ShowCityInfo(null);
        }

    }


    public void SetCity(int index)
    {
        cityAtInfo = regionAtInfo.GetCityValue(index);
        UpUIData();
        //regionOutLine.DrawCountyOutline(region);
    }




    public void UpUIData()
    {
        if (regionAtInfo == null) return;

        UpRegionTitle();
        UpLord();

        ShowCityLine();

        InitStarButton();
        UpCityValueInfos();
    }


    void ShowCityLine()
    {
        CityConnetManage.Instance.AnimateNeighborEdgesToward(cityAtInfo);
    }

    void UpCityValueInfos()
    {
        if (scrollRect != null) { scrollRect.horizontalNormalizedPosition = 0f; }
        for (int i = 0; i < cityValueInfos.Count; i++)
        {
            cityValueInfos[i].SetCityValue(regionAtInfo);
        }
    }



    void StopCityLine()
    {
        if (cityConnetManage != null) cityConnetManage.StopAllAnimations();

    }



    public string GetRegionCountryName()
    {
        return regionAtInfo.GetCountryName();
    }

    void UpRegionTitle()
    {
        // regionTitle.GetComponentInChildren<Image>().sprite = regionAtInfo.GetRegionIcon();
        if (regionCountryImage == null) return;
        regionCountryImage.sprite = regionAtInfo.GetCountryIcon();
        regionCountryImage.GetComponent<IntroPanelShow>().SetIntroName(regionAtInfo.GetCountryNameWithColor());

        CountryManager countryManager = GameValue.Instance.GetCountryManager();

        if (countryManager.HasOverlord(regionAtInfo.GetCityCountry(0)))
        {
            subordinateCountryImage.gameObject.SetActive(true);
            subordinateCountryImage.GetComponent<IntroPanelShow>().SetIntroName(regionAtInfo.GetCityCountryWithColor(0));
            subordinateCountryImage.sprite = regionAtInfo.GetRegionIcon();
        }
        else
        {
            subordinateCountryImage.gameObject.SetActive(false);
        }


        regionTitle.GetComponentInChildren<TextMeshProUGUI>().text = GetCountryColorString(regionAtInfo.GetRegionName(), regionAtInfo.GetCityCountry(0));

    }
    void UpCityIcons()
    {
        if (regionAtInfo == null) return;

        int cityCount = regionAtInfo.GetCityCountryNum();
        for (int i = 0; i < cityIcons.Count; i++)
        {
            cityIcons[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < cityCount; i++)
        {
            cityIcons[i].sprite = regionAtInfo.GetCityIcon(i);
            cityIcons[i].gameObject.SetActive(true);
        }
    }


    //void UpResP(int index)
    //{
    //    if (regionAtInfo == null) return;
    //    if (resPTexts.Count == 0) return;

    //    resPTexts[index].text = GetRegionValueColorString(FormatNumberToString(regionAtInfo.GetRegionResourceParameter(index)), index);
    //}

    void UpResGrwoth(int index)
    {
        if (regionAtInfo == null) return;
        if (resGrowthTexts.Count == 0) return;

        resGrowthTexts[index].text = GetRegionValueColorString(regionAtInfo.GetRegionResourceGrowthString(index), index);
    }

    void UpRes(int index)
    {
        if (regionAtInfo == null) return;
        if (resNowTexts.Count == 0) return;
        resNowTexts[index].text = GetRegionValueColorString(FormatNumberToString(regionAtInfo.GetRegionResourceSurplus(index)), index);

    }

    void UpResNext(int index)
    {
        if (regionAtInfo == null) return;
        if (resNextTexts.Count == 0) return;
        resNextTexts[index].text = GetRegionValueColorString(FormatNumberToString(regionAtInfo.GetRegionResourceNext(index)), index);

    }

    void UpResNextTax(int index)
    {
        if (regionAtInfo == null) return;
        if (resNextTaxTexts.Count == 0) return;
        resNextTaxTexts[index].text = GetRegionValueColorString(FormatNumberToString(regionAtInfo.GetRegionResoureNextTax(index)), index);
    }

    void UpLord()
    {
        if (regionAtInfo == null) return;
        if (regionAtInfo.HasLord())
        {
            Character lord = regionAtInfo.GetLord();
            lordNameText.gameObject.SetActive(true);
            lordNameText.text = lord.GetCharacterName();
            lordImage.sprite = lord.icon;
        }
        else
        {
            lordNameText.gameObject.SetActive(false);
            lordImage.sprite = GetCharacterStateSprite("lord");

        }

    }

    public string GetCityCoutyString(int index)
    {
        if (regionAtInfo == null) return null;
        return regionAtInfo.GetCityCountryString(index);
    }
}
