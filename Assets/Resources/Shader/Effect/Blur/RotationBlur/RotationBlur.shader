Shader "Effect/RotationBlur"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _Center ("Rotation Center", Vector) = (0.5, 0.5, 0, 0)
        _Area ("Rotation Area", Vector) = (0.5, 0.5, 0, 0)
        _Progress ("Progress", Range(0,1)) = 0.5
        _MaxAngle ("Max Rotation Angle (deg)", Range(0,360)) = 30
        _Samples ("Samples", Range(2,64)) = 16
        _Intensity ("Intensity", Range(0,1)) = 1.0
        [ToggleUI]_Clockwise ("Clockwise", Float) = 1
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        ZTest Always
        Cull Off
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Center;
            float4 _Area;
            float _Progress;
            float _MaxAngle;
            float _Samples;
            float _Intensity;
            float _Clockwise;

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
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv;

                // ?????????
                float2 diff = uv - _Center.xy;
                if(abs(diff.x) > _Area.x * 0.5 || abs(diff.y) > _Area.y * 0.5)
                {
                    return tex2D(_MainTex, uv) * i.color;
                }

                float4 col = float4(0,0,0,0);
                int samples = max(2, (int)_Samples);

                // ?? bool ????????
                float dir = _Clockwise > 0.5 ? 1.0 : -1.0;
                float angleRad = radians(_MaxAngle) * dir * _Progress;

                [unroll(64)]
                for(int s = 0; s < 64; s++)
                {
                    if(s >= samples) break;

                    float t = (float)s / (float)(samples - 1);
                    float a = angleRad * t;
                    float cosA = cos(a);
                    float sinA = sin(a);

                    float2 offset = uv - _Center.xy;
                    float2 rotatedUV = float2(
                        offset.x * cosA - offset.y * sinA,
                        offset.x * sinA + offset.y * cosA
                    ) + _Center.xy;

                    col += tex2D(_MainTex, rotatedUV);
                }

                col /= (float)samples;
                col *= _Intensity;
                col.a = 1.0;

                return col * i.color;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
