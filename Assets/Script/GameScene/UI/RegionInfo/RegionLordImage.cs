using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RegionLordImage : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler,IPointerClickHandler
{
    public RegionInfo regionInfoUI;
    private GameObject draggedIcon;

    public Button lordButton;

    public GameObject characterPanel;

    private float lastClickTime = 0f;
    private const float doubleClickThreshold = 0.2f;


    void Start()
    {
        lordButton.onClick.AddListener(() => TogglePanel(characterPanel, Vector2.zero));

    }

    public void OnBeginDrag(PointerEventData eventData)
    {

        if (draggedIcon != null)
        {
            Destroy(draggedIcon);
        }

        draggedIcon = new GameObject("DraggedIcon");
        draggedIcon.transform.SetParent(transform.root); // 确保是在 Canvas 下面
        draggedIcon.transform.SetAsLastSibling(); // 确保图标显示在最上层

        Image draggedImage = draggedIcon.AddComponent<Image>();
       // draggedImage.sprite = regionInfoUI.lord.icon;
        draggedImage.raycastTarget = false;

        return;

        RectTransform draggedRectTransform = draggedIcon.AddComponent<RectTransform>();

        draggedRectTransform.sizeDelta = new Vector2(100f, 100f); 
        draggedRectTransform.localScale = Vector3.one; 

        // 设置锚点为中心
        draggedRectTransform.pivot = new Vector2(0.5f, 0.5f);
        draggedRectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        draggedRectTransform.anchorMax = new Vector2(0.5f, 0.5f);

        // 正确转换屏幕坐标到本地坐标
        Vector2 localPoint;
        RectTransform canvasRect = transform.root.GetComponent<RectTransform>();

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            eventData.position,
            null, // 因为是 Screen Space - Overlay，传 null
            out localPoint))
        {
            draggedRectTransform.localPosition = localPoint;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Destroy(draggedIcon);
        draggedIcon = null;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (draggedIcon != null)
        {
            RectTransform draggedIconRect = draggedIcon.GetComponent<RectTransform>();
            RectTransform canvasRect = transform.root.GetComponent<RectTransform>();

            Vector2 localPoint;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect,
                eventData.position,
                null, // Screen Space - Overlay 模式下是 null
                out localPoint))
            {
                draggedIconRect.localPosition = localPoint;
                draggedIconRect.localScale = Vector3.one; // 保持正常缩放
            }
        }
    }


    public void OnEndDrag(PointerEventData eventData)
    {
        Destroy(draggedIcon);
        draggedIcon = null;

        GameObject Temp = eventData.pointerCurrentRaycast.gameObject;
/*        if (Temp == null) {regionInfoUI.regionAtUI.MoveLord();return;}

        RegionColumControl regionColumControl = Temp.GetComponent<RegionColumControl>();
        if (regionColumControl != null){
            if (regionColumControl.region == regionInfoUI.regionAtUI) return;
            regionColumControl.region.SetLord(regionInfoUI.regionAtUI.lord);
            regionInfoUI.regionAtUI.lord = null;
            return;
        }

        if(Temp.GetComponent<RegionLordImage>() != null){
            return;
        }



        regionInfoUI.regionAtUI.MoveLord();*/

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        float currentTime = Time.time;

        if (currentTime - lastClickTime <= doubleClickThreshold)
        {

            TogglePanel(characterPanel, Vector2.zero);
        }

        lastClickTime = currentTime;
    }

    void TogglePanel(GameObject panel, Vector2 setPosition)
    {

        if (panel != null && !panel.activeSelf)
        {
            RectTransform panelRectTransform = panel.GetComponent<RectTransform>();
            panelRectTransform.anchoredPosition = setPosition;
            panel.SetActive(true);
        } else
        {
            panel.SetActive(false);

        }
    }


}