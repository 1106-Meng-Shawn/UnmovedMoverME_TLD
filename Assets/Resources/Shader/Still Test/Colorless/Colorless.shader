Shader "NV/Colorless"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _T ("Distortion Amount", Range(0,1)) = 0.0
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

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }

            sampler2D _MainTex;
            float _T;

            // ????
            float Luminance(float3 col)
            {
                return dot(col, float3(0.299, 0.587, 0.114));
            }

            // ?? 2D ??????? [-1,1] ??
            float2 snoise2(float3 v)
            {
                float n = frac(sin(dot(v.xy ,float2(12.9898,78.233))) * 43758.5453);
                float m = frac(sin(dot(v.yz ,float2(12.9898,78.233))) * 43758.5453);
                return float2(n,m) * 2.0 - 1.0;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float4 col = tex2D(_MainTex, i.uv);

                // ????????
                float2 v = 0.001 * snoise2(float3(i.uv * 10.0, _Time.y));

                // ?? mask
                float gray = Luminance(col.rgb);
                float mask = gray * gray;
                mask = 2.0 * mask * mask * _T;
                v *= mask;

                // RGB ??
                col.r = tex2D(_MainTex, i.uv + v).r;
                col.g = tex2D(_MainTex, i.uv - v).g;
                // B ??????

                col *= i.color;

                return col;
            }
            ENDCG
        }
    }
}
