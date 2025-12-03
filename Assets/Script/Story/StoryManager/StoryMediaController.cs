using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using static ExcelReader;
using static GeneralAnimation;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;


/// <summary>
/// StoryMediaController handles background images, character images, text effects, and media playback
/// in a visual novel / galgame style.
/// </summary>
public class StoryMediaController : MonoBehaviour
{
    [Header("Background & Characters")]
    [SerializeField] private Image backgroundImage;
    private AsyncOperationHandle<Sprite>? currentBackgroundHandle = null;

    [SerializeField] private GameObject CharacterObjectManage;
    [SerializeField] private StoryCharacterImageControl CharacterImagePrefab;
    [SerializeField] private StoryTextEffect storyTextEffect;

    private List<StoryCharacterImageControl> CharacterImageGameObjects = new List<StoryCharacterImageControl>();
    private List<string> textEffect = new List<string>();




    // ====================================================
    // Media Playback
    // ====================================================
    public void PlayMediaAndBackground(ExcelPlotData data)
    {
        if (!string.IsNullOrEmpty(data.vocalAudioFileName))
            SoundManager.Instance.PlayVocal(data.vocalAudioFileName);

        if (!string.IsNullOrEmpty(data.backgroundImageFileName))
            UpdateBackgroundImage(data.backgroundImageFileName);

        if (!string.IsNullOrEmpty(data.backgroundMusicFileName))
            SoundManager.Instance.PlayBGM(data.backgroundMusicFileName);
    }

    // ====================================================
    // Background Image
    // ====================================================
    public async void UpdateBackgroundImage(string imageFileName)
    {
        if (string.IsNullOrWhiteSpace(imageFileName))
        {
            Debug.LogWarning("UpdateBackgroundImage: empty imageFileName");
            return;
        }

        if (currentBackgroundHandle.HasValue)
        {
            Addressables.Release(currentBackgroundHandle.Value);
            currentBackgroundHandle = null;
        }

        // 调用 SpriteLoadHelper
        SpriteInfo spriteInfo = await SpriteLoadHelper.LoadAsync(imageFileName);

        if (spriteInfo == null || spriteInfo.Sprite == null)
        {
            Debug.LogError($"Background sprite not found: {imageFileName}");
            return;
        }

        backgroundImage.sprite = spriteInfo.Sprite;
        backgroundImage.gameObject.SetActive(true);
    }


    // ====================================================
    // Character Image Handling
    // ====================================================
    public void UpdateCharacterImages(ExcelPlotData data, bool isLoad = false)
    {
        UpdateCharacterCount(data, isLoad);
    }

    /// <summary>
    /// Update the number of character image objects to match data.CharacterCount
    /// </summary>
    private void UpdateCharacterCount(ExcelPlotData data, bool isLoad = false)
    {
        int currentCount = CharacterImageGameObjects.Count;
        int targetCount = data.CharacterCount;

        // 创建/删除角色对象
        if (targetCount > currentCount)
        {
            for (int i = currentCount; i < targetCount; i++)
            {
                GameObject newObj = Instantiate(CharacterImagePrefab.gameObject);
                newObj.transform.SetParent(CharacterObjectManage.transform, false);
                var control = newObj.GetComponent<StoryCharacterImageControl>();
                string pureFileName = data.CharacterImageFileNames[i];
                string characterKey = pureFileName.Split('_')[0];
                control.SetCurrentCharacterKey(characterKey);
                CharacterImageGameObjects.Add(control);
            }
        }
        else if (targetCount < currentCount)
        {
            for (int i = currentCount - 1; i >= targetCount; i--)
            {
                Destroy(CharacterImageGameObjects[i].gameObject);
                CharacterImageGameObjects.RemoveAt(i);
            }
        }

        // 更新所有角色
        for (int i = 0; i < targetCount; i++)
        {
            var control = CharacterImageGameObjects[i];
            string pureFileName = data.CharacterImageFileNames[i];
            string characterKey = pureFileName.Split('_')[0];
            control.SetCurrentCharacterKey(characterKey);

            // ✅ 使用重载版本：直接传入 control
            UpdateCharacterImageQueued(data.CharacterActions[i], pureFileName, control);
        }
    }



    // --------------------------
    // ✅ 自动解析参数部分
    // --------------------------
    // --------------------------
    private void ParseParameters(string param1, string param2, out string characterKey, out string imageFileName, out string action)
    {
        characterKey = null;
        imageFileName = null;
        action = null;

        bool IsImageFileName(string s) => !string.IsNullOrEmpty(s) && s.Contains("_");

        if (IsImageFileName(param1)) { imageFileName = param1; characterKey = param1.Split('_')[0]; }
        else if (IsAction(param1)) { action = param1; }
        else { characterKey = param1; }

        if (!string.IsNullOrEmpty(param2))
        {
            if (IsImageFileName(param2)) { imageFileName = param2; if (string.IsNullOrEmpty(characterKey)) characterKey = param2.Split('_')[0]; }
            else if (IsAction(param2)) { action = param2; }
            else { if (string.IsNullOrEmpty(characterKey)) characterKey = param2; }
        }

        if (string.IsNullOrEmpty(characterKey) && !string.IsNullOrEmpty(action))
            Debug.LogWarning($"⚠️ Action '{action}' has no character key specified.");
    }

    public void UpdateCharacterImageSmart(string param1, string param2 = null, StoryCharacterImageControl targetControl = null)
    {
        // 如果已经有control对象，说明是Excel加载（初始化阶段）
        if (targetControl != null)
        {
            Debug.Log($"🟢 SmartUpdate: Using direct control mode for [{targetControl.GetCurrentCharacterKey()}]");
            ParseParameters(param1, param2, out _, out string imageFileName, out string action);
            UpdateCharacterImageQueued(action, imageFileName, targetControl);
            return;
        }

        // 否则是运行时命令执行（字符串输入）
        Debug.Log($"🟢 SmartUpdate: Using queued mode for runtime command: {param1}, {param2}");
        UpdateCharacterImageQueued(param1, param2);
    }


    public void UpdateCharacterImageQueued(string param1, string param2 = null)
    {
        Debug.Log($"🔵 UpdateCharacterImageQueued called: param1=[{param1}], param2=[{param2}]");

        ParseParameters(param1, param2, out string characterKey, out string imageFileName, out string action);

        Debug.Log($"🔵 Parsed: characterKey=[{characterKey}], imageFileName=[{imageFileName}], action=[{action}]");

        if (string.IsNullOrEmpty(characterKey))
        {
            Debug.LogWarning("⚠️ Could not determine character key.");
            return;
        }

        var targetControl = CharacterImageGameObjects.FirstOrDefault(c =>
            c != null && c.GetCurrentCharacterKey().Equals(characterKey, StringComparison.OrdinalIgnoreCase));

        Debug.Log($"🔵 Found control: {(targetControl != null ? targetControl.GetCurrentCharacterKey() : "NULL")}");

        if (targetControl == null)
        {
            Debug.LogWarning($"⚠️ No active character found for key [{characterKey}].");
            Debug.Log($"🔵 Available characters: {string.Join(", ", CharacterImageGameObjects.Select(c => c?.GetCurrentCharacterKey() ?? "null"))}");
            return;
        }

        Debug.Log($"🔵 Calling UpdateCharacterImageQueued overload...");
        UpdateCharacterImageQueued(action, imageFileName, targetControl);
    }
    /// <summary>
    /// 直接通过 Control 对象更新角色（用于 Excel 数据加载）
    /// </summary>
    public void UpdateCharacterImageQueued(string action, string imageFileName, StoryCharacterImageControl targetControl)
    {
        Debug.Log($"🔵 Calling UpdateCharacterImageQueued Start;string {action}, string {imageFileName}, StoryCharacterImageControl {targetControl}");

        if (targetControl == null)
        {
            Debug.LogWarning("UpdateCharacterImageQueued: targetControl is null.");
            return;
        }

        targetControl.EnqueueAction(async (duration) =>
        {
            var req = new CharacterActionRequest
            {
                ActionType = GetActionType(action),
                OriginalActionString = action,
                Control = targetControl,
                Obj = targetControl.gameObject,
                CanvasGroup = targetControl.GetComponent<CanvasGroup>() ?? targetControl.gameObject.AddComponent<CanvasGroup>(),
                IsLoad = false,
                Duration = duration,
                ImageFileName = imageFileName
            };

            Debug.Log($"Executing HandleCharacterAction({req.ActionType})");
            await HandleCharacterAction(req);

            string logMsg = $"✅ Character [{targetControl.GetCurrentCharacterKey()}]";
            if (!string.IsNullOrEmpty(imageFileName))
                logMsg += $" | Image: {imageFileName}";
            if (!string.IsNullOrEmpty(action))
                logMsg += $" | Action: {action}";
            Debug.Log(logMsg);
        });
        Debug.Log($"🔵 Calling UpdateCharacterImageQueued done");

    }




    private (string pureFileName, bool shouldFlip) ParseImageFileName(string imageFileName)
    {
        string pureImageFileName = imageFileName;
        bool shouldFlip = false;

        int parenStart = imageFileName.IndexOf('(');
        int parenEnd = imageFileName.IndexOf(')');

        if (parenStart != -1 && parenEnd != -1 && parenEnd > parenStart)
        {
            string command = imageFileName.Substring(parenStart + 1, parenEnd - parenStart - 1).Trim();
            pureImageFileName = imageFileName.Substring(0, parenStart).Trim();
            if (command == "Flip") shouldFlip = true;
        }

        return (pureImageFileName, shouldFlip);
    }

    private void UpdateImage(string imageFileName, Image image)
    {
        (string pureFileName, bool shouldFlip) = ParseImageFileName(imageFileName);
        Sprite sprite = Resources.Load<Sprite>(pureFileName);

        if (sprite != null)
        {
            image.sprite = sprite;
            image.gameObject.SetActive(true);

            Vector3 scale = image.rectTransform.localScale;
            scale.x = shouldFlip ? -Mathf.Abs(scale.x) : Mathf.Abs(scale.x);
            image.rectTransform.localScale = scale;
        }
        else
        {
            Debug.LogError(Constants.IMAGE_LOAD_FAILED + pureFileName);
        }
    }

    // ====================================================
    // Cleanup
    // ====================================================
    public void RemoveCharacterImageGameObject(StoryCharacterImageControl removeImage)
    {
        CharacterImageGameObjects.Remove(removeImage);
    }

    public void ClearCharacterImageGameObjects()
    {
        // ? ????????????? Destroy ????
        var toDestroy = new List<StoryCharacterImageControl>(CharacterImageGameObjects);

        foreach (var characterImage in toDestroy)
        {
            characterImage.Destroy();
        }

        // ? ????????
        CharacterImageGameObjects.Clear();
    }

    public void ClearAll()
    {
        backgroundImage.gameObject.SetActive(false);
        ClearCharacterImageGameObjects();
    }

    // ====================================================
    // Text Effect Accessors
    // ====================================================
    public List<string> GetTextEffect() => textEffect;
    public void SetTextEffect(List<string> effects) => textEffect = new List<string>(effects);
    public void AddTextEffect(string effect) => textEffect.Add(effect);
    public void ClearTextEffect() => textEffect.Clear();

    public List<StoryCharacterImageControl> GetCharacterImageGameObjects() => CharacterImageGameObjects;
}
