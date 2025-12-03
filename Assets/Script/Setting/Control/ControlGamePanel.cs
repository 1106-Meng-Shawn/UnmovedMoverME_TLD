using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ControlGamePanel : ControlPanelBase
{
    [Header("Buttons")]
    [SerializeField] RightTopButtons rightTopButtons;
    [SerializeField] ShortcutKeyControls shortcutKeyControls;
    [SerializeField] CameraShortcutKeys cameraShortcutKeys;


    #region Buttons
    [System.Serializable]
    public class RightTopButtons
    {
        public Button SaveButton;
        public Button LoadButton;
        public Button MusicButton;
        public Button SettingButton;
        public Button GuideButton;
        public Button MenuButton;
    }

    [System.Serializable]
    public class ShortcutKeyControls
    {
        public ShortcutKeyControl NextTurn;
        public ShortcutKeyControl RightCol;
        public ShortcutKeyControl Scout;
        public ShortcutKeyControl Build;
        public ShortcutKeyControl Negotiation;

        public ShortcutKeyControl Elector;
        public ShortcutKeyControl Country;
        public ShortcutKeyControl Region;
        public ShortcutKeyControl Character;
        public ShortcutKeyControl Item;
        public ShortcutKeyControl Recruit;
    }


    [System.Serializable]
    public class CameraShortcutKeys
    {
        public ShortcutKeyControl ZoomIn;
        public ShortcutKeyControl ArrowUp;
        public ShortcutKeyControl ZoomOut;
        public ShortcutKeyControl ArrowLeft;
        public ShortcutKeyControl ArrowDown;
        public ShortcutKeyControl ArrowRight;
    }



    #endregion

    private void Start()
    {
        InitRightTopButtons();
        SetShortcutkeyData();
        SetCameraShortcutkeyData();
    }

    void InitRightTopButtons()
    {
        rightTopButtons.SaveButton.onClick.AddListener(() => SettingsManager.Instance.OpenGeneralControlPanel());
        rightTopButtons.MusicButton.onClick.AddListener(() => SettingsManager.Instance.OpenGeneralControlPanel());
        rightTopButtons.LoadButton.onClick.AddListener(() => SettingsManager.Instance.OpenGeneralControlPanel());
        rightTopButtons.MenuButton.onClick.AddListener(() => SettingsManager.Instance.OpenGeneralControlPanel());
        rightTopButtons.GuideButton.onClick.AddListener(() => SettingsManager.Instance.OpenGeneralControlPanel());
        rightTopButtons.SettingButton.onClick.AddListener(() => SettingsManager.Instance.OpenGeneralControlPanel());
    }


    void SetShortcutkeyData()
    {
        SettingShortcutkeyData settingShortcutkeyData = SettingValue.Instance.settingValueData.settingShortcutkeyData;
        GameShortcutkeyData shortcutkeyData = SettingValue.Instance.settingValueData.settingShortcutkeyData.GameShortcutkeyData;
        shortcutKeyControls.NextTurn.BindToKey(() => shortcutkeyData.NextTurnKey,(value) => shortcutkeyData.NextTurnKey = value,settingShortcutkeyData.CreateValidator(ShortcutKeyCategory.Game));
        shortcutKeyControls.RightCol.BindToKey(() => shortcutkeyData.RightColKey,(value) => shortcutkeyData.RightColKey = value, settingShortcutkeyData.CreateValidator(ShortcutKeyCategory.Game));
        shortcutKeyControls.Scout.BindToKey(() => shortcutkeyData.ScoutKey,(value) => shortcutkeyData.ScoutKey = value, settingShortcutkeyData.CreateValidator(ShortcutKeyCategory.Game));
        shortcutKeyControls.Build.BindToKey(() => shortcutkeyData.BuildKey,(value) => shortcutkeyData.BuildKey = value, settingShortcutkeyData.CreateValidator(ShortcutKeyCategory.Game));
        shortcutKeyControls.Negotiation.BindToKey(() => shortcutkeyData.NegotiationKey,(value) => shortcutkeyData.NegotiationKey = value, settingShortcutkeyData.CreateValidator(ShortcutKeyCategory.Game));
        shortcutKeyControls.Elector.BindToKey(() => shortcutkeyData.ElectorKey, (value) => shortcutkeyData.ElectorKey = value, settingShortcutkeyData.CreateValidator(ShortcutKeyCategory.Game));
        shortcutKeyControls.Country.BindToKey(() => shortcutkeyData.CountryKey,(value) => shortcutkeyData.CountryKey = value, settingShortcutkeyData.CreateValidator(ShortcutKeyCategory.Game));
        shortcutKeyControls.Region.BindToKey(() => shortcutkeyData.RegionKey,(value) => shortcutkeyData.RegionKey = value, settingShortcutkeyData.CreateValidator(ShortcutKeyCategory.Game));
        shortcutKeyControls.Character.BindToKey(() => shortcutkeyData.CharacterKey,(value) => shortcutkeyData.CharacterKey = value, settingShortcutkeyData.CreateValidator(ShortcutKeyCategory.Game));
        shortcutKeyControls.Item.BindToKey(() => shortcutkeyData.ItemKey,(value) => shortcutkeyData.ItemKey = value, settingShortcutkeyData.CreateValidator(ShortcutKeyCategory.Game));
        shortcutKeyControls.Recruit.BindToKey(() => shortcutkeyData.RecruitKey,(value) => shortcutkeyData.RecruitKey = value, settingShortcutkeyData.CreateValidator(ShortcutKeyCategory.Game));
    }

    void SetCameraShortcutkeyData()
    {
        SettingShortcutkeyData settingShortcutkeyData = SettingValue.Instance.settingValueData.settingShortcutkeyData;
        GameShortcutkeyData shortcutkeyData = SettingValue.Instance.settingValueData.settingShortcutkeyData.GameShortcutkeyData;
        cameraShortcutKeys.ZoomOut.BindToKey(() => shortcutkeyData.ZoomOutKey, (value) => shortcutkeyData.ZoomOutKey = value, settingShortcutkeyData.CreateValidator(ShortcutKeyCategory.Game));
        cameraShortcutKeys.ArrowUp.BindToKey(() => shortcutkeyData.ArrowUpKey, (value) => shortcutkeyData.ArrowUpKey = value, settingShortcutkeyData.CreateValidator(ShortcutKeyCategory.Game));
        cameraShortcutKeys.ZoomIn.BindToKey(() => shortcutkeyData.ZoomInKey, (value) => shortcutkeyData.ZoomInKey = value, settingShortcutkeyData.CreateValidator(ShortcutKeyCategory.Game));
        cameraShortcutKeys.ArrowLeft.BindToKey(() => shortcutkeyData.ArrowLeftKey, (value) => shortcutkeyData.ArrowLeftKey = value, settingShortcutkeyData.CreateValidator(ShortcutKeyCategory.Game));
        cameraShortcutKeys.ArrowDown.BindToKey(() => shortcutkeyData.ArrowDownKey, (value) => shortcutkeyData.ArrowDownKey = value, settingShortcutkeyData.CreateValidator(ShortcutKeyCategory.Game));
        cameraShortcutKeys.ArrowRight.BindToKey(() => shortcutkeyData.ArrowRightKey, (value) => shortcutkeyData.ArrowRightKey = value, settingShortcutkeyData.CreateValidator(ShortcutKeyCategory.Game));
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
        SettingValue.Instance.settingValueData.settingShortcutkeyData.GameShortcutkeyData = new GameShortcutkeyData();
        SetShortcutkeyData();
    }


}
