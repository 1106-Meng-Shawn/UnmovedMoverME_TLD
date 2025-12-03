Shader "Transition/Circle"
{
    Properties
    {
        _MainTex ("Main Tex", 2D) = "white" {}
        _SubTex ("Sub Tex", 2D) = "white" {}

        _MainColor ("Main Color (Fallback)", Color) = (1,1,1,1)
        _SubColor ("Sub Color (Tint / Fallback)", Color) = (1,1,1,1)

        _Progress ("Progress", Range(0,1)) = 0

        _GridXY ("Grid X/Y", Vector) = (10,10,0,0)
        _Pivot ("Pivot", Vector) = (0.5,0.5,0,0)

        _SoftEdge ("Soft Edge", Range(0,0.5)) = 0

        // 2 = Inner ? Outer, 3 = Outer ? Inner
        [Enum(InnerToOuter,2, OuterToInner,3)]
        _Direction ("Direction", Float) = 2
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            sampler2D _SubTex;

            float4 _MainColor;
            float4 _SubColor;

            float _Progress;

            float2 _GridXY;
            float2 _Pivot;

            float _SoftEdge;

            float _Direction; // 2 or 3

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            //--------- Texture sampling with tint/fallback ---------
            float4 SampleMain(float2 uv)
            {
                float4 t = tex2D(_MainTex, uv);
                return float4(t.rgb * _MainColor.rgb, t.a);
            }

            float4 SampleSub(float2 uv)
            {
                float4 t = tex2D(_SubTex, uv);
                return float4(t.rgb * _SubColor.rgb, t.a);
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv;
    
                if (_Progress <= 0.0)
                    return SampleMain(uv);
                if (_Progress >= 1.0)
                    return SampleSub(uv);
    
                // distance to pivot (circle center factor)
                float distCenter = length(uv - _Pivot) / 0.70710678;
    
                // grid-based distance factor
                float2 gridUV = uv * _GridXY;
                float2 fracUV = frac(gridUV);
                float distGrid = distance(fracUV, float2(0.5,0.5)) / 0.70710678;
    
                float distortion = distCenter * 0.7 + distGrid * 0.3;
                distortion = saturate(distortion);
    
                // Direction: 2 = Inner?Outer, 3 = Outer?Inner
                if (_Direction > 2.5) // Outer?Inner
                    distortion = 1.0 - distortion;
    
                // threshold using SoftEdge
                float threshold = _Progress * (1.0 + 2.0 * _SoftEdge) - _SoftEdge;
                float blend = smoothstep(threshold - _SoftEdge,
                                         threshold + _SoftEdge,
                                         distortion);
                blend = saturate(blend);
                blend = 1.0 - blend;
    
                float4 c1 = SampleMain(uv);
                float4 c2 = SampleSub(uv);
                return lerp(c1, c2, blend);
            }
            ENDCG
        }
    }

    FallBack "Diffuse"
}
