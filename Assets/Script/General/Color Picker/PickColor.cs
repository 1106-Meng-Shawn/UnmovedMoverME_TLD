using System;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.UI;

public class PickColor : MonoBehaviour
{
    [Header("HSV Pickers")]
    [SerializeField] private OneDHandler hsvPick;
    [SerializeField] private TwoDHandler rgbPick;

    [Header("RGB Pickers")]
    [SerializeField] private OneDHandler rPick;
    [SerializeField] private OneDHandler gPick;
    [SerializeField] private OneDHandler bPick;
    [SerializeField] private OneDHandler aPick;

    [Header("Input Fields")]
    [SerializeField] private TMP_InputField rInput;
    [SerializeField] private TMP_InputField gInput;
    [SerializeField] private TMP_InputField bInput;
    [SerializeField] private TMP_InputField aInput;
    [SerializeField] private TMP_InputField hexInput;

    [Header("Visual")]
    [SerializeField] private Image colorPreview;
    [SerializeField] private Material rgbMat;
    [SerializeField] private Texture alphaTexture;

    // 内部状态
    private Vector4 hsvVector = new Vector4(0, 1, 1, 1); // H, S, V, A
    private Color currentColor = Color.white;
    private bool isUpdating = false;
    private Action<Color32> onColorChangedCallback;


    // 材质缓存
    private Material rMat;
    private Material gMat;
    private Material bMat;
    private Material aMat;

    private void Awake()
    {
        InitializeMaterials();
        RegisterListeners();
        UpdateAllVisuals();
    }

    private void Start()
    {
        SetColor(Color.black);        
    }

    private void OnDestroy()
    {
        UnregisterListeners();
        DestroyMaterials();
    }

    #region Initialization

    private void InitializeMaterials()
    {
        rMat = CreateRGBAMaterial(1);
        rPick.GetComponent<Image>().material = rMat;

        gMat = CreateRGBAMaterial(2);
        gPick.GetComponent<Image>().material = gMat;

        bMat = CreateRGBAMaterial(3);
        bPick.GetComponent<Image>().material = bMat;

        aMat = CreateRGBAMaterial(4);
        aMat.mainTexture = alphaTexture;
        aPick.GetComponent<Image>().material = aMat;
    }

    private Material CreateRGBAMaterial(int rgbIndex)
    {
        Material mat = new Material(Shader.Find("Color/RGBA"));
        mat.SetFloat("_RgbIndex", rgbIndex);
        return mat;
    }

    private void DestroyMaterials()
    {
        if (rMat != null) Destroy(rMat);
        if (gMat != null) Destroy(gMat);
        if (bMat != null) Destroy(bMat);
        if (aMat != null) Destroy(aMat);
    }

    #endregion

    #region Event Registration

    private void RegisterListeners()
    {
        // HSV 滑块
        hsvPick.onValueChanged.AddListener(OnHueChanged);
        rgbPick.onValueChanged.AddListener(OnSaturationValueChanged);

        // RGB 滑块
        rPick.onValueChanged.AddListener(OnRedSliderChanged);
        gPick.onValueChanged.AddListener(OnGreenSliderChanged);
        bPick.onValueChanged.AddListener(OnBlueSliderChanged);
        aPick.onValueChanged.AddListener(OnAlphaSliderChanged);

        // 输入框
        rInput.onEndEdit.AddListener(OnRedInputChanged);
        gInput.onEndEdit.AddListener(OnGreenInputChanged);
        bInput.onEndEdit.AddListener(OnBlueInputChanged);
        aInput.onEndEdit.AddListener(OnAlphaInputChanged);
        hexInput.onEndEdit.AddListener(OnHexInputChanged);
    }

    private void UnregisterListeners()
    {
        hsvPick.onValueChanged.RemoveListener(OnHueChanged);
        rgbPick.onValueChanged.RemoveListener(OnSaturationValueChanged);

        rPick.onValueChanged.RemoveListener(OnRedSliderChanged);
        gPick.onValueChanged.RemoveListener(OnGreenSliderChanged);
        bPick.onValueChanged.RemoveListener(OnBlueSliderChanged);
        aPick.onValueChanged.RemoveListener(OnAlphaSliderChanged);

        rInput.onEndEdit.RemoveListener(OnRedInputChanged);
        gInput.onEndEdit.RemoveListener(OnGreenInputChanged);
        bInput.onEndEdit.RemoveListener(OnBlueInputChanged);
        aInput.onEndEdit.RemoveListener(OnAlphaInputChanged);
        hexInput.onEndEdit.RemoveListener(OnHexInputChanged);

        onColorChangedCallback = null;
    }

    #endregion

    #region Event Handlers - HSV

    private void OnHueChanged(float hue)
    {
        if (isUpdating) return;
        hsvVector.x = hue;
        ConvertHSVToRGB();
        UpdateAllVisuals();
    }

    private void OnSaturationValueChanged(Vector2 sv)
    {
        if (isUpdating) return;
        hsvVector.y = sv.x; // Saturation
        hsvVector.z = sv.y; // Value
        ConvertHSVToRGB();
        UpdateAllVisuals();
    }
    #endregion

    #region Event Handlers - RGB Sliders

    private void OnRedSliderChanged(float value)
    {
        if (isUpdating) return;
        currentColor.r = value;
        ConvertRGBToHSV();
        UpdateAllVisuals();
    }

    private void OnGreenSliderChanged(float value)
    {
        if (isUpdating) return;
        currentColor.g = value;
        ConvertRGBToHSV();
        UpdateAllVisuals();
    }

    private void OnBlueSliderChanged(float value)
    {
        if (isUpdating) return;
        currentColor.b = value;
        ConvertRGBToHSV();
        UpdateAllVisuals();
    }

    private void OnAlphaSliderChanged(float value)
    {
        if (isUpdating) return;
        currentColor.a = value;
        hsvVector.w = value;
        UpdateAllVisuals();
    }

    #endregion

    #region Event Handlers - Input Fields

    private void OnRedInputChanged(string value)
    {
        if (isUpdating || string.IsNullOrEmpty(value)) return;
        if (float.TryParse(value, out float r))
        {
            currentColor.r = Mathf.Clamp01(r / 255f);
            ConvertRGBToHSV();
            UpdateAllVisuals();
        }
    }

    private void OnGreenInputChanged(string value)
    {
        if (isUpdating || string.IsNullOrEmpty(value)) return;
        if (float.TryParse(value, out float g))
        {
            currentColor.g = Mathf.Clamp01(g / 255f);
            ConvertRGBToHSV();
            UpdateAllVisuals();
        }
    }

    private void OnBlueInputChanged(string value)
    {
        if (isUpdating || string.IsNullOrEmpty(value)) return;
        if (float.TryParse(value, out float b))
        {
            currentColor.b = Mathf.Clamp01(b / 255f);
            ConvertRGBToHSV();
            UpdateAllVisuals();
        }
    }

    private void OnAlphaInputChanged(string value)
    {
        if (isUpdating || string.IsNullOrEmpty(value)) return;
        if (float.TryParse(value, out float a))
        {
            currentColor.a = Mathf.Clamp01(a / 255f);
            hsvVector.w = currentColor.a;
            UpdateAllVisuals();
        }
    }

    private void OnHexInputChanged(string value)
    {
        if (isUpdating || string.IsNullOrEmpty(value)) return;

        string hexString = value.StartsWith("#") ? value : "#" + value;
        if (ColorUtility.TryParseHtmlString(hexString, out Color color))
        {
            currentColor = color;
            currentColor.a = hsvVector.w; // 保持当前 alpha
            ConvertRGBToHSV();
            UpdateAllVisuals();
        }
    }

    #endregion

    #region Color Conversion

    private void ConvertHSVToRGB()
    {
        currentColor = Color.HSVToRGB(hsvVector.x, hsvVector.y, hsvVector.z);
        currentColor.a = hsvVector.w;
    }

    private void ConvertRGBToHSV()
    {
        Color.RGBToHSV(currentColor, out float h, out float s, out float v);
        hsvVector.x = h;
        hsvVector.y = s;
        hsvVector.z = v;
        hsvVector.w = currentColor.a;
    }

    #endregion

    #region Visual Updates

    private void UpdateAllVisuals()
    {
        isUpdating = true;

        // 更新预览颜色
        if (colorPreview != null)
        {
            colorPreview.color = currentColor;
        }

        // 更新 HSV 材质
        if (rgbMat != null)
        {
            rgbMat.SetFloat("_H", hsvVector.x);
        }

        // 更新 RGB 滑块材质的当前颜色
        UpdateRGBSliderMaterials();

        // 更新 HSV 滑块位置
        hsvPick.SetPos(hsvVector.x);
        rgbPick.SetPos(hsvVector.y, hsvVector.z);

        // 更新 RGB 滑块位置
        rPick.SetPos(currentColor.r);
        gPick.SetPos(currentColor.g);
        bPick.SetPos(currentColor.b);
        aPick.SetPos(currentColor.a);

        // 更新输入框文本
        rInput.SetTextWithoutNotify(Mathf.RoundToInt(currentColor.r * 255).ToString());
        gInput.SetTextWithoutNotify(Mathf.RoundToInt(currentColor.g * 255).ToString());
        bInput.SetTextWithoutNotify(Mathf.RoundToInt(currentColor.b * 255).ToString());
        aInput.SetTextWithoutNotify(Mathf.RoundToInt(currentColor.a * 255).ToString());
        hexInput.SetTextWithoutNotify(ColorUtility.ToHtmlStringRGB(currentColor));

        isUpdating = false;

        onColorChangedCallback?.Invoke(currentColor);

    }

    private void UpdateRGBSliderMaterials()
    {
        // 更新 R 滑块：显示从 (0,G,B) 到 (1,G,B) 的渐变
        if (rMat != null)
        {
            rMat.SetColor("_BaseColor", new Color(0, currentColor.g, currentColor.b, 1));
        }

        // 更新 G 滑块：显示从 (R,0,B) 到 (R,1,B) 的渐变
        if (gMat != null)
        {
            gMat.SetColor("_BaseColor", new Color(currentColor.r, 0, currentColor.b, 1));
        }

        // 更新 B 滑块：显示从 (R,G,0) 到 (R,G,1) 的渐变
        if (bMat != null)
        {
            bMat.SetColor("_BaseColor", new Color(currentColor.r, currentColor.g, 0, 1));
        }

        // 更新 A 滑块：显示从透明到当前 RGB 颜色
        if (aMat != null)
        {
            aMat.SetColor("_BaseColor", new Color(currentColor.r, currentColor.g, currentColor.b, 1));
        }
    }

    #endregion

    #region Public API

    public Color GetColor()
    {
        return currentColor;
    }

    public void SetColor(Color color)
    {
        currentColor = color;
        ConvertRGBToHSV();
        UpdateAllVisuals();
    }

    #endregion

    public void SetColor(Color32 defaultColor, System.Action<Color32> onColorChanged)
    {
        SetColor(defaultColor);
        onColorChangedCallback = onColorChanged;
        onColorChangedCallback?.Invoke(defaultColor);
    }

    private void OnDisable()
    {
        onColorChangedCallback = null;
    }
}


