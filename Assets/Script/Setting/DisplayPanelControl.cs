using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Settings;
using System;

public class DisplayPanelControl : SettingPanelBase
{
    [Header("Display Panel UI")]
    [SerializeField] private TMP_Dropdown languageDropdown;
    [SerializeField] private Toggle fullScreenToggle;
    [SerializeField] private Toggle windowedToggle;
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private TMP_Dropdown displayTimeDropdown;


    public TextMeshProUGUI playerNameTitel;
    public Button playerNameButton;

    public TextMeshProUGUI playerNameText;
    public InputPanelControl inputPanelControl;

   // public SettingsManager settingsManager;
    public GameValue gameValue;

    public DisplayTime displayTime;

    public DisplaySettingValue displaySettingValue;



    // Start is called before the first frame update
    public override void Init()
    {
        InitializationDisplay();
        languageDropdown.onValueChanged.AddListener((index) => {
            displaySettingValue.LanguageType = index;
            OnLanguageChanged(index);
            SettingValue.Instance.GetDisplaySettingValue().LanguageType = index;
        });

        fullScreenToggle.onValueChanged.AddListener((isOn) => {
            displaySettingValue.IsFullScreen = isOn;
            OnFullScreenToggleChanged(isOn);
            SettingValue.Instance.GetDisplaySettingValue().IsFullScreen = isOn;
        });

        windowedToggle.onValueChanged.AddListener((isOn) => {
            displaySettingValue.IsFullScreen = !isOn;
            OnWindowedToggleChanged(isOn);
            SettingValue.Instance.GetDisplaySettingValue().IsFullScreen = !isOn;
        });

        resolutionDropdown.onValueChanged.AddListener((index) => {
            displaySettingValue.ResolutionType = index;
            OnResolutionChanged(index);
            SettingValue.Instance.GetDisplaySettingValue().ResolutionType = index;

        });

        displayTimeDropdown.onValueChanged.AddListener((index) => {
            displaySettingValue.DisplayTimeType = index;
            OnDisplayTimeChanged(index);
            SettingValue.Instance.GetDisplaySettingValue().DisplayTimeType = index;
        });


        playerNameButton.onClick.AddListener(OnPlayerNameButtonClick);
        OnResolutionChanged(displaySettingValue.ResolutionType);
    }


    public void InitializationDisplay()
    {
        SettingsManager settingsManager = SettingsManager.Instance;
        displaySettingValue = settingsManager.GetDisplaySettingValue();

        languageDropdown.value = displaySettingValue.LanguageType;
        OnLanguageChanged(languageDropdown.value);

        fullScreenToggle.isOn = displaySettingValue.IsFullScreen;

        OnFullScreenToggleChanged(fullScreenToggle.isOn);
        windowedToggle.isOn = !displaySettingValue.IsFullScreen;
        resolutionDropdown.value = displaySettingValue.ResolutionType;

        OnResolutionChanged(resolutionDropdown.value);

        displayTimeDropdown.value = displaySettingValue.DisplayTimeType;

        OnDisplayTimeChanged(displayTimeDropdown.value);


    }



    void ShowPlayerName()
    {
        SettingsManager settingsManager = SettingsManager.Instance;
        string playerName = SettingValue.Instance.GetPlayerName();
        if (string.IsNullOrWhiteSpace(playerName))
        {
            // playerNameText.gameObject.SetActive(false);
            playerNameText.text = "<i>Enter...</i>";
        } else
        {
            playerNameText.text = $"<i>{playerName}</i>";
        }

        settingsManager.NotifyInputPanelState(false);

        if (gameValue != null) gameValue.SetPlayerName(playerName);
    }

    void OnPlayerNameButtonClick()
    {
        SettingsManager settingsManager = SettingsManager.Instance;

        settingsManager.NotifyInputPanelState(true);
        string playerName = SettingValue.Instance.GetPlayerName();
        inputPanelControl.ShowInputPanel(SetPlayerName, false,SettingsManager.Instance.GetPlayerName());
    }


    void SetPlayerName()
    {
        string playerNewName = inputPanelControl.inputField.text;
        SettingsManager.Instance.SetPlayerName(playerNewName);
    }


    public override void OnDefaultButtonClick()
    {
        //nothing yet
    }

    public void OnLanguageChanged(int index)
    {
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[index];
    }

    private int GetLanguageIndex()
    {
        string currentLanguage = LocalizationSettings.SelectedLocale.Identifier.Code;
        switch (currentLanguage)
        {
            case LanguageCode.EN:
                return 2;
            case LanguageCode.CNS:
                return 0;
            case LanguageCode.CNT:
                return 1;
            case LanguageCode.JA:
                return 3;
            default:
                return 2;
        }
    }


    private void OnFullScreenToggleChanged(bool isOn)
    {
        if (isOn && Screen.fullScreen == false)
        {
            Screen.fullScreen = true;
            windowedToggle.isOn = false;
        }
        else if (!isOn && Screen.fullScreen == true)
        {
            Screen.fullScreen = false;
            windowedToggle.isOn = true;
        }
    }

    private void OnWindowedToggleChanged(bool isOn)
    {
        if (isOn && Screen.fullScreen == true)
        {
            Screen.fullScreen = false;
            fullScreenToggle.isOn = false;
        }
        else if (!isOn && Screen.fullScreen == false)
        {
            fullScreenToggle.isOn = true;
            Screen.fullScreen = true;
        }
    }

    public void OnDisplayTimeChanged(int index)
    {
        if (displayTime == null) return;
        displayTime.SetTimeType(index);

    }

    public void OnResolutionChanged(int index)
    {
        if (Screen.fullScreen == false)
        {
            switch (index)
            {
                case 0:
                    SetResolution(854, 480); break;
                case 1:
                    SetResolution(1280, 720); break;
                case 2:
                    SetResolution(1600, 900); break;
                case 3:
                    SetResolution(1920, 1080); break;
                case 4:
                    SetResolution(2560, 1440); break;
                case 5:
                    SetResolution(3840, 2160); break;
                default:
                    SetResolution(1920, 1080); break;

            }
        }
    }

    public void SetResolution(int width, int height)
    {
        Screen.SetResolution(width, height, Screen.fullScreen);
    }

}


public class DisplaySettingValue
{
    public int LanguageType { get; set; } = 2;
    public bool IsFullScreen { get; set; } = true;
    public int ResolutionType { get; set; } = 3;
    public int DisplayTimeType { get; set; } = 1;

    public DisplaySettingValue() { }


}
