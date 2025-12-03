using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ReminderPanelControl : MonoBehaviour
{
    public static ReminderPanelControl Instance { get; private set; }

    [Header("UI Elements")]
    public GameObject ReminderPanel;
    public Button CheckButton;
    public Button CancelButton;
    public TextMeshProUGUI TitleText;
    public TextMeshProUGUI DescribeText;

    private Action onConfirmAction; // ???????

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        ReminderPanel.SetActive(false);
    }

    private void Start()
    {
        CancelButton.onClick.AddListener(ClosePanel);
        CheckButton.onClick.AddListener(OnConfirmButtonClick);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1)) ClosePanel(); // ????
    }

    /// <summary>
    /// ????????
    /// </summary>
    public void ShowWarDeclarationReminder(CityValue enemyCity)
    {
        const string TITLE = "Declaration of War!!!";

        TitleText.text = TITLE.ToUpper();
        DescribeText.text = string.Format(
            "Your country - {0} will declare war on {1}.\nDo you accept?",
            GameValue.Instance.GetPlayerCountryENName(),
            enemyCity.GetCityCountryNameWithColor()
        );

        onConfirmAction = () =>
        {
            DeclareWar(enemyCity);
        };

        ShowReminderPanel();
    }

    private void ShowReminderPanel()
    {
        ReminderPanel.SetActive(true);
    }

    private void OnConfirmButtonClick()
    {
        onConfirmAction?.Invoke();
        ClosePanel();
    }

    private void DeclareWar(CityValue enemyCity)
    {
        CountryManager countryManager = GameValue.Instance.GetCountryManager();
        countryManager.SetRelation(GameValue.Instance.GetPlayerCountryENName(), enemyCity.cityCountry, RelationshipType.War);

        BattlePanelManage.Instance.ShowBattlePanel(enemyCity.regionValue, enemyCity.cityIndex, false);
        NotificationManage.Instance.ShowToTopByKey(NotificationKeyConstants.War_WithCountry,enemyCity.cityCountry);
        CityConnetManage.Instance.UpLineShow();
    }

    private void ClosePanel()
    {
        onConfirmAction = null;
        ReminderPanel.SetActive(false);
    }
}
