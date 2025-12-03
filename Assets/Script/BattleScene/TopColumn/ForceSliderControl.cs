using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static GetColor;
using static GetSprite;


public class ForceSliderControl : MonoBehaviour
{
    public static ForceSliderControl Instance { get; private set; }

    public TMP_Text playerForceText;
    public TMP_Text enemyForceText;
    public TMP_Text playerPercentText;
    public TMP_Text enemyPercentText;

    public Slider forceSlider;

    private List<BattleCharacterValue> playerList;
    private List<BattleCharacterValue> enemyList;

    public Image battleCountryImage;
    public Image battleCityImage;
    public TextMeshProUGUI battleUpText;
    public TextMeshProUGUI battleDownText;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void InitTopRow()
    {
        InitForceSilde();
        SetBattleTopInfo();
    }


    void InitForceSilde()
    {
        playerList = BattleManage.Instance.playerBattleValue;
        enemyList = BattleManage.Instance.enemyBattleValue;

        // ??????
        foreach (var character in playerList)
        {
            character.OnValueChanged += UpdateForceDisplay;
        }

        foreach (var character in enemyList)
        {
            character.OnValueChanged += UpdateForceDisplay;
        }

        UpdateForceDisplay(); // ???
    }

    void UpdateForceDisplay()
    {
        int playerForce = 0;
        foreach (var c in playerList) playerForce += c.GetForce();

        int enemyForce = 0;
        foreach (var c in enemyList) enemyForce += c.GetForce();

        playerForceText.text = GetValueColorString(playerForce.ToString("N0"), ValueColorType.Pop);
        enemyForceText.text = GetValueColorString(enemyForce.ToString("N0"),ValueColorType.Pop);

        int total = playerForce + enemyForce;
        float percent = total > 0 ? (float)playerForce / total : 0.5f;

        playerPercentText.text = GetValueColorString(Mathf.RoundToInt(percent * 100f) + "%",ValueColorType.Pop);
        enemyPercentText.text = GetValueColorString(Mathf.RoundToInt((1 - percent) * 100f) + "%", ValueColorType.Pop);

        forceSlider.value = percent;
    }

    void SetBattleTopInfo()
    {
        BattleData battleData = GameValue.Instance.GetBattleData();
        if (battleData.isExplore)
        {

        } else
        {
            battleUpText.text = battleData.battleRegion.GetRegionNameWithColor();
            

            battleCountryImage.sprite =  GameValue.Instance.GetCountryIcon(battleData.battleRegion.GetCityCountry(battleData.cityIndex));
            battleCityImage.sprite = GetCitySprite(battleData.cityIndex);
            //  battleDownText.text = GameValue.Instance.GetCountryColorString(battleData.battleRegion.GetCityName(battleData.cityIndex), battleData.battleRegion.GetCityCountry(battleData.cityIndex));
            string CityName = battleData.battleRegion.GetCityName(battleData.cityIndex);
            string CityCountry = battleData.battleRegion.GetCityCountry(battleData.cityIndex);
            battleDownText.text = GetCountryColorString(battleData.battleRegion.GetCityName(battleData.cityIndex), battleData.battleRegion.GetCityCountry(battleData.cityIndex));
        }
    }
}


/*using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class ForceSliderControl : MonoBehaviour
{
    public TMP_Text playerForceText;
    public TMP_Text enemyForceText;
    public TMP_Text playerPercentText;
    public TMP_Text enemyPercentText;

    public Slider forceSlider;

    public int BattleCity;
    public RegionValue battleRegion;
    public bool isPersonBattle;


    private void UpdateSlider()
    {
        playerForce = isPersonBattle ? CalculateTotal(playerValueList, true) : CalculateTotal(playerValueList, false);
        enemyForce = isPersonBattle ? CalculateTotal(enemyValueList, true) : CalculateTotal(enemyValueList, false);

        playerForceText.text = playerForce.ToString();
        enemyForceText.text = enemyForce.ToString();

        float totalForce = playerForce + enemyForce;
        if (totalForce > 0)
        {
            float adjustedSliderValue = 0;
            if (BattleCity != 0)
            {
                adjustedSliderValue = (float)playerForce / totalForce - BattleParameter(BattleCity, battleRegion);

            } else
            {
                adjustedSliderValue = (float)playerForce / totalForce; // need change by level maybe

            }
            forceSlider.value = Mathf.Clamp(adjustedSliderValue, 0.01f, 0.99f);
        }

        playerPercentText.text = ToPercentString(forceSlider.value);
        enemyPercentText.text = ToPercentString(1 - forceSlider.value);
    }

    public void SetForceSlider(int battleCity, RegionValue battleRegion)
    {
        playerForce = CalculateTotal(playerValueList, false);
        enemyForce = CalculateTotal(enemyValueList, false);
        this.battleRegion = battleRegion;
        UpdateSlider();
    }

    public void SetHealthSlider(int level, RegionValue battleRegion)
    {
        playerForce = CalculateTotal(playerValueList, true);
        enemyForce = CalculateTotal(enemyValueList, true);
        this.battleRegion = battleRegion;

        UpdateSlider();
    }

    private int CalculateTotal(List<BattleCharacterValue> characters, bool isHealth)
    {
        int total = 0;
        foreach (var character in characters)
        {
            if (character != null && character.characterValue != null)
            {
                total += isHealth ? character.characterValue.Health : character.characterValue.Force;
            }
        }
        return total;
    }

    private float BattleParameter(int battleCity, RegionValue battleRegion)
    {
        float[,] parameters = new float[,] {
            {0.25f, 0.2f, 0.1f},  // cityNum == 3
            {0.3f, 0.15f, 0},     // cityNum == 2
            {0.35f, 0, 0}         // cityNum == 1
        };

            int cityIndex = battleRegion.GetCityCountryNum() - 1;
            if (cityIndex >= 0 && cityIndex < parameters.GetLength(0) && BattleCity >= 0 && BattleCity < parameters.GetLength(1))
            {
                return parameters[cityIndex, BattleCity];
            }
        

        return 0f;
    }

    private string ToPercentString(float value)
    {
        return (value * 100).ToString("F1") + "%";
    }*/

