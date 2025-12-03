using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// Controls a layered Story Character image.
/// Supports poses, expressions, accessories, object pooling, and fade animations.
/// </summary>
/// 

public class StoryCharacterImageControl : MonoBehaviour
{
    [Header("Root Layer for Character Sprites")]
    [SerializeField] private RectTransform layerRoot;

    private StoryCharacterImageDataBase currentDB;
    [SerializeField] string currentCharacterKey;
    private string currentCharacterType;
    private string currentPose;
    private string currentExpression;
    private List<string> currentAccessories = new List<string>();

    private Dictionary<string, Image> layerPool = new Dictionary<string, Image>();
    private readonly Stack<Image> inactiveImages = new Stack<Image>();

    [SerializeField] LayerMask StoryLayerMask;

    // === ?????? ===
    private Queue<Func<Task>> actionQueue = new Queue<Func<Task>>();
    private bool isPlayingAction = false;

    public string GetCurrentCharacterKey() => currentCharacterKey;
    //  private const float DEFAULT_FADE_TIME = 0.35f;

    // ================================================================
    // Public API: set character image using CharacterKey + State string
    // ================================================================
    public void SetCharacterImage(string characterKey, string state)
    {
        if (string.IsNullOrEmpty(characterKey) || string.IsNullOrEmpty(state)) return;

        SetCurrentCharacterKey(characterKey);

        if (string.IsNullOrEmpty(state))
        {
            Debug.LogWarning("[StoryCharacterImageControl] state is empty.");
            return;
        }

        string[] parts = state.Split('_');
        if (parts.Length < 1)
        {
            Debug.LogWarning($"[StoryCharacterImageControl] invalid input: {state}");
            return;
        }

        currentCharacterKey = characterKey;

        string newCharType = parts[0];
        string newPose = parts.Length > 1 ? parts[1] : currentPose;
        string newExpr = parts.Length > 2 ? parts[2] : currentExpression;

        List<string> addAccessories = new List<string>();
        List<string> removeAccessories = new List<string>();

        for (int i = 3; i < parts.Length; i++)
        {
            string acc = parts[i];
            if (acc.StartsWith("!"))
                removeAccessories.Add(acc.Substring(1));
            else
                addAccessories.Add(acc);
        }

        if (currentDB == null || currentCharacterType != newCharType)
        {
            currentDB = LoadCharacterDB(characterKey);
            currentCharacterType = newCharType;
        }

        if (newPose != currentPose)
        {
            if (string.IsNullOrEmpty(newExpr))
                newExpr = currentExpression;

            foreach (var acc in currentAccessories)
                if (!addAccessories.Contains(acc))
                    addAccessories.Add(acc);
        }

        currentPose = newPose;
        currentExpression = newExpr;

        foreach (var acc in removeAccessories)
            currentAccessories.RemoveAll(a => a.Equals(acc, StringComparison.OrdinalIgnoreCase));

        foreach (var acc in addAccessories)
            if (!currentAccessories.Contains(acc))
                currentAccessories.Add(acc);

        string fullCode = BuildFullCode();
        ApplyCharacterState(fullCode);
    }

    // ================================================================
    // Apply state: generate layers and fade in/out
    // ================================================================
    private void ApplyCharacterState(string fullCode)
    {
        if (currentDB == null)
        {
            Debug.LogWarning($"[StoryCharacterImageControl] No DB loaded for {fullCode}");
            return;
        }

        var layers = currentDB.GetSpritesByCode(fullCode);
        var activeTags = new HashSet<string>(layers.Select(l => l.tag));

        // === ????: Back?? < ?? < ?? < Front?? ===
        layers = layers.OrderBy(l =>
        {
            string tagLower = l.tag.ToLower();
            if (tagLower.Contains("accessory_back")) return 3;   // ??????
            if (tagLower.Contains("body")) return 2;             // ????
            if (tagLower.Contains("expression")) return 1;       // ??????
            if (tagLower.Contains("accessory_front")) return 0;  // ??????
            return 0; // ????
        }).ToList();

        int order = 0;
        foreach (var layerInfo in layers)
        {
            var img = GetOrCreateLayer(layerInfo.tag);
            if (img.sprite != layerInfo.sprite)
                img.sprite = layerInfo.sprite;

            img.rectTransform.SetSiblingIndex(order++);
            if (!img.gameObject.activeSelf)
                img.gameObject.SetActive(true);

            //var cg = img.GetComponent<CanvasGroup>();
            //if (cg == null) cg = img.gameObject.AddComponent<CanvasGroup>();
            //cg.DOFade(1, DEFAULT_FADE_TIME).From(0);
        }

        // Hide unused layers
        var unusedTags = layerPool.Keys.Except(activeTags).ToList();
        foreach (var tag in unusedTags)
        {
            var img = layerPool[tag];
            if (img.gameObject.activeSelf)
            {
                img.gameObject.SetActive(false);
            }
        }
    }

    public void EnqueueAction(Func<float, Task> actionWithDuration, float customDuration = -1f)
    {
        actionQueue.Enqueue(async () =>
        {
            float duration = customDuration > 0 ? customDuration : Constants.DURATION_TIME;
            await actionWithDuration(duration);
        });

        if (!isPlayingAction)
            PlayNextActionInQueue();
    }


    private async void PlayNextActionInQueue() // ? ?? async void
    {
        if (actionQueue.Count == 0)
        {
            isPlayingAction = false;
            return;
        }

        isPlayingAction = true;

        try
        {
            var nextAction = actionQueue.Dequeue();
            await nextAction();
        }
        catch (Exception ex)
        {
            Debug.LogError($"? PlayNextActionInQueue Exception: {ex}");
        }

        PlayNextActionInQueue(); // ???????
    }


    private Image GetOrCreateLayer(string tag)
    {
        if (layerPool.TryGetValue(tag, out Image existing))
            return existing;

        Image img;
        if (inactiveImages.Count > 0)
        {
            img = inactiveImages.Pop();
            img.gameObject.SetActive(true);
        }
        else
        {
            var go = new GameObject($"Layer_{tag}", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            go.transform.SetParent(layerRoot, false);
            int layerIndex = Mathf.RoundToInt(Mathf.Log(StoryLayerMask.value, 2));
            go.layer = layerIndex;

            img = go.GetComponent<Image>();
            img.rectTransform.anchorMin = Vector2.zero;
            img.rectTransform.anchorMax = Vector2.one;
            img.rectTransform.offsetMin = Vector2.zero;
            img.rectTransform.offsetMax = Vector2.zero;
        }

        layerPool[tag] = img;
        img.name = $"Layer_{tag}";
        return img;
    }

    // ================================================================
    // Generate full code for DB query
    // ================================================================
    private string BuildFullCode()
    {
        string accStr = string.Join("_", currentAccessories);
        if (!string.IsNullOrEmpty(accStr))
            return $"{currentCharacterType}_{currentPose}_{currentExpression}_{accStr}";
        else
            return $"{currentCharacterType}_{currentPose}_{currentExpression}";
    }

    // ================================================================
    // Load character DB
    // ================================================================
    private StoryCharacterImageDataBase LoadCharacterDB(string charKey)
    {
#if UNITY_EDITOR
        string[] guids = UnityEditor.AssetDatabase.FindAssets($"{charKey}_DB t:StoryCharacterImageDataBase");
        if (guids.Length > 0)
        {
            string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[0]);
            return UnityEditor.AssetDatabase.LoadAssetAtPath<StoryCharacterImageDataBase>(path);
        }
        return null;
#else
        return Resources.Load<StoryCharacterImageDataBase>($"CharacterImage/{charKey}_DB");
#endif
    }

    public void SetCharacterKey(string characterKey)
    {
        currentCharacterKey = characterKey;
    }


    public Sprite GetIcon()
    {
        return currentDB.GetIcon(currentCharacterType, currentPose);
    }

    // ================================================================
    // Clear all layers
    // ================================================================
    public void ClearAllLayers()
    {
        foreach (var kv in layerPool)
        {
            kv.Value.gameObject.SetActive(false);
            inactiveImages.Push(kv.Value);
        }
        layerPool.Clear();
    }



    // ================================================================
    // Destroy control
    // ================================================================
    public void Destroy()
    {
        if (DOTween.IsTweening(gameObject))
            DOTween.Kill(gameObject);
        Destroy(gameObject);
        TotalStoryManager.Instance.MediaController.RemoveCharacterImageGameObject(this);
    }

    public void SetCurrentCharacterKey(string characterKey)
    {
        if (string.IsNullOrEmpty(characterKey)) return;
        currentCharacterKey = characterKey;
    }

}
