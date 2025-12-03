using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Globalization;
using System;
using DG.Tweening;
using static GetSprite;
using static GetColor;
using System.Net;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization;

public class CityValueInfo : MonoBehaviour
{
    [SerializeReference] private bool isPlayer = true;
    [Header("City base left value")]
    public Image cityCountryIcon;
    public TextMeshProUGUI cityName;
    public TextMeshProUGUI buildMax;
    public TextMeshProUGUI buildNum;
    public TextMeshProUGUI expoloreLevel;

    [Header("Button")]
    public Button cityButton;
    public Button buildButton;
    public Button exploreButton;
    public Button battleButton;


    [Header("Other")]
    [SerializeField] private GameObject battleRow;
    [SerializeField] private GameObject buildRow;

    [SerializeField] private int cityIndex = 0;
    private RegionValue regionValue;
    private CityValue cityValue;
    void OnEnable()
    {
        LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
    }

    void OnDisable()
    {
        LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;
    }

    private void OnLocaleChanged(Locale locale)
    {
        SetCityName(); // ???? locale.Identifier ??????
    }

    void Start()
    {
        InitButtons();

    }

    void InitButtons()
    {
        if (buildButton != null) buildButton.onClick.AddListener(OnBuildButtonClick);
        if (exploreButton != null) exploreButton.onClick.AddListener(OnExploreButtonClick);
        if (cityButton != null) cityButton.onClick.AddListener(OnCityButtonClick);
        if (battleButton != null) battleButton.onClick.AddListener(OnBattleButtonClick);

        InitValueButtons();
    }

    void InitValueButtons()
    {

    }

    public void SetCityValue(RegionValue regionValue)
    {

        gameObject.SetActive(true);
        this.regionValue = regionValue;
        this.cityValue = regionValue.GetCityValue(cityIndex);
        SetCityData();
        battleButton.GetComponent<ButtonEffect>().SetIsTriggerEffect(CityConnetManage.Instance.IsAdjacentToPlayerCity(cityValue));
    }

    void SetCityData()
    {
        gameObject.SetActive(false);

        if (cityValue != null)
        {
            gameObject.SetActive(true);
            UpData();
        }

    }

    void UpData()
    {
        cityCountryIcon.sprite = GameValue.Instance.GetCountryIcon(cityValue.GetCityCountry());
        expoloreLevel.text = cityValue.exploreLevel.ToString();

        bool isPlayerCountry = cityValue.GetCityCountry() == GameValue.Instance.GetPlayerCountryENName();
        buildRow.gameObject.SetActive(isPlayerCountry);
        battleRow.gameObject.SetActive(!isPlayerCountry);

        SetCityName();

    }

    void SetCityName()
    {
        //  cityName.text = GameValue.Instance.GetCountryColorString(cityValue.GetCityName(),cityValue.GetCityCountry());
        cityName.text = GetCountryColorString(cityValue.GetCityName(), cityValue.GetCityCountry());
        string CityCountryName = cityValue.GetCityCountryNameWithColor();
        cityCountryIcon.GetComponent<IntroPanelShow>().SetIntroName(CityCountryName);

    }

    void OnCityButtonClick()
    {
        if (regionValue.GetCityValue(cityIndex).IsShowRegionPanel())
        {
            RegionInfo.Instance.ShowPanel(regionValue,cityIndex);
        } else
        {
            UnplayerRegionInfo.Instance.OpenPanel(regionValue, cityIndex);
        }

        regionValue.region.ZoomToCity(cityIndex);
    }



    void OnBuildButtonClick()
    {
        //BuildRegionPanelControl.Instance.OpenPanel(regionValue, cityIndex);
        BuildData buildData = new BuildData(regionValue,cityIndex);
        GameValue.Instance.SetBuildData(buildData);
        SceneTransferManager.Instance.LoadScene(Scene.BuildScene);
    }

    void OnExploreButtonClick()
    {
        BattlePanelManage.Instance.ShowBattlePanel(regionValue,cityIndex, true);
    }

    void OnBattleButtonClick()
    {
        if (CityConnetManage.Instance == null)
        {
            Debug.LogError("CityConnetManage.Instance is null!");
            return;
        }

        if (regionValue == null)
        {
            Debug.LogError("regionValue is null!");
            return;
        }

        CityValue city = regionValue.GetCityValue(cityIndex);
        if (city == null)
        {
            Debug.LogError($"City at index {cityIndex} is null!");
            return;
        }

        if (CityConnetManage.Instance.IsAdjacentToPlayerCity(city))
        {
            CountryManager countryManager = GameValue.Instance.GetCountryManager();
            if (countryManager.IsAtWar(GameValue.Instance.GetPlayerCountryENName(),city.cityCountry))
            {
                BattlePanelManage.Instance.ShowBattlePanel(regionValue, cityIndex, false);
            }
            else
            {
                ReminderPanelControl.Instance.ShowWarDeclarationReminder(city);
            }


        }
        else
        {
            //  Debug.Log("need to add Notification reminder");
            //   NotificationManage.Instance.ShowToTop("You have no cities connected to this city");
            NotificationManage.Instance.ShowToTopByKey(NotificationKeyConstants.City_NoConnection, city.GetCityNameWithColor());
        }
    }




}
