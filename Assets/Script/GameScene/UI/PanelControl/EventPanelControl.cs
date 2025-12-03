using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using System.Linq;
using static EnumHelper;
public class EventPanelControl : MonoBehaviour
{
    public TextMeshProUGUI Title;
    public TextMeshProUGUI Introduction;
    public TextMeshProUGUI ChooseText;

    public Image EventImage;
    public Button CloseButton;
    public TypewriterEffect typewriterEffect;

    public GameObject OptionsPrefab;
    public GameObject OptionsPanel;
    public List<GameObject> Options;

    public ColumnControl columnControl;
    public CharacterAssistRowControl characterAssistRowControl;

    private List<OptionCost> optionCosts;
    public GameValue gameValue;
    private EventsRowPrefab eventsRowPrefab;

    public class OptionCost
    {
        public Dictionary<string, int> costData = new Dictionary<string, int>();
    }

    private StoryNode eventNode;

    void Start()
    {
        if (gameValue == null) gameValue = FindObjectOfType<GameValue>();
        CloseButton.onClick.AddListener(OnCloseButtonClick);
    }

    void OnCloseButtonClick()
    {
        CloseEventPanel();
    }

    public void ShowEventPanel(StoryNode eventData,EventsRowPrefab eventsRowPrefab)
    {
        optionCosts = new List<OptionCost>();
        eventNode = eventData;

        Title.text = eventData.GetTitle();
        Introduction.text = eventData.GetIntroduction();
        ChooseText.text = eventData.GetOptionsAsk();

        string path = eventData.storyImage;
        Sprite sprite = Resources.Load<Sprite>(path);
        EventImage.sprite = sprite;

        this.eventsRowPrefab = eventsRowPrefab;

        gameObject.SetActive(true);
        UpButtonText();
    }



    public string ParseOptionText(string costText, out OptionCost optionCost)
    {
        optionCost = new OptionCost();

        if (string.IsNullOrEmpty(costText)) return "";

        string[] parts = costText.Split(',');
        List<string> pureTextParts = new();

        foreach (var part in parts)
        {
            string[] keyValue = part.Trim().Split(':');

            // 如果是 key:value 格式，就解析成资源消耗
            if (keyValue.Length == 2 && int.TryParse(keyValue[1].Trim(), out int amount))
            {
                string key = keyValue[0].Trim();
                optionCost.costData[key] = amount;
            }
            else
            {
                // 否则，这段就当作按钮正文的一部分
                pureTextParts.Add(part.Trim());
            }
        }

        return string.Join(" ", pureTextParts);
    }




    void UpButtonText()
    {
        // 清空旧选项
        foreach (Transform child in OptionsPanel.transform)
        {
            Destroy(child.gameObject);
        }
        Options.Clear();
        optionCosts.Clear();

        // 生成新选项按钮
        for (int i = 0; i < eventNode.GetOptionsNum(); i++)
        {
            GameObject optionGO = Instantiate(OptionsPrefab, OptionsPanel.transform);
            TextMeshProUGUI btnText = optionGO.GetComponentInChildren<TextMeshProUGUI>();


            if (eventNode.selOption != -1) optionGO.GetComponent<ButtonEffect>().SetButtonUnInteractable(i == eventNode.selOption);


            // 从文本解析出展示文本 + 资源消耗
            string ContentText = eventNode.GetOptionText(i);
            OptionCost parsedCost;
            if (eventNode == null)
            {
                Debug.LogError("eventNode is null!");
                return;
            }
            if (eventNode.OptionsCost == null)
            {
                Debug.LogError("eventNode.OptionsCost is null!");
                return;
            }
            if (i >= eventNode.OptionsCost.Count)
            {
                Debug.LogError($"Index {i} is out of range. OptionsCost.Count = {eventNode.OptionsCost.Count}");
                return;
            }
            if (eventNode.OptionsCost[i] == null)
            {
                Debug.LogError($"eventNode.OptionsCost[{i}] is null");
                return;
            }

            string costText = ParseOptionText(eventNode.OptionsCost[i], out parsedCost);
            optionCosts.Add(parsedCost);

            // 设置按钮文本
            btnText.text = ContentText + "    " + CostDisplayText(parsedCost,btnText);

            // 添加点击事件
            int index = i;
            optionGO.GetComponent<Button>().onClick.AddListener(() => OnOptionClick(index));
            Options.Add(optionGO);
        }

        // 移除监听防止重复调用
        CharacterAssistRowControl.afterClose -= UpButtonText;
    }

    string CostDisplayText(OptionCost cost, TMP_Text textComponent)
    {
        if (cost == null || cost.costData == null || cost.costData.Count == 0)
            return string.Empty;

        // 1️⃣ 加载并绑定 TMP_SpriteAsset
        TMP_SpriteAsset spriteAsset = GetSprite.LoadValueSpriteAsset();
        if (spriteAsset == null)
        {
            Debug.LogError("CostDisplayText: TMP Sprite Asset not found!");
            return string.Empty;
        }

        if (textComponent != null)
            textComponent.spriteAsset = spriteAsset; // 绑定 TMP_Text

        // 2️⃣ 按顺序生成显示
        List<string> parts = new List<string>();
        string[] displayOrder = new string[] { "Scout", "Build", "Negotiation" };

        foreach (var key in displayOrder)
        {
            if (cost.costData.TryGetValue(key, out int value))
            {
                // 这里使用 spriteAsset.name 生成 <sprite> 标签
                string spriteTag = $"<sprite name=\"{key}\">";
                string valueText = GetColoredCostText(key, value);
                parts.Add($"{spriteTag} {valueText}");
            }
        }

        return string.Join(" ", parts);
    }

    string GetColoredCostText(string typeName, int costValue)
    {
        double currentValue = GetCurrentResourceValue(typeName);
        if (currentValue < costValue)
        {
            return $"<color=#ff0000ff>{costValue}</color>";
        }
        else
        {
            return costValue.ToString();
        }
    }

    double GetCurrentResourceValue(string typeName)
    {
        if (gameValue == null) gameValue = FindObjectOfType<GameValue>();
        switch (typeName)
        {
            case "Scout": return gameValue.GetResourceValue().Scout;
            case "Build": return gameValue.GetResourceValue().Build;
            case "Negotiation": return gameValue.GetResourceValue().Negotiation;
            default: return 0;
        }
    }

    void OnOptionClick(int index)
    {
        if (index < eventNode.OptionsAction.Count)
        {
            string action = eventNode.OptionsAction[index];

            if (CheckAndConsumeCost(index))
            {
                HandleAction(action);
                eventsRowPrefab.SetClear(index);
            }
        }
    }

    bool CheckAndConsumeCost(int index)
    {
        var cost = optionCosts[index];
        foreach (var kvp in cost.costData)
        {
            double currentValue = GetCurrentResourceValue(kvp.Key);
            if (currentValue < kvp.Value)
            {
                CharacterAssistRowControl.afterClose += UpButtonText;
                characterAssistRowControl.ToggleRow(ParseEnumOrDefault<SBNType>(kvp.Key), kvp.Value);
                return false;
            }
        }

        foreach (var kvp in cost.costData)
        {
            ConsumeResource(kvp.Key, kvp.Value);
        }

        return true;
    }

    int GetRowIndex(string typeName)
    {
        switch (typeName)
        {
            case "scout": return 1;
            case "build": return 2;
            case "negotiation": return 3;
            default: return 0;
        }
    }

    void ConsumeResource(string typeName, int amount)
    {
        if (gameValue == null) gameValue = FindObjectOfType<GameValue>();

        switch (typeName)
        {
            case "scout": gameValue.GetResourceValue().Scout -= amount; break;
            case "build": gameValue.GetResourceValue().Build -= amount; break;
            case "negotiation": gameValue.GetResourceValue().Negotiation -= amount; break;
        }
    }

    public void FristOptionAction(StoryNode eventNode)
    {
        // maybe need to more change
        this.eventNode = eventNode;
        OnOptionClick(0);
    }


    void HandleAction(string fullCode)
    {
        Debug.Log(fullCode);

        // 先尝试匹配 if / else if / else 分支结构
        var branches = Regex.Matches(fullCode,
            @"(?:(if|else\s*if|else)\s*(?:\(\s*(?:condition\((.*?)\)|(.*?))\s*\))?)\s*\{\s*(.*?)\s*\}",
            RegexOptions.Singleline);

        bool matchedAndExecuted = false;

        foreach (Match branch in branches)
        {
            string type = branch.Groups[1].Value.Trim();       // if / else if / else
            string condition = branch.Groups[2].Success && !string.IsNullOrEmpty(branch.Groups[2].Value)
                ? branch.Groups[2].Value.Trim()    // condition(...)
                : branch.Groups[3].Value.Trim();   // 普通表达式

            string statement = branch.Groups[4].Value.Trim();  // 花括号中的语句

            bool conditionResult = false;

            if (type == "if" || type == "else if")
            {
                conditionResult = EvaluateCondition(condition);
            }
            else
            {
                conditionResult = true; // else 分支默认执行
            }

            if (conditionResult)
            {
                ExecuteAction(statement);
                matchedAndExecuted = true;
                break; // 满足一个条件后就退出分支处理
            }
        }

        // 如果没有分支结构被执行，尝试直接执行整句（比如 goToFile(...)）
        if (!matchedAndExecuted && !string.IsNullOrWhiteSpace(fullCode))
        {
            string trimmed = fullCode.Trim();

            // 排除只有花括号包裹的情况（如 "{ ... }"）
            if (!trimmed.StartsWith("if") && !trimmed.StartsWith("else") && !trimmed.StartsWith("{"))
            {
                ExecuteAction(trimmed);
            }
        }
    }




    bool EvaluateCondition(string condition)
    {
        if (string.IsNullOrWhiteSpace(condition))
            return false;

        condition = condition.Trim();

        // 支持布尔常量
        if (condition == "true") return true;
        if (condition == "false") return false;

        // 括号嵌套处理（仅一层）
        if (condition.Contains("("))
        {
            int openIndex = condition.IndexOf('(');
            int closeIndex = condition.LastIndexOf(')');
            if (openIndex >= 0 && closeIndex > openIndex)
            {
                string inner = condition.Substring(openIndex + 1, closeIndex - openIndex - 1);
                bool innerResult = EvaluateCondition(inner);
                condition = condition.Substring(0, openIndex) + innerResult.ToString().ToLower() + condition.Substring(closeIndex + 1);
            }
        }

        // 支持逻辑或
        if (condition.Contains("||"))
        {
            var parts = condition.Split(new[] { "||" }, StringSplitOptions.RemoveEmptyEntries);
            return parts.Any(part => EvaluateCondition(part.Trim()));
        }

        // 支持逻辑与
        if (condition.Contains("&&"))
        {
            var parts = condition.Split(new[] { "&&" }, StringSplitOptions.RemoveEmptyEntries);
            return parts.All(part => EvaluateCondition(part.Trim()));
        }

        // 基本比较
        return EvaluateSimpleComparison(condition);
    }


    bool EvaluateSimpleComparison(string expr)
    {
        string pattern = @"(\w+)\.(\w+)\s*(==|!=|<=|>=|<|>)\s*(-?\d+)";
        Match match = Regex.Match(expr, pattern);
        if (!match.Success)
        {
            Debug.LogWarning("无法解析条件表达式: " + expr);
            return false;
        }

        string character = match.Groups[1].Value;
        string attribute = match.Groups[2].Value;
        string op = match.Groups[3].Value;
        int value = int.Parse(match.Groups[4].Value);

        int actual = GetAttributeValue(character, attribute);
        return Compare(actual, op, value);
    }

    int GetAttributeValue(string characterName, string attribute)
    {
        Character character = gameValue.GetCharacterByName(characterName);
        if (character == null)
        {
            Debug.LogWarning($"Cant find character: {characterName}");
            return -1;
        }

        if (attribute == "favorabilityLevel") return (int)character.FavorabilityLevel;
        if (attribute == "favorability") return character.Favorability;

        Debug.LogWarning($"Unknow attribute: {attribute}");
        return -1;
    }

    bool Compare(int a, string op, int b)
    {
        return op switch
        {
            "==" => a == b,
            "!=" => a != b,
            "<" => a < b,
            "<=" => a <= b,
            ">" => a > b,
            ">=" => a >= b,
            _ => false
        };
    }




    void ExecuteAction(string actionString)
    {
        Match match = Regex.Match(actionString, @"(\w+)\((.*?)\)");
        if (match.Success)
        {
            string command = match.Groups[1].Value;
            string argument = match.Groups[2].Value;

            switch (command)
            {
                case "goToFile":
                    GoToFile(argument);
                    break;
                case "closePanel":
                    CloseEventPanel();
                    break;
            }
        }
        else
        {
            Debug.LogWarning("Unknown action: " + actionString);
        }
    }


    void GoToFile(string fileName)
    {
        CloseEventPanel();
        eventNode.SetCurrentFileName(fileName);
        StoryRemindPanelManager.Instance.StoryHappenNode(eventNode,fileName);
        Debug.Log("GoTofileDONE");
    }



  /*  void GoToFile(string fileName)
    {
        CloseEventPanel();
        StoryNode nextNode = columnControl.eventGraph.FindNode(fileName);
        storyRemindPanelControl.StoryHappendNode(nextNode);
    }*/

    void CloseEventPanel()
    {
        gameObject.SetActive(false);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
            CloseEventPanel();
    }
}


