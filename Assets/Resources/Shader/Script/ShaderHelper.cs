using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#region ShaderConstants

public enum SpeedLineType
{
    AnimeSpeedLine, SpeedLine1, SpeedLine2
}

public static class ShaderConstants
{
    public const float ShaderDuration = 3f;
    public const float CurtainPullDuration = 0.1f;
    public const string TransitionPath = "Shader/Transition";
    public const string EffectPath = "Shader/Effect";
    public const string CameraPath = "Shader/Camera";


    public const string Fade = "Fade";
    public const string Grid = "Grid";
    public const string Blur = "Blur";
    public const string LerpSlicesClock = "LerpSlicesClock";
    public const string Water = "Water";
    public const string Barrel = "Barrel";
}


public static class ShaderProperty
{
    public const string Progress = "_Progress";
    public const string MainTex = "_MainTex";
    public const string SubTex = "_SubTex";

    public const string MainColor = "_MainColor";
    public const string SubColor = "_SubColor";
    public const string Color = "_Color";


    public const string Shape = "_SHAPE";
    public const string TransitionColor = "_TransitionColor";
    public const string TransitionWidth = "_TransitionWidth";

    public const string SoftEdge = "_SoftEdge";
    public const string CurveStrength = "_CurveStrength";
    public const string Pivot = "_Pivot";
    public const string Direction = "_Direction";

    public const string GridXY = "_GridXY";

    public const string Count = "_Count";
    public const string Height = "_Height";
    public const string Size = "_Size";

    public const string Rotation = "_Rotation";

    public const string ScaleXY = "_ScaleXY";

    public const string IsInvert = "_IsInvert";
    public const string IsTimeDifference = "_IsTimeDifference";

    public const string IsPixelt = "_IsPixelt";
    public const string IsClockwise = "_IsClockwise";

    public const string Zoom = "_Zoom";
    public const string Pan = "_Pan";

}


public static class ShaderPropertyValue
{
    public const float Rectangular = 0f;
    public const float Doors = 1f;
    public const float Radius = 2f;

    public static readonly Vector4 Center = new Vector4(0.5f, 0.5f, 0, 0);

    public const float Horizontal = 0;
    public const float Vertical = 1;
    public const float InnerToOuter = 2;
    public const float OuterToInner = 3;
    public const float Ripple = 6;

    public const float True = 1f;
    public const float False = 0f;

}


#region  Shader Path


public static class CameraShader
{
    public static readonly string ZoomAndPan = "Camera/ZoomAndPan";
    public static readonly string ZoomAndPanPath = $"{ShaderConstants.CameraPath}/ZoomAndPan/ZoomAndPan";

}
public static class TransitionShader
{
    public static readonly string BlinkEyePath = $"{ShaderConstants.TransitionPath}/BlinkEye/BlinkEye";
    public static readonly string CirclePath = $"{ShaderConstants.TransitionPath}/Circle/Circle";
    public static readonly string CurtainPullPath = $"{ShaderConstants.TransitionPath}/CurtainPull/CurtainPull";
    public static readonly string FadePath = $"{ShaderConstants.TransitionPath}/{ShaderConstants.Fade}/Fade/Fade";
    public static readonly string FadeSlideGlobalPath = $"{ShaderConstants.TransitionPath}/{ShaderConstants.Fade}/FadeSlideGlobal/FadeSlideGlobal";
    public static readonly string FadeSlideWavePath = $"{ShaderConstants.TransitionPath}/{ShaderConstants.Fade}/FadeSlideWave/FadeSlideWave";
    public static readonly string GridComplexPath = $"{ShaderConstants.TransitionPath}/{ShaderConstants.Grid}/GridComplex/GridComplex";
    public static readonly string GridTimePath = $"{ShaderConstants.TransitionPath}/{ShaderConstants.Grid}/GridTime/GridTime";
    public static readonly string PolygonShrinkPath = $"{ShaderConstants.TransitionPath}/PolygonShrink/PolygonShrink";
    public static readonly string ClockWipePath = $"{ShaderConstants.TransitionPath}/{ShaderConstants.LerpSlicesClock}/ClockWipe/ClockWipe";
    public static readonly string LerpSlicesPath = $"{ShaderConstants.TransitionPath}/{ShaderConstants.LerpSlicesClock}/LerpSlices/LerpSlices";
    public static readonly string SawtoothPath = $"{ShaderConstants.TransitionPath}/Sawtooth/Sawtooth";
    public static readonly string SplitPath = $"{ShaderConstants.TransitionPath}/Split/Split";
    public static readonly string WavePath = $"{ShaderConstants.TransitionPath}/Wave/Wave";
}

public static class EffectShader
{
    public static readonly string BarrelPath = $"{ShaderConstants.EffectPath}/{ShaderConstants.Barrel}/Barrel/Barrel";
    public static readonly string BarrelHyperPath = $"{ShaderConstants.EffectPath}/{ShaderConstants.Barrel}/BarrelHyper/BarrelHyper";

    public static readonly string GaussianBlurPath = $"{ShaderConstants.EffectPath}/{ShaderConstants.Blur}/GaussianBlur/GaussianBlur";
    public static readonly string LensBlurPath = $"{ShaderConstants.EffectPath}/{ShaderConstants.Blur}/LensBlur/LensBlur";
    public static readonly string MotionBlurPath = $"{ShaderConstants.EffectPath}/{ShaderConstants.Blur}/MotionBlur/MotionBlur";
    public static readonly string RadialBlurPath = $"{ShaderConstants.EffectPath}/{ShaderConstants.Blur}/RadialBlur/RadialBlur";
    public static readonly string RotationBlurPath = $"{ShaderConstants.EffectPath}/{ShaderConstants.Blur}/RotationBlur/RotationBlur";


    public static readonly string BrokenTVPath = $"{ShaderConstants.EffectPath}/BrokenTV/BrokenTV";

    public static readonly string GlowPath = $"{ShaderConstants.EffectPath}/Glow/Glow";
    public static readonly string KaleidoPath = $"{ShaderConstants.EffectPath}/Kaleido/Kaleido";
    public static readonly string MonoPath = $"{ShaderConstants.EffectPath}/Mono/Mono";

    public static readonly string OverglowPath = $"{ShaderConstants.EffectPath}/Overglow/Overglow";

    public static readonly string RandRollPath = $"{ShaderConstants.EffectPath}/RandRoll/RandRoll";
    public static readonly string RippleMovePath = $"{ShaderConstants.EffectPath}/RippleMove/RippleMove";

    public static readonly string ScreenFlickeringPath = $"{ShaderConstants.EffectPath}/ScreenFlickering/ScreenFlickering";
    public static readonly string ShakePath = $"{ShaderConstants.EffectPath}/Shake/Shake";

    public static readonly string AnimeSpeedLinePath = $"{ShaderConstants.EffectPath}/SpeedLine/AnimeSpeedLine/AnimeSpeedLine";
    public static readonly string SpeedLine1Path = $"{ShaderConstants.EffectPath}/SpeedLine/SpeedLine1/SpeedLine1";
    public static readonly string SpeedLine2Path = $"{ShaderConstants.EffectPath}/SpeedLine/SpeedLine2/SpeedLine2";

    public static readonly string SpiralBackgroundPath = $"{ShaderConstants.EffectPath}/SpiralBackground/SpiralBackground";

    public static readonly string OceanPath = $"{ShaderConstants.EffectPath}/{ShaderConstants.Water}/Ocean/Ocean";
    public static readonly string RainPath = $"{ShaderConstants.EffectPath}/{ShaderConstants.Water}/Rain/Rain";
    public static readonly string WaterPath = $"{ShaderConstants.EffectPath}/{ShaderConstants.Water}/Water/Water";
    public static readonly string WaterAdvancedPath = $"{ShaderConstants.EffectPath}/{ShaderConstants.Water}/WaterAdvanced/WaterAdvanced";


}

#endregion


#endregion

#region ShaderAnimationRequest
public struct ShaderAnimationRequest
{
    public string ShaderName;
    public Image TargetImage;
    public Sprite TargetSprite;
    public float AnimationDuration;
    public Dictionary<string, object> CustomParams;

    public ShaderAnimationRequest(string shaderName, Image targetImage, Sprite targetSprite, float duration = ShaderConstants.ShaderDuration,
        Dictionary<string, object> customParams = null)
    {
        ShaderName = shaderName;
        TargetImage = targetImage;
        TargetSprite = targetSprite;
        AnimationDuration = duration;
        CustomParams = customParams;
    }
}
#endregion

public static class ShaderHelper
{
    #region Public Interface

    public static Material CreateMaterial(ShaderAnimationRequest req)
    {
        Shader shader = Resources.Load<Shader>(req.ShaderName);
        if (shader == null)
        {
            Debug.LogError("Shader not found: " + req.ShaderName);
            return null;
        }
        req.TargetImage.material = new Material(shader);
        Material mat = req.TargetImage.material;

        if (mat.HasProperty(ShaderProperty.MainTex))
        {
            Texture mainTexture = req.TargetImage.sprite != null
                ? req.TargetImage.sprite.texture
                : GetTransparentTexture();
            mat.SetTexture(ShaderProperty.MainTex, mainTexture);
        }

        if (mat.HasProperty(ShaderProperty.SubTex))
        {
            Texture subTexture = req.TargetSprite != null
            ? req.TargetSprite.texture
            : GetTransparentTexture();
            mat.SetTexture(ShaderProperty.SubTex, subTexture);

        }

        if (mat.HasProperty(ShaderProperty.MainColor))
            mat.SetColor(ShaderProperty.MainColor, req.TargetImage.color);

        if (mat.HasProperty(ShaderProperty.SubColor))
            mat.SetColor(ShaderProperty.SubColor, req.TargetImage.color);

        ApplyCustomParams(mat, req.CustomParams);
        return mat;
    }
    public static void TransitionAnimation(ShaderAnimationRequest req, System.Action onComplete = null)
    {
        if (req.TargetImage == null)
        {
            Debug.LogWarning("TargetImage is null.");
            return;
        }

        Material mat = CreateMaterial(req);
        if (mat == null) return;

        TweenProgress(mat, req.AnimationDuration, 1f, () =>
        {
            SetImageMaterial(req.TargetImage, req.TargetSprite);
            onComplete?.Invoke();
        });
    }

    private static Texture2D GetTransparentTexture()
    {
        Texture2D transparentTexture  = new Texture2D(1, 1);

        transparentTexture.SetPixel(0, 0, new Color(1, 1, 1, 0)); 
            transparentTexture.Apply();
        return transparentTexture;
    }

    public static void ApplyEffectShader(Image TargetImage, string shaderPath, Dictionary<string, object> customParams = null)
    {

        Shader shader = Resources.Load<Shader>(shaderPath);
        if (shader == null)
        {
            Debug.LogError("Shader not found at path: " + shaderPath);
            return;
        }

        Material mat = new Material(shader);
        TargetImage.material = mat;

        if (mat.HasProperty(ShaderProperty.MainTex)) mat.SetTexture(ShaderProperty.MainTex, TargetImage.sprite.texture);
        if (mat.HasProperty(ShaderProperty.MainColor)) mat.SetColor(ShaderProperty.MainColor, TargetImage.color);
        ApplyCustomParams(mat, customParams);

    }



    public static void ApplyCustomParams(Material mat, Dictionary<string, object> customParams)
    {
        if (mat == null || customParams == null)
            return;

        foreach (var kv in customParams)
        {
            string prop = kv.Key;
            object val = kv.Value;

            if (!mat.HasProperty(prop))
            {
                Debug.LogWarning("Material has no property: " + prop);
                continue;
            }

            switch (val)
            {
                case float f:
                    mat.SetFloat(prop, f);
                    break;

                case int i:
                    mat.SetFloat(prop, i);
                    break;

                case Vector2 v2:
                    mat.SetVector(prop, v2);
                    break;

                case Vector3 v3:
                    mat.SetVector(prop, v3);
                    break;

                case Vector4 v4:
                    mat.SetVector(prop, v4);
                    break;

                case Color color:
                    mat.SetColor(prop, color);
                    break;

                case Texture tex:
                    mat.SetTexture(prop, tex);
                    break;

                default:
                    Debug.LogWarning("Unsupported param type for: " + prop);
                    break;
            }
        }
    }


    #endregion

    #region Utility Functions

    public static void SetImageMaterial(Image targetImage, Sprite targetSprite)
    {
        if (targetImage == null) return;

        targetImage.material = null;
        targetImage.sprite = targetSprite;
    }


    private static void TweenProgress(Material mat, float duration, float target = 1f, System.Action onComplete = null)
    {
        if (!mat.HasProperty(ShaderProperty.Progress)) return;

        mat.SetFloat(ShaderProperty.Progress, 0f);
        DOTween.To(() => mat.GetFloat(ShaderProperty.Progress),
                   x => mat.SetFloat(ShaderProperty.Progress, x),
                   target,
                   duration)
               .SetEase(Ease.Linear)
               .OnComplete(() => onComplete?.Invoke());
    }


    #endregion

    #region Camera 
    public static void AnimateZoom(Image targetImage, float targetZoom, Vector2? targetPan = null,
                                   float duration = 1f, System.Action onComplete = null)
    {
        if (targetImage == null) return;

        Material mat = EnsureZoomAndPanMaterial(targetImage);
        if (mat == null) return;

        Sequence seq = DOTween.Sequence();

        if (mat.HasProperty(ShaderProperty.Zoom))
        {
            Tween zoomTween = DOTween.To(
                () => mat.GetFloat(ShaderProperty.Zoom),
                x => mat.SetFloat(ShaderProperty.Zoom, x),
                targetZoom,
                duration
            ).SetEase(Ease.Linear);

            seq.Join(zoomTween);
        }

        if (targetPan.HasValue && mat.HasProperty(ShaderProperty.Pan))
        {
            Tween panTween = DOTween.To(
                () => (Vector2)mat.GetVector(ShaderProperty.Pan),
                x => mat.SetVector(ShaderProperty.Pan, x),
                targetPan.Value,
                duration
            ).SetEase(Ease.Linear);

            seq.Join(panTween);
        }

        seq.OnComplete(() => onComplete?.Invoke());
    }

    public static void AnimatePan(Image targetImage, Vector2 targetPan, float duration = 1f, System.Action onComplete = null)
    {
        if (targetImage == null) return;

        Material mat = EnsureZoomAndPanMaterial(targetImage);

        if (!mat.HasProperty(ShaderProperty.Pan))
        {
            onComplete?.Invoke();
            return;
        }

        DOTween.To(
            () => (Vector2)mat.GetVector(ShaderProperty.Pan),
            panValue => mat.SetVector(ShaderProperty.Pan, panValue),
            targetPan,
            duration
        )
        .SetEase(Ease.Linear)
        .OnComplete(() => onComplete?.Invoke());
    }


    private static Material EnsureZoomAndPanMaterial(Image targetImage)
    {
        if (targetImage.material != null && targetImage.material.shader.name == CameraShader.ZoomAndPan)
        {
            return targetImage.material;
        }

        Shader shader = Resources.Load<Shader>(CameraShader.ZoomAndPanPath);
        if (shader == null)
        {
            Debug.LogError("ZoomAndPan Shader not found!");
            return null;
        }

        Material mat = new Material(shader);
        if (targetImage.sprite != null)
            mat.SetTexture(ShaderProperty.MainTex, targetImage.sprite.texture);

        targetImage.material = mat;
        return mat;
    }

    #endregion

    #region Effect Shaders

    public static void SetSpeedLine(Image targetImage,
                                    SpeedLineType speedLineType = SpeedLineType.AnimeSpeedLine,
                                    float duration = ShaderConstants.ShaderDuration,
                                    Dictionary<string, object> customParams = null,
                                    System.Action onComplete = null)
    {
        if (targetImage == null)
        {
            Debug.LogWarning("TargetImage is null.");
            return;
        }

        string shaderPath = speedLineType switch
        {
            SpeedLineType.AnimeSpeedLine => EffectShader.AnimeSpeedLinePath,
            SpeedLineType.SpeedLine1 => EffectShader.SpeedLine1Path,
            SpeedLineType.SpeedLine2 => EffectShader.SpeedLine2Path,
            _ => EffectShader.AnimeSpeedLinePath
        };

        ShaderAnimationRequest req = new ShaderAnimationRequest(
            shaderPath,
            targetImage,
            null,
            duration,
            customParams
        );

        targetImage.color = Color.white;
        Material mat = CreateMaterial(req);
        if (mat == null) return;
    }

    #endregion

    #region Transition Shaders

    public static void SetCurtainPull(Image targetImage,
                                     float duration = ShaderConstants.CurtainPullDuration,
                                     System.Action onComplete = null)
    {
        if (targetImage == null)
        {
            Debug.LogWarning("TargetImage is null.");
            return;
        }


        var customParams = new Dictionary<string, object>
        {
            { ShaderProperty.TransitionColor, Color.black },
            { ShaderProperty.SoftEdge, 0f },
            { ShaderProperty.Direction, 1f },
            { ShaderProperty.Count, 0f },
            { ShaderProperty.Height, 1.5f }
        };

        ShaderAnimationRequest req = new ShaderAnimationRequest(
            TransitionShader.CurtainPullPath,
            targetImage,
            null,
            duration,
            customParams
        );

        Material mat = CreateMaterial(req);
        if (mat == null) return;

        if (mat.HasProperty(ShaderProperty.MainColor))
        {
            Color mainColor = targetImage.color;
            mainColor.a = 0f;
            mat.SetColor(ShaderProperty.MainColor, mainColor);
        }

        targetImage.color = Color.white;


        TweenProgress(mat, duration, ShaderConstants.CurtainPullDuration, () =>
        {
            onComplete?.Invoke();
        });
    }

    #endregion

    public static void ResetTargetImage(Image TargetImage)
    {
        TargetImage.material = null;
    }


    public static void ResetEffectImage(Image EffectImage)
    {
        EffectImage.sprite = null;
        EffectImage.color = new Color32(255, 255, 255, 0);
        EffectImage.material = null;  
    }
}