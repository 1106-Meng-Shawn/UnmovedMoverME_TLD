using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuPanelControl : MonoBehaviour
{
    public Button ReturnButton;
    public Button SaveButton;
    public Button LoadButton;
    public Button SettingButton;
    public Button MainMenuButton;
    public Button ExitButton;

    private void Awake()
    {
        ReturnButton.onClick.AddListener(OnReturnButtonClick);
        SaveButton.onClick.AddListener(OnSaveButtonClick);
        LoadButton.onClick.AddListener(OnLoadButtonClick);
        SettingButton.onClick.AddListener(OnSettingButtonClick);
        MainMenuButton.onClick.AddListener(OnMainMenuButtonClick);
        ExitButton.onClick.AddListener(OnExitButtonClick);
    }

    public void ShowMenuPanel()
    {
        gameObject.SetActive(true);
    }

    void CloseMenuPanel()
    {
        gameObject.SetActive(false);
    }

    void OnReturnButtonClick()
    {
        CloseMenuPanel();
    }

    void OnSaveButtonClick()
    {
        LoadPanelManage.Instance.NormalSaveGame();
    }

    void OnLoadButtonClick()
    {
        LoadPanelManage.Instance.ShowLoadPanel();
    }

    void OnSettingButtonClick()
    {
        SettingsManager.Instance.OpenPanel();
    }

    void OnMainMenuButtonClick()
    {
        SceneTransferManager.Instance.LoadScene(Scene.MainMenuScene);
    }

    void OnExitButtonClick()
    {
        SceneTransferManager.Instance.ExitGame();
    }
}
