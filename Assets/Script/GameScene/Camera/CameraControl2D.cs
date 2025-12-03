using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.EventSystems;
using static ShortcutKeyCodeString;
public class CameraConstants
{
    public static Vector3 CameraInitPosition = new Vector3(-11f, 11, -10);
    public static float CameraInitZoom = 15f;
    public const float CameraDuration = 0.1f;
    public const float CameraSaveDuration = 0.85f;
    public const float CameraMoveDuration = 0.25f;
}

public class CameraControl2D : MonoBehaviour
{
    #region Inspector Fields

    public float zoomSpeed = 2f;
    public float MaxZoomSize = 15f;
    public float MinZoomSize = 2f;
    public float moveSpeed = 0.1f;
    public float keyboardMoveSpeed = 5f;
    public float moveDistance = 2f;

    public Renderer top;
    public Renderer bottom;
    public Renderer left;
    public Renderer right;

    public LayerMask validLayerMask;
    public GameObject Map;

    #endregion

    #region Camera Action Enum

    public enum CameraAction
    {
        None,
        MoveUp,
        MoveDown,
        MoveLeft,
        MoveRight,
        ZoomIn,
        ZoomOut,
        ResetView
    }

    #endregion

    #region Private Fields

    private Camera cam;
    private Vector3 lastMousePosition;
    private bool isDragging = false;
    private float initialOrthographicSize;
    private Vector3 currentVelocity = Vector3.zero;
    private Vector3 lastMouseScreen;

    // Hotkey system
    private Dictionary<KeyCode, CameraAction> hotkeyDictionary = new Dictionary<KeyCode, CameraAction>();

    #endregion

    #region Events

    public static event Action<float> OnZoomChanged;

    #endregion

    #region Singleton

    public static CameraControl2D Instance { get; private set; }

    #endregion

    #region Unity Lifecycle

    private void Awake()
    {
        InitializeSingleton();
        InitializeCamera();
    }

    private void Start()
    {
        RegisterEventListeners();
        UpdateHotkeyDictionary();
    }

    private void Update()
    {
        if (IsMouseOverValidObject() && IsMapShow())
        {
            HandleMouseDrag();
            HandleZoom();
            HandleHotkeyInput();
        }

        ClampCameraPosition();
    }

    private void OnDestroy()
    {
        UnregisterEventListeners();
    }

    #endregion

    #region Initialization

    private void InitializeSingleton()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void InitializeCamera()
    {
        cam = Camera.main;
        initialOrthographicSize = cam.orthographicSize;
        CameraData initCameraData = new CameraData();
        ClampCameraPosition();
    }

    #endregion

    #region Event Management

    private void RegisterEventListeners()
    {
        if (SettingsManager.Instance != null)
        {
            GameShortcutkeyData shortcutData = SettingValue.Instance.settingValueData?.settingShortcutkeyData?.GameShortcutkeyData;
            if (shortcutData != null)
            {
                shortcutData.OnShortcutKeysChanged += UpdateHotkeyDictionary;
                Debug.Log("[CameraControl2D] Successfully subscribed to OnShortcutKeysChanged event");
            }
            else
            {
                Debug.LogWarning("[CameraControl2D] GameShortcutkeyData is null, cannot register event listener");
            }
        }
        else
        {
            Debug.LogWarning("[CameraControl2D] SettingsManager.Instance is null, cannot register event listener");
        }
    }

    private void UnregisterEventListeners()
    {
        GameShortcutkeyData shortcutData = SettingValue.Instance?.settingValueData?.settingShortcutkeyData?.GameShortcutkeyData;
        if (shortcutData != null)
        {
            shortcutData.OnShortcutKeysChanged -= UpdateHotkeyDictionary;
            Debug.Log("[CameraControl2D] Unsubscribed from OnShortcutKeysChanged event");
        }
    }

    #endregion

    #region Hotkey Management

    private void UpdateHotkeyDictionary()
    {
        hotkeyDictionary.Clear();

        GameShortcutkeyData shortcutData = SettingValue.Instance?.settingValueData?.settingShortcutkeyData?.GameShortcutkeyData;
        if (shortcutData == null)
        {
            Debug.LogWarning("[CameraControl2D] Cannot update hotkey dictionary - shortcutData is null");
            return;
        }

        // 构建相机动作的快捷键映射
        var actionMappings = new Dictionary<string, CameraAction>
        {
            { "cameraup", CameraAction.MoveUp },
            { "cameradown", CameraAction.MoveDown },
            { "cameraleft", CameraAction.MoveLeft },
            { "cameraright", CameraAction.MoveRight },
            { "camerazoomin", CameraAction.ZoomIn },
            { "camerazoomout", CameraAction.ZoomOut },
        };

        foreach (var mapping in actionMappings)
        {
            KeyCode keyCode = GetKeyCodeForCameraAction(mapping.Key);
            if (keyCode != KeyCode.None)
            {
                hotkeyDictionary[keyCode] = mapping.Value;
            }
        }

        Debug.Log($"[CameraControl2D] Hotkey dictionary updated with {hotkeyDictionary.Count} camera actions");
    }

    private KeyCode GetKeyCodeForCameraAction(string actionKey)
    {
        GameShortcutkeyData shortcutData = SettingValue.Instance?.settingValueData?.settingShortcutkeyData?.GameShortcutkeyData;
        if (shortcutData == null) return KeyCode.None;

        switch (actionKey.ToLower())
        {
            case "cameraup":
                return ToKeyCode(shortcutData.ArrowUpKey); // 或从 shortcutData 获取
            case "cameradown":
                return ToKeyCode(shortcutData.ArrowDownKey);
            case "cameraleft":
                return ToKeyCode(shortcutData.ArrowLeftKey);
            case "cameraright":
                return ToKeyCode(shortcutData.ArrowRightKey);
            case "camerazoomin":
                return ToKeyCode(shortcutData.ZoomInKey);
            case "camerazoomout":
                return ToKeyCode(shortcutData.ZoomOutKey);
            default:return KeyCode.None;
        }
    }

    private void HandleHotkeyInput()
    {
        foreach (var kvp in hotkeyDictionary)
        {
            if (Input.GetKey(kvp.Key))
            {
                ExecuteCameraAction(kvp.Value);
            }
        }
    }

    private void ExecuteCameraAction(CameraAction action)
    {
        switch (action)
        {
            case CameraAction.MoveUp:
                MoveCameraByOffset(Vector3.up * moveDistance);
                break;
            case CameraAction.MoveDown:
                MoveCameraByOffset(Vector3.down * moveDistance);
                break;
            case CameraAction.MoveLeft:
                MoveCameraByOffset(Vector3.left * moveDistance);
                break;
            case CameraAction.MoveRight:
                MoveCameraByOffset(Vector3.right * moveDistance);
                break;
            case CameraAction.ZoomIn:
                ZoomCamera(-0.5f);
                break;
            case CameraAction.ZoomOut:
                ZoomCamera(0.5f);
                break;
            case CameraAction.ResetView:
                ResetCameraView();
                break;
        }

        Debug.Log($"[CameraControl2D] Executed camera action: {action}");
    }

    private void MoveCameraByOffset(Vector3 offset)
    {
        transform.position += offset;
        ClampCameraPosition();
    }

    private void ZoomCamera(float zoomDelta)
    {
        float newZoom = cam.orthographicSize + zoomDelta;
        newZoom = Mathf.Clamp(newZoom, MinZoomSize, MaxZoomSize);

        if (Mathf.Abs(newZoom - cam.orthographicSize) > 0.001f)
        {
            cam.orthographicSize = newZoom;
            OnZoomChanged?.Invoke(newZoom);
        }
    }

    private void ResetCameraView()
    {
        SmoothMoveAndZoom(CameraConstants.CameraInitPosition, CameraConstants.CameraInitZoom);
    }

    #endregion

    #region Camera Control (Original)

    private bool IsMapShow()
    {
        return Map.activeSelf;
    }

    private bool IsMouseOverValidObject()
    {
        Vector3 mouseWorldPosition = cam.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mouseWorldPosition, Vector2.zero, Mathf.Infinity, validLayerMask);

        if (EventSystem.current.IsPointerOverGameObject())
        {
            return false;
        }

        if (hit.collider != null)
        {
            return hit.collider.CompareTag("Map");
        }
        return false;
    }

    private void HandleMouseDrag()
    {
        if (Input.GetMouseButtonDown(0) && IsMouseOverValidObject())
        {
            lastMouseScreen = Input.mousePosition;
            isDragging = true;
        }

        if (Input.GetMouseButton(0) && isDragging)
        {
            Vector3 currentMouseScreen = Input.mousePosition;
            Vector3 screenDelta = currentMouseScreen - lastMouseScreen;

            Vector3 worldDelta = cam.ScreenToWorldPoint(
                new Vector3(currentMouseScreen.x, currentMouseScreen.y, cam.nearClipPlane))
                - cam.ScreenToWorldPoint(
                new Vector3(lastMouseScreen.x, lastMouseScreen.y, cam.nearClipPlane));

            transform.position -= worldDelta * moveSpeed;

            lastMouseScreen = currentMouseScreen;
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }
    }

    private void HandleZoom()
    {
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        float newZoom = cam.orthographicSize - scrollInput * zoomSpeed;
        newZoom = Mathf.Clamp(newZoom, MinZoomSize, MaxZoomSize);

        if (Mathf.Abs(newZoom - cam.orthographicSize) > 0.001f)
        {
            cam.orthographicSize = newZoom;
            OnZoomChanged?.Invoke(newZoom);
        }
    }

    private void ClampCameraPosition()
    {
        float camHeight = cam.orthographicSize;
        float camWidth = camHeight * cam.aspect;

        float minX = left.bounds.max.x + camWidth;
        float maxX = right.bounds.min.x - camWidth;
        float minY = bottom.bounds.max.y + camHeight;
        float maxY = top.bounds.min.y - camHeight;

        float clampedX = Mathf.Clamp(transform.position.x, minX, maxX);
        float clampedY = Mathf.Clamp(transform.position.y, minY, maxY);
        float clampedZ = -10f;

        Vector3 targetPosition = new Vector3(clampedX, clampedY, clampedZ);

        float smoothTime = 0.5f;
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref currentVelocity, smoothTime);
    }

    #endregion

    #region Public Methods

    public static void SetCameraZoom(Camera cam, float newSize)
    {
        cam.orthographicSize = newSize;
        OnZoomChanged?.Invoke(newSize);
    }

    public void SmoothMoveAndZoom(Vector3 targetPosition, float targetZoom, float duration = CameraConstants.CameraDuration)
    {
        StopAllCoroutines();
        StartCoroutine(MoveAndZoomCoroutine(targetPosition, targetZoom, duration));
    }

    private IEnumerator MoveAndZoomCoroutine(Vector3 targetPos, float targetSize, float duration)
    {
        Vector3 startPos = cam.transform.position;
        float startSize = cam.orthographicSize;

        float time = 0f;

        targetPos.z = startPos.z;

        while (time < duration)
        {
            float t = time / duration;
            cam.transform.position = Vector3.Lerp(startPos, targetPos, t);
            cam.orthographicSize = Mathf.Lerp(startSize, targetSize, t);
            OnZoomChanged?.Invoke(cam.orthographicSize);
            time += Time.deltaTime;
            yield return null;
        }

        cam.transform.position = targetPos;
        cam.orthographicSize = targetSize;
        OnZoomChanged?.Invoke(cam.orthographicSize);
    }

    public void SetCameraData(CameraData data, float duration = CameraConstants.CameraSaveDuration)
    {
        if (data == null) return;
        if (data.CameraMove)
        {
            Debug.Log("Camera MOVE");
            StartCoroutine(MoveCameraRoutine(data, duration));
            data.CameraMove = false;
        }
        else
        {
            Debug.Log("Camera NOT MOVE");
            Camera cam = Camera.main;
            cam.transform.position = data.GetPosition();
            cam.orthographicSize = data.CameraZoom;
            OnZoomChanged?.Invoke(cam.orthographicSize);
        }
    }

    private IEnumerator MoveCameraRoutine(CameraData data, float duration)
    {
        Vector3 startPos = cam.transform.position;
        float startZoom = cam.orthographicSize;

        Vector3 targetPos = data.GetPosition();
        targetPos.z = startPos.z;

        float targetZoom = data.CameraZoom;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            cam.transform.position = Vector3.Lerp(startPos, targetPos, t);
            cam.orthographicSize = Mathf.Lerp(startZoom, targetZoom, t);

            OnZoomChanged?.Invoke(cam.orthographicSize);

            yield return null;
        }

        cam.transform.position = targetPos;
        cam.orthographicSize = targetZoom;
        OnZoomChanged?.Invoke(cam.orthographicSize);
    }

    #endregion
}

#region Camera Data

public class CameraData
{
    public float CameraPositionX;
    public float CameraPositionY;
    public float CameraPositionZ;
    public float CameraZoom;
    public bool CameraMove = false;

    public CameraData(bool CameraMove = false)
    {
        this.CameraMove = CameraMove;
        SetPosition(Camera.main.transform.position);
        CameraZoom = Camera.main.orthographicSize;
    }

    public CameraData GetData(bool CameraMove = false)
    {
        Camera mainCam = Camera.main;
        SetPosition(mainCam.transform.position);
        CameraZoom = mainCam.orthographicSize;
        this.CameraMove = CameraMove;
        return this;
    }

    public void SetPosition(Vector3 pos)
    {
        CameraPositionX = pos.x;
        CameraPositionY = pos.y;
        CameraPositionZ = pos.z;
    }

    public Vector3 GetPosition()
    {
        Vector3 pos = new Vector3(CameraPositionX, CameraPositionY, CameraPositionZ);
        return pos;
    }
}

#endregion
