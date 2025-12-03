Shader "NV/MaskedMosaic"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _Mask ("Mask", 2D) = "white" {}
        _T ("Time", Range(0,1)) = 0.0
        _Size ("Block Size", Float) = 4.0
        _Strength ("Mosaic Strength", Float) = 8.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
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

            sampler2D _MainTex, _Mask;
            float4 _MainTex_TexelSize;
            float _T, _Size, _Strength;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }

            // ?????????? uv + ??
            float2 RandomOffset(float2 uv, float time)
            {
                float seed = dot(uv, float2(12.9898, 78.233)) + time * 43758.5453;
                float rndX = frac(sin(seed) * 43758.5453) - 0.5;
                float rndY = frac(sin(seed * 1.31) * 43758.5453) - 0.5;
                return float2(rndX, rndY);
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float4 col = tex2D(_MainTex, i.uv);

                // ?????????
                float2 uvBlock = floor(i.uv * _Size) / _Size + 0.5/_Size;

                // ????
                float2 delta = RandomOffset(uvBlock, _Time.y) * (_Strength / 100.0); // ?????
                float2 uv2 = uvBlock + delta;

                float4 col2 = tex2D(_MainTex, uv2);

                // ????
                float mask = tex2D(_Mask, i.uv).r * _T;

                col = lerp(col, col2, mask) * i.color;

                return col;
            }
            ENDCG
        }
    }
}
