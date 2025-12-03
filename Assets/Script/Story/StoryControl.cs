using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization.Settings;
using static ExcelReader;

#region StoryControl
public class StoryControl
{
    private StoryGraph storyGraph; // Instance of the StoryGraph class to manage the story nodes
    private StoryGraph eventsGraph;

    private Dictionary<int, List<TaskData>> taskNodeDict;
    public GameValue gameValue;

    public void Init()
    {
        storyGraph = new StoryGraph(); // Initialize the StoryGraph instance
        storyGraph.GenerateStoryGraph();

        eventsGraph = new StoryGraph();
        eventsGraph.GenerateEventGraph();

        taskNodeDict = GenerateTaskDict();

    }

    public List<TaskData> GetTotalTaskList()
    {
        return taskNodeDict.Values.SelectMany(list => list).ToList();
    }

    Dictionary<int, List<TaskData>> GenerateTaskDict()
    {
        Dictionary<int, List<TaskData>> TaskDict = new Dictionary<int, List<TaskData>>();
        List<ExcelTaskValue> taskNodeData = ExcelReader.ReadTaskValue();

        if (taskNodeData == null || taskNodeData.Count == 0)
        {
            Debug.LogWarning("No data found in the task graph file.");
            return TaskDict;
        }

        foreach (var task in taskNodeData)
        {
            ExcelTaskStory excelTaskStory = GetTaskStoryByKey(task.storyKey);
            TaskData taskData = new TaskData(excelTaskStory,task);
            if (!TaskDict.ContainsKey(taskData.GetRegionID()))
            {
                TaskDict[taskData.GetRegionID()] = new List<TaskData>();
            }

            TaskDict[taskData.GetRegionID()].Add(taskData);
        }

        return TaskDict;
    }


    public StoryNode GetStoryNode()
    {

       return test();
    }


    public StoryNode PrologueStoryLine()
    {
        StoryNode test = storyGraph.StoryHappenByStoryLine("1368StoryLine");
        if (test != null)
        {
            Debug.Log("Node found: " + test.Title);
            Debug.Log("Introduction: " + test.Introduction);
            Debug.Log("StoryLine: " + test.StoryLine);
            Debug.Log("Next Nodes Count: " + test.NextNodes.Count);
        }
        else
        {
            Debug.Log("Node not found.");
        }

        return test;
    }

    public StoryNode test()
    {
        StoryNode test = storyGraph.StoryHappenByStoryLine("test");
        return test;

        if (test != null)
        {
            Debug.Log("Node found: " + test.Title);
            Debug.Log("Introduction: " + test.Introduction);
            Debug.Log("StoryLine: " + test.StoryLine);
           // Debug.Log("Type: " + test.Type);
            Debug.Log("Next Nodes Count: " + test.NextNodes.Count);
        }
        else
        {
            Debug.Log("Node not found.");
        }

       // return test;
    }

    public StoryControlSaveData GetSaveData()
    {
        StoryControlSaveData saveData = new StoryControlSaveData(this);
        return saveData;
    }


    public void SetSaveData(StoryControlSaveData saveData)
    {
        GenerateTaskDict();
        taskNodeDict = new Dictionary<int, List<TaskData>>();
        if (saveData.taskSaveNodeList == null || saveData.taskSaveNodeList.Count == 0)
        {
            Debug.LogWarning("No task save data found.");
            return;
        }
        foreach (var saveTask in saveData.taskSaveNodeList)
        {
            ExcelTaskStory excelTaskStory = GetTaskStoryByKey(saveTask.storyKey);
            TaskData taskData = new TaskData(excelTaskStory, saveTask);
            if (!taskNodeDict.ContainsKey(taskData.GetRegionID()))
            {
                taskNodeDict[taskData.GetRegionID()] = new List<TaskData>();
            }
            taskNodeDict[taskData.GetRegionID()].Add(taskData);
            Debug.Log($"{excelTaskStory.ID} why you cant work?{excelTaskStory.currentFileName} {excelTaskStory.Title} {excelTaskStory}");
            Debug.Log($"{taskData.GetTaskKey()} why you cant work?{taskData.GetCharacterKey()}");
        }
    }

    public TaskData GetTaskDataByTaskKey(string taskKey)
    {
        var task = taskNodeDict
            .SelectMany(kvp => kvp.Value)
            .FirstOrDefault(task => task.GetTaskKey() == taskKey);

        if (task == null)
        {
            UnityEngine.Debug.LogWarning($"[GetTaskDataByTaskKey] Task not found for key: {taskKey}");
        }

        return task;
    }

    public bool HasTask(string taskKey)
    {
        return taskNodeDict
            .SelectMany(kvp => kvp.Value)
            .Any(task => task.GetTaskKey() == taskKey);
    }


    #region Helper
    public ExcelTaskStory GetTaskStoryByKey(string key)
    {

        ExcelTaskStory excelTaskStory = GameValue.Instance.GetExcelTaskStory(key);

        if (string.IsNullOrEmpty(excelTaskStory.key))  
        {
            Debug.LogWarning($"TaskStory with key [{key}] not found.");
        }

        return excelTaskStory;
    }





    #endregion

}
#endregion

#region StoryGraph

public class StoryGraph 
{
    public List<StoryNode> rootNodes; // The root node of the story graph
    private List<ExcelEventData> eventNodeData;
    private List<ExcelStoryData> storyNodeData;
    public static string storyGraphMangeFile = "Assets/StreamingAssets/Text/Story/StoryGraph.xlsx";
    public string nextHappendNodeID; // ID of the next node (if any)


    public StoryNode StoryHappenByStoryLine(string storyLine)
    {
        foreach (var rootNode in rootNodes)
        {
            if (rootNode.StoryLine == storyLine)
            {
                return StoryHappen(rootNode);
            }
        }
        return null;
    }

    StoryNode StoryHappen(StoryNode Node)
    {
        if (Node.isHappend && Node.NextNodes.Count == 0) return null; 

        if (Node.isHappend)
        {
            if (Node.NextNodes != null) {
             //   if (Node.Type == "Random Branches") return RandomBranches(Node);
              // if (Node.Type == "normal") return StoryHappen(Node.NextNodes[0]);

            }
            return null;
        }
        else
        {
            Node.isHappend = true;
            nextHappendNodeID = Node.NodeName;
            return Node;
        }
    }

    StoryNode RandomBranches(StoryNode Node)
    {
        List<StoryNode> unHappenedNodes = new List<StoryNode>();

        foreach (StoryNode nextNode in Node.NextNodes)
        {
            if (!nextNode.isHappend)  
            {
                unHappenedNodes.Add(nextNode);
            }
        }

        if (unHappenedNodes.Count > 0)
        {
            return unHappenedNodes[Random.Range(0, unHappenedNodes.Count)];
        }
        else
        {
            return Node.NextNodes[0].NextNodes.Count > 0 ? StoryHappen(Node.NextNodes[0].NextNodes[0]) : null;
        }
    }
    public void GenerateStoryGraph()
    {
        storyNodeData = ExcelReader.ReadStoryGraph(storyGraphMangeFile);

        if (storyNodeData == null || storyNodeData.Count == 0)
        {
            Debug.LogWarning("No data found in the Story graph file.");
            return;
        } else
        {
           // Debug.Log($"storyNodeData has {storyNodeData.Count}");

        }
        CreateStoryNodesByNodeDate();

    }

    public void GenerateEventGraph()
    {

        eventNodeData = ExcelReader.ReadEventGraph();

        if (eventNodeData == null || eventNodeData.Count == 0)
        {
            Debug.LogWarning("No data found in the event graph file.");
            return;
        }
        else
        {
          //  Debug.Log($"event graph has {eventAndTaskNodeData.Count}");

        }

        CreateEventNodesByNodeDate();
    }


    void CreateStoryNodesByNodeDate()
    {
        rootNodes = new List<StoryNode>();

        var storyLines = new Dictionary<string, StoryNode>();
        var allNodes = new List<StoryNode>(); // ??????????????

        foreach (var data in storyNodeData)
        {
            StoryNode node = new StoryNode
            {
                ID = data.ID,
                Title = data.Title,
                Introduction = data.Introduction,
                storyImage = data.StoryImage,
                currentFileName = $"Story/{data.StoryLine}/{data.currentFileName}",  // Convert to boolean
                Prefix = "Story",
                StoryLine = data.StoryLine,
                NextNodes = new List<StoryNode>(),
            };

            // may be it have bug
           if (data.PreviousNodeID.Count != 0)
            {
                rootNodes.Add(node);
                storyLines[data.StoryLine] = node;
            }

            allNodes.Add(node);

            if (data.PreviousNodeID.Count != 0)
            {

                foreach (var parentNodeID in data.PreviousNodeID)
                {
                    foreach (var foundNode in allNodes)
                    {
                        if (foundNode.ID == parentNodeID && foundNode.StoryLine == data.StoryLine)
                        {
                            foundNode.AddNextNode(node);
                            break;
                        }
                    }
                }
            }
        }
    }

    void CreateEventNodesByNodeDate()
    {
        rootNodes = new List<StoryNode>();

        // ????????????
        var storyLines = new Dictionary<string, StoryNode>();
        var allNodes = new List<StoryNode>(); // ??????????????

        foreach (var data in eventNodeData)
        {
            StoryNode node = new StoryNode
            {
                ID = data.ID,
                Title = data.Title,
                Introduction = data.Introduction,
                storyImage = data.StoryImage,
                Prefix = "Events",
                StoryLine = data.StoryLine,
                currentFileName = $"Events/{data.StoryLine}/{data.currentFileName}",
                NextNodes = new List<StoryNode>(),  // Initialize an empty list for next nodes
                OptionsAsk = data.OptionsAsk,
                OptionsText = data.OptionsTexts,
                OptionsCost = data.OptionsCosts,
                OptionsAction = data.OptionsActions,
            };

            // of couse it have bug
            if (data.PreviousNodeID != -1)
            {
                rootNodes.Add(node);
                storyLines[data.StoryLine] = node;
            }

            allNodes.Add(node);

            if (data.PreviousNodeID != -1)
            {

                    foreach (var foundNode in allNodes)
                    {
                        if (foundNode.ID == data.PreviousNodeID && foundNode.StoryLine == data.StoryLine)
                        {
                            foundNode.AddNextNode(node);
                            break;
                        }
                    }
            }
        }
    }

   /* void CreateTaskNodesByNodeDate()
    {
        rootNodes = new List<StoryNode>();

        var storyLines = new Dictionary<string, StoryNode>();
        var allNodes = new List<StoryNode>();

        foreach (var data in taskNodeData)
        {
            StoryNode node = new StoryNode
            {
                ID = data.ID,
                Title = data.Title,
                Introduction = data.Introduction,
                storyImage = data.StoryImage,
                Prefix = "Tasks",
                StoryLine = data.StoryLine,
                currentFileName = $"Tasks/{data.StoryLine}/{data.currentFileName}",
                Type = data.Type,
                TaskType = data.TaskType,
                deadLineTurn = data.deadLineTurn,
                characterID = data.characterID,
                regionID = data.regionID,
                cityIndex = data.cityIndex,

                NextNodes = new List<StoryNode>(),
                OptionsAsk = data.OptionsAsk,
                OptionsText = data.OptionsTexts,
                OptionsCost = data.OptionsCosts,
                OptionsAction = data.OptionsActions,
            };


            if (data.PreviousNodeID != -1)
            {
                rootNodes.Add(node);
                storyLines[data.StoryLine] = node;
            }

            allNodes.Add(node);

            if (data.PreviousNodeID != -1)
            {

                foreach (var foundNode in allNodes)
                {
                    if (foundNode.ID == data.PreviousNodeID && foundNode.StoryLine == data.StoryLine)
                    {
                        foundNode.AddNextNode(node);
                        break;
                    }
                }
            }
        }
    } 
     */


    public StoryNode FindNode(string id)
    {
        foreach (var rootNode in rootNodes)
        {
            var foundNode = rootNode.FindNode(id);
            if (foundNode != null)
            {
                return foundNode;
            }
        }
        return null; // Return null if the node with the specified ID is not found
    }

}

#endregion

#region StoryNode
public class StoryNode
{
    public int ID; // stoty nodeID
    public string NodeName; // stoty nodeID

    public string key;

    public string currentFileName;

    public Dictionary<string, string> Title = new Dictionary<string, string>();
    public Dictionary<string, string> Introduction = new Dictionary<string, string>();
    public string storyImage;
    public bool isHappend = false;
    public string Prefix;
    public string StoryLine;
    public List<StoryNode> NextNodes; 
    public int nextHappendNodeIndex = 0;


    public Dictionary<string, string> OptionsAsk = new Dictionary<string, string>();
    public List<Dictionary<string, string>> OptionsText = new List<Dictionary<string, string>>();
    public List<string> OptionsCost = new List<string>();
    public List<string> OptionsAction = new List<string>();

    public int selOption = -1;

    public StoryNode()
    {

    }


    public StoryNode(ExcelTaskStory excelTaskStory)
    {
        ID = excelTaskStory.ID;
        key = excelTaskStory.key;
        currentFileName = excelTaskStory.currentFileName;
        Title = excelTaskStory.Title;
        Introduction = excelTaskStory.Introduction;
        storyImage = excelTaskStory.StoryImage;
        Prefix = "Task";
        OptionsAsk = excelTaskStory.OptionsAsk;
        OptionsText = excelTaskStory.OptionsTexts;
        OptionsCost = excelTaskStory.OptionsCosts;
        OptionsAction = excelTaskStory.OptionsActions;

    }



    public string GetTitle()
    {
        string currentLanguage = LocalizationSettings.SelectedLocale.Identifier.Code;
        return Title.TryGetValue(currentLanguage, out var text) ? text : Title[LanguageCode.EN];
    }

    public string GetIntroduction()
    {
        string currentLanguage = LocalizationSettings.SelectedLocale.Identifier.Code;
        return Introduction.TryGetValue(currentLanguage, out var text) ? text : Introduction[LanguageCode.EN];
    }

    public string GetOptionsAsk()
    {
        string currentLanguage = LocalizationSettings.SelectedLocale.Identifier.Code;
        return OptionsAsk.TryGetValue(currentLanguage, out var text) ? text : OptionsAsk[LanguageCode.EN];
    }



    public void AddNextNode(StoryNode nextNode)
    {
        if (NextNodes == null)
        {
            NextNodes = new List<StoryNode>();
        }
        NextNodes.Add(nextNode);
    }

    public StoryNode FindNode(string id)
    {
        if (NodeName == id)
        {
            return this;
        }
        foreach (var nextNode in NextNodes)
        {
            var foundNode = nextNode.FindNode(id);
            if (foundNode != null)
            {
                return foundNode;
            }
        }
        return null;
    }

    public void SetCurrentFileName(string currentFileName)
    {
        this.currentFileName = $"{Prefix}/{StoryLine}/{currentFileName}";
    }

    public int GetOptionsNum()
    {
        return OptionsText.Count;
    }

    public string GetOptionText(int index)
    {
        string currentLanguage = LocalizationSettings.SelectedLocale.Identifier.Code;
        return OptionsText[index].TryGetValue(currentLanguage, out var text) ? text : OptionsText[index][LanguageCode.EN];
    }

}


#endregion


#region StoryControlSaveData

public class StoryControlSaveData
{
    public List<TaskSaveData> taskSaveNodeList;
    public StoryControlSaveData(StoryControl storyControl)
    {
        if (storyControl == null) return;
        taskSaveNodeList = new  List<TaskSaveData>();

        foreach (var task in storyControl.GetTotalTaskList())
        {
            TaskSaveData taskSaveData = new TaskSaveData(task);
            taskSaveNodeList.Add(taskSaveData);
        }
    }
}


#endregion