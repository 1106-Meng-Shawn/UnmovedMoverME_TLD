using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static GetSprite;
using static GetColor;
using static BattlePanelManage;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;
using Unity.VisualScripting.Antlr3.Runtime;


public class ExploreUI : MonoBehaviour
{

    public static ExploreUI Instance { get; private set; }

    public Image exploreRegionIcon;
    public Image exploreCityIcon;

    public TextMeshProUGUI regionTitle;

    public Button characterButton;
    public Button itemButton;

    public Button ExitExploreButton;

    public BattlePanelManage battlePanelManage;

    public Image countryIcon;

    public TextMeshProUGUI gameTimeTitle;
    public TextMeshProUGUI levelText;

    public List<ExploreLevel> levelLists = new List<ExploreLevel>();

    private GameValue gameValue;
    private ExploreData exploreData;
    RegionValue region;
    int cityIndex;

    [SerializeField] private GameObject selPanelCorner;
    bool isPulsing = false;
    private Coroutine cornerPulseCoroutine;
    public float duration = 0.5f;


    public Image reasonIcon;
    public Image reasonBox;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void OnDisable()
    {
        LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
    }

    private void OnDestroy()
    {
        LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;

    }

    private void Start()
    {
        InitValue();
        TitleSet();
        InitButtons();
        StartCornersPulse();
    }

    void InitValue()
    {
        gameValue = GameValue.Instance;
        exploreData = gameValue.GetExploreData();
        region = exploreData.exploreRegion;
        cityIndex = exploreData.cityIndex;

    }

    #region UI

    void OnLocaleChanged(UnityEngine.Localization.Locale newLocale)
    {
        TitleSet();

    }

    void InitButtons()
    {
        characterButton.onClick.AddListener(OnCharacterButtonClick);
        itemButton.onClick.AddListener(OnItemButtonClick);
        ExitExploreButton.onClick.AddListener(OnExitExploreButtonClick);

    }

    void OnCharacterButtonClick()
    {
        battlePanelManage.ShowBattlePanel(region,cityIndex,true);
    }

    void OnItemButtonClick()
    {
        battlePanelManage.itemPanelManger.OpenPanel();
    }

    void OnExitExploreButtonClick()
    {
        SceneManager.LoadScene("GameScene");

    }


    void TitleSet()
    {

        var cityCountry = region.GetCityCountry(cityIndex);
        var cityColor = GameValue.Instance.GetCountryColor(cityCountry);
        string cityHex = ColorUtility.ToHtmlStringRGB(cityColor);
        string cityName = region.GetCityName(cityIndex);
        int exploreLevel = region.GetCityValue(cityIndex).GetExploreLevel();

        reasonIcon.sprite = GetSeasonSprite(gameValue.GetCurrentSeason());
        exploreCityIcon.sprite = GetCitySprite(cityIndex);
        reasonBox.sprite = GetSeasonSpriteBox(gameValue.GetCurrentSeason());


        exploreRegionIcon.sprite = GameValue.Instance.GetCountryIcon(cityCountry);
        regionTitle.text = $"<color=#{cityHex}>{cityName}</color>";
        levelText.text = exploreLevel.ToString();

        countryIcon.sprite = GameValue.Instance.GetCountryIcon(gameValue.GetPlayerCountryENName());
        gameTimeTitle.text = gameValue.GetTurnString();
    }


    public void StartCornersPulse()
    {
        isPulsing = true;
        cornerPulseCoroutine = StartCoroutine(CornersPulseRoutine());
    }

    private IEnumerator CornersPulseRoutine()
    {
        RectTransform rt = selPanelCorner.GetComponent<RectTransform>();
        float range = 10f;

        Vector2 baseOffsetMin = Vector2.zero;
        Vector2 baseOffsetMax = Vector2.zero;

        // ????????????
        float startTime = Mathf.Floor(Time.time); // ?????

        while (isPulsing)
        {
            float t = Mathf.PingPong(Time.time - startTime, duration) / (duration);

            float offset = Mathf.Lerp(-range, range, t);

            rt.offsetMin = baseOffsetMin + new Vector2(-offset, -offset);
            rt.offsetMax = baseOffsetMax + new Vector2(offset, offset);

            yield return null;
        }

    }

    public void ChangeDuration(float duration)
    {
        this.duration = duration;
        if (isPulsing)
        {
            StopCoroutine(cornerPulseCoroutine);
            cornerPulseCoroutine = StartCoroutine(CornersPulseRoutine()); 
        }

    }
    #endregion

    void InitList()
    {
        for (int i = 0; i < levelLists.Count -1; i++) {

            for (int j = 0; j < levelLists[i].exploreCards.Count; j++)
            {
                List<ExploreCardNode> nextCards = new List<ExploreCardNode> { levelLists[i+1].exploreCards[j], levelLists[i+1].exploreCards[j+1], levelLists[i+1].exploreCards[j+2] };
                levelLists[i].exploreCards[j].SetNextCards(nextCards);
            }
            
        }
    }

    public void GenerateTree(int level)
    {
        InitList();
        for (int i = 0; i < levelLists[0].exploreCards.Count; i++)
        {
            int firstLevel = level + 1;
            ExploreCardData exploreCardData = new ExploreCardData(firstLevel);
            levelLists[0].exploreCards[i].SetExploreCardData(exploreCardData);
        }
        ExploreRecordData exploreRecordData = new ExploreRecordData(level);
        ExploreRightCol.Instance.SetExploreRecord(exploreRecordData);



    }

    public void SetExploreCardData(ExploreCardData exploreCardData)
    {
        region.GetCityValue(cityIndex).exploreLevel = exploreCardData.level;
        levelText.text = region.GetCityValue(cityIndex).exploreLevel.ToString();

        levelLists[0].exploreCards[0].SetExploreCardData(exploreCardData.nextCardData[0]);
        levelLists[0].exploreCards[1].SetExploreCardData(exploreCardData.nextCardData[1]);
        levelLists[0].exploreCards[2].SetExploreCardData(exploreCardData.nextCardData[2]);

        ExploreRecordData exploreRecordData = new ExploreRecordData(exploreCardData.level, exploreCardData);
        ExploreRightCol.Instance.SetExploreRecord(exploreRecordData);


    }

}
