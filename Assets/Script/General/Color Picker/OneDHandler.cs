using UnityEngine;
using UnityEngine.EventSystems;

public class OneDHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [Tooltip("Value change callback (0-1)")]
    public UnityEngine.Events.UnityEvent<float> onValueChanged;

    [SerializeField] private RectTransform handler;

    private RectTransform rectTransform;
    private float width;

    private bool isDragging = false;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        width = rectTransform.rect.width;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // ????????? handler
        UpdateHandlerPosition(eventData);

        // ??????
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
        localPoint.y = 0;

        // ?? handler
        handler.localPosition = localPoint;

        // ????
        float normalized = localPoint.x / width;
        onValueChanged?.Invoke(normalized);
    }

    public void SetPos(float normalizedValue)
    {
        Vector2 pos = new Vector2(width * Mathf.Clamp01(normalizedValue), 0);
        handler.localPosition = pos;
    }
}
