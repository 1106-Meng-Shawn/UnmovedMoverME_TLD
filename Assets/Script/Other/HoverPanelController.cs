using UnityEngine;
using UnityEngine.EventSystems;

public class HoverPanelController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject panelToToggle;
    public HoverPanelController linkedPartner; // 与按钮或面板的另一端链接

    private bool isHovering = false;
    private float hideDelay = 0.05f; // 延迟一点点时间再隐藏，避免短暂跳跃

    private int globalHoverCount = 0; // 用于追踪当前是否在任一组件上

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
        globalHoverCount++;
        CancelInvoke(nameof(HidePanel));
        panelToToggle.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
        globalHoverCount--;
        Invoke(nameof(HidePanel), hideDelay);
    }
    private void HidePanel()
    {
        if (!isHovering && (linkedPartner == null || !linkedPartner.isHovering))
        {
            if (globalHoverCount <= 0)
            {
                globalHoverCount = 0; 
                panelToToggle.SetActive(false);
            }
        }
    }

    void OnDisable()
    {
        HidePanel();

    }

    void OnEnable()
    {
        HidePanel();
    }
}
