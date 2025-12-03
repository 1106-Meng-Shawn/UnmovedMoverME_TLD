using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static FormatNumber;
using static GetColor;
using static GetSprite;
using Unity.VisualScripting;
using Unity.Collections.LowLevel.Unsafe;
using System;


public class BuildPanelTopRowControl : MonoBehaviour
{
    public Image regionIcon;
    public TextMeshProUGUI regionName;

    public Image lordIcon;
    public TextMeshProUGUI lordName;

    public TextMeshProUGUI taxRateText;
    public Button increaseTaxRateButton;
    public Button decreaseTaxRateButton;

    public TextMeshProUGUI buildMaxText;
    public TextMeshProUGUI buildCurrentText;

    public List<Button> valueTypeButtons;
    private int currentValueType  = 5; // 5 is population, 0 is food, 1 is science , 2 is politics, 3 is gold , 4 is faith

    public Button buildButton;
    public CharacterAssistRowControl characterAssistRowControl;

    private List<TextMeshProUGUI> valueText = new List<TextMeshProUGUI>();
    private List<Image> valueIcon = new List<Image>();
    private List<CanvasGroup> canvasGroup = new List<CanvasGroup>();         // 用于控制透明度


    //    public GameObject buildValueObject;


    public Button lockButton;
    private bool isLock; public bool GetIsLock() { return isLock; }

    // 0 max, 1 current, 2 growth rate,3 parameter? , 4 new turn value,5 this turn take value,6 next turn will tak value? 
    // 0 max population, 1 current population , 2 rowth rate , 3 new turn population,4 surrpot rate?,5 Maximum recruitable population,6 current recruitable population? 

    public List<Transform> valueObjects;

    private Coroutine displayLoopCoroutine;
    private bool isFadingCancelled = false;
    public float fadeDuration = 0.1f;         // 渐变时间
    public float displayDuration = 0.8f;      // 显示时间


    private RegionValue regionValue;
    private GameValue gameValue;
    private CityValue cityValue;

    Sprite selSprite; // just this one look like better in Ui
    Sprite unSelSprite;

    private void Awake()
    {
        selSprite = Resources.Load<Sprite>("MyDraw/UI/Other/SettingBoxUnsel");
        unSelSprite = Resources.Load<Sprite>("MyDraw/UI/GameUI/Box");
    }




    private void Start()
    {
        InitValueOj();

        increaseTaxRateButton.onClick.AddListener(OnIncreaseTaxRateButtonClick);
        decreaseTaxRateButton.onClick.AddListener(OnDecreaseTaxRateButtonClick);

        buildButton.onClick.AddListener(OnBuildButtonClick);

        InitLockButton();

        for (int i = 0; i < valueTypeButtons.Count; i++)
        {
            int index = i;
            valueTypeButtons[i].onClick.AddListener(() => OnValueTypeButtonClick(index));
        }


    }

    void InitValueOj()
    {
        valueText.Clear();
        valueIcon.Clear();
        canvasGroup.Clear();

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
            canvasGroup.Add(group);
        }
    }

    void InitLockButton()
    {
        UpLockSprite();
        lockButton.onClick.AddListener(OnLockButtonClick);

    }
    void UpLockSprite()
    {
        string iconPath = $"MyDraw/UI/Other/";

        if (isLock)
        {
            lockButton.image.sprite = Resources.Load<Sprite>(iconPath + "Lock");
            lockButton.gameObject.GetComponent<ButtonEffect>().SetChangeSprite(Resources.Load<Sprite>(iconPath + "LockSel"), Resources.Load<Sprite>(iconPath + "Lock"));

        }
        else
        {
            lockButton.image.sprite = Resources.Load<Sprite>(iconPath + "Unlock");
            lockButton.gameObject.GetComponent<ButtonEffect>().SetChangeSprite(Resources.Load<Sprite>(iconPath + "UnlockSel"), Resources.Load<Sprite>(iconPath + "Unlock"));

        }
    }

    public void SetRegionValue(RegionValue regionValue,CityValue cityValue)
    {
        this.regionValue = regionValue;
        this.cityValue = cityValue;


        FadingCancelled(); // 停止旧协程、立刻显示当前值
        displayLoopCoroutine = StartCoroutine(InitialDisplayThenLoop()); // 启动新的等待+切换协程

        UpUI(); // 你的其他 UI 更新逻辑
    }
    void OnBuildButtonClick()
    {
        characterAssistRowControl.ToggleBuildRow();
    }

    void OnIncreaseTaxRateButtonClick()
    {

    }

    void OnDecreaseTaxRateButtonClick()
    {

    }


    public void UpUI()
    {
        SetRegionTitle();
        SetRegionLordText();
        SetNormalRegionValue();
        UpValueTypeUI(currentValueType);

    }

    void SetRegionTitle()
    {
        Debug.Log(cityValue.GetCityCountry());
        regionIcon.sprite = GameValue.Instance.GetCountryIcon(cityValue.GetCityCountry());
        //  regionName.text = regionValue.GetRegionName();
        regionName.text = GetCountryColorString(cityValue.GetCityName(), cityValue.GetCityCountry());
      

    }
    void SetRegionLordText()
    {
        Character lord = regionValue.lord;
        if (lord == null)
        {
            lordIcon.gameObject.SetActive(false);
            lordName.gameObject.SetActive(false);
        }
        else
        {
            lordIcon.gameObject.SetActive(true);
            lordName.gameObject.SetActive(true);


            lordIcon.sprite = lord.icon;
            lordName.text = lord.GetCharacterName();
        }
    }
    void SetNormalRegionValue()
    {
        if (gameValue == null) gameValue = FindObjectOfType<GameValue>();
      //  taxRateText.text = regionValue.
        taxRateText.text = regionValue.GetTaxRateString();
        //buildMaxText.text = regionValue.max;
        //buildCurrent.text = regionValue.current

        buildButton.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = gameValue.GetResourceValue().Build.ToString();
    }


  

    public void UpValueTypeUI(int valueType)
    {
        if (valueIcon.Count == 0 || valueText.Count == 0) InitValueOj();
        SetValueType(valueType);

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

    // 0 max, 1 current, 2 growth rate,3 parameter? , 4 this turn take value，5 new turn value ,6 next turn will tak value? 
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


    public void OnLockButtonClick()
    {
        isLock = !isLock;
        UpLockSprite(); // 更新锁图标

        if (isLock)
        {
            FadingCancelled();
        }
        else
        {
            if (displayLoopCoroutine == null)
            {
                displayLoopCoroutine = StartCoroutine(InitialDisplayThenLoop());
            }
        }
    }

    public void OnValueTypeButtonClick(int index)
    {
        SetValueType(index);

        FadingCancelled(); // 立即停止切换并显示选中值
        displayLoopCoroutine = StartCoroutine(InitialDisplayThenLoop()); // 启动新的等待+切换协程

    }

    void SetValueType(int index)
    {
        valueTypeButtons[currentValueType].gameObject.GetComponent<ButtonEffect>().SetChangeSprite(selSprite, unSelSprite);
        currentValueType = index;
        valueTypeButtons[currentValueType].gameObject.GetComponent<ButtonEffect>().SetChangeSprite(selSprite, selSprite);

    }


    // ----------------- Fading & Switching Logic -----------------

    private void FadingCancelled()
    {
        isFadingCancelled = true;

        if (displayLoopCoroutine != null)
        {
            StopCoroutine(displayLoopCoroutine);
            displayLoopCoroutine = null;
        }

        foreach (var cg in canvasGroup)
        {
            cg.alpha = 1f;
        }

        UpValueTypeUI(currentValueType);
    }

    private IEnumerator InitialDisplayThenLoop()
    {
        yield return new WaitForSeconds(1.5f);

        if (!isLock)
        {
            displayLoopCoroutine = StartCoroutine(DisplayLoop());
        }
        else
        {
            displayLoopCoroutine = null;
        }
    }

    private IEnumerator DisplayLoop()
    {
        while (true)
        {
            if (isLock || valueObjects.Count == 0)
            {
                yield return null;
                continue;
            }

            // Fade Out
            yield return StartCoroutine(FadeCanvasGroup(canvasGroup, 1f, 0f, 0.75f));
            if (isFadingCancelled) yield break;

            // 切换 valueType
            SetValueType((currentValueType + 1) % valueTypeButtons.Count);
            UpValueTypeUI(currentValueType);

            // Fade In
            yield return StartCoroutine(FadeCanvasGroup(canvasGroup, 0f, 1f, 0.75f));
            if (isFadingCancelled) yield break;

            // 停留展示
            yield return new WaitForSeconds(1.5f);
        }
    }

    private IEnumerator FadeCanvasGroup(List<CanvasGroup> group, float start, float end, float duration)
    {
        float elapsed = 0f;
        isFadingCancelled = false;

        while (elapsed < duration)
        {
            if (isFadingCancelled)
                yield break;

            for (int i = 0; i < group.Count; i++)
            {
                group[i].alpha = Mathf.Lerp(start, end, elapsed / duration);
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        for (int i = 0; i < group.Count; i++)
        {
            group[i].alpha = end;
        }
    }


}

