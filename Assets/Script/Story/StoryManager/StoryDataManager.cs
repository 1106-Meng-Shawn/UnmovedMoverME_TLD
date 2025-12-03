using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using static ExcelReader;

/// <summary>
/// ?????????????????
/// </summary>
public class StoryDataManager : MonoBehaviour
{
    private List<ExcelPlotData> storyData;
    private int currentLine;
    private int currentSheetIndex = 0;
    private string currentStoryFileName;
    private List<int> readSheetIndex = new List<int>();
    private List<int> maxReadLine = new List<int>();

    private readonly int defaultStartLine = Constants.DEFAULT_START_LINE;
    private readonly string excelFileExtension = Constants.EXCEL_FILE_FXTENSION;

    /// <summary>
    /// ?????????
    /// </summary>
    public void LoadStoryFromFile(string fileName, int sheetIndex)
    {
        currentStoryFileName = fileName;
        string filePath = Path.Combine(Application.streamingAssetsPath, $"Text/{fileName + excelFileExtension}");

        storyData = ExcelReader.ReadPlotExcel(filePath, sheetIndex);
        currentSheetIndex = sheetIndex;

        if (storyData == null || storyData.Count == 0)
        {
            Debug.LogError("No data found in the file");
        }

        InitializeMaxReadLine(sheetIndex);
    }

    /// <summary>
    /// ?????????
    /// </summary>
    private void InitializeMaxReadLine(int sheetIndex)
    {
        if (maxReadLine.Count == 0)
        {
            for (int i = 0; i <= sheetIndex; i++)
            {
                maxReadLine.Add(0);
            }
        }
        else if (maxReadLine.Count <= sheetIndex)
        {
            while (maxReadLine.Count <= sheetIndex)
            {
                maxReadLine.Add(0);
            }
        }
    }

    /// <summary>
    /// ???????
    /// </summary>
    public void Initialize(int line, int sheetIndex)
    {
        readSheetIndex = new List<int>();
        currentLine = line;
        currentSheetIndex = sheetIndex;
        readSheetIndex.Add(currentSheetIndex);
        maxReadLine = new List<int>();
        InitializeMaxReadLine(sheetIndex);
    }

    /// <summary>
    /// ????????
    /// </summary>
    public bool CanSkip(bool skipUnread)
    {
        return ((currentLine < maxReadLine[currentSheetIndex]) || skipUnread);
    }

    /// <summary>
    /// ??????
    /// </summary>
    public void AdvanceLineIndex(ExcelPlotData data)
    {
        if (data.ID == Constants.OPTION_END)
        {
            do { currentLine++; }
            while (storyData[currentLine].ID != Constants.OPTION_AFTER);
        }
        else
        {
            currentLine++;
        }
    }

    /// <summary>
    /// ????????
    /// </summary>
    public void UpdateMaxReadLine()
    {
        if (currentLine > maxReadLine[currentSheetIndex])
        {
            if (maxReadLine.Count <= currentSheetIndex)
            {
                for (int i = maxReadLine.Count; i < currentSheetIndex; i++)
                {
                    maxReadLine.Add(0);
                }
            }
            maxReadLine[currentSheetIndex] = currentLine;
        }
    }

    // ==================== Getters ====================

    public List<ExcelPlotData> GetStoryData() => storyData;
    public int GetCurrentLine() => currentLine;
    public int GetCurrentSheetIndex() => currentSheetIndex;
    public string GetCurrentStoryFileName() => currentStoryFileName;
    public List<int> GetReadSheetIndex() => readSheetIndex;
    public List<int> GetMaxReadLine() => maxReadLine;
    public ExcelPlotData GetCurrentData() => storyData != null && currentLine < storyData.Count ? storyData[currentLine] : new ExcelPlotData();

    // ==================== Setters ====================

    public void SetCurrentLine(int line) => currentLine = line;
    public void SetCurrentSheetIndex(int sheetIndex) => currentSheetIndex = sheetIndex;
    public void SetMaxReadLine(List<int> maxLines) => maxReadLine = new List<int>(maxLines);
    public void SetReadSheetIndex(List<int> readSheets) => readSheetIndex = new List<int>(readSheets);

    /// <summary>
    /// ?????????
    /// </summary>
    public static bool NotNullNorEmpty(string str)
    {
        return !string.IsNullOrEmpty(str);
    }
}