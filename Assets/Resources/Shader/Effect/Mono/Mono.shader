Shader "Effect/Mono"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _MonoAmount ("Mono Amount", Range(0,1)) = 0.5    // ? ??
        _ColorMul ("Color Multiplier", Color) = (1,1,1,1)
        _ColorAdd ("Color Offset", Vector) = (0,0,0,0)
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

            sampler2D _MainTex;
            float _MonoAmount;        // ? ??
            float4 _ColorMul;
            float4 _ColorAdd;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float4 col = tex2D(_MainTex, i.uv) * i.color;

                // ???
                float gray = dot(col.rgb, float3(0.299, 0.587, 0.114));
                float4 mono = float4(gray, gray, gray, col.a);

                // ??????
                mono = mono * _ColorMul + _ColorAdd;

                // ????????
                col = lerp(col, mono, _MonoAmount);

                return col;
            }
            ENDCG
        }
    }
}
