using System.Collections;
using System;
using UnityEngine;
using TMPro;
using static GetSprite;
using static GetColor;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization;

public class CityControl : MonoBehaviour
{
    public Region region;
    public int cityIndex;
    public GameObject CityInfoPanel;

    public float baseScale = 0.1f;
    [SerializeField] float baseOrthoSize = 15f;
    [SerializeField] Vector3 minScaleVec = new Vector3(0.1f, 0.1f, 0.1f);


    private float lastClickTime = 0f;
    private float doubleClickThreshold = 0.3f;

    private SpriteRenderer spriteRenderer;
    public SpriteRenderer markSpriteRenderer;

    public SpriteButton cityIconSprite;
    public SpriteButton markSprite;

    private Color originalColor;
    public float darkenFactor = 0.6f;
    public float flashDuration = 0.1f;

    private Coroutine clickFlashCoroutine;
    private bool isHovered = false;
    public IntroPanelShow cityMarkIntroPanelShow;
    private CityValue cityValue;
    private Action taskDataChangedHandler;
    private enum CityVisibilityState
    {
        Hidden,
        MainCity,
        All
    }

    private CityVisibilityState currentState = CityVisibilityState.All;


    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
        AddListeners();
        cityIconSprite.SetAction(OnCityIconClick);
        markSprite.SetAction(OnCityIconClick);
    }

    void AddListeners()
    {
        CameraControl2D.OnZoomChanged += OnZoomChange;
        LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;

    }

    void OnCityIconClick()
    {
        if (cityValue.IsPlayerCity())
        {
            RegionInfo.Instance.ShowPanel(region.GetRegionValue(), cityIndex);
        }else
        {
            UnplayerRegionInfo.Instance.OpenPanel(region.GetRegionValue(), cityIndex);
        }
    }

    void OnDestroy()
    {
        RemoveListeners();
    }

    void RemoveListeners()
    {
        CameraControl2D.OnZoomChanged -= OnZoomChange;
        LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;

        if (this.cityValue != null && taskDataChangedHandler != null)
        {
            this.cityValue.OnTaskDatasChanged -= taskDataChangedHandler;
        }
    }

    private void OnLocaleChanged(Locale locale)
    {
        SetCityName();
    }

    void OnZoomChange(float currentZoom)
    {
        if (CityInfoPanel != null)
        {
            float scaleFactor = currentZoom / baseOrthoSize;
            Vector3 desiredScale = Vector3.one * baseScale * scaleFactor;

            CityInfoPanel.transform.localScale = Vector3.Max(desiredScale, minScaleVec);
        }
    }


    public void SetCityValue(CityValue cityValue)
    {
        this.cityValue = cityValue;

        taskDataChangedHandler = () => ApplyZoomVisibility(Camera.main.orthographicSize);
        this.cityValue.OnTaskDatasChanged += taskDataChangedHandler;
        SetCityName();
    }

    public void SetCityName()
    {
        IntroPanelControl introPanelControl = CityInfoPanel.GetComponent<IntroPanelControl>();
        introPanelControl.nameText.text = cityValue.GetCityNameWithColor();
        introPanelControl.introImage.sprite = cityValue.GetCityCountryIcon();
    }

    void OnMouseEnter()
    {
       /* isHovered = true;
        SetColor(DarkenedColor());*/
    }

    void OnMouseExit()
    {
       /* isHovered = false;
        SetColor(originalColor);*/
    }


    void OnMouseDown()
    {
        if (clickFlashCoroutine != null)
            StopCoroutine(clickFlashCoroutine);

        clickFlashCoroutine = StartCoroutine(FlashThenDarken());

        region.ShowRegionPanel(cityIndex);

        if (Time.time - lastClickTime < doubleClickThreshold)
        {
            region.ZoomToCity(cityIndex);
        }

        lastClickTime = Time.time;
    }

    IEnumerator FlashThenDarken()
    {
        SetColor(originalColor);  // ???
        yield return new WaitForSeconds(flashDuration);

        SetColor(isHovered ? DarkenedColor() : originalColor);
    }


    void SetColor(Color color)
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = color;
        }
    }

    Color DarkenedColor()
    {
        Color newColor = new Color(
    originalColor.r * darkenFactor,
    originalColor.g * darkenFactor,
    originalColor.b * darkenFactor,
    originalColor.a
        );

        return newColor;
    }

    public string GetCityENName()
    {
        return region.GetRegionValue().GetCityENName(cityIndex);
    }

    public void ApplyZoomVisibility(float zoom)
    {
        CityVisibilityState newState;

        if (zoom >= 14f)
            newState = CityVisibilityState.Hidden;
        else if (zoom >= 8f)
            newState = CityVisibilityState.MainCity;
        else
            newState = CityVisibilityState.All;

            currentState = newState;

            switch (currentState)
            {
                case CityVisibilityState.Hidden:
                    HideCity();
                    break;
                case CityVisibilityState.MainCity:
                    ShowMainCity();
                    break;
                case CityVisibilityState.All:
                    ShowCity();
                    break;
            }
    }



    public void HideCity()
    {
        if (cityValue.HasTask())
        {
            ShowMarkSprite(true);
            cityIconSprite.gameObject.SetActive(false);
        } else
        {
            ShowMarkSprite(false);
            cityIconSprite.gameObject.SetActive(false);
        }
    }

     public void ShowMainCity()
    {
        if (cityValue.HasTask())
        {
            if (cityIndex == 0)
            {
                ShowMarkSprite(false);
                ShowCityIconSprite(true);
                markSpriteRenderer.gameObject.SetActive(true);
            }
            else
            {
                ShowMarkSprite(true);
                ShowCityIconSprite(false);
            }
        }
        else
        {
            ShowMarkSprite(false);
            ShowCityIconSprite(cityIndex == 0);
        }
    }

    public void ShowCity()
    {
        ShowCityIconSprite(true);
        markSprite.gameObject.SetActive(false);

        if (cityValue.HasTask())
        {
            markSpriteRenderer.gameObject.SetActive(true);
        }
        else
        {
            markSpriteRenderer.gameObject.SetActive(false);
        }
    }

    void ShowCityIconSprite(bool isActive)
    {
        if (string.IsNullOrEmpty(cityValue.GetCityENName())) {
            cityIconSprite.gameObject.SetActive(false);
        }
        else
        {
            cityIconSprite.gameObject.SetActive(isActive);
        }
    }

    void ShowMarkSprite(bool isActive)
    {
        markSprite.gameObject.SetActive(isActive);
        if (isActive) {
            cityMarkIntroPanelShow.SetIntroNameAndSprite(cityValue.GetCityNameWithColor(), cityValue.GetCityCountryIcon());
        }
    }


    public void SetActive(bool isActive)
    {
        if (string.IsNullOrEmpty(GetCityENName()))
        {
            gameObject.SetActive(false);
            return;
        }
        gameObject.SetActive(isActive);
        Sprite sprite = region.GetRegionValue().GetCityIcon(cityIndex);
        if (sprite != null)
        {
            CityInfoPanel.GetComponent<IntroPanelControl>().introImage.gameObject.SetActive(true);
            CityInfoPanel.GetComponent<IntroPanelControl>().introImage.sprite = sprite;

        } else
        {
            CityInfoPanel.GetComponent<IntroPanelControl>().introImage.gameObject.SetActive(false);

        }
    }

  

    public string CityConnectEditorHelper()
    {
        string regionName = this.region.GetRegionValue().GetRegionENName();
        string regionID = this.region.GetRegionValue().GetRegionID().ToString();
        string cityName = GetCityENName();
        string cityIndex = this.cityIndex.ToString();
        return $"{regionName},{regionID},{cityName},{cityIndex}";
    }

    public void SetPosition(Vector3 newPosition, Vector3 regionLocalScale)
    {
        transform.position = newPosition;
    }
}
