using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ExcelDataReader;

public class BGMScriptableObjectGenerator : EditorWindow
{
    private const string OUTPUT_PATH = "Assets/ScriptableObject/BGMData";
    private Vector2 scrollPosition;
    private List<BGMData> bgmDataList;
    private bool isLoaded = false;

    [MenuItem("Tools/BGM ScriptableObject Generator")]
    public static void ShowWindow()
    {
        BGMScriptableObjectGenerator window = GetWindow<BGMScriptableObjectGenerator>("BGM Generator");
        window.minSize = new Vector2(400, 300);
        window.Show();
    }

    private void OnGUI()
    {
        GUILayout.Space(10);
        EditorGUILayout.LabelField("BGM ScriptableObject Generator", EditorStyles.boldLabel);
        GUILayout.Space(10);

        EditorGUILayout.HelpBox("This tool will generate BGM ScriptableObjects from Excel data.", MessageType.Info);
        GUILayout.Space(10);

        // 加载数据按钮
        if (GUILayout.Button("Load BGM Data from Excel", GUILayout.Height(30)))
        {
            LoadBGMData();
        }

        GUILayout.Space(10);

        // 显示加载的数据
        if (isLoaded && bgmDataList != null)
        {
            EditorGUILayout.LabelField($"Loaded {bgmDataList.Count} BGM entries", EditorStyles.boldLabel);
            GUILayout.Space(5);

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            foreach (var bgmData in bgmDataList)
            {
                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.LabelField($"ID: {bgmData.ID}");

                string titleEN = bgmData.BGMTitles.ContainsKey(LanguageCode.EN) ? bgmData.BGMTitles[LanguageCode.EN] : "Unknown";
                EditorGUILayout.LabelField($"Title: {titleEN}");

                EditorGUILayout.LabelField($"Path: {bgmData.Path}");
                EditorGUILayout.EndVertical();
                GUILayout.Space(2);
            }

            EditorGUILayout.EndScrollView();

            GUILayout.Space(10);

            // 生成 ScriptableObjects 按钮
            if (GUILayout.Button("Generate All ScriptableObjects", GUILayout.Height(40)))
            {
                GenerateScriptableObjects();
            }
        }
        else if (isLoaded)
        {
            EditorGUILayout.HelpBox("No BGM data found. Please check your Excel file.", MessageType.Warning);
        }
    }

    private void LoadBGMData()
    {
        try
        {
            bgmDataList = LoadBGMDatas();
            isLoaded = true;
            Debug.Log($"Successfully loaded {bgmDataList.Count} BGM entries.");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to load BGM data: {e.Message}");
            EditorUtility.DisplayDialog("Error", $"Failed to load BGM data:\n{e.Message}", "OK");
            isLoaded = false;
        }
    }

    private void GenerateScriptableObjects()
    {
        if (bgmDataList == null || bgmDataList.Count == 0)
        {
            EditorUtility.DisplayDialog("Error", "No BGM data loaded. Please load data first.", "OK");
            return;
        }

        // 确保输出目录存在
        if (!Directory.Exists(OUTPUT_PATH))
        {
            Directory.CreateDirectory(OUTPUT_PATH);
            AssetDatabase.Refresh();
        }

        int successCount = 0;
        int failCount = 0;

        EditorUtility.DisplayProgressBar("Generating ScriptableObjects", "Starting...", 0);

        for (int i = 0; i < bgmDataList.Count; i++)
        {
            BGMData data = bgmDataList[i];
            float progress = (float)i / bgmDataList.Count;

            string titleEN = data.BGMTitles.ContainsKey(LanguageCode.EN) ? data.BGMTitles[LanguageCode.EN] : $"BGM_{data.ID}";
            EditorUtility.DisplayProgressBar("Generating ScriptableObjects", $"Creating: {titleEN}", progress);

            try
            {
                // 创建 ScriptableObject
                BGMScriptableObject bgmSO = ScriptableObject.CreateInstance<BGMScriptableObject>();

                // 设置数据
                bgmSO.ID = data.ID;

                // 使用字典设置方法
                bgmSO.SetTitles(data.BGMTitles);
                bgmSO.SetAuthorNames(data.AuthorTitles);

                // 尝试加载音频文件
                string audioPath = data.Path;
                AudioClip clip = Resources.Load<AudioClip>(audioPath);
                if (clip != null)
                {
                    bgmSO.musicClip = clip;
                    bgmSO.musicTime = clip.length;
                }
                else
                {
                    Debug.LogWarning($"Audio clip not found at path: {audioPath} for BGM ID: {data.ID}");
                }

                // 生成安全的文件名
                string titleForFilename = bgmSO.GetTitle(LanguageCode.EN);
                string safeFileName = SanitizeFileName($"BGM_{data.ID}_{titleForFilename}");
                string assetPath = $"{OUTPUT_PATH}/{safeFileName}.asset";

                // 检查文件是否已存在
                if (File.Exists(assetPath))
                {
                    // 如果存在，更新现有的
                    BGMScriptableObject existingSO = AssetDatabase.LoadAssetAtPath<BGMScriptableObject>(assetPath);
                    if (existingSO != null)
                    {
                        existingSO.ID = bgmSO.ID;
                        existingSO.SetTitles(data.BGMTitles);
                        existingSO.SetAuthorNames(data.AuthorTitles);
                        if (bgmSO.musicClip != null)
                        {
                            existingSO.musicClip = bgmSO.musicClip;
                            existingSO.musicTime = bgmSO.musicTime;
                        }
                        EditorUtility.SetDirty(existingSO);
                        Debug.Log($"Updated existing BGM ScriptableObject: {assetPath}");
                    }
                }
                else
                {
                    // 创建新的
                    AssetDatabase.CreateAsset(bgmSO, assetPath);
                    Debug.Log($"Created new BGM ScriptableObject: {assetPath}");
                }

                successCount++;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to create ScriptableObject for BGM ID {data.ID}: {e.Message}");
                failCount++;
            }
        }

        EditorUtility.ClearProgressBar();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        // 显示完成对话框
        string message = $"Generation Complete!\n\nSuccess: {successCount}\nFailed: {failCount}\n\nAssets saved to: {OUTPUT_PATH}";
        EditorUtility.DisplayDialog("Complete", message, "OK");

        Debug.Log($"BGM ScriptableObject generation complete. Success: {successCount}, Failed: {failCount}");
    }

    /// <summary>
    /// 清理文件名中的非法字符
    /// </summary>
    private string SanitizeFileName(string fileName)
    {
        char[] invalidChars = Path.GetInvalidFileNameChars();
        foreach (char c in invalidChars)
        {
            fileName = fileName.Replace(c, '_');
        }
        // 额外清理一些可能有问题的字符
        fileName = fileName.Replace(' ', '_');
        fileName = fileName.Replace(':', '_');
        fileName = fileName.Replace('/', '_');
        fileName = fileName.Replace('\\', '_');

        // 限制文件名长度
        if (fileName.Length > 100)
        {
            fileName = fileName.Substring(0, 100);
        }

        return fileName;
    }

    #region BGMData
    public struct BGMData
    {
        public int ID;
        public string Path;
        public Dictionary<string, string> BGMTitles;
        public Dictionary<string, string> AuthorTitles;
    }

    public static List<BGMData> LoadBGMDatas()
    {
        List<BGMData> BGMDatas = new();
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        string filePath = System.IO.Path.Combine(Application.streamingAssetsPath, PathConstants.BGMDataPath);

        if (!File.Exists(filePath))
        {
            Debug.LogError($"BGM Excel file not found at: {filePath}");
            return BGMDatas;
        }

        using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
        {
            using (var reader = ExcelReaderFactory.CreateReader(stream))
            {
                reader.Read(); // Skip the first row (header)
                do
                {
                    while (reader.Read())
                    {
                        if (reader.IsDBNull(0) || string.IsNullOrWhiteSpace(reader.GetValue(0)?.ToString()))
                        {
                            continue;
                        }

                        CellReader cellReader = new CellReader(reader);

                        BGMData data = new BGMData
                        {
                            BGMTitles = new Dictionary<string, string>(),
                            AuthorTitles = new Dictionary<string, string>()
                        };

                        data.ID = cellReader.ReadInt();
                        data.Path = $"{PathConstants.BGMPath}/{cellReader.ReadString()}";
                        data.BGMTitles = cellReader.ReadLocalizationDictionary();
                        data.AuthorTitles = cellReader.ReadLocalizationDictionary();

                        BGMDatas.Add(data);
                    }
                } while (reader.NextResult());
            }
        }
        return BGMDatas;
    }

    /// <summary>
    /// CellReader辅助类 - 自动递增索引
    /// </summary>
    private class CellReader
    {
        private ExcelDataReader.IExcelDataReader reader;
        private int currentIndex;

        public CellReader(ExcelDataReader.IExcelDataReader reader, int startIndex = 0)
        {
            this.reader = reader;
            this.currentIndex = startIndex;
        }

        public int ReadInt()
        {
            int result = 0;
            try
            {
                if (!reader.IsDBNull(currentIndex))
                {
                    object value = reader.GetValue(currentIndex);
                    if (value is int i)
                        result = i;
                    else if (value is double d)
                        result = System.Convert.ToInt32(d);
                    else if (value is string s && int.TryParse(s, out int parsed))
                        result = parsed;
                }
            }
            catch { }
            currentIndex++;
            return result;
        }

        public string ReadString()
        {
            string result = string.Empty;
            try
            {
                if (!reader.IsDBNull(currentIndex))
                {
                    result = reader.GetValue(currentIndex)?.ToString() ?? string.Empty;
                }
            }
            catch { }
            currentIndex++;
            return result;
        }

        public Dictionary<string, string> ReadLocalizationDictionary()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            string[] languageOrder = { LanguageCode.EN, LanguageCode.CNS, LanguageCode.CNT, LanguageCode.JA };

            for (int i = 0; i < languageOrder.Length; i++)
            {
                string value = string.Empty;
                try
                {
                    if (!reader.IsDBNull(currentIndex + i))
                    {
                        value = reader.GetValue(currentIndex + i)?.ToString() ?? string.Empty;
                    }
                }
                catch { }
                dict[languageOrder[i]] = value;
            }

            currentIndex += languageOrder.Length;
            return dict;
        }
    }
    #endregion
}