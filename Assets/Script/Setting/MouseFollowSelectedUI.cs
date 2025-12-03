using UnityEngine;
using UnityEngine.EventSystems;
using System.Runtime.InteropServices;

public class MouseFollowSelectedUI : MonoBehaviour
{
    [DllImport("user32.dll")]
    static extern bool SetCursorPos(int X, int Y);

    private GameObject lastSelected = null;

    public Camera uiCamera; 
    void Update()
    {
        GameObject selected = EventSystem.current.currentSelectedGameObject;
        if (selected != null && selected != lastSelected)
        {
            MoveCursorToSelected(selected);
            lastSelected = selected;
        }
    }

    void MoveCursorToSelected(GameObject obj)
    {
        RectTransform rect = obj.GetComponent<RectTransform>();
        if (rect == null) return;

        Vector2 screenPos;
        if (uiCamera == null)
        {
            screenPos = RectTransformUtility.WorldToScreenPoint(null, rect.position);
        }
        else
        {
            screenPos = RectTransformUtility.WorldToScreenPoint(uiCamera, rect.position);
        }

        SetCursorPos((int)screenPos.x, (int)(Screen.height - screenPos.y));
    }
}
