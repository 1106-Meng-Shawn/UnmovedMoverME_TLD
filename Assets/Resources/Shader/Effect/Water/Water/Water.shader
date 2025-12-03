Shader "Effect/Water"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BaseColor ("Base Color", Color) = (0.2,0.4,0.8,0.5)
        _HighlightColor ("Highlight Color", Color) = (1,1,1,0.5)
        _WaveSpeed ("Wave Speed", Float) = 1.0
        _WaveAmplitude ("Amplitude", Float) = 0.05
        _WaveFrequency ("Frequency", Float) = 5.0
        _DistortStrength ("Distort Strength", Float) = 0.03
        [Toggle]_UseExternalTime ("Use External Time", Float) = 0
        _ExternalTime ("External Time", Range(1,10)) = 1
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
            float4 _BaseColor;
            float4 _HighlightColor;
            float _WaveSpeed;
            float _WaveAmplitude;
            float _WaveFrequency;
            float _DistortStrength;
            float _UseExternalTime;
            float _ExternalTime;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color;
                return o;
            }

            float noise(float2 uv)
            {
                return frac(sin(dot(uv , float2(12.9898,78.233))) * 43758.5453);
            }

            float wave(float2 uv, float t)
            {
                float h = sin(uv.x * _WaveFrequency + t * _WaveSpeed);
                h += cos(uv.y * _WaveFrequency * 0.5 + t * _WaveSpeed * 1.3);
                h += noise(uv * 5.0) * 0.3;
                return h * _WaveAmplitude;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // ???????????
                float time = (_UseExternalTime > 0.5) ? _ExternalTime : _Time.y;

                // UV ??
                float2 uvDelta = float2(wave(i.uv, time), wave(i.uv * 1.3, time)) * _DistortStrength;

                // ????
                float4 texCol = tex2D(_MainTex, i.uv + uvDelta);

                // ????
                float h = wave(i.uv * 2.0, time);
                float4 color = lerp(_BaseColor, _HighlightColor, saturate(h * 0.5 + 0.5));

                color.rgb *= texCol.rgb;
                color.a *= texCol.a;

                return saturate(color);
            }
            ENDCG
        }
    }
}
