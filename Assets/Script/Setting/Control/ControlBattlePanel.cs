using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ControlBattlePanel : ControlPanelBase
{
    [Header("Buttons")]
    [SerializeField] BattleFieldButtons battleFieldButtons;
    [SerializeField] RightTopButtons rightTopButtons;


    #region Buttons
    [System.Serializable]
    public class BattleFieldButtons
    {
        public Button SwitchButton;
    }

    [System.Serializable]
    public class RightTopButtons
    {
        public Button MusicButton;
        public Button SettingButton;
        public Button GuideButton;
    }
    #endregion

    private void Awake()
    {
        InitBattleFieldButtons();
        InitRightTopButtons();
    }

    void InitBattleFieldButtons()
    {
        battleFieldButtons.SwitchButton.onClick.AddListener(() => SettingsManager.Instance.OpenGeneralControlPanel());
    }




    void InitRightTopButtons()
    {
        rightTopButtons.MusicButton.onClick.AddListener(() => SettingsManager.Instance.OpenGeneralControlPanel());
        rightTopButtons.GuideButton.onClick.AddListener(() => SettingsManager.Instance.OpenGeneralControlPanel());
        rightTopButtons.SettingButton.onClick.AddListener(() => SettingsManager.Instance.OpenGeneralControlPanel());
    }


    void Start()
    {
        
    }

    void Update()
    {
        
    }


    public override void RestorePanel()
    {

    }

    public override void OnDefaultButtonClick()
    {
    }
}
