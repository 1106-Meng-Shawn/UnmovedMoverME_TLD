using System.Collections.Generic;
using ExcelDataReader;
using System.IO;
using System.Text;
using System;
using System.Diagnostics;
using UnityEngine;
using static ExcelReader;
using Debug = UnityEngine.Debug;


public static class LanguageCode
{
    public const string EN = "en";
    public const string CNS = "zh-Hans";
    public const string CNT = "zh-Hant";
    public const string JA = "ja";

}



public class ExcelReader
{
    #region CellReader Helper Class

    /// <summary>
    /// 包装类，用于自动递增索引读取Excel单元格
    /// 每次读取操作后自动将索引+1
    /// </summary>
    private class CellReader
    {
        private IExcelDataReader reader;
        private int currentIndex;

        public CellReader(IExcelDataReader reader, int startIndex = 0)
        {
            this.reader = reader;
            this.currentIndex = startIndex;
        }

        public int CurrentIndex => currentIndex;

        /// <summary>
        /// 读取整数并自动递增索引
        /// </summary>
        public int ReadInt()
        {
            int result = ExcelReader.ReadInt(reader, currentIndex);
            currentIndex++;
            return result;
        }

        /// <summary>
        /// 读取布尔值并自动递增索引
        /// </summary>
        public bool ReadBool()
        {
            bool result = ExcelReader.ReadBool(reader, currentIndex);
            currentIndex++;
            return result;
        }

        /// <summary>
        /// 读取字符串并自动递增索引
        /// </summary>
        public string ReadString()
        {
            string result = ExcelReader.ReadString(reader, currentIndex);
            currentIndex++;
            return result;
        }

        /// <summary>
        /// 读取浮点数并自动递增索引
        /// </summary>
        public float ReadFloat()
        {
            float result = ExcelReader.ReadFloat(reader, currentIndex);
            currentIndex++;
            return result;
        }

        /// <summary>
        /// 读取本地化字典并自动递增索引（会递增多列）
        /// </summary>
        public Dictionary<string, string> ReadLocalizationDictionary()
        {
            Dictionary<string, string> result = ExcelReader.ReadLocalizationDictionary(reader, ref currentIndex);
            return result;
        }

        /// <summary>
        /// 重置索引到指定位置
        /// </summary>
        public void Reset(int index = 0)
        {
            currentIndex = index;
        }

        /// <summary>
        /// 跳过指定数量的列
        /// </summary>
        public void Skip(int count = 1)
        {
            currentIndex += count;
        }
    }

    #endregion

    #region HelperFunction

    static int ReadInt(IExcelDataReader reader, int index)
    {
        try
        {
            if (reader.IsDBNull(index))
                return 0;

            object value = reader.GetValue(index);

            if (value is int i)
                return i;

            if (value is double d) // Excel often stores numbers as double
                return Convert.ToInt32(d);

            if (value is string s)
            {
                if (int.TryParse(s, out int parsed))
                    return parsed;
                else
                    Debug.LogWarning($"[ReadInt] Cell content is a string but cannot be parsed as int: \"{s}\", at column index {index}");
            }

            // Unexpected type
            Debug.LogWarning($"[ReadInt] Cell content is not an int, but {value.GetType().Name}. Content: {value}, at column index {index}");
            return 0;
        }
        catch (Exception ex)
        {
            Debug.LogError($"[ReadInt] Error while reading int: {ex.Message}, at column index {index}");
            return 0;
        }
    }


    static bool ReadBool(IExcelDataReader r, int index)
    {
        return r.IsDBNull(index) ? false : Convert.ToInt32(r.GetValue(index).ToString()) == 1;
    }



    static string ReadString(IExcelDataReader r, int index)
    {
        return r.IsDBNull(index) ? string.Empty : r.GetValue(index)?.ToString();
    }

    static readonly string[] LanguageOrder = { LanguageCode.EN, LanguageCode.CNS, LanguageCode.CNT, LanguageCode.JA };
    private static Dictionary<string, string> ReadLocalizationDictionary(IExcelDataReader reader, ref int startIndex)
    {
        Dictionary<string, string> dict = new Dictionary<string, string>();
        for (int i = 0; i < LanguageOrder.Length; i++)
        {
            string key = LanguageOrder[i];
            string value = ReadString(reader, startIndex + i);
            dict[key] = value;
        }
        startIndex += LanguageOrder.Length;
        return dict;
    }


    public static List<int> ParseIntListFromCell(string cellContent)
    {
        List<int> result = new List<int>();

        if (string.IsNullOrWhiteSpace(cellContent))
            return result;

        string[] parts = cellContent.Split(',');

        foreach (string part in parts)
        {
            if (int.TryParse(part.Trim(), out int number))
            {
                result.Add(number);
            }
            else
            {
                UnityEngine.Debug.LogWarning($"Can't read?{part}");
            }
        }

        return result;
    }

    static float ReadFloat(IExcelDataReader r, int index)
    {
        if (r.IsDBNull(index)) return 0f;

        var value = r.GetValue(index);
        if (value is double d)
            return (float)d;
        if (float.TryParse(value.ToString(), out float result))
            return result;

        Debug.LogWarning($"[Can't read?{index}?value is?{value}");
        return 0f;
    }

    #endregion
    #region ExcelPlotData


    public struct ExcelPlotData
    {
        public string ID;
        public string speaker;
        public Dictionary<string, string> contents;

        public string storyType;

        public string avatarImageFileName;

        public string effect;

        public string vocalAudioFileName;
        public string backgroundImageFileName;

        public string backgroundMusicFileName;


        public int CharacterCount;
        public List<string> CharacterActions;
        public List<string> CharacterImageFileNames;


        public string currentFileName;
        public string StoryLine;
        public string Type;
        public string PreviousNodeID;

    }

    public static List<ExcelPlotData> ReadPlotExcel(string filePath, int sheetIndex)
    {
        List<ExcelPlotData> excelData = new List<ExcelPlotData>();
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
        {
            using (var reader = ExcelReaderFactory.CreateReader(stream))
            {
                int currentSheetIndex = 1;

                while (currentSheetIndex < sheetIndex && reader.NextResult())
                {
                    currentSheetIndex++;
                }

                if (currentSheetIndex != sheetIndex)
                {
                    throw new ArgumentException($"Sheet index {sheetIndex} is out of bounds. currentSheetIndex is {currentSheetIndex} The file has only {reader.ResultsCount} sheets.");
                }


                do
                {
                    while (reader.Read())
                    {

                        ExcelPlotData data = new ExcelPlotData()
                        {
                            CharacterActions = new List<string>(),
                            CharacterImageFileNames = new List<string>(),
                        };

                        CellReader cellReader = new CellReader(reader);

                        data.ID = cellReader.ReadString();
                        data.speaker = cellReader.ReadString();
                        data.storyType = cellReader.ReadString();
                        data.avatarImageFileName = cellReader.ReadString();
                        data.contents = cellReader.ReadLocalizationDictionary();
                        data.effect = cellReader.ReadString();
                        data.vocalAudioFileName = cellReader.ReadString();
                        data.backgroundImageFileName = cellReader.ReadString();
                        data.backgroundMusicFileName = cellReader.ReadString();
                        data.CharacterCount = cellReader.ReadInt();

                        AddCharacterData(reader, data.CharacterImageFileNames, data.CharacterActions, cellReader.CurrentIndex, data.CharacterCount);

                        excelData.Add(data);
                    }
                } while (reader.NextResult());
            }
        }



        return excelData;

    }


    public static ExcelPlotData GetExcelFilePath(string filePath) //Only First row
    {
        ExcelPlotData excelData = new ExcelPlotData();
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        if (!File.Exists(filePath))
        {
            return default;
        }

        using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
        {
            using (var reader = ExcelReaderFactory.CreateReader(stream))
            {
                if (reader.Read()) // Read the first row only
                {
                    CellReader cellReader = new CellReader(reader);

                    excelData.ID = cellReader.ReadString();
                    excelData.speaker = cellReader.ReadString();
                    excelData.contents = cellReader.ReadLocalizationDictionary();
                    excelData.avatarImageFileName = cellReader.ReadString();
                }
            }
        }

        return excelData;
    }

    private static void AddCharacterData(IExcelDataReader reader, List<string> imageList, List<string> actionList, int startColumnIndex, int characterCount)
    {
        CellReader cellReader = new CellReader(reader, startColumnIndex);

        for (int i = 0; i < characterCount; i++)
        {
            imageList.Add(cellReader.ReadString());
            actionList.Add(cellReader.ReadString());
        }
    }
    #endregion
    #region Item


    public struct ExcelItemData
    {
        public int ID;
        public Dictionary<string, string> itemNames;
        public int rare;
        public int price;
        public int num;
        public int maxInStore;
        public string itemType;
        public Dictionary<string, string> effectDescribe;
        public Dictionary<string, string> itemDescribe;

    }

    public static List<ExcelItemData> GetItemData()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, "Value/ItemValue.xlsx");

        List<ExcelItemData> excelDataList = new List<ExcelItemData>();
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        if (!File.Exists(filePath))
            return excelDataList;

        using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
        {
            using (var reader = ExcelReaderFactory.CreateReader(stream))
            {

                reader.Read(); // Skip the first row (header)
                do
                {
                    while (reader.Read())
                    {

                        ExcelItemData data = new ExcelItemData()
                        {
                            itemNames = new Dictionary<string, string>(),
                            effectDescribe = new Dictionary<string, string>(),
                            itemDescribe = new Dictionary<string, string>(),
                        };

                        CellReader cellReader = new CellReader(reader);

                        data.ID = cellReader.ReadInt();
                        data.itemNames = cellReader.ReadLocalizationDictionary();
                        data.itemType = cellReader.ReadString();
                        data.rare = cellReader.ReadInt();
                        data.price = cellReader.ReadInt();
                        data.num = cellReader.ReadInt();
                        data.maxInStore = cellReader.ReadInt();
                        data.effectDescribe = cellReader.ReadLocalizationDictionary();
                        data.itemDescribe = cellReader.ReadLocalizationDictionary();

                        excelDataList.Add(data);

                    }



                } while (reader.NextResult());
            }
        }
        return excelDataList;
    }

    #endregion
    #region Character

    public struct ExcelCharacterData
    {
        public int ID;
        public string CharacterKey;
        public string characterType;
        public string country;
        public int roleClass;

        public bool isPersonBattle;
        public bool canDeleted;
        public int maxLevel;
        public int currentLevel;


        public string itemWithCharacter; // maybe change to ID
        public int moveNum;
        public int maxForce;
        public int force;
        public int favorability;
        public int favorabilityLevel;
        public int captureProbalilty;
        public int recruitCost;

        public List<int> initValues;

        //        public List<string> initSkillNames; // need to add skill library
        public List<int> initSkillIDs;
        public List<int> initSkillLibraryIDs;

        public Dictionary<string, string> characterName;
    }

    public static List<ExcelCharacterData> GetCharacterData()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, "Value/CharacterValue.xlsx");


        List<ExcelCharacterData> excelDataList = new List<ExcelCharacterData>();
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        if (!File.Exists(filePath))
            return excelDataList;

        using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
        {
            using (var reader = ExcelReaderFactory.CreateReader(stream))
            {

                reader.Read();
                while (reader.Read())
                {
                    excelDataList.Add(CharacterData(reader, string.Empty));

                }

            }
        }
        return excelDataList;
    }

    public static List<ExcelCharacterData> GetImportantCharacterData()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, "Value/CharacterValue.xlsx");

        List<ExcelCharacterData> excelDataList = new List<ExcelCharacterData>();
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        if (!File.Exists(filePath))
            return excelDataList;

        using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
        {
            using (var reader = ExcelReaderFactory.CreateReader(stream))
            {
                reader.Read();

                while (reader.Read())
                {
                    excelDataList.Add(CharacterData(reader, string.Empty));
                }
            }
        }
        return excelDataList;
    }


    public static List<ExcelCharacterData> GetNormalCharacterData()
    {

        string filePath = Path.Combine(Application.streamingAssetsPath, "Value/CharacterValue.xlsx");


        List<ExcelCharacterData> excelDataList = new List<ExcelCharacterData>();
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        if (!File.Exists(filePath))
            return excelDataList;

        using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
        {
            using (var reader = ExcelReaderFactory.CreateReader(stream))
            {

                int sheetIndex = 0;
                while (sheetIndex < 1)
                {
                    if (!reader.NextResult())
                        return excelDataList;
                    sheetIndex++;
                }


                reader.Read();
                while (reader.Read())
                {
                    excelDataList.Add(CharacterData(reader, string.Empty));

                }
            }
        }
        return excelDataList;
    }

    public static List<ExcelCharacterData> GetMonsterData()
    {

        string filePath = Path.Combine(Application.streamingAssetsPath, "Value/CharacterValue.xlsx");


        List<ExcelCharacterData> excelDataList = new List<ExcelCharacterData>();
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        if (!File.Exists(filePath))
            return excelDataList;

        using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
        {
            using (var reader = ExcelReaderFactory.CreateReader(stream))
            {

                int sheetIndex = 0;
                while (sheetIndex < 2)
                {
                    if (!reader.NextResult())
                        return excelDataList;
                    sheetIndex++;
                }


                reader.Read();
                while (reader.Read())
                {
                    excelDataList.Add(CharacterData(reader, "Monster/"));

                }
            }
        }
        return excelDataList;
    }



    static ExcelCharacterData CharacterData(IExcelDataReader reader, string Prefix)
    {
        ExcelCharacterData data = new ExcelCharacterData()
        {
            initValues = new List<int>(),
            initSkillIDs = new List<int>(),
            initSkillLibraryIDs = new List<int>(),
            characterName = new Dictionary<string, string>(),
        };

        CellReader cellReader = new CellReader(reader);

        data.ID = cellReader.ReadInt();
        data.CharacterKey = cellReader.ReadString();
        data.characterType = Prefix + cellReader.ReadString();
        data.country = cellReader.ReadString();
        data.roleClass = cellReader.ReadInt();
        data.isPersonBattle = cellReader.ReadBool();
        data.canDeleted = cellReader.ReadBool();
        data.maxLevel = cellReader.ReadInt();
        data.currentLevel = cellReader.ReadInt();
        data.itemWithCharacter = cellReader.ReadString();
        data.moveNum = cellReader.ReadInt();
        data.maxForce = cellReader.ReadInt();
        data.force = cellReader.ReadInt();
        data.favorability = cellReader.ReadInt();
        data.favorabilityLevel = cellReader.ReadInt();
        data.captureProbalilty = cellReader.ReadInt();
        data.recruitCost = cellReader.ReadInt();

        for (int i = 0; i < 15; i++)
        {
            data.initValues.Add(cellReader.ReadInt());
        }

        for (int i = 0; i < 5; i++)
        {
            data.initSkillIDs.Add(cellReader.ReadInt());
        }

        string SkillLibraryString = cellReader.ReadString();
        data.initSkillLibraryIDs = ParseIntListFromCell(SkillLibraryString);
        data.characterName = cellReader.ReadLocalizationDictionary();

        return data;
    }




    public struct ExcelCharacterENDData
    {
        public int ENDID;
        public int CharacterID;
        //  public string ENDTitle;
        public string CharacterENDIcon;
        public bool isGE;
        public bool isTE;
        public Dictionary<string, string> ENDContent;

    }

    public static List<ExcelCharacterENDData> GetChracterENDData()
    {

        string filePath = Path.Combine(Application.streamingAssetsPath, "Value/CharacterEND.xlsx");

        List<ExcelCharacterENDData> excelDataList = new List<ExcelCharacterENDData>();
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        if (!File.Exists(filePath))
            return excelDataList;

        using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
        {
            using (var reader = ExcelReaderFactory.CreateReader(stream))
            {
                reader.Read();
                do
                {
                    while (reader.Read())
                    {
                        CellReader cellReader = new CellReader(reader);

                        ExcelCharacterENDData data = new ExcelCharacterENDData()
                        {
                            ENDContent = new Dictionary<string, string>()
                        };

                        data.ENDID = cellReader.ReadInt();
                        data.CharacterID = cellReader.ReadInt();
                        cellReader.Skip(); //END title in the excel, don't care
                        data.CharacterENDIcon = cellReader.ReadString();
                        data.isGE = cellReader.ReadBool();
                        data.isTE = cellReader.ReadBool();
                        data.ENDContent = cellReader.ReadLocalizationDictionary();

                        excelDataList.Add(data);

                    }



                } while (reader.NextResult());
            }
        }
        return excelDataList;
    }


    #endregion
    #region Skill

    public struct ExcelSkillData
    {
        public int ID;
        public Dictionary<string, string> skillNames;
        public int skillRare;
        // public int skillType;
        public string functionType;
        public string targetType;
        public string rangeType;
        public string triggerType;
        public int moveCost;
        public int speedCost;
        public string targetLifeStatus;
        public int distance;
        public string skillEffect;
        public Dictionary<string, string> skillDescribe;
    }

    public static List<ExcelSkillData> GetSkillData()
    {

        string filePath = Path.Combine(Application.streamingAssetsPath, "Value/SkillValue.xlsx");


        List<ExcelSkillData> excelDataList = new List<ExcelSkillData>();
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        if (!File.Exists(filePath))
            return excelDataList;

        using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
        {
            using (var reader = ExcelReaderFactory.CreateReader(stream))
            {

                reader.Read();
                do
                {
                    while (reader.Read())
                    {

                        ExcelSkillData data = new ExcelSkillData()
                        {
                            skillNames = new Dictionary<string, string>(),
                            skillDescribe = new Dictionary<string, string>(),
                        };

                        CellReader cellReader = new CellReader(reader);

                        data.ID = cellReader.ReadInt();
                        data.skillNames = cellReader.ReadLocalizationDictionary();
                        data.skillRare = cellReader.ReadInt();
                        // data.skillType = cellReader.ReadInt();
                        data.functionType = cellReader.ReadString();
                        data.targetType = cellReader.ReadString();
                        data.rangeType = cellReader.ReadString();
                        data.triggerType = cellReader.ReadString();
                        data.moveCost = cellReader.ReadInt();
                        data.speedCost = cellReader.ReadInt();
                        data.targetLifeStatus = cellReader.ReadString();
                        data.distance = cellReader.ReadInt();
                        data.skillDescribe = cellReader.ReadLocalizationDictionary();
                        data.skillEffect = cellReader.ReadString();

                        excelDataList.Add(data);

                    }



                } while (reader.NextResult());
            }
        }
        return excelDataList;
    }

    #endregion
    #region Region

    public struct ExcelRegionData
    {
        public int ID;
        public Dictionary<string, string> regionName;
        public string countryName;
        public string initLordKey;
        public int cityNum;

        public string initRelation;

        public int initMAXPopulation;
        public float InitTaxRate;
        public List<ExcelCityValueData> cityValueDatas;

        public List<int> riverCount; // 0 is min, 1 is max

        public bool hasSea;
        public float elevationMIN;
        public float elevationMAX;

        public float temperatureMIN;
        public float temperatureMAX;

        public float humidityMIN;
        public float humidityMAX;
    }

    public struct ExcelCityValueData
    {
        public float InitSupporRate;
        public string cityName;
        public string cityCountry;
        public int initMaxPop;
        public int initPop;
        public int initRecuitedPop;

        public List<float> initResourceMax;
        public List<float> initResourceSurplus;
        public List<float> baseParameter;

        public List<int> ExploreItemsID;
        public List<int> ExploreMonsterID;
    }

    public static List<ExcelRegionData> GetRegionData()
    {

        string filePath = Path.Combine(Application.streamingAssetsPath, "Value/RegionValue.xlsx");


        List<ExcelRegionData> excelDataList = new List<ExcelRegionData>();
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        if (!File.Exists(filePath))
            return excelDataList;

        using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
        {
            using (var reader = ExcelReaderFactory.CreateReader(stream))
            {

                reader.Read();
                do
                {
                    while (reader.Read())
                    {
                        if (reader.IsDBNull(0)) continue;


                        ExcelRegionData data = new ExcelRegionData()
                        {
                            regionName = new Dictionary<string, string>(),
                            cityValueDatas = new List<ExcelCityValueData>(),
                            riverCount = new List<int>()

                        };

                        CellReader cellReader = new CellReader(reader);

                        data.ID = cellReader.ReadInt();
                        data.regionName = cellReader.ReadLocalizationDictionary();
                        data.countryName = cellReader.ReadString();
                        data.initLordKey = cellReader.ReadString();
                        data.initRelation = cellReader.ReadString();
                        data.riverCount.Add(cellReader.ReadInt());
                        data.riverCount.Add(cellReader.ReadInt());

                        int Sea = cellReader.ReadInt();
                        data.hasSea = Sea == 1;

                        data.elevationMIN = cellReader.ReadFloat();
                        data.elevationMAX = cellReader.ReadFloat();
                        data.temperatureMIN = cellReader.ReadFloat();
                        data.temperatureMAX = cellReader.ReadFloat();
                        data.humidityMIN = cellReader.ReadFloat();
                        data.humidityMAX = cellReader.ReadFloat();
                        data.InitTaxRate = cellReader.ReadFloat();
                        data.cityNum = cellReader.ReadInt();

                        for (int i = 0; i < data.cityNum; i++) // ?????3???
                        {
                            ExcelCityValueData cityData = new ExcelCityValueData
                            {
                                cityName = cellReader.ReadString(),
                                cityCountry = cellReader.ReadString(),
                                initMaxPop = cellReader.ReadInt(),
                                initPop = cellReader.ReadInt(),
                                initRecuitedPop = cellReader.ReadInt(),
                                InitSupporRate = cellReader.ReadFloat(),
                                initResourceMax = new List<float>(),
                                initResourceSurplus = new List<float>(),
                                baseParameter = new List<float>(),
                                ExploreItemsID = new List<int>(),
                                ExploreMonsterID = new List<int>()
                            };

                            // ?????????????5????
                            for (int k = 0; k < 5; k++)
                            {
                                cityData.initResourceMax.Add(cellReader.ReadFloat());
                                cityData.initResourceSurplus.Add(cellReader.ReadFloat());
                                cityData.baseParameter.Add(cellReader.ReadFloat());
                            }
                            cityData.ExploreItemsID = ParseIntListFromCell(cellReader.ReadString());
                            cityData.ExploreMonsterID = ParseIntListFromCell(cellReader.ReadString());

                            data.cityValueDatas.Add(cityData);
                        }

                        excelDataList.Add(data);
                    }

                } while (reader.NextResult());
            }
        }
        return excelDataList;
    }


    #endregion
    #region StoryGraph

    public struct ExcelStoryData
    {
        public int ID;
        public string NodeName;
        public Dictionary<string, string> Title;
        public Dictionary<string, string> Introduction;
        public string currentFileName;
        public string Type;
        public string StoryLine;
        public string StoryImage;
        public List<int> PreviousNodeID;
        public int NextNodeID;

    }

    public static List<ExcelStoryData> ReadStoryGraph(string filePath)
    {
        List<ExcelStoryData> excelData = new List<ExcelStoryData>();
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);


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

                        ExcelStoryData data = new ExcelStoryData()
                        {
                            Title = new Dictionary<string, string>(),
                            Introduction = new Dictionary<string, string>(),
                            PreviousNodeID = new List<int>()
                        };

                        data.ID = cellReader.ReadInt();
                        data.NodeName = cellReader.ReadString();// title
                        data.Title = cellReader.ReadLocalizationDictionary();
                        data.Introduction = cellReader.ReadLocalizationDictionary();
                        data.currentFileName = cellReader.ReadString();
                        data.Type = cellReader.ReadString();
                        data.StoryLine = cellReader.ReadString();
                        data.StoryImage = cellReader.ReadString();
                        data.PreviousNodeID = ParseIntListFromCell(cellReader.ReadString());

                        excelData.Add(data);
                    }
                } while (reader.NextResult());
            }
        }

        return excelData;

    }

    #endregion
    #region EventAndTask

    public struct ExcelTaskValue
    {
        public int ID;
        public string storyKey;
        public string taskID;

        public TaskSize size;
        public TaskType type;

        public int deadLineTurn;
        public string characterKey;
        public int regionID;
        public int cityIndex;

    }

    public struct ExcelTaskStory
    {
        public int ID;
        public string key;
        public Dictionary<string, string> Title;
        public Dictionary<string, string> Introduction;
        public string currentFileName;
        public string StoryLine;
        public string StoryImage;
        public int PreviousNodeID;
        public Dictionary<string, string> OptionsAsk;
        public List<Dictionary<string, string>> OptionsTexts;
        public List<string> OptionsCosts;
        public List<string> OptionsActions;
    }


    public struct ExcelEventData
    {
        public int ID;
        public string NodeName;

        public Dictionary<string, string> Title;           // ?????
        public Dictionary<string, string> Introduction;   // ?????

        public string currentFileName;
        public string Type;
        public string TaskType;

        public string StoryLine;
        public string StoryImage;
        public int PreviousNodeID;

        public Dictionary<string, string> OptionsAsk;

        public List<Dictionary<string, string>> OptionsTexts;
        public List<string> OptionsCosts;
        public List<string> OptionsActions;
    }
    public static List<ExcelEventData> ReadEventGraph()
    {


        string filePath = Path.Combine(Application.streamingAssetsPath, "Text/Events/EventsGraph.xlsx");


        List<ExcelEventData> excelData = new List<ExcelEventData>();
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

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

                        ExcelEventData data = new ExcelEventData()
                        {
                            Title = new Dictionary<string, string>(),
                            Introduction = new Dictionary<string, string>(),
                            OptionsAsk = new Dictionary<string, string>(),
                            OptionsTexts = new List<Dictionary<string, string>>(),
                            OptionsCosts = new List<string>(),
                            OptionsActions = new List<string>(),
                            PreviousNodeID = -1

                        };

                        data.ID = cellReader.ReadInt();
                        data.NodeName = cellReader.ReadString();// title
                        data.Title = cellReader.ReadLocalizationDictionary();
                        data.Introduction = cellReader.ReadLocalizationDictionary();
                        data.currentFileName = cellReader.ReadString();
                        data.Type = cellReader.ReadString();
                        data.TaskType = cellReader.ReadString();
                        data.StoryLine = cellReader.ReadString();
                        data.StoryImage = cellReader.ReadString();
                        data.PreviousNodeID = cellReader.ReadInt();
                        data.OptionsAsk = cellReader.ReadLocalizationDictionary();

                        int OptionsCount = cellReader.ReadInt();

                        for (int i = 0; i < OptionsCount; i++)
                        {
                            Dictionary<string, string> OptionText = cellReader.ReadLocalizationDictionary();
                            data.OptionsTexts.Add(OptionText);
                            data.OptionsCosts.Add(cellReader.ReadString());
                            data.OptionsActions.Add(cellReader.ReadString());

                        }



                        excelData.Add(data);
                    }
                } while (reader.NextResult());
            }
        }

        return excelData;

    }

    public static List<ExcelTaskValue> ReadTaskValue()
    {

        string filePath = Path.Combine(Application.streamingAssetsPath, "Text/Tasks/TasksValue.xlsx");
        List<ExcelTaskValue> excelData = new List<ExcelTaskValue>();
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

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

                        ExcelTaskValue data = new ExcelTaskValue();

                        data.ID = cellReader.ReadInt();
                        cellReader.Skip();// Node name , no need in data;
                        data.taskID = cellReader.ReadString();
                        data.storyKey = cellReader.ReadString();
                        data.size = Tool.ParseEnumOrDefault<TaskSize>(cellReader.ReadString());
                        data.type = Tool.ParseEnumOrDefault<TaskType>(cellReader.ReadString());
                        data.deadLineTurn = cellReader.ReadInt();
                        data.characterKey = cellReader.ReadString();
                        cellReader.Skip();// Character name , no need in data;
                        data.regionID = cellReader.ReadInt();
                        cellReader.Skip();// Region name , no need in data;
                        data.cityIndex = cellReader.ReadInt();
                        cellReader.Skip();// City name , no need in data;

                        excelData.Add(data);
                    }
                } while (reader.NextResult());
            }
        }

        return excelData;

    }

    public static List<ExcelTaskStory> ReadTaskStory()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, "Text/Tasks/TaskStory.xlsx");
        List<ExcelTaskStory> excelData = new List<ExcelTaskStory>();

        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
        {
            using (var reader = ExcelReaderFactory.CreateReader(stream))
            {
                reader.Read(); // Skip header row

                do
                {
                    while (reader.Read())
                    {
                        if (reader.IsDBNull(0) || string.IsNullOrWhiteSpace(reader.GetValue(0)?.ToString()))
                        {
                            continue;
                        }

                        CellReader cellReader = new CellReader(reader);

                        ExcelTaskStory data = new ExcelTaskStory
                        {
                            Title = new Dictionary<string, string>(),
                            Introduction = new Dictionary<string, string>(),
                            OptionsAsk = new Dictionary<string, string>(),
                            OptionsTexts = new List<Dictionary<string, string>>(),
                            OptionsCosts = new List<string>(),
                            OptionsActions = new List<string>()
                        };

                        data.ID = cellReader.ReadInt();
                        data.key = cellReader.ReadString();
                        data.Title = cellReader.ReadLocalizationDictionary();
                        data.Introduction = cellReader.ReadLocalizationDictionary();
                        data.currentFileName = cellReader.ReadString();
                        data.StoryLine = cellReader.ReadString();
                        data.StoryImage = cellReader.ReadString();
                        data.PreviousNodeID = cellReader.ReadInt();
                        data.OptionsAsk = cellReader.ReadLocalizationDictionary();

                        int optionsCount = cellReader.ReadInt();

                        for (int i = 0; i < optionsCount; i++)
                        {
                            data.OptionsTexts.Add(cellReader.ReadLocalizationDictionary());
                            data.OptionsCosts.Add(cellReader.ReadString());
                            data.OptionsActions.Add(cellReader.ReadString());
                        }

                        excelData.Add(data);
                    }
                }
                while (reader.NextResult());
            }
        }

        return excelData;
    }

    #endregion
    #region GameEND

    public struct ExcelGameENDData
    {
        public int ID;
        public string ENDKey;
        public Dictionary<string, string> ENDTitles;
        public string backgroundImPath;

    }

    public static List<ExcelGameENDData> ReadGameENDData()
    {
        List<ExcelGameENDData> excelData = new List<ExcelGameENDData>();
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        string filePath = Path.Combine(Application.streamingAssetsPath, "Value/ENDTitle.xlsx");


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

                        ExcelGameENDData data = new ExcelGameENDData()
                        {
                            ENDTitles = new Dictionary<string, string>(),
                        };

                        data.ID = cellReader.ReadInt();
                        data.ENDKey = cellReader.ReadString();
                        data.ENDTitles = cellReader.ReadLocalizationDictionary();
                        data.backgroundImPath = cellReader.ReadString();

                        excelData.Add(data);
                    }
                } while (reader.NextResult());
            }
        }

        return excelData;

    }

    #endregion
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

        string filePath = Path.Combine(Application.streamingAssetsPath, PathConstants.BGMDataPath);


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

                        BGMData data = new BGMData();

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



    #endregion

    #region ControlEditor
    public struct CityConnetData
    {
        public int CityConnetID;

        public string Region1Name;
        public int Region1ID;
        public string City1Name;
        public int City1ID;

        public string Region2Name;
        public int Region2ID;
        public string City2Name;
        public int City2ID;
    }

    public static List<CityConnetData> LoadCityConnections()
    {
        List<CityConnetData> connections = new();

        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        string filePath = Path.Combine(Application.streamingAssetsPath, "Value/CityConnect.xlsx");


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

                        CityConnetData data = new CityConnetData();

                        data.CityConnetID = cellReader.ReadInt();
                        data.Region1Name = cellReader.ReadString();
                        data.Region1ID = cellReader.ReadInt();
                        data.City1Name = cellReader.ReadString();
                        data.City1ID = cellReader.ReadInt();
                        data.Region2Name = cellReader.ReadString();
                        data.Region2ID = cellReader.ReadInt();
                        data.City2Name = cellReader.ReadString();
                        data.City2ID = cellReader.ReadInt();

                        connections.Add(data);
                    }
                } while (reader.NextResult());
            }
        }

        return connections;
    }


    public static List<CityConnetData> LoadCityConnections(string filePath)
    {
        List<CityConnetData> connections = new();

        if (!File.Exists(filePath))
        {
            Debug.LogWarning("????????" + filePath);
            return connections;
        }

        var lines = File.ReadAllLines(filePath);

        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;

            string[] parts = line.Split(',');

            if (parts.Length != 8)
            {
                Debug.LogWarning("?????" + line);
                continue;
            }

            if (!int.TryParse(parts[1].Trim(), out int region1ID) ||
                !int.TryParse(parts[3].Trim(), out int city1ID) ||
                !int.TryParse(parts[5].Trim(), out int region2ID) ||
                !int.TryParse(parts[7].Trim(), out int city2ID))
            {
                Debug.LogWarning("ID ?????" + line);
                continue;
            }

            connections.Add(new CityConnetData
            {
                Region1Name = parts[0].Trim(),
                Region1ID = region1ID,
                City1Name = parts[2].Trim(),
                City1ID = city1ID,
                Region2Name = parts[4].Trim(),
                Region2ID = region2ID,
                City2Name = parts[6].Trim(),
                City2ID = city2ID
            });
        }

        return connections;
    }



    #endregion

}





#region Constants


public class PathConstants
{
    public const string BGMDataPath = "SettingValue/BGMData.xlsx";
    public const string BGMPath = "Sounds/BGM";

}



public class ActionConstants
{
    public const string APPEAR_AT = "AppearAt";
    public const string APPEAR_AT_INSTANTLY = "AppearAtInstantly";
    public const string DISAPPEAR = "Disappear";
    public const string MOVE_TO = "MoveTo";
    public const string MOVE_BY = "MoveBy";
    public const string STAY = "StayAt";


}

public class Constants
{
    public static string EXCEL_FILE_FXTENSION = ".xlsx";
    public static int DEFAULT_START_LINE = 2;
    public static char placeholder = '\uFFF9';


    public static string AVATAR_PATH = "MyDraw/Character/";
    public static string CHARACTER_PATH = "MyDraw/Character/";


    public static string VOCAL_PATH = "audio/vocal/";
    public static string AUDIO_LOAD_FAILED = "Failed to load audio: ";
    public static string IMAGE_LOAD_FAILED = "Failed to load image:";

    //public static string BACKGROUND_PATH = "MyDraw/Background/";
    public static string BACKGROUND_PATH = "MyDraw/";

    public static string MUSIC_PATH = "audio/music/";
    public static string MUSIC_LOAD_FAILED = "Failed to load music: ";

    public static float DEFAULT_TYPING_SPEED = 0.05f;
    public static float SKIP_TYPING_SPEED = 0.01f;


    public const string UNKNOWN = "Unknown";
    public static string NO_DATA_FOUND = "No data found";
    public static string CAMERA_NOT_FOUND = "No Camera found";


    public const string DIALOGUE = "Dialogue";
    public const string PANEL = "Panel";
    public const string VOICEOVER = "Voiceover";



    public static string CHOICE = "Choice";
    public static string OPTION_END = "Option End";
    public static string OPTION_AFTER = "After Option";

    public static int DEFAULT_SHEET_INDEX = 1;

    /* public static string GO_TO_FILE = "GoToFile";
     public static string GO_TO_SHEET = "GoToSheet";*/



    public static string END_OF_STORY = "END";
    public static float DEFAULT_WAITING_SECONDS = 2.0f;
    public static float DEFAULT_SKIP_WAITING_SECONDS = 0.05f;

    public static int DURATION_TIME = 1;
    // public static float AUTOPLAY_WAITING_SECONDS = 1f;
    public static string COORDINATE_MISSING = "Coordinate missing";


    public static int DEFAULT_START_INDEX = 2;
    public static int SLOTS_PER_PAGE = 8; // need to change to 10;
    public static int TOTAL_SLOTS = 80; // need to change to 100;
    public static string COLON = ": ";
    public static string SAVE_GAME = "save_game";
    public static string LOAD_GAME = "load_game";
    public static string EMPLY_SLOT = "empty_slot";

    public static string SAVE_FILE_PATH = "saves";
    public static string SAVE_FILE_EXTENSION = ".json";


    public static string STORY_PROGRESS_SaveFileName = "Story_Progress.json";


    public static string RANDOM = "RAND";
}

#endregion