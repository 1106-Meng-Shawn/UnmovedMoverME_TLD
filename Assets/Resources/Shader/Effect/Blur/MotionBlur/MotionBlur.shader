Shader "Effect/MotionBlur"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _BlurAmount ("Blur Amount", Range(0,1)) = 0.5
        _Size ("Size", Float) = 1.0
        _Theta ("Theta (Degrees)", Range(0,360)) = 0.0
        _Samples ("Samples", Range(1,16)) = 8
        _Offset ("Color Offset", Float) = 0.0
        [KeywordEnum(Horizontal, Vertical, CustomAngle)]
        _DirectionMode ("Direction Mode", Float) = 2
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
            float _BlurAmount;
            float _Size;
            float _Theta; // in degrees
            float _Offset;
            float _Samples;
            float _DirectionMode;

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
                float2 dir;

                // ?? DirectionMode ????
                if (_DirectionMode < 0.5)        dir = float2(1,0);             // Horizontal
                else if (_DirectionMode < 1.5)   dir = float2(0,1);             // Vertical
                else
                {
                    // CustomAngle: convert degrees to radians
                    float thetaRad = radians(_Theta);
                    dir = float2(cos(thetaRad), sin(thetaRad));
                }

                dir *= _Size * _BlurAmount / _Samples;

                float4 accum = float4(0,0,0,0);
                float alphaSum = 0;

                for (int s = 0; s < _Samples; ++s)
                {
                    float2 offsetUV = i.uv + dir * (s - _Samples/2);
                    float4 sampleCol = tex2D(_MainTex, offsetUV);
                    accum.rgb += sampleCol.rgb * sampleCol.a;
                    alphaSum += sampleCol.a;
                }

                float4 col;
                if (alphaSum > 0)
                {
                    col.rgb = accum.rgb / alphaSum;
                    col.a = alphaSum / _Samples;
                }
                else
                {
                    col = float4(0,0,0,0);
                }

                col *= i.color;
                col.rgb += _Offset * _BlurAmount;
                col.rgb = saturate(col.rgb);

                return col;
            }
            ENDCG
        }
    }
}
