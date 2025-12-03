using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ShaderTest : MonoBehaviour
{
    [Header("Target UI Elements")]
    public Image TargetImage;
    public Image EffectImage;
    public Sprite TargetSprite;

    [Header("Animation Settings")]
    public float AnimationDuration = 3f;

    [Header("Optional Parameters")]

    public bool IsAnimating = false;
    #region Enum and Dictionary

    public enum ShaderEffectType
    {
        // --- Barrel ---
        Barrel,
        BarrelHyper,

        // --- Blur  ---
        GaussianBlur,
        LensBlur,
        MotionBlur,
        RadialBlur,
        RotationBlur,

        // --- TV / Distortion ---
        BrokenTV,

        // --- Glow  ---
        Glow,
        Overglow,

        Kaleido,
        Mono,

        // --- Rand / Ripple ---
        RandRoll,
        RippleMove,

        // --- Screen Effects ---
        ScreenFlickering,
        Shake,

        // --- Speed Lines ---
        AnimeSpeedLine,
        SpeedLine1,
        SpeedLine2,

        // --- Background ---
        SpiralBackground,

        // --- Water / Ocean / Rain  ---
        Ocean,
        Rain,
        Water,
        WaterAdvanced
    }

    public ShaderEffectType CurrentEffectShader = ShaderEffectType.Barrel;


    public enum ShaderTransitionType
    {
        BlinkEye, Circle, CurtainPull, Fade, FadeSlideGlobal, FadeSlideWave,
        GridComplex, GridTime, PolygonShrink, ClockWipe, LerpSlices, Sawtooth,
        Split, Wave
    }

    public ShaderTransitionType CurrentTransitionShader = ShaderTransitionType.Wave;

    private Dictionary<ShaderTransitionType, string> shaderTransitionMap;
    private Dictionary<ShaderEffectType, string> shaderEffectMap;

    #endregion

    private void Awake()
    {
        shaderTransitionMap = new Dictionary<ShaderTransitionType, string>
        {
            { ShaderTransitionType.BlinkEye, TransitionShader.BlinkEyePath },
            { ShaderTransitionType.Circle, TransitionShader.CirclePath },
            { ShaderTransitionType.CurtainPull, TransitionShader.CurtainPullPath },
            { ShaderTransitionType.Fade, TransitionShader.FadePath },
            { ShaderTransitionType.FadeSlideGlobal, TransitionShader.FadeSlideGlobalPath },
            { ShaderTransitionType.FadeSlideWave, TransitionShader.FadeSlideWavePath },
            { ShaderTransitionType.GridComplex, TransitionShader.GridComplexPath },
            { ShaderTransitionType.GridTime, TransitionShader.GridTimePath },
            { ShaderTransitionType.PolygonShrink, TransitionShader.PolygonShrinkPath },
            { ShaderTransitionType.ClockWipe, TransitionShader.ClockWipePath },
            { ShaderTransitionType.LerpSlices, TransitionShader.LerpSlicesPath },
            { ShaderTransitionType.Sawtooth, TransitionShader.SawtoothPath },
            { ShaderTransitionType.Split, TransitionShader.SplitPath },
            { ShaderTransitionType.Wave, TransitionShader.WavePath }
        };

        shaderEffectMap = new Dictionary<ShaderEffectType, string>
        {
            { ShaderEffectType.Barrel, EffectShader.BarrelPath },
            { ShaderEffectType.BarrelHyper, EffectShader.BarrelHyperPath },

            { ShaderEffectType.GaussianBlur, EffectShader.GaussianBlurPath },
            { ShaderEffectType.LensBlur, EffectShader.LensBlurPath },
            { ShaderEffectType.MotionBlur, EffectShader.MotionBlurPath },
            { ShaderEffectType.RadialBlur, EffectShader.RadialBlurPath },
            { ShaderEffectType.RotationBlur, EffectShader.RotationBlurPath },

            { ShaderEffectType.BrokenTV, EffectShader.BrokenTVPath },

            { ShaderEffectType.Glow, EffectShader.GlowPath },
            { ShaderEffectType.Overglow, EffectShader.OverglowPath },
            { ShaderEffectType.Kaleido, EffectShader.KaleidoPath },
            { ShaderEffectType.Mono, EffectShader.MonoPath },

            { ShaderEffectType.RandRoll, EffectShader.RandRollPath },
            { ShaderEffectType.RippleMove, EffectShader.RippleMovePath },

            { ShaderEffectType.ScreenFlickering, EffectShader.ScreenFlickeringPath },
            { ShaderEffectType.Shake, EffectShader.ShakePath },

            { ShaderEffectType.AnimeSpeedLine, EffectShader.AnimeSpeedLinePath },
            { ShaderEffectType.SpeedLine1, EffectShader.SpeedLine1Path },
            { ShaderEffectType.SpeedLine2, EffectShader.SpeedLine2Path },

            { ShaderEffectType.SpiralBackground, EffectShader.SpiralBackgroundPath },

            { ShaderEffectType.Ocean, EffectShader.OceanPath },
            { ShaderEffectType.Rain, EffectShader.RainPath },
            { ShaderEffectType.Water, EffectShader.WaterPath },
            { ShaderEffectType.WaterAdvanced, EffectShader.WaterAdvancedPath },
        };

    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !IsAnimating)PlayShaderAnimation();

        if (Input.GetKeyDown(KeyCode.A) && !IsAnimating)TestGridComplexWithShape(ShaderPropertyValue.Rectangular);
        if (Input.GetKeyDown(KeyCode.W) && !IsAnimating)TestGridComplexWithShape(ShaderPropertyValue.Doors);
        if (Input.GetKeyDown(KeyCode.S) && !IsAnimating)TestGridComplexWithShape(ShaderPropertyValue.Radius);

        if (Input.GetKeyDown(KeyCode.V) && !IsAnimating)ShaderHelper.AnimateZoom(TargetImage, 3f, null, 2f, () => Debug.Log("Zoom done"));
        if (Input.GetKeyDown(KeyCode.B) && !IsAnimating)ShaderHelper.AnimateZoom(TargetImage, 3f, new Vector2(0f, 0f), 2f, () => Debug.Log("Zoom done"));
        if (Input.GetKeyDown(KeyCode.N) && !IsAnimating)ShaderHelper.AnimateZoom(TargetImage, 3f, new Vector2(0.5f, 0.5f), 2f, () => Debug.Log("Zoom done"));
        if (Input.GetKeyDown(KeyCode.M) && !IsAnimating)ShaderHelper.AnimateZoom(TargetImage, 3f, new Vector2(1f, 1f), 2f, () => Debug.Log("Zoom done"));
        if (Input.GetKeyDown(KeyCode.L) && !IsAnimating)ShaderHelper.AnimatePan(TargetImage, new Vector2(0.2f, 0.3f), 2f, () => Debug.Log("Pan done"));

        if (Input.GetKeyDown(KeyCode.U))ShaderHelper.SetSpeedLine(EffectImage, SpeedLineType.AnimeSpeedLine);
        if (Input.GetKeyDown(KeyCode.I) )ShaderHelper.SetSpeedLine(EffectImage, SpeedLineType.SpeedLine1);
        if (Input.GetKeyDown(KeyCode.O) )ShaderHelper.SetSpeedLine(EffectImage, SpeedLineType.SpeedLine2);
        if (Input.GetKeyDown(KeyCode.P))ShaderHelper.SetCurtainPull(EffectImage);
        if (Input.GetKeyDown(KeyCode.R)) ShaderHelper.ResetEffectImage(EffectImage);
        
        if (Input.GetKeyDown(KeyCode.Space)) ShaderHelper.ApplyEffectShader(TargetImage, shaderEffectMap[CurrentEffectShader]);
        if (Input.GetMouseButtonDown(1)) ShaderHelper.ResetTargetImage(TargetImage);

        if (Input.GetKeyDown(KeyCode.G)) ApplyFadeSlideWaveTransition();

    }


    void ApplyFadeSlideWaveTransition()
    {
        Sprite originalSprite = TargetImage.sprite;

        var request = new ShaderAnimationRequest(
            shaderName: TransitionShader.FadeSlideWavePath, // Shader Path
            targetImage: TargetImage,// Shader Path
            targetSprite: TargetSprite, // The Sprite we want to set
                                        // after the transition is here set to "CreationOfEva".
            customParams: new Dictionary<string, object>
            {
                { ShaderProperty.TransitionColor, Color.red },              // The color that appears during the transition
                { ShaderProperty.Pivot, new Vector4(0.5f, 0, 0, 0) },        // Transition starting point (Bottom)
                { ShaderProperty.Direction, ShaderPropertyValue.Vertical },  // Slide direction: vertical
                { ShaderProperty.SoftEdge, 0f },                             // Hard edge (no soft fade)
            },

            duration: 10f // Total transition time, you can custom it
        );

        ShaderHelper.TransitionAnimation(request); // Apply transition animations
    }





    private void PlayShaderAnimation()
    {
        Sprite originalSprite = TargetImage.sprite;
        if (!shaderTransitionMap.TryGetValue(CurrentTransitionShader, out string shaderPath))
        {
            Debug.LogWarning("Shader not found: " + CurrentTransitionShader);
            return;
        }

        IsAnimating = true;

        var request = new ShaderAnimationRequest(
            shaderName: shaderPath,
            targetImage: TargetImage,
            targetSprite: TargetSprite,
            duration: AnimationDuration//,
            //customParams: new Dictionary<string, object>
            //{
            //{ ShaderProperty.Pivot, new Vector4(0, 0, 0, 0) }
            //}
        );

        ShaderHelper.TransitionAnimation(request);

        DOVirtual.DelayedCall(AnimationDuration + 1f, () =>
        {
            IsAnimating = false;
            Debug.Log("Transition Done, Restore to original sprite");
            TargetImage.sprite = originalSprite;
        });
    }



    public void TestGridComplexWithShape(float shape)
    {
        Sprite originalSprite = TargetImage.sprite;
        if (TargetImage == null || TargetSprite == null)
        {
            Debug.LogWarning("TargetImage or TargetSprite is null.");
            return;
        }

        if (!shaderTransitionMap.TryGetValue(ShaderTransitionType.GridComplex, out string shaderPath))
        {
            Debug.LogWarning("GridComplex shader path not found.");
            return;
        }

        IsAnimating = true;

        var request = new ShaderAnimationRequest(
            shaderName: shaderPath,
            targetImage: TargetImage,
            targetSprite: TargetSprite,
            duration: AnimationDuration,
            customParams: new Dictionary<string, object>
            {
                { ShaderProperty.Shape, (float)shape }  
            }
        );

        ShaderHelper.TransitionAnimation(request);

        DOVirtual.DelayedCall(AnimationDuration, () =>
        {
            IsAnimating = false;
            Debug.Log("GridComplex transition done with shape: " + shape);
            TargetImage.sprite = originalSprite;

        });
    }

}
