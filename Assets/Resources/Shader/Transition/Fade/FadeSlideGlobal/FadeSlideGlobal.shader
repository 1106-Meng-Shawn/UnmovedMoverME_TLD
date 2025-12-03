Shader "Transition/FadeSlideGlobal"
{
    Properties
    {
        _MainTex ("Main Tex", 2D) = "white" {}
        _SubTex  ("Sub Tex", 2D) = "black" {}

        _MainColor ("Main Color (Fallback/Tint)", Color) = (1,1,1,1)
        _SubColor  ("Sub Color (Fallback/Tint)", Color) = (1,1,1,1)

        _Progress ("Progress", Range(0, 1)) = 0.0
        _SoftEdge ("Soft Edge", Range(0.0, 0.5)) = 0

        _Pivot ("Pivot", Vector) = (0.5,0.5,0,0)
        [Enum(Horizontal,0, Vertical,1, InnerToOuter,2, OuterToInner,3, Ripple,6)]
        _Direction("Direction", Float) = 2

        _RippleFreq ("Ripple Frequency", Range(0,50)) = 10
        _RippleAmp  ("Ripple Amplitude", Range(0,0.1)) = 0.02
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            sampler2D _SubTex;
            float4 _MainTex_TexelSize; // Unity自动提供，用于检测纹理是否存在
            float4 _SubTex_TexelSize;
            float4 _MainColor;
            float4 _SubColor;

            float _Progress;
            float _SoftEdge;
            float _Direction;
            float2 _Pivot;

            float _RippleFreq;
            float _RippleAmp;

            struct appdata {
                float4 vertex : POSITION;
                float2 uv     : TEXCOORD0;
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                float2 uv     : TEXCOORD0;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            // Smoothstep helper
            float smoother(float x)
            {
                x = saturate(x);
                return x*x*(3.0 - 2.0*x);
            }

            // --- 改进的纹理采样函数 ---
            // 逻辑：
            // 1. 如果纹理不存在或alpha为0 → 返回纯色
            // 2. 如果纹理存在且alpha>0 → 返回纹理原色（带alpha），Color仅作为调制
            float4 SampleTex(sampler2D tex, float4 fallbackColor, float2 uv, float4 texelSize)
            {
                // 检查纹理是否有效（TexelSize.z和.w在无纹理时为0）
                bool hasTexture = (texelSize.z > 1.0 && texelSize.w > 1.0);
                
                if (!hasTexture)
                {
                    // 没有纹理，返回纯色
                    return fallbackColor;
                }
                
                // 采样纹理
                float4 texColor = tex2D(tex, uv);
                
                // 如果纹理像素完全透明，返回纯色的透明版本
                if (texColor.a < 0.001)
                {
                    return float4(fallbackColor.rgb, 0.0);
                }
                
                // 有纹理且不透明：纹理颜色 × Color的RGB作为调制，保持纹理alpha
                return float4(texColor.rgb * fallbackColor.rgb, texColor.a * fallbackColor.a);
            }

            float getTransition(float2 uv)
            {
                float val = 0.0;
                float dist = distance(uv, _Pivot);

                if (_Direction < 0.5) // Horizontal
                    val = (_Pivot.x < 0.5) ? uv.x : 1.0 - uv.x;
                else if (_Direction < 1.5) // Vertical
                    val = (_Pivot.y < 0.5) ? uv.y : 1.0 - uv.y;
                else if (_Direction < 2.5) // Inner → Outer
                    val = dist / 0.707;
                else if (_Direction < 3.5) // Outer → Inner
                    val = 1.0 - dist / 0.707;
                else // Ripple
                    val = dist / 0.707 + sin(dist * _RippleFreq - _Progress * 10.0) * _RippleAmp;

                return saturate(val);
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float transitionVal = getTransition(i.uv);

                float4 mainCol = SampleTex(_MainTex, _MainColor, i.uv, _MainTex_TexelSize);
                float4 subCol  = SampleTex(_SubTex,  _SubColor,  i.uv, _SubTex_TexelSize);

                if (_Progress <= 0.0) return mainCol;
                if (_Progress >= 1.0) return subCol;

                float mask = 1.0 - smoothstep(_Progress - _SoftEdge, _Progress + _SoftEdge, transitionVal);

                // 在两个颜色之间插值
                float4 outCol = lerp(mainCol, subCol, mask);

                return outCol;
            }

            ENDCG
        }
    }
}