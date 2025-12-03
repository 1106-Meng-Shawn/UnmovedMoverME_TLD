using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class LoadingSceneData
{
    public static Scene TargetScene;
    public static SaveData SaveData;
    public static bool IsNewGame;
}

public class LoadingSceneManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Slider loadingSlider;
    [SerializeField] private TextMeshProUGUI loadingText;
    [SerializeField] private TipPanelControl tipPanelControl;
    [SerializeField] private Button backgroundButton;

    [Header("Background Settings")]
    [SerializeField] private List<Sprite> backgroundSprites;

    [Header("Settings")]
    [SerializeField] private float progressSmoothSpeed = 0.5f;

    private void Start()
    {
        backgroundButton.onClick.AddListener(OnBackgroundClicked);
     
        StartCoroutine(LoadTargetScene());
    }

    private void OnBackgroundClicked()
    {
        if (backgroundSprites == null || backgroundSprites.Count == 0) return;
        int index = Random.Range(0, backgroundSprites.Count);
        backgroundButton.image.sprite = backgroundSprites[index];
    }

    private IEnumerator LoadTargetScene()
    {
        Scene targetScene = LoadingSceneData.TargetScene;
        SaveData saveData = LoadingSceneData.SaveData;
        bool isNewGame = LoadingSceneData.IsNewGame;

        tipPanelControl?.ShowRandomTip();

        string sceneName = GetSceneName(targetScene);
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false;

        float displayedProgress = 0f;

        while (!asyncLoad.isDone)
        {
            float targetProgress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
            displayedProgress = Mathf.MoveTowards(displayedProgress, targetProgress, progressSmoothSpeed * Time.deltaTime);
            if (loadingSlider != null) loadingSlider.value = displayedProgress;
            if (loadingText != null) loadingText.text = $"Loading {displayedProgress * 100f:0}%";
            if (asyncLoad.progress >= 0.9f && displayedProgress >= 1f)
            {
                asyncLoad.allowSceneActivation = true;
            }

            yield return null;
        }

        if (saveData != null)
            ApplySaveData(saveData);

        if (isNewGame)
            LoadPanelManage.Instance.AutoSaveGameTurnBegin();
    }

    private string GetSceneName(Scene sceneType)
    {
        switch (sceneType)
        {
            case Scene.MainMenuScene: return "MainMenu";
            case Scene.BuildScene: return "BuildScene";
            case Scene.GameScene: return "GameScene";
            case Scene.BattleScene: return "BattleScene";
            case Scene.ExploreScene: return "ExploreScene";
            case Scene.LoadingScene: return "LoadingScene";
            default: return "MainMenu";
        }
    }

    private void ApplySaveData(SaveData saveData)
    {
        GameValue.Instance.SetSaveData(saveData.gameValueData);
        BottomButton.Instance.SetTotalPanelSaveData(saveData.totalPanelSaveData);
        CameraControl2D.Instance.SetCameraData(saveData.cameraSaveData);
        NotificationManage.Instance.ShowToTopByKey(NotificationKeyConstants.Load_Success);
    }
}
