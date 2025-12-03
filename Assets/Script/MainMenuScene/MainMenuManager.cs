using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{

    [SerializeField] private Button StartButton;
    [SerializeField] private Button ContinueButton;
    [SerializeField] private Button LoadButton;
    [SerializeField] private Button ExtrasButton;
    [SerializeField] private Button SettingButton;
    [SerializeField] private Button ExitButton;

    [SerializeField] private InputPanelControl InputPanelControl;

    private void Awake()
    {
        AddAllButtonsListener();
    }

    #region Buttons

    void AddAllButtonsListener()
    {
        StartButton.onClick.AddListener(OnStartButtonClick);
        ContinueButton.onClick.AddListener(OnContinueButtonClick);
        LoadButton.onClick.AddListener(OnLoadButtonClick);
        ExtrasButton.onClick.AddListener(OnExtrasButtonClick);
        SettingButton.onClick.AddListener(OnSettingButtonClick);
        ExitButton.onClick.AddListener(OnExitButtonClick);
    }

    void OnStartButtonClick()
    {
        StartNewGame();
    }

    void OnContinueButtonClick()
    {
        SaveData saveData = SettingValue.Instance.settingValueData.saveData;
        SceneTransferManager.Instance.LoadScene(Scene.GameScene,saveData);
    }

    void OnLoadButtonClick()
    {
        LoadPanelManage.Instance.ShowLoadPanel();
    }

    void OnExtrasButtonClick()
    {
        ExtrasPanelManager.Instance.ShowExtrasPanel();
    }

    void OnSettingButtonClick()
    {
        SettingsManager.Instance.OpenPanel();
    }

    public void OnExitButtonClick()
    {
        SceneTransferManager.Instance.ExitGame();
    }
    void StartNewGame()
    {
        if (SettingsManager.Instance.IsMultiplePlaythroughs())
        {
            MultiplePlaythroughsManager.Instance.ShowPanel();

        } else
        {
            string originalString = SettingsManager.Instance.GetPlayerName();
            InputPanelControl.ShowInputPanel(InitGame,false, originalString);
        }
    }

    void InitGame()
    {
        GameValue.Instance.Init();
        SceneTransferManager.Instance.StartNewGame(InputPanelControl.GetInputText());
    }

    #endregion

    void Start()
    {
        InitButtons();
    }

    void InitButtons()
    {
        ContinueButton.gameObject.SetActive(SettingsManager.Instance.HasSaveData());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
