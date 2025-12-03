Shader "Effect/Radial Blur"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _BlurAmount ("Blur Amount", Range(0.0, 1.0)) = 0.5
        _Size ("Size", Float) = 1.0
        _Offset ("Offset", Float) = 0.0
        _Center ("Blur Center", Vector) = (0.5, 0.5, 0, 0)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Transparent" }
        ZTest Always
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            float _BlurAmount;
            float _Size;
            float _Offset;
            float4 _Center;

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

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }

            // 计算径向模糊
            float4 RadialBlur(sampler2D tex, float2 uv, float2 uvShift, float strength, int samples)
            {
                float4 col = float4(0,0,0,0);
                float total = 0.0;

                for (int i = 0; i < samples; i++)
                {
                    float t = i / float(samples - 1);
                    float2 sampleUV = uv - uvShift * strength * t;
                    col += tex2D(tex, clamp(sampleUV, 0.0, 1.0));
                    total += 1.0;
                }
                return col / total;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uvShift = i.uv - _Center.xy; 
                int samples = 8;

                float4 col = RadialBlur(_MainTex, i.uv, uvShift, _Size * _BlurAmount, samples);
                col *= i.color;

                col.rgb += _Offset * length(uvShift) * _BlurAmount;
                col.rgb = saturate(col.rgb);

                return col;
            }
            ENDCG
        }
    }
}
