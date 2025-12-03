using UnityEngine;
using UnityEngine.EventSystems;

public class DraggablePanel : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    private Vector2 offset;
    private RectTransform rectTransform;

    [Header("Resize Handles")]
    public RectTransform topLeftHandle;
    public RectTransform topRightHandle;
    public RectTransform bottomLeftHandle;
    public RectTransform bottomRightHandle;

    private bool isResizing = false;
    private RectTransform activeHandle;
    private Vector2 originalSize;
    private float aspectRatio;

    public bool isResiz = false;
    public bool shouldBlockPanelDrag = false; 
    private bool isMouseOverPanel;
    private bool isDragging = false;
    [SerializeField] private bool isRightClickClose = true;

    void Start()
    {

        rectTransform = GetComponent<RectTransform>();

        if (isResiz)
        {
            originalSize = rectTransform.sizeDelta;
            aspectRatio = originalSize.x / originalSize.y;

            topLeftHandle = transform.Find("TopLeft") as RectTransform;
            topRightHandle = transform.Find("TopRight") as RectTransform;
            bottomLeftHandle = transform.Find("BottomLeft") as RectTransform;
            bottomRightHandle = transform.Find("BottomRight") as RectTransform;

            AddHandleEvents(topLeftHandle);
            AddHandleEvents(topRightHandle);
            AddHandleEvents(bottomLeftHandle);
            AddHandleEvents(bottomRightHandle);
        }
    }

    void AddHandleEvents(RectTransform handle)
    {
        if (handle == null) return;

        EventTrigger trigger = handle.gameObject.AddComponent<EventTrigger>();

        var entry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.BeginDrag
        };
        entry.callback.AddListener((data) =>
        {
            isResizing = true;
            activeHandle = handle;
        });
        trigger.triggers.Add(entry);

        var dragEntry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.Drag
        };
        dragEntry.callback.AddListener((data) =>
        {
            if (!isResizing) return;
            OnHandleDrag((PointerEventData)data);
        });
        trigger.triggers.Add(dragEntry);

        var endEntry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.EndDrag
        };
        endEntry.callback.AddListener((data) =>
        {
            isResizing = false;
            activeHandle = null;
        });
        trigger.triggers.Add(endEntry);
    }

    void OnHandleDrag(PointerEventData data)
    {
        Vector2 delta = data.delta;
        float scaleDelta = Mathf.Max(delta.x, delta.y);

        // ?????
        Vector2 newSize = rectTransform.sizeDelta + new Vector2(scaleDelta, scaleDelta / aspectRatio);
        newSize = ClampSize(newSize);

        rectTransform.sizeDelta = newSize;
    }

    Vector2 ClampSize(Vector2 size)
    {
        float min = 100f; // ????
        float width = Mathf.Clamp(size.x, min, originalSize.x);
        float height = width / aspectRatio;
        return new Vector2(width, height);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (isResizing || shouldBlockPanelDrag) return;

        isDragging = true;
        offset = (Vector2)rectTransform.localPosition - eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isResizing || shouldBlockPanelDrag) return;

        rectTransform.localPosition = eventData.position + offset;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        transform.SetAsLastSibling(); // ????

        if (isMouseOverPanel && Input.GetMouseButtonDown(1) && isRightClickClose)
        {
            ClosePanel();
        }

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isMouseOverPanel = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isMouseOverPanel = false;
    }

    public bool IsMouseOverPanel()
    {
        return isMouseOverPanel;
    }


    void ClosePanel()
    {
        gameObject.SetActive(false);
    }

}
