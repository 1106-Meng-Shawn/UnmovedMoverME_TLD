using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public enum ControlPanelType
{
    General, Game, Battle, Explore, Text
}

public class ControlPanelManager : SettingPanelBase
{
    [Serializable]
    public struct PanelEntry
    {
        public Button button;
        public ControlPanelBase panel;
    }

    [Header("All Panels")]
    [SerializeField] PanelEntry GeneralPanel;
    [SerializeField] PanelEntry GamePanel;
    [SerializeField] PanelEntry BattlePanel;
    [SerializeField] PanelEntry ExplorePanel;
    [SerializeField] PanelEntry TextPanel;

    private ControlPanelType currentPanelType = ControlPanelType.General;

    private Dictionary<ControlPanelType, PanelEntry> panelMap;

    private void Start()
    {
        InitButtons();
    }

    public override void Init()
    {
    }

    void InitButtons()
    {
        panelMap = new Dictionary<ControlPanelType, PanelEntry>()
        {
            { ControlPanelType.General, GeneralPanel },
            { ControlPanelType.Game, GamePanel },
            { ControlPanelType.Battle, BattlePanel },
            { ControlPanelType.Explore, ExplorePanel },
            { ControlPanelType.Text, TextPanel },
        };

        foreach (var kv in panelMap)
        {
            ControlPanelType type = kv.Key;
            PanelEntry entry = kv.Value;

            if (entry.button != null)
            {
                entry.button.onClick.AddListener(() => OpenPanel(type));
            }
            entry.panel.ClosePanel();
        }
    }



    public override void OpenPanel()
    {
        base.OpenPanel();
        OpenPanel(currentPanelType);
    }


    public void OpenPanel(ControlPanelType newType)
    {
        if (panelMap[currentPanelType].panel != null)panelMap[currentPanelType].panel.ClosePanel();
        currentPanelType = newType;
        if (panelMap[currentPanelType].panel != null)panelMap[currentPanelType].panel.OpenPanel();
    }



    public override void OnDefaultButtonClick()
    {
        switch (currentPanelType) {
            case ControlPanelType.General: GeneralPanel.panel.OnDefaultButtonClick();  break;
            case ControlPanelType.Game: GamePanel.panel.OnDefaultButtonClick(); break;
            case ControlPanelType.Battle: BattlePanel.panel.OnDefaultButtonClick(); break;
            case ControlPanelType.Explore: ExplorePanel.panel.OnDefaultButtonClick(); break;
            case ControlPanelType.Text: TextPanel.panel.OnDefaultButtonClick(); break;

        }
    }

}
