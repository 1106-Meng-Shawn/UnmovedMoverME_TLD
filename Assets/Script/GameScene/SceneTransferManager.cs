using UnityEngine;
using UnityEngine.SceneManagement;

public enum Scene
{
    MainMenuScene,
    GameScene, 
    BuildScene,
    BattleScene,
    ExploreScene,
    LoadingScene
}


public class SceneTransferManager : MonoBehaviour
{
    public static SceneTransferManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // -----------------------------
    // ?????
    // -----------------------------
    public void StartNewGame(string playerName)
    {
        SettingsManager.Instance.SetPlayerName(playerName);
        LoadScene(Scene.GameScene, null, true);
    }

    // -----------------------------
    // ?????????? LoadingScene?
    // -----------------------------

    public void LoadScene(Scene sceneType, SaveData saveData = null, bool isNewGame = false)
    {
        LoadingSceneData.TargetScene = sceneType;
        LoadingSceneData.SaveData = saveData;
        LoadingSceneData.IsNewGame = isNewGame;
        SceneManager.LoadScene("LoadingScene");
    }

    // -----------------------------
    // ?? Scene enum ??????
    // -----------------------------
    public string GetSceneName(Scene sceneType)
    {
        switch (sceneType)
        {
            case Scene.MainMenuScene: return "MainMenu";
            case Scene.GameScene: return "GameScene";
            case Scene.BuildScene: return "BuildScene";
            case Scene.BattleScene: return "BattleScene";
            case Scene.ExploreScene: return "ExploreScene";
            default: return "MainMenu";
        }
    }

    // -----------------------------
    // ????
    // -----------------------------
    public void ExitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
