Shader "Effect/GaussianBlur"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _Size ("Blur Size", Range(0, 200)) = 100
        _Offset ("Offset", Float) = 0.0
        _Direction ("Blur Direction", Vector) = (1,0,0,0) // (1,0)=??, (0,1)=??
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
            float4 _MainTex_TexelSize;
            float _Size;
            float _Offset;
            float2 _Direction;

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
                // 9-tap Gaussian weights
                float w0 = 0.05;
                float w1 = 0.09;
                float w2 = 0.12;
                float w3 = 0.15;
                float w4 = 0.16;

                float2 texelOffset = _MainTex_TexelSize.xy * _Size * _Direction;

                fixed4 col = tex2D(_MainTex, i.uv) * w4;
                col += tex2D(_MainTex, i.uv + texelOffset * 1.0) * w3;
                col += tex2D(_MainTex, i.uv - texelOffset * 1.0) * w3;
                col += tex2D(_MainTex, i.uv + texelOffset * 2.0) * w2;
                col += tex2D(_MainTex, i.uv - texelOffset * 2.0) * w2;
                col += tex2D(_MainTex, i.uv + texelOffset * 3.0) * w1;
                col += tex2D(_MainTex, i.uv - texelOffset * 3.0) * w1;
                col += tex2D(_MainTex, i.uv + texelOffset * 4.0) * w0;
                col += tex2D(_MainTex, i.uv - texelOffset * 4.0) * w0;

                // ?????????
                col *= i.color;
                col.rgb += _Offset;
                col.rgb = saturate(col.rgb);

                return col;
            }
            ENDCG
        }
    }
}
