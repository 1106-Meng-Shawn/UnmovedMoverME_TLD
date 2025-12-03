Shader "Effect/BrokenTV"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _T ("Time", Range(0,10)) = 5
        _Roll ("Y Rolling Strength", Range(0,1)) = 0.07
        _ColorOffset ("Color Offset Strength", Range(0,0.5)) = 0.1
        _ScanStrength ("Scanline Strength", Range(0,0.5)) = 0.1
        _NoiseStrength ("Noise Strength", Range(0,0.5)) = 0.05
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

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
                float4 color : COLOR;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float _T;
            float _Roll;
            float _ColorOffset;
            float _ScanStrength;
            float _NoiseStrength;

            // 顶点
            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color;
                return o;
            }

            // 简单随机噪声函数
            float rand(float2 co)
            {
                return frac(sin(dot(co.xy , float2(12.9898,78.233))) * 43758.5453);
            }

            // 平滑噪声
            float snoise(float2 uv)
            {
                float2 i = floor(uv);
                float2 f = frac(uv);
                float a = rand(i);
                float b = rand(i + float2(1.0,0.0));
                float c = rand(i + float2(0.0,1.0));
                float d = rand(i + float2(1.0,1.0));
                float2 u = f*f*(3.0-2.0*f);
                return lerp(a, b, u.x) + (c - a)* u.y * (1.0 - u.x) + (d - b) * u.x * u.y;
            }

            // 强化时间影响
            float powerT(float t)
            {
                t += 1.0;
                t *= t;
                t *= t;
                t -= 1.0;
                return t;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv;
                float pt = powerT(_T);

                // x偏移噪声
                float xOffset = snoise(uv * 20.0 + _T*5.0) * 0.05 * pt;

                // Y滚动
                float yOffset = (_T + xOffset) * pt * _Roll;
                uv.y = frac(uv.y + yOffset);

                // RGB偏移 (色差)
                float rbOffset = xOffset * _ColorOffset * pt;
                float r = tex2D(_MainTex, float2(uv.x - rbOffset, uv.y)).r;
                float g = tex2D(_MainTex, float2(uv.x, uv.y)).g;
                float b = tex2D(_MainTex, float2(uv.x + rbOffset, uv.y)).b;
                float a = tex2D(_MainTex, uv).a;

                float4 col = float4(r,g,b,a);
                col *= i.color;

                // 扫描线
                col.rgb -= sin((uv.y + _T*2.0) * 800.0) * _ScanStrength * col.a;

                // 雪花噪声
                col.rgb += rand(uv*100.0 + _T*10.0) * _NoiseStrength * pt * col.a;

                return saturate(col);
            }
            ENDCG
        }
    }
}
