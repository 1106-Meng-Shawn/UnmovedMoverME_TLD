Shader "Effect/Ocean"
{
    Properties
    {
        _MainTex("Base Texture", 2D) = "white" {}
        _BaseColor("Deep Water Color", Color) = (0.0,0.1,0.3,1)
        _ShallowColor("Shallow Water Color", Color) = (0.2,0.4,0.8,1)
        _WaveSpeed("Wave Speed", Float) = 1.0
        _WaveAmplitude("Wave Amplitude", Float) = 0.05
        _WaveFrequency("Wave Frequency", Float) = 5.0
        _DistortStrength("UV Distort Strength", Float) = 0.03
        _FresnelPower("Fresnel Power", Float) = 3.0
        _FresnelIntensity("Fresnel Intensity", Range(0,1)) = 0.5
        _FoamStrength("Foam Strength", Range(0,1)) = 0.3
        _FoamThreshold("Foam Threshold", Range(0,1)) = 0.7
        [Toggle]_UseExternalTime("Use External Time", Float) = 0
        _ExternalTime("External Time", Range(0,10)) = 1
        _EdgeFixSoftness("Edge Fix Softness", Range(0.01,1)) = 0.2
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _BaseColor;
            float4 _ShallowColor;
            float _WaveSpeed;
            float _WaveAmplitude;
            float _WaveFrequency;
            float _DistortStrength;
            float _FresnelPower;
            float _FresnelIntensity;
            float _FoamStrength;
            float _FoamThreshold;
            bool _UseExternalTime;
            float _ExternalTime;
            float _EdgeFixSoftness;

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
                float3 worldPos : TEXCOORD1;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color;
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            float noise(float2 uv)
            {
                return frac(sin(dot(uv, float2(12.9898,78.233))) * 43758.5453);
            }

            float gerstnerWave(float2 uv, float2 dir, float freq, float amp, float speed, float time)
            {
                return sin(dot(uv, dir) * freq + time * speed) * amp;
            }

            float2 multiWaveOffset(float2 uv, float time)
            {
                float2 offset = float2(0,0);
                offset += float2(1,0) * gerstnerWave(uv, normalize(float2(1,0)), _WaveFrequency, _WaveAmplitude, _WaveSpeed, time);
                offset += float2(0.5,0.8) * gerstnerWave(uv, normalize(float2(0.5,0.8)), _WaveFrequency*1.2, _WaveAmplitude*0.8, _WaveSpeed*1.1, time);
                offset += float2(-0.6,0.7) * gerstnerWave(uv, normalize(float2(-0.6,0.7)), _WaveFrequency*0.8, _WaveAmplitude*0.6, _WaveSpeed*0.9, time);
                offset += (noise(uv*5.0)-0.5) * _WaveAmplitude * 0.2;
                return offset;
            }

            float computeFoam(float waveHeight)
            {
                return smoothstep(_FoamThreshold, 1.0, waveHeight) * _FoamStrength;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float time = _UseExternalTime ? _ExternalTime : _Time.y;

                // ??????
                float4 baseTex = tex2D(_MainTex, i.uv);

                // ? alpha ??????
                float edgeMask = smoothstep(_EdgeFixSoftness, 1.0, baseTex.a);

                // ?????? alpha>0 ????
                float2 uvOffset = multiWaveOffset(i.uv, time) * _DistortStrength * edgeMask;

                // UV ???????????????????????
                float2 uvSample = i.uv + uvOffset;
                float4 sampledTex = tex2D(_MainTex, uvSample);

                // ??????????????
                float waveHeight = multiWaveOffset(i.uv*2.0, time).x + multiWaveOffset(i.uv*2.0, time).y;
                waveHeight = saturate(waveHeight*0.5 + 0.5);

                // ????
                float4 waterColor = lerp(_BaseColor, _ShallowColor, waveHeight);

                // Fresnel ??
                float3 viewDir = normalize(_WorldSpaceCameraPos - i.worldPos);
                float fresnel = pow(1.0 - saturate(dot(float3(0,1,0), viewDir)), _FresnelPower);
                waterColor.rgb = lerp(waterColor.rgb, float3(1,1,1), fresnel * _FresnelIntensity);

                // ????
                float foam = computeFoam(waveHeight);
                waterColor.rgb = lerp(waterColor.rgb, float3(1,1,1), foam);

                // ??????? alpha ?????
                fixed4 finalColor = lerp(baseTex, waterColor * sampledTex, edgeMask);
                finalColor.a = baseTex.a;

                return saturate(finalColor);
            }
            ENDCG
        }
    }
}
