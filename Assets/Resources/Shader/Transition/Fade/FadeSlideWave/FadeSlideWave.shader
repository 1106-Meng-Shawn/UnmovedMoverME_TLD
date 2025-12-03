Shader "Transition/FadeSlideWave"
{
    Properties
    {
        _MainTex ("Main Tex", 2D) = "white" {}
        _SubTex  ("Sub Tex", 2D) = "white" {}

        _MainColor ("Main Color (Fallback/Tint)", Color) = (1,1,1,1)
        _SubColor  ("Sub Color (Fallback/Tint)", Color) = (1,1,1,1)

        _Progress ("Progress", Range(0, 1)) = 0.0

        _TransitionColor ("Transition Color", Color) = (0,0,0,1)
        _TransitionWidth ("Transition Width", Range(0.0,1.0)) = 1
        _SoftEdge ("Soft Edge", Range(0.0,0.2)) = 0.02

        _Pivot ("Pivot", Vector) = (0.5,0.5,0,0)
        [Enum(Horizontal,0, Vertical,1)]
        _Direction("Direction", Float) = 0
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

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
            float4 _TransitionColor;

            float _Progress;
            float _TransitionWidth;
            float _SoftEdge;
            float _Direction;
            float2 _Pivot;

            struct appdata { float4 vertex : POSITION; float2 uv : TEXCOORD0; };
            struct v2f { float4 vertex : SV_POSITION; float2 uv : TEXCOORD0; };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float smoother(float x)
            {
                x = saturate(x);
                return x*x*(3.0 - 2.0*x);
            }

            // --- Sample with tint, keep transparency ---
            float4 SampleTexOrColor(sampler2D tex, float4 tint, float2 uv)
            {
                float4 t = tex2D(tex, uv);
                return float4(t.rgb * tint.rgb, t.a); // alpha preserved
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float4 mainCol = SampleTexOrColor(_MainTex, _MainColor, i.uv);
                float4 subCol  = SampleTexOrColor(_SubTex, _SubColor, i.uv);

                if (_Progress <= 0.0001) return mainCol;
                if (_Progress >= 0.9999) return subCol;

                float coord;
                if (_Direction < 0.5) // Horizontal
                    coord = (_Pivot.x < 0.5) ? i.uv.x : 1.0 - i.uv.x;
                else // Vertical
                    coord = (_Pivot.y < 0.5) ? i.uv.y : 1.0 - i.uv.y;

                float trail = _Progress * (1.0 + _TransitionWidth) - _TransitionWidth;
                float front = _Progress * (1.0 + _TransitionWidth);

                float4 col;

                if (_SoftEdge <= 0.0001)
                {
                    if (coord < trail) col = subCol;
                    else if (coord < front) col = _TransitionColor;
                    else col = mainCol;
                }
                else
                {
                    float leftBlend  = smoother(saturate((coord - (trail - _SoftEdge)) / (_SoftEdge * 2.0)));
                    float rightBlend = smoother(saturate((coord - (front - _SoftEdge)) / (_SoftEdge * 2.0)));

                    if (coord < trail - _SoftEdge)
                        col = subCol;
                    else if (coord < trail + _SoftEdge)
                        col = lerp(subCol, _TransitionColor, leftBlend);
                    else if (coord < front - _SoftEdge)
                        col = _TransitionColor;
                    else if (coord < front + _SoftEdge)
                        col = lerp(_TransitionColor, mainCol, rightBlend);
                    else
                        col = mainCol;
                }

                return col;
            }
            ENDCG
        }
    }
}
