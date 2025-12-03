Shader "Transition/BlinkEye"
{
    Properties
    {
        _MainTex ("Main Tex", 2D) = "white" {}
        _SubTex ("Sub Tex", 2D) = "black" {}

        _MainColor ("Main Color (Fallback)", Color) = (1, 1, 1, 1)
        _SubColor ("Second Texture Color", Color) = (1, 1, 1, 1)

        _Progress ("Progress", Range(0, 1)) = 0.0

        _TransitionColor ("Blink Transition Color", Color) = (0, 0, 0, 1)

        _SoftEdge ("Edge Softness", Range(0.0, 0.3)) = 0

        _CurveStrength ("Curve Strength", Range(0, 2)) = 1.0

        _Pivot ("Pivot", Vector) = (0.5, 0.5, 0, 0)
        [Enum(Horizontal,0, Vertical,1)]
        _Direction ("Blink Direction", Float) = 0
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
            
            sampler2D _MainTex, _SubTex;
            float4 _MainColor, _SubColor;
            float4 _TransitionColor;

            float _Progress, _SoftEdge, _CurveStrength;
            float2 _Pivot;
            float _Direction;

            struct appdata 
            { 
                float4 vertex : POSITION; 
                float2 uv : TEXCOORD0; 
            };
            
            struct v2f 
            { 
                float4 vertex : SV_POSITION; 
                float2 uv : TEXCOORD0; 
            };
            
            v2f vert(appdata v)
            { 
                v2f o; 
                o.vertex = UnityObjectToClipPos(v.vertex); 
                o.uv = v.uv; 
                return o; 
            }

            float smootherStep(float x) 
            { 
                x = saturate(x); 
                return x * x * x * (x * (x * 6.0 - 15.0) + 10.0); 
            }

            float getEyeCurve(float coord, float center)
            {
                float offset = coord - center;
                return _CurveStrength * offset * offset;
            }

            float getEyelidMask(float coord, float eyelidPos, float softEdge)
            {
                float dist = eyelidPos - coord;
                float aa = fwidth(coord) * 0.5;
                float totalSoft = softEdge + aa;
                return smootherStep(saturate(dist / totalSoft));
            }

            // Sample with tint, keep alpha; transparent areas stay transparent
            float4 SampleMain(float2 uv)
            {
                float4 tex = tex2D(_MainTex, uv);
                return float4(tex.rgb * _MainColor.rgb, tex.a);
            }

            float4 SampleSub(float2 uv)
            {
                float4 tex = tex2D(_SubTex, uv);
                return float4(tex.rgb * _SubColor.rgb, tex.a);
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float4 mainCol = SampleMain(i.uv);
                float4 subCol  = SampleSub(i.uv);

                if (_Progress <= 0.001) return mainCol;
                if (_Progress >= 0.999) return subCol;
                
                float2 uv = i.uv;

                float coord, perpCoord, center;

                // Horizontal = 0, Vertical = 1
                if (_Direction < 0.5)  // Horizontal
                {
                    coord = uv.x;
                    perpCoord = uv.y;
                    center = _Pivot.x;
                }
                else                    // Vertical
                {
                    coord = uv.y;
                    perpCoord = uv.x;
                    center = _Pivot.y;
                }

                float eyeCurve = getEyeCurve(perpCoord, center);
                float softEdge = max(_SoftEdge, 0.001);

                float topMask, bottomMask, blinkMask;
                float4 targetColor;

                if (_Progress < 0.5)
                {
                    float t1 = _Progress * 2.0;
                    float topEyelid = t1 * 0.5 + eyeCurve;
                    float bottomEyelid = 1.0 - t1 * 0.5 - eyeCurve;

                    topMask = getEyelidMask(coord, topEyelid, softEdge);
                    bottomMask = 1.0 - getEyelidMask(coord, bottomEyelid, softEdge);

                    blinkMask = max(topMask, bottomMask);

                    targetColor = mainCol;
                    return lerp(targetColor, _TransitionColor, blinkMask);
                }
                else
                {
                    float t2 = (_Progress - 0.5) * 2.0;
                    float topEyelid = 0.5 - t2 * 0.5 + eyeCurve;
                    float bottomEyelid = 0.5 + t2 * 0.5 - eyeCurve;

                    topMask = getEyelidMask(coord, topEyelid, softEdge);
                    bottomMask = 1.0 - getEyelidMask(coord, bottomEyelid, softEdge);

                    blinkMask = max(topMask, bottomMask);

                    targetColor = subCol;
                    return lerp(targetColor, _TransitionColor, blinkMask);
                }
            }
            ENDCG
        } 
    }
}
