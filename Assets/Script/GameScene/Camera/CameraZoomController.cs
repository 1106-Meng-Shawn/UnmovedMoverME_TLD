using UnityEngine;
using System;

public class CameraZoomController : MonoBehaviour
{
    public static Action<float> OnZoomChanged;

    private Camera cam;
    private float lastZoom = -1f;

    void Awake()
    {
        cam = Camera.main;
    }

    void LateUpdate()
    {
        if (cam.orthographicSize != lastZoom)
        {
            lastZoom = cam.orthographicSize;
            OnZoomChanged?.Invoke(lastZoom);
        }
    }
}
