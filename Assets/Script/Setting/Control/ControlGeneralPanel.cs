using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlGeneralPanel : ControlPanelBase
{
    [Header("Left Shortcut Key")]
    public LeftShortcutKey leftShortcutKey;

    [Serializable]
    public struct LeftShortcutKey
    {
        public ShortcutKeyControl switchControl;
    }

    [Header("Right Shortcut Key")]
    public RightShortcutKey rightShortcutKey;
    
    [Serializable]
    public struct RightShortcutKey
    {
        public ShortcutKeyControl SaveControl;
        public ShortcutKeyControl SaveQuickControl;
        public ShortcutKeyControl LoadControl;
        public ShortcutKeyControl LoadQuickControl;

        public ShortcutKeyControl SettingControl;
        public ShortcutKeyControl MusicControl;
        public ShortcutKeyControl GuideControl;
        public ShortcutKeyControl MenuControl;

    }


    private void Start()
    {
        SetShortcutkeyData();
    }

    public override void RestorePanel()
    {
        SetShortcutkeyData();
    }

    public override void OnDefaultButtonClick()
    {
        SettingValue.Instance.settingValueData.settingShortcutkeyData.GeneralShortcutkeyData = new GeneralShortcutkeyData();

    }


    void SetShortcutkeyData()
    {
        SetLeftShortcutkeyData();
        SetRightShortcutkeyData();
    }

    void SetLeftShortcutkeyData()
    {
        SettingShortcutkeyData settingShortcutkeyData = SettingValue.Instance
            .settingValueData
            .settingShortcutkeyData;

        GeneralShortcutkeyData shortcutkeyData = settingShortcutkeyData.GeneralShortcutkeyData;

        // ?? BindToKey ??????
        leftShortcutKey.switchControl.BindToKey(
            () => shortcutkeyData.SwitchKey,                    // Getter
            (value) => shortcutkeyData.SwitchKey = value  ,      // Setter
            settingShortcutkeyData.CreateValidator(ShortcutKeyCategory.General)
        );
    }

    void SetRightShortcutkeyData()
    {
        SettingShortcutkeyData settingShortcutkeyData = SettingValue.Instance
            .settingValueData
            .settingShortcutkeyData;

        GeneralShortcutkeyData shortcutkeyData = settingShortcutkeyData.GeneralShortcutkeyData;

        // ?????????
        rightShortcutKey.SaveControl.BindToKey(
            () => shortcutkeyData.SaveKey,
            (value) => shortcutkeyData.SaveKey = value,
            settingShortcutkeyData.CreateValidator(ShortcutKeyCategory.General)
        );

        rightShortcutKey.SaveQuickControl.BindToKey(
            () => shortcutkeyData.SaveQuickKey,
            (value) => shortcutkeyData.SaveQuickKey = value,
            settingShortcutkeyData.CreateValidator(ShortcutKeyCategory.General)
        );

        rightShortcutKey.LoadControl.BindToKey(
            () => shortcutkeyData.LoadKey,
            (value) => shortcutkeyData.LoadKey = value,
            settingShortcutkeyData.CreateValidator(ShortcutKeyCategory.General)
        );

        rightShortcutKey.LoadQuickControl.BindToKey(
            () => shortcutkeyData.LoadQuickKey,
            (value) => shortcutkeyData.LoadQuickKey = value,
            settingShortcutkeyData.CreateValidator(ShortcutKeyCategory.General)
        );

        rightShortcutKey.SettingControl.BindToKey(
            () => shortcutkeyData.SettingKey,
            (value) => shortcutkeyData.SettingKey = value,
            settingShortcutkeyData.CreateValidator(ShortcutKeyCategory.General)
        );

        rightShortcutKey.MusicControl.BindToKey(
            () => shortcutkeyData.MusicKey,
            (value) => shortcutkeyData.MusicKey = value,
            settingShortcutkeyData.CreateValidator(ShortcutKeyCategory.General)
        );

        rightShortcutKey.GuideControl.BindToKey(
            () => shortcutkeyData.GuideKey,
            (value) => shortcutkeyData.GuideKey = value,
            settingShortcutkeyData.CreateValidator(ShortcutKeyCategory.General)
        );

        rightShortcutKey.MenuControl.BindToKey(
            () => shortcutkeyData.MenuKey,
            (value) => shortcutkeyData.MenuKey = value,
            settingShortcutkeyData.CreateValidator(ShortcutKeyCategory.General)
        );
    }
}
