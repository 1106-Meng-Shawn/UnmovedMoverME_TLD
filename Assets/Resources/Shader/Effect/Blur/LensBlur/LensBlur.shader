Shader "Effect/LensBlur"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _BlurAmount ("Blur Amount", Range(0,1)) = 0.5   // ? ??
        _Size ("Blur Size", Float) = 1.0
        _Offset ("Brightness Offset", Float) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Overlay" }
        LOD 100

        Pass
        {
            ZTest Always Cull Off ZWrite Off
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            float _BlurAmount;        // ? ??
            float _Size;
            float _Offset;
            float4 _MainTex_TexelSize;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv;
                float4 col = float4(0,0,0,0);
                int samples = 8;

                // ?? BlurAmount ????
                float radius = _Size * _BlurAmount * 0.05;

                for (int s=0; s<samples; s++)
                {
                    float angle = 6.2831853 * s / samples; // 2*pi
                    float2 offset = float2(cos(angle), sin(angle)) * radius;
                    col += tex2D(_MainTex, uv + offset);
                }

                col /= samples;

                // ??????
                col.rgb += _Offset * _BlurAmount;
                col.rgb = saturate(col.rgb);

                return col;
            }

            ENDCG
        }
    }
}
