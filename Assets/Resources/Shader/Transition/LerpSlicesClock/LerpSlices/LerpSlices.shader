Shader "Transition/LerpSlices"
{
    Properties
    {
        _MainTex ("Main Tex", 2D) = "white" {}
        _SubTex  ("Sub Tex", 2D) = "black" {}
        
        _MainColor ("Main Color (Fallback/Tint)", Color) = (1,1,1,1)
        _SubColor  ("Sub Color (Fallback/Tint)", Color) = (1,1,1,1)
        
        _Progress("Progress", Range(0, 1)) = 0
        
        [Enum(Single,0, Multiple,1)] _Mode("Mode", Float) = 0
        _Count("Count", Int) = 8
        _Rotation("Rotation", Range(0, 360)) = 45
        [ToggleUI]_IsPixelt("Pixelt (mask only)", Float) = 0
        _PixelXY("Pixel XY", Vector) = (40, 40, 0, 0)
        _SoftEdge("Soft Edge", Range(0, 1.0)) = 0.02
        [ToggleUI]_IsClockwise("Clockwise", Float) = 1
    }

    SubShader
    {
        Tags 
        { 
            "RenderType"="Transparent" 
            "Queue"="Transparent"
        }
        
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            sampler2D _SubTex;
            float4 _MainTex_TexelSize;
            float4 _SubTex_TexelSize;
            float4 _MainColor;
            float4 _SubColor;
            
            float _Progress;
            float _Mode; // 0 = Single, 1 = Multiple
            int _Count;
            float _Rotation;
            float _IsPixelt; // applies to mask only
            float2 _PixelXY;
            float _IsClockwise; // 1 = CW, 0 = CCW
            float _SoftEdge;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv  : TEXCOORD0;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv  = v.uv;
                return o;
            }

            //---------------------------------------------
            // 改进的纹理采样函数
            //---------------------------------------------
            float4 SampleTex(sampler2D tex, float4 fallbackColor, float2 uv, float4 texelSize)
            {
                // 检查纹理是否有效
                bool hasTexture = (texelSize.z > 1.0 && texelSize.w > 1.0);
                
                if (!hasTexture)
                {
                    // 没有纹理，返回纯色
                    return fallbackColor;
                }
                
                // 采样纹理
                float4 texColor = tex2D(tex, uv);
                
                // 如果纹理像素完全透明，返回透明
                if (texColor.a < 0.001)
                {
                    return float4(fallbackColor.rgb, 0.0);
                }
                
                // 有纹理且不透明：纹理颜色 × Color调制
                return float4(texColor.rgb * fallbackColor.rgb, texColor.a * fallbackColor.a);
            }

            //---------------------------------------------
            // 像素化 UV（仅用于遮罩）
            //---------------------------------------------
            float2 PixelateUV_ForMask(float2 uv)
            {
                if (_IsPixelt > 0.5)
                {
                    float2 steps = max(_PixelXY, float2(1.0, 1.0));
                    return floor(uv * steps) / steps + (0.5 / steps); 
                }
                return uv;
            }

            //---------------------------------------------
            // UV 旋转
            //---------------------------------------------
            float2 RotateUV(float2 uv, float2 center, float degrees)
            {
                if (abs(degrees) < 0.0001) return uv;
                float rad = radians(degrees);
                float2 p = uv - center;
                float s = sin(rad), c = cos(rad);
                float2 r = float2(c * p.x - s * p.y, s * p.x + c * p.y);
                return r + center;
            }

            //---------------------------------------------
            // 从 UV 计算角度（0~1）
            //---------------------------------------------
            float Angle01_FromUV(float2 uv, float2 center)
            {
                float2 d = uv - center;
                float a = atan2(d.y, d.x);
                float n = (a + UNITY_PI) / (2.0 * UNITY_PI);
                return frac(n);
            }

            //---------------------------------------------
            // Fragment Shader
            //---------------------------------------------
            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv;
                float2 center = float2(0.5, 0.5);

                float4 mainCol = SampleTex(_MainTex, _MainColor, uv, _MainTex_TexelSize);
                float4 subCol = SampleTex(_SubTex, _SubColor, uv, _SubTex_TexelSize);

                if (_Progress <= 0.0) return mainCol;
                if (_Progress >= 1.0) return subCol;

                // 计算遮罩用的 UV
                float2 uv_mask = uv;
                uv_mask = PixelateUV_ForMask(uv_mask);
                uv_mask = RotateUV(uv_mask, center, _Rotation);

                // 计算角度（0~1）
                float angle01 = Angle01_FromUV(uv_mask, center);
                
                // Clockwise: 1 = CW (反转), 0 = CCW (不反转)
                if (_IsClockwise > 0.5) 
                    angle01 = frac(1.0 - angle01);

                // 根据模式计算值
                float value = 0.0;
                if (_Mode < 0.5) // Single
                {
                    value = angle01;
                }
                else // Multiple
                {
                    float n = max(1.0, (float)_Count);
                    float scaled = angle01 * n;
                    value = frac(scaled);
                }

                value = saturate(value);

                // 计算过渡
                float transition = smoothstep(
                    saturate(_Progress - _SoftEdge), 
                    saturate(_Progress + _SoftEdge), 
                    value
                );

                // 在两个颜色之间插值
                return lerp(subCol, mainCol, transition);
            }

            ENDCG
        }
    }

    FallBack "Diffuse"
}