Shader "Transition/Fade"
{
    Properties
    {
        _MainTex ("Main Tex", 2D) = "white" {}
        _SubTex  ("Sub Tex", 2D) = "black" {}

        _MainColor ("Main Color", Color) = (1,1,1,1)
        _SubColor  ("Sub Color", Color) = (1,1,1,1)

        _Offset ("Offset (x1, y1, x2, y2)", Vector) = (0,0,0,0)

        _Progress ("Progress", Range(0,1)) = 0.0
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off
        LOD 200

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv     : TEXCOORD0;
                float4 color  : COLOR;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv     : TEXCOORD0;
                float4 color  : COLOR;
            };

            sampler2D _MainTex;
            sampler2D _SubTex;

            float4 _MainColor;
            float4 _SubColor;

            float4 _Offset;
            float _Progress;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }

            // Helper: sample texture and apply tint
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

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uvMain = i.uv - _Offset.xy;
                float2 uvSub  = i.uv - _Offset.zw;

                float4 mainCol = SampleMain(clamp(uvMain, 0.0, 1.0));
                float4 subCol  = SampleSub(clamp(uvSub, 0.0, 1.0));

                float4 result = lerp(mainCol, subCol, _Progress);
                result *= i.color;

                return result;
            }
            ENDCG
        }
    }
}
