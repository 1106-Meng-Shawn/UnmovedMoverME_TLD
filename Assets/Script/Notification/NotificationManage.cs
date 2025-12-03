using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class NotificationManage : MonoBehaviour
{
    public static NotificationManage Instance { get; private set; }

    [Header("Notification Prefab")]
    public GameObject feedbackPrefab;

    [Header("UI Parent (e.g. Canvas Panel)")]
    public Transform feedbackParent;

    [Header("Display Interval")]
    public float messageInterval = 0.25f; // ?????????

    private Queue<string> messageQueue = new Queue<string>();
    private bool isShowing = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    // ======================
    // ?? ????
    // ======================
    public void ShowAtTopByKey(NotificationKeyConstants.NotificationKey key, params object[] args)
    {
        if (args.Length != key.ParamCount)
        {
            Debug.LogWarning($"Key '{key.Key}' expects {key.ParamCount} parameters, but got {args.Length}.");
        }

        string formatted = GetLocalizedFormattedText(key.Key, args);
        EnqueueMessage(() => ShowAtTopInternal(formatted), formatted);
    }

    public void ShowToTopByKey(NotificationKeyConstants.NotificationKey key, params object[] args)
    {
        if (args.Length != key.ParamCount)
        {
            Debug.LogWarning($"Key '{key.Key}' expects {key.ParamCount} parameters, but got {args.Length}.");
        }

        string formatted = GetLocalizedFormattedText(key.Key, args);
        EnqueueMessage(() => ShowToTopInternal(formatted), formatted);
    }

    // ======================
    // ?? ??????
    // ======================
    private void ShowAtTopInternal(string content)
    {
        GameObject feedbackObject = Instantiate(feedbackPrefab, feedbackParent ?? transform);
        MessageFeedback feedback = feedbackObject.GetComponent<MessageFeedback>();

        if (feedback != null)
            feedback.ShowMessageAtTop(content);
        else
            Debug.LogWarning("Feedback prefab is missing MessageFeedback component!");
    }

    private void ShowToTopInternal(string content)
    {
        GameObject feedbackObject = Instantiate(feedbackPrefab, feedbackParent ?? transform);
        MessageFeedback feedback = feedbackObject.GetComponent<MessageFeedback>();

        if (feedback != null)
        {
            feedback.rectTransform = feedbackObject.GetComponent<RectTransform>();
            feedback.ShowMessageToTop(content);
        }
        else
            Debug.LogWarning("Feedback prefab is missing MessageFeedback component!");
    }

    // ======================
    // ?? ??????
    // ======================
    private string isShowingLastMessage = null;

    private void EnqueueMessage(System.Action action, string content)
    {
        // ??????????????????????
        if (isShowingLastMessage == content || messageQueue.Contains(content))
            return;

        messageQueue.Enqueue(content);
        StartCoroutine(ProcessQueue(action, content));
    }

    private IEnumerator ProcessQueue(System.Action action, string content)
    {
        while (isShowing)
            yield return null;

        isShowing = true;
        isShowingLastMessage = content;

        action?.Invoke();

        yield return new WaitForSeconds(messageInterval);

        isShowing = false;
        isShowingLastMessage = null;

        // ??????????
        if (messageQueue.Count > 0)
            messageQueue.Dequeue();
    }

    // ======================
    // ?? ? Unity Localization ????
    // ======================
    public static string GetLocalizedFormattedText(string key, params object[] args)
    {
        string localeCode = LocalizationSettings.SelectedLocale.Identifier.Code;
        string localized = LocalizationSettings.StringDatabase.GetLocalizedString("Notifications", key);

        if (string.IsNullOrEmpty(localized))
        {
            Debug.LogWarning($"?? Localized text for '{key}' is empty!");
            return key;
        }

        int argIndex = 0;
        int searchStart = 0;

        // ????????????
        while (searchStart < localized.Length)
        {
            // ????????
            Match match = Regex.Match(localized.Substring(searchStart), @"\{(\w*)\}");

            if (!match.Success)
                break; // ???????

            int actualIndex = searchStart + match.Index;
            string placeholder = match.Groups[1].Value; // ???????

            // ?? 1: {num} - ??????
            if (placeholder == "num")
            {
                if (argIndex >= args.Length)
                {
                    Debug.LogWarning($"?? Not enough arguments for {{num}} in key '{key}'");
                    break;
                }

                if (int.TryParse(args[argIndex]?.ToString(), out int num))
                {
                    // ?? {num} ???
                    localized = localized.Remove(actualIndex, match.Length)
                                       .Insert(actualIndex, num.ToString());

                    argIndex++;

                    // ?????????? {s}
                    int nextSearchStart = actualIndex + num.ToString().Length;
                    int sIndex = localized.IndexOf("{s}", nextSearchStart);

                    if (sIndex != -1)
                    {
                        if (localeCode.StartsWith("en"))
                        {
                            string sReplacement = num == 1 ? "" : "s";
                            localized = localized.Remove(sIndex, 3).Insert(sIndex, sReplacement);
                            searchStart = sIndex + sReplacement.Length;
                        }
                        else
                        {
                            // ???????
                            localized = localized.Remove(sIndex, 3);
                            searchStart = sIndex;
                        }
                    }
                    else
                    {
                        searchStart = nextSearchStart;
                    }
                }
                else
                {
                    Debug.LogWarning($"?? Expected number for {{num}} but got '{args[argIndex]}'");
                    string replacement = args[argIndex]?.ToString() ?? "";
                    localized = localized.Remove(actualIndex, match.Length)
                                       .Insert(actualIndex, replacement);
                    argIndex++;
                    searchStart = actualIndex + replacement.Length;
                }
            }
            // ?? 2: {s} - ??? {s}?????? {num}?
            else if (placeholder == "s")
            {
                if (localeCode.StartsWith("en"))
                {
                    // ???????????????????????
                    localized = localized.Remove(actualIndex, match.Length); // ??
                    searchStart = actualIndex;
                }
                else
                {
                    // ???????
                    localized = localized.Remove(actualIndex, match.Length);
                    searchStart = actualIndex;
                }
            }
            // ?? 3: {} ??? {xxx} - ??????
            else
            {
                if (argIndex >= args.Length)
                {
                    Debug.LogWarning($"?? Not enough arguments for '' in key '{key}'");
                    searchStart = actualIndex + match.Length;
                    continue;
                }

                string replacement = args[argIndex]?.ToString() ?? "";
                localized = localized.Remove(actualIndex, match.Length)
                                   .Insert(actualIndex, replacement);
                argIndex++;
                searchStart = actualIndex + replacement.Length;
            }
        }

        return localized;
    }

}