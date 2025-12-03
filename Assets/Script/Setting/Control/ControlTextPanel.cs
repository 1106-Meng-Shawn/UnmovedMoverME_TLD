using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControlTextPanel : ControlPanelBase
{
    [SerializeField] TextBackgroundControl textBackgroundControl;

    [Header("Buttons")]
    [SerializeField] RightButtons rightButtons;


    #region Buttons
    [System.Serializable]
    public class RightButtons
    {
        public Button SaveButton;
        public Button SaveQuickButton;
        public Button LoadButton;
        public Button LoadQuickButton;
        public Button SettingButton;

        public ShortcutKeyControl LockShortCutKey;
        public ShortcutKeyControl LogShortCutKey;
        public ShortcutKeyControl AutoShortCutKey;
        public ShortcutKeyControl SkipShortCutKey;
        public ShortcutKeyControl SkipToChoiceShortCutKey;
        public ShortcutKeyControl SkipNodeShortCutKey;

    }
    #endregion


    void InitRightButtons()
    {
        rightButtons.SaveButton.onClick.AddListener(() => SettingsManager.Instance.OpenGeneralControlPanel());
        rightButtons.SaveQuickButton.onClick.AddListener(() => SettingsManager.Instance.OpenGeneralControlPanel());
        rightButtons.LoadButton.onClick.AddListener(() => SettingsManager.Instance.OpenGeneralControlPanel());
        rightButtons.LoadQuickButton.onClick.AddListener(() => SettingsManager.Instance.OpenGeneralControlPanel());
        rightButtons.SettingButton.onClick.AddListener(() => SettingsManager.Instance.OpenGeneralControlPanel());

    }

    void SetShortcutkeyData()
    {
        SettingShortcutkeyData settingShortcutkeyData = SettingValue.Instance.settingValueData.settingShortcutkeyData;
        TextShortcutkeyData shortcutkeyData = SettingValue.Instance.settingValueData.settingShortcutkeyData.TextShortcutkeyData;
        rightButtons.LockShortCutKey.BindToKey(() => shortcutkeyData.LockKey,(value) => shortcutkeyData.LockKey = value, settingShortcutkeyData.CreateValidator(ShortcutKeyCategory.Text));
        rightButtons.LogShortCutKey.BindToKey(() => shortcutkeyData.LogKey,(value) => shortcutkeyData.LogKey = value, settingShortcutkeyData.CreateValidator(ShortcutKeyCategory.Text));
        rightButtons.AutoShortCutKey.BindToKey(() => shortcutkeyData.AutoKey,(value) => shortcutkeyData.AutoKey = value, settingShortcutkeyData.CreateValidator(ShortcutKeyCategory.Text));
        rightButtons.SkipShortCutKey.BindToKey(() => shortcutkeyData.SkipKey,(value) => shortcutkeyData.SkipKey = value, settingShortcutkeyData.CreateValidator(ShortcutKeyCategory.Text));
        rightButtons.SkipToChoiceShortCutKey.BindToKey(() => shortcutkeyData.SkipToChoiceKey,(value) => shortcutkeyData.SkipToChoiceKey = value, settingShortcutkeyData.CreateValidator(ShortcutKeyCategory.Text));
        rightButtons.SkipNodeShortCutKey.BindToKey(() => shortcutkeyData.SkipNodeKey,(value) => shortcutkeyData.SkipNodeKey = value, settingShortcutkeyData.CreateValidator(ShortcutKeyCategory.Text));
    }




    public override void OpenPanel()
    {
        base.OpenPanel();
        textBackgroundControl.OpenPanel("Anna");
    }


    void Start()
    {
        InitRightButtons();
        SetShortcutkeyData();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public override void RestorePanel()
    {
        SetShortcutkeyData();
    }

    public override void OnDefaultButtonClick()
    {
        SettingValue.Instance.settingValueData.settingShortcutkeyData.TextShortcutkeyData = new TextShortcutkeyData();
        SetShortcutkeyData();
    }
}
