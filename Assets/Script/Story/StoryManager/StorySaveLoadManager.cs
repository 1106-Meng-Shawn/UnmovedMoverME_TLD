using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

/// <summary>
/// ????????????
/// </summary>
public class StorySaveLoadManager : MonoBehaviour
{
    private string saveFolderPath;
    private Dictionary<string, List<int>> globalMaxReachLineIndices = new Dictionary<string, List<int>>();

    private void Start()
    {
        InitializeSaveFilePath();
    }


    private void InitializeSaveFilePath()
    {
        saveFolderPath = Path.Combine(Application.persistentDataPath, Constants.SAVE_FILE_PATH);
        if (!Directory.Exists(saveFolderPath))
        {
            Directory.CreateDirectory(saveFolderPath);
        }
    }

    /// <summary>
    /// ???????
    /// </summary>
    public void LoadOrCreateProgress()
    {
        string folderPath = Path.Combine(Application.persistentDataPath, Constants.SAVE_FILE_PATH);
        string progressFilePath = Path.Combine(folderPath, Constants.STORY_PROGRESS_SaveFileName);

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        if (File.Exists(progressFilePath))
        {
            string json = File.ReadAllText(progressFilePath);
            TotalStoryProgress loaded = JsonUtility.FromJson<TotalStoryProgress>(json);
            LoadGlobalFromTotal(loaded);
        }
        else
        {
            var empty = new TotalStoryProgress();
            string json = JsonUtility.ToJson(empty, true);
            File.WriteAllText(progressFilePath, json);
            globalMaxReachLineIndices = new Dictionary<string, List<int>>();
        }
    }
    public void SaveProgress(string currentStoryFileName, List<int> currentLines)
    {
        string progressFilePath = Path.Combine(Application.persistentDataPath, Constants.SAVE_FILE_PATH, Constants.STORY_PROGRESS_SaveFileName);
        TotalStoryProgress total;

        if (File.Exists(progressFilePath))
        {
            string oldJson = File.ReadAllText(progressFilePath);
            total = JsonUtility.FromJson<TotalStoryProgress>(oldJson);
        }
        else
        {
            total = new TotalStoryProgress();
        }

        if (total.progressList == null)
        {
            total.progressList = new List<StoryProgress>();
        }

        var existingProgress = total.progressList.Find(p => p.fileName == currentStoryFileName);
        if (existingProgress != null && existingProgress.maxReadStoryLines != null)
        {
            for (int i = 0; i < currentLines.Count; i++)
            {
                if (i >= existingProgress.maxReadStoryLines.Count)
                {
                    existingProgress.maxReadStoryLines.Add(currentLines[i]);
                }
                else
                {
                    if (currentLines[i] > existingProgress.maxReadStoryLines[i])
                    {
                        existingProgress.maxReadStoryLines[i] = currentLines[i];
                    }
                }
            }

            for (int i = 0; i < total.progressList.Count; i++)
            {
                if (total.progressList[i].fileName == currentStoryFileName)
                {
                    total.progressList[i] = existingProgress;
                    break;
                }
            }
        }
        else
        {
            var newProgress = new StoryProgress
            {
                fileName = currentStoryFileName,
                maxReadStoryLines = new List<int>(currentLines),
            };

            total.progressList.Add(newProgress);
        }

        string updatedJson = JsonUtility.ToJson(total, true);
        File.WriteAllText(progressFilePath, updatedJson);
    }

    /// <summary>
    /// ???????
    /// </summary>
    public void MarkStoryAsCompleted(string fileName)
    {
        string progressFilePath = Path.Combine(Application.persistentDataPath, Constants.SAVE_FILE_PATH, Constants.STORY_PROGRESS_SaveFileName);
        Debug.Log("MarkStoryAsCompleted happened");

        TotalStoryProgress progressData;
        if (File.Exists(progressFilePath))
        {
            string json = File.ReadAllText(progressFilePath);
            progressData = JsonUtility.FromJson<TotalStoryProgress>(json);
        }
        else
        {
            Debug.LogError("Progress file not found");
            return;
        }

        bool found = false;
        foreach (var sp in progressData.progressList)
        {
            if (sp.fileName == fileName)
            {
                Debug.Log("MarkStoryAsCompleted true");
                sp.storyCompleted = true;
                found = true;
                break;
            }
        }

        if (!found)
        {
            Debug.LogError("Cannot find the fileName");
            return;
        }

        string updatedJson = JsonUtility.ToJson(progressData, true);
        File.WriteAllText(progressFilePath, updatedJson);
    }

    /// <summary>
    /// ????????
    /// </summary>
    public bool IsFileCompleted(string fileName)
    {
        LoadOrCreateProgress();
        string progressFilePath = Path.Combine(Application.persistentDataPath, Constants.SAVE_FILE_PATH, Constants.STORY_PROGRESS_SaveFileName);

        if (!File.Exists(progressFilePath))
        {
            return false;
        }

        string json = File.ReadAllText(progressFilePath);
        TotalStoryProgress totalStoryProgress = JsonUtility.FromJson<TotalStoryProgress>(json);

        if (totalStoryProgress == null || totalStoryProgress.progressList == null)
        {
            return false;
        }

        foreach (var progress in totalStoryProgress.progressList)
        {
            if (progress.fileName == fileName)
            {
                return progress.storyCompleted;
            }
        }

        return false;
    }

    /// <summary>
    /// ?Global??Total
    /// </summary>
    private TotalStoryProgress BuildTotalFromGlobal()
    {
        var total = new TotalStoryProgress();
        foreach (var kvp in globalMaxReachLineIndices)
        {
            var sp = new StoryProgress
            {
                fileName = kvp.Key,
                maxReadStoryLines = new List<int>(kvp.Value),
            };
            total.progressList.Add(sp);
        }
        return total;
    }

    /// <summary>
    /// ?Total???Global
    /// </summary>
    private void LoadGlobalFromTotal(TotalStoryProgress total)
    {
        globalMaxReachLineIndices.Clear();
        foreach (var sp in total.progressList)
        {
            globalMaxReachLineIndices[sp.fileName] = new List<int>(sp.maxReadStoryLines);
        }
    }

    // ==================== Getters ====================

    public Dictionary<string, List<int>> GetGlobalMaxReachLineIndices() => globalMaxReachLineIndices;
    public string GetSaveFolderPath() => saveFolderPath;

    // ==================== Setters ====================

    public void SetGlobalMaxReachLineIndices(Dictionary<string, List<int>> indices) => globalMaxReachLineIndices = new Dictionary<string, List<int>>(indices);
}

/// <summary>
/// ??????
/// </summary>
[System.Serializable]
public class StorySaveData
{
    public string savedStoryFileName;
    public int savedLine;
    public int saveSheetIndex;
    public List<int> saveReadSheet;
    public string savedSpeakingContent;
    public List<HistoryData> savedHistoryRecords;
    public List<PanelSotryData> savedPanelSotryDatas;

    public StorySaveData(string storyFileName, int line, int sheetIndex, string speakingContent,
                        LinkedList<HistoryData> historyRecords,
                        LinkedList<PanelSotryData> panelSotryDatas,
                        List<int> readIndex)
    {
        savedStoryFileName = storyFileName;
        savedLine = line;
        saveSheetIndex = sheetIndex;
        savedSpeakingContent = speakingContent;
        if (historyRecords != null) savedHistoryRecords = historyRecords.ToList();
        if (panelSotryDatas != null) savedPanelSotryDatas = panelSotryDatas.ToList();
        saveReadSheet = readIndex;
    }
}

/// <summary>
/// ????
/// </summary>
[System.Serializable]
public class StoryProgress
{
    public string fileName;
    public List<int> maxReadStoryLines = new List<int>();
    public bool storyCompleted = false;
}

/// <summary>
/// ?????
/// </summary>
[System.Serializable]
public class TotalStoryProgress
{
    public List<StoryProgress> progressList = new List<StoryProgress>();
}