using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using static GetColor;

public class CityEventControl : MonoBehaviour
{

    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private bool isHovered = false;
    public float darkenFactor = 0.6f;
    public CityConnection cityConnection;

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
        SetCityEventName(); 
    }




    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
        SetCityEventName();

    }

    void OnMouseEnter()
    {
        isHovered = true;
        SetColor(DarkenedColor());
    }

    void OnMouseExit()
    {
        isHovered = false;
        SetColor(originalColor);
    }

    void OnMouseDown()
    {
        if (IsPointerOverUI()) return;

        cityConnection.BattleHappend();
        GetComponent<IntroPanelShow>().DestroyPanel();
    }

    private bool IsPointerOverUI()
    {
        return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
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


    void SetCityEventName()
    {
        bool aIsPlayer = cityConnection.cityA.cityCountry == GameValue.Instance.GetPlayerCountryENName();

        CityValue player = aIsPlayer ? cityConnection.cityA : cityConnection.cityB;
        CityValue enemy = aIsPlayer ? cityConnection.cityB : cityConnection.cityA;

        string playerCity = GetCountryColorString(player.GetCityName(), player.GetCityCountry());
        string enemyCity = GetCountryColorString(enemy.GetCityName(), enemy.GetCityCountry());

        gameObject.GetComponent<IntroPanelShow>().SetIntroName($"{playerCity} > {enemyCity}");
    }

}
