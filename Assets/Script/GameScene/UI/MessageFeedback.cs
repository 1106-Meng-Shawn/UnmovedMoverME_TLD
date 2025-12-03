using TMPro;
using UnityEngine;
using DG.Tweening;

public class MessageFeedback : MonoBehaviour
{
    [Header("References")]
    public TMP_Text messageText;
    public RectTransform rectTransform;
    public CanvasGroup canvasGroup;
    public Canvas parentCanvas;

    [Header("Animation Settings")]
    public float duration = 1.2f;
    public float startYOffset = 100f;   // 起始 Y 位置
    public float marginToTop = 100f;    // 距离顶部边缘的间距
    public float stayTime = 0.25f;    // 距离顶部边缘的间距


    void Awake()
    {
        if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();
        if (rectTransform == null) rectTransform = GetComponent<RectTransform>();
        if (parentCanvas == null) parentCanvas = GetComponentInParent<Canvas>();
    }

    public void ShowMessageToTop(string content)
    {
        if (messageText == null || rectTransform == null || parentCanvas == null)
        {
            Debug.LogError("MessageFeedback: missing references.");
            return;
        }

        messageText.text = content;

        // 初始状态：透明且稍微缩小
        canvasGroup.alpha = 0f;
        rectTransform.localScale = Vector3.one * 0.95f;

        // 锚点设置到底部中间
        rectTransform.anchorMin = new Vector2(0.5f, 0f);
        rectTransform.anchorMax = new Vector2(0.5f, 0f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);

        float canvasHeight = ((RectTransform)parentCanvas.transform).rect.height;
        float targetY = canvasHeight - marginToTop;

        // 这里是从最底部开始，即anchoredPosition.y设为0，或更低一些（0表示锚点位置底部）
        float startY = 0f;  // 你也可以用 -10f 或其他负值微调起点

        rectTransform.anchoredPosition = new Vector2(0f, startY);

        Sequence seq = DOTween.Sequence();

        // 入场：从最底部向顶部移动 + 缩放 + 淡入
        seq.Append(rectTransform.DOAnchorPosY(targetY, duration).SetEase(Ease.OutCubic));
        seq.Join(canvasGroup.DOFade(1f, duration * 0.8f));
        seq.Join(rectTransform.DOScale(1f, duration * 0.6f).SetEase(Ease.OutBack));

        // 顶部停留时间
        seq.AppendInterval(stayTime);

        // 出场：缩小 + 淡出
        seq.Append(canvasGroup.DOFade(0f, 0.6f));
        seq.Join(rectTransform.DOScale(0.95f, 0.6f));

        seq.OnComplete(() => Destroy(gameObject));
    }




    public float stackedSpacing = 40f;       // 每条消息之间的 Y 间隔
    public float floatUpDistance = 50f;      // 向上浮动的动画距离
    public void ShowMessageAtTop(string content)
    {
        if (messageText == null || rectTransform == null || parentCanvas == null)
        {
            Debug.LogError("MessageFeedback: missing references.");
            return;
        }

        messageText.text = content;
        canvasGroup.alpha = 0f; // 初始透明
        rectTransform.localScale = Vector3.one * 0.95f; // 初始稍小

        // 设置锚点到底部中间
        rectTransform.anchorMin = new Vector2(0.5f, 0f);
        rectTransform.anchorMax = new Vector2(0.5f, 0f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);

        var canvasRect = (RectTransform)parentCanvas.transform;
        float canvasHeight = canvasRect.rect.height;
        float targetY = canvasHeight - marginToTop;

        // 从略低位置开始
        float startY = targetY - 40f;
        rectTransform.anchoredPosition = new Vector2(0f, startY);

        Sequence seq = DOTween.Sequence();

        // 入场：位置移动 + 缩放 + 淡入
        seq.Append(rectTransform.DOAnchorPosY(targetY, duration).SetEase(Ease.OutCubic));
        seq.Join(canvasGroup.DOFade(1f, duration * 0.8f));
        seq.Join(rectTransform.DOScale(1f, duration * 0.6f).SetEase(Ease.OutBack));

        // 可选：浮动/等待
        seq.AppendInterval(stayTime);

        // 出场：淡出 + 缩小
        seq.Append(canvasGroup.DOFade(0f, 0.6f));
        seq.Join(rectTransform.DOScale(0.95f, 0.6f));

        seq.OnComplete(() => Destroy(gameObject));
    }

}
