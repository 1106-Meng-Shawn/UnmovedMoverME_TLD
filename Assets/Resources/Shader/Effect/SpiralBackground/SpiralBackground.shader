Shader "Effect/SpiralBackground"
{
    Properties
    {
        [Header(Colors)]
        _Color1("Color 1", Color) = (1, 1, 1, 1)
        _Color2("Color 2", Color) = (0.3, 0.3, 0.3, 1)
        
        [Header(Spiral Shape)]
        _Center("Center", Vector) = (0.5, 0.5, 0, 0)
        _Arms("Number of Arms", Range(2, 20)) = 8
        _Twist("Twist Amount", Range(0, 10)) = 2
        _EdgeSharpness("Edge Sharpness", Range(0.001, 0.5)) = 0.05
        
        [Header(Curve Control)]
        [KeywordEnum(Linear, Smooth, Exponential, Sine)] _CurveType("Curve Type", Float) = 0
        _CurvePower("Curve Power", Range(0.1, 5.0)) = 1.0
        _WaveAmplitude("Wave Amplitude", Range(0, 1)) = 0
        _WaveFrequency("Wave Frequency", Range(0, 20)) = 4
        
        [Header(Animation)]
        _Rotation("Rotation", Range(0, 360)) = 0
        [ToggleUI]_UseExternalTime("Use External Time", Float) = 0
        _ExternalTime("External Time", Float) = 0
        _RotationSpeed("Rotation Speed", Range(-5, 5)) = 1
    }
    
    SubShader
    {
        Tags 
        { 
            "RenderType"="Opaque" 
            "Queue"="Background"
        }
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _CURVETYPE_LINEAR _CURVETYPE_SMOOTH _CURVETYPE_EXPONENTIAL _CURVETYPE_SINE
            #include "UnityCG.cginc"

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

            float4 _Color1;
            float4 _Color2;
            float2 _Center;
            float _Arms;
            float _Twist;
            float _EdgeSharpness;
            float _CurvePower;
            float _WaveAmplitude;
            float _WaveFrequency;
            float _Rotation;
            float _UseExternalTime;
            float _ExternalTime;
            float _RotationSpeed;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            // ??????????????
            float ApplyCurve(float radius)
            {
                #ifdef _CURVETYPE_LINEAR
                    // ???????
                    return radius;
                    
                #elif _CURVETYPE_SMOOTH
                    // ???????smoothstep
                    return smoothstep(0.0, 1.0, radius);
                    
                #elif _CURVETYPE_EXPONENTIAL
                    // ???????????????????
                    return pow(radius, _CurvePower);
                    
                #elif _CURVETYPE_SINE
                    // ????????????
                    return radius;
                    
                #else
                    return radius;
                #endif
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // ??????????
                float2 uv = i.uv - _Center;
                
                // ?????
                float radius = length(uv);
                float angle = atan2(uv.y, uv.x);
                
                // ????
                float time;
                if (_UseExternalTime > 0.5)
                {
                    time = _ExternalTime;
                }
                else
                {
                    time = _Time.y;
                }
                
                // ??????
                float totalRotation = _Rotation + time * _RotationSpeed * 10.0;
                angle += radians(totalRotation);
                
                // ?????????
                float curvedRadius = ApplyCurve(radius);
                
                // ??????????????
                // ???angle = angle + radius * twist
                // ??????????????????????
                angle += curvedRadius * _Twist;
                
                // ???????????????
                if (_WaveAmplitude > 0.001)
                {
                    float wave = sin(angle * _WaveFrequency) * _WaveAmplitude;
                    radius += wave * 0.1;
                }
                
                // ?????? [0, 2?]
                float PI = 3.14159265359;
                float TAU = 6.28318530718;
                angle = fmod(angle + PI, TAU);
                
                // ???????
                // ???????2???? N ?????????
                float armAngle = TAU / _Arms;
                float armIndex = angle / armAngle;
                
                // ????????????
                float pattern = frac(armIndex);
                pattern = smoothstep(0.5 - _EdgeSharpness, 0.5 + _EdgeSharpness, pattern);
                
                // ????????
                float4 finalColor = lerp(_Color1, _Color2, pattern);
                
                return finalColor;
            }
            ENDCG
        }
    }
    
    FallBack "Unlit/Color"
}