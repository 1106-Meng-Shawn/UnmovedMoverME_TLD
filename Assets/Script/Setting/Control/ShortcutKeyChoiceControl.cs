using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum TextShortcutFunctions
{
    EmergencyExit,
    HideUI,
    ShowSettingPanel,
    ShowHistory,
    NextSentence,
    Vioceover
}

public class ShortcutKeyChoiceControl : MonoBehaviour
{
    [SerializeField] Image ShortCutIcon;
    [SerializeField] TMP_Text ShortCutText;

    [Header("Buttons")]
    [SerializeField] Buttons buttons;

    TextShortcutFunctions currentTextShortcutFunction;

    #region Buttons
    [System.Serializable]
    public class Buttons
    {
        public Button EmergencyExitButton;
        public Button HideUIButton;
        public Button ShowSettingPanel;
        public Button ShowHistoryPanel;
        public Button NextSentence;
        public Button Voiceover;
    }
    #endregion

    private void Awake()
    {
        InitButtons();
        UpdateShortcutUI();
    }

    void InitButtons()
    {
        buttons.EmergencyExitButton.onClick.AddListener(() => OnShortcutSelected(TextShortcutFunctions.EmergencyExit));
        buttons.HideUIButton.onClick.AddListener(() => OnShortcutSelected(TextShortcutFunctions.HideUI));
        buttons.ShowSettingPanel.onClick.AddListener(() => OnShortcutSelected(TextShortcutFunctions.ShowSettingPanel));
        buttons.ShowHistoryPanel.onClick.AddListener(() => OnShortcutSelected(TextShortcutFunctions.ShowHistory));
        buttons.NextSentence.onClick.AddListener(() => OnShortcutSelected(TextShortcutFunctions.NextSentence));
        buttons.Voiceover.onClick.AddListener(() => OnShortcutSelected(TextShortcutFunctions.Vioceover));
    }

    void OnShortcutSelected(TextShortcutFunctions func)
    {
        currentTextShortcutFunction = func;
        UpdateShortcutUI();
    }

    public void SetShortcutType(TextShortcutFunctions func)
    {
        currentTextShortcutFunction = func;
        UpdateShortcutUI();
    }

    void UpdateShortcutUI()
    {
        if (ShortCutText != null)ShortCutText.text = currentTextShortcutFunction.ToString();
    }
}
