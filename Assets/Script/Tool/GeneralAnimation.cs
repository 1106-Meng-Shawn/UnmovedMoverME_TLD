using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static BattleCharacterInfo;
using static GeneralAnimation;
using static PositionAtBattle;



[System.Serializable]
public class AnimationConfig
{
    public float TotalDuration = EffectConstants.totalEffectDuration ;
    public float StartOffsetX = 25f;
    public float StartOffsetY = 25f;
    public float EndOffsetX = 75f;
    public float EndOffsetY = 75f;
    public float FadeRatio = 0.6f;  

    public static AnimationConfig Default => new AnimationConfig();
}



public static class AnimationConstants
{
    public const float GalDefaultX = 0f;
    public const float GalDefaultY = -950f;

}

public class CharacterActionRequest
{
    public string ActionType;                    // ?????? "Appear", "MoveTo", "Disappear"
    public string OriginalActionString;          // ?? action????????
    public StoryCharacterImageControl Control;   // ????
    public GameObject Obj;
    public CanvasGroup CanvasGroup;
    public bool IsLoad;
    public float Duration = Constants.DURATION_TIME; // ??????
    public string ImageFileName;                // ???????
}



public static class GeneralAnimation
{
    public static Sequence CreateDamageTextSequence(
       DamageResult damageResult,
       GameObject prefab,
       Transform parent,
       float direction = 1f, // 1 = ??-1 = ?
       AnimationConfig config = null)
    {
        if (prefab == null || parent == null)
            return null;

        config ??= AnimationConfig.Default;

        // ??? prefab
        GameObject dmgObj = GameObject.Instantiate(prefab, parent);
        var dmgControl = dmgObj.GetComponent<DamageTextControl>();
        var dmgRT = dmgObj.GetComponent<RectTransform>();
        var dmgText = dmgObj.GetComponent<TMP_Text>();

        if (dmgControl == null || dmgText == null || dmgRT == null)
        {
            GameObject.Destroy(dmgObj);
            return null;
        }

        // ??????
        dmgControl.SetDamageResult(damageResult);

        // ???????
        RectTransform parentRT = parent as RectTransform;
        Vector2 parentPivot = parentRT.pivot;
        Vector2 parentSize = parentRT.rect.size;

        // ??? = (?? * (1 - pivot.x), ?? * (1 - pivot.y))
        Vector2 topRight = new Vector2(parentSize.x * (1 - parentPivot.x), parentSize.y * (1 - parentPivot.y));

        // ????
        Vector2 startOffset = new Vector2(direction * config.StartOffsetX, config.StartOffsetY);
        dmgRT.anchoredPosition = topRight + startOffset;

        // ????
        dmgText.alpha = 1f;

        // ????
        Vector2 endOffset = new Vector2(direction * config.EndOffsetX, config.EndOffsetY);

        // ?? Sequence
        Sequence seq = DOTween.Sequence();
        seq.Append(dmgRT.DOAnchorPos(topRight + endOffset, config.TotalDuration).SetEase(Ease.OutCubic));
        seq.Join(dmgText.DOFade(0, config.TotalDuration * config.FadeRatio)
            .SetDelay(config.TotalDuration * (1 - config.FadeRatio)));

        seq.OnComplete(() => GameObject.Destroy(dmgObj));
        return seq;
    }


    public static Sequence CreateHealTextSequence(
    int healValue,
    GameObject prefab,
    Transform parent,
    AnimationConfig config = null)
    {
        if (prefab == null || parent == null)
            return null;

        config ??= AnimationConfig.Default;

        // ??? prefab
        GameObject healObj = GameObject.Instantiate(prefab, parent);
        healObj.transform.rotation = Quaternion.identity;

        var healText = healObj.GetComponent<TMP_Text>();
        var healRT = healObj.GetComponent<RectTransform>();

        if (healText == null || healRT == null)
        {
            GameObject.Destroy(healObj);
            return null;
        }

        // ????
        healText.text = $"+{healValue}";
        healText.color = new Color(0f, 1f, 0.3f);
        healText.alpha = 1f;

        // ???????
        RectTransform parentRT = parent as RectTransform;
        Vector2 parentPivot = parentRT.pivot;
        Vector2 parentSize = parentRT.rect.size;
        Vector2 topRight = new Vector2(parentSize.x * (1 - parentPivot.x), parentSize.y * (1 - parentPivot.y));

        // ???? & ??
        Vector2 startOffset = new Vector2(0, config.StartOffsetY);
        Vector2 endOffset = new Vector2(0, config.EndOffsetY + 60f);
        healRT.anchoredPosition = topRight + startOffset;
        healRT.localScale = Vector3.zero; // ? 0 ????

        // ?? Sequence
        Sequence seq = DOTween.Sequence();

        // 1?? ????
        seq.Append(healRT.DOScale(1.5f, config.TotalDuration * 0.2f).SetEase(Ease.OutBack));

        // 2?? ??????
        seq.Append(healRT.DOScale(1f, config.TotalDuration * 0.1f).SetEase(Ease.InOutSine));

        // 3?? ?? + ??
        seq.Append(healRT.DOAnchorPos(topRight + endOffset, config.TotalDuration * 0.7f).SetEase(Ease.OutCubic));
        seq.Join(healText.DOFade(0, config.TotalDuration * config.FadeRatio)
            .SetDelay(config.TotalDuration * (1 - config.FadeRatio)));

        // ?????
        seq.OnComplete(() => GameObject.Destroy(healObj));

        return seq;
    }


    public static IEnumerator FadeOutCoroutine(CanvasGroup canvasGroup)
    {
        float fadeDuration = EffectConstants.totalEffectDuration / 2;

        if (canvasGroup == null)
        {
            yield break;
        }

        float startAlpha = EffectConstants.aliveAlpha;
        float targetAlpha = EffectConstants.deadAlpha;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = targetAlpha;
    }



    #region GalGame Animation


    private static readonly Dictionary<string, Func<CharacterActionRequest, Task>> actionHandlers
    = new(StringComparer.OrdinalIgnoreCase)
    {
        { ActionConstants.APPEAR_AT, HandleAppearAction },
        { ActionConstants.APPEAR_AT_INSTANTLY, HandleAppearAtInstantlyAction },
        { ActionConstants.DISAPPEAR, HandleDisappearAction },
        { ActionConstants.MOVE_TO, HandleMoveToAction },
        { ActionConstants.MOVE_BY, HandleMoveByAction },
        { ActionConstants.STAY, HandleStayAction }
    };


    public static bool IsAction(string s)
    {
        if (string.IsNullOrEmpty(s)) return false;
        foreach (var key in actionHandlers.Keys)
        {
            if (s.StartsWith(key, StringComparison.OrdinalIgnoreCase))
                return true;
        }
        return false;
    }
    public static string GetActionType(string s)
    {
        if (string.IsNullOrEmpty(s)) return null;
        foreach (var key in actionHandlers.Keys)
        {
            if (s.StartsWith(key, StringComparison.OrdinalIgnoreCase))
                return key;
        }
        return null;
    }

    public static async Task HandleCharacterAction(CharacterActionRequest request)
    {
        if (!string.IsNullOrEmpty(request.ImageFileName)) request.Control.SetCharacterImage(request.Control.GetCurrentCharacterKey(), request.ImageFileName);

        Debug.Log("HandleCharacterAction(CharacterActionRequest request) work");

        if (actionHandlers.TryGetValue(request.ActionType, out var handler))
        {
            await handler(request);
        }
    }


    public static async Task HandleAppearAtInstantlyAction(CharacterActionRequest request)
    {
        var (x, y) = ParseCoordinates(request.OriginalActionString, request.Obj);
        request.Obj.GetComponent<RectTransform>().anchoredPosition = new Vector2(x, y);

        request.Duration = 0f;

        await request.CanvasGroup.DOFade(1, request.Duration).From(0).AsyncWaitForCompletion();
    }


    public static async Task HandleAppearAction(CharacterActionRequest request)
    {
        var (x, y) = ParseCoordinates(request.OriginalActionString, request.Obj);
        request.Obj.GetComponent<RectTransform>().anchoredPosition = new Vector2(x, y);

        if (request.IsLoad || request.ActionType == ActionConstants.APPEAR_AT_INSTANTLY)
            request.Duration = 0f;

        await request.CanvasGroup.DOFade(1, request.Duration).From(0).AsyncWaitForCompletion();
    }

    public static async Task HandleDisappearAction(CharacterActionRequest request)
    {
        float duration = request.Duration <= 0f ? Constants.DURATION_TIME : request.Duration;
        await request.CanvasGroup.DOFade(0, duration).AsyncWaitForCompletion();
        ClearCharacterImageGameObject(request.Control);
    }

    public static async Task HandleMoveToAction(CharacterActionRequest request)
    {
        Debug.Log("HandleMoveToAction(CharacterActionRequest request) work");
        var (x, y) = ParseCoordinates(request.OriginalActionString, request.Obj);
        await request.Obj.GetComponent<RectTransform>().DOAnchorPos(new Vector2(x, y), request.Duration).AsyncWaitForCompletion();
    }

    public static async Task HandleMoveByAction(CharacterActionRequest request)
    {
        var (dx, dy) = ParseCoordinates(request.OriginalActionString, request.Obj);
        var rect = request.Obj.GetComponent<RectTransform>();
        Vector2 target = rect.anchoredPosition + new Vector2(dx, dy);
        await rect.DOAnchorPos(target, request.Duration).AsyncWaitForCompletion();
    }

    public static async Task HandleStayAction(CharacterActionRequest request)
    {
        var (x, y) = ParseCoordinates(request.OriginalActionString, request.Obj);
        request.Obj.GetComponent<RectTransform>().anchoredPosition = new Vector2(x, y);
        await Task.CompletedTask;
    }



    private static (float, float) ParseCoordinates(string action, GameObject obj)
        {
            float defaultX = AnimationConstants.GalDefaultX;
            float defaultY = AnimationConstants.GalDefaultY;

            int start = action.IndexOf('(') + 1;
            int end = action.IndexOf(')');
            if (start < 0 || end < 0 || end <= start)
                return (defaultX, defaultY);

            string[] parts = action.Substring(start, end - start).Split(',');
            if (parts.Length < 2)
                return (defaultX, defaultY);

            float x = defaultX;
            float y = defaultY;

            string xPart = parts[0].Trim();
            if (!string.Equals(xPart, "N", StringComparison.OrdinalIgnoreCase) && float.TryParse(xPart, out float parsedX))
                x = parsedX;

            string yPart = parts[1].Trim();
            if (!string.Equals(yPart, "N", StringComparison.OrdinalIgnoreCase) && float.TryParse(yPart, out float parsedY))
                y = parsedY;

            return (x, y);
        }
    



    private static void ClearCharacterImageGameObject(StoryCharacterImageControl control)
    {
        if (control != null && control.gameObject != null)control.Destroy();
    }

    #endregion

}
