using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using static GetSprite;
using static GetColor;
using System.Globalization;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using System;
using static LoadPanelManage.TotalSave;


public class CityInfoControl : MonoBehaviour
{
    public Image cityIndexIcon;


    public Image regionCountryImage;
    public Image subordinateCountryImage;
    public TextMeshProUGUI regionNameText;
    public Button starButton;
    public Button regionTitleButton;


    public Image cityImage;
    public TextMeshProUGUI cityNameText;

    public List<Button> valueTypeButtons = new List<Button>();
    public List<Transform> valueObjects = new List<Transform>();
    List<Image> valueIcon = new List<Image>();
    List<TextMeshProUGUI> valueText = new List<TextMeshProUGUI>();


    public TextMeshProUGUI exploreLevelText;
    public Button exploreButton;
    public Button battleButton;


    public TextMeshProUGUI buildMaxText;
    public TextMeshProUGUI buildNumText;
    public Button buildButton;

    CityValue cityValue;
    int currentValueType = 0;
    Sprite selSprite; // just this one look like better in Ui
    Sprite unSelSprite;

    public TaskSortControl taskSortControl;

    private void Awake()
    {
        InitValueOj();
        InitSprite();
        InitButtons();
        InitTasksButtons();
    }

    void OnEnable()
    {
        LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
    }

    void OnDisable()
    {
        LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;
        RemoveCityListener();
    }

    private void OnDestroy()
    {
        LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;
        RemoveCityListener();
    }

    void InitSprite()
    {
        selSprite = Resources.Load<Sprite>("MyDraw/UI/Other/SettingBoxUnsel");
        unSelSprite = Resources.Load<Sprite>("MyDraw/UI/GameUI/Box");
    }

    void InitButtons()
    {
        regionTitleButton.onClick.AddListener(CloseCityInfoPanel);
    }

    private void OnLocaleChanged(Locale locale)
    {
        SetText(); 
    }


    void InitTasksButtons()
    {
        if (exploreButton != null) exploreButton.onClick.AddListener(OnExploreButtonClick);
        if (battleButton != null) battleButton.onClick.AddListener(OnBattleButtonClick);
        if (buildButton != null) buildButton.onClick.AddListener(OnBuildButtonClick);
        if (starButton != null) starButton.onClick.AddListener(OnStarButtonClick);
    }


 



    public void ShowCityInfo(CityValue cityValue)
    {
        if (cityValue == null)
        {
            gameObject.SetActive(false);
            return;
        }

        RemoveCityListener();
        AddCityListener();
        SetValueType(5);
        gameObject.SetActive(true);
        this.cityValue = cityValue;
        UpCityInfoUI();

    }

    void CloseCityInfoPanel()
    {
     //   gameObject.SetActive(false);
        this.cityValue.regionValue.region.ShowRegionPanel();
        CityConnetManage.Instance.StopAllAnimations();
    }

    void UpCityInfoUI()
    {
        SetTitle();
        SetCityExploreLevelText();
        SetBuildValueText();
        SetStarButtonSprite();
        UpValueTypeUI(currentValueType);
        SetCityTask();

    }

    void SetTitle()
    {
        SetIcon();
        SetText();

    }
    void SetIcon()
    {
        cityIndexIcon.sprite = GetCitySprite(cityValue.cityIndex);
        SetRegionIcon();
        SetCityIcon();
    }


    void SetRegionIcon()
    {
        if (regionCountryImage == null) return;
        regionCountryImage.sprite = cityValue.regionValue.GetCountryIcon();
        regionCountryImage.GetComponent<IntroPanelShow>().SetIntroName(cityValue.regionValue.GetCountryNameWithColor());

        CountryManager countryManager = GameValue.Instance.GetCountryManager();

        if (countryManager.HasOverlord(cityValue.GetCityCountry()))
        {
            subordinateCountryImage.gameObject.SetActive(true);
            subordinateCountryImage.GetComponent<IntroPanelShow>().SetIntroName(cityValue.GetCityCountryNameWithColor());
            subordinateCountryImage.sprite = cityValue.regionValue.GetRegionIcon();
        }
        else
        {
            subordinateCountryImage.gameObject.SetActive(false);
        }

    }

    void SetCityIcon()
    {
        cityImage.sprite = cityValue.regionValue.GetCityIcon(cityValue.cityIndex);
    }


    void SetText()
    {
        regionNameText.text = cityValue.regionValue.GetRegionNameWithColor();
        cityNameText.text = cityValue.GetCityNameWithColor();
    }


    void OnBuildButtonClick()
    {
        // BuildRegionPanelControl.Instance.OpenPanel(cityValue.regionValue, cityValue.cityIndex);
        BuildData buildData = new BuildData(cityValue.regionValue, cityValue.cityIndex);
        GameValue.Instance.SetBuildData(buildData);
        SceneTransferManager.Instance.LoadScene(Scene.BuildScene);
    }

    void OnExploreButtonClick()
    {
        BattlePanelManage.Instance.ShowBattlePanel(cityValue.regionValue, cityValue.cityIndex, true);
    }

    void OnBattleButtonClick()
    {
        if (CityConnetManage.Instance == null)
        {
            Debug.LogError("CityConnetManage.Instance is null!");
            return;
        }


        CityValue city = this.cityValue;
        

        if (CityConnetManage.Instance.IsAdjacentToPlayerCity(city))
        {
            CountryManager countryManager = GameValue.Instance.GetCountryManager();
            if (countryManager.IsAtWar(GameValue.Instance.GetPlayerCountryENName(), city.cityCountry))
            {
                BattlePanelManage.Instance.ShowBattlePanel(city.regionValue, city.cityIndex, false);
            }
            else
            {
                ReminderPanelControl.Instance.ShowWarDeclarationReminder(city);
            }


        }
        else
        {
            //  Debug.Log("need to add Notification reminder");
            //NotificationManage.Instance.ShowToTop("You have no cities connected to this city");
            NotificationManage.Instance.ShowToTopByKey(NotificationKeyConstants.City_NoConnection,city.GetCityNameWithColor());
        }
    }

    void OnStarButtonClick()
    {
        cityValue.regionValue.IsStar = !cityValue.regionValue.IsStar;
        SetStarButtonSprite();

    }

    private void SetStarButtonSprite()
    {
        if (starButton != null) starButton.gameObject.GetComponent<Image>().sprite = UpStarButtonSprite(cityValue.regionValue.IsStar);
    }


    void SetCityExploreLevelText()
    {
        exploreLevelText.text = cityValue.GetExploreLevel().ToString();
    }

    void SetBuildValueText()
    {
        if (buildMaxText == null) return;
        if (buildNumText == null) return;

        buildMaxText.text = cityValue.GetBuildMaxString();
        buildNumText.text = cityValue.GetBuildNumString();

    }

    void InitValueOj()
    {
        valueText.Clear();
        valueIcon.Clear();

        if (valueObjects.Count <= 0) return;

        foreach (var obj in valueObjects)
        {
            if (obj.childCount == 0 || obj.GetChild(0).childCount == 0)
            {
                Debug.LogWarning($"{obj.name} it don't have child");
                continue;
            }

            var child = obj.GetChild(0).GetChild(0);

            var text = child.GetComponentInChildren<TextMeshProUGUI>();
            var icon = child.GetComponentInChildren<Image>();
            var group = child.GetComponentInChildren<CanvasGroup>();

            if (text == null || icon == null || group == null)
            {
                Debug.LogWarning($"child {child.name} don;t have componet");
                continue;
            }

            valueText.Add(text);
            valueIcon.Add(icon);
        }

        for (int i = 0; i < valueTypeButtons.Count; i++)
        {
            int index = i;
            valueTypeButtons[i].onClick.AddListener(() => OnValueTypeButtonClick(index));
        }

    }

    public void OnValueTypeButtonClick(int index)
    {
        UpValueTypeUI(index);
    }

    void SetValueType(int index)
    {
        if (valueTypeButtons.Count <= 0) return;

        valueTypeButtons[currentValueType].gameObject.GetComponent<ButtonEffect>().SetChangeSprite(selSprite, unSelSprite);
        currentValueType = index;
        valueTypeButtons[currentValueType].gameObject.GetComponent<ButtonEffect>().SetChangeSprite(selSprite, selSprite);

    }


    public void UpValueTypeUI(int valueType)
    {
        if (valueObjects.Count <= 0) return;
        SetValueType(valueType);


        if (cityValue == null) return;
        string typeName = null;
        switch (currentValueType)
        {
            case 0: typeName = "Food"; break;
            case 1: typeName = "Science"; break;
            case 2: typeName = "Politics"; break;
            case 3: typeName = "Gold"; break;
            case 4: typeName = "Faith"; break;
            case 5: typeName = "Population"; break;

        }

        if (typeName == null) return;
        UpValueTypeIcon(typeName);
        UpValueText();

    }

    // 0 max, 1 current, 2 growth rate,3 parameter? , 4 this turn take value?5 new turn value ,6 next turn will tak value? 
    void UpValueTypeIcon(string valueType)
    {
        if (valueType == "Population") { UpPopulationIcon(); return; }
        valueIcon[0].sprite = Resources.Load<Sprite>($"MyDraw/UI/Region/Value/{valueType}/{valueType}MAX");
        valueIcon[1].sprite = Resources.Load<Sprite>($"MyDraw/UI/Region/Value/{valueType}/{valueType}NOW");
        valueIcon[2].sprite = Resources.Load<Sprite>($"MyDraw/UI/Region/Value/{valueType}/{valueType}Growth%");
        valueIcon[3].sprite = Resources.Load<Sprite>($"MyDraw/UI/Region/Value/{valueType}/{valueType}P");
        valueIcon[4].sprite = Resources.Load<Sprite>($"MyDraw/UI/Region/Value/{valueType}/{valueType}Take");
        valueIcon[5].sprite = Resources.Load<Sprite>($"MyDraw/UI/Region/Value/{valueType}/{valueType}NEXT");
        valueIcon[6].sprite = Resources.Load<Sprite>($"MyDraw/UI/Region/Value/{valueType}/{valueType}NEXTTake");

    }
    void UpValueText()
    {
        if (currentValueType == 5) { UpPopulationValueText(); return; }
        valueText[0].text = GetRegionValueColorString(cityValue.GetResourceMax(currentValueType).ToString("N0"), currentValueType);
        valueText[1].text = GetRegionValueColorString(cityValue.GetResourceSurplus(currentValueType).ToString("N0"), currentValueType);
        valueText[2].text = GetRegionValueColorString(cityValue.GetResourceGrowthString(currentValueType), currentValueType);// regionValue.population.ToString("N0");
        valueText[3].text = GetRegionValueColorString(cityValue.GetParameterWithLord(currentValueType).ToString("N0"), currentValueType);
        valueText[4].text = GetRegionValueColorString(cityValue.GetResourceLastTax(currentValueType).ToString("N0"), currentValueType);
        valueText[5].text = GetRegionValueColorString(cityValue.GetResourceNext(currentValueType).ToString("N0"), currentValueType);
        valueText[6].text = GetRegionValueColorString(cityValue.GetResoureNextTax(currentValueType).ToString("N0"), currentValueType);
    }

    // 0 max population, 1 current population , 2 rowth rate , 3 new turn population,4 surrpot rate?,5 Maximum recruitable population,6 current recruitable population? 
    void UpPopulationValueText()
    {
        valueText[0].text = GetRegionValueColorString(cityValue.GetMaxPopulation().ToString("N0"), currentValueType);
        valueText[1].text = GetRegionValueColorString(cityValue.GetPopulation().ToString("N0"), currentValueType);
        valueText[2].text = GetRegionValueColorString(cityValue.GetPopulationGrowthString(), currentValueType);// regionValue.population.ToString("N0");
        valueText[3].text = GetRegionValueColorString(cityValue.GetNextTurnPopulation().ToString("N0"), currentValueType);
        valueText[4].text = GetRegionValueColorString(cityValue.GetSupportRateString(), currentValueType);
        valueText[5].text = GetRegionValueColorString(cityValue.GetAvailablePopulation().ToString("N0"), currentValueType);
        valueText[6].text = GetRegionValueColorString(cityValue.GetRecruitedPopulation().ToString("N0"), currentValueType);

    }
    void UpPopulationIcon()
    {
        valueIcon[0].sprite = Resources.Load<Sprite>($"MyDraw/UI/Region/Value/Population/RegionPopulationMAX");
        valueIcon[1].sprite = Resources.Load<Sprite>($"MyDraw/UI/Region/Value/Population/RegionPopulationNOW");
        valueIcon[2].sprite = Resources.Load<Sprite>($"MyDraw/UI/Region/Value/Population/RegionPopulationGrowth%");
        valueIcon[3].sprite = Resources.Load<Sprite>($"MyDraw/UI/Region/Value/Population/RegionPopulationNEXT");
        valueIcon[4].sprite = Resources.Load<Sprite>($"MyDraw/UI/Region/Value/Population/SupportRate");
        valueIcon[5].sprite = Resources.Load<Sprite>($"MyDraw/UI/Region/Value/Population/AvailablePopulation");
        valueIcon[6].sprite = Resources.Load<Sprite>($"MyDraw/UI/Region/Value/Population/RecruitmentPopulation");

    }


    void SetCityTask()
    {
        taskSortControl.SetTaskList(cityValue.GetTaskDatas());
    }


    void AddCityListener()
    {
        if (cityValue != null)
        {
            cityValue.OnCityValueChanged += OnCityValueChange;

            cityValue.regionValue.OnValueChanged += OnRegionValueChange;
        }

    }

    void RemoveCityListener()
    {
        if (cityValue != null)
        {
            cityValue.OnCityValueChanged -= OnCityValueChange;

            cityValue.regionValue.OnValueChanged -= OnRegionValueChange;
        }
    }

    void OnCityValueChange()
    {
        if (cityValue != null)
        {
            UpCityInfoUI();
        }
    }

    void OnRegionValueChange()
    {
        if (cityValue != null)
        {
            SetRegionIcon();
            SetStarButtonSprite();
            UpValueTypeUI(currentValueType);
        }
    }


}
