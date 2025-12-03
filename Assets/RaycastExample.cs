using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class RaycastExample : MonoBehaviour
{
    public GraphicRaycaster raycaster; // ?? Canvas ?? GraphicRaycaster
    public EventSystem eventSystem;    // ?????? EventSystem

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // ??????
        {
            Debug.Log("Click detected");

            // === UI ???? ===
            PointerEventData pointerData = new PointerEventData(eventSystem)
            {
                position = Input.mousePosition
            };

            List<RaycastResult> results = new List<RaycastResult>();
            raycaster.Raycast(pointerData, results);

            if (results.Count > 0)
            {
                foreach (var result in results)
                {
                    Debug.Log($"[UI] Hit: {result.gameObject.name}, Tag: {result.gameObject.tag}");
                }
            }
            else
            {
                Debug.Log("[UI] No UI element hit.");
            }

            // === 2D ?????? ===
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

            Debug.DrawRay(ray.origin, ray.direction * 10f, Color.red, 1f);

            if (hit.collider != null)
            {
                Debug.Log($"[2D] Hit: {hit.collider.name}, Tag: {hit.collider.tag}, Point: {hit.point}");
            }
            else
            {
                Debug.Log("[2D] No 2D collider hit.");
            }
        }
    }
}
