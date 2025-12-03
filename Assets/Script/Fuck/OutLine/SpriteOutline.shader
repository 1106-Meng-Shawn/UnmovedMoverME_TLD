// ========================================
// SpriteOutline.shader
// ???? 2D Sprite ?? Shader
// ???? Sprite ?????
// ========================================

Shader "Sprites/Outline"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        
        [Header(Outline Settings)]
        _OutlineColor ("Outline Color", Color) = (1,1,0,1)
        _OutlineWidth ("Outline Width", Range(0, 10)) = 3
        _EnableOutline ("Enable Outline", Float) = 1
        
        [HideInInspector] _TextureSize ("Texture Size", Vector) = (256, 256, 0, 0)
        
        [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
        [HideInInspector] _RendererColor ("RendererColor", Color) = (1,1,1,1)
        [HideInInspector] _Flip ("Flip", Vector) = (1,1,1,1)
        [PerRendererData] _AlphaTex ("External Alpha", 2D) = "white" {}
        [PerRendererData] _EnableExternalAlpha ("Enable External Alpha", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex SpriteVert
            #pragma fragment SpriteFrag
            #pragma target 2.0
            #pragma multi_compile_instancing
            #pragma multi_compile_local _ PIXELSNAP_ON
            #pragma multi_compile _ ETC1_EXTERNAL_ALPHA
            
            #include "UnitySprites.cginc"
            
            // ????
            fixed4 _OutlineColor;
            float _OutlineWidth;
            float _EnableOutline;
            float4 _TextureSize;
            
            // ????????
            fixed4 SpriteFrag(v2f IN) : SV_Target
            {
                // ???? Sprite
                fixed4 c = SampleSpriteTexture(IN.texcoord) * IN.color;
                c.rgb *= c.a;
                
                // ?????????????
                if (_EnableOutline < 0.5)
                    return c;
                
                // ????
                float outlineAlpha = 0;
                
                // ???????????????????????
                if (c.a < 0.1)
                {
                    // ??????????????
                    float2 texelSize = float2(1.0 / _TextureSize.x, 1.0 / _TextureSize.y);
                    float offset = _OutlineWidth * 0.5;
                    
                    // 8????
                    float maxAlpha = 0;
                    
                    for (int x = -1; x <= 1; x++)
                    {
                        for (int y = -1; y <= 1; y++)
                        {
                            if (x == 0 && y == 0) continue; // ?????
                            
                            float2 sampleUV = IN.texcoord + float2(x, y) * texelSize * offset;
                            fixed4 sample = SampleSpriteTexture(sampleUV);
                            maxAlpha = max(maxAlpha, sample.a);
                        }
                    }
                    
                    outlineAlpha = maxAlpha;
                }
                
                // ??????
                if (outlineAlpha > 0.1)
                {
                    fixed4 outlineColor = _OutlineColor;
                    outlineColor.rgb *= outlineColor.a;
                    
                    // ????????????
                    c = lerp(c, outlineColor, outlineAlpha);
                }
                
                return c;
            }
            ENDCG
        }
    }
    
    Fallback "Sprites/Default"
}