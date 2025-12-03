Shader "Effect/ScreenFlickering"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _BaseColor ("Base Color", Color) = (0.2, 0.4, 0.8, 0.5)
        _HighlightColor ("Highlight Color", Color) = (1,1,1,0.5)
        _T ("Time", Range(0.0,1.0)) = 0.5
        _Size ("Size", Float) = 10.0
        _Aspect ("Aspect Ratio", Float) = 1.77778
        _Freq ("Frequency", Float) = 50.0
        _Distort ("Distort", Float) = 5.0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            #define PI 3.1415927

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float2 uvEffect : TEXCOORD1;
                float4 color : COLOR;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Size, _Aspect, _T, _Freq, _Distort;
            float4 _BaseColor, _HighlightColor;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.uvEffect = float2(v.uv.x * _Size * _Aspect, v.uv.y * _Size);
                o.color = v.color;
                return o;
            }

            // 简单噪声
            float noise(float2 x)
            {
                return frac(sin(dot(x , float2(12.9898,78.233))) * 43758.5453);
            }

            float water(float2 uv)
            {
                const float n = 3.1;
                float h = 0.0;
                for (float i = 1.0; i <= n; i += 0.7)
                {
                    float2 p = uv * i * i;
                    p.y += _Freq / i * _Time.y;
                    h += cos(2.0 * PI * noise(p) * _T) / (i * i * i);
                }
                return h;
            }

            float2 waterNormal(float2 uv)
            {
                float2 e = float2(1e-3, 0.0);
                float h0 = water(uv);
                float h1 = water(uv + e);
                float h2 = water(uv + e.yx);
                return float2(h1 - h0, h2 - h0);
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uvDelta = _Distort * waterNormal(i.uvEffect);
                float4 texCol = tex2D(_MainTex, i.uv + uvDelta);

                // 混合 BaseColor 与 HighlightColor，根据波动高度动态叠加
                float h = water(i.uvEffect);
                float4 color = lerp(_BaseColor, _HighlightColor, saturate(h * 0.5 + 0.5));

                // 叠加纹理
                color.rgb *= texCol.rgb;
                color.a *= texCol.a;

                return saturate(color);
            }

            ENDCG
        }
    }
}
