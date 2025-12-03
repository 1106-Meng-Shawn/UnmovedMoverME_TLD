using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class TypewriterEffect : MonoBehaviour
{
    public static TypewriterEffect Instance;

    public float typingSpeed = Constants.DEFAULT_TYPING_SPEED;
    public float waitTime = Constants.DEFAULT_WAITING_SECONDS;

    private Coroutine typingCoroutine;
    private bool isTyping;
    private TextMeshProUGUI textDisplayRef;
    private string currentFullText;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void SetTypingSpeedAndWaitTime(float type, float wait)
    {
        typingSpeed = type;
        waitTime = wait;
    }

    public void StartTyping(string text, TextMeshProUGUI refText)
    {
        if (isTyping && typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        if (!string.IsNullOrEmpty(text) && refText != null)
        {
            currentFullText = text;
            typingCoroutine = StartCoroutine(TypeLine(text, refText));
        }
    }

    private IEnumerator TypeLine(string text, TextMeshProUGUI refText)
    {
        isTyping = true;
        textDisplayRef = refText;
        refText.text = string.Empty;

        int i = 0;
        while (i < text.Length)
        {
            i = ProcessNextSegment(text, refText, i, out bool shouldWait);

            if (shouldWait)
                yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }

    /// <summary>
    /// ?????????????????/??/??
    /// </summary>
    private int ProcessNextSegment(string text, TextMeshProUGUI refText, int startIndex, out bool shouldWait)
    {
        shouldWait = false;

        if (startIndex >= text.Length)
            return startIndex;

        char c = text[startIndex];

        switch (c)
        {
            case '{':
                return HandleCommandBlock(text, startIndex);

            case '<':
                return HandleRichTextTag(text, refText, startIndex);

            default:
                refText.text += c;
                shouldWait = true;
                return startIndex + 1;
        }
    }

    private int HandleCommandBlock(string text, int startIndex)
    {
        int endIndex = text.IndexOf('}', startIndex + 1);
        if (endIndex != -1)
        {
            string command = text.Substring(startIndex + 1, endIndex - startIndex - 1);
            ExecuteInlineCommand(command.Trim());
            return endIndex + 1;
        }
        return startIndex + 1;
    }

    private int HandleRichTextTag(string text, TextMeshProUGUI refText, int startIndex)
    {
        int tagEnd = text.IndexOf('>', startIndex);
        if (tagEnd != -1)
        {
            string tag = text.Substring(startIndex, tagEnd - startIndex + 1);
            refText.text += tag;
            return tagEnd + 1;
        }
        return startIndex + 1;
    }

    /// <summary>
    /// ?????? - ???? UpdateCharacterImage
    /// ?????
    /// - {UpdateCharacterImage(Emilia_Stand_Sad)}
    /// - {UpdateCharacterImage(Emilia_Stand_Sad, moveTo(0,N))}
    /// - {UpdateCharacterImage(Emilia, AppearAt_Left)}
    /// </summary>
    private void ExecuteInlineCommand(string command)
    {
        try
        {
            if (command.StartsWith("UpdateCharacterImage("))
            {
                string parameters = ExtractParameter(command);

                // ???????????????
                var parts = SplitParameters(parameters);

                string param1 = parts.Count > 0 ? parts[0].Trim() : null;
                string param2 = parts.Count > 1 ? parts[1].Trim() : null;

                if (!string.IsNullOrEmpty(param1))
                {
                    TotalStoryManager.Instance.MediaController.UpdateCharacterImageSmart(param1, param2);

                    if (string.IsNullOrEmpty(param2))
                        Debug.Log($"?? Executed: UpdateCharacterImage({param1})");
                    else
                        Debug.Log($"???? Executed: UpdateCharacterImage({param1}, {param2})");
                }
            }
            else
            {
                Debug.LogWarning($"?? Unknown command: {command}");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"? Command execution failed: {command}, error: {ex.Message}");
        }
    }

    /// <summary>
    /// ?????? - ????????
    /// ??: "Emilia_Stand_Sad, moveTo(0,N)" -> ["Emilia_Stand_Sad", "moveTo(0,N)"]
    /// </summary>
    private List<string> SplitParameters(string parameters)
    {
        List<string> result = new List<string>();
        StringBuilder current = new StringBuilder();
        int bracketDepth = 0;

        for (int i = 0; i < parameters.Length; i++)
        {
            char c = parameters[i];

            if (c == '(')
            {
                bracketDepth++;
                current.Append(c);
            }
            else if (c == ')')
            {
                bracketDepth--;
                current.Append(c);
            }
            else if (c == ',' && bracketDepth == 0)
            {
                // ???????????????
                result.Add(current.ToString().Trim());
                current.Clear();
            }
            else
            {
                current.Append(c);
            }
        }

        // ????????
        if (current.Length > 0)
        {
            result.Add(current.ToString().Trim());
        }

        return result;
    }

    private string ExtractParameter(string cmd)
    {
        int start = cmd.IndexOf('(');
        int end = cmd.LastIndexOf(')');
        if (start != -1 && end != -1 && end > start)
            return cmd.Substring(start + 1, end - start - 1).Trim();
        return string.Empty;
    }

    /// <summary>
    /// ?????????????????????????
    /// </summary>
    public void CompleteLine()
    {
        if (isTyping && typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        isTyping = false;

        if (textDisplayRef != null && !string.IsNullOrEmpty(currentFullText))
        {
            textDisplayRef.text = string.Empty;
            int i = 0;
            while (i < currentFullText.Length)
            {
                i = ProcessNextSegment(currentFullText, textDisplayRef, i, out _);
            }
        }
    }

    public bool IsTyping() => isTyping;
    public void SetIsTyping(bool isType) => isTyping = isType;
}
