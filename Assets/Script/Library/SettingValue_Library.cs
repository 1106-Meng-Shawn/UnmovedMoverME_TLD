using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class SettingValue_Library : MonoBehaviour
{
    [Header("BGM Data")]
    public List<BGMScriptableObject> allBGMs = new List<BGMScriptableObject>();



    #region Runtime Access Methods


    public BGMScriptableObject GetBGMByID(int id)
    {
        return allBGMs.Find(bgm => bgm.ID == id);
    }
    public BGMScriptableObject GetBGMByTitle(string title, string languageCode = null)
    {
        if (string.IsNullOrEmpty(languageCode))languageCode = LanguageCode.EN;
        return allBGMs.Find(bgm => bgm.GetTitle(languageCode) == title);
    }

    #endregion

    #region Editor Methods

#if UNITY_EDITOR

    [ContextMenu("Refresh All Data")]
    public void RefreshAllData()
    {
        RefreshBGMs();

        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"[SettingValue_Library] Refreshed All Data - BGMs: {allBGMs.Count}");
    }

    [ContextMenu("Refresh BGMs")]
    public void RefreshBGMs()
    {
        allBGMs.Clear();
        string[] guids = AssetDatabase.FindAssets("t:BGMScriptableObject", new string[] { "Assets/ScriptableObject/BGMData" });

        foreach (var guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            var bgm = AssetDatabase.LoadAssetAtPath<BGMScriptableObject>(path);
            if (bgm != null)
                allBGMs.Add(bgm);
        }

        allBGMs.Sort((a, b) => a.ID.CompareTo(b.ID));

        Debug.Log($"[Editor Mode] Refreshed {allBGMs.Count} BGMs");
    }



    [ContextMenu("Validate Data")]
    public void ValidateData()
    {
        int errorCount = 0;

        foreach (var bgm in allBGMs)
        {
            if (bgm.ID < 0)
            {
                Debug.LogError($"[Validation] BGM has invalid ID: {bgm.name}");
                errorCount++;
            }
            if (bgm.musicClip == null)
            {
                Debug.LogWarning($"[Validation] BGM missing audio clip: {bgm.name} (ID: {bgm.ID})");
            }
        }

        HashSet<int> bgmIDs = new HashSet<int>();
        foreach (var bgm in allBGMs)
        {
            if (!bgmIDs.Add(bgm.ID))
            {
                Debug.LogError($"[Validation] Duplicate BGM ID found: {bgm.ID}");
                errorCount++;
            }
        }

        if (errorCount == 0)
        {
            Debug.Log("[Validation] All data validated successfully!");
        }
        else
        {
            Debug.LogError($"[Validation] Found {errorCount} errors!");
        }
    }

    [ContextMenu("Clear All Data")]
    public void ClearAllData()
    {
        if (EditorUtility.DisplayDialog("Clear All Data",
            "Are you sure you want to clear all loaded data? This won't delete the ScriptableObjects.",
            "Yes", "Cancel"))
        {
            allBGMs.Clear();

            EditorUtility.SetDirty(this);
            Debug.Log("[SettingValue_Library] Cleared all data");
        }
    }

    [ContextMenu("Print Statistics")]
    public void PrintStatistics()
    {
        Debug.Log("=== SettingValue Library Statistics ===");
        Debug.Log($"Total BGMs: {allBGMs.Count}");
        Debug.Log("=====================================");
    }

#endif

    #endregion
}