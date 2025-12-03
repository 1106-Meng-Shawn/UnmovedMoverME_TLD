Shader "Effect/Glow"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _T ("Time", Range(0,1)) = 0.5
        _Size ("Size", Float) = 1.0
        _Strength ("Strength", Float) = 1.0
        _GlowCenter ("Glow Center", Vector) = (0.5,0.5,0,0)
        _GlowRadius ("Glow Radius", Float) = 0.3

        [Toggle] _InvertMask ("Invert Mask", Float) = 0   // ? ??? Bool
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            // Bool keyword
            #pragma multi_compile _INVERTMASK_ON _INVERTMASK_OFF

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
            float4 _MainTex_TexelSize;
            float _T;
            float _Size;
            float _Strength;
            float2 _GlowCenter;
            float _GlowRadius;

            float3 SampleGlow(float2 uv, float2 texelSize, float radius)
            {
                float3 sum = float3(0,0,0);
                float weightSum = 0.0;

                for(int x=-1; x<=1; x++)
                {
                    for(int y=-1; y<=1; y++)
                    {
                        float2 offset = float2(x,y) * texelSize * radius;
                        float w = 1.0 / (1.0 + x*x + y*y);
                        sum += tex2D(_MainTex, uv + offset).rgb * w;
                        weightSum += w;
                    }
                }
                return sum / weightSum;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float4 col = tex2D(_MainTex, i.uv) * i.color;

                float3 glow = SampleGlow(i.uv, _MainTex_TexelSize.xy, _Size * _T) * i.color.rgb;
                glow *= glow;
                glow *= glow;
                glow *= _Strength * _T;
                glow = saturate(glow);

                float dist = distance(i.uv, _GlowCenter);

                //------------------------------------------------------
                //      Bool ???? (Toggle)
                //------------------------------------------------------
                float mask;
                #if defined(_INVERTMASK_ON)
                    // ??????
                    mask = smoothstep(0.0, _GlowRadius, dist);
                #else
                    // ??????
                    mask = smoothstep(_GlowRadius, 0.0, dist);
                #endif
                //------------------------------------------------------

                col.rgb = col.rgb + glow * mask - col.rgb * glow * mask;
                return col;
            }
            ENDCG
        }
    }
}
