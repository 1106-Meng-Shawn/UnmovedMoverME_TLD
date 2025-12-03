using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using static ExcelReader;
using UnityEngine.Localization.Settings;

public class ENDPanelManager : MonoBehaviour
{
    public static ENDPanelManager Instance { get; private set; }


    public TextMeshProUGUI ENDTitle;
    public Image ENDBackground;


    public Button CharacterPanelButton;
    public Button SettlementPanelButton;
    public Button CloseButton;
    public Button CheckButton;

    public GameObject ResultPanel;
    public CharacterENDControl CharacterENDPanel;
    public SettlementPanelControl SettlementPanel;

    public Button isPlayerGEButtonInSettlement;
    public Button isPlayerBEButtonInSettlement;
    public Button isPlayerGEButtonInCharacetEND;
    public Button isPlayerBEButtonInCharacetEND;


    private bool isPlayerGE = false;
    private bool isPlayerBE = false;

    private ExcelGameENDData ENDData;
    private string epilogueFileName = null;
    private int sheetIndex = -1;

    public GameObject ENDPanel;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        CharacterPanelButton.onClick.AddListener(OnCharacterPanelButtonClick);
        SettlementPanelButton.onClick.AddListener(OnSettlementPanelButtonClick);
        CloseButton.onClick.AddListener(OnCloseButtonClick);
        CheckButton.onClick.AddListener(OnCheckButtonClick);

        isPlayerGEButtonInSettlement.onClick.AddListener(OnPlayerGEButtonClick);
        isPlayerGEButtonInCharacetEND.onClick.AddListener(OnPlayerGEButtonClick);
        isPlayerBEButtonInSettlement.onClick.AddListener(OnPlayerBEButtonClick);
        isPlayerBEButtonInCharacetEND.onClick.AddListener(OnPlayerBEButtonClick);

    }


    void OnPlayerGEButtonClick()
    {
        if (isPlayerGE)
        {
            isPlayerGE = false;
            CharacterENDPanel.RemovePlayerEND();
            return;
        }
        CharacterENDPanel.RemovePlayerEND();
        isPlayerBE = false;
        isPlayerGE = true;
        CharacterENDPanel.AddPlayerEND(true);
    }

    void OnPlayerBEButtonClick()
    {
        if (isPlayerBE)
        {
            isPlayerBE = false;
            CharacterENDPanel.RemovePlayerEND();
            return;
        }
        CharacterENDPanel.RemovePlayerEND();
        isPlayerGE = false;
        isPlayerBE = true;
        CharacterENDPanel.AddPlayerEND(false);

    }



    void OnCheckButtonClick()
    {
        if (sheetIndex == -1 || string.IsNullOrEmpty(epilogueFileName))
        {
            SceneManager.LoadScene("MainMenu");

        } else
        {
            Debug.Log($"why？{epilogueFileName}{sheetIndex}");
            TotalStoryManager.Instance.LoadStory(epilogueFileName, Constants.DEFAULT_START_LINE, sheetIndex);
            gameObject.SetActive(false);
        }
        ExtrasValue.Instance.SetGameValue(GameValue.Instance, CharacterENDPanel.GetCharacterENDSaveDatas());
    }



    private void Update()
    {
        if (Input.GetMouseButtonDown(1)) ActiveResultPanel();
    }


    void OnCloseButtonClick()
    {
        ResultPanel.SetActive(false);
    }

    void ActiveResultPanel()
    {
        ResultPanel.SetActive(!ResultPanel.activeSelf);
    }


    void OnCharacterPanelButtonClick()
    {
        CharacterENDPanel.ShowCharacterENDPanel();
        SettlementPanel.HideSettlementPanel();
    }

    void OnSettlementPanelButtonClick()
    {
        CharacterENDPanel.HideCharacterENDPanel();
        SettlementPanel.ShowSettlementPanel();
    }

    public void ShowENDPanel(string ENDKey, string epilogueFileName = null, int sheetIndex = -1)//, List<int> readIndex = null)
    {

       GameValue gameValue = GameValue.Instance;
        ENDPanel.gameObject.SetActive(true);
        SettlementPanel.InitSettlementPanel();
        CharacterENDPanel.InitCharacterENDPanel();
        SettlementPanel.ShowSettlementPanel();
        CharacterENDPanel.HideCharacterENDPanel();
        gameObject.SetActive(true);
    }


    void SetENDData(ExcelGameENDData ENDData)
    {
        this.ENDData = ENDData;
        SetENDTitle();
        SetENDBackground();
    }

    void SetENDTitle()
    {
        string currentLanguage = LocalizationSettings.SelectedLocale.Identifier.Code;
        ENDTitle.text = ENDData.ENDTitles.TryGetValue(currentLanguage, out var text) ? text : ENDData.ENDTitles["en"];
    }

    void SetENDBackground()
    {
        string BackgroundPath = $"MyDraw/{ENDData.backgroundImPath}";
        ENDBackground.sprite = Resources.Load<Sprite>(BackgroundPath);
    }

}

