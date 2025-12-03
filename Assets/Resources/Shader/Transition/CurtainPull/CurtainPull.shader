Shader "Transition/CurtainPull"
{
    Properties
    {
        _MainTex ("Main Tex", 2D) = "white" {}
        _SubTex ("Sub Tex", 2D) = "black" {}

        _MainColor ("Main Color", Color) = (1,1,1,1)
        _SubColor ("Sub Color", Color) = (1,1,1,1)

        _Progress ("Progress", Range(0, 1)) = 0.0

        _TransitionColor ("Transition Color", Color) = (0,0,0,1)
        _SoftEdge ("Soft Edge", Range(0.0, 0.1)) = 0
        
        [Enum(Horizontal,0, Vertical,1)]
        _Direction ("Direction", Float) = 0

        [Header(Curtain Pattern)]
        _Count ("Fold Count", Range(0,50)) = 15
        _Height ("Fold Height", Range(0,0.05)) = 0.01
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off  
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            sampler2D _SubTex;

            float4 _MainTex_ST;
            float4 _SubTex_ST;

            float4 _MainColor;
            float4 _SubColor;

            float4 _TransitionColor; // <—— 改名

            float _Progress;
            float _SoftEdge;
            float _Direction;

            float _Count;
            float _Height;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv     : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv     : TEXCOORD0;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float4 SampleTexOrColor(sampler2D tex, float4 texST, float2 uv, float4 tint)
            {
                float noTex = step(texST.x + texST.y, 0.0001);
                float4 t = tex2D(tex, uv);
                float4 tintedTex = float4(t.rgb * tint.rgb, t.a * tint.a);

                return lerp(tintedTex, tint, noTex);
            }

            float smoother(float x)
            {
                x = saturate(x);
                return x * x * (3.0 - 2.0 * x);
            }

            fixed4 frag(v2f i) : SV_Target
            {
                if (_Progress <= 0.001)
                    return SampleTexOrColor(_MainTex, _MainTex_ST, i.uv, _MainColor);

                if (_Progress >= 0.999)
                    return SampleTexOrColor(_SubTex, _SubTex_ST, i.uv, _SubColor);

                float4 mainCol = SampleTexOrColor(_MainTex, _MainTex_ST, i.uv, _MainColor);
                float4 subCol  = SampleTexOrColor(_SubTex,  _SubTex_ST, i.uv, _SubColor);

                float coord, perpCoord;

                if (_Direction < 0.5)
                {
                    coord = i.uv.x;
                    perpCoord = i.uv.y;
                }
                else
                {
                    coord = i.uv.y;
                    perpCoord = i.uv.x;
                }

                float fold = 0.0;

                if (_Count > 0)
                    fold = sin(perpCoord * _Count * 6.2831853) * _Height;

                float soft = _SoftEdge;
                float adjustedCoord = coord + fold;

                // PHASE 1 — OPEN
                if (_Progress < 0.5)
                {
                    float t1 = _Progress * 2.0;

                    float leftCurtain  = t1 * 0.5;
                    float rightCurtain = 1.0 - t1 * 0.5;

                    if (adjustedCoord < leftCurtain - soft)
                        return _TransitionColor;

                    if (adjustedCoord < leftCurtain + soft)
                    {
                        float blend = smoother((adjustedCoord - (leftCurtain - soft)) / (soft * 2.0));
                        return lerp(_TransitionColor, mainCol, blend);
                    }

                    if (adjustedCoord > rightCurtain + soft)
                        return _TransitionColor;

                    if (adjustedCoord > rightCurtain - soft)
                    {
                        float blend = smoother((rightCurtain - adjustedCoord) / (soft * 2.0));
                        return lerp(mainCol, _TransitionColor, blend);
                    }

                    return mainCol;
                }
                // PHASE 2 — CLOSE
                else
                {
                    float t2 = (_Progress - 0.5) * 2.0;

                    float leftCurtain  = 0.5 - t2 * 0.5;
                    float rightCurtain = 0.5 + t2 * 0.5;

                    if (adjustedCoord < leftCurtain - soft)
                        return _TransitionColor;

                    if (adjustedCoord < leftCurtain + soft)
                    {
                        float blend = smoother((adjustedCoord - (leftCurtain - soft)) / (soft * 2.0));
                        return lerp(_TransitionColor, subCol, blend);
                    }

                    if (adjustedCoord > rightCurtain + soft)
                        return _TransitionColor;

                    if (adjustedCoord > rightCurtain - soft)
                    {
                        float blend = smoother((rightCurtain - adjustedCoord) / (soft * 2.0));
                        return lerp(subCol, _TransitionColor, blend);
                    }

                    return subCol;
                }
            }

            ENDCG
        }
    }
}
