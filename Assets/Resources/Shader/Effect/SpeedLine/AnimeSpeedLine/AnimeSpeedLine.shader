Shader "Effect/AnimeSpeedLine"
{
    Properties
    {
        _MainTex("Main Tex", 2D) = "black" {}
        _MainColor ("Main Color (Fallback/Tint)", Color) = (1,1,1,0)

        _Color("Line Color", Color) = (0,0,0,1)
        _Tile("Tiling", Float) = 200
        _Radial("Radial Scale", Range(0, 10)) = 0.1
        _Power("Noise Power", Float) = 1
        _Remap("Noise Remap", Range(0, 1)) = 0.8
        _MaskScale("Mask Scale", Range(0, 2)) = 1
        _MaskHard("Mask Hardness", Range(0, 1)) = 0
        _MaskPow("Mask Power", Float) = 5
        _Pivot("Pivot", Vector) = (0.5, 0.5, 0, 0)
        _Anim("Animation", Float) = 3
        [ToggleUI]_SelfTime("Self Time", Float) = 1
        _AnimationProgress("Animation Progress", Float) = 0
    }
    
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        ZTest Always
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            float4 _MainColor;
            
            float4 _Color;
            float4 _Pivot;
            float _Radial;
            float _Tile;
            float _Anim;
            float _Power;
            float _Remap;
            float _MaskScale;
            float _MaskHard;
            float _MaskPow;
            float _SelfTime;
            float _AnimationProgress;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float4 SampleTex(sampler2D tex, float4 fallbackColor, float2 uv, float4 texelSize)
            {
                bool hasTexture = (texelSize.z >= 1.0 && texelSize.w >= 1.0);
    
                if (!hasTexture)
                {
                    return fallbackColor;
                }
    
                float4 texColor = tex2D(tex, uv);
                return texColor * fallbackColor;
            }
            float3 mod2D289(float3 x) { return x - floor(x * (1.0 / 289.0)) * 289.0; }
            float2 mod2D289(float2 x) { return x - floor(x * (1.0 / 289.0)) * 289.0; }
            float3 permute(float3 x) { return mod2D289(((x * 34.0) + 1.0) * x); }
            
            float snoise(float2 v)
            {
                const float4 C = float4(0.211324865405187, 0.366025403784439,
                                        -0.577350269189626, 0.024390243902439);
                float2 i = floor(v + dot(v, C.yy));
                float2 x0 = v - i + dot(i, C.xx);
                float2 i1 = (x0.x > x0.y) ? float2(1.0, 0.0) : float2(0.0, 1.0);
                float4 x12 = x0.xyxy + C.xxzz;
                x12.xy -= i1;
                i = mod2D289(i);
                float3 p = permute(permute(i.y + float3(0.0, i1.y, 1.0)) + i.x + float3(0.0, i1.x, 1.0));
                float3 m = max(0.5 - float3(dot(x0, x0), dot(x12.xy, x12.xy), dot(x12.zw, x12.zw)), 0.0);
                m = m * m; m = m * m;
                float3 x = 2.0 * frac(p * C.www) - 1.0;
                float3 h = abs(x) - 0.5;
                float3 ox = floor(x + 0.5);
                float3 a0 = x - ox;
                m *= 1.79284291400159 - 0.85373472095314 * (a0 * a0 + h * h);
                float3 g;
                g.x = a0.x * x0.x + h.x * x0.y;
                g.yz = a0.yz * x12.xz + h.yz * x12.yw;
                return 130.0 * dot(m, g);
            }

            //---------------------------------------------
            // Fragment Shader
            //---------------------------------------------
            fixed4 frag(v2f i) : SV_Target
            {
                float4 sceneColor = SampleTex(_MainTex, _MainColor, i.uv, _MainTex_TexelSize);
                
                float t = (_SelfTime > 0.5) ? _Time.y : _AnimationProgress;
                float2 uv = i.uv - _Pivot.xy;
                
                float2 radialUV = float2(
                    length(uv) * _Radial * 2.0,
                    atan2(uv.x, uv.y) * (1.0 / 6.28318548202515) * _Tile
                );
                
                float2 offset = float2(-_Anim * t, 0.0);
                float n = snoise(radialUV + offset);
                n = n * 0.5 + 0.5;
                float lines = saturate((pow(n, _Power) - _Remap) / (1.0 - _Remap));
                
                float2 centered = i.uv * 2 - 1;
                float lerpMask = lerp(0.0, _MaskScale, _MaskHard);
                float mask = pow((1.0 - saturate((length(centered) - _MaskScale)
                         / ((lerpMask - 0.001) - _MaskScale))), _MaskPow);
                
                float final = lines * mask;
                
                float4 lineColor = float4(_Color.rgb, _Color.a * final);
                
                float4 result;
                result.rgb = lineColor.rgb * lineColor.a + sceneColor.rgb * (1.0 - lineColor.a);
                result.a = lineColor.a + sceneColor.a * (1.0 - lineColor.a);
                
                return result;
            }
            ENDCG
        }
    }
}