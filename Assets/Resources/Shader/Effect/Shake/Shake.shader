Shader "Effect/Shake"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _Progress ("Shake Strength", Range(0,1)) = 0.5
        _XAmp ("X Amplitude", Float) = 0.01
        _YAmp ("Y Amplitude", Float) = 0.01
        _Freq ("Frequency", Float) = 10.0
        [ToggleUI]_UseExternal ("Use External Shake", Float) = 0
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Transparent" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float _Progress, _XAmp, _YAmp, _Freq, _UseExternal;

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

            // ??????
            float rand(float2 co)
            {
                return frac(sin(dot(co.xy ,float2(12.9898,78.233))) * 43758.5453);
            }

            float2 rand2(float2 co)
            {
                return float2(rand(co), rand(co + 0.5));
            }

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);

                // ????????? Progress
                float shakeTime = (_UseExternal > 0.5) ? _Progress : _Time.y;
                float2 seed = v.vertex.xy * _Freq + shakeTime;

                // [-1,1] ????
                float2 offset = rand2(seed) * 2 - 1;
                o.uv = saturate(v.uv + float2(_XAmp, _YAmp) * offset * _Progress);

                o.color = v.color;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv) * i.color;
                return col;
            }
            ENDCG
        }
    }
}
