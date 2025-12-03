Shader "Custom/SmoothDoubleOutline"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _OutlineColor ("Outline Color", Color) = (1, 0, 0, 1)
        _OutlineSizeOuter ("Outline Outer Size", Float) = 4.0
        _OutlineSizeInner ("Outline Inner Size", Float) = 2.0
        _AlphaThreshold ("Alpha Threshold", Range(0,1)) = 0.1
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Cull Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _OutlineColor;
            float _OutlineSizeOuter;
            float _OutlineSizeInner;
            float _AlphaThreshold;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            // 获取采样点（16方向，单位向量）
            float2 getOffset(int index)
            {
                float angle = index * 6.2831853 / 16.0; // 2PI
                return float2(cos(angle), sin(angle));
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float centerAlpha = tex2D(_MainTex, i.uv).a;
                if (centerAlpha > _AlphaThreshold)
                    discard;

                float2 px = 1.0 / _ScreenParams.xy;

                float alphaOuter = 0.0;
                float alphaInner = 0.0;

                int samples = 16;

                for (int j = 0; j < samples; j++)
                {
                    float2 dir = getOffset(j);
                    alphaOuter += tex2D(_MainTex, i.uv + dir * px * _OutlineSizeOuter).a;
                    alphaInner += tex2D(_MainTex, i.uv + dir * px * _OutlineSizeInner).a;
                }

                alphaOuter /= samples;
                alphaInner /= samples;

                // 软阈值平滑：过渡更自然
                float outerMask = smoothstep(_AlphaThreshold * 0.9, _AlphaThreshold + 0.05, alphaOuter);
                float innerMask = smoothstep(_AlphaThreshold * 0.9, _AlphaThreshold + 0.05, alphaInner);

                if (outerMask <= 0.01)
                    discard;

                float finalAlpha = outerMask;

                if (innerMask > 0.8)
                    return float4(1, 1, 1, finalAlpha); // 白色内圈
                else
                    return _OutlineColor * finalAlpha;  // 彩色外圈
            }
            ENDCG
        }
    }
}



/*Shader "Custom/CleanDashedOutline"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _OutlineColor ("Outline Color", Color) = (1, 0, 0, 1)
        _OutlineSize ("Outline Size", Float) = 3.0
        _AlphaThreshold ("Alpha Threshold", Range(0,1)) = 0.1
        _DashLength ("Dash Length", Float) = 8.0
        _DashSpacing ("Dash Spacing", Float) = 6.0
        _DashDirection ("Dash Direction", Vector) = (1, 0, 0, 0) // 默认横向
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Cull Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _OutlineColor;
            float _OutlineSize;
            float _AlphaThreshold;
            float _DashLength;
            float _DashSpacing;
            float4 _DashDirection;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float aCenter = tex2D(_MainTex, i.uv).a;
                if (aCenter > _AlphaThreshold)
                    discard;

                // ------ Outline Sampling ------
                float2 offsetOuter = (_OutlineSize * 1.3) / _ScreenParams.xy;
                float2 offsetInner = (_OutlineSize * 0.5) / _ScreenParams.xy;

                float alphaOuter = 0;
                float alphaInner = 0;

                float2 dir[8] = {
                    float2(-1, 0), float2(1, 0),
                    float2(0, -1), float2(0, 1),
                    float2(-1, -1), float2(1, -1),
                    float2(-1, 1), float2(1, 1)
                };

                for (int j = 0; j < 8; j++)
                {
                    alphaOuter += tex2D(_MainTex, i.uv + dir[j] * offsetOuter).a;
                    alphaInner += tex2D(_MainTex, i.uv + dir[j] * offsetInner).a;
                }

                alphaOuter /= 8.0;
                alphaInner /= 8.0;

                if (alphaOuter <= _AlphaThreshold)
                    return float4(0, 0, 0, 0); // 不在描边带

                // ------ 虚线控制 ------
                float2 dashUV = i.pos.xy / _ScreenParams.xy;
                float proj = dot(dashUV, normalize(_DashDirection.xy)); // 方向投影

                float totalCycle = _DashLength + _DashSpacing;
                float dashPos = fmod(proj * 1000.0, totalCycle); // 1000 是比例放大

                if (dashPos > _DashLength)
                    discard; // 空白段

                // ------ 显示颜色 ------
                if (alphaInner > _AlphaThreshold)
                    return float4(1, 1, 1, 1); // 白线
                else
                    return _OutlineColor; // 外圈颜色
            }
            ENDCG
        }
    }
}*/
