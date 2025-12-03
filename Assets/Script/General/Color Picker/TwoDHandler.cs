using UnityEngine;
using UnityEngine.EventSystems;

public class TwoDHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [Tooltip("Value change callback (Vector2: x=horizontal 0-1, y=vertical 0-1)")]
    public UnityEngine.Events.UnityEvent<Vector2> onValueChanged;

    [SerializeField] private RectTransform handler;

    private RectTransform rectTransform;
    private float width;
    private float height;

    private bool isDragging = false;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        width = rectTransform.rect.width;
        height = rectTransform.rect.height;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // 点击空白区域也移动 handler
        UpdateHandlerPosition(eventData);

        // 开始拖拽
        isDragging = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isDragging = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isDragging)
        {
            UpdateHandlerPosition(eventData);
        }
    }

    private void UpdateHandlerPosition(PointerEventData eventData)
    {
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform, eventData.position, eventData.pressEventCamera, out Vector2 localPoint))
        {
            return;
        }

        localPoint.x = Mathf.Clamp(localPoint.x, 0, width);
        localPoint.y = Mathf.Clamp(localPoint.y, 0, height);

        handler.localPosition = localPoint;

        Vector2 normalizedValue = new Vector2(
            localPoint.x / width,
            localPoint.y / height
        );

        onValueChanged?.Invoke(normalizedValue);
    }

    public void SetPos(float normalizedX, float normalizedY)
    {
        Vector2 pos = new Vector2(
            width * Mathf.Clamp01(normalizedX),
            height * Mathf.Clamp01(normalizedY)
        );
        handler.localPosition = pos;
    }
}
