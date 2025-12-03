using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static EnumHelper;
using static PanelExtensions;

public enum SBNType
{
    None,Scout,Build, Negotiation
}

public class TaskPanelControl : PanelBase
{
    public TextMeshProUGUI Title;
    public TextMeshProUGUI Introduction;
    public TextMeshProUGUI ChooseText;

    public TextMeshProUGUI TaskTypeText;


    public Image TaskImage;
    public Button CloseButton;

    public TypewriterEffect typewriterEffect; // maybe don't need

    public GameObject OptionsPrefab;
    public GameObject OptionsPanel;
    public List<GameObject> Options;

    private CharacterAssistRowControl characterAssistRowControl;


    public GameValue gameValue;


    private TasksRowPrefab tasksRowPrefab;
    private TaskData taskData;

    private List<OptionCost> optionCosts;


    public class OptionCost
    {
        public Dictionary<string, int> costData = new Dictionary<string, int>();
    }



    void Start()
    {
        CloseButton.onClick.AddListener(OnCloseButtonClick);
        gameValue = GameValue.Instance;

    }

    void OnCloseButtonClick()
    {
        ClosePanel();
    }

    public void ShowTaskPanel(TaskData taskData, CharacterAssistRowControl characterAssistRowControl, TasksRowPrefab tasksRowPrefab)
    {
        BottomButton.Instance.otherPanels.Add(this.gameObject);
        this.characterAssistRowControl = characterAssistRowControl;
        this.taskData = taskData;
        this.tasksRowPrefab = tasksRowPrefab;
        OpenPanel();
    }

    public override void OpenPanel()
    {
        Title.text = taskData.GetTitle();
        Introduction.text = taskData.GetTitle();
        ChooseText.text = taskData.GetOptionsAsk();
        optionCosts = new List<OptionCost>();
        TaskImage.sprite = taskData.GetStoryImage();
        gameObject.SetActive(true);
        UpButtonText();
        SetTaskType();
    }



void SetTaskType()
    {
        switch (taskData.GetTaskType())
        {
            case TaskType.Story: StoryTypeSet(); break;
            case TaskType.Battle: BattleTypeSet(); break;

        }
    }

    void StoryTypeSet()
    {
        TaskTypeText.text = "STORY";
        TaskTypeText.color = new Color32(255, 255, 255, 255);
    }

    void BattleTypeSet()
    {
        TaskTypeText.text = "BATTLE";
        TaskTypeText.color = new Color32(255, 0, 0, 255);
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
            if (keyValue.Length == 2 && int.TryParse(keyValue[1].Trim(), out int amount))
            {
                string key = keyValue[0].Trim();
                optionCost.costData[key] = amount;
            }
            else
            {
                pureTextParts.Add(part.Trim());
            }
        }

        return string.Join(" ", pureTextParts);
    }




    void UpButtonText()
    {
        foreach (Transform child in OptionsPanel.transform)
        {
            Destroy(child.gameObject);
        }
        Options.Clear();
        optionCosts.Clear();

        for (int i = 0; i < taskData.GetOptionsNum(); i++)
        {
            GameObject optionGO = Instantiate(OptionsPrefab, OptionsPanel.transform);
            TextMeshProUGUI btnText = optionGO.GetComponentInChildren<TextMeshProUGUI>();


            if (taskData.GetSelOption() != -1) optionGO.GetComponent<ButtonEffect>().SetButtonUnInteractable(i == taskData.GetSelOption());
            string ContentText = taskData.GetOptionText(i);
            OptionCost parsedCost;
            string costText = ParseOptionText(taskData.GetOptionCostString(i), out parsedCost);
            optionCosts.Add(parsedCost);
            btnText.text = ContentText + "    " + CostDisplayText(parsedCost, btnText);
            int index = i;
            optionGO.GetComponent<Button>().onClick.AddListener(() => OnOptionClick(index));
            Options.Add(optionGO);
        }
        CharacterAssistRowControl.afterClose -= UpButtonText;
    }

    public string CostDisplayText(OptionCost cost, TMP_Text textComponent)
    {
        if (cost == null || cost.costData == null || cost.costData.Count == 0)
            return string.Empty;

        TMP_SpriteAsset spriteAsset = GetSprite.LoadValueSpriteAsset();
        if (spriteAsset == null)
        {
            Debug.LogError("CostDisplayText: TMP Sprite Asset not found!");
            return string.Empty;
        }

        textComponent.spriteAsset = spriteAsset;

        List<string> parts = new List<string>();

        foreach (var kvp in cost.costData)
        {
            string spriteTag = $"<sprite name=\"{kvp.Key}\">";
            string valueText = GetColoredCostText(ParseEnumOrDefault<SBNType>(kvp.Key), kvp.Value);
            parts.Add($"{spriteTag} {valueText}");
        }

        return string.Join(" ", parts);
    }







    string GetColoredCostText(SBNType type, int costValue)
    {
        double currentValue = GetCurrentResourceValue(type);
        if (currentValue < costValue)
        {
            return $"<color=#ff0000ff>{costValue}</color>";
        }
        else
        {
            return costValue.ToString();
        }
    }


    void OnOptionClick(int index)
    {
        if (index < taskData.GetOptionsActionCount())
        {
            string action = taskData.GetOptionsActionString(index);
            if (CheckAndConsumeCost(index))
            {
                HandleAction(action);
            }
        }

        tasksRowPrefab.SetClear(index);
    }

    bool CheckAndConsumeCost(int index)
    {
        var cost = optionCosts[index];
        foreach (var kvp in cost.costData)
        {
            double currentValue = GetCurrentResourceValue(ParseEnumOrDefault<SBNType>(kvp.Key));
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

    double GetCurrentResourceValue(SBNType type)
    {
        if (gameValue == null) gameValue = FindObjectOfType<GameValue>();
        switch (type)
        {
            case SBNType.Scout: return gameValue.GetResourceValue().Scout;
            case SBNType.Build: return gameValue.GetResourceValue().Build;
            case SBNType.Negotiation: return gameValue.GetResourceValue().Negotiation;
            default: return 0;
        }
    }


    void ConsumeResource(string typeName, int amount)
    {
        if (gameValue == null) gameValue = FindObjectOfType<GameValue>();

        switch (typeName)
        {
            case "Scout": gameValue.GetResourceValue().Scout -= amount; break;
            case "Build": gameValue.GetResourceValue().Build -= amount; break;
            case "Negotiation": gameValue.GetResourceValue().Negotiation -= amount; break;
        }
    }



    public void FristOptionAction(TaskData taskNode)
    {
        this.taskData = taskNode;
        OnOptionClick(0);
    }


    void HandleAction(string actionString)
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
                    ClosePanel();
                    break;

            }
        }
        else
        {
            Debug.LogWarning("Dont have" + actionString);
        }
    }

    void GoToFile(string fileName)
    {
        ClosePanel();
        StoryRemindPanelManager.Instance.StoryHappenNode(taskData.GetStoryNode(), fileName);

    }

    public override PanelSaveData GetPanelSaveData()
    {
        PanelSaveData panelSaveData = new PanelSaveData();
        panelSaveData = GetSaveData(this,gameObject, PanelType.Task);
        panelSaveData.customData.Add(CustomDataType.Task, taskData.GetTaskKey());
        return panelSaveData;
    }


    public override void ClosePanel()
    {
        BottomButton.Instance.otherPanels.Remove(this.gameObject);
        Destroy(gameObject);
    }


    public override void SetPanelSaveData(PanelSaveData panelSaveData)
    {
        SetSaveData(this,gameObject, panelSaveData);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1)) ClosePanel();
    }
}
