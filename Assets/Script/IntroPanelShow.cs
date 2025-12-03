using System;
using System.Drawing;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;
using static GetColor;

public class IntroPanelShow : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    #region Serialized Fields
    [Header("Panel Configuration")]
    [SerializeField] private GameObject introPanelPrefab;
    [SerializeField] private Image introImage;
    [SerializeField] private Sprite introSprite;

    [Header("Content")]
    [SerializeField] private string introNameLocalizationTableName = "IntroName";
    [SerializeField] private string introName;
    [SerializeField, TextArea] private string introDescription;

    [Header("Position")]
    [SerializeField] private bool useFixedPosition = false;
    [SerializeField] private Vector2 panelOffset = new Vector2(125, -175);
    [SerializeField] private float pivotX;
    [SerializeField] private float pivotY;

    [Header("Panel Type")]
    [SerializeField] private PanelType panelType = PanelType.Normal;
    [SerializeField] private ContentType contentType = ContentType.None;
    [SerializeField] private int cityCount = 0;

    [Header("References")]
    [SerializeField] private SaveAndLoadButtonControl saveAndLoadButtonControl;
    [SerializeField] private RegionInfo regionInfo;
    [SerializeField] private Canvas uiCanvas;
    #endregion

    #region Private Fields
    private GameObject introPanelInstance;
    private Character characterData;
    private ItemBase itemData;
    #endregion

    #region Enums
    public enum PanelType { Normal = 1, NameOnly = 2 }
    public enum ContentType { None, City, Region, Country, Character, Item, SaveData, Sprite }
    #endregion

    #region Unity Lifecycle
    private void Start()
    {
        InitializeComponents();
    }

    private void Update()
    {
        HandleInput();
        UpdatePanelFollowMouse();
    }

    private void OnDisable()
    {
        DestroyPanel();
    }
    #endregion

    #region Initialization
    private void InitializeComponents()
    {
        if (introImage == null)
            introImage = GetComponent<Image>();

        if (introImage != null)
            introSprite = introImage.sprite;

        if (uiCanvas == null)
            uiCanvas = GeneralManager.Instance.uiCanvas;
    }
    #endregion

    #region Pointer Events
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (ShouldIgnorePointerEnter()) return;

        CreatePanel();
        SetupPanelContent();
        UpdatePanelPosition(introPanelInstance.GetComponent<RectTransform>());

        if (contentType == ContentType.Region)
            CheckRegionMouseOver();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        DestroyPanel();
    }
    #endregion

    #region Mouse Events (for non-UI objects)
    private void OnMouseEnter()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            OnMouseExit();
            return;
        }

        if (introPanelInstance != null) return;

        CreatePanel();
        SetupPanelContent();
        UpdatePanelPosition(introPanelInstance.GetComponent<RectTransform>());
    }

    private void OnMouseExit()
    {
        DestroyPanel();
    }
    #endregion

    #region Panel Management
    private void CreatePanel()
    {
        if (introPanelInstance != null) return;

        introPanelInstance = Instantiate(introPanelPrefab, GeneralManager.Instance.uiCanvas.transform);
    }

    private void SetupPanelContent()
    {
        if (introPanelInstance == null) return;

        var panelControl = introPanelInstance.GetComponent<IntroPanelControl>();
        if (panelControl == null) return;

        switch (panelType)
        {
            case PanelType.Normal:
                SetupNormalPanel(panelControl);
                break;

            case PanelType.NameOnly:
                SetupNameOnlyPanel(panelControl);
                break;
        }
    }

    public void SetPivot(Vector2 pivot)
    {
        pivotX = pivot.x;
        pivotY = pivot.y;
    }


    private void SetupNormalPanel(IntroPanelControl control)
    {
        control.SetValue(introSprite, GetLocalizedName(), introDescription, pivotX, pivotY);
    }

    private void SetupNameOnlyPanel(IntroPanelControl control)
    {
        string displayName = GetContentName();

        switch (contentType)
        {
            case ContentType.Region:
                SetupRegionContent(control);
                break;

            case ContentType.Item:
            case ContentType.City:
            case ContentType.Country:
            case ContentType.Character:
                control.SetName(displayName, pivotX, pivotY);
                break;

            case ContentType.SaveData:
                SetupSaveDataContent(control);
                break;

            default:
                control.SetImageAndName(introSprite, GetLocalizedName(), pivotX, pivotY);
                break;
        }

        ScalePanelByText();
    }

    public void DestroyPanel()
    {
        if (introPanelInstance != null)
        {
            Destroy(introPanelInstance);
            introPanelInstance = null;
        }
    }
    #endregion

    #region Content Setup
    private string GetContentName()
    {
        switch (contentType)
        {
            case ContentType.Item:
                return GetItemName();

            case ContentType.City:
                return regionInfo?.GetCityCoutyString(cityCount) ?? introName;

            case ContentType.Region:
                return GetRegionName();

            case ContentType.Country:
                return regionInfo?.GetRegionCountryName() ?? introName;

            case ContentType.Character:
                return characterData?.GetCharacterName() ?? introName;

            case ContentType.SaveData:
                return GetSaveDataCountryName();

            default:
                return introName;
        }
    }

    private void SetupRegionContent(IntroPanelControl control)
    {
        Region region = GetComponent<Region>();
        if (region?.GetRegionValue() == null) return;

        RegionValue regionValue = region.GetRegionValue();
        string regionName = regionValue.GetRegionName();
        Color32 regionColor = GetRegionColor(regionValue);
        string coloredName = GetColoredString(regionName, regionColor);

        control.SetImageAndName(
            regionValue.GetRegionIcon(),
            $"<b>{coloredName.ToUpper()}</b>",
            pivotX,
            pivotY
        );
    }

    private void SetupSaveDataContent(IntroPanelControl control)
    {
        if (saveAndLoadButtonControl == null) return;

        SaveData saveData = saveAndLoadButtonControl.GetSaveData();
        if (saveData == null) return;

        string countryName = saveData.gameValueData.GetPlayerCountry();
        control.SetName(GetLocalizedString(countryName), pivotX, pivotY);
    }
    #endregion

    #region Name Getters

    public void SetIntroName(string introName)
    {
        this.introName = introName;
    }

    public void SetIntroNameAndSprite(string introName,Sprite introSprite)
    {
        this.introName = introName;
        this.introSprite = introSprite;
    }




    private string GetItemName()
    {
        if (itemData == null) return introName;

        string itemName = itemData.GetItemName();
        string removeText = LocalizationSettings.StringDatabase.GetLocalizedString("GameSetting", "RemoveItem");
        return $"{itemName}\n<color=#FF0000>{removeText}</color>";
    }

    private string GetRegionName()
    {
        Region region = GetComponent<Region>();
        return region?.GetRegionValue()?.GetRegionName() ?? introName;
    }

    private string GetSaveDataCountryName()
    {
        if (saveAndLoadButtonControl == null) return introName;

        SaveData saveData = saveAndLoadButtonControl.GetSaveData();
        return saveData?.gameValueData.GetPlayerCountry() ?? introName;
    }
    #endregion

    #region Localization
    private string GetLocalizedName()
    {
        if (string.IsNullOrEmpty(introNameLocalizationTableName))
            return introName;

        return LocalizationSettings.StringDatabase.GetLocalizedString(
            introNameLocalizationTableName,
            introName
        );
    }

    private string GetLocalizedString(string key)
    {
        if (string.IsNullOrEmpty(introNameLocalizationTableName))
            return key;

        return LocalizationSettings.StringDatabase.GetLocalizedString(
            introNameLocalizationTableName,
            key
        );
    }
    #endregion

    #region Position Management
    private void UpdatePanelFollowMouse()
    {
        if (introPanelInstance == null || useFixedPosition) return;

        if (contentType == ContentType.Region && EventSystem.current.IsPointerOverGameObject())
        {
            OnMouseExit();
            return;
        }

        // ?? RectTransform
        var panelRect = introPanelInstance.GetComponent<RectTransform>();

        // ????????????
        Vector3 worldPos;
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(
            transform.root.GetComponent<RectTransform>(),
            Input.mousePosition,
            uiCanvas.worldCamera,
            out worldPos))
        {
            panelRect.position = worldPos + (Vector3)panelOffset; 
        }
    }


    private void UpdatePanelPosition(RectTransform rectTransform)
    {
        RectTransform canvasRect = transform.root.GetComponent<RectTransform>();
        Vector2 localPoint;

        if (useFixedPosition)
        {
            Vector2 screenPoint = GetScreenPoint();
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect, screenPoint, Camera.main, out localPoint))
            {
                rectTransform.anchoredPosition = localPoint + panelOffset;
            }
        }
        else
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect, Input.mousePosition, Camera.main, out localPoint);
            rectTransform.anchoredPosition = localPoint + panelOffset;
        }
    }

    private Vector2 GetScreenPoint()
    {
        if (introImage != null)
            return RectTransformUtility.WorldToScreenPoint(Camera.main, introImage.rectTransform.position);
        else
            return RectTransformUtility.WorldToScreenPoint(Camera.main, transform.position);
    }
    #endregion

    #region Helpers
    private bool ShouldIgnorePointerEnter()
    {
        return introPanelInstance != null || string.IsNullOrEmpty(introName);
    }

    private void HandleInput()
    {
        if (Input.GetMouseButtonDown(1))
            DestroyPanel();
    }

    private void CheckRegionMouseOver()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

        if (hit.collider?.gameObject == gameObject)
            OnMouseEnter();
        else
            OnMouseExit();
    }

    private void ScalePanelByText()
    {
        if (introPanelInstance == null) return;

        var control = introPanelInstance.GetComponent<IntroPanelControl>();
        if (control?.nameText == null) return;

        var preferredSize = control.nameText.GetPreferredValues();
        var rect = control.nameText.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(preferredSize.x, rect.sizeDelta.y);
    }

    private Color32 GetRegionColor(RegionValue regionValue)
    {
        CountryManager countryManager = GameValue.Instance.GetCountryManager();

        if (countryManager.HasOverlord(regionValue.GetCityCountry(0)))
            return countryManager.GetCountryColor(regionValue.GetCityCountry(0));
        else
            return countryManager.GetCountryColor(regionValue.GetCountryENName());
    }

    private string GetColoredString(string text, Color32 color)
    {
        string hexColor = ColorUtility.ToHtmlStringRGB(color);
        return $"<color=#{hexColor}>{text}</color>";
    }
    #endregion

    #region Public Setters
    public void SetCharacter(Character character)
    {
        characterData = character;
    }

    public void SetItem(ItemBase item)
    {
        DestroyPanel();
        itemData = item;
    }
    #endregion
}