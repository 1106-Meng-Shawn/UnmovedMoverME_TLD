Shader "Effect/Overglow"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _GlowAmount ("Glow Amount", Range(0,1)) = 0.5
        _Zoom ("Zoom", Float) = 0.5
        _Mul ("Multiplier", Float) = 0.5
        _Center ("Glow Center", Vector) = (0.5, 0.5, 0, 0)
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
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
            float _GlowAmount;
            float _Zoom;
            float _Mul;
            float2 _Center;

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

                if (_GlowAmount > 0.0 && _GlowAmount < 1.0)
                {
                    // ?????????
                    float2 uv2 = (i.uv - _Center) * (1.0 - _Zoom * _GlowAmount) + _Center;
                    float4 col2 = tex2D(_MainTex, uv2) * i.color;

                    col.rgb += col2.rgb * _Mul * (1.0 - _GlowAmount);
                }
                else if (_GlowAmount >= 1.0)
                {
                    col.rgb *= (1.0 + _Mul);
                }

                col.rgb = saturate(col.rgb);
                col.a *= col.a; // ?????????

                return col;
            }
            ENDCG
        }
    }
}
