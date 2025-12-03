using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TopRightButtonControl : MonoBehaviour
{
    [SerializeField] private Scene currentScene;

    [Header("Game Scene Buttons")]
    public GameSceneButtons gameSceneButtons;

    [System.Serializable]
    public class GameSceneButtons
    {
        public Button SaveButton;
        public Button LoadButton;
        public Button MusicButton;
        public Button SettingButton;
        public Button GuideButton;
        public Button MenuButton;

        public MenuPanelControl MenuPanelControl;
    }

    [Header("Battle Scene Buttons")]
    public BattleSceneButtons battleSceneButtons;

    [System.Serializable]
    public class BattleSceneButtons
    {
        public Button MusicButton;
        public Button SettingButton;
        public Button GuideButton;
        public Button ExitButton;
    }


    void Start()
    {
         switch (currentScene)
        {
            case Scene.GameScene: InitGameSceneButtons(); break;
            case Scene.BattleScene: InitBattleSceneButtons(); break;
        }
    }

    void InitGameSceneButtons()
    {
        AddButtonListener(gameSceneButtons.SaveButton, OnSaveButtonClick);
        AddButtonListener(gameSceneButtons.LoadButton, OnLoadButtonClick);
        AddButtonListener(gameSceneButtons.MusicButton, OnMusicButtonClick);
        AddButtonListener(gameSceneButtons.SettingButton, OnSettingButtonClick);
        AddButtonListener(gameSceneButtons.GuideButton, OnGameGuideButtonClick);
        AddButtonListener(gameSceneButtons.MenuButton, OnMenuButtonClick);
    }

    void InitBattleSceneButtons()
    {
        AddButtonListener(battleSceneButtons.MusicButton, OnMusicButtonClick);
        AddButtonListener(battleSceneButtons.SettingButton, OnSettingButtonClick);
        AddButtonListener(battleSceneButtons.GuideButton, OnBattleGuideButtonClick);
        AddButtonListener(battleSceneButtons.ExitButton, OnBattleExitButtonClick);
    }




    void AddButtonListener(Button button,Action OnButtonClick)
    {
        if (button == null) return;
        button.onClick.AddListener(() => OnButtonClick());
    }

    void OnSaveButtonClick()
    {
        LoadPanelManage.Instance.NormalSaveGame();
    }

    void OnLoadButtonClick()
    {
        LoadPanelManage.Instance.ShowLoadPanel();
    }
    void OnMusicButtonClick()
    {
        MusicPanelManager.Instance.OpenPanel();
    }

    void OnSettingButtonClick()
    {
        SettingsManager.Instance.OpenPanel();
    }

    void OnGameGuideButtonClick()
    {
    }

    void OnBattleGuideButtonClick()
    {
    }

    void OnBattleExitButtonClick()
    {
        BattleResultPanelControl.Instance.ShowBattleResultPanel(true);
    }


    void OnMenuButtonClick()
    {
        gameSceneButtons.MenuPanelControl.ShowMenuPanel();
    }




    // Update is called once per frame
    void Update()
    {
        
    }
}
