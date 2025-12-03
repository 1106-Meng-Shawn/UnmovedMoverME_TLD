Shader "Transition/SpiralWipe"
{
    Properties
    {
        _MainTex ("From Texture", 2D) = "white" {}
        _ToTex ("To Texture", 2D) = "white" {}
        _Progress ("Progress", Range(0, 1)) = 0
        _CenterX ("Center X", Range(0,1)) = 0.5
        _CenterY ("Center Y", Range(0,1)) = 0.5
        _Segments ("Number of Branches", Range(1, 24)) = 4
        _Clockwise ("Clockwise", Float) = 1
        _Smoothness ("Smoothness", Range(0,0.05)) = 0.02
        _TwistStrength ("Twist Strength", Range(0,5)) = 2.0
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            sampler2D _ToTex;
            float _Progress;
            float _CenterX;
            float _CenterY;
            float _Segments;
            float _Clockwise;
            float _Smoothness;
            float _TwistStrength;

            #define PI 3.14159265359
            #define TWO_PI 6.28318530718

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

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv;
                float2 center = float2(_CenterX, _CenterY);
                float2 dir = uv - center;

                float radius = length(dir);
                float angle = atan2(dir.y, dir.x) + PI/2.0;
                if (angle < 0) angle += TWO_PI;
                if (_Clockwise < 0.5) angle = TWO_PI - angle;

                // ---- ???? ----
                float segmentAngle = TWO_PI / _Segments;
                float branchIndex = floor(angle / segmentAngle);
                float localAngle = angle - branchIndex * segmentAngle;

                // ---- ?????? ----
                float spiralOffset = localAngle / TWO_PI * _TwistStrength;
                float spiralProgress = radius + spiralOffset;

                // ---- ?? blend ----
                float blend = spiralProgress - (1.0 - _Progress);
                if (_Progress <= 0.0) blend = 0.0;
                else if (_Progress >= 1.0) blend = 1.0;
                else
                {
                    if (_Smoothness > 0.0)
                        blend = smoothstep(0.0, _Smoothness*2.0, blend + _Smoothness);
                    else
                        blend = (blend >= 0.0) ? 1.0 : 0.0;
                }
                blend = saturate(blend);

                fixed4 fromColor = tex2D(_MainTex, uv);
                fixed4 toColor = tex2D(_ToTex, uv);
                return lerp(fromColor, toColor, blend);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
