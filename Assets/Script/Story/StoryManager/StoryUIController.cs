using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Localization.Settings;
using System.Text.RegularExpressions;
using static ExcelReader;

/// <summary>
/// ??UI?????????????????
/// </summary>
public class StoryUIController : MonoBehaviour
{
    [SerializeField] private GameObject storyPanel;
    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private GameObject PanelStory;
    [SerializeField] private GameObject PanelRangeStory;
    [SerializeField] private GameObject choicePanel;
    [SerializeField] private GameObject StoryUI;

    [SerializeField] private TextMeshProUGUI speakingContent;
    [SerializeField] private GameObject panelTextPrefab;
    [SerializeField] private GameObject choicButtonsPrefab;

    [SerializeField] private StorySpeakerControl storySpeakerControl;
    [SerializeField] private TypewriterEffect typewriterEffect;

    private List<Button> choiceButtons = new List<Button>();
    private List<GameObject> currentPanelTexts = new List<GameObject>();
    private LinkedList<PanelSotryData> currentPanelTextdatas = new LinkedList<PanelSotryData>();

    [SerializeField] private StoryDataManager storyDataManager;
    [SerializeField] private HistoryManager historyManager;
    [SerializeField] private StoryTextEffect storyTextEffect;

    private bool changeReadColor = true;
    private bool isChoice = false;


    private void Start()
    {
        typewriterEffect = TypewriterEffect.Instance;
    }

    public void DisplayThisLine(ExcelPlotData data, StoryMediaController mediaController)
    {
        if (data.storyType != Constants.VOICEOVER)
        {
            UpdatePanelVisibility(data);
            mediaController.PlayMediaAndBackground(data);
            mediaController.UpdateCharacterImages(data);

            if (data.storyType == Constants.PANEL && storyDataManager.GetStoryData()[storyDataManager.GetCurrentLine() - 1].storyType == Constants.DIALOGUE)
            {
                ClearPanelText();
            }

            if (data.storyType == Constants.DIALOGUE)
                HandleDialogue(data);
            else if (data.storyType == Constants.PANEL)
                HandlePanel(data);
        }
    }

    /// <summary>
    /// ????
    /// </summary>
    private void HandleDialogue(ExcelPlotData data)
    {
        storySpeakerControl.SetSpeakerTitle(data.speaker, data.avatarImageFileName);
        speakingContent.text = GetContentText(data.contents);
        typewriterEffect.StartTyping(GetContentText(data.contents), speakingContent);
        ChangeReadTextColor(speakingContent);
        RecordHistory(storySpeakerControl.GetSpeakerName(), speakingContent.text,storySpeakerControl.GetIconPath());
    }

    /// <summary>
    /// ??????
    /// </summary>
    private void HandlePanel(ExcelPlotData data)
    {
        GameObject panelText = Instantiate(panelTextPrefab);
        var panelTextControl = panelText.GetComponent<PanelStoryTextControl>();

        RecordCurrentPanelData(data.speaker, GetContentText(data.contents));
        panelTextControl.SetTheTextToPanelStoryText(data.speaker, GetContentText(data.contents));

        LayoutRebuilder.ForceRebuildLayoutImmediate(panelText.GetComponent<RectTransform>());

        float totalHeight = 0f;
        float spacing = PanelRangeStory.GetComponent<VerticalLayoutGroup>().spacing;
        foreach (Transform child in PanelRangeStory.transform)
        {
            totalHeight += child.GetComponent<RectTransform>().rect.height + spacing;
        }
        totalHeight += panelText.GetComponent<RectTransform>().rect.height;

        float maxHeight = PanelRangeStory.GetComponent<RectTransform>().rect.height;
        if (totalHeight > maxHeight)
            ClearPanelText();

        panelText.transform.SetParent(PanelRangeStory.transform, false);
        currentPanelTexts.Add(panelText);

        VerticalLayoutGroup verticalLayoutGroup = PanelRangeStory.GetComponent<VerticalLayoutGroup>();
        if (panelTextControl.ContentText.text.Contains("<align=center>"))
        {
            verticalLayoutGroup.childAlignment = TextAnchor.MiddleCenter;
        }
        else
        {
            verticalLayoutGroup.childAlignment = TextAnchor.UpperLeft;
        }

        ChangeReadTextColor(panelTextControl.ContentText);
        typewriterEffect.StartTyping(GetContentText(data.contents), panelTextControl.ContentText);
        RecordHistory(panelTextControl.nameText.text, panelTextControl.ContentText.text, data.avatarImageFileName);
    }

    /// <summary>
    /// ???????
    /// </summary>
    private void UpdatePanelVisibility(ExcelPlotData data)
    {
        dialogueBox.SetActive(data.storyType == Constants.DIALOGUE);
        PanelStory.SetActive(data.storyType == Constants.PANEL);
    }

    /// <summary>
    /// ????????
    /// </summary>
    private void ChangeReadTextColor(TextMeshProUGUI textMeshProUGUI)
    {
        if (!changeReadColor) return;

        if (storyDataManager.GetMaxReadLine()[storyDataManager.GetCurrentSheetIndex()] > storyDataManager.GetCurrentLine())
        {
            textMeshProUGUI.color = Color.gray;
        }
        else
        {
            textMeshProUGUI.color = Color.black;
        }
    }

    /// <summary>
    /// ??????????
    /// </summary>
    private void ChangeChoiceTextColor(TextMeshProUGUI textMeshProUGUI, int optionIndex)
    {
        if (!changeReadColor) return;
        if (storyDataManager.GetCurrentSheetIndex() + optionIndex >= storyDataManager.GetMaxReadLine().Count) return;

        if (storyDataManager.GetMaxReadLine()[storyDataManager.GetCurrentSheetIndex() + optionIndex] != 0)
        {
            textMeshProUGUI.color = Color.gray;
        }
    }

    /// <summary>
    /// ????
    /// </summary>
    public void ShowChoices(int choicesNum, ExcelPlotData data, StoryMediaController mediaController)
    {
        isChoice = true;
        int currentLine = storyDataManager.GetCurrentLine();
        storyDataManager.UpdateMaxReadLine();

        switch (data.storyType)
        {
            case Constants.DIALOGUE:HandleDialogueChoice(data);break;
            case Constants.PANEL:HandlePanelChoice(data);break;
            default:Debug.LogWarning($"Unkonw storyType: {data.storyType}");break;
        }

        storyDataManager.UpdateMaxReadLine();
        StartCoroutine(WaitAndShowChoices(currentLine, choicesNum));
    }

    private void HandleDialogueChoice(ExcelPlotData data)
    {
        storySpeakerControl.SetSpeakerTitle(data.speaker, data.avatarImageFileName);
        ChangeReadTextColor(speakingContent);

        string content = GetContentText(data.contents);
        typewriterEffect.StartTyping(content, speakingContent);
        RectTransform rt = choicePanel.GetComponent<RectTransform>();
        Vector2 anchoredPos = rt.anchoredPosition;
        anchoredPos.y = 200f;
        rt.anchoredPosition = anchoredPos;
    }

    private void HandlePanelChoice(ExcelPlotData data)
    {
        ClearPanelText();
        GameObject panelText = Instantiate(panelTextPrefab);
        var panelTextControl = panelText.GetComponent<PanelStoryTextControl>();

        string content = GetContentText(data.contents);
        string cleaned = content;

        RecordCurrentPanelData(null, cleaned);
        panelTextControl.SetTheTextToPanelStoryText(null, cleaned);

        panelText.transform.SetParent(PanelRangeStory.transform, false);
        LayoutRebuilder.ForceRebuildLayoutImmediate(panelText.GetComponent<RectTransform>());
        currentPanelTexts.Add(panelText);

        ChangeReadTextColor(panelTextControl.ContentText);
        typewriterEffect.StartTyping(content, panelTextControl.ContentText);
        RectTransform rt = choicePanel.GetComponent<RectTransform>();
        Vector2 anchoredPos = rt.anchoredPosition;
        anchoredPos.y = 100f;
        rt.anchoredPosition = anchoredPos;
    }

    private IEnumerator WaitAndShowChoices(int startingLine, int choicesNum)
    {
        while (typewriterEffect.IsTyping())
        {
            yield return null;
        }

        ClearChoiceButtons();
        var data = storyDataManager.GetStoryData()[startingLine];

        for (int i = 0; i < choicesNum; i++)
        {
            GameObject newButtonObj = Instantiate(choicButtonsPrefab);
            newButtonObj.transform.SetParent(choicePanel.transform, false);
            choiceButtons.Add(newButtonObj.GetComponent<Button>());

            var textMeshPro = choiceButtons[i].GetComponentInChildren<TextMeshProUGUI>();
            ChangeChoiceTextColor(textMeshPro, i + 1);

            int nextLineIndex = startingLine;
            var nextData = storyDataManager.GetStoryData()[nextLineIndex];
            textMeshPro.text = GetContentText(nextData.contents);

            string capturedOption = nextData.speaker;
            string capturedContent = GetContentText(nextData.contents);
            int buttonIndex = i;

            if (StoryDataManager.NotNullNorEmpty(nextData.effect))
            {
                choiceButtons[buttonIndex].onClick.AddListener(() =>
                {
                    storyTextEffect.DoEffect(nextData.effect);

                });

            }

            choiceButtons[buttonIndex].onClick.AddListener(() =>
            {
                OnChoiceSelected(capturedOption, capturedContent);
            });

            startingLine++;
        }

        choicePanel.SetActive(true);

        int afterChoiceLine = startingLine;
        TotalStoryManager.Instance.SetCurrentLine(afterChoiceLine);
    }

    /// <summary>
    /// ?????
    /// </summary>
    public void OnChoiceSelected(string Option, string capturedContent)
    {
        isChoice = false;
        choicePanel.SetActive(false);
        TotalStoryManager.Instance.OnChoiceSelected(Option, capturedContent);
    }

    private string GetChoiceText(string capturedContent)
    {
        string choiceText = string.Empty;
        for (int i = 0; i < choiceButtons.Count; i++)
        {
            string buttonText = choiceButtons[i].GetComponentInChildren<TMP_Text>().text;

            if (buttonText == capturedContent)
                buttonText = "<color=#ff0000ff>" + buttonText + "</color>";

            if (buttonText != null)
            {
                choiceText += buttonText + " \n";
            }
        }
        return choiceText;
    }

    private void ClearChoiceButtons()
    {
        foreach (var buttons in choiceButtons)
        {
            Destroy(buttons.gameObject);
        }
        choiceButtons.Clear();
    }

    /// <summary>
    /// ??????
    /// </summary>
    public void ClearPanelText()
    {
        foreach (Transform child in PanelRangeStory.transform)
        {
            Destroy(child.gameObject);
        }
        currentPanelTexts.Clear();
        currentPanelTextdatas.Clear();
    }

    /// <summary>
    /// ??UI
    /// </summary>
    public void OpenUI()
    {
        StoryUI.SetActive(true);
        //dialogueBox.SetActive(data.storyType == Constants.DIALOGUE);
        //PanelStory.SetActive(data.storyType == Constants.PANEL);

        //if (isChoice)
        //{
        //    choicePanel.SetActive(true);
        //}
    }
    public void CloseUI()
    {
        StoryUI.SetActive(false);

        //dialogueBox.SetActive(false);
        //PanelStory.SetActive(false);

        //if (isChoice)
        //{
        //    choicePanel.SetActive(false);
        //}

        Debug.Log("is CloseUI");
        
    }

    public bool IsShowText()
    {
        return StoryUI.activeSelf;
    }


    private void RecordHistory(string speaker, string content, string iconPath)
    {
        Debug.Log($"RecordHistory(string {speaker}, string {content}, string {iconPath})");
        HistoryData data = new HistoryData(speaker, content, iconPath);
        historyManager.RecordHistory(data);
    }

    private void RecordCurrentPanelData(string speaker, string content)
    {
        PanelSotryData data = new PanelSotryData(speaker, content);
        currentPanelTextdatas.AddLast(data);
    }

    /// <summary>
    /// ????????
    /// </summary>

    /// <summary>
    /// ???????????
    /// </summary>
    public static string GetContentText(Dictionary<string, string> contents)
    {
        string currentLanguage = LocalizationSettings.SelectedLocale.Identifier.Code;
        return contents.TryGetValue(currentLanguage, out var text) ? text : contents["en"];
    }


    // ==================== Getters ====================

    public List<Button> GetChoiceButtons() => choiceButtons;
    public List<GameObject> GetCurrentPanelTexts() => currentPanelTexts;
    public LinkedList<PanelSotryData> GetCurrentPanelTextdatas() => currentPanelTextdatas;
    public bool GetIsChoice() => isChoice;
    public TextMeshProUGUI GetSpeakingContent() => speakingContent;
    public GameObject GetStoryPanel() => storyPanel;

    // ==================== Setters ====================

    public void SetChangeReadColor(bool value) => changeReadColor = value;
    public void SetIsChoice(bool value) => isChoice = value;
    public void SetCurrentPanelTextdatas(LinkedList<PanelSotryData> datas) => currentPanelTextdatas = new LinkedList<PanelSotryData>(datas);
}